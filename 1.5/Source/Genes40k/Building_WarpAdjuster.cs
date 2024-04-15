using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;


namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class Building_WarpAdjuster : Building_Enterable, IThingHolderWithDrawnPawn, IThingHolder
    {
        private int ticksRemaining;

        private int powerCutTicks;

        private int nextFuelConsumption = 99;
        private GeneDef geneToAdd = null;
        private GeneDef geneToRemove = null;

        private bool cannotUpgrade;
        private bool hasIllegalGene;
        private GeneDef forbiddenGene = null;

        DefModExtension_WarpAdjuster defModExtension = null;

        [Unsaved(false)]
        private CompPowerTrader cachedPowerComp;

        [Unsaved(false)]
        private Texture2D cachedInsertPawnTex;

        [Unsaved(false)]
        private Sustainer sustainerWorking;

        [Unsaved(false)]
        private Effecter progressBar;

        private static readonly Texture2D CancelIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel");

        private Pawn ContainedPawn => innerContainer.FirstOrDefault() as Pawn;

        public bool PowerOn => PowerTraderComp.PowerOn;

        public override bool IsContentsSuspended => false;

        private CompPowerTrader PowerTraderComp
        {
            get
            {
                if (cachedPowerComp == null)
                {
                    cachedPowerComp = this.TryGetComp<CompPowerTrader>();
                }
                return cachedPowerComp;
            }
        }

        public CompRefuelable fuelComp;

        private CompRefuelable Fuel
        {
            get
            {
                if (fuelComp == null)
                {
                    fuelComp = this.TryGetComp<CompRefuelable>();
                }
                return fuelComp;
            }
        }

        public Texture2D InsertPawnTex
        {
            get
            {
                if (cachedInsertPawnTex == null)
                {
                    cachedInsertPawnTex = ContentFinder<Texture2D>.Get("UI/Gizmos/InsertPawn");
                }
                return cachedInsertPawnTex;
            }
        }

        public float HeldPawnDrawPos_Y => DrawPos.y + 3f / 74f;

        public float HeldPawnBodyAngle => base.Rotation.Opposite.AsAngle;

        public PawnPosture HeldPawnPosture => PawnPosture.LayingOnGroundFaceUp;

        public override Vector3 PawnDrawOffset => IntVec3.West.RotatedBy(base.Rotation).ToVector3();

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            sustainerWorking = null;
            if (progressBar != null)
            {
                progressBar.Cleanup();
                progressBar = null;
            }
            base.DeSpawn(mode);
        }

        public override void Tick()
        {
            base.Tick();
            innerContainer.ThingOwnerTick();
            if (this.IsHashIntervalTick(250))
            {
                PowerTraderComp.PowerOutput = (base.Working ? (0f - base.PowerComp.Props.PowerConsumption) : (0f - base.PowerComp.Props.idlePowerDraw));
            }
            if (base.Working)
            {
                if (ContainedPawn == null)
                {
                    Cancel();
                    return;
                }
                if (PowerTraderComp.PowerOn)
                {
                    TickEffects();
                    if (PowerOn)
                    {
                        ticksRemaining--;
                    }
                    if (ticksRemaining <= 0)
                    {
                        Finish();
                    }
                    return;
                }
                powerCutTicks++;
                if (powerCutTicks >= 60000)
                {
                    Pawn containedPawn = ContainedPawn;
                    if (containedPawn != null)
                    {
                        Messages.Message("GeneExtractorNoPowerEjectedMessage".Translate(containedPawn.Named("PAWN")), containedPawn, MessageTypeDefOf.NegativeEvent, historical: false);
                    }
                    Cancel();
                }
            }
            else
            {
                if (selectedPawn != null && selectedPawn.Dead)
                {
                    Cancel();
                }
                if (progressBar != null)
                {
                    progressBar.Cleanup();
                    progressBar = null;
                }
            }
        }

        private void TickEffects()
        {
            if (sustainerWorking == null || sustainerWorking.Ended)
            {
                sustainerWorking = SoundDefOf.GeneExtractor_Working.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
            }
            else
            {
                sustainerWorking.Maintain();
            }
            if (progressBar == null)
            {
                progressBar = EffecterDefOf.ProgressBarAlwaysVisible.Spawn();
            }
            progressBar.EffectTick(new TargetInfo(base.Position + IntVec3.North.RotatedBy(base.Rotation), base.Map), TargetInfo.Invalid);
            MoteProgressBar mote = ((SubEffecter_ProgressBar)progressBar.children[0]).mote;
            if (mote != null)
            {
                mote.progress = 1f - Mathf.Clamp01((float)ticksRemaining / 30000f);
                mote.offsetZ = ((base.Rotation == Rot4.North) ? 0.5f : (-0.5f));
            }
        }

        public override AcceptanceReport CanAcceptPawn(Pawn pawn)
        {
            if (defModExtension == null)
            {
                defModExtension = def.GetModExtension<DefModExtension_WarpAdjuster>();
            }

            GetGeneChangeAndFuel(pawn);

            if (!pawn.IsColonist)
            {
                return false;
            }
            if (pawn.IsSlaveOfColony && pawn.IsPrisonerOfColony)
            {
                return false;
            }
            if (selectedPawn != null && selectedPawn != pawn)
            {
                return false;
            }
            if (!pawn.RaceProps.Humanlike || pawn.IsQuestLodger())
            {
                return false;
            }
            if (!PowerOn)
            {
                return "NoPower".Translate().CapitalizeFirst();
            }
            if (innerContainer.Count > 0)
            {
                return "Occupied".Translate();
            }
            if (pawn.genes == null)
            {
                return "PawnHasNoGenes".Translate(pawn.Named("PAWN"));
            }

            if (cannotUpgrade)
            {
                return "CannotIncreaseEffectiveness".Translate(pawn.Named("PAWN"));
            }

            if (hasIllegalGene)
            {
                return "MayNotHaveGenePotential".Translate(pawn.Named("PAWN"), defModExtension.potentialType, forbiddenGene.label);
            }

            if (Fuel.Fuel < nextFuelConsumption)
            {
                return "NotEnoughFuelPresent".Translate(nextFuelConsumption, pawn.Named("PAWN"));
            }

            return true;
        }

        private void Cancel()
        {
            startTick = -1;
            selectedPawn = null;
            sustainerWorking = null;
            powerCutTicks = 0;

            if (ContainedPawn.health.hediffSet.GetFirstHediffOfDef(Genes40kDefOf.BEWH_GenetorGrowing) != null)
            {
                ContainedPawn.health.RemoveHediff(ContainedPawn.health.hediffSet.GetFirstHediffOfDef(Genes40kDefOf.BEWH_GenetorGrowing));
            }
            innerContainer.TryDropAll(def.hasInteractionCell ? InteractionCell : base.Position, base.Map, ThingPlaceMode.Near);
        }

        private void Finish()
        {
            startTick = -1;
            selectedPawn = null;
            sustainerWorking = null;
            powerCutTicks = 0;
            if (ContainedPawn == null)
            {
                return;
            }
            Pawn containedPawn = ContainedPawn;
            if (ContainedPawn.health.hediffSet.GetFirstHediffOfDef(Genes40kDefOf.BEWH_GenetorGrowing) != null)
            {
                ContainedPawn.health.RemoveHediff(ContainedPawn.health.hediffSet.GetFirstHediffOfDef(Genes40kDefOf.BEWH_GenetorGrowing));
            }

            GetGeneChangeAndFuel(containedPawn);

            if (geneToRemove != null)
            {
                Gene toRemove = null;
                foreach (Gene gene in containedPawn.genes.Xenogenes)
                {
                    if (geneToRemove == gene.def)
                    {
                        toRemove = gene;
                    }
                }
                if (toRemove == null)
                {
                    Log.Error("Pawn did not have gene to remove");
                    return;
                }
                containedPawn.genes.RemoveGene(toRemove);
            }


            if (geneToAdd != null && !containedPawn.genes.HasGene(geneToAdd))
            {
                containedPawn.genes.AddGene(geneToAdd, true);
            }

            IntVec3 intVec = (def.hasInteractionCell ? InteractionCell : base.Position);
            innerContainer.TryDropAll(intVec, base.Map, ThingPlaceMode.Near);

            Messages.Message("EnhanceComplete".Translate(containedPawn.Named("PAWN"), defModExtension.potentialType).CapitalizeFirst(), new LookTargets(containedPawn), MessageTypeDefOf.PositiveEvent);

            Fuel.ConsumeFuel(nextFuelConsumption);
        }

        public override void TryAcceptPawn(Pawn pawn)
        {
            if ((bool)CanAcceptPawn(pawn))
            {
                selectedPawn = pawn;
                bool num = pawn.DeSpawnOrDeselect();
                if (innerContainer.TryAddOrTransfer(pawn))
                {
                    Genes40kModSettings modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();
                    startTick = Find.TickManager.TicksGame;

                    ticksRemaining = (int)modSettings.warpTime;

                    pawn.health.AddHediff(Genes40kDefOf.BEWH_GenetorGrowing);
                }
                if (num)
                {
                    Find.Selector.Select(pawn, playSound: false, forceDesignatorDeselect: false);
                }
            }
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            if (defModExtension == null)
            {
                defModExtension = def.GetModExtension<DefModExtension_WarpAdjuster>();
            }
            foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn))
            {
                yield return floatMenuOption;
            }
            if (!selPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly))
            {
                yield return new FloatMenuOption("CannotEnterBuilding".Translate(this) + ": " + "NoPath".Translate().CapitalizeFirst(), null);
                yield break;
            }
            AcceptanceReport acceptanceReport = CanAcceptPawn(selPawn);
            if (acceptanceReport.Accepted)
            {
                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("EnterBuilding".Translate(this), delegate
                {
                    SelectPawn(selPawn);
                }), selPawn, this);
            }
            else if (base.SelectedPawn == selPawn && !selPawn.IsPrisonerOfColony)
            {
                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("EnterBuilding".Translate(this), delegate
                {
                    selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.EnterBuilding, this), JobTag.Misc);
                }), selPawn, this);
            }
            else if (!acceptanceReport.Reason.NullOrEmpty())
            {
                yield return new FloatMenuOption("CannotEnterBuilding".Translate(this) + ": " + acceptanceReport.Reason.CapitalizeFirst(), null);
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (defModExtension == null)
            {
                defModExtension = def.GetModExtension<DefModExtension_WarpAdjuster>();
            }

            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            if (base.Working)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "CommandCancelProcedure".Translate();
                command_Action.defaultDesc = "CommandCancelProcedureDesc".Translate();
                command_Action.icon = CancelIcon;
                command_Action.action = delegate
                {
                    Cancel();
                };
                command_Action.activateSound = SoundDefOf.Designate_Cancel;
                yield return command_Action;
                if (DebugSettings.ShowDevGizmos)
                {
                    Command_Action command_Action2 = new Command_Action();
                    command_Action2.defaultLabel = "DevFinishProcedure".Translate();
                    command_Action2.action = delegate
                    {
                        Finish();
                    };
                    yield return command_Action2;
                }
                yield break;
            }
            if (selectedPawn != null)
            {
                Command_Action command_Action3 = new Command_Action();
                command_Action3.defaultLabel = "CommandCancelLoad".Translate();
                command_Action3.defaultDesc = "CommandCancelLoadDesc".Translate();
                command_Action3.icon = CancelIcon;
                command_Action3.activateSound = SoundDefOf.Designate_Cancel;
                command_Action3.action = delegate
                {
                    innerContainer.TryDropAll(base.Position, base.Map, ThingPlaceMode.Near);
                    if (selectedPawn.CurJobDef == JobDefOf.EnterBuilding)
                    {
                        selectedPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }
                    selectedPawn = null;
                    startTick = -1;
                    sustainerWorking = null;
                };
                yield return command_Action3;
                yield break;
            }
            Command_Action command_Action4 = new Command_Action();
            command_Action4.defaultLabel = "InsertPerson".Translate() + "...";
            command_Action4.defaultDesc = "InsertPersonWarpAdjusterDesc".Translate(defModExtension.potentialType);
            command_Action4.icon = InsertPawnTex;
            command_Action4.action = delegate
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (Pawn item in base.Map.mapPawns.AllPawnsSpawned)
                {
                    Pawn pawn = item;
                    if (pawn.genes != null)
                    {
                        AcceptanceReport acceptanceReport = CanAcceptPawn(pawn);
                        string text = pawn.LabelShortCap + ", " + pawn.genes.XenotypeLabelCap;
                        if (!acceptanceReport.Accepted)
                        {
                            if (!acceptanceReport.Reason.NullOrEmpty())
                            {
                                list.Add(new FloatMenuOption(text + ": " + acceptanceReport.Reason, null, pawn, Color.white));
                            }
                        }
                        else
                        {
                            text += "\n" + "Adds gene: " + geneToAdd.label + "\n";

                            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating);
                            if (firstHediffOfDef != null)
                            {
                                text = text + " (" + firstHediffOfDef.LabelBase + ", " + firstHediffOfDef.TryGetComp<HediffComp_Disappears>().ticksToDisappear.ToStringTicksToPeriod(allowSeconds: true, shortForm: true).Colorize(ColoredText.SubtleGrayColor) + ")";
                            }
                            list.Add(new FloatMenuOption(text, delegate
                            {
                                SelectPawn(pawn);
                            }, pawn, Color.white));
                        }
                    }
                }
                if (!list.Any())
                {
                    list.Add(new FloatMenuOption("NoValidPawns".Translate(), null));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            };
            if (!PowerOn)
            {
                command_Action4.Disable("NoPower".Translate().CapitalizeFirst());
            }
            yield return command_Action4;
        }

        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();
            if (base.Working && selectedPawn != null && innerContainer.Contains(selectedPawn))
            {
                selectedPawn.Drawer.renderer.RenderPawnAt(DrawPos, null, neverAimWeapon: true);
            }
        }

        public override string GetInspectString()
        {
            string text = base.GetInspectString();
            if (selectedPawn != null && innerContainer.Count == 0)
            {
                if (!text.NullOrEmpty())
                {
                    text += "\n";
                }
                text += "WaitingForPawn".Translate(selectedPawn.Named("PAWN")).Resolve();
            }
            else if (base.Working && ContainedPawn != null)
            {
                if (!text.NullOrEmpty())
                {
                    text += "\n";
                }

                text += "IncreasingEffectiveness".Translate(ContainedPawn.Named("PAWN"), defModExtension.potentialType).Resolve() + "\n";

                text = ((!PowerOn) ? (text + "ElevatingPausedNoPower".Translate((60000 - powerCutTicks).ToStringTicksToPeriod().Named("TIME")).Colorize(ColorLibrary.RedReadable)) : (text + "DurationLeft".Translate(ticksRemaining.ToStringTicksToPeriod()).Resolve()));
            }
            return text;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
            Scribe_Values.Look(ref powerCutTicks, "powerCutTicks", 0);
        }

        private void GetGeneChangeAndFuel(Pawn pawn)
        {
            cannotUpgrade = false;
            hasIllegalGene = false;
            geneToAdd = null;
            geneToRemove = null;
            forbiddenGene = null;
            nextFuelConsumption = 99;

            //Checks if pawn has genes to upgrade
            for (int i = 0; i < defModExtension.genesList.Count; i++)
            {
                if (pawn.genes.HasGene(defModExtension.genesList[i]))
                {
                    if (i < defModExtension.genesList.Count-1)
                    {
                        geneToAdd = defModExtension.genesList[i + 1];
                        geneToRemove = defModExtension.genesList[i];
                        nextFuelConsumption = defModExtension.costList[i+1];
                        break;
                    }
                    else
                    {
                        cannotUpgrade = true;
                    }
                }
            }
            //Checks if pawn has illegal gene
            for (int i = 0; i < defModExtension.cannotHaveGenes.Count; i++)
            {
                if (pawn.genes.HasGene(defModExtension.cannotHaveGenes[i]))
                {
                    hasIllegalGene = true;
                    forbiddenGene = defModExtension.cannotHaveGenes[i];
                    break;
                }
            }
            //Pawn has no genes to upgrade or any illegal ones
            if (geneToAdd == null)
            {
                geneToAdd = defModExtension.genesList[0];
                nextFuelConsumption = defModExtension.costList[0];
            }
        }
    }
}
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
    public class Building_GeneseedGenetor : Building_Enterable, IThingHolderWithDrawnPawn, IThingHolder
    {
        private int ticksRemaining;

        private int powerCutTicks;

        private readonly int SpaceMarineFuel = 1;
        private readonly int PrimarisMarineFuel = 5;
        private readonly int CustodesFuel = 20;
        private readonly int PrimarchFuel = 40;

        private GeneDef geneToAdd = null;

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

            if (Genes40kUtils.IsPrimaris(pawn))
            {
                if (!Genes40kDefOf.BEWH_CustodesCreation.IsFinished)
                {
                    return "ResearchNotCompleted".Translate(Genes40kDefOf.BEWH_CustodesCreation.label);
                }
                if (Fuel.Fuel < CustodesFuel)
                {
                    return "NotEnoughFuelPresent".Translate(CustodesFuel, pawn.Named("PAWN"));
                }
            }
            else if (Genes40kUtils.IsSpaceMarine(pawn))
            {
                if (!Genes40kDefOf.BEWH_PrimarisMarineCreation.IsFinished)
                {
                    return "ResearchNotCompleted".Translate(Genes40kDefOf.BEWH_PrimarisMarineCreation.label);
                }
                if (Fuel.Fuel < PrimarisMarineFuel)
                {
                    return "NotEnoughFuelPresent".Translate(PrimarisMarineFuel, pawn.Named("PAWN"));
                }
            }
            else if (IsCustodes(pawn))
            {
                if (!Genes40kDefOf.BEWH_PrimarchCreation.IsFinished)
                {
                    return "ResearchNotCompleted".Translate(Genes40kDefOf.BEWH_PrimarchCreation.label);
                }
                if (Fuel.Fuel < PrimarchFuel)
                {
                    return "NotEnoughFuelPresent".Translate(PrimarchFuel, pawn.Named("PAWN"));
                }
            }
            else if (IsPrimarch(pawn))
            {
                return "GenesCannotBeElevated".Translate(pawn.Named("PAWN"));
            }
            else
            {
                if (!Genes40kDefOf.BEWH_SpaceMarineCreation.IsFinished)
                {
                    return "ResearchNotCompleted".Translate(Genes40kDefOf.BEWH_SpaceMarineCreation.label);
                }
                if (Fuel.Fuel < SpaceMarineFuel)
                {
                    return "NotEnoughFuelPresent".Translate(SpaceMarineFuel, pawn.Named("PAWN"));
                }
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
            List<GeneDef> genesToRemove = null;
            string elevatedTo = "";
            string progressTo = "";
            XenotypeIconDef xenoIcon = null;

            NextGeneToAdd(containedPawn);

            bool fullyElevated = false;

            if (geneToAdd == Genes40kDefOf.BEWH_PrimarchStature)
            {
                genesToRemove = CustodesGenes();
                elevatedTo = "Primarch".Translate();
                xenoIcon = Genes40kDefOf.BEWH_PrimarchIcon;
                fullyElevated = true;
            }
            if (geneToAdd == Genes40kDefOf.BEWH_CustodesStature)
            {
                genesToRemove = Genes40kUtils.SpaceMarineGenes();
                genesToRemove.AddRange(Genes40kUtils.PrimarisGenes());
                elevatedTo = "Custodes".Translate();
                xenoIcon = Genes40kDefOf.BEWH_CustodesIcon;
                fullyElevated = true;
            }
            if (geneToAdd == Genes40kDefOf.BEWH_BelisarianFurnace)
            {
                elevatedTo = "PrimarisMarine".Translate();
                xenoIcon = Genes40kDefOf.BEWH_PrimarisIcon;
                fullyElevated = true;
            }
            if (geneToAdd == Genes40kDefOf.BEWH_BlackCarapace)
            {
                elevatedTo = "SpaceMarine".Translate();
                xenoIcon = Genes40kDefOf.BEWH_AstartesIcon;
                fullyElevated = true;
            }
            if (Genes40kUtils.SpaceMarineGenes().Contains(geneToAdd))
            {
                progressTo = "SpaceMarine".Translate();
            }
            if (Genes40kUtils.PrimarisGenes().Contains(geneToAdd))
            {
                progressTo = "PrimarisMarine".Translate();
            }


            if (!genesToRemove.NullOrEmpty())
            {
                List<Gene> removeThese = new List<Gene>();
                foreach (Gene gene in containedPawn.genes.Xenogenes)
                {
                    if (genesToRemove.Contains(gene.def))
                    {
                        removeThese.Add(gene);
                    }
                }
                foreach (Gene gene in removeThese)
                {
                    containedPawn.genes.RemoveGene(gene);
                }
            }

            
            if (geneToAdd != null && !containedPawn.genes.HasGene(geneToAdd))
            {
                containedPawn.genes.AddGene(geneToAdd, true);
                if (geneToAdd.HasModExtension<DefModExtension_Custodes>())
                {
                    containedPawn.genes.AddGene(Genes40kDefOf.BEWH_CustodesResilience, true);
                    containedPawn.genes.AddGene(Genes40kDefOf.BEWH_CustodesToughness, true);
                    containedPawn.genes.AddGene(Genes40kDefOf.BEWH_CustodesExpertise, true);
                    containedPawn.genes.AddGene(Genes40kDefOf.BEWH_CustodesStrength, true);
                    containedPawn.genes.AddGene(Genes40kDefOf.BEWH_CustodesAnatomy, true);
                }
                if (geneToAdd.HasModExtension<DefModExtension_Primarch>())
                {
                    containedPawn.genes.AddGene(Genes40kDefOf.BEWH_PrimarchResilience, true);
                    containedPawn.genes.AddGene(Genes40kDefOf.BEWH_PrimarchToughness, true);
                    containedPawn.genes.AddGene(Genes40kDefOf.BEWH_PrimarchExpertise, true);
                    containedPawn.genes.AddGene(Genes40kDefOf.BEWH_PrimarchStrength, true);
                    containedPawn.genes.AddGene(Genes40kDefOf.BEWH_PrimarchAnatomy, true);
                }
                if (fullyElevated)
                {
                    if (xenoIcon != null)
                    {
                        containedPawn.genes.iconDef = xenoIcon;
                    }
                    if (elevatedTo != "")
                    {
                        containedPawn.genes.xenotypeName = elevatedTo;
                    }
                }
            }

            IntVec3 intVec = (def.hasInteractionCell ? InteractionCell : base.Position);
            innerContainer.TryDropAll(intVec, base.Map, ThingPlaceMode.Near);

            if (fullyElevated)
            {
                Messages.Message("GeneElevationComplete".Translate(containedPawn.Named("PAWN"), elevatedTo).CapitalizeFirst(), new LookTargets(containedPawn), MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                Messages.Message("GeneElevationProgress".Translate(containedPawn.Named("PAWN"), progressTo).CapitalizeFirst(), new LookTargets(containedPawn), MessageTypeDefOf.PositiveEvent);
            }

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
                    if (Genes40kUtils.IsPrimaris(pawn))
                    {
                        ticksRemaining = (int)modSettings.custodesTime;
                        Fuel.ConsumeFuel(CustodesFuel);
                    }
                    else if (Genes40kUtils.IsSpaceMarine(pawn))
                    {
                        ticksRemaining = (int)modSettings.primarisMarineTime;
                        Fuel.ConsumeFuel(PrimarisMarineFuel);
                    }
                    else if (IsCustodes(pawn))
                    {
                        ticksRemaining = (int)modSettings.primarchTime;
                        Fuel.ConsumeFuel(PrimarchFuel);
                    }
                    else
                    {
                        ticksRemaining = (int)modSettings.spaceMarineTime;
                        Fuel.ConsumeFuel(SpaceMarineFuel);
                    }
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
            command_Action4.defaultDesc = "InsertPersonGeneseedGenetorDesc".Translate();
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
                            NextGeneToAdd(pawn);

                            text += "\n" + "AddsGene".Translate(geneToAdd.label) + "\n";

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

        public override void Draw()
        {
            base.Draw();
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

                string elevatingTo = "";
                if (Genes40kUtils.IsPrimaris(selectedPawn))
                {
                    elevatingTo = "Custodes";
                }
                else if (Genes40kUtils.IsSpaceMarine(selectedPawn))
                {
                    elevatingTo = "Primaris Marine";
                }
                else if (IsCustodes(selectedPawn))
                {
                    elevatingTo = "Primarch";
                }
                else
                {
                    elevatingTo = "Space Marine";
                }

                text += "CurrentlyElevatingTo".Translate(ContainedPawn.Named("PAWN"), elevatingTo).Resolve() + "\n";
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

        private List<GeneDef> CustodesGenes()
        {
            List<GeneDef> genedef = new List<GeneDef>
            {
                Genes40kDefOf.BEWH_CustodesStature,
                Genes40kDefOf.BEWH_CustodesExpertise,
                Genes40kDefOf.BEWH_CustodesStrength,
                Genes40kDefOf.BEWH_CustodesResilience,
                Genes40kDefOf.BEWH_CustodesToughness,
                Genes40kDefOf.BEWH_CustodesAnatomy
            };
            return genedef;
        }

        private bool IsCustodes(Pawn pawn)
        {
            if (pawn.genes.HasGene(Genes40kDefOf.BEWH_CustodesAnatomy))
            {
                return true;
            }
            return false;
        }

        private bool IsPrimarch(Pawn pawn)
        {
            if (pawn.genes.HasGene(Genes40kDefOf.BEWH_PrimarchAnatomy))
            {
                return true;
            }
            return false;
        }

        private void NextGeneToAdd(Pawn pawn)
        {
            if (IsCustodes(pawn))
            {
                geneToAdd = Genes40kDefOf.BEWH_PrimarchStature;
            }
            else if (Genes40kUtils.IsPrimaris(pawn))
            {
                geneToAdd = Genes40kDefOf.BEWH_CustodesStature;
            }
            else if (Genes40kUtils.IsSpaceMarine(pawn))
            {
                List<GeneDef> primarisGenes = Genes40kUtils.PrimarisGenes();
                primarisGenes.Reverse();
                foreach (GeneDef gene in primarisGenes)
                {
                    if (!pawn.genes.HasGene(gene))
                    {
                        geneToAdd = gene;
                    }
                }
            }
            else
            {
                List<GeneDef> spaceMarineGenes = Genes40kUtils.SpaceMarineGenes();
                spaceMarineGenes.Reverse();
                foreach (GeneDef gene in spaceMarineGenes)
                {
                    if (!pawn.genes.HasGene(gene))
                    {
                        geneToAdd = gene;
                    }
                }
            }
        }
    }
}
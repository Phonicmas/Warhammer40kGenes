using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k
{
    public class WorkerClass_GeneProgenoidRemoval : Recipe_Surgery
    {
        private bool isPrimaris = false;

        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if (!base.AvailableOnNow(thing, part) || !(thing is Pawn pawn))
            {
                return false;
            }
            if (!pawn.genes.HasGene(Genes40kDefOf.BEWH_ProgenoidGlands))
            {
                return false;
            }
            DefModExtension_ProgenoidHarvest defModExtension;
            if (recipe.HasModExtension<DefModExtension_ProgenoidHarvest>())
            {
                defModExtension = recipe.GetModExtension<DefModExtension_ProgenoidHarvest>();
            }
            else
            {
                return false;
            }
            isPrimaris = IsPrimaris(pawn);
            if (!defModExtension.astartesPack && !isPrimaris)
            {
                return false;
            }
                
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int index = 0; index < hediffs.Count; ++index)
            {
                if ((!recipe.targetsBodyPart || hediffs[index].Part != null) && hediffs[index].def == recipe.removesHediff && hediffs[index].Visible)
                {
                    if (hediffs[index].Severity >= 1f)
                    {
                        return true;
                    }
                        
                }
            }
            return false;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
            {
                if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
                if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(billDoer))
                {
                    string text = (recipe.successfullyRemovedHediffMessage.NullOrEmpty() ? ((string)"MessageSuccessfullyRemovedHediff".Translate(billDoer.LabelShort, pawn.LabelShort, recipe.removesHediff.label.Named("HEDIFF"), billDoer.Named("SURGEON"), pawn.Named("PATIENT"))) : ((string)recipe.successfullyRemovedHediffMessage.Formatted(billDoer.LabelShort, pawn.LabelShort)));
                    Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent);
                }
            }
            Hediff hediff = pawn.health.hediffSet.hediffs.Find((Hediff x) => x.def == recipe.removesHediff && x.Part == part && x.Visible);
            if (hediff != null)
            {
                pawn.health.RemoveHediff(hediff);
                OnSurgerySuccess(pawn, part, billDoer, ingredients, bill);
            }
        }

        protected override void OnSurgerySuccess(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            Genepack genepack = (Genepack)ThingMaker.MakeThing(ThingDefOf.Genepack);

            List<GeneDef> genesForPack = new List<GeneDef>();

            Genes40kModSettings modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();

            if (modSettings.progenoidHarvestsAllXenogenes)
            {
                for (int i = 0; i < pawn.genes.Xenogenes.Count(); i++)
                {
                    genesForPack.Add(pawn.genes.Xenogenes[i].def);
                }
            }
            else
            {
                genesForPack = AstartesPack();

                if (isPrimaris)
                {
                    genesForPack.AddRange(PrimarisPack());
                }
            }

            genepack.Initialize(genesForPack);

            List<Genepack> genepacks = new List<Genepack>
            {
                genepack
            };

            Xenogerm xenogerm = (Xenogerm)ThingMaker.MakeThing(ThingDefOf.Xenogerm);
            if (isPrimaris)
            {
                xenogerm.Initialize(genepacks, "PrimarisMarine".Translate(), Genes40kDefOf.BEWH_PrimarisIcon);
            }
            else
            {
                xenogerm.Initialize(genepacks, "SpaceMarine".Translate(), Genes40kDefOf.BEWH_AstartesIcon);
            }

            ClearQueue(pawn);
            if (GenPlace.TryPlaceThing(((Thing)xenogerm), pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Near))
            {
                return;
            } 
            Log.Error("Could not drop item near " + (object)pawn.PositionHeld);
        }

        private void ClearQueue(Pawn pawn)
        {
            BillStack bills = pawn.health.surgeryBills;
            for (int i = 1; i < bills.Count; i++)
            {
                if (bills[i].recipe.HasModExtension<DefModExtension_ProgenoidHarvest>())
                {
                    bills.Delete(bills[i]);
                    i--;
                }
            }
        }

        private List<GeneDef> AstartesPack()
        {
            List<GeneDef> genedef = new List<GeneDef>
            {
                Genes40kDefOf.BEWH_SecondaryHeart,
                Genes40kDefOf.BEWH_Ossmodula,
                Genes40kDefOf.BEWH_Biscopea,
                Genes40kDefOf.BEWH_Haemastamen,
                Genes40kDefOf.BEWH_LarramansOrgan,
                Genes40kDefOf.BEWH_CatalepseanNode,
                Genes40kDefOf.BEWH_Preomnor,
                Genes40kDefOf.BEWH_Omophagea,
                Genes40kDefOf.BEWH_MultiLung,
                Genes40kDefOf.BEWH_Occulobe,
                Genes40kDefOf.BEWH_LymansEar,
                Genes40kDefOf.BEWH_SusAnMembrane,
                Genes40kDefOf.BEWH_Melanochrome,
                Genes40kDefOf.BEWH_OoliticKidney,
                Genes40kDefOf.BEWH_Neuroglottis,
                Genes40kDefOf.BEWH_Mucranoid,
                Genes40kDefOf.BEWH_BetchersGland,
                Genes40kDefOf.BEWH_ProgenoidGlands,
                Genes40kDefOf.BEWH_BlackCarapace
            };
            return genedef;
        }

        private List<GeneDef> PrimarisPack()
        {
            List<GeneDef> genedef = new List<GeneDef>
            {
                Genes40kDefOf.BEWH_SinewCoil,
                Genes40kDefOf.BEWH_Magnificat,
                Genes40kDefOf.BEWH_BelisarianFurnace
            };
            return genedef;
        }
    
        private bool IsPrimaris(Pawn pawn)
        {
            if (pawn.genes.HasGene(Genes40kDefOf.BEWH_SinewCoil) && pawn.genes.HasGene(Genes40kDefOf.BEWH_Magnificat) && pawn.genes.HasGene(Genes40kDefOf.BEWH_BelisarianFurnace))
            {
                return true;
            }
            return false;
        }
    }

}
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
            isPrimaris = Genes40kUtils.IsPrimaris(pawn);
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
            Genes40kUtils.MakeGenePack40k(pawn, isPrimaris);
            ClearQueue(pawn);
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

    }

}
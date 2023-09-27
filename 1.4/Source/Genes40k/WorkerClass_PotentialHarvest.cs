using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace Genes40k
{
    public class WorkerClass_PotentialHarvest : Recipe_Surgery
    {

        private DefModExtension_PotentialHarvest defModExtension = null;

        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if (recipe.HasModExtension<DefModExtension_PotentialHarvest>())
            {
                defModExtension = recipe.GetModExtension<DefModExtension_PotentialHarvest>();
            }
            else
            {
                return false;
            }

            if (!base.AvailableOnNow(thing, part) || !(thing is Pawn pawn))
            {
                return false;
            }

            bool hasAny = false;
            bool hasAny2 = false;

            if (thing is Pawn pawn2)
            {
                foreach (GeneDef gene in defModExtension.mustHaveOne)
                {
                    if (pawn.genes.HasGene(gene))
                    {
                        hasAny = true;
                    }
                    if (pawn.genes.HasGene(gene))
                    {
                        hasAny2 = true;
                    }
                }
            }

            if (!hasAny && !hasAny2)
            {
                return false;
            }

            return true;
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

            OnSurgerySuccess(pawn, part, billDoer, ingredients, bill);
        }

        protected override void OnSurgerySuccess(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            Thing harvestedItem = ThingMaker.MakeThing(defModExtension.harvestedItem);

            int harvestNum = 1;

            for (int i = 0; i < defModExtension.mustHaveOne.Count; i++)
            {
                if (pawn.genes.HasGene(defModExtension.mustHaveOne[i]))
                {
                    harvestNum = i;
                }
            }

            Gene toRemove = null;
            GeneDef toRemoveDef = defModExtension.mustHaveOne[harvestNum];
            foreach (Gene gene in pawn.genes.Xenogenes)
            {
                if (toRemoveDef == gene.def)
                {
                    toRemove = gene;
                }
            }
            if (toRemove == null)
            {
                Log.Error("Pawn did not have gene to remove");
                return;
            }
            pawn.genes.RemoveGene(toRemove);

            harvestedItem.stackCount = defModExtension.amountFromTier[harvestNum];

            ClearQueue(pawn);

            Random rand = new Random();
            int j = rand.Next(100);
            if (j < defModExtension.chanceToKill)
            {
                pawn.Kill(null, null);
            }
            else
            {
                pawn.health.AddHediff(defModExtension.backlashHediff);
            }
            
            if (GenPlace.TryPlaceThing(harvestedItem, billDoer.PositionHeld, billDoer.MapHeld, ThingPlaceMode.Near))
            {
                return;
            }
            
            Log.Error("Could not drop item near " + (object)billDoer.PositionHeld);
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
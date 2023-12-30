using Core40k;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k
{
	[StaticConstructorOnStartup]
    public static class NaturalBirthPsykerPariah
    {
		static NaturalBirthPsykerPariah()
		{
			Genes40kMod.harmony.Patch(AccessTools.Method(typeof(PregnancyUtility), "ApplyBirthOutcome"), null, new HarmonyMethod(AccessTools.Method(typeof(NaturalBirthPsykerPariah), "Postfix")));
		}

		public static void Postfix(ref Thing __result, Pawn geneticMother)
        {
            Pawn pawn = (Pawn)__result;
            if (pawn.Faction != Faction.OfPlayer)
            {
                return;
            }
            List<GeneDef> genes = new List<GeneDef>();
            bool isPsyker = false;
            bool isPariah = false;
            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                genes.Add(gene.def);
                if (gene.def.HasModExtension<DefModExtension_Pariah>())
                {
                    isPariah = true;
                }
                if (gene.def.HasModExtension<DefModExtension_Psyker>())
                {
                    isPsyker = true;
                }
            }
            WeightedSelection<GeneDef> weightedSelection = new WeightedSelection<GeneDef>();
            //No extra gene
            weightedSelection.AddEntry(null, 150);
            //Psyker genes
            if (!isPsyker)
            {
                if (!genes.Contains(Genes40kDefOf.BEWH_IotaPsyker))
                {
                    weightedSelection.AddEntry(Genes40kDefOf.BEWH_IotaPsyker, 20);
                }
                if (!genes.Contains(Genes40kDefOf.BEWH_Psyker))
                {
                    weightedSelection.AddEntry(Genes40kDefOf.BEWH_Psyker, 10);
                }
                if (!genes.Contains(Genes40kDefOf.BEWH_DeltaPsyker))
                {
                    weightedSelection.AddEntry(Genes40kDefOf.BEWH_DeltaPsyker, 3);
                }
                if (!genes.Contains(Genes40kDefOf.BEWH_BetaPsyker))
                {
                    weightedSelection.AddEntry(Genes40kDefOf.BEWH_BetaPsyker, 1);
                }
            }
            //Pariah Genes
            if (!isPariah)
            {
                if (!genes.Contains(Genes40kDefOf.BEWH_SigmaPariah))
                {
                    weightedSelection.AddEntry(Genes40kDefOf.BEWH_SigmaPariah, 20);
                }
                if (!genes.Contains(Genes40kDefOf.BEWH_UpsilonPariah))
                {
                    weightedSelection.AddEntry(Genes40kDefOf.BEWH_UpsilonPariah, 10);
                }
                if (!genes.Contains(Genes40kDefOf.BEWH_OmegaPariah))
                {
                    weightedSelection.AddEntry(Genes40kDefOf.BEWH_OmegaPariah, 1);
                }
            }
            
            GeneDef chosenGene = weightedSelection.GetRandomUnique();
            if (chosenGene != null)
            {
                string typeBorn = "Psyker"; 
                if (chosenGene.HasModExtension<DefModExtension_Pariah>())
                {
                    typeBorn = "Pariah";
                }
                Letter_JumpTo letter = new Letter_JumpTo
                {
                    lookTargets = pawn,
                    def = Genes40kDefOf.BEWH_NaturalBornX,
                    Text = "NaturalBornXMessage".Translate(geneticMother.Named("PAWN"), pawn.Named("PAWN"), typeBorn),
                    title = "NaturalBornXLetter".Translate(typeBorn)
                };

                Find.LetterStack.ReceiveLetter(letter);
                pawn.genes.AddGene(chosenGene, true);
            }
        }
    }
}
using HarmonyLib;
using System;
using Verse;


namespace Genes40k
{
    [HarmonyPatch(typeof(BodyPartDef), "GetMaxHealth")]
    public class GetMaxHealthPatch
    {
        public static void Postfix(Pawn pawn, ref float __result)
        {
            if (pawn.genes != null)
            {
                float amount = 1;

                if (pawn.genes.HasGene(Genes40kDefOf.BEWH_SecondaryHeart))
                {
                    amount += 0.1f;
                }
                if (pawn.genes.HasGene(Genes40kDefOf.BEWH_Ossmodula))
                {
                    amount += 0.4f;
                }
                if (pawn.genes.HasGene(Genes40kDefOf.BEWH_Biscopea))
                {
                    amount += 0.2f;
                }
                if (pawn.genes.HasGene(Genes40kDefOf.BEWH_Melanochrome))
                {
                    amount += 0.4f;
                }
                if (pawn.genes.HasGene(Genes40kDefOf.BEWH_SinewCoil))
                {
                    amount += 0.4f;
                }
                if (pawn.genes.HasGene(Genes40kDefOf.BEWH_NurgleMark))
                {
                    amount += 0.5f;
                }
                if (pawn.genes.HasGene(Genes40kDefOf.BEWH_UndividedMark))
                {
                    amount += 0.25f;
                }
                if (pawn.genes.HasGene(Genes40kDefOf.BEWH_DaemonHide))
                {
                    amount += 2.5f;
                }
                if (pawn.genes.HasGene(Genes40kDefOf.BEWH_Custodes))
                {
                    amount += 5f;
                }
                if (pawn.genes.HasGene(Genes40kDefOf.BEWH_Primarch))
                {
                    amount += 10f;
                }

                float temp = (float)Math.Round(__result * amount);
                __result = temp;
            }
        }
    }
}
using Genes40k;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(PawnCapacityWorker_Breathing), "CalculateCapacityLevel")]
    public class MultiLungBreathingSourcePatch
    {
        public static float Postfix(float originalResult, HediffSet __0) 
        {
            HediffSet hediffSet = __0;

            Pawn pawn = hediffSet.pawn;
            if (pawn.DestroyedOrNull() || pawn.genes == null || pawn.Dead)
            {
                return originalResult;
            }
            if (pawn.genes.HasGene(Genes40kDefOf.BEWH_MultiLung))
            {
                float result = originalResult + 0.4f;
                return result;
            }
            return originalResult;
        }
    }
}
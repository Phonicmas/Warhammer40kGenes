using HarmonyLib;
using RimWorld;
using Verse;


namespace Genes40k
{
    [HarmonyPatch(typeof(PawnCapacityWorker_BloodPumping), "CalculateCapacityLevel")]
    public class SecondaryHeartBloodSourcePatch
    {
        public static float Postfix(float originalResult, HediffSet __0) 
        {
            HediffSet hediffSet = __0;

            Pawn pawn = hediffSet.pawn;
            if (pawn.DestroyedOrNull() || pawn.genes == null || pawn.Dead)
            {
                return originalResult;
            }
            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (gene.def.HasModExtension<DefModExtension_LostHeartSurvival>())
                {
                    float result = originalResult + 0.5f;
                    return result;
                }
            }
            return originalResult;
        }
    }
}
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
            float healthMultipler = 1;

            if (pawn.genes != null)
            {
                foreach (Gene gene in pawn.genes.GenesListForReading)
                {
                    if (gene.def.HasModExtension<DefModExtension_MaxHealth>())
                    {
                        healthMultipler += gene.def.GetModExtension<DefModExtension_MaxHealth>().increaseMultiplier;
                    }
                }
                __result = (float)Math.Round(__result * healthMultipler);
            }
            return;
        }
    }
}
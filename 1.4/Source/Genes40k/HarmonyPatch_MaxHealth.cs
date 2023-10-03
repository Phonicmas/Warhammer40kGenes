using HarmonyLib;
using RimWorld;
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

            if (pawn.genes != null && !pawn.genes.GenesListForReading.NullOrEmpty())
            {
                foreach (Gene gene in pawn.genes.GenesListForReading)
                {
                    if (gene.def.HasModExtension<DefModExtension_MaxHealth>())
                    {
                        healthMultipler += gene.def.GetModExtension<DefModExtension_MaxHealth>().increaseMultiplier;
                    }
                }
            }

            if (pawn.story != null && pawn.story.traits != null && !pawn.story.traits.allTraits.NullOrEmpty())
            {
                foreach (Trait trait in pawn.story.traits.allTraits)
                {
                    if (trait.def.HasModExtension<DefModExtension_MaxHealth>())
                    {
                        healthMultipler += trait.def.GetModExtension<DefModExtension_MaxHealth>().increaseMultiplier;
                    }
                }
            }

            int temp = (int)Math.Round(__result * healthMultipler);
            if (temp <= 0)
            {
                __result = 1;
            }
            else
            {
                __result = temp;
            }
        }
    }
}
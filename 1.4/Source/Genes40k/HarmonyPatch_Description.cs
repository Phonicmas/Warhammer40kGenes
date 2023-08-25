using HarmonyLib;
using RimWorld;
using System;
using System.Text;
using Verse;


namespace Genes40k
{
    [HarmonyPatch(typeof(GeneDef), "GetDescriptionFull")]
    public class GeneDescriptionPatch
    {
        public static void Postfix(ref string __result)
        {
            if (__result.Contains("FINESTCODE1234567890"))
            {
                StringBuilder sb = new StringBuilder();

                int removalIndex = __result.IndexOf("Suppressed");
                if (removalIndex == -1)
                {
                    return;
                }
                int s1 = __result.IndexOf("CUSTOMSTART12345");
                int s2 = __result.IndexOf("CUSTOMEND67890");
                string customTemp = __result.Substring(s1, s2-s1);
                customTemp = customTemp.Replace("CUSTOMSTART12345", "");
                string wip = __result.Remove(removalIndex - 17); //17 is the amount of prefix color that also needs to be removed
                wip = wip.Replace("FINESTCODE1234567890", "");

                sb.Append(wip);
                sb.AppendLineTagged(("Effects".Translate().CapitalizeFirst() + ":").Colorize(ColoredText.TipSectionTitleColor));
                sb.AppendLine("  - " + customTemp);

                string result = sb.ToString().TrimEndNewlines();

                __result = result;
                return;
            }
            return;
        }
    }
}
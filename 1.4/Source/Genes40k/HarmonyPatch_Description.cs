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
        public static void Postfix(ref string __result, ref GeneDef __instance)
        {
            if (!__instance.HasModExtension<DefModExtension_Desciption>())
            {
                return;
            }
            DefModExtension_Desciption defModExtension = __instance.GetModExtension<DefModExtension_Desciption>();
            StringBuilder sb = new StringBuilder();

            sb.Append(__instance.description).AppendLine().AppendLine();

            if (__instance.minAgeActive > 0f)
            {
                sb.AppendLine((string)("TakesEffectAfterAge".Translate() + ": ") + __instance.minAgeActive);
            }
            sb.AppendLine();
            if (__instance.biostatCpx != 0)
            {
                sb.AppendLineTagged("Complexity".Translate().Colorize(GeneUtility.GCXColor) + ": " + __instance.biostatCpx.ToStringWithSign());
            }
            if (__instance.biostatMet != 0)
            {
                sb.AppendLineTagged("Metabolism".Translate().Colorize(GeneUtility.METColor) + ": " + __instance.biostatMet.ToStringWithSign());
            }
            if (__instance.biostatArc != 0)
            {
                sb.AppendLineTagged("ArchitesRequired".Translate().Colorize(GeneUtility.ARCColor) + ": " + __instance.biostatArc.ToStringWithSign());
            }
            sb.AppendLine();
            sb.AppendLine(("Effects".Translate().CapitalizeFirst() + ":").Colorize(ColoredText.TipSectionTitleColor));

            sb.AppendLine("  - " + defModExtension.newCustomEffectText.Translate());

            __result = sb.ToString().TrimEndNewlines();

            return;

        }
    }
}
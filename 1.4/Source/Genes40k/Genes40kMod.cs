using HarmonyLib;
using UnityEngine;
using Verse;


namespace Genes40k
{
    public class Genes40kMod : Mod
    {
        public static Harmony harmony;

        readonly Genes40kModSettings settings;

        public Genes40kMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<Genes40kModSettings>();
            harmony = new Harmony("Genes40k.Mod");
            harmony.PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("GenetorCycleTimeSM".Translate((settings.spaceMarineTime / 60000).ToString("0.00")));
            settings.spaceMarineTime = listingStandard.Slider(settings.spaceMarineTime, 1f, 3600000f);

            listingStandard.Label("GenetorCycleTimePM".Translate((settings.primarisMarineTime / 60000).ToString("0.00")));
            settings.primarisMarineTime = listingStandard.Slider(settings.primarisMarineTime, 1f, 3600000f);

            listingStandard.Label("GenetorCycleTimeCS".Translate((settings.custodesTime / 60000).ToString("0.00")));
            settings.custodesTime = listingStandard.Slider(settings.custodesTime, 1f,3600000f);

            listingStandard.Label("GenetorCycleTimePMH".Translate((settings.primarchTime / 60000).ToString("0.00")));
            settings.primarchTime = listingStandard.Slider(settings.primarchTime, 1f, 3600000f);

            listingStandard.Label("WarpAdjusterCycleTime".Translate((settings.warpTime / 60000).ToString("0.00")));
            settings.warpTime = listingStandard.Slider(settings.warpTime, 1f, 3600000f);

            listingStandard.Label("KillChanceOnHarvest".Translate(settings.killChance));
            settings.killChance = (int)listingStandard.Slider(settings.killChance, 0, 100);

            listingStandard.CheckboxLabeled("progenoidHarvestsAllXenogenes".Translate(), ref settings.progenoidHarvestsAllXenogenes);

            listingStandard.Label("CheckVEFPatches".Translate());

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "ModSettingsNameGenes".Translate();
        }
    }
}
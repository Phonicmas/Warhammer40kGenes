using Verse;


namespace Genes40k
{
    public class Genes40kModSettings : ModSettings
    {
        public float spaceMarineTime = 60000;
        public float primarisMarineTime = 90000;
        public float custodesTime = 240000;
        public float primarchTime = 480000;

        public float warpTime = 100000;

        public int killChance = 35;

        public bool progenoidHarvestsAllXenogenes = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref spaceMarineTime, "SpaceMarineTime", 60000);
            Scribe_Values.Look(ref primarisMarineTime, "PrimarisMarineTime", 90000);
            Scribe_Values.Look(ref custodesTime, "CustodesTime", 240000);
            Scribe_Values.Look(ref primarchTime, "PrimarchTime", 480000);
            Scribe_Values.Look(ref warpTime, "WarpTime", 100000);
            Scribe_Values.Look(ref killChance, "PotentialHarvestKillChance", 35);
            Scribe_Values.Look(ref progenoidHarvestsAllXenogenes, "progenoidHarvestsAllXenogenes", false);
            base.ExposeData();
        }
    }
}
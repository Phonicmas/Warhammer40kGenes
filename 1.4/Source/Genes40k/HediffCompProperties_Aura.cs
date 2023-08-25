using Verse;


namespace Genes40k
{
    public class HediffCompProperties_Aura : HediffCompProperties
    {

        public int radius = 1;

        public int tickInterval = 100;

        public HediffDef hediff;

        public float severity = 1f;

        public HediffCompProperties_Aura()
        {
            compClass = typeof(HediffComp_Aura);
        }

    }
}
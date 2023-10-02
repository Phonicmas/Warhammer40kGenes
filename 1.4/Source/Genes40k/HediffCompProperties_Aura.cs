using RimWorld;
using Verse;


namespace Genes40k
{
    public class HediffCompProperties_Aura : HediffCompProperties
    {

        public int radius = 1;

        public HediffDef hediff;

        public HediffCompProperties_Aura()
        {
            compClass = typeof(HediffComp_Aura);
        }

    }
}
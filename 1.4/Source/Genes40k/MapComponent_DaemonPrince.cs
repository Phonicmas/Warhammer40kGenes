using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace Genes40k
{
    public class MapComponent_DaemonPrince : MapComponent
    {
        public const int checkingInterval = 10000;

        public const int deathTimer = 12000;

        public int tickCounter = 0;

        public MapComponent_DaemonPrince(Map map)
            : base(map)
        {
        }

        public override void MapComponentTick()
        {
            base.MapComponentUpdate();
            if (tickCounter >= checkingInterval)
            {
                tickCounter = 0;

                foreach (Thing thing in map.spawnedThings)
                {
                    if (thing != null && thing is Corpse corpse)
                    {
                        if (corpse.InnerPawn != null && corpse.InnerPawn.genes != null && corpse.InnerPawn.genes.HasGene(Genes40kDefOf.BEWH_DaemonMutation) && Find.TickManager.TicksGame - corpse.timeOfDeath >= deathTimer)
                        {
                            
                            ResurrectionUtility.Resurrect(corpse.InnerPawn);
                        }
                    }
                }
            }
            tickCounter++;
        }

    }
}
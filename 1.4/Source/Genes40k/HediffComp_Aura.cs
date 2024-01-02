using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;


namespace Genes40k
{
    public class HediffComp_Aura : HediffComp
    {
        private int tickInterval = 400;

        public HediffCompProperties_Aura Props => (HediffCompProperties_Aura)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            Pawn pawn = parent.pawn;
            if (pawn.IsHashIntervalTick(tickInterval) || pawn.needs == null || pawn.needs.mood == null || pawn.Faction == null)
            {
                return;
            }
            if (pawn.Spawned)
            {
                List<Pawn> pawns = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
                AffectPawns(pawn, pawns);
                return;
            }
            Caravan caravan = pawn.GetCaravan();
            if (caravan != null)
            {
                AffectPawns(pawn, caravan.pawns.InnerListForReading);
            }
        }

        private void AffectPawns(Pawn p, List<Pawn> pawns)
        {
            for (int i = 0; i < pawns.Count; i++)
            {
                Pawn pawn = pawns[i];
                if (p == pawn || !p.RaceProps.Humanlike || pawn == null || pawn.needs == null || pawn.needs.mood == null || pawn.needs.mood.thoughts == null || pawn.Position.DistanceTo(p.Position) > Props.radius)
                {
                    continue;
                }
                bool flag = false;
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    if (hediff.def.HasModExtension<DefModExtension_Pariah>())
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    pawn.health.AddHediff(Props.hediff);
                }
            }
        }
    }
}
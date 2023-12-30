using RimWorld;
using System.Collections.Generic;
using Verse;


namespace Genes40k
{
    public class Gene_DaemonPrince : Gene
    {

        private static readonly FloatRange TendingQualityRange = new FloatRange(0.2f, 0.7f);

        public override void Tick()
        {
            base.Tick();
            if (!pawn.IsHashIntervalTick(360))
            {
                return;
            }
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int num = hediffs.Count - 1; num >= 0; num--)
            {
                if (hediffs[num].Bleeding)
                {
                    hediffs[num].Tended(TendingQualityRange.RandomInRange, TendingQualityRange.TrueMax, 1);
                }
            }
        }

        public override void Notify_PawnDied()
        {
            GameComponent_DaemonPrince gameComp = Current.Game.GetComponent<GameComponent_DaemonPrince>();
            pawn.Corpse.Strip();
            gameComp.pawns.Add(pawn, pawn.Map);
            pawn.Corpse.DeSpawn();
            base.Notify_PawnDied();
        }
    }
}
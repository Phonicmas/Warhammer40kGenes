using Verse;


namespace Genes40k
{
    public class HediffComp_Aura : HediffComp
    {
        public int tickCounter = 0;

        public HediffCompProperties_Aura Props => (HediffCompProperties_Aura)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            tickCounter++;
            if (tickCounter < Props.tickInterval)
            {
                return;
            }
            Pawn pawn = parent.pawn;
            if (pawn != null && pawn.Map != null && !pawn.Dead)
            {
                foreach (Thing item in GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, Props.radius, useCenter: true))
                {
                    if (item is Pawn otherPawn && !pawn.Dead && otherPawn != pawn)
                    {
                        otherPawn.health.AddHediff(Props.hediff);
                    }
                }

            }
            tickCounter = 0;
        }
    }
}
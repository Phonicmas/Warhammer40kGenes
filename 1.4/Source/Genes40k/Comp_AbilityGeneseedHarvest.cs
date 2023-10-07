using RimWorld;
using Verse;


namespace Genes40k
{
    public class Comp_AbilityGeneseedHarvest : CompAbilityEffect
    {
        public new CompProperties_AbilityGeneseedHarvest Props => (CompProperties_AbilityGeneseedHarvest)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Corpse corpse = target.Thing as Corpse;
            Pawn pawn = corpse.InnerPawn;

            Genes40kUtils.MakeGenePack40k(pawn, Genes40kUtils.IsPrimaris(pawn));

            pawn.health.AddHediff(Genes40kDefOf.BEWH_SecondGeneseedHarvest, null);

            base.Apply(target, dest);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            base.Valid(target, throwMessages);
            if (!(target.Thing is Corpse))
            {
                return false;
            }
            Corpse corpse = target.Thing as Corpse;

            if (corpse.InnerPawn.genes != null && corpse.InnerPawn.genes.HasGene(Genes40kDefOf.BEWH_ProgenoidGlands) && !corpse.InnerPawn.health.hediffSet.HasHediff(Genes40kDefOf.BEWH_SecondGeneseedHarvest))
            {
                return true;
            }
            return false;
        }

    }
}
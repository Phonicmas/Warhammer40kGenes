using RimWorld;
using Verse;


namespace Genes40k
{
    public class CompAbilityEffect_WarpShieldOff : CompAbilityEffect
    {
        public new CompProperties_AbilityWarpShieldOff Props => (CompProperties_AbilityWarpShieldOff)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Hediff hediff = parent.pawn.health.hediffSet.hediffs.Find(x => x.def.HasModExtension<DefModExtension_Custodes>());
            if (hediff == null)
            {
                return;
            }
            hediff.Severity = 1;
            parent.pawn.abilities.RemoveAbility(Genes40kDefOf.BEWH_PsySensOff);
            parent.pawn.abilities.GainAbility(Genes40kDefOf.BEWH_PsySensOn);
        }
    }
}
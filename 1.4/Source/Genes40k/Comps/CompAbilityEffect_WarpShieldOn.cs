using RimWorld;
using Verse;


namespace Genes40k
{
    public class CompAbilityEffect_WarpShieldOn : CompAbilityEffect
    {
        public new CompProperties_AbilityWarpShieldOn Props => (CompProperties_AbilityWarpShieldOn)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Hediff hediff = parent.pawn.health.hediffSet.hediffs.Find(x => x.def.HasModExtension<DefModExtension_Custodes>());
            if (hediff == null)
            {
                return;
            }
            hediff.Severity = 2;
            parent.pawn.abilities.RemoveAbility(Genes40kDefOf.BEWH_PsySensOn);
            parent.pawn.abilities.GainAbility(Genes40kDefOf.BEWH_PsySensOff);
        }
    }
}
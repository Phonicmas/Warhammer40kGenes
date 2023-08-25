using RimWorld;
using Verse;


namespace Genes40k
{
    public class ThoughtWorker_UpsilonPariah : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
        {
            if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
            {
                return false;
            }
            if (pawn.story.traits.HasTrait(TraitDefOf.Kind))
            {
                return false;
            }
            if (!other.genes.HasGene(Genes40kDefOf.BEWH_UpsilonPariah))
            {
                return false;
            }
            return true;
        }
    }
}
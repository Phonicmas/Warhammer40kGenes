using RimWorld;
using Verse;


namespace Genes40k
{
    public class ThoughtWorker_Pariah : ThoughtWorker
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
            if (def.HasModExtension<DefModExtension_Pariah>())
            {
                if (!other.genes.HasGene(def.GetModExtension<DefModExtension_Pariah>().pariahGene))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
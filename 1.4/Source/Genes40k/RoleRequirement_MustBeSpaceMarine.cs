using RimWorld;
using System.Collections.Generic;
using Verse;


namespace Genes40k
{
    public class RoleRequirement_MustBeSpaceMarine : RoleRequirement
    {
        public List<GeneDef> genes;

        [NoTranslate]
        private string labelCached;

        public override string GetLabel(Precept_Role role)
        {
            if (labelCached == null)
            {
                labelCached = "MustBeSpaceMarine".Translate();
            }
            return labelCached;
        }

        public override bool Met(Pawn p, Precept_Role role)
        {
            if (p.genes == null)
            {
                return false;
            }
            foreach (GeneDef gene in genes)
            {
                if (!p.genes.HasGene(gene))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
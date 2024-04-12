using System.Collections.Generic;
using Verse;

namespace Genes40k
{
    public class DefModExtension_WarpAdjuster : DefModExtension
    {
        //Size of both lists should be the same.
        public string potentialType;
        public List<int> costList;
        public List<GeneDef> genesList;
        public List<GeneDef> cannotHaveGenes;
    }

}
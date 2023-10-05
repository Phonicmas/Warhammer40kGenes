using Core40k;
using System.Collections.Generic;
using Verse;

namespace Genes40k
{
    public class DefModExtension_DaemonMutation : DefModExtension
    {
        public DaemonParts daemonPart = DaemonParts.None;

        public List<ChaosGods> godGiver = new List<ChaosGods>();
    }

}
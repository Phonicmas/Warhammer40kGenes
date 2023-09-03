using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class Verb_CastAbility_DaemonSummon : Verb_CastAbility
    {

        protected override bool TryCastShot()
        {
            if (!ability.def.HasModExtension<DefModExtension_DaemonSummon>())
            {
                return false;
            }

            Pawn pawn = Caster as Pawn;
            int scaleSkill = pawn.skills.GetSkill(ability.def.GetModExtension<DefModExtension_DaemonSummon>().statScale).Level;
            
            int summonAmount = Mathf.FloorToInt(scaleSkill / 2);
            PawnKindDef toSummon = ability.def.GetModExtension<DefModExtension_DaemonSummon>().daemonToSummon;

            if (base.TryCastShot())
            {
                for (int i = 0; i < summonAmount; i++)
                {
                    GenSpawn.Spawn(PawnGenerator.GeneratePawn(toSummon, pawn.Faction), currentTarget.Cell, pawn.Map);
                }
                return true;
            }

            return false;
        }
    }
}
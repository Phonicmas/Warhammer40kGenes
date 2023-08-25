using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class Verb_CastAbility_SlaanneshSummon : Verb_CastAbility
    {

        protected override bool TryCastShot()
        {
            Pawn pawn = Caster as Pawn;
            float psySens = pawn.GetStatValue(StatDefOf.PsychicSensitivity);


            int summonAmount = Mathf.FloorToInt(psySens);

            if (base.TryCastShot())
            {
                for (int i = 0; i < summonAmount; i++)
                {
                    GenSpawn.Spawn(PawnGenerator.GeneratePawn(Genes40kDefOf.BEWH_SummonedDaemonette, pawn.Faction), currentTarget.Cell, pawn.Map);
                }
                return true;
            }

            return false;
        }
    }
}
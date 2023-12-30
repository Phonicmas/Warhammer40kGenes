using Core40k;
using Genes40k;
using HarmonyLib;
using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace Mutations40k
{
    [HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
    public class MentalBreakdownPsyker
    {
        public static void Postfix(MentalStateDef stateDef, MentalStateHandler __instance, bool __result)
        {
            //Mental state didn't start, skip
            if (!__result)
            {
                return;
            }

            Pawn pawn = __instance.CurState.pawn;

            if (pawn.genes == null)
            {
                return;
            }

            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (gene.def.HasModExtension<DefModExtension_Psyker>())
                {
                    Random rand = new Random();
                    int roll = rand.Next(1, 100);
                    switch (roll)
                    {
                        case int n when (n >= 1 && n <=10):
                            //Makew new Harmless stuff
                            pawn.Map.weatherManager.TransitionTo(Genes40kDefOf.BEWH_BloodRain);
                            break;
                        case int n when (n >= 11 && n <= 24):
                            //Make new Basically Harmless
                            pawn.Map.weatherManager.TransitionTo(Genes40kDefOf.BEWH_BloodRain);
                            break;
                        case int n when (n >= 25 && n <= 39):
                            pawn.Map.weatherManager.TransitionTo(Genes40kDefOf.BEWH_BloodRain);
                            break;
                        case int n when (n >= 40 && n <= 54):
                            ReappearLaterRandomly(pawn);
                            break;
                        case int n when (n >= 55 && n <= 69):
                            //Make newSome annoyance, not harmless
                            pawn.Map.weatherManager.TransitionTo(Genes40kDefOf.BEWH_BloodRain);
                            break;
                        case int n when (n >= 70 && n <= 84):
                            pawn.health.AddHediff(Genes40kDefOf.BEWH_PsychicComa);
                            break;
                        case int n when (n >= 85 && n <= 94):
                            GenExplosion.DoExplosion(pawn.Position, pawn.Map, pawn.GetStatValue(StatDefOf.PsychicSensitivity) * 2, Genes40kDefOf.BEWH_WarpEnergy, pawn);
                            break;
                        case int n when (n >= 95 && n <= 99):
                            SummonDaemons(pawn);
                            break;
                        case 100:
                            pawn.Kill(null);
                            GenExplosion.DoExplosion(pawn.Corpse.Position, pawn.Corpse.Map, pawn.GetStatValue(StatDefOf.PsychicSensitivity) * 5, Genes40kDefOf.BEWH_WarpEnergy, pawn, damAmount: 40, armorPenetration: 1.5f);
                            break;
                        default:
                            break;
                    }
                    
                    return;
                }
            }

        }
    
        private static void SummonDaemons(Pawn pawn)
        {
            Random rand = new Random();
            int randNum = rand.Next(1, 4);
            PawnKindDef chosenSummon = null;
            switch (randNum)
            {
                case 1:
                    chosenSummon = Core40kDefOf.BEWH_SummonedPinkHorror;
                    break;
                case 2:
                    chosenSummon = Core40kDefOf.BEWH_SummonedPlaguebearer;
                    break;
                case 3:
                    chosenSummon = Core40kDefOf.BEWH_SummonedBloodletter;
                    break;
                case 4:
                    chosenSummon = Core40kDefOf.BEWH_SummonedDaemonette;
                    break;
                default:
                    break;
            }

            randNum = rand.Next(1, (int)pawn.GetStatValue(StatDefOf.PsychicSensitivity));

            //Make sort of portal first, then spawn.
            for (int i = 0; i < randNum; i++)
            {
                GenSpawn.Spawn(PawnGenerator.GeneratePawn(chosenSummon, null), pawn.Position.RandomAdjacentCell8Way(), pawn.Map);
            }
        }
    
        private static void ReappearLaterRandomly(Pawn pawn)
        {
            //attach pawn to game comp?
        }
    }
}
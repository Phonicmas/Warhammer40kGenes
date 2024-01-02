using Core40k;
using Genes40k;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    if (!gene.Active)
                    {
                        continue;
                    }
                    Letter_JumpTo letter = new Letter_JumpTo
                    {
                        lookTargets = pawn,
                        def = Genes40kDefOf.BEWH_NaturalBornX,
                    };
                    bool sendLetter = true;
                    Random rand = new Random();
                    int roll = rand.Next(1, 100);
                    roll += (int)pawn.GetStatValue(StatDefOf.PsychicSensitivity);
                    switch (roll)
                    {
                        case 100:
                            pawn.Kill(null);
                            GenExplosion.DoExplosion(pawn.Corpse.Position, pawn.Corpse.Map, pawn.GetStatValue(StatDefOf.PsychicSensitivity) * 5, Genes40kDefOf.BEWH_WarpEnergy, pawn, damAmount: (int)(pawn.GetStatValue(StatDefOf.PsychicSensitivity) * 100), armorPenetration: 10f);
                            letter.Text = "Annihilation".Translate(pawn.Named("PAWN"));
                            letter.Label = "PerilsOfTheWarp".Translate();
                            break;
                        case int n when n >= 99:
                            SummonDaemons(pawn);
                            letter.Text = "DaemonHost".Translate(pawn.Named("PAWN"));
                            letter.Label = "PerilsOfTheWarp".Translate();
                            break;
                        case int n when n >= 95:
                            GenExplosion.DoExplosion(pawn.Position, pawn.Map, pawn.GetStatValue(StatDefOf.PsychicSensitivity) * 2, Genes40kDefOf.BEWH_WarpEnergy, pawn);
                            letter.Text = "DaemonHost".Translate(pawn.Named("PAWN"));
                            letter.Label = "UncontrollablePowers".Translate();
                            break;
                        case int n when n >= 90:
                            pawn.health.AddHediff(Genes40kDefOf.BEWH_PsychicComa);
                            letter.Text = "DaemonHost".Translate(pawn.Named("PAWN"));
                            letter.Label = "PsychicComa".Translate();
                            break;
                        /*case int n when n >= 80:
                            //??
                            break;*/
                        case int n when n >= 70:
                            pawn.health.AddHediff(Genes40kDefOf.BEWH_PsychicConnectionSevered);
                            letter.Text = "DaemonHost".Translate(pawn.Named("PAWN"));
                            letter.Label = "PsychicConnectionSevered".Translate();
                            break;
                        case int n when n >= 60:
                            pawn.Map.weatherManager.TransitionTo(Genes40kDefOf.BEWH_BloodRain);
                            letter.Text = "DaemonHost".Translate(pawn.Named("PAWN"));
                            letter.Label = "BloodRain".Translate();
                            break;
                        case int n when n >= 30:
                            IEnumerable<IntVec3> t = GenRadial.RadialCellsAround(pawn.Position, 8, true);
                            foreach (IntVec3 c in t)
                            {
                                Plant plant = c.GetPlant(pawn.Map);
                                if (plant != null)
                                {
                                    plant.Kill();
                                }
                            }
                            letter.Text = "DaemonHost".Translate(pawn.Named("PAWN"));
                            letter.Label = "PlantRot".Translate();
                            break;
                        default:
                            sendLetter = false;
                            break;
                    }
                    if (sendLetter)
                    {
                        Find.LetterStack.ReceiveLetter(letter);
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
    
    }
}
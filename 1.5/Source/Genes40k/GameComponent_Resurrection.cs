using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace Genes40k
{
    public class GameComponent_DaemonPrince : GameComponent
    {
        private const int checkingInterval = 60000;

        private const int deathTimer = 120000;

        public Dictionary<Pawn, Map> pawns = new Dictionary<Pawn, Map>();

        public List<Pawn> resurrectedPawns = new List<Pawn>();

        public GameComponent_DaemonPrince(Game game)
        {
        }

        public override void GameComponentTick()
        {
            if (!pawns.NullOrEmpty())
            {
                foreach (KeyValuePair<Pawn, Map> pair in pawns)
                {
                    if (!pair.Key.IsHashIntervalTick(checkingInterval))
                    {
                        continue;
                    }
                    if (pair.Value == null)
                    {
                        Map map = Find.AnyPlayerHomeMap;
                        if (map != null)
                        {
                            pawns.SetOrAdd(pair.Key, map);
                        }
                        else
                        {
                            List<Map> maps = Find.Maps;
                            if (maps != null)
                            {
                                pawns.SetOrAdd(pair.Key, maps.RandomElement());
                            }
                            else 
                            {
                                Map lastResort = MapGenerator.GenerateMap(new IntVec3(50, 50, 50), Find.WorldObjects.MapParentAt(TileFinder.RandomStartingTile()), MapGeneratorDefOf.Base_Player);
                                pawns.SetOrAdd(pair.Key, lastResort);
                            }
                        }
                    }
                    if (pair.Key.Corpse != null && (Find.TickManager.TicksGame - pair.Key.Corpse.timeOfDeath) >= deathTimer && !pair.Key.Spawned)
                    {
                        GenSpawn.Spawn(pair.Key.Corpse, CellFinder.RandomCell(pair.Value), pair.Value);
                        Mesh boltMesh = LightningBoltMeshPool.RandomBoltMesh;
                        Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt");

                        DaemonDoStrike(pair.Key.Corpse.Position, pair.Key.Corpse.Map, ref boltMesh);
                        Graphics.DrawMesh(boltMesh, pair.Key.Corpse.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(LightningMat, 1), 0);
                        ResurrectionParams resurrectionParams = new ResurrectionParams
                        {
                            removeDiedThoughts = false
                        };
                        ResurrectionUtility.TryResurrect(pair.Key.Corpse.InnerPawn, resurrectionParams);
                        resurrectedPawns.Add(pair.Key);
                    }
                }
                if (!resurrectedPawns.NullOrEmpty())
                {
                    foreach (Pawn pawn in resurrectedPawns)
                    {
                        if (!pawns.Remove(pawn))
                        {
                            Log.Error("Could not remove pawn on daemon resurrection - Report this to Phonicmas if you see it");
                        }
                    }
                }
                resurrectedPawns = new List<Pawn>();
            }   
            base.GameComponentTick();
        }

        private static void DaemonDoStrike(IntVec3 strikeLoc, Map map, ref Mesh boltMesh)
        {
            SoundDefOf.Thunder_OffMap.PlayOneShotOnCamera(map);
            if (!strikeLoc.IsValid)
            {
                strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => sq.Standable(map) && !map.roofGrid.Roofed(sq), map);
            }
            boltMesh = LightningBoltMeshPool.RandomBoltMesh;
            if (!strikeLoc.Fogged(map))
            {
                Vector3 loc = strikeLoc.ToVector3Shifted();
                for (int i = 0; i < 4; i++)
                {
                    FleckMaker.ThrowSmoke(loc, map, 1.5f);
                    FleckMaker.ThrowMicroSparks(loc, map);
                    FleckMaker.ThrowLightningGlow(loc, map, 1.5f);
                }
            }
            SoundInfo info = SoundInfo.InMap(new TargetInfo(strikeLoc, map));
            SoundDefOf.Thunder_OnMap.PlayOneShot(info);
        }

    }
}
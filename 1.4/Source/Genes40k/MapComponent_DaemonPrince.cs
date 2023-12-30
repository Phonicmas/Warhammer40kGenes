using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Genes40k
{
    public class MapComponent_DaemonPrince : MapComponent
    {
        private const int checkingInterval = 10000;

        private const int deathTimer = 12000;

        public List<Pawn> pawns = new List<Pawn>();

        public MapComponent_DaemonPrince(Map map)
            : base(map)
        {
        }

        public override void MapComponentTick()
        {
            if (!pawns.NullOrEmpty())
            {
                if (map.IsHashIntervalTick(checkingInterval))
                {
                    for (int i = pawns.Count; i >= 0; i--)
                    {
                        if (pawns[i - 1].Corpse != null && (Find.TickManager.TicksGame - pawns[i - 1].Corpse.timeOfDeath) >= deathTimer)
                        {
                            GenSpawn.Spawn(pawns[i - 1].Corpse, CellFinder.RandomCell(map), map);
                            Mesh boltMesh = LightningBoltMeshPool.RandomBoltMesh;
                            Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt");

                            DaemonDoStrike(pawns[i - 1].Corpse.Position, pawns[i - 1].Corpse.Map, ref boltMesh);
                            Graphics.DrawMesh(boltMesh, pawns[i - 1].Corpse.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(LightningMat, 1), 0);
                            ResurrectionUtility.Resurrect(pawns[i - 1].Corpse.InnerPawn);
                            pawns.RemoveAt(i - 1);
                        }
                    }
                }
            }   
            base.MapComponentTick();
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
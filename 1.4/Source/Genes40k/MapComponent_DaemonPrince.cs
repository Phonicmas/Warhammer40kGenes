using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Genes40k
{
    public class MapComponent_DaemonPrince : MapComponent
    {
        public const int checkingInterval = 10000;

        public const int deathTimer = 12000;

        public int tickCounter = 0;

        public MapComponent_DaemonPrince(Map map)
            : base(map)
        {
        }

        public override void MapComponentTick()
        {
            base.MapComponentUpdate();
            if (tickCounter >= checkingInterval)
            {
                tickCounter = 0;

                foreach (Thing thing in map.spawnedThings)
                {
                    if (thing != null && thing is Corpse corpse)
                    {
                        if (corpse.InnerPawn != null && corpse.InnerPawn.genes != null && corpse.InnerPawn.genes.HasGene(Genes40kDefOf.BEWH_DaemonMutation) && Find.TickManager.TicksGame - corpse.timeOfDeath >= deathTimer)
                        {
                            Mesh boltMesh = LightningBoltMeshPool.RandomBoltMesh;
                            Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt");

                            DaemonDoStrike(corpse.Position, corpse.Map, ref boltMesh);
                            Graphics.DrawMesh(boltMesh, corpse.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(LightningMat, 1), 0);
                            ResurrectionUtility.Resurrect(corpse.InnerPawn);
                        }
                    }
                }
            }
            tickCounter++;
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
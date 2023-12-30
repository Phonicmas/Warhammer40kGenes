using RimWorld;
using System.Collections.Generic;
using Verse;


namespace Genes40k
{
    public class Genes40kUtils
    {
        public static List<GeneDef> SpaceMarineGenes()
        {
            List<GeneDef> genedef = new List<GeneDef>
            {
                Genes40kDefOf.BEWH_SecondaryHeart,
                Genes40kDefOf.BEWH_Ossmodula,
                Genes40kDefOf.BEWH_Biscopea,
                Genes40kDefOf.BEWH_Haemastamen,
                Genes40kDefOf.BEWH_LarramansOrgan,
                Genes40kDefOf.BEWH_CatalepseanNode,
                Genes40kDefOf.BEWH_Preomnor,
                Genes40kDefOf.BEWH_Omophagea,
                Genes40kDefOf.BEWH_MultiLung,
                Genes40kDefOf.BEWH_Occulobe,
                Genes40kDefOf.BEWH_LymansEar,
                Genes40kDefOf.BEWH_SusAnMembrane,
                Genes40kDefOf.BEWH_Melanochrome,
                Genes40kDefOf.BEWH_OoliticKidney,
                Genes40kDefOf.BEWH_Neuroglottis,
                Genes40kDefOf.BEWH_Mucranoid,
                Genes40kDefOf.BEWH_BetchersGland,
                Genes40kDefOf.BEWH_ProgenoidGlands,
                Genes40kDefOf.BEWH_BlackCarapace
            };
            return genedef;
        }

        public static List<GeneDef> PrimarisGenes()
        {
            List<GeneDef> genedef = new List<GeneDef>
            {
                Genes40kDefOf.BEWH_SinewCoil,
                Genes40kDefOf.BEWH_Magnificat,
                Genes40kDefOf.BEWH_BelisarianFurnace
            };
            return genedef;
        }

        public static bool IsSpaceMarine(Pawn pawn)
        {
            foreach (GeneDef geneDef in SpaceMarineGenes())
            {
                if (!pawn.genes.HasGene(geneDef))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsPrimaris(Pawn pawn)
        {
            foreach (GeneDef geneDef in PrimarisGenes())
            {
                if (!pawn.genes.HasGene(geneDef))
                {
                    return false;
                }
            }
            return true;
        }
    
        public static bool IsPsyker(Pawn pawn)
        {
            if (pawn.genes.HasGene(Genes40kDefOf.BEWH_IotaPsyker) || pawn.genes.HasGene(Genes40kDefOf.BEWH_Psyker) || pawn.genes.HasGene(Genes40kDefOf.BEWH_DeltaPsyker) || pawn.genes.HasGene(Genes40kDefOf.BEWH_BetaPsyker))
            {
                return true;
            }
            return false;
        }

        public static bool IsDaemonPrince(Pawn pawn)
        {
            if (pawn.genes.HasGene(Genes40kDefOf.BEWH_DaemonMutation))
            {
                return true;
            }
            return false;
        }

        public static void MakeGenePack40k(Pawn pawn, bool isPrimaris)
        {
            Genepack genepack = (Genepack)ThingMaker.MakeThing(ThingDefOf.Genepack);

            List<GeneDef> genesForPack = new List<GeneDef>();

            Genes40kModSettings modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();

            if (modSettings.progenoidHarvestsAllXenogenes)
            {
                for (int i = 0; i < pawn.genes.Xenogenes.Count; i++)
                {
                    genesForPack.Add(pawn.genes.Xenogenes[i].def);
                }
            }
            else
            {
                genesForPack = SpaceMarineGenes();

                if (isPrimaris)
                {
                    genesForPack.AddRange(PrimarisGenes());
                }
            }

            genepack.Initialize(genesForPack);

            List<Genepack> genepacks = new List<Genepack>
            {
                genepack
            };

            Xenogerm xenogerm = (Xenogerm)ThingMaker.MakeThing(ThingDefOf.Xenogerm);
            if (isPrimaris)
            {
                xenogerm.Initialize(genepacks, "PrimarisMarine".Translate(), Genes40kDefOf.BEWH_PrimarisIcon);
            }
            else
            {
                xenogerm.Initialize(genepacks, "SpaceMarine".Translate(), Genes40kDefOf.BEWH_AstartesIcon);
            }

            if (GenPlace.TryPlaceThing(xenogerm, pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Near))
            {
                return;
            }
            Log.Error("Could not drop item near " + pawn.PositionHeld);
        }

    }
}
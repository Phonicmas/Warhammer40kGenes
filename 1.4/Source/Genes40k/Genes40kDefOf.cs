using RimWorld;
using Verse;


namespace Genes40k
{
    [DefOf]
    public static class Genes40kDefOf
    {
        public static GeneDef BEWH_CustodesStature;
        public static GeneDef BEWH_CustodesExpertise;
        public static GeneDef BEWH_CustodesStrength;
        public static GeneDef BEWH_CustodesResilience;
        public static GeneDef BEWH_CustodesToughness;
        public static GeneDef BEWH_CustodesAnatomy;

        public static GeneDef BEWH_PrimarchStature;
        public static GeneDef BEWH_PrimarchExpertise;
        public static GeneDef BEWH_PrimarchStrength;
        public static GeneDef BEWH_PrimarchResilience;
        public static GeneDef BEWH_PrimarchToughness;
        public static GeneDef BEWH_PrimarchAnatomy;

        public static GeneDef BEWH_SecondaryHeart;
        public static GeneDef BEWH_Ossmodula;
        public static GeneDef BEWH_Biscopea;
        public static GeneDef BEWH_Haemastamen;
        public static GeneDef BEWH_LarramansOrgan;
        public static GeneDef BEWH_CatalepseanNode;
        public static GeneDef BEWH_Preomnor;
        public static GeneDef BEWH_Omophagea;
        public static GeneDef BEWH_MultiLung;
        public static GeneDef BEWH_Occulobe;
        public static GeneDef BEWH_LymansEar;
        public static GeneDef BEWH_SusAnMembrane;
        public static GeneDef BEWH_Melanochrome;
        public static GeneDef BEWH_OoliticKidney;
        public static GeneDef BEWH_Neuroglottis;
        public static GeneDef BEWH_Mucranoid;
        public static GeneDef BEWH_BetchersGland;
        public static GeneDef BEWH_ProgenoidGlands;
        public static GeneDef BEWH_BlackCarapace;

        public static GeneDef BEWH_SinewCoil;
        public static GeneDef BEWH_Magnificat;
        public static GeneDef BEWH_BelisarianFurnace;

        public static GeneDef BEWH_DaemonMutation;

        public static GeneDef BEWH_UndividedMark;
        public static GeneDef BEWH_SlaaneshMark;
        public static GeneDef BEWH_TzeentchMark;
        public static GeneDef BEWH_NurgleMark;
        public static GeneDef BEWH_KhorneMark;

        public static GeneDef BEWH_IotaPsyker;
        public static GeneDef BEWH_Psyker;
        public static GeneDef BEWH_DeltaPsyker;
        public static GeneDef BEWH_BetaPsyker;

        public static GeneDef BEWH_SigmaPariah;
        public static GeneDef BEWH_UpsilonPariah;
        public static GeneDef BEWH_OmegaPariah;

        public static XenotypeIconDef BEWH_DPUndividedIcon;
        public static XenotypeIconDef BEWH_DPSlaaneshIcon;
        public static XenotypeIconDef BEWH_DPTzeentchIcon;
        public static XenotypeIconDef BEWH_DPNurgleIcon;
        public static XenotypeIconDef BEWH_DPKhorneIcon;

        public static HediffDef BEWH_GenetorGrowing;

        public static XenotypeIconDef BEWH_AstartesIcon;
        public static XenotypeIconDef BEWH_PrimarisIcon;
        public static XenotypeIconDef BEWH_CustodesIcon;
        public static XenotypeIconDef BEWH_PrimarchIcon;

        public static PawnKindDef BEWH_SummonedBloodletter;
        public static PawnKindDef BEWH_SummonedPlaguebearer;
        public static PawnKindDef BEWH_SummonedDaemonette;
        public static PawnKindDef BEWH_SummonedPinkHorror;

        public static ResearchProjectDef BEWH_SpaceMarineCreation;
        public static ResearchProjectDef BEWH_PrimarisMarineCreation;
        public static ResearchProjectDef BEWH_CustodesCreation;
        public static ResearchProjectDef BEWH_PrimarchCreation;

        public static HediffDef BEWH_SecondGeneseedHarvest;
        public static HediffDef BEWH_PsychicComa;
        public static HediffDef BEWH_PsychicConnectionSevered;

        public static TraitDef BEWH_Genes;

        public static AbilityDef BEWH_PsySensOff;
        public static AbilityDef BEWH_PsySensOn;

        public static LetterDef BEWH_NaturalBornX;

        public static DamageDef BEWH_WarpEnergy;

        public static WeatherDef BEWH_BloodRain;

        static Genes40kDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(Genes40kDefOf));
        }
    }
}
using RimWorld;
using Verse;


namespace Genes40k
{
    [DefOf]
    public static class Genes40kDefOf
    {
        public static GeneDef BEWH_Custodes;
        public static GeneDef BEWH_Primarch;

        public static GeneDef BEWH_SigmaPariah;
        public static GeneDef BEWH_UpsilonPariah;
        public static GeneDef BEWH_OmegaPariah;

        public static GeneDef BEWH_IotaPsyker;
        public static GeneDef BEWH_Psyker;
        public static GeneDef BEWH_DeltaPsyker;
        public static GeneDef BEWH_BetaPsyker;

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

        public static GeneDef BEWH_NurgleMark;
        public static GeneDef BEWH_UndividedMark;
        public static GeneDef BEWH_KhorneMark;
        public static GeneDef BEWH_TzeentchMark;
        public static GeneDef BEWH_SlaaneshMark;

        public static GeneDef BEWH_DaemonMutation;
        public static GeneDef BEWH_DaemonHide;
        public static GeneDef BEWH_DaemonWings;
        public static GeneDef BEWH_DaemonTail;
        public static GeneDef BEWH_DaemonHorns;

        public static RecipeDef BEWH_AstartesPack;
        public static RecipeDef BEWH_PrimarisPack;

        public static HediffDef BEWH_DaemonPrince;

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

        static Genes40kDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(Genes40kDefOf));
        }
    }
}
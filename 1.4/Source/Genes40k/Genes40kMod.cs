using HarmonyLib;
using Verse;


namespace Genes40k
{
    public class Genes40kMod : Mod
    {
        public static Harmony harmony;
        public Genes40kMod(ModContentPack content) : base(content)
        {
            harmony = new Harmony("Genes40k.Mod");
            harmony.PatchAll();
        }
    }
}
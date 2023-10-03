using Verse;

namespace Genes40k
{
    public class DefModExtension_MaxHealth : DefModExtension
    {
        //This multiplier is added to a base of 1, so 0.1 in increaseMultiplier gives a total amount of health equal to 1.1x of normal
        public float increaseMultiplier;
    }

}
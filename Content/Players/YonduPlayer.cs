using Terraria.ModLoader;

namespace YonduArrow.Content.Players
{
    public class YonduPlayer : ModPlayer
    {
        public bool yonduHelmetEquipped;
        public bool yonduArrowChanneled;
        public bool yonduIsRiding;

        public override void ResetEffects()
        {
            yonduHelmetEquipped = false;
            yonduArrowChanneled = false;
            yonduIsRiding = false;
        }
    }
}

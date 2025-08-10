using Terraria;
using Terraria.ModLoader;

namespace YonduArrow.Content.Dusts
{
    public class RedTrail : ModDust
    {
        public override void SetStaticDefaults()
        {
        base.SetStaticDefaults();
        }
        public override bool Update(Dust dust)
        {
            Player player = Main.LocalPlayer;
            var isRiding = player.GetModPlayer<Players.YonduPlayer>().yonduIsRiding;

            // custom counter to add a delay before drawing the dust
            if (dust.customData == null)
                if (isRiding)
                    dust.customData = -1; // add more delay if riding the arrow
                else
                    dust.customData = 0;

            dust.customData = (int)dust.customData + 1;

            return true;
        }

        public override bool PreDraw(Dust dust)
        {
            if (dust.customData is int counter && counter < 2)
                return false;

            return true;
        }
    }
}
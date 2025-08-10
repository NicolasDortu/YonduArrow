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
            // custom counter to add a delay before drawing the dust
            if (dust.customData == null)
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
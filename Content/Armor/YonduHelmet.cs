using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace YonduArrow.Content.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class YonduHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.LightPurple;
            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<Players.YonduPlayer>().yonduHelmetEquipped = true;
        }

        public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
        {
            var yonduPlayer = drawPlayer.GetModPlayer<Players.YonduPlayer>();
            if (yonduPlayer.yonduArrowChanneled)
            {
                Lighting.AddLight(drawPlayer.Top, 1f, 0.2f, 0.2f);
                color = Color.Lerp(color, Color.Red, 1f);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MythrilBar, 5)
                .AddIngredient(ItemID.Ruby, 50)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

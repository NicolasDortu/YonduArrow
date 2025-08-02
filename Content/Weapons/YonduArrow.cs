using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace YonduArrow.Content.Weapons
{
    /// <summary>
    /// it is a test
    /// </summary>
    public class YonduArrow : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.mana = 14;
            Item.width = 26;
            Item.height = 26;
            Item.useTime = 15;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.noMelee = true;
            Item.channel = true; //Channel so that you can held the weapon [Important]
            Item.knockBack = 8;
            Item.value = Item.sellPrice(platinum: 50);
            Item.rare = ItemRarityID.Master;
            Item.UseSound = SoundID.Item9;
            Item.shoot = ModContent.ProjectileType<Content.Projectiles.YonduArrowProjectile>();
            Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player)
        {
            // Prevent usage if player already has a Yondu arrow projectile
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active &&
                    proj.owner == player.whoAmI &&
                    proj.type == ModContent.ProjectileType<Content.Projectiles.YonduArrowProjectile>())
                {
                    return false;
                }
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WoodenArrow, 100)
                .AddTile(TileID.Anvils)
                .Register();
        }   
    }
}

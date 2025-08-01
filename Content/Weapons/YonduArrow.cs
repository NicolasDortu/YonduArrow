using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace YonduArrow.Content.Weapons
{
    public class YonduArrow : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.mana = 14;
            Item.width = 26;
            Item.height = 26;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.channel = true; //Channel so that you can held the weapon [Important]
            Item.knockBack = 8;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item9;
            Item.shoot = ModContent.ProjectileType<Content.Projectiles.YonduArrowProjectile>();
            Item.shootSpeed = 10f;



            //Item.damage = 50;
            //Item.DamageType = DamageClass.Ranged;
            //Item.width = 14;
            //Item.height = 28;
            //Item.useTime = 20;
            //Item.useAnimation = 20;
            //Item.useStyle = ItemUseStyleID.Shoot;
            //Item.noMelee = true;
            //Item.knockBack = 5f;
            //Item.value = Item.buyPrice(gold: 1);
            //Item.rare = ItemRarityID.Green;
            //Item.UseSound = SoundID.Item5;
            //Item.autoReuse = true;
            //Item.shoot = ModContent.ProjectileType<Content.Projectiles.YonduArrowProjectile>();
            //Item.shootSpeed = 16f;
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

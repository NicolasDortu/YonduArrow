using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace YonduArrow.Content.Weapons
{
    public class YonduArrow : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.mana = 14;
            Item.width = 72;
            Item.height = 14;
            Item.useTime = 15;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.noMelee = true;
            Item.channel = true; //Channel so that you can held the weapon [Important]
            Item.knockBack = 8;
            Item.value = Item.sellPrice(platinum: 5);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = new SoundStyle("YonduArrow/Content/Weapons/YonduWhistle")
            {
                Volume = 0.4f,
                PitchVariance = 0.2f,
                MaxInstances = 1,
                PlayOnlyIfFocused = true,
            };
            Item.shoot = ModContent.ProjectileType<Content.Projectiles.YonduArrowProjectile>();
            Item.shootSpeed = 5f;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.GetModPlayer<Players.YonduPlayer>().yonduHelmetEquipped)
            {
                damage *= 2f;
            }
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
                .AddIngredient(ItemID.FlamingArrow, 100)
                .AddIngredient(ItemID.UnholyArrow, 100)
                .AddIngredient(ItemID.JestersArrow, 100)
                .AddIngredient(ItemID.HellfireArrow, 100)
                .AddIngredient(ItemID.HolyArrow, 100)
                .AddIngredient(ItemID.CursedArrow, 100)
                .AddIngredient(ItemID.FrostburnArrow, 100)
                .AddIngredient(ItemID.ChlorophyteArrow, 100)
                .AddIngredient(ItemID.IchorArrow, 100)
                .AddIngredient(ItemID.VenomArrow, 100)
                .AddIngredient(ItemID.BoneArrow, 100)
                .AddIngredient(ItemID.ShimmerArrow, 100)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }   
    }
}

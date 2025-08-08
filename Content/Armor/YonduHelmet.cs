using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using YonduArrow.Content.Players;

namespace YonduArrow.Content.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class YonduHelmet : ModItem
    {
        // public static readonly int AdditiveGenericDamageBonus = 20;

        // public static LocalizedText SetBonusText { get; private set; }

        public override void SetStaticDefaults()
        {
            // If your head equipment should draw hair while drawn, use one of the following:
            // ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
            // ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
            // ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
            // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;

            // SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs(AdditiveGenericDamageBonus);
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

        // Add glowmask if yonduArrowChanneled

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WoodenArrow, 100)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Items.Zayin;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Ruina.Religion
{
    public class PenitenceR : SEgoItem
	{
		public override void SetDefaults() 
		{
            Item.damage = 88;
			Item.DamageType = DamageClass.Magic; LobItemBase.ConvertVanillaDamageToExtractor(Item);
            Item.width = 40;
			Item.height = 40;
			Item.useTime = 26;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 18f;
			Item.value = 10000;
			Item.shoot = ModContent.ProjectileType<PenitenceRCross>();
			Item.shootSpeed = 0;
			Item.rare = ModContent.RarityType<ZayinR>();
			Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.mana = 4;
			Item.buffType = ModContent.BuffType<PenitenceAtonement>();
        }

        public override void HoldItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
			{
				if (player.HasBuff(Item.buffType))
					player.GetModPlayer<LobotomyZayinPlayer>().PenitenceHardshipAttack(Item, player.GetManaCost(Item));
            }
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			position = Main.MouseWorld;
        }

        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] == 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Penitence>())
            .AddIngredient(ItemID.Pearlwood, 15)
            .AddIngredient(ItemID.ThornsPotion)
            .AddIngredient(ItemID.SoulofLight, 3)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
    }
}
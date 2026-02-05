using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Ruina.Natural
{
    public class SwordSharpenedWithTearsR : SEgoItem
	{
        public override void SetStaticDefaults() 
		{
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 0f;
		}

		public override void SetDefaults() 
		{
            Item.damage = 88;
			Item.DamageType = ModContent.GetInstance<Players.DamageType.ExtractorSummon>();
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 26;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<WawR>();
			Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.autoReuse = true;
            Item.channel = true;
            Item.buffType = ModContent.BuffType<SwordSharpened>();
            Item.shoot = ModContent.ProjectileType<SwordSharpenedWithTearsRSword>();
        }

        public override bool SafeCanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useAnimation = 40;
                Item.useTime = 52;
            }
            else
            {
                Item.useTime = 26;
                Item.useAnimation = 20;
            }

            return base.SafeCanUseItem(player);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            if (player.altFunctionUse != 2 && !player.GetModPlayer<LobotomyWawPlayer>().SwordSharpenedJustice)
            {
                int extraType = ModContent.ProjectileType<SwordSharpenedWithTearsRSwordExtra>();
                if (player.ownedProjectileCounts[extraType] == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int p = Projectile.NewProjectile(source, player.Center, velocity, extraType, damage, knockback, player.whoAmI);
                        Main.projectile[p].originalDamage = damage;
                    }
                }
                int projectile = Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);
                Main.projectile[projectile].originalDamage = damage;
            }
            return false;
        }

        public override bool? UseItem(Player player)
        {
            LobotomyWawPlayer modPlayer = player.GetModPlayer<LobotomyWawPlayer>();
            if (player.altFunctionUse == 2 && !modPlayer.SwordSharpenedJustice)
            {
                if (modPlayer.SwordSharpenedDespair)
                {
                    player.ClearBuff(ModContent.BuffType<Despair>());
                    modPlayer.SwordSharpenedDespair = false;
                    modPlayer.SwordSharpenedImpaledReset();
                }
                if (LobotomyWawPlayer.SwordSharpenedTotalOwned(player) >= 2)
                {
                    player.AddBuff(ModContent.BuffType<Justice>(), 5);
                    player.GetModPlayer<LobotomyWawPlayer>().SwordSharpenedResetCurrentSwords();
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/KnightOfDespair_Change") with { Volume = 0.2f }, player.Center);
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    modPlayer.SwordSharpenedApplyBlessing();
                return true;
            }
            else
            {
                // Check if current sword has invalid id, find next sword if it is
                if (modPlayer.SwordSharpenedCurrentSword[0] >= LobotomyWawPlayer.SwordSharpenedTotalOwned(player))
                {
                    modPlayer.SwordSharpenedFindNextValidSword();
                    return true;
                }
            }

            return base.UseItem(player);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void AddRecipes() 
		{
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SwordSharpenedWithTears>())
            .AddIngredient(ItemID.FallenStar, 10)
            .AddIngredient(ItemID.CobaltShield)
            .AddIngredient(ItemID.Ectoplasm, 6)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }

        public override void HoldItem(Player player)
        {
            //LobotomyWawPlayer modPlayer = player.GetModPlayer<LobotomyWawPlayer>();
            //Main.NewText(modPlayer.SwordSharpenedCurrentSword[0] + " " + modPlayer.SwordSharpenedCurrentSword[1] + " " + modPlayer.SwordSharpenedCurrentSword[2]);
        }
    }


}
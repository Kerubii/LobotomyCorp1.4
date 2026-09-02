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

namespace LobotomyCorp.Items.Ruina.Religion
{
    public class SoundOfAStarR : SEgoItem
	{
        public override void SetStaticDefaults() 
		{
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 0f;
            _ = PassiveList;
		}

		public override void SetDefaults() 
		{
            Item.damage = 88;
            Item.DamageType = DamageClass.Summon; LobItemBase.ConvertVanillaDamageToExtractor(Item);
            Item.width = 40;
			Item.height = 40;
			Item.useTime = 26;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 18f;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<AlephR>();
			Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.autoReuse = true;
            //Item.channel = true;
            Item.buffType = ModContent.BuffType<SoundOfAStarMinionBuff>();
            Item.shoot = ModContent.ProjectileType<SoundOfAStarMinion>();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            position = Main.MouseWorld;
            if (player.altFunctionUse == 2)
            {
                player.AddBuff(ModContent.BuffType<SoundOfAStarBlueStarBuff>(), 2);
                type = ModContent.ProjectileType<SoundOfAStarBlueStar>();
                damage = 530;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && player.ownedProjectileCounts[ModContent.ProjectileType<SoundOfAStarBlueStar>()] > 0)
            {
                return false;
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2 && player.ownedProjectileCounts[ModContent.ProjectileType<SoundOfAStarBlueStar>()] > 0)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.owner == player.whoAmI && p.type == ModContent.ProjectileType<SoundOfAStarBlueStar>())
                    {
                        p.Center = Main.MouseWorld;
                        p.netUpdate = true;
                        break;
                    }
                }
                return true;
            }

            return base.UseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Aleph.SoundOfAStar>())
            .AddIngredient(ItemID.FragmentStardust, 6)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
    }
}
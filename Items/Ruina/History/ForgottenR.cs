using LobotomyCorp.Items.He;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Localization.Language;

namespace LobotomyCorp.Items.Ruina.History
{
    public class ForgottenR : SEgoItem
	{
        public int Swing = 1;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            // DisplayName.SetDefault("Forgotten");
            
            // Tooltip.SetDefault(GetTooltip());
		}

		public override void SetDefaults() 
		{
            //Why doesnt this work on StaticDefaults now :V
            //Passive Text at Localization files now like omg its so good wtf
            /*PassiveText = "Display of Affection - Attacking the same enemy three times in a row marks them, unmarks enemies when far away\n" +
                          "Happy Memories - Attacking the same enemy in a row gives 1% increased contact damage resistance against them, up to 40%. Lost when switching targets\n" +
                          "Nostalgic Embrace - Alternate attack to hug a marked enemy for a short while based on Happy Memories, become invincible if Happy Memories is max." +
                          "|Unattended - After marking an enemy, Unable to hit any other enemies other than the marked enemy";*/

            EgoColor = LobotomyCorp.HeRarity;

            Item.damage = 76;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = -1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<HeR>();
			Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Projectiles.Realized.ForgottenR>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
			Item.autoReuse = true;
            
		}

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.bodyFrame.Y = player.bodyFrame.Height * 4;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<Projectiles.Realized.ForgottenR2>();
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {            
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.altFunctionUse == 2)
                {
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Projectiles.Realized.ForgottenR2Behind>(), damage, knockback, player.whoAmI, -1);

                    return true;
                }

                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, Swing);
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1, Swing);
            }
            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            LobotomyHePlayer modPlayer = player.GetModPlayer<LobotomyHePlayer>();
            return modPlayer.ForgottenAffectionResistance >= 0.03f;
        }

        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                Swing *= -1;
            return base.UseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<BearPaw>())
            .AddIngredient(ItemID.SoulofMight, 2)
            .AddIngredient(ItemID.BrownString)
            .AddIngredient(ItemID.AncientCloth, 2)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
    }
}
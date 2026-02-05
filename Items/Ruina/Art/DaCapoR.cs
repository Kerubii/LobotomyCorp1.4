using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Players.DamageType;

namespace LobotomyCorp.Items.Ruina.Art
{
    public class DaCapoR : SEgoItem
	{
        public override void SetDefaults() 
		{
            Item.damage = 240;
			Item.DamageType = ModContent.GetInstance<Players.DamageType.ExtractorMelee>();
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.useStyle = 15;
			Item.knockBack = 5;
			Item.value = 10000;
		    Item.rare = ModContent.RarityType<AlephR>();
            Item.UseSound = null;
			Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DaCapoMusicSlash>();
            Item.shootSpeed = 1;

            EgoColor = LobotomyCorp.AlephRarity;
        }

        public override bool AltFunctionUse(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<DaCapoPerformance>()] == 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //if (GetPlayerCombo(player) % 2 != 0)
                //return false;
            if (player.altFunctionUse == 2)
            {
                return true;
            }
            if (GetPlayerCombo(player) == 6)
            {
                Projectile.NewProjectile(source, position, new Vector2(player.direction, 0), type, damage, knockback, player.whoAmI, 0, player.itemAnimationMax);
                return false;
            }

            bool silentMusic = player.GetModPlayer<LobotomyAlephPlayer>().DaCapoSilentMusic;
            int musicPhase = player.GetModPlayer<LobotomyAlephPlayer>().DaCapoSilentMusicPhase % 5;
            if (silentMusic)
            {
                int extraType = ModContent.ProjectileType<DaCapoClef>();
                Projectile.NewProjectile(source, position, velocity * (8f + 20 * (musicPhase / 5f)), extraType, damage / 2, knockback, player.whoAmI, 30 * musicPhase);
            }
            if (GetPlayerCombo(player) <= 2)
            {
                Projectile.NewProjectile(source, position, new Vector2(player.direction, 0), type, damage, knockback, player.whoAmI, -1, player.itemAnimationMax);
                return false;
            }
            Projectile.NewProjectile(source, position, new Vector2(player.direction, 0), type, damage, knockback, player.whoAmI, 1, player.itemAnimationMax);
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<DaCapoPerformance>();
            }

            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool SafeCanUseItem(Player player)
        {	
            // Get Player Combo number
            int combo = GetPlayerCombo(player);
            // Set the Player Combo number to 1 if Combo has not started
            if (combo == 0)
                combo++;
            // Increment Player Combo by 2 to move to next combo attack
            else
                combo += 2;

            SetPlayerCombo(player, combo, player.itemAnimationMax + 8);
            
            // If the player is not aplicable to use the third Combo, reset back to first
            if (combo == 5 || combo >= 7)
                SetPlayerCombo(player, 1, player.itemAnimationMax + 8);

            return base.SafeCanUseItem(player);
        }

        public override void UseAnimation(Player player)
        {
            base.UseAnimation(player);
        }

        public override void UseItemFrame(Player player)
        {
            LobCorpLight.LobItemFrame(player, Rotation(player));
            if (GetPlayerCombo(player) == 6)
            {
                float progress = 1f - player.itemAnimation / (float)player.itemAnimationMax;
                if (progress < 0.25f)
                {
                    player.bodyFrame.Y = player.bodyFrame.Height * 2;
                }
                else
                    player.bodyFrame.Y = player.bodyFrame.Height * 3;
            }
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            //LobCorpLight.PseudoUseStyleSwing(player, heldItemFrame, Rotation(player));
            float rot = Rotation(player);
            player.itemLocation = LobCorpLight.LobItemLocation(player, heldItemFrame, rot, player.direction, 16);
            player.itemRotation = MathHelper.ToRadians(rot + 45) * player.direction;
            if (GetPlayerCombo(player) == 6)
            {
                player.itemLocation.X += 16 * player.direction;
                float progress = 1f - player.itemAnimation / (float)player.itemAnimationMax;
                if (progress < 0.5f)
                {
                    progress /= 0.5f;
                    player.itemLocation.Y += 30 * (float)Math.Sin(1.57f * progress);
                }
                else
                    player.itemLocation.Y += 30;
            }
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            int combo = GetPlayerCombo(player);
            float progress = 1f - player.itemAnimation / (float)player.itemAnimationMax;
            if (progress < .75)
            {
                progress = (progress / .75f);
                if (combo == 3 || combo == 4)
                    progress = 1f - progress;

                if (progress < .33f)
                {
                    hitbox.Y += hitbox.Height + 30;
                    if (player.direction > 0)
                        hitbox.X -= hitbox.Width / 2;
                    hitbox.Width = (int)(hitbox.Width * 1.5f);
                }
                else if (progress < .66f)
                {
                    hitbox.Y += hitbox.Height / 2;
                }   
                else
                {
                    hitbox.Y -= 20;
                    if (player.direction > 0)
                    {
                        hitbox.X -= hitbox.Width;
                    }
                    hitbox.Width *= 2;
                }
            }
            else
                noHitbox = true;

            base.UseItemHitbox(player, ref hitbox, ref noHitbox);
        }

        public static float Rotation(Player player)
        {
            float progress = 1f - player.itemAnimation / (float)player.itemAnimationMax;
			int combo = GetPlayerCombo(player);

            float angle = 0f;
            if (combo <= 2)
			{
				if (progress < .75)
				{
					progress = progress / .75f;
					angle += 45f - 180f * (float)Math.Sin(progress * 1.57f);
				}
				else
				{
					progress = (progress - .75f) / .25f;
					angle += -135f - 5f * progress;
				}
				return angle;
			}
            else if (combo == 6)
            {
                angle = -90;
                return angle;
            }

            if (progress < 0.75f)
            {
                progress = progress / .75f;
                angle += -140 + 260f * (float)Math.Sin(progress * 1.57f);
            }
            else
            {
                angle += 120f;
            }
            return angle;
		}

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // If the player hits an enemy during odd number, convert to even and allow the Third hit of the combo
            if (GetPlayerCombo(player) == 1)
            {
                SetPlayerCombo(player, 2, player.itemAnimation + 8);
            }

            if (GetPlayerCombo(player) == 3)
            {
                SetPlayerCombo(player, 4, 60);
            }

            LobotomyAlephPlayer modPlayer = player.GetModPlayer<LobotomyAlephPlayer>();
            if (modPlayer.DaCapoSilentMusic)
                modPlayer.DaCapoTotalDamage += damageDone;

            base.OnHitNPC(player, target, hit, damageDone);
        }

        public static int GetPlayerCombo(Player player)
		{
			return LobotomyModPlayer.ModPlayer(player).AttackComboOrder;
		}

        public static void SetPlayerCombo(Player player, int combo, int cooldown)
		{
            LobotomyModPlayer modPlayer = player.GetModPlayer<LobotomyModPlayer>();
            modPlayer.AttackComboOrder = combo;
			modPlayer.AttackComboOrderCooldown = cooldown;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Aleph.DaCapo>())
            .AddIngredient(ItemID.DeathSickle)
            .AddIngredient(ItemID.LightShard)
            .AddIngredient(ItemID.BeetleHusk)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
    }
}
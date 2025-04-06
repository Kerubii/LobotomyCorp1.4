using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Aleph
{
    public class Smile : LobCorpHeavy
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("It has the pale faces of nameless employees and a giant mouth on it.\n" +
                               "Upon striking with the weapon, the monstrous mouth opens wide to devour the target, its hunger insatiable.\n" +
							   "Alternate attack to slam down an enemy or the ground"); */

        }

        public override void SetDefaults()
        {
            Item.damage = 94;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;

            Item.useTime = 68;
            Item.useAnimation = 68;
            Item.useStyle = 15;

            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Red;

            SwingSound = LobotomyCorp.WeaponSounds.Hammer;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.SmileBitsFriendly>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = false;
            Item.noMelee = false;

            hasHitEnemy = false;
            EGORiskLevel = RiskLevel.Aleph;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void UseStyleAlt(Player player, Rectangle heldItemFrame)
        {
            if (player.altFunctionUse != 2)
                ResetPlayerAttackCooldown(player);

            float rotation = ItemRotation(player) - 90;
            /*if ((player.direction == 1 && rotation < 0) ||
                    (player.direction == -1 && rotation > 0))
            {
				player.itemLocation.X += 12 * player.direction;
            }
            else if ((player.direction == 1 && rotation < 90) ||
                     (player.direction == -1 && rotation > -90))
            {
				player.itemLocation.X -= 12 * player.direction;
			}
            else if ((player.direction == 1 && rotation < 180) ||
                    (player.direction == -1 && rotation > -180))
            {
				player.itemLocation.X -= 12 * player.direction;
			}*/

            rotation = Math.Clamp(rotation, -180, 180);
            rotation = MathHelper.ToRadians(rotation);
            float x = (float)Math.Cos(rotation);
            if (x < 0)
            {
                player.itemLocation.X += 12 * player.direction;
            }
            else
            {
                player.itemLocation.X -= 12 * player.direction;
            }
        }

        public override bool? UseItem(Player player)
        {
            hasHitEnemy = false;
            if (player.altFunctionUse == 2)
            {
                Item.noMelee = true;
            }
            else
            {
                Item.noMelee = false;
                return true;
            }
            return base.UseItem(player);
        }

        public override void UseAnimation(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.noUseGraphic = true;
            }
            else
            {
                //SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/LWeapons/Danggo_Lv2") with { Volume = 0.25f }, player.Center);
                Item.useStyle = 15;
                Item.noUseGraphic = false;
            }
        }

        public override bool CanShoot(Player player)
        {
            return base.CanShoot(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                for (int i = 0; i < 7; i++)
                {
                    Vector2 vel = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-15, 15))) * Main.rand.Next(10, 14);
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, vel, type, damage / 3, 0, player.whoAmI);
                }
                return false;
            }
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                
                /*
                float scale = Item.scale;
                ItemLoader.ModifyItemScale(Item, player, ref scale);
                Vector2 offset = new Vector2(100 * scale, 0).RotatedBy(player.itemRotation - MathHelper.ToRadians(45) + (player.direction < 0 ? -1.57f : 0));
                position += offset;
                velocity = Vector2.Zero;
                type = ModContent.ProjectileType<Projectiles.SmileScream>();
                damage = (int)(damage * 0.2f);
                knockback *= 0.1f;
                */
            }
            else
            {
                type = ModContent.ProjectileType<Projectiles.SmileSpecial>();
            }

            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        private bool hasHitEnemy = false;

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hasHitEnemy && Main.myPlayer == player.whoAmI)
            {
                Vector2 velocity = target.Center - player.Center;
                velocity.Normalize();
                for (int i = 0; i < 7; i++)
                {
                    Vector2 vel = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-15, 15))) * Main.rand.Next(10, 14);
                    Projectile.NewProjectile(player.GetSource_FromThis(), target.Center, vel, Item.shoot, Item.damage / 3, 0, player.whoAmI, target.whoAmI);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Pwnhammer)
            .AddIngredient(ItemID.SoulofNight, 8)
            .AddIngredient(ItemID.RottenChunk, 5)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}
using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace LobotomyCorp.Items.Ruina.Language
{
    public class CrimsonScarR : SEgoItem
    {
        public static Asset<Texture2D> Gun;
        public static Asset<Texture2D> Sickle;
        public static Asset<Texture2D> SickleOpen;

        public override void Load()
        {
            Gun = ModContent.Request<Texture2D>(Texture + "Gun");
            Sickle = ModContent.Request<Texture2D>(Texture + "Sickle");
            SickleOpen = ModContent.Request<Texture2D>(Texture + "SickleOpen");
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 32;
            Item.value = 3000;
            Item.rare = ModContent.RarityType<WawR>();
            Item.damage = 200;
            Item.knockBack = 3f;
            Item.shootSpeed = 12f;
            Item.shoot = 10;
            Item.useAmmo = AmmoID.Bullet;
            Item.useTime = 44;
            Item.useAnimation = 44;
            Item.useStyle = -1;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            LobItemBase.ConvertVanillaDamageToExtractor(Item);
            Item.noUseGraphic = true;
            //Item.reuseDelay = 20;
        }

        public override void HoldItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<RuddedWelts>(), 60 * 10);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            damage = damage / 4;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int delay = player.itemAnimationMax * 2;
            if (player.altFunctionUse == 2)
            {
                player.itemRotation = (float)Math.Atan2(velocity.Y * player.direction, velocity.X * player.direction) - player.fullRotation;
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
                NetMessage.SendData(41, -1, -1, null, player.whoAmI);
                if (!player.GetModPlayer<LobotomyWawPlayer>().CrimsonScarLowHealthActive)
                {
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrimsonScarRSickle>(), damage * 2, knockback, player.whoAmI, delay, -10 * 4);
                }
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                Main.projectile[p].GetGlobalProjectile<LobotomyGlobalProjectile>().CrimsonScarRBullet = true;
                return false;
            }
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrimsonScarRSickle>(), damage * 2, knockback, player.whoAmI, delay);
            if (player.GetModPlayer<LobotomyWawPlayer>().CrimsonScarLowHealthActive)
            {
                int amount = Main.rand.Next(3, 7);
                for (int i = 0; i < amount; i++)
                {
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrimsonScarRSickle>(), damage, knockback, player.whoAmI, delay, 30 * 4, Main.rand.NextFloat(-0.26f, 0.26f));
                }
            }
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return player.altFunctionUse == 2;
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (player.statLife <= player.statLifeMax2 / 2 && player.altFunctionUse == 2)
            {
                return 0.5f;
            }
            return base.UseTimeMultiplier(player);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.altFunctionUse != 2)
            {
                float rotation = ItemRotation(player);
                LobCorpLight.PseudoUseStyleSwing(player, heldItemFrame, rotation);
            }
            else
            {
                player.itemLocation = LobCorpLight.LobItemLocation(player, heldItemFrame, 90);
            }
        }

        public override void UseItemFrame(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                float rotation = ItemRotation(player);
                LobCorpLight.LobItemFrame(player, rotation - 90);
            }
            else
            {
                if (player.GetModPlayer<LobotomyWawPlayer>().CrimsonScarLowHealthActive)
                {
                    float num23 = player.itemRotation * (float)player.direction;
                    player.bodyFrame.Y = player.bodyFrame.Height * 3;
                    if ((double)num23 < -0.75)
                    {
                        player.bodyFrame.Y = player.bodyFrame.Height * 2;
                        if (player.gravDir == -1f)
                        {
                            player.bodyFrame.Y = player.bodyFrame.Height * 4;
                        }
                    }
                    if ((double)num23 > 0.6)
                    {
                        player.bodyFrame.Y = player.bodyFrame.Height * 4;
                        if (player.gravDir == -1f)
                        {
                            player.bodyFrame.Y = player.bodyFrame.Height * 2;
                        }
                    }
                }
                else
                {
                    float prog = 1f - player.itemAnimation / (float)player.itemAnimationMax;
                    player.bodyFrame.Y = player.bodyFrame.Height * 4;
                    if (prog > 0.4f)
                    {
                        float rotation = ItemRotation(player);
                        LobCorpLight.LobItemFrame(player, rotation - 90);
                    }
                }
            }
            base.UseItemFrame(player);
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/RedHood_Atk1") with { Volume = 0.2f, PitchVariance = 0.1f }, player.Center);
                //This is a jank way of changing item's attack cooldown, thought it would better fit at onhit same as immune but I guess not since that ones before they change the immune time and attackCD
                LobCorpLight.ResetPlayerAttackCooldown(player);
            }
            else
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/RedHood_Gun") with { Volume = 0.2f, PitchVariance = 0.1f, MaxInstances = 0 }, player.Center);
            return null;
        }

        public sealed override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (player.altFunctionUse != 2)
            {
                hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, 32, 32);
                if (!Main.dedServ)
                {
                    Rectangle hitboxSize = Item.GetDrawHitbox(Item.type, player);
                    hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, hitboxSize.Width, hitboxSize.Height);
                }
                float adjustedItemScale = player.GetAdjustedItemScale(Item);
                hitbox.Width = (int)((float)hitbox.Width * adjustedItemScale);
                hitbox.Height = (int)((float)hitbox.Height * adjustedItemScale);
                if (player.direction == -1)
                {
                    hitbox.X -= hitbox.Width;
                }
                if (player.gravDir == 1f)
                {
                    hitbox.Y -= hitbox.Height;
                }

                float prog = 1f - player.itemAnimation / (float)player.itemAnimationMax;
                if (prog < .2f)
                {
                    if (player.direction == 1)
                    {
                        hitbox.X -= (int)(hitbox.Width * 1);
                    }
                    hitbox.Width *= 2;
                    hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
                    hitbox.Height = (int)(hitbox.Height * 1.4);
                }
                else if (prog < .4f)
                {
                    if (player.direction == -1)
                    {
                        hitbox.X -= (int)((double)hitbox.Width * 1.4 - (double)hitbox.Width);
                    }
                    hitbox.Width = (int)((double)hitbox.Width * 1.4);
                    hitbox.Y += (int)((double)hitbox.Height * 0.5 * (double)player.gravDir);
                    hitbox.Height = (int)((double)hitbox.Height * 1.4);
                }
                else
                    noHitbox = true;
            }
            else
                noHitbox = true;
        }

        public static float ItemRotation(Player player)
        {
            float prog = 1f - player.itemAnimation / (float)player.itemAnimationMax;
            float rotation = 0;

            if (prog < 0.2f)
            {
                prog = prog / 0.2f;
                rotation = (-60 + 200 * (float)Math.Sin(1.57f * prog));// * player.direction;
            }
            else if (prog < 0.4f)
            {
                rotation = 140;// * player.direction;
            }
            else
            {
                prog = (prog - 0.4f) / 0.6f;
                rotation = (140 - 200 * prog);// * player.direction;
            }
            return rotation;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.GetGlobalNPC<LobotomyGlobalNPC>().CrimsonScarPrey)
            {
                modifiers.SourceDamage += LobotomyWawPlayer.CrimsonScarPreyBoost;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CrimsonScar>())
            .AddIngredient(ItemID.AdhesiveBandage)
            .AddIngredient(ItemID.BloodMoonStarter)
            .AddIngredient(ItemID.LunarTabletFragment, 2)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
    }
}
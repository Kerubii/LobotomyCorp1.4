using LobotomyCorp.Players.DamageType;
using LobotomyCorp.Projectiles.RedMist;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class Regret : LobCorpHeavy
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Enemies crushed by this regret can never return to their normal life.\n" +
                               "Before swinging this weapon, expressing one's condolences for the demise of the inmate who couldn't even have a funeral would be nice."); */

        }

        public override void LobSetDefaults()
        {
            Item.damage = 52;
            Item.DamageType = RWBPEnable(DamageClass.Melee, ModContent.GetInstance<ExtractorRed>());
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 56;
            Item.useAnimation = 56;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<TethB>();
            SwingSound = LobotomyCorp.WeaponSounds.Hammer;
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.Teth;

            Item.shoot = ModContent.ProjectileType<RegretHelper>();
            Item.shootSpeed = 1;
        }

        public override bool CanShoot(Player player)
        {
            return RedMistMaskUpgrade(player);
        }

        public override bool AltFunctionUse(Player player)
        {
            return RedMistMaskUpgrade(player);
        }

        public override bool UseStyleAlt(Player player, Rectangle heldItemFrame)
        {
            if (RedMistMaskUpgrade(player))
            {
                float rotation = ItemRotationAlt(player);
                PseudoUseStyleSwing(player, heldItemFrame, rotation);

                if (player.altFunctionUse == 2)
                {
                    if ((player.itemAnimation == (int)(player.itemAnimationMax * 0.92f) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.72f) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.52f) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.24f))
                    && SwingSound != null)
                        SoundEngine.PlaySound(SwingSound, player.Center);
                }
                else
                {
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.9f))
                        SoundEngine.PlaySound(SwingSound, player.Center);
                    if (player.itemAnimation <= player.itemAnimationMax / 2)
                        player.itemLocation = new Vector2(-10, -10);
                }

                return false;
            }
            return true;
        }

        public override bool UseItemFrameAlt(Player player)
        {
            if (RedMistMaskUpgrade(player))
            {
                float rotation = ItemRotationAlt(player);
                LobCorpLight.LobItemFrame(player, rotation - 90);
                return false;
            }
            return true;
        }

        public override bool UseItemHitboxAlt(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (RedMistMaskUpgrade(player))
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
                if (player.altFunctionUse == 2)
                {
                    if (prog < 0.6f)
                    {
                        prog = (prog % 0.2f) / 0.2f;
                        if (prog > 0.4f && prog < 0.8f)
                        {
                            if (prog < .54f)
                            {
                                if (player.direction == 1)
                                {
                                    hitbox.X -= (int)(hitbox.Width * 1);
                                }
                                hitbox.Width *= 2;
                                hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
                                hitbox.Height = (int)(hitbox.Height * 1.4);
                            }
                            else if (prog < .67f)
                            {
                                if (player.direction == -1)
                                {
                                    hitbox.X -= (int)((double)hitbox.Width * 1.4 - (double)hitbox.Width);
                                }
                                hitbox.Width = (int)((double)hitbox.Width * 1.4);
                                hitbox.Y += (int)((double)hitbox.Height * 0.5 * (double)player.gravDir);
                                hitbox.Height = (int)((double)hitbox.Height * 1.4);
                            }
                        }
                        else
                            noHitbox = true;
                    }
                    else
                    {
                        prog = (prog - 0.6f) / 0.4f;
                        if (prog > 0.6f && prog < 0.8f)
                        {
                            if (prog < .67f)
                            {
                                if (player.direction == 1)
                                {
                                    hitbox.X -= (int)(hitbox.Width * 1);
                                }
                                hitbox.Width *= 2;
                                hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
                                hitbox.Height = (int)(hitbox.Height * 1.4);
                            }
                            else if (prog < .73f)
                            {
                                if (player.direction == -1)
                                {
                                    hitbox.X -= (int)((double)hitbox.Width * 1.4 - (double)hitbox.Width);
                                }
                                hitbox.Width = (int)((double)hitbox.Width * 1.4);
                                hitbox.Y += (int)((double)hitbox.Height * 0.5 * (double)player.gravDir);
                                hitbox.Height = (int)((double)hitbox.Height * 1.4);
                            }
                        }
                        else
                            noHitbox = true;
                    }
                }
                else
                {
                    if (prog < 0.5f)
                    {
                        prog /= 0.5f;
                        if (prog > 0.5f && prog < 0.8f)
                        {
                            if (prog < .6f)
                            {
                                if (player.direction == 1)
                                {
                                    hitbox.X -= (int)(hitbox.Width * 1);
                                }
                                hitbox.Width *= 2;
                                hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
                                hitbox.Height = (int)(hitbox.Height * 1.4);
                            }
                            else if (prog < .7f)
                            {
                                if (player.direction == -1)
                                {
                                    hitbox.X -= (int)((double)hitbox.Width * 1.4 - (double)hitbox.Width);
                                }
                                hitbox.Width = (int)((double)hitbox.Width * 1.4);
                                hitbox.Y += (int)((double)hitbox.Height * 0.5 * (double)player.gravDir);
                                hitbox.Height = (int)((double)hitbox.Height * 1.4);
                            }
                        }
                        else
                            noHitbox = true;
                    }
                    else
                        noHitbox = true;
                }

                return false;
            }

            return true;
        }

        private float ItemRotationAlt(Player player)
        {
            float prog = 1f - player.itemAnimation / (float)player.itemAnimationMax;
            float rotation = 0;
            if (player.altFunctionUse == 2)
            {
                if (prog < 0.6f)
                {
                    prog = (prog % 0.2f) / 0.2f;
                    if (prog < 0.3f)
                    {
                        prog = prog / 0.3f;
                        prog = 1f - (float)Math.Pow(1 - prog, 3);
                        rotation = 140 - 200 * prog;
                    }
                    else if (prog < 0.4f)
                    {
                        rotation = -60;
                    }
                    else if (prog < 0.8f)
                    {
                        prog = (prog - 0.4f) / 0.4f;
                        prog = 1f - (float)Math.Pow(1 - prog, 3);
                        rotation = (-60 + 200 * prog);
                    }
                    else
                    {
                        rotation = 140;
                    }
                }
                else
                {
                    prog = (prog - 0.6f) / 0.4f;
                    if (prog < 0.4f)
                    {
                        prog = prog / 0.4f;
                        prog = 1f - (float)Math.Pow(1 - prog, 3);
                        rotation = 140 - 200 * prog;
                    }
                    else if (prog < 0.6f)
                    {
                        rotation = -60;
                    }
                    else if (prog < 0.8f)
                    {
                        prog = (prog - 0.6f) / 0.2f;
                        prog = 1f - (float)Math.Pow(1 - prog, 3);
                        rotation = (-60 + 260 * prog);
                    }
                    else
                    {
                        rotation = 200;
                    }
                }
            }
            else
            {
                if (prog < 0.5f)
                {
                    prog /= 0.5f;
                    if (prog < 0.2f)
                    {
                        prog = prog / 0.2f;
                        prog = 1f - (float)Math.Pow(1 - prog, 3);
                        rotation = 140 - 200 * prog;
                    }
                    else if (prog < 0.5f)
                    {
                        rotation = -60;
                    }
                    else if (prog < 0.8f)
                    {
                        prog = (prog - 0.5f) / 0.3f;
                        prog = 1f - (float)Math.Pow(1 - prog, 3);
                        rotation = (-60 + 200 * prog);
                    }
                    else
                    {
                        rotation = 140;
                    }
                }
                else
                {
                    if (player.heldProj >= 0)
                    {
                        rotation = MathHelper.ToDegrees(Main.projectile[player.heldProj].velocity.ToRotation());
                    }
                }
            }

            return rotation;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (RedMistMaskUpgrade(player))
            {
                if (player.altFunctionUse == 2)
                    player.GetModPlayer<LobotomyModPlayer>().ReplaceItemCooldown(0.06f, 0.2f, target.whoAmI);
            }
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (RedMistMaskUpgrade(player))
            {
                if (player.altFunctionUse == 2)
                    return 0.4f;
                else
                    return 0.8f;
            }

            return base.UseSpeedMultiplier(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.IronBar, 10)
            .AddIngredient(ItemID.Chain, 12)
            .AddIngredient(ItemID.Wood, 5)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Aleph
{
    public class DaCapo : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("A scythe that swings silently and with discipline like a conductor's gestures and baton.\n" +
                               "If there were a score for this song, it would be one that sings of the apocalypse."); */
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 52;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = 1;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.DaCapo>();
            Item.shootSpeed = 1f;
            Item.scale = 0.8f;
            PreviousTarget = -1;
            EGORiskLevel = RiskLevel.Aleph;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        private int PreviousTarget = -1;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = 5;
                Item.noUseGraphic = true;
            }
            else
            {
                Item.useStyle = 1;
                Item.noUseGraphic = false;
            }

            if (Item.useStyle == 1)
            {
                float progress = 1f - player.itemAnimation / (float)player.itemAnimationMax;
                float rotation = ItemRotation(progress, 1);
                LobCorpLight.PseudoUseStyleSwing(player, heldItemFrame, rotation);

                if (progress > 0.5f)
                {
                    int distancIn = 66;
                    if (progress < 0.8f)
                    {
                        progress = (progress - 0.5f) / 0.3f;
                        player.itemLocation.X -= distancIn * (float)Math.Sin(1.57f * progress) * player.direction;
                    }
                    else
                    {
                        player.itemLocation.X -= distancIn * player.direction;
                    }
                }
            }
        }

        public override void UseItemFrame(Player player)
        {
            if (Item.useStyle == 1)
            {
                LobCorpLight.LobItemFrame(player, ItemRotation(1f - player.itemAnimation / (float)player.itemAnimationMax, player.direction));
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            PreviousTarget = target.whoAmI;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                return 0.54f;
            }
            return base.UseSpeedMultiplier(player);
        }

        public override bool CanShoot(Player player)
        {
            return player.altFunctionUse == 2;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            damage = (int)(damage * 0.6f);
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = LobotomyCorp.WeaponSound("silent2_1");
                Item.noMelee = true;
            }
            else
            {
                Item.UseSound = LobotomyCorp.WeaponSound("silent1");
                Item.noMelee = false;
            }
            return base.CanUseItem(player);
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.noMelee = true;
            }
            else
            {
                Item.noMelee = false;
            }

            if (PreviousTarget >= 0)
            {
                int immuneLimit = player.itemAnimationMax / 2 - 2;
                LobCorpLight.ResetPlayerImmuneHit(player, ref PreviousTarget, immuneLimit);
            }
            /*
            if (Item.useStyle == 1 && PreviousTarget >= 0)
            {
                //Item.noUseGraphic = false;
                //Just a way to make this thing hit twice :V
                NPC target = Main.npc[PreviousTarget];
                int immuneLimit = player.itemAnimationMax / 2 - 2;
                if (player.itemAnimation > immuneLimit)
                {
                    player.SetMeleeHitCooldown(target.whoAmI, 0);// player.itemAnimation - immuneLimit);
                    target.immune[player.whoAmI] = player.itemAnimation - immuneLimit;
                }

                PreviousTarget = -1;
                //This is a jank way of changing item's attack cooldown, thought it would better fit at onhit same as immune but I guess not since that ones before they change the immune time and attackCD
                //int cooldown = Math.Max(1, (int)((double)player.itemAnimationMax * 0.1f));
                LobCorpLight.ResetPlayerAttackCooldown(player);
            }*/
            return base.UseItem(player);
        }

        public override void ModifyItemScale(Player player, ref float scale)
        {
            scale *= 1.2f;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (player.itemAnimation > player.itemAnimationMax / 2)
                modifiers.Knockback *= 0.75f;
        }

        private float ItemRotation(float progress, int direction)
        {
            float rotation = 0;
            if (progress < 0.3f)
            {
                progress = progress / 0.3f;
                rotation += (-45 + 135 * (float)Math.Sin(1.57f * progress)) * direction;
            }
            else if (progress > 0.5f && progress < 0.8f)
            {
                rotation += (94 + Main.rand.Next(-4, 5)) * direction;
            }
            else
            {
                rotation += 94 * direction;
            }
            return rotation;
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (Item.useStyle == 1)
            {
                hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, 32, 32);
                if (!Main.dedServ)
                {
                    Rectangle hitboxSize = Item.GetDrawHitbox(Item.type, player);
                    hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, hitboxSize.Width, hitboxSize.Height);
                }
                float adjustedItemScale = player.GetAdjustedItemScale(Item);
                hitbox.Width = (int)(hitbox.Width * adjustedItemScale);
                hitbox.Height = (int)(hitbox.Height * adjustedItemScale);
                if (player.direction == -1)
                {
                    hitbox.X -= hitbox.Width;
                }
                if (player.gravDir == 1f)
                {
                    hitbox.Y -= hitbox.Height;
                }

                float prog = 1f - player.itemAnimation / (float)player.itemAnimationMax;
                if (prog < .15f)
                {
                    if (player.direction == 1)
                    {
                        hitbox.X -= hitbox.Width * 1;
                    }
                    hitbox.Width *= 2;
                    hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
                    hitbox.Height = (int)(hitbox.Height * 1.4);
                }
                else
                {
                    if (player.direction == -1)
                    {
                        hitbox.X -= (int)(hitbox.Width * 1.4 - hitbox.Width);
                    }
                    hitbox.Width = (int)(hitbox.Width * 1.4);
                    hitbox.Y += (int)(hitbox.Height * 0.5 * player.gravDir);
                    hitbox.Height = (int)(hitbox.Height * 1.2);
                }
                if (prog > 0.5f)
                {
                    hitbox.Y += hitbox.Height / 2;
                    hitbox.Height /= 2;
                }

                if (prog > 0.3f && prog < 0.5f ||
                    prog > 0.8f)
                    noHitbox = true;
            }
            base.UseItemHitbox(player, ref hitbox, ref noHitbox);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.DarkShard)
            .AddIngredient(ItemID.LightShard)
            .AddIngredient(ItemID.MusicBox)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}
using LobotomyCorp.Misc;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles;
using LobotomyCorp.Projectiles.Realized;
using LobotomyCorp.Util;
using LobotomyCorp.Visuals.LobEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Ruina.Art
{
    public class FaintAromaR : SEgoItem
	{
        public override string Texture => "LobotomyCorp/Items/Ruina/Art/FaintAromaS";
        private static Asset<Texture2D> Display;
        private bool swingdirection = false;
        public bool SwingDirection { get { return swingdirection; } }

        private int PreviousTarget = -1;

        public override void Load()
        {
            Display = ModContent.Request<Texture2D>(Texture + "Display");
            base.Load();
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
        }

		public override void SetDefaults() 
		{
            Item.damage = 116;
			Item.DamageType = DamageClass.Melee;
            LobItemBase.ConvertVanillaDamageToExtractor(Item);
            Item.width = 40;
			Item.height = 40;
			Item.useTime = 42;
			Item.useAnimation = 42;
            Item.reuseDelay = 4;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.knockBack = 0;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<WawR>();
			Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/Ali_Sub_Atk") with {MaxInstances = 2};
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FaintAromaSlash>();//ModContent.ProjectileType<FaintAromaRAlt>();
            Item.shootSpeed = 1f;
            swingdirection = false;
		}

        public override bool AltFunctionUse(Player player)
        {
            return true;//player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetalNum() > 0;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                LobotomyWawPlayer wawPlayer = player.GetModPlayer<LobotomyWawPlayer>();
                float boost = 1f + .2f * wawPlayer.FaintAromaPetalNum();
                velocity *= 28f * boost;

                type = ModContent.ProjectileType<FaintAromaRAlt>();
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == Item.shoot)
            {
                int petals = player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetalNum();
                int time = 20;
                float velMult = 12f;
                switch (petals)
                {
                    case 1:
                        velMult += 4f;
                        time -= 2;
                        break;
                    case 2:
                        velMult += 12f;
                        time -= 4;
                        break;
                    case 3:
                        velMult += 20f;
                        time -= 8;
                        break;
                }
                velocity *= velMult;
                damage /= 3;

                // First instant Projectile
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, time, 0);

                // Second slash Projectile with delay
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, time, player.itemAnimationMax / 2);

                return false;
            }
            return true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            int petals = player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetalNum();
            if (player.altFunctionUse != 2)
            {
                if (player.itemAnimation == player.itemAnimationMax - 1)
                {
                    if (Main.rand.NextBool(2))
                    {
                        SmearCircle(player, swingdirection);
                    }
                    else
                    {
                        SmearEllipse(player, swingdirection);
                    }                        
                    SoundEngine.PlaySound(Item.UseSound, player.position);
                }

                if (player.itemAnimation == player.itemAnimationMax / 2)
                {
                    if (Main.rand.NextBool(2))
                    {
                        SmearCircle(player, swingdirection);
                    }
                    else
                    {
                        SmearEllipse(player, swingdirection);
                    }
                    SoundEngine.PlaySound(Item.UseSound, player.position);
                }

                LobCorpLight.PseudoUseStyleSwing(player, heldItemFrame, SwingRotation(player));
            }
            else
            {
                if (player.heldProj >= 0)
                {
                    Projectile held = Main.projectile[player.heldProj];
                    player.itemRotation = (float)Math.Atan2(held.velocity.Y * (float)player.direction, held.velocity.X * (float)player.direction) - player.fullRotation;    
                }

                player.itemLocation.X = player.position.X + (float)player.width * 0.5f - (float)(player.direction * 2); // forward port from 1.4.5
                player.itemLocation.Y = player.MountedCenter.Y - (float)heldItemFrame.Height * 0.5f;
            }

            if (player.itemAnimation == player.itemAnimationMax / 2)
            {
                player.ResetMeleeHitCooldowns();
            }

            float backWeaponSwing = SwingRotation2(player);
            player.GetModPlayer<LobotomyModPlayer>().DrawWeaponBack(
                TextureAssets.Item[Item.type].Value,
                LobCorpLight.LobItemLocation(player, heldItemFrame, backWeaponSwing - 90) + new Vector2(8f * player.direction, 0),
                MathHelper.ToRadians(backWeaponSwing - 45) * player.direction);
        }

        public static void SmearCircle(Player player, bool isDown)
        {
            WeaponSmearCircle smear = new WeaponSmearCircle();
            
            int dir = isDown ? -1 : 1;
            float start = player.direction > 0 ? -140 : -50;

            smear.Setup(player, new Vector2(12 * player.direction, 0), MathHelper.ToRadians(start * dir), player.itemAnimationMax / 3, player.direction * dir, true);
            smear.SetupSemiCircle(35, 100, MathHelper.ToRadians(10), MathHelper.ToRadians(180), MathHelper.ToRadians(270));
            smear.SetShaderImage(
                MiscAssets.FlatColor,
                MiscAssets.TexTrail2,
                MiscAssets.Gradient
                );
            smear.Color = new Color(249, 159, 253);

            smear.AddEffect();
        }

        public static void SmearEllipse(Player player, bool isDown)
        {
            WeaponSmearEllipse smear = new WeaponSmearEllipse();    
            int dir = isDown ? -1 : 1;
            float start = -140 * player.direction;

            smear.Setup(player, new Vector2(12 * player.direction, 0), (player.direction > 0 ? 0 : 3.14f) + Main.rand.NextFloat(-0.3f, 0.3f), player.itemAnimationMax / 3, player.direction * dir, true);
            smear.SetupPartEllipse(90, 65, 160, 125, MathHelper.ToRadians(start * dir), MathHelper.ToRadians(10), MathHelper.ToRadians(180), MathHelper.ToRadians(270));
            smear.Color = new Color(249, 159, 253);
            smear.SetShaderImage(
                MiscAssets.FlatColor,
                MiscAssets.TexTrail2,
                MiscAssets.Gradient
                );

            LobCustomDraw.Instance().AddVEffects(smear);
        }

        public override bool? UseItem(Player player)
        {
            swingdirection = !swingdirection;
            if (player.altFunctionUse == 2)
                Item.noUseGraphic = true;
            else
                Item.noUseGraphic = false;
            return true;
        }

        public override void UseItemFrame(Player player)
        {
            if (player.altFunctionUse != 2)
                LobCorpLight.LobItemFrame(player, SwingRotation(player) - 90);
            else
            {
                float num20 = player.itemRotation * (float)player.direction;
                player.bodyFrame.Y = player.bodyFrame.Height * 3;
                if ((double)num20 < -0.75)
                {
                    player.bodyFrame.Y = player.bodyFrame.Height * 2;
                    if (player.gravDir == -1f)
                        player.bodyFrame.Y = player.bodyFrame.Height * 4;
                }

                if ((double)num20 > 0.6)
                {
                    player.bodyFrame.Y = player.bodyFrame.Height * 4;
                    if (player.gravDir == -1f)
                        player.bodyFrame.Y = player.bodyFrame.Height * 2;
                }
            }
        }

        float SwingRotation(Player player)
        {
            float time = 1f - (float)player.itemAnimation / player.itemAnimationMax;
            bool direction = swingdirection;
            if (player.ItemAnimationJustStarted)
                direction = !direction;

            if (!direction)
            {
                return -70 + 290 * Easing.EaseOutExpo(time);
            }
            else
            {
                return 220 - 290 * Easing.EaseOutExpo(time);
            }
        }
        
        public float SwingRotation2(Player player)
        {
            float time = 1f - (float)player.itemAnimation / player.itemAnimationMax;
            bool direction = swingdirection;
            if (player.ItemAnimationJustStarted)
                direction = !direction;

            if (time < 0.5f)
            {
                time += 0.5f;
                direction = !direction;
            }
            else
                time -= 0.5f;

            time = Math.Clamp(time, 0, 1);


            if (!direction)
            {
                return -70 + 290 * Easing.EaseOutExpo(time);
            }
            else
            {
                return 220 - 290 * Easing.EaseOutExpo(time);
            }
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            float time = ((float)player.itemAnimation / player.itemAnimationMax) % 0.5f;
            if (time > 0.2f && (player.altFunctionUse != 2 || player.itemAnimation <= player.itemAnimationMax / 2))
            {
                if (swingdirection)
                    time = 0.2f + (0.5f - time);

                hitbox = new Rectangle((int)player.Center.X, (int)player.Center.Y, 32, 32);
                if (!Main.dedServ)
                {
                    Rectangle hitboxSize = Item.GetDrawHitbox(Item.type, player);
                    hitbox = new Rectangle((int)player.Center.X, (int)player.Center.Y, hitboxSize.Width, hitboxSize.Height);
                }
                float secondaryScale = 0.4f;
                float adjustedItemScale = player.GetAdjustedItemScale(Item);
                hitbox.Width = (int)(hitbox.Width * adjustedItemScale);
                hitbox.Height = (int)(hitbox.Height * (adjustedItemScale + secondaryScale));
                if (player.direction == -1)
                {
                    hitbox.X -= hitbox.Width;
                }
                if (player.gravDir == 1f)
                {
                    hitbox.Y -= hitbox.Height;
                }

                if (time > 0.4f)
                {
                    if (player.direction == 1)
                    {
                        hitbox.X -= (int)(hitbox.Width * 1);
                    }
                    hitbox.Width *= 2;
                    hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
                    hitbox.Height = (int)(hitbox.Height * 1.4);
                }
                else if (time > 0.3f)
                {
                    if (player.direction == -1)
                    {
                        hitbox.X -= (int)((double)hitbox.Width * (1.4 + secondaryScale + 0.3f) - (double)hitbox.Width);
                    }
                    hitbox.Width = (int)((double)hitbox.Width * (1.4 + secondaryScale + 0.3f));
                    hitbox.Y += (int)((double)hitbox.Height * 0.5 * (double)player.gravDir);
                    hitbox.Height = (int)((double)hitbox.Height * 1.4);
                }
                else
                {
                    if (player.direction == 1)
                    {
                        hitbox.X -= (int)(hitbox.Width * 1);
                    }
                    hitbox.Width *= 2;
                    hitbox.Y += (int)((hitbox.Height * 2.0 - hitbox.Height) * player.gravDir);
                    hitbox.Height = (int)(hitbox.Height * 1.4);
                }
            }
            else
                noHitbox = true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyWawPlayer wawPlayer = player.GetModPlayer<LobotomyWawPlayer>();
            int variableAmount = 10;
            if (wawPlayer.FaintAromaPetalNum() <= 1)
                variableAmount *= 3;
            wawPlayer.FaintAromaAddPetal(variableAmount);

            player.GetModPlayer<LobotomyModPlayer>().ReplaceItemCooldown(0.1f, 0.5f, target.whoAmI);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            float time = ((float)player.itemAnimation / player.itemAnimationMax) % 0.5f;
            if (time > 0.2f && player.altFunctionUse != 2)
            {
                Dust dust = Main.dust[Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.VenomStaff)];
                //dust.fadeIn = 1.2f;
                dust.noGravity = true;
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color ItemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(Display.Value, position, frame, drawColor, 0, origin, scale, 0, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = Display.Value;
            spriteBatch.Draw(tex, Item.position - Main.screenPosition + new Vector2(Item.width/2, Item.height - tex.Height/2), tex.Frame(), lightColor, rotation, tex.Size()/2, scale, 0, 0);
            return false;
        }

        public override void AddRecipes() 
		{
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Items.Waw.FaintAroma>())
            .AddIngredient(ItemID.MudBud)
            .AddIngredient(ItemID.GuideVoodooDoll)
            .AddIngredient(ItemID.Pearlwood, 10)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
	}
}
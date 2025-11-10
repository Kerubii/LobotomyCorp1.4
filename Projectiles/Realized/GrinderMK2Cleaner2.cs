using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using Terraria.Audio;
using Terraria.GameContent;
using System.IO;
using LobotomyCorp.Players;

namespace LobotomyCorp.Projectiles.Realized
{
    public class GrinderMk2Cleaner2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cleaning Tools");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 1000;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        private float order
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private float AttackRotation;

        private Vector2 ProjectileRoot => Projectile.Center + ProjectileRootOffset;

        private Vector2 ProjectileRootOffset => new Vector2(-2 * Projectile.direction, 14).RotatedBy(Projectile.rotation);

        private Vector2 HitboxExtension => new Vector2(Projectile.width / 2, 0).RotatedBy(Projectile.rotation);

        private bool CanAttack(Player player, LobotomyHePlayer modPlayer, Item heldItem)
        {
            if (player.altFunctionUse != 2 && heldItem.type == ModContent.ItemType<Items.Ruina.Technology.GrinderMk52R>() && ((Items.Ruina.Technology.GrinderMk52R)heldItem.ModItem).GrinderWeaponOrder == order)
                return true;

            if (modPlayer.GrinderMk2AttackCooldown <= 0 && Main.rand.NextBool(10) && modPlayer.GrinderMk2AttackRandom == order && player.itemAnimation == player.itemAnimationMax - 1)
                return true;

            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            LobotomyHePlayer modPlayer = player.GetModPlayer<LobotomyHePlayer>();
            LobotomyDashPlayer dashPlayer = player.GetModPlayer<LobotomyDashPlayer>();

            //Add Movespeed Buff
            int BuffType = ModContent.BuffType<Buffs.GrinderMk52Activated>();
            if (!player.HasBuff(BuffType))
                player.AddBuff(BuffType, 1);

            Vector2 ownerMountedCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.direction = player.direction;
            Projectile.spriteDirection = player.direction;

            Projectile.rotation = (Projectile.Center - ownerMountedCenter).ToRotation() + 1.57f;

            //Which Quadrant?
            int dirX = order < 2 ? -1 : 1;
            int dirY = order % 2 == 0 ? -1 : 1;

            //Change targetPos to move Blades
            Vector2 targetPos = ownerMountedCenter + new Vector2(32 * dirX, 12 * dirY);
            float speed = 12f;

            //When PlayerSwings
            int AttackSwingTime = 30;

            Item heldItem = player.HeldItem;
            if (Projectile.owner == Main.myPlayer &&
                Projectile.ai[1] <= 0)
            {
                if (CanAttack(player, modPlayer, heldItem) && player.itemAnimation > player.itemAnimation / 2)
                {
                    ChangeBatteryValue(-8, false);
                    Projectile.ai[1] = AttackSwingTime;

                    AttackRotation = (Main.MouseWorld - player.Center).ToRotation() + Main.rand.NextFloat(-0.08f, 0.08f);

                    modPlayer.GrinderMk2AttackCooldown = 60 * 2;
                    modPlayer.GrinderMk2AttackRandom = Main.rand.Next(4);
                }
                else if (player.altFunctionUse == 2 && heldItem.type == ModContent.ItemType<Items.Ruina.Technology.GrinderMk52R>() && player.itemAnimation > 0)
                {
                    float playerAnimation = (float)player.itemAnimation / player.itemAnimationMax;
                    /*if ((order == 0 && playerAnimation > 0.75f) ||
                        (order == 1 && playerAnimation < 0.75f && playerAnimation > 0.5f) ||
                        (order == 2 && playerAnimation < 0.5f && playerAnimation > 0.25f) ||
                        (order == 3 && playerAnimation < 0.25f))*/
                    if (((order == 0 || order == 2) && playerAnimation > 0.5f) ||
                        ((order == 1 || order == 3) && playerAnimation < 0.5f))
                    {
                        ChangeBatteryValue(-4, false);
                        Projectile.ai[1] = AttackSwingTime;

                        int angle = 90 * (int)order;
                        if (order % 2 != 0)
                            angle += 90;
                        AttackRotation = (Main.MouseWorld - player.Center).ToRotation() + MathHelper.ToRadians(angle);
                        dashPlayer.DashTimer = 0;
                    }
                }

                Projectile.netUpdate = true;
            }

            bool IsDashing = dashPlayer.DashTimer > 0;
            //When the player is slashing
            if (Projectile.ai[1] > 0)
            {
                speed = IsDashing ? 48 : 32;
                int dir = -1;
                if (order % 2 == 0)
                    dir = 1;

                dir *= AttackRotation > -1.57f && AttackRotation < 1.57f ? 1 : -1;

                float progress = (float)Math.Sin(((Projectile.ai[1] - 10) / 20) * 1.57f);
                float rotation = MathHelper.ToRadians(135 - 270 * progress) * dir;

                float X = 68 * (float)Math.Cos(rotation);
                float Y = 34 * (float)Math.Sin(rotation);
                targetPos = player.Center + new Vector2(X, Y).RotatedBy(AttackRotation);

                if (Projectile.ai[1] < 10)
                {
                    progress = Projectile.ai[1] / 10f;
                    rotation = MathHelper.ToRadians(135 + 5 * progress) * dir;

                    X = 68 * (float)Math.Cos(rotation);
                    Y = 34 * (float)Math.Sin(rotation);
                    targetPos = player.Center + new Vector2(X, Y).RotatedBy(AttackRotation);
                }
                else
                {
                    float distance = (20 + 60 * GetOwnerBatteryLife()) * 2;
                    X = (68 + distance) * (float)Math.Cos(rotation);
                    Vector2 extendedHitbox = new Vector2(X, Y).RotatedBy(AttackRotation);

                    if (Projectile.ai[1] < 27 && Projectile.ai[1] > 12)
                    {
                        int d = Dust.NewDust(Projectile.position + extendedHitbox, Projectile.width, Projectile.height, DustID.Blood);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = new Vector2(2, 0).RotatedBy(AttackRotation);
                    }

                    if (Main.rand.NextBool(15))
                    {
                        Vector2 dustSpeed = Projectile.velocity * -1;
                        dustSpeed.Normalize();
                        SpawnTrailDust(Projectile.position + extendedHitbox, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.ElectricTrail>(), dustSpeed, 0.5f, 5);
                    }
                }

                Projectile.ai[1]--;
            }
            
            //When the player is dashing
            else if (IsDashing)
            {
                Vector2 offset = new Vector2(45 * dirX, 45 * dirY).RotatedBy(MathHelper.ToRadians(dashPlayer.DashTimer * 22 * -player.direction));
                targetPos = ownerMountedCenter + offset;
                speed = 48;
                ChangeBatteryValue(-2);
                player.immune = true;
                player.immuneNoBlink = true;
                player.immuneTime = 5;
                Projectile.usesLocalNPCImmunity = true;

                for (int i = 0; i < 4; i++)
                {
                    int d = Dust.NewDust(Projectile.position + HitboxExtension, Projectile.width, Projectile.height, DustID.Blood);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.25f;
                }

                if (Collision.SolidTiles(Projectile.position, Projectile.width,Projectile.height) &&
                    Main.rand.Next(5) == 0)
                {
                    SpawnTrailDust(Projectile.position + HitboxExtension, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.ElectricTrail>(), Vector2.Zero, 1f, 10);
                }
            }
            //If default, normal knife positioning
            else
            {
                Projectile.usesLocalNPCImmunity = false;
            }

            //Move Blades towards TargetPosition
            Vector2 delta = targetPos - ProjectileRoot;
            if (delta.Length() > speed)
            {
                Projectile.velocity = new Vector2(speed, 0).RotatedBy(delta.ToRotation());
            }
            else
                Projectile.velocity = delta;
            Projectile.Center += Projectile.velocity;
            if (speed == 16)
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            delta = ProjectileRoot - ownerMountedCenter;
            
            //If the Blade is too far from its allowed length from the player, it goes near
            if (delta.Length() > 68)
            {
                Projectile.Center = ownerMountedCenter + new Vector2(68, 0).RotatedBy(delta.ToRotation()) - ProjectileRootOffset;
            }
            Projectile.timeLeft = 300;

            //When Charge runs out, Kill Projectile
            ChangeBatteryValue(-1);
            if (modPlayer.GrinderMk2Battery <= 0 || modPlayer.GrinderMk2Recharging)
            {
                Projectile.Kill();
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AttackRotation);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AttackRotation = reader.ReadSingle();
        }

        private void ChangeBatteryValue(int amount, bool firstOnly = true)
        {
            if (firstOnly && order != 0)
                return;

            LobotomyHePlayer modPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyHePlayer>();
            modPlayer.GrinderMk2Battery += amount;
            if (modPlayer.GrinderMk2Battery > modPlayer.GrinderMk2BatteryMax)
                modPlayer.GrinderMk2Battery = modPlayer.GrinderMk2BatteryMax;
        }

        public static int SpawnTrailDust(Vector2 Position, int width, int height, int type, Vector2 velocity, float scale, int amount)
        {
            Dust ParentDust = null;
            int initialDust = -1;
            for (int i = 0; i < amount; i++)
            {
                float dustScale = scale * (1f - (float)i / amount);
                int d = Dust.NewDust(Position, width, height, type, velocity.X, velocity.Y);
                Main.dust[d].scale = dustScale;
                if (ParentDust != null)
                {
                    initialDust = d;
                    Main.dust[d].customData = ParentDust;
                }
                ParentDust = Main.dust[d];
            }
            return initialDust;
        }

        private Vector2 ProjectileElbow(Player player, int dir)
        {
            Vector2 elbow = player.RotatedRelativePoint(player.MountedCenter, true);
            int legLength = 34;
            float Length = Vector2.Distance(ProjectileRoot, elbow);
            if (Length > legLength * 2)
                Length = legLength * 2;
            float Angle = (float)Math.Acos(Length * Length / (2 * legLength * Length));
            float Rotation = (ProjectileRoot - elbow).ToRotation() + Angle * dir;
            elbow = elbow + new Vector2(legLength, 0).RotatedBy(Rotation);
            return elbow;
        }

        private float GetOwnerBatteryLife()
        {
            Player player = Main.player[Projectile.owner];
            LobotomyHePlayer modPlayer = player.GetModPlayer<LobotomyHePlayer>();

            return (float)modPlayer.GrinderMk2Battery / modPlayer.GrinderMk2BatteryMax;
        }

        public override void OnKill(int timeLeft)
        {
            Main.player[Projectile.owner].AddBuff(ModContent.BuffType<Buffs.GrinderMk2Recharge>(), 180);
            LobotomyHePlayer modPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyHePlayer>();
            if (modPlayer.GrinderMk2Recharging == false)
            {
                modPlayer.GrinderMk2Dash = 0;
                modPlayer.GrinderMk2Active = false;
                modPlayer.GrinderMk2Recharging = true;
                Main.player[Projectile.owner].velocity *= 0;

                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Helper_Down") with { Volume = 0.25f }, Projectile.Center);
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            LobotomyHePlayer modPlayer = player.GetModPlayer<LobotomyHePlayer>();
            ChangeBatteryValue(16, false);

            if (modPlayer.GrinderMk2Dash > 0 || Projectile.ai[1] > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 dustSpeed = Projectile.velocity;
                    dustSpeed.Normalize();
                    int d = Dust.NewDust(target.position, target.width, target.height, DustID.Blood, dustSpeed.X, dustSpeed.Y);
                }

                if (Main.rand.Next(4) == 0)
                {
                    SpawnTrailDust(target.position, target.width, target.height, ModContent.DustType<Misc.Dusts.ElectricTrail>(), Vector2.Zero, 0.5f, 5);
                }
            }

            if (target.life <= 0)
                modPlayer.GrinderMk2Battery = modPlayer.GrinderMk2BatteryMax;
        }
        
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.X += (int)HitboxExtension.X;
            hitbox.Y += (int)HitboxExtension.Y;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            LobotomyHePlayer modPlayer = player.GetModPlayer<LobotomyHePlayer>();
            Vector2 ownerMountedCenter = player.RotatedRelativePoint(player.MountedCenter, true);

            //Elbow Direction
            int dir = 1;
            if (order < 3 && order > 0)
                dir = -1;

            if (Projectile.ai[1] > 0)
            {
                dir = -1;
                if (order % 2 == 0)
                    dir = 1;
            }

            if (modPlayer.GrinderMk2Dash > 0)
                dir = player.direction;

            //7/3, 41
            Texture2D texture = Mod.Assets.Request<Texture2D>("Projectiles/Realized/GrinderMk2Arm").Value;
            Vector2 pos = ownerMountedCenter - Main.screenPosition;
            Vector2 origin = new Vector2(texture.Width / 2, 5);
            Rectangle frame = texture.Frame();
            float rotation = (ProjectileElbow(player, dir) - ownerMountedCenter).ToRotation() - 1.57f;
            Main.EntitySpriteDraw(texture, pos, new Rectangle?(frame), lightColor, rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            pos = ProjectileElbow(player, dir) - Main.screenPosition;
            rotation = (ProjectileRoot - ProjectileElbow(player, dir)).ToRotation() - 1.57f;
            Main.EntitySpriteDraw(texture, pos, new Rectangle?(frame), lightColor, rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            texture = TextureAssets.Projectile[Projectile.type].Value;
            if (order < 3 && order > 0)
                texture = Mod.Assets.Request<Texture2D>("Projectiles/Realized/GrinderMk2Cleaner").Value;

            pos = Projectile.Center - Main.screenPosition;
            origin = texture.Size() / 2;
            frame = texture.Frame();
            Main.EntitySpriteDraw(texture, pos, new Rectangle?(frame), lightColor, Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            if (Projectile.ai[1] > 0)
            {
                dir *= AttackRotation > -1.57f && AttackRotation < 1.57f ? 1 : -1;

                float progress = (float)Math.Sin(((Projectile.ai[1] - 10) / 20) * 1.57f);
                rotation = MathHelper.ToRadians(135 - 270 * progress) * dir;

                float X = 68 * 2;
                float Y = 34 * 2;

                if (Projectile.ai[1] < 10)
                {
                    progress = Projectile.ai[1] / 10f;
                    rotation = MathHelper.ToRadians(135 + 5 * progress) * dir;
                }
                else
                {
                    float prog = ((Projectile.ai[1] - 10) / 20);

                    CustomShaderData shader = LobotomyCorp.LobcorpShaders["TextureTrail"].UseOpacity(0.5f + 0.5f * prog);
                    shader.UseImage1(Mod, "Misc/BloodTrail");
                    shader.UseImage2(Mod, "Misc/BloodTrail");

                    //Im a fucking idiot wtf is this code, too diferent than circle >:V

                    float extraTileDist = 20 + 60 * GetOwnerBatteryLife();

                    float XOFfset = -12 + extraTileDist;// * ((float)Math.Sin(prog * 3.14f));
                    float radiusX = 36 + XOFfset * 2;
                    SlashTrail trail = new SlashTrail(radiusX, 12 + 12 * prog, 0);
                    trail.color = Color.DarkRed;// * (float)Math.Sin(prog * 3.14f);
                    //trail.DrawEllipse(player.Center, AttackRotation, rotationOffset + rotation, dir * -1, X - 12 + 28 * (1f - prog), Y, 128, shader);
                    trail.DrawPartEllipse(player.Center, AttackRotation, rotation, 1.5f * (float)Math.Sin(progress * 3.14f), dir, X - 36 + radiusX + XOFfset, Y, 128, shader);
                }
            }

            if (order > 0)
                return false;

            texture = Mod.Assets.Request<Texture2D>("Projectiles/Realized/GrinderMk2Battery").Value;
            ownerMountedCenter.Y -= 48;
            pos = ownerMountedCenter - Main.screenPosition;
            origin = texture.Size() / 2;
            frame = new Rectangle(0, 0, texture.Width, texture.Height);
            lightColor = Lighting.GetColor((int)ownerMountedCenter.X / 16, (int)ownerMountedCenter.Y / 16);
            Main.EntitySpriteDraw(texture, pos, new Rectangle?(frame), lightColor, 0, origin, Projectile.scale, SpriteEffects.None, 0);

            texture = Mod.Assets.Request<Texture2D>("Projectiles/Realized/GrinderMk2Bar").Value;
            LobotomyHePlayer ModPlayer = player.GetModPlayer<LobotomyHePlayer>();
            int frameY = (int)((ModPlayer.GrinderMk2BatteryMax - (float)ModPlayer.GrinderMk2Battery) / ((float)ModPlayer.GrinderMk2BatteryMax / 5)) * texture.Height / 6;
            if (ModPlayer.GrinderMk2Battery < ModPlayer.GrinderMk2BatteryMax / 5 && ModPlayer.GrinderMk2Battery % 30 < 15)
                frameY += texture.Height / 6;
            frame = new Rectangle(0, frameY, texture.Width, texture.Height / 6);
            Main.EntitySpriteDraw(texture, pos, new Rectangle?(frame), Color.White, 0, origin, Projectile.scale, SpriteEffects.None, 0);

            /*TrailingShader Trail = default(TrailingShader);
            Trail.ColorStart = Color.Blue;
            Trail.ColorEnd = Color.Blue;
            Trail.Draw(Projectile);*/

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[1] > 10)
            {
                float distance = (20 + 60 * GetOwnerBatteryLife()) * 3;
                Vector2 extendedHitbox = new Vector2(distance, 0).RotatedBy(AttackRotation);
                if (Collision.CheckAABBvLineCollision2(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + extendedHitbox))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
    }

}

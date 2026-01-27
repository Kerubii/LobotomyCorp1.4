using LobotomyCorp.Buffs;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;

namespace LobotomyCorp.Projectiles
{
    public class SmileSpecial : ModProjectile
    {
        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults() {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1.3f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 600;

            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.extraUpdates = 2;
        }

        //private Vector2 PreviousPosition;

        public override void AI() {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;

            /*if (Projectile.ai[1] == 0)
            {
                PreviousPosition = projOwner.position;
                Projectile.ai[1] = 1;
            }*/

            int dir = Projectile.spriteDirection;

            if (Projectile.ai[0] == 0) //Ready Swing
            {
                Projectile.scale = 1f;
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 8)
                {
                    //projOwner.velocity.Y -= 6f;
                    Projectile.ai[0]++;
                    Projectile.ai[1] = 0;
                }
                Projectile.rotation = MathHelper.ToRadians(dir == 1 ? 45 : 135);
            }
            else if (Projectile.ai[0] == 1) //Raise up
            {
                if (Projectile.scale < 1f)
                    Projectile.scale += 0.05f;
                if ((dir ==  1 && Projectile.rotation > MathHelper.ToRadians(-140)) ||
                    (dir == -1 && Projectile.rotation < MathHelper.ToRadians(320)))
                {
                    Projectile.rotation -= MathHelper.ToRadians(4) * dir;
                }
                else
                {
                    Projectile.scale = 1.6f;
                    //projOwner.velocity.Y = 6f;
                    Projectile.ai[0]++;
                }
            }
            else if (Projectile.ai[0] == 2) //Swing Downwards
            {
                for (int i = 0; i < 3; i++)
                {
                    Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith)].noGravity = true;
                }

                if ((dir ==  1 && Projectile.rotation < MathHelper.ToRadians(135)) ||
                    (dir == -1 && Projectile.rotation > MathHelper.ToRadians(45)))
                {
                    Projectile.rotation += MathHelper.ToRadians(6) * dir;
                }
                else
                {
                    Projectile.ai[0] += 2;
                }
                Vector2 nextPos = ownerMountedCenter + 110 * (new Vector2(1, 0).RotatedBy(Projectile.rotation + MathHelper.ToRadians(6 * dir)));
                if (Projectile.rotation > MathHelper.ToRadians(-80) && TileCollision(new Rectangle((int)nextPos.X - Projectile.width / 2, (int)nextPos.Y - Projectile.height / 2,Projectile.width, Projectile.height)))
                {
                    Vector2 vel = new Vector2(16, 0).RotatedBy(Projectile.rotation + 1.57f * dir);
                    Collision.HitTiles(Projectile.position, vel, Projectile.width, Projectile.height);
                    Projectile.ai[0]++;
                }
            }
            else if (Projectile.ai[0] == 3) //Hit a tile/Hit an enemy cause a shockwave
            {
                if (Projectile.ai[1] == 0 && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(20, 0).RotatedBy(Projectile.rotation + 1.57f * dir), Vector2.Zero, ModContent.ProjectileType<SmileShockwave>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.rotation);
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Danggo_Lv3_Atk") with { Volume = 0.3f }, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(100 * projOwner.direction, 0), ModContent.ProjectileType<SmileScream>(), Projectile.damage / 3, 0.1f, Projectile.owner, 0, 5, 1);

                }
                Projectile.ai[1]++;
                if (Projectile.ai[1] > projOwner.itemAnimationMax * 1.5f)
                {
                    Projectile.Kill();
                    projOwner.itemAnimation = 0;
                }
            }
            else if (Projectile.ai[0] == 4) //Failure
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] >= projOwner.itemAnimationMax)
                {
                    Projectile.Kill();
                    projOwner.itemAnimation = 0;
                }
            }


            Vector2 velRot = new Vector2(1, 0).RotatedBy(Projectile.rotation);
            projOwner.itemRotation = (float)Math.Atan2(velRot.Y * Projectile.direction, velRot.X * Projectile.direction);
            projOwner.direction = Projectile.spriteDirection;
            Projectile.Center = ownerMountedCenter + 90 * velRot;

            projOwner.itemTime = 2;
            projOwner.itemAnimation = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 2)
            {
                Projectile.ai[0]++;
            }
        }

        private bool TileCollision(Rectangle hitbox)
        {
            for (int x = (int)(hitbox.X / 16); x < (int)((hitbox.X + hitbox.Width) / 16); x++)
            {
                for (int y = (int)(hitbox.Y / 16); y < (int)((hitbox.Y + hitbox.Height) / 16); y++)
                {
                    Tile t = Main.tile[x, y];
                    if (t.HasTile && (Main.tileSolid[t.TileType] || Main.tileSolidTop[t.TileType]))
                        return true;
                }
            }
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] == 2;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            //Dust.NewDustPerfect(ownerMountedCenter, 14, Vector2.Zero);
            Vector2 position = ownerMountedCenter - Main.screenPosition;
            Vector2 originOffset = new Vector2(12, 0).RotatedBy(MathHelper.ToRadians(Projectile.direction == 1 ? 135 : 45));
            Vector2 origin = new Vector2((Projectile.spriteDirection == 1 ? 9 : 74), 74) + originOffset;
            float rotation = Projectile.rotation + MathHelper.ToRadians(Projectile.spriteDirection == 1 ? 45 : 135);
            SpriteEffects spriteEffect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, new Microsoft.Xna.Framework.Rectangle?
                                    (
                                        new Rectangle
                                        (
                                            0, 0, TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height()
                                        )
                                    ),
                lightColor * ((float)(255 - Projectile.alpha) / 255f), rotation, origin, Projectile.scale, spriteEffect, 0);
            /*
            SlashTrail trail = new SlashTrail(80, 1.57f);
            trail.DrawTrail(Projectile, LobcorpShaders["TwilightSlash"]);*/

            return false;
        }
    }

    class SmileShockwave : ModProjectile
    {
        public static Asset<Texture2D> SmileShockwaveTex;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                SmileShockwaveTex = Mod.Assets.Request<Texture2D>("Projectiles/SmileShockwave", AssetRequestMode.ImmediateLoad);

                Main.QueueMainThreadAction(() =>
                {
                    LobotomyCorp.PremultiplyTexture(SmileShockwaveTex.Value);
                });
            }
        }

        public override void Unload()
        {
            SmileShockwaveTex = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 15;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.ai[0];
            Projectile.scale = 1f - (Projectile.timeLeft / 15f);// (float)Math.Sin(1.57f * (1f - (Projectile.timeLeft / 15f)));
            Projectile.alpha = 255 - (int)(255 * (1f - (Projectile.timeLeft / 15f)));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float scaleFactor = (float)Math.Sin(1.57f * (Projectile.scale));
            Vector2 lineStart = projHitbox.Center() - new Vector2(200 * scaleFactor, 0).RotatedBy(Projectile.rotation);
            Vector2 lineEnd = projHitbox.Center() + new Vector2(200 * scaleFactor, 0).RotatedBy(Projectile.rotation);
            float e = 0;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), lineStart, lineEnd, 20, ref e))
                return true;
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = SmileShockwaveTex.Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = tex.Size() / 2;
            
            if (Projectile.scale < 0.85f)
            {
                float scaleFactor = (float)Math.Sin(1.57f * (Projectile.scale / 0.85f));
                Vector2 scale = new Vector2(6 * scaleFactor, scaleFactor) * 0.8f;
                Color color = Color.White;
                color *= 1f - scaleFactor;
                Main.EntitySpriteDraw(tex, position, tex.Frame(), color, Projectile.rotation, origin, scale, 0, 0);
            }
            if (Projectile.scale > 0.25f)
            {
                float scaleFactor = (float)Math.Sin(1.57f * ((Projectile.scale - 0.15f) / 0.85f));
                Vector2 scale = new Vector2(6 * scaleFactor, scaleFactor) * 0.8f;
                Color color = Color.White;
                color *= 1f - scaleFactor;
                Main.EntitySpriteDraw(tex, position, tex.Frame(), color, Projectile.rotation, origin, scale, 0, 0);
            }
            return false;
        }
    }

    class SmileScream : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/SmileShockwave";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 15;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;

            Projectile.ArmorPenetration = 20;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(3.15f);
                Projectile.localAI[0] = 1 + Main.rand.Next(3);
            }

            if (Projectile.ai[2] == 1)
            {
                if (Projectile.ai[0] == 5 && Main.myPlayer == Projectile.owner && Projectile.ai[1] > 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.ai[1] - 1, 1);

                    float random = Main.rand.NextFloat(1.00f);
                    for (int i = 0; i < 28; i++)
                    {
                        Vector2 vel = new Vector2(16, 0).RotatedBy(random + MathHelper.ToRadians((360 / 28f) * i));
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Wraith, vel);
                        d.noGravity = true;
                    }
                }
            }
            else
            {
                if (player.itemAnimation > player.itemAnimationMax * 0.6f)
                {
                    //Projectile.velocity = new Vector2(100, 0).RotatedBy(player.itemRotation - MathHelper.ToRadians(45) + (player.direction < 0 ? -1.57f : 0));
                    //Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity;

                    if (Projectile.ai[0] == 5 && Main.myPlayer == Projectile.owner)
                    {
                        Vector2 offset = new Vector2(120, 0).RotatedBy(player.itemRotation - MathHelper.ToRadians(45) + (player.direction < 0 ? -1.57f : 0));
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center + offset, offset, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner);

                        float random = Main.rand.NextFloat(1.00f);
                        for (int i = 0; i < 28; i++)
                        {
                            Vector2 vel = new Vector2(16, 0).RotatedBy(random + MathHelper.ToRadians((360 / 28f) * i));
                            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Wraith, vel);
                            d.noGravity = true;
                        }
                    }
                }
            }            

            Projectile.scale = Projectile.ai[2] - Projectile.ai[2] * (Projectile.timeLeft / 15f);// (float)Math.Sin(1.57f * (1f - (Projectile.timeLeft / 15f)));
            Projectile.alpha = 255 - (int)(255 * (1f - (Projectile.timeLeft / 15f)));
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int hitboxSize = (int)(300 * Projectile.scale);
            hitbox.X += hitbox.Width / 2 - hitboxSize / 2;
            hitbox.Y += hitbox.Height / 2 - hitboxSize / 2;
            hitbox.Width = hitboxSize;
            hitbox.Height = hitboxSize;
            base.ModifyDamageHitbox(ref hitbox);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Scream>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = SmileShockwave.SmileShockwaveTex.Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = tex.Size() / 2;

            float scaleFactor = (float)Math.Sin(1.57f * ((Projectile.scale / Projectile.ai[2]) / 0.85f));
            Vector2 scale = new Vector2(scaleFactor, scaleFactor) * 3f;
            switch (Projectile.localAI[0])
            {
                case 1:
                    scale.X *= 0.6f;
                    break;
                case 2:
                    scale.Y *= 0.6f;
                    break;
            }

            Color color = Color.Black;
            color *= 1f - scaleFactor;
            Main.EntitySpriteDraw(tex, position, tex.Frame(), color, Projectile.rotation, origin, scale * Projectile.ai[2], 0, 0);            

            return false;
        }
    }
}

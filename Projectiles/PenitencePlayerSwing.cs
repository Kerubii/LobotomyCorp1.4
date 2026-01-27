using System;
using LobotomyCorp.Items.Zayin;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class PenitencePlayerSwing : ModProjectile
	{
        public override string Texture => "Terraria/Images/Sun";

        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 60;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.dead || owner.itemAnimation == 0 || owner.HeldItem.type != ModContent.ItemType<Penitence>())
                Projectile.Kill();
            float adjustedScale = owner.GetAdjustedItemScale(owner.HeldItem);
            Projectile.scale = adjustedScale;
            Projectile.rotation = owner.itemRotation + (owner.direction > 0 ? -1.57f : 3.14f);
            Projectile.Center = owner.itemLocation + new Vector2(40 * adjustedScale).RotatedBy(Projectile.rotation);

            float prog = (float)Math.Sin(3.14f * (float)owner.itemAnimation / owner.itemAnimationMax);
            Projectile.scale = 1f + 1f * prog;
            Projectile.alpha = (int)(255 * (1 - prog));

            float range = 114 / 2 * Projectile.scale * 2f;
            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] <= 8)
            {
                foreach (NPC n in Main.npc)
                {
                    if (Projectile.ai[0] > 8)
                        break;

                    if (n.active && n.life > 0 && !n.friendly && (n.Center - Projectile.Center).LengthSquared() < range * range && Projectile.localNPCImmunity[n.whoAmI] == 0)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.Next(n.width), Main.rand.Next(n.height)), ModContent.ProjectileType<PenitencePlayerLight>(), Projectile.damage, 0, Projectile.owner, n.whoAmI);
                        Projectile.localNPCImmunity[n.whoAmI] = Projectile.localNPCHitCooldown;
                        Projectile.ai[0]++;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 origin = tex.Size() / 2;
            Rectangle frame = tex.Frame();
            float alpha = 1f - (Projectile.alpha / 255f);
            
            Main.EntitySpriteDraw(tex, pos, frame, Color.White * 0.5f * alpha, 0, origin, Projectile.scale * 1.5f, 0, 0);

            tex = TextureAssets.Projectile[927].Value;
            origin = tex.Size() / 2;
            DrawStar(tex, pos, Color.White * 0.4f * alpha, 0.785f, origin, Projectile.scale * 4);
            DrawStar(tex, pos, Color.White * 0.4f * alpha, 0, origin, Projectile.scale * 6);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        private void DrawStar(Texture2D tex, Vector2 pos, Color color, float rotation, Vector2 origin, float scaleBase)
        {
            Vector2 scale = new Vector2(scaleBase, 1f);
            Main.EntitySpriteDraw(tex, pos, tex.Frame(), color, rotation, origin, scale * 0.6f, 0, 0);
            Main.EntitySpriteDraw(tex, pos, tex.Frame(), color, rotation + 1.57f, origin, scale * 0.6f, 0, 0);
        }
    }

    public class PenitencePlayerLight : ModProjectile
    {
        public static Asset<Texture2D> Light;

        public override void Load()
        {
            Light = Mod.Assets.Request<Texture2D>("Projectiles/PenitenceLight");
        }

        public override string Texture => "Terraria/Images/Projectile_927";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 120;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.timeLeft <= 60)
            {
                if (Projectile.ai[1] == 0)
                {
                    if (Projectile.ai[0] >= 0)
                        Projectile.Center = Main.npc[(int)Projectile.ai[0]].position + Projectile.velocity;

                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 vel = new Vector2(2, 0).RotatedBy(MathHelper.ToRadians(360 / 8 * i));
                        Dust d = Dust.NewDustPerfect(Projectile.Center, 20, vel);
                        d.noGravity = true;
                    }
                    for (int i = -3; i < 0; i++)
                    {
                        Vector2 pos = Projectile.Center + Vector2.UnitY * (401 / 3f) * i;

                        for (int j = 0; j < 16; j++)
                        {
                            float angle = 6.28f * j / 16f;
                            Vector2 vel = new Vector2(4 * (float)Math.Cos(angle), 1 * (float)Math.Sin(angle));

                            Dust d = Dust.NewDustPerfect(pos, 20, vel);
                            d.noGravity = true;
                        }
                    }
                    Projectile.rotation = 1.57f - 0.015f + Main.rand.NextFloat(0.03f);
                }

                Projectile.ai[1]++;
            }

            if (Projectile.ai[1] == 0)
            {
                Projectile.rotation += MathHelper.ToRadians(4);
                Projectile.localAI[0]++;
            }
            if (Projectile.ai[0] < 0 || Projectile.ai[1] != 0)
                return;

            NPC n = Main.npc[(int)Projectile.ai[0]];
            if (!n.active || n.life <= 0)
            {
                Projectile.ai[0] = -1;
            }
            if (Main.rand.NextBool(5))
            {
                Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 20)];
                d.noGravity = true;
            }

            Projectile.Center = Vector2.Lerp(Projectile.Center, n.position + Projectile.velocity, 0.15f);//(n.Center + Projectile.velocity - Projectile.Center) / 10;
            //Projectile.localAI[1] = (n.width >= n.height ? n.width : n.height) * 4 / 100;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value; 
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 origin = tex.Size() / 2;
            Rectangle frame = tex.Frame();
            if (Projectile.ai[1] == 0)
            {
                float scale = 0.8f;
                float opacity = 1f;
                if (Projectile.timeLeft < 75)
                {
                    if (Projectile.ai[0] < 0)
                    {
                        scale -= 1f - (Projectile.timeLeft - 60) / 15f;
                        opacity *= (Projectile.timeLeft - 60) / 15f;
                    }
                    else
                    {
                        scale += 1f - (Projectile.timeLeft - 60) / 15f;
                        opacity *= (Projectile.timeLeft - 60) / 15f;
                    }
                }

                float starscale = (1f + 0.1f * (float)Math.Sin(3.14f * Projectile.localAI[0] / 20)) * scale;
                drawStar(tex, pos, Color.Yellow * opacity, 0, origin, starscale);

                starscale = 1f * scale;
                drawStar(tex, pos, Color.White * 0.4f, Projectile.rotation, origin, starscale);
                drawStar(tex, pos, Color.White * 0.4f, -Projectile.rotation, origin, starscale);
            }
            else
            {
                tex = Light.Value;
                frame = tex.Frame(1, 2);
                origin = frame.Size() / 2;
                origin.X = frame.Width - 40;

                float scalethick = 1f;
                float alpha = 0.7f;
                if (Projectile.ai[1] < 20)
                {
                    scalethick = (float)Math.Sin(1.57f * Projectile.ai[1] / 20f);
                }
                else if (Projectile.ai[1] > 25)
                {
                    alpha *= 1f - (Projectile.ai[1] - 25f) / 5f;
                }
                Vector2 scale = new Vector2(0.5f, scalethick);
                Color color = Color.LightYellow;
                //color.A = 100;
                //Main.EntitySpriteDraw(tex, pos, frame, color * alpha, Projectile.rotation, origin, scale, 0, 0);

                frame = tex.Frame(1, 2, 0, 1);
                for (int i = 4; i >= 0; i--)
                {
                    float prog = 1f - (i / 5f);
                    scale.Y = scalethick * (prog);
                    color = Color.Lerp(Color.White, Color.LightYellow, prog) * 0.9f * (1f - prog);
                    Main.EntitySpriteDraw(tex, pos, frame, color * alpha, Projectile.rotation, origin, scale, 0, 0);
                }
            }
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int LENGTH = 300;
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - Vector2.UnitY * LENGTH / 2,
                Projectile.Center, 16, ref point);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[1] == 0 || target.whoAmI != (int)Projectile.ai[0])
                return false;
            return base.CanHitNPC(target);
        }

        private void drawStar(Texture2D tex, Vector2 pos, Color color, float rotation, Vector2 origin, float scaleBase)
        {
            Vector2 scale = new Vector2(scaleBase, 1f);
            Main.EntitySpriteDraw(tex, pos, tex.Frame(), color, rotation, origin, scale * 0.6f, 0, 0);
            Main.EntitySpriteDraw(tex, pos, tex.Frame(), color, rotation + 1.57f, origin, scale * 0.6f, 0, 0);
        }
    }
}

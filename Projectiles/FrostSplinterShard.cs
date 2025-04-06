using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.ValueContentAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	class FrostSplinterShard : ModProjectile
	{
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 2;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        private float State { get { return Projectile.ai[0]; } set { Projectile.ai[0] = value; } }
        private float Target { get { return Projectile.ai[1]; } set { Projectile.ai[1] = value; } }
        private float Timer { get { return Projectile.ai[2]; } set { Projectile.ai[2] = value; } }

        public override void AI()
        {
            // After Projectile hits something, stick to em
            if (State == 1) // Stick state
            {
                // Delete when sticking target is dead
                NPC n = Main.npc[(int)Target];
                if (!n.active && n.life <= 0)
                {
                    Projectile.Kill();
                    return;
                }

                // Stick to target
                Projectile.Center = Main.npc[(int)Target].Center + Projectile.velocity;

                // Check if target has has multiple sticked shards
                // Second kiss activates slow downs
                // Third kiss ruptures all shards,
                int Count = 0;
                foreach (Projectile p in Main.projectile)
                {
                    // Check if the Projectile is another shard and has the same target as this
                    // Does not check for owner or state, so others can contribute and it switching to explode state trigers near simultaneously for all of them
                    if (p.active && p.type == Projectile.type && p.ai[1] == Target && p.ai[0] > 0 && p.ai[0] < 3)
                    {
                        Count++;
                        if (Count > 2)
                            break;
                    }
                }
                if (Count > 2)
                {
                    State++; // EXPLODE
                }
                else if (Count > 1)
                {
                    Timer = 5 + 5 * (float)Math.Sin(6.28f * Main.timeForVisualEffects / 60f);
                }
            }
            else if (State == 2) // Ready to Explode, Intemediary State, Used for Checks
            {
                Timer = 0;
                State++;
            }
            else if (State == 3) // Explode State
            {
                NPC n = Main.npc[(int)Target];
                if (!n.active && n.life <= 0)
                {
                    State = 1;
                    Projectile.Kill();
                    return;
                }

                Timer++;

                Projectile.Center = Main.npc[(int)Target].Center + Projectile.velocity;
                if (Timer >= 30) // After 30 seconds, apply damage
                {
                    if (Main.player[Projectile.owner].CanNPCBeHitByPlayerOrPlayerProjectile(Main.npc[(int)Target], Projectile))
                        Main.player[Projectile.owner].ApplyDamageToNPC(Main.npc[(int)Target], Projectile.damage, 0, 1, Main.rand.Next(100) < Projectile.CritChance, Projectile.DamageType, true);

                    Projectile.Kill();
                }
            }
            else
            {
                if (Main.rand.NextBool(3))
                {
                    int i = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice);
                    Main.dust[i].noGravity = true;
                }

                Target = -1;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (State < 2)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 vel = new Vector2(8, 0).RotatedBy(Projectile.rotation);

                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, vel.X, vel.Y);
                    Main.dust[d].noGravity = true;
                }
            }
            else
            {
                for (int i = -6; i <= 6; i++)
                {
                    Vector2 arrowPos = Projectile.Center + new Vector2( Projectile.width / 2 - (Math.Abs(i) * 4), i).RotatedBy(Projectile.velocity.ToRotation());
                    Vector2 vel = new Vector2(8, 0).RotatedBy(Projectile.rotation);

                    Dust d = Dust.NewDustPerfect(arrowPos, DustID.IceRod, vel);
                    d.noGravity = true;

                    if (i > -2 && i < 2)
                    {
                        vel *= -1;

                        arrowPos = Projectile.Center + new Vector2(-Projectile.width / 2 + (Math.Abs(i)), i * 0.5f).RotatedBy(Projectile.velocity.ToRotation());
                        d = Dust.NewDustPerfect(arrowPos, DustID.IceRod, vel);
                        d.noGravity = true;
                    }
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.penetrate > 1)
                return null;
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            if (State > 0)
                return false;
            return base.ShouldUpdatePosition();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage /= 4;
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            State = 1;
            Projectile.netUpdate = true;
            Projectile.timeLeft = 3600;
            target.immune[Projectile.owner] = 15;

            // Record the position between this Projectile and npc
            Projectile.velocity = Projectile.Center - target.Center;
            Target = target.whoAmI;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;

            Color color = lightColor;
            if (Timer > 0)
            {
                Color white = Color.White;
                white *= 0.6f;
                white.A = (byte)(white.A * 0.2f);
                color = Color.Lerp(color, white, (Timer / 30f));
            }
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, tex.Frame(), color, Projectile.rotation + 1.57f, new Vector2(tex.Width / 2, Projectile.height / 2), Projectile.scale, Projectile.spriteDirection > 0 ? 0 : SpriteEffects.FlipVertically) ;

            return false;
        }
    }
}

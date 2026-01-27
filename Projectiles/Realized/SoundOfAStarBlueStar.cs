using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using Terraria.GameContent;
using System.Collections.Generic;
using Terraria.Audio;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Players;
using LobotomyCorp.Buffs;
using System.IO;
using System.Diagnostics.Eventing.Reader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Graphics.Effects;
using LobotomyCorp.Visuals.DeathAnimations;
using ReLogic.Content;
using Terraria.Graphics.Renderers;
using LobotomyCorp.Visuals.PrimEffects;

namespace LobotomyCorp.Projectiles.Realized
{
	public class SoundOfAStarBlueStar : LobProjectile
	{
        public static Asset<Texture2D> Legs1;
        public static Asset<Texture2D> Legs2;
        public static Asset<Texture2D> Legs3;
        public static Texture2D Glow;

        public override void Load()
        {
            Legs1 = ModContent.Request<Texture2D>(Texture + "Legs1");
            Legs2 = ModContent.Request<Texture2D>(Texture + "Legs2");
            Legs3 = ModContent.Request<Texture2D>(Texture + "Legs3");

            if (Main.netMode != NetmodeID.Server)
            {
                Glow = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.ImmediateLoad).Value;

                Main.QueueMainThreadAction(() =>
                {
                    LobotomyCorp.PremultiplyTexture(Glow);
                });
            }
        }

        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
		}

        public override void SetDefaults()
        {
			Projectile.width = 186;
			Projectile.height = 186;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = false;

            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;

            Projectile.hide = true;

            LobotomyGlobalProjectile.SetDeathAnimation(Projectile, ModContent.GetInstance<LobDeathBlueStar>().Type);
        }

        private float state
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }

        private float timer
        {
            get { return Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }

        private float power
        {
            get { return Projectile.ai[2]; }
            set { Projectile.ai[2] = value; }
        }

        public override void AI()
        {
            if (!CheckActive(Main.player[Projectile.owner]))
                return;

            Projectile.localAI[0]++;

            // Idle State
            if (state == 0)
            {
                if (timer == 0)
                {
                    timer = 60 * 5 + Main.rand.Next(60 * 2);
                }
                Projectile.frame = 0;
                timer--;

                if (timer <= 1)
                {
                    timer = 60;
                    state++;
                }
            }
            // Ping preperation
            else if (state == 1)
            {
                timer--;
                Projectile.frame = 1;
                if (timer <= 0)
                {
                    timer = 0;
                    state++;

                    // If Power is enough, also hits the owner
                    if (power > 0)
                    {
                        Player owner = Main.player[Projectile.owner];
                        if (owner.CanHit(Projectile))
                        {
                            NetworkText text = NetworkText.FromKey("Mods.LobotomyCorp.DeathMessages.SoundStar" + Main.rand.Next(1, 5), owner.name);
                            PlayerDeathReason playerDeath = PlayerDeathReason.ByCustomReason(text);
                            playerDeath.SourceProjectileType = Projectile.type;
                            playerDeath.SourceProjectileLocalIndex = Projectile.whoAmI;
                            owner.Hurt(playerDeath, (int)power + 20, 0, dodgeable: false, knockback: 0);
                            owner.AddBuff(ModContent.BuffType<SoundOfAStarNostalgic>(), 60 * 5);
                        }
                    }
                }                
            }
            // Bing
            else if (state == 2)
            {
                Projectile.netUpdate = true;
                Projectile.frame = 2;
                state++;
                timer = 30;

                LobCustomDraw.Instance().StartShockwave(Projectile.Center, 2, 3, 30, 60);

                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/BlueStar/BlueStar_Atk") with { Volume = 0.25f }, Projectile.Center);
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/BlueStar/BlueStar_In") with { Volume = 0.25f }, Projectile.Center);

                for (int i = 0; i < 180; i++)
                {
                    Vector2 speed = new Vector2(30, 0).RotatedBy(MathHelper.ToRadians(i * 2));
                    Dust.NewDustPerfect(Projectile.Center, 91, speed).noGravity = true;
                }

                for (int i = 0; i < 3; i++)
                {
                    CreateShineRandom();
                }
            }
            // Bing recovery
            else if (state == 3)
            {
                // Sucks in gore too why not
                foreach (Gore g in Main.gore)
                {
                    if (g.active && Collision.CanHit(g.position, (int)g.Width, (int)g.Height, Projectile.position, Projectile.width, Projectile.height))
                    {
                        g.velocity = g.position.DirectionTo(Projectile.Center) * 12f;
                    }
                }

                timer--;
                Projectile.frame = 2;
                if (timer <= 0)
                {
                    timer = 60 * 5 + Main.rand.Next(60 * 2);
                    state = 0;
                }
            }
            else
            {
                state = 0;
                timer = 300;
            }

            if (Projectile.localAI[1] > 0)
            {
                Projectile.localAI[1] -= 1 / 45f;
                if (Projectile.localAI[1] < 0)
                {
                    Projectile.localAI[1] = 0;
                    Projectile.localAI[2] = 0;
                    return;
                }
                if (Main.netMode != NetmodeID.Server && Filters.Scene["LobotomyCorp:BlueStar"].IsActive())
                {
                    float prog = Projectile.localAI[1];
                    if (prog < 0.5f)
                    {
                        prog /= 0.5f;
                    }
                    else
                    {
                        prog = 1f - (prog - 0.5f) / 0.5f;
                    }
                    prog = 1 - (float)Math.Pow(1 - prog, 3);
                    Filters.Scene["LobotomyCorp:Swirley"].GetShader().UseColor(Projectile.localAI[2], 0, 0);
                    Filters.Scene["LobotomyCorp:Swirley"].GetShader().UseTargetPosition(Projectile.Center);
                    Filters.Scene["LobotomyCorp:Swirley"].GetShader().UseProgress(prog * Projectile.localAI[2] * 4);
                }
            }

            foreach (Gore g in Main.gore)
            {
                if (g.active && new Rectangle((int)g.position.X, (int)g.position.Y, (int)g.Width, (int)g.Height).Intersects(Projectile.getRect()))
                {
                    g.active = false;
                }
            }
        }

        private bool CheckActive(Player owner)
        {
            int bufType = ModContent.BuffType<SoundOfAStarBlueStarBuff>();

            if (owner.dead || !owner.active)
            {
                var death = owner.GetModPlayer<LobotomyDeathPlayer>();
                if (death.deathActive)
                {
                    Projectile.timeLeft = 3;
                    return true;
                }

                owner.ClearBuff(bufType);

                return false;
            }

            if (owner.HasBuff(bufType) && Projectile.timeLeft < 2)
            {
                Projectile.timeLeft = 3;
            }
            return true;
        }

        private void CreateShineRandom()
        {
            Vector2 position = Projectile.position + new Vector2(Main.rand.Next(Projectile.width), Main.rand.Next(Projectile.height));
            Vector2 vel = Projectile.Center.DirectionTo(position);
            BlueStarShine shine = new BlueStarShine(position, 45, 0.7f + Main.rand.NextFloat(0.4f), vel);
            LobCustomDraw.Instance().AddVEffects(shine);
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server)
                Filters.Scene["LobotomyCorp:Swirley"].GetShader().UseProgress(0);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (state == 2)
                return base.CanHitNPC(target);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0)
            {
                power += target.lifeMax / 1000f;

                Projectile.localAI[1] = 1f;
                float distance = (target.Distance(Projectile.Center) + 32) / Main.screenHeight;
                if (distance > Projectile.localAI[2])
                {
                    Projectile.localAI[2] = distance;
                }
                int heal = 100 - (int)power;
                if (heal <= 0)
                    heal = 1;
                Main.player[Projectile.owner].Heal(heal);
                CreateShineRandom();
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += power / 100f;

            if (!target.boss && target.life <= target.lifeMax * 0.2f)
            {
                modifiers.FinalDamage += 1000f;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Collision.CanHit(projHitbox.TopLeft(), projHitbox.Width, projHitbox.Height, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height))
                return true;
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Legs3.Value;
            float offY = 12 * (float)Math.Sin(Projectile.localAI[0] / 120 * 3.14f);
            Rectangle frame = tex.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame);
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * (offY + Projectile.gfxOffY);
            Vector2 origin = new Vector2(95, 79);

            float rotOff = MathHelper.ToRadians(2) * (float)Math.Sin((Projectile.localAI[0] / 314f) * 6.28f);
            Main.EntitySpriteDraw(tex, pos, frame, Color.White, Projectile.rotation + rotOff, origin, Projectile.scale + 0.6f, 0);

            tex = Legs2.Value;
            rotOff = MathHelper.ToRadians(-4) * (float)Math.Sin((Projectile.localAI[0] / 634f) * 6.28f);
            Main.EntitySpriteDraw(tex, pos, frame, Color.White, Projectile.rotation + rotOff, origin, Projectile.scale + 0.6f, 0);

            tex = Legs1.Value;
            rotOff = MathHelper.ToRadians(5) * (float)Math.Sin((Projectile.localAI[0] / 512f) * 6.28f);
            Main.EntitySpriteDraw(tex, pos, frame, Color.White, Projectile.rotation + rotOff, origin, Projectile.scale + 0.6f, 0);

            tex = Glow;
            frame = tex.Frame();
            float plusScale = 0.01f * (float)Math.Sin((Projectile.localAI[0] / 432f) * 6.28f);
            float colorOp = 0.5f + 0.2f * (float)Math.Sin((Projectile.localAI[0] / 432f) * 6.28f);

            Main.EntitySpriteDraw(tex, pos, frame, Color.White * colorOp, Projectile.rotation, origin, Projectile.scale + 0.6f + plusScale, 0);

            tex = TextureAssets.Projectile[Projectile.type].Value;			
            Vector2 scale = new Vector2(1f, 1f);
            if (state == 3)
            {
                float lerp = timer / 15f;
                if (lerp > 1f)
                    lerp = 1f;
                lerp *= lerp * lerp;
                scale *= 1f + lerp;
            }
            else if (state == 1 && timer < 18)
            {
                float time = timer / 18f;
                float sine = 1f - 0.8f * (float)Math.Sin(time * 6.28f);
                float xBounce = sine - 1f;
                if (timer < 9)
                {
                    xBounce = 1f - timer / 9f;
                }
                scale = new Vector2(1f + 1f * xBounce, sine);
            }

            scale *= Projectile.scale + 0.6f + plusScale;

            Main.EntitySpriteDraw(tex, pos, frame, Color.White, Projectile.rotation, origin, scale, 0);

			return false;
        }
    }
}

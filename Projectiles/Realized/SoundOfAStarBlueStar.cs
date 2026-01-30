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
using LobotomyCorp.Visuals.LobEffects;

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
            Projectile.timeLeft = 1500;

            //Projectile.sentry = true;
            //Projectile.minion = true;

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
            {
                if (Projectile.timeLeft >= 999)
                {
                    if (state == 0)
                    {
                        Projectile.timeLeft = (int)timer + 180;
                    }
                    else
                    {
                        Projectile.timeLeft = 180;
                    }
                }
            }

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
                    if (power >= 20)
                    {
                        Player owner = Main.player[Projectile.owner];
                        if (owner.CanHit(Projectile))
                        {
                            NetworkText text = NetworkText.FromKey("Mods.LobotomyCorp.DeathMessages.SoundStar" + Main.rand.Next(1, 5), owner.name);
                            PlayerDeathReason playerDeath = PlayerDeathReason.ByCustomReason(text);
                            playerDeath.SourceProjectileType = Projectile.type;
                            playerDeath.SourceProjectileLocalIndex = Projectile.whoAmI;
                            int damage = (int)power;
                            if (Main.masterMode)
                                damage /= 3;
                            else if (Main.expertMode)
                                damage /= 2;

                            owner.Hurt(playerDeath, damage, 0, dodgeable: false, knockback: 0);
                            owner.AddBuff(ModContent.BuffType<SoundOfAStarNostalgic>(), 60 * 10);
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

                foreach (Projectile minion in Main.ActiveProjectiles)
                {
                    if (minion.owner == Projectile.owner && minion.minion && !SoundOfAStarEGOProjType(minion.type))
                    {
                        if (Collision.CanHit(Projectile, minion))
                        {
                            HealPlayer();

                            minion.Kill();
                            SetSwirlyDistortion(minion);
                            power += minion.minionSlots * 10f;
                        }
                    }
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

                Main.player[Projectile.owner].GetModPlayer<LobotomyAlephPlayer>().SoundOfAStarBlueStarPower = (int)power;

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

        public static bool SoundOfAStarEGOProjType(int type)
        {
            return type == ModContent.ProjectileType<SoundOfAStarBall>() || type == ModContent.ProjectileType<SoundOfAStarMinion>();
        }

        private bool CheckActive(Player owner)
        {
            int bufType = ModContent.BuffType<SoundOfAStarBlueStarBuff>();

            if (owner.dead || !owner.active)
            {
                var death = owner.GetModPlayer<LobotomyDeathPlayer>();
                if (death.deathActive)
                {
                    Projectile.timeLeft = 31;
                    return true;
                }

                owner.ClearBuff(bufType);

                return false;
            }

            if (owner.HasBuff(bufType))
            {
                if (Projectile.timeLeft < 1000)
                    Projectile.timeLeft = 1000;
                return true;
            }
            return false;
        }

        private void CreateShineRandom()
        {
            Vector2 position = Projectile.position + new Vector2(Main.rand.Next(Projectile.width), Main.rand.Next(Projectile.height));
            Vector2 vel = Projectile.Center.DirectionTo(position);
            BlueStarShine shine = new BlueStarShine(position, 45, 0.7f + Main.rand.NextFloat(0.4f), vel);
            LobCustomDraw.Instance().AddVEffects(shine);
        }

        private void IncreasePower(int add)
        {
            power += add;
            int limit = 300;
            if (power > limit)
                power = limit;
        }

        private void HealPlayer()
        {
            if (timer == 30 || timer == 0)
            {
                int heal = 100 - (int)(power * 0.3f);
                if (heal <= 0)
                    heal = 1;
                timer = 31;
                Main.player[Projectile.owner].Heal(heal);
            }
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
                HealPlayer();

                IncreasePower(10);

                SetSwirlyDistortion(target);
                
                CreateShineRandom();
            }
        }

        private void SetSwirlyDistortion(Entity target)
        {
            Projectile.localAI[1] = 1f;
            float distance = (target.Distance(Projectile.Center) + 32) / Main.screenHeight;
            if (distance > Projectile.localAI[2])
                Projectile.localAI[2] = distance;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += power / 100f;
            modifiers.SourceDamage += EmptySlotAmount(Projectile.owner) * 0.9f;

            if (!target.boss && target.life <= target.lifeMax * 0.2f)
            {
                modifiers.FinalDamage += 1000f;
            }
        }

        private int EmptySlotAmount(int whoAmI)
        {
            Player player = Main.player[whoAmI];
            int slots = player.maxTurrets;
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.owner == whoAmI && proj.sentry)
                {
                    slots--;
                }
            }
            slots--;
            if (slots < 0)
                slots = 0;
            return slots;
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

            float projScale = Projectile.scale + 0.6f;
            float timeLeft = Math.Clamp((Projectile.timeLeft - 15) / 15f, 0f, 1f);
            if (timeLeft < 1f)
            {
                timeLeft = 1f - (float)Math.Pow(1f - timeLeft, 3);
            }

            float rotOff = MathHelper.ToRadians(2) * (float)Math.Sin((Projectile.localAI[0] / 314f) * 6.28f);
            Main.EntitySpriteDraw(tex, pos, frame, Color.White, Projectile.rotation + rotOff, origin, projScale * timeLeft, 0);

            tex = Legs2.Value;
            rotOff = MathHelper.ToRadians(-4) * (float)Math.Sin((Projectile.localAI[0] / 634f) * 6.28f);
            Main.EntitySpriteDraw(tex, pos, frame, Color.White, Projectile.rotation + rotOff, origin, projScale * timeLeft, 0);

            tex = Legs1.Value;
            rotOff = MathHelper.ToRadians(5) * (float)Math.Sin((Projectile.localAI[0] / 512f) * 6.28f);
            Main.EntitySpriteDraw(tex, pos, frame, Color.White, Projectile.rotation + rotOff, origin, projScale * timeLeft, 0);

            tex = Glow;
            frame = tex.Frame();
            float plusScale = 0.01f * (float)Math.Sin((Projectile.localAI[0] / 432f) * 6.28f);
            float colorOp = 0.5f + 0.2f * (float)Math.Sin((Projectile.localAI[0] / 432f) * 6.28f);

            timeLeft = Math.Clamp(Projectile.timeLeft / 15f, 0f, 1f);

            Main.EntitySpriteDraw(tex, pos, frame, Color.White * colorOp, Projectile.rotation, origin, (projScale + plusScale) * timeLeft, 0);

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

            Main.EntitySpriteDraw(tex, pos, frame, Color.White, Projectile.rotation, origin, scale * timeLeft, 0);

			return false;
        }
    }
}

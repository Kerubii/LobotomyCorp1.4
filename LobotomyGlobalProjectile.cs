using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles;
using LobotomyCorp.Projectiles.Realized;
using LobotomyCorp.Projectiles.RedMist;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace LobotomyCorp
{
	public class LobotomyGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        //public override bool CloneNewInstances => true;

        public int DeathAnimation = -1;

        public byte Lament = 0;

        public bool BlackSwanReflected = false;
        public bool CrimsonScarRBullet = false;
        public bool CrimsonScarBullet = false;
        public bool HypocrisyArrow = false;
        public bool LampArrow = false;
        public bool SodaSpecial = false;
        public int SolitudeTimer = 0;
        public bool SolitudeSpecial = false;
        public bool LaetitiaBullet = false;

        public static void SetDeathAnimation(Projectile p, int deathAnimation)
        {
            p.GetGlobalProjectile<LobotomyGlobalProjectile>().DeathAnimation = deathAnimation;
        }

        public override bool PreAI(Projectile projectile)
        {
            if (BlackSwanReflected)
            {
                projectile.rotation += 0.12f;
                for (int i = 0; i < 3; i++)
                {
                    int type = Main.rand.Next(2, 4);
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, type);
                    Main.dust[d].velocity *= 0.2f;
                    Main.dust[d].noGravity = true;
                }
                return false;
            }
            return base.PreAI(projectile);
        }

        public override void PostAI(Projectile projectile)
        {
            if (SolitudeSpecial)
            {
                SolitudeTimer++;
                if (Main.myPlayer == projectile.owner && SolitudeTimer % (5 + 5 * projectile.extraUpdates) == 0)
                {
                    Vector2 vel = new Vector2(1, 0).RotatedBy(Main.rand.NextFloat(6.28f));
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, vel, ModContent.ProjectileType<SolitudeSmoke>(), projectile.damage / 3, 0, projectile.owner);
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.owner == Main.myPlayer && Lament > 0)
            {
                if (LobotomyCorp.LamentValid(target, projectile) && target.CanBeChasedBy(projectile))
                {
                    int p = Projectile.NewProjectile(projectile.GetSource_FromThis(), Main.player[projectile.owner].Center, new Vector2(6, 0).RotateRandom(6.28f), ModContent.ProjectileType<Projectiles.Kaleidoscope>(), projectile.damage, projectile.knockBack, projectile.owner, target.whoAmI);
                    Main.projectile[p].localAI[0] = Lament;
                    if (projectile.type != ModContent.ProjectileType<Projectiles.Kaleidoscope>())
                    {
                        for (int i = 0, amount = Main.rand.Next(4, 9); i < amount; i++)
                        {
                            Vector2 vel = new Vector2(Main.rand.NextFloat(2, 5), 0).RotatedByRandom(6.29f);
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, vel, ModContent.ProjectileType<Projectiles.KaleidoscopeEffect>(), 0, 0, projectile.owner, Lament);
                        }
                    }

                    SoundStyle ding = new SoundStyle("LobotomyCorp/Sounds/Item/ButterFlyMan_StongAtk_Black");
                    int dustType = 91;
                    ScreenFilter screenFilter = new Items.Ruina.Technology.SolemnLamentWhite();
                    ScreenFilter screenFilter2 = new Items.Ruina.Technology.SolemnLamentBlack();

                    if (Lament == 1)
                    {
                        ding = new SoundStyle("LobotomyCorp/Sounds/Item/ButterFlyMan_StongAtk_White");
                        dustType = 109;

                        screenFilter = new Items.Ruina.Technology.SolemnLamentBlack();
                        screenFilter2 = new Items.Ruina.Technology.SolemnLamentWhite();
                    }
                    ding.Volume = 0.5f;
                    //SoundEngine.PlaySound(ding, target.Center);

                    if (projectile.owner == Main.myPlayer && !LobCustomDraw.Instance().ContainsFilter(screenFilter2))
                        LobCustomDraw.Instance().AddFilter(screenFilter, 0, false, false);

                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 pos = target.position;
                        pos.X += Main.rand.Next(target.width);
                        pos.Y += Main.rand.Next(target.height);
                        int limit = 16;

                        float speed = Main.rand.NextFloat(8f);
                        for (int a = 0; a < limit; a++)
                        {
                            float angle = (float)a / limit * 6.34f;

                            Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                            Dust d = Dust.NewDustPerfect(pos + velocity * speed, dustType, velocity * 2);
                            d.noGravity = true;
                            d.scale = 0.5f;
                        }
                    }
                }
                // On Kill, Disable Lament
                if (target.life <= 0)
                {
                    Main.player[projectile.owner].AddBuff(ModContent.BuffType<Buffs.Lament>(), 180);
                    Main.player[projectile.owner].GetModPlayer<LobotomyWawPlayer>().SolemnLamentDisable = 1;
                }
            }
        
            if (CrimsonScarBullet)
            {
                Main.player[projectile.owner].GetModPlayer<LobotomyWawPlayer>().CrimsonScarEmpower = 1;
            }

            if (CrimsonScarRBullet)
            {
                target.AddBuff(ModContent.BuffType<Prey>(), 60 * 15);

                Player player = Main.player[projectile.owner];
                LobotomyWawPlayer wawPlayer = player.GetModPlayer<LobotomyWawPlayer>();
                if (wawPlayer.CrimsonScarLowHealthActive && Main.rand.NextBool(3))
                {
                    Vector2 velocity = new Vector2(8 * player.direction, 8).RotatedByRandom(MathHelper.ToRadians(10));
                    Vector2 position = target.Center - velocity * 15 * 4;
                    int amount = 12;
                    for (int i = 0; i < amount; i++)
                    {
                        float rot = 6.28f * (i / amount);
                        Vector2 dustVel = new Vector2(3, 0).RotatedBy(rot);
                        Dust d = Dust.NewDustPerfect(position, DustID.Blood, dustVel);
                        d.noGravity = true;
                    }
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), position, velocity, ModContent.ProjectileType<CrimsonScarRSickle2>(), projectile.damage / 2, 0, projectile.owner, target.whoAmI, 30 * 4);
                }
            }

            if (LaetitiaBullet)
            {
                LaetitiaExplodeGift(target, projectile.damage, projectile.owner);
            }

            if (LampArrow)
            {
                target.AddBuff(BuffID.OnFire3, 4 * 60);
            }

            if (projectile.owner == Main.myPlayer)
            {
                if (HypocrisyArrow)
                {
                    int heal = (int)(damageDone * 0.1f);
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<HypocrisyHeal>(), 0, 0, projectile.owner, projectile.owner, heal);
                }
            }

        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (projectile.owner >= 0 && projectile.owner < Main.maxPlayers)
            {
                LobotomyTethPlayer modPlayer = Main.player[projectile.owner].GetModPlayer<LobotomyTethPlayer>(); 
                if (modPlayer.TodaysExpressionActive)
                    modifiers.FinalDamage *= modPlayer.TodaysExpressionDamage();
            }
            if (Lament > 0 && LobotomyCorp.LamentValid(target, projectile) && target.CanBeChasedBy(projectile))
            {
                modifiers.FinalDamage *= 1.15f;
            }
            if (CrimsonScarRBullet && target.GetGlobalNPC<LobotomyGlobalNPC>().CrimsonScarPrey)
            {
                modifiers.SourceDamage += LobotomyWawPlayer.CrimsonScarPreyBoost;
            }
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (SodaSpecial)
            {
                Projectile.NewProjectile(Main.player[projectile.owner].GetSource_FromThis(), projectile.Center, -oldVelocity, ModContent.ProjectileType<SodaGeyser>(), projectile.damage, projectile.knockBack, projectile.owner);
            }
            return base.OnTileCollide(projectile, oldVelocity);
        }

        public override void ModifyDamageHitbox(Projectile projectile, ref Rectangle hitbox)
        {
            if (BlackSwanReflected)
            {
                float growth = 1.75f;
                hitbox.Width = (int)(hitbox.Width * growth);
                hitbox.X -= hitbox.Width / 4;

                hitbox.Height = (int)(hitbox.Height * growth);
                hitbox.Y -= hitbox.Height / 4;
            }
            base.ModifyDamageHitbox(projectile, ref hitbox);
        }

        public void LaetitiaExplodeGift(NPC target, int damage, int owner, int chance = 4)
        {
            LobotomyGlobalNPC ltarget = target.GetGlobalNPC<LobotomyGlobalNPC>();

            if (ltarget.LaetitiaGiftRM && Main.rand.NextBool(chance))
            {
                LaetitiaExplodeOutcome(target, damage, owner);
            }
            else
                ltarget.LaetitiaGiftRM = true;
        }

        public static void LaetitiaExplodeOutcome(NPC target, int damage, int owner)
        {
            LobotomyGlobalNPC ltarget = target.GetGlobalNPC<LobotomyGlobalNPC>();

            ltarget.LaetitiaGiftRM = false;
            //Spawns Projectile
            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Literature/Laetitia_Friend_Born") with { Volume = 0.2f }, target.Center);
            Projectile.NewProjectile(Main.player[owner].GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<LaetitiaExplosion>(), damage, 0, owner);
            Projectile.NewProjectile(Main.player[owner].GetSource_FromThis(), target.Center, new Vector2(12, 0).RotatedByRandom(6.28f), ModContent.ProjectileType<LaetitiaFriendRocket>(), damage, 0f, owner, target.whoAmI);
        }
    }
}
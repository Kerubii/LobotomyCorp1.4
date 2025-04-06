using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Projectiles;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Players;

namespace LobotomyCorp
{
	public class LobotomyGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        //public override bool CloneNewInstances => true;

        public byte Lament = 0;

        public bool BlackSwanReflected = false;
        public bool CrimsonScarBullet = false;
        public bool HypocrisyArrow = false;

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
    }
}
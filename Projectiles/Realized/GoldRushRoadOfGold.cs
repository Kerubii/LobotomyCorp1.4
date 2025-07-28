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
using System.Collections.Generic;
using LobotomyCorp.Projectiles.KingPortal;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Buffs;
using Steamworks;
using System.Linq;

namespace LobotomyCorp.Projectiles.Realized
{
    public class GoldRushRoadOfGold : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/Realized/GoldRushFlurryR";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;

            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.hide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.dead)
            {
                Projectile.Kill();
                foreach (Projectile p in Main.projectile)
                {
                    if (p.type == ModContent.ProjectileType<RoadOfGoldPlayer>() && p.owner == Projectile.owner && Projectile.timeLeft > 15)
                    {
                        p.timeLeft = 15;
                        return;
                    }
                }    
            }

            LobotomyAlephPlayer modOwner = owner.GetModPlayer<LobotomyAlephPlayer>();

            // Spawns all the nescessary Circles and Targets on each target possible
            if (Projectile.ai[0] == 0)
            {
                // Used to check if the enemy is already targeted by road of gold when trying to find other targets
                int[] rushTargets = { -1, -1, -1, -1, -1};
                bool buffApply = false;
                for (int i = 0; i < 5; i++)
                {
                    int target = modOwner.GoldRushBrilliantBliss[i];
                    if (target > -1)
                    {
                        NPC n = Main.npc[target];
                        int dir = Main.rand.NextBool(2) ? 1 : -1;
                        Vector2 pos = n.Center;
                        Vector2 offset = new Vector2((n.width / 2 + 64) * dir, 0);

                        rushTargets[i] = target;
                        Projectile.ai[0]++;
                        Projectile.ai[2] = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos + offset, offset, ModContent.ProjectileType<RoadOfGoldPlayer>(), 0, 0, Projectile.owner, Projectile.ai[0], modOwner.GoldRushBrilliantBliss[i]);
                        buffApply = true;
                    }
                }
                // If there isn't enough Gold Rush targets, find other nearest ones
                if (Projectile.ai[0] > 0 && Projectile.ai[0] < 5 && Main.myPlayer == Projectile.owner)
                {
                    while (Projectile.ai[0] < 5)
                    {
                        float dist = 1600;
                        int target = -1;
                        foreach (NPC n in Main.npc)
                        {
                            if (n.active && !n.friendly && !n.dontTakeDamage)
                            {
                                float ndist = Main.MouseWorld.Distance(n.Center);
                                if (ndist < dist && !rushTargets.Contains(n.whoAmI))
                                {
                                    dist = ndist;
                                    target = n.whoAmI;
                                }
                            }
                        }
                        if (target > -1)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (rushTargets[i] == -1)
                                {
                                    rushTargets[i] = target;
                                    break;
                                }
                            }

                            NPC n = Main.npc[target];
                            int dir = Main.rand.NextBool(2) ? 1 : -1;
                            Vector2 pos = n.Center;
                            Vector2 offset = new Vector2((n.width / 2 + 64) * dir, 0);

                            Projectile.ai[0]++;
                            Projectile.ai[2] = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos + offset, offset, ModContent.ProjectileType<RoadOfGoldPlayer>(), 0, 0, Projectile.owner, Projectile.ai[0], target);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (Projectile.ai[0] > 0)
                {
                    owner.itemTime = owner.itemAnimation = owner.itemAnimationMax;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center, Vector2.Zero, ModContent.ProjectileType<RoadOfGoldPlayer>(), 0, 0, Projectile.owner, 0, -2);
                    Main.projectile[p].timeLeft = 45;
                    Main.projectile[p].netUpdate = true;
                    Main.projectile[(int)Projectile.ai[2]].ai[2] = 1;
                    Projectile.ai[1] = -30;
                    owner.AddBuff(ModContent.BuffType<Buffs.RoadOfGold>(), 60 * 30);
                    if (buffApply)
                    {
                        owner.AddBuff(ModContent.BuffType<Happiness>(), 900);
                        modOwner.GoldRushHappinessBuff = true;
                        modOwner.GoldRushHappiness = 0;
                    }
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/Greed_StrongAtk_Ready") with { Volume = 0.2f}, owner.Center);
                }
                else
                {
                    Projectile.Kill();
                    return;
                }
            }
            // Delay fisting for a bit
            else if (Projectile.ai[1] < 0)
            {
                Projectile.Center = owner.Center;
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 0)
                {
                    // Leaves behind Projectile to last location to teleport player back
                    Projectile.Center = owner.Center - Vector2.UnitX * (Main.rand.NextBool(2) ? -32 : 32);
                    if (owner.mount.Active)
                    {
                        owner.mount.Dismount(owner);
                    }
                }
            }
            // Fists each enemy sequentially
            else
            {
                float prog = Projectile.ai[1] / 60f;
                Vector2 portalPos = owner.Center;
                int direction = 1;
                Projectile portal = Main.projectile[(int)Projectile.ai[2]];
                if (portal.active || portal.type == ModContent.ProjectileType<RoadOfGoldPlayer>() || portal.owner == Projectile.owner)
                {
                    portalPos = portal.Center;
                    direction = Math.Sign(portal.velocity.X);
                }

                owner.direction = direction;
                if (prog < 0.5f)
                {
                    prog /= 0.5f;
                    owner.Center = portalPos + new Vector2(16 * prog, 0) * direction;
                }
                else if (prog < 0.75f)
                {
                    prog -= 0.5f;
                    prog /= 0.25f;
                    owner.Center = portalPos + new Vector2(16 + 32 * (float)Math.Sin(1.57f * prog), 0) * direction;
                }
                else
                {
                    owner.Center = portalPos + new Vector2(48, 0) * direction;
                }

                owner.immune = true;
                owner.immuneTime = 15;
                owner.immuneNoBlink = true;
                owner.itemTime = owner.itemAnimation = owner.itemAnimationMax;


                // Teleport Initial Effcts
                if (Projectile.ai[1] == 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int d = Dust.NewDust(owner.position, owner.width, owner.height, DustID.GoldCoin, 0, 0, 0);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].fadeIn = 1.2f;
                    }
                }
                // Swing Sound
                else if (Projectile.ai[1] == 15)
                {
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/Greed_StrongAtk") with { Volume = 0.2f}, owner.Center);
                }

                Projectile.ai[1]++;
                if (Projectile.ai[1] > 60)
                {
                    portal.ai[2] = 0;
                    portal.timeLeft = 15;
                    portal.netUpdate = true;

                    Projectile.ai[1] = 0;
                    Projectile.ai[0]--;
                    FindNextPortal();
                    if (Projectile.ai[0] <= 0)
                    {
                        owner.immuneNoBlink = false;
                        foreach (Projectile p in Main.projectile)
                        {
                            if (p.active && p.type == ModContent.ProjectileType<RoadOfGoldPlayer>() && p.owner == Projectile.owner && p.ai[0] == -1)
                            {
                                p.timeLeft = 15;
                                p.netUpdate = true;
                                owner.Center = p.Center;
                            }
                        }
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/Greed_GetPower") with { Volume = 0.2f }, owner.Center);
                        owner.velocity *= 0;
                        Projectile.Kill();
                        if (Main.myPlayer == Projectile.owner)
                            ModContent.GetInstance<ScreenSystem>().RedEyesSpecialCamera(Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2, owner.Center, 0.2f);
                        return;
                    }
                }
                if (Projectile.ai[1] < 30)
                {
                    if (Projectile.owner == Main.myPlayer)
                        ModContent.GetInstance<ScreenSystem>().RedEyesSpecialCamera(Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2, owner.Center, 0.2f);
                    int d2 = Dust.NewDust(owner.position, owner.width, owner.height, DustID.GoldCoin, -4 * owner.direction, 0, 0);
                    Main.dust[d2].noGravity = true;
                    Main.dust[d2].fadeIn = 1.2f;
                }
                else if (Projectile.ai[1] == 30)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        int d2 = Dust.NewDust(owner.position, owner.width, owner.height, DustID.GoldCoin, -4 * owner.direction);
                        Main.dust[d2].velocity.Y *= 4;
                        Main.dust[d2].noGravity = true;
                        Main.dust[d2].fadeIn = 1.2f;
                    }

                    if (Projectile.ai[0] == 1)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RoadOfGoldPlayer>(), 0, 0, Projectile.owner, -1, -1);
                    }
                }
                else if (Projectile.ai[1] == 40)
                {
                    Vector2 speed = new Vector2(-12 * direction, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center - speed * 8f, speed, ModContent.ProjectileType<GoldRushEffect>(), 0, 0, Projectile.owner);
                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 pos = owner.Center + new Vector2(24 * owner.direction, 0);
                        Vector2 vel = new Vector2(Main.rand.Next(-6, -2) * owner.direction, 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-60, 60)));
                        Dust gold = Dust.NewDustPerfect(pos, DustID.GoldCoin, vel);
                        gold.noGravity = true;
                        gold.fadeIn = 1.2f;
                    }                    
                    GoldRushHold.DiamondDust(owner.Center + new Vector2(18 * owner.direction, 0), DustID.GoldCoin, 8, 2, 8, 1);
                    GoldRushHold.DiamondDust(owner.Center + new Vector2(24 * owner.direction, 0), DustID.GoldCoin, 8, 1, 4, 1);
                }
            }
            Projectile.direction = Projectile.spriteDirection = owner.direction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile portal = Main.projectile[(int)Projectile.ai[2]];
            if (portal.active || portal.type == ModContent.ProjectileType<RoadOfGoldPlayer>() || portal.owner == Projectile.owner)
            {
                portal.ai[1] = -1;
                portal.netUpdate = true;
            }
            for (int i = 0; i < 8; i++)
            {
                int dir = Main.player[Projectile.owner].direction;
                int d = Dust.NewDust(new Vector2(Main.player[Projectile.owner].Center.X, Main.player[Projectile.owner].Center.Y - 15), 40, 30, DustID.GoldCoin);
                Main.dust[d].velocity.X = 8 * dir;
                Main.dust[d].velocity.Y = 0;
                if (dir == -1)
                {
                    Main.dust[d].position.X -= 40;
                }
            }

            Main.player[Projectile.owner].GetModPlayer<LobotomyAlephPlayer>().GoldRushBreakBliss(target, 60 * 5);
            if (Main.myPlayer == Projectile.owner)
                ModContent.GetInstance<ScreenSystem>().ScreenShake(20, 10f);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Width = hitbox.Height = 64;
            int halfwidth = hitbox.Width / 2;
            hitbox.X = (int)Main.player[Projectile.owner].Center.X - halfwidth + halfwidth * Main.player[Projectile.owner].direction;
            hitbox.Y = (int)Main.player[Projectile.owner].Center.Y - hitbox.Height / 2;
            base.ModifyDamageHitbox(ref hitbox);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] > 0 && Projectile.ai[1] == 40)
            {
                //if (target.whoAmI == Main.projectile[(int)Projectile.ai[2]].ai[1])
                    return null;
            }
            return false;
        }

        private void FindNextPortal()
        {
            while (Projectile.ai[0] > 0)
            {
                foreach (Projectile p in Main.projectile)
                {
                    if (p.active && p.type == ModContent.ProjectileType<RoadOfGoldPlayer>() && p.owner == Projectile.owner && p.ai[0] == Projectile.ai[0] && p.timeLeft > 15)
                    {
                        p.ai[2] = 1;
                        p.netUpdate = true;
                        Projectile.ai[2] = p.whoAmI;
                        return;
                    }
                }
                Projectile.ai[0]--;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Main.player[Projectile.owner].MountedCenter - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 origin = new Vector2(tex.Width - 10, 10);
            float rotation = Projectile.rotation + 0.785f;
            lightColor *= (1f - Projectile.alpha / 255f);
            float prog = Math.Clamp( (Projectile.ai[1] - 30f) / 8f, 0f, 1f);
            Vector2 offset = new Vector2((float)Terraria.Utils.Lerp(-8, 24, (float)Math.Sin(1.57f * prog)), 0) * Projectile.spriteDirection;
            

            SpriteEffects sp = SpriteEffects.None;
            if (Projectile.spriteDirection < 0)
            {
                sp = SpriteEffects.FlipHorizontally;
                origin.X = 10;
                rotation = Projectile.rotation + 2.356f + 3.14f;
            }

            Main.EntitySpriteDraw(tex, pos + offset, tex.Frame(), lightColor, rotation, origin, Projectile.scale + 0.3f, sp, 0);

            return false;
        }
    }
}

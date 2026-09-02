using LobotomyCorp.Items.Ruina.General;
using LobotomyCorp.Misc;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using LobotomyCorp.Visuals.LobEffects;
using log4net.Filter;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
    public class RemorseHammerEX : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Friend!");
        }

        public SkeletonBase FSkel;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1000;

            Projectile.DamageType = DamageClass.Default;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.alpha = 255;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            target = new Vector2(default);
        }

        Vector2 target = new Vector2(default);

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(target);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            target = reader.ReadVector2();
        }

        public override void AI()
        {
            if (Projectile.localAI[0] < 0)
            {
                Projectile.Opacity += Projectile.localAI[0];
                if (Projectile.Opacity < 0f)
                    Projectile.Opacity = 0f;
            }
            else
            {
                Projectile.Opacity += Projectile.localAI[0];
                if (Projectile.Opacity > 1f)
                    Projectile.Opacity = 1f;
            }

            Player player = Main.player[Projectile.owner];

            float handRot = 70f;
            float handSpeed = 0.087f;
            float shoulderRot = -60;

            float ikDist = 0.5f;
            float ikRot = -50;
            float ikSpeed = 16;
            // Idle Behavior
            if (Projectile.ai[0] == 0)
            {
                Projectile.localAI[0] = 0.1f;

                Vector2 targetPos = player.Center + new Vector2(100, player.height);

                AIHelper.ChaseTargetLerp(Projectile.Center, targetPos, ref Projectile.velocity, 16f, 0.3f);

                Projectile.spriteDirection = 1;
                if (Main.myPlayer == Projectile.owner && Projectile.Opacity >= 1f && player.controlUseItem && player.altFunctionUse != 2)
                {
                    target = Main.MouseWorld;
                    Projectile.ai[1] = 1;
                    Projectile.ai[0] = 1;
                    if (target.X < player.Center.X)
                        Projectile.spriteDirection = -1;
                    Projectile.netUpdate = true;
                }

                float closest = 10000;
                bool hasTarget = false;
                foreach (NPC n in Main.ActiveNPCs)
                {
                    float dist = n.Distance(target);
                    if (n.GetGlobalNPC<LobotomyGlobalNPC>().RemorseGuilt >= 20 && n.CanBeChasedBy(Projectile) && dist < closest)
                    {
                        closest = dist;
                        Projectile.ai[2] = n.whoAmI;
                        Projectile.ai[1] = 0;
                        Projectile.ai[0] = 3;
                        Projectile.spriteDirection = 1;
                        Projectile.netUpdate = true;
                        hasTarget = true;
                    }
                }
                if (hasTarget)
                {
                    foreach (Projectile p in Main.ActiveProjectiles)
                    {
                        if (p.type == ModContent.ProjectileType<RemorseNailEX>() && p.owner == Projectile.owner)
                        {
                            p.ai[2] = Projectile.ai[2];
                            p.ai[1] = 0;
                            p.ai[0] = 3;
                            Projectile.spriteDirection = -1;
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }
            // Swings at selected position
            else if (Projectile.ai[0] < 3)
            {
                //AIHelper.ChaseTargetLerp(Projectile.Center, target + new Vector2(-300 * Projectile.spriteDirection, 0), ref Projectile.velocity, 26f, 0.3f);
                Projectile.velocity = (target + new Vector2(-300 * Projectile.spriteDirection, 0) - Projectile.Center) * 0.2f;

                int maxTime = player.itemAnimationMax;
                if (maxTime < 25)
                    maxTime = 25;
                int time1 = (int)(maxTime * 0.2f);
                int time2 = time1 + time1;
                int time3 = time2 + time1;

                if (Projectile.ai[1] < time1)
                {
                    ikRot = -135;
                    shoulderRot = -120;
                }
                else if (Projectile.ai[1] < time2)
                {
                    if (Projectile.ai[1] == time1)
                    {
                        SoundEngine.PlaySound(LobotomyCorp.ItemLobSound("GeneralWorks/Slientgirl_Hammer"), Projectile.Center);

                        SpawnSmear(time3);
                    }

                    float time = (Projectile.ai[1] - time1) / time2;
                    time = 1 - (float)Math.Pow(1 - time, 3);

                    ikDist = 0.6f + 0.4f * time;
                    ikRot = -135 + 155 * time;
                    ikSpeed = -1;

                    handRot = ikRot + 180;
                    handSpeed = -1;

                    shoulderRot = -120 + 130 * time;
                    Projectile.ai[0] = 2;
                }
                else if (Projectile.ai[1] < time3)
                {
                    if (Projectile.ai[1] == time3 - 1)
                    {
                        SpawnImpact(Projectile.Center + new Vector2(330 * Projectile.spriteDirection, 32));
                    }

                    ikDist = 1f;
                    ikRot = 20f;
                    shoulderRot = 10;
                    handRot = ikRot + 180;
                }
                else
                {
                    Projectile.ai[0] = 1;
                }
                if (Projectile.ai[1] >= maxTime)
                {
                    Projectile.ai[0] = 0;
                    Projectile.ai[1] = 0;
                }
            }
            // Performs an attack to targetted enemy
            else if (Projectile.ai[0] < 6)
            {
                NPC n = Main.npc[(int)Projectile.ai[2]];
                if (!n.active)
                {
                    Projectile.ai[0] = 6;
                    Projectile.ai[1] = 0;
                }
                if (Projectile.ai[0] == 3)
                {
                    Projectile.localAI[0] = -0.05f;
                    Projectile.velocity.X = 0;
                    Projectile.velocity.Y = 6;
                    if (Projectile.ai[1] > 20)
                    {
                        Projectile.ai[0] = 4;
                        Projectile.ai[1] = 0;
                        Projectile.Center = n.Center - new Vector2(300 * Projectile.spriteDirection, -60);
                        Projectile.velocity.Y = 0;
                        Projectile.velocity.X = 0;
                        Projectile.spriteDirection = -1;
                        SoundEngine.PlaySound(LobotomyCorp.ItemLobSound("GeneralWorks/Slientgirl_Strong_Start"), Projectile.Center);
                    }
                }
                else
                {
                    Projectile.localAI[0] = 0.1f;
                    float offset = Projectile.ai[1] / 10;
                    if (offset > 1f)
                        offset = 1f;
                    Vector2 target = n.Center - new Vector2(300 * Projectile.spriteDirection, -60 + 42 * offset);
                    Projectile.Center = target;

                    int timeStart = 70;
                    int windup = 30;
                    if (Projectile.ai[1] >= timeStart - windup)
                    {
                        float realTime = (Projectile.ai[1] - timeStart) % 24;
                        float maxTime = 24;
                        int time1 = (int)(maxTime * 0.2f);
                        int time2 = time1 + time1;
                        int time3 = time2 + time1;

                        if (realTime < time1 || Projectile.ai[1] < timeStart)
                        {
                            ikRot = -135;
                            handRot -= 30;
                            shoulderRot = -120;
                        }
                        else if (realTime < time2)
                        {
                            if (realTime == time1)
                            {
                                SoundEngine.PlaySound(LobotomyCorp.ItemLobSound("GeneralWorks/Slientgirl_Hammer"), Projectile.Center);

                                SpawnSmear(time3);
                            }

                            float time = (realTime - time1) / time2;
                            time = 1 - (float)Math.Pow(1 - time, 3);

                            ikDist = 0.6f + 0.4f * time;
                            ikRot = -135 + 155 * time;
                            ikSpeed = -1;

                            handRot = ikRot + 180;
                            handSpeed = -1;

                            shoulderRot = -120 + 130 * time;
                            Projectile.ai[0] = 5;
                        }
                        else if (realTime < time3)
                        {
                            ikDist = 1f;
                            ikRot = 20f;
                            shoulderRot = 10;
                            handRot = ikRot + 180;

                            if (realTime == time3 - 1)
                            {
                                SpawnImpact(Projectile.Center + new Vector2(300 * Projectile.spriteDirection, 32));
                            }
                        }
                        else
                        {
                            Projectile.ai[0] = 4;
                        }
                        if (Projectile.ai[1] >= timeStart + maxTime * 4)
                        {
                            Projectile.ai[0] = 6;
                            Projectile.ai[1] = 0;
                        }
                    }
                }
            }
            // Teleports back to player after doing an animation
            else
            {
                Projectile.localAI[0] = -0.1f;
                Projectile.velocity.X = 0;
                Projectile.velocity.Y = 6;
                if (Projectile.ai[1] > 10)
                {
                    Projectile.ai[0] = 0;
                    Projectile.ai[1] = 0;
                    Projectile.ai[2] = -1;
                    Projectile.Center = player.Center + new Vector2(100, player.height);
                }
            }

            if (!player.GetModPlayer<LobotomyTethPlayer>().RemorseHammerTime || player.dead)
            {
                if (Projectile.timeLeft > 10)
                    Projectile.timeLeft = 10;
                if (Projectile.ai[0] != 6)
                {
                    Projectile.ai[0] = 6;
                    Projectile.ai[1] = 0;
                }                    
            }
            else
            {
                Projectile.timeLeft = 30;
            }

            if (FSkel == null)
            {
                InitializeSkeleton();
            }
            Projectile.ai[1]++;
            FSkel.BoneName[Bone.Shoulder].ChangeRotation(MathHelper.ToRadians(shoulderRot + 5 * (float)Math.Sin(-Projectile.ai[1] * 0.017f)), 0.087f);
            FSkel.BoneName[Bone.Shoulder].ChangeOffset(Projectile.Center);

            FSkel.BoneName[Bone.Arm].ChangeRotation(MathHelper.WrapAngle(MathHelper.ToRadians(handRot)), handSpeed);

            Vector2 ikpos = new Vector2(GetLength * ikDist, 0).RotatedBy(MathHelper.ToRadians(ikRot));
            FSkel.BoneName[Bone.ArmIK].ChangeOffset(ikpos, ikSpeed);
            FSkel.RotationIK(Bone.LowerArm, Bone.UpperArm, Bone.ArmIK);
        }

        private void SpawnSmear(int time)
        {
            WeaponSmearCircle circ = new WeaponSmearCircle();
            float startRot = MathHelper.ToRadians(Projectile.spriteDirection == 1 ? -135f : -45);
            circ.Setup(Projectile, new Vector2(40 * Projectile.spriteDirection, 0), startRot, time, Projectile.spriteDirection);
            circ.SetupSemiCircle(55, 250, MathHelper.ToRadians(20), MathHelper.ToRadians(120), MathHelper.ToRadians(170));
            circ.SetShaderImage(MiscAssets.RemorseBrown);
            circ.Color = Color.White;
            LobCustomDraw.Instance().AddVEffects(circ);
        }

        private void SpawnImpact(Vector2 position)
        {
            WeaponSmearLine line = new();
            line.Setup(position, Vector2.Zero, -1.57f, 12, 0);
            line.SetupLine(38, 12, 240);
            line.SetShaderImage(MiscAssets.RemorseBrown);
            line.SetShaderTexOffset(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
            line.Color = Color.White * 0.6f;
            LobCustomDraw.Instance().AddVEffects(line);            

            WeaponSmearEllipse elli = new();
            elli.Setup(position, Vector2.Zero, 0, 10, Projectile.spriteDirection);
            elli.SetupEllipse(60, 23, 60, 20, Main.rand.NextFloat(6.28f), 0.1f, 2f);
            elli.SetShaderImage(MiscAssets.RemorseBrown); 
            elli.Color = Color.White * 0.6f;
            LobCustomDraw.Instance().AddVEffects(elli);

            for (int i = 0; i < 5; i++)
            {
                WeaponSmearLine line2 = new();
                Vector2 vel = new Vector2(1 + Main.rand.NextFloat(3), 0).RotatedByRandom(-1.57f) * 6;
                vel = vel.RotatedBy(-1.57f);
                line2.Setup(position, vel, vel.ToRotation() + 3.14f, 12 + Main.rand.Next(7), 1);
                line2.SetupLine(6, 12, 30 + Main.rand.Next(60));
                line2.SetShaderImage(MiscAssets.RemorseBright, MiscAssets.WindTrail);
                line2.SetShaderTexOffset(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
                LobCustomDraw.Instance().AddVEffects(line2);
            }
        }

        private float GetLength => FSkel.BoneName[Bone.LowerArm].Length + FSkel.BoneName[Bone.UpperArm].Length;

        private void InitializeSkeleton()
        {
            Dictionary<Enum, BonePart> list = new Dictionary<Enum, BonePart>();
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            list.Add(Bone.Shoulder, new BonePart(Projectile.Center, MathHelper.ToRadians(-60), 1f, 32 * 3)
                    .SetDraw(tex, 
                    new Rectangle(0, 64 * 4, 224, 64), 
                    new Vector2(32, 32)));

            list.Add(Bone.LowerArm, new BonePart(Vector2.Zero, MathHelper.ToRadians(60), 1, 32 * 2, list[Bone.Shoulder]).
                    SetDraw(tex,
                    new Rectangle(0, 64 * 3, 224, 64),
                    new Vector2(32, 32)).
                    ChangeIRotation(false));

            list.Add(Bone.UpperArm, new BonePart(Vector2.Zero, MathHelper.ToRadians(-70), 1, 32 * 4, list[Bone.LowerArm])
                   .SetDraw(tex,
                   new Rectangle(0, 64 * 2, 224, 64),
                   new Vector2(32, 32)).
                    ChangeIRotation(false));

            list.Add(Bone.Arm, new BonePart(Vector2.Zero, MathHelper.ToRadians(70), 1, 1, list[Bone.UpperArm])
                   .SetDraw(tex,
                   new Rectangle(0, 0, 224, 128),
                   new Vector2(32 * 3 + 16, 32 * 2 + 16)).
                    ChangeIRotation(false));

            list.Add(Bone.ArmIK, new BonePart(Vector2.Zero, 0, 1, 1, list[Bone.LowerArm])
                   .ChangeIRotation(false)
                   .ChangeIOffestRotation(false)
                   .ChangeParentOrigin(true));

            FSkel = new SkeletonBase(list);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (FSkel != null) {
                BonePart arm = FSkel.BoneName[Bone.Arm];
                Vector2 armPos = arm.GetPosition(Projectile.spriteDirection);
                armPos += new Vector2(75, 15 * Projectile.spriteDirection).RotatedBy(arm.GetRotation(Projectile.spriteDirection) + 3.14f);
                int size = 110;
                int off = size / 2;
                hitbox.X = (int)armPos.X - off;
                hitbox.Y = (int)armPos.Y - off;
                hitbox.Width = hitbox.Height = size;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 2 || Projectile.ai[0] == 5)
                return null;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RemorseNail.RemorseOnHitNailActivate(target.whoAmI);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (FSkel == null)
                return false;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            int dir = Projectile.spriteDirection;
            List<BonePart> draw = FSkel.GetBoneList(true);
            foreach (BonePart b in draw)
            {
                Vector2 Origin = b.Origin;
                SpriteEffects sp = 0;
                float rot = 0;
                if (dir < 0)
                {
                    Origin.X = tex.Width - Origin.X;
                    sp = SpriteEffects.FlipHorizontally;
                    rot = MathHelper.ToRadians(180);
                }

                Main.EntitySpriteDraw(tex,
                    b.GetPosition(dir) - Main.screenPosition,
                    b.Frame,
                    lightColor * Projectile.Opacity,
                    b.GetRotation(dir) + rot,
                    Origin,
                    b.GetScale(),
                    sp,
                    0);
            }
            return false;
        }

        enum Bone
        {
            Arm,
            LowerArm,
            UpperArm,
            Shoulder,
            ArmIK
        }
    }
}

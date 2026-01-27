using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.KingPortal
{
	public class RoadOfGold : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Projectiles/KingPortal/KingPortal1";

        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Road to Happiness");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 300;

            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.netImportant = true;
        }

        public float portalCounter
        {
            get { return Projectile.ai[2]; }
            set { Projectile.ai[2] = value; }
        }
        public override void AI() {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.scale = 0.1f;
                Projectile.localAI[0]++;
            }
            if (Projectile.scale < 1.5f)
                Projectile.scale += 0.3f;

            portalCounter++;
            if (portalCounter > 30 && Projectile.ai[1] > 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 center = Vector2.Zero;
                int height = 0;

                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.type == ModContent.NPCType<NPCs.RedMist.RedMist>())
                    {
                        if (npc.HasValidTarget)
                        {
                            height = npc.GetTargetData().Height;
                            center = npc.GetTargetData().Center;
                        }
                        else
                        {
                            center = npc.Center;
                        }
                        break;
                    }
                }

                int dir = Main.rand.NextBool(2) ? 1 : -1;
                if (Projectile.ai[0] == -1)
                {
                    int y = 0;
                    if (Projectile.ai[1] == 1)
                        y = height / 2 - 50;
                    Projectile.ai[0] = Projectile.NewProjectile(Projectile.GetSource_FromThis(), center + new Vector2(400 * dir, y), new Vector2(-22f * dir, 0), Projectile.type, 0, 0, -1, -2, -1);
                    if (Projectile.ai[1] > 1)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), center - new Vector2(400 * dir, 0), new Vector2(-24f * dir, 0), Projectile.type, 0, 0, -1, -1, Projectile.ai[1] - 1);
                    //Main.NewText(Projectile.ai[0]);
                    //Main.NewText(Projectile.ai[1]);
                    Projectile.netUpdate = true;
                }
                else if (Projectile.ai[0] == -3)
                {
                    float rotation = MathHelper.ToRadians(45 * Main.rand.Next(4));
                    Vector2 norm = new Vector2(1, 0).RotatedBy(rotation);
                    if (Projectile.ai[1] == 1)
                    {
                        dir = -1;
                        norm = new Vector2(0, 1);
                    }
                    Projectile.ai[0] = Projectile.NewProjectile(Projectile.GetSource_FromThis(), center + norm * (400 * dir), norm * (-24f * dir), Projectile.type, 0, 0, -1, -2, -1);
                    if (Projectile.ai[1] > 1)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), center - norm * (400 * dir), norm * (-24f * dir), Projectile.type, 0, 0, -1, -3, Projectile.ai[1] - 1);

                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.ai[0] >= 0 && Projectile.timeLeft > 10)
            {
                //Teleport
                foreach (NPC npc in Main.npc)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (npc.active && npc.type == ModContent.NPCType<NPCs.RedMist.RedMist>() && Projectile.getRect().Intersects(npc.getRect()))
                        {
                            Projectile otherPortal = Main.projectile[(int)Projectile.ai[0]];
                            npc.Center = otherPortal.Center;
                            npc.velocity = otherPortal.velocity;
                            npc.spriteDirection = Math.Sign(npc.velocity.X);
                            if (npc.spriteDirection == 0)
                                npc.spriteDirection = 1;
                            npc.netUpdate = true;
                            otherPortal.timeLeft = 10;
                            otherPortal.netUpdate = true;
                            Projectile.timeLeft = 10;
                            Projectile.netUpdate = true;

                            SoundEngine.PlaySound(SoundID.Item25, Projectile.Center);

                            for (int i = 0; i < 8; i++)
                            {
                                Vector2 Box = new Vector2(50, 100);
                                Dust d = Main.dust[Dust.NewDust(Projectile.Center - Box / 2, (int)Box.X, (int)Box.Y, 87)];
                                d.noGravity = true;
                                d.fadeIn = 1.8f;
                                d = Main.dust[Dust.NewDust(otherPortal.Center - Box / 2, (int)Box.X, (int)Box.Y, 87)];
                                d.noGravity = true;
                                d.fadeIn = 1.8f;
                            }

                            if (Main.expertMode && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = -1; i < 2; i += 2)
                                {
                                    float rot = otherPortal.velocity.ToRotation();
                                    Vector2 vel = new Vector2(4, 6 * i).RotatedBy(rot);
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), otherPortal.Center, vel, ModContent.ProjectileType<NPCs.RedMist.GoldRushCrystal>(), 15, 2, -1, rot);
                                }
                            }
                            break;
                        }
                    }
                }
            }

            Projectile.rotation += MathHelper.ToRadians(1);
            if (Projectile.rotation > (float)Math.PI * 2)
                Projectile.rotation -= (float)Math.PI * 2;

            if (Projectile.timeLeft < 10)
                Projectile.alpha += 25;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] == 0)
                return false;

            Texture2D texture = LobotomyCorp.KingPortal1.Value;
            float rot = Projectile.velocity.ToRotation();
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);// - new Vector2(32f, 0).RotatedBy(rot);
            Vector2 origin = new Vector2(61, 61);
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 scale = new Vector2(0.5f, 1f) * Projectile.scale;
            Color color = Color.White * (1 - ((float)Projectile.alpha / 255));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            float rotateProg = Projectile.rotation / (2f * (float)Math.PI);

            var resizeShader = GameShaders.Misc["LobotomyCorp:Rotate"];
            /*resizeShader.UseOpacity(Projectile.rotation / (2 * (float)Math.PI));
            resizeShader.Apply(null);

            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, scale, SpriteEffects.None, 0);*/
            resizeShader.UseOpacity(1f - ((float)Projectile.alpha / 255));
            resizeShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(1f - rotateProg));
            resizeShader.Apply(null);

            texture = LobotomyCorp.KingPortal2.Value;
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, scale, SpriteEffects.None, 0);

            resizeShader.UseOpacity(1f - ((float)Projectile.alpha / 255));
            resizeShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(rotateProg));
            resizeShader.Apply(null);

            texture = LobotomyCorp.KingPortal3.Value;
            frame = new Rectangle(0, 0, texture.Width, texture.Height);
            origin = frame.Size() / 2;
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            
            return false;
        }
    }

    public class RoadOfKing : ModProjectile
    {
        //Portal spawns more portal depending 
        public override string Texture => "LobotomyCorp/Projectiles/KingPortal/KingPortal1";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Road of King");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 1200;

            Projectile.tileCollide = false;
            Projectile.hostile = true;
            portalCounter = 0;
            portalGoldEntered = false;
        }

        private int portalPair //-1 Current No Pair, -2 Never spawn Pair, -3 Spawn exit only, -4 Final portal
        {
            set { Projectile.ai[0] = value; }
            get { return (int)Projectile.ai[0]; }
        }

        private float portalTimer
        {
            set { Projectile.ai[1] = value; }
            get { return Projectile.ai[1]; }
        }

        private int portalCounter = 0;
        private bool portalGoldEntered = false;
        public bool portalRedMistEntered = false;

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.scale = 0.1f;
                Projectile.localAI[0]++;
            }
            if (Projectile.scale < 1.5f)
                Projectile.scale += 0.3f;

            portalTimer--;
            if (portalTimer < 0 && portalPair == -1 && IsRedMistActive() && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Player targetPlayer = getNearest();

                float goldRushSpeed = NPCs.RedMist.RedMist.GOLDRUSH4SPEED;

                if (targetPlayer == null)
                {
                    Vector2 portalCenter = Vector2.One;
                    Vector2 velocity = new Vector2(goldRushSpeed, 0);

                    Vector2 portalPos = portalCenter - velocity * NPCs.RedMist.RedMist.GOLDRUSH4DELAY;
                    portalPair = Projectile.NewProjectile(Projectile.GetSource_FromThis(), portalPos, velocity, Projectile.type, 0, 0, -1, -2);
                }
                else
                {
                    bool centerOnPlayer = Main.rand.Next(3) == 0;
                    Vector2 portalCenter = targetPlayer.Center;
                    if (!centerOnPlayer)
                    {
                        portalCenter.X += Main.rand.NextFloat(-180f, 180f);
                        portalCenter.Y += Main.rand.NextFloat(-180f, 180f);
                    }

                    float nextAngle = Main.rand.NextFloat(-1.0472f, 1.0472f) + 3.14f * Main.rand.Next(2);
                    Vector2 velocity = new Vector2(goldRushSpeed, 0).RotatedBy(nextAngle);

                    Vector2 portalPos = portalCenter - velocity * NPCs.RedMist.RedMist.GOLDRUSH4DELAY;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        portalPair = Projectile.NewProjectile(Projectile.GetSource_FromThis(), portalPos, velocity, Projectile.type, 0, 0, -1, -2);

                    portalPos = portalCenter + velocity * NPCs.RedMist.RedMist.GOLDRUSH4DELAY;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), portalPos, velocity, Projectile.type, 0, 0, -1, -1, NPCs.RedMist.RedMist.GOLDRUSH4DELAY * 2 + 1);
                }
                Projectile.netUpdate = true;
                //Main.NewText("Beep boop made new Portal");
            }

            if (portalTimer < 0 && portalPair == -3)
            {
                Player targetPlayer = getNearest();
                if (targetPlayer == null)
                    portalPair = -2;
                else
                {
                    Vector2 portalCenter = targetPlayer.Center;
                    portalCenter.Y = targetPlayer.position.Y + targetPlayer.height - 52;

                    float goldRushSpeed = NPCs.RedMist.RedMist.GOLDRUSH4SPEED;
                    float nextAngle = 3.14f * Main.rand.Next(2);
                    Vector2 velocity = new Vector2(goldRushSpeed, 0).RotatedBy(nextAngle);

                    Vector2 portalPos = portalCenter - velocity * NPCs.RedMist.RedMist.GOLDRUSH4DELAY / 2;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        portalPair = Projectile.NewProjectile(Projectile.GetSource_FromThis(), portalPos, velocity, Projectile.type, 0, 0, -1, -4);
                    Projectile.netUpdate = true;
                }
            }

            //Main.NewText("Portal #" + Projectile.whoAmI + " at " + portalTimer);

            if (portalPair >= 0 && !portalGoldEntered)
            {
                TeleportGoldRush();
            }
            if (portalGoldEntered && !portalRedMistEntered)
            {
                TeleportRedMist();
            }

            Projectile.rotation += MathHelper.ToRadians(1);
            if (Projectile.rotation > (float)Math.PI * 2)
                Projectile.rotation -= (float)Math.PI * 2;

            if (Projectile.timeLeft < 10)
                Projectile.alpha += 25;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(portalGoldEntered);
            writer.Write(portalRedMistEntered);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            portalGoldEntered = reader.ReadBoolean();
            portalRedMistEntered = reader.ReadBoolean();
        }

        private bool IsRedMistActive()
        {
            return (NPC.AnyNPCs(ModContent.NPCType<NPCs.RedMist.RedMist>()) || NPC.AnyNPCs(ModContent.ProjectileType<GoldRushRedMist>()));
        }

        private void TeleportGoldRush()
        {
            foreach(Projectile p in Main.projectile)
            {
                if (p.active && p.type == ModContent.ProjectileType<GoldRushRedMist>() &&
                    (p.getRect().Intersects(Projectile.getRect()) || p.Center.Distance(Projectile.Center) < 16f + p.velocity.Length()))
                {
                    Projectile pair = Main.projectile[portalPair];
                    p.velocity = pair.velocity;
                    p.Center = pair.Center;

                    portalGoldEntered = true;

                    //Main.NewText("Teleport GR Success");

                    p.ai[1]++;
                    Projectile.timeLeft = 240;
                    pair.timeLeft = 240;
                    if (p.ai[1] > p.ai[0])
                    {
                        foreach(Projectile portal in Main.projectile)
                        {
                            if (portal.active && portal.type == Projectile.type && portal.ai[0] == -1)
                            {
                                portal.ai[0] = -3;
                                portal.netUpdate = true;
                            }
                        }
                    }

                    Projectile.netUpdate = true;
                    pair.netUpdate = true;
                    p.netUpdate = true;

                    return;
                }
            }
        }

        private void TeleportRedMist()
        {
            foreach (NPC n in Main.npc)
            {
                if (n.active && n.type == ModContent.NPCType<NPCs.RedMist.RedMist>() &&
                    (n.getRect().Intersects(Projectile.getRect()) || n.Center.Distance(Projectile.Center) < 16f + n.velocity.Length()))
                {
                    Projectile pair = Main.projectile[portalPair];
                    portalRedMistEntered = true;

                    NPCs.RedMist.RedMist redMist = (NPCs.RedMist.RedMist)n.ModNPC;
                    redMist.TeleportP4(Projectile.whoAmI, pair.whoAmI, pair.Center, Vector2.Normalize(pair.velocity) * n.velocity.Length());
                    //Main.NewText("Teleport RM Success");
                    Projectile.timeLeft = 60;
                    pair.timeLeft = 60;

                    Projectile.netUpdate = true;
                    return;
                }
            }
        }

        private Player getNearest()
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                Player target = null;
                float distance = 12000;
                foreach (Player p in Main.ActivePlayers)
                {
                    if (!p.dead)
                    {
                        float checkDist = Vector2.Distance(Projectile.Center, p.Center);
                        if (checkDist < distance)
                            target = p;
                    }
                }
                return target;
            }
            else
            {
                if (Main.LocalPlayer.dead)
                    return null;
                return Main.LocalPlayer;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] == 0)
                return false;

            Texture2D texture = LobotomyCorp.KingPortal1.Value;
            float rot = Projectile.velocity.ToRotation();
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);// - new Vector2(32f, 0).RotatedBy(rot);
            Vector2 origin = new Vector2(61, 61);
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 scale = new Vector2(0.5f, 1f) * Projectile.scale;
            Color color = Color.White * (1 - ((float)Projectile.alpha / 255));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            float rotateProg = Projectile.rotation / (2f * (float)Math.PI);

            var resizeShader = GameShaders.Misc["LobotomyCorp:Rotate"];
            resizeShader.UseOpacity(1f - ((float)Projectile.alpha / 255));
            resizeShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(1f - rotateProg));
            resizeShader.Apply(null);

            texture = LobotomyCorp.KingPortal2.Value;
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, scale, SpriteEffects.None, 0);

            resizeShader.UseOpacity(1f - ((float)Projectile.alpha / 255));
            resizeShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(rotateProg));
            resizeShader.Apply(null);

            texture = LobotomyCorp.KingPortal3.Value;
            frame = new Rectangle(0, 0, texture.Width, texture.Height);
            origin = frame.Size() / 2;
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}

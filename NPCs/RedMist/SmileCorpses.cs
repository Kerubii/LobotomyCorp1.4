using LobotomyCorp;
using LobotomyCorp.Items.Teth;
using LobotomyCorp.Projectiles;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.NPCs.RedMist
{
    class SmileCorpses : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 24;

            NPC.lifeMax = 60;
            NPC.damage = 60;
            NPC.defense = 12;

            NPC.aiStyle = -1;
            NPC.timeLeft *= 10000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.knockBackResist = 0.01f;
            LobotomyGlobalNPC.LNPC(NPC).RiskLevel = (int)RiskLevel.Aleph;
        }

        float AiState { set { NPC.ai[0] = value; } get { return NPC.ai[0]; } }
        float AiCounter { set { NPC.ai[1] = value; } get { return NPC.ai[1]; } }
        const int BLOBSTATE = 0;
        const int CORPSESTATE = 1;
        const int ATTACKSTATE = 2;

        public override void AI()
        {
            if (AiState == BLOBSTATE)
            {
                if (AiCounter == 0)
                {
                    NPC.velocity = new Vector2(Main.rand.Next(-5, 5), -Main.rand.Next(10, 16));
                    NPC.direction = Main.rand.NextBool(2) ? -1 : 1;
                    NPC.localAI[1] = Main.rand.Next(3);
                }
                AiCounter++;

                NPC.rotation += MathHelper.ToRadians(3) * NPC.velocity.X;
                if (Main.rand.NextBool(3))
                {
                    
                }

                if (NPC.velocity.Y > 0)
                {
                    if (NPC.collideX || NPC.collideY)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith, 0, -2);
                        }
                        AiState++;
                        AiCounter = 0;
                        NPC.velocity *= 0;
                        NPC.rotation = 0;
                    }
                }
            }
            else if (AiState == CORPSESTATE)
            {
                AiCounter++;

                if (AiCounter > 60 && Main.rand.NextBool(30))
                {
                    AiState = ATTACKSTATE;
                    AiCounter = 0;
                }
            }
            else if (AiState == ATTACKSTATE)
            {
                AiCounter++;
                if (AiCounter < 30)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 vel = new Vector2(0, -2).RotatedByRandom(0.1f);
                        Dust d = Dust.NewDustPerfect(NPC.Center, DustID.Wraith, vel);
                        d.scale = 0.75f;
                        //d.noGravity = true;
                    }
                }

                if (AiCounter == 30)
                {
                    NPC.TargetClosest(false);
                    float dist = (NPC.Center - NPC.GetTargetData().Center).LengthSquared();
                    if (NPC.position.Y - 300 < NPC.GetTargetData().Position.Y && dist > 300 * 300)
                    {
                        //Vector2 delta = NPC.GetTargetData().Center - NPC.Center;
                        //delta.Normalize();
                        Vector2 delta = new Vector2(0, -1f).RotatedByRandom(MathHelper.ToRadians(5));
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = (Main.rand.NextBool(2) ? -1 : 1);

                            float ai0 = Main.rand.Next(5, 10);
                            if (type < 0)
                                ai0 = Main.rand.Next(3, 8) * type;
                            //Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, delta * 12f, ModContent.ProjectileType<Projectiles.SmileBits>(), 16, 0, Main.myPlayer);//, Main.rand.NextFloat(0.26f));
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, delta * 8f, ModContent.ProjectileType<Projectiles.SmileBobs>(), 1, 0, -1, ai0, 8f, NPC.target);
                        }

                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 dustVel = delta.RotatedByRandom(0.1f) * Main.rand.NextFloat(2, 4);
                            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith, delta.X, delta.Y);
                            Main.dust[d].noGravity = true;
                        }
                    }
                }
                else if (AiCounter == 60)
                {
                    AiState = CORPSESTATE;
                    AiCounter = -Main.rand.Next(500, 720);
                }
            }

            NPC.localAI[0]++;
        }

        public override void OnKill()
        {
            Player closestPlayer = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];

            if (closestPlayer.statLife < closestPlayer.statLifeMax2)
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.localAI[0] == 1)
            {
                NPC.frame.Y = Main.rand.Next(3) * frameHeight;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Projectile[ModContent.ProjectileType<SmileBobs>()].Value;
            Vector2 pos = NPC.Center - screenPos;
            Rectangle frame = tex.Frame(1, 3, 0, (int)NPC.localAI[1]);
            Vector2 origin = frame.Size() / 2;
            if (NPC.ai[0] == BLOBSTATE)
            {
                Main.EntitySpriteDraw(tex, pos, frame, drawColor, NPC.rotation, origin, NPC.scale, 0, 0);
                return false;
            }

            tex = TextureAssets.Npc[NPC.type].Value;
            pos = NPC.position + new Vector2(NPC.width / 2, NPC.height) - screenPos;
            origin = new Vector2(NPC.frame.Width / 2, NPC.frame.Height - 4);
            Vector2 scale = new Vector2(1f + 0.1f * (float)Math.Sin(6.28f * NPC.localAI[0] / 60f), 1f) * NPC.scale;
            Main.EntitySpriteDraw(tex, pos, NPC.frame, drawColor, NPC.rotation, origin, scale, NPC.direction > 0 ? 0 : SpriteEffects.FlipHorizontally, 0);

            return false;
        }
    }
}

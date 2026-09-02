using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace LobotomyCorp.NPCs.WhiteNight
{
    //[AutoloadBossHead]
    class WhiteNight : ModNPC
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("WhiteNight");
            Main.npcFrameCount[NPC.type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 56;
            NPC.lifeMax = 6000;
            NPC.boss = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.damage = 25;
			NPC.defense = 0;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.0f;
            NPC.buffImmune[BuffID.Cursed] = true;
			NPC.HitSound = SoundID.NPCHit1;
            LobotomyGlobalNPC.LNPC(NPC).RiskLevel = (int)RiskLevel.Aleph;
        }
		
		public override void AI()
		{
            NPC.localAI[3] += MathHelper.ToRadians(1);
            if (NPC.localAI[3] > MathHelper.ToRadians(360))
                NPC.localAI[3] = 0;
            NPC.gfxOffY = 8 + 8 * (float)Math.Sin(NPC.localAI[3]);

            if (NPC.ai[2] == 1)
            {
                NPC.localAI[1]++;

            }
		}
		
        public override void FindFrame(int frameHeight)
        {
            if (NPC.localAI[1] == 11)
                NPC.frameCounter += NPC.frame.Y += frameHeight;

            NPC.frameCounter++;
            if (NPC.frameCounter > 15)
            {                
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.localAI[1] < 10)
                {
                    if (NPC.frame.Y > frameHeight * 3)
                        NPC.frame.Y = 0;
                }
                else
                {
                    if (NPC.frame.Y > frameHeight * 7)
                        NPC.frame.Y = 4 * frameHeight;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Aleph.ParadiseLost>()));
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type == ModContent.ProjectileType<PenitenceRLight>())
            {
                //modifiers.SourceDamage.Base = 666;
                modifiers.FinalDamage.Base = 0;
                modifiers.SourceDamage.Base = 666;
                modifiers.ScalingArmorPenetration += 1f;
                modifiers.SetMaxDamage(666);
                modifiers.DamageVariationScale *= 0;
                modifiers.DisableCrit();
            }
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ModContent.ProjectileType<PenitenceRLight>())
            {
                if (NPC.ai[2] <= 0)
                {
                    NPC.ai[2] = 1;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
            Texture2D texture = Mod.Assets.Request<Texture2D>("NPCs/WhiteNight/PaleRing").Value;
            Vector2 position = NPC.position - Main.screenPosition + new Vector2(8, 41 + NPC.gfxOffY);
            Vector2 origin = new Vector2(128, 128);
            float rotation = NPC.localAI[3];
            spriteBatch.Draw(texture, position, new Microsoft.Xna.Framework.Rectangle?
                        (
                            new Rectangle
                            (
                                0, 0, 256, 256
                            )
                        ),
                        Color.White, MathHelper.ToRadians(rotation), origin, 0.7f, SpriteEffects.None, 0f);

            texture = Mod.Assets.Request<Texture2D>("NPCs/WhiteNight/WhiteNightWingBack").Value;
            for (int i = 0; i < 8; i++)
            {
                int length = i % 2 == 0 ? 4 : 10;
                int height = 36 + (i > 1 ? 6 : 0);
                position = NPC.position - Main.screenPosition + new Vector2( length, height + NPC.gfxOffY);
                if (NPC.localAI[1] > 10)
                {
                    position.X += Main.rand.Next(-2, 3);
                    position.Y += Main.rand.Next(-2, 3);
                }
                origin = new Vector2(84, 70);
                if (i % 2 == 1)
                    origin = new Vector2(6, 70);
                rotation = 13 + 8 * (float)Math.Sin(NPC.localAI[3]);
                float scale = 0.8f;
                if (i > 1)
                {
                    rotation = 0 + 14 * (float)Math.Sin(NPC.localAI[3]);
                    scale = 1f;
                }
                if (i > 3)
                {
                    rotation = -35 + 12 * (float)Math.Sin(NPC.localAI[3]);
                    scale = 0.9f;
                }
                if (i > 5)
                {
                    rotation = -95 + 8 * (float)Math.Sin(NPC.localAI[3]);
                }
                if (i % 2 == 1)
                    rotation *= -1;
                SpriteEffects spriteeffect = SpriteEffects.None;
                if (i % 2 == 1)
                    spriteeffect = SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(texture, position, new Microsoft.Xna.Framework.Rectangle?
                                                    (
                                                        new Rectangle
                                                        (
                                                            0, 0, 90, 74
                                                        )
                                                    ),
                                drawColor, MathHelper.ToRadians(rotation), origin, scale, spriteeffect, 0f);
            }   

            texture = Mod.Assets.Request<Texture2D>("NPCs/WhiteNight/WhiteNightWingFrontR").Value;
            position = NPC.position - Main.screenPosition + new Vector2(14, 44 + NPC.gfxOffY);
            if (NPC.localAI[1] > 10)
            {
                position.X += Main.rand.Next(-2, 3);
                position.Y += Main.rand.Next(-2, 3);
            }
            origin = new Vector2(6, 18);
            rotation = 8 * (float)Math.Sin(NPC.localAI[3]);
            spriteBatch.Draw(texture, position, new Microsoft.Xna.Framework.Rectangle?
                                    (
                                        new Rectangle
                                        (
                                            0, 0, 62, 68
                                        )
                                    ),
                drawColor, MathHelper.ToRadians(rotation), origin, NPC.scale, SpriteEffects.None, 0f);

            texture = Mod.Assets.Request<Texture2D>("NPCs/WhiteNight/WhiteNightWingFrontL").Value;
            position = NPC.position - Main.screenPosition + new Vector2(2, 44 + NPC.gfxOffY);
            if (NPC.localAI[1] > 10)
            {
                position.X += Main.rand.Next(-2, 3);
                position.Y += Main.rand.Next(-2, 3);
            }
            origin = new Vector2(52, 16);
            rotation = -8 * (float)Math.Sin(NPC.localAI[3]);
            spriteBatch.Draw(texture, position, new Microsoft.Xna.Framework.Rectangle?
                                    (
                                        new Rectangle
                                        (
                                            0, 0, 64, 76
                                        )
                                    ),
                drawColor, MathHelper.ToRadians(rotation), origin, NPC.scale, SpriteEffects.None, 0f);
            return true;
		}
	}
}

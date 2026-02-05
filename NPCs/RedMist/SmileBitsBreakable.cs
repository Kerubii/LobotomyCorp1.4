using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Chat;
using LobotomyCorp;
using LobotomyCorp.Util;
using Terraria.Audio;

namespace LobotomyCorp.NPCs.RedMist
{
    class SmileBitsBreakable : ModNPC
    {
        public override string Texture => "LobotomyCorp/Projectiles/SmileBits";

        public override void SetStaticDefaults()
        {
            NPCID.Sets.ProjectileNPC[NPC.type] = true;
            Main.npcFrameCount[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 16;
            NPC.height = 16;

            NPC.lifeMax = 1;
            NPC.damage = 1;
            NPC.defense = 12;

            NPC.scale = 0.75f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.timeLeft *= 120;
            NPC.knockBackResist = 0.01f;
            //NPC.DeathSound = SoundID.NPCDeath1;
            LobotomyGlobalNPC.LNPC(NPC).RiskLevel = (int)RiskLevel.Aleph;
        }

        public override void AI()
        {
            if (NPC.localAI[0] == 0)
            {
                NPC.frame.Y = Main.rand.Next(3);
            }
            NPC.rotation += 0.01f;

            //NPC.velocity = NPC.velocity.RotatedBy(NPC.ai[0]);

            if (Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith);
                Main.dust[d].noGravity = true;
            }

            if (NPC.timeLeft < 10)
                NPC.scale -= 0.05f;

            NPC.ai[1]++;
            if ((NPC.ai[1] > 120 || Collision.SolidCollision(NPC.position + NPC.velocity, NPC.width, NPC.height)) && Main.netMode != NetmodeID.MultiplayerClient)
                NPC.StrikeInstantKill();
        }

        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Vomit>(), 300);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[NPC.type].Value;
            Vector2 pos = NPC.Center + Vector2.UnitY * NPC.gfxOffY - Main.screenPosition;
            Rectangle frame = new Rectangle(0, 0, tex.Width, tex.Height / 3);
            frame.Y = frame.Height * NPC.frame.Y;
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(tex, pos, frame, drawColor, NPC.rotation, origin, NPC.scale, 0f, 0);
            return false;
        }
    }
}

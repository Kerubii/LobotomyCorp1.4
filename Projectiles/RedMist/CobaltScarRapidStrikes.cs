using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
    public class CobaltScarRapidStrikes : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/Slash2A";

        public override void SetDefaults()
        {
            Projectile.height = 24;
            Projectile.width = 24;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 15;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.timeLeft;

            Projectile.hide = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Main.rand.Next(104, 182);
                Projectile.rotation = Main.rand.NextFloat(6.28f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = SpearExtender.SpearTrail;
            Rectangle frame = tex.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);
            float prog = Projectile.timeLeft / 15f;
            Vector2 position = Projectile.Center - new Vector2((Projectile.localAI[0] * (1f - (float)Math.Sin(1.57f - (1f - prog) * 1.57f))) - (Projectile.localAI[0] / 2), 0).RotatedBy(Projectile.rotation);
            prog = (float)Math.Sin(prog * 3.14f);
            if (prog > 0.9f)
                prog = 0.9f;
            Vector2 scale = new Vector2(Projectile.localAI[0] * prog * 1.4f / frame.Width, 0.1f);
            for (int i = -1; i <= 1; i++)
            {
                Vector2 offset = new Vector2(0, 14 * i).RotatedBy(Projectile.rotation);
                Main.EntitySpriteDraw(tex, position + offset - Main.screenPosition, frame, Color.Red, Projectile.rotation - 3.14f, origin, scale, 0);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust d = Main.dust[Dust.NewDust(target.position, target.width, target.height, DustID.Blood)];
                d.noGravity = true;
                d.velocity *= 1.2f;
                d.fadeIn = 0.5f;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
    }
}
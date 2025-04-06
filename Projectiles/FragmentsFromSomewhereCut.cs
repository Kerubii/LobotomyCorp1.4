using LobotomyCorp.Misc.Dusts;
using LobotomyCorp.ModSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Rocks;
using Steamworks;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class FragmentsFromSomewhereCut : ModProjectile
	{
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 300;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hide = true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > Projectile.ai[0])
            {
                Projectile.timeLeft = (int)Projectile.ai[0];
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.netMode != NetmodeID.Server)
                ModContent.GetInstance<ScreenSystem>().FragmentScreenActivate();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int type = ModContent.DustType<WhiteDust>();// Main.rand.Next(71, 74);
                Dust d = Dust.NewDustPerfect(Projectile.Center, type);
                float length = 192 * Projectile.ai[1] / 2;
                Vector2 offset = new Vector2(Main.rand.NextFloat(-length, length), 0).RotatedBy(Projectile.rotation);
                d.position += offset;
                d.noGravity = true;
                d.fadeIn = 1.5f;
                d.color = LobotomyCorp.FragmentShaderColor;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float prog = Projectile.ai[1] * 2 * (1f - Projectile.timeLeft / Projectile.ai[0]);
            if (prog > Projectile.ai[1])
                prog = Projectile.ai[1];
            float length = (192 * prog) / 2;
            Vector2 line = new Vector2(length, 0).RotatedBy(Projectile.rotation);
            return Collision.CheckAABBvLineCollision2(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - line, Projectile.Center + line);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = LobotomyCorp.FragmentShaderColor;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 scale = new Vector2(Projectile.ai[1] * 2 * (1f - Projectile.timeLeft / Projectile.ai[0]), (float)Math.Sin(3.14f * (Projectile.timeLeft / Projectile.ai[0])));
            if (scale.X > Projectile.ai[1])
                scale.X = Projectile.ai[1];
            Vector2 origin = tex.Size() / 2;
            //origin.X = 0;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenLastPosition + Vector2.UnitY * Projectile.gfxOffY, tex.Frame(), lightColor, Projectile.rotation, origin, scale, 0);
            return false;
        }
    }
}

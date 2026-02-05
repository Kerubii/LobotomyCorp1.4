using System;
using System.Collections.Generic;
using LobotomyCorp.Items;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.Player;
using static tModPorter.ProgressUpdate;

namespace LobotomyCorp.Projectiles.Realized
{
    public class SmileRBite: ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_644";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < Projectile.timeLeft)
                Projectile.timeLeft = (int)Projectile.ai[0];
            Player owner = Main.player[Projectile.owner];
            float rotation = Projectile.velocity.ToRotation();

            Projectile.rotation = rotation;
            Vector2 Offset = new Vector2(Projectile.ai[1], 45 * Projectile.timeLeft / Projectile.ai[0] * Projectile.ai[2]).RotatedBy(Projectile.rotation);
            Projectile.Center = LobCorpLight.LobItemLocation(owner, TextureAssets.Item[owner.HeldItem.type].Value.Frame(), rotation, Projectile.spriteDirection) + Offset;

            LobotomyAlephPlayer modPlayer = owner.GetModPlayer<LobotomyAlephPlayer>();
            foreach (Projectile p in Main.projectile)
            {
                if (p.active && (p.type == ModContent.ProjectileType<SmileCorpse>() || p.type == ModContent.ProjectileType<SmileCorpseSmall>()) && Projectile.Hitbox.Intersects(p.Hitbox) && Projectile.ai[1] > 30)
                {
                    modPlayer.SmileConsumeCorpse(p);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = new Vector2(0, 5 * Projectile.ai[2]).RotatedBy(Projectile.rotation);
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, vel.X, vel.Y);
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Player owner = Main.player[Projectile.owner];
                if (target.boss)
                {
                    owner.GetModPlayer<LobotomyAlephPlayer>().SmileCreateCorpseBoss(target);
                }
                if (target.life <= 0)
                {
                    owner.GetModPlayer<LobotomyAlephPlayer>().SmileCreateCorpse(target);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            float rand = Main.rand.NextFloat(0.4f);
            for (int i = 0; i < 16; i++)
            {
                float rot = i / 16f * 6.28f + rand;
                Vector2 Ellipse = new Vector2(8f * (float)Math.Cos(rot), 3f * (float)Math.Sin(rot)).RotatedBy(Projectile.rotation);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Wraith, Ellipse);
                d.noGravity = true;
                d.fadeIn = 1.2f;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 Pos = Projectile.Center;
            DrawTeeth(tex, Pos, Color.DarkRed, (float)Math.Sin((float)Projectile.timeLeft / Projectile.ai[0]), Projectile.rotation);
            return false;
        }

        private void DrawTeeth(Texture2D tex, Vector2 pos, Color color, float opacity, float rot)
        {
            Rectangle frame = tex.Frame();
            color *= opacity;
            Vector2 offset = new Vector2(10f, 0).RotatedBy(rot);
            for (int i = -2; i <= 2; i++)
            {
                Vector2 teethPos = offset * i;
                Main.EntitySpriteDraw(tex, pos - Main.screenPosition + teethPos, frame, color, rot, frame.Size() / 2, 1f, 0);
            }
        }
    }
}

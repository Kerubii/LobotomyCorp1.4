using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class HOUSE : ModProjectile
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("ROADA HOME DA");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 13;
        }

        public override void SetDefaults()
        {
            Projectile.width = 164;
            Projectile.height = 128;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 2f;
            Projectile.timeLeft = 300;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.extraUpdates = 3;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 600;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 15)
                Projectile.alpha -= 15;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 0)
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                Projectile.ai[0]++;
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/House_HouseBoom") with {Volume = 0.25f}, Projectile.Center);
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 position = Projectile.Center - Main.screenPosition;
            float alpha = ((float)(255f - (float)Projectile.alpha) / 255f);
                for (int i = 0; i < 4; i++)
                {
                    position = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;

                    Color color = lightColor * ((float)(255 - Projectile.alpha) / 255f) * 0.5f;
                    color *= (4f - i) / 4f;

                    Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, new Microsoft.Xna.Framework.Rectangle?
                            (
                                new Rectangle
                                (
                                    0, 0, TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height()
                                )
                            ),
                    color * alpha, Projectile.rotation, Projectile.Size / 4, Projectile.scale, SpriteEffects.None, 0);
                }
            position = Projectile.Center - Main.screenPosition;
            position.Y += Projectile.gfxOffY;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, new Microsoft.Xna.Framework.Rectangle?
                                    (
                                        new Rectangle
                                        (
                                            0, 0, TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height()
                                        )
                                    ),
                lightColor * alpha, Projectile.rotation, Projectile.Size / 4, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.position.Y + Projectile.height >= Main.player[Projectile.owner].position.Y + Main.player[Projectile.owner].height - 10)
            {
                fallThrough = false;
            }
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
    }
}

using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Xml.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class LaetitiaFriendRocket: ModProjectile
	{
		public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }    
            
        public override void AI() {
            if (Main.npc[(int)Projectile.ai[0]].life <= 0 || !Main.npc[(int)Projectile.ai[0]].active)
                Projectile.ai[0] = -1;

            if (Projectile.ai[0] < 0)
            {
                float dist = 10000;
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.CanBeChasedBy(Projectile))
                    {
                        float curDist = n.Center.Distance(Projectile.Center);
                        if (curDist > dist)
                        {
                            dist = curDist;
                            Projectile.ai[0] = n.whoAmI;
                        }
                    }
                }
            }

            if (Main.rand.NextBool(15))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith);
                Main.dust[d].noGravity = true;
                Main.dust[d].fadeIn = 1.2f;
            }

            Projectile.ai[1]++;
            if (Projectile.ai[1] < 25)
            {
                Projectile.scale = 1f + 0.5f * (float)Math.Sin(3.14f * Projectile.ai[1] / 25f);
                if (Projectile.ai[1] == 1)
                {
                    Projectile.rotation = Main.rand.NextFloat(6.28f);
                }
                Projectile.velocity *= 0.98f;
            }
            else
            {
                if (Projectile.ai[0] < 0)
                    return;

                NPC target = Main.npc[(int)Projectile.ai[0]];
                AIHelper.ChaseTargetLerp(Projectile.Center, target.Center, ref Projectile.velocity, 16f, 0.2f);
                Projectile.rotation = Terraria.Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation(), MathHelper.ToRadians(5));
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[1] < 30 || (Projectile.ai[0] > -1 && target.whoAmI != (int)Projectile.ai[0]))
                return false;

            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith);
                Main.dust[d].velocity = Vector2.Normalize(Projectile.velocity) * 2f;
                Main.dust[d].noGravity = true;
                Main.dust[d].fadeIn = 1.2f;
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LaetitiaExplosion>(), Projectile.damage, 0, Projectile.owner);
            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Abnormalities/WitchMonster_Death") with { Volume = 0.2f }, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = tex.Frame(1, Main.projFrames[Projectile.type]);
            Vector2 Position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);//.RotatedBy(Projectile.rotation);
            Vector2 origin = frame.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : 0;
            Main.EntitySpriteDraw(tex, Position, frame, lightColor, Projectile.rotation, origin, Projectile.scale, effect);
            return false;
        }
    }
}

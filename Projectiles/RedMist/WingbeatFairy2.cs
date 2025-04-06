using LobotomyCorp.NPCs.RedMist;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class WingbeatFairy2 : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Projectiles/WingbeatFairy";

        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 6;
        }

		public override void SetDefaults() {
			Projectile.width = 30;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
            Projectile.timeLeft = 6000;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;

            Projectile.localNPCHitCooldown = 5;
            Projectile.usesLocalNPCImmunity = true;
		}

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[2]++;

                if (Projectile.ai[2] > 15)
                    Projectile.velocity *= 0.95f;
                if (Projectile.ai[2] > 30)
                {
                    Projectile.ai[0] = -1;
                    Projectile.ai[2] = 0;
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 180);
                        Dust dust = Main.dust[d];
                        dust.noGravity = true;
                    }
                }
            }
            else if (Projectile.ai[0] > 0)
            {
                NPC n = Main.npc[(int)Projectile.ai[1] - 1];
                if (!n.active || n.life <= 0)
                {
                    Projectile.ai[0] = -1;
                }
                Projectile.ai[2]++;
                if (Projectile.ai[2] == 0)
                    Projectile.ai[2]++;
                Projectile.velocity *= 0.9f;
                if (Projectile.ai[2] > 15)
                {
                    Vector2 delNorm = n.Center - Projectile.Center;
                    Vector2 delt = delNorm;
                    delNorm.Normalize();
                    float extra = (n.width > n.height ? n.width : n.height);
                    float randRot = Main.rand.NextFloat(-0.2f, 0.2f);
                    for (int i = 0; i < 30; i++)
                    {
                        float rand = Main.rand.NextFloat(delt.Length() + extra);

                        int d = Dust.NewDust(Projectile.position + delNorm.RotatedBy(randRot) * rand, Projectile.width, Projectile.height, 180);
                        Dust dust = Main.dust[d];
                        dust.noGravity = true;
                    }

                    Projectile.Center = n.Center + delNorm.RotatedBy(randRot) * extra * 1.5f;
                    Projectile.velocity = delNorm * 4;
                    Projectile.ai[2] = 0;
                }
                if (Projectile.ai[2] < 0)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 180);
                    Dust dust = Main.dust[d];
                    dust.noGravity = true;

                    Vector2 delta = n.Center - Projectile.Center;
                    float extra = (n.width > n.height ? n.width : n.height) * 4;
                    if (delta.LengthSquared() > extra * extra)
                    {
                        float num1 = delta.Length();
                        float Speed = 16f;
                        Speed += num1 / 50f;

                        int num2 = 50;
                        delta.Normalize();
                        delta *= Speed;
                        Projectile.velocity = (Projectile.velocity * (float)(num2 - 1) + delta) / num2;
                    }
                    else
                    {
                        Projectile.velocity *= 0.9f;
                    }
                }
            }
            else
            {
                if (Projectile.ai[0] == -1)
                {
                    if (Projectile.velocity.LengthSquared() > 0)
                    {
                        Projectile.velocity *= 0.95f;
                        if (Projectile.velocity.LengthSquared() < 16)
                        {
                            Projectile.velocity *= 0;
                            Projectile.ai[0]--;
                        }
                    }
                }
                else
                {
                    Player owner = Main.player[Projectile.owner];

                    Vector2 delta = owner.Center - Projectile.Center;

                    if (delta.Length() < 32)
                        Projectile.Kill();

                    float num1 = delta.Length();
                    float Speed = 16f;
                    Speed += num1 / 50f;

                    int num2 = 50;
                    delta.Normalize();
                    delta *= Speed;
                    Projectile.velocity = (Projectile.velocity * (float)(num2 - 1) + delta) / num2;
                }
            }

            if (Projectile.frameCounter++ > 30)
            {
                Projectile.frameCounter = 0;
            }
            Projectile.frame = (int)(Projectile.frameCounter / 6);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1] = target.whoAmI + 1;
                Projectile.ai[2] = -30;
            }
            Projectile.ai[0] += damageDone;
            if (120 < Main.rand.Next((int)Projectile.ai[0]))
            {
                Projectile.ai[2] = Projectile.ai[0];
                Projectile.ai[0] = -1;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] < 0)
                return false;

            if (Projectile.ai[1] > 0)
            {
                if (target.whoAmI + 1 != Projectile.ai[1])
                    return false;
                else if (Projectile.ai[0] > 0 && Projectile.ai[2] == 0)
                    return true;
                else return false;
            }
            return base.CanHitNPC(target);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (Projectile.ai[1] == 0)
                return;

            NPC n = Main.npc[(int)Projectile.ai[1] - 1];
            if (!n.active || n.life <= 0)
            {
                return;
            }
            hitbox.X = (int)n.position.X;
            hitbox.Y = (int)n.position.Y;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 dir = new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(45) * i);
                Dust.NewDustPerfect(Projectile.Center + dir, 15, dir);
            }
            SoundEngine.PlaySound(SoundID.NPCHit5, Projectile.Center);
            float healamount = Projectile.ai[2] * 0.01f;
            if (healamount > 0.7f)
            {
                Player owner = Main.player[Projectile.owner];
                owner.Heal((int)healamount);
            }
        }
    }
}

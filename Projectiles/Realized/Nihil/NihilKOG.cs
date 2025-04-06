using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized.Nihil
{
	public class NihilKOG : ModProjectile
	{
        public override void SetDefaults() {
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;

			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
		}

        //Attack Parameters
        private readonly float attackDowntime = 90;
        private readonly float attackAnimation = 90;

        public override void AI()
        {
            //Main.NewText(Projectile.ai[0]);

			Player player = Main.player[Projectile.owner];

			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
			Vector2 restPosition = playerCenter + new Vector2(30, -player.height/2 - 4);
            Projectile.rotation = 0;

            //Return Logic
            int flyDistance = 1200;
            if (player.rocketDelay2 > 0)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            float distance = Vector2.Distance(Projectile.Center, player.Center);
            if (distance > 2000f)
            {
                Projectile.position.X = player.position.X + (float)(player.width / 2) - (float)(Projectile.width / 2);
                Projectile.position.Y = player.position.Y + (float)(player.height / 2) - (float)(Projectile.height / 2);
            }
            else if (distance > flyDistance || Math.Abs(Projectile.Center.Y - player.Center.Y) > 480f)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }

            if (Projectile.ai[0] != 0)
            {
                Projectile.tileCollide = false;
                float moveDistX = restPosition.X - Projectile.Center.X;
                float moveDistY = restPosition.Y - Projectile.Center.Y;
                float moveDist = (float)Math.Sqrt((double)(moveDistX * moveDistX + moveDistY * moveDistY));
                float maxSpeed = 10f;

                if (moveDist < 480f && Projectile.position.Y + (float)Projectile.height <= player.position.Y + (float)player.height && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }

                if (moveDist < 60f)
                {
                    moveDistX = Projectile.velocity.X;
                    moveDistY = Projectile.velocity.Y;
                }
                else
                {
                    moveDist = maxSpeed / moveDist;
                    moveDistX *= moveDist;
                    moveDistY *= moveDist;
                }

                float acceleration = 0.2f;
                if (Projectile.velocity.X < moveDistX)
                {
                    Projectile.velocity.X += acceleration;
                    if (Projectile.velocity.X < 0f)
                    {
                        Projectile.velocity.X += acceleration * 1.5f;
                    }
                }
                if (Projectile.velocity.X > moveDistX)
                {
                    Projectile.velocity.X -= acceleration;
                    if (Projectile.velocity.X > 0f)
                    {
                        Projectile.velocity.X -= acceleration * 1.5f;
                    }
                }
                if (Projectile.velocity.Y < moveDistY)
                {
                    Projectile.velocity.Y += acceleration;
                    if (Projectile.velocity.Y < 0f)
                    {
                        Projectile.velocity.Y += acceleration * 1.5f;
                    }
                }
                if (Projectile.velocity.Y > moveDistY)
                {
                    Projectile.velocity.Y -= acceleration;
                    if (Projectile.velocity.Y > 0f)
                    {
                        Projectile.velocity.Y -= acceleration * 1.5f;
                    }
                }
            }
			else
            {
                Vector2 targetPosition = restPosition;

                bool target = false;
                Vector2 targetCenter = Projectile.position;

                float dist = 1500f;
                float currentTarget = -1;
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    if (Main.npc[k].CanBeChasedBy(this))
                    {
                        NPC n = Main.npc[k];
                        float npcDist = Vector2.Distance(n.Center, playerCenter);
                        if (npcDist < dist)
                        {
                            if (currentTarget == -1 && npcDist <= dist)
                            {
                                dist = npcDist;
                            }
                            if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, n.position, n.width, n.height))
                            {
                                currentTarget = k;
                                target = true;
                                targetCenter = n.Center;
                                dist = npcDist;
                            }
                        }
                    }
                }

                if (target)
                {
                    targetPosition = targetCenter;
                    Projectile.rotation = (float)Math.Atan2(targetPosition.Y - Projectile.position.Y, targetPosition.X - Projectile.position.X);

                    if (Projectile.ai[1] == 0)
                    {
                        Projectile.spriteDirection = Projectile.Center.X < targetPosition.X ? 1 : -1;
                        Projectile.ai[1] = attackDowntime + attackAnimation;
                    }
                }

                dist = (targetPosition - Projectile.Center).Length();
                float range = 60;
                float acceleration = 0.4f;
                float maxSpeed = 12f;

                if ((target && dist > range) || !target)
                {
                    if (Projectile.position.X < targetPosition.X)
                    {
                        Projectile.velocity.X += acceleration;
                        if (Projectile.velocity.X < 0f)
                            Projectile.velocity.X += acceleration * 1.5f;
                        if (Projectile.velocity.X > maxSpeed)
                            Projectile.velocity.X = maxSpeed;
                    }
                    if (Projectile.position.X > targetPosition.X)
                    {
                        Projectile.velocity.X -= acceleration;
                        if (Projectile.velocity.X > 0f)
                            Projectile.velocity.X -= acceleration * 1.5f;
                        if (Projectile.velocity.X < -maxSpeed)
                            Projectile.velocity.X = -maxSpeed;
                    }
                    if (Projectile.position.Y < targetPosition.Y)
                    {
                        Projectile.velocity.Y += acceleration;
                        if (Projectile.velocity.Y < 0f)
                            Projectile.velocity.Y += acceleration * 1.5f;
                        if (Projectile.velocity.Y > maxSpeed)
                            Projectile.velocity.Y -= maxSpeed;
                    }
                    if (Projectile.position.Y > targetPosition.Y)
                    {
                        Projectile.velocity.Y -= acceleration;
                        if (Projectile.velocity.Y > 0f)
                            Projectile.velocity.Y -= acceleration * 1.5f;
                        if (Projectile.velocity.Y < -maxSpeed)
                            Projectile.velocity.Y = -maxSpeed;
                    }
                }
                else
                {
                    Projectile.velocity *= 0.9f;
                    if (Projectile.velocity.Length() < 1f)
                        Projectile.velocity *= 0;
                }

            }

            if (Projectile.ai[1] > 0)
            {
                if (Projectile.ai[1] > attackDowntime)
                {
                    if (Projectile.ai[1] > attackDowntime + attackAnimation * 0.25f && Projectile.ai[1] % 10 == 0)
                    {
                        Vector2 vel = new Vector2(24, 0).RotatedBy(Projectile.rotation);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Main.rand.Next(16), Main.rand.Next(16)), vel, ModContent.ProjectileType<NihilKOGFists>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 4);
                    }
                    float prog = (Projectile.ai[1] - attackDowntime) / (float)attackAnimation;

                    Projectile.localAI[0] = prog;
                }
                Projectile.ai[1]--;
            }

            if (player.HeldItem.type != ModContent.ItemType<Items.Ruina.Natural.NihilR>() || !player.GetModPlayer<LobotomyAlephPlayer>().NihilActive)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Projectiles/Realized/Nihil/NihilCard").Value;
            Vector2 position = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Rectangle frame = texture.Frame(1, 5, 0, 1);
            Vector2 origin = frame.Size() / 2;

            Main.EntitySpriteDraw(texture, position, frame, lightColor, Projectile.rotation, origin, 0.8f, 0f, 0);

            if (Projectile.ai[1] > attackDowntime)
            {
                texture = TextureAssets.Projectile[Projectile.type].Value;
                Color color = lightColor;
                float prog = Projectile.localAI[0];
                float rot = Projectile.rotation;
                bool draw = false;
                if (prog > 0.1f && prog <= 0.25f)
                {
                    prog = 1f - (prog - 0.1f) / 0.15f;

                    rot += MathHelper.ToRadians(45 - 135 * (float)Math.Sin(1.57f * prog)) * Projectile.spriteDirection;
                    draw = true;
                }
                else if (prog <= 0.1f)
                {
                    prog = 1f - prog / 0.1f;

                    rot += MathHelper.ToRadians(-95 - 10 * prog) * Projectile.spriteDirection;
                    color *= 1f - prog;
                    draw = true;
                }

                position += new Vector2(16, 0).RotatedBy(rot);
                origin = new Vector2(0, 31);
                if (draw)
                    Main.EntitySpriteDraw(texture, position, null, color, rot, origin, 1f, 0f, 0);
            }
            return false;
        }

        public override bool? CanDamage()
        {
            return false;
        }
    }

    public class NihilKOGFists : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/Realized/Nihil/NihilKOG";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 22;
            Projectile.aiStyle = -1;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.8f;
            Projectile.alpha += 7;
            Projectile.rotation = Projectile.velocity.ToRotation() + 0.785f;
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = tex.Frame();
            Vector2 origin = frame.Size() / 2;

            Color color2 = lightColor * (1f - Projectile.alpha / 255f);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(0,Projectile.gfxOffY), (Rectangle?)frame, color2, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}

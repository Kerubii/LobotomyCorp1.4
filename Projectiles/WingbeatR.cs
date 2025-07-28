using LobotomyCorp.Buffs;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Players;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class WingbeatR : ModProjectile
	{
        public const int WingbeatRDistance = 400;

		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Wingbeat");
        }

		public override void SetDefaults() {
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.alpha = 0;
            Projectile.timeLeft = 600;

			//Projectile.hide = true;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
			//Projectile.tileCollide = false;
			Projectile.friendly = true;

            Projectile.extraUpdates = 5;

		}

		public override void AI() {
            
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1] = -1;

                Projectile.localAI[0] = 1 + Main.rand.Next(2);

                float distance = WingbeatRDistance;
                foreach (NPC n in Main.npc)
                {
                    if (n.active && !n.friendly && !n.dontTakeDamage && n.HasBuff<Buffs.Fairy>())
                    {
                        Vector2 delta = n.Center - Main.player[Projectile.owner].Center;
                        float currDistance = delta.Length();
                        if (n.width > n.height)
                            currDistance -= n.width / 2;
                        else
                            currDistance -= n.height / 2;

                        if (currDistance < distance)
                        {
                            distance = currDistance;
                            float x = Math.Max(1f, distance / 200f);
                            delta.Normalize();
                            Projectile.velocity = delta * Projectile.velocity.Length() * x;
                            Main.player[Projectile.owner].direction = Math.Sign(Projectile.velocity.X);
                        }
                    }
                }
            }

            bool advance = true;
            if (Projectile.ai[0] <= 15 && Projectile.ai[1] > -1)
            {
                NPC n = Main.npc[(int)Projectile.ai[1]];

                //foreach (NPC n in Main.npc)
                //{
                if (n.active && !n.friendly && !n.dontTakeDamage && Projectile.Colliding(Projectile.getRect(), n.getRect()))
                {
                    if (Projectile.ai[0] > 7)
                        Projectile.ai[0] = 7;
                    advance = false;
                }
                //}
            }

            if (advance)
                Projectile.ai[0]++;

            Player owner = Main.player[Projectile.owner];
            owner.armorEffectDrawShadow = true;

            if (Projectile.ai[0] > 15)
            {
                Projectile.width = 160;
                Projectile.height = 160;
                Projectile.extraUpdates = 0;

                //Projectile.velocity *= 0;
                Projectile.Center = owner.MountedCenter;

                if (Projectile.ai[0] < 20)
                {
                    Vector2 center = owner.MountedCenter;
                    for (int i = 0; i < 3; i++)
                    {
                        float x = 76 - Main.rand.Next(12);
                        float y = 0;

                        Vector2 position = center + new Vector2(x, y).RotatedByRandom(6.28f);

                        if (Projectile.localAI[0] == 2)
                        {
                            float angle = Main.rand.NextFloat(6.28f);
                            x = (float)Math.Cos(angle) * x;
                            y = (float)Math.Sin(angle) * (x / 3);

                            position = center + new Vector2(x, y).RotatedByRandom(Projectile.velocity.ToRotation());
                        }

                        Dust dust2 = Dust.NewDustPerfect(position, 180);
                        dust2.noGravity = true;
                        dust2.velocity = Vector2.Normalize(position - center) * 2f;
                        dust2.fadeIn = 1.2f;
                    }

                    if (Projectile.localAI[1] != 0)
                    {
                        float x = 76 - Main.rand.Next(12);
                        float y = 0;

                        Vector2 position = center + new Vector2(x, y).RotatedByRandom(6.28f);

                        if (Projectile.localAI[0] == 2)
                        {
                            x = (float)Math.Cos(Main.rand.NextFloat(6.28f)) * x;
                            y = (float)Math.Sin(Main.rand.NextFloat(6.28f)) * (x / 3);

                            position = center + new Vector2(x, y).RotatedByRandom(Projectile.velocity.ToRotation());
                        }

                        Dust dust2 = Dust.NewDustPerfect(position, DustID.Blood);
                        dust2.noGravity = true;
                        dust2.velocity = Vector2.Normalize(position - center) * 2f;
                        dust2.fadeIn = 1.2f;
                    }
                }

                if (Projectile.ai[1] >= 0)
                {
                    owner.velocity *= 0;
                    owner.immune = true;
                    owner.immuneNoBlink = true;
                    owner.immuneTime = 6;

                    if (owner.itemTime > 5)
                    {
                        owner.itemTime = 5;
                        owner.itemAnimation = 5;
                    }
                }

                if (Projectile.ai[0] > 30)
                    Projectile.Kill();
                return;
            }

            int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 180);
            Dust dust = Main.dust[d];
            dust.noGravity = true;
            Vector2 vel = Vector2.Normalize(Projectile.velocity);
            dust.velocity = vel * (Projectile.ai[0] / 15 * -10);
            dust.fadeIn = 1.6f;
            dust.scale = 1.4f;

            owner.itemTime = 88;
            owner.itemAnimation = 88;
            //if (!Collision.SolidTiles)
            owner.velocity = Projectile.velocity * 0.8f;
            owner.Teleport(Projectile.Center - owner.Size / 2, -1, -1);

            owner.immune = true;
            owner.immuneNoBlink = true;
            owner.immuneTime = 5;

            //Main.NewText(Projectile.ai[1]);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff<Buffs.Fairy>())
            {
                modifiers.FinalDamage *= 1.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //target.immune[Projectile.owner] = 10;
            bool hasFairy = target.HasBuff<Buffs.Fairy>();
            if (damageDone > 10)
            {
                float healPercent = 0;
                if (Projectile.ai[1] == -1)
                    healPercent += 0.1f;
                if (hasFairy)
                    healPercent += 0.1f;
                int heal = (int)(damageDone * 0.1f);
                if (heal > 0)
                {
                    Main.player[Projectile.owner].GetModPlayer<LobotomyZayinPlayer>().WingbeatStoreHeal(heal);
                }
            }

            /*if (damageDone > 10)
            {
                int heal = 0;
                if (Projectile.ai[1] == -1)
                    heal += (int)(damageDone * 0.1f);
                if (hasFairy)
                    heal += (int)(damageDone * 0.1f);
                if (heal > 0)
                {
                    Main.player[Projectile.owner].statLife += heal;
                    Main.player[Projectile.owner].HealEffect(heal);
                }
            }*/
            if (Projectile.ai[0] <= 15 && Projectile.ai[1] < 0)
            {
                Projectile.ai[1] = target.whoAmI;
                if (Main.rand.NextBool(7))
                {
                    //Vector2 pos = Main.player[Projectile.owner].Center;
                    //for (int i = 0; i < 16; i++)
                    //{
                    //    Dust d = Dust.NewDustPerfect(pos, DustID.Blood, new Vector2(4, 0).RotatedBy(6.28f * (i/16f)));
                    //    d.noGravity = true;
                    //}

                    target.RequestBuffRemoval(ModContent.BuffType<Fairy>());
                    Projectile.ai[1] = -1;
                }
            }
            else if (Main.player[Projectile.owner].itemTime > 5)
            {
                Main.player[Projectile.owner].itemTime = 5;
                Main.player[Projectile.owner].itemAnimation = 5;
            }

            if (Projectile.ai[0] > 15 && Projectile.ai[1] < 0)
                Projectile.ai[1] = -2;

            for (int i = 0; i < 4;i++)
            {
                Dust.NewDust(target.Center, 1, 1, DustID.Blood);
            }

            if (target.life <= 0)
            {
                //Main.player[Projectile.owner].AddBuff(ModContent.BuffType<Buffs.Festival>(), 300);
                int heal = (int)(target.lifeMax * 0.1f);
                Main.player[Projectile.owner].GetModPlayer<LobotomyZayinPlayer>().WingbeatStoreHeal(heal);
            }
            if (Main.myPlayer == Projectile.owner)
                ModContent.GetInstance<ScreenSystem>().ScreenShake(15, 4f, 0, false);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0;
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[0] <= 15;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] <= 20)
                return null;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] < 15)
                return false;

            float prog = (Projectile.ai[0] - 15f) / 15f;

            Player player = Main.player[Projectile.owner];
            float opacity = 1f;
            if (prog > 0.5f)
                opacity *= 1f - (prog - 0.5f) / 0.5f;
            CustomShaderData shader = LobotomyCorp.LobcorpShaders["SwingTrail"].UseOpacity(opacity);
            shader.UseImage1(Mod, "Misc/FX_Tex_Trail1");
            shader.UseImage2(Mod, "Misc/FX_Tex_Trail1");
            shader.UseImage3(Mod, "Misc/Worley");

            int dir = player.direction;

            if (Projectile.localAI[0] == 1)
            {
                SlashTrail trail = new SlashTrail(24, 1.57f);
                trail.color = new Color(158, 255, 249);

                float rot = -MathHelper.ToRadians(135f) * dir;
                float rotationOffset = 6.8f * (float)Math.Sin(prog * 1.57f) * dir;
                trail.DrawCircle(player.Center, Projectile.velocity.ToRotation() + rot + rotationOffset, dir, 64, 64, shader);
            }
            else
            {
                //Im a fucking idiot wtf is this code, too diferent than circle >:V
                SlashTrail trail = new SlashTrail(36, 12, 0);
                trail.color = new Color(158, 255, 249);

                float rot = MathHelper.ToRadians(30);
                float rotationOffset = -3.14f * (float)Math.Sin(prog * 1.57f);
                trail.DrawEllipse(player.Center, Projectile.velocity.ToRotation(), (rot + rotationOffset) * dir, dir * -1, 102, 40, 128, shader);
            }

            return false;
        }
    }
}

using System;
using System.IO;
using LobotomyCorp.Items;
using LobotomyCorp.Projectiles.RedMist;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class MeltyLove : ModProjectile
	{
        public static Asset<Texture2D> MeltyLoveStickTex;

        public override void Load()
        {
            MeltyLoveStickTex = ModContent.Request<Texture2D>(Texture + "Stick");
        }

		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Gunk");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 300;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            StickToTarget = -1;
        }

        public int StickToTarget;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(StickToTarget);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            StickToTarget = reader.ReadInt32();
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (Projectile.ai[1] == 0)
            {
                if (owner.channel)
                {
                    if (Projectile.ai[0] == 80)
                    {
                        owner.channel = false;
                    }

                    if (Projectile.ai[0] < 80)
                        Projectile.ai[0]++;
                    owner.heldProj = Projectile.whoAmI;
                    Projectile.alpha = 255;
                    Projectile.timeLeft = 300;
                    Projectile.Center = owner.MountedCenter;

                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 delta = Main.MouseWorld - owner.MountedCenter;
                        delta.Normalize();
                        Projectile.velocity = Projectile.velocity.Length() * delta;
                        owner.direction = Math.Sign(delta.X);

                        if (Projectile.ai[0] % 20 == 0 && owner.CheckMana(owner.GetManaCost(owner.HeldItem) / 2, true))
                        {
                            Vector2 vel = Vector2.Normalize(Projectile.velocity).RotatedByRandom(Main.rand.NextFloat(-0.5f, 0.5f));
                            vel *= Main.rand.Next(12, 16);

                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<MeltyLoveSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }

                    owner.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * owner.direction, Projectile.velocity.X * owner.direction);
                    owner.itemTime = owner.itemAnimation = owner.itemAnimationMax;

                    return;
                }
                for (int i = 0; i < 10; i++)
                {
                    Vector2 dustVel = Projectile.velocity * Main.rand.NextFloat(-0.5f, 1.5f);
                    dustVel = dustVel.RotatedByRandom(MathHelper.ToRadians(20));
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 251, dustVel.X, dustVel.Y);
                }

                SoundEngine.PlaySound(LobotomyCorp.WeaponSound("Slime"), Projectile.position);

                float scale = Projectile.ai[0] / 80;
                if (scale > 1f)
                    scale = 1f;

                Projectile.velocity *= 1f + (0.3f * Projectile.ai[0] / 80f);

                Projectile.ai[1] = 1;
                Projectile.scale = 1f + scale;
                Projectile.alpha = 0;
            }
            else if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 10)
                {
                    Projectile.velocity.Y += 0.12f;
                }
            }
            else
            {
                Projectile.ai[1]--;
                Projectile.Center = Main.npc[StickToTarget].Center + Projectile.velocity;

                if (Projectile.timeLeft < 15)
                Projectile.scale -= 0.005f;

                if (Projectile.ai[1] == 60)
                    Projectile.Kill();
                return;
            }

            if (++Projectile.ai[2] % (int)(60 - 50 * (Projectile.ai[0] / 80f)) == 0)
            {
                /*float speed = 7.6f + 2f * (Projectile.ai[0] / 80f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, speed), ModContent.ProjectileType<MeltyLoveSmall>(), (int)(Projectile.damage * (1f + damageBoost())), Projectile.knockBack, Projectile.owner);*/
            }

            if (Projectile.localAI[1]++ == 2)
            {
                Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 251, -Projectile.velocity.X / 2, -Projectile.velocity.Y / 2)].noGravity = true;
                Projectile.localAI[1] = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool ShouldUpdatePosition()
        {
            Player owner = Main.player[Projectile.owner];
            return !(owner.channel && Projectile.ai[1] == 0 && StickToTarget > 0);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 251);
            }

            if (Projectile.ai[1] < 0)
            {
                int amount = 4;
                if (Projectile.ai[0] > 40)
                {
                    amount += (int)(4 * (Projectile.ai[0] - 40) / 40f);
                }
                for (int i = 0; i < amount; i++)
                {
                    float speed = 7;
                    if (Projectile.ai[0] > 40)
                    {
                        speed *= 1f + 0.5f * ((Projectile.ai[0] - 40) / 40f);
                    }
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 velRand = new Vector2(speed, 0).RotateRandom(6.28f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velRand, ModContent.ProjectileType<MeltyLoveSmall>(), (int)(Projectile.damage * (1f + damageBoost()) / 2), Projectile.knockBack, Projectile.owner, 0, StickToTarget);
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Slow>(), 300);
            if (Projectile.ai[0] < 20 || Projectile.ai[1] < 0)
                return;
            
            Projectile.ai[1] = -1;
            StickToTarget = target.whoAmI;
            Projectile.velocity = Projectile.Center - target.Center + Projectile.velocity;
            Projectile.frame = Main.rand.Next(3);
            Projectile.rotation = Main.rand.NextFloat(6.28f);
            Projectile.timeLeft = 60 * 3;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (Projectile.ai[1] < 0)
            {
                int size = 70;
                hitbox = new Rectangle(hitbox.Center.X - size / 2, hitbox.Center.Y - size / 2, size, size);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[1] < 0)
            {
                modifiers.SourceDamage -= 0.5f;
                return;

            }
            float dmgMult = damageBoost();
            modifiers.FinalDamage += dmgMult;
            base.ModifyHitNPC(target, ref modifiers);
        }

        private float damageBoost()
        {
            return 4f * (Projectile.ai[0] / 80f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (StickToTarget > 0)
            {
                Texture2D tex2 = MeltyLoveStickTex.Value;
                Rectangle frame = tex2.Frame(1, 3, frameY: Projectile.frame);
                Main.EntitySpriteDraw(
                    tex2,
                    Projectile.Center - Main.screenPosition,
                    frame,
                    lightColor * (1f - Projectile.alpha / 255f),
                    Projectile.rotation,
                    frame.Size() / 2,
                    Projectile.scale,
                    0,
                    0);

                return false;
            }

            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                tex.Frame(),
                lightColor * (1f - Projectile.alpha / 255f),
                Projectile.rotation,
                tex.Size() / 2 + new Vector2(4, 0),
                Projectile.scale,
                0,
                0);
            return false;
        }
    }

    public class MeltyLoveSmall : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/MeltyLove";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gunk");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 120;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            //Projectile.alpha += (int)(255 / 30);
            if (Projectile.timeLeft < 15)
                Projectile.scale -= (1f / 15f);
            if (Projectile.localAI[1]++ == 2)
            {
                Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 251, -Projectile.velocity.X / 2, -Projectile.velocity.Y / 2)].noGravity = true;
                Projectile.localAI[1] = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.ai[2] < 0)
                return;

            Projectile.velocity.Y += 0.12f;
            if (Projectile.velocity.Y > 24f)
                Projectile.velocity.Y = 24f;            
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Main.myPlayer == Projectile.owner && Projectile.ai[2] == 0)
            {
                if (LobItemBase.RedMistMaskUpgrade(Main.player[Projectile.owner], RiskLevel.Aleph))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AdorationSplashSlime>(), Projectile.damage, Projectile.knockBack, Projectile.owner, -1);
                }
            }

            return base.OnTileCollide(oldVelocity);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if ((int)Projectile.ai[1] == target.whoAmI)
                return false;
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Slow>(), 300);

            if (Main.myPlayer == Projectile.owner && Projectile.ai[2] == 0)
            {
                if (LobItemBase.RedMistMaskUpgrade(Main.player[Projectile.owner], RiskLevel.Aleph))
                {
                    Vector2 distance = Projectile.Center - target.Center;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, distance, ModContent.ProjectileType<AdorationSplashSlime>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                tex.Frame(),
                lightColor * (1f - Projectile.alpha / 255f),
                Projectile.rotation,
                tex.Size() / 2 + new Vector2(4, 0),
                Projectile.scale,
                0,
                0);
            return false;
        }
    }
}

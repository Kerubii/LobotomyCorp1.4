using System;
using LobotomyCorp.Buffs;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;
using LobotomyCorp.Utils;
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
    public class MimicryR : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Items/Ruina/Language/MimicryR";

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            //Projectile.aiStyle = -;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 300;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        private const int MimicryChargeMax = 60;
        private const int MimicryChargeLimit = 240;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            owner.heldProj = Projectile.whoAmI;
            int dir = owner.direction;
            float rotation = Projectile.velocity.ToRotation();
            float scale = owner.GetAdjustedItemScale(owner.HeldItem);

            Projectile.Center = owner.MountedCenter;
            if (Projectile.ai[0] <= 0)
            {
                float progress = 1f - (owner.itemAnimation - 5f) / (owner.itemAnimationMax - 5f);

                rotation += MathHelper.ToRadians(135 - 270 * (float)Math.Sin(1.57f * progress)) * dir;
                //scale += 0.8f * (float)Math.Sin(3.14f * progress);

                if (progress >= 1)
                {
                    Projectile.ai[0]++;
                }

                Projectile.spriteDirection = -1 * dir;
            }

            else if (Projectile.ai[0] == 1)
            {
                owner.itemAnimation = 5;
                owner.itemTime = 5;
                Projectile.timeLeft = 100;
                if (owner.channel)
                {
                    Projectile.ai[1]++;
                    if (Projectile.ai[1] == MimicryChargeMax)
                    {
                        if (!(owner.GetModPlayer<LobotomyAlephPlayer>().MimicryShell || owner.GetModPlayer<LobotomyAlephPlayer>().MimicryHusk))
                        {
                            owner.channel = false;
                            Projectile.ai[1]--;
                        }
                        else
                        {
                            //Create Dust Transformation effects
                            Projectile.scale = 1.2f;
                        }
                    }
                    if (Projectile.ai[1] > MimicryChargeLimit)
                    {
                        owner.channel = false;
                        Projectile.ai[1] = 0;
                    }

                    if (Projectile.ai[1] > MimicryChargeMax)
                    {
                        float lerp = Projectile.ai[1] - MimicryChargeMax;
                        if (lerp < 5)
                            scale *= 1f + 0.2f * (1f - lerp / 5f);

                        if (Projectile.ai[1] > MimicryChargeLimit - 60)
                        {
                            float prog = (Projectile.ai[1] - (MimicryChargeLimit - 60f)) / 60f;
                            Projectile.position.X += Main.rand.NextFloat(-8f, 8f) * prog;
                            Projectile.position.Y += Main.rand.NextFloat(-8f, 8f) * prog;
                        }
                    }
                    else
                    {
                        float prog = Projectile.ai[1] / 60f;
                        Projectile.position.X += Main.rand.NextFloat(-8f, 8f) * prog;
                        Projectile.position.Y += Main.rand.NextFloat(-8f, 8f) * prog;
                    }

                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 target = Main.MouseWorld - Projectile.Center;
                        float targetRot = target.ToRotation();
                        rotation = Terraria.Utils.AngleLerp(rotation, targetRot, MathHelper.ToRadians(8f));
                        Projectile.velocity = new Vector2(1, 0).RotatedBy(rotation);
                        owner.direction = Math.Sign(target.X);
                        Projectile.spriteDirection = -1 * owner.direction;
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    owner.itemTime = owner.itemAnimation = owner.itemAnimationMax;

                    //Do a normal second hit
                    if (Projectile.ai[1] < 60)
                    {
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Nullthing_Skill2_Hook") with { Volume = 0.25f }, Projectile.Center);

                        scale *= 1f;
                        Projectile.ai[0] = 2;
                        Projectile.spriteDirection = 1 * dir;
                    }
                    //Goodbye Attack
                    else
                    {
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/NothingThere_Goodbye") with { Volume = 0.25f }, Projectile.Center);
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Nullthing_Skill3_Finish") with { Volume = 0.25f }, Projectile.Center);

                        scale *= 1.4f;
                        Projectile.ai[0] = 3;
                        Projectile.spriteDirection = -1 * dir;
                    }
                }
                rotation -= MathHelper.ToRadians(135f * dir);
            }

            //Uncharged Slash
            else if (Projectile.ai[0] == 2)
            {
                if (Projectile.localAI[0] <= 0 && Main.myPlayer == Projectile.owner)
                {
                    Projectile.localAI[0]++;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.MountedCenter, Projectile.velocity, ModContent.ProjectileType<MimicryREffectSlash>(), 0, 0, Projectile.owner, Main.rand.Next(1, 5), owner.itemAnimationMax, 0);
                }

                float progress = 1f - (float)owner.itemAnimation / owner.itemAnimationMax;
                scale *= 1f + (float)Math.Sin(3.14f * progress);
                rotation += MathHelper.ToRadians(-135 + 270 * (float)Math.Sin(1.57f * progress)) * dir;
            }

            //Goodbye
            else if (Projectile.ai[0] == 3)
            {
                if (Projectile.localAI[0] <= 0 && Main.myPlayer == Projectile.owner)
                {
                    Projectile.localAI[0]++;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.MountedCenter, Projectile.velocity, ModContent.ProjectileType<MimicryREffectSlash>(), 0, 0, Projectile.owner, Main.rand.Next(1, 5), (int)(owner.itemAnimationMax * 0.6f), 60);
                }

                float time = .4f;
                float timeDelt = 1f - time;

                float progress = 1f - ((float)owner.itemAnimation / owner.itemAnimationMax);
                if (progress < time)
                {
                    progress = progress / time;
                    rotation += MathHelper.ToRadians(-135 + 270 * progress * dir);
                    if (progress <= 0.5f)
                        scale *= 1f + 0.2f + (float)Math.Sin(1.57f * progress);
                    else
                        scale *= 1f + 0.8f + 0.4f * (float)Math.Sin(1.57f + 1.57f * progress);
                }
                else
                {
                    int power = (int)(10 * (progress - time) / timeDelt);
                    if (power < 0)
                        power = 0;
                    rotation += MathHelper.ToRadians(164 - 30 * (float)Math.Pow(0.8f, power)) * dir;
                    scale *= 1f + 0.8f;
                }
                //rotation += MathHelper.ToRadians(-135 + 300 * (float)Math.Sin(1.57f * progress)) * dir;
            }

            Projectile.scale = scale;
            Projectile.rotation = rotation;
            if (owner.itemAnimation == 1)
            {
                if (Projectile.ai[0] == 3)
                {
                    owner.itemAnimation = 15;
                    owner.itemTime = 15;
                }
                Projectile.Kill();
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Height = hitbox.Width = (int)(hitbox.Width * Projectile.scale);
            Vector2 origin = Main.player[Projectile.owner].MountedCenter - hitbox.Size() / 2;

            float length = 60;
            if (Projectile.ai[0] == 3)
                length = 80;
            Vector2 target = new Vector2(length * Projectile.scale, 0).RotatedBy(Projectile.rotation);
            target = origin + target;
            hitbox.X = (int)target.X;
            hitbox.Y = (int)target.Y;
        }

        public bool SweetspotAngle(Vector2 targetCenter)
        {
            float angle = Projectile.velocity.ToRotation();
            float range = MathHelper.ToRadians(15);
            float targetAngle = (targetCenter - Projectile.Center).ToRotation();
            if (AIHelper.AngleDistance(angle, targetAngle) < range)
                return true;
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[1] < 60)
            {
                float boost = (Projectile.ai[1] / 60);
                modifiers.SourceDamage += boost;
            }
            else
            {
                Player owner = Main.player[Projectile.owner];
                float progress = 1f - ((float)owner.itemAnimation / owner.itemAnimationMax);
                if ((progress > .15f && progress < .25f) || SweetspotAngle(target.Center))
                    modifiers.SourceDamage += 10f;
                else
                    modifiers.SourceDamage += 1f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Main.player[Projectile.owner].attackCD > 0)
                return false;
            if (Projectile.ai[0] != 1)
                return null;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyAlephPlayer modPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyAlephPlayer>();
            target.immune[Projectile.owner] = Main.player[Projectile.owner].itemAnimation;
            Main.player[Projectile.owner].attackCD = Main.player[Projectile.owner].itemAnimationMax / 6;
            if (Projectile.ai[1] == 3)
            {
                Projectile.localAI[0] += 1;
            }    
            if (Projectile.ai[0] == 3)
            {
                Player owner = Main.player[Projectile.owner];
                float progress = 1f - ((float)owner.itemAnimation / owner.itemAnimationMax);
                if ((progress > .15f && progress < .25f) || SweetspotAngle(target.Center))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<MimicrySEffect>(), 0, 0, Projectile.owner, Main.player[Projectile.owner].direction);
                    if (modPlayer.MimicryShell)
                    {
                        modPlayer.MimicryIncreaseShellTime(60);
                    }
                    if (Projectile.ai[2] == 0 && target.type != NPCID.TargetDummy && NPCID.Sets.ProjectileNPC[target.type] == false)
                    {
                        Projectile.ai[2]++;
                        int heal = (int)(damageDone * 0.01f);
                        Player player = Main.player[Projectile.owner];
                        if (heal > player.statLifeMax2 * 0.05f)
                        {
                            heal = (int)(player.statLifeMax2 * 0.05f);
                        }
                        player.HealEffect(heal);
                        player.statLife += heal;
                    }
                }
            }
            if (target.life <= 0 && !modPlayer.MimicryShell)
            {
                modPlayer.MimicryWearShell(target);
                /*
                int attack = target.damage;
                int healthBoost = (int)(target.lifeMax / 10);
                int defense = target.defDefense / 3;
                int shellTimer = target.lifeMax * 30;

                modPlayer.MimicryKillCocoon(attack, healthBoost, defense, shellTimer);
                */
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            bool Goodbye = false;

            if (Projectile.ai[1] >= 60)
                tex = Mod.Assets.Request<Texture2D>("Items/Ruina/Language/MimicryRAlt").Value;
            float rotation = Projectile.rotation + 0.785f;
            Vector2 origin = new Vector2(8, tex.Height - 8);
            SpriteEffects sp = 0;
            if (Projectile.spriteDirection == -1)
            {
                sp = SpriteEffects.FlipHorizontally;
                origin.X = tex.Width - origin.X;
                rotation += 1.57f;
            }
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, tex.Frame(), lightColor, rotation, origin, Projectile.scale, sp);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }

    class MimicryRHello : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/MimicryHello";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 24;
            Projectile.extraUpdates = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0 && Main.player[Projectile.owner].GetModPlayer<LobotomyAlephPlayer>().MimicryShell)
            {
                if (Main.rand.NextBool(3))
                {
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Nullthing_Skill1_casting") with { Volume = 0.1f, MaxInstances = 1 }, Projectile.Center);
                }

                Projectile.velocity *= 2;

                Projectile.ai[1]++;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.ai[0] == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 dustVel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.47f, 0.47f));
                    Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 5, dustVel.X, dustVel.Y)];
                    dust.fadeIn = 1.4f;
                }
                Projectile.ai[0]++;
            }
            if (Main.rand.Next(10) == 0)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 5, Projectile.velocity.X, Projectile.velocity.Y)];
                dust.fadeIn = 1.4f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 splosh = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f));
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, 133, splosh, 0, default(Color), 0.1f);
                dust2.fadeIn = Main.rand.NextFloat(0.5f, 1.2f);
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 splosh = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f));
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, 5, splosh, 0, default(Color), 1f);
                dust2.fadeIn = Main.rand.NextFloat(1f, 1.6f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            float rotation = Projectile.rotation;
            SpriteEffects sp = 0;
            if (Projectile.spriteDirection == -1)
            {
                sp = SpriteEffects.FlipHorizontally;
                rotation += 3.14f;
            }
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, tex.Frame(), lightColor, rotation, tex.Size() / 2f, Projectile.scale, sp);

            for (int i = 0; i < 5; i++)
            {
                float opacity = 0.8f * (1f - (i / 5f));
                Vector2 oldCenter = Projectile.oldPos[i] + new Vector2(Projectile.width / 2, Projectile.height / 2) - Main.screenPosition;

                Main.EntitySpriteDraw(tex, oldCenter, tex.Frame(), lightColor * opacity, rotation, tex.Size() / 2, Projectile.scale, sp);
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyAlephPlayer modPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyAlephPlayer>();
            target.immune[Projectile.owner] = Main.player[Projectile.owner].itemAnimation;
            if (Projectile.ai[1] == 3)
            {
                Projectile.localAI[0] += 1;
            }

            if (target.life <= 0 && !modPlayer.MimicryShell)
            {
                modPlayer.MimicryWearShell(target);
                /*
                int attack = target.damage;
                int healthBoost = (int)(target.lifeMax / 10);
                int defense = target.defDefense / 3;
                int shellTimer = target.lifeMax * 30;

                modPlayer.MimicryKillCocoon(attack, healthBoost, defense, shellTimer);
                */
            }
            base.OnHitNPC(target, hit, damageDone);
        }
    }

    /*
    public class MimicryEyeParticles : AuraBehavior
    {
        public int intensity => 1;

        public CustomShaderData shaderData => null;

        public Texture2D GetTexture(Mod mod) { return mod.Assets.Request<Texture2D>("Misc/MimicryEye").Value;}

        public Rectangle GetSourceRect(Texture2D texture, int index)
        {
            return texture.Frame(22, 14);
        }

        public Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle) 
        {
            if (particle.particleTime < 20)
                return Color.White;
            return Color.White ((particle.particleTime - 20) / 4f); 
        }

        public void SpawnParam(Player player, int dir, float gravDir, float time, AuraParticle particle, int index)
        {
            particle.textureIndex = Main.rand.Next(4);
            particle.Position.Y -= 12;
            particle.Position.X += (8 * (float)Math.Sin(0.436332f * time));

            //Using Velocity as an Offset to Position
            particle.Velocity.X = Main.rand.Next();

            particle.Rotation = Main.rand.NextFloat(-0.261799f, 0.261799f);
            particle.Scale = Main.rand.NextFloat(0.8f, 1.1f);
        }

        public void Behavior(Player player, int dir, float gravDir, float time, AuraParticle particle)
        {
            particle.Position.Y += particle.Velocity.Y;
            particle.Scale -= 0.04f;

            float rotation = MathHelper.ToRadians(particle.particleTime / 24f);
            


            if (particle.particleTime > 24)
            {
                particle.Active = false;
            }
        }
    }*/
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Chat;
using LobotomyCorp;
using LobotomyCorp.Utils;

namespace LobotomyCorp.NPCs.RedMist
{
    //[AutoloadBossHead]
    class DaCapoThrow : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Items/Aleph/DaCapo";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Da Capo");
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;

            Projectile.hostile = true;
            Projectile.friendly = false;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
                Projectile.localAI[0]++;
            }
            Projectile.rotation += MathHelper.ToRadians(15) * Projectile.spriteDirection;

            Projectile.ai[1]++;


            if (Projectile.ai[1] > 60)
            {
                NPC redmist = Main.npc[(int)Projectile.ai[0]];
                if (!redmist.active || redmist.life <= 0 || redmist.type != ModContent.NPCType<RedMist>())
                    Projectile.Kill();

                Vector2 delt = redmist.Center - Projectile.Center;
                delt.Normalize();

                Projectile.velocity += delt * 1.2f;
                float maxSpeed = 16f;
                if (Projectile.velocity.Length() > maxSpeed)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= maxSpeed;
                }
                delt = redmist.Center - Projectile.Center;
                if (delt.Length() < 64)
                    Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            NPC redmist = Main.npc[(int)Projectile.ai[0]];
            if (redmist.active && redmist.life >= 0 && redmist.type == ModContent.NPCType<RedMist>())
            {
                redmist.ai[3] = -1;
                redmist.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value,
                             Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                             TextureAssets.Projectile[Projectile.type].Frame(),
                             lightColor,
                             Projectile.rotation,
                             TextureAssets.Projectile[Projectile.type].Size() / 2,
                             Projectile.scale,
                             Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                             0);
            return false;
        }
    }

    class DaCapoLegato : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Items/Aleph/DaCapo";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Legato");
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;

            Projectile.hostile = true;
            Projectile.friendly = false;
        }

        public Trailhelper trail1;
        public Trailhelper trail2;

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
                Projectile.localAI[0]++;
            }

            int trail = 30;
            if (trail1 == null)
                trail1 = new Trailhelper(trail);
            if (trail2 == null)
                trail2 = new Trailhelper(trail);

            Projectile.rotation += (MathHelper.ToRadians(15) * (Projectile.velocity.Length() / 16f)) * Projectile.spriteDirection;

            Projectile.ai[1]++;

            if (!Main.expertMode)
            {
                if (Projectile.ai[1] > 30)
                {
                    Projectile.velocity *= 0.95f;
                }
            }
            else
            {
                Vector2 pos = Main.player[(int)Projectile.ai[2]].Center - Projectile.Center;
                if (pos.Length() > 4 * 16 && !Main.player[(int)Projectile.ai[2]].dead)
                {
                    /*Projectile.ai[1] = 0;
                    Vector2 delta = Main.player[(int)Projectile.ai[2]].Center - Projectile.Center;
                    Projectile.velocity = delta / 20;*/

                    Vector2 delta = Main.player[(int)Projectile.ai[2]].Center - Projectile.Center;
                    float num1 = delta.Length();
                    float Speed = 16f;
                    Speed += num1 / 50f;

                    int num2 = 50;
                    delta.Normalize();
                    delta *= Speed;
                    Projectile.velocity = (Projectile.velocity * (float)(num2 - 1) + delta) / num2;
                }

            }

            //int type = 88;
            Vector2 scythe = Projectile.Center + new Vector2(60f, 0).RotatedBy(Projectile.rotation);
            trail1.TrailUpdate(scythe, Projectile.rotation + 1.57f);
            //Main.NewText(trail1.TrailPos[0]);
            /*Dust d = Dust.NewDustPerfect(scythe, 88);
            d.noGravity = true;
            d.velocity *= 0;*/
            scythe = Projectile.Center + new Vector2(23.3f, 42.5f * Projectile.spriteDirection).RotatedBy(Projectile.rotation);
            trail2.TrailUpdate(scythe, Projectile.rotation + 1.57f);
            /*d = Dust.NewDustPerfect(scythe, 88);
            d.noGravity = true;
            d.velocity *= 0;*/
        }

        public override void OnKill(int timeLeft)
        {
            NPC redmist = Main.npc[(int)Projectile.ai[0]];
            if (redmist.active && redmist.life >= 0 && redmist.type == ModContent.NPCType<RedMist>())
            {
                redmist.ai[3] = -1;
                redmist.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CustomShaderData shader = LobotomyCorp.LobcorpShaders["GenericTrail"].UseImage1(Mod, "Misc/GenTrail");
            TaperingTrail trail = new TaperingTrail();
            trail.ColorStart = Color.LightBlue;
            trail.ColorEnd = Color.Blue;
            trail.width = 6;

            trail.Draw(trail1.TrailPos, trail1.TrailRotation, Vector2.Zero, shader);
            trail.Draw(trail2.TrailPos, trail2.TrailRotation, Vector2.Zero, shader);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value,
                             Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                             TextureAssets.Projectile[Projectile.type].Frame(),
                             lightColor,
                             Projectile.rotation + 0.768f + (Projectile.spriteDirection == 1 ? 0 : 1.57f),
                             TextureAssets.Projectile[Projectile.type].Size() / 2,
                             Projectile.scale,
                             Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                             0);
            return false;
        }
    }

    class SwitchMimicryThrow : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Items/Aleph/Mimicry";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mimicry");
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 4;

            Projectile.hostile = true;
            Projectile.friendly = false;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction = Math.Sign(Projectile.velocity.X);
            Projectile.rotation = Projectile.velocity.ToRotation() + 0.785f;
            if (Projectile.spriteDirection < 0)
                Projectile.rotation += 1.57f;

            for (int i = 0; i < 2; i++)
            {
                Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood)];
                d.noGravity = true;
                Vector2 dustvel = Vector2.Normalize(Projectile.velocity) * 1f;
                dustvel.X *= -1;
                d.velocity = dustvel;
                d.fadeIn = 1.2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                TextureAssets.Projectile[Projectile.type].Value,
                Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                TextureAssets.Projectile[Projectile.type].Frame(),
                lightColor,
                Projectile.rotation,
                TextureAssets.Projectile[Projectile.type].Size() / 2,
                Projectile.scale,
                Projectile.spriteDirection > 0 ? 0 : SpriteEffects.FlipHorizontally,
                0);
            return false;
        }
    }

    class SwitchDaCapoThrow : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Items/Aleph/DaCapo";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Da Capo");
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 4;

            Projectile.hostile = true;
            Projectile.friendly = false;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction = Math.Sign(Projectile.velocity.X);
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57f - 0.785f;
            if (Projectile.spriteDirection < 0)
                Projectile.rotation += 1.57f;

            for (int i = 0; i < 2; i++)
            {
                Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith)];
                d.noGravity = true;
                Vector2 dustvel = Vector2.Normalize(Projectile.velocity) * 1f;
                dustvel.X *= -1;
                d.velocity = dustvel;
                d.fadeIn = 1.2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                TextureAssets.Projectile[Projectile.type].Value,
                Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                TextureAssets.Projectile[Projectile.type].Frame(),
                lightColor,
                Projectile.rotation,
                TextureAssets.Projectile[Projectile.type].Size() / 2,
                Projectile.scale,
                Projectile.spriteDirection > 0 ? 0 : SpriteEffects.FlipHorizontally,
                0);
            return false;
        }
    }
}

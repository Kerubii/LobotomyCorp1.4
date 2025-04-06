using LobotomyCorp.Buffs;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
    public class SmileRVomit : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_85";

        public override void SetDefaults()
        {
            Projectile.ArmorPenetration = 15;
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 4;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            int num = 60;
            int num2 = 12;
            int num3 = num + num2;
            if (Projectile.localAI[0] >= (float)num3)
            {
                Projectile.Kill();
            }
            if (Projectile.localAI[0] >= (float)num)
            {
                Projectile.velocity *= 0.95f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.localAI[0] > 54f)
                return false;

            return base.CanHitNPC(target);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int num = (int)Terraria.Utils.Remap(Projectile.localAI[0], 0f, 72f, 10f, 40f);
            hitbox.Inflate(num, num);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Vomit>(), 300);
            if (Main.myPlayer == Projectile.owner)
            {
                if (target.boss)
                {
                    Main.player[Projectile.owner].GetModPlayer<LobotomyAlephPlayer>().SmileCreateCorpseBoss(target);
                }
                else if (target.life <= 0)
                    Main.player[Projectile.owner].GetModPlayer<LobotomyAlephPlayer>().SmileCreateCorpse(target, true);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawProj_Flamethrower(Projectile);
            return base.PreDraw(ref lightColor);
        }

        private static void DrawProj_Flamethrower(Projectile proj)
        {
            float num = 60f;
            float num10 = 12f;
            float fromMax = num + num10;
            Texture2D value = TextureAssets.Projectile[proj.type].Value;
            Color color = new Color(80, 20, 20) * .8f;
            Color color2 = new Color(60, 20, 1) * .3f;
            Color color3 = Color.Lerp(new Color(80, 20, 20) * 0.4f, color2, 0.25f);
            Color color4 = new Color(50, 40, 30) * 0.4f;
            float num11 = 0.35f;
            float num12 = 0.7f;
            float num13 = 0.85f;
            float num14 = ((proj.localAI[0] > num - 10f) ? 0.175f : 0.2f);
            int verticalFrames = 7;
            float num15 = Terraria.Utils.Remap(proj.localAI[0], num, fromMax, 1f, 0f);
            float num2 = Math.Min(proj.localAI[0], 20f);
            float num3 = Terraria.Utils.Remap(proj.localAI[0], 0f, fromMax, 0f, 1f);
            float num4 = Terraria.Utils.Remap(num3, 0.2f, 0.5f, 0.25f, 1f);
            Rectangle rectangle = value.Frame(1, verticalFrames, 0, (int)(7 * proj.localAI[0] / 72f));
            if (!(num3 < 1f))
            {
                return;
            }
            for (int i = 0; i < 2; i++)
            {
                for (float num5 = 1f; num5 >= 0f; num5 -= num14)
                {
                    Color val = ((num3 < 0.1f) ? Color.Lerp(Color.Transparent, color, Terraria.Utils.GetLerpValue(0f, 0.1f, num3, clamped: true)) : ((num3 < 0.2f) ? Color.Lerp(color, color2, Terraria.Utils.GetLerpValue(0.1f, 0.2f, num3, clamped: true)) : ((num3 < num11) ? color2 : ((num3 < num12) ? Color.Lerp(color2, color3, Terraria.Utils.GetLerpValue(num11, num12, num3, clamped: true)) : ((num3 < num13) ? Color.Lerp(color3, color4, Terraria.Utils.GetLerpValue(num12, num13, num3, clamped: true)) : ((!(num3 < 1f)) ? Color.Transparent : Color.Lerp(color4, Color.Transparent, Terraria.Utils.GetLerpValue(num13, 1f, num3, clamped: true))))))));
                    float num6 = (1f - num5) * Terraria.Utils.Remap(num3, 0f, 0.2f, 0f, 1f);
                    Vector2 vector = proj.Center - Main.screenPosition + proj.velocity * (0f - num2) * num5;
                    Color color5 = val * num6;
                    Color color6 = color5;
                    float num7 = 1f / num14 * (num5 + 1f);
                    float num8 = proj.rotation + num5 * ((float)Math.PI / 2f) + Main.GlobalTimeWrappedHourly * num7 * 2f;
                    float num9 = proj.rotation - num5 * ((float)Math.PI / 2f) - Main.GlobalTimeWrappedHourly * num7 * 2f;

                    Main.EntitySpriteDraw(value, vector + proj.velocity * (0f - num2) * num14 * 0.5f, rectangle, color6 * num15 * 0.25f, num8 + (float)Math.PI / 4f, rectangle.Size() / 2f, num4, (SpriteEffects)0);
                    Main.EntitySpriteDraw(value, vector, rectangle, color6 * num15, num9, rectangle.Size() / 2f, num4, (SpriteEffects)0);
                }
            }
        }
    }
}

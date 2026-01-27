using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;

namespace LobotomyCorp.Projectiles
{
    public class PleasureSpear : ModProjectile
    {
        public static Asset<Texture2D> Handle;

        public override void Load()
        {
            Handle = ModContent.Request<Texture2D>(Texture + "Handle");
        }

        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults() {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1.3f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 120;

            //Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI() {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;

            float rot = Projectile.velocity.ToRotation();
            float progress = 1f - (float)projOwner.itemAnimation / (float)projOwner.itemAnimationMax;
            Projectile.ai[0] = 10 * (float)Math.Sin(progress * 3.14f);

            Vector2 velRot = new Vector2(1, 0).RotatedBy(rot);
            projOwner.itemRotation = (float)Math.Atan2(velRot.Y * Projectile.direction, velRot.X * Projectile.direction);
            projOwner.direction = Projectile.spriteDirection;
            Projectile.rotation = rot;// + MathHelper.ToRadians(Projectile.spriteDirection == 1 ? 45 : 135);

            Projectile.Center = ownerMountedCenter + 67 * velRot + Projectile.ai[0] * Projectile.velocity;

            if (projOwner.itemAnimation == 1)
                Projectile.Kill();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] > 8.5f)
                modifiers.FinalDamage *= 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0]++;
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Porccu_Atk1") with { Volume = 0.5f , MaxInstances = 0});
            }
            target.AddBuff(ModContent.BuffType<Buffs.Pleasure>(), 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Pleasure>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Handle.Value;
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            //Dust.NewDustPerfect(ownerMountedCenter, 14, Vector2.Zero);
            Vector2 position = ownerMountedCenter - Main.screenPosition;
            Vector2 origin = new Vector2((Projectile.spriteDirection == 1 ? 2 : tex.Width - 2), tex.Height / 2);
            float rotation = Projectile.rotation + (Projectile.spriteDirection == 1 ? 0 : 3.14f);
            SpriteEffects spriteEffect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(tex, position, tex.Frame(),
                lightColor * ((float)(255 - Projectile.alpha) / 255f), rotation, origin, Projectile.scale, spriteEffect, 0);

            position += new Vector2(tex.Width + 2, 0).RotatedBy(Projectile.rotation);
            tex = TextureAssets.Projectile[Projectile.type].Value;
            origin = new Vector2((Projectile.spriteDirection == 1 ? 0 : tex.Width), tex.Height/2);
            float minScale = 0.2f;
            float maxScale = (Projectile.velocity.Length() * 10) / (float)tex.Width;
            float progress = 1f - (float)projOwner.itemAnimation / (float)projOwner.itemAnimationMax;
            Vector2 scale = new Vector2(minScale + (maxScale - minScale) * (float)Math.Sin(progress * 3.14f), 1) * Projectile.scale;
            Main.EntitySpriteDraw(tex, position, tex.Frame(),
                lightColor * ((float)(255 - Projectile.alpha) / 255f), rotation, origin, scale, spriteEffect, 0);
            return false;
        }
    }
}

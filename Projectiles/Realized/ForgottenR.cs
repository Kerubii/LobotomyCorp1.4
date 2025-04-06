using System;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Player;

namespace LobotomyCorp.Projectiles.Realized
{
    public class ForgottenR : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("BigHug");
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 60;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.ai[1] == 0)
                Projectile.ai[1] = 1;

            int dir = owner.direction;// * (int)Projectile.ai[1];
            float progress = owner.itemAnimation / (float)owner.itemAnimationMax;
            int diff = (int)(owner.itemAnimationMax * 0.33f);
            if (Projectile.ai[0] == 0)
            {
                owner.heldProj = Projectile.whoAmI;
                DrawHeldProjInFrontOfHeldItemAndArms = true;
                progress = (float)(owner.itemAnimation - diff) / (owner.itemAnimationMax - diff);
                if (progress <= 0f)
                    progress = 0f;
            }
            else
            {
                progress = (float)owner.itemAnimation / (owner.itemAnimationMax - diff);
                if (progress >= 1)
                    progress = 1;
            }
            progress = (float)Math.Sin((1f - progress) * 1.57f);

            Projectile.rotation = Projectile.velocity.ToRotation() + (-2.09f + 4.18f * progress) * owner.direction * Projectile.ai[1];
            if (Projectile.ai[0] == 0)
            {
                if (owner.itemAnimation <= diff)
                {
                    float extra = 1f - (float)owner.itemAnimation / diff;
                    Projectile.rotation += 0.1f * extra * owner.direction;
                }
                owner.SetCompositeArmFront(enabled: true, CompositeArmStretchAmount.Full, Projectile.rotation - 1.57f); //- 0.785f);// * owner.direction);
            }

            Projectile.scale = 1.5f + 0.25f * (float)Math.Sin(progress * 3.14f);
            Projectile.Center = owner.RotatedRelativePoint(owner.MountedCenter) + new Vector2(65 * Projectile.scale, 0).RotatedBy(Projectile.rotation);
            if (progress < 1 && progress > 0)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
                Main.dust[d].noGravity = true;

            }

            if (owner.itemAnimation == 0)
                Projectile.Kill();

            //Main.NewText(LobotomyModPlayer.ModPlayer(owner).ForgottenAffection + " - " + LobotomyModPlayer.ModPlayer(owner).ForgottenAffectionResistance);
        }

        public override bool? CanHitNPC(NPC target)
        {
            Player owner = Main.player[Projectile.owner];

            LobotomyHePlayer modOwner = owner.GetModPlayer<LobotomyHePlayer>();

            //if (modOwner.ForgottenAffectionResistance >= 0.03f && !ValidTarget(owner, target))
                //return false;

            float progress = owner.itemAnimation / (float)owner.itemAnimationMax;
            int diff = (int)(owner.itemAnimationMax * 0.33f);

            if (Projectile.ai[0] == 0)
                progress = (float)(owner.itemAnimation - diff) / (owner.itemAnimationMax - diff);
            else
                progress = (float)owner.itemAnimation / (owner.itemAnimationMax - diff);

            if (progress < 1 || progress > 0)
                return base.CanHitNPC(target);

            return false;
        }

        public override bool CanHitPvp(Player target)
        {
            Player owner = Main.player[Projectile.owner];
            float progress = owner.itemAnimation / (float)owner.itemAnimationMax;
            int diff = (int)(owner.itemAnimationMax * 0.33f);

            if (Projectile.ai[0] == 0)
                progress = (float)(owner.itemAnimation - diff) / (owner.itemAnimationMax - diff);
            else
                progress = (float)owner.itemAnimation / (owner.itemAnimationMax - diff);

            if (progress < 1 || progress > 0)
                return base.CanHitPvp(target);

            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Main.player[Projectile.owner].MountedCenter, projHitbox.Center()))
                return true;
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyHePlayer owner = Main.player[Projectile.owner].GetModPlayer<LobotomyHePlayer>();
            if (ValidTarget(owner.Player, target))
            {
                if (owner.ForgottenAffectionResistance < 0.4f)
                    owner.ForgottenAffectionResistance += 0.01f;
            }
            else if (owner.ForgottenAffectionResistance < 0.03f)
            {
                owner.ForgottenAffectionResistance = 0f;
                owner.ForgottenAffection = target.whoAmI;

                if (target.realLife >= 0)
                {
                    owner.ForgottenAffection = target.realLife;
                }
            }

            if (owner.ForgottenAffectionResistance == 0.03f)
            {
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Teddy_On") with { Volume = 0.5f, MaxInstances = 1 }, Projectile.Center);
            }

            Vector2 delta = Projectile.Center - owner.Player.Center;
            Gore g = Main.gore[Gore.NewGore(Projectile.GetSource_FromAI(), owner.Player.Center + delta * Main.rand.NextFloat(0.1f, 1.0f), new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f)), Main.rand.Next(61, 64))];
            g.scale = 0.4f;
            g.rotation = Main.rand.NextFloat(6.28f);
        }

        //for multisegmented enemies/bosses
        public bool ValidTarget(Player player, NPC target)
        {
            LobotomyHePlayer owner = Main.player[Projectile.owner].GetModPlayer<LobotomyHePlayer>();
            return owner.ForgottenAffection == target.whoAmI || (target.realLife >= 0 && owner.ForgottenAffection == target.realLife);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            Vector2 position = owner.RotatedRelativePoint(owner.MountedCenter) + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;

            if (Projectile.ai[0] == 0)
            {
                position.X -= 4 * owner.direction;

                tex = Mod.Assets.Request<Texture2D>("Projectiles/Realized/ForgottenR2").Value;
            }
            else
                position.X += 4 * owner.direction;

            Vector2 Origin = new Vector2(6, 6);

            Main.EntitySpriteDraw(tex, position, tex.Frame(1, 2), lightColor, Projectile.rotation - 0.785f, Origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}

using System;
using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Items.Zayin;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class LampPlayerSwing : ModProjectile
	{
        public override string Texture => "Terraria/Images/Sun";

        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 60;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.dead || owner.itemAnimation == 0 || owner.HeldItem.type != ModContent.ItemType<Lamp>())
                Projectile.Kill();
            float adjustedScale = owner.GetAdjustedItemScale(owner.HeldItem);
            Projectile.scale = adjustedScale;
            Projectile.rotation = owner.itemRotation + (owner.direction > 0 ? -1.57f : 3.14f);
            Projectile.Center = owner.itemLocation + new Vector2(60 * adjustedScale).RotatedBy(Projectile.rotation);

            float prog = (float)Math.Sin(3.14f * (float)owner.itemAnimation / owner.itemAnimationMax);
            Projectile.scale = 1f + 1f * prog;
            Projectile.alpha = (int)(255 * (1 - prog));

            float range = 114 / 2 * Projectile.scale * 2f;
            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] <= 8)
            {
                foreach (NPC n in Main.npc)
                {
                    if (n.active && n.life > 0 && !n.friendly && (n.Center - Projectile.Center).LengthSquared() < range * range && Projectile.localNPCImmunity[n.whoAmI] == 0)
                    {
                        LobotomyGlobalNPC.LampInflictEverlastingLight(n, 1 * 60, Projectile.damage, Projectile.owner);
                        Projectile.localNPCImmunity[n.whoAmI] = Projectile.localNPCHitCooldown;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D tex = LobotomyCorp.CircleGlow.Value;// TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 origin = tex.Size() / 2;
            Rectangle frame = tex.Frame();
            float alpha = 1f - (Projectile.alpha / 255f);
            
            Main.EntitySpriteDraw(tex, pos, frame, Color.White * 0.25f * alpha, 0, origin, Projectile.scale * 1.5f, 0, 0);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
    }
}

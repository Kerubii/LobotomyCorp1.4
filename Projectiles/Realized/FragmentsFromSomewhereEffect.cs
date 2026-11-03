using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
	public class FragmentsFromSomewhereEffect : ModProjectile
	{
        public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;

			Projectile.timeLeft = 20;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

        public override void AI()
        {
			if (Projectile.ai[0] == 0)
            {
				Projectile.rotation = MathHelper.ToRadians(Main.rand.Next(360));
			}
			Projectile.rotation += MathHelper.ToRadians(4);
			Projectile.ai[0]++;
			Projectile.alpha += 225 / 15;

			if (!Main.player[Projectile.owner].dead)
            {
				Projectile.Center = Main.player[Projectile.owner].MountedCenter;

				if (Main.myPlayer == Projectile.owner)
					LobotomyCorp.ScreenShake(4, 4, 0.45f);
			}
		}

        public override bool PreDraw(ref Color lightColor)
        {
			lightColor = Color.White * (1f - Projectile.alpha / 255f);
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle frame = tex.Frame();
			Vector2 position = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
			Vector2 origin = frame.Size() / 2;
			for (int i = 0; i < 3; i++)
            {
				float scale = Projectile.ai[1] * (float)Math.Sin(Projectile.ai[0] / 20f * (0.5f + 0.5f * (1f - i / 3f)) * 1.57f);
				float rot = Projectile.rotation;
				if (i % 2 == 0)
					rot += 3.14f;
				Main.EntitySpriteDraw(tex, position, frame, lightColor, rot, origin, scale, 0, 0);
            }

			return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            LobotomyGlobalNPC Lnpc = LobotomyGlobalNPC.LNPC(target);
			if (Lnpc.FragmentsFromSomewhereEnlightenment || Lnpc.FragmentsFromSomewhereTentacles == 0)
				return false;
            return base.CanHitNPC(target);
        }

		Dictionary<int, int> ResistanceList = new Dictionary<int, int>();
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			if (LobotomyGlobalNPC.LNPC(target).FragmentsFromSomewhereTentacles == 0)
			{
				modifiers.FinalDamage *= 0.1f;
                return;
			}
			float damage = LobotomyGlobalNPC.LNPC(target).FragmentsFromSomewhereTentacles;
			if (ResistanceList.ContainsKey(target.realLife) )
			{
				damage *= (1f / ResistanceList[target.realLife]) * 0.25f;
				ResistanceList[target.realLife]++;
			}
			else if (ResistanceList.ContainsKey(target.whoAmI))
            {
                damage *= (1f / ResistanceList[target.whoAmI]) * 0.25f;
                ResistanceList[target.whoAmI]++;
            }
			else if (target.realLife > 0)
			{
				ResistanceList.Add(target.realLife, 1);
			}
			damage -= 1f;
			if (damage < -1f)
				damage = -1f;

            modifiers.FinalDamage += damage;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyGlobalNPC.ApplyPreEnlightenment(target, Projectile.owner);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			float radius = Projectile.ai[1] * (float)Math.Sin(Projectile.ai[0] / 20f * (0.5f + 0.5f * (1f / 3f)) * 1.57f) * 128f;

			Vector2 origin = Projectile.Center - new Vector2(radius, radius);
			int rr = (int)(radius * 2);
			hitbox = new Rectangle((int)origin.X, (int)origin.Y, rr, rr);
		}
	}
}

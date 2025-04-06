using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using static Humanizer.In;

namespace LobotomyCorp.Projectiles.Realized
{
	public class RegretShockwave : ModProjectile
	{
		public override string Texture => "LobotomyCorp/Projectiles/Realized/RegretR";

		public override void SetDefaults() {
			Projectile.width = 100;
			Projectile.height = 100;
			Projectile.aiStyle = -1;
			Projectile.penetrate = 11;
			Projectile.scale = 1f;
			Projectile.timeLeft = 35;
			Projectile.tileCollide = false;

			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = true;
			Projectile.friendly = true;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
				Projectile.ai[0]++;
				SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Abandoned_Strong_Vert") with { Volume = 0.2f }, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                    LobotomyCorp.ScreenShake(15, 8f, 0.1f);
				if (Main.netMode != NetmodeID.Server)
				{
					int xdir = Main.rand.NextBool(2) ? -1 : 1;

					Filters.Scene.Activate("LobotomyCorp:BrokenScreen");
					Filters.Scene["LobotomyCorp:BrokenScreen"].GetShader().UseProgress(1f);
					Filters.Scene["LobotomyCorp:BrokenScreen"].GetShader().UseIntensity(0.05f + Main.rand.NextFloat(0.07f));
					Filters.Scene["LobotomyCorp:BrokenScreen"].GetShader().UseDirection(new Vector2(xdir, Main.rand.NextBool(2) ? -1 : 1));
					Filters.Scene["LobotomyCorp:BrokenScreen"].GetShader().UseColor(1.2f + Main.rand.NextFloat(0.25f), 1f + Main.rand.NextFloat(0.25f), 1f);
					Filters.Scene["LobotomyCorp:BrokenScreen"].GetShader().UseTargetPosition(Projectile.Center - new Vector2(10 * xdir, 0));
				}
			}
			else if(Projectile.timeLeft < 10 && Main.netMode != NetmodeID.Server && Filters.Scene["LobotomyCorp:BrokenScreen"].IsActive())
			{
				Filters.Scene["LobotomyCorp:BrokenScreen"].GetShader().UseProgress(1f * (Projectile.timeLeft / 10f));
			}
        }

        public override bool? CanHitNPC(NPC target)
        {
			if (Projectile.penetrate <= 1)
				return false;
            return base.CanHitNPC(target);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			modifiers.FinalDamage *= 0.4f + 0.6f * (Projectile.penetrate / 11);
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			if (target.whoAmI == (int)Projectile.ai[1])
				target.AddBuff(ModContent.BuffType<Buffs.MetallicRinging>(), 480);
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && Filters.Scene["LobotomyCorp:BrokenScreen"].IsActive())
            {
                Filters.Scene["LobotomyCorp:BrokenScreen"].Opacity = 0;
                Filters.Scene["LobotomyCorp:BrokenScreen"].Deactivate();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
			return false;
        }
    }
}

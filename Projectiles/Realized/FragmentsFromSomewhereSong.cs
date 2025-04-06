using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
	public class FragmentsFromSomewhereSong : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Projectiles/Realized/FragmentsFromSomewhereRSpear";

        public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1.3f;
			Projectile.alpha = 0;

			Projectile.timeLeft = 75;
			Projectile.hide = true;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
		}

        public override void AI()
        {
            if (Projectile.ai[0] == 0 && Projectile.localAI[0]++ == 0)
				SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Art/Cosmos_Sing") with { Volume = 0.25f }, Projectile.Center);
			float prog = (1f - Projectile.timeLeft / 75f);

			if (Projectile.ai[1] % 15 == 0 && Projectile.owner == Main.myPlayer)
            {
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FragmentsFromSomewhereEffect>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 2f + 2f * prog);
            }

			if (Projectile.ai[1] % 3 == 0)
            {
				for (int i = 0; i < 3; i++)
                {
					Vector2 vel = new Vector2(16 + 16 * prog, 0).RotatedBy(Main.rand.NextFloat(6.28f));
					Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Misc.Dusts.FragmentNote>(), vel);
					d.noGravity = true;
					d.fadeIn = 1f;
                }

				for (int i = 0; i < 6; i++)
				{
					Vector2 vel = new Vector2(16 + 16 * prog, 0).RotatedBy(Main.rand.NextFloat(6.28f));
					Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RedMoss, vel);
					d.noGravity = true;
					//d.fadeIn = 1f;
				}
			}
			Projectile.ai[1]++;

			Projectile.Center = Main.player[Projectile.owner].MountedCenter;
		}

        public override void OnKill(int timeLeft)
        {
			/*
			if (Projectile.ai[0] < 5 && Main.myPlayer == Projectile.owner)
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.player[Projectile.owner].Center, Vector2.Zero, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, ++Projectile.ai[0]);*/
        }

        public override bool? CanHitNPC(NPC target)
        {
			return false;// !LobotomyGlobalNPC.LNPC(target).FragmentsFromSomewhereEnlightenment;
        }

        public override bool PreDraw(ref Color lightColor)
        {
			return false;
        }
    }
}

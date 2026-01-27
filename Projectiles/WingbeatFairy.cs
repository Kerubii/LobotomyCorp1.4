using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class WingbeatFairy : ModProjectile
	{
        public static Asset<Texture2D> Target;
        public static Asset<Texture2D> Target2;

        public override void Load()
        {
            Target = Mod.Assets.Request<Texture2D>("Projectiles/WingbeatTarget");
            Target2 = Mod.Assets.Request<Texture2D>("Projectiles/WingbeatTarget2");
        }

		public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 6;
        }

		public override void SetDefaults() {
			Projectile.width = 30;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
            Projectile.timeLeft = 60;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
		}

        public override void AI()
        {
            if(Main.rand.Next(5) == 0)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 15);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 36)
            {
                Projectile.frameCounter = 0;
            }
            Projectile.frame = (int)(Projectile.frameCounter / 6);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Fairy>(), 600);
        }

        public override void OnKill(int timeLeft)
        {
           for (int i = 0; i < 8; i++)
           {
                Vector2 dir = new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(45) * i);
                Dust.NewDustPerfect(Projectile.Center + dir, 15, dir);
           }
            SoundEngine.PlaySound(SoundID.NPCHit5, Projectile.Center);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage.Base = 1;
        }
    }
}

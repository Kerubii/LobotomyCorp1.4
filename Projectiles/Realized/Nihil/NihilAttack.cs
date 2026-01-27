using LobotomyCorp.Buffs;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized.Nihil
{
	public class NihilAttack : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Projectiles/Realized/Nihil/NihilCard";

        public override void SetDefaults() {
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.timeLeft = 180;

			Projectile.DamageType = DamageClass.Summon;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
		}

        public override void AI()
        {
            //Main.NewText(Projectile.ai[0]);

			Player player = Main.player[Projectile.owner];

			Projectile.Center = player.Center;

			if (Main.netMode != NetmodeID.Server)
			{
                if (Projectile.ai[0] == 0)
                {
                    if (!Filters.Scene["LobotomyCorp:GrayscaleShader"].IsActive())
                    {
                        Filters.Scene.Activate("LobotomyCorp:GrayscaleShader").GetShader().UseProgress(0);
                        Filters.Scene.Activate("LobotomyCorp:InvertShader").GetShader().UseIntensity(0);
                    }
                }
                else
                {
                    float Progress = (Projectile.ai[0] % 30) / 30f;
                    float Intensity = (Projectile.ai[0] % 30) / 30f;
                    if (Projectile.ai[0] >= 30f && Projectile.ai[0] < 150f)
                        Progress = 1f;
                    else if (Projectile.ai[0] >= 150f)
                        Progress = 1f - Progress;
                    if (Projectile.ai[0] >= 60f && Projectile.ai[0] < 120f)
                        Intensity = 1f;
                    else if (Projectile.ai[0] >= 120f)
                        Intensity = 1f - Intensity;
                    if (Projectile.ai[0] < 30f || Projectile.ai[0] >= 150f)
                        Intensity = 0f;
                    Filters.Scene["LobotomyCorp:GrayscaleShader"].GetShader().UseProgress(Progress);
                    Filters.Scene["LobotomyCorp:InvertShader"].GetShader().UseIntensity(Intensity);
                }

            }
            Projectile.ai[0]++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<NihilDebuff>(), 60 * 10);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<NihilDebuff>(), 60 * 10);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Collision.CanHit(Main.player[Projectile.owner], target))
                return true;
            return base.CanHitNPC(target);
        }

        public override void OnKill(int timeLeft)
        {
            if (Filters.Scene["LobotomyCorp:GrayscaleShader"].IsActive())
            {
                Filters.Scene["LobotomyCorp:GrayscaleShader"].Deactivate();
                Filters.Scene["LobotomyCorp:InvertShader"].Deactivate();
            }
        }
    }
}

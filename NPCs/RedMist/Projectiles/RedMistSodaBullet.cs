using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Chat;
using LobotomyCorp;
using Terraria.GameContent;
using LobotomyCorp.Buffs;
using LobotomyCorp.Visuals.LobEffects;
using static LobotomyCorp.Misc.MiscAssets;


namespace LobotomyCorp.NPCs.RedMist
{
	class RedMistSodaBullet : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_180";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BulletDeadeye);
            AIType = ProjectileID.BulletDeadeye;
        }

        public override void AI()
        {
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, -18), ModContent.ProjectileType<RedMistSodaGeyser>(), Projectile.damage, 0);
            }
        }
    }
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using Terraria.Graphics.Shaders;
using Terraria.GameContent;
using System.Security.Cryptography.Pkcs;
using LobotomyCorp.Players;
using Microsoft.Build.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using Terraria.Graphics;
using LobotomyCorp.Buffs;
using System.Threading;
using System.Security.Policy;
using Terraria.Audio;
using rail;
using ReLogic.Content;

namespace LobotomyCorp.Projectiles.Realized
{
	public class SwordSharpenedWithTearsRSwordExtra : SwordSharpenedWithTearsRSword
    {
        public override string Texture => "LobotomyCorp/Projectiles/Realized/SwordSharpenedWithTearsRSword";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spear");
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 120;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 10000;

            //Projectile.hide = true;
            //Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.extraUpdates = 3;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }
    }
}

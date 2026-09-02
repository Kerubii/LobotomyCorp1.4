using LobotomyCorp.Projectiles.RedMist;
using Microsoft.Xna.Framework;
using System.Collections;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.He
{
    public class Christmas : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("It is patched with heavy leather of unknown origin.\n" +
							   "The stitches are carefully woven, but for whom or for what, exactly, is unclear.\n" +
							   "It is not elegant, but you can feel the devotion of its creator.\n" +
							   "Can be obtained from Presents"); */
        }

        public override void LobSetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 5000;
            Item.rare = ModContent.RarityType<HeB>();
            Item.UseSound = LobotomyCorp.WeaponSounds.Mace;
            Item.autoReuse = true;

            Item.shoot = ProjectileID.OrnamentFriendly;
            Item.shootSpeed = 16f;
            EGORiskLevel = RiskLevel.He;

            RedMistMaskDamageBoost = 0.5f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (RedMistMaskUpgrade(player))
            {
                int which = Main.rand.Next(4);
                switch (which)
                {
                    case 0: type = ModContent.ProjectileType<ChristmasBall>(); velocity *= 1.5f; break;
                    case 1: type = ModContent.ProjectileType<ChristmasStar>(); velocity *= 0.3f; break;
                    case 2: type = ModContent.ProjectileType<ChristmasPresent>(); velocity *= 0.5f; break;
                    case 3: type = ModContent.ProjectileType<ChristmasCane>(); velocity *= 0.6f; break;
                }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (RedMistMaskUpgrade(player))
            {
                for (int i = 0; i < 2; i++)
                {
                    int which = Main.rand.Next(4);
                    int projType = type;
                    Vector2 projVel = Vector2.Normalize(velocity).RotatedByRandom(MathHelper.ToRadians(20)) * Item.shootSpeed;
                    switch (which)
                    {
                        case 0: projType = ModContent.ProjectileType<ChristmasBall>(); projVel *= 1.5f; break;
                        case 1: projType = ModContent.ProjectileType<ChristmasStar>(); projVel *= 0.3f; break;
                        case 2: projType = ModContent.ProjectileType<ChristmasPresent>(); projVel *= 0.5f; break;
                        case 3: projType = ModContent.ProjectileType<ChristmasCane>(); projVel *= 0.6f; break;
                    }
                    Projectile.NewProjectile(source, position, projVel, projType, damage, knockback, player.whoAmI);
                }
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (RedMistMaskUpgrade(player))
            {
                target.AddBuff(GetRandomDebuff(), 60 * 8);
            }
        }

        public override void AddRecipes()
        {
        }
        public static int GetRandomDebuff()
        {
            int type;
            int which = Main.rand.Next(7);
            switch (which)
            {
                default: type = BuffID.OnFire; break;
                case 1: type = BuffID.ShadowFlame; break;
                case 2: type = BuffID.OnFire3; break;
                case 3: type = BuffID.Venom; break;
                case 4: type = BuffID.Ichor; break;
                case 5: type = BuffID.CursedInferno; break;
                case 6: type = BuffID.Frostburn2; break;
            }
            return type;
        }
    }
}
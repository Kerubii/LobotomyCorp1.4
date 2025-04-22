using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class Beak : LobItemBase
    {
        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.knockBack = 4;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 16;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.noMelee = true;
            Item.value = 7500;
            Item.rare = ModContent.RarityType<TethB>();
            Item.UseSound = LobotomyCorp.WeaponSounds.Gun;
            Item.autoReuse = true;

            Item.shoot = 10;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;
            EGORiskLevel = RiskLevel.Teth;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (RedMistMaskUpgrade(player))
            {
                float counter = player.GetModPlayer<LobotomyTethPlayer>().BeakGunCounter;
                if (counter > 0 && player.altFunctionUse == 2)
                {
                    damage = (int)(damage * 5 * counter); 
                    for (int i = 0; i < 18; i++)
                    {
                        Vector2 dustVel = velocity.RotatedBy(Main.rand.NextFloat(MathHelper.ToRadians(-30), MathHelper.ToRadians(30)));
                        dustVel *= Main.rand.NextFloat(4f, 8f);
                        Dust d = Dust.NewDustPerfect(position, DustID.Blood, dustVel);
                    }
                    velocity *= 2f;

                    player.GetModPlayer<LobotomyTethPlayer>().BeakGunCounter = 0;
                }
                else
                {
                    velocity = velocity.RotatedBy(Main.rand.NextFloat(MathHelper.ToRadians(-10), MathHelper.ToRadians(10)));
                }
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        /*public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (RedMistMaskUpgrade(player))
            {
                damage += 1f;
                if (player.altFunctionUse == 2)
                    damage += player.GetModPlayer<LobotomyTethPlayer>().BeakGunCounter;
            }

            base.ModifyWeaponDamage(player, ref damage);
        }*/

        public override float UseAnimationMultiplier(Player player)
        {
            if (RedMistMaskUpgrade(player))
            {
                if (player.altFunctionUse == 2)
                {
                    return 2f;
                }
                return 0.1f;
            }

            return base.UseAnimationMultiplier(player);
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (RedMistMaskUpgrade(player))
            {
                if (player.altFunctionUse == 2)
                {
                    return 2f;
                }
                return 0.1f;
            }

            return base.UseTimeMultiplier(player);
        }

        public override bool AltFunctionUse(Player player)
        {
            return (RedMistMaskUpgrade(player) && player.GetModPlayer<LobotomyTethPlayer>().BeakGunCounter > 0);
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (RedMistMaskUpgrade(player) && player.altFunctionUse != 2)
            {
                return Main.rand.NextBool(50);
            }
            return base.CanConsumeAmmo(ammo, player);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.FlintlockPistol)
            .AddIngredient(ItemID.Feather, 2)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}

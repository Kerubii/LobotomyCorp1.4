using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Ruina.Social
{
    public class TheHomingInstinctS : SEgoItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        /*
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("\"Friends, Friends, let us all go home together!\"");
            PassiveText = "Home - Drop a house\n" +
                          "A Road Walked Together - Create temporary yellow brick road that gives buffs to nearby teammates\n" +
                          "|On the Way Home - While a yellow brick road or this weapon is active, the user gains a debuff when not near it";
            EgoColor = LobotomyCorp.HeRarity;
        }*/

        public override void SetDefaults()
        {
            Item.damage = 9000; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage damageed together.
            Item.DamageType = ModContent.GetInstance<Players.DamageType.ExtractorRanged>(); // sets the damage type to ranged
            Item.width = 40; // hitbox width of the Item
            Item.height = 42; // hitbox height of the Item
            Item.useTime = 120; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 120; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = 10; // how you use the Item (swinging, holding out, etc)
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 4; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback damageed together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ModContent.RarityType<HeR>(); // the color that the Item's name will be in-game
            Item.UseSound = SoundID.Item11; // The sound that this Item plays when used.
            Item.autoReuse = true; // if you can hold click to automatically use it again
            Item.shoot = ModContent.ProjectileType<Projectiles.HOUSE>();
            Item.shootSpeed = 15f; // the speed of the projectile (measured in pixels per frame)
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                if (player == Main.LocalPlayer)
                {
                    position.X = Main.MouseWorld.X;
                    position.Y = player.position.Y - 600;
                    velocity.X = 0;
                    velocity.Y = Item.shootSpeed;
                }
            }
            else
            {
                position.Y = player.position.Y + player.height;
                position.X = (int)(position.X / 16) * 16;
                position.Y = (int)(position.Y / 16) * 16;
                float rot = velocity.ToRotation();
                if (rot < 0)
                    rot += 6.28f;
                if (rot > 6.28f)
                    rot -= 6.28f;
                rot = (float)MathHelper.ToDegrees(rot);
                if (rot > 360 - 45 || rot < 45 || rot < 180 + 45 && rot > 180 - 45)
                {
                    velocity.Y = 0;
                    velocity.X = 4f * Math.Sign(velocity.X);
                }
                else
                {
                    velocity.X = (float)Math.Sqrt(8) * Math.Sign(velocity.X);
                    velocity.Y = (float)Math.Sqrt(8) * Math.Sign(velocity.Y);
                }
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool SafeCanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                Item.UseSound = SoundID.Item11;
                Item.useAnimation = 120;
                Item.useTime = 120;
                Item.shoot = ModContent.ProjectileType<Projectiles.HOUSE>();
                Item.shootSpeed = 15f;
            }
            else
            {
                Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/House_MakeRoad") with { Volume = 0.1f };
                Item.useAnimation = 28;
                Item.useTime = 28;
                Item.shoot = ModContent.ProjectileType<Projectiles.HomingInstinct>();
                Item.shootSpeed = 8f;
            }
            return base.SafeCanUseItem(player);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, 0);
        }
    }
}

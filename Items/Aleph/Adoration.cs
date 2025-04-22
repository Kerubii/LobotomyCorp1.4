using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Projectiles;
using Terraria;
using System;

namespace LobotomyCorp.Items.Aleph
{
    public class Adoration : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("A big mug filled with mysterious slime that never runs out.\n" +
                               "It�'s the byproduct of some horrid experiment in a certain laboratory that eventually failed.\n" +
                               "Inflicts Slow"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 48;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.width = 40;
            Item.height = 16;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = 8000;
            Item.rare = ModContent.RarityType<AlephB>();
            //Item.UseSound = LobotomyCorp.WeaponSound("Slime");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MeltyLove>();
            Item.shootSpeed = 7.6f;
            Item.channel = true;
            EGORiskLevel = RiskLevel.Aleph;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            heldItemFrame.Y = 1000;
            heldItemFrame.X -= 1000;
            /*int half = player.itemAnimationMax / 2;
            if (player.itemAnimation > half)
            {
                float prog = (player.itemAnimation - half) / (float)half;
                float rot = player.itemRotation;

                Vector2 heldOffset = new Vector2(-10 * (float)Math.Sin(prog * 3.14f), 0).RotatedBy(rot);
                heldItemFrame.X += (int)heldOffset.X;
                heldItemFrame.Y += (int)heldOffset.Y;
            }*/
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Gel, 300)
            .AddIngredient(ItemID.PinkGel, 20)
            .AddIngredient(ItemID.AdamantiteBar, 10)
            .AddIngredient(ItemID.Bottle)
            .AddTile<Tiles.BlackBox3>()
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.Gel, 300)
            .AddIngredient(ItemID.PinkGel, 20)
            .AddIngredient(ItemID.TitaniumBar, 10)
            .AddIngredient(ItemID.Bottle)
            .AddTile<Tiles.BlackBox3>()
            .Register();
        }
    }
}

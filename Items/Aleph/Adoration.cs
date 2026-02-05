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
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void LobSetDefaults()
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

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            float rot = player.itemRotation;
            float factor = 0f;
            if (player.channel)
            {
                // Find Shot projectile since heldProj isn't updated yet
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.type == Item.shoot && proj.owner == player.whoAmI && proj.ai[1] == 0)
                    {
                        factor = proj.ai[0] / 80f;
                    }
                }
            }

            player.itemLocation -= new Vector2(20 * factor * player.direction, 0).RotatedBy(rot);
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

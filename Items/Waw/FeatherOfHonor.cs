using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class FeatherOfHonor : LobItemBase
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("The feather strikes with vivid flame. It is not weak, nor faint.\n" +
                               "The flame pierces the body and melts the frost of the heart."); */
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 0f;
        }

        [Obsolete]
        public int FeatherShoot = 0;

        public override void LobSetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 6;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.4f;
            Item.value = 5000;
            Item.rare = ModContent.RarityType<WawB>();
            //Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.FeatherOfHonorMinion>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = true;
            Item.channel = true;
            //FeatherShoot = 0;
            Item.buffType = ModContent.BuffType<Buffs.FeatherOfHonorM>();
        }

        public override void HoldItem(Player player)
        {
            if (FeatherShoot > 0)
                FeatherShoot--;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            /*
            for (int i = 0; i < 5; i++)
            {
                int order = i;
                if (i >= 2)
                    order++;
                if (i == 4)
                    order = 2;
                if (Main.myPlayer == player.whoAmI)
                    Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, order);
            }

            return false;
            */
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            player.AddBuff(Item.buffType, 2);
        }

        public override bool? UseItem(Player player)
        {
            List<Projectile> feathers = new List<Projectile>();
            float minionCount = 0;

            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.minion && p.owner == player.whoAmI)
                {
                    minionCount += p.minionSlots;
                    if (p.type == Item.shoot)
                    {
                        feathers.Add(p);
                    }
                }
            }

            if (minionCount >= player.maxMinions)
            {
                int order = 0;
                //int extra = 5 - feathers.Count;

                while (feathers.Count > 0)
                {
                    Projectile p = feathers[Main.rand.Next(feathers.Count)];
                    if (p.ai[1] == 0)
                    {
                        p.ai[1] = 5 + 5 * order;
                        p.netUpdate = true;
                    }
                    order++;

                    feathers.Remove(p);
                }

                //while (extra > 0)
                //{
                    //extra--;
                //}
                return true;
            }

            return base.UseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Feather, 5)
            .AddIngredient(ItemID.InfernoPotion)
            .AddIngredient(ItemID.Fireblossom, 4)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}
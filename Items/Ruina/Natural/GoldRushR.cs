using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Aleph;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.QueenLaser;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using rail;
using System;
using System.Collections.Generic;
using System.Net;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Ruina.Natural
{
    public class GoldRushR : SEgoItem
    {
        public override void SetDefaults()
        {
            EgoColor = LobotomyCorp.AlephRarity;

            Item.damage = 60;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 9;
            Item.useAnimation = 20;
            Item.reuseDelay = 7;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<AlephR>();
            //Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/Natural/Greed_Stab") with { Volume = 0.2f, MaxInstances = 0 };

            Item.shoot = ModContent.ProjectileType<GoldRushFlurryR>();
            Item.shootSpeed = 8f;
        }

        public override bool AltFunctionUse(Player player)
        {
            return !player.GetModPlayer<LobotomyAlephPlayer>().GoldRushRoadCooldown;
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 4;
            else if (player.GetModPlayer<LobotomyAlephPlayer>().GoldRushGreed)
                return 8;
            return base.UseTimeMultiplier(player);
        }

        public override float UseAnimationMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return base.UseAnimationMultiplier(player);
            else if (player.GetModPlayer<LobotomyAlephPlayer>().GoldRushGreed)
                return 2;
            return base.UseAnimationMultiplier(player);
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            if (player.altFunctionUse == 2)
            {
                crit *= 3;
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.GetModPlayer<LobotomyAlephPlayer>().GoldRushGreed)
                damage += 3f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<LobotomyAlephPlayer>().GoldRushGreed)
            {
                type = ModContent.ProjectileType<GoldRushFlurryRAlt>();
                velocity *= 0.66f;
            }

            if (player.altFunctionUse == 2)
            {
                velocity *= 0;
                damage *= 5;
                knockback *= 2;
                type = ModContent.ProjectileType<GoldRushRoadOfGold>();
                if (player.statLife < player.statLifeMax2/2 || player.GetModPlayer<LobotomyAlephPlayer>().GoldRushGreed)
                    type = ModContent.ProjectileType<GoldRushRoadOfGoldAlt>();
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 || player.ownedProjectileCounts[type] == 0)
            {
                return true;
            }
            velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.040000f, 0.040000f));
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Main.rand.NextFloat(-16.0000f, 16.0000f), Main.rand.NextFloat(-16.0000f, 16.0000f));
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GoldRush>())
            .AddIngredient(ItemID.GoldCoin, 99)
            .AddIngredient(ItemID.Amber)
            .AddIngredient(ItemID.Diamond)
            .AddIngredient(ItemID.Ectoplasm, 13)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
    }
}

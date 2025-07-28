using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Teth;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.QueenLaser;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using rail;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace LobotomyCorp.Items.Ruina.Natural
{
    public class InTheNameOfLoveAndHateR : SEgoItem
	{
        public override void SetDefaults()
        {
            EgoColor = LobotomyCorp.WawRarity;
            Item.damage = 60;
			Item.DamageType = DamageClass.Magic;
			Item.width = 40;
			Item.height = 20;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 4;
			Item.value = 10000; 
			Item.rare = ModContent.RarityType<WawR>();
            Item.noUseGraphic = true;
            Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<ItnolahVisual>();
            Item.shootSpeed = 8f;
            Item.channel = true;
            Item.mana = 8;
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2 && player.CheckMana(player.GetModPlayer<LobotomyWawPlayer>().LoveAndHateArcanaCost))
            {
                mult *= 0;
            }
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.GetModPlayer<LobotomyWawPlayer>().LoveAndHateHatred)
                return 0.5f;
            return base.UseSpeedMultiplier(player);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<StarShot2>();
            position += Vector2.Normalize(velocity) * 80;
            if (player.GetModPlayer<LobotomyWawPlayer>().LoveAndHateHatred)
            {
                type = ModContent.ProjectileType<MiniLaser>();
                velocity.Normalize();
            }
            if (player.altFunctionUse == 2)
            {
                if (player.CheckMana(player.GetModPlayer<LobotomyWawPlayer>().LoveAndHateArcanaCost, true, true))
                {
                    type = ModContent.ProjectileType<Circle1>();
                    position -= Vector2.Normalize(velocity) * 130;
                    velocity.Normalize();
                    damage += player.statManaMax2 / 4;
                }
                else
                {
                    type = ModContent.ProjectileType<ArcanaBeatsv2>();
                    velocity *= 2;
                    damage *= 3;
                }
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = ModContent.ProjectileType<ItnolahVisual>();
            if (player.ownedProjectileCounts[proj] == 0)
                Projectile.NewProjectile(player.GetSource_FromThis(), position, Vector2.Zero, proj, damage, knockback, player.whoAmI);

            if (player.GetModPlayer<LobotomyWawPlayer>().LoveAndHateHatred)
            {
                if (type == ModContent.ProjectileType<Circle1>())
                {
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 0, 1);
                    return false;
                }
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool LobModifyTooltips(List<TooltipLine> tooltips, ref int num)
        {
            num = Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>().LoveAndHateArcanaCost;
            return base.LobModifyTooltips(tooltips, ref num);
        }

        public override void HoldItem(Player player)
        {
            if (!player.GetModPlayer<LobotomyWawPlayer>().LoveAndHateHatred)
                player.AddBuff(ModContent.BuffType<Love>(), 2);
        }

        public override bool AltFunctionUse(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<Circle1>()] == 0;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, 0);
        }

        public override bool? UseItem(Player player)
        {
            LobotomyWawPlayer wawPlayer = player.GetModPlayer<LobotomyWawPlayer>();
            if (player.altFunctionUse != 2)
            {
                if (wawPlayer.LoveAndHateHatred)
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/MagicalGirl_SnakeAtk") with { Volume = 0.2f, MaxInstances = 0 });
                else
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/MagicalGirl_Atk") with { Volume = 0.2f, MaxInstances = 0 });
            }
            else
            {
                if (wawPlayer.LoveAndHateHatred)
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/MagicalGirl_Snake_Casting") with { Volume = 0.2f, MaxInstances = 0 });
                else
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/MagicalGirl_Casting") with { Volume = 0.2f});
            }
            return base.UseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<InTheNameOfLoveAndHate>())
            .AddIngredient(ItemID.LifeCrystal)
            .AddIngredient(ItemID.LifeFruit)
            .AddIngredient(ItemID.LunarTabletFragment, 12)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
    }
}

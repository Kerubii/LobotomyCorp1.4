using LobotomyCorp.Buffs;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.QueenLaser;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using rail;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Ruina.Natural
{
    public class InTheNameOfLoveAndHateR : SEgoItem
	{
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public int ArcanaManaCost = 500;

		public override void SetStaticDefaults() {
            EgoColor = LobotomyCorp.WawRarity;
        }

        public override void SetDefaults() {
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
			Item.rare = ItemRarityID.Green;
            Item.noUseGraphic = true;
			//Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<ArcanaBeatsv2>();
            Item.shootSpeed = 16f;
            Item.channel = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<StarShot2>();
            if (player.altFunctionUse == 2)
            {
                if (player.CheckMana(player.GetModPlayer<LobotomyWawPlayer>().LoveAndHateArcanaCost, true, true))
                    type = ModContent.ProjectileType<Circle1>();
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = ModContent.ProjectileType<ArcanaBeatsv2>();
            if (player.ownedProjectileCounts[proj] == 0)
                Projectile.NewProjectile(player.GetSource_FromThis(), position, Vector2.Zero, proj, 0, knockback, player.whoAmI);

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool LobModifyTooltips(List<TooltipLine> tooltips, ref int num)
        {
            num = Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>().LoveAndHateArcanaCost;
            return base.LobModifyTooltips(tooltips, ref num);
        }

        public override void HoldItem(Player player)
        {
            if (player.statLife > player.statLifeMax2 / 2)
                player.AddBuff(ModContent.BuffType<Love>(), 2);
            else
                player.AddBuff(ModContent.BuffType<Hatred>(), 2);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, 0);
        }
    }
}

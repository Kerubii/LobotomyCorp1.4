using LobotomyCorp.Buffs;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.QueenLaser;
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
    public class GoldRushR : SEgoItem
	{
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

		public override void SetStaticDefaults() {
            EgoColor = LobotomyCorp.WawRarity;
        }

        public override void SetDefaults() {
            Item.damage = 200;
			Item.DamageType = DamageClass.Magic;
			Item.width = 40;
			Item.height = 20;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 4;
			Item.value = 10000; 
			Item.rare = ItemRarityID.Purple;
        }
    }
}

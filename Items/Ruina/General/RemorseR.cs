using LobotomyCorp.Items.Teth;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Localization.Language;

namespace LobotomyCorp.Items.Ruina.General
{
    public class RemorseR : SEgoItem
	{
        public override bool IsLoadingEnabled(Mod mod)
        {
			return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults() 
		{
            Item.damage = 30;
			Item.DamageType = DamageClass.Default;
			Item.width = 24;
			Item.height = 24;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<TethR>();
			Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<RemorseHammer>();
            Item.shootSpeed = 16f;
			Item.autoReuse = true;            
		}

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1.5f;
            return 1f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
                type = ModContent.ProjectileType<RemorseNail>();
        }
	}
}
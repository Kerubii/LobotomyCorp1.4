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
			return false;
            //return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public override void SetDefaults() 
		{
            Item.damage = 1;
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
            Item.shoot = ModContent.ProjectileType<RemorseNail>();
            Item.shootSpeed = 1f;
			Item.autoReuse = true;            
		}
	}
}
using LobotomyCorp.Items.Waw;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LobotomyCorp.Items.Ruina.History
{
    public class GreenStemR : SEgoItem
	{
        public override void SetStaticDefaults() 
		{
            // DisplayName.SetDefault("Green Stem");
            // Tooltip.SetDefault(GetTooltip());
        }

        public override void SetDefaults()
        {
            EgoColor = LobotomyCorp.WawRarity;

            Item.damage = 48;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;

            Item.useTime = 10;
            Item.useAnimation = 10;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<WawR>();
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<Projectiles.GreenStemArea>();
            
            Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/SnowWhite_Vine") with { Volume = 0.25f };
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.channel = true;

            Item.mana = 120;
        }

        public override bool CanShoot(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] == 0;
        }

        public override void AddRecipes() 
		{
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GreenStem>())
            .AddIngredient(ItemID.Vilethorn)
            .AddIngredient(ItemID.NettleBurst)
            .AddIngredient(ItemID.Vine, 10)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
	}

}
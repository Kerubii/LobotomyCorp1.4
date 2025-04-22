using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Ruina.Language
{
    public class SmileR : SEgoItem
	{
        public override void SetStaticDefaults() 
		{
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("\"And the many shells cried out one word, \"Manager\".\"");
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
            EgoColor = LobotomyCorp.AlephRarity;
        }

        public override void SetDefaults() 
		{
            Item.damage = 280;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 48;
			Item.useAnimation = 48;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<AlephR>();
			//Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/NothingThere_Goodbye");
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Realized.SmileRSword>();
            Item.shootSpeed = 1;
            Item.channel = true;
		}

        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool AltFunctionUse(Player player)
        {
            return player.GetModPlayer<LobotomyAlephPlayer>().SmileMountain > player.statLifeMax2 * 0.6f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<SmileRSwordAlt>();
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override float UseAnimationMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1.4f;
            return base.UseAnimationMultiplier(player);
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1.4f;
            return base.UseTimeMultiplier(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Aleph.Smile>())
            .AddIngredient(ItemID.LesionBlock, 10)
            .AddIngredient(ItemID.FragmentSolar)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
    }
}
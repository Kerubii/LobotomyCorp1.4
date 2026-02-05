using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Players;

namespace LobotomyCorp.Items.Ruina.Technology
{
    public class MagicBulletR : SEgoItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Magic Bullet"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault(GetTooltip());
		}

		public override void SetDefaults() 
		{
			//Magic Bullet - Targets Nearest to Cursor
			//Inevitable Bullet - Targets 4 nearby enemies except nearest to Cursor
			//Captivating Bullet - Targets 3 Random enemies
			//Ruthless Bullet - Targets 5 nearest enemies ignoring defense
			//Silent Bullet - Targets 5 nearest enemies inflicting debuffs
			//Flooding Bullets - Target Nearest to Cursor and shoots them 5 times
			//Bullet of Despair - Targets Town NPCs and yourself

			EgoColor = LobotomyCorp.WawRarity;

			Item.damage = 320;
			Item.DamageType = ModContent.GetInstance<Players.DamageType.ExtractorRanged>();
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 100;
			Item.useAnimation = 100;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<WawR>();
			//Item.UseSound = SoundID.Item11;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
            Item.channel = true;

			Item.shoot = ModContent.ProjectileType<Projectiles.Realized.MagicBulletRGun>();
			Item.shootSpeed = 1f;
		}

        public override bool CanShoot(Player player)
        {
			return player.altFunctionUse != 2;
        }

        public override bool AltFunctionUse(Player player)
        {
			return true;
        }

        public override bool? UseItem(Player player)
        {
			if (Main.netMode != 2 && player.whoAmI == Main.myPlayer && player.altFunctionUse == 2)
            {
				player.GetModPlayer<LobotomyWawPlayer>().MagicBulletRequest = FindNearest();
            }
			return true;
        }

		private int FindNearest()
		{
			int target = -1;
			float distance = 160;
			Vector2 compareTo = Main.MouseWorld;
			foreach (NPC n in Main.ActiveNPCs)
			{
				float targetDist = n.Center.Distance(compareTo);
				if (n.friendly)
					targetDist += 80;
				if (!n.dontTakeDamage && targetDist < distance)// && n.CanBeChasedBy(this))
				{
					distance = targetDist;
					target = n.whoAmI;
				}
			}

			return target;
		}

		public override void AddRecipes() 
		{
			CreateRecipe()
			   .AddIngredient(ModContent.ItemType<MagicBullet>())
			   .AddIngredient(ItemID.SniperRifle, 1)
			   .AddIngredient(ItemID.SoulofSight, 2)
			   .AddIngredient(ItemID.Teleporter, 2)
			   .AddTile<Tiles.BlackBox3>()
               .AddCondition(RedMistCond)
               .Register();
		}
	}
}
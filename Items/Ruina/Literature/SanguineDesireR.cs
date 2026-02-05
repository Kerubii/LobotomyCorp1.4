using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using LobotomyCorp.Items.He;

namespace LobotomyCorp.Items.Ruina.Literature
{
    public class SanguineDesireR : SEgoItem
	{
		private int GlitterTimer = 0;

		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Sanguine Desire"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault(GetTooltip());
		}

		public override void SetDefaults() 
		{
			EgoColor = LobotomyCorp.HeRarity;

			Item.width = 70;
			Item.height = 70;

			Item.damage = 92;
			Item.DamageType = ModContent.GetInstance<Players.DamageType.ExtractorMelee>();
			Item.knockBack = 2.3f;
			Item.useTime = 42;
			Item.useAnimation = 42;
			Item.useStyle = ItemUseStyleID.Shoot;

			Item.value = 10000;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			//Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.rare = ModContent.RarityType<HeR>();

			Item.shoot = ModContent.ProjectileType<Projectiles.Realized.SanguineDesireR>();
			Item.shootSpeed = 16f;
		}

        public override bool SafeCanUseItem(Player player)
        {
			if (player.altFunctionUse == 2)
            {
				Item.shoot = ModContent.ProjectileType<Projectiles.Realized.SanguineDesireRHeavy>();
			}
			else
            {
				Item.shoot = ModContent.ProjectileType<Projectiles.Realized.SanguineDesireR>();
			}

			return base.SafeCanUseItem(player);
        }

        public override float UseSpeedMultiplier(Player player)
        {
			if (player.altFunctionUse == 2)
			{
				return 0.8f;
			}
			return base.UseSpeedMultiplier(player);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void HoldItem(Player player)
        {
			if (GlitterTimer > 0)
				GlitterTimer--;
			else
			{
				GlitterTimer = 1200 + Main.rand.Next(1200);
				if (Glitter(player, (int)(GlitterTimer * 0.8f)))
				{
					if (player.whoAmI == Main.myPlayer)
						SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Literature/RedShoes_On2") with { Volume = 0.5f }, player.Center);
				}
				else
					GlitterTimer = 120;
			}
            base.HoldItem(player);
        }

        public override void AddRecipes() 
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<SanguineDesire>())
			.AddIngredient(ItemID.SoulofFright, 3)
			.AddIngredient(ItemID.AdamantiteWaraxe)
			.AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();

			CreateRecipe()
			.AddIngredient(ModContent.ItemType<SanguineDesire>())
			.AddIngredient(ItemID.SoulofFright, 3)
			.AddIngredient(ItemID.TitaniumWaraxe)
			.AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
		}

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
			LobotomyGlobalNPC.SanguineDesireApplyBleed(target, 0.8f, target.damage * 3, 60, 600);
        }

        /// <summary>
        /// Returns true if an npc was Glitter'd
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool Glitter(Player player, int time, float chance = 0.8f, float distance = 2000)
        {


			bool GlitterSuccess = false;
			foreach (NPC n in Main.ActiveNPCs)
            {
				if (n.life > 0 && !n.friendly && Main.rand.NextFloat(1f) < chance && n.Center.Distance(player.Center) < distance)
                {
					GlitterSuccess = true;
					n.AddBuff(ModContent.BuffType<Buffs.Glitter>(), time);
					LobotomyGlobalNPC.LNPC(n).SanguineDesireGlitter = true;
					LobotomyGlobalNPC.LNPC(n).SanguineDesireGlitterTarget = player.whoAmI;
					n.target = player.whoAmI;
				}
            }
			return GlitterSuccess;
        }
	}
}
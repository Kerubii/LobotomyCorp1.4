using LobotomyCorp.Items.Waw;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Ruina.History
{
    public class HornetR : SEgoItem
	{
		public override void SetStaticDefaults() 
		{
            // DisplayName.SetDefault("Hornet");
            // Tooltip.SetDefault(GetTooltip());
        }

		public override void SetDefaults() 
		{
            EgoColor = LobotomyCorp.WawRarity;

            Item.damage = 110;
			Item.DamageType = ModContent.GetInstance<Players.DamageType.ExtractorSummon>();
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 26;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<WawR>();
			Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.RealizedHornet>();
            Item.shootSpeed = 122f;

            Item.channel = true;
		}

        public override bool AltFunctionUse(Player player)
        {
            return !player.HasBuff<Buffs.BoostSpore>();
        }

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.WorkerBee>()] <= 1 && player.statLife > player.statLifeMax2 * 0.25f)
            {
                int beeDamage = (Item.damage / 2);
                int bee = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.WorkerBee>(), beeDamage, Item.knockBack, player.whoAmI);
                Main.projectile[bee].originalDamage = beeDamage;
                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<Misc.Dusts.HornetDust>());
                }
            }
        }

        public override bool SafeCanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/QueenBee_AtkBuff") with {Volume = 0.25f};
                Item.shoot = 0;
                for (int i = 0; i < 40; i++)
                {
                    Vector2 vel = new Vector2(4, 0).RotateRandom(6.28f);
                    Dust dust = Main.dust[Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<Misc.Dusts.HornetDust>(), vel.X, vel.Y)];
                    dust.noLight = false;
                    dust.fadeIn = Main.rand.NextFloat(2.2f, 2.8f);
                }
                for (int i = 0; i < 80; i++)
                {
                    Vector2 vel = new Vector2(4, 0).RotateRandom(6.28f);
                    Dust dust = Main.dust[Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<Misc.Dusts.HornetDust>(), vel.X, vel.Y)];
                    dust.noLight = false;
                    dust.fadeIn = Main.rand.NextFloat(1.8f, 2f);
                }
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].chaseable && Vector2.Distance(player.Center, Main.npc[i].Center) < 1500f)
                        Main.npc[i].AddBuff(ModContent.BuffType<Buffs.BeeSpore>(), 1200);
                }
                player.AddBuff(ModContent.BuffType<Buffs.BoostSpore>(), 30 * 60);
            }
            else
            {
                Item.UseSound = null;
                Item.shoot = ModContent.ProjectileType<Projectiles.RealizedHornet>();
            }
            return true;
        }

        public override void AddRecipes() 
		{
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Hornet>())
            .AddIngredient(ItemID.Stinger, 20)
            .AddIngredient(ItemID.VialofVenom, 20)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
	}
}
using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
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
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 0f;
        }

		public override void SetDefaults() 
		{
            EgoColor = LobotomyCorp.WawRarity;

            Item.damage = 110;
            Item.DamageType = DamageClass.Summon;
            LobItemBase.ConvertVanillaDamageToExtractor(Item);
            Item.width = 40;
			Item.height = 40;
			Item.useTime = 26;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<WawR>();
			//Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            //Item.shoot = ModContent.ProjectileType<Projectiles.RealizedHornet>();
            Item.shoot = ModContent.ProjectileType<WorkerBee>();            
            Item.shootSpeed = 122f;

            Item.buffType = ModContent.BuffType<HornetWorkerBee>();
            Item.channel = true;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                velocity *= 0;

                position = Main.MouseWorld;

                float minionCount = 0;

                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.minion && p.owner == player.whoAmI)
                    {
                        minionCount += p.minionSlots;
                    }
                }
                if (minionCount > player.maxMinions - 1)
                {
                    if (!player.HasBuff<BoostSpore>()) SpawnSpores(player);
                    type = 0;
                }
                player.AddBuff(Item.buffType, 60);
            }
            else
            {
                type = ModContent.ProjectileType<Projectiles.RealizedHornet>();
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == 0)
            {
                return false;
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        /*
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
        }*/

        public override bool SafeCanUseItem(Player player)
        {
            /*
            if (player.altFunctionUse == 2)
            {
                
            }
            else
            {
                Item.UseSound = null;
                Item.shoot = ModContent.ProjectileType<Projectiles.RealizedHornet>();
            }*/
            return true;
        }

        private void SpawnSpores(Player player)
        {
            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/QueenBee_AtkBuff") with { Volume = 0.25f }, player.Center);
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
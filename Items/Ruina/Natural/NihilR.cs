using LobotomyCorp.ModSystems;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.Realized.Nihil;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Players.DamageType;

namespace LobotomyCorp.Items.Ruina.Natural
{
    public class NihilR : SEgoItem
	{
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public int ArcanaManaCost = 500;

		public override void SetStaticDefaults() {
			// Tooltip.SetDefault(GetTooltip());
            
            //Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            EgoColor = LobotomyCorp.AlephRarity;
            Item.damage = 30;
            Item.DamageType = DamageClass.Generic; LobItemBase.ConvertVanillaDamageToExtractor(Item);
            Item.width = 20;
			Item.height = 20;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 4;
			Item.value = 10000; 
			Item.UseSound = SoundID.Item11; 
			Item.autoReuse = true;
            Item.channel = true;

            Item.shoot = ModContent.ProjectileType<NihilAttack>();
            Item.shootSpeed = 0f;
            Item.rare = ModContent.RarityType<AlephR>();
        }

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI && player.GetModPlayer<LobotomyAlephPlayer>().NihilCheckActive())
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Realized.Nihil.NihilQOH>()] == 0)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Realized.Nihil.NihilQOH>(), Item.damage, Item.knockBack, player.whoAmI);
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Realized.Nihil.NihilSOW>(), Item.damage, Item.knockBack, player.whoAmI);
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Realized.Nihil.NihilKOG>(), Item.damage, Item.knockBack, player.whoAmI);
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Realized.Nihil.NihilKOD>(), Item.damage, Item.knockBack, player.whoAmI);
                }
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                LobUISystem ui = LobUISystem.Instance;
                if (ui.UINotInUse())
                    ui.NihilUIActivate();
                else
                    ui.ClearUI();
                return true;
            }

            return base.UseItem(player);
        }

        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] == 0;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, 0);
        }
    }
}

using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Ruina.Art
{
    public class FaintAromaS : SEgoItem
	{
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable;
        }

        public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("\"Bearing the hope to return to dust, it shall go back to the grave with all that desires to live.\"");

            EgoColor = LobotomyCorp.WawRarity;
        }

		public override void SetDefaults() 
		{
            Item.damage = 128;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 32;
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.knockBack = 0;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<WawR>();
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.AlriuneDeathAnimation>();
            Item.shootSpeed = 1f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            /*foreach (NPC n in Main.npc)
            {
                if (n.active && n.chaseable && n.CanBeChasedBy(ModContent.ProjectileType<Projectiles.AlriuneDeathAnimation")) && (n.Center - player.Center).Length() < 800)
                    Projectile.NewProjectile(n.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.AlriuneDeathAnimation"), Item.damage, 0, player.whoAmI, n.whoAmI);
            }*/
            if (Main.myPlayer == player.whoAmI)
                Projectile.NewProjectile(source, position, velocity, type, damage, 0, player.whoAmI, (int)(player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetal/ player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetalMax) - 1);
            return false;
        }

        public override bool SafeCanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                Item.useTime = 26;
                Item.useAnimation = 26;
                Item.shootSpeed = 1f;
                if (player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetal > player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetalMax)
                {
                    Item.shoot = ModContent.ProjectileType<Projectiles.FaintAromaS>();
                    Item.useStyle = 5;
                    Item.noUseGraphic = true;
                    Item.noMelee = true;
                    switch ((int)(player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetal / player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetalMax))
                    {
                        case 1:
                            Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/Ali_Sub_Atk");
                            break;
                        case 2:
                            Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/Ali_StrongAtk");
                            break;
                        case 3:
                            Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/Ali_StrongAtk_Finish");
                            break;
                        default:
                            Item.UseSound = SoundID.Item1;
                            break;
                    }
                }
                else
                {
                    Item.shoot = ModContent.ProjectileType<Projectiles.Realized.FaintAromaSlash>();
                    Item.shootSpeed = 24;
                    Item.useStyle = 1;
                    Item.noUseGraphic = false;
                    Item.noMelee = false;
                    Item.UseSound = SoundID.Item1;
                }
            }
            else
            {
                Item.useTime = 2;
                Item.useAnimation = 2;
                Item.shoot = 0;
                Item.shootSpeed = 0;
                Item.useStyle = ItemUseStyleID.HoldUp;
                Item.noUseGraphic = true;
                Item.noMelee = true;
                Item.UseSound = SoundID.Item1;
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.altFunctionUse == 2 && player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetal < player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetalMax * 3 + 30)
            {
                player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetal += 1f + player.GetModPlayer<LobotomyWawPlayer>().FaintAromaDecay * 2;
                if (player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetal > player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetalMax * 3 + 30)
                    player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetal = player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetalMax * 3 + 30;
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetal += 30f;
            if (player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetal > player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetalMax * 3 + 30)
                player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetal = player.GetModPlayer<LobotomyWawPlayer>().FaintAromaPetalMax * 3 + 30;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color ItemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Ruina/Art/FaintAromaSDisplay").Value, position, frame, drawColor, 0, origin, scale, 0, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = Mod.Assets.Request<Texture2D>("Items/Ruina/Art/FaintAromaSDisplay").Value;
            spriteBatch.Draw(tex, Item.position - Main.screenPosition + new Vector2(Item.width/2, Item.height - tex.Height/2), tex.Frame(), lightColor, rotation, tex.Size()/2, scale, 0, 0);
            return false;
        }

        public override void AddRecipes() 
		{
		}
	}
}
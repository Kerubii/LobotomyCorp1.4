using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class Lamp : LobCorpHeavy
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Big Bird's eyes gained another in number for every creature it saved.\n" +
                               "On this weapon, the radiant pride is apparent."); */
        }

        public override void LobSetDefaults()
        {
            Item.damage = 110;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 56;
            Item.useAnimation = 56;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<WawB>();
            SwingSound = LobotomyCorp.WeaponSounds.Hammer;
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.Waw;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            int amount = 1;
            if (target.life <= 0)
                amount = Main.rand.Next(4, 7);
            for (int i = 0; i < amount; i++)
            {
                Vector2 speed = new Vector2(16, 0).RotatedByRandom(6.28f);

                Projectile.NewProjectile(player.GetSource_FromThis(), target.Center, speed, ModContent.ProjectileType<Projectiles.LampProjectile>(), hit.Damage * 2 / 3, hit.Knockback, player.whoAmI, target.whoAmI);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("LobotomyCorp:DungeonLantern")
            .AddIngredient(ItemID.Feather, 4)
            .AddIngredient(ItemID.Bone, 10)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.He
{
    public class Christmas : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("It is patched with heavy leather of unknown origin.\n" +
							   "The stitches are carefully woven, but for whom or for what, exactly, is unclear.\n" +
							   "It is not elegant, but you can feel the devotion of its creator.\n" +
							   "Can be obtained from Presents"); */
        }

        public override void LobSetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 5000;
            Item.rare = ModContent.RarityType<HeB>();
            Item.UseSound = LobotomyCorp.WeaponSounds.Mace;
            Item.autoReuse = true;

            Item.shoot = ProjectileID.OrnamentFriendly;
            Item.shootSpeed = 16f;
            EGORiskLevel = RiskLevel.He;
        }

        public override void AddRecipes()
        {
        }
    }
}
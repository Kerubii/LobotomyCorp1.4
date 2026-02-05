using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class Diffraction : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("To see this E.G.O, one must thoroughly concentrate.\n" +
                               "You can ignore the ridiculous advice that you can see it with your mind."); */
        }

        public override void LobSetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 28;
            Item.useAnimation = 26;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 5000;
            Item.rare = ModContent.RarityType<WawB>();
            Item.UseSound = LobotomyCorp.WeaponSounds.Mace;
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.Waw;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.TheBreaker)
            .AddIngredient(ItemID.InvisibilityPotion, 4)
            .AddTile(Mod, "BlackBox3")
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.FleshGrinder)
            .AddIngredient(ItemID.InvisibilityPotion, 4)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            DiffractionApply(target);
        }

        public static void DiffractionApply(NPC npc)
        {
            LobotomyGlobalNPC Lnpc = LobotomyGlobalNPC.LNPC(npc);
            if (Lnpc.DiffractionDiffracted)
                return;

            Lnpc.DiffractionDiffracted = true;
            float range = 0.5f;
            if (npc.boss)
                range = 0.5f - 0.5f * 0.8f;

            Lnpc.DiffractionDamage = Main.rand.NextFloat(1f - range, 1f + range);
            Lnpc.DiffractionDefense = Main.rand.NextFloat(1f - range, 1f + range);
            Lnpc.DiffractionHealth = Main.rand.NextFloat(1f - range, 1f + range);

            npc.lifeMax = (int)(npc.lifeMax * Lnpc.DiffractionHealth);
        }
    }
}
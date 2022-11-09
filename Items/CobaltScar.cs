using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items
{
	public class CobaltScar : LobCorpLight
	{
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("The weapon resembles the claws of a vicious wolf.\n" +
                               "Once upon a time, these claws would cut open the bellies of numerous creatures and tear apart their guts.");
        }

        public override void SetDefaults() {
			Item.CloneDefaults(ItemID.FetidBaghnakhs);
			Item.damage = 56;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = 15;
            Item.rare = ItemRarityID.Purple;
            Item.scale = 1.2f;
            Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/Wolf_Scratch") with { Volume = 0.5f, PitchVariance = 0.1f };
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.statLife <= player.statLifeMax / 2)
            {
                damage += 0.5f;    
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (Main.rand.NextBool(3))
                player.AddBuff(ModContent.BuffType<Buffs.WillBeBad>(), 180);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(3))
                player.AddBuff(ModContent.BuffType<Buffs.WillBeBad>(), 180);
        }

        public override void AddRecipes() {
            CreateRecipe()
            .AddIngredient(ItemID.BladedGlove)
            .AddIngredient(ItemID.CyanHusk, 2)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
	}
}
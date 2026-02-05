using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class CobaltScar : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("The weapon resembles the claws of a vicious wolf.\n" +
                               "Once upon a time, these claws would cut open the bellies of numerous creatures and tear apart their guts.\n" +
                               "Hitting an enemy has a chance of increasing melee damage by 10%\n" +
                               "50% increased damage while under 50% health"); */
        }

        public override void LobSetDefaults()
        {
            Item.CloneDefaults(ItemID.FetidBaghnakhs);
            Item.damage = 60;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 15;
            Item.rare = ModContent.RarityType<WawB>();
            Item.scale = 1.2f;
            Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/Wolf_Scratch") with { Volume = 0.5f, PitchVariance = 0.1f };
            EGORiskLevel = RiskLevel.Waw;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.statLife <= player.statLifeMax / 2)
            {
                damage += 0.5f;
            }
        }

        public override void HoldItem(Player player)
        {
            if (player.statLife <= player.statLifeMax / 2)
            {
                player.AddBuff(ModContent.BuffType<Buffs.WillBeBad>(), 5);
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
                player.AddBuff(ModContent.BuffType<Buffs.WillBeBad>(), 600);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool(3))
                player.AddBuff(ModContent.BuffType<Buffs.WillBeBad>(), 600);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BladedGlove)
            .AddIngredient(ItemID.CyanHusk)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}
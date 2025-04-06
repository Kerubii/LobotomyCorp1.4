using LobotomyCorp.Items.Ruina.Natural;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class Love : ModBuff
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Festival");
            // Description.SetDefault("Everything will be peaceful");
            //BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.Love.ArcanaSlaveCost", Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>().LoveAndHateArcanaCost)}";
        }

        public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
        {
            string cost = "" + Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>().LoveAndHateArcanaCost;
            DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.ItemStack.Value, cost, drawParams.TextPosition, drawParams.DrawColor, 0f, default(Vector2), 0.8f, (SpriteEffects)0, 0f);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            LobotomyZayinPlayer modPlayer = player.GetModPlayer<LobotomyZayinPlayer>();
            if (player.HeldItem.type != ModContent.ItemType<InTheNameOfLoveAndHateR>())
            {
                player.DelBuff(buffIndex);
                --buffIndex;
            }
        }
    }
}
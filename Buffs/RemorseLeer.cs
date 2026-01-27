using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class RemorseLeer : ModBuff
	{
		public override void SetStaticDefaults()
		{
            Main.buffNoTimeDisplay[Type] = true;
        }
        
        public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
        {
            float leer = Main.LocalPlayer.GetModPlayer<LobotomyTethPlayer>().RemorseLeer;
            string stacks = ""+leer;
            DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.ItemStack.Value, stacks, drawParams.TextPosition, drawParams.DrawColor, 0f, default(Vector2), 0.8f, (SpriteEffects)0, 0f);
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}
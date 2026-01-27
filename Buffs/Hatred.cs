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
	public class Hatred : ModBuff
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Festival");
            // Description.SetDefault("Everything will be peaceful");
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.Love.ArcanaSlaveCost", Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>().LoveAndHateArcanaCost)}";
            tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.Love.Hysteria", (int)(Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>().LoveAndHateHysteriaPercent() * 100))}";
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            LobotomyWawPlayer wawPlayer = Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>();
            string cost = "" + wawPlayer.LoveAndHateArcanaCost;
            Texture2D texture = TextureAssets.Buff[ModContent.BuffType<Love>()].Value;//Mod.Assets.Request<Texture2D>("Buffs/Love").Value;
            int length = (int)(30 * wawPlayer.LoveAndHateHysteriaPercent());
            Rectangle frame = new Rectangle(0, 0, length, 32);
            spriteBatch.Draw(drawParams.Texture, drawParams.Position, frame, drawParams.DrawColor, 0, new Vector2(0, 0), 1, 0, 0);
            frame = new Rectangle(length, 0, 32 - length, 32);
            spriteBatch.Draw(texture, drawParams.Position + new Vector2(length, 0), frame, drawParams.DrawColor, 0, new Vector2(0, 0), 1, 0, 0);
            DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.ItemStack.Value, cost, drawParams.TextPosition, drawParams.DrawColor, 0f, default(Vector2), 0.8f, (SpriteEffects)0, 0f);
            return false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            LobotomyWawPlayer modPlayer = player.GetModPlayer<LobotomyWawPlayer>();
            modPlayer.LoveAndHateHatred = true;
            //player.endurance -= 0.25f;
            if (modPlayer.LoveAndHateHysteria <= 0)
            {
                player.DelBuff(buffIndex);
                --buffIndex;
            }
        }
    }
}
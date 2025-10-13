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
	public class Justice : ModBuff
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Festival");
            // Description.SetDefault("Everything will be peaceful");
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            LobotomyWawPlayer wawPlayer = Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>();
            int count = wawPlayer.SwordSharpenedImpaledCount;
            Texture2D texture = Mod.Assets.Request<Texture2D>("Buffs/Despair").Value;

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    Vector2 pos = drawParams.Position + new Vector2(16 * x, 16 * y);
                    Rectangle frame = new Rectangle(16 * x, 16 * y, 16, 16);
                    if (x == y)
                    {
                        if ((x == 0 && count > 0) || count > 1)
                            spriteBatch.Draw(texture, pos, frame, drawParams.DrawColor, 0, Vector2.Zero, 1, 0, 0);   
                        else
                            spriteBatch.Draw(drawParams.Texture, pos, frame, drawParams.DrawColor, 0, Vector2.Zero, 1, 0, 0);
                    }
                    else
                        spriteBatch.Draw(drawParams.Texture, pos, frame, drawParams.DrawColor, 0, Vector2.Zero, 1, 0, 0);
                }
            }

            DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.ItemStack.Value, "" + count, drawParams.TextPosition, drawParams.DrawColor, 0f, default(Vector2), 0.8f, (SpriteEffects)0, 0f);
            return false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            LobotomyWawPlayer modPlayer = player.GetModPlayer<LobotomyWawPlayer>();
            modPlayer.SwordSharpenedJustice = true;
            if (player.HeldItem.type != ModContent.ItemType<SwordSharpenedWithTearsR>())
            {
                player.DelBuff(buffIndex);
                --buffIndex;
            }
        }
    }
}
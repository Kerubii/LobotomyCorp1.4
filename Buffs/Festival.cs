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
	public class Festival : ModBuff
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
            tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.Festival.HealPoolDesc", Main.LocalPlayer.GetModPlayer<LobotomyZayinPlayer>().WingbeatRegenPool)}";
        }

        public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
        {
            string stacks = "" + Main.LocalPlayer.GetModPlayer<LobotomyZayinPlayer>().WingbeatRegenPool;
            DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.ItemStack.Value, stacks, drawParams.TextPosition, drawParams.DrawColor, 0f, default(Vector2), 0.8f, (SpriteEffects)0, 0f);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            LobotomyZayinPlayer modPlayer = player.GetModPlayer<LobotomyZayinPlayer>();
            modPlayer.WingbeatFairyMeal = true;
            player.moveSpeed += 0.05f;
            player.GetDamage(DamageClass.Generic) += 0.05f;
            if (player.FindBuffIndex(BuffID.PotionSickness) != -1)
            {
                modPlayer.WingbeatRegenPool = 0;
                modPlayer.WingbeatRegenTime = 0;
                player.DelBuff(buffIndex);
                buffIndex--;
            }

            if (modPlayer.WingbeatRegenPool <= 0)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
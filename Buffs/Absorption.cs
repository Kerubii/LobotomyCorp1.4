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
	public class Absorption : ModBuff
	{
		public override void SetStaticDefaults()
		{
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
       
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            int corpseStack = Main.LocalPlayer.GetModPlayer<LobotomyAlephPlayer>().SmileMountain;
            float percent = Math.Min(corpseStack / (float)Main.LocalPlayer.statLifeMax2, 1f);

            int damage = (int)(20 * percent);
            int resistance = (int)(25 + 25 * percent);
            int swing = (int)(10 * percent);
            int speed = (int)(20 * percent);

            tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.Absorption.Tooltip2", damage)}";
            tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.Absorption.Tooltip3", resistance)}";
            tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.Absorption.Tooltip4", swing)}";
            tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.Absorption.Tooltip5", speed)}";
            tip += $"\n{Language.GetTextValue("Mods.LobotomyCorp.Buffs.Absorption.Tooltip1", corpseStack)}";
        }

        public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
        {
            float mountain = Main.LocalPlayer.GetModPlayer<LobotomyAlephPlayer>().SmileMountain / (float)Main.LocalPlayer.statLifeMax2;
            float percent = (int)(mountain * 100);
            string stacks = percent + "%";
            DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.ItemStack.Value, stacks, drawParams.TextPosition, drawParams.DrawColor, 0f, default(Vector2), 0.8f, (SpriteEffects)0, 0f);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            int corpseStack = player.GetModPlayer<LobotomyAlephPlayer>().SmileMountain;
            float percent = Math.Min(corpseStack / (float) player.statLifeMax2, 1f);
            player.GetDamage(DamageClass.Generic) += (.2f * percent);
            player.GetAttackSpeed(DamageClass.Generic) -= 0.1f * percent;
            player.moveSpeed -= 0.2f * percent;
            if (corpseStack <= 0)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }    
        }
    }
}
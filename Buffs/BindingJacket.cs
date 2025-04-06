using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Audio;
using LobotomyCorp.Players;

namespace LobotomyCorp.Buffs
{
	public class BindingJacket : ModBuff
	{
		public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("Binding Jacket");
			// Description.SetDefault("Why is this place always so dark and cold?");
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) -= 0.35f;
            player.statDefense -= 10;
            player.GetModPlayer<LobotomyTethPlayer>().RegretBinded = true;
        }
    }
}
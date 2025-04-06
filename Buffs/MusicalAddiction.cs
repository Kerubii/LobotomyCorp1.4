using LobotomyCorp.Players;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class MusicalAddiction : ModBuff
	{
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Musical Addiction");
			// Description.SetDefault("Never stop performing until the body crumbles to dust");
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }

        public override void Update(Player player, ref int buffIndex)
        {

            player.GetDamage(DamageClass.Generic) += 0.12f;
            player.GetAttackSpeed(DamageClass.Generic) += 0.08f;
            player.GetModPlayer<LobotomyHePlayer>().HarmonyAddiction = true;
        }
    }
}
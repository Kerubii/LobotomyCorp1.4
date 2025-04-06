using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
	public class WillBeBad : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blue Moon");
			// Description.SetDefault("10% increased melee damage");
		}

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Melee) += 0.1f;
			player.moveSpeed += .1f;
        }
    }
}
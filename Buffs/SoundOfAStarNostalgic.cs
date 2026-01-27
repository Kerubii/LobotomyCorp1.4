using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.Realized;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
    public class SoundOfAStarNostalgic : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.1f;
        }
    }
}
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles;
using LobotomyCorp.Projectiles.Realized;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{
    public class FeatherOfHonorM : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // If the minions exist reset the buff time, otherwise remove the buff from the player
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FeatherOfHonorMinion>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
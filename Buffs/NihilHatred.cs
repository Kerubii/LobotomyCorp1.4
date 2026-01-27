using Terraria;
using Terraria.ModLoader;
using LobotomyCorp.Players;
using static LobotomyCorp.Players.LobotomyAlephPlayer;

namespace LobotomyCorp.Buffs
{
    public class NihilHatred : Nihil
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;

            DefenseA = 62;

            DamageA = .60f;
            DamageB = .4f;

            SpeedA = .14f;
            SpeedB = .0f;

            CriticalA = .4f;
            CriticalB = .02f;
        }

        public override void ExtraEffects(Player player, bool notDebuffed)
        {
            player.manaCost -= notDebuffed ? .25f : .05f;
        }

        public override NihilType GetMode => NihilType.Hatred;

        public override DamageClass GetDamageType => DamageClass.Magic;
    }
}
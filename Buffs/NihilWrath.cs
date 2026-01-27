using Terraria;
using Terraria.ModLoader;
using LobotomyCorp.Players;
using static LobotomyCorp.Players.LobotomyAlephPlayer;

namespace LobotomyCorp.Buffs
{
    public class NihilWrath : Nihil
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;

            DefenseA = 74;

            DamageA = .72f;
            DamageB = .08f;

            SpeedA = .15f;
            SpeedB = .03f;

            CriticalA = .33f;
            CriticalB = .02f;
        }

        public override NihilType GetMode => NihilType.Wrath;

        public override DamageClass GetDamageType => DamageClass.Melee;
    }
}
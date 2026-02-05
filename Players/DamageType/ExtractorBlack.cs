using Terraria.ModLoader;

namespace LobotomyCorp.Players.DamageType
{
    public class ExtractorBlack : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic ||
                damageClass == ModContent.GetInstance<ExtractorDamage>())
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }
    }
}
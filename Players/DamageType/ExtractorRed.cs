using Terraria.ModLoader;

namespace LobotomyCorp.Players.DamageType
{
    public class ExtractorRed : DamageClass
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
using Terraria.ModLoader;

namespace LobotomyCorp.Players.DamageType
{
    public class ExtractorMagic : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic ||
                damageClass == ModContent.GetInstance<ExtractorDamage>())
                return StatInheritanceData.Full;
            if (damageClass == DamageClass.Magic)
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Magic ||
                damageClass == ModContent.GetInstance<ExtractorDamage>())
                return true;
            return false;
        }

        public override bool GetPrefixInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Magic)
                return true;
            return false;
        }
    }
}
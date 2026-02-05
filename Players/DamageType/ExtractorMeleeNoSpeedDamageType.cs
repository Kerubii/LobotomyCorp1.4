using Terraria.ModLoader;

namespace LobotomyCorp.Players.DamageType
{
    public class ExtractorMeleeNoSpeed : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic || 
                damageClass == ModContent.GetInstance<ExtractorDamage>())
                return StatInheritanceData.Full;
            if (damageClass == DamageClass.MeleeNoSpeed)
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.MeleeNoSpeed ||
                damageClass == ModContent.GetInstance<ExtractorDamage>())
                return true;
            return false;
        }

        public override bool GetPrefixInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.MeleeNoSpeed)
                return true;
            return false;
        }
    }
}
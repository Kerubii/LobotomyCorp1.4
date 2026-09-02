using Terraria.ModLoader;

namespace LobotomyCorp.Players.DamageType
{
    public class ExtractorSummon : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic ||
                damageClass == ModContent.GetInstance<ExtractorDamage>())
                return StatInheritanceData.Full;
            if (damageClass == DamageClass.Summon)
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }

        public override bool UseStandardCritCalcs => false;

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic || 
                damageClass == DamageClass.Summon ||
                damageClass == ModContent.GetInstance<ExtractorDamage>())
                return true;
            return false;
        }

        public override bool GetPrefixInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic || damageClass == DamageClass.Summon)
                return true;
            return false;
        }
    }
}
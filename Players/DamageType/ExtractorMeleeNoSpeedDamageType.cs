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
            if (damageClass == DamageClass.Melee)
                return new StatInheritanceData(1, 1, 0, 1, 1);
            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic || 
                damageClass == DamageClass.MeleeNoSpeed ||
                damageClass == DamageClass.Melee ||
                damageClass == ModContent.GetInstance<ExtractorDamage>())
                return true;
            return false;
        }

        
        public override bool GetPrefixInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic || damageClass == DamageClass.Melee || damageClass == DamageClass.MeleeNoSpeed)
                return true;
            return false;
        }
    }
}
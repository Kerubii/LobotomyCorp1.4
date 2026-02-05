using Terraria.ModLoader;

namespace LobotomyCorp.Players.DamageType
{
    public class ExtractorMelee : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic || 
                damageClass == ModContent.GetInstance<ExtractorDamage>())
                return StatInheritanceData.Full;
            if (damageClass == DamageClass.Melee)
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Melee ||
                damageClass == ModContent.GetInstance<ExtractorDamage>())
                return true;
            return false;
        }

        public override bool GetPrefixInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Melee)
                return true;
            return false;
        }
    }
}
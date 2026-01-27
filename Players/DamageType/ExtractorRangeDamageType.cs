using Terraria.ModLoader;

namespace LobotomyCorp.Players.DamageType
{
    public class ExtractorRanged : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic ||
                damageClass == Mod.GetContent<ExtractorDamage>())
                return StatInheritanceData.Full;
            if (damageClass == DamageClass.Ranged)
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Ranged ||
                damageClass == Mod.GetContent<ExtractorDamage>())
                return true;
            return false;
        }

        public override bool GetPrefixInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Ranged)
                return true;
            return false;
        }
    }
}
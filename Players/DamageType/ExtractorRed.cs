using Terraria.ModLoader;

namespace LobotomyCorp.Players.DamageType
{
    public class ExtractorRed : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic ||
                damageClass == Mod.GetContent<ExtractorDamage>())
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }
    }
}
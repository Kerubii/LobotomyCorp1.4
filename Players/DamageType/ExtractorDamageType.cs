using System;
using System.Collections.Generic;
using System.Linq;
using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Aleph;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.ModSystems;
using LobotomyCorp.NPCs.RedMist;
using LobotomyCorp.PlayerDrawEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace LobotomyCorp.Players.DamageType
{
    /// <summary>
    /// Used for ZAYIN ego effects
    /// </summary>
    public class ExtractorDamage : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic)
                return StatInheritanceData.Full;

            if (damageClass == DamageClass.Melee ||
                damageClass == DamageClass.Ranged ||
                damageClass == DamageClass.Magic ||
                damageClass == DamageClass.Summon)
                return new StatInheritanceData(
                    damageInheritance: 0.25f,
                    critChanceInheritance: 0.25f,
                    attackSpeedInheritance: 0.25f,
                    armorPenInheritance: 0.25f,
                    knockbackInheritance: 0.25f
                );

            return StatInheritanceData.None;
        }


    }
}
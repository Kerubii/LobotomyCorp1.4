using System;
using System.Collections.Generic;
using System.Linq;
using LobotomyCorp.Buffs;
using LobotomyCorp.Configs;
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
    public class ExtractorDefault : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == ModContent.GetInstance<ExtractorDamage>())
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            if (damageClass == ModContent.GetInstance<ExtractorDamage>())
                return true;
            return false;
        }
    }
}
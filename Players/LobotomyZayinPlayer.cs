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

namespace LobotomyCorp.Players
{
    /// <summary>
    /// Used for ZAYIN ego effects
    /// </summary>
    public class LobotomyZayinPlayer : ModPlayer
    {
        public int RealizedWingbeatMeal = -1;
        public bool WingbeatFairyMeal = false;
        public bool WingbeatGluttony = false;
        public int WingbeatRegenPool = 0;
        public int WingbeatRegenTime = 0;
        public int WingbeatRegenTimeMax = 60;
        public int WingbeatRegenHeal = 5;

        public override void ResetEffects()
        {
            if (RealizedWingbeatMeal >= 0 && (!Main.npc[RealizedWingbeatMeal].active || Main.npc[RealizedWingbeatMeal].life <= 0))
                RealizedWingbeatMeal = -1;
            WingbeatFairyMeal = false;
            WingbeatGluttony = false;
        }

        public override void PreUpdate()
        {
            if (RealizedWingbeatMeal > -1)
            {
                Main.npc[RealizedWingbeatMeal].GetGlobalNPC<LobotomyGlobalNPC>().WingbeatTarget = Player.whoAmI;
            }
        }

        public override void PostUpdate()
        {
            if (WingbeatRegenPool > 0)
            {
                if (WingbeatRegenTime > 0)
                    WingbeatRegenTime--;
                else
                {
                    int heal = WingbeatRegenHeal;
                    if (WingbeatRegenPool < WingbeatRegenHeal)
                        heal = WingbeatRegenPool;
                    WingbeatRegenPool -= heal;
                    Player.Heal(heal);
                    WingbeatRegenTime = WingbeatRegenTimeMax;
                }
            }
        }

        public override void UpdateBadLifeRegen()
        {
            /*
            if (WingbeatGluttony && !WingbeatFairyMeal)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
                Player.lifeRegen = -15;
            }*/
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (WingbeatRegenPool > 0)
            {
                WingbeatRegenPool = 0;
                WingbeatRegenTime = 0;
            }
        }

        public void WingbeatStoreHeal(int healAmount)
        {
            int num = Player.FindBuffIndex(BuffID.PotionSickness);
            if (num != -1)
            {
                return;
            }

            WingbeatRegenPool += healAmount;
            if (WingbeatRegenTime == 0)
            {
                WingbeatRegenTime = WingbeatRegenTimeMax;
            }
            Player.AddBuff(ModContent.BuffType<Buffs.Festival>(), 300);
        }
    }
}
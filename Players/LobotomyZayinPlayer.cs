using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Aleph;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Items.Zayin;
using LobotomyCorp.ModSystems;
using LobotomyCorp.NPCs.RedMist;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public int PenitenceMaxTime = 1;
        public int PenitenceAttackDuration = 0;

        public override void ResetEffects()
        {
            if (RealizedWingbeatMeal >= 0 && (!Main.npc[RealizedWingbeatMeal].active || Main.npc[RealizedWingbeatMeal].life <= 0))
                RealizedWingbeatMeal = -1;
            WingbeatFairyMeal = false;
            WingbeatGluttony = false;
        }

        public override void UpdateDead()
        {
            PenitenceMaxTime = 1;
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
                WingbeatRegenTimeMax = Math.Clamp((int)(120 * (1f - WingbeatRegenPool / 200f)), 20, 60);
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
                WingbeatRegenPool = Math.Min((int)(WingbeatRegenPool * 0.1f), 30);
                WingbeatRegenTime = 0;
            }

            if (Player.HasBuff<PenitenceAtonement>())
            {
                int bufindex = Player.FindBuffIndex(ModContent.BuffType<PenitenceAtonement>());
                int reduction = (int)Math.Min(info.Damage, 60 * 2) * 5;
                Player.buffTime[bufindex] -= reduction;
                if (Player.buffTime[bufindex] < 1)
                    Player.buffTime[bufindex] = 1;
            }

            if (Player.HeldItem.type == ModContent.ItemType<Penitence>())
                Player.statMana = Math.Min(Player.statManaMax2, Player.statMana + info.SourceDamage);
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

        
        public void PenitenceHardshipAttack(Item held, int cost, int target = -1, bool ignoreAttackSpeed = false, float damageMult = 1f)
        {
            float attackSpeed = held.useTime;
            if (!ignoreAttackSpeed)
            {
                if (PenitenceAttackDuration > 0)
                {
                    PenitenceAttackDuration--;
                    return;
                }
                PenitenceAttackDuration = (int)attackSpeed;
            }

            int damage = (int)(Player.GetWeaponDamage(held) * damageMult);
            int type = ModContent.ProjectileType<PenitenceRLight>();
            
            float dist = 2000;
            int nearest = target;
            if (nearest == -1)
            {
                foreach (NPC n in Main.npc)
                {
                    if (n.CanBeChasedBy(type))
                    {
                        float nDist = n.Center.Distance(Player.Center);
                        if (nDist < dist)
                        {
                            dist = nDist;
                            nearest = n.whoAmI;
                        }
                    }
                }
            }            
            if (nearest > -1)
            {
                if (cost > -1)
                {
                    if (!Player.CheckMana(cost, true))
                        return;
                    Player.manaRegenDelay = Player.maxRegenDelay;
                }
                Projectile.NewProjectile(held.GetSource_FromThis(), Main.npc[nearest].Center, Vector2.Zero, type, damage, 0, Player.whoAmI);
            }
        }

        /// <summary>
        /// Returns remaining time from maxtime in 0-1 float
        /// </summary>
        /// <returns></returns>
        public float PenitenceHealNearbyAllies()
        {
            int buftype = ModContent.BuffType<PenitenceAtonement>();
            int remainingTime = PenitenceMaxTime;            

            if (Player.HasBuff<PenitenceAtonement>())
            {
                remainingTime = PenitenceMaxTime - Player.buffTime[Player.FindBuffIndex(buftype)];
                if (remainingTime < (int)(PenitenceMaxTime * 0.25f))
                    remainingTime = 0;
                else if (remainingTime > PenitenceMaxTime)
                    remainingTime = PenitenceMaxTime;

                if (PenitenceMaxTime <= 1)
                {
                    PenitenceMaxTime = 1;
                    remainingTime = 1;
                }
            }

            float teee = (float)remainingTime / PenitenceMaxTime;
            Main.NewText(remainingTime +" "+ PenitenceMaxTime);

            PenitenceMaxTime = 60 * 120;

            Player.AddBuff(buftype, PenitenceMaxTime);

            float amountHeal = Player.statLifeMax2 * 0.5f * teee;

            foreach (Player p in Main.ActivePlayers)
            {
                if (!p.dead && p.team == Player.team)
                {
                    p.statLife += (int)amountHeal;
                    p.HealEffect((int)amountHeal);
                }
            }

            return teee;
        }
    }
}
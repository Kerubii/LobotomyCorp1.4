using System;
using System.Collections.Generic;
using System.Linq;
using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Aleph;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.ModSystems;
using LobotomyCorp.NPCs.RedMist;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Steamworks;
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
    /// Used for ALEPH ego effects
    /// </summary>
    public class LobotomyAlephPlayer : ModPlayer
    {
        public bool DaCapoSilentMusic = false;
        // Used by buff "Silent Music"
        public int DaCapoSilentMusicPhase = 0;
        public int DaCapoTotalDamage = 0;

        public int TwilightSpecial = 10;

        public bool MimicryShell = false;
        public int MimicryShellHealth = 0;
        public float MimicryShellDamage = 1f;
        public int MimicryShellDefense = 0;
        public int MimicryShellTimerMax = 0;
        public bool MimicryHusk = false;
        public int MimicryHuskDeficit = 0;

        public bool NihilActive = false;
        public int NihilMode = 0;

        public bool SmileDebuff = false;
        public int SmileMountain = 0;
        public bool SmileMelting = false;

        public override void ResetEffects()
        {
            DaCapoSilentMusic = false;
            MimicryShell = false;
            MimicryHusk = false;
            MimicryHuskDeficit = 0;

            NihilActive = false;

            SmileDebuff = false;
            SmileMelting = false;
        }

        public override void UpdateDead()
        {
            NihilActive = false;

            SmileMountain = 0;
        }

        public override void UpdateBadLifeRegen()
        {
            if (SmileDebuff)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
                Player.lifeRegen = -30;
            }
        }

        public override void OnEnterWorld()
        {
            SmileMountain = 0;

            if (Player.HasBuff<SilentMusic>())
                Player.ClearBuff(ModContent.BuffType<SilentMusic>());

            if (Player.HasBuff<Absorption>())
                Player.ClearBuff(ModContent.BuffType<Absorption>());

            if (Player.HasBuff<Buffs.Shell>())
                Player.ClearBuff(ModContent.BuffType<Buffs.Shell>());
            if (Player.HasBuff<Buffs.Husk>())
                Player.ClearBuff(ModContent.BuffType<Buffs.Husk>());
        }

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
            if (Player.HasBuff<Absorption>())
            {
                Player.ClearBuff(ModContent.BuffType<Absorption>());
                SmileMountain = 0;
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (SmileMountain > 0)
            {
                float percent = Math.Min(1f, (float)SmileMountain / Player.statLifeMax2);
                float resistance = .25f + .25f * percent;
                modifiers.FinalDamage *= 1f - resistance;
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (SmileMountain > 0)
            {
                float percent = Math.Min(1f, (float)SmileMountain / Player.statLifeMax2);
                float resistance = .25f + .25f * percent;
                int damage = (int)(info.SourceDamage * resistance);
                SmileReduceCorpse(damage, true);
            }
        }

        public override void PostHurt(Player.HurtInfo info)
        {
            int heldItem = Player.HeldItem.type;
            if (heldItem == ModContent.ItemType<Items.Aleph.Censored>())
            {
                float healing = (float)info.Damage * 0.4f;
                if (info.Damage > 1)
                {
                    Player.statLife += (int)healing;
                    Player.HealEffect((int)healing);
                }
            }
        }

        /// <summary>
        /// Copies enemy stats when killed by Mimicry, Soft Limits are HP = 500, Damage = 50%, Defense = 30
        /// </summary>
        /// <param name="n"></param>
        public void MimicryWearShell(NPC n)
        {
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.Blood);
            }

            if (Player.HasBuff<Husk>())
            {
                Player.ClearBuff(ModContent.BuffType<Husk>());
            }
            int time = Math.Min(n.lifeMax * 30, 300 * 60);
            Player.AddBuff(ModContent.BuffType<Shell>(), time);
            MimicryShell = true;
            MimicryShellDamage = Math.Min(n.damage / 10f, .8f);
            MimicryShellDefense = Math.Min(n.defense, 30);
            MimicryShellHealth = Math.Min((int)(n.lifeMax * 0.1f), 500);
            MimicryShellTimerMax = time;
        }

        /// <summary>
        /// Extends shell buff timer and auto applies it to self if shell is missing
        /// </summary>
        /// <param name="Time"></param>
        public void MimicryIncreaseShellTime(int Time)
        {
            int index = Player.FindBuffIndex(ModContent.BuffType<Shell>());
            Player.buffTime[index] += Time;
            if (Player.buffTime[index] > MimicryShellTimerMax)
                Player.buffTime[index] = MimicryShellTimerMax;
        }

        /// <summary>
        /// Gets the remaining shell timer as a percentage to multiply on bonus stats for decay
        /// </summary>
        /// <returns></returns>
        public float MimicryShellPercent()
        {
            int index = -1;
            if (Player.HasBuff<Shell>())
                index = Player.FindBuffIndex(ModContent.BuffType<Shell>());
            if (index < 0)
                return 0;

            float percent = (float)Player.buffTime[index] / MimicryShellTimerMax;
            if (percent > 1f)
                percent = 1f;
            return percent;
        }
        public int MimicryBonusHealth { get { return (int)(MimicryShellHealth * MimicryShellPercent()); } }
        public int MimicryBonusDamage { get { return (int)(MimicryShellDamage * MimicryShellPercent()); } }
        public int MimicryBonusDefense { get { return (int)(MimicryShellDefense * MimicryShellPercent()); } }

        /// <summary>
        /// Is player elligible to use Nihil, have 0 items and armor in all slots
        /// </summary>
        /// <returns></returns>
        public bool NihilCheckActive()
        {
            bool checkActive = false;

            for (int slot = 0; slot < Main.InventorySlotsTotal; slot++)
            {
                if (!Player.inventory[slot].IsAir && Player.inventory[slot].type != ModContent.ItemType<Items.Ruina.Natural.NihilR>())
                {
                    checkActive = true;
                }
            }

            for (int slot = 0; slot < 8 + Player.extraAccessorySlots; slot++)
            {
                if (!Player.armor[slot].IsAir && Player.armor[slot].type != ModContent.ItemType<Items.Ruina.Natural.NihilR>())
                {
                    checkActive = true;
                }
            }

            NihilActive = checkActive;
            return NihilActive;
        }

        public void SmileCreateCorpse(NPC target, bool isSmall = false)
        {
            if (SmileMelting || NPCID.Sets.ProjectileNPC[target.type] == true)
                return;
            Vector2 randVel = new Vector2(Main.rand.NextFloat(8, 12), 0).RotatedBy(Main.rand.NextFloat(6.28f));
            if (isSmall)
                Projectile.NewProjectile(target.GetSource_FromThis(), target.Center, randVel, ModContent.ProjectileType<SmileCorpseSmall>(), 0, 0, Player.whoAmI, Player.statLifeMax2 * 0.01f);
            else
                Projectile.NewProjectile(target.GetSource_FromThis(), target.Center, randVel, ModContent.ProjectileType<SmileCorpse>(), 0, 0, Player.whoAmI, Player.statLifeMax2 * 0.1f);
        }

        public void SmileCreateCorpseBoss(NPC target)
        {
            if (!target.boss || SmileMelting)
                return;
            LobotomyGlobalNPC modTarget = target.GetGlobalNPC<LobotomyGlobalNPC>();
            if (target.realLife > -1)
                modTarget = Main.npc[target.realLife].GetGlobalNPC<LobotomyGlobalNPC>();
            float lifePercent = (float)target.life / (float)target.lifeMax;
            for (float i = 0.1f; i < modTarget.SmileCorpseThreshold; i += 0.1f)
            {
                if (lifePercent < i)
                {
                    Vector2 randVel = new Vector2(Main.rand.NextFloat(8, 12), 0).RotatedBy(Main.rand.NextFloat(6.28f));
                    Projectile.NewProjectile(target.GetSource_FromThis(), target.Center, randVel, ModContent.ProjectileType<SmileCorpse>(), 0, 0, Player.whoAmI, Player.statLifeMax2 * 0.1f);

                    modTarget.SmileCorpseThreshold = i;
                    break;
                }
            }
        }

        public void SmileConsumeCorpse(Projectile p, bool announce = true)
        {
            if (SmileMelting)
                return;
            Player.AddBuff(ModContent.BuffType<Absorption>(), 60);
            int amount = (int)p.ai[0];
            SmileMountain += amount;
            if (announce)
                CombatText.NewText(Player.getRect(), Color.DarkGray, amount);
            if (SmileMountain > Player.statLifeMax2)
                SmileMountain = Player.statLifeMax2;

            for (int i = 0; i < 5; i++)
            {
                Dust d = Main.dust[Dust.NewDust(p.position, p.width, p.height, DustID.Wraith)];
                Vector2 dustVel = Player.Center - p.Center;
                dustVel.Normalize();
                d.velocity = dustVel * Main.rand.Next(2,8);
                d.noGravity = true;
            }
            p.Kill();
        }

        /// <summary>
        /// Function to reduce Smile's Mountain of Corpses passive, Set announce to true to show combat text numbers
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="announce"></param>
        public void SmileReduceCorpse(int amount, bool announce = false)
        {
            SmileMountain -= amount;
            if (announce)
                CombatText.NewText(Player.getRect(), new Color(0.4f, 0.4f, 0.4f), amount);
            if (SmileMountain < 0)
            {
                SmileMountain = 0;
                Player.ClearBuff(ModContent.BuffType<Absorption>());
                Player.AddBuff(ModContent.BuffType<SmileMelting>(), 600);
            }
        }
    }
}
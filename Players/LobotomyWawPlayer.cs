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
    /// Used for WAW ego effects
    /// </summary>
    public class LobotomyWawPlayer : ModPlayer
    {
        public int BlackSwanParryChance = 0;
        public float BlackSwanNettleClothing = 0;
        public static int BlackSwanNettleClothingMax = 6;
        public bool BlackSwanBrokenDream = false;

        /// <summary>
        /// Use 1 to empower Melee, Use 2 to empower Range
        /// </summary>
        public int CrimsonScarEmpower = 0;

        public float FaintAromaPetal = 0;
        public int FaintAromaPetalMax = 60;
        public float FaintAromaDecay = 0.1f;

        public bool PleasureDebuff = false;
        public bool PleasureTail = false;

        public int LoveAndHateArcanaCost = 0;
        public int LoveAndHateVillain = -1;

        public int MagicBulletNthShot = 0;
        public int MagicBulletRequest = -1;
        public bool MagicBulletDarkFlame = false;

        public bool SolemnSwitch = false;
        public int SolemnLamentDisableDamage = 0;
        public int SolemnLamentDisable = 0;
        public int SolemnLamentCooldown = 0;
        public float SolemnLamentFireRate = 0;

        public override void ResetEffects()
        {
            BlackSwanParryChance = 0;
            BlackSwanBrokenDream = false;

            PleasureDebuff = false;
            PleasureTail = false;

            if (MagicBulletRequest >= 0 && Player.HeldItem.type != ModContent.ItemType<Items.Ruina.Technology.MagicBulletR>())
                MagicBulletRequest = -1;
            MagicBulletDarkFlame = false;
        }

        public override void UpdateDead()
        {
            BlackSwanNettleClothing = 0;

            FaintAromaPetal = 0;

            SolemnLamentDisable = 0;

            LoveAndHateCostReset();
        }

        public override void OnEnterWorld()
        {
            LoveAndHateCostReset();

            if (Player.HasBuff<Buffs.NettleClothing>())
                Player.ClearBuff(ModContent.BuffType<Buffs.NettleClothing>());

            if (Player.HasBuff<PleasureTail>())
                Player.ClearBuff(ModContent.BuffType<PleasureTail>());
        }

        public override void PostUpdateBuffs()
        {
            if (PleasureTail)
            {
                Player.manaRegenBuff = false;
                if (Player.manaRegenBonus > 0)
                    Player.manaRegenBonus = 0;
                Player.manaRegenBonus = -10;
                Player.manaRegenDelayBonus = 0;
                Player.wingAccRunSpeed += 0.15f;
            }
        }

        public override void PostUpdate()
        {
            if (FaintAromaPetal > 0)
            {
                FaintAromaPetal -= FaintAromaDecay;
            }

            if (SolemnLamentFireRate > 0 && Player.itemAnimation <= 0)
            {
                SolemnLamentFireRate -= 0.0001f;
                if (SolemnLamentFireRate < 0)
                    SolemnLamentFireRate = 0;
            }
        }

        public override void UpdateLifeRegen()
        {
            if (FaintAromaPetal > 0 && Player.lifeRegen < 0)
                Player.lifeRegen = 0;
        }

        public override void UpdateBadLifeRegen()
        {
            if (PleasureDebuff)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
                Player.lifeRegen = -5;
            }
        }

        public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
        {
            if (PleasureTail)
                healValue = 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0 && MagicBulletRequest == target.whoAmI)
            {
                Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>().MagicBulletRequest = -1;
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Player.HeldItem.type == ModContent.ItemType<BlackSwan>() && Main.rand.Next(100) < 10)
            {
                Player.ApplyDamageToNPC(npc, hurtInfo.Damage, 0, Player.direction, false);
            }
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            // Pleasure has a bonus chance to dodge Projectiles when no enemies are near or a boss is active, should NOT prevent contact damage
            if (PleasureTail && info.Dodgeable && info.DamageSource.SourceProjectileType > 0 && Main.rand.NextFloat(1f) < PleasureDodgeChance())
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 velocity = new Vector2(4, 0).RotatedBy(6.28f * i / 8f);
                    Gore.NewGore(Player.GetSource_FromThis(), Player.MountedCenter, velocity, 331, Main.rand.NextFloat(0.9f, 1.1f));
                }
                Player.immune = true;
                Player.AddImmuneTime(ImmunityCooldownID.General, 45);
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Art/Porccu_Nodmg") with { Volume = 0.5f }, Player.Center);
                return true;
            }
            return base.FreeDodge(info);
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            bool takeDamage = true;
            //Black Swan Nettle Damage Reduction
            if (BlackSwanNettleClothing > 0)
            {
                BlackSwanNettleRemove(1);
                if (BlackSwanNettleClothing < 0)
                {
                    BlackSwanNettleClothing = 0;
                    if (Player.HasBuff<Buffs.NettleClothing>())
                        Player.ClearBuff(ModContent.BuffType<Buffs.NettleClothing>());
                    Player.AddBuff(ModContent.BuffType<Buffs.BrokenDreams>(), 30 * 60);
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Sis_Trans") with { Volume = 0.25f }, Player.Center);
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        int dustType = Main.rand.Next(2, 4);
                        Vector2 dustVel = new Vector2(3f * (float)Math.Cos(6.28f * (i / 8f)), 3f * (float)Math.Sin(6.28f * (i / 8f)));

                        Dust.NewDustPerfect(Player.MountedCenter, dustType, dustVel, 0, default, 1.5f).noGravity = true;
                    }

                    int reflectDamage = info.Damage;

                    if (Player.HeldItem.type == ModContent.ItemType<Items.Ruina.Literature.BlackSwanR>())
                    {
                        if (BlackSwanNettleClothing >= 3 - 1 || BlackSwanBrokenDream)
                        {
                            reflectDamage = (int)(reflectDamage * 1.2f);
                        }
                        if (BlackSwanNettleClothing >= 6 - 1)
                        {
                            Player.immune = true;
                            Player.immuneTime = 30;
                            takeDamage = false;
                        }
                    }

                    if (info.DamageSource.SourceNPCIndex >= 0)
                    {
                        Player.ApplyDamageToNPC(Main.npc[info.DamageSource.SourceNPCIndex], reflectDamage, 0f, Player.direction, false);
                    }
                    if (!takeDamage)
                        return takeDamage;
                    info.Damage = (int)(info.Damage * 0.25f);
                }
            }

            return base.ConsumableDodge(info);
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (PleasureTail)
            {
                float lifePercent = (float)info.Damage * 2 / Player.statLifeMax2;
                int manaLost = (int)(lifePercent * Player.statManaMax2);
                int max = info.Damage * 2;
                if (manaLost < max)
                {
                    manaLost = max;
                }

                //Main.NewText(manaLost);
                if (!Player.CheckMana(manaLost, true, true))
                {
                    Player.statMana = Player.statManaMax2;
                    int damage = Player.statLifeMax2 / 2;
                    Player.statLife -= damage;
                    CombatText.NewText(Player.getRect(), CombatText.DamagedHostile, damage, true);
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Art/Porccu_Special") with { Volume = 0.5f }, Player.Center);
                }
                else
                {
                    Player.manaRegenDelay = 900;
                }
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.GreenStemArea>()] >= 1)
            {
                modifiers.FinalDamage += 0.3f;
            }

            if (PleasureTail)
            {
                modifiers.FinalDamage *= 0.33f;
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (FaintAromaPetal > 0)
            {
                if (FaintAromaPetal < FaintAromaPetalMax)
                    modifiers.FinalDamage *= (1.1f + ((float)FaintAromaPetal / (float)FaintAromaPetalMax));
                else
                    modifiers.FinalDamage *= 1.2f;
            }
        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (FaintAromaPetal > 0 && Player.HeldItem.type == ModContent.ItemType<Items.Ruina.Art.FaintAromaS>() && Player.itemAnimation > 0 && npc.immune[Player.whoAmI] > 0)
                return false;
            return base.CanBeHitByNPC(npc, ref cooldownSlot);
        }

        /// <summary>
        /// True Nettle amount
        /// </summary>
        /// <param name="player"></param>
        /// <param name="NettleAmount"></param>
        /// <returns></returns>
        public bool IsNettleOver(int NettleAmount, bool BrokenDreams = true)
        {
            if (BrokenDreams)
                return BlackSwanNettleClothing >= NettleAmount || BlackSwanBrokenDream;
            return BlackSwanNettleClothing >= NettleAmount;
        }

        public void BlackSwanNettleAdd(float val)
        {
            int nextVal = (int)Math.Ceiling(BlackSwanNettleClothing);
            float oldVal = BlackSwanNettleClothing;
            BlackSwanNettleClothing += val;
            if (nextVal != 0 && BlackSwanNettleClothing > nextVal && oldVal < nextVal)
            {
                //SpecialEffects with Nettle Clothing
                Vector2 dustPos = Player.MountedCenter + new Vector2(30 + 3 * (float)Math.Sin(MathHelper.ToRadians(5 * (float)Main.timeForVisualEffects)), 0).RotatedBy(MathHelper.ToRadians(60 + 60 * nextVal - 1));
                for (int i = 0; i < 8; i++)
                {
                    int dustType = Main.rand.Next(2, 4);
                    Vector2 dustVel = new Vector2(1f * (float)Math.Cos(6.28f * (i / 8f)), 1f * (float)Math.Sin(6.28f * (i / 8f)));

                    Dust.NewDustPerfect(dustPos, dustType, dustVel).noGravity = true;
                }

                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/BlackSwan_Revive") with { Volume = 0.1f }, Player.Center);
            }
        }

        public void BlackSwanNettleRemove(int val)
        {
            BlackSwanNettleClothing = (int)Math.Floor(BlackSwanNettleClothing - val);
        }

        // Calculates dodge chance for Pleasure Tail, used on FreeDodge()
        private float PleasureDodgeChance()
        {
            // Default Dodge Chance
            float dodge = .2f;

            // If a boss is alive at any point in time, Dodge chance is normal
            if (Main.CurrentFrameFlags.AnyActiveBossNPC)
                return dodge;


            // When an enemy is near, reduce chance to dodge which range from 80% to 20% (20% is impossible since they need to be inside you)
            // 15 Tiles?
            float maxDist = 15 * 16;
            float distance = maxDist;
            foreach (NPC n in Main.npc)
            {
                if (n.active && !n.friendly)
                {
                    float npcDist = n.Center.Distance(Player.Center);
                    if (npcDist < distance)
                    {
                        distance = npcDist;
                    }
                }
            }
            dodge += (0.8f - dodge) * distance / maxDist;

            return dodge;
        }

        public void PleasureManaConsume(int cost)
        {
            bool Mana = Player.CheckMana(Main.LocalPlayer.statManaMax2 / 5, true, true);

            if (!Mana)
            {
                Player.statMana = Player.statManaMax2;
                Player.ManaEffect(Player.statManaMax2);
                Player.statLife -= Player.statLifeMax / 2;
                Player.HealEffect(-Player.statLifeMax / 2);
            }
        }

        /// <summary>
        /// Old Solemn Lament Sealing Mechanic
        /// </summary>
        /// <param name="damage"></param>
        private void OnHitSolemnLament(double damage)
        {
            if (Player.HeldItem.type == ModContent.ItemType<Items.Ruina.Technology.SolemnLamentR>())
            {
                SolemnLamentDisableDamage += (int)damage;
                if (SolemnLamentDisableDamage < Player.statLifeMax2 * 0.25f)
                    return;
                SolemnLamentDisableDamage = 0;
                Player.AddBuff(ModContent.BuffType<Buffs.Lament>(), 300);
                if (SolemnLamentDisable == 0)
                    SolemnLamentDisable = Main.rand.Next(2) + 1;
            }
        }

        public void LoveAndHateCostReduce(float mult = 1f)
        {
            LoveAndHateArcanaCost -= (int)(Player.statManaMax2 * 0.05f * mult);
            if (LoveAndHateArcanaCost < 0)
                LoveAndHateArcanaCost = 0;
        }

        public void LoveAndHateCostReset()
        {
            LoveAndHateArcanaCost = Player.statManaMax2;// + 200;
        }
    }
}
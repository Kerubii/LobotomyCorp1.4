using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Ruina.Language;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Projectiles.Realized;
using LobotomyCorp.Visuals.ParticlesAura;
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

        public bool CrimsonScarRuddedWelts = false;
        public bool CrimsonScarPrey = false;
        public float CrimsonScarVengeanceBoost = 0;
        public bool CrimsonScarHowlingNightmare = false;
        public static float CrimsonScarPreyBoost = 0.2f;

        public float FaintAromaPetal = 0;
        public int FaintAromaPetalMax = 60;
        public float FaintAromaDecay = 0.1f;

        public bool PleasureDebuff = false;
        public bool PleasureTail = false;

        public bool LoveAndHateLove = false;
        public bool LoveAndHateRegenBuff = false;
        public bool LoveAndHateHatred = false;
        public int LoveAndHateHysteria = 0;
        public int LoveAndHateHysteriaDecay = 0;
        public int LoveAndHateArcanaCost = 0;
        public int LoveAndHateVillain = -1;
        public int LoveAndHateArcanaCooldown = 0;

        public int MagicBulletNthShot = 0;
        public int MagicBulletRequest = -1;
        public bool MagicBulletDarkFlame = false;

        public bool SolemnSwitch = false;
        public int SolemnLamentDisableDamage = 0;
        public int SolemnLamentDisable = 0;
        public int SolemnLamentCooldown = 0;
        public float SolemnLamentFireRate = 0;

        public bool SwordSharpenedBlessing = false;
        public int SwordSharpenedBlessingBestower = -1;
        public bool SwordSharpenedJustice = false;
        public bool SwordSharpenedDespair = false;
        public int SwordSharpenedImpaledCount = 0;
        public int[] SwordSharpenedCurrentSword = { -1, -1, -1};
        public Vector3[] SwordSharpenedImpalePosition = new Vector3[30];

        public override void ResetEffects()
        {
            BlackSwanParryChance = 0;
            BlackSwanBrokenDream = false;

            CrimsonScarPrey = false;
            CrimsonScarRuddedWelts = false;
            CrimsonScarHowlingNightmare = false;

            LoveAndHateLove = false;
            LoveAndHateRegenBuff = false;
            LoveAndHateHatred = false;
            if (LoveAndHateArcanaCooldown > 0)
            {
                LoveAndHateArcanaCooldown--;
            }
            if (LoveAndHateVillain > -1)
            {
                if (!Main.npc[LoveAndHateVillain].HasBuff<Villain>())
                    LoveAndHateVillain = -1;
            }

            PleasureDebuff = false;
            PleasureTail = false;

            if (MagicBulletRequest >= 0 && Player.HeldItem.type != ModContent.ItemType<Items.Ruina.Technology.MagicBulletR>())
                MagicBulletRequest = -1;
            MagicBulletDarkFlame = false;

            SwordSharpenedBlessing = false;
            SwordSharpenedJustice = false;
            SwordSharpenedDespair = false;
        }

        public override void UpdateDead()
        {
            BlackSwanNettleClothing = 0;

            FaintAromaPetal = 0;

            SolemnLamentDisable = 0;

            LoveAndHateCostReset();

            SwordSharpenedImpaledReset();
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (SwordSharpenedBlessing)
            {
                SwordSharpenedBlessingBestower = -1;
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<SwordSharpenedWithTearsRDespair>(), 15, 0, SwordSharpenedBlessingBestower, 1);
                }
            }
        }

        public override void OnEnterWorld()
        {
            LoveAndHateCostReset();

            SwordSharpenedImpaledReset();
        }

        public override void PreUpdateBuffs()
        {
            if (SwordSharpenedBlessingBestower > -1)
            {
                Player.AddBuff(ModContent.BuffType<Blessing>(), 60);
            }
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

            LoveAndHateHysteriaUpdate();
            if (LoveAndHateLove)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    foreach (Player ally in Main.player)
                    {
                        if (ally.active && ally.whoAmI != Player.whoAmI && !ally.dead && ally.team == Player.team)
                        {
                            ally.AddBuff(ModContent.BuffType<LoveTeam>(), 5);
                        }
                    }
                }
            }
            if (LoveAndHateArcanaCooldown > 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemAmethyst);
                    Main.dust[d].noGravity = true;
                }
                Player.statDefense -= 10;
                Player.noItems = true;
                Player.cursed = true;
            }

            if (CrimsonScarRuddedWelts && CrimsonScarLowHealthActive)
            {
                AuraBehavior buffAura = new CrimsonScarAura();
                Player.GetModPlayer<LobotomyModPlayer>().CurrentAura.Add(buffAura);
            }

            // Just incase Swords weren't cleared when Justice buff is active
            if (SwordSharpenedJustice && !SwordSharpenedDespair)
            {
                if (SwordSharpenedImpaledCount == 3)
                {
                    SwordSharpenedImpaledReset();
                }
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
            if (LoveAndHateRegenBuff)
            {
                Player.lifeRegen += 10;
            }
            if (FaintAromaPetal > 0 && Player.lifeRegen < 0)
                Player.lifeRegen = 0;
        }

        public override void UpdateBadLifeRegen()
        {
            if (CrimsonScarRuddedWelts && !CrimsonScarHowlingNightmare && Player.statLife >= Player.statLifeMax2 / 2)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
            }

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
            if (CrimsonScarRuddedWelts)
            {
                CrimsonScarVengeanceBoost = 0.1f;
                if (npc.GetGlobalNPC<LobotomyGlobalNPC>().CrimsonScarPrey)
                {
                    CrimsonScarVengeanceBoost = 0.2f;
                }
                Player.AddBuff(ModContent.BuffType<Vengeance>(), 10 * 60);
                npc.AddBuff(ModContent.BuffType<Prey>(), 60 * 15);
            }
            if (Player.HeldItem.type == ModContent.ItemType<BlackSwan>() && Main.rand.Next(100) < 10)
            {
                Player.ApplyDamageToNPC(npc, hurtInfo.Damage, 0, Player.direction, false);
            }
            if (LoveAndHateLove)
            {
                LoveAndHateHysteriaIncrease(hurtInfo.Damage);
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (CrimsonScarRuddedWelts)
            {
                CrimsonScarVengeanceBoost = 0.1f;
                Player.AddBuff(ModContent.BuffType<Vengeance>(), 10 * 60);
            }
            if (LoveAndHateLove)
            {
                LoveAndHateHysteriaIncrease(hurtInfo.Damage);
            }
        }

        public override void PostHurt(Player.HurtInfo info)
        {
            if (CrimsonScarRuddedWelts)
            {
                if (CrimsonScarLowHealthActive && Player.statLife + info.Damage > Player.statLifeMax2 / 2)
                {
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Language/RedHood_Change") with { Volume = 0.2f }, Player.Center);
                }
            }
            if (SwordSharpenedImpaledCount < 3 && info.DamageSource.SourceProjectileType == ModContent.ProjectileType<SwordSharpenedWithTearsRDespair>())
            {
                Player.immune = false;
                Player.immuneTime = 0;
            }
        }

        /*
        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            if (LoveAndHateLove)
            {
                LoveAndHateHysteriaIncrease(-healValue);
            }
        }*/

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
            else if (SwordSharpenedBlessing)
            {
                modifiers.SourceDamage += 0.2f;
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (SwordSharpenedBlessing)
            {
                modifiers.SourceDamage -= 0.1f;
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
        /// <summary>
        /// Change Arcana Slave cost to default
        /// </summary>
        public void LoveAndHateCostReset()
        {
            LoveAndHateArcanaCost = Player.statManaMax2 + 200;
        }
        public float LoveAndHateHysteriaPercent()
        {
            return Math.Min(LoveAndHateHysteria / (Player.statLifeMax2 / 2f), 1f);
        }
        /// <summary>
        /// Increase or decrease Hysteria, If it goes over the limit, Hate is automatically applied to the Player
        /// </summary>
        /// <param name="amount"></param>
        public void LoveAndHateHysteriaIncrease(int amount)
        {
            if (LoveAndHateHatred)
                return;

            LoveAndHateHysteria += amount;
            if (LoveAndHateHysteria > Player.statLifeMax2 / 2)
            {
                LoveAndHateHysteria = Player.statLifeMax2 / 2;
                Player.AddBuff(ModContent.BuffType<Hatred>(), 10);
                LoveAndHateCostReset();
            }
            if (LoveAndHateHysteria < 0)
                LoveAndHateHysteria = 0;
            LoveAndHateHysteriaDecay = 60;
        }
        /// <summary>
        /// Reset Hysteria back to 0, used after firing Arcana Slave under Hatred
        /// </summary>
        public void LoveAndHateHysteriaReset()
        {
            LoveAndHateHysteria = 0;
            LoveAndHateHysteriaDecay = 0;
        }
        private void LoveAndHateHysteriaUpdate()
        {
            if (LoveAndHateHysteria <= 0)
                return;

            if (LoveAndHateHysteriaDecay > 0)
                LoveAndHateHysteriaDecay--;
            else
            {
                LoveAndHateHysteria--;
                LoveAndHateHysteriaDecay = 60;
            }
        }

        public static bool SwordSharpenedIsType(int type)
        {
            return type == ModContent.ProjectileType<SwordSharpenedWithTearsRSword>() ||
                   type == ModContent.ProjectileType<SwordSharpenedWithTearsRSwordExtra>();
        }

        public static int SwordSharpenedTotalOwned(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<SwordSharpenedWithTearsRSword>()] +
                   player.ownedProjectileCounts[ModContent.ProjectileType<SwordSharpenedWithTearsRSwordExtra>()];
        }

        public void SwordSharpenedImpaledReset()
        {
            SwordSharpenedImpaledCount = 0;
            SwordSharpenedImpalePosition = new Vector3[3];
        }

        public void SwordSharpenedImpaledBy(Projectile proj, int damage)
        {
            if (damage > Player.statLife)
                damage = Player.statLife - 1;
            if (damage > 0)
            {
                Player.Hurt(new PlayerDeathReason(), damage, 0, false, false, -1, false, 0, 0, 0);
            }

            if (SwordSharpenedImpaledCount < SwordSharpenedImpalePosition.Count())
            {
                Vector2 delta = (proj.Center + proj.velocity * 5f) - Player.Center;
                float rotation = proj.rotation;
                // Mirrors it if player is facing opposite
                if (Player.direction == -1)
                    rotation = 3.14f - rotation;
                SwordSharpenedImpalePosition[SwordSharpenedImpaledCount] = new Vector3(delta.X * Player.direction, delta.Y, MathHelper.WrapAngle(rotation));
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    LobotomyCorp.NetworkSharpenedVisual(SwordSharpenedImpalePosition[SwordSharpenedImpaledCount], Main.myPlayer);
                SwordSharpenedImpaledCount++;
                if (SwordSharpenedImpaledCount >= SwordSharpenedImpalePosition.Count())
                {
                    if (SwordSharpenedJustice)
                        Player.ClearBuff(ModContent.BuffType<Justice>());
                    Player.AddBuff(ModContent.BuffType<Despair>(), 2);
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/KnightOfDespair_Groggy") with { Volume = 0.2f}, Player.Center);
                }
            }
        }

        /// <summary>
        /// Sets CurrentSword to the next available sword, if a sword is not available sets it to -1
        /// </summary>
        /// <returns></returns>
        public bool SwordSharpenedFindNextValidSword()
        {
            SwordSharpenedCurrentSword[0] = SwordSharpenedCurrentSword[1];
            if (SwordSharpenedCurrentSword[0] == -1)
            {
                SwordSharpenedCurrentSword[0] = SwordSharpenedCurrentSword[2];
                SwordSharpenedCurrentSword[1] = -1;
            }
            else
            {
                SwordSharpenedCurrentSword[1] = SwordSharpenedCurrentSword[2];
            }
            SwordSharpenedCurrentSword[2] = -1;
            for (int i = 0; i < 3; i++)
            {
                int order = -1;
                if (SwordSharpenedCurrentSword[i] >= 0)
                    continue;
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (SwordSharpenedIsType(p.type) && p.owner == Player.whoAmI)
                    {
                        order++;
                        if (!SwordSharpenedCurrentSword.Contains(order) && p.ai[1] <= 0)
                        {
                            SwordSharpenedCurrentSword[i] = order;
                            //Main.NewText("Added " + order + " to " + i);
                            break;
                        }
                    }
                }
            }
            return false;
        }

        private void SSListScript()
        {
            string test = "Current Sword List:";
            for (int i = 0; i < 3; i++)
            {
                test += " " + SwordSharpenedCurrentSword[i];
            }
            Main.NewText(test);
        }

        /// <summary>
        /// Add sword's order to the queue, if the queue is full returns false
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool SwordSharpenedAddSwordToQueue(int order)
        {
            for (int i = 0; i < 3; i++)
            {
                if (SwordSharpenedCurrentSword[i] == order)
                    return false;
                if (SwordSharpenedCurrentSword[i] == -1)
                {
                    SwordSharpenedCurrentSword[i] = order;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Finds all available swords, sets it to nothing if no sword is available
        /// </summary>
        public void SwordSharpenedResetCurrentSwords()
        {
            int order = -1;
            int i = 0;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (SwordSharpenedIsType(p.type) && p.owner == Player.whoAmI)
                {
                    order++;
                    if (p.ai[1] <= 0)
                    {
                        SwordSharpenedCurrentSword[i] = order;
                        i++;
                        if (i > 2)
                            return;
                    }
                }
            }
            for (int j = i; j < 3; j++)
            {
                SwordSharpenedCurrentSword[j] = -1;
            }
        }

        public void SwordSharpenedApplyBlessing()
        {
            int applyTo = -1;
            float dist = 16 * 2;
            foreach (Player ally in Main.ActivePlayers)
            {
                if (ally.whoAmI != Player.whoAmI && ally.team == Player.team)
                {
                    if (ally.GetModPlayer<LobotomyWawPlayer>().SwordSharpenedBlessingBestower == Player.whoAmI)
                        return;
                    float pDist = ally.Center.Distance(Main.MouseWorld);
                    if (pDist < dist)
                    {
                        dist = pDist;
                        applyTo = ally.whoAmI;
                    }
                }
            }
            if (applyTo > -1)
            {
                Main.player[applyTo].GetModPlayer<LobotomyWawPlayer>().SwordSharpenedBlessingBestower = Player.whoAmI;
                LobotomyCorp.NetworkBlessingSync(Player.whoAmI, applyTo);
            }
        }

        /*
        public void SwordSharpenedApplyBlessing()
        {
            if (SwordSharpenedBlessingBestowed > -1)
            {
                Player blesee = Main.player[SwordSharpenedBlessingBestowed];
                if (!blesee.active || blesee.dead || !blesee.GetModPlayer<LobotomyWawPlayer>().SwordSharpenedBlessing)
                {
                    SwordSharpenedBlessingBestowed = -1;
                }
            }
            if (SwordSharpenedBlessingBestowed == -1)
            {
                float dist = 16 * 2;
                foreach (Player ally in Main.ActivePlayers)
                {
                    if (ally.team == Player.team)
                    {
                        float pDist = ally.Center.Distance(Main.MouseWorld);
                        if (pDist < dist)
                        {
                            dist = pDist;
                            SwordSharpenedBlessingBestowed = ally.whoAmI;
                        }
                    }
                }
                if (SwordSharpenedBlessingBestowed > -1)
                {
                    //int type = ModContent.BuffType<Blessing>();
                    //Main.player[SwordSharpenedBlessingBestowed].AddBuff(type, 60, false);
                    LobotomyCorp.NetworkBlessingSync(Player.whoAmI, SwordSharpenedBlessingBestowed);
                }
            }
        }*/

        public bool CrimsonScarLowHealthActive => Player.statLife <= Player.statLifeMax2 / 2;
    }
}
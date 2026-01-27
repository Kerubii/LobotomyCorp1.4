using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Aleph;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.ModSystems;
using LobotomyCorp.NPCs.RedMist;
using LobotomyCorp.PlayerDrawEffects;
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
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace LobotomyCorp.Players
{
    /// <summary>
    /// Used for HE ego effects
    /// </summary>
    public class LobotomyHePlayer : ModPlayer
    {
        /// <summary>
        /// Use 1 to empower Melee, Use 2 to empower Range
        /// </summary>
        public int CrimsonScarEmpower = 0;

        public int HarmonyTime = 0;
        public bool HarmonyAddiction = false;
        public bool HarmonyConnected = false;

        public int ForgottenSoundCooldown = 0;
        public int ForgottenAffection = -1;
        public float ForgottenAffectionResistance = 0;

        public bool GrinderMk2Active = false;
        public int GrinderMk2Dash = 0;
        public int GrinderMk2BatteryMax = 1400;
        public int GrinderMk2Battery = 1400;
        public bool GrinderMk2Recharging = false;
        public int GrinderMk2AttackCooldown = 0;
        public int GrinderMk2AttackRandom = 0;

        public int LifeForADareDevilGift = 0;
        public bool LifeForADareDevilGiftActive = false;
        public bool LifeForADareDevilCounterStance = false;
        public int LifeForADareDevilCounterArea = 0;

        public bool OurGalaxyStone = false;
        public int OurGalaxyOwner = -1;

        public override void ResetEffects()
        {
            HarmonyAddiction = false;
            HarmonyConnected = false;

            GrinderMk2Active = false;

            LifeForADareDevilGiftActive = false;

            if (!OurGalaxyStone)
                OurGalaxyOwner = -1;
            OurGalaxyStone = false;
        }

        public override void OnRespawn()
        {
            ForgottenAffection = -1;
            ForgottenAffectionResistance = 0;
        }

        public override void UpdateDead()
        {
            ForgottenAffection = -1;
            ForgottenAffectionResistance = 0;

            HarmonyTime = 0;
            HarmonyAddiction = false;

            GrinderMk2Recharging = false;

            LifeForADareDevilGift = 0;
            LifeForADareDevilCounterStance = false;
        }

        public override void PreUpdate()
        {
            // Grinder Battery
            if (GrinderMk2Recharging)
            {
                if (GrinderMk2Battery < 0)
                    GrinderMk2Battery = 0;
                GrinderMk2Battery += 8;
                if (GrinderMk2Battery > GrinderMk2BatteryMax)
                {
                    GrinderMk2Battery = GrinderMk2BatteryMax;
                    GrinderMk2Recharging = !GrinderMk2Recharging;
                }

                if (Main.rand.Next(8) == 0)
                {
                    Projectiles.Realized.GrinderMk2Cleaner2.SpawnTrailDust(Player.position, Player.width, Player.height, ModContent.DustType<Misc.Dusts.ElectricTrail>(), new Vector2(Main.rand.Next(-2, 3), -1f), 0.5f, 5);
                }
            }
        }

        public override void PostUpdate()
        {
            if (ForgottenAffection >= 0)
            {
                if (Player.HeldItem.type != ModContent.ItemType<Items.Ruina.History.ForgottenR>() || (Main.npc[ForgottenAffection].Center - Player.Center).Length() > 960)
                {
                    ForgottenAffectionResistance -= 0.01f;
                }
                if (ForgottenAffectionResistance <= 0 || (!Main.npc[ForgottenAffection].active || Main.npc[ForgottenAffection].life <= 0))
                {
                    ForgottenAffectionResistance = 0;
                    ForgottenAffection = -1;
                }
            }

            if (HarmonyTime > 0)
                HarmonyTime--;
            if (HarmonyAddiction && !HarmonyConnected)
            {
                Player.statDefense -= 15;
            }

            GrinderMk2Active = false;
            if (GrinderMk2AttackCooldown > 0)
                GrinderMk2AttackCooldown--;

            if (LifeForADareDevilGift > 0)
            {
                LifeForADareDevilGift--;
            }
        }

        public override void UpdateLifeRegen()
        {
            if (OurGalaxyStone && !(Player.HasBuff(BuffID.PotionSickness) || Player.potionDelay > 0))
            {
                int value = 5 + 5 * OurGalaxyStoneAllies();
                if (value > 30)
                    value = 30;
                Player.lifeRegen += value;
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if (HarmonyAddiction && !HarmonyConnected)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
                Player.lifeRegen = -12;
            }
        }

        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (Player.HeldItem.type == ModContent.ItemType<Items.Ruina.LifeForADaredevilR>() && LifeForADareDevilCounterStance)
            {
                if (Player.channel)
                {
                    LifeForADareDevilCounterStance = false;
                    Player.channel = false;
                    /*
                    foreach (Projectile p in Main.projectile)
                    {
                        if (p.type == ModContent.ProjectileType<Projectiles.Realized.LifeForADaredevilR>() && p.owner == Player.whoAmI)
                        {
                            p.Kill();
                            break;
                        }
                    }*/
                }
                else
                {
                    Player.itemAnimation = Player.itemAnimationMax / 2;
                    Player.itemTime = Player.itemAnimationMax / 2;
                    LifeForADaredevilCounterAttack(Player.HeldItem.damage);
                    LifeForADareDevilGift = 1800;
                    Player.AddBuff(ModContent.BuffType<Buffs.InspiredBravery>(), 10);
                    return false;
                }
            }

            return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if(Player.HeldItem.type == ModContent.ItemType<Items.Ruina.History.ForgottenR>() && modifiers.DamageSource.SourceNPCIndex == ForgottenAffection)
            {
                modifiers.FinalDamage -= ForgottenAffectionResistance;
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Teddy_Guard") with { Volume = 0.5f }, Player.Center);
                modifiers.DisableSound();
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (OurGalaxyStone)
            {
                foreach (Player p in Main.ActivePlayers)
                {
                    if (p.whoAmI != Player.whoAmI && p.team == Player.team && !p.dead)
                    {
                        LobotomyHePlayer modPlayer = p.GetModPlayer<LobotomyHePlayer>();
                        if (modPlayer.OurGalaxyStone && modPlayer.OurGalaxyOwner == OurGalaxyOwner)
                        {
                            NetworkText text = NetworkText.FromKey("Mods.LobotomyCorp.DeathMessages.Stone", p.name);
                            PlayerDeathReason playerDeath = PlayerDeathReason.ByCustomReason(text);
                            p.Hurt(playerDeath, p.statLifeMax2 / 4, 0, dodgeable: false, scalingArmorPenetration: 1);
                            //p.KillMe(damageSource, p.statLifeMax2 * 4, 1);
                        }
                    }
                }
            }
        }

        public override void SetControls()
        {
            if (GrinderMk2Recharging)
            {
                Player.controlMount = false;
                Player.controlHook = false;
                Player.controlUseItem = false;
                Player.controlRight = false;
                Player.controlJump = false;
                Player.controlDown = false;
                Player.controlLeft = false;
                Player.controlRight = false;
                Player.controlUp = false;
            }
            if (GrinderMk2Active && GrinderMk2Dash > 0 &&
                Player.HeldItem.type != ModContent.ItemType<Items.Ruina.Technology.GrinderMk52R>())
            {
                Player.controlUseItem = false;
            }
        }

        // Old Dash
        private void DashMovement()
        {
            if (GrinderMk2Active)
            {
                float grinderSpeed = 16f;

                if (GrinderMk2Dash > 0)
                {
                    int dir = Math.Sign(Player.velocity.X);
                    Player.dashDelay = 20;

                    Player.velocity.X = grinderSpeed * dir;

                    int Distance = 54;
                    if (Collision.SolidTiles(Player.position + Vector2.UnitY * Player.height, Player.width, Distance + 8, true))
                    {
                        Player.gravity = 0;
                        Player.velocity.Y = 0.00001f;
                        if (Collision.SolidTiles(Player.position + Vector2.UnitY * Player.height, Player.width, Distance, true))
                        {
                            Player.velocity.Y = -4f;
                        }
                    }
                }
                if (Player.dashDelay > 0)
                {
                    Player.velocity.X *= 0.98f;
                    GrinderMk2Dash--;
                }
                else if (!Player.mount.Active)
                {
                    int dir = 0;
                    bool dashing = false;
                    if (Player.dashTime > 0)
                    {
                        Player.dashTime--;
                    }
                    if (Player.dashTime < 0)
                    {
                        Player.dashTime++;
                    }
                    if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[2] < 15)
                    {
                        if (Player.dashTime > 0)
                        {
                            dir = 1;
                            dashing = true;
                            Player.dashTime = 0;
                            Player.timeSinceLastDashStarted = 0;
                        }
                        else
                        {
                            Player.dashTime = 15;
                        }
                    }
                    else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[3] < 15)
                    {
                        if (Player.dashTime < 0)
                        {
                            dir = -1;
                            dashing = true;
                            Player.dashTime = 0;
                            Player.timeSinceLastDashStarted = 0;
                        }
                        else
                        {
                            Player.dashTime = -15;
                        }
                    }

                    if (dashing)
                    {
                        Player.velocity.X = grinderSpeed * dir;
                        Point point = (Player.Center + new Vector2((float)(dir * Player.width / 2 + 2), Player.gravDir * (float)(-Player.height) / 2f + Player.gravDir * 2f)).ToTileCoordinates();
                        Point point2 = (Player.Center + new Vector2((float)(dir * Player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point.X, point.Y) || WorldGen.SolidOrSlopedTile(point2.X, point2.Y))
                        {
                            Player.velocity.X /= 2f;
                        }
                        Player.dashDelay = -1;
                        Player.direction = dir;
                        GrinderMk2Dash = 40;

                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Helper_Atk") with { Volume = 0.25f }, Player.Center);
                    }
                }
            }
            else
                GrinderMk2Dash = 0;
        }

        private int OurGalaxyStoneAllies()
        {
            int count = 0;
            foreach (Player p in Main.ActivePlayers)
            {
                if (p.whoAmI != Player.whoAmI && p.team == Player.team && !p.dead)
                {
                    LobotomyHePlayer modPlayer = p.GetModPlayer<LobotomyHePlayer>();
                    if (modPlayer.OurGalaxyStone && modPlayer.OurGalaxyOwner == OurGalaxyOwner)
                        count++;
                }
            }

            return count;
        }

        public void LifeForADaredevilCounterAttack(int damage)
        {
            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Armor_HeadOff"), Player.position);

            int Distance = 16 * 20;//Radius
            //Additional Damage
            int diff = Player.statLifeMax2 - Player.statLife;
            damage += (int)Player.GetDamage(DamageClass.Melee).ApplyTo(diff);
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.active && p.hostile && !p.netImportant && p.damage > 0 && p.type != ProjectileID.DrManFlyFlask)
                {
                    if (p.Center.Distance(Player.Center) < Distance)
                    {
                        LifeForADareDevilPierceEffect(p.Center, p.width, p.height);
                        p.Kill();
                    }
                }
            }

            Dictionary<int, int> SegmentList = new Dictionary<int, int>();
            float dmgMult = 1f;
            bool NearNPC = false;

            if (LifeForADareDevilGift < 1200)
                dmgMult += .15f;
            else
                dmgMult += 0.1f;
            foreach (NPC n in Main.npc)
            {
                if (n.active && !n.friendly)
                {
                    float npcDist = Distance * (n.boss ? 2 : 1);
                    bool segmentLimit = false;
                    if (n.realLife > -1 && SegmentList.ContainsKey(n.realLife) && SegmentList[n.realLife] > 5)
                    {
                        segmentLimit = true;
                    }
                    float distance = n.Center.Distance(Player.Center);
                    float extra = +(n.width > n.height ? n.width / 2 : n.height / 2);
                    if (distance < npcDist + extra && !segmentLimit)
                    {
                        if (distance < 30 + (n.width > n.height ? n.width / 2 : n.height / 2))
                            NearNPC = true;
                        int x = Math.Sign(Player.position.X - n.position.X) * -1;
                        LifeForADareDevilPierceEffect(n.Center, n.width, n.height);
                        int hitDamage = (int)(damage * dmgMult);
                        Player.ApplyDamageToNPC(n, hitDamage, 8f, x, true);
                        n.immune[Player.whoAmI] = 15;
                        if (LifeForADareDevilGift <= 1200)
                        {
                            int slashes = 2;
                            if (LifeForADareDevilGift <= 600)
                                slashes += 2;
                            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.position, Vector2.Zero, ModContent.ProjectileType<Projectiles.Realized.LifeForADareDevilEffectsSlashes>(), hitDamage, 0, Player.whoAmI, 60, slashes, n.whoAmI);
                        }
                        if (n.realLife > -1)
                        {
                            if (SegmentList.ContainsKey(n.realLife))
                            {
                                SegmentList[n.realLife]++;
                            }
                            else
                            {
                                SegmentList.Add(n.realLife, 1);
                            }
                        }
                    }
                }
            }

            float scale = 1f + 1.5f * (1f - Player.statLife / (float)Player.statLifeMax2);
            if (Main.myPlayer == Player.whoAmI)
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Realized.LifeForADareDevilEffects>(), 0, 0, Player.whoAmI, scale, Player.direction);

            Player.immune = true;
            Player.immuneTime = 45;
            if (NearNPC)
                Player.immuneTime += 45;
            //Player.immuneNoBlink = true;
            LifeForADareDevilCounterStance = false;
        }

        static public List<int> LifeForADareDevilProjectileBlacklist = new List<int>();

        public void LifeForADareDevilPierceEffect(Vector2 pos, int width, int height)
        {
            if (height > width)
                width = height;

            if (width < 16)
                width = 16;

            float scale = 1f + 1.5f * (1f - Player.statLife / (float)Player.statLifeMax2);

            if (Main.myPlayer == Player.whoAmI)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), pos, Vector2.Zero, ModContent.ProjectileType<Projectiles.Realized.LifeForADareDevilEffects>(), 0, 0, Player.whoAmI, -Main.rand.NextFloat(6.28f), width * scale);
            }
        }
    }
}
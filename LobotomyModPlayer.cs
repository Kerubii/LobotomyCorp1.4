using System;
using System.Collections.Generic;
using System.Linq;
using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Aleph;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.ModSystems;
using LobotomyCorp.NPCs.RedMist;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Visuals.ParticlesAura;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace LobotomyCorp
{
    public class LobotomyModPlayer : ModPlayer
    {
        public int SynchronizedEGO = -1;
        public bool Desync = false;

        public int AttackComboOrder = 0;
        public int AttackComboOrderCooldown = 0;

        public int HeavyWeaponHelper = 0;
        public float ChargeWeaponHelper = 0;

        public bool RedShield = false;
        public bool WhiteShield = false;
        public bool BlackShield = false;
        public bool PaleShield = false;
        public bool CooldownShield = false;
        public int ShieldReapplyCooldown = 0;
        public int ShieldHP = 0;
        public int ShieldHPMax = 0;
        public int ShieldAnim = 0;

        public int statSanity = 100;
        public int statSanityMax = 100;

        public int statFortitude = 25;
        public int statPrudence = 25;
        public int statTemperance = 25;
        public int statJustice = 25;

        public bool RedMistMask = false;

        public bool Hopeless = false;

        public int LuminousGreed = 0;

        private bool forcePlayerVelocity = false;
        private Vector2 forcePlayerVelocityValue;

        //Aura Vanity
        public List<AuraBehavior> CurrentAura;
        public AuraParticle[] PlayerParticles = new AuraParticle[100];

        public static LobotomyModPlayer ModPlayer(Player Player)
        {
            return Player.GetModPlayer<LobotomyModPlayer>();
        }

        public override void ResetEffects()
        {
            Desync = false;

            if (HeavyWeaponHelper > 0)
                HeavyWeaponHelper--;

            if (Player.itemAnimation == 0)
                ChargeWeaponHelper = 0;

            ResetAttackCombo();

            RedShield = false;
            WhiteShield = false;
            BlackShield = false;
            PaleShield = false;
            CooldownShield = false;

            RedMistMask = false;

            Hopeless = false;

            statSanityMax = 17 + statPrudence;

            //RemoveMaxFallSpeed = false;

            CurrentAura = new List<AuraBehavior>();
        }

        private void ResetAttackCombo()
        {
            if (AttackComboOrderCooldown <= 0)
            {
                AttackComboOrder = 0; return;
            }
            AttackComboOrderCooldown--;
        }

        public override void OnRespawn()
        {
        }

        public override void UpdateDead()
        {
            SynchronizedEGO = -1;
            Desync = false;

            
            LuminousGreed = 0;

            for(int i = 0; i < PlayerParticles.Length; i++)
            {
                if (PlayerParticles[i] != null && PlayerParticles[i].Active)
                    PlayerParticles[i].Active = false;
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<RedMist>()))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    LobEventFlags.killedByRedMist = true;
                else
                    LobEventFlags.BinahEntitySendPacket(LobEventFlags.FlagIDs.KilledByRedMist, true);
            }
        }

        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            if (!mediumCoreDeath)
            {
                return new[]
                {
                    new Item(ModContent.ItemType<Items.ItemTiles.BlackBox>()),
                    new Item(ModContent.ItemType<Items.Zayin.Penitence>())
                };
            }
            return Enumerable.Empty<Item>();
        }

        public override void PreUpdate()
        {
            if (ShieldActive)
            {
                ShieldAnim--;
                /*Main.NewText(ShieldHP);
                Main.NewText(ShieldAnim);*/
                if (ShieldHP <= ShieldHPMax / 2)
                {
                    if (ShieldAnim > 120)
                    {
                        ShieldAnim = 60;
                        //Dust Particles when Shield breaks a bit here
                    }
                    if (ShieldAnim <= 0)
                        ShieldAnim = 60;
                }
                else
                {
                    if (ShieldAnim <= 60)
                        ShieldAnim = 120;
                }
            }
        }

        public override void PreUpdateMovement()
        {
            if (forcePlayerVelocity)
            {
                Player.velocity = forcePlayerVelocityValue;
                forcePlayerVelocity = false;
            }
        }

        public override void SetControls()
        {
            if (Hopeless)
            {
                if (Player.velocity.Length() > 0)
                    Player.controlUseItem = false;

                if (Player.itemAnimation > 0)
                {
                    Player.controlJump = false;
                    Player.controlDown = false;
                    Player.controlLeft = false;
                    Player.controlRight = false;
                    Player.controlUp = false;
                }
            }
        }

        public override void PostUpdate()
        {
            if (statSanity > statSanityMax)
                statSanity = statSanityMax;
        }

        public override void PostUpdateBuffs()
        {
        }

        public override void PostUpdateMiscEffects()
        {
            //Aura Effects
            if (CurrentAura.Count > 0)
            {
                foreach (AuraBehavior aura in CurrentAura)
                {
                    if (aura.SpawnCond)
                        LobotomyPlayerParticle.GenerateAuraParticle(this, aura);
                }
            }
            for (int i = 0; i < PlayerParticles.Length; i++)
            {
                if (PlayerParticles[i] != null && PlayerParticles[i].Active)
                {
                    PlayerParticles[i].Update(Player, Player.direction, Player.gravDir, (float)Main.timeForVisualEffects);
                }
            }
        }

        public void ForcePlayerVelocity(Vector2 vel)
        {
            forcePlayerVelocity = true;
            forcePlayerVelocityValue = vel;
        }


        public override bool PreItemCheck()
        {
            if (SynchronizedEGO >= 0 && Player.HeldItem.type != SynchronizedEGO)
            {
                SynchronizedEGO = -1;
            }
            return base.PreItemCheck();
        }


        public override void OnHurt(Player.HurtInfo info)
        {
            if (info.DamageSource.SourceNPCIndex >= 0)
            {
                if (RedShield || BlackShield && !Player.immune)
                {
                    info.Damage = ShieldDamage(info.Damage);
                }
            }

            if (info.DamageSource.SourceProjectileLocalIndex >= 0)
            {
                if (WhiteShield || BlackShield && !Player.immune)
                {
                    info.Damage = ShieldDamage(info.Damage);
                }
            }
        }

        /*
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (damageSource.SourceCustomReason != null)
                return;

            if (damageSource.SourceProjectileType == ModContent.ProjectileType<Projectiles.DespairSword>())
            {
                if (Main.rand.Next(2) == 0)
                    damageSource.SourceCustomReason = Player.name + " fell into Despair.";
                else
                    damageSource.SourceCustomReason = Player.name + " was stricken with Grief.";
            }
            if (damageSource.SourceProjectileType == ModContent.ProjectileType<Projectiles.FourthMatchFlameExplosion>())
            {
                damageSource.SourceCustomReason = Player.name + " was reduced to ashes...";
            }
            if (WingbeatGluttony && damageSource.SourceOtherIndex == 8)
            {
                damageSource.SourceCustomReason = Player.name + " was starved of its prey";
            }
            if (LuminousGreed > 1200 && damageSource.SourceOtherIndex == 8)
            {
                damageSource.SourceCustomReason = Player.name + "'s body turned into lumps of overgrown meat";
            }
            if (Main.npc[damageSource.SourceNPCIndex].type == ModContent.NPCType<NPCs.RedMist.RedMist>() ||
                damageSource.SourceProjectileType == ModContent.ProjectileType<NPCs.RedMist.RedMistMelee>() ||
                damageSource.SourceProjectileType == ModContent.ProjectileType<NPCs.RedMist.RedMistMimicry>() ||
                damageSource.SourceProjectileType == ModContent.ProjectileType<NPCs.RedMist.RedMistDaCapo>() ||
                damageSource.SourceProjectileType == ModContent.ProjectileType<NPCs.RedMist.DaCapoThrow>() ||
                damageSource.SourceProjectileType == ModContent.ProjectileType<NPCs.RedMist.DaCapoLegato>() ||
                damageSource.SourceProjectileType == ModContent.ProjectileType<NPCs.RedMist.SwitchMimicryThrow>() ||
                damageSource.SourceProjectileType == ModContent.ProjectileType<NPCs.RedMist.SwitchDaCapoThrow>() ||
                damageSource.SourceProjectileType == ModContent.ProjectileType<NPCs.RedMist.HeavenBoss>() ||
                damageSource.SourceProjectileType == ModContent.ProjectileType<NPCs.RedMist.JustitiaSlashBoss>())
            {
                damageSource.SourceCustomReason = Player.name + "'s body was reduced to a red mist";
            }
            //could not finish their performance
            //"Goodbye"
            //'s head bursted from pleasure
            //tried to forcefully escape their established role
            //'s malice overtook their body
            //'s role as a villain has ended
            //thirst went unquenched
            //was judged to be sinful
            //disrespected the fairy's care
            //'s unbearable loneliness crushed them
            //averted their gaze 
            
            
            //was overthrown
            //was consumed by Malice
        
            //could not satiate their addiction
            //'s soul has fallen to hell

            //could not protect their beloved family
            base.Kill(damage, hitDirection, pvp, damageSource);
        }*/


        public static float Lerp(float x, float x2, float progress, bool reverse = false)
        {
            if (progress < 0)
                progress = 0;
            if (progress > 1f)
                progress = 1f;
            if (reverse)
                return x2 * (1 - progress) + x * progress;
            else
                return x * (1 - progress) + x2 * progress;
        }

        #region SHIELD STUFF
        /// <summary>
        /// Apply the RWBP shields, use the buff IDs.
        /// </summary>
        public void ApplyShield(int type, int time, int shieldHP, bool forceApply = false)
        {
            if (!forceApply && ShieldActive)
                return;

            ShieldReset(ShieldActive, forceApply);
            Player.AddBuff(type, time);
            ShieldHP = shieldHP;
            ShieldHPMax = ShieldHP;
            ShieldAnim = 120;

            int dustType = 62;
            if (type == ModContent.BuffType<Buffs.ShieldR>())
                dustType = 60;
            else if (type == ModContent.BuffType<Buffs.ShieldW>())
                dustType = 63;
            else if (type == ModContent.BuffType<Buffs.ShieldP>())
                dustType = 59;
            for (int i = 0; i < 10; i++)
            {
                Dust dust;
                // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                Vector2 position = Player.RotatedRelativePoint(Player.MountedCenter, true) - new Vector2(33, 33);
                dust = Main.dust[Terraria.Dust.NewDust(position, 66, 66, dustType, 0f, 0f, 0, new Color(255, 255, 255), 1f)];
                dust.noGravity = true;
                dust.fadeIn = 1.2f;
            }
        }

        /// <summary>
		/// Use "R" "W" "B" "P" for type for this one, only works if buff name is "Shield" + Color.
		/// </summary>
        public void ApplyShield(string type, int time, int shieldHP, bool forceApply = false)
        {
            ApplyShield(Mod.Find<ModBuff>("Shield" + type).Type, time, shieldHP, forceApply);
        }

        public bool ShieldActive => RedShield || WhiteShield || BlackShield || PaleShield || CooldownShield;

        /// <summary>
		/// Manually Reset Shields, True for break for shield breaking particles. 
		/// </summary>
        public void ShieldReset(bool broke = false, bool forceApply = false)
        {
            ShieldHP = 0;
            ShieldHPMax = 0;
            ShieldAnim = 0;
            int remainingTime = 0;
            for (int i = 0; i < 4 + forceApply.ToInt(); i++)
            {
                string letter = "R";
                switch (i)
                {
                    case 0:
                        letter = "R";
                        break;
                    case 1:
                        letter = "W";
                        break;
                    case 2:
                        letter = "B";
                        break;
                    case 3:
                        letter = "P";
                        break;
                    default:
                        letter = "Cooldown";
                        break;
                }
                int ModBuff = Mod.Find<ModBuff>("Shield" + letter).Type;
                if (Player.HasBuff(ModBuff))
                {
                    if (letter != "Cooldown")
                        remainingTime += Player.buffTime[Player.FindBuffIndex(ModBuff)];
                    Player.buffTime[Player.FindBuffIndex(ModBuff)] = 0;
                }
            }

            if (remainingTime > 0 && !forceApply)
                Player.AddBuff(ModContent.BuffType<Buffs.ShieldCooldown>(), remainingTime);

            //if (broke) //Dust Particles when breaking
        }

        /// <summary>
		/// Returns leftovers
		/// </summary>
        public int ShieldDamage(int damage)
        {
            Player.immune = true;
            Player.immuneTime = 40;
            int damageAbsorb = Main.DamageVar(damage);
            ShieldHP -= damageAbsorb;
            if (ShieldHP <= 0)
            {
                int leftover = ShieldHP * -1;
                ShieldReset(true);
                return leftover;
            }
            return 0;
        }

        public int shakeTimer = 0;
        public int shakeIntensity = 0;

        public override void ModifyScreenPosition()
        {
            if (shakeTimer > 0)
            {
                shakeTimer--;

                Main.screenPosition += new Vector2(shakeIntensity * Main.rand.NextFloat(), shakeIntensity * Main.rand.NextFloat());

                if (shakeTimer % 5 == 0 && shakeIntensity > 1)
                    shakeIntensity--;
            }
            else
                shakeIntensity = 0;
        }

        #endregion
    }
}
using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Aleph;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.ModSystems;
using LobotomyCorp.NPCs.RedMist;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace LobotomyCorp.Players
{
    /// <summary>
    /// Used for TETH ego effects
    /// </summary>
    public class LobotomyTethPlayer : ModPlayer
    {
        public int BeakParry = 0;
        public int BeakPunish = 0;
        public float BeakGunCounter = 0;

        public bool FourthMatchFlameGift = false;
        public int FourthMatchFlameR = 0;
        public bool MatchstickBurn = false;
        public int MatchstickBurnTime = 0;

        public int RegretShockwave = 0;
        public bool RegretChainedWrath = false;
        public bool RegretBinded = false;

        public bool RedEyesAlerted = false;
        public bool RedEyesPredator = false;
        public float RedEyesOpacity = 0f;
        public int RedEyesMealMax = 60 * 8;

        public bool RemorseHammerTime = false;
        public int RemorseNailInflictMax = 100;
        public int RemorseLeer = 0;
        public int RemorseLeerMax = 30;
        public int RemorseDecay = 0;

        public int TodaysExpressionFace = 0;
        public int TodaysExpressionTimer = 0;
        public int TodaysExpressionTimerMax = 180;
        public bool TodaysExpressionActive = false;

        public bool WristCutterScars = false;

        public override void ResetEffects()
        {
            MatchstickBurn = false;

            RegretChainedWrath = false;
            RegretBinded = false;

            RedEyesAlerted = false;
            RedEyesPredator = false;
            RedEyesOpacity = 0f;

            RemorseNailInflictMax = 100;
            RemorseHammerTime = false;
            RemorseLeerMax = 30;

            TodaysExpressionTimerMax = 300;
            TodaysExpressionActive = false;

            WristCutterScars = false;
        }

        public override void OnEnterWorld()
        {
            if (Player.HasBuff<TodaysLook>())
                Player.ClearBuff(ModContent.BuffType<Buffs.TodaysLook>());
        }

        public override void PostUpdateEquips()
        {
            if (TodaysExpressionActive && TodaysExpressionFace == 4)//If Angry, set def to 0
            {
                Player.statDefense -= 30;
                if (Player.statDefense > 0)
                    Player.statDefense *= 0;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (RegretBinded)
            {
                Player.moveSpeed = 1f;
            }
            else if (Player.HeldItem.type == ModContent.ItemType<Items.Ruina.Technology.RegretR>() && !Player.mount.Active)
            {
                Player.GetDamage(DamageClass.Melee) += (Player.moveSpeed - 1) * 0.5f;
                RegretChainedWrath = true;
                Player.moveSpeed = 1f;
            }
        }

        public override void PreUpdate()
        {
            if (RemorseLeer > 0)
            {
                Player.AddBuff(ModContent.BuffType<RemorseLeer>(), 2);
                if (RemorseLeer >= RemorseLeerMax)
                {
                    Player.AddBuff(ModContent.BuffType<RemorseCrack>(), 2);
                }
                RemorseDecay--;
                if (RemorseDecay <= 0)
                {
                    RemorseDecay = 60 * 5;
                    RemorseLeer--;
                }
            }
        }

        public override void PostUpdate()
        {
            if (TodaysExpressionActive)
            {
                TodaysExpressionTimer--;
                if (TodaysExpressionTimer <= 0)
                {
                    TodayExpressionChangeFace(Main.rand.Next(5));
                }
            }

            if (RemorseHammerTime)
            {
                if (RemorseLeer >= 10)
                    Player.AddBuff(ModContent.BuffType<RemorseCrack>(), 2);

                if (Player.ownedProjectileCounts[ModContent.ProjectileType<RemorseNailEX>()] == 0)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<RemorseNailEX>(), 1, 0, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<RemorseHammerEX>(), 1, 0, Player.whoAmI);
                }
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if (WristCutterScars)
            {
                Player.lifeRegen = 0;
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (RemorseLeer > 0)
            {
                modifiers.FinalDamage.Flat += RemorseLeer;
            }
        }

        public override void PostHurt(Player.HurtInfo info)
        {
            int heldItem = Player.HeldItem.type;
            if (heldItem == ModContent.ItemType<Items.Ruina.Literature.TodaysExpressionR>() && Main.myPlayer == Player.whoAmI)
            {
                float velocityMult = 1f;
                float knockback = 6f;
                float projDamage = Player.HeldItem.damage;
                switch (TodaysExpressionFace)
                {
                    case 0://Happy
                        velocityMult *= 0.1f;
                        knockback *= 3f;
                        projDamage = (int)(info.Damage * 0.05f);
                        break;
                    case 1://Smile
                        velocityMult *= 0.4f;
                        knockback *= 2f;
                        projDamage = (int)(info.Damage * 0.5f);
                        break;
                    default://Neutral
                        break;
                    case 3://Sad
                        velocityMult *= 1.5f;
                        knockback *= 0.5f;
                        projDamage = (int)(info.Damage * 1.1f);
                        break;
                    case 4://Angry
                        velocityMult *= 2.2f;
                        knockback *= 0f;
                        projDamage = (int)(info.Damage * 1.5f);
                        break;
                }

                for (int i = 0; i < 6; i++)
                {
                    Vector2 vel = new Vector2(16f, 0).RotatedBy(MathHelper.ToRadians(60 * i));

                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, vel * velocityMult, ModContent.ProjectileType<Projectiles.Realized.TodaysExpressionWall>(), (int)projDamage, knockback, Player.whoAmI, TodaysExpressionFace);
                }

                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Literature/Shy_Strong_Guard") with { Volume = 0.5f }, Player.position);
            }
            if (heldItem == ModContent.ItemType<Items.Teth.Beak>())
            {
                int damage = info.Damage;
                float percent = (float)damage / Player.statLifeMax2;
                if (BeakGunCounter < 1f)
                    BeakGunCounter = 1f;
                BeakGunCounter += percent * 2;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (RemorseLeer > 0)
            {
                modifiers.FlatBonusDamage += RemorseLeer;
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (RedEyesOpacity > 0f)
            {
                float opacity = 1f - RedEyesOpacity;
                r *= opacity;
                g *= opacity;
                b *= opacity;
                a *= opacity;
            }
        }

        public void FourthMatchExplode(bool forced = false)
        {
            if (FourthMatchFlameGift)
                return;
            //Player.AddBuff(BuffID.OnFire, 120);
            Player.AddBuff(ModContent.BuffType<Matchstick>(), 600);
            if (Main.myPlayer == Player.whoAmI && forced)// || (Main.rand.Next(100) == 0 && (Player.statLife == Player.statLifeMax2 || (float)(Player.statLife / (float)Player.statLifeMax2) < 0.25f)))
            {
                int dmg = (int)(Player.statLifeMax2 * 0.2f);
                int i = Projectile.NewProjectile(Player.GetSource_Misc("ItemUse_FourthMatchFlameExplosion"), Player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.FourthMatchFlameSelfExplosion>(), dmg, 10f, Player.whoAmI);

                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, i);
            }
        }

        public void TodayExpressionChangeFace(int face)
        {
            TodaysExpressionTimer = TodaysExpressionTimerMax;
            TodaysExpressionFace = face;
        }

        public float TodaysExpressionDamage()
        {
            switch (TodaysExpressionFace)
            {
                case 0://Happy
                    return Buffs.TodaysLook.TODAYDAMAGEHAPPY;
                case 1://Smile
                    return Buffs.TodaysLook.TODAYDAMAGESMILE;
                default://Neutral
                    return Buffs.TodaysLook.TODAYDAMAGENEUTRAL;
                case 3://Sad
                    return Buffs.TodaysLook.TODAYDAMAGESAD;
                case 4://Angry
                    return Buffs.TodaysLook.TODAYDAMAGEANGRY;
            }
        }

        public void RemorseGainLeer(int amount)
        {
            RemorseDecay = 60 * 5;
            RemorseLeer += amount;
            if (RemorseLeer > RemorseLeerMax)
            {
                RemorseLeer = RemorseLeerMax;
            }
        }

        public void RemorseApplyNail(int target, bool big = false)
        {

            int type = ModContent.ProjectileType<RemorseNail>();
            if (big)
                type = ModContent.ProjectileType<RemorseNailEX2>();

            if (!big)
            {
                if (LobotomyGlobalNPC.RemorseGetNailAmount(target) > RemorseLeerMax)
                    return;
            }
            Vector2 position = Main.npc[target].Center;
            int real = target + 1;
            Projectile.NewProjectile(Player.GetSource_FromThis(), position, Vector2.Zero, type, 1, 0, Player.whoAmI, -1, real);
        }

        public bool RedEyesEitherHeld => Main.LocalPlayer.HeldItem.type == ModContent.ItemType<Items.Ruina.Literature.RedEyesR>() || Main.LocalPlayer.HeldItem.type == ModContent.ItemType<Items.Teth.RedEyes>();
    }
}
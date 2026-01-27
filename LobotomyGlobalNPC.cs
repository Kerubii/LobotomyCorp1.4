using FullSerializer;
using LobotomyCorp;
using LobotomyCorp.Buffs;
using LobotomyCorp.Items.NonEgo;
using LobotomyCorp.Items.Ruina.Art;
using LobotomyCorp.Items.Ruina.Language;
using LobotomyCorp.Items.Ruina.Natural;
using LobotomyCorp.Items.Ruina.Technology;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles;
using LobotomyCorp.Projectiles.KingPortal;
using LobotomyCorp.Projectiles.Realized;
using log4net.Util;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp
{
    public class LobotomyGlobalNPC : GlobalNPC
    {
        public override void Load()
        {
            Terraria.IL_NPC.TargetClosest += forcetarget;

            base.Load();
        }

        private void forcetarget(ILContext il)
        {
            var c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchLdloc(3)))
                throw new NotImplementedException("A Mod is Incompatible, Report to Lobotomy Mod Author Kerubii");

            c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
            c.EmitDelegate<Action<NPC>>((npc) =>
            {
                npc.GetGlobalNPC<LobotomyGlobalNPC>().ForceChangeTarget(npc);
            });
        }

        public void ForceChangeTarget(Terraria.NPC npc)
        {
            if (SanguineDesireGlitter && SanguineDesireGlitterTarget > -1 && !Main.player[SanguineDesireGlitterTarget].dead)
            {
                npc.target = SanguineDesireGlitterTarget;
            }
            //Main.NewText("Code is running, Target is " + SanguineDesireGlitterTarget);
        }

        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        public int BeakTarget = 0;
		public bool BODExecute = false;

        public bool CrimsonScarPrey = false;

        public int DaCapoSilentMusicPhase = 0;
        public bool DaCapoSilentMusic = false;

        public bool DiffractionDiffracted = false;
        public float DiffractionDamage = 1f;
        public float DiffractionDefense = 1f;
        public float DiffractionHealth = 1f;

        public float FragmentsFromSomewhereTentacles = 0;
        public bool FragmentsFromSomewhereEnlightenment = false;
        public int FragmentsFromSomewherePlayer = -1;

        public bool GoldRushBlissBuff = false;
        public bool GoldRushBrokenBlissBuff = false;

        public bool HarmonyMusicalAddiction = false;

        public bool InTheNameOfLoveAndHateVillain = false;

        public int LaetitiaGiftDamage = 0;
        public int LaetitiaGiftOwner = -1;
        public bool LaetitiaGiftRM = false;

        public bool MatchstickBurn = false;
        public int MatchstickBurnTime = 0;

        public bool NihilDebuff = false;

        public bool PleasureDebuff = false;
        public int PleasureCount = 0;

        public bool QueenBeeSpore = false;
        public bool QueenBeeLarva = true;

        public bool RedEyesCocoon = false;
        private int RedEyesMeal = 0;
        private int RedEyesMealDecay = 0;
        public int RedEyesCocoonPlayer = -1;
        public int RedEyesCocoonCooldown = 0;

        public bool RegretMetallicRinging = false;

        public int RemorseNailTotal = 0;
        public int RemorseGuilt = 0;
        public int RemorseNailActive = 0;
        public int RemorseNailTimer = 0;

        public bool SanguineDesireGlitter = false;
        public int SanguineDesireGlitterTarget = -1;
        public bool SanguineDesireExtensiveBleeding = false;
        public float SanguineDesireExtensiveBleedAmount = 0;

        public bool SmileVomit = false;

        public float SmileCorpseThreshold = 1f;

        public bool WingbeatFairyMeal = false;
        public int WingbeatFairyHeal = 0;
        public int WingbeatRotation = Main.rand.Next(360);
        public int WingbeatTarget = -1;
        public int WingbeatIndicator = 0;
        public bool WingbeatFairyAnger = false;
        public int RiskLevel = 0;

        public bool WristCutterScars = false;

        public static LobotomyGlobalNPC LNPC (NPC npc)
        {
            return npc.GetGlobalNPC<LobotomyGlobalNPC>();
        }

        public override void ResetEffects(NPC npc)
        {
            if (BeakTarget > 0)
                BeakTarget--;

            CrimsonScarPrey = false;

            DaCapoSilentMusic = false;

            FragmentsFromSomewhereEnlightenment = false;

            GoldRushBlissBuff = false;
            GoldRushBrokenBlissBuff = false;

            HarmonyMusicalAddiction = false;

            InTheNameOfLoveAndHateVillain = false;

            MatchstickBurn = false;

            PleasureDebuff = false;

            QueenBeeSpore = false;

            if (!RedEyesCocoon)
                RedEyesCocoonPlayer = -1;
            RedEyesCocoon = false;
            if (RedEyesMealDecay <= 0)
            {
                if (RedEyesMeal > 0)
                    RedEyesMeal--;
            }
            else
                RedEyesMealDecay--;

            RegretMetallicRinging = false;

            if (!SanguineDesireGlitter)
                SanguineDesireGlitterTarget = -1;
            SanguineDesireGlitter = false;
            SanguineDesireExtensiveBleeding = false;

            SmileVomit = false;

            if (RedEyesCocoonCooldown > 0)
                RedEyesCocoonCooldown--;

            if (!WingbeatFairyMeal)
            {
                WingbeatFairyHeal = 0;
                WingbeatRotation = Main.rand.Next(360);
            }
            if (WingbeatTarget > -1 && Main.player[WingbeatTarget].GetModPlayer<LobotomyZayinPlayer>().RealizedWingbeatMeal != npc.whoAmI)
                WingbeatTarget = -1;
            WingbeatFairyMeal = false;

            WristCutterScars = false;
        }

		public override bool PreAI(NPC npc)
		{
			return !BODExecute;
		}

        public override void AI(NPC npc)
        {
            if (QueenBeeSpore)
            {
                Dust.NewDust(npc.Center, 1, 1, ModContent.DustType<Misc.Dusts.HornetDust>());
            }

            if (WingbeatFairyMeal)
            {
                WingbeatRotation += 3;
                WingbeatFairyHeal -= 3;
                if (WingbeatRotation > 360)
                    WingbeatRotation -= 360;
                if (Main.rand.Next(30) == 0)
                {
                    Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 89)];
                    dust.velocity *= 0;
                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                }
                if (WingbeatFairyHeal <= 0)
                {
                    WingbeatFairyHeal = 120;
                    npc.life += 10;
                    if (npc.life > npc.lifeMax)
                        npc.life = npc.lifeMax;
                    npc.HealEffect(10, true);
                    for (int i = 0; i < 10; i++)
                    {
                        Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 89)];
                        dust.velocity *= 0;
                        dust.noGravity = true;
                        dust.fadeIn = 1f;
                    }
                }
            }
            if (WingbeatFairyMeal && WingbeatNear(npc))
            {
                if (WingbeatIndicator < 5)
                    WingbeatIndicator++;
            }
            else if (WingbeatIndicator > 0)
                WingbeatIndicator--;
        }

        public override void PostAI(NPC npc)
        {
            if (RemorseNailActive > 0)
            {
                RemorseNailTimer--;
                if (RemorseNailTimer <= 0)
                {
                    npc.SimpleStrikeNPC(25, 1);
                    RemorseNailActive--;
                    RemorseNailTimer = 8;
                }
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (HarmonyMusicalAddiction)
            {
                npc.lifeRegen = -12;
                damage += 5;
            }

            if (MatchstickBurn)
            {
                npc.lifeRegen -= MatchstickBurnTime * 2;
                damage += MatchstickBurnTime;
            }
            if (PleasureDebuff)
            {
                npc.lifeRegen -= 10;
                damage += 5;
            }
            if (QueenBeeSpore)
            {
                npc.lifeRegen -= 30;
                damage += 10;
            }

            if (SmileVomit)
            {
                npc.lifeRegen -= 40;
                damage += 30;
            }

            if (WristCutterScars)
            {
                npc.lifeRegen -= 10;
                damage += 2;
            }

            if (DaCapoSilentMusic)
            {
                npc.lifeRegen -= 35;
                damage += 35;
                if (DaCapoSilentMusicPhase % 5 > 1)
                {
                    npc.lifeRegen -= 10;
                    damage += 10;
                }
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (HarmonyMusicalAddiction)
            {
                modifiers.FinalDamage *= 1.1f;
            }

            if (SanguineDesireGlitter)
            {
                if (SanguineDesireGlitterTarget == target.whoAmI)
                    modifiers.FinalDamage *= 1.15f;
                else
                    modifiers.FinalDamage *= 0.8f;
            }

            if (RedEyesCocoon)
            {
                modifiers.FinalDamage *= 0.88f;
            }

            if (DiffractionDiffracted)
            {
                modifiers.SourceDamage *= DiffractionDamage;
            }

            if (NihilDebuff)
                modifiers.FinalDamage *= 0.7f;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            if (SanguineDesireExtensiveBleeding && !npc.immortal)
            {
                int bleedDamage = (int)SanguineDesireExtensiveBleedAmount;
                
                CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), CombatText.DamagedHostile, (int)bleedDamage, false);
                if (npc.realLife >= 0)
                {
                    Main.npc[npc.realLife].life -= (int)bleedDamage;
                    npc.life = Main.npc[npc.realLife].life;
                    npc.lifeMax = Main.npc[npc.realLife].lifeMax;
                }
                else
                {
                    npc.life -= (int)bleedDamage;
                }
                npc.HitEffect(0, bleedDamage);

                for (int i = 0; i < bleedDamage / 2; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
                }

                SanguineDesireExtensiveBleedAmount /= 3;
                if (SanguineDesireExtensiveBleedAmount < 1f)
                {
                    int type = ModContent.BuffType<Buffs.ExtensiveBleeding>();
                    if (npc.HasBuff<Buffs.ExtensiveBleeding>())
                    {
                        int i = npc.FindBuffIndex(type);
                        npc.DelBuff(i);
                    }
                }
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (LaetitiaGiftOwner >= 0)
            {
                LaetitiaGiftDamage += damageDone;
                if (LaetitiaGiftDamage > 10f)
                {
                    Main.player[LaetitiaGiftOwner].ApplyDamageToNPC(npc, LaetitiaGiftDamage, 0f, 1, false);
                    if (Main.myPlayer == LaetitiaGiftOwner)
                        Projectile.NewProjectile(Main.player[LaetitiaGiftOwner].GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Realized.LaetitiaFriend>(), 60, 3.5f, LaetitiaGiftOwner);

                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 velocity = (Vector2.UnitX * Main.rand.Next(5)).RotateRandom(6.28f);

                        Dust.NewDustPerfect(npc.Center, 69, velocity).noGravity = true;
                    }

                    LaetitiaGiftDamage = 0;
                    LaetitiaGiftOwner = -1;
                }
            }

            if (RegretMetallicRinging && projectile.type == ModContent.ProjectileType<Projectiles.Realized.RegretR>() && projectile.owner == Main.myPlayer)
            {
                Player player = Main.player[projectile.owner];
                int type = ModContent.ProjectileType<Projectiles.Realized.RegretShock>();
                Projectile.NewProjectile(player.GetSource_FromThis(), npc.Center, Vector2.Zero, type, projectile.damage, 0.2f, projectile.owner);
            }
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (HarmonyMusicalAddiction)
                modifiers.ArmorPenetration += 15;

            if (DiffractionDiffracted)
            {
                modifiers.SourceDamage *= DiffractionDefense;
            }

            if (DaCapoSilentMusic)
            {
                modifiers.ArmorPenetration += 15;
                if (DaCapoSilentMusicPhase % 5 > 2)
                {
                    modifiers.SourceDamage *= 1.15f;
                }
                if (DaCapoSilentMusicPhase > 4 && DaCapoSilentMusicPhase % 5 > 2)
                    modifiers.SourceDamage *= 0.9f;
            }

            if (GoldRushBlissBuff)
            {
                modifiers.Defense.Flat += 5;
            }
            else if (GoldRushBrokenBlissBuff)
            {
                modifiers.Defense.Flat -= 10;
            }

            if (NihilDebuff)
            {
                modifiers.Defense.Base *= 0.7f;
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (FragmentsFromSomewhereEnlightenment && player.whoAmI == FragmentsFromSomewherePlayer)
            {
                modifiers.SourceDamage += 0.2f;
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (FragmentsFromSomewhereEnlightenment)
            {
                if (projectile.owner == FragmentsFromSomewherePlayer)
                {
                    if (FragmentEnlightenmentBlacklist(projectile))
                        modifiers.FinalDamage /= 2;
                    else
                        modifiers.SourceDamage += 0.2f;
                }
                else
                    modifiers.SourceDamage += 0.2f;
            }
        }

        bool FragmentEnlightenmentBlacklist(Projectile projectile)
        {
            if (projectile.type != ModContent.ProjectileType<Projectiles.Realized.FragmentsFromSomewhereRSpear>() || projectile.aiStyle != ProjAIStyleID.Spear)
                return true;
            return false;
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (BeakTarget > 0)
                drawColor = Color.Red;

            if (SanguineDesireGlitter && SanguineDesireGlitterTarget == Main.myPlayer)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust d = Dust.NewDustPerfect(npc.Center, DustID.PinkCrystalShard);
                    d.noGravity = true;
                    d.scale = 0.5f;
                }
            }

            if (MatchstickBurn)
            {
                if (MatchstickBurnTime < 20)
                {
                    if (Main.rand.NextBool(25 - MatchstickBurnTime))
                        Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch);
                }
                else
                {
                    if (Main.rand.NextBool(2))
                    {
                        float scale = 0.5f + MatchstickBurnTime / 40f;
                        if (scale > 4f)
                            scale = 4f;
                        Dust d = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch)];
                        d.scale = scale;
                        d.noGravity = true;
                    }
                }
            }

            if (npc.HasBuff(ModContent.BuffType<Buffs.CrookedNotes>()) && Main.rand.NextBool(3))
            {
                Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Misc.Dusts.NoteDust>())].velocity.Y -= 1f;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                if (RedEyesMealAmount > 0 && Main.LocalPlayer.GetModPlayer<LobotomyTethPlayer>().RedEyesEitherHeld)// Main.LocalPlayer.HeldItem.type == ModContent.ItemType<Items.Ruina.Literature.RedEyesR>())
                {
                    int RedEyesMax = Main.LocalPlayer.GetModPlayer<LobotomyTethPlayer>().RedEyesMealMax;
                    if (RedEyesMealAmount > RedEyesMax)
                    {
                        drawColor = Color.Lerp(Color.Red, Color.Pink, (float)Math.Sin(6.28f * ((float)Main.timeForVisualEffects % 30) / 30f));
                    }
                    else
                    {
                        float amount = (float)RedEyesMealAmount / RedEyesMax;
                        if (amount > 1)
                            amount = 1;
                        Color red = drawColor;
                        byte r = (byte)(255 - red.R);
                        red.R = (byte)(red.R + r * amount);
                        red.G = (byte)(red.G * amount);
                        red.B = (byte)(red.B * amount);
                        drawColor = Color.Lerp(drawColor, red, amount);
                    }
                }
            }
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (InTheNameOfLoveAndHateVillain && Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>().LoveAndHateVillain == npc.whoAmI)
            {
                Texture2D mark = LobotomyCorp.VillainMark.Value;
                Vector2 position = npc.position - Main.screenPosition + new Vector2(npc.width/2, 0);
                float scale = 1.05f + 0.05f * (float)Math.Sin(Main.timeForVisualEffects / 120 * 6.34f);

                spriteBatch.Draw(mark, position, mark.Frame(), Color.White * 0.8f, 0, mark.Frame().Size() / 2, scale, SpriteEffects.None, 0f);
            }

            if (FragmentsFromSomewhereTentacles > 0f && Main.LocalPlayer.HeldItem.type == ModContent.ItemType<FragmentsFromSomewhereR>())
            {
                spriteBatch.End();
                spriteBatch.Begin((SpriteSortMode)1, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect)null, Main.Transform);
                Color color = drawColor;
                Color def = default(Color);
                if (color != def)
                {
                    //color = npc.GetColor(drawColor);
                    //color.R = (byte)(color.R * drawColor.R / 255f);
                    //color.G = (byte)(color.G * drawColor.G / 255f);
                    //color.B = (byte)(color.B * drawColor.B / 255f);
                    //color.A = (byte)(color.A * drawColor.A / 255f);
                }

                DrawData drawData = new DrawData(TextureAssets.Npc[npc.type].Value, npc.position - screenPos, npc.frame, drawColor);
                LobotomyCorp.LobcorpShaders["Fragment"].UseSaturation(FragmentsFromSomewhereTentacles / 4f);
                LobotomyCorp.LobcorpShaders["Fragment"].UseColor(color.R / 255f, color.G / 255f, color.B / 255f);
                LobotomyCorp.LobcorpShaders["Fragment"].UseOpacity(color.A / 255f);
                LobotomyCorp.LobcorpShaders["Fragment"].Apply(drawData);
            }
            else if (FragmentsFromSomewhereEnlightenment && Main.myPlayer == FragmentsFromSomewherePlayer)
            {
                spriteBatch.End();
                spriteBatch.Begin((SpriteSortMode)1, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect)null, Main.Transform);

                Color color = npc.color;
                color.R = (byte)(color.R * drawColor.R / 255f);
                color.G = (byte)(color.G * drawColor.G / 255f);
                color.B = (byte)(color.B * drawColor.B / 255f);
                color.A = (byte)(color.A * drawColor.A / 255f);

                float width = 1f;
                float maxWidth = npc.width * 0.1f < 3 ? 3 : npc.width * 0.1f;
                maxWidth /= 2;
                width += maxWidth;

                DrawData drawData = new DrawData(TextureAssets.Npc[npc.type].Value, npc.position - screenPos, npc.frame, drawColor);
                LobotomyCorp.LobcorpShaders["FragmentEnlightened"].UseColor(drawColor.R / 255f, drawColor.G / 255f, drawColor.B / 255f);
                LobotomyCorp.LobcorpShaders["FragmentEnlightened"].UseOpacity(drawColor.A / 255f);
                LobotomyCorp.LobcorpShaders["FragmentEnlightened"].UseCustomShaderDate(width + maxWidth * (float)Math.Sin(6.28f * Main.timeForVisualEffects / 60f));
                LobotomyCorp.LobcorpShaders["FragmentEnlightened"].Apply(drawData);
            }

            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (CrimsonScarPrey && Main.LocalPlayer.HeldItem.type == ModContent.ItemType<CrimsonScarR>())
            {
                Texture2D mark = Mod.Assets.Request<Texture2D>("Misc/LittleRedMark").Value;
                Vector2 position = npc.position - Main.screenPosition + new Vector2(npc.width / 2, - 40);

                spriteBatch.Draw(mark, position, mark.Frame(), Color.White, 0, mark.Frame().Size() / 2, 1f, SpriteEffects.None, 0f);
            }

            if (FragmentsFromSomewhereTentacles > 0f ||
                FragmentsFromSomewhereEnlightenment)
            {
                spriteBatch.End();
                spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, (Effect)null, Main.Transform);
            }

            if (WingbeatFairyMeal)
            {
                Texture2D texture = TextureAssets.Projectile[ModContent.ProjectileType<WingbeatFairy>()].Value;
                Vector2 position = npc.Center - Main.screenPosition + new Vector2(npc.width, 0).RotatedBy(MathHelper.ToRadians(WingbeatRotation));

                Rectangle? frame = new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, texture.Height / 6 * (int)(WingbeatRotation / 60f), texture.Width, texture.Height / 6));

                spriteBatch.Draw(texture, position, frame, drawColor, 0, new Vector2(15, 12), 1f, SpriteEffects.None, 0f);
            }

            if (Main.netMode != NetmodeID.Server)
            {
                LobotomyTethPlayer modTethPlayer = Main.LocalPlayer.GetModPlayer<LobotomyTethPlayer>();
                LobotomyHePlayer modHePlayer = Main.LocalPlayer.GetModPlayer<LobotomyHePlayer>();
                LobotomyWawPlayer modWawPlayer = Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>();
                LobotomyAlephPlayer modAlephPlayer = Main.LocalPlayer.GetModPlayer<LobotomyAlephPlayer>();

                if (WingbeatIndicator > 0)
                {
                    Texture2D texture = WingbeatFairy.Target.Value;
                    Vector2 position = npc.Center - Main.screenPosition;
                    Rectangle? frame = new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height));
                    float rotation = (Main.LocalPlayer.Center - npc.Center).ToRotation();
                    Vector2 scale = new Vector2(((npc.width < npc.height ? npc.height * 2 : npc.width * 2) / 60f), 0.5f);
                    Color color = Color.White * (WingbeatIndicator / 10f);
                    color.A = (byte)(color.A * 0.8f);
                    spriteBatch.Draw(texture, position, frame, color, rotation, new Vector2(20, 15), scale, SpriteEffects.None, 0f);
                }

                if (modHePlayer.ForgottenAffectionResistance >= 0.03f && modHePlayer.ForgottenAffection == npc.whoAmI)
                {
                    Texture2D texture = Mod.Assets.Request<Texture2D>("Misc/HappyMemories").Value;
                    Vector2 position = npc.position + new Vector2(npc.width / 2, npc.gfxOffY) - Main.screenPosition;
                    position.Y -= texture.Height - 4;
                    Vector2 origin = new Vector2(23, 24);

                    int time = (int)Main.timeForVisualEffects % 180;
                    int dir = 0;
                    if (time < 60)
                        dir = 1;
                    if (time >= 120)
                        dir = -1;
                    float rotation = 0.174f * dir;

                    float opacity = modHePlayer.ForgottenAffectionResistance / 0.4f;

                    spriteBatch.Draw(texture, position, null, Color.White * opacity, rotation + 0.03f, origin, 1f, 0, 0);

                    if (modHePlayer.ForgottenAffectionResistance >= 0.4f)
                    {
                        texture = Mod.Assets.Request<Texture2D>("Misc/HappyMemoriesW").Value;
                        opacity = (float)Math.Sin(((int)Main.timeForVisualEffects % 60) / 60f * 3.14) * 0.5f;

                        spriteBatch.Draw(texture, position, null, Color.White * opacity, rotation + 0.03f, origin, 1f, 0, 0);
                    }
                }

                if (modWawPlayer.MagicBulletRequest == npc.whoAmI)
                {
                    Texture2D texture = Mod.Assets.Request<Texture2D>("Misc/MagicBulletCircle").Value;
                    Vector2 position = npc.Center + new Vector2(0, npc.gfxOffY) - Main.screenPosition;
                    float scale = npc.width > npc.height ? npc.width / 122f : npc.height / 122f;
                    if (scale > 0.5f)
                        scale = 0.5f;

                    float rotation = MathHelper.ToRadians((5 + 20f * scale) * (float)Main.timeForVisualEffects);

                    spriteBatch.Draw(texture, position, null, Color.White * 0.7f, rotation, texture.Size() / 2, scale, 0, 0);
                }

                if (RedEyesMealAmount >= modTethPlayer.RedEyesMealMax && modTethPlayer.RedEyesEitherHeld)
                {
                    Texture2D texture = Mod.Assets.Request<Texture2D>("Misc/MarkedMeal").Value;
                    Vector2 position = npc.Center + new Vector2(0, npc.gfxOffY) - Main.screenPosition;
                    float scale = 1f + 0.2f * (float)Math.Sin(0.052f * Main.timeForVisualEffects);

                    spriteBatch.Draw(texture, position, null, Color.White * (0.5f + 0.2f * (float)Math.Sin(0.15f * Main.timeForVisualEffects)), 0f, texture.Size() / 2, scale, 0, 0);
                }

                if ((GoldRushBlissBuff && modAlephPlayer.GoldRushHasBliss(npc)) || GoldRushBrokenBlissBuff)
                {
                    Texture2D texture = TextureAssets.Projectile[ModContent.ProjectileType<BrilliantBliss>()].Value;
                    Vector2 position = npc.position - Main.screenPosition + new Vector2(npc.width / 2, -40);
                    Color color = Color.White;
                    if (GoldRushBrokenBlissBuff)
                    {
                        texture = BrilliantBliss.Broken.Value;
                        if (npc.HasBuff<BrokenBliss>())
                        {
                            int time = npc.buffTime[npc.FindBuffIndex(ModContent.BuffType<BrokenBliss>())];
                            if (time < 60)
                            {
                                color *= (time / 60f);
                            }
                        }
                        spriteBatch.Draw(texture, position, texture.Frame(), color, 0, texture.Size() / 2, 1f, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        spriteBatch.Draw(texture, position, texture.Frame(), color, 0, texture.Size() / 2, 1f, SpriteEffects.None, 0f);
                        Color lightColor = new Color(1f, 1f, 1f, 0.8f) * 0.4f;
                        spriteBatch.Draw(texture, position, texture.Frame(), lightColor, 0, texture.Size() / 2, 1.1f + 0.1f * (float)Math.Sin(3.14f * Main.timeForVisualEffects / 120f), SpriteEffects.None, 0f);
                    }
                }
            }

            if (SanguineDesireGlitter && SanguineDesireGlitterTarget == Main.myPlayer && SanguineDesireGlitterTarget == npc.target)
            {
                Texture2D texture = LobotomyCorp.RedShoesGlittering.Value;
                Vector2 scale = new Vector2(npc.width / 50f, npc.width / 50f * 0.8f) * 0.75f;
                if (npc.height > npc.width)
                {
                    scale = new Vector2(npc.height / 50f, npc.height / 50f * 0.8f) * 0.75f;
                }
                Color color = Color.White * (0.7f + 0.1f * (float) Math.Sin(0.03f * Main.timeForVisualEffects));

                spriteBatch.Draw(texture, npc.Center - Main.screenPosition + Vector2.UnitY * (npc.gfxOffY - npc.height * 0.25f), texture.Frame(), color, 0f, texture.Size() / 2, scale, 0, 0);
            }
        
            if (LaetitiaGiftOwner >= 0)
            {
                float progress = Math.Clamp(LaetitiaGiftDamage / 1600f, 0f, 1f);

                Texture2D texture = Mod.Assets.Request<Texture2D>("Projectiles/Realized/LaetitiaR").Value;
                Vector2 position = npc.Center - Main.screenPosition + new Vector2(0, -npc.height / 2 - 30);
                float scale = 1f + 0.2f * (float)Math.Sin(6.28f * ((Main.timeForVisualEffects * (8 * progress)) / 60));
                if (scale < 1f)
                    scale = 1f;
                Rectangle frame = texture.Frame();

                spriteBatch.Draw(texture, position, frame, drawColor, 0f, frame.Size()/2, scale, SpriteEffects.None, 0f);
            }

            if (LaetitiaGiftRM)
            {
                Texture2D texture = Mod.Assets.Request<Texture2D>("Projectiles/Realized/LaetitiaR").Value;
                Vector2 position = npc.Center - Main.screenPosition + new Vector2(0, -npc.height / 2 - 30);
                float scale = 1f + 0.2f * (float)Math.Sin(6.28f * ((Main.timeForVisualEffects * (2)) / 60));
                if (scale < 1f)
                    scale = 1f;
                Rectangle frame = texture.Frame();

                spriteBatch.Draw(texture, position, frame, drawColor, 0f, frame.Size() / 2, scale, SpriteEffects.None, 0f);
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (HornetTarget(npc))
            {
                SpawnHornet(npc);
            }
            return true;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (!Main.expertMode)
            {
                if (npc.type == NPCID.QueenBee)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Waw.Hornet>(), 2));
                }
                if (npc.type == NPCID.WallofFlesh)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType <Items.Aleph.Censored>(), 4));
                }
                if (npc.type == NPCID.EyeofCthulhu)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType <Items.He.Syrinx>(), 5));
                }
            }
        }

        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Merchant)
            {
                shop.Add(ModContent.ItemType<Items.Teth.WristCutter>(), Condition.BloodMoon);
            }
            else if (shop.NpcType == NPCID.Dryad)
            {
                shop.Add(new Item(ModContent.ItemType<Items.Waw.Hypocrisy>())
                {
                    shopCustomPrice = 100000
                }, Condition.DownedQueenBee, Condition.DownedSkeletron);
            }
            else if (shop.NpcType == NPCID.ArmsDealer)
            {
                Condition condition = new Condition("Lobotomy:HasMagicBullet", new Func<bool>(() => Main.LocalPlayer.HasItem(ModContent.ItemType<MagicBullet>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<Items.Ruina.Technology.MagicBulletR>())));

                shop.Add(new Item(ModContent.ItemType<Items.NonEgo.MagicBulletBullet>())
                {
                    shopCustomPrice = 50
                }, condition);
            }
        }

        public bool WingbeatNear(NPC npc)
        {
            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<Items.Ruina.History.WingbeatR>())
                return Vector2.Distance(npc.Center, Main.LocalPlayer.Center) - (npc.width < npc.height ? npc.height/2 : npc.width/2) < Projectiles.WingbeatR.WingbeatRDistance;
            return false;
        }

        public static bool HornetTarget(NPC npc)
        {
            return LNPC(npc).QueenBeeSpore;
        }

        public static void SpawnHornet(NPC npc, int player = -1, int damage = 0, float knockBack = 0)
        {
            Mod Mod = ModLoader.GetMod("LobotomyCorp");
            if (Main.myPlayer == player && LNPC(npc).QueenBeeLarva)
            {
                if (player <= -1)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player p = Main.player[i];
                        if (p.active && !p.dead && p.HeldItem.type == ModContent.ItemType<Items.Ruina.History.HornetR>())
                        {
                            Item item = p.HeldItem;
                            player = i;
                            damage = item.damage / 2;
                            knockBack = item.knockBack;
                            break;
                        }
                    }

                    if (player <= -1)
                        return;
                }

                int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.WorkerBee>(), damage, knockBack, player);
                Main.projectile[proj].originalDamage = damage;
                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Misc.Dusts.HornetDust>());
                }
                LNPC(npc).QueenBeeLarva = false;
            }
        }

        public void RedEyesApplyMeal(int amount, int decayTimer = 600, bool predatorBoosted = false)
        {
            if (RedEyesCocoon)
            {
                RedEyesMeal = 0;
                RedEyesMealDecay = 0;
                return;
            }
            RedEyesMeal += amount;
            if (predatorBoosted)
                RedEyesMeal += amount / 2;
            RedEyesMealDecay = decayTimer;
        }
        public int RedEyesMealAmount { get { return RedEyesMeal; } }

        public static void SanguineDesireApplyBleed(NPC npc, float amount, int amountMax, int time, int timeMax)
        {
            LNPC(npc).SanguineDesireExtensiveBleedAmount += amount;
            if (LNPC(npc).SanguineDesireExtensiveBleedAmount > amountMax)
                LNPC(npc).SanguineDesireExtensiveBleedAmount = amountMax;
            if (npc.HasBuff<Buffs.ExtensiveBleeding>())
            {
                int i = npc.FindBuffIndex(ModContent.BuffType<Buffs.ExtensiveBleeding>());
                int buffTime = npc.buffTime[i] += time;
                if (buffTime > timeMax)
                    buffTime = timeMax;
                npc.AddBuff(ModContent.BuffType<Buffs.ExtensiveBleeding>(), buffTime);
            }
            else
                npc.AddBuff(ModContent.BuffType<Buffs.ExtensiveBleeding>(), time);
        }

        public static int SanguineDesireConsumeBleed(NPC npc)
        {
            LobotomyGlobalNPC Lnpc = LNPC(npc); 
            int bleedAmount = (int)Lnpc.SanguineDesireExtensiveBleedAmount;
            Lnpc.SanguineDesireExtensiveBleedAmount = 0;
            Lnpc.SanguineDesireExtensiveBleeding = false;
            npc.RequestBuffRemoval(ModContent.BuffType<Buffs.ExtensiveBleeding>());

            return bleedAmount;
        }

        public static void ApplyTentacles(NPC npc, float amount, float amountLimit = 2)
        {
            LobotomyGlobalNPC Lnpc = LNPC(npc);
            if (Lnpc.FragmentsFromSomewhereEnlightenment)
                return;

            Lnpc.FragmentsFromSomewhereTentacles += amount;
            if (Lnpc.FragmentsFromSomewhereTentacles > amountLimit)
                Lnpc.FragmentsFromSomewhereTentacles = amountLimit;

            if (npc.realLife > -1)
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (npc.whoAmI != n.whoAmI && (n.whoAmI == npc.realLife || n.realLife == npc.realLife))
                    {
                        Lnpc = LNPC(n);
                        Lnpc.FragmentsFromSomewhereTentacles += amount;
                        if (Lnpc.FragmentsFromSomewhereTentacles > amountLimit)
                            Lnpc.FragmentsFromSomewhereTentacles = amountLimit;
                    }
                }
            }
        }

        public static void ApplyPreEnlightenment(NPC npc, int player)
        {
            LobotomyGlobalNPC Lnpc = LNPC(npc);
            if (Lnpc.FragmentsFromSomewhereEnlightenment || npc.HasBuff<EnlightenmentPre>())
                return;

            int buffType = ModContent.BuffType<EnlightenmentPre>();
            npc.AddBuff(buffType, 60 * 2);
            Lnpc.FragmentsFromSomewherePlayer = player;

            if (npc.realLife > -1)
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (npc.whoAmI != n.whoAmI && (n.whoAmI == npc.realLife || n.realLife == npc.realLife))
                    {
                        Lnpc = LNPC(n);
                        if (Lnpc.FragmentsFromSomewhereEnlightenment || n.HasBuff<EnlightenmentPre>())
                            continue;

                        n.AddBuff(buffType, 60 * 2);
                        Lnpc.FragmentsFromSomewherePlayer = player;
                    }
                }
            }
        }

        public static void ApplyEnlightenment(NPC npc)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.DemonTorch);
            }

            npc.AddBuff(ModContent.BuffType<Enlightenment>(), 60 * 20);
            LobotomyGlobalNPC Lnpc = LNPC(npc);
            Lnpc.FragmentsFromSomewhereTentacles = 0;
            Lnpc.FragmentsFromSomewhereEnlightenment = true;
        }

        public static void PleasureApplyPleasure(Projectile proj, NPC npc, Player player, int damage, float maxMult = 1f)
        {
            LobotomyGlobalNPC Lnpc = LNPC(npc);
            // Increase Pleasure Count by damage given
            Lnpc.PleasureCount += damage;
            // Check if Pleasure reaches a certain Threshold for bonus damage
            float maximum = npc.lifeMax * 0.15f;
            if (Lnpc.PleasureCount > maximum * maxMult)
            {
                Lnpc.PleasureCount = 0;

                // Check if Pleasure damage (3 times more damage) is lower than 5% of enemy health, allows for true damage
                int pleasureDamage = proj.damage * 3;
                int minDamage = (int)(npc.lifeMax * 0.05f);
                if (pleasureDamage < minDamage)
                    pleasureDamage = minDamage;
                    
                player.ApplyDamageToNPC(npc, pleasureDamage, 0f, 1, false, DamageClass.Summon, true);
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Art/Porccu_Special") with { Volume = 0.25f }, npc.Center);
            }
        }

        public static int RemorseGetNailAmount(int npc)
        {
            int amount = 0;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                bool isType = p.type == ModContent.ProjectileType<RemorseNail>() || p.type == ModContent.ProjectileType<RemorseNailEX2>();
                if (isType && (int)p.ai[1] - 1 == npc)
                {
                    amount++;
                }
            }
            return amount;
        }   

        public void RemorseApplyGuilt(int amount = 1)
        {
            RemorseGuilt += amount;
            if (RemorseGuilt > 20)
            {
                RemorseGuilt = 20;
            }
        }

        public void RemorseApplyNail(int amount = 1)
        {
            RemorseNailTotal += amount;
            if (RemorseNailTotal > 100)
                RemorseNailTotal = 100;
        }

        public static void RemorseActivateNail(NPC npc, Player player)
        {
            LobotomyGlobalNPC Lnpc = LNPC(npc);
            Lnpc.RemorseNailActive = Lnpc.RemorseNailTotal;
            int remove = Math.Min(10, Lnpc.RemorseNailTotal/2);
            Lnpc.RemorseNailTotal -= remove;
            Lnpc.RemorseGuilt += remove;
        }
    }
}
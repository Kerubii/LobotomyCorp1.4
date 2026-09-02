using LobotomyCorp;
using LobotomyCorp.Items.Teth;
using LobotomyCorp.Items.Zayin;
using LobotomyCorp.Misc;
using LobotomyCorp.ModSystems;
using LobotomyCorp.NPCs.RedMist;
using LobotomyCorp.NPCs.RedMist.Projectiles;
using LobotomyCorp.Projectiles;
using LobotomyCorp.Projectiles.KingPortal;
using LobotomyCorp.UI;
using LobotomyCorp.Util;
using LobotomyCorp.Visuals.LobEffects;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static LobotomyCorp.NPCs.RedMist.RedMistSkeleton;

namespace LobotomyCorp.NPCs.RedMist
{
    [AutoloadBossHead]
    class RedMist : ModNPC
    {
        public override void Load()
        {
            string tex = "LobotomyCorp/NPCs/RedMist/RedMist_Head_Boss";
            Mod.AddBossHeadTexture(tex + "_2", -1);
            BossHead2 = ModContent.GetModBossHeadSlot(tex + "_2");
            Mod.AddBossHeadTexture(tex + "_3", -1);
            BossHead3 = ModContent.GetModBossHeadSlot(tex + "_3");
            Mod.AddBossHeadTexture(tex + "_4", -1);
            BossHead4 = ModContent.GetModBossHeadSlot(tex + "_4");
        }

        public static int BossHead2 = -1;
        public static int BossHead3 = -1;
        public static int BossHead4 = -1;

        public override void BossHeadSlot(ref int index)
        {
            switch (NPC.ai[0])
            {
                case 1:
                    index = BossHead2;
                    break;
                case 2:
                    index = BossHead3;
                    break;
                case 3:
                    index = BossHead4;
                    break;
                default:
                    break;
            }
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "LobotomyCorp/NPCs/RedMist/RedMist_Preview",
                PortraitScale = 1.4f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                Position = new Vector2(25, 75),
                PortraitPositionYOverride = 50,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("A shell of the legendary colored fixer known to be the strongest. Able to draw the full potential of an E.G.O, she has a numerous amount of deadly arsenal to turn her foes into a fine red mist")
            });
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.9f * balance * bossAdjustment);
            int def = Math.Min(20, 5 * (numPlayers - 1));
            NPC.defense += def;
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 100;
            NPC.lifeMax = 36000;
            //NPC.noTileCollide = true;
            //NPC.noGravity = true;
            NPC.damage = 20;
            NPC.defense = 34;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.0f;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.boss = true;
            NPC.SpawnWithHigherTime(30);
            NPC.DeathSound = SoundID.Item14;
            LobotomyGlobalNPC.LNPC(NPC).RiskLevel = (int)RiskLevel.Aleph;
            GoldRushCount = 0;
            if (!Main.dedServ)
            {
                //Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/PMSecondWarning");
            }

            NPC.npcSlots = 6;
        }

        private float Phase
        {
            get { return NPC.ai[0]; }
            set { NPC.ai[0] = value; }
        }

        private float AiState
        {
            get { return NPC.ai[1]; }
            set { NPC.ai[1] = value; }
        }

        private float Timer
        {
            get { return NPC.ai[2]; }
            set { NPC.ai[2] = value; }
        }

        private float NextState
        {
            get { return NPC.ai[3]; }
            set { NPC.ai[3] = value; }
        }

        private int FrontWeapon
        {
            get { return (int)NPC.localAI[1]; }
            set { NPC.localAI[1] = value; }
        }

        private int BackWeapon
        {
            get { return (int)NPC.localAI[3]; }
            set { NPC.localAI[3] = value; }
        }

        private RedMistSkeleton redmistSkeleton;
        private int suppTextCooldown = 0;
        public int GoldRushCount = 0;
        public int TwilightRushTime = 0;
        public const float GOLDRUSH4SPEED = 22f;
        public const int GOLDRUSH4DELAY = 20;
        /// <summary>
        /// Counts down by two if target is being actively seen
        /// </summary>
        private int Aggression = 0;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(GoldRushCount);
            writer.Write(Aggression);
            writer.Write(TwilightRushTime);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            GoldRushCount = reader.ReadInt32();
            Aggression = reader.ReadInt32();
            TwilightRushTime = reader.ReadInt32();
        }

        public override void AI()
        {
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
            }
            Player player = Main.player[NPC.target];
            DespawnCheck(player);
            Vector2 target = NPC.Center;
            target = NPC.GetTargetData().Center;
            float healthPercent = NPC.life / (float)NPC.lifeMax;

            if (Phase == 0)
            {
                //if (ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable && Main.masterMode)
                    //Phase2EX();
                //else
                    Phase1();
            }
            else if (Phase == 1)
            {
                //if (ModContent.GetInstance<Configs.LobotomyServerConfig>().TestItemEnable && Main.masterMode)
                    //Phase2EX();
                //else
                    Phase2();
            }
            else if (Phase == 2)
            {
                Phase3();
            }
            else if (Phase == 3)
            {
                Phase4();
            }
            else if (Phase == 4)
            {
                DeathPhase();
            }

            UpdateAggression();
            // ScreenFilterHandler();
            bubbleShieldDustDisplay((int)Phase);

            // Music Changer (Now in RedMistScene)
            /*if (Phase > 0)
            {
                if (Phase < 3)
                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/TilaridsDistortedNight");
                else
                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/TilaridsInsigniaDecay");
            }*/

            SuppressionTextSpawner();

            SkeletonHandler();
        }

        private void DespawnCheck(Player player)
        {
            if (player.dead)
            {
                // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                NPC.EncourageDespawn(10);
                if (AiState == IdleState)
                {
                    NPC.velocity.X *= 0.95f;
                    if (Math.Abs(NPC.velocity.X) < 1f)
                        NPC.velocity.X = 0;
                    // If the targeted player is dead, flee
                    Timer--;
                    if (Phase == 0)
                        ChangeAnimation(AnimationState.Idle1);
                    else if (Phase == 1)
                        ChangeAnimation(AnimationState.Idle2);
                    else if (Phase == 2)
                        ChangeAnimation(AnimationState.Idle3);
                    else if (Phase == 3)
                        ChangeAnimation(AnimationState.Idle4);

                    if (Timer <= -180)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                        }
                        NPC.position = Vector2.Zero;
                    }
                    return;
                }
            }
        }

        const int TeleportToPlayer = -1;

        const int FollowState = 0;
        const int SwingPenitence = 1;
        const int SwingRedEyes = 2;
        const int SwingBoth = 3;
        const int GoldRushBig = 4;
        const int GoldRushSmall = 8;
        const int SwitchPhase = 10;

        private void Phase1()
        {
            if (AiState == -1)
            {
                ChangeAnimation(AnimationState.Intro);
                NPC.dontTakeDamage = true;
                Timer++;
                if (Timer > 120)
                {
                    AiState = 0;
                    Timer = 0;
                    NPC.dontTakeDamage = false;
                    NPC.netUpdate = true;
                    // Multiplayer sometimes spawns her with lower HP, this is here to circumvent that
                    NPC.life = NPC.lifeMax;
                }
            }
            // Follow Mode
            else if (AiState <= FollowState)
            {
                NPC.dontTakeDamage = false;
                FollowMode1();
            }
            // EGO Swing
            else if (AiState <= SwingBoth)
            {
                SwingWeapon();
            }
            // Transition to Phase 2
            else if (AiState == SwitchPhase)
            {
                /*NPC.velocity.X = 0;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<RedMistEye>(), 0, 0, -1, NPC.whoAmI);
                Phase++;
                AiState = 0;
                Timer = 0;
                NPC.netUpdate = true;*/

                ChangeAnimation(AnimationState.Phase2Transition);
                Timer++;
                NPC.velocity.X = 0;

                if (Timer == 30 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<RedMistEye>(), 0, 0, -1, NPC.whoAmI);
                }

                if (Timer > 60)
                {
                    Phase++;
                    AiState = 0;
                    Timer = 0;
                    NPC.ai[3] = -1;
                    NPC.netUpdate = true;
                }
            }
            else
                GoldRush1Sequence();
        }

        void FollowMode1()
        {
            float healthPercent = NPC.life / (float)NPC.lifeMax;

            if (NPC.velocity.Y == 0)
            {
                if (NPC.velocity.X == 0)
                    ChangeAnimation(AnimationState.Idle1);
                else
                    ChangeAnimation(AnimationState.Walk1);
            }
            else
                ChangeAnimation(AnimationState.MidAir);

            fighterAI(NPC.GetTargetData(), 64f, 0.08f, 2f);
            Vector2 delta = NPC.GetTargetData().Center - NPC.Center;

            if (Timer > 0)
                Timer--;
            else
            {
                float attackRange = 128f;
                if (Main.expertMode)
                    attackRange *= 4;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if ((delta.Length() > 2000f && Main.rand.NextBool(360)) || Aggression > 300)
                    {
                        AiState = GoldRushSmall;
                        Aggression = 0;
                        NPC.netUpdate = true;
                    }
                    else if (delta.Length() < attackRange && Main.rand.NextBool(30))
                    {
                        int atk = Main.rand.Next(3);
                        AiState = 1 + atk; //Swings takes 50 frames 
                        NPC.netUpdate = true;
                    }
                }
            }

            if (CheckGoldRushCounterValid(0))
            {
                AiState = GoldRushBig;
                Timer = 0;
                Talk("GoldRush1", NPC.spriteDirection);
                NPC.netUpdate = true;
            }

            if (healthPercent <= .75f && GoldRushCount >= 3)
            {
                AiState = SwitchPhase;
                Timer = 0;
                NPC.netUpdate = true;
            }
        }

        bool CheckGoldRushCounterValid(int Phase)
        {
            float healthPercent = NPC.life / (float)NPC.lifeMax;
            healthPercent -= 0.75f - (0.25f * Phase);

            healthPercent /= 0.25f;
            int RushCountOffset = 3 * Phase;

            bool first = healthPercent < 0.66f && GoldRushCount <= 0 + RushCountOffset;
            bool second = healthPercent < 0.33f && GoldRushCount <= 1 + RushCountOffset;
            bool third = healthPercent <= 0f && GoldRushCount <= 2 + RushCountOffset;
            return first || second || third;
        }

        void SwingWeapon()
        {
            NPC.velocity.X *= 0;

            switch (AiState)
            {
                case SwingRedEyes:
                    ChangeAnimation(AnimationState.SwingRedEyes);
                    if (Timer == 0)
                        Talk("RedEyes", NPC.spriteDirection);
                    break;
                case SwingPenitence:
                    ChangeAnimation(AnimationState.SwingPenitence);
                    if (Timer == 0)
                        Talk("Penitence", NPC.spriteDirection);
                    break;
                case SwingBoth:
                    ChangeAnimation(AnimationState.SwingBoth);
                    if (Timer == 0)
                        Talk("Both", NPC.spriteDirection);
                    break;
            }

            Timer++;
            // Used to spawn Projectiles to act as melee hitboxes
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (20 < Timer && Timer < 30)
                {
                    if (AiState != SwingRedEyes)
                    {
                        Vector2 pos = redmistSkeleton.BoneName[BoneLabel.FrontWeapon].EndPoint(NPC.spriteDirection);

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<RedMistMelee>(), 25, 2);
                        if (Main.expertMode && Timer == 28)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 target = NPC.GetTargetData().Center;
                                if (i > 0)
                                {
                                    int dir = i == 1 ? -1 : 1;
                                    target.X += Main.rand.Next(180, 240) * dir;
                                    target.Y += Main.rand.Next(-180, 180);
                                }
                                Vector2 speed = (target - pos) / PenitenceStar.TIME;

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, speed * 4, ModContent.ProjectileType<PenitenceStar>(), 15, 2);
                            }
                        }
                    }
                    if (AiState != SwingPenitence)
                    {
                        Vector2 pos = redmistSkeleton.BoneName[BoneLabel.FrontWeapon].EndPoint(NPC.spriteDirection);

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), redmistSkeleton.BoneName[BoneLabel.BackWeapon].EndPoint(NPC.spriteDirection), Vector2.Zero, ModContent.ProjectileType<RedMistMelee>(), 25, 2);

                        if (Main.expertMode && Timer == 28)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 target = NPC.GetTargetData().Center;
                                Vector2 speed = Vector2.Normalize(target - pos) * Main.rand.NextFloat(4f, 8f);
                                if (i > 0)
                                {
                                    speed = speed.RotatedByRandom(0.08f);
                                }

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, speed * 2, ModContent.ProjectileType<RedEyesEgg>(), 15, 2);
                            }
                        }
                    }
                }
            }
            if (Timer >= 50)
            {
                AiState = FollowState;
                Timer = 30;
            }
            if (Timer == 20)
            {
                SoundEngine.PlaySound(LobotomyCorp.WeaponSounds.Mace, NPC.Center);
            }
        }

        void GoldRush1Sequence()
        {
            //GoldRush Start
            if (AiState == GoldRushBig)
            {
                GoldRushCount++;
                NPC.velocity *= 0;
                NPC.spriteDirection *= -1;
                AiState++;
                ChangeAnimation(AnimationState.GoldRushIntro);
                NPC.noGravity = true;
                NPC.noTileCollide = true;
                NPC.netUpdate = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(320 * NPC.spriteDirection, 0), Vector2.Zero, ModContent.ProjectileType<RoadOfGold>(), 0, 0, -1, -1, 5);
                }
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Teleport_Start") with { Volume = 0.25f }, NPC.position);
            }
            //GoldRush Charging
            else if (AiState == 5)
            {
                Timer++;
                if (Timer > 30)
                {
                    Timer = 30 * 5 + 15;
                    AiState++;
                    NPC.velocity.X = 24f * NPC.spriteDirection;
                    NPC.velocity.Y = 0;
                }
            }
            //GoldRush Charge
            else if (AiState == 6)
            {
                NPC.damage = 65;
                Timer--;
                if (Timer <= 0)
                {
                    ChangeAnimation(AnimationState.GoldRushEnd);
                    AiState++;
                    Timer = 0;
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                    redmistSkeleton.BoneName[BoneLabel.FrontWeapon].GetPosition(NPC.direction);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), redmistSkeleton.BoneName[BoneLabel.FrontWeapon].GetPosition(NPC.direction), NPC.velocity, ModContent.ProjectileType<GoldRushRedMistImpact>(), 30, 10);
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Teleport_Finish") with { Volume = 0.25f }, NPC.position);
                    NPC.netUpdate = true;
                }

                Dust d = Dust.NewDustPerfect(new Vector2(Main.rand.NextFloat(NPC.position.X, NPC.position.X + NPC.width), NPC.position.Y + NPC.height), 57);
                d.fadeIn = 1.4f;
                d.noGravity = true;
            }
            //GoldRush End
            else if (AiState == 7)
            {
                NPC.damage = 0;
                NPC.velocity.X *= 0.9f;
                Timer++;
                if (Timer > 120)
                {
                    ChangeAnimation(AnimationState.Idle1);
                    AiState = 0;
                    Timer = 0;
                    NPC.netUpdate = true;
                }
            }

            //Mini GoldRush
            else if (AiState == GoldRushSmall)
            {
                NPC.velocity *= 0;
                AiState++;
                ChangeAnimation(AnimationState.GoldRushIntro);
                NPC.noGravity = true;
                NPC.noTileCollide = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int i = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.GetTargetData().Center + new Vector2(400 * NPC.spriteDirection, Main.LocalPlayer.height / 2 - 50), new Vector2(-22f * NPC.spriteDirection, 0), ModContent.ProjectileType<RoadOfGold>(), 0, 0, -1, -2);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(320 * NPC.spriteDirection, 0), Vector2.Zero, ModContent.ProjectileType<RoadOfGold>(), 0, 0, -1, i);
                }
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Teleport_Start") with { Volume = 0.25f }, NPC.position);
            }
            //Mini GoldRush Charge
            else if (AiState == 9)
            {
                Timer++;
                if (Timer > 30)
                {
                    Timer = 30;
                    AiState = 11;
                    NPC.velocity.X = 24f * NPC.spriteDirection;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), redmistSkeleton.BoneName[BoneLabel.FrontWeapon].GetPosition(NPC.direction), NPC.velocity, ModContent.ProjectileType<GoldRushRedMistImpact>(), 30, 10);
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Teleport_Finish") with { Volume = 0.25f }, NPC.position);
                }
            }
            else if (AiState == 11)
            {
                NPC.damage = 65;
                Timer--;
                if (Timer <= 0)
                {
                    NPC.damage = 0;
                    ChangeAnimation(AnimationState.Idle1);
                    AiState = 0;
                    Timer = 0;
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;

                    NPC.velocity.X *= 0.3f;
                }

                Dust d = Dust.NewDustPerfect(new Vector2(Main.rand.NextFloat(NPC.position.X, NPC.position.X + NPC.width), NPC.position.Y + NPC.height), 57);
                d.fadeIn = 1.4f;
                d.noGravity = true;
            }
        }

        const int SwingMimicry = 1;
        const int SwingDaCapo = 2;
        const int ThrowHeaven = 4;
        const int Dash = 5;
        const int DashSwingMimicry = 6;
        const int SpecialAttackStart = 7;
        const int GoldRushMimicryCombo = 11;

        private void Phase2()
        {
            if (AiState == FollowState)
            {
                FollowMode2();
            }
            else if (AiState == SwingMimicry)
            {
                SwingingMimicry();
            }
            else if (AiState <= SwingDaCapo)
            {
                SwingingDaCapo();
            }
            else if (AiState == SwingDaCapo + 1)//ThrowDaCapo <- Piece of shit why aren't you in a fucking function
            {
                NPC.velocity.X *= 0.9f;
                Vector2 delta = NPC.GetTargetData().Center - NPC.Center;
                Timer++;
                if (Timer == 50)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.ai[3] = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(delta) * 24f, ModContent.ProjectileType<DaCapoThrow>(), 20, 2, -1, NPC.whoAmI);
                    SoundEngine.PlaySound(SoundID.Item19, NPC.Center);
                    NPC.netUpdate = true;
                }
                if (Timer > 100)
                {
                    AiState = FollowState;
                    Timer = 30;
                    ChangeAnimation(AnimationState.Idle2);
                    redmistSkeleton.BoneName[BoneLabel.FrontWeapon].Visible = true;
                }
            }
            else if (AiState == ThrowHeaven)
            {
                ThrowingHeaven();
            }
            else if (AiState == Dash)
            {
                DashFront(FollowState);
            }
            else if (AiState == DashSwingMimicry)
            {
                DashFront(SwingMimicry);
            }

            else if (AiState <= SpecialAttackStart + 2)
            {
                SpecialAttack2();
            }

            //Uses on Aggresion
            else if (AiState >= GoldRushMimicryCombo)
            {
                Phases2MiniGoldRush();
            }
            //Transition Phase
            else if (AiState == SwitchPhase)
            {
                ChangeAnimation(AnimationState.Phase3Transition);
                Timer++;
                if (Timer == 60 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase2_Change") with { Volume = 0.5f }, NPC.position);
                    //Shoot Mimicry forwards and Dacapo backwards
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(6 * NPC.spriteDirection, 0), ModContent.ProjectileType<SwitchMimicryThrow>(), 20, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-6 * NPC.spriteDirection, 0), ModContent.ProjectileType<SwitchDaCapoThrow>(), 20, 0);
                }
                if (Timer == 130)
                {
                    Talk("Shift2", NPC.spriteDirection);
                }
                if (Timer > 160)
                {
                    Phase++;
                    AiState = FollowState;
                    GoldRushCount++;
                    Timer = 0;
                    NPC.netUpdate = true;
                }
            }
        }

        void FollowMode2()
        {
            float healthPercent = NPC.life / (float)NPC.lifeMax;

            if (NPC.velocity.Y == 0)
            {
                if (NPC.velocity.X == 0)
                    ChangeAnimation(AnimationState.Idle2);
                else
                    ChangeAnimation(AnimationState.Walk2);
            }
            else
                ChangeAnimation(AnimationState.MidAir);

            fighterAI(NPC.GetTargetData(), 64f, 0.08f, 5f);
            Vector2 difference = NPC.GetTargetData().Center - NPC.Center;
            float distance = difference.Length();

            if (Timer > 0)
                Timer--;
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (distance < 128f && Main.rand.NextBool(30))
                    {
                        if (Main.rand.NextBool(2))
                        {
                            Talk("Mimicry" + (1 + Main.rand.Next(2)), NPC.spriteDirection);
                            AiState = SwingMimicry;
                            ChangeAnimation(AnimationState.SwingMimicry);
                            NPC.velocity.X = 0;

                            NPC.netUpdate = true;
                        }
                        else if (IsDaCapoHeld)//if DaCapo is held by red mist
                        {
                            Talk("DaCapo" + (1 + Main.rand.Next(2)), NPC.spriteDirection);
                            AiState = SwingDaCapo;
                            ChangeAnimation(AnimationState.SwingDaCapo);
                            NPC.velocity.X = 0;

                            NPC.netUpdate = true;
                        }
                    }

                    //Check if Heaven throw is valid, random
                    HeavenThrowCheck(distance, difference.Y);

                    if ((distance > 2000 && Main.rand.NextBool(360)) || Aggression > 300)
                    {
                        AiState = GoldRushMimicryCombo;
                        Aggression = 0;
                        NPC.netUpdate = true;
                    }
                }
            }

            if (distance > 460)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (distance < 600 && Main.rand.NextBool(200))
                    {
                        AiState = DashSwingMimicry;
                        Timer = 30;
                        NPC.velocity.Y *= 0;
                        NPC.velocity.X = 18 * NPC.spriteDirection;
                        ChangeAnimation(AnimationState.Dash);

                        NPC.netUpdate = true;
                    }
                }

                if (IncomingProjectile())
                {
                    AiState = Dash;
                    Timer = 30;
                    NPC.velocity.Y *= 0;
                    NPC.velocity.X = 18 * NPC.spriteDirection;
                    ChangeAnimation(AnimationState.Dash);

                    NPC.netUpdate = true;
                }
            }

            if (IsDaCapoHeld && CheckGoldRushCounterValid(1))
            {
                AiState = SpecialAttackStart;
                Timer = 0;
                NPC.velocity *= 0;
                if (healthPercent <= .50f)
                {
                    AiState = SwitchPhase;
                    Talk("Shift1", NPC.spriteDirection);
                }
                else
                {
                    GoldRushCount++;
                    Talk("Teleport1", NPC.spriteDirection);
                }
            }
        }

        void SwingingMimicry()
        {
            ChangeAnimation(AnimationState.SwingMimicry);
            NPC.velocity.X *= 0.9f;
            Timer++;
            if (60 < Timer && Timer < 67)
            {
                Vector2 position = redmistSkeleton.BoneName[BoneLabel.FrontWeapon].GetPosition(NPC.spriteDirection) + new Vector2(100, 0).RotatedBy(redmistSkeleton.BoneName[BoneLabel.FrontWeapon].GetRotation(NPC.spriteDirection));

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), position, Vector2.Zero, ModContent.ProjectileType<RedMistMimicry>(), 50, 2);
            }

            if (Timer == 60)
            {
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase2_Atk1") with { Volume = 0.5f }, NPC.position);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && Timer == 64 && Main.expertMode)
            {
                for (int i = 0; i < 16; i++)
                {
                    float angle = MathHelper.ToRadians(-120f + (240f * i / 16f)) + (NPC.spriteDirection > 0 ? 0 : 3.14f);
                    Vector2 vel = new Vector2(100, 0).RotatedBy(angle);
                    Vector2 pos = redmistSkeleton.BoneName[BoneLabel.UpperArmR].GetPosition(NPC.spriteDirection);
                    vel.Normalize();
                    vel *= 8;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, vel, ModContent.ProjectileType<RedMistMimicryHello>(), 30, 2);
                }
            }

            if (Timer > 90)
            {
                AiState = 0;
                Timer = 60;
            }
        }

        void SwingingDaCapo()
        {
            ChangeAnimation(AnimationState.SwingDaCapo);
            Timer++;
            NPC.velocity.X *= 0.9f;
            if (Timer > 20 && Timer < 90 && Timer % 30 < 10)
            {
                Vector2 position = redmistSkeleton.BoneName[BoneLabel.BackWeapon].GetPosition(NPC.spriteDirection) + new Vector2(40, 0).RotatedBy(redmistSkeleton.BoneName[BoneLabel.BackWeapon].GetRotation(NPC.spriteDirection));
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), position, Vector2.Zero, ModContent.ProjectileType<RedMistDaCapo>(), 15, 0);
            }
            if (Timer == 90) //Throw DaCapo when enemy is at certain distance
            {
                Vector2 delta = NPC.GetTargetData().Center - NPC.Center;
                if (delta.Length() > 260)
                {
                    AiState++;
                    ChangeAnimation(AnimationState.DaCapoThrow);
                    Timer = 0;
                }
            }
            if (Timer > 100)
            {
                AiState = FollowState;
                Timer = 30;
            }

            if (Timer == 1 || Timer == 30 || Timer == 60)
            {
                switch (Timer)
                {
                    case 1:
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/LWeapons/silent2_1") with { Volume = 0.25f });
                        break;
                    case 30:
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/LWeapons/silent2_2") with { Volume = 0.25f });
                        break;
                    case 60:
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/LWeapons/silent2_3") with { Volume = 0.25f });
                        break;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && Main.expertMode)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.GetTargetData().Center, Vector2.Zero, ModContent.ProjectileType<RedMistMovement>(), 24, 0);
            }
        }

        void HeavenThrowCheck(float distance, float differenceY)
        {
            if ((distance + Aggression > 600 && Main.rand.NextBool(240)) ||
                    (Main.rand.NextBool(2400)) ||
                    (differenceY < -150 && Main.rand.NextBool(200)))
            {
                AiState = ThrowHeaven;
                Talk("Heaven", NPC.spriteDirection);
                NPC.velocity.X = 0;
                NPC.netUpdate = true;
            }
        }

        void ThrowingHeaven()
        {
            ChangeAnimation(AnimationState.HeavenThrow);
            Timer++;
            NPC.velocity.X *= 0.9f;
            if (Timer == 50)
            {
                Vector2 delta = NPC.GetTargetData().Center - NPC.Center + new Vector2(0, -13);
                if (Math.Sign(delta.X) != Math.Sign(NPC.spriteDirection) || delta.Y > 0)
                {
                    delta = new Vector2(1 * NPC.spriteDirection, 0);
                }
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -13), Vector2.Normalize(delta) * 12f, ModContent.ProjectileType<HeavenBoss>(), 25, 2, -1, 15);
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase2_Spear") with { Volume = 0.5f }, NPC.position);
            }
            if (Timer > 80)
            {
                AiState = FollowState;
                Timer = 30;
                NPC.netUpdate = true;
            }
        }

        void DashFront(int ToAiState)
        {
            ChangeAnimation(AnimationState.Dash);
            Timer--;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            if (Timer < 10)
            {
                NPC.velocity.X *= 0.9f;
            }
            if (Timer <= 0)
            {
                AiState = ToAiState;

                NPC.noTileCollide = false;
                NPC.noGravity = false;
            }
        }

        bool IsDaCapoHeld
        {
            get
            {
                // Edge case if Da Capo despawns, so the fight can move forwards
                if (NPC.ai[3] >= 0)
                {
                    Projectile p = Main.projectile[(int)NPC.ai[3]];
                    if (!p.active || p.type != ModContent.ProjectileType<DaCapoThrow>() || (int)p.ai[0] != NPC.whoAmI)
                        NPC.ai[3] = -1;
                }
                return NPC.ai[3] < 0;
            }
        }

        void SpecialAttack2()
        {
            if (AiState == SpecialAttackStart)
            {
                ChangeAnimation(AnimationState.DaCapoThrow);
                NPC.velocity.X *= 0.9f;
                Vector2 delta = NPC.GetTargetData().Center - NPC.Center;
                Timer++;
                if (Timer == 50 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[3] = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, delta /= 30, ModContent.ProjectileType<DaCapoLegato>(), 20, 2, -1, NPC.whoAmI);
                    NPC.netUpdate = true;
                }
                if (Timer > 100)
                {
                    AiState = 8;
                    Timer = 0;
                    redmistSkeleton.BoneName[BoneLabel.FrontWeapon].Visible = true;
                    Talk("Teleport2", NPC.spriteDirection);
                }
            }
            else if (AiState == SpecialAttackStart + 1)
            {
                ChangeAnimation(AnimationState.MimicryPrepare);
                if (Timer == 0)
                {
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase2_Cast") with { Volume = 0.5f }, NPC.position);
                }
                Timer++;
                if (Timer >= 120)
                {
                    AiState = 9;
                    Timer = 0;
                    Projectile p = Main.projectile[(int)NPC.ai[3]];
                    if (p.active && p.type == ModContent.ProjectileType<DaCapoLegato>())
                    {
                        NPC.Center = p.Center;// + new Vector2(200, 0).RotateRandom(6.28f);
                        p.Kill();
                    }
                    NPC.ai[3] = -1;
                    NPC.netUpdate = true;
                }
            }
            else if (AiState == SpecialAttackStart + 2)
            {
                if (Timer == 0)
                {
                    Talk("Teleport3", NPC.spriteDirection);
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase2_CastAtk") with { Volume = 0.5f }, NPC.position);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 32; i++)
                        {
                            float angle = 6.28f / 32;

                            Vector2 vel = new Vector2(8, 0).RotatedBy(angle * i);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), redmistSkeleton.BoneName[BoneLabel.FrontWeapon].GetPosition(NPC.spriteDirection) + vel * 3, vel, ModContent.ProjectileType<RedMistMimicryHello>(), 30, 2);
                        }
                    }
                }

                NPC.velocity *= 0;
                NPC.noGravity = true;
                ChangeAnimation(AnimationState.MimicryGreaterSplitV);

                Timer++;
                if (Timer >= 60)
                {
                    NPC.noGravity = false;
                    AiState = 0;
                    Timer = 0;
                    NPC.velocity.Y = -8f;
                    NPC.netUpdate = true;
                }
            }
        }

        void Phases2MiniGoldRush()
        {
            if (AiState == GoldRushMimicryCombo)
            {
                NPC.velocity *= 0;
                AiState++;
                Timer = 0;
                ChangeAnimation(AnimationState.GoldRushIntro);
                NPC.noGravity = true;
                NPC.noTileCollide = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int i = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.GetTargetData().Center + new Vector2(400 * NPC.spriteDirection, Main.LocalPlayer.height / 2 - 50), new Vector2(-22f * NPC.spriteDirection, 0), ModContent.ProjectileType<RoadOfGold>(), 0, 0, -1, -2);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(320 * NPC.spriteDirection, 0), Vector2.Zero, ModContent.ProjectileType<RoadOfGold>(), 0, 0, -1, i);
                }
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Teleport_Start") with { Volume = 0.25f }, NPC.position);
            }
            //Mini GoldRush Charge
            else if (AiState == GoldRushMimicryCombo + 1)
            {
                Timer++;
                if (Timer > 30)
                {
                    Timer = 30;
                    AiState++;
                    NPC.velocity.X = 24f * NPC.spriteDirection;
                }
            }
            else if (AiState == GoldRushMimicryCombo + 2)
            {
                NPC.damage = 65;
                Timer--;
                if (Timer <= 0)
                {
                    NPC.damage = 0;
                    AiState = SwingMimicry;
                    Timer = 0;
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;

                    NPC.velocity.X *= 0.3f;
                }
                NPC.netUpdate = true;
                Dust d = Dust.NewDustPerfect(new Vector2(Main.rand.NextFloat(NPC.position.X, NPC.position.X + NPC.width), NPC.position.Y + NPC.height), 57);
                d.fadeIn = 1.4f;
                d.noGravity = true;
            }
        }

        const int SwingSmile = 4;

        private void Phase3()
        {
            if (AiState == FollowState)
            {
                FollowMode3();
            }
            //Justitia Slashes
            else if (AiState > 0 && AiState <= 3)
            {
                ChangeAnimation(AnimationState.JustitiaSwing);
                Timer++;
                if (Timer > 30 && Timer % 45 == 0)
                {
                    //Shoot Justitia Slashes
                    Vector2 delta = NPC.GetTargetData().Center - NPC.Center;
                    delta.Normalize();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, 16 * delta, ModContent.ProjectileType<JustitiaSlashBoss>(), 15, 0);
                    if (AiState > 1)
                        AiState--;
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase3_Atk3") with { Volume = 0.5f }, NPC.position);

                }
                if (AiState == 1)
                {
                    if (Timer % 90 >= 59)
                    {
                        AiState = 0;
                        Timer = 100 * (int)(Timer / 60);
                    }
                }
            }
            else if (AiState == SwingSmile)
            {
                ChangeAnimation(AnimationState.SmileSwing);
                Timer++;
                if (Timer == 70)
                {
                    SmileImpact();
                }
                else if (Timer >= 160 && Timer < 240)
                {
                    SmileScream();
                }

                if (Timer > 270)
                {
                    AiState = 0;
                    Timer = 180;
                }
            }
            //Heaven Throw
            else if (AiState == ThrowHeaven + 1)
            {
                ThrowingHeaven();
            }
            //GoldRush Start
            else if (AiState == 6)
            {
                //GoldRushCount++;
                NPC.velocity *= 0;
                NPC.spriteDirection *= -1;
                AiState++;
                ChangeAnimation(AnimationState.GoldRushIntro);
                NPC.noGravity = true;
                NPC.noTileCollide = true;
                //Spawn Portals MANUALLY AAAAAA
                int portals = 8;
                int currentPortal = -1;
                float currentRotation = 0;
                //Doesnt work because of portal intersecting :(
                Vector2 targetPos = NPC.Center;
                /*
                for (int i = 0; i < portals; i++)
                {
                    if (currentPortal == -1)
                    {
                        currentRotation = Main.rand.NextFloat((float)Math.PI * 2);
                        currentPortal = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.GetTargetData().Center + new Vector2(400, 0).RotatedBy(currentRotation), new Vector2(-22, 0).RotatedBy(currentRotation), ModContent.ProjectileType<GoldRushPortal>(), 0, 0, 0, -1, 5);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), targetPos + new Vector2(320 * NPC.spriteDirection, 0), Vector2.Zero, ModContent.ProjectileType<GoldRushPortal>(), 0, 0, 0, currentPortal, 5);
                        continue;
                    }
                    if (i == portals - 1)
                    {
                        int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.GetTargetData().Center + new Vector2(-400, 0).RotatedBy(currentRotation), new Vector2(-22, 0).RotatedBy(currentRotation), ModContent.ProjectileType<GoldRushPortal>(), 0, 0, 0, -2, 5);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.GetTargetData().Center + new Vector2(0, -320), new Vector2(0, -22), ModContent.ProjectileType<GoldRushPortal>(), 0, 0, 0, -2, 5);
                        Main.Projectile[p].ai[0] = currentPortal;
                        Main.Projectile[p].netUpdate = true;
                    }
                    else
                    {
                        int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.GetTargetData().Center + new Vector2(-400, 0).RotatedBy(currentRotation), new Vector2(-22, 0).RotatedBy(currentRotation), ModContent.ProjectileType<GoldRushPortal>(), 0, 0, 0, -2, 5);
                        currentRotation = Main.rand.NextFloat((float)Math.PI * 2);
                        currentPortal = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.GetTargetData().Center + new Vector2(400, 0).RotatedBy(currentRotation), new Vector2(-22, 0).RotatedBy(currentRotation), ModContent.ProjectileType<GoldRushPortal>(), 0, 0, 0, currentPortal, 5);
                        Main.Projectile[p].ai[0] = currentPortal;
                        Main.Projectile[p].netUpdate = true;
                    }
                }*/
                //One at a time version
                /*
                Also Kinda bad
                currentRotation = Main.rand.NextFloat((float)Math.PI * 2);
                currentPortal = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.GetTargetData().Center + new Vector2(320, 0).RotatedBy(currentRotation), new Vector2(-22, 0).RotatedBy(currentRotation), ModContent.ProjectileType<GoldRushPortal>(), 0, 0, 0, currentPortal, 5);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), targetPos + new Vector2(320 * NPC.spriteDirection, 0), Vector2.Zero, ModContent.ProjectileType<GoldRushPortal>(), 0, 0, 0, currentPortal, 5);
                */
                //Default
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(320 * NPC.spriteDirection, 0), Vector2.Zero, ModContent.ProjectileType<RoadOfGold>(), 0, 0, -1, -3, 8);
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Teleport_Start") with { Volume = 0.5f }, NPC.position);
            }
            //GoldRush Charging
            else if (AiState == 7)
            {
                Timer++;
                if (Timer > 30)
                {
                    Timer = 30 * 8;// + 15;
                    AiState++;
                    NPC.velocity.X = 24f * NPC.spriteDirection;
                    NPC.velocity.Y = 0;
                }
            }
            //GoldRush Charge
            else if (AiState == 8)
            {
                NPC.damage = 65;
                Timer--;
                if (Timer <= 0)
                {
                    ChangeAnimation(AnimationState.GoldRushEnd);
                    AiState++;
                    Timer = 0;
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), redmistSkeleton.BoneName[BoneLabel.FrontWeapon].GetPosition(NPC.direction), NPC.velocity, ModContent.ProjectileType<GoldRushRedMistImpact>(), 30, 10);
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Teleport_Finish") with { Volume = 0.5f }, NPC.position);
                }
                /*
                if (Timer % 45 == 0)
                {
                    float currentRotation = Main.rand.NextFloat((float)Math.PI * 2);
                    int currentPortal = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.GetTargetData().Center + new Vector2(600, 0).RotatedBy(currentRotation), new Vector2(-22, 0).RotatedBy(currentRotation), ModContent.ProjectileType<GoldRushPortal>(), 0, 0, 0, -1, 5);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(400, 0).RotatedBy(NPC.velocity.ToRotation()), new Vector2(-22, 0).RotatedBy(NPC.velocity.ToRotation()), ModContent.ProjectileType<GoldRushPortal>(), 0, 0, 0, currentPortal, 5);
                }*/

                Dust d = Dust.NewDustPerfect(new Vector2(Main.rand.NextFloat(NPC.position.X, NPC.position.X + NPC.width), NPC.position.Y + NPC.height), 57);
                d.fadeIn = 1.4f;
                d.noGravity = true;
            }
            //GoldRush End
            else if (AiState == 9)
            {
                NPC.damage = 0;
                NPC.velocity.X *= 0.9f;
                Timer++;
                if (Timer > 120)
                {
                    ChangeAnimation(AnimationState.Idle1);
                    AiState = 0;
                    Timer = 0;
                    NPC.netUpdate = true;
                }
            }

            //Phase4Transition
            else if (AiState == SwitchPhase)
            {
                ChangeAnimation(AnimationState.Phase4Transition);
                NPC.velocity.X = 0;
                Timer++;
                if (Timer == 120)
                {
                    Talk("Shift4", NPC.spriteDirection);
                }
                if (Timer > 160)
                {
                    NPC.ai[0] = 3;
                    AiState = 0;// -60;
                    Timer = 0;
                    NPC.netUpdate = true;
                }
                if (Timer == 30)
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase3_Change") with { Volume = 0.5f }, NPC.position);
                else if (Timer == 120)
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase3_Change_justice") with { Volume = 0.5f }, NPC.position);
            }
        }

        void FollowMode3()
        {
            float healthPercent = NPC.life / (float)NPC.lifeMax;

            if (NPC.velocity.Y == 0)
            {
                if (NPC.velocity.X == 0)
                    ChangeAnimation(AnimationState.Idle3);
                else
                    ChangeAnimation(AnimationState.Walk3);
            }
            else
                ChangeAnimation(AnimationState.MidAir);

            fighterAI(NPC.GetTargetData(), 90, 0.08f, 2f);

            Vector2 delta = NPC.GetTargetData().Center - NPC.Center;

            if (Timer > 0)
                Timer--;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Timer <= 0 && Main.rand.NextBool(60))
                {
                    if (Main.rand.NextBool(2))
                    {
                        NPC.velocity.X *= 0;
                        //Justitia Attack, up to three just to catch people offguard
                        //Up to three is removed, shits kinda wack to dodge anyawy
                        AiState = 1;
                        Timer = 0;
                        ChangeAnimation(AnimationState.JustitiaSwing);

                        Talk("Justitia" + Main.rand.Next(1, 3), NPC.spriteDirection);
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        //Smile Attack
                        NPC.velocity.X *= 0;
                        AiState = 4;
                        Timer = 0;
                        ChangeAnimation(AnimationState.SmileSwing);
                        Talk("Smile" + Main.rand.Next(1, 3), NPC.spriteDirection);
                        NPC.netUpdate = true;
                    }

                    //Heaven Attack
                    HeavenThrowCheck(delta.Length(), delta.Y);
                }
            }

            //Gold Rush!
            if (CheckGoldRushCounterValid(2))
            {
                Timer = 0;
                GoldRushCount++;
                if (healthPercent <= .25f)
                {
                    AiState = SwitchPhase;
                    GoldRushCount++;
                    Talk("Shift3", NPC.spriteDirection);
                    ChangeAnimation(AnimationState.Phase4Transition);
                    return;
                }
                //Main.NewText(healthPercent);
                AiState = 6;//Gold Rush!
                NPC.velocity *= 0;
                Talk("GoldRush1", NPC.spriteDirection);
                ChangeAnimation(AnimationState.GoldRushIntro);
            }
        }

        void SmileImpact()
        {
            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase3_Atk1") with { Volume = 0.5f }, NPC.position);

            Vector2 hammerPos = redmistSkeleton.BoneName[BoneLabel.BackWeapon].GetPosition(NPC.spriteDirection) + new Vector2(70, 0).RotatedBy(redmistSkeleton.BoneName[BoneLabel.BackWeapon].GetRotation(NPC.spriteDirection));
            foreach (Player p in Main.ActivePlayers)
            {
                if (!p.dead && Collision.CanHit(hammerPos, 1, 1, p.position, p.width, p.height))
                {
                    p.AddBuff(ModContent.BuffType<Buffs.Scream>(), 300);
                    p.velocity.Y = 6;
                }
            }

            for (int i = 0; i < 24; i++)
            {
                Vector2 vel = new Vector2(Main.rand.NextFloat(10f, 20f), 0).RotatedBy(Main.rand.NextFloat(0.34f) + MathHelper.ToRadians(-90 + 45 * NPC.spriteDirection));
                Dust d = Dust.NewDustPerfect(hammerPos, DustID.Wraith, vel);
                d.noGravity = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && Main.expertMode)
            {
                int amount = 1 + Main.rand.Next(2);
                int corpseType = ModContent.NPCType<SmileCorpses>();
                int limit = 10;
                int amountAlive = NPC.CountNPCS(corpseType);
                if (amountAlive + amount > limit)
                {
                    amount = limit - amountAlive;
                }
                if (amount > 0)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        NPC.NewNPC(NPC.GetSource_FromThis(), (int)hammerPos.X, (int)hammerPos.Y, corpseType);
                    }
                }
            }
        }

        void SmileScream()
        {
            if (Timer == 160)
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase3_Atk3") with { Volume = 0.5f }, NPC.position);

            Vector2 hammerPos = redmistSkeleton.BoneName[BoneLabel.BackWeapon].GetPosition(NPC.spriteDirection) + new Vector2(70, 0).RotatedBy(redmistSkeleton.BoneName[BoneLabel.BackWeapon].GetRotation(NPC.spriteDirection));
            /*if (Timer % 10 == 0)
            {
                foreach (Player p in Main.player)
                {
                    if (p.active && !p.dead && Collision.CanHit(hammerPos, 1, 1, p.position, p.width, p.height))
                        p.AddBuff(ModContent.BuffType<Buffs.Vomit>(), 60);
                }
            }*/

            if (Timer % 5 == 0)
            {
                float random = Main.rand.NextFloat(1.00f);
                for (int i = 0; i < 28; i++)
                {
                    Vector2 vel = new Vector2(16, 0).RotatedBy(random + MathHelper.ToRadians(11.25f * i));
                    Vector2 dustPos = new Vector2(70, 0).RotatedBy(redmistSkeleton.BoneName[BoneLabel.BackWeapon].GetRotation(NPC.spriteDirection));
                    Dust d = Dust.NewDustPerfect(hammerPos, DustID.Wraith, vel);
                    d.noGravity = true;
                }
                int dir = (int)(Timer % 40) / 5;
                for (int i = -2; i <= 2; i++)
                {
                    float angle = MathHelper.ToRadians(dir * 45f + i * (45f / 5f));
                    Vector2 vel = new Vector2(8, 0).RotatedBy(angle);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (Main.rand.NextBool(3))
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), hammerPos, vel, ModContent.ProjectileType<SmileBits>(), 2, 0, -1, Main.rand.NextFloat(0.26f));
                        else
                        {
                            int n = NPC.NewNPC(NPC.GetSource_FromThis(), (int)hammerPos.X, (int)hammerPos.Y, ModContent.NPCType<SmileBitsBreakable>(), 0, Main.rand.NextFloat(0.26f));
                            Main.npc[n].velocity = vel;
                            Main.npc[n].netUpdate = true;
                        }
                    }
                }
            }
        }

        public void TeleportP4(int Entrance, int Exit, Vector2 position, Vector2 velocity)
        {
            NPC.Center = position;
            NPC.velocity = velocity;
            Timer = 0;
            bool isExitLast = Main.projectile[Exit].ai[0] == -4;
            if (isExitLast)
            {
                AiState = 3;
            }
            NPC.netUpdate = true;
        }

        void GoldRushReposition()
        {

        }

        const int IdleState = 0;
        const int GoldRushFollowStart = 1;
        const int GoldRushFollow = 2;
        const int TwilightJumpslash = 3;
        const int RedMistExhaust = 4;
        const int TwilightDashslash = 5;
        const int TwilightNormal1 = 6;
        const int TwilightNormal2 = 7;
        const int TwilightDarknessStart = 8;
        const int TwilightTeleport = 9;
        const int TwilightRun = 10;
        const int TwilightRunDash = 11;
        const int TwilightLampstrike = 12;
        const int TwilightCounter = 13;

        private void Phase4()
        {
            if (AiState < 0)
            {
                AiState++;
            }
            else
            {
                //Idle
                if (AiState == IdleState)
                {
                    ChangeAnimation(AnimationState.Idle4);
                    Timer++;
                    if (Timer > 60 && NPC.velocity.Y == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.TargetClosest();
                        Timer = 0;
                        int atkMax = 8;
                        if (Main.expertMode)
                            atkMax += 2;
                        int attack = Main.rand.Next(atkMax);
                        if (attack < 4)
                        {
                            AiState = GoldRushFollowStart;
                        }
                        else if (attack < 8)
                        {
                            AiState = TwilightRun;
                        }
                        else
                        {
                            AiState = TwilightLampstrike;
                        }
                        NPC.netUpdate = true;
                    }

                    if (((NPC.GetTargetData().Center - NPC.Center).Length() > 2000f && Main.rand.NextBool(360)) || Aggression > 300 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        AiState = TwilightTeleport;
                        Aggression = 0;
                        NPC.netUpdate = true;
                    }
                }

                GoldRush4Sequence();

                //Twilight JumpSlash Finisher
                if (AiState == TwilightJumpslash)
                {
                    TwilightJumpslashFinisher();
                }

                //KNEEL
                else if (AiState == RedMistExhaust)
                {
                    NPC.noTileCollide = false;
                    NPC.noGravity = false;

                    if (Timer % 30 == 0)
                    {
                        Talk("Tired" + (Main.rand.Next(5) + 1), Timer % 60 == 30 ? 1 : -1);
                    }

                    ChangeAnimation(AnimationState.TwilightEnd);
                    Timer++;
                    if (Timer > 120)
                    {
                        Timer = 0;
                        AiState = 0;
                    }
                }

                //Twilight DashSlash
                else if (AiState == TwilightDashslash)
                {
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;
                    NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;
                    ChangeAnimation(AnimationState.TwilightDashSlash);
                    Timer++;

                    if (Timer % 3 == 0)
                    {
                        Vector2 SlashPosition = NPC.Center + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-100, 101));
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), SlashPosition, Vector2.Zero, ModContent.ProjectileType<RedMistSlashes>(), 43, 0);
                    }

                    if (Timer > 20)
                    {
                        AiState = 2;
                    }
                }

                //Twilight Normal Attack
                else if (AiState == TwilightNormal1)
                {
                    if (Timer == 0)
                    {
                        NPC.TargetClosest(true);
                        NPC.spriteDirection = NPC.direction;
                    }

                    ChangeAnimation(AnimationState.TwilightDashSlash);
                    Timer++;

                    if (Timer == 10)
                    {
                        Vector2 velocity = (NPC.GetTargetData().Center - NPC.Center);
                        velocity.Normalize();
                        velocity *= 32;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<RedMistSlashSpawner>(), 43, 0);
                    }

                    if (Timer > 45)
                    {
                        Timer = 0;
                        AiState = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(3))
                        {
                            AiState = 7;
                            NPC.netUpdate = true;
                        }
                    }
                }

                //Twilight Normal Attack 2
                else if (AiState == TwilightNormal2)
                {
                    if (Timer == 0)
                    {
                        NPC.TargetClosest(true);
                        NPC.spriteDirection = NPC.direction;
                    }

                    ChangeAnimation(AnimationState.TwilightDashSlash2);
                    Timer++;

                    if (Timer == 10)
                    {
                        Vector2 velocity = (NPC.GetTargetData().Center - NPC.Center);
                        velocity.Normalize();
                        velocity *= 32;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<RedMistSlashSpawner>(), 43, 0);
                    }

                    if (Timer > 45)
                    {
                        Timer = 0;
                        AiState = 0;
                        if (Main.rand.NextBool(3) && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            AiState = 3;
                            NPC.netUpdate = true;
                        }
                    }
                }

                else if (AiState == TwilightRun)
                {
                    ChangeAnimation(AnimationState.TwilightChase);
                    NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;

                    float speed = 0.2f;
                    float maxSpeed = 14f;

                    Vector2 pos = NPC.GetTargetData().Center - NPC.Center;
                    Vector2 NormPos = pos;
                    NormPos.Normalize();

                    if (NPC.velocity.X < NormPos.X * maxSpeed)
                    {
                        NPC.velocity.X += speed;
                        if (NPC.velocity.X < 0)
                            NPC.velocity.X += speed * 4;
                        if (NPC.velocity.X > maxSpeed)
                            NPC.velocity.X = maxSpeed;
                    }
                    else if (NPC.velocity.X > NormPos.X * maxSpeed)
                    {
                        NPC.velocity.X -= speed;
                        if (NPC.velocity.X > 0)
                            NPC.velocity.X -= speed * 4;
                        if (NPC.velocity.X < -maxSpeed)
                            NPC.velocity.X = -maxSpeed;
                    }

                    if (NPC.velocity.Y < NormPos.Y * maxSpeed)
                    {
                        NPC.velocity.Y += speed;
                        if (NPC.velocity.Y < 0)
                            NPC.velocity.Y += speed * 4;
                        if (NPC.velocity.Y > maxSpeed)
                            NPC.velocity.Y = maxSpeed;
                    }
                    else if (NPC.velocity.Y > NormPos.Y * maxSpeed)
                    {
                        NPC.velocity.Y -= speed;
                        if (NPC.velocity.Y > 0)
                            NPC.velocity.Y -= speed * 4;
                        if (NPC.velocity.Y < -maxSpeed)
                            NPC.velocity.Y = -maxSpeed;
                    }
                    /*
                    if (NPC.velocity.Length() < maxSpeed * 0.1f)
                    {
                        NPC.velocity = pos * maxSpeed * 0.15f;
                    }
                    else
                    {
                        pos *= speed;
                        NPC.velocity.X += pos.X;
                        NPC.velocity.Y += pos.Y;
                    }

                    if (NPC.velocity.Length() > maxSpeed)
                    {
                        NPC.velocity.Normalize();
                        NPC.velocity *= maxSpeed;
                    }*/

                    for (int i = 0; i < 3; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = Vector2.Normalize(NPC.velocity) * 1.5f;
                    }

                    float dist = 16 * 25;
                    if (Timer > 30 && pos.Length() < dist)
                    {
                        NPC.velocity = NormPos * 28;

                        if (TwilightRushTime < 600)
                        {
                            Talk("Pass" + (1 + Main.rand.Next(3)), NPC.spriteDirection);
                            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase4_Atk1") with { Volume = 0.5f }, NPC.position);

                            AiState = TwilightRunDash;
                            TwilightRushTime += 60;
                            Timer = 0;
                        }
                        else
                        {
                            AiState = TwilightJumpslash;
                            TwilightRushTime = 0;
                            Timer = 0;
                        }
                    }

                    Timer++;
                    TwilightRushTime++;
                }

                else if (AiState == TwilightRunDash)
                {
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;
                    NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                    ChangeAnimation(AnimationState.TwilightDashSlash);

                    Timer++;
                    if (Timer < 20)
                    {
                        if (Timer % 3 == 0)
                        {
                            Vector2 SlashPosition = NPC.Center + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 11));
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), SlashPosition, Vector2.Zero, ModContent.ProjectileType<RedMistSlashes>(), 43, 0);
                        }
                    }
                    else if (Timer > 30 && Main.expertMode)
                    {
                        ChangeAnimation(AnimationState.TwilightDashSlash2);
                        if (Timer % 2 == 0)
                        {
                            spawnTwilightLampProjectiles();
                        }
                    }

                    if (Timer > 40)
                    {
                        AiState = TwilightRun;
                        Timer = 0;
                    }
                    else if (Timer > 30)
                    {
                        NPC.velocity *= 0.95f;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 0;
                    }
                }

                else if (AiState == TwilightLampstrike)
                {
                    TwilightLampAttack();
                }

                else if (AiState == TwilightTeleport)
                {
                    TwilightApocalypseTeleport(IdleState);
                }
                /*
                if (Timer == 0)
                {
                    NPC.velocity.X = 4f;
                }
                else
                    NPC.velocity.X *= 0.9f;
                */
                /*
                if (NPC.velocity.Y == 0)
                {
                    if (NPC.velocity.X == 0)
                        ChangeAnimation(AnimationState.Idle4);
                    else
                        ChangeAnimation(AnimationState.TwilightChase);
                }
                else
                    ChangeAnimation(AnimationState.MidAir);

                fighterAI(NPC.GetTargetData(), 16f, 0.8f, 10f);
                */
            }
        }

        void GoldRush4Sequence()
        {
            //GoldRush Follow Start
            if (AiState == GoldRushFollowStart)
            {
                NPC.velocity.X = 0;

                ChangeAnimation(AnimationState.GoldRushThrow);
                if (Timer == 0)
                {
                    Talk("GoldRush" + Main.rand.Next(2, 4), NPC.spriteDirection);

                    Vector2 velocity = new Vector2(GOLDRUSH4SPEED * NPC.spriteDirection, 0);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + velocity * GOLDRUSH4DELAY, velocity, ModContent.ProjectileType<RoadOfKing>(), 0, 0, -1, -1, 170 - GOLDRUSH4DELAY);

                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Teleport_Start") with { Volume = 0.5f }, NPC.position);
                }

                Timer++;

                if (Timer == 160)
                {
                    Vector2 velocity = new Vector2(GOLDRUSH4SPEED * NPC.spriteDirection, 0);
                    //if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<GoldRushRedMist>(), 45, 1f, -1, 10);

                    NPC.noTileCollide = true;
                    NPC.noGravity = true;
                }

                if (Timer > 200)
                {
                    Timer = 0;
                    AiState++;
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;
                    NPC.velocity = new Vector2(GOLDRUSH4SPEED * NPC.spriteDirection, 0);
                }
            }

            //GoldRush Follow
            else if (AiState == GoldRushFollow)
            {
                NPC.noTileCollide = true;
                NPC.noGravity = true;
                NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;
                Timer++;

                if (NPC.velocity.LengthSquared() < GOLDRUSH4SPEED * GOLDRUSH4SPEED - 1)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= GOLDRUSH4SPEED;
                }

                ChangeAnimation(AnimationState.TwilightChase);

                NPC.TargetClosest();
                Rectangle targetRect = new Rectangle((int)NPC.GetTargetData().Position.X, (int)NPC.GetTargetData().Position.Y, NPC.GetTargetData().Width, NPC.GetTargetData().Height);

                if (WillHitTarget(targetRect, 10))//Collision.CheckAABBvLineCollision(NPC.GetTargetData().Position, NPC.GetTargetData().Size, NPC.Center, NPC.Center + NPC.velocity * 8))
                {
                    Talk("Pass" + (1 + Main.rand.Next(3)), NPC.spriteDirection);
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase4_Atk1") with { Volume = 0.5f }, NPC.position);

                    AiState = 5;
                    Timer = 0;
                }

                if (Timer > 300)
                {
                    Timer = 0;
                    AiState = 4;
                }

                Dust d = Dust.NewDustPerfect(new Vector2(Main.rand.NextFloat(NPC.position.X, NPC.position.X + NPC.width), NPC.position.Y + NPC.height), 57);
                d.fadeIn = 1.4f;
                d.noGravity = true;
            }
        }

        void TwilightJumpslashFinisher()
        {
            ChangeAnimation(AnimationState.TwilightFinisher);
            NPC.velocity *= 0.95f;
            if (NPC.velocity.X > -1 && NPC.velocity.X < 1)
                NPC.velocity.X = 0;
            Timer++;

            if (Timer == 25)
            {
                Talk("Arrive", NPC.spriteDirection);

                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase4_CastAtk") with { Volume = 0.5f }, NPC.position);
            }

            if (Timer > 20 && Timer < 60)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 SlashPosition = NPC.Center + new Vector2(Main.rand.Next(-260, 261), Main.rand.Next(-10, 51));
                    //Projectile.NewProjectile(NPC.GetSource_FromThis(), SlashPosition, Vector2.Zero, ModContent.ProjectileType<Projectiles.RedMistSlashes>(), 43, 0);

                    float angle = Main.rand.NextFloat(6.28f);
                    Vector2 velocity = new Vector2(16f, 0f).RotatedBy(angle) * Main.rand.NextFloat(0.5f, 1f);
                    Vector2 position = -velocity * 15;
                    int offsetX = 5;
                    int offsetY = 5;

                    position += SlashPosition;

                    position.X += Main.rand.Next(-offsetX, offsetX);
                    position.Y += Main.rand.Next(-offsetY, offsetY);
                    int type = ModContent.ProjectileType<RedMistStrikes>();
                    if (Main.rand.NextBool(5))
                    {
                        position += velocity * 15;
                        velocity *= 0;
                        type = ModContent.ProjectileType<RedMistSlashes>();
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), position, velocity, type, 43, 0);
                }
            }

            if (Timer > 100)
            {
                Timer = 0;
                AiState++;
            }
        }

        void TwilightRedMistResting()
        {

        }

        void TwilightLampAttack()
        {
            ChangeAnimation(AnimationState.TwilightLamp);
            if (Timer == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/ApocalypseBird/BossBird_laser_Cast_Short") with { Volume = 0.5f }, NPC.position);
            }
            Timer++;

            //Spawns Lamp Projectile
            if (Timer >= 60 && Timer <= 70)
            {
                spawnTwilightLampProjectiles();
            }

            if (Timer == 90)
            {
                AiState = IdleState;
                Timer = 0;
            }
        }

        void spawnTwilightLampProjectiles()
        {
            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/ApocalypseBird/BossBird_laser_Fire") with { Volume = 0.2f }, NPC.position);

            Vector2 pos = redmistSkeleton.BoneName[BoneLabel.FrontWeapon].GetPosition(NPC.spriteDirection) + new Vector2(40 + Main.rand.Next(90), 0).RotatedBy(redmistSkeleton.BoneName[BoneLabel.FrontWeapon].GetRotation(NPC.spriteDirection));
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, new Vector2(16, 0).RotatedByRandom(6.28f), ModContent.ProjectileType<RedMistLampProjectile>(), 10, 0, -1, NPC.target);
            }
        }

        /// <summary>
        /// Teleport Uses Timer between 0 and 30, Use Timer after 30 or required State
        /// </summary>
        /// <param name="StateAfter"></param>
        /// <param name="Target"></param>
        void TwilightApocalypseTeleport(int StateAfter)
        {
            ChangeAnimation(AnimationState.Idle4);

            if (NPC.velocity.Y == 0)
                Timer++;
            // Create Teleport Effect
            if (Timer == 1)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TwilightTeleport>(), 0, 0, -1, 0);
                }
            }

            // Create Teleport Effect at destination, teleport to location
            if (Timer == 30)
            {
                Vector2 targetPos = NPC.Center;

                Vector2 playerPos = Main.player[NPC.target].position;
                int targetX = (int)playerPos.X / 16;
                int targetY = (int)playerPos.Y / 16;

                int npcX = (int)NPC.position.X / 16;
                int npcY = (int)NPC.position.Y / 16;

                int range = 20;
                int attempt = 0;
                bool playerFar = false;
                /*if ((double)Math.Abs(NPC.position.X - playerPos.X) + (double)Math.Abs(NPC.position.Y - playerPos.Y) > 2000)
                {
                    attempt = 100;
                    playerFar = true;
                }*/
                while (!playerFar && attempt < 100)
                {
                    attempt++;
                    int x = Main.rand.Next(targetX - range, targetX + range);
                    for (int y = Main.rand.Next(targetY - range, targetY + range); y < targetY + range; ++y)
                    {
                        if ((y < targetY - 4 || y > targetY + 4 || (x < targetX - 4 || x > targetX + 4)) && (y < npcY - 1 || y > npcY + 1 || (x < npcX - 1 || x > npcX + 1)) && Main.tile[x, y].HasUnactuatedTile)
                        {
                            bool validTile = true;
                            if ((Main.tile[x, y - 1].LiquidType == LiquidID.Lava))
                                validTile = false;
                            if (validTile && Main.tileSolid[(int)Main.tile[x, y].TileType] && !Collision.SolidTiles(x - 3, x + 3, y - 5, y - 1))
                            {
                                targetPos.X = (float)(x * 16 - NPC.width / 2);
                                targetPos.Y = (float)(y * 16 - NPC.height);
                            }
                        }
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), targetPos + new Vector2(0, NPC.height / 2), Vector2.Zero, ModContent.ProjectileType<TwilightTeleport>(), 0, 0, -1, 1);
                }
                NPC.position = targetPos;
                AiState = StateAfter;
                Timer = 0;
            }
        }

        private void DeathPhase()
        {
            //Activate death animation
            animState = AnimationState.TwilightEnd;
            Timer++;
            NPC.velocity.X *= 0.8f;
            if (Timer > 180)
            {
                if (Main.netMode == NetmodeID.Server)
                {
                    NPC.HitInfo hit = new NPC.HitInfo();
                    hit.InstantKill = true;
                    NPC.StrikeNPC(hit, false, true);
                }
                else if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.StrikeInstantKill();
                    NPC.netUpdate = true;
                }
            }
        }

        /// <summary>
        /// Returns False if the previous held item is not the same, Dust makes it spawn dust associated with the weapon
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dust"></param>
        /// <returns></returns>
        bool ChangeFrontWeapon(int type, int visibleTimer = 0, bool dust = true)
        {
            bool changed = false;
            if (FrontWeapon != type)
            {
                if (dust)
                {
                    int oldType = FrontWeapon;
                    spawnWeaponDust(oldType);
                }
                changed = true;
                //Main.NewText("Changed");
                if (visibleTimer != 0)
                {
                    redmistSkeleton.BoneName[BoneLabel.FrontWeapon].Visible = false;
                }
            }
            else if (!redmistSkeleton.BoneName[BoneLabel.FrontWeapon].Visible && Timer == visibleTimer)
            {
                if (dust)
                {
                    spawnWeaponDust(type);
                }
                redmistSkeleton.BoneName[BoneLabel.FrontWeapon].Visible = true;
            }
            FrontWeapon = type;
            return changed;
        }

        /// <summary>
        /// if Visible timer is set above 0, auto hides the weapon and reappears whenever Timer (NPC.ai[2]) is equal and spawns weapon dust
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dustTimer"></param>
        /// <param name="dust"></param>
        /// <returns></returns>
        bool ChangeBackWeapon(int type, int visibleTimer = 0, bool dust = true)
        {
            bool changed = false;
            if (BackWeapon != type)
            {
                if (dust)
                {
                    int oldType = BackWeapon;
                    spawnWeaponDust(oldType);
                }
                changed = true;
                //Main.NewText("Changed");
                if (visibleTimer > 0)
                {
                    redmistSkeleton.BoneName[BoneLabel.BackWeapon].Visible = false;
                }
            }
            else if (!redmistSkeleton.BoneName[BoneLabel.BackWeapon].Visible && Timer == visibleTimer && dust)
            {
                spawnWeaponDust(type);
                redmistSkeleton.BoneName[BoneLabel.BackWeapon].Visible = true;
            }
            BackWeapon = type;
            return changed;
        }

        void spawnWeaponDust(int type)
        {
            // Front Weapons
            if (type == ModContent.ItemType<Wingbeat>())
            {
                for (int i = 0; i < 12; i++)
                {
                    SpawnDustonWeapon(-1, 70f, 10f, DustID.BlueFairy);
                }
            }
            else if (type == ModContent.ItemType<Soda>())
            {
                for (int i = 0; i < 12; i++)
                {
                    SpawnDustonWeapon(-1, 70f, 10f, DustID.Corruption);
                }
            }

            // Back Weapons
            else if (type == ModContent.ItemType<Penitence>())
            {
                for (int i = 0; i < 12; i++)
                {
                    SpawnDustonWeapon(1, 70f, 10f, DustID.WoodFurniture);
                }
            }
            else if (type == ModContent.ItemType<Tough>())
            {
                for (int i = 0; i < 12; i++)
                {
                    SpawnDustonWeapon(1, 70f, 10f, DustID.Wraith);
                }
            }
        }

        /// <summary>
        /// -1 Front, 1 Back
        /// </summary>
        /// <param name="which"></param>
        /// <param name="length"></param>
        /// <param name=""></param>
        void SpawnDustonWeapon(int which, float width, float height, int type, Vector2? velocity = null, float fadeIn = 0f)
        {
            BoneLabel whichBone = which < 0 ? BoneLabel.FrontWeapon : BoneLabel.BackWeapon;

            Vector2 pos = redmistSkeleton.BoneName[whichBone].GetPosition(NPC.spriteDirection) + new Vector2(Main.rand.NextFloat(width), Main.rand.NextFloat(-height/2, height/2)).RotatedBy(redmistSkeleton.BoneName[whichBone].GetRotation(NPC.spriteDirection));
            int d = Dust.NewDust(pos, 4, 4, type);
            Main.dust[d].noGravity = true;
            Main.dust[d].fadeIn = fadeIn;
            if (velocity != null)
            {
                Main.dust[d].velocity = (Vector2)velocity;
            }
        }

        enum PhaseEXStates{
            PhaseTransition,
            Idle,
            RedEyesPenitenceJumpAttack,
            WingbeatIntro,
            WingbeatDashAttack,
            WingbeatPenitence,
            Soda,
            Tough,
            SodaTough,
            RedEyes,
            Horn,
            Beak,
            Lantern,
            CherryBlossom,
            Solitude,
            Regret,
            Fragment,
            Look,
            Dream,
            Daredevil,
            FourthMatch,
            ScreamingWedge,
            SoCute
            }

        void Phase1EX()
        {
            //Initiates Phase by using RE P the same way as Ryoshu
            //Bends down, teleports, jumps up and slams both down
            //Afterwards, uses all Zayin EGO in sequence before switching after losing 10% of health
            
            if (AiState == (int)PhaseEXStates.Idle)
            {
                EXFollowMode1();
            }
            else if (AiState == (int)PhaseEXStates.RedEyesPenitenceJumpAttack)
            {
                RedEyesPenitenceJumpAttack();
            }
            else if (AiState == (int)PhaseEXStates.WingbeatIntro)
            {
                WingbeatDualWieldStart();
            }
            else if (AiState == (int)PhaseEXStates.WingbeatDashAttack)
            {
                WingbeatDashAttack();
            }
            else if (AiState == (int)PhaseEXStates.WingbeatPenitence)
            {
                if (FrontWeapon == ModContent.ItemType<Soda>())
                    PenitenceSoda();
                else
                    WingbeatPenitence();
            }
            else if (AiState == (int)PhaseEXStates.Tough)
            {
                ToughShoot();
            }
            else if (AiState == (int)PhaseEXStates.Soda)
            {
                SodaShoot();
            }
            else
            {
                AiState = (int)PhaseEXStates.RedEyesPenitenceJumpAttack;
                Timer = 0;
            }
        }

        void EXFollowMode1()
        {
            float healthPercent = NPC.life / (float)NPC.lifeMax;

            if (NPC.velocity.Y == 0)
            {
                if (NPC.velocity.X == 0)
                    ChangeAnimation(AnimationState.Idle1);
                else
                    ChangeAnimation(AnimationState.Run);
            }
            else
                ChangeAnimation(AnimationState.MidAir);

            float maxSpeed = 12f;
            if (BackWeapon == ModContent.ItemType<Tough>())
            {
                maxSpeed = 10f;

                if (Timer % 60 == 50)
                {
                    Vector2 position = redmistSkeleton.BoneName[BoneLabel.UpperArmR].GetPosition(NPC.spriteDirection);
                    Vector2 delta = NPC.GetTargetData().Center - position;
                    delta.Normalize();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), position + delta * 60, delta * 16, ProjectileID.BulletDeadeye, 10, 0);
                }
            }

            fighterAI(NPC.GetTargetData(), 64f, 0.3f, maxSpeed);
            Vector2 difference = NPC.GetTargetData().Center - NPC.Center;
            float distance = difference.Length();

            Timer++;

            if (distance < 400)
            {
                int attack = Main.rand.Next(4);
                if (attack == 0)
                {
                    AiState = (int)PhaseEXStates.WingbeatDashAttack;
                    Timer = 30 * Main.rand.Next(2, 7) - 1;
                }
                else if (attack == 1)
                {
                    AiState = (int)PhaseEXStates.WingbeatPenitence;
                    Timer = 0;
                    NPC.velocity.X = 12 * NPC.spriteDirection;
                }
                else if (attack == 2)
                {
                    AiState = (int)PhaseEXStates.Soda;
                    Timer = -30;
                }
                else
                {
                    AiState = (int)PhaseEXStates.Tough;
                    Timer = -30;
                }
                NPC.direction = NPC.spriteDirection;
            }

            if (healthPercent < 0.75f)
            {
                AiState = 0;
                Timer = 0;
                Phase++;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<RedMistEye>(), 0, 0, -1, NPC.whoAmI);
                }
            }
        }

        void RedEyesPenitenceJumpAttack()
        {
            Timer++;
            FrontWeapon = ModContent.ItemType<RedEyes>();
            BackWeapon = ModContent.ItemType<Penitence>();
            ChangeAnimation(AnimationState.RedEyesPenitenceIntro);
            if (Timer < 60)
            {
                //Ready Stance
                if (Timer == 59)
                {
                    NPC.TargetClosest(true);
                    NPC.spriteDirection = NPC.direction;
                    NPC.velocity = new Vector2(16 * NPC.direction, -30);
                    NPC.position = NPC.GetTargetData().Center + new Vector2(-16 * 20 * NPC.direction, 0);
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;

                    WeaponSmearLine smear = new WeaponSmearLine();
                    smear.Setup(NPC, Vector2.Normalize(NPC.velocity) * 52, 3.14f + NPC.velocity.ToRotation(), 20, NPC.direction);
                    smear.SetShaderImage(
                        MiscAssets.BloodTexture,
                        MiscAssets.TrailSmoke,
                        MiscAssets.Worley);
                    smear.SetupLine(52, 52, 340);
                    LobCustomDraw.Instance().AddVEffects(smear);

                    SoundEngine.PlaySound(LobotomyCorp.ItemLobSound("Literature/Spidermom_Down"), NPC.Center);
                }
            }
            else if (Timer < 90)
            {
                if (Timer < 80)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, -NPC.velocity.X / 4, -NPC.velocity.Y / 4);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].fadeIn = 1.5f;
                    }
                }
                NPC.velocity *= 0.9f;
                if (Timer == 89)
                {
                    NPC.noTileCollide = false;
                    float rot = (float)Math.Atan2(NPC.GetTargetData().Center.Y - NPC.Center.Y, NPC.GetTargetData().Center.X - NPC.Center.X);
                    NPC.velocity = new Vector2(32, 0).RotatedBy(rot);
                    if (NPC.velocity.Y < 12)
                        NPC.velocity.Y = 12;

                    WeaponSmearLine smear = new WeaponSmearLine();
                    smear.Setup(NPC, Vector2.Zero, NPC.spriteDirection < 0 ? 0 : 3.14f, 20, NPC.direction);
                    smear.SetShaderImage(
                        MiscAssets.BloodTexture,
                        MiscAssets.TrailSmoke,
                        MiscAssets.Worley);
                    smear.SetupLine(52, 52, 300);
                    LobCustomDraw.Instance().AddVEffects(smear);

                    WeaponSmearCircle circ = new WeaponSmearCircle();
                    circ.Setup(NPC, Vector2.Zero, MathHelper.ToRadians(NPC.spriteDirection < 0 ? -45 : -135), 30, NPC.spriteDirection);
                    circ.SetShaderImage(
                        MiscAssets.PenitenceGradient,
                        MiscAssets.TexTrail1,
                        MiscAssets.PlasmaNoise);
                    circ.SetShaderTexOffset(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
                    circ.SetupSemiCircle(30, 80, MathHelper.ToRadians(30), MathHelper.ToRadians(260), MathHelper.ToRadians(280), .2f);
                    circ.Color = Color.White;
                    circ.Color.A = 250;
                    LobCustomDraw.Instance().AddVEffects(circ);

                    WeaponSmearEllipse ellp = new WeaponSmearEllipse();
                    ellp.Setup(NPC, Vector2.Zero, MathHelper.ToRadians(NPC.spriteDirection < 0 ? 150 : 30), 30, NPC.spriteDirection);
                    ellp.SetShaderImage(
                        MiscAssets.RedEyesSlash,
                        MiscAssets.RedEyesSlashA,
                        MiscAssets.PlasmaNoise);
                    ellp.SetShaderTexOffset(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
                    ellp.SetupPartEllipse(80, 30, 160, 80, MathHelper.ToRadians(-135) * NPC.spriteDirection, MathHelper.ToRadians(120), MathHelper.ToRadians(180), MathHelper.ToRadians(270), 0.7f);
                    LobCustomDraw.Instance().AddVEffects(ellp);

                    SoundEngine.PlaySound(LobotomyCorp.ItemLobSound("Literature/Spidermom_Hit"), NPC.Center);
                }
            }
            else if (Timer < 210)
            {
                if (Timer < 120)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, -NPC.velocity.X / 2, -NPC.velocity.Y / 2);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].fadeIn = 1.5f;
                    }
                }
                if (NPC.velocity.Y == 0)
                {
                    NPC.noGravity = false;
                    NPC.velocity.X *= 0.95f;
                }
                //Slashes downwards to slow down
            }
            else
            {
                NPC.velocity.X *= 0;
                AiState = (int)PhaseEXStates.WingbeatIntro;
                Timer = 0;
            }
        }

        void WingbeatDualWieldStart()
        {
            Timer++;
            ChangeFrontWeapon(ModContent.ItemType<Wingbeat>());
            ChangeAnimation(AnimationState.WingbeatWield);
            if (Timer == 1)
            {
                for (int i = 0; i < 24; i++)
                {
                    SpawnDustonWeapon(-1, 70f, 10f, DustID.Wraith);
                    /*
                    Vector2 pos = redmistSkeleton.BoneName[BoneLabel.FrontWeapon].GetPosition(NPC.spriteDirection) + new Vector2(Main.rand.NextFloat(70.000f), Main.rand.Next(-5, 6)).RotatedBy(redmistSkeleton.BoneName[BoneLabel.FrontWeapon].GetRotation(NPC.spriteDirection));
                    int d = Dust.NewDust(pos, 4, 4, DustID.Wraith);
                    Main.dust[d].noGravity = true;*/
                }
            }
            else if (Timer > 50 && Timer < 90)
            {
                if (Timer == 51)
                {
                    SoundEngine.PlaySound(LobotomyCorp.ItemLobSound("Fairy_QueenChange"), NPC.Center);
                }

                for (int i = 0; i < 8; i++)
                {
                    SpawnDustonWeapon(-1, 70f, 10f, DustID.DungeonSpirit);
                    /*
                    Vector2 pos = redmistSkeleton.BoneName[BoneLabel.FrontWeapon].GetPosition(NPC.spriteDirection) + new Vector2(Main.rand.NextFloat(70.000f), Main.rand.Next(-5, 6)).RotatedBy(redmistSkeleton.BoneName[BoneLabel.FrontWeapon].GetRotation(NPC.spriteDirection));
                    int d = Dust.NewDust(pos, 4, 4, DustID.DungeonSpirit);
                    Main.dust[d].noGravity = true;
                    */
                }
            }
            else if (Timer == 120)
            {
                AiState = (int)PhaseEXStates.Idle;
                Timer = 0;
            }
        }

        private void WingbeatDashVSFXEffect()
        {
            WeaponSmearLine smear = new WeaponSmearLine();
            smear.Setup(NPC.Center + new Vector2(50 * NPC.spriteDirection, 0), NPC.velocity, NPC.spriteDirection < 0 ? 0 : 3.14f, 14, NPC.direction);
            smear.SetShaderImage(
                MiscAssets.FlatColor,
                MiscAssets.TexTrail1,
                MiscAssets.PlasmaNoise);
            smear.SetupLine(24, 24, 480);
            smear.Color = new Color(158, 255, 249) * 0.8f;
            LobCustomDraw.Instance().AddVEffects(smear);

            WeaponSmearEllipse ellp = new WeaponSmearEllipse();
            ellp.Setup(NPC, Vector2.Zero, MathHelper.ToRadians(NPC.spriteDirection < 0 ? 180 : 0), 15, NPC.spriteDirection);
            ellp.SetShaderImage(
                 MiscAssets.TexTrail1,
                 MiscAssets.TexTrail1,
                 MiscAssets.Worley);
            ellp.SetShaderTexOffset(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
            ellp.SetupEllipse(64, 20, 140, 55, MathHelper.ToRadians(30), 3.14f, 0.5f);
            ellp.Color = new Color(158, 255, 249) * 0.9f;
            LobCustomDraw.Instance().AddVEffects(ellp);

            for (int i = 0; i < 16; i++)
            {
                Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DungeonSpirit, -NPC.velocity.X)].noGravity = true;
            }

            SoundEngine.PlaySound(LobotomyCorp.ItemLobSound("Fairy_QueenAtk"), NPC.Center);
        }

        void WingbeatDashAttack()
        {
            ChangeAnimation(AnimationState.DashSlash);
            ChangeFrontWeapon(ModContent.ItemType<Wingbeat>());
            Timer--;
            int attackLoop = (int)(Timer % 30);
            if (attackLoop > 15 || attackLoop < 0)
            {
                NPC.velocity.X *= 0.95f;
                
                if (attackLoop == 16)
                {
                    NPC.direction = Math.Sign(NPC.GetTargetData().Center.X - NPC.Center.X);
                    NPC.spriteDirection = NPC.direction;
                    Vector2 pos = new Vector2(NPC.GetTargetData().Center.X - 400 * NPC.direction, NPC.GetTargetData().Position.Y + NPC.GetTargetData().Height - NPC.height / 2);
                    NPC.Center = pos;
                    NPC.velocity.Y = 0;
                    NPC.velocity.X = 32 * NPC.direction;
                    NPC.noGravity = true;

                    WingbeatDashVSFXEffect();
                }
            }
            else
            {
                NPC.noGravity = false;
            }
            if ((int)Timer <= -16)
            {
                if (BackWeapon == ModContent.ItemType<Tough>())
                {
                    AiState = (int)(PhaseEXStates.Tough);
                    Timer = 15;
                }
                else
                {
                    AiState = (int)(PhaseEXStates.Idle);
                    Timer = 0;
                }
                NPC.velocity.X *= .2f;
            }

            Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DungeonSpirit)].noGravity = true;
            NPC.localAI[2] = 1f;
        }

        void WingbeatPenitence()
        {
            ChangeBackWeapon(ModContent.ItemType<Penitence>(), 20);

            ChangeAnimation(AnimationState.PenitenceSlamCombo);
            Timer++;
            if (Timer < 45)
            {
                if (Timer == 30)
                {
                    WeaponSmearCircle circ = new WeaponSmearCircle();
                    circ.Setup(NPC, Vector2.Zero, MathHelper.ToRadians(NPC.spriteDirection < 0 ? -45 : -135), 24, NPC.spriteDirection);
                    circ.SetShaderImage(
                        MiscAssets.PenitenceGradient,
                        MiscAssets.TexTrail1,
                        MiscAssets.PlasmaNoise);
                    circ.SetShaderTexOffset(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
                    circ.SetupSemiCircle(30, 80, MathHelper.ToRadians(30), MathHelper.ToRadians(260), MathHelper.ToRadians(280), .4f);
                    circ.Color = Color.White;
                    circ.Color.A = 250;
                    LobCustomDraw.Instance().AddVEffects(circ);

                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 offset = new Vector2(Main.rand.NextFloat(16 * 10, 16 * 22), 0).RotatedByRandom(6.28f);
                        PenitenceShootStarAt(NPC.Center, NPC.GetTargetData().Center + offset, 60);
                    }

                    SoundEngine.PlaySound(LobotomyCorp.WeaponSounds.Mace, NPC.Center);
                }

                NPC.velocity.X *= 0.98f;
            }
            else if (Timer < 60)
            {
                if (Timer == 59)
                {
                    NPC.TargetClosest();
                    NPC.velocity.X = 24 * NPC.direction;
                    NPC.spriteDirection = NPC.direction;
                    WingbeatDashVSFXEffect();
                }
            }
            else if (Timer < 85)
            {
                if (Timer == 84)
                {
                    NPC.TargetClosest();
                    NPC.spriteDirection = NPC.direction;
                }
            }
            else if (Timer < 145)
            {
                NPC.velocity.X *= 0.95f;

                if (Timer == 115)
                {
                    WeaponSmearCircle circ = new WeaponSmearCircle();
                    circ.Setup(NPC, Vector2.Zero, MathHelper.ToRadians(NPC.spriteDirection < 0 ? -45 : -135), 24, NPC.spriteDirection);
                    circ.SetShaderImage(
                        MiscAssets.PenitenceGradient,
                        MiscAssets.TexTrail1,
                        MiscAssets.PlasmaNoise);
                    circ.SetShaderTexOffset(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
                    circ.SetupSemiCircle(30, 80, MathHelper.ToRadians(30), MathHelper.ToRadians(260), MathHelper.ToRadians(280), .4f);
                    circ.Color = Color.White;
                    circ.Color.A = 250;
                    LobCustomDraw.Instance().AddVEffects(circ);

                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 offset = new Vector2(Main.rand.NextFloat(16 * 10, 16 * 22), 0).RotatedByRandom(6.28f);
                        PenitenceShootStarAt(NPC.Center, NPC.GetTargetData().Center + offset, 60);
                    }

                    WeaponSmearEllipse ellp = new WeaponSmearEllipse();
                    ellp.Setup(NPC, Vector2.Zero, MathHelper.ToRadians(NPC.spriteDirection < 0 ? 180 : 0), 24, NPC.spriteDirection);
                    ellp.SetShaderImage(
                         MiscAssets.TexTrail1,
                         MiscAssets.TexTrail1,
                         MiscAssets.Worley);
                    ellp.SetShaderTexOffset(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
                    ellp.SetupEllipse(64, 20, 140, 55, MathHelper.ToRadians(30), 3.14f, 0.5f);
                    ellp.Color = new Color(158, 255, 249) * 0.9f;
                    LobCustomDraw.Instance().AddVEffects(ellp);

                    SoundEngine.PlaySound(LobotomyCorp.WeaponSounds.Mace, NPC.Center);
                }
            }

            if (Timer > 145)
            {
                AiState = 1;
                Timer = 0;
            }
        }

        void PenitenceSoda()
        {
            ChangeBackWeapon(ModContent.ItemType<Penitence>(), 20);

            Timer++;
            if (Timer < 45)
            {
                if (Timer == 30)
                {
                    WeaponSmearCircle circ = new WeaponSmearCircle();
                    circ.Setup(NPC, Vector2.Zero, MathHelper.ToRadians(NPC.spriteDirection < 0 ? -45 : -135), 24, NPC.spriteDirection);
                    circ.SetShaderImage(
                        MiscAssets.PenitenceGradient,
                        MiscAssets.TexTrail1,
                        MiscAssets.PlasmaNoise);
                    circ.SetShaderTexOffset(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
                    circ.SetupSemiCircle(30, 80, MathHelper.ToRadians(30), MathHelper.ToRadians(260), MathHelper.ToRadians(280), .4f);
                    circ.Color = Color.White;
                    circ.Color.A = 250;
                    LobCustomDraw.Instance().AddVEffects(circ);

                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 offset = new Vector2(Main.rand.NextFloat(16 * 10, 16 * 22), 0).RotatedByRandom(6.28f);
                        PenitenceShootStarAt(NPC.Center, NPC.GetTargetData().Center + offset, 60);
                    }

                    SoundEngine.PlaySound(LobotomyCorp.WeaponSounds.Mace, NPC.Center);
                }

                ChangeAnimation(AnimationState.PenitenceSlamCombo);
                NPC.velocity.X *= 0.98f;
            }
            else if (Timer < 60)
            {
                if (Timer == 59)
                {
                    NPC.velocity.X *= 0;
                    NPC.TargetClosest();
                    NPC.spriteDirection = NPC.direction;
                }
            }
            else if (Timer < 85)
            {
                ChangeAnimation(AnimationState.Soda);
                if (Timer % 15 == 0)
                {
                    SodaTargetProjectile();
                }

                if (Timer == 84)
                {
                    NPC.TargetClosest();
                    NPC.spriteDirection = NPC.direction;
                }
            }
            else if (Timer < 145)
            {
                ChangeAnimation(AnimationState.PenitenceSlamCombo, 85);
                NPC.velocity.X *= 0.95f;

                if (Timer == 115)
                {
                    WeaponSmearCircle circ = new WeaponSmearCircle();
                    circ.Setup(NPC, Vector2.Zero, MathHelper.ToRadians(NPC.spriteDirection < 0 ? -45 : -135), 24, NPC.spriteDirection);
                    circ.SetShaderImage(
                        MiscAssets.PenitenceGradient,
                        MiscAssets.TexTrail1,
                        MiscAssets.PlasmaNoise);
                    circ.SetShaderTexOffset(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
                    circ.SetupSemiCircle(30, 80, MathHelper.ToRadians(30), MathHelper.ToRadians(260), MathHelper.ToRadians(280), .4f);
                    circ.Color = Color.White;
                    circ.Color.A = 250;
                    LobCustomDraw.Instance().AddVEffects(circ);

                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 offset = new Vector2(Main.rand.NextFloat(16 * 10, 16 * 22), 0).RotatedByRandom(6.28f);
                        PenitenceShootStarAt(NPC.Center, NPC.GetTargetData().Center + offset, 60);
                    }

                    WeaponSmearEllipse ellp = new WeaponSmearEllipse();
                    ellp.Setup(NPC, Vector2.Zero, MathHelper.ToRadians(NPC.spriteDirection < 0 ? 180 : 0), 24, NPC.spriteDirection);
                    ellp.SetShaderImage(
                         MiscAssets.TexTrail1,
                         MiscAssets.TexTrail1,
                         MiscAssets.Worley);
                    ellp.SetShaderTexOffset(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
                    ellp.SetupEllipse(64, 20, 140, 55, MathHelper.ToRadians(30), 3.14f, 0.5f);
                    ellp.Color = new Color(158, 255, 249) * 0.9f;
                    LobCustomDraw.Instance().AddVEffects(ellp);

                    SoundEngine.PlaySound(LobotomyCorp.WeaponSounds.Mace, NPC.Center);
                }
            }

            if (Timer > 145)
            {
                AiState = 1;
                Timer = 0;
            }
        }

        void ToughShoot()
        {
            NPC.velocity *= 0.8f;

            bool twoshooter = FrontWeapon == ModContent.ItemType<Soda>();

            ChangeBackWeapon(ModContent.ItemType<Tough>(), -20);

            Vector2 position = redmistSkeleton.BoneName[BoneLabel.UpperArmR].GetPosition(NPC.spriteDirection);
            Vector2 delta = NPC.GetTargetData().Center - position;
            delta.Normalize();
            if (twoshooter)
                ChangeAnimation(AnimationState.ShootBoth);
            else
                ChangeAnimation(AnimationState.Tough);

            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;

            Timer++;
            if (Timer % 15 == 5 && Timer > 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), position + delta * 60, delta * 16, ProjectileID.BulletDeadeye, 10, 0);

                if (twoshooter)
                {
                    SodaTargetProjectile();
                }
            }

            if (Timer > 45) 
                AiState = 1;
        }

        void SodaShoot()
        {
            NPC.velocity *= 0.8f;

            bool twoshooter = BackWeapon == ModContent.ItemType<Tough>();

            ChangeFrontWeapon(ModContent.ItemType<Soda>(), -20);

            Vector2 position = redmistSkeleton.BoneName[BoneLabel.UpperArmL].GetPosition(NPC.spriteDirection);
            Vector2 targetLoc = NPC.GetTargetData().Center;
            if (twoshooter)
                ChangeAnimation(AnimationState.ShootBoth);
            else
                ChangeAnimation(AnimationState.Soda);

            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;

            Timer++;
            if (Timer % 15 == 5 && Timer > 0)
            {
                Vector2 delta = targetLoc - position;
                delta.Normalize();
                if (twoshooter)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), position + delta * 60, delta * 16, ProjectileID.BulletDeadeye, 10, 0);
                }
                SodaTargetProjectile();
            }

            if (Timer > 45)
                AiState = 1;
        }

        void SodaTargetProjectile()
        {
            Vector2 position = redmistSkeleton.BoneName[BoneLabel.UpperArmL].GetPosition(NPC.spriteDirection);
            Vector2 delta = NPC.GetTargetData().Center - position;
            delta.Normalize();

            Vector2 target = NPC.GetTargetData().Center;
            if (Timer == 45)
                target.X -= 16 * 5;
            if (Timer == 15)
                target.X += 16 * 5;

            int tries = 0;
            int maxTries = 30;
            while (tries < maxTries)
            {
                int y = (int)((target.Y + 16 * tries) / 16);
                int x = (int)(target.X / 16);
                //LobotomyCorp.LookingAt(new Vector2(x * 16, y * 16));
                if (Main.tile[x,y].HasTile && Main.tileSolid[Main.tile[x, y].TileType] && !Main.tileSolidTop[Main.tile[x, y].TileType])
                    break;
                tries++;    
            }
            tries++;
            if (tries <= maxTries)
            {
                target.Y += tries * 16;
            }
            else
            {
                target = NPC.GetTargetData().Center;
            }
            Vector2 shootTo = position.DirectionTo(target) * 16;

            Projectile.NewProjectile(NPC.GetSource_FromThis(), position + delta * 60, shootTo, ModContent.ProjectileType<RedMistSodaBullet>(), 10, 0);
        }

        void PenitenceShootStarAt(Vector2 origin, Vector2 target, int damage)
        {
            Vector2 vel = target - origin;
            int time = 40;
            vel /= time * 0.5f;

            Projectile.NewProjectile(NPC.GetSource_FromThis(), origin, vel, ModContent.ProjectileType<RedMistPenitenceStar2>(), damage, 0);
        }

        void ResetAI()
        {
            AiState = (int)PhaseEXStates.Idle;
            Timer = 0;
        }

        void Phase2EX()
        {
            if (AiState == (int)PhaseEXStates.Idle)
            {
                EXFollowMode2();
            }
            else if (AiState == (int)PhaseEXStates.RedEyes)
            {
                RedEyesSolo();
            }
            else
            {
                WristCutterStart();
            }
        }

        void EXFollowMode2()
        {
            float healthPercent = NPC.life / (float)NPC.lifeMax;

            if (NPC.velocity.Y == 0)
            {
                if (NPC.velocity.X == 0)
                    ChangeAnimation(AnimationState.EXIdle);
                else
                    ChangeAnimation(AnimationState.Run);
            }
            else
                ChangeAnimation(AnimationState.MidAir);

            float maxSpeed = 12f;

            fighterAI(NPC.GetTargetData(), 64f, 0.3f, maxSpeed);
            Vector2 difference = NPC.GetTargetData().Center - NPC.Center;
            float distance = difference.Length();

            Timer++;
            if (Timer > 90)
            {
                AiState = (int)PhaseEXStates.RedEyes;
                Timer = 0;
            }

            if (distance < 400)
            {
                int attack = Main.rand.Next(4);
                NPC.direction = NPC.spriteDirection;
            }
        }

        private void WristCutterStart()
        {
            NPC.spriteDirection = 1;
            ChangeAnimation(AnimationState.WristCutterStart);
            Timer++;
            if (Timer > 50)
            {
                ChangeBackWeapon(ModContent.ItemType<Regret>(), 250);
                if (Timer < 190)
                {
                    ChangeFrontWeapon(ModContent.ItemType<Items.Teth.WristCutter>(), 90);
                }
                else
                {
                    // Throws WristCutter to every players in the server that inflicts a permanent life regen negation
                    if (Timer == 190)
                    {
                        foreach (Player p in Main.ActivePlayers)
                        {
                            Vector2 direction = NPC.Center.DirectionTo(p.MountedCenter) * 20;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction, ModContent.ProjectileType<RedMistWristCutter>(), 0, 0, ai0: p.whoAmI);
                        }
                    }
                    ChangeFrontWeapon(ModContent.ItemType<Items.Teth.RedEyes>(), 250);
                }
            }
            if (Timer > 290)
            {
                ResetAI();
            }
        }

        private void RedEyesSolo()
        {
            Timer--;
            ChangeFrontWeapon(ModContent.ItemType<Items.Teth.RedEyes>());
            if (Timer < 0)
            {
                ChangeAnimation(AnimationState.EXIdle);
                NPC.velocity *= 0.8f;
                if (Timer == -1 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int tpTo = Main.rand.Next(15);
                    for (int i = 0; i < 15; i++)
                    {
                        Vector2 validLocation = NPC.GetTargetData().Center;
                        int retries = 0;
                        while (retries < 32 || (tpTo == i && retries < 1000))
                        {
                            validLocation = NPC.GetTargetData().Center + new Vector2(Main.rand.Next(-400, 400), Main.rand.Next(-400, 400));
                            if (Collision.CanHit(validLocation, 8, 8, NPC.GetTargetData().Position, NPC.GetTargetData().Width, NPC.GetTargetData().Height))
                                break;
                        }
                        if (retries < 32 || tpTo == i)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), validLocation, Vector2.Zero, ModContent.ProjectileType<RedMistRedEyesPortal>(), 0, 0, ai0: tpTo == i ? 1 : 0, ai1: i * 2);
                        }
                    }

                    foreach (Player p in Main.ActivePlayers)
                    {
                        if (p.Center.Distance(NPC.Center) < 4000)
                        {
                            p.AddBuff(BuffID.Webbed, 60);
                        }
                    }
                }
            }
            else if (Timer > 90)
            {
                NPC.velocity *= 0.95f;
            }
            else if (Timer == 90)
            {
                NPC.noGravity = true;
                NPC.velocity = NPC.Center.DirectionTo(NPC.GetTargetData().Center) * 16f;

                WeaponSmearEllipse ellp = new WeaponSmearEllipse();
                ellp.Setup(NPC, Vector2.Zero, NPC.velocity.ToRotation(), 30, NPC.spriteDirection);
                ellp.SetShaderImage(
                    MiscAssets.RedEyesSlash,
                    MiscAssets.RedEyesSlashA,
                    MiscAssets.PlasmaNoise);
                ellp.SetShaderTexOffset(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
                ellp.SetupPartEllipse(80, 30, 160, 80, MathHelper.ToRadians(-135) * NPC.spriteDirection, MathHelper.ToRadians(120), MathHelper.ToRadians(180), MathHelper.ToRadians(270), 0.7f);
                LobCustomDraw.Instance().AddVEffects(ellp);
            }
            else if (Timer > 20)
            {
                NPC.velocity *= 0.98f;
            }
            else if (Timer > 1)
            {
                NPC.velocity *= 0.95f;
            }
            if (Timer == 1 || Timer < -400)
            {
                // Go back to normal running to player
                NPC.noGravity = false;
                ResetAI();
            }
        }

        public void RedEyesTeleportRedMistTo(Vector2 pos, Vector2 vel)
        {
            Timer = 120;
            NPC.Center = pos;
            NPC.velocity = vel;
            NPC.TargetClosest();
            NPC.netUpdate = true;
        }

        private void RedEyesRegret()
        {
            Timer++;
            if (Timer < 30)
            {
                RedEyesVanish(1);
            }
            else if (Timer < 60)
            {

            }
        }

        private void RedEyesVanish(int Vanish)
        {
            if (Timer == Vanish)
            {
                //Player Fade Animation
                NPC.velocity.Y = 0;
                NPC.velocity.X = NPC.spriteDirection * -22;
            }
            else if (Timer > Vanish)
            {
                //Continues backing out
            }
        }

        private bool WillHitTarget(Rectangle Target, float Time)
        {
            for (int i = 0; i <= Time; i++)
            {
                Rectangle npcHitbox = NPC.getRect();
                npcHitbox.X += (int)(i * NPC.velocity.X);
                npcHitbox.Y += (int)(i * NPC.velocity.Y);

                if (npcHitbox.Intersects(Target))
                    return true;
            }
            return false;
        }

        public void bubbleShieldRange(int phase, ref float min, ref float max)
        {
            if (phase == 0)
            {
                min = 20 * 16;
                max = 30 * 16;
            }
            else if (phase == 1)
            {
                min = 16 * 16;
                max = 25 * 16;
            }
            else if (phase == 2)
            {
                min = 45 * 16;
                max = 60 * 16;
            }
            else
            {
                min = 25 * 16;
                max = 30 * 16;
            }
            if (isUsingGoldRush())
            {
                min = 0;
                max = 16;
            }
        }

        private void bubbleShieldDustDisplay(int phase)
        {
            Vector2 ShieldCenter = NPC.Center;
            Vector2 delta = Main.LocalPlayer.Center - ShieldCenter;
            float min = 0, max = 0;
            bubbleShieldRange((int)NPC.ai[0], ref min, ref max);
            float minDist = max - (max - min) / 2;
            if (delta.LengthSquared() < minDist * minDist || isUsingGoldRush())
            {
                return;
            }
            float radius = max;
            float arc = 6 * 11;
            float angle = arc / radius / 5f;
            float rotationToPlayer = delta.ToRotation();
            for (int i = -5; i <= 5; i++)
            {
                Vector2 dustPos = ShieldCenter + new Vector2(radius, 0).RotatedBy(rotationToPlayer + angle * i);
                Dust d = Dust.NewDustPerfect(dustPos, DustID.GemRuby);
                d.noGravity = true;
                d.velocity = Vector2.Zero;
            }

            if (delta.LengthSquared() < max * max)
                return;

            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.owner == Main.myPlayer && p.friendly)
                {
                    Vector2 dist = p.Center - NPC.Center;
                    if (dist.LengthSquared() < 200 * 200)
                    {
                        int d = Dust.NewDust(p.position, p.width, p.height, DustID.GemRuby);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 0;
                    }
                }
            }
        }

        private bool isUsingGoldRush()
        {
            if (Phase == 0)
            {
                switch (AiState)
                {
                    case GoldRushBig:
                    case GoldRushBig + 1:
                    case GoldRushBig + 2:
                    case GoldRushBig + 3:
                    case GoldRushSmall:
                    case 9:
                    case 11:
                        return true;
                    default: return false;
                }
            }
            if (Phase == 1)
            {

                switch (AiState)
                {
                    case SpecialAttackStart:
                    case SpecialAttackStart + 1:
                    case SpecialAttackStart + 2:
                    case GoldRushMimicryCombo:
                    case GoldRushMimicryCombo + 1:
                    case GoldRushMimicryCombo + 3:
                        return true;
                    default: return false;
                }
            }
            if (Phase == 2)
            {
                switch (AiState)
                {
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        return true;
                    default: return false;
                }
            }
            if (Phase == 3)
            {
                switch (AiState)
                {
                    case GoldRushFollowStart:
                    case GoldRushFollow:
                    case TwilightJumpslash:
                    case RedMistExhaust:
                    case TwilightRun:
                    case TwilightRunDash:
                        return true;
                    default: return false;
                }
            }
            return false;
        }

        private void UpdateAggression()
        {
            // Aggression
            // Increases when direct line of sight between the Terrarian and the Red Mist is broken for a certain amount of time
            // During phase 1 and phase 3, She does a quick one portal Gold Rush to a random target player
            // In phase 2, After quick Gold Rush, it will do a mimicry slash
            // In phase 3, it will do Smile Slam
            // Increasing Aggression in phase 4 makes her shift dash to the player
            if (!Collision.CanHit(NPC, NPC.GetTargetData()))
            {
                Aggression++;
            }
            else if (Aggression > 0)
            {
                Aggression -= 2;
                if (Aggression < 0)
                    Aggression = 0;
            }
        }

        private void ChangeAnimation(AnimationState i, double frameCounter = 0)
        {
            if (NPC.localAI[0] != (float)i)
                NPC.frameCounter = frameCounter;
            NPC.localAI[0] = (float)i;
        }

        private void SuppressionTextSpawner()
        {
            // Suppresion Text Maker
            if (suppTextCooldown-- <= 0 && Main.rand.NextBool(300))
            {
                List<string> possibleText = new List<string>();
                if (Main.rand.NextBool(2))
                {
                    possibleText.Add(SuppressionText.GetSephirahText("Gebura", 0));
                    possibleText.Add(SuppressionText.GetSephirahText("Gebura", 1));
                    possibleText.Add(SuppressionText.GetSephirahText("Gebura", 2));
                    possibleText.Add(SuppressionText.GetSephirahText("Gebura", 3));
                }

                if (NPC.ai[0] == 0)
                {
                    possibleText.Add(SuppressionText.GetSephirahText("Gebura", 4));
                    possibleText.Add(SuppressionText.GetSephirahText("Gebura", 5));
                }

                else if (NPC.ai[0] == 1)
                {
                    possibleText.Add(SuppressionText.GetSephirahText("Gebura", 6));
                    possibleText.Add(SuppressionText.GetSephirahText("Gebura", 7));
                }

                else if (NPC.ai[0] == 2)
                {
                    possibleText.Add(SuppressionText.GetSephirahText("Gebura", 8));
                    possibleText.Add(SuppressionText.GetSephirahText("Gebura", 9));
                }

                else if (NPC.ai[0] == 3)
                {
                    possibleText.Add(SuppressionText.GetSephirahText("Gebura", 10));
                }
                Vector2 offsetRandom = new Vector2(Main.rand.Next(-1000, 1000), Main.rand.Next(-1000, 1000));
                Color textColor = Color.Red;
                textColor *= 0.5f;
                SuppressionText.AddText(possibleText[Main.rand.Next(possibleText.Count)], NPC.Center + offsetRandom, Main.rand.NextFloat(-0.5000f, 0.5000f), 2f, textColor, 0.2f, 240, Main.rand.Next(-1, 2), Main.rand.NextFloat(1.000f));
                suppTextCooldown = 300;
            }
        }

        private void SkeletonHandler()
        {
            if (redmistSkeleton == null)
            {
                AiState = -1;
                redmistSkeleton = new RedMistSkeleton(NPC);
                //redmistSkeleton.BoneName = [];
                //redmistSkeleton.Init(NPC);
            }
            //LobotomyCorp.LookingAt(redmistSkeleton.BoneName[BoneLabel.HandRIK].GetPosition());
            //LobotomyCorp.LookingAt(redmistSkeleton.BoneName[BoneLabel.HandLIK].GetPosition(), DustID.GemRuby);
            //LobotomyCorp.LookingAt(redmistSkeleton.BoneName[BoneLabel.FeetRIK].GetPosition(), DustID.GemSapphire);
            //LobotomyCorp.LookingAt(redmistSkeleton.BoneName[BoneLabel.FeetLIK].GetPosition(), DustID.GemEmerald);
        }

        public override void FindFrame(int frameHeight)
        {
            if (redmistSkeleton == null)
            {
                return;
            }
            redmistSkeleton.Update(NPC);
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (isUsingGoldRush())
                return;
            int phase = (int)Phase;
            float damageReduction = 0.33f;
            float min = 0;
            float max = 0;
            bubbleShieldRange(phase, ref min, ref max);
            switch (phase)
            {
                case 1:
                    damageReduction = 0.2f;
                    break;
                case 2:
                    damageReduction = 0.75f;
                    break;
                case 3:
                    damageReduction = 0.9f;
                    break;
                default:
                    break;
            }

            if ((NPC.Center - Main.player[projectile.owner].Center).Length() > max)
                modifiers.FinalDamage *= damageReduction;
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (isUsingGoldRush())
                return;
            int phase = (int)Phase;
            float damageReduction = 0.33f;
            float min = 0;
            float max = 0;
            bubbleShieldRange(phase, ref min, ref max);
            switch (phase)
            {
                case 1:
                    damageReduction = 0.2f;
                    break;
                case 2:
                    damageReduction = 0.75f;
                    break;
                case 3:
                    damageReduction = 0.9f;
                    break;
                default:
                    break;
            }

            if ((NPC.Center - Main.player[projectile.owner].Center).Length() > max)
                SoundEngine.PlaySound(SoundID.NPCHit43, NPC.Center);
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (!LobEventFlags.downedRedMist)
            {
                bool reduce = false;
                if (Phase == 0)
                {
                    if (AiState == 5 || AiState == 6 || AiState == SwitchPhase)
                        reduce = true;
                }
                if (Phase == 1)
                {
                    if (AiState == SpecialAttackStart || AiState == SpecialAttackStart + 1 || AiState == SwitchPhase)
                        reduce = true;
                }
                if (Phase == 2)
                {
                    if (AiState == 6 || AiState == 7 || AiState == 8 || AiState == SwitchPhase)
                        reduce = true;
                }
                if (reduce)
                    modifiers.FinalDamage *= 0;
            }
        }

        private void fighterAI(Terraria.DataStructures.NPCAimedTarget target, float EffectiveRange, float speed, float maxSpeed)
        {
            /*
            NPC.directionY = 1;
            if (target.Position.Y + target.Height <= NPC.position.Y + (float)NPC.height)
            {
                NPC.directionY = -1;
            }*/
            Vector2 delta = target.Center - NPC.Center;

            int dir = 0;

            if (delta.X + EffectiveRange < 0)
                dir = -1;
            if (delta.X - EffectiveRange > 0)
                dir = 1;

            // Slowly stop
            if (dir == 0)
            {
                NPC.velocity.X *= 0.8f;
                if (NPC.velocity.X < speed || NPC.velocity.X > -speed)
                    NPC.velocity.X = 0;
            }
            // Move towards direction
            else
            {
                NPC.velocity.X += speed * dir;
                if (Math.Abs(NPC.velocity.X) > maxSpeed)
                    NPC.velocity.X = maxSpeed * dir;
                if (Math.Abs(NPC.velocity.X) < 0)
                    NPC.velocity.X += speed * dir * 0.5f;

                NPC.spriteDirection = dir;
            }

            int x = (int)NPC.Center.X;
            int y = (int)(NPC.position.Y + NPC.height);

            x += ((int)(NPC.width / 2) + 1 * dir) * dir;

            x /= 16;
            y /= 16;

            NPC.stepSpeed = 2;
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false);

            float py = NPC.GetTargetData().Position.Y + NPC.GetTargetData().Height;
            // Lets take 20 Tiles being the limit of when Red Mist can jump, If its above 20 Tiles, use the 'anti stuck' with aggression
            if (NPC.velocity.Y == 0 && dir != 0)
            {
                //Checks for empty tiles in front
                int x2 = (int)NPC.position.X;
                if (dir > 0)
                {
                    x2 += (NPC.width) * dir;
                }
                x2 /= 16;
                bool jump = false;
                if (!Collision.SolidTiles(x2, x2 + 3, y, y, true) && NPC.GetTargetData().Position.Y + NPC.GetTargetData().Height <= NPC.position.Y + NPC.height)
                    jump = true;
                //Wall Checking

                if (!jump)
                {
                    for (int i = 2; i < 10; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            int fy = y - i;
                            int fx = x2 + j * dir;
                            int tileHeight = (NPC.height / 16);

                            Tile t = Main.tile[fx, fy];
                            int ttype = t.TileType;
                            if (t.HasTile && Main.tileSolid[ttype] && !Main.tileSolidTop[ttype] && !Collision.SolidTiles(fx - 3, fx + 3, fy - tileHeight, fy - 1, false))
                            {
                                jump = true;
                                break;
                            }
                        }
                    }
                }
                if (jump && NPC.position.Y + NPC.height > py)
                {
                    NPC.velocity.Y -= 12f;

                    if (Timer <= 120)
                        Timer = 120;
                    return;
                }

                // Check if RM can reach player's elevation throught platforms
                int py2 = (int)(py / 16);
                int check = (int)(y - py2);
                int time = 140;
                if (check > 6)
                {
                    if (check > 30)
                    {
                        check = 30;
                        py2 = y - check;
                    }

                    jump = false;
                    Vector2 targetPos = Vector2.Zero;
                    for (int i = 0; i < check - 6; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int fx = x + j;
                            int fy = py2 + i;
                            if (Main.tile[fx, fy].HasTile && Main.tileSolidTop[Main.tile[fx, fy].TileType])
                            {
                                targetPos = new Vector2(fx * 16, fy * 16);
                                jump = true;
                                time -= (int)(60 * (1f - (float)(check - i) / 30));
                                break;
                            }
                        }
                        if (jump)
                            break;
                    }

                    if (jump)
                    {
                        Vector2 velocity = AIHelper.ProjectileMotion(new Vector2(0, NPC.position.Y + NPC.height), targetPos, time, NPC.gravity);
                        NPC.velocity.Y = velocity.Y;
                    }

                    else if (NPC.GetTargetData().Velocity.Y != 0 || check > 30)
                    {
                        //Aggression += 4;
                    }
                }

            }
        }

        public override bool? CanFallThroughPlatforms()
        {
            bool isFighterAI = AiState <= FollowState;
            if (NPC.directionY == 1 && isFighterAI && Main.player[NPC.target].position.Y > NPC.position.Y + (float)NPC.height)
            {
                return true;
            }

            return base.CanFallThroughPlatforms();
        }

        /*
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (NPC.damage > 0)
                return base.CanHitPlayer(target, ref cooldownSlot);
            return false;
        }*/

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref LobEventFlags.downedRedMist, -1);

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Vector2 pos = redmistSkeleton.BoneName[BoneLabel.Pelvis].GetPosition(NPC.direction);
                Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore1").Type);
                pos = redmistSkeleton.BoneName[BoneLabel.Pelvis].GetPosition(NPC.direction);
                Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore2").Type);
                pos = redmistSkeleton.BoneName[BoneLabel.UpperArmR].GetPosition(NPC.direction);
                Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore3").Type);
                pos = redmistSkeleton.BoneName[BoneLabel.UpperArmL].GetPosition(NPC.direction);
                Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore4").Type);
                pos = redmistSkeleton.BoneName[BoneLabel.UpperLegR].GetPosition(NPC.direction);
                Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore5").Type);
                pos = redmistSkeleton.BoneName[BoneLabel.LowerLegR].GetPosition(NPC.direction);
                Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore6").Type);
                pos = redmistSkeleton.BoneName[BoneLabel.UpperLegL].GetPosition(NPC.direction);
                Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore7").Type);
                pos = redmistSkeleton.BoneName[BoneLabel.LowerLegL].GetPosition(NPC.direction);
                Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore8").Type);
                pos = redmistSkeleton.BoneName[BoneLabel.Head].GetPosition(NPC.direction);
                Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore9").Type);
                pos = redmistSkeleton.BoneName[BoneLabel.Head].GetPosition(NPC.direction);
                Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore10").Type);
                pos = redmistSkeleton.BoneName[BoneLabel.Head].GetPosition(NPC.direction);
                Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore11").Type);
                pos = redmistSkeleton.BoneName[BoneLabel.Head].GetPosition(NPC.direction);
                Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore12").Type);
            }
        }

        public bool IncomingProjectile()
        {
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.friendly && p.damage > 0)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        Rectangle hb = p.getRect();
                        hb.X = (int)p.position.X + p.width; hb.Y = (int)p.position.Y + p.height;
                        if (hb.Intersects(NPC.getRect()))
                            return true;
                    }
                }
            }
            return false;
        }

        public override bool? CanBeHitByProjectile(Projectile Projectile)
        {
            if (Phase == 1 && (AiState == Dash || AiState == DashSwingMimicry))
                return false;
            return null;
        }

        public void Talk(string id, int dir)
        {
            string text = SuppressionText.GetSephirahText("Gebura", id);// SuppressionTextData.GeburaBark[id];
            //int dir = NPC.spriteDirection;
            Vector2 pos = new Vector2(NPC.Center.X, NPC.position.Y - NPC.height / 2);
            if (dir < 0)
                pos.X = NPC.position.X + NPC.width * 0.25f;
            else if (dir > 0)
                pos.X = NPC.position.X + NPC.width * 0.75f;
            SuppressionText.AddText(text, pos, Main.rand.NextFloat(-0.12f, 0.12f), 0.5f, Color.Red, 0.75f, 30, dir, 0);
        }

        private float filterProgress = 0;
        // Unused, Now in RedMistScene
        public void ScreenFilterHandler()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (!Terraria.Graphics.Effects.Filters.Scene["LobotomyCorp:RedMistOverlay"].IsActive())
                {
                    Terraria.Graphics.Effects.Filters.Scene.Activate("LobotomyCorp:RedMistOverlay");
                }
                else
                {
                    float distance = NPC.Distance(Main.LocalPlayer.Center);
                    float minDist = 120;
                    float maxDist = 250;
                    bubbleShieldRange((int)NPC.ai[0], ref minDist, ref maxDist);

                    distance = (distance - minDist) / (maxDist - minDist);
                    distance = Math.Clamp(distance, 0.1f, 1f);

                    Terraria.Graphics.Effects.Filters.Scene["LobotomyCorp:RedMistOverlay"].GetShader().UseIntensity(distance);

                    filterProgress += 1f / 180f;
                    if (filterProgress > 1f)
                        filterProgress -= 1f;

                    Terraria.Graphics.Effects.Filters.Scene["LobotomyCorp:RedMistOverlay"].GetShader().UseProgress(filterProgress);
                }
            }
        }

        public override bool CheckDead()
        {
            if (Phase < 4 && Main.netMode == NetmodeID.SinglePlayer)
            {
                Phase = 4;
                Timer = 0;
                NPC.life = 1;
                NPC.dontTakeDamage = true;
                NPC.noGravity = false;
                NPC.noTileCollide = false;
                List<string> possibleText = new List<string>
                {
                    SuppressionText.GetSephirahText("Gebura", 11),
                    SuppressionText.GetSephirahText("Gebura", 12)
                };
                Color textColor = Color.Red;
                textColor *= 0.5f;
                SuppressionText.AddText(possibleText[Main.rand.Next(possibleText.Count)], NPC.Center, Main.rand.NextFloat(-0.5000f, 0.5000f), 2f, textColor, 0.2f, 240, 0, 0);
                suppTextCooldown = 1000;

                NPC.netUpdate = true;
                return false;
            }
            return base.CheckDead();
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        private AnimationState animState
        {
            get { return (AnimationState)NPC.localAI[0]; }
            set { NPC.localAI[0] = (int)value; }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (redmistSkeleton == null)
            {
                return false;
            }

            //Should probably add Keter Suppresion Red filter when player is outside effective range
            if (NPC.localAI[2] > 0)
            {
                int Factor = (int)(10 * (NPC.localAI[2] / 1f));
                for (int i = 0; i < Factor; i++)
                {
                    Color trailColor = Color.Red;
                    trailColor.A = 178;
                    trailColor *= (0.3f * (1f - (float)i / (float)Factor));
                    redmistSkeleton.DrawSkeleton(spriteBatch, NPC, trailColor, i);
                }
            }

            // Draws Red Hood's Mark Symbol on current player target during twilight chase
            if (Phase == 3 && (AiState == TwilightRun || AiState == TwilightRunDash))
            {
                Player target = Main.player[NPC.target];
                Texture2D tex = Mod.Assets.Request<Texture2D>("Misc/LittleRedMark").Value;
                Vector2 pos = target.MountedCenter - Main.screenPosition - new Vector2(0, target.height);
                if (TwilightRushTime >= 540)
                {
                    tex = Mod.Assets.Request<Texture2D>("Misc/RedMistMark").Value;
                    pos.X += Main.rand.Next(-5, 5);
                    pos.Y += Main.rand.Next(-5, 5);
                }
                Main.EntitySpriteDraw(tex, pos, tex.Frame(), Color.White, 0, tex.Size() / 2, 1f, 0, 0);
            }

            redmistSkeleton.DrawSkeleton(spriteBatch, NPC, drawColor, 0, true);
            return false;
        }

        public void GetEye(ref Vector2 position, ref float rotation)
        {
            position = redmistSkeleton.BoneName[BoneLabel.Head].GetPosition(NPC.spriteDirection) + new Vector2(-2, 8 * NPC.spriteDirection).RotatedBy(redmistSkeleton.BoneName[BoneLabel.Head].GetRotation());
            rotation = redmistSkeleton.BoneName[BoneLabel.Head].GetRotation();
        }
    }
}

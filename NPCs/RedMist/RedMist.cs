using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using LobotomyCorp;
using LobotomyCorp.Utils;
using LobotomyCorp.UI;
using System.Collections.Generic;
using System.IO;
using LobotomyCorp.Projectiles;
using LobotomyCorp.Projectiles.KingPortal;
using LobotomyCorp.ModSystems;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.NetModules;
using Terraria.Localization;
using Terraria.Chat;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            switch(NPC.ai[0])
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
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 100;
            NPC.lifeMax = 23000;
            //NPC.noTileCollide = true;
            //NPC.noGravity = true;
            NPC.damage = 20;
            NPC.defense = 12;
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
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/PMSecondWarning");
            }
        }

        private RedMistSkeletonHelper skelly;
        private Projectile HeldProjectile => Main.projectile[(int)NPC.ai[3]];
        private int suppTextCooldown = 0;

        public int GoldRushCount = 0;
        public int TwilightRushTime = 0;
        public const float GOLDRUSH4SPEED = 22f;
        public const int GOLDRUSH4DELAY = 20;

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

        private float DaCapoState
        {
            get { return NPC.ai[3]; }
            set { NPC.ai[3] = value; }
        }

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
            // Initialize Skeleton Red Mist, This uses an older skeleton model than the Utils one so bear with it. Its their grandad
            if (skelly == null)
            {
                AiState = -1;
                skelly = new RedMistSkeletonHelper
                        (new Vector2(NPC.Center.X, NPC.position.Y + NPC.height),
                         new Vector2(0, -72),
                         new Vector2(26, -14),
                         new Vector2(26, 2),
                         12,
                         16,
                         new Vector2(-4, -10),
                         new Vector2(-4, 2),
                         30,
                         42);
                return;
            }

            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
            }
            Player player = Main.player[NPC.target];
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
            Vector2 target = NPC.Center;
            /*if (Main.player[NPC.target].dead && NPC.timeLeft > 2)
            {
                NPC.timeLeft = 2;
                NPC.position = Vector2.Zero;
                return;
            }*/
            target = NPC.GetTargetData().Center;
            float healthPercent = NPC.life / (float)NPC.lifeMax;

            // Uncomment these below to test out different phases

            //NPC.spriteDirection = 1;
            //ChangeAnimation(AnimationState.TwilightChase);
            //return;
            //return;
            /*
            if (NPC.ai[0] == 0)
            {
                NPC.ai[0] = 2;
                AiState = 10;
                NPC.ai[3] = -1;
                NPC.localAI[1] = 2;
                ChangeAnimation(AnimationState.Phase4Transition);
                Talk("Shift3", NPC.spriteDirection);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<RedMistEye>(), 0, 0, 0, NPC.whoAmI);
            }*/
            /*
            if (Phase < 0)
            {
                ChangeAnimation(AnimationState.Intro);
                Timer++;
                if (Timer > 120)
                {
                    Phase = 0;
                    Timer = 0;
                    NPC.netUpdate = true;
                }
            }*/
            // First Phase - Lobcorp Phase
            if (Phase == 0)
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
            // Second Phase
            // Changes to Mimicry and DaCapo
            // Allows Heaven Strikes as anti air
            // Focused on closing her distance between you and her
            // Allows DaCapo toss as anti range
            // Dashes through incoming Projectiles, gaining Projectile invincibility
            // Teleports to DaCapo and unleashes a 180 degree slash depending on player direction
            // Range needed to deal full damage shrinks
            else if (Phase == 1)
            {
                if (AiState == FollowState)
                {
                    FollowMode2();
                }
                else if (AiState == SwingMimicry)
                {
                    SwingingMimicry();
                }
                else if (AiState == SwingDaCapo)
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
                        skelly.Weapon1.Visible = true;
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

                else if (AiState == SpecialAttackStart)
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
                        skelly.Weapon1.Visible = true;
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
                        NPC.Center = p.Center;// + new Vector2(200, 0).RotateRandom(6.28f);
                        p.Kill();
                        NPC.ai[3] = -1;
                        Talk("Teleport3", NPC.spriteDirection);
                        NPC.netUpdate = true;
                    }
                }
                else if (AiState == SpecialAttackStart + 2)
                {
                    if (Timer == 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Phase2_CastAtk") with { Volume = 0.5f }, NPC.position);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 32; i++)
                            {
                                float angle = 6.28f / 32;

                                Vector2 vel = new Vector2(8, 0).RotatedBy(angle * i);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), skelly.Weapon1.Position(NPC.spriteDirection) + vel * 3, vel, ModContent.ProjectileType<RedMistMimicryHello>(), 30, 2);
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
            // Third Phase
            // Throw Mimicry and DaCapo forward and equip Smile and Justitia
            // Back to normal walking speed
            // Smile forces all players to plummet down if in midair or go through platforms if above
            // Smile inflicts "Horrifying Screech", disabling wings and lowers movement speed by 8%     
            // Smile second hit releases shockwaves with infinite range that is blocked by direct line of sight, signified by black dust hitting tiles
            // Getting hit by Smile shockwave reduces movement by 92% for 2 seconds
            // Justitia Releases Energy Slashes, sometimes two or three hit combos, has a higher end lag when doing three hit combos
            // Gold Rush has random center positions with different angles, ending with a strike from above
            // Range needed to deal full damage expands
            else if (Phase == 2)
            {
                RedMistPhase3();
            }
            // Instead of using Gold Rush a third time before transitioning, begins transition immediently
            // Smile gets used one last time, Justitia transforms into Twilight
            // Gold Rush gets summoned, and thrown as a Projectile with half the speed of red mist, creating random barriers all around the player
            // Red Mist gains semi flight with low gravity, runs as normal, can dash
            // Red Mist leaves behind numerous slashes when dashing
            // Red Mist deals NO contact damage at all
            // Red Mist relies on passing through players
            // Red Mist sometimes enters a portal to pass through it
            // Red Mist sometimes stops and redirects her velocity towards the player
            // After 25 seconds, she stops, falls to the ground, and lays motionless for 10 seconds with increased damage taken, before attacking again
            else if (Phase == 3)
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
                    }

                    if (((NPC.GetTargetData().Center - NPC.Center).Length() > 2000f && Main.rand.NextBool(360)) || Aggression > 300 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        AiState = TwilightTeleport;
                        Aggression = 0;
                        NPC.netUpdate = true;
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
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), SlashPosition, Vector2.Zero, ModContent.ProjectileType<Projectiles.RedMistSlashes>(), 43, 0);
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
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<Projectiles.RedMistSlashSpawner>(), 43, 0);
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
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<Projectiles.RedMistSlashSpawner>(), 43, 0);
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
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), SlashPosition, Vector2.Zero, ModContent.ProjectileType<Projectiles.RedMistSlashes>(), 43, 0);
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
            //Death Phase
            else if (Phase == 4)
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
            
            // Hidden Red Mist Phase??
            // For the worthy phase!
            // Hold Apocalypse up high, envelopes the area in darkness as it dissipates, red mist suddenly gets a burst of red mist, as Mimicry is summoned and blazes of red flames replaces her hair and gains a burning red cloak
            // Can throw mimicry spear form multiple times when player is away
            // Can Transform mimicry into a scythe to for wide slashes,
            // Can Transform mimicry spear form to poke
            // Can Transform mimicry hammer form as heavy hitter

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

            ScreenFilterHandler();
            bubbleShieldDustDisplay((int)Phase);

            // Music Changer
            if (Phase > 0)
            {
                if (Phase < 3)
                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/TilaridsDistortedNight");
                else
                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/TilaridsInsigniaDecay");
            }

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

        const int TeleportToPlayer = -1;

        const int FollowState = 0;
        const int SwingPenitence = 1;
        const int SwingRedEyes = 2;
        const int SwingBoth = 3;
        const int GoldRushBig = 4;
        const int GoldRushSmall = 8;
        const int SwitchPhase = 10;

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
            healthPercent -= 0.75f - ( 0.25f * Phase);

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
                        Vector2 pos = skelly.Weapon1.EndPoint(NPC.spriteDirection);
                        
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
                        Vector2 pos = skelly.Weapon1.EndPoint(NPC.spriteDirection);

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), skelly.Weapon2.EndPoint(NPC.spriteDirection), Vector2.Zero, ModContent.ProjectileType<RedMistMelee>(), 25, 2);

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

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), skelly.Weapon1.Position(NPC.direction), NPC.velocity, ModContent.ProjectileType<GoldRushRedMistImpact>(), 30, 10);
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
                    int i = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.GetTargetData().Center + new Vector2(400 * NPC.spriteDirection, Main.LocalPlayer.height / 2 - 50), new Vector2(-22f * NPC.spriteDirection, 0), ModContent.ProjectileType<Projectiles.KingPortal.RoadOfGold>(), 0, 0, -1, -2);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(320 * NPC.spriteDirection, 0), Vector2.Zero, ModContent.ProjectileType<Projectiles.KingPortal.RoadOfGold>(), 0, 0, -1, i);
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
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), skelly.Weapon1.Position(NPC.direction), NPC.velocity, ModContent.ProjectileType<GoldRushRedMistImpact>(), 30, 10);
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
                Vector2 position = skelly.Weapon1.Position(NPC.spriteDirection) + new Vector2(100, 0).RotatedBy(skelly.Weapon1.GetRotation(NPC.spriteDirection));

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
                    Vector2 pos = skelly.UpperArmR.Position(NPC.spriteDirection);
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
                Vector2 position = skelly.Weapon2.Position(NPC.spriteDirection) + new Vector2(40, 0).RotatedBy(skelly.Weapon2.GetRotation(NPC.spriteDirection));
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

        bool IsDaCapoHeld { get
            {
                // Edge case if Da Capo despawns, so the fight can move forwards
                if (NPC.ai[3] >= 0)
                {
                    Projectile p = Main.projectile[(int)NPC.ai[3]];
                    if (!p.active || p.type != ModContent.ProjectileType<DaCapoThrow>() || (int)p.ai[0] != NPC.whoAmI)
                        NPC.ai[3] = -1;
                }
                return NPC.ai[3] < 0; 
            } }

        const int SwingSmile = 4;

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
                    int i = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.GetTargetData().Center + new Vector2(400 * NPC.spriteDirection, Main.LocalPlayer.height / 2 - 50), new Vector2(-22f * NPC.spriteDirection, 0), ModContent.ProjectileType<Projectiles.KingPortal.RoadOfGold>(), 0, 0, -1, -2);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(320 * NPC.spriteDirection, 0), Vector2.Zero, ModContent.ProjectileType<Projectiles.KingPortal.RoadOfGold>(), 0, 0, -1, i);
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

        void RedMistPhase3()
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
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(320 * NPC.spriteDirection, 0), Vector2.Zero, ModContent.ProjectileType<Projectiles.KingPortal.RoadOfGold>(), 0, 0, -1, -3, 8);
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
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), skelly.Weapon1.Position(NPC.direction), NPC.velocity, ModContent.ProjectileType<GoldRushRedMistImpact>(), 30, 10);
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

            Vector2 hammerPos = skelly.Weapon2.Position(NPC.spriteDirection) + new Vector2(70, 0).RotatedBy(skelly.Weapon2.GetRotation(NPC.spriteDirection));
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

            Vector2 hammerPos = skelly.Weapon2.Position(NPC.spriteDirection) + new Vector2(70, 0).RotatedBy(skelly.Weapon2.GetRotation(NPC.spriteDirection));
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
                    Vector2 dustPos = new Vector2(70, 0).RotatedBy(skelly.Weapon2.GetRotation(NPC.spriteDirection));
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
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), hammerPos, vel, ModContent.ProjectileType<Projectiles.SmileBits>(), 2, 0, -1, Main.rand.NextFloat(0.26f));
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
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + velocity * GOLDRUSH4DELAY, velocity, ModContent.ProjectileType<Projectiles.KingPortal.RoadOfKing>(), 0, 0, -1, -1, 170 - GOLDRUSH4DELAY);

                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/Gebura/Gebura_Teleport_Start") with { Volume = 0.5f }, NPC.position);
                }

                Timer++;

                if (Timer == 160)
                {
                    Vector2 velocity = new Vector2(GOLDRUSH4SPEED * NPC.spriteDirection, 0);
                    //if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<Projectiles.KingPortal.GoldRushRedMist>(), 45, 1f, -1, 10);
                }

                if (Timer > 200)
                {
                    Timer = 0;
                    AiState++;
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
                    int type = ModContent.ProjectileType<Projectiles.RedMistStrikes>();
                    if (Main.rand.NextBool(5))
                    {
                        position += velocity * 15;
                        velocity *= 0;
                        type = ModContent.ProjectileType<Projectiles.RedMistSlashes>();
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

            Vector2 pos = skelly.Weapon1.Position(NPC.spriteDirection) + new Vector2(40 + Main.rand.Next(90), 0).RotatedBy(skelly.Weapon1.GetRotation(NPC.spriteDirection));
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
                            if (validTile && Main.tileSolid[(int)Main.tile[x,y].TileType] && !Collision.SolidTiles(x-3, x+3, y-5, y-1))
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
                Timer -= 30;
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

        private void bubbleShieldRange(int phase, ref float min, ref float max)
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

        public override void FindFrame(int frameHeight)
        {
            //The Animation part for pure pain
            if (skelly != null)
            {
                Vector2 origin = new Vector2(NPC.Center.X, NPC.position.Y + NPC.height);
                skelly.Origin.BoneOffset = origin;
                //skelly.CalculateHandIK();
                bool LookAtPlayer = true;
                NPC.frameCounter++;
                if (animState == AnimationState.Intro)
                {
                    if (NPC.frameCounter < 30)
                    {
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength() - 4, 0).RotatedBy(MathHelper.ToRadians(-200)), 100);
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(4, skelly.ArmLength() - 1), 100);
                        skelly.CalculateHandIK();

                        skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation - MathHelper.ToRadians(45), 1f);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-skelly.LowerLegL.length, -4), 100);
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(skelly.UpperLegR.length + 12, -4), 100);

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-50), 1f);
                        skelly.Pelvis.ChangeBoneOffset(new Vector2(14, -skelly.UpperLegL.length), 100);

                        skelly.CalculateLegIK();

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(140), 1f);

                        skelly.Head.rotation = NPC.spriteDirection == 1 ? -0.79f : -2.35f;
                        LookAtPlayer = false;
                    }
                    else
                    {
                        skelly.Weapon1.Visible = true;
                        skelly.Weapon2.Visible = true;
                        skelly.Gauntlet.Visible = false;

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0), 1);
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0),1 );

                        skelly.CalculateLegIK();

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(-4, skelly.ArmLength() - 1), 1);
                        skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-75);// * NPC.spriteDirection);
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(4, skelly.ArmLength() - 1), 1);
                        skelly.Weapon2.rotation = skelly.LowerArmR.rotation + MathHelper.ToRadians(-45);// * NPC.spriteDirection);

                        skelly.CalculateHandIK();

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))), 1);
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                    }
                }
                else if (animState == AnimationState.Idle1)
                {
                    skelly.Weapon1.Visible = true;
                    skelly.Weapon2.Visible = true;
                    skelly.Gauntlet.Visible = false;

                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                    skelly.CalculateLegIK();

                    skelly.HandLIK.ChangeBoneOffset(new Vector2(-4, skelly.ArmLength() - 1));
                    skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-75);// * NPC.spriteDirection);
                    skelly.HandRIK.ChangeBoneOffset(new Vector2(4, skelly.ArmLength() - 1));
                    skelly.Weapon2.rotation = skelly.LowerArmR.rotation + MathHelper.ToRadians(-45);// * NPC.spriteDirection);

                    skelly.CalculateHandIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));
                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                }
                else if (animState == AnimationState.Walk1)
                {
                    float x = 16 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4));// * NPC.spriteDirection;
                    float y = 6 * (float)Math.Cos(MathHelper.ToRadians((float)NPC.frameCounter * 4));
                    if (y < 0)
                        y = 0;
                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12 + x, y * -1));
                    y = 6 * (float)Math.Cos(MathHelper.ToRadians((float)NPC.frameCounter * 4));
                    if (y > 0)
                        y = 0;
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0 - x, y));

                    skelly.HandLIK.ChangeBoneOffset(new Vector2(-4, skelly.ArmLength() - 1));
                    skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-75);// * NPC.spriteDirection);
                    skelly.HandRIK.ChangeBoneOffset(new Vector2(4, skelly.ArmLength() - 1));
                    skelly.Weapon2.rotation = skelly.LowerArmR.rotation + MathHelper.ToRadians(-45);// * NPC.spriteDirection);

                    skelly.CalculateHandIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -74 + 1f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 8))));
                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90));

                    skelly.CalculateLegIK();
                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(100));
                }
                else if (animState == AnimationState.MidAir)
                {
                    if (NPC.velocity.Y > 0)
                    {
                        Vector2 originLeg = skelly.UpperLegL.Position() - skelly.Origin.Position();
                        skelly.FeetLIK.ChangeBoneOffset(originLeg + new Vector2(skelly.LegLength() - 6, 0).RotatedBy(MathHelper.ToRadians(80)), 0.5f);
                        skelly.FeetRIK.ChangeBoneOffset(originLeg + new Vector2(skelly.LegLength() * 0.66f, 0).RotatedBy(MathHelper.ToRadians(80)), 0.5f);

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(-16, skelly.ArmLength() - 24), 0.5f);
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(-12, skelly.ArmLength() - 24), 0.5f);

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-80));
                    }
                    else
                    {
                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-8, 0), 0.5f);
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(4, 0), 0.5f);

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(-16, skelly.ArmLength() - 12));
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(-12, skelly.ArmLength() - 12));

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90));

                        LookAtPlayer = false;
                        skelly.Head.ChangeBoneRotation(-1.57f + 0.3f * NPC.spriteDirection);
                    }

                    skelly.CalculateHandIK();
                    skelly.CalculateLegIK();
                }
                else if (animState == AnimationState.SwingRedEyes)
                {
                    if (NPC.frameCounter < 20)
                    {
                        float angle = 1.57f - ((float)NPC.frameCounter / 19f) * 3.14f;// * NPC.spriteDirection;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 6);

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90 - 6)); // * NPC.spriteDirection));
                    }
                    else if (NPC.frameCounter < 30)
                    {
                        float angle = -1.57f + (((float)NPC.frameCounter - 20) / 10) * 4.71f;// * NPC.spriteDirection;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-80));// * NPC.spriteDirection));
                    }
                    else if (NPC.frameCounter < 50)
                    {
                        float angle = 3.14f;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);
                    }
                    else
                    {
                        ChangeAnimation(0);
                    }
                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                    skelly.HandRIK.ChangeBoneOffset(new Vector2(4, skelly.ArmLength() - 1));

                    skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.rotation - MathHelper.ToRadians(45));// * NPC.spriteDirection);
                    skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation - 1.309f, 1.57f);// * NPC.spriteDirection, 1.57f);

                    skelly.CalculateLegIK();
                    skelly.CalculateHandIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                }
                else if (animState == AnimationState.SwingPenitence)
                {
                    if (NPC.frameCounter < 20)
                    {
                        float angle = 1.57f - ((float)NPC.frameCounter / 19f) * 3.14f;
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 6);

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90 - 6));// * NPC.spriteDirection));
                    }
                    else if (NPC.frameCounter < 30)
                    {
                        float angle = -1.57f + (((float)NPC.frameCounter - 20) / 10) * 4.71f;// * NPC.spriteDirection;
                        //Main.NewText(MathHelper.ToDegrees(angle));
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-80));// * NPC.spriteDirection));
                    }
                    else if (NPC.frameCounter < 50)
                    {
                        float angle = 3.14f;
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);
                    }
                    else
                    {
                        ChangeAnimation(0);
                    }
                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                    skelly.HandLIK.ChangeBoneOffset(new Vector2(-4, skelly.ArmLength() - 1));

                    skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.rotation - MathHelper.ToRadians(45), 1.57f);
                    skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation - 1.309f, 1.57f);

                    skelly.CalculateLegIK();
                    skelly.CalculateHandIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                }
                else if (animState == AnimationState.SwingBoth)
                {
                    if (NPC.frameCounter < 20)
                    {
                        float angle = 1.57f - ((float)NPC.frameCounter / 19f) * 3.14f;// * NPC.spriteDirection;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 6);
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 6);

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90 - 6));// * NPC.spriteDirection));
                    }
                    else if (NPC.frameCounter < 30)
                    {
                        float angle = -1.57f + (((float)NPC.frameCounter - 20) / 10) * 4.71f;// * NPC.spriteDirection;
                        //Main.NewText(MathHelper.ToDegrees(angle));
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-80));// * NPC.spriteDirection));
                    }
                    else if (NPC.frameCounter < 50)
                    {
                        float angle = 3.14f;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);
                    }
                    else
                    {
                        ChangeAnimation(0);
                    }
                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                    skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.rotation - MathHelper.ToRadians(45), 1.57f);
                    skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation - 1.309f, 1.57f);

                    skelly.CalculateLegIK();
                    skelly.CalculateHandIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                }
                else if (animState == AnimationState.GoldRushIntro)
                {
                    skelly.Weapon1.Visible = false;
                    skelly.Weapon2.Visible = false;
                    skelly.Gauntlet.Visible = true;
                    if (NPC.frameCounter == 1)
                    {
                        skelly.Gauntlet.scale = 1f;

                    }
                    if (skelly.Gauntlet.scale < 2f)
                        skelly.Gauntlet.scale = 1f + 1f * ((float)NPC.frameCounter / 30f);

                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-54, 0));
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(40, 0));
                    skelly.CalculateLegIK();

                    skelly.HandLIK.ChangeBoneOffset(new Vector2(-16 + Main.rand.NextFloat(-1, 1), 6 + Main.rand.NextFloat(-1, 1)));
                    skelly.HandRIK.ChangeBoneOffset(new Vector2(-16, 6));
                    skelly.CalculateHandIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -50));
                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-75 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));


                    skelly.Head.rotation = NPC.spriteDirection == 1 ? -1.57f : -1.57f;

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(110));

                    if (NPC.frameCounter > 30)
                    {
                        ChangeAnimation(AnimationState.GoldRushLoop);
                        skelly.Gauntlet.scale = 2f;
                    }
                }
                else if (animState == AnimationState.GoldRushLoop)
                {
                    float x = 102 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 16));// * NPC.spriteDirection;
                    float y = 20 * (float)Math.Cos(MathHelper.ToRadians((float)NPC.frameCounter * 16));
                    if (y < 0)
                        y = 0;
                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-18 + x, y * -1), 12);
                    y = 20 * (float)Math.Cos(MathHelper.ToRadians((float)NPC.frameCounter * 16));
                    if (y > 0)
                        y = 0;
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0 - x, y), 12);
                    skelly.CalculateLegIK();

                    float rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X * NPC.spriteDirection);
                    skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(rotation), 16);
                    skelly.HandRIK.ChangeBoneOffset(new Vector2(-skelly.UpperArmR.length, skelly.LowerArmL.length));
                    skelly.CalculateHandIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -50 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 16))));
                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-50 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(140));
                }
                else if (animState == AnimationState.GoldRushLoopFall)
                {

                }
                else if (animState == AnimationState.GoldRushEnd)
                {
                    skelly.Weapon1.Visible = false;
                    skelly.Weapon2.Visible = false;
                    skelly.Gauntlet.Visible = true;
                    skelly.Gauntlet.scale = 1f;

                    skelly.HandLIK.ChangeBoneOffset(new Vector2(-14, 26));
                    skelly.HandRIK.ChangeBoneOffset(new Vector2(-16, 6));
                    skelly.CalculateHandIK();

                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(skelly.UpperLegL.length - 6, 0));
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(-skelly.LowerArmL.length - 20, 0));
                    skelly.CalculateLegIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -skelly.UpperLegL.length - 4));
                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-73 + 3 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 10))));

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(100));
                }
                else if (animState == AnimationState.Phase2Transition)
                {
                    skelly.Weapon1.Visible = true;
                    skelly.Weapon2.Visible = true;
                    skelly.Gauntlet.Visible = false;
                    if (NPC.frameCounter < 30)
                    {
                        float factor = ((float)NPC.frameCounter / 30f);
                        float angle = 1.57f - factor * 3.14f;// * NPC.spriteDirection;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-16, 0));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-94));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -80));
                    }
                    else if (NPC.frameCounter < 40)
                    {
                        NPC.localAI[1] = 1;

                        float factor = (((float)NPC.frameCounter - 30) / 10f);
                        float angle = -1.57f + factor * 4.17f;// * NPC.spriteDirection;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-26, 0));

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-80));
                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -80 + factor * 2));
                    }
                    else if (NPC.frameCounter < 60)
                    {
                        NPC.localAI[1] = 1;

                        float angle = 3.14f;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 6);
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 6);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-26, 0));
                    }
                    else
                    {
                        NPC.localAI[1] = 1;
                        ChangeAnimation(AnimationState.Idle2);
                    }

                    skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-75);// * NPC.spriteDirection);
                    skelly.Weapon2.rotation = skelly.LowerArmR.rotation + MathHelper.ToRadians(-45);// * NPC.spriteDirection);
                    skelly.CalculateHandIK();

                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                    skelly.CalculateLegIK();

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                }
                else if (animState == AnimationState.Idle2)
                {
                    NPC.localAI[1] = 1;
                    skelly.Weapon1.Visible = true;
                    //skelly.Weapon2.Visible = true;
                    skelly.Gauntlet.Visible = false;

                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                    skelly.CalculateLegIK();

                    skelly.HandLIK.ChangeBoneOffset(new Vector2(-4, skelly.ArmLength() - 1));
                    skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-50);// * NPC.spriteDirection);
                    skelly.HandRIK.ChangeBoneOffset(new Vector2(4, skelly.ArmLength() - 1));
                    skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.rotation + MathHelper.ToRadians(135), 0.3f);// * NPC.spriteDirection);

                    skelly.CalculateHandIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));
                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                }
                else if (animState == AnimationState.Walk2)
                {
                    skelly.Weapon1.Visible = true;

                    float speed = 10;// * (Math.Abs(NPC.velocity.X) / 5f);
                    if (speed > 10)
                        speed = 10;

                    float x = 30 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * speed));// * NPC.spriteDirection;
                    float y = 20 * (float)Math.Cos(MathHelper.ToRadians((float)NPC.frameCounter * speed - 20));
                    if (y < 0)
                        y = 0;
                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12 + x, y * -1));
                    y = 14 * (float)Math.Cos(MathHelper.ToRadians((float)NPC.frameCounter * speed - 20));
                    if (y > 0)
                        y = 0;
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0 - x, y));

                    skelly.HandLIK.ChangeBoneOffset(new Vector2(-16, skelly.ArmLength() - 12));
                    skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation + MathHelper.ToRadians(-50), 0.2f);
                    skelly.HandRIK.ChangeBoneOffset(new Vector2(-12, skelly.ArmLength() - 12));
                    skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.rotation + MathHelper.ToRadians(135), 0.2f);
                    skelly.CalculateHandIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(5, -72 + 1f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * speed * 2))));
                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-70 + 3 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));

                    skelly.CalculateLegIK();
                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(120));
                }
                else if (animState == AnimationState.Dash)
                {
                    skelly.HandLIK.ChangeBoneOffset(new Vector2(-skelly.ArmLength(), 0));
                    skelly.Weapon1.ChangeBoneRotation(MathHelper.ToRadians(105), 0.12f);
                    skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(MathHelper.ToRadians(135)));
                    skelly.Weapon2.rotation = skelly.LowerArmR.rotation + MathHelper.ToRadians(180);
                    skelly.CalculateHandIK();

                    Vector2 point = skelly.Origin.DifferenceBone(skelly.UpperLegL) + new Vector2(skelly.LegLength() - 2f, 0).RotatedBy(MathHelper.ToRadians(165));
                    skelly.FeetLIK.ChangeBoneOffset(point, 32);
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, point.Y), 16);
                    skelly.CalculateLegIK();

                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-30));
                    skelly.Pelvis.ChangeBoneOffset(new Vector2(5, -40));

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(160));
                }
                else if (animState == AnimationState.SwingMimicry)
                {
                    skelly.Weapon1.Visible = true;
                    if (NPC.frameCounter < 20)
                    {
                        float angle = 1.57f - ((float)NPC.frameCounter / 19f) * 3.14f;// * NPC.spriteDirection;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 6);
                        skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation - MathHelper.ToRadians(45), 1.57f);// * NPC.spriteDirection);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90 - 6)); // * NPC.spriteDirection));
                    }
                    else if (NPC.frameCounter < 60)
                    {
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength() + Main.rand.NextFloat(-0.5000f, 0.5000f), Main.rand.NextFloat(-0.5000f, 0.5000f)).RotatedBy(-1.57f), 6);
                        skelly.Weapon1.scale = 1f + 1f * ((float)NPC.frameCounter - 20) / 30f;
                        skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation - MathHelper.ToRadians(45), 1.57f);// * NPC.spriteDirection);

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));
                    }
                    else if (NPC.frameCounter < 70)
                    {
                        float angle = -1.57f + (((float)NPC.frameCounter - 60) / 10) * 5.06f;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);
                        skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation - MathHelper.ToRadians(15), 1.57f);// * NPC.spriteDirection);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-28, 0));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(-10, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-80));// * NPC.spriteDirection));
                    }
                    else if (NPC.frameCounter < 90)
                    {
                        skelly.Weapon1.scale = 2f - 1f * ((float)NPC.frameCounter - 70) / 10f;
                        if (skelly.Weapon1.scale < 1f)
                            skelly.Weapon1.scale = 1f;
                        skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation - MathHelper.ToRadians(15), 1.57f);

                        float angle = 3.49f;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-28, 0));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(-10, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));
                    }
                    else
                    {
                        ChangeAnimation(AnimationState.Idle2);
                    }
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                    skelly.HandRIK.ChangeBoneOffset(new Vector2(4, skelly.ArmLength() - 1));

                    skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.rotation + MathHelper.ToRadians(135), 1.57f);// * NPC.spriteDirection, 1.57f);

                    skelly.CalculateLegIK();
                    skelly.CalculateHandIK();


                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                }
                else if (animState == AnimationState.SwingDaCapo)
                {
                    float progress = ((float)NPC.frameCounter % 30) / 10f;
                    if (progress > 1f)
                        progress = 1f;
                    if (NPC.frameCounter < 30)
                    {
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(1.57f - 3.14f * progress), 16);
                        skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.GetRotation(), 1.2f);

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-96));
                    }
                    else if (NPC.frameCounter < 40)
                    {
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(-1.57f + 4.71239f * progress), 16);
                        skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.GetRotation() - MathHelper.ToRadians(60), 1.2f);

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-84));
                    }
                    else if (NPC.frameCounter < 60)
                    {
                        progress = (((float)NPC.frameCounter - 40) / 20f);

                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(3.14f - 0.6f * progress));
                        skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.GetRotation() - MathHelper.ToRadians(60), 1.2f);

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-84));
                    }
                    else if (NPC.frameCounter < 90)
                    {
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(3.14f - 4.71239f * progress), 16);
                        skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.GetRotation(), 1.2f);

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-96));
                    }
                    else if (NPC.frameCounter < 100)
                    {
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(-1.57f + 3.14f * progress), 5);
                        skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.rotation + MathHelper.ToRadians(135), 0.2f);

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90));
                    }

                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                    skelly.HandLIK.ChangeBoneOffset(new Vector2(-4, skelly.ArmLength() - 1));
                    skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation + MathHelper.ToRadians(-50), 0.3f);

                    skelly.CalculateHandIK();
                    skelly.CalculateLegIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                }
                else if (animState == AnimationState.DaCapoThrow)
                {
                    if (NPC.frameCounter < 15)
                    {
                        float angle = 1.57f - ((float)NPC.frameCounter / 15f) * 3.14f;
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 6);
                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                        skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.GetRotation() + MathHelper.ToRadians(60));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))), 1);
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90 - 6));
                    }
                    else if (NPC.frameCounter < 50)
                    {
                        float prog = ((float)NPC.frameCounter - 15) / 35f;
                        float angle = -1.57f - (prog * MathHelper.ToRadians(10));
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 6);
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                        skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.GetRotation() + MathHelper.ToRadians(60));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))), 1);
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-96 - 3 * prog));
                    }
                    else if (NPC.frameCounter < 60)
                    {
                        skelly.Weapon2.Visible = false;

                        float prog = ((float)NPC.frameCounter - 50) / 10f;
                        float angle = -1.57f - MathHelper.ToRadians(10) + (prog * MathHelper.ToRadians(270));
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(14, 0), 4);
                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-36, 0), 4);

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(-10, -65 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))), 1);
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-60), 0.32f);
                    }
                    else
                    {
                        float angle = -1.57f - MathHelper.ToRadians(10) + MathHelper.ToRadians(270);
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 6);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-36, 0), 4);
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(14, 0), 4);

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(-10, -65 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))), 1);
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-60), 0.32f);
                    }

                    skelly.CalculateLegIK();
                    skelly.CalculateHandIK();

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(110));
                }
                else if (animState == AnimationState.HeavenThrow)
                {
                    skelly.Weapon2.Visible = false;
                    if (NPC.frameCounter < 50)
                    {
                        skelly.Weapon1.Visible = true;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(-skelly.UpperArmL.length, -skelly.LowerArmL.length));
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0));
                        skelly.Weapon1.ChangeBoneRotation(Main.rand.NextFloat(-0.08f, 0.08f), 0.5f);
                        skelly.Weapon1.scale = 0.2f + 0.8f * ((float)NPC.frameCounter/50f);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12 - 10 - skelly.LowerLegL.length, 0), 5);
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(-12 + 2 + skelly.UpperLegL.length, 0), 5);

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(-12, -46));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-100));

                        skelly.CalculateHandIK(-1);

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                    }
                    else
                    {
                        skelly.Weapon1.Visible = false;
                        if (NPC.frameCounter < 60)
                        {
                            float progress = ((float)NPC.frameCounter - 50)/9f;
                            skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(MathHelper.ToRadians(-90 + 270 * progress)), 16);
                            skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(MathHelper.ToRadians(110)), 8);

                            skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-100 + 40 * progress), 1.2f);

                            skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(180), 0.12f);
                        }
                        else
                        {
                            skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(130));
                        }
                        
                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(12 + skelly.LowerLegL.length, 0), 16);
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(-6 - skelly.UpperLegL.length, 0), 16);
                        skelly.Pelvis.ChangeBoneOffset(new Vector2(20, -46));
                        LookAtPlayer = false;
                        skelly.Head.ChangeBoneRotation(MathHelper.ToRadians(NPC.spriteDirection == 1 ? -45 : -135), 0.8f);

                        skelly.CalculateHandIK();
                    }
                    skelly.CalculateLegIK();
                }
                else if (animState == AnimationState.MimicryPrepare)
                {
                    skelly.Weapon2.Visible = false;
                    if (NPC.frameCounter < 30)
                    {
                        float prog = (float)NPC.frameCounter / 30f;

                        Vector2 handR = new Vector2(skelly.ArmLength() - 6, 0).RotatedBy(MathHelper.ToRadians(-80 + 170 * (1f - prog)));
                        skelly.HandLIK.ChangeBoneOffset(handR, 8f);
                        skelly.Weapon1.ChangeBoneRotation(handR.ToRotation() - MathHelper.ToRadians(90 + Main.rand.NextFloat(-1f, 1f)), 1.2f);

                        skelly.HandRIK.ChangeBoneOffset(handR + new Vector2(-10, 0).RotatedBy(skelly.Weapon1.GetRotation()), 8f);
                    }
                    else
                    {
                        Vector2 handR = new Vector2(skelly.ArmLength() - 6, 0).RotatedBy(MathHelper.ToRadians(-80));
                        skelly.Weapon1.ChangeBoneRotation(handR.ToRotation() - MathHelper.ToRadians(90 + Main.rand.NextFloat(-1f, 1f)), 1.2f);
                        skelly.HandRIK.ChangeBoneOffset(handR + new Vector2(-10, 0).RotatedBy(skelly.Weapon1.GetRotation()), 8f);
                    }

                    if (NPC.frameCounter < 50)
                    {
                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-42, 0));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(16, 0));
                    }

                    if (NPC.frameCounter < 60)
                    {
                        float weaponScale = (float)NPC.frameCounter / 60f;

                        skelly.Weapon1.scale = 1f + 1f * (float)Math.Sin(1.57f * weaponScale);
                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -68 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-95));
                    }
                    else if (NPC.frameCounter >= 70)
                    {
                        float weaponScale = ((float)NPC.frameCounter - 70f)/ 60f;
                        if (weaponScale > 1f)
                            weaponScale = 1f;

                        skelly.Weapon1.scale = 2f + 2f * (float)Math.Sin(1.57f * weaponScale);
                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -68 + (weaponScale * 12f) + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));

                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-80));
                    }

                    skelly.CalculateHandIK();
                    skelly.CalculateLegIK();
                }
                else if (animState == AnimationState.MimicryGreaterSplitV)
                {
                    Vector2 originLeg;
                    if (NPC.frameCounter == 1)
                    {
                        Vector2 handR = new Vector2(skelly.ArmLength(), 0).RotatedBy(MathHelper.ToRadians(-80));
                        skelly.HandLIK.BoneOffset = handR;

                        skelly.Weapon1.rotation = handR.ToRotation() - MathHelper.ToRadians(90);
                        skelly.HandRIK.BoneOffset = handR + new Vector2(-10, 0).RotatedBy(skelly.Weapon1.GetRotation());

                        originLeg = skelly.UpperLegL.Position() - skelly.Origin.Position();
                        skelly.FeetLIK.BoneOffset = originLeg + new Vector2(skelly.LegLength() - 6, 0).RotatedBy(MathHelper.ToRadians(60));
                        skelly.FeetRIK.BoneOffset = originLeg + new Vector2(skelly.LegLength()/2, 0).RotatedBy(MathHelper.ToRadians(60));

                        skelly.Pelvis.rotation = MathHelper.ToRadians(-100);
                        skelly.Pelvis.BoneOffset = new Vector2(0, -72);
                    }
                    if (NPC.frameCounter >= 15)
                    {
                        float scale = (((float)NPC.frameCounter - 15) / 20);
                        if (scale > 1f)
                            scale = 1f;

                        skelly.Weapon1.scale = 4f - 3f * scale;
                    }

                    float prog = (float)NPC.frameCounter / 10f;
                    if (prog > 1f)
                        prog = 1f;

                    Vector2 hand = new Vector2(skelly.ArmLength() - 6 * prog, 0).RotatedBy(MathHelper.ToRadians(-80 + 320f * (float)Math.Sin(1.57f * prog)));
                    skelly.HandLIK.BoneOffset = hand;

                    skelly.Weapon1.rotation = hand.ToRotation() - MathHelper.ToRadians(90);
                    skelly.HandRIK.BoneOffset = hand + new Vector2(-10, 0).RotatedBy(skelly.Weapon1.GetRotation());

                    originLeg = skelly.UpperLegL.Position() - skelly.Origin.Position();
                    skelly.FeetLIK.BoneOffset = originLeg + new Vector2(skelly.LegLength() - 6, 0).RotatedBy(MathHelper.ToRadians(60 + 15 * prog));
                    skelly.FeetRIK.BoneOffset = originLeg + new Vector2(skelly.LegLength() / 2, 0).RotatedBy(MathHelper.ToRadians(60 + 15 * prog));

                    skelly.Pelvis.rotation = MathHelper.ToRadians(-100 + 20 * prog);

                    skelly.CalculateHandIK();
                    skelly.CalculateLegIK();
                }
                else if (animState == AnimationState.Phase3Transition)
                {
                    if (NPC.frameCounter < 60)
                    {
                        float angle;
                        if (NPC.frameCounter < 20)
                        {
                            angle = 1.57f - ((float)NPC.frameCounter / 20f) * 3.14f;
                            skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);

                            skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.GetRotation() + MathHelper.ToRadians(60));

                        }
                        angle = 1.57f - ((float)NPC.frameCounter / 60f) * 2.35619f;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))), 1);
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-96));

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));
                    }
                    else if (NPC.frameCounter < 70)
                    {
                        skelly.Weapon1.Visible = false;
                        skelly.Weapon2.Visible = false;

                        float prog = (((float)NPC.frameCounter - 60f) / 10f);
                        float angle = -1.57f + 4.71239f * prog;
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);
                        angle = -0.786f + 3.92699f;
                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(angle), 16);

                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(14, 0), 4);
                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-36, 0), 4);

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(-10, -65 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))), 1);
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-60), 0.32f);
                    }
                    else if (NPC.frameCounter >= 130)
                    {
                        NPC.localAI[1] = 2;
                        skelly.Weapon1.Visible = true;
                        skelly.Weapon2.Visible = true;

                        /*float prog = (((float)NPC.frameCounter - 130f) / 30f);
                        if (prog > 1f)
                            prog = 1f;*/

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(-4, skelly.ArmLength() - 1));
                        skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-75);// * NPC.spriteDirection);

                        skelly.HandRIK.ChangeBoneOffset(new Vector2(8, 0).RotatedBy(-0.523599f));
                        skelly.Weapon2.rotation = skelly.LowerArmR.rotation + MathHelper.ToRadians(-60);// * NPC.spriteDirection);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))), 1);
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-96));
                    }

                    skelly.CalculateHandIK();
                    skelly.CalculateLegIK();
                }
                else if (animState == AnimationState.Idle3)
                {
                    NPC.localAI[1] = 2;

                    skelly.Weapon1.Visible = true;
                    skelly.Weapon2.Visible = true;
                    skelly.Gauntlet.Visible = false;

                    

                    skelly.CalculateLegIK();

                    skelly.HandLIK.ChangeBoneOffset(new Vector2(-4, skelly.ArmLength() - 1));
                    skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-75);// * NPC.spriteDirection);

                    skelly.HandRIK.ChangeBoneOffset(new Vector2(8, 0).RotatedBy(-0.523599f));
                    skelly.Weapon2.rotation = skelly.LowerArmR.rotation + MathHelper.ToRadians(-60);// * NPC.spriteDirection);

                    skelly.CalculateHandIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));
                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                }
                else if (animState == AnimationState.Walk3)
                {
                    NPC.localAI[1] = 2;

                    float x = 16 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4));// * NPC.spriteDirection;
                    float y = 6 * (float)Math.Cos(MathHelper.ToRadians((float)NPC.frameCounter * 4));
                    if (y < 0)
                        y = 0;
                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12 + x, y * -1));
                    y = 6 * (float)Math.Cos(MathHelper.ToRadians((float)NPC.frameCounter * 4));
                    if (y > 0)
                        y = 0;
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0 - x, y));

                    skelly.HandLIK.ChangeBoneOffset(new Vector2(-4, skelly.ArmLength() - 1));
                    skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-75);// * NPC.spriteDirection);

                    skelly.HandRIK.ChangeBoneOffset(new Vector2(8, 0).RotatedBy(-0.523599f));
                    skelly.Weapon2.rotation = skelly.LowerArmR.rotation + MathHelper.ToRadians(-60);// * NPC.spriteDirection);

                    skelly.CalculateHandIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -74 + 1f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 8))));
                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90));

                    skelly.CalculateLegIK();
                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(100));
                }
                else if (animState == AnimationState.JustitiaSwing)
                {
                    if (NPC.frameCounter < 45)
                    {
                        float prog = (float)NPC.frameCounter / 20;
                        if (prog > 1)
                            prog = 1;

                        Vector2 position = new Vector2( 0, skelly.ArmLength() - 1 - 6 * prog).RotatedBy(-1.57f * prog);
                        float rotation = (position).ToRotation() - 1.57f;
                        skelly.Weapon1.ChangeBoneRotation(rotation, 0.8f);

                        if (prog == 1)
                            position += new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                        skelly.HandLIK.ChangeBoneOffset(position);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));
                    }
                    else
                    {
                        float prog = ((float)NPC.frameCounter - 45)/ 6;
                        if (prog > 1)
                            prog = 1;

                        Vector2 position = new Vector2(-4, skelly.ArmLength() - 1).RotatedBy(-1.57f + 3.3f * (float)Math.Sin(1.57f * prog));
                        skelly.HandLIK.BoneOffset = position;
                        skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation + MathHelper.ToRadians(-75 - 15 * prog), 1.2f);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-24, 0));
                        skelly.Pelvis.ChangeBoneOffset(new Vector2(-4, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));
                    }

                    skelly.Weapon1.Visible = true;
                    skelly.Weapon2.Visible = true;
                    skelly.Gauntlet.Visible = false;

                    
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                    skelly.CalculateLegIK();

                    //skelly.HandLIK.ChangeBoneOffset(new Vector2(-4, skelly.ArmLength() - 1));
                    //skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-75);// * NPC.spriteDirection);

                    skelly.HandRIK.ChangeBoneOffset(new Vector2(8, 0).RotatedBy(-0.523599f));
                    skelly.Weapon2.rotation = skelly.LowerArmR.rotation + MathHelper.ToRadians(-60);// * NPC.spriteDirection);

                    skelly.CalculateHandIK();

                    //skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                }
                else if (animState == AnimationState.SmileSwing)
                {
                    //skelly.Weapon1.Visible = false;
                    Vector2 justitaPlacement = skelly.Origin.Position() + new Vector2(-36, -90);
                    if (NPC.frameCounter < 60)
                    {
                        float prog = (float)NPC.frameCounter / 60;
                        skelly.Weapon2.scale = 1f + (1f * prog);

                        skelly.HandLIK.ChangeBoneOffset(justitaPlacement - skelly.UpperArmL.Position());
                        skelly.Weapon1.ChangeBoneRotation(MathHelper.ToRadians(90));

                        Vector2 position = new Vector2(8 + 20 * prog, 0).RotatedBy(MathHelper.ToRadians(-30 - 30 * prog));
                        skelly.HandRIK.ChangeBoneOffset(position, 8f);

                        Vector2 offset = new Vector2(36 * prog, 0).RotatedBy(skelly.Weapon2.GetRotation() - skelly.LowerArmR.GetRotation());
                        skelly.Weapon2.BoneOffset = offset;
                        //float rotation = (position).ToRotation() + MathHelper.ToRadians(60);
                        //skelly.Weapon2.ChangeBoneRotation(rotation, 0.8f);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90));

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(100));
                    }
                    else if (NPC.frameCounter < 90)
                    {
                        float prog = ((float)NPC.frameCounter - 60) / 10;
                        if (prog > 1)
                            prog = 1;

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(-12, skelly.ArmLength() - 4));

                        Vector2 position = new Vector2(28 - 16 * prog, 0).RotatedBy(MathHelper.ToRadians(-60 + 180 * prog));
                        skelly.HandRIK.ChangeBoneOffset(position, 8f);

                        float rotation = (position).ToRotation() - MathHelper.ToRadians(80);
                        skelly.Weapon2.ChangeBoneRotation(rotation,0.8f);

                        Vector2 offset = new Vector2(36, 0).RotatedBy(skelly.Weapon2.GetRotation() - skelly.LowerArmR.GetRotation());
                        skelly.Weapon2.BoneOffset = offset;

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-48, 0), 8);
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(2, 0), 8);

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -62), 8);
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-45), 0.8f);

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(140));
                    }
                    else if (NPC.frameCounter < 150)
                    {
                        float prog = ((float)NPC.frameCounter - 90) / 60;
                        if (prog > 1)
                            prog = 1;

                        Vector2 position = new Vector2(12 + 16 * prog, 0).RotatedBy(MathHelper.ToRadians(120 - 210 * prog));
                        skelly.HandRIK.ChangeBoneOffset(position, 8f);

                        float rotation = (position).ToRotation() - MathHelper.ToRadians(80);
                        skelly.Weapon2.ChangeBoneRotation(rotation, 0.8f);

                        position += (skelly.UpperArmR.Position() - skelly.UpperArmL.Position()) + new Vector2(-16, 0).RotatedBy(skelly.Weapon2.GetRotation());
                        skelly.HandLIK.ChangeBoneOffset(position, 8f);

                        Vector2 offset = new Vector2(36, 0).RotatedBy(skelly.Weapon2.GetRotation() - skelly.LowerArmR.GetRotation());
                        skelly.Weapon2.BoneOffset = offset;

                            
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-45 - 10f * prog), 0.8f);

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(110));
                    }
                    else if (NPC.frameCounter < 240)
                    {
                        float prog = ((float)NPC.frameCounter - 150) / 10;
                        if (prog > 1)
                            prog = 1;

                        LookAtPlayer = false;
                        skelly.Head.ChangeBoneRotation(-1.57f + 0.875f * NPC.spriteDirection);

                        Vector2 position = new Vector2(28 - 8 * prog, 0).RotatedBy(MathHelper.ToRadians(-90 + 145 * prog));
                        skelly.HandRIK.ChangeBoneOffset(position, 8f);

                        float rotation = (position).ToRotation() - MathHelper.ToRadians(50);
                        skelly.Weapon2.ChangeBoneRotation(rotation, 0.8f);

                        position += (skelly.UpperArmR.Position() - skelly.UpperArmL.Position()) + new Vector2(-16, 0).RotatedBy(skelly.Weapon2.GetRotation());
                        skelly.HandLIK.ChangeBoneOffset(position, 8f);

                        Vector2 offset = new Vector2(36, 0).RotatedBy(skelly.Weapon2.GetRotation() - skelly.LowerArmR.GetRotation());
                        skelly.Weapon2.BoneOffset = offset;

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -42));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-10 + Main.rand.NextFloat(-5, 5)));

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(210));
                    }
                    else// if (NPC.frameCounter < 210)
                    {
                        float prog = ((float)NPC.frameCounter - 240) / 30;
                        if (prog > 1)
                            prog = 1;

                        skelly.Weapon2.scale = 2f - (1f * prog);

                        Vector2 position = new Vector2(20 - 12 * prog, 0).RotatedBy(MathHelper.ToRadians(55 - 85 * prog));
                        skelly.HandRIK.ChangeBoneOffset(position);
                        skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmR.rotation + MathHelper.ToRadians(-60), 0.4f);

                        Vector2 offset = new Vector2(36 - 36 * prog, 0).RotatedBy(skelly.Weapon2.GetRotation() - skelly.LowerArmR.GetRotation());
                        skelly.Weapon2.BoneOffset = offset;

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90));

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));

                        skelly.HandLIK.ChangeBoneOffset(justitaPlacement - skelly.UpperArmL.Position());
                    }

                    skelly.CalculateLegIK();
                    skelly.CalculateHandIK();

                    if (NPC.frameCounter >= 60 && NPC.frameCounter < 260)
                    {
                        skelly.Weapon1.BoneOffset = (justitaPlacement - skelly.LowerArmL.EndPoint()).RotatedBy(-skelly.LowerArmL.GetRotation());
                    }
                    else
                    {
                        skelly.Weapon1.BoneOffset = Vector2.Zero;
                    }

                    //LookingAt(skelly.Weapon1.Position(), 64);
                    //LookingAt(justitaPlacement, 64);
                }
                else if (animState == AnimationState.Phase4Transition)
                {
                    if (NPC.frameCounter < 30)
                    {
                        float prog = ((float)NPC.frameCounter) / 30f;
                        skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(MathHelper.ToRadians(-90 * prog)));
                        skelly.Weapon2.ChangeBoneRotation(skelly.LowerArmL.GetRotation() - MathHelper.ToRadians(90), 0.3f);

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(0, skelly.ArmLength() - 1));
                        skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-75);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));
                    }
                    else if (NPC.frameCounter < 75)
                    {
                        LookAtPlayer = false;
                        skelly.Head.ChangeBoneRotation(-1.57f + MathHelper.ToRadians(45 * NPC.spriteDirection));
                        float prog = ((float)NPC.frameCounter - 30f) / 10f;
                        if (prog > 1f)
                            prog = 1f;

                        Vector2 offset = new Vector2(38 * prog, 0).RotatedBy(skelly.Weapon2.GetRotation() - skelly.LowerArmR.GetRotation());
                        skelly.Weapon2.BoneOffset = offset;
                        skelly.Weapon2.scale = 1f + 0.5f * prog;

                        Vector2 position = new Vector2(skelly.ArmLength() - 8, 0).RotatedBy(MathHelper.ToRadians(-90 + 90 * prog));
                        Vector2 position2 = position + new Vector2(0, 100);
                        if (prog >= 1f)
                        {
                            position.X += Main.rand.NextFloat(-1, 1);
                            position.Y += Main.rand.NextFloat(-1, 1);
                        }
                        skelly.HandRIK.ChangeBoneOffset(position);
                        skelly.Weapon2.ChangeBoneRotation((position2 - position).ToRotation(), 0.3f);

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(-48, skelly.ArmLength() - 8));
                        skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-75);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-14, 0));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(2, 0));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -62), 8);
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90), 0.8f);
                    }
                    else if (NPC.frameCounter < 90)
                    {
                        skelly.Weapon2.Visible = false;

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(-4, skelly.ArmLength() - 1));
                        skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-75);

                        skelly.HandRIK.ChangeBoneOffset(new Vector2(4, skelly.ArmLength() - 1));

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));
                    }
                    else if (NPC.frameCounter < 120)
                    {
                        float prog = ((float)NPC.frameCounter - 90f) / 30f;

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(1.57f - prog * 3.14f), 6);
                        skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation - 1.309f, 1.57f);

                        skelly.HandRIK.ChangeBoneOffset(new Vector2(4, skelly.ArmLength() - 1));

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90 - 6));
                    }
                    else if (NPC.frameCounter < 130)
                    {
                        NPC.localAI[1] = 3;
                        float prog = ((float)NPC.frameCounter - 120f) / 10f;

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(-1.57f + prog * MathHelper.ToRadians(280)), 16);
                        skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation - 1.309f, 1.57f);

                        skelly.HandRIK.ChangeBoneOffset(new Vector2(4, skelly.ArmLength() - 1));

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-12, 0));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(0, 0));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * 4))));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90));
                    }
                    skelly.CalculateHandIK();
                    skelly.CalculateLegIK();
                }
                else if (animState == AnimationState.Idle4)
                {
                    float animSpeedMult = 8;
                    skelly.Weapon1.Visible = true;
                    skelly.Weapon2.Visible = true;
                    skelly.Gauntlet.Visible = false;

                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-36, -5));
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(18, -5));

                    skelly.CalculateLegIK();

                    skelly.HandLIK.ChangeBoneOffset(new Vector2(-4, skelly.ArmLength() - 1));
                    skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-60);// * NPC.spriteDirection);
                    skelly.HandRIK.ChangeBoneOffset(new Vector2(4, skelly.ArmLength() - 1));
                    skelly.Weapon2.rotation = skelly.LowerArmR.rotation + MathHelper.ToRadians(-45);// * NPC.spriteDirection);

                    skelly.CalculateHandIK();

                    skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -52 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * animSpeedMult))));
                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-80 + 6 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * animSpeedMult))));

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(100));

                    skelly.Weapon1.ChangeBoneScale(1.3f);
                }
                else if (animState == AnimationState.TwilightDashSlash)
                {
                    float prog = (float)NPC.frameCounter / 15f;
                    if (prog > 1f)
                        prog = 1f;
                    prog = (float)Math.Sin(prog * 1.57f);
                    skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(MathHelper.ToRadians(150 - 270 * prog)), 16);

                    skelly.CalculateHandIK();

                    skelly.Weapon1.rotation = skelly.LowerArmL.rotation;

                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-36, 0), 12f);
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(36, 0), 12f);

                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-50));
                    skelly.Pelvis.ChangeBoneOffset(new Vector2(14, -40));

                    skelly.CalculateLegIK();

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(140));
                }
                else if (animState == AnimationState.TwilightDashSlash2)
                {
                    float prog = (float)NPC.frameCounter / 15f;
                    if (prog > 1f)
                        prog = 1f;
                    prog = (float)Math.Sin(prog * 1.57f);
                    skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(MathHelper.ToRadians(-120 + 270 * prog)), 16);

                    skelly.CalculateHandIK();

                    skelly.Weapon1.rotation = skelly.LowerArmL.rotation;

                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-36, 0), 12f);
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(36, 0), 12f);

                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-50));
                    skelly.Pelvis.ChangeBoneOffset(new Vector2(14, -40));

                    skelly.CalculateLegIK();

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(140));
                }
                else if (animState == AnimationState.TwilightFinisher)
                {
                    if (NPC.frameCounter < 20)
                    {
                        float prog = (float)NPC.frameCounter / 15f;
                        if (prog > 1f)
                            prog = 1f;
                        prog = (float)Math.Sin(prog * 1.57f);

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength() - 8f, 0).RotatedBy(MathHelper.ToRadians(150 - 270 * prog)), 16);
                        skelly.CalculateHandIK();

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-48, -48 * prog), 12f);
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(8, -56 * prog), 12f);
                        skelly.CalculateLegIK();

                        skelly.Weapon1.rotation = skelly.LowerArmL.rotation - MathHelper.ToRadians(45);

                        prog = (float)NPC.frameCounter / 20f;
                        if (prog > 1f)
                            prog = 1f;
                        prog = (float)Math.Sin(prog * 1.57f);

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -40 - 80 * prog), 16f);
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-50 - 65 * prog));

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(100));
                    }
                    else
                    {
                        float prog = (float)(NPC.frameCounter - 15f) / 5f;
                        if (prog > 1f)
                            prog = 1f;
                        //prog = 1f + (float)Math.Sin(prog * 1.57f - 1.57f);

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(MathHelper.ToRadians(-120 + 165 * prog)), 16);
                        skelly.CalculateHandIK();

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-48, -4), 12f);
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(16, -4), 12f);
                        skelly.CalculateLegIK();

                        skelly.Weapon1.rotation = skelly.LowerArmL.rotation - MathHelper.ToRadians(20);

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -40), 16f);
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-50), 0.18f);

                        skelly.Weapon1.ChangeBoneScale(1.5f);

                        skelly.Head.rotation = NPC.spriteDirection == 1 ? -0.79f : -2.35f;
                        LookAtPlayer = false;

                        if (NPC.frameCounter < 25f)
                            skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(250), 0.12f);
                        else
                            skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(140));
                    }
                }
                else if (animState == AnimationState.TwilightEnd) //I KNEEL
                {
                    float prog = MathHelper.ToRadians((float)NPC.frameCounter);
                    prog = (float)Math.Sin(prog * 4);
                    skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength() - 4, 0).RotatedBy(MathHelper.ToRadians(-200)), 16);

                    skelly.CalculateHandIK();

                    skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation - MathHelper.ToRadians(45), 0.14f);

                    skelly.FeetLIK.ChangeBoneOffset(new Vector2(-skelly.LowerLegL.length, -4), 12f);
                    skelly.FeetRIK.ChangeBoneOffset(new Vector2(skelly.UpperLegR.length + 12, -4), 12f);

                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-50 + 8 * prog));
                    skelly.Pelvis.ChangeBoneOffset(new Vector2(14, -skelly.UpperLegL.length));

                    skelly.CalculateLegIK();

                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(140));

                    skelly.Head.rotation = NPC.spriteDirection == 1 ? -0.79f : -2.35f;
                    LookAtPlayer = false;

                    skelly.Weapon1.ChangeBoneScale(1.3f);
                }
                else if (animState == AnimationState.TwilightChase)
                {
                    float animSpeedMult = (int)(NPC.velocity.Length() * 2);
                    if (animSpeedMult > 20)
                        animSpeedMult = 20;
                    //Main.NewText(animSpeedMult);
                    float x = 16 * 3 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * animSpeedMult));// * NPC.spriteDirection;
                    float y = 16 * (float)Math.Cos(MathHelper.ToRadians((float)NPC.frameCounter * animSpeedMult));
                    if (y < 0)
                        y = 0;
                    skelly.FeetLIK.BoneOffset = new Vector2(-12 + x, y * -1); //.ChangeBoneOffset(new Vector2(-12 + x, y * -1), 30);
                    y = 16 * (float)Math.Cos(MathHelper.ToRadians((float)NPC.frameCounter * animSpeedMult));
                    if (y > 0)
                        y = 0;
                    skelly.FeetRIK.BoneOffset = new Vector2(0 - x, y);//.ChangeBoneOffset(, 30);

                    float velocityRotation = 0;//(float)Math.Atan2(NPC.velocity.Y * NPC.spriteDirection, NPC.velocity.X * NPC.spriteDirection);
                    skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength() - 2, 0).RotatedBy(MathHelper.ToRadians(150) + velocityRotation));
                    skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(10);// * NPC.spriteDirection);
                    skelly.HandRIK.ChangeBoneOffset(new Vector2(skelly.ArmLength() , 0).RotatedBy(MathHelper.ToRadians(150) + velocityRotation));
                    skelly.Weapon2.rotation = skelly.LowerArmR.rotation + MathHelper.ToRadians(-45);// * NPC.spriteDirection);

                    skelly.CalculateHandIK();

                    skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-50));
                    skelly.Pelvis.ChangeBoneOffset(new Vector2(14, -40 + 0.6f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * animSpeedMult))));

                    skelly.CalculateLegIK();
                    skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(160));

                    skelly.Weapon1.ChangeBoneScale(1.3f);
                }
                else if (animState == AnimationState.TwilightLamp)
                {
                    if (NPC.frameCounter < 60)
                    {

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-26, 0));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(10, 0));

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(-90), 16);
                        skelly.Weapon1.ChangeBoneRotation(skelly.LowerArmL.rotation);
                        //skelly.Weapon1.rotation = skelly.LowerArmL.rotation + MathHelper.ToRadians(-75);// * NPC.spriteDirection);
                        //skelly.HandRIK.ChangeBoneOffset(new Vector2(4, skelly.ArmLength() - 1));
                        //skelly.Weapon2.rotation = skelly.LowerArmR.rotation + MathHelper.ToRadians(-45);// * NPC.spriteDirection);

                        skelly.CalculateHandIK();

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -62));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90));

                        LookAtPlayer = false;
                        skelly.Head.ChangeBoneRotation(MathHelper.ToRadians(-90));

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                    }
                    else
                    {
                        float prog = 1f;
                        if (NPC.frameCounter < 70)
                            prog = (float)(NPC.frameCounter - 60f) / 10f;

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-36, -5));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(18, -5));

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength(), 0).RotatedBy(MathHelper.ToRadians(-90 + 270 * prog)), 64);
                        skelly.Weapon1.rotation = skelly.LowerArmL.rotation;

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -52 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter))));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-50 + 6 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter))), 0.1f);

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(150), 0.1f);
                    }
                    skelly.CalculateLegIK();
                    skelly.CalculateHandIK();
                }

                else if (animState == AnimationState.GoldRushThrow)
                {
                    float animSpeedMult = 8;
                    Vector2 twilightPlacement = skelly.Origin.Position() + new Vector2(-36, -90);

                    if (NPC.frameCounter < 10)
                    {
                        skelly.HandLIK.ChangeBoneOffset(twilightPlacement - skelly.UpperArmL.Position(), 12f);
                        skelly.Weapon1.ChangeBoneRotation(MathHelper.ToRadians(120), 1.4f);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-36, -5));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(24, -5));

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));
                    }
                    else if (NPC.frameCounter < 45)
                    {
                        float prog = (float)(NPC.frameCounter - 10) / 35;
                        skelly.Gauntlet.Visible = true;
                        skelly.Gauntlet.scale = 1f;

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength() - 4, 0).RotatedBy(MathHelper.ToRadians(120 - 210 * prog)), 16);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-36, -5));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(24, -5));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -62 + 0.5f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * animSpeedMult))));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90));
                    }
                    else if (NPC.frameCounter < 105)
                    {
                        skelly.Gauntlet.ChangeBoneScale(2f, .01f);

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength() - 4 + Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)).RotatedBy(MathHelper.ToRadians(-90)));

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-36, -5));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(24, -5));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -62 + 0.5f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * animSpeedMult))));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90));
                    }
                    else if (NPC.frameCounter < 155)
                    {
                        float prog = (float)(NPC.frameCounter - 105) / 50f;

                        skelly.HandLIK.ChangeBoneOffset(new Vector2(skelly.ArmLength() - 4, 0).RotatedBy(MathHelper.ToRadians(-90 + 270 * prog)), 16);

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -52 + 0.5f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * animSpeedMult))));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-75));

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-36, -5));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(24, -5));

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(110));

                    }
                    else if (NPC.frameCounter < 165)
                    {
                        float prog = (float)(NPC.frameCounter - 155) / 10f;

                        skelly.HandLIK.ChangeBoneOffset(new Vector2((skelly.ArmLength() - 4) + skelly.ArmLength() * prog, 4), 16);
                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -52 + 0.5f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * animSpeedMult))));

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(24, -5), 16);
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(-36, -5), 16);

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(10, -57 + 0.5f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * animSpeedMult))));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-90));

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(130));

                        if (prog > 0.5f)
                        {
                            skelly.Gauntlet.Visible = false;
                            skelly.Gauntlet.scale = 1f;
                        }
                    }
                    else if (NPC.frameCounter < 195)
                    {
                        skelly.Gauntlet.Visible = false;
                        skelly.Gauntlet.scale = 1f;

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(110));
                    }
                    else
                    {
                        skelly.HandLIK.ChangeBoneOffset(twilightPlacement - skelly.UpperArmL.Position(), 12f);

                        skelly.FeetLIK.ChangeBoneOffset(new Vector2(-36, -5));
                        skelly.FeetRIK.ChangeBoneOffset(new Vector2(18, -5));

                        skelly.Pelvis.ChangeBoneOffset(new Vector2(0, -52 + 0.5f * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * animSpeedMult))));
                        skelly.Pelvis.ChangeBoneRotation(MathHelper.ToRadians(-80 + 6 * (float)Math.Sin(MathHelper.ToRadians((float)NPC.frameCounter * animSpeedMult))));

                        skelly.Hair.ChangeBoneRotation(MathHelper.ToRadians(90));

                    }

                    skelly.CalculateHandIK();
                    skelly.CalculateLegIK();

                    if (NPC.frameCounter >= 10)
                        skelly.Weapon1.BoneOffset = (twilightPlacement - skelly.LowerArmL.EndPoint()).RotatedBy(-skelly.LowerArmL.GetRotation());
                    else if (NPC.frameCounter > 135)
                        skelly.Weapon1.BoneOffset = Vector2.Zero;
                }

                //Draw Afterimages
                if (animState == AnimationState.GoldRushLoop ||
                    animState == AnimationState.Dash ||
                    animState == AnimationState.MimicryGreaterSplitV ||
                    (animState == AnimationState.TwilightChase && AiState == 2) ||
                    (animState == AnimationState.TwilightDashSlash && AiState == 5) ||
                    (animState == AnimationState.TwilightChase && AiState == TwilightRun) ||
                    (animState == AnimationState.TwilightDashSlash && AiState == TwilightRunDash))
                    NPC.localAI[2] = 1f;
                else if (NPC.localAI[2] > 0f)
                    NPC.localAI[2] -= 0.1f;


                if (NPC.ai[0] == 1 && NPC.ai[3] < 0 && 
                  !(animState == AnimationState.HeavenThrow || 
                    animState == AnimationState.MimicryPrepare || 
                    animState == AnimationState.MimicryGreaterSplitV || 
                    animState == AnimationState.Phase3Transition))
                {
                    skelly.Weapon2.Visible = true;
                }

                if (LookAtPlayer)
                {
                    Vector2 delta = (NPC.GetTargetData().Center - skelly.Head.Position());
                    float headRot = (float)(Math.Atan2(delta.Y * NPC.spriteDirection, delta.X * NPC.spriteDirection));

                    if (headRot > MathHelper.ToRadians(25))
                        headRot = MathHelper.ToRadians(25);
                    else if (headRot < -MathHelper.ToRadians(30))
                        headRot = -MathHelper.ToRadians(30);

                    int FacingDirection = delta.X < 0 ? -1 : 1;

                    if (FacingDirection == NPC.spriteDirection)
                        skelly.Head.ChangeBoneRotation(headRot - 1.57f);
                    else
                        skelly.Head.ChangeBoneRotation(-1.57f);
                }

                /*if (NPC.ai[0] > 0 && (skelly.Head.Position() - skelly.Head.OldPosition(1)).Length() > 0.05f)
                {
                    Vector2 EyePosition = skelly.Head.Position(NPC.spriteDirection) + new Vector2(-2,8 * NPC.spriteDirection).RotatedBy(skelly.Head.GetRotation());
                    Dust d = Dust.NewDustPerfect(EyePosition, Mod.DustType("RedMistEye"));
                    d.velocity *= 0;
                    d.fadeIn = 0.1f;
                    d.noGravity = true;
                }*/

                skelly.UpdateOldBones();
                //LookingAt(skelly.HandLIK.Position(), 15);
                //LookingAt(skelly.HandRIK.Position(), 6);

                //LookingAt(skelly.Weapon2.Position(), 6);

                //LookingAt(skelly.FeetLIK.Position(), 6);
                //LookingAt(skelly.FeetRIK.Position(), 15);
            }
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

                            if (Main.tile[fx, fy].HasTile && Main.tileSolid[Main.tile[fx, fy].TileType] && !Collision.SolidTiles(fx - 3, fx + 3, fy - tileHeight, fy - 1))
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
                        Vector2 velocity = LobHelper.ProjectileMotion(new Vector2(0, NPC.position.Y + NPC.height), targetPos, time, NPC.gravity);
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

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref LobEventFlags.downedRedMist, -1);     
				
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
			else if (Main.netMode == NetmodeID.SinglePlayer)
			{
				Vector2 pos = skelly.Pelvis.Position(NPC.direction);
				Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore1").Type);
				pos = skelly.Pelvis.Position(NPC.direction);
				Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore2").Type);
				pos = skelly.UpperArmR.Position(NPC.direction);
				Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore3").Type);
				pos = skelly.UpperArmL.Position(NPC.direction);
				Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore4").Type);
				pos = skelly.UpperLegR.Position(NPC.direction);
				Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore5").Type);
				pos = skelly.LowerLegR.Position(NPC.direction);
				Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore6").Type);
				pos = skelly.UpperLegL.Position(NPC.direction);
				Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore7").Type);
				pos = skelly.LowerLegL.Position(NPC.direction);
				Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore8").Type);
				pos = skelly.Head.Position(NPC.direction);
				Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore9").Type);
				pos = skelly.Head.Position(NPC.direction);
				Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore10").Type);
				pos = skelly.Head.Position(NPC.direction);
				Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore11").Type);
				pos = skelly.Head.Position(NPC.direction);
				Gore.NewGore(NPC.GetSource_Death(), pos, Vector2.Zero, ModContent.Find<ModGore>("LobotomyCorp/RedMistGore12").Type);
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
            set { NPC.localAI[0] = (int) value; }
        }

        private enum AnimationState
        {
            Idle1,//Phase1
            Walk1,
            SwingRedEyes,
            SwingPenitence,
            SwingBoth,
            GoldRushIntro,//GoldRushAnimations
            GoldRushLoop,
            GoldRushLoopFall,
            GoldRushEnd,
            Phase2Transition,//Phase2
            Idle2,
            Walk2,
            Dash,
            Run,
            SwingDaCapo,
            SwingMimicry,
            DaCapoThrow,
            MimicryPrepare,
            MimicryGreaterSplitV,
            MidAir,
            HeavenThrow,
            Phase3Transition,//Phase3
            Walk3,
            Idle3,
            JustitiaSwing,
            SmileSwing,
            Phase4Transition,//Phase4
            Idle4,
            GoldRushThrow,
            TwilightChase,
            TwilightDashSlash,
            TwilightDashSlash2,
            TwilightFinisher,
            TwilightDrift,
            TwilightEnd, //KNEEL
            TwilightCounter,
            TwilightLamp,
            Intro
        }

        private void ChangeAnimation(AnimationState i)
        {
            if (animState != i)
                NPC.frameCounter = 0;
            animState = i;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (skelly == null)
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
                    DrawRedMist(spriteBatch, trailColor, i);
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

            DrawRedMist(spriteBatch, drawColor, -1, true);
            return false;
        }

        public void GetEye(ref Vector2 position, ref float rotation)
        {
            position = skelly.Head.Position(NPC.spriteDirection) + new Vector2(-2, 8 * NPC.spriteDirection).RotatedBy(skelly.Head.GetRotation());
            rotation = skelly.Head.GetRotation();
        }

        private void DrawRedMist(SpriteBatch spriteBatch, Color lightColor, int i = -1, bool glowMask = false)
        {
            Texture2D tex = Mod.Assets.Request<Texture2D>("NPCs/RedMist/RedMistAssembled").Value;
            Texture2D texGlow = Mod.Assets.Request<Texture2D>("NPCs/RedMist/RedMistAssembled_Glow").Value;
            Vector2 origin;
            Vector2 position;
            float rot;
            Color color = lightColor;
            Color glowmaskColor = Color.White;
            if (Main.player[NPC.target].dead && AiState == IdleState)
            {
                if (Timer > -120)
                    color = Color.Lerp(color, Color.Black, -Timer / 120f);
                else
                {
                    color = Color.Lerp(Color.Black, Color.Transparent, (-Timer - 120) / 60);
                    glowmaskColor = Color.Lerp(Color.White, Color.Transparent, (-Timer - 120) / 60);
                }
            }
            Rectangle frame = new Rectangle(0, 0, 48, 66);

            //Hair
            origin = new Vector2(27, 5);
            position = skelly.Head.Position(NPC.spriteDirection, i);
            rot = skelly.Hair.GetRotation(NPC.spriteDirection, i) - 1.57f;//MathHelper.ToRadians(90);//skelly.Head.rotation + 1.57f;
            frame.Y = frame.Height * 11;
            Draw(spriteBatch, tex, position, frame, color, rot, origin, 1f, NPC.spriteDirection);

            //Lower Layer Arm
            position = skelly.UpperArmR.Position(NPC.spriteDirection, i);
            rot = skelly.UpperArmR.GetRotation(NPC.spriteDirection, i) - (NPC.spriteDirection == 1 ? 0.785f : 2.355f);
            origin = new Vector2(3, 3);
            frame.Y = frame.Height * 4;
            Draw(spriteBatch, tex, position, frame, color, rot, origin, 1f, NPC.spriteDirection);

            position = skelly.LowerArmR.Position(NPC.spriteDirection, i);
            rot = skelly.LowerArmR.GetRotation(NPC.spriteDirection, i) - (NPC.spriteDirection == 1 ? 0.785f : 2.355f);
            origin = new Vector2(3, 3);
            frame.Y = frame.Height * 5;
            Draw(spriteBatch, tex, position, frame, color, rot, origin, 1f, NPC.spriteDirection);

            position = skelly.HandR.Position(NPC.spriteDirection, i);
            frame.Y = frame.Height * 6;
            Draw(spriteBatch, tex, position, frame, color, rot, origin, 1f, NPC.spriteDirection);

            //Weapon 2
            if (skelly.Weapon2.Visible && NPC.localAI[1] < 3)
            {
                int dir = NPC.spriteDirection;
                Texture2D weapon = Mod.Assets.Request<Texture2D>("Items/Zayin/Penitence").Value;
                Vector2 weaponOrigin = new Vector2(4, 49);
                if (NPC.localAI[1] == 1)
                {
                    dir *= -1;
                    weapon = Mod.Assets.Request<Texture2D>("Items/Aleph/DaCapo").Value;
                    weaponOrigin = new Vector2(45, 63);
                }
                if (NPC.localAI[1] == 2)
                {
                    weapon = Mod.Assets.Request<Texture2D>("Items/Aleph/Smile").Value;
                    weaponOrigin = new Vector2(39, weapon.Height - 39);
                }
                position = skelly.Weapon2.Position(NPC.spriteDirection, i);
                rot = skelly.Weapon2.GetRotation(NPC.spriteDirection, i) + 0.785f;

                if (animState == AnimationState.SwingDaCapo && 30 <= NPC.frameCounter && NPC.frameCounter < 60)
                {
                    dir *= -1;
                }

                Draw(spriteBatch, weapon, position, weapon.Frame(), color, rot, weaponOrigin, skelly.Weapon2.scale, dir, true);
            }

            //Lower Layer Leg
            origin = new Vector2(5, 9);
            position = skelly.UpperLegR.Position(NPC.spriteDirection, i);
            rot = skelly.UpperLegR.GetRotation(NPC.spriteDirection, i) - 1.57f;
            frame.Y = frame.Height * 9;
            Draw(spriteBatch, tex, position, frame, color, rot, origin, 1f, NPC.spriteDirection);

            origin = new Vector2(7, 5);
            position = skelly.LowerLegR.Position(NPC.spriteDirection, i);
            rot = skelly.LowerLegR.GetRotation(NPC.spriteDirection, i) - 1.57f;
            frame.Y = frame.Height * 10;
            Draw(spriteBatch, tex, position, frame, color, rot, origin, 1f, NPC.spriteDirection);

            //Body
            origin = new Vector2(17, 41);
            position = skelly.Pelvis.Position(NPC.spriteDirection, i);
            rot = skelly.Pelvis.GetRotation(NPC.spriteDirection, i) + 1.57f;
            frame.Y = frame.Height * 0;
            Draw(spriteBatch, tex, position, frame, color, rot, origin, 1f, NPC.spriteDirection);
            if (glowMask)
                Draw(spriteBatch, texGlow, position, frame, glowmaskColor, rot, origin, 1f, NPC.spriteDirection);

            int x = Math.Max(0, (int)NPC.ai[0]);
            if (x > 3)
                x = 3;
            origin = new Vector2(13, 21);
            position = skelly.Head.Position(NPC.spriteDirection, i);
            rot = skelly.Head.GetRotation(1, i) + 1.57f;
            frame.Y = frame.Height * (12 + x);
            Draw(spriteBatch, tex, position, frame, color, rot, origin, 1f, NPC.spriteDirection);
            if (glowMask)
                Draw(spriteBatch, texGlow, position, frame, glowmaskColor, rot, origin, 1f, NPC.spriteDirection);

            //Top Layer Leg
            origin = new Vector2(11, 9);
            position = skelly.UpperLegL.Position(NPC.spriteDirection, i);
            rot = skelly.UpperLegL.GetRotation(NPC.spriteDirection, i) - 1.57f;
            frame.Y = frame.Height * 7;
            Draw(spriteBatch, tex, position, frame, color, rot, origin, 1f, NPC.spriteDirection);
            if (glowMask)
                Draw(spriteBatch, texGlow, position, frame, glowmaskColor, rot, origin, 1f, NPC.spriteDirection);

            origin = new Vector2(11, 5);
            position = skelly.LowerLegL.Position(NPC.spriteDirection, i);
            rot = skelly.LowerLegL.GetRotation(NPC.spriteDirection, i) - 1.57f;
            frame.Y = frame.Height * 8;
            Draw(spriteBatch, tex, position, frame, color, rot, origin, 1f, NPC.spriteDirection);
            if (glowMask)
                Draw(spriteBatch, texGlow, position, frame, glowmaskColor, rot, origin, 1f, NPC.spriteDirection);

            //Weapon 1
            if (skelly.Weapon1.Visible)
            {
                Texture2D weapon = Mod.Assets.Request<Texture2D>("Items/Teth/RedEyes").Value;
                Vector2 weaponOrigin = new Vector2(5, weapon.Height - 5);
                if (NPC.localAI[1] == 1)
                {
                    weapon = Mod.Assets.Request<Texture2D>("Items/Aleph/Mimicry").Value;
                    weaponOrigin = new Vector2(9, weapon.Height - 9);
                }
                if (NPC.localAI[1] == 2)
                {
                    weapon = Mod.Assets.Request<Texture2D>("Items/Aleph/Justitia").Value;
                    weaponOrigin = new Vector2(12, weapon.Height - 12);
                }
                if (NPC.localAI[1] == 3)
                {
                    weapon = Mod.Assets.Request<Texture2D>("Items/Aleph/Twilight").Value;
                    weaponOrigin = new Vector2(12, weapon.Height - 12);
                }
                if (animState == AnimationState.HeavenThrow)
                {
                    weapon = Mod.Assets.Request<Texture2D>("NPCs/RedMist/HeavenBoss").Value;
                    weaponOrigin = new Vector2(44, 44);
                }
                position = skelly.Weapon1.Position(NPC.spriteDirection, i);
                rot = skelly.Weapon1.GetRotation(NPC.spriteDirection, i) + 0.785f;
                Draw(spriteBatch, weapon, position, weapon.Frame(), color, rot, weaponOrigin, skelly.Weapon1.scale, NPC.spriteDirection, true);
            }

            //Top Layer Arm
            position = skelly.UpperArmL.Position(NPC.spriteDirection, i);
            rot = skelly.UpperArmL.GetRotation(NPC.spriteDirection, i) - (NPC.spriteDirection == 1 ? 0.785f : 2.355f);
            origin = new Vector2(3, 3);
            frame.Y = frame.Height * 1;
            Draw(spriteBatch, tex, position, frame, color, rot, origin, 1f, NPC.spriteDirection);
            if (glowMask)
                Draw(spriteBatch, texGlow, position, frame, glowmaskColor, rot, origin, 1f, NPC.spriteDirection);

            position = skelly.LowerArmL.Position(NPC.spriteDirection, i);
            rot = skelly.LowerArmL.GetRotation(NPC.spriteDirection, i) - (NPC.spriteDirection == 1 ? 0.785f : 2.355f);
            origin = new Vector2(3, 3);
            frame.Y = frame.Height * 2;
            Draw(spriteBatch, tex, position, frame, color, rot, origin, 1f, NPC.spriteDirection);
            if (glowMask)
                Draw(spriteBatch, texGlow, position, frame, glowmaskColor, rot, origin, 1f, NPC.spriteDirection);

            position = skelly.HandL.Position(NPC.spriteDirection, i);
            frame.Y = frame.Height * 3;
            Draw(spriteBatch, tex, position, frame, color, rot, origin, 1f, NPC.spriteDirection);
            if (glowMask)
                Draw(spriteBatch, texGlow, position, frame, glowmaskColor, rot, origin, 1f, NPC.spriteDirection);

            //---Gauntlet
            if (skelly.Gauntlet.Visible)
            {
                Texture2D weapon = Mod.Assets.Request<Texture2D>("Projectiles/GoldRushPunches").Value;
                position = skelly.Gauntlet.Position(NPC.spriteDirection, i);
                rot = skelly.LowerArmL.GetRotation(NPC.spriteDirection, i) + (NPC.spriteDirection == -1 ? 2.355f : 2.355f - 1.57f);
                Vector2 weaponOrigin = new Vector2(3, 28);
                Draw(spriteBatch, weapon, position, weapon.Frame(), color, rot, weaponOrigin, skelly.Gauntlet.scale, NPC.spriteDirection);
            }
        }

        private void Draw(SpriteBatch spriteBatch, Texture2D tex, Vector2 position, Rectangle frame, Color color, float rot, Vector2 origin, float scale, int dir, bool Weapon = false)
        {
            SpriteEffects speffect = SpriteEffects.None;
            if (dir == -1)
            {
                origin.X = frame.Width - origin.X;
                speffect = SpriteEffects.FlipHorizontally;
                if (Weapon)
                    rot += 1.57f;
            }
            spriteBatch.Draw(tex, position - Main.screenPosition, frame, color, rot, origin, scale, speffect, 0f);
        }
    }    

    /// <summary>
    /// This class is old and specialized for red mist, for the actual skeleton helper check SkeletonBase/Skeletonii
    /// </summary>
    class RedMistSkeletonHelper
    {
        //I hate myself
        //Specialied for Red Mist since I think its the only thing that will use this boy sure I hope so :D
        //Rotation Based because math hurts my brain so I just let the computer do it :V
        //Remember its original rotation is 0 which is ->
        //Origin makes everything move, its all parented on Origin
        //Origin doesnt make stuff rotate tho so fuck me I guess
        public Bone Origin;

        public Bone Pelvis;
        public Bone Head;
        public Bone Hair;

        public Bone UpperArmL;
        public Bone UpperArmR;

        public Bone LowerArmL;
        public Bone LowerArmR;

        public Bone HandR;
        public Bone HandL;

        public Bone Weapon1;
        public Bone Weapon2;

        public Bone Gauntlet;

        public Bone HandRIK;
        public Bone HandLIK;

        public Bone UpperLegL;
        public Bone UpperLegR;

        public Bone LowerLegL;
        public Bone LowerLegR;

        public Bone FeetRIK;
        public Bone FeetLIK;

        public RedMistSkeletonHelper(Vector2 origin, Vector2 pelvis, Vector2 shoulderOffsetL, Vector2 shoulderOffsetR, float upperArmLength, float lowerArmLength, Vector2 legOffsetL, Vector2 legOffsetR, float upperLegLength, float lowerLegLength,float StartRot = 1.57f)
        {
            Origin = new Bone(origin, 0, 1, null, false, true);
            Pelvis = new Bone(pelvis, StartRot - 3.14f, 0, Origin, false, true, 10, true);
            Head = new Bone(new Vector2(44, -6), StartRot - 3.14f, 0, Pelvis);
            Hair = new Bone(Vector2.Zero, StartRot, 0, Head);

            UpperArmL = new Bone(shoulderOffsetL, StartRot + MathHelper.ToRadians(90), upperArmLength, Pelvis);
            UpperArmR = new Bone(shoulderOffsetR, StartRot - MathHelper.ToRadians(90), upperArmLength, Pelvis);

            LowerArmL = new Bone(Vector2.Zero, StartRot + MathHelper.ToRadians(90), lowerArmLength, UpperArmL);
            LowerArmR = new Bone(Vector2.Zero, StartRot - MathHelper.ToRadians(90), lowerArmLength, UpperArmR);

            HandL = new Bone(Vector2.Zero, StartRot, 2, LowerArmL);
            HandR = new Bone(Vector2.Zero, StartRot, 2, LowerArmR);

            Weapon1 = new Bone(Vector2.Zero, StartRot, 52, LowerArmL);
            Weapon1.scale = 1.3f;
            Weapon2 = new Bone(Vector2.Zero, StartRot, 52, LowerArmR);

            Gauntlet = new Bone(Vector2.Zero, StartRot, 8, UpperArmL);
            Gauntlet.scale = 0.1f;
            //Gauntlet.Visible = false;

            HandLIK = new Bone(Vector2.Zero, 0, 1, UpperArmL, true);
            HandRIK = new Bone(Vector2.Zero, 0, 1, UpperArmR, true);

            UpperLegL = new Bone(legOffsetL, StartRot, upperLegLength, Pelvis);
            UpperLegR = new Bone(legOffsetR, StartRot, upperLegLength, Pelvis);

            LowerLegL = new Bone(Vector2.Zero, StartRot, lowerLegLength, UpperLegL);
            LowerLegR = new Bone(Vector2.Zero, StartRot, lowerLegLength, UpperLegR);

            FeetLIK = new Bone(Vector2.Zero, 0, 1, Origin, true);
            FeetRIK = new Bone(Vector2.Zero, 0, 1, Origin, true);
        }

        public void UpdateOldBones()
        {
            Origin.Record();

            Pelvis.Record();
            Head.Record();
            Hair.Record();

            UpperArmL.Record();
            UpperArmR.Record();

            LowerArmL.Record();
            LowerArmR.Record();

            HandR.Record();
            HandL.Record();

            Weapon1.Record();
            Weapon2.Record();
            Gauntlet.Record();

            HandRIK.Record();
            HandLIK.Record();

            UpperLegL.Record();
            UpperLegR.Record();

            LowerLegL.Record();
            LowerLegR.Record();

            FeetRIK.Record();
            FeetLIK.Record();
        }

        public enum Part
        {
            O,
            P,
            H,
            UAL,
            UAR,
            FAL,
            FAR,
            HL,
            HLIK,
            HR,
            HRIK,
            ULL,
            ULR,
            UFL,
            UFR,
            LLIK,
            LRIK
        }

        /// <summary>
        /// Direction 1 = Clockwise, Direction -1 = CounterClockwise
        /// </summary>
        /// <param name="dir"></param>
        public void CalculateHandIK(int dir = 1)
        {
            //Left Side
            Vector2 startPoint = UpperArmL.Position();
            Vector2 endPoint = HandLIK.Position();
            Vector2 elbow = ElbowIK(startPoint, endPoint, UpperArmL.length, LowerArmL.length, dir);
            //Main.NewText(elbow);
            UpperArmL.rotation = (elbow - startPoint).ToRotation();
            LowerArmL.rotation = (endPoint - elbow).ToRotation();

            //Right Side
            startPoint = UpperArmR.Position();
            endPoint = HandRIK.Position();
            elbow = ElbowIK(startPoint, endPoint, UpperArmR.length, LowerArmR.length, dir);
            //Main.NewText(elbow);
            UpperArmR.rotation = (elbow - startPoint).ToRotation();
            LowerArmR.rotation = (endPoint - elbow).ToRotation();
        }

        /// <summary>
        /// Direction 1 = Clockwise, Direction -1 = CounterClockwise
        /// </summary>
        /// <param name="dir"></param>
        public void CalculateLegIK(int dir = 1)
        {
            //Left Side
            Vector2 startPoint = UpperLegL.Position();
            Vector2 endPoint = FeetLIK.Position();
            Vector2 elbow = ElbowIK(startPoint, endPoint, UpperLegL.length, LowerLegL.length, -1 * dir);
            //Main.NewText(elbow);
            UpperLegL.rotation = (elbow - startPoint).ToRotation();
            LowerLegL.rotation = (endPoint - elbow).ToRotation();

            //Right Side
            startPoint = UpperLegR.Position();
            endPoint = FeetRIK.Position();
            elbow = ElbowIK(startPoint, endPoint, UpperLegR.length, LowerLegR.length, -1 * dir);
            //Main.NewText(elbow);
            UpperLegR.rotation = (elbow - startPoint).ToRotation();
            LowerLegR.rotation = (endPoint - elbow).ToRotation();
        }

        public Vector2 ElbowIK(Vector2 startPoint, Vector2 endPoint, float length1, float length2, int dir = 1)
        {
            Vector2 elbow = startPoint;
            float Dist = Vector2.Distance(endPoint, startPoint);
            if (Dist > length1 + length2)
                Dist = length1 + length2;
            float Angle = (float)Math.Acos(Dist * Dist / ((length1 + length2) * Dist));
            float Rotation = (endPoint - elbow).ToRotation() + Angle * dir;
            elbow += new Vector2(length1, 0).RotatedBy(Rotation);
            return elbow;
        }
    
        public float ArmLength()
        {
            return UpperArmL.length + LowerArmL.length;
        }

        public float LegLength()
        {
            return UpperLegL.length + LowerLegL.length;
        }
    }

    class Bone
    {
        public Vector2 BoneOffset;
        public Vector2[] oldOffset;

        public float rotation;
        public float[] oldRot;

        Bone Parent;
        public float length;
        public float scale;
        public bool Origin;
        public bool Visible;
        public bool Pelvis;

        bool IKBone;

        public Bone(Vector2 pos, float rot = 0, float Length = 1, Bone parent = null, bool iKBone = false, bool origin = false, int record = 10, bool pelvis = false)
        {
            BoneOffset = pos;
            rotation = rot;
            length = Length;
            Parent = parent;
            IKBone = iKBone;
            Origin = origin;
            Visible = true;
            scale = 1f;
            Pelvis = pelvis;

            oldOffset = new Vector2[record];
            oldRot = new float[record];
        }

        public void Record()
        {
            for (int i = oldOffset.Length - 1; i >= 0; i--)
            {
                if (i > 0)
                {
                    if (oldOffset[i - 1] != null)
                        oldOffset[i] = oldOffset[i - 1];

                    oldRot[i] = oldRot[i - 1];
                }
                else
                {
                    oldOffset[i] = BoneOffset;
                    oldRot[i] = rotation;
                }
            }
        }

        public float GetRotation(int dir = 1, int old = -1)
        {
            if (old >= 0)
                return GetOldRotation(old, dir);
            Vector2 vec1 = new Vector2(1, 0).RotatedBy(rotation);
            vec1.X *= dir;
            return vec1.ToRotation();
        }

        public float GetOldRotation(int i,int dir = 1)
        {
            Vector2 vec1 = new Vector2(1, 0).RotatedBy(oldRot[i]);
            vec1.X *= dir;
            return vec1.ToRotation();
        }

        public Vector2 Position(int dir = 1, int old = -1)
        {
            if (old >= 0)
                return OldPosition(old, dir);
            if (IKBone)
                return BoneOffset + Parent.Position();
            if (Parent != null)
            {
                if (Pelvis)
                    return new Vector2(BoneOffset.X * dir, BoneOffset.Y).RotatedBy(Parent.GetRotation()) + Parent.EndPoint();
                return new Vector2(BoneOffset.X, BoneOffset.Y * dir).RotatedBy(Parent.GetRotation(dir)) + Parent.EndPoint(dir);
            }
            else
                return new Vector2(BoneOffset.X, BoneOffset.Y * dir);
        }

        public Vector2 OldPosition(int i, int dir = 1)
        {
            if (IKBone)
                return oldOffset[i] + Parent.OldPosition(i);
            if (Parent != null)
            {
                if (Pelvis)
                    return new Vector2(oldOffset[i].X * dir, oldOffset[i].Y).RotatedBy(Parent.GetOldRotation(i)) + Parent.OldEndPoint(i);
                return new Vector2(oldOffset[i].X, oldOffset[i].Y * dir).RotatedBy(Parent.GetOldRotation(i, dir)) + Parent.OldEndPoint(i, dir);
            }
            else
                return new Vector2(oldOffset[i].X, oldOffset[i].Y * dir);
        }

        public void AssignParent(Bone bone)
        {
            Parent = bone;
        }

        public float InheritRotation()
        {
            if (Parent != null)
                return rotation + Parent.InheritRotation();
            else
                return rotation;
        }

        public Vector2 EndPoint(int dir = 1)
        {
            if (Pelvis)
                return Position(dir) + new Vector2(length, 0).RotatedBy(GetRotation(dir));
            if (Origin)
                return Position() + new Vector2(length, 0).RotatedBy(rotation);
            return Position(dir) + new Vector2(length, 0).RotatedBy(GetRotation(dir));
        }

        public Vector2 OldEndPoint(int i, int dir = 1)
        {
            if (Pelvis)
                return OldPosition(i, dir) + new Vector2(length, 0).RotatedBy(GetRotation(dir));
            if (Origin)
                return OldPosition(i) + new Vector2(length, 0).RotatedBy(oldRot[i]);
            return OldPosition( i, dir) + new Vector2(length, 0).RotatedBy(GetOldRotation(i, dir));
        }

        public void ChangeBone(Vector2 newPos, float speed, float rot, float rotSpeed)
        {
            if (speed > 0)
            {
                Vector2 bonePos = BoneOffset;
                Vector2 delta = newPos - bonePos;

                if (delta.Length() > speed)
                {
                    delta.Normalize();
                    delta *= speed;
                }
                BoneOffset += delta;
            }

            if (rotSpeed > 0)
                rotation = Terraria.Utils.AngleLerp(rotation, rot, rotSpeed);
        }

        public void ChangeBoneOffset(Vector2 newPos, float speed = 3)
        {
            ChangeBone(newPos, speed, 0, 0);
        }

        public void ChangeBoneRotation(float rot, float rotSpeed = 0.0872f)
        {
            ChangeBone(Vector2.Zero, 0, rot, rotSpeed);
        }
    
        public Vector2 DifferenceBone(Bone bone)
        {
            return bone.Position() - Position();
        }

        public void ChangeBoneScale(float Scale, float speed = 0.15f)
        {
            if (scale < Scale)
            {
                scale += speed;
                if (scale > Scale)
                    scale = Scale;
            }
            else if (scale > Scale)
            {
                scale -= speed;
                if (scale < Scale)
                    scale = Scale;
            }
        }
    }

    class RedMistEye : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Misc/Dusts/RedMistEye";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eye");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10;

            Projectile.hostile = true;
            Projectile.friendly = false;
        }

        private Trailhelper trail;

        public override void AI()
        {
            NPC n = Main.npc[(int)Projectile.ai[0]];
            if (!n.active || n.life <= 0 || n.type != ModContent.NPCType<RedMist>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 10;

            if (trail == null)
                trail = new Trailhelper(30);
            RedMist redmist = (RedMist)n.ModNPC;
            Vector2 pos = Projectile.Center; float rot = 0;
            redmist.GetEye(ref pos, ref rot);
            trail.TrailUpdate(pos, rot + 1.57f);
            Projectile.Center = pos;
            Projectile.rotation = rot;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;

            CustomShaderData shader = LobotomyCorp.LobcorpShaders["GenericTrail"].UseImage1(Mod, "Misc/GenTrail");
            TaperingTrail Trail = new TaperingTrail();
            Trail.ColorStart = Color.Red;
            Trail.ColorEnd = Color.Red;
            Trail.width = 3;

            Trail.Draw(trail.TrailPos, trail.TrailRotation, Vector2.Zero, shader);
            return true;
        }
    }
}

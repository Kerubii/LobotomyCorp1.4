using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Aleph;
using LobotomyCorp.Items.He;
using LobotomyCorp.Items.Teth;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Items.Zayin;
using LobotomyCorp.Projectiles;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.NPCs.RedMist
{
    class RedMistSkeleton : SkeletonBase
    {
        NPC NPC;

        public enum BoneLabel
        {
            Origin,

            Pelvis,
            Head,
            Hair,

            UpperArmL,
            UpperArmR,

            LowerArmL,
            LowerArmR,

            HandL,
            HandR,

            FrontWeapon,
            BackWeapon,

            Gauntlet,

            HandLIK,
            HandRIK,

            UpperLegL,
            UpperLegR,

            LowerLegL,
            LowerLegR,

            FeetLIK,
            FeetRIK
        }

        public RedMistSkeleton(NPC redMist)
        {
            BoneName = new Dictionary<Enum, BonePart>();
            float startRot = 1.57f;
            Texture2D tex = Mod.Assets.Request<Texture2D>("NPCs/RedMist/RedMistAssembled", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle frame = new(0, 0, 48, 66);

            BoneName.Add(BoneLabel.Origin,
                new BonePart(new Vector2(redMist.Center.X, redMist.position.Y + redMist.height), 0, 1, 1, null, 10)
                );
            frame.Y = frame.Height * 0;
            BoneName.Add(BoneLabel.Pelvis,
                new BonePart(new Vector2(0, -72), startRot - 3.14f, 1, 0, BoneName[BoneLabel.Origin], 10)
                .SetDraw(tex, frame, new Vector2(17, 41), 1.57f)
                .ChangeIRotation(false)
                );
            frame.Y = frame.Height * 12;
            BoneName.Add(BoneLabel.Head,
                new BonePart(new Vector2(44, -6), startRot - 3.14f, 1, 0, BoneName[BoneLabel.Pelvis], 10)
                .SetDraw(tex, frame, new Vector2(13, 21), 1.57f)
                .ChangeIRotation(false)
                );
            frame.Y = frame.Height * 11;
            BoneName.Add(BoneLabel.Hair,
                new BonePart(Vector2.Zero, startRot, 1, 0, BoneName[BoneLabel.Head], 10)
                .SetDraw(tex, frame, new Vector2(27, 5), -1.57f)
                .ChangeIRotation(false)
                );

            Vector2 origin = new Vector2(3, 3);
            float upperArmLength = 12;
            frame.Y = frame.Height * 1;
            BoneName.Add(BoneLabel.UpperArmL,
                new BonePart(new Vector2(26, -14), startRot + 1.57f, 1, upperArmLength, BoneName[BoneLabel.Pelvis], 10)
                .SetDraw(tex, frame, origin)
                .ChangeIRotation(false)
                );
            frame.Y = frame.Height * 4;
            BoneName.Add(BoneLabel.UpperArmR,
                new BonePart(new Vector2(26, 2), startRot - 1.57f, 1, upperArmLength, BoneName[BoneLabel.Pelvis], 10)
                .SetDraw(tex, frame, origin)
                .ChangeIRotation(false)
                );

            float lowerArmLength = 16;
            frame.Y = frame.Height * 2;
            BoneName.Add(BoneLabel.LowerArmL,
                new BonePart(Vector2.Zero, startRot + 1.57f, 1, lowerArmLength, BoneName[BoneLabel.UpperArmL], 10)
                .SetDraw(tex, frame, origin)
                .ChangeIRotation(false)
                );
            frame.Y = frame.Height * 5;
            BoneName.Add(BoneLabel.LowerArmR,
                new BonePart(Vector2.Zero, startRot - 1.57f, 1, lowerArmLength, BoneName[BoneLabel.UpperArmR], 10)
                .SetDraw(tex, frame, origin)
                .ChangeIRotation(false)
                );

            frame.Y = frame.Height * 3;
            BoneName.Add(BoneLabel.HandL,
                new BonePart(Vector2.Zero, startRot, 1, 2, BoneName[BoneLabel.LowerArmL], 10)
                .SetDraw(tex, frame, origin));
            frame.Y = frame.Height * 6;
            BoneName.Add(BoneLabel.HandR,
                new BonePart(Vector2.Zero, 0, 1, 2, BoneName[BoneLabel.LowerArmR], 10)
                .SetDraw(tex, frame, origin));

            BoneName.Add(BoneLabel.FrontWeapon,
                new BonePart(Vector2.Zero, 0, 1.3f, 52, BoneName[BoneLabel.LowerArmL], 10)
                .ChangeIRotation(false));
            BoneName.Add(BoneLabel.BackWeapon,
                new BonePart(Vector2.Zero, startRot, 1, 52, BoneName[BoneLabel.LowerArmR], 10)
                .ChangeIRotation(false));

            BoneName.Add(BoneLabel.Gauntlet,
                new BonePart(Vector2.Zero, startRot, 1, 8, BoneName[BoneLabel.UpperArmL], 10)
                .ChangeIRotation(false)
                );

            Init(redMist);
        }

        public void Init(NPC redMist)
        {
            float startRot = 1.57f;
            Texture2D tex = Mod.Assets.Request<Texture2D>("NPCs/RedMist/RedMistAssembled", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle frame = new(0, 0, 48, 66);

            

            BoneName.Add(BoneLabel.HandLIK,
                new BonePart(Vector2.Zero, 0, 1, 1, BoneName[BoneLabel.UpperArmL], 10)
                .ChangeIRotation(false)
                .ChangeIOffestRotation(false)
                .ChangeParentOrigin(true));
            BoneName.Add(BoneLabel.HandRIK,
                new BonePart(Vector2.Zero, 0, 1, 1, BoneName[BoneLabel.UpperArmR], 10)
                .ChangeIRotation(false)
                .ChangeIOffestRotation(false)
                .ChangeParentOrigin(true));

            int upperLegLength = 30;
            frame.Y = frame.Height * 7;
            BoneName.Add(BoneLabel.UpperLegL,
                new BonePart(new Vector2(-4, -10), startRot, 1, upperLegLength, BoneName[BoneLabel.Pelvis], 10)
                .SetDraw(tex, frame, new Vector2(11, 9), -1.57f)
                .ChangeIRotation(false)
                );
            frame.Y = frame.Height * 9;
            BoneName.Add(BoneLabel.UpperLegR,
                new BonePart(new Vector2(-4, 2), startRot, 1, upperLegLength, BoneName[BoneLabel.Pelvis], 10)
                .SetDraw(tex, frame, new Vector2(5, 9), -1.57f)
                .ChangeIRotation(false)
                );

            int lowerLegLength = 42;
            frame.Y = frame.Height * 8;
            BoneName.Add(BoneLabel.LowerLegL,
                new BonePart(Vector2.Zero, startRot, 1, lowerLegLength, BoneName[BoneLabel.UpperLegL], 10)
                .SetDraw(tex, frame, new Vector2(11, 5), -1.57f)
                .ChangeIRotation(false)
                );
            frame.Y = frame.Height * 10;
            BoneName.Add(BoneLabel.LowerLegR,
                new BonePart(Vector2.Zero, startRot, 1, lowerLegLength, BoneName[BoneLabel.UpperLegR], 10)
                .SetDraw(tex, frame, new Vector2(7, 5), -1.57f)
                .ChangeIRotation(false)
                );

            BoneName.Add(BoneLabel.FeetLIK,
                new BonePart(Vector2.Zero, 0, 1, 1, BoneName[BoneLabel.Origin], 10)
                .ChangeIRotation(false)
                .ChangeIOffestRotation(false)
                .ChangeParentOrigin(true));
            BoneName.Add(BoneLabel.FeetRIK,
                new BonePart(Vector2.Zero, 0, 1, 1, BoneName[BoneLabel.Origin], 10)
                .ChangeIRotation(false)
                .ChangeIOffestRotation(false)
                .ChangeParentOrigin(true));


        }

        public enum AnimationState
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
            Intro,

            EXIdle,
            RedEyesPenitenceIntro,
            WingbeatWield,
            PenitenceSlamCombo,
            DashSlash,
            PenitenceRaise,
            ShootBoth,
            Soda,
            Tough,

            MistTeleport,
            WristCutterStart,
            Horn,
            Solitude,
            FourthMatch,
            Regret,
            SoCute,
            Fragments,
            RedEyes,
            CherryBlossom,
            BeakCounter,
            BeakShoot,
            LookOfTheDay,
            EngulfingDream,
            Lantern,
            LifeForaDaredevil,
            ScreamingWedge,

            BearPaw,
            Syrinx,
            Christmas,
            FrostSplinter,
            GrinderMk4,
            Laetitia,
            Lumber,
            OurGalaxy,
            SanguineDesire,
            Gaze,
            Harmony,
            Harvest,
            SolemntLament,
            MagicBullet,
            Hornet,
            Discord,
            FaintAroma,
            Lamp,
            Pleasure,
            BlackSwan,
            Amrita,
            CobaltScar,
            Diffraction,
            Ecstacy,
            GreenStem,
            Heaven,
            Hypocrisy,
            InTheNameOfLoveAndHate,
            Moonlight,
            Spore,
            SwordSharpened,
            Justitia,
            GoldRush,

            BlueStar,
            Mimicry,
            Smile,
            Censored,
            Pink,
            Adoration,
            DaCapo,

            ParadiseLost
        }

        public void Update(NPC npc)
        {
            Vector2 origin = new Vector2(npc.Center.X, npc.position.Y + npc.height);
            BoneName[BoneLabel.Origin].ChangeOffset(origin);
            bool LookAtPlayer = true;
            npc.frameCounter++;
            int AiState = (int)npc.ai[1];
            AnimationState state = (AnimationState)npc.localAI[0];
            switch (state)
            {
                case AnimationState.Intro:
                    LookAtPlayer = Intro(npc);
                    break;
                case AnimationState.Idle1:
                    Idle1(npc);
                    break;
                case AnimationState.Walk1:
                    Walk1(npc);
                    break;
                case AnimationState.SwingRedEyes:
                    SwingRedEyes(npc);
                    break;
                case AnimationState.SwingPenitence:
                    SwingPenitence(npc);
                    break;
                case AnimationState.SwingBoth:
                    SwingBoth(npc);
                    break;
                case AnimationState.GoldRushIntro:
                    GoldRushIntro(npc);
                    break;
                case AnimationState.GoldRushLoop:
                    GoldRushLoop(npc);
                    break;
                //case AnimationState.GoldRushLoopFall:
                //GoldRushLoopFall(npc);
                //break;
                case AnimationState.GoldRushEnd:
                    GoldRushEnd(npc);
                    break;
                case AnimationState.Phase2Transition:
                    Phase2Transition(npc);
                    break;
                case AnimationState.Idle2:
                    Idle2(npc);
                    break;
                case AnimationState.Walk2:
                    Walk2(npc);
                    break;
                case AnimationState.Dash:
                    Dash(npc);
                    break;
                //case AnimationState.Run:
                //LookAtPlayer = Run(npc);
                //break;
                case AnimationState.SwingDaCapo:
                    SwingDaCapo(npc);
                    break;
                case AnimationState.SwingMimicry:
                    SwingMimicry(npc);
                    break;
                case AnimationState.DaCapoThrow:
                    DaCapoThrow(npc);
                    break;
                case AnimationState.MimicryPrepare:
                    MimicryPrepare(npc);
                    break;
                case AnimationState.MimicryGreaterSplitV:
                    MimicryGreaterSplitV(npc);
                    break;
                case AnimationState.MidAir:
                    LookAtPlayer = MidAir(npc);
                    break;
                case AnimationState.HeavenThrow:
                    LookAtPlayer = HeavenThrow(npc);
                    break;
                case AnimationState.Phase3Transition:
                    Phase3Transition(npc);
                    break;
                case AnimationState.Walk3:
                    Walk3(npc);
                    break;
                case AnimationState.Idle3:
                    Idle3(npc);
                    break;
                case AnimationState.JustitiaSwing:
                    JustitiaSwing(npc);
                    break;
                case AnimationState.SmileSwing:
                    SmileSwing(npc);
                    break;
                case AnimationState.Phase4Transition:
                    LookAtPlayer = Phase4Transition(npc);
                    break;
                case AnimationState.Idle4:
                    Idle4(npc);
                    break;
                case AnimationState.GoldRushThrow:
                    GoldRushThrow(npc);
                    break;
                case AnimationState.TwilightChase:
                    TwilightChase(npc);
                    break;
                case AnimationState.TwilightDashSlash:
                    TwilightDashSlash(npc);
                    break;
                case AnimationState.TwilightDashSlash2:
                    TwilightDashSlash2(npc);
                    break;
                case AnimationState.TwilightFinisher:
                    LookAtPlayer = TwilightFinisher(npc);
                    break;
                //case AnimationState.TwilightDrift:
                //LookAtPlayer = TwilightDrift(npc);
                //break;
                case AnimationState.TwilightEnd:
                    LookAtPlayer = TwilightEnd(npc);
                    break;
                //case AnimationState.TwilightCounter:
                //LookAtPlayer = TwilightCounter(npc);
                //break;
                case AnimationState.TwilightLamp:
                    LookAtPlayer = TwilightLamp(npc);
                    break;
                case AnimationState.RedEyesPenitenceIntro:
                    LookAtPlayer = RedEyesPenitenceIntro(npc);
                    break;
                case AnimationState.WingbeatWield:
                    WingbeatWield(npc);
                    break;
                case AnimationState.Run:
                    LookAtPlayer = EXRun(npc);
                    break;
                case AnimationState.DashSlash:
                    BothSwingDashLoop(npc);
                    break;
                case AnimationState.PenitenceRaise:
                    PenitenceRaise(npc);
                    break;
                case AnimationState.PenitenceSlamCombo:
                    PenitenceComboSlam(npc);
                    break;
                case AnimationState.Tough:
                    LookAtPlayer = ToughShoot(npc);
                    break;
                case AnimationState.Soda:
                    LookAtPlayer = SodaShoot(npc);
                    break;
                case AnimationState.ShootBoth:
                    LookAtPlayer = DoubleShoot(npc);
                    break;
                case AnimationState.WristCutterStart:
                    WristCutterStart(npc);
                    break;
                case AnimationState.EXIdle:
                    EXIdle1(npc);
                    break;
            }

            if (state == AnimationState.GoldRushLoop ||
                    state == AnimationState.Dash ||
                    state == AnimationState.MimicryGreaterSplitV ||
                    (state == AnimationState.TwilightChase && AiState == 2) ||
                    (state == AnimationState.TwilightDashSlash && AiState == 5) ||
                    (state == AnimationState.TwilightChase && AiState == 10) ||
                    (state == AnimationState.TwilightDashSlash && AiState == 11))
                npc.localAI[2] = 1f;
            else if (npc.localAI[2] > 0f)
                npc.localAI[2] -= 0.1f;

            if (npc.ai[0] == 1 && npc.ai[3] < 0 &&
                  !(state == AnimationState.HeavenThrow ||
                    state == AnimationState.MimicryPrepare ||
                    state == AnimationState.MimicryGreaterSplitV ||
                    state == AnimationState.Phase3Transition))
            {
                BoneName[BoneLabel.BackWeapon].Visible = true;
            }

            if (LookAtPlayer)
            {
                Vector2 delta = (npc.GetTargetData().Center - BoneName[BoneLabel.Head].GetPosition());
                float headRot = (float)(Math.Atan2(delta.Y * npc.spriteDirection, delta.X * npc.spriteDirection));

                if (headRot > MathHelper.ToRadians(25))
                    headRot = MathHelper.ToRadians(25);
                else if (headRot < -MathHelper.ToRadians(30))
                    headRot = -MathHelper.ToRadians(30);

                int FacingDirection = delta.X < 0 ? -1 : 1;

                if (FacingDirection == npc.spriteDirection)
                    BoneName[BoneLabel.Head].ChangeRotation(headRot - 1.57f);
                else
                    BoneName[BoneLabel.Head].ChangeRotation(-1.57f);
            }

            Record();
        }

        public float ArmLength()
        {
            return BoneName[BoneLabel.UpperArmL].Length + BoneName[BoneLabel.LowerArmL].Length;
        }

        public float LegLength()
        {
            return BoneName[BoneLabel.UpperLegL].Length + BoneName[BoneLabel.LowerLegL].Length;
        }

        public void CalculateHandIK(int dir1 = 1, int dir2 = 1)
        {
            RotationIK(BoneLabel.UpperArmL, BoneLabel.LowerArmL, BoneLabel.HandLIK, dir1);
            RotationIK(BoneLabel.UpperArmR, BoneLabel.LowerArmR, BoneLabel.HandRIK, dir2);
        }

        public void CalculateLegIK(int dir1 = -1, int dir2 = -1)
        {
            RotationIK(BoneLabel.UpperLegL, BoneLabel.LowerLegL, BoneLabel.FeetLIK, dir1);
            RotationIK(BoneLabel.UpperLegR, BoneLabel.LowerLegR, BoneLabel.FeetRIK, dir2);
        }

        private void ChangeAnimation(NPC npc, AnimationState i)
        {
            if (npc.localAI[0] != (float)i)
                npc.frameCounter = 0;
            npc.localAI[0] = (float)i;
        }

        public void DrawSkeleton(SpriteBatch sp, NPC npc, Color lightColor, int i = -1, bool glowMask = false)
        {
            Texture2D tex = Mod.Assets.Request<Texture2D>("NPCs/RedMist/RedMistAssembled").Value;
            Texture2D texGlow = Mod.Assets.Request<Texture2D>("NPCs/RedMist/RedMistAssembled_Glow").Value;
            Vector2 origin;
            Vector2 position;
            float rot;
            Color color = lightColor;
            Color glowmaskColor = Color.White;
            if (Main.player[npc.target].dead && npc.ai[1] == 0)
            {
                if (npc.ai[2] > -120)
                    color = Color.Lerp(color, Color.Black, -npc.ai[2] / 120f);
                else
                {
                    color = Color.Lerp(Color.Black, Color.Transparent, (-npc.ai[2] - 120) / 60);
                    glowmaskColor = Color.Lerp(Color.White, Color.Transparent, (-npc.ai[2] - 120) / 60);
                }
            }
            Rectangle frame = new Rectangle(0, 0, 48, 66);

            BoneName[BoneLabel.Hair].DrawBone(sp, color, npc.spriteDirection, i);
            BoneName[BoneLabel.UpperArmR].DrawBoneAltRot(sp, color, npc.spriteDirection, i);
            BoneName[BoneLabel.LowerArmR].DrawBoneAltRot(sp, color, npc.spriteDirection, i);
            //BoneName[BoneLabel.HandR].DrawBoneAltRot(sp, color, npc.spriteDirection, i);

            frame.Y = frame.Height * 6;
            position = BoneName[BoneLabel.HandR].GetPosition(npc.spriteDirection, i) - Main.screenPosition;
            rot = BoneName[BoneLabel.LowerArmR].GetRotation(npc.spriteDirection, i) - (npc.spriteDirection == 1 ? 0.785f : 2.355f);
            origin = new Vector2(3, 3);
            SpriteEffects speffect1 = SpriteEffects.None;
            if (npc.spriteDirection == -1)
            {
                origin.X = frame.Width - origin.X;
                speffect1 = SpriteEffects.FlipHorizontally;
            }
            sp.Draw(tex, position, frame, color, rot, origin, 1f, speffect1, 0);

            if (BoneName[BoneLabel.BackWeapon].Visible)
            {
                if (npc.localAI[3] > 4)
                {
                    GetWeaponDraw(sp, npc, BoneLabel.BackWeapon, color, (int)npc.localAI[3], i);
                }
                else if (npc.localAI[1] < 3)
                {
                    int dir = npc.spriteDirection;
                    Texture2D weapon = TextureAssets.Item[ModContent.ItemType<Penitence>()].Value;
                    Vector2 weaponOrigin = new Vector2(4, 49);
                    if (npc.localAI[1] == 1)
                    {
                        dir *= -1;
                        weapon = TextureAssets.Item[ModContent.ItemType<Items.Aleph.DaCapo>()].Value;
                        weaponOrigin = new Vector2(45, 63);
                    }
                    if (npc.localAI[1] == 2)
                    {
                        weapon = TextureAssets.Item[ModContent.ItemType<Smile>()].Value;
                        weaponOrigin = new Vector2(39, weapon.Height - 39);
                    }
                    position = BoneName[BoneLabel.BackWeapon].GetPosition(npc.spriteDirection, i) - Main.screenPosition;
                    rot = BoneName[BoneLabel.BackWeapon].GetRotation(npc.spriteDirection, i) + 0.785f;

                    if (npc.ai[1] == (int)AnimationState.SwingDaCapo && 30 <= npc.frameCounter && npc.frameCounter < 60)
                    {
                        dir *= -1;
                    }
                    SpriteEffects speffect = SpriteEffects.None;
                    if (dir == -1)
                    {
                        weaponOrigin.X = weapon.Frame().Width - weaponOrigin.X;
                        speffect = SpriteEffects.FlipHorizontally;
                        rot += 1.57f;
                    }
                    sp.Draw(weapon, position, weapon.Frame(), color, rot, weaponOrigin, BoneName[BoneLabel.BackWeapon].GetScale(), speffect, 0);
                }
            }

            BoneName[BoneLabel.UpperLegR].DrawBone(sp, color, npc.spriteDirection, i);
            BoneName[BoneLabel.LowerLegR].DrawBone(sp, color, npc.spriteDirection, i);

            BoneName[BoneLabel.Pelvis].DrawBone(sp, color, npc.spriteDirection, i);
            if (glowMask)
                BoneName[BoneLabel.Pelvis].DrawBone(sp, texGlow, glowmaskColor, npc.spriteDirection, i);

            int x = Math.Max(0, (int)npc.ai[0]);
            if (x > 3)
                x = 3;
            origin = new Vector2(13, 21);
            frame.Y = frame.Height * (12 + x);
            position = BoneName[BoneLabel.Head].GetPosition(npc.spriteDirection, i) - Main.screenPosition;
            rot = BoneName[BoneLabel.Head].GetRotation(1, i) + 1.57f;
            SpriteEffects spheadeff = SpriteEffects.None;
            if (npc.spriteDirection == -1)
            {
                origin.X = frame.Width - origin.X;
                spheadeff = SpriteEffects.FlipHorizontally;
            }
            sp.Draw(tex, position, frame, color, rot, origin, BoneName[BoneLabel.Head].GetScale(i), spheadeff, 0f);
            if (glowMask)
                sp.Draw(texGlow, position, frame, glowmaskColor, rot, origin, BoneName[BoneLabel.Head].GetScale(i), spheadeff, 0f);

            BoneName[BoneLabel.UpperLegL].DrawBone(sp, color, npc.spriteDirection, i);
            if (glowMask)
                BoneName[BoneLabel.UpperLegL].DrawBone(sp, texGlow, glowmaskColor, npc.spriteDirection, i);
            BoneName[BoneLabel.LowerLegL].DrawBone(sp, color, npc.spriteDirection, i);
            if (glowMask)
                BoneName[BoneLabel.LowerLegL].DrawBone(sp, texGlow, glowmaskColor, npc.spriteDirection, i);

            if (BoneName[BoneLabel.FrontWeapon].Visible)
            {
                if (npc.localAI[1] > 4)
                {
                    GetWeaponDraw(sp, npc, BoneLabel.FrontWeapon, color, (int)npc.localAI[1], i);
                }
                else
                {
                    Texture2D weapon = TextureAssets.Item[ModContent.ItemType<RedEyes>()].Value;
                    Vector2 weaponOrigin = new Vector2(5, weapon.Height - 5);
                    if (npc.localAI[1] == 1)
                    {
                        weapon = TextureAssets.Item[ModContent.ItemType<Mimicry>()].Value;
                        weaponOrigin = new Vector2(9, weapon.Height - 9);
                    }
                    if (npc.localAI[1] == 2)
                    {
                        weapon = TextureAssets.Item[ModContent.ItemType<Justitia>()].Value;
                        weaponOrigin = new Vector2(12, weapon.Height - 12);
                    }
                    if (npc.localAI[1] == 3)
                    {
                        weapon = TextureAssets.Item[ModContent.ItemType<Twilight>()].Value;
                        weaponOrigin = new Vector2(12, weapon.Height - 12);
                    }
                    if (npc.ai[0] == (int)AnimationState.HeavenThrow)
                    {
                        weapon = Mod.Assets.Request<Texture2D>("NPCs/RedMist/HeavenBoss").Value;
                        weaponOrigin = new Vector2(44, 44);
                    }
                    position = BoneName[BoneLabel.FrontWeapon].GetPosition(npc.spriteDirection, i) - Main.screenPosition;
                    rot = BoneName[BoneLabel.FrontWeapon].GetRotation(npc.spriteDirection, i) + 0.785f;

                    SpriteEffects speffect = SpriteEffects.None;
                    if (npc.spriteDirection == -1)
                    {
                        weaponOrigin.X = weapon.Frame().Width - weaponOrigin.X;
                        speffect = SpriteEffects.FlipHorizontally;
                        rot += 1.57f;
                    }
                    sp.Draw(weapon, position, weapon.Frame(), color, rot, weaponOrigin, BoneName[BoneLabel.FrontWeapon].GetScale(), speffect, 0);
                }
            }

            BoneName[BoneLabel.UpperArmL].DrawBoneAltRot( sp, color, npc.spriteDirection, i);
            if (glowMask)
                BoneName[BoneLabel.UpperArmL].DrawBoneAltRot(sp, texGlow, glowmaskColor, npc.spriteDirection, i);
            BoneName[BoneLabel.LowerArmL].DrawBoneAltRot( sp, color, npc.spriteDirection, i);
            if (glowMask)
                BoneName[BoneLabel.LowerArmL].DrawBoneAltRot(sp, texGlow, glowmaskColor, npc.spriteDirection, i);
            //BoneName[BoneLabel.HandL].DrawBoneAltRot( sp, color, npc.spriteDirection, i);
            frame.Y = frame.Height * 6;
            position = BoneName[BoneLabel.HandL].GetPosition(npc.spriteDirection, i) - Main.screenPosition;
            rot = BoneName[BoneLabel.LowerArmL].GetRotation(npc.spriteDirection, i) - (npc.spriteDirection == 1 ? 0.785f : 2.355f);
            origin = new Vector2(3, 3);
            if (npc.spriteDirection == -1)
            {
                origin.X = frame.Width - origin.X;
            }
            sp.Draw(tex, position, frame, color, rot, origin, 1f, speffect1, 0);

            if (glowMask)
            {
                //BoneName[BoneLabel.HandL].DrawBoneAltRot(sp, texGlow, glowmaskColor, npc.spriteDirection, i);
                sp.Draw(texGlow, BoneName[BoneLabel.HandL].GetPosition(npc.spriteDirection, i), frame, glowmaskColor, rot, origin, 1f, speffect1, 0);
            }

            if (BoneName[BoneLabel.Gauntlet].Visible)
            {
                Texture2D weapon = TextureAssets.Projectile[ModContent.ProjectileType<GoldRushPunches>()].Value;
                position = BoneName[BoneLabel.Gauntlet].GetPosition(npc.spriteDirection, i);
                rot = BoneName[BoneLabel.LowerArmL].GetRotation(npc.spriteDirection, i) + (npc.spriteDirection == -1 ? 2.355f : 2.355f - 1.57f);
                Rectangle gauntletFrame = weapon.Frame();
                Vector2 weaponOrigin = new Vector2(3, 28);
                SpriteEffects speffect = SpriteEffects.None;
                if (npc.spriteDirection == -1)
                {
                    origin.X = gauntletFrame.Width - origin.X;
                    speffect = SpriteEffects.FlipHorizontally;
                }
                sp.Draw(weapon, position - Main.screenPosition, gauntletFrame, color, rot, weaponOrigin, BoneName[BoneLabel.Gauntlet].GetScale(i), speffect, 0);
            }
        }
        
        public void GetWeaponDraw(SpriteBatch sp, NPC npc, BoneLabel weapon, Color color, int egoType, int i)
        {
            int dir = npc.spriteDirection;
            Texture2D tex = TextureAssets.Item[egoType].Value;
            Rectangle frame = tex.Frame();
            Item item = new();
            item.SetDefaults(egoType);

            Vector2 weaponOrigin = new Vector2(5, tex.Height - 5);
            Vector2 position = BoneName[weapon].GetPosition(npc.spriteDirection, i) - Main.screenPosition;
            float rot = BoneName[weapon].GetRotation(npc.spriteDirection, i) + 0.785f;
            SpriteEffects speffect = 0;

            if (item.useStyle == ItemUseStyleID.Shoot)
            {
                weaponOrigin = new Vector2(5, tex.Height / 2);
                rot -= 0.785f;
            }

            if (npc.spriteDirection == -1)
            {
                weaponOrigin.X = frame.Width - weaponOrigin.X;
                speffect = SpriteEffects.FlipHorizontally;
                if (item.useStyle == ItemUseStyleID.Shoot)
                {
                    rot -= 3.14f;
                }
                else
                {
                    rot += 1.57f;
                }
            }
            sp.Draw(tex, position, frame, color, rot, weaponOrigin, BoneName[weapon].GetScale(), speffect, 0);
        }

        /// <summary>
        /// 0 - Swords, 1 - Hammers, 2 - Fists, 3 - Spears, 4 - Gun, 5 - Rifle, 6 - Cannon
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="egoType"></param>
        /// <returns></returns>
        public int GetWeaponType(int egoType)
        {
            if (egoType == ModContent.ItemType<Regret>() ||
                egoType == ModContent.ItemType<Lantern>() ||
                egoType == ModContent.ItemType<Lumber>() ||
                egoType == ModContent.ItemType<Lamp>() ||
                egoType == ModContent.ItemType<Smile>())
            {
                return 1;
            }
            return 0;
        }

        public void UpdateWeaponScale(int type, int which)
        {
            BoneLabel whichBone = which < 0 ? BoneLabel.FrontWeapon : BoneLabel.BackWeapon;

            if (type == ModContent.ItemType<Soda>())
            {
                BoneName[whichBone].ChangeScale(0.8f);
            }
            else
            {
                BoneName[whichBone].ChangeScale(1f);
            }
        }

        #region Base Animation Functions

        bool Intro(NPC n)
        {
            if (n.frameCounter < 30)
            {
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength() - 4, 0).RotatedBy(MathHelper.ToRadians(-200)), 3f);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1), 3f);
                CalculateHandIK();

                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - 0.785f, 1f);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-BoneName[BoneLabel.LowerLegL].Length, -4), 3f);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(BoneName[BoneLabel.UpperLegR].Length + 12, -4), 3f);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-50), 1f);
                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(14, -BoneName[BoneLabel.UpperLegL].Length), 3f);

                CalculateLegIK();

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(140), 1f);

                BoneName[BoneLabel.Head].ChangeRotation(n.spriteDirection == 1 ? -0.79f : 2.35f);
                return false;
            }
            else
            {
                BoneName[BoneLabel.FrontWeapon].Visible = true;
                BoneName[BoneLabel.BackWeapon].Visible = true;
                BoneName[BoneLabel.Gauntlet].Visible = false;

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 1);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 1);

                CalculateLegIK();

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, ArmLength() - 1), 1);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-75));// * npc.spriteDirection);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1), 1);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-45));// * npc.spriteDirection);

                CalculateHandIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)n.frameCounter * 4))), 1);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)n.frameCounter * 4))), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
                return true;
            }
        }

        void Idle1(NPC npc)
        {
            BoneName[BoneLabel.FrontWeapon].Visible = true;
            BoneName[BoneLabel.BackWeapon].Visible = true;
            BoneName[BoneLabel.Gauntlet].Visible = false;

            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3f);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

            CalculateLegIK();

            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, ArmLength() - 1), 3);
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-75));// * npc.spriteDirection);
            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1), 3);
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-45));// * npc.spriteDirection);

            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);
            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 0.0872f);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
        }

        void Walk1(NPC npc)
        {
            float x = 16 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4));// * npc.spriteDirection;
            float y = 6 * (float)Math.Cos(MathHelper.ToRadians((float)npc.frameCounter * 4));
            if (y < 0)
                y = 0;
            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12 + x, y * -1), 3);
            y = 6 * (float)Math.Cos(MathHelper.ToRadians((float)npc.frameCounter * 4));
            if (y > 0)
                y = 0;
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0 - x, y), 3);

            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, ArmLength() - 1), 3);
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-75));
            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1), 3);
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-45));

            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -74 + 1f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 8))), 3);
            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);

            CalculateLegIK();
            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(100), 0.0872f);
        }

        bool MidAir(NPC npc)
        {
            bool LookAtPlayer = true;
            if (npc.velocity.Y > 0)
            {
                Vector2 originLeg = BoneName[BoneLabel.UpperLegL].GetPosition() - BoneName[BoneLabel.Origin].GetPosition();
                BoneName[BoneLabel.FeetLIK].ChangeOffset(originLeg + new Vector2(LegLength() - 6, 0).RotatedBy(MathHelper.ToRadians(80)), 0.5f);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(originLeg + new Vector2(LegLength() * 0.66f, 0).RotatedBy(MathHelper.ToRadians(80)), 0.5f);

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-16, ArmLength() - 24), 0.5f);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(-12, ArmLength() - 24), 0.5f);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-80), 0.0872f);
            }
            else
            {
                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-8, 0), 0.5f);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(4, 0), 0.5f);

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-16, ArmLength() - 12), 3);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(-12, ArmLength() - 12), 3);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);

                LookAtPlayer = false;
                BoneName[BoneLabel.Head].ChangeRotation(-1.57f + 0.3f * npc.spriteDirection, 0.0872f);
            }

            CalculateHandIK();
            CalculateLegIK();
            return LookAtPlayer;
        }

        void SwingRedEyes(NPC npc)
        {
            if (npc.frameCounter < 20)
            {
                float angle = 1.57f - ((float)npc.frameCounter / 19f) * 3.14f;// * npc.spriteDirection;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 6);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 - 6), 0.0872f); // * npc.spriteDirection));
            }
            else if (npc.frameCounter < 30)
            {
                float angle = -1.57f + (((float)npc.frameCounter - 20) / 10) * 4.71f;// * npc.spriteDirection;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-80), 0.0872f);// * npc.spriteDirection));
            }
            else if (npc.frameCounter < 50)
            {
                float angle = 3.14f;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);
            }
            else
            {
                ChangeAnimation(npc, 0);
            }
            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1), 3);

            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() - MathHelper.ToRadians(45));
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - 1.309f, 1.57f);// * npc.spriteDirection, 1.57f);

            CalculateLegIK();
            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
        }

        void SwingPenitence(NPC npc)
        {
            if (npc.frameCounter < 20)
            {
                float angle = 1.57f - ((float)npc.frameCounter / 19f) * 3.14f;
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 6);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 - 6), 0.0872f);// * npc.spriteDirection));
            }
            else if (npc.frameCounter < 30)
            {
                float angle = -1.57f + (((float)npc.frameCounter - 20) / 10) * 4.71f;// * npc.spriteDirection;
                                                                                     //Main.NewText(MathHelper.ToDegrees(angle));
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-80), 0.0872f);// * npc.spriteDirection));
            }
            else if (npc.frameCounter < 50)
            {
                float angle = 3.14f;
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);
            }
            else
            {
                ChangeAnimation(npc, 0);
            }
            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, ArmLength() - 1), 3);

            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() - MathHelper.ToRadians(45), 1.57f);
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - 1.309f, 1.57f);

            CalculateLegIK();
            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
        }
        void SwingBoth(NPC npc)
        {
            if (npc.frameCounter < 20)
            {
                float angle = 1.57f - ((float)npc.frameCounter / 19f) * 3.14f;// * npc.spriteDirection;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 6);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 6);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 - 6), 0.0872f);// * npc.spriteDirection));
            }
            else if (npc.frameCounter < 30)
            {
                float angle = -1.57f + (((float)npc.frameCounter - 20) / 10) * 4.71f;// * npc.spriteDirection;
                                                                                     //Main.NewText(MathHelper.ToDegrees(angle));
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-80), 0.0872f);// * npc.spriteDirection));
            }
            else if (npc.frameCounter < 50)
            {
                float angle = 3.14f;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);
            }
            else
            {
                ChangeAnimation(npc, 0);
            }
            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() - MathHelper.ToRadians(45), 1.57f);
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - 1.309f, 1.57f);

            CalculateLegIK();
            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
        }
        void GoldRushIntro(NPC npc)
        {
            BoneName[BoneLabel.FrontWeapon].Visible = false;
            BoneName[BoneLabel.BackWeapon].Visible = false;
            BoneName[BoneLabel.Gauntlet].Visible = true;
            if (npc.frameCounter == 1)
            {
                BoneName[BoneLabel.Gauntlet].ChangeScale(1f);

            }
            if (BoneName[BoneLabel.Gauntlet].GetScale() < 2f)
                BoneName[BoneLabel.Gauntlet].ChangeScale(1f + 1f * ((float)npc.frameCounter / 30f));

            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-54, 0), 3);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(40, 0), 3);
            CalculateLegIK();

            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-16 + Main.rand.NextFloat(-1, 1), 6 + Main.rand.NextFloat(-1, 1)), 3);
            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(-16, 6), 3);
            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -50), 3);
            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-75 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 0.0872f);


            BoneName[BoneLabel.Head].ChangeRotation(npc.spriteDirection == 1 ? -1.57f : -1.57f, 0.0872f);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(110), 0.0872f);

            if (npc.frameCounter > 30)
            {
                ChangeAnimation(npc, AnimationState.GoldRushLoop);
                BoneName[BoneLabel.Gauntlet].ChangeScale(2f);
            }
        }
        void GoldRushLoop(NPC npc)
        {
            float x = 102 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 16));// * npc.spriteDirection;
            float y = 20 * (float)Math.Cos(MathHelper.ToRadians((float)npc.frameCounter * 16));
            if (y < 0)
                y = 0;
            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-18 + x, y * -1), 12);
            y = 20 * (float)Math.Cos(MathHelper.ToRadians((float)npc.frameCounter * 16));
            if (y > 0)
                y = 0;
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0 - x, y), 12);
            CalculateLegIK();

            float rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X * npc.spriteDirection);
            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(rotation), 16);
            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(-BoneName[BoneLabel.UpperArmR].Length, BoneName[BoneLabel.LowerArmL].Length), 3);
            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -50 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 16))), 3);
            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-50 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 0.0872f);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(140), 0.0872f);
        }
        void GoldRushLoopFall()
        {

        }
        void GoldRushEnd(NPC npc)
        {
            BoneName[BoneLabel.FrontWeapon].Visible = false;
            BoneName[BoneLabel.BackWeapon].Visible = false;
            BoneName[BoneLabel.Gauntlet].Visible = true;
            BoneName[BoneLabel.Gauntlet].ChangeScale(1f);

            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-14, 26), 3);
            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(-16, 6), 3);
            CalculateHandIK();

            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(BoneName[BoneLabel.UpperLegL].Length - 6, 0), 3);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(-BoneName[BoneLabel.LowerArmL].Length - 20, 0), 3);
            CalculateLegIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -BoneName[BoneLabel.UpperLegL].Length - 4), 3);
            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-73 + 3 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 10))), 0.0872f);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(100), 0.0872f);
        }
        void Phase2Transition(NPC npc)
        {
            BoneName[BoneLabel.FrontWeapon].Visible = true;
            BoneName[BoneLabel.BackWeapon].Visible = true;
            BoneName[BoneLabel.Gauntlet].Visible = false;
            if (npc.frameCounter < 30)
            {
                float factor = ((float)npc.frameCounter / 30f);
                float angle = 1.57f - factor * 3.14f;// * npc.spriteDirection;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-16, 0), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-94), 0.0872f);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -80));
            }
            else if (npc.frameCounter < 40)
            {
                npc.localAI[1] = 1;

                float factor = (((float)npc.frameCounter - 30) / 10f);
                float angle = -1.57f + factor * 4.17f;// * npc.spriteDirection;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-26, 0), 3);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-80), 0.0872f);
                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -80 + factor * 2));
            }
            else if (npc.frameCounter < 60)
            {
                npc.localAI[1] = 1;

                float angle = 3.14f;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 6);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 6);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-26, 0), 3);
            }
            else
            {
                npc.localAI[1] = 1;
                ChangeAnimation(npc, AnimationState.Idle2);
            }

            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-75));
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-45));
            CalculateHandIK();

            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

            CalculateLegIK();

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
        }
        void Idle2(NPC npc)
        {
            npc.localAI[1] = 1;
            BoneName[BoneLabel.FrontWeapon].Visible = true;
            //BoneName[Bone.Weapon2].Visible = true;
            BoneName[BoneLabel.Gauntlet].Visible = false;

            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

            CalculateLegIK();

            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, ArmLength() - 1), 3);
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-50));
            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1), 3);
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(135), 0.3f);// * npc.spriteDirection);

            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);
            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 0.0872f);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
        }
        void Walk2(NPC npc)
        {
            BoneName[BoneLabel.FrontWeapon].Visible = true;

            float speed = 10;// * (Math.Abs(NPC.velocity.X) / 5f);
            if (speed > 10)
                speed = 10;

            float x = 30 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * speed));// * npc.spriteDirection;
            float y = 20 * (float)Math.Cos(MathHelper.ToRadians((float)npc.frameCounter * speed - 20));
            if (y < 0)
                y = 0;
            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12 + x, y * -1), 3);
            y = 14 * (float)Math.Cos(MathHelper.ToRadians((float)npc.frameCounter * speed - 20));
            if (y > 0)
                y = 0;
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0 - x, y), 3);

            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-16, ArmLength() - 12), 3);
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-50), 0.2f);
            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(-12, ArmLength() - 12), 3);
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(135), 0.2f);
            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(5, -72 + 1f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * speed * 2))), 3);
            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-70 + 3 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 0.0872f);

            CalculateLegIK();
            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(120), 0.0872f);
        }
        void Dash(NPC npc)
        {
            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-ArmLength(), 0), 3);
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(MathHelper.ToRadians(105), 0.12f);
            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(MathHelper.ToRadians(135)), 3);
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(180));
            CalculateHandIK();

            Vector2 point = BoneName[BoneLabel.Origin].DifferenceBone(BoneName[BoneLabel.UpperLegL]) + new Vector2(LegLength() - 2f, 0).RotatedBy(MathHelper.ToRadians(165));
            BoneName[BoneLabel.FeetLIK].ChangeOffset(point, 32);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, point.Y), 16);
            CalculateLegIK();

            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-30), 0.0872f);
            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(5, -40), 3);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(160), 0.0872f);
        }
        void SwingMimicry(NPC npc)
        {
            BoneName[BoneLabel.FrontWeapon].Visible = true;
            if (npc.frameCounter < 20)
            {
                float angle = 1.57f - ((float)npc.frameCounter / 19f) * 3.14f;// * npc.spriteDirection;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle));
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - MathHelper.ToRadians(45), 1.57f);// * npc.spriteDirection);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 - 6), 0.0872f); // * npc.spriteDirection));
            }
            else if (npc.frameCounter < 60)
            {
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength() + Main.rand.NextFloat(-0.5000f, 0.5000f), Main.rand.NextFloat(-0.5000f, 0.5000f)).RotatedBy(-1.57f), 6);
                BoneName[BoneLabel.FrontWeapon].ChangeScale(1f + 1f * ((float)npc.frameCounter - 20) / 30f);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - MathHelper.ToRadians(45), 1.57f);// * npc.spriteDirection);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);
            }
            else if (npc.frameCounter < 70)
            {
                float angle = -1.57f + (((float)npc.frameCounter - 60) / 10) * 5.06f;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - MathHelper.ToRadians(15), 1.57f);// * npc.spriteDirection);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-28, 0), 3);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(-10, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-80), 0.0872f);// * npc.spriteDirection));
            }
            else if (npc.frameCounter < 90)
            {
                BoneName[BoneLabel.FrontWeapon].ChangeScale(2f - 1f * ((float)npc.frameCounter - 70) / 10f);
                if (BoneName[BoneLabel.FrontWeapon].GetScale() < 1f)
                    BoneName[BoneLabel.FrontWeapon].ChangeScale(1f);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - MathHelper.ToRadians(15), 1.57f);

                float angle = 3.49f;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-28, 0), 3);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(-10, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);
            }
            else
            {
                ChangeAnimation(npc, AnimationState.Idle2);
            }
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1), 3);

            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(135), 1.57f);// * npc.spriteDirection, 1.57f);

            CalculateLegIK();
            CalculateHandIK();


            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
        }
        void SwingDaCapo(NPC npc)
        {
            float progress = ((float)npc.frameCounter % 30) / 10f;
            if (progress > 1f)
                progress = 1f;
            if (npc.frameCounter < 30)
            {
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(1.57f - 3.14f * progress), 16);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation(), 1.2f);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-96), 0.0872f);
            }
            else if (npc.frameCounter < 40)
            {
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(-1.57f + 4.71239f * progress), 16);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() - MathHelper.ToRadians(60), 1.2f);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-84), 0.0872f);
            }
            else if (npc.frameCounter < 60)
            {
                progress = (((float)npc.frameCounter - 40) / 20f);

                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(3.14f - 0.6f * progress), 3);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() - MathHelper.ToRadians(60), 1.2f);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-84), 0.0872f);
            }
            else if (npc.frameCounter < 90)
            {
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(3.14f - 4.71239f * progress), 16);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation(), 1.2f);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-96), 0.0872f);
            }
            else if (npc.frameCounter < 100)
            {
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(-1.57f + 3.14f * progress), 5);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(135), 0.2f);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);
            }

            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, ArmLength() - 1), 3);
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-50), 0.3f);

            CalculateHandIK();
            CalculateLegIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
        }
        void DaCapoThrow(NPC npc)
        {
            if (npc.frameCounter < 15)
            {
                float angle = 1.57f - ((float)npc.frameCounter / 15f) * 3.14f;
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 6);
                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(60), 0.0872f);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 1);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 - 6), 0.0872f);
            }
            else if (npc.frameCounter < 50)
            {
                float prog = ((float)npc.frameCounter - 15) / 35f;
                float angle = -1.57f - (prog * MathHelper.ToRadians(10));
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 6);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(60), 0.0872f);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 1);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-96 - 3 * prog), 0.0872f);
            }
            else if (npc.frameCounter < 60)
            {
                BoneName[BoneLabel.BackWeapon].Visible = false;

                float prog = ((float)npc.frameCounter - 50) / 10f;
                float angle = -1.57f - MathHelper.ToRadians(10) + (prog * MathHelper.ToRadians(270));
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(14, 0), 4);
                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-36, 0), 4);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(-10, -65 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 1);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-60), 0.32f);
            }
            else
            {
                float angle = -1.57f - MathHelper.ToRadians(10) + MathHelper.ToRadians(270);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 6);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-36, 0), 4);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(14, 0), 4);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(-10, -65 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 1);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-60), 0.32f);
            }

            CalculateLegIK();
            CalculateHandIK();

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(110), 0.0872f);
        }
        bool HeavenThrow(NPC npc)
        {
            bool LookAtPlayer = true;
            BoneName[BoneLabel.BackWeapon].Visible = false;
            if (npc.frameCounter < 50)
            {
                BoneName[BoneLabel.FrontWeapon].Visible = true;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-BoneName[BoneLabel.UpperArmL].Length, -BoneName[BoneLabel.LowerArmL].Length), 3);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0), 3);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(Main.rand.NextFloat(-0.08f, 0.08f), 0.5f);
                BoneName[BoneLabel.FrontWeapon].ChangeScale(0.2f + 0.8f * ((float)npc.frameCounter / 50f));

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12 - 10 - BoneName[BoneLabel.LowerLegL].Length, 0), 5);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(-12 + 2 + BoneName[BoneLabel.UpperLegL].Length, 0), 5);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(-12, -46), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-100), 0.0872f);

                CalculateHandIK(-1);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
            }
            else
            {
                BoneName[BoneLabel.FrontWeapon].Visible = false;
                if (npc.frameCounter < 60)
                {
                    float progress = ((float)npc.frameCounter - 50) / 9f;
                    BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(MathHelper.ToRadians(-90 + 270 * progress)), 16);
                    BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(MathHelper.ToRadians(110)), 8);

                    BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-100 + 40 * progress), 1.2f);

                    BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(180), 0.12f);
                }
                else
                {
                    BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(130), 0.0872f);
                }

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(12 + BoneName[BoneLabel.LowerLegL].Length, 0), 16);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(-6 - BoneName[BoneLabel.UpperLegL].Length, 0), 16);
                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(20, -46), 3);
                LookAtPlayer = false;
                BoneName[BoneLabel.Head].ChangeRotation(MathHelper.ToRadians(npc.spriteDirection == 1 ? -45 : -135), 0.8f);

                CalculateHandIK();
            }
            CalculateLegIK();
            return LookAtPlayer;
        }
        void MimicryPrepare(NPC npc)
        {
            BoneName[BoneLabel.BackWeapon].Visible = false;
            if (npc.frameCounter < 30)
            {
                float prog = (float)npc.frameCounter / 30f;

                Vector2 handR = new Vector2(ArmLength() - 6, 0).RotatedBy(MathHelper.ToRadians(-80 + 170 * (1f - prog)));
                BoneName[BoneLabel.HandLIK].ChangeOffset(handR, 8f);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(handR.ToRotation() - MathHelper.ToRadians(90 + Main.rand.NextFloat(-1f, 1f)), 1.2f);

                BoneName[BoneLabel.HandRIK].ChangeOffset(handR + new Vector2(-10, 0).RotatedBy(BoneName[BoneLabel.FrontWeapon].GetRotation()), 8f);
            }
            else
            {
                Vector2 handR = new Vector2(ArmLength() - 6, 0).RotatedBy(MathHelper.ToRadians(-80));
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(handR.ToRotation() - MathHelper.ToRadians(90 + Main.rand.NextFloat(-1f, 1f)), 1.2f);
                BoneName[BoneLabel.HandRIK].ChangeOffset(handR + new Vector2(-10, 0).RotatedBy(BoneName[BoneLabel.FrontWeapon].GetRotation()), 8f);
            }

            if (npc.frameCounter < 50)
            {
                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-42, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(16, 0), 3);
            }

            if (npc.frameCounter < 60)
            {
                float weaponScale = (float)npc.frameCounter / 60f;

                BoneName[BoneLabel.FrontWeapon].ChangeScale(1f + 1f * (float)Math.Sin(1.57f * weaponScale));
                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -68 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-95), 0.0872f);
            }
            else if (npc.frameCounter >= 70)
            {
                float weaponScale = ((float)npc.frameCounter - 70f) / 60f;
                if (weaponScale > 1f)
                    weaponScale = 1f;

                BoneName[BoneLabel.FrontWeapon].ChangeScale(2f + 2f * (float)Math.Sin(1.57f * weaponScale), 0.015f);
                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -68 + (weaponScale * 12f) + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-80), 0.0872f);
            }

            CalculateHandIK();
            CalculateLegIK();
        }
        void MimicryGreaterSplitV(NPC npc)
        {
            Vector2 originLeg;
            if (npc.frameCounter == 1)
            {
                Vector2 handR = new Vector2(ArmLength(), 0).RotatedBy(MathHelper.ToRadians(-80));
                BoneName[BoneLabel.HandLIK].ChangeOffset(handR);

                BoneName[BoneLabel.FrontWeapon].ChangeRotation(handR.ToRotation() - MathHelper.ToRadians(90));
                BoneName[BoneLabel.HandRIK].ChangeOffset(handR + new Vector2(-10, 0).RotatedBy(BoneName[BoneLabel.FrontWeapon].GetRotation()), 3);

                originLeg = BoneName[BoneLabel.UpperLegL].GetPosition() - BoneName[BoneLabel.Origin].GetPosition();
                BoneName[BoneLabel.FeetLIK].ChangeOffset(originLeg + new Vector2(LegLength() - 6, 0).RotatedBy(MathHelper.ToRadians(60)), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(originLeg + new Vector2(LegLength() / 2, 0).RotatedBy(MathHelper.ToRadians(60)), 3);

                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-100));
                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -72), 3);
            }
            if (npc.frameCounter >= 15)
            {
                float scale = (((float)npc.frameCounter - 15) / 20);
                if (scale > 1f)
                    scale = 1f;

                BoneName[BoneLabel.FrontWeapon].ChangeScale(4f - 3f * scale);
            }

            float prog = (float)npc.frameCounter / 10f;
            if (prog > 1f)
                prog = 1f;

            Vector2 hand = new Vector2(ArmLength() - 6 * prog, 0).RotatedBy(MathHelper.ToRadians(-80 + 320f * (float)Math.Sin(1.57f * prog)));
            BoneName[BoneLabel.HandLIK].ChangeOffset(hand);

            BoneName[BoneLabel.FrontWeapon].ChangeRotation(hand.ToRotation() - MathHelper.ToRadians(90));
            BoneName[BoneLabel.HandRIK].ChangeOffset(hand + new Vector2(-10, 0).RotatedBy(BoneName[BoneLabel.FrontWeapon].GetRotation()));

            originLeg = BoneName[BoneLabel.UpperLegL].GetPosition() - BoneName[BoneLabel.Origin].GetPosition();
            BoneName[BoneLabel.FeetLIK].ChangeOffset(originLeg + new Vector2(LegLength() - 6, 0).RotatedBy(MathHelper.ToRadians(60 + 15 * prog)));
            BoneName[BoneLabel.FeetRIK].ChangeOffset(originLeg + new Vector2(LegLength() / 2, 0).RotatedBy(MathHelper.ToRadians(60 + 15 * prog)));

            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-100 + 20 * prog));

            CalculateHandIK();
            CalculateLegIK();
        }
        void Phase3Transition(NPC npc)
        {
            if (npc.frameCounter < 60)
            {
                float angle;
                if (npc.frameCounter < 20)
                {
                    angle = 1.57f - ((float)npc.frameCounter / 20f) * 3.14f;
                    BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);

                    BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(60), 0.0872f);

                }
                angle = 1.57f - ((float)npc.frameCounter / 60f) * 2.35619f;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 1);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-96), 0.0872f);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);
            }
            else if (npc.frameCounter < 70)
            {
                BoneName[BoneLabel.FrontWeapon].Visible = false;
                BoneName[BoneLabel.BackWeapon].Visible = false;

                float prog = (((float)npc.frameCounter - 60f) / 10f);
                float angle = -1.57f + 4.71239f * prog;
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);
                angle = -0.786f + 3.92699f;
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);

                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(14, 0), 4);
                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-36, 0), 4);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(-10, -65 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 1);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-60), 0.32f);
            }
            else if (npc.frameCounter >= 130)
            {
                npc.localAI[1] = 2;
                BoneName[BoneLabel.FrontWeapon].Visible = true;
                BoneName[BoneLabel.BackWeapon].Visible = true;

                /*float prog = (((float)npc.frameCounter - 130f) / 30f);
                if (prog > 1f)
                    prog = 1f;*/

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, ArmLength() - 1), 3);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-75));

                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(8, 0).RotatedBy(-0.523599f), 3);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-60));

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 1);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-96), 0.0872f);
            }

            CalculateHandIK();
            CalculateLegIK();
        }
        void Idle3(NPC npc)
        {
            npc.localAI[1] = 2;

            BoneName[BoneLabel.FrontWeapon].Visible = true;
            BoneName[BoneLabel.BackWeapon].Visible = true;
            BoneName[BoneLabel.Gauntlet].Visible = false;



            CalculateLegIK();

            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, ArmLength() - 1), 3);
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-75));

            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(8, 0).RotatedBy(-0.523599f), 3);
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-60));

            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);
            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 0.0872f);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
        }
        void Walk3(NPC npc)
        {
            npc.localAI[1] = 2;

            float x = 16 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4));// * npc.spriteDirection;
            float y = 6 * (float)Math.Cos(MathHelper.ToRadians((float)npc.frameCounter * 4));
            if (y < 0)
                y = 0;
            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12 + x, y * -1), 3);
            y = 6 * (float)Math.Cos(MathHelper.ToRadians((float)npc.frameCounter * 4));
            if (y > 0)
                y = 0;
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0 - x, y), 3);

            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, ArmLength() - 1), 3);
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-75));

            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(8, 0).RotatedBy(-0.523599f), 3);
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-60));

            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -74 + 1f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 8))), 3);
            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);

            CalculateLegIK();
            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(100), 0.0872f);
        }
        void JustitiaSwing(NPC npc)
        {
            if (npc.frameCounter < 45)
            {
                float prog = (float)npc.frameCounter / 20;
                if (prog > 1)
                    prog = 1;

                Vector2 position = new Vector2(0, ArmLength() - 1 - 6 * prog).RotatedBy(-1.57f * prog);
                float rotation = (position).ToRotation() - 1.57f;
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(rotation, 0.8f);

                if (prog == 1)
                    position += new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                BoneName[BoneLabel.HandLIK].ChangeOffset(position);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);
            }
            else
            {
                float prog = ((float)npc.frameCounter - 45) / 6;
                if (prog > 1)
                    prog = 1;

                Vector2 position = new Vector2(-4, ArmLength() - 1).RotatedBy(-1.57f + 3.3f * (float)Math.Sin(1.57f * prog));
                BoneName[BoneLabel.HandLIK].ChangeOffset(position);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-75 - 15 * prog), 1.2f);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-24, 0), 3);
                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(-4, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);
            }

            BoneName[BoneLabel.FrontWeapon].Visible = true;
            BoneName[BoneLabel.BackWeapon].Visible = true;
            BoneName[BoneLabel.Gauntlet].Visible = false;


            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

            CalculateLegIK();

            //BoneName[Bone.HandLIK].ChangeOffset(new Vector2(-4, ArmLength() - 1));
            //BoneName[Bone.Weapon1].ChangeRotation(BoneName[Bone.LowerArmL].GetRotation() + MathHelper.ToRadians(-75);// * npc.spriteDirection);

            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(8, 0).RotatedBy(-0.523599f), 3);
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-60));

            CalculateHandIK();

            //BoneName[Bone.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))));

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
        }
        bool SmileSwing(NPC npc)
        {
            bool LookAtPlayer = true;
            //BoneName[Bone.Weapon1].Visible = false;
            Vector2 justitaPlacement = BoneName[BoneLabel.Origin].GetPosition() + new Vector2(-36, -90);
            if (npc.frameCounter < 60)
            {
                float prog = (float)npc.frameCounter / 60;
                BoneName[BoneLabel.BackWeapon].ChangeScale(1f + (1f * prog));

                BoneName[BoneLabel.HandLIK].ChangeOffset(justitaPlacement - BoneName[BoneLabel.UpperArmL].GetPosition());
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);

                Vector2 position = new Vector2(8 + 20 * prog, 0).RotatedBy(MathHelper.ToRadians(-30 - 30 * prog));
                BoneName[BoneLabel.HandRIK].ChangeOffset(position, 8f);

                Vector2 offset = new Vector2(36 * prog, 0).RotatedBy(BoneName[BoneLabel.BackWeapon].GetRotation() - BoneName[BoneLabel.LowerArmR].GetRotation());
                BoneName[BoneLabel.BackWeapon].ChangeOffset(offset);
                //float rotation = (position).ToRotation() + MathHelper.ToRadians(60);
                //BoneName[Bone.Weapon2].ChangeRotation(rotation, 0.8f);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(100), 0.0872f);
            }
            else if (npc.frameCounter < 90)
            {
                float prog = ((float)npc.frameCounter - 60) / 10;
                if (prog > 1)
                    prog = 1;

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-12, ArmLength() - 4), 3);

                Vector2 position = new Vector2(28 - 16 * prog, 0).RotatedBy(MathHelper.ToRadians(-60 + 180 * prog));
                BoneName[BoneLabel.HandRIK].ChangeOffset(position, 8f);

                float rotation = (position).ToRotation() - MathHelper.ToRadians(80);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(rotation, 0.8f);

                Vector2 offset = new Vector2(36, 0).RotatedBy(BoneName[BoneLabel.BackWeapon].GetRotation() - BoneName[BoneLabel.LowerArmR].GetRotation());
                BoneName[BoneLabel.BackWeapon].ChangeOffset(offset);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-48, 0), 8);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(2, 0), 8);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -62), 8);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-45), 0.8f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(140), 0.0872f);
            }
            else if (npc.frameCounter < 150)
            {
                float prog = ((float)npc.frameCounter - 90) / 60;
                if (prog > 1)
                    prog = 1;

                Vector2 position = new Vector2(12 + 16 * prog, 0).RotatedBy(MathHelper.ToRadians(120 - 210 * prog));
                BoneName[BoneLabel.HandRIK].ChangeOffset(position, 8f);

                float rotation = (position).ToRotation() - MathHelper.ToRadians(80);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(rotation, 0.8f);

                position += (BoneName[BoneLabel.UpperArmR].GetPosition() - BoneName[BoneLabel.UpperArmL].GetPosition()) + new Vector2(-16, 0).RotatedBy(BoneName[BoneLabel.BackWeapon].GetRotation());
                BoneName[BoneLabel.HandLIK].ChangeOffset(position, 8f);

                Vector2 offset = new Vector2(36, 0).RotatedBy(BoneName[BoneLabel.BackWeapon].GetRotation() - BoneName[BoneLabel.LowerArmR].GetRotation());
                BoneName[BoneLabel.BackWeapon].ChangeOffset(offset);


                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-45 - 10f * prog), 0.8f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(110), 0.0872f);
            }
            else if (npc.frameCounter < 240)
            {
                float prog = ((float)npc.frameCounter - 150) / 10;
                if (prog > 1)
                    prog = 1;

                LookAtPlayer = false;
                BoneName[BoneLabel.Head].ChangeRotation(-1.57f + 0.875f * npc.spriteDirection, 0.0872f);

                Vector2 position = new Vector2(28 - 8 * prog, 0).RotatedBy(MathHelper.ToRadians(-90 + 145 * prog));
                BoneName[BoneLabel.HandRIK].ChangeOffset(position, 8f);

                float rotation = (position).ToRotation() - MathHelper.ToRadians(50);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(rotation, 0.8f);

                position += (BoneName[BoneLabel.UpperArmR].GetPosition() - BoneName[BoneLabel.UpperArmL].GetPosition()) + new Vector2(-16, 0).RotatedBy(BoneName[BoneLabel.BackWeapon].GetRotation());
                BoneName[BoneLabel.HandLIK].ChangeOffset(position, 8f);

                Vector2 offset = new Vector2(36, 0).RotatedBy(BoneName[BoneLabel.BackWeapon].GetRotation() - BoneName[BoneLabel.LowerArmR].GetRotation());
                BoneName[BoneLabel.BackWeapon].ChangeOffset(offset);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -42), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-10 + Main.rand.NextFloat(-5, 5)), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(210), 0.0872f);
            }
            else// if (npc.frameCounter < 210)
            {
                float prog = ((float)npc.frameCounter - 240) / 30;
                if (prog > 1)
                    prog = 1;

                BoneName[BoneLabel.BackWeapon].ChangeScale(2f - (1f * prog));

                Vector2 position = new Vector2(20 - 12 * prog, 0).RotatedBy(MathHelper.ToRadians(55 - 85 * prog));
                BoneName[BoneLabel.HandRIK].ChangeOffset(position);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-60), 0.4f);

                Vector2 offset = new Vector2(36 - 36 * prog, 0).RotatedBy(BoneName[BoneLabel.BackWeapon].GetRotation() - BoneName[BoneLabel.LowerArmR].GetRotation());
                BoneName[BoneLabel.BackWeapon].ChangeOffset(offset);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);

                BoneName[BoneLabel.HandLIK].ChangeOffset(justitaPlacement - BoneName[BoneLabel.UpperArmL].GetPosition(), 3);
            }

            CalculateLegIK();
            CalculateHandIK();

            if (npc.frameCounter >= 60 && npc.frameCounter < 260)
            {
                BoneName[BoneLabel.FrontWeapon].ChangeOffset((justitaPlacement - BoneName[BoneLabel.LowerArmL].EndPoint()).RotatedBy(-BoneName[BoneLabel.LowerArmL].GetRotation()));
            }
            else
            {
                BoneName[BoneLabel.FrontWeapon].ChangeOffset(Vector2.Zero);
            }

            //LookingAt(BoneName[Bone.Weapon1].GetPosition(), 64);
            //LookingAt(justitaPlacement, 64);
            return LookAtPlayer;
        }
        bool Phase4Transition(NPC npc)
        {
            bool LookAtPlayer = true;
            if (npc.frameCounter < 30)
            {
                float prog = ((float)npc.frameCounter) / 30f;
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(MathHelper.ToRadians(-90 * prog)), 3);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - MathHelper.ToRadians(90), 0.3f);

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(0, ArmLength() - 1), 3);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-75));

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);
            }
            else if (npc.frameCounter < 75)
            {
                LookAtPlayer = false;
                BoneName[BoneLabel.Head].ChangeRotation(-1.57f + MathHelper.ToRadians(45 * npc.spriteDirection), 0.0872f);
                float prog = ((float)npc.frameCounter - 30f) / 10f;
                if (prog > 1f)
                    prog = 1f;

                Vector2 offset = new Vector2(38 * prog, 0).RotatedBy(BoneName[BoneLabel.BackWeapon].GetRotation() - BoneName[BoneLabel.LowerArmR].GetRotation());
                BoneName[BoneLabel.BackWeapon].ChangeOffset(offset, 3);
                BoneName[BoneLabel.BackWeapon].ChangeScale(1f + 0.5f * prog);

                Vector2 position = new Vector2(ArmLength() - 8, 0).RotatedBy(MathHelper.ToRadians(-90 + 90 * prog));
                Vector2 position2 = position + new Vector2(0, 100);
                if (prog >= 1f)
                {
                    position.X += Main.rand.NextFloat(-1, 1);
                    position.Y += Main.rand.NextFloat(-1, 1);
                }
                BoneName[BoneLabel.HandRIK].ChangeOffset(position, 3);
                BoneName[BoneLabel.BackWeapon].ChangeRotation((position2 - position).ToRotation(), 0.3f);

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-48, ArmLength() - 8), 3);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-75));

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-14, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(2, 0), 3);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -62), 8);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.8f);
            }
            else if (npc.frameCounter < 90)
            {
                BoneName[BoneLabel.BackWeapon].Visible = false;

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, ArmLength() - 1), 3);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-75));

                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1), 3);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 0.0872f);
            }
            else if (npc.frameCounter < 120)
            {
                float prog = ((float)npc.frameCounter - 90f) / 30f;

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(1.57f - prog * 3.14f), 6);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - 1.309f, 1.57f);

                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1), 3);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 - 6), 0.0872f);
            }
            else if (npc.frameCounter < 130)
            {
                npc.localAI[1] = 3;
                float prog = ((float)npc.frameCounter - 120f) / 10f;

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(-1.57f + prog * MathHelper.ToRadians(280)), 16);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - 1.309f, 1.57f);

                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1), 3);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);
            }
            CalculateHandIK();
            CalculateLegIK();
            return LookAtPlayer;
        }
        void Idle4(NPC npc)
        {
            float animSpeedMult = 8;
            BoneName[BoneLabel.FrontWeapon].Visible = true;
            BoneName[BoneLabel.BackWeapon].Visible = true;
            BoneName[BoneLabel.Gauntlet].Visible = false;

            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-36, -5), 3);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(18, -5), 3);

            CalculateLegIK();

            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, ArmLength() - 1), 3);
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-60));
            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1), 3);
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-45));

            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -52 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * animSpeedMult))), 3);
            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-80 + 6 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * animSpeedMult))), 0.0872f);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(100), 0.0872f);

            BoneName[BoneLabel.FrontWeapon].ChangeScale(1.3f, 0.15f);
        }
        void TwilightDashSlash(NPC npc)
        {
            float prog = (float)npc.frameCounter / 15f;
            if (prog > 1f)
                prog = 1f;
            prog = (float)Math.Sin(prog * 1.57f);
            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(MathHelper.ToRadians(150 - 270 * prog)), 16);

            CalculateHandIK();

            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation());

            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-36, 0), 12f);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(36, 0), 12f);

            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-50), 0.0872f);
            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(14, -40), 3);

            CalculateLegIK();

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(140), 0.0872f);
        }
        void TwilightDashSlash2(NPC npc)
        {
            float prog = (float)npc.frameCounter / 15f;
            if (prog > 1f)
                prog = 1f;
            prog = (float)Math.Sin(prog * 1.57f);
            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(MathHelper.ToRadians(-120 + 270 * prog)), 16);

            CalculateHandIK();

            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation());

            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-36, 0), 12f);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(36, 0), 12f);

            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-50), 0.0872f);
            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(14, -40), 3);

            CalculateLegIK();

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(140), 0.0872f);
        }
        bool TwilightFinisher(NPC npc)
        {
            bool LookAtPlayer = true;
            if (npc.frameCounter < 20)
            {
                float prog = (float)npc.frameCounter / 15f;
                if (prog > 1f)
                    prog = 1f;
                prog = (float)Math.Sin(prog * 1.57f);

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength() - 8f, 0).RotatedBy(MathHelper.ToRadians(150 - 270 * prog)), 16);
                CalculateHandIK();

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-48, -48 * prog), 12f);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(8, -56 * prog), 12f);
                CalculateLegIK();

                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - MathHelper.ToRadians(45));

                prog = (float)npc.frameCounter / 20f;
                if (prog > 1f)
                    prog = 1f;
                prog = (float)Math.Sin(prog * 1.57f);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -40 - 80 * prog), 16f);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-50 - 65 * prog));

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(100), 0.0872f);
            }
            else
            {
                float prog = (float)(npc.frameCounter - 15f) / 5f;
                if (prog > 1f)
                    prog = 1f;
                //prog = 1f + (float)Math.Sin(prog * 1.57f - 1.57f);

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(MathHelper.ToRadians(-120 + 165 * prog)), 16);
                CalculateHandIK();

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-48, -4), 12f);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(16, -4), 12f);
                CalculateLegIK();

                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - MathHelper.ToRadians(20));

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -40), 16f);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-50), 0.18f);

                BoneName[BoneLabel.FrontWeapon].ChangeScale(1.5f);

                BoneName[BoneLabel.Head].ChangeRotation(npc.spriteDirection == 1 ? -0.79f : -2.35f);
                LookAtPlayer = false;

                if (npc.frameCounter < 25f)
                    BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(250), 0.12f);
                else
                    BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(140), 0.0872f);
            }
            return LookAtPlayer;
        }
        bool TwilightEnd(NPC npc) //I KNEEL
        {
            float prog = MathHelper.ToRadians((float)npc.frameCounter);
            prog = (float)Math.Sin(prog * 4);
            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength() - 4, 0).RotatedBy(MathHelper.ToRadians(-200)), 16);

            CalculateHandIK();

            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - MathHelper.ToRadians(45), 0.14f);

            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-BoneName[BoneLabel.LowerLegL].Length, -4), 12f);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(BoneName[BoneLabel.UpperLegR].Length + 12, -4), 12f);

            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-50 + 8 * prog), 0.0872f);
            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(14, -BoneName[BoneLabel.UpperLegL].Length), 3);

            CalculateLegIK();

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(140), 0.0872f);

            BoneName[BoneLabel.Head].ChangeRotation(npc.spriteDirection == 1 ? -0.79f : -2.35f);

            BoneName[BoneLabel.FrontWeapon].ChangeScale(1.3f, 0.15f);
            return false;
        }
        void TwilightChase(NPC npc)
        {
            float animSpeedMult = (int)(npc.velocity.Length() * 2);
            if (animSpeedMult > 20)
                animSpeedMult = 20;
            //Main.NewText(animSpeedMult);
            float x = 16 * 3 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * animSpeedMult));// * npc.spriteDirection;
            float y = 16 * (float)Math.Cos(MathHelper.ToRadians((float)npc.frameCounter * animSpeedMult));
            if (y < 0)
                y = 0;
            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12 + x, y * -1)); //].ChangeOffset(new Vector2(-12 + x, y * -1), 30);
            y = 16 * (float)Math.Cos(MathHelper.ToRadians((float)npc.frameCounter * animSpeedMult));
            if (y > 0)
                y = 0;
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0 - x, y));//].ChangeOffset(, 30);

            float velocityRotation = 0;//(float)Math.Atan2(NPC.velocity.Y * npc.spriteDirection, NPC.velocity.X * npc.spriteDirection);
            BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength() - 2, 0).RotatedBy(MathHelper.ToRadians(150) + velocityRotation), 3);
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(10));
            BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(MathHelper.ToRadians(150) + velocityRotation), 3);
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-45));

            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-50), 0.0872f);
            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(14, -40 + 0.6f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * animSpeedMult))), 3);

            CalculateLegIK();
            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(160), 0.0872f);

            BoneName[BoneLabel.FrontWeapon].ChangeScale(1.3f, 0.15f);
        }
        bool TwilightLamp(NPC npc)
        {
            bool LookAtPlayer = true;
            if (npc.frameCounter < 60)
            {

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-26, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(10, 0), 3);

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(-90), 16);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation(), 0.0872f);
                //BoneName[Bone.Weapon1].ChangeRotation(BoneName[Bone.LowerArmL].GetRotation() + MathHelper.ToRadians(-75);// * npc.spriteDirection);
                //BoneName[Bone.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1));
                //BoneName[Bone.Weapon2].ChangeRotation(BoneName[Bone.LowerArmR].GetRotation() + MathHelper.ToRadians(-45);// * npc.spriteDirection);

                CalculateHandIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -62), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);

                LookAtPlayer = false;
                BoneName[BoneLabel.Head].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
            }
            else
            {
                float prog = 1f;
                if (npc.frameCounter < 70)
                    prog = (float)(npc.frameCounter - 60f) / 10f;

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-36, -5), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(18, -5), 3);

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(MathHelper.ToRadians(-90 + 270 * prog)), 64);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation());

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -52 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter))), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-50 + 6 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter))), 0.1f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(150), 0.1f);
            }
            CalculateLegIK();
            CalculateHandIK();
            return LookAtPlayer;
        }

        void GoldRushThrow(NPC npc)
        {
            float animSpeedMult = 8;
            Vector2 twilightPlacement = BoneName[BoneLabel.Origin].GetPosition() + new Vector2(-36, -90);

            if (npc.frameCounter < 10)
            {
                BoneName[BoneLabel.HandLIK].ChangeOffset(twilightPlacement - BoneName[BoneLabel.UpperArmL].GetPosition(), 12f);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(MathHelper.ToRadians(120), 1.4f);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-36, -5), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(24, -5), 3);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
            }
            else if (npc.frameCounter < 45)
            {
                float prog = (float)(npc.frameCounter - 10) / 35;
                BoneName[BoneLabel.Gauntlet].Visible = true;
                BoneName[BoneLabel.Gauntlet].ChangeScale(1f);

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength() - 4, 0).RotatedBy(MathHelper.ToRadians(120 - 210 * prog)), 16);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-36, -5), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(24, -5), 3);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -62 + 0.5f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * animSpeedMult))), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);
            }
            else if (npc.frameCounter < 105)
            {
                BoneName[BoneLabel.Gauntlet].ChangeScale(2f, .01f);

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength() - 4 + Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)).RotatedBy(MathHelper.ToRadians(-90)), 3);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-36, -5), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(24, -5), 3);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -62 + 0.5f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * animSpeedMult))), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);
            }
            else if (npc.frameCounter < 155)
            {
                float prog = (float)(npc.frameCounter - 105) / 50f;

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength() - 4, 0).RotatedBy(MathHelper.ToRadians(-90 + 270 * prog)), 16);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -52 + 0.5f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * animSpeedMult))), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-75), 0.0872f);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-36, -5), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(24, -5), 3);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(110), 0.0872f);

            }
            else if (npc.frameCounter < 165)
            {
                float prog = (float)(npc.frameCounter - 155) / 10f;

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2((ArmLength() - 4) + ArmLength() * prog, 4), 16);
                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -52 + 0.5f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * animSpeedMult))));

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(24, -5), 16);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(-36, -5), 16);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(10, -57 + 0.5f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * animSpeedMult))), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(130), 0.0872f);

                if (prog > 0.5f)
                {
                    BoneName[BoneLabel.Gauntlet].Visible = false;
                    BoneName[BoneLabel.Gauntlet].ChangeScale(1f);
                }
            }
            else if (npc.frameCounter < 195)
            {
                BoneName[BoneLabel.Gauntlet].Visible = false;
                BoneName[BoneLabel.Gauntlet].ChangeScale(1f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(110), 0.0872f);
            }
            else
            {
                BoneName[BoneLabel.HandLIK].ChangeOffset(twilightPlacement - BoneName[BoneLabel.UpperArmL].GetPosition(), 12f);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-36, -5), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(18, -5), 3);

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -52 + 0.5f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * animSpeedMult))), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-80 + 6 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * animSpeedMult))), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);

            }

            CalculateHandIK();

            CalculateLegIK();

            if (npc.frameCounter >= 10)
                BoneName[BoneLabel.FrontWeapon].ChangeOffset((twilightPlacement - BoneName[BoneLabel.LowerArmL].EndPoint()).RotatedBy(-BoneName[BoneLabel.LowerArmL].GetRotation()));
            else if (npc.frameCounter > 135)
                BoneName[BoneLabel.FrontWeapon].ChangeOffset(Vector2.Zero);
        }

        #endregion

        #region Master Animation Functions

        /// <summary>
        /// -1 Front, +1 Back
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="which"></param>
        public void ArmSwing(NPC npc, float length, float rot, float speed, int which = 0, bool updateIK = true)
        {
            if (which < 1)
            {
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(length, 4).RotatedBy(rot), speed);
            }
            if (which > -1)
            {
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(length, 4).RotatedBy(rot), speed);
            }
            if (updateIK)
                CalculateHandIK();
        }

        /// <summary>
        /// -1 Front, +1 Back. Length 0 to 1 of ArmLength. rot uses degrees
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="length"></param>
        /// <param name="rot"></param>
        /// <param name="speed"></param>
        /// <param name="which"></param>
        /// <param name="updateIK"></param>
        public void PoseArm(NPC npc, float length, float rot, float speed, int which = 0, bool updateIK = true)
        {
            rot = MathHelper.ToRadians(rot);
            if (which < 1)
            {
                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength() * length, 4).RotatedBy(rot), speed);
            }
            if (which > -1)
            {
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(ArmLength() * length, 4).RotatedBy(rot), speed);
            }
            if (updateIK)
                CalculateHandIK();
        }

        /// <summary>
        /// Adds (rot) value to LowerArmX's rotation and gives result to XWeapon, -1 Front, +1 Back
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="length"></param>
        /// <param name="rot"></param>
        /// <param name="speed"></param>
        /// <param name="which"></param>
        /// <param name="updateIK"></param>
        public void HeldItemRotation(NPC npc, float rot, float speed = -1, int which = 0)
        {
            if (which < 1)
            {
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + rot, speed);// * npc.spriteDirection);
            }
            if (which >- 1)
            {
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + rot, -speed);// * npc.spriteDirection);
            }
        }

        /// <summary>
        /// Point of reference for Body Idle Params
        /// </summary>
        /// <param name="npc"></param>
        void IdleBody(NPC npc)
        {
            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 6f);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 6);

            CalculateLegIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 6);
            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 0.0872f);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
        }

        /// <summary>
        /// Point of reference for 
        /// </summary>
        void DashBody()
        {
            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-80, -10), 10);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, -24), 10);

            CalculateLegIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -38), 10);
            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-30), 0.0872f);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(140), 0.0872f);
        }

        bool RedEyesPenitenceIntro(NPC npc)
        {
            if (npc.frameCounter < 60)
            {
                BoneName[BoneLabel.FrontWeapon].Visible = true;
                BoneName[BoneLabel.BackWeapon].Visible = true;
                BoneName[BoneLabel.Gauntlet].Visible = false;

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(20, 0), 3);

                CalculateLegIK();

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, ArmLength() - 1).RotatedBy(MathHelper.ToRadians(30)), 3);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-45));// * npc.spriteDirection);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1).RotatedBy(MathHelper.ToRadians(30)), 3);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-45));// * npc.spriteDirection);

                CalculateHandIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -35), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-45), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(135), 0.0872f);
            }
            else if (npc.frameCounter < 90)
            {
                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-LegLength() / 2, 0), 6);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(4, -52), 6);

                CalculateLegIK();

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-2, -4), 10);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(0), 0.1f);// * npc.spriteDirection);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(-ArmLength() + 8, 8), 10);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(0), 0.1f);// * npc.spriteDirection);

                CalculateHandIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76), 6);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-120), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(120), 0.0872f);
                BoneName[BoneLabel.Head].ChangeRotation(-1.57f);
                return false;
            }
            else
            {
                float prog = ((float)npc.frameCounter - 90f) / 10f;
                if (prog > 1f)
                    prog = 1f;
                prog = (float)Math.Sin(1.57f * prog);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-30, 0), 6);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(30, 0), 6);

                CalculateLegIK();

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-ArmLength(), 4).RotatedBy(MathHelper.ToRadians(350) * prog));
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation());// * npc.spriteDirection);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(-ArmLength(), 0).RotatedBy(MathHelper.ToRadians(350) * prog));
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation());// * npc.spriteDirection);

                CalculateHandIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(12, -40), 6);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-45), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(135), 0.1272f);
            }
            return true;
        }
        
        void WingbeatWield(NPC npc)
        {
            if (npc.frameCounter < 60)
            {
                BoneName[BoneLabel.FrontWeapon].Visible = false;

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 6f);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 6);

                CalculateLegIK();

                BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, -ArmLength()), 3);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - 1.309f, 1.57f);
                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1), 3);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-45));// * npc.spriteDirection);

                CalculateHandIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 6);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
            }
            else if (npc.frameCounter < 120)
            {
                BoneName[BoneLabel.FrontWeapon].Visible = true;

                if (npc.frameCounter < 80)
                {
                    BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(-4, -ArmLength()), 3);

                    BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 - 6), 0.0872f); // * npc.spriteDirection));
                }
                else if (npc.frameCounter < 90)
                {
                    float angle = -1.57f + (((float)npc.frameCounter - 80) / 10) * 4.71f;// * npc.spriteDirection;
                    BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);

                    BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-80), 0.0872f);// * npc.spriteDirection));
                }
                else if (npc.frameCounter < 110)
                {
                    float angle = 3.14f;
                    BoneName[BoneLabel.HandLIK].ChangeOffset(new Vector2(ArmLength(), 0).RotatedBy(angle), 16);
                }
                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

                BoneName[BoneLabel.HandRIK].ChangeOffset(new Vector2(4, ArmLength() - 1), 3);

                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() - MathHelper.ToRadians(45));
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() - 1.309f, 1.57f);// * npc.spriteDirection, 1.57f);

                CalculateLegIK();
                CalculateHandIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
            }
        }
        
        bool EXRun(NPC npc)
        {
            float speed = 16;// * (Math.Abs(NPC.velocity.X) / 5f);

            float x = 50 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * speed));// * npc.spriteDirection;
            float y = 30 * (float)Math.Cos(MathHelper.ToRadians((float)npc.frameCounter * speed - 20));
            if (y < 0)
                y = 0;
            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12 + x, y * -1));
            y = 30 * (float)Math.Cos(MathHelper.ToRadians((float)npc.frameCounter * speed - 20));
            if (y > 0)
                y = 0;
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0 - x, y));

            int type = GetWeaponType((int)npc.localAI[1]);
            HandIdleRunPose(npc, type, 1, true);

            type = GetWeaponType((int)npc.localAI[3]);
            if ((int)npc.localAI[3] == ModContent.ItemType<Tough>())
            {
                Vector2 position = BoneName[BoneLabel.UpperArmR].GetPosition();
                Vector2 delta = npc.GetTargetData().Center - position;
                delta.Normalize();
                float rot = (float)Math.Atan2(delta.Y, delta.X * npc.spriteDirection);

                float blowback = ((float)npc.ai[2] % 60 - 50) / 10f;
                if (blowback < 0f)
                    blowback = 1f;
                ArmSwing(npc, ArmLength() - 4 + 4 * blowback, rot, 6f, 1);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation());
            }
            else
            {
                HandIdleRunPose(npc, type, 1, true);
            }
            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-30), 0.0872f);
            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(16, -44), 3);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(160), 0.0872f);
            CalculateLegIK();

            return true;
        }

        void BothSwingDashLoop(NPC npc)
        {
            int legChange = (int)npc.frameCounter % 30;

            if (legChange < 15)
            {
                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-24, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(6, 0), 3);

                CalculateLegIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -44), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-45), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(135), 0.0872f);
            }
            else if (legChange < 30)
            {
                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-80, -10), 10);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, -24), 10);

                CalculateLegIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -38), 10);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-30), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(140), 0.0872f);
            }

            if (npc.frameCounter < 15)
            {
                BoneName[BoneLabel.FrontWeapon].Visible = true;
                BoneName[BoneLabel.BackWeapon].Visible = true;

                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-15));// * npc.spriteDirection);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-15));// * npc.spriteDirection);
                ArmSwing(npc, 20f, MathHelper.ToRadians(170), 3);
            }
            else if (npc.frameCounter < 30)
            {
                float prog = ((float)(npc.frameCounter - 30f) / 15f);

                float extraRot = -340 * prog;
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-15));// * npc.spriteDirection);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-15));// * npc.spriteDirection);
                ArmSwing(npc, 20f, MathHelper.ToRadians(170 + extraRot), -1);
            }
            else if (npc.frameCounter < 45)
            {
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-15));// * npc.spriteDirection);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-15));// * npc.spriteDirection);
                ArmSwing(npc, 19.5f, MathHelper.ToRadians(-170), 3);
            }
            else if (npc.frameCounter < 60)
            {
                float prog = ((float)(npc.frameCounter - 30f) / 15f);

                float extraRot = 340 * prog;
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-15));// * npc.spriteDirection);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-15));// * npc.spriteDirection);
                ArmSwing(npc, 18f, MathHelper.ToRadians(-170 + extraRot), -1);
            }
            else
            {
                npc.frameCounter = 0;
            }
        }

        void PenitenceRaise(NPC npc)
        {
            float prog = Math.Clamp(((float)npc.frameCounter - 60) / 15f, 0f, 1f);

            ArmSwing(npc, ArmLength(), 0, 3, 1);
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation(), 0.2f);
        }

        void PenitenceComboSlam(NPC npc)
        {
            if (npc.frameCounter < 30) // Wind up
            {
                float prog = ((float)npc.frameCounter) / 30f;
                if (prog > 1f)
                    prog = 1f;
                prog = (float)Math.Sin(1.57f * prog);

                ArmSwing(npc, ArmLength() - 4, MathHelper.ToRadians(-100 * prog), 3, 1, false);
                ArmSwing(npc, ArmLength() - 1, MathHelper.ToRadians(135), 3, -1, false);
                CalculateHandIK();
                HeldItemRotation(npc, -1.57f, -1, 1);
                HeldItemRotation(npc, -MathHelper.ToRadians(120), -1, -1);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-24, 0), 6f);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(14, 0), 6);

                CalculateLegIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0,-76), 6);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-80), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
            }
            else if (npc.frameCounter < 60) // Penitence Swing
            {
                float prog = ((float)npc.frameCounter - 30f) / 15f;
                if (prog > 1f)
                    prog = 1f;
                prog = (float)Math.Sin(1.57f * prog);
                ArmSwing(npc, ArmLength(), MathHelper.ToRadians(-100 + 180 * prog), -1, 1, false);
                ArmSwing(npc, ArmLength() - 4, MathHelper.ToRadians(180), 3, -1, false);
                CalculateHandIK();
                HeldItemRotation(npc, -MathHelper.ToRadians(45), -1, 1);
                HeldItemRotation(npc, -MathHelper.ToRadians(45), 0.8f, -1);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-30, 0), 6f);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(30, 0), 6);

                CalculateLegIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(16, -64), 6);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-60), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(110), 0.0872f);
            }
            else if (npc.frameCounter < 75) // Wingbeat Dash
            {
                float prog = ((float)npc.frameCounter - 60) / 15f;
                if (prog > 1f)
                    prog = 1f;
                prog = (float)Math.Sin(1.57f * prog);
                ArmSwing(npc, ArmLength() - 4, MathHelper.ToRadians(-135), 3, 1, false);
                ArmSwing(npc, ArmLength(), MathHelper.ToRadians(180 - 280  *prog), -1, -1, false);
                CalculateHandIK();
                HeldItemRotation(npc, -MathHelper.ToRadians(90), -1, 1);
                HeldItemRotation(npc, 0, -1, -1);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-80, -10), 10);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, -24), 10);

                CalculateLegIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -38), 10);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-30), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(140), 0.0872f);
            }
            else if (npc.frameCounter < 85) // Wingbeat Rest
            {
                ArmSwing(npc, ArmLength() - 4, MathHelper.ToRadians(-135), 3, 1, false);
                ArmSwing(npc, ArmLength(), MathHelper.ToRadians(-100), -1, -1, false);
                CalculateHandIK();
                HeldItemRotation(npc, -MathHelper.ToRadians(90), 0.8f, 1);
                HeldItemRotation(npc, 0, -1, -1);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-24, 0), 3);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(6, 0), 3);

                CalculateLegIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -44), 3);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-45), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(135), 0.0872f);
            }
            else if (npc.frameCounter < 115) // Raise Both
            {
                ArmSwing(npc, ArmLength() - 2, MathHelper.ToRadians(-110), 10);
                HeldItemRotation(npc, MathHelper.ToRadians(-45), 0.8f);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 6f);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 6);

                CalculateLegIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76), 6);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
            }
            else // Swing Both
            {
                float prog = ((float)npc.frameCounter - 115) / 15f;
                if (prog > 1f)
                    prog = 1f;
                prog = (float)Math.Sin(1.57f * prog);

                ArmSwing(npc, ArmLength() - 2, MathHelper.ToRadians(-110 + 200 * prog), -1);
                HeldItemRotation(npc, MathHelper.ToRadians(-45));

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-30, 0), 6f);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(30, 0), 6);

                CalculateLegIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(8, -64), 6);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-45), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(110), 0.0872f);
            }
        }
        
        void AimArmAtTarget(NPC npc, float length, float speed, int which = 0)
        {
            Vector2 position = BoneName[BoneLabel.UpperArmR].GetPosition();
            Vector2 delta = npc.GetTargetData().Center - position;
            delta.Normalize();
            float rot = (float)Math.Atan2(delta.Y, delta.X * npc.spriteDirection);

            ArmSwing(npc, length, rot, speed, which);
        }

        bool ToughShoot(NPC npc)
        {
            BoneName[BoneLabel.HandLIK].ChangeBone(new Vector2(-4, ArmLength()), 3, 
                                                   BoneName[BoneLabel.LowerArmL].GetRotation() - 1.309f, 1.57f);
            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation() + MathHelper.ToRadians(-45));

            float blowback = ((float)npc.ai[2] % 15 - 5) / 10f;
            if (blowback < 0f || npc.ai[2] <= 0)
                blowback = 1f;
            AimArmAtTarget(npc, ArmLength() - 4 + 4 * blowback, 6f, 1);

            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation());

            IdleBody(npc);
            return true;
        }

        bool SodaShoot(NPC npc)
        {
            BoneName[BoneLabel.HandRIK].ChangeBone(new Vector2(-4, ArmLength()), 3,
                                                   BoneName[BoneLabel.LowerArmR].GetRotation() - 1.309f, 1.57f);
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation() + MathHelper.ToRadians(-45));

            float blowback = ((float)npc.ai[2] % 15 - 5) / 10f;
            if (blowback < 0f || npc.ai[2] <= 0)
                blowback = 1f;
            AimArmAtTarget(npc, ArmLength() - 4 + 4 * blowback, 6f, -1);

            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation());
            IdleBody(npc);

            return true;
        }

        bool DoubleShoot(NPC npc)
        {
            float blowback = ((float)npc.ai[2] % 15 - 5) / 10f;
            if (blowback < 0f || npc.ai[2] <= 0)
                blowback = 1f;
            AimArmAtTarget(npc, ArmLength() - 4 + 4 * blowback, 6f, 0);

            BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation());
            BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation());
            IdleBody(npc);

            return true;
        }
        
        void WristCutterStart(NPC npc)
        {
            if (npc.frameCounter < 30)
            {
                PoseArm(npc, 0.5f, 170, 6f);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(MathHelper.ToRadians(90), 1f);
                BoneName[BoneLabel.BackWeapon].ChangeRotation(MathHelper.ToRadians(90), 1f);
                
                IdleBody(npc);
            }
            else if (npc.frameCounter < 90)
            {
                PoseArm(npc, 1f, 135, 6f, -1, false);
                PoseArm(npc, 1f, 45, 6f, 1);

                IdleBody(npc);
            }
            else if (npc.frameCounter < 120)
            {
                PoseArm(npc, 1f, 0, 6f, 1, false);
                PoseArm(npc, 0.4f, -20, 6f, -1);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation());

                IdleBody(npc);
            }
            else if (npc.frameCounter < 180)
            {
                float time = ((float)npc.frameCounter % 60f)/ 29f;
                if (time > 1f)
                    time = 1f;
                PoseArm(npc, 1f, 45 * time, 16f, 1, false);
                time = time == 1 ? 1 : 1 - (float)Math.Pow(2, -10 * time);
                PoseArm(npc, 0.4f, -20 + 210 * time, 16f, -1);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation());

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 10);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(12, 0), 10);

                CalculateLegIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -45), 10);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-65), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(140), 0.0872f);
            }
            else if (npc.frameCounter < 210)
            {
                PoseArm(npc, .3f, 75f, 6f, 1, false);
                float time = ((float)npc.frameCounter % 30f) / 14f;
                time = time >= 1 ? 1 : 1 - (float)Math.Pow(2, -10 * time);
                PoseArm(npc, 0.4f + 0.6f * time, 190 - 280 * time, 16f, -1);

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-18, 0), 10);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 10);

                CalculateLegIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -70), 10);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
            }
            else if (npc.frameCounter >= 240 && npc.frameCounter < 270)
            {
                float time = ((float)npc.frameCounter % 30f) / 29f;
                PoseArm(npc, .3f + .7f * time, 75f + 105f * time, 6f, 1, false);
                PoseArm(npc, 1f, -90 + 270 * time, 16f, -1);
                BoneName[BoneLabel.FrontWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmL].GetRotation());
                BoneName[BoneLabel.BackWeapon].ChangeRotation(BoneName[BoneLabel.LowerArmR].GetRotation());

                BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 10);
                BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(12, 0), 10);

                CalculateLegIK();

                BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -65), 10);
                BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-45), 0.0872f);

                BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(140), 0.0872f);
            }
        }

        /// <summary>
        /// -1 Front, 1 Back
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="type"></param>
        /// <param name="dir"></param>
        /// <param name="run"></param>
        void HandIdleRunPose(NPC npc, int type, int dir, bool run = false)
        {
            BoneLabel Hand = dir == -1 ? BoneLabel.HandLIK : BoneLabel.HandRIK;
            BoneLabel Weapon = dir == -1 ? BoneLabel.FrontWeapon : BoneLabel.BackWeapon;
            if (type == 0)
            {
                Vector2 offset = run ? new Vector2(-ArmLength() + 6 + 2 * dir, 2) : new Vector2(4 * dir, ArmLength() - 1);
                BoneName[Hand].ChangeOffset(offset, 3);
                float rot = run ? -17.5f + 2.5f * dir : -60 + 15 * dir;
                BoneName[Weapon].ChangeRotation(BoneName[Weapon].GetParent.GetRotation() + MathHelper.ToRadians(rot));// * npc.spriteDirection);
            }
            else if (type == 1)
            {
                BoneName[Hand].ChangeOffset(new Vector2(8, 0).RotatedBy(-0.523599f), 3);
                BoneName[Weapon].ChangeRotation(BoneName[Weapon].GetParent.GetRotation() + MathHelper.ToRadians(-60));
            }
            else if (type == 4)
            {
                Vector2 position = BoneName[Hand].GetParent.GetParent.GetPosition();
                Vector2 delta = npc.GetTargetData().Center - position;
                delta.Normalize();
                float rot = (float)Math.Atan2(delta.Y, delta.X * npc.spriteDirection);

                float blowback = 1f;
                ArmSwing(npc, ArmLength() - 4 + 4 * blowback, rot, 6f, 1);
                BoneName[Weapon].ChangeRotation(BoneName[Weapon].GetParent.GetRotation());
            }
        }

        // 1 - Front Weapon, 3 - Back Weapon
        void EXIdle1(NPC npc)
        {
            BoneName[BoneLabel.FeetLIK].ChangeOffset(new Vector2(-12, 0), 3f);
            BoneName[BoneLabel.FeetRIK].ChangeOffset(new Vector2(0, 0), 3);

            CalculateLegIK();

            int type = GetWeaponType((int)npc.localAI[1]);
            HandIdleRunPose(npc, type, -1);

            type = GetWeaponType((int)npc.localAI[3]);
            HandIdleRunPose(npc, type, 1);

            CalculateHandIK();

            BoneName[BoneLabel.Pelvis].ChangeOffset(new Vector2(0, -76 + 0.25f * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 3);
            BoneName[BoneLabel.Pelvis].ChangeRotation(MathHelper.ToRadians(-90 + 2 * (float)Math.Sin(MathHelper.ToRadians((float)npc.frameCounter * 4))), 0.0872f);

            BoneName[BoneLabel.Hair].ChangeRotation(MathHelper.ToRadians(90), 0.0872f);
        }


        #endregion
    }
}

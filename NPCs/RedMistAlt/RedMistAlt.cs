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
using Terraria.GameContent;

namespace LobotomyCorp.NPCs.RedMistAlt
{
    //[AutoloadBossHead]
    class RedMistAlt : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 100;
            NPC.lifeMax = 26000;
            //NPC.noTileCollide = true;
            //NPC.noGravity = true;
            //NPC.damage = 240;
            NPC.defense = 12;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.0f;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.timeLeft *= 10000;
            NPC.DeathSound = SoundID.Item14;
            LobotomyGlobalNPC.LNPC(NPC).RiskLevel = (int)RiskLevel.Aleph;
        }
        private SkeletonBase Skelly;

        public override void AI()
        {
            InitializeSkeleton();
        }

        private void InitializeSkeleton()
        {
            Dictionary<Enum, BonePart> Skeleton = new Dictionary<Enum, BonePart>();
            Texture2D tex = TextureAssets.Npc[NPC.type].Value;
            Skeleton.Add(Bone.Origin, new BonePart(new Vector2(NPC.Center.X, NPC.position.Y + NPC.height), 0, 1, 1));
        
            
        }

        enum Bone
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
            Weapon1,
            Weapon2,
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
    }
}

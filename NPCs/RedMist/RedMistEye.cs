using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using LobotomyCorp;
using LobotomyCorp.Util;
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using LobotomyCorp.Util;

namespace LobotomyCorp.Visuals.LobEffects
{
    class FourthMatchSmear : WeaponSmear
    {
        public override void Draw()
        {
            float prog = (1 - (Time / (float)TimeMax));

            CustomShaderData shader = LobotomyCorp.LobcorpShaders["FourthMatchFlame"].UseOpacity(0.5f * (float)Math.Cos(3.15f * prog) + 0.5f);

            int dir = Direction;
            SlashTrail trail = new SlashTrail(180, 45, 0);
            trail.color = Color.Red;
            prog = 1f - prog;
            float offset = MathHelper.ToRadians(-85 - 40 * (float)Math.Sin(1.57f * prog)) * dir;
            trail.DrawEllipse(position, rotation, offset, dir * -1, 400, 75, 128, shader);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using LobotomyCorp.Misc;

namespace LobotomyCorp.Visuals.LobEffects
{
    class BlueStarShine : LobDrawEffects
    {
        float Scale;

        public BlueStarShine(Vector2 pos, int time, float scale, Vector2 vel = default)
        {
            active = true;
            TimeMax = 30;
            velocity = vel;
            position = pos;
            Color = Color.White;
            Scale = scale;
            rotation = Main.rand.NextFloat(0.785f);
        }

        public override void UpdateBehavior()
        {
            rotation += MathHelper.ToRadians(2);
            position += velocity;
            velocity *= 0.98f;
        }

        public override void Draw()
        {
            float prog = Time / (float)TimeMax;
            prog *= prog * prog;
            Texture2D tex = MiscAssets.BlueStarShine.Value;
            Vector2 origin = MiscAssets.BlueStarShine.Size() / 2;

            Main.spriteBatch.Draw(tex, position - Main.screenPosition, null, Color * (0.7f - 0.5f * prog), rotation, origin, Scale * (1f - prog), 0, 0);
            Main.spriteBatch.Draw(tex, position - Main.screenPosition, null, Color * (0.7f - 0.5f * prog), -rotation + 0.785f, origin, Scale * (1f - prog), 0, 0);
        }
    }
}

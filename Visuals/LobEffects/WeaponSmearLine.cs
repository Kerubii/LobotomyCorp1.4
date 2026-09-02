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
    class WeaponSmearLine : WeaponSmear
    {
        protected float Width;
        protected float WidthOffset;

        protected float Length;
        protected float LengthOffset;

        public void SetupLine(float width, float length, float lengthOffset = 0, float widthOffset = 0)
        {
            Width = width;
            WidthOffset = widthOffset;
            Length = length;
            LengthOffset = lengthOffset;

            Color = Color.White;
        }

        public virtual Vector2[] GetStrip(float length)
        {
            Vector2[] strip = [position, position + new Vector2(length, 0).RotatedBy(rotation)];
            return strip;
        }

        public virtual float[] GetRot(float length)
        {
            float[] rot = [rotation, rotation];
            return rot;
        }
        
        public override void Draw()
        {
            float prog = Time / (float)TimeMax;

            CustomShaderData shader = LobotomyCorp.LobcorpShaders["SwingTrail"].UseOpacity(GetOpacity(prog));
            shader.UseImage1(Image1)
                 .UseImage2(Image2)
                 .UseImage3(Image3)
                 .UseCustomShaderDate(TexOffX, TexOffY);

            int direction = Direction;
            int newWidth = (int)(Width + (WidthOffset * (float)Math.Sin(prog * 1.57f)));
            if (newWidth < 1)
                newWidth = 1;
            SlashTrail trail = new SlashTrail(newWidth, 0);
            trail.color = GetColor(prog);

            float len = Length + LengthOffset * (float)Math.Sin(prog * 1.57f);
            trail.DrawSpecific(GetStrip(len), GetRot(len), Vector2.Zero, shader);
        }
    }
}

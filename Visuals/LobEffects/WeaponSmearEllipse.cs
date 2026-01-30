using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using LobotomyCorp.Utils;

namespace LobotomyCorp.Visuals.LobEffects
{
    class WeaponSmearEllipse : WeaponSmear
    {
        protected float WidthA;
        protected float WidthB;
        protected float RotationStart;
        protected float RotationOffset;
        protected float RadiusA;
        protected float RadiusB;
        protected float RadiusScale;

        protected bool Semi;
        protected float Length;
        protected float LengthOffset;

        public void SetupEllipse(float widthA, float widthB, float radiusA, float radiusB, float rotationStart, float rotationOffset = 0, float radiusScale = 0)
        {
            SetupPartEllipse(widthA, widthB, radiusA, radiusB, rotationStart, 0, 0, rotationOffset, radiusScale);
            Semi = false;
        }



        public void SetupPartEllipse(float widthA, float widthB, float radiusA, float radiusB, float rotationStart, float length, float lengthOffset = 0, float rotationOffset = 0, float radiusScale = 0)
        {
            WidthA = widthA;
            WidthB = widthB;
            RadiusA = radiusA;
            RadiusB = radiusB;
            RotationStart = rotationStart;
            RotationOffset = rotationOffset;
            RadiusScale = radiusScale;

            Semi = true;
            Length = length;
            LengthOffset = lengthOffset;
            RotationOffset = rotationOffset;

            Color = Color.White;
        }

        public override void Draw()
        {
            float prog = Time / (float)TimeMax;

            CustomShaderData shader = LobotomyCorp.LobcorpShaders["SwingTrail"].UseOpacity(GetOpacity(prog));
            shader.UseImage1(Mod, Image1)
                  .UseImage2(Mod, Image2)
                  .UseImage3(Mod, Image3)
                  .UseCustomShaderDate(TexOffX, TexOffY);

            int direction = Direction;
            SlashTrail trail = new SlashTrail(WidthA, WidthB, 0);
            trail.color = GetColor(prog);

            float radiussc = 1f + RadiusScale * prog;

            if (Semi)
            {
                float rotationOffset = RotationOffset * Easing(prog) * direction;  
                float endAngle = Length + LengthOffset * Easing(prog);
                trail.DrawPartEllipse(position, rotation, RotationStart + rotationOffset, endAngle, direction, RadiusA * radiussc, RadiusB * radiussc, 64, shader);
            }
            else
            {
                float rotationOffset = RotationOffset * Easing(prog) * direction;
                trail.DrawEllipse(position, rotation, RotationStart + rotationOffset, direction, RadiusA * radiussc, RadiusB * radiussc, 64, shader);
            }
        }
    }
}

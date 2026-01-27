using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using LobotomyCorp.Utils;

namespace LobotomyCorp.Visuals.PrimEffects
{
    class WeaponSmearCircle : WeaponSmear
    {
        protected float Width;
        protected float RotationOffset;
        protected float Radius;
        protected float RadiusScale;

        protected bool Semi;
        protected float Length;
        protected float LengthOffset;

        /// <summary>
        /// Draws a full circle, this or SetupSemiCircle must be used
        /// </summary>
        /// <param name="width">Thickness of the shader</param>
        /// <param name="radius">How far the middle of the circle is from the position</param>
        /// <param name="rotationOffset">Adds or Subtracts to the original rotation based on Direction, Defaults to 0 which does not move</param>
        public void SetupCircle(float width, float radius, float rotationOffset = 0, float radiusScale = 0)
        {
            Width = width;
            RotationOffset = rotationOffset;
            Radius = radius;
            RadiusScale = radiusScale;

            Semi = false;
            Length = 0;

            Color = Color.White;
        }

        /// <summary>
        /// Draws a semi circle, this or SetupCircle must be used
        /// </summary>
        /// <param name="width">Thickness of the shader</param>
        /// <param name="radius">How far the middle of the circle is from the position</param>
        /// <param name="length">Length of the Semi circle</param>
        /// <param name="lengthOffset">Adds to the Length based on time</param>
        /// <param name="rotationOffset">Adds or Subtracts to the original rotation based on Direction, Defaults to 0 which does not move</param>
        public void SetupSemiCircle(float width, float radius, float length, float lengthOffset = 0, float rotationOffset = 0, float radiusScale = 0)
        {
            Width = width;
            RotationOffset = rotationOffset;
            Radius = radius;
            RadiusScale = radiusScale;

            Semi = true;
            Length = length;
            LengthOffset = lengthOffset;

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
            SlashTrail trail = new SlashTrail(Width, 1.57f);
            trail.color = GetColor(prog);

            float radiussc = 1f + RadiusScale * prog;

            if (Semi)
            {
                float rotationOffset = RotationOffset * (float)Math.Sin(prog * 1.57f) * direction;  
                float endAngle = Length + LengthOffset * (float)Math.Sin(prog * 1.57f);
                trail.DrawPartCircle(position, rotation + rotationOffset, endAngle, Direction, Radius * radiussc, 64, shader);
            }
            else
            {
                float rotationOffset = RotationOffset * (float)Math.Sin(prog * 1.57f) * direction;
                trail.DrawCircle(position, rotation + rotationOffset, direction, Radius * radiussc, 64, shader);
            }
        }
    }
}

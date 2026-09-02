using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Steamworks;
using System;
using Terraria;

namespace LobotomyCorp.Visuals.LobEffects
{
    class WeaponSmearSineWave : WeaponSmear
    {
        protected int Width;

        protected int Radius;
        protected int RadiusOffset;
        protected int Length;
        protected int LengthOffset;

        Trailhelper Trailhelper;

        protected float Steps;
        protected int CurrentStep;
        protected int ExtraUpdates;

        protected bool Absolute;
        protected float VelOffset;

        /// <summary>
        /// Generates position based on a Sine Wave
        /// </summary>
        /// <param name="limit">How many positions is recorded for the trail</param>
        /// <param name="width">The shader trail's Width</param>
        /// <param name="radius">How far the trail is from the point</param>
        /// <param name="length">How far ahead or behind the trail is being recorded</param>
        /// <param name="steps">How fast the sine wave goes</param>
        /// <param name="radiusOffset">Adds to the Radius based on time</param>
        /// <param name="lengthOffset">Adds to the Length based on time</param>
        /// <param name="absolute">If the trail is recorded in the world (true) or relative to the object (false)</param>
        /// <param name="extraupdates">Additional updates for smooth</param>
        public void SetupWave(int limit, int width, int radius, int length, float steps, int radiusOffset = 0, int lengthOffset = 0, bool absolute = false, int extraupdates = 0)
        {
            Width = width;
            Radius = radius;
            Length = length;
            RadiusOffset = radiusOffset;
            LengthOffset = lengthOffset;


            Trailhelper = new Trailhelper(limit);
            Trailhelper.TrailPos[0] = Vector2.Zero;
            Trailhelper.TrailRotation[0] = rotation;

            Absolute = absolute;
            VelOffset = 0;

            Steps = steps;
            CurrentStep = 0;
            ExtraUpdates = extraupdates + 1;

            RandomizeStep(360);
        }

        /// <summary>
        /// Generates position based on a Sine Wave, Variant where the trail automatically widens
        /// </summary>
        /// <param name="limit">How many positions is recorded for the trail</param>
        /// <param name="width">The shader trail's Width</param>
        /// <param name="radius">How far the trail is from the point</param>
        /// <param name="length">How far ahead or behind the trail is being recorded</param>
        /// <param name="steps">How fast the sine wave goes</param>
        /// <param name="vel">How fast the trail moves away from the center</param>
        /// <param name="radiusOffset">Adds to the Radius based on time</param>
        /// <param name="lengthOffset">Adds to the Length based on time</param>
        /// <param name="extraupdates">Additional updates for smooth</param>
        public void SetupWave(int limit, int width, int radius, int length, float steps, float vel, int radiusOffset = 0, int lengthOffset = 0, int extraupdates = 0)
        {
            Width = width;
            Radius = radius;
            Length = length;
            RadiusOffset = radiusOffset;
            LengthOffset = lengthOffset;


            Trailhelper = new Trailhelper(limit);
            Trailhelper.TrailPos[0] = Vector2.Zero;
            Trailhelper.TrailRotation[0] = rotation;

            Absolute = true;
            VelOffset = vel;

            Steps = steps;
            CurrentStep = 0;
            ExtraUpdates = extraupdates + 1;

            RandomizeStep(360);
        }

        public override void UpdateBehavior()
        {
            base.UpdateBehavior();
            for (int i = 0; i < ExtraUpdates; i++)
            {
                float timer = (Time + (float)(CurrentStep % ExtraUpdates) / ExtraUpdates) / TimeMax;

                Vector2 newPos = new Vector2(Length + LengthOffset * timer, (Radius + RadiusOffset * timer) * (float)Math.Sin(Steps * CurrentStep)).RotatedBy(rotation);
                if (Absolute)
                    newPos += position;

                if (VelOffset > 0)
                {
                    Vector2 vel = new Vector2(0, VelOffset * (float)Math.Sin(Steps * CurrentStep)).RotatedBy(rotation);
                    Trailhelper.TrailUpdate(newPos, rotation, vel);
                }
                else Trailhelper.TrailUpdate(newPos, rotation);
                CurrentStep++;
            }
        }

        /// <summary>
        /// Randomizes current step (what part of the Sine Wave its doing
        /// </summary>
        /// <param name="rand"></param>
        public void RandomizeStep(int rand)
        {
            CurrentStep = Main.rand.Next(rand);
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
            SlashTrail trail = new SlashTrail(Width, 1.57f);
            trail.color = GetColor(prog);

            int x = Trailhelper.TrailPos.Length;
            Vector2[] passLinePos = new Vector2[x];
            for (int i = 0; i < x; i++)
            {
                if (Trailhelper.TrailPos[i] == Vector2.Zero)
                    passLinePos[i] = Vector2.Zero;
                passLinePos[i] = position + Trailhelper.TrailPos[i];
                if (Absolute)
                    passLinePos[i] = Trailhelper.TrailPos[i];
            }

            trail.DrawSpecific(passLinePos, Trailhelper.TrailRotation, Vector2.Zero, shader);
        }
    }
}

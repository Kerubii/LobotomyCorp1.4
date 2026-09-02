using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using LobotomyCorp.Util;
using static LobotomyCorp.Misc.MiscAssets;

namespace LobotomyCorp.Visuals.LobEffects
{
    abstract class WeaponSmear : LobDrawEffects
    {
        // Set
        protected Entity Owner;
        protected bool Accelerate;
        protected Vector2 origVelocity;

        protected Asset<Texture2D> Image1;
        protected Asset<Texture2D> Image2;
        protected Asset<Texture2D> Image3;
        protected float TexOffX;
        protected float TexOffY;
        protected float TexScrollX;
        protected float TexScrollY;

        protected int SmearType;
        protected float Easing(float prog)
        {
            switch (SmearType)
            {
                case 0:
                    return 1f - (float)Math.Pow(1f - prog, 4);
                default:
                    return (float)Math.Sin(prog * 1.57f);
            }
        }

        public void Setup(Entity owner, Vector2 vel, float rot, int maxTime, int direction, bool accelerate = false)
        {
            Owner = owner;
            Setup(owner.Center, vel, rot, maxTime, direction, accelerate);
        }

        public void Setup(Vector2 pos, Vector2 vel, float rot, int maxTime, int direction, bool accelerate = false)
        {
            position = pos;
            velocity = vel;
            rotation = rot;
            Time = 0;
            TimeMax = maxTime;
            Direction = direction;

            Accelerate = false;
            origVelocity = vel;
            active = true;

            Image1 = FlatColor;
            Image2 = TexTrail1;
            Image3 = Worley;
            TexOffX = 0;
            TexOffY = 0;
            TexScrollX = 0;
            TexScrollY = 0;

            SmearType = 0;
        }

        /// <summary>
        /// "Direct folder from LobotomyCorp/ ie 'Misc/FlatColor'"
        /// </summary>
        /// <param name="image1"></param>
        /// <param name="image2"></param>
        /// <param name="image3"></param>
        public void SetShaderImage(string image1 = null, string image2 = null, string image3 = null)
        {
            if (image1 != null)
                Image1 = Mod.Assets.Request<Texture2D>(image1);
            if (image2 != null)
                Image2 = Mod.Assets.Request<Texture2D>(image2);
            if (image3 != null)
                Image3 = Mod.Assets.Request<Texture2D>(image3);
        }

        public void SetShaderImage(Asset<Texture2D> image1 = null, Asset<Texture2D> image2 = null, Asset<Texture2D> image3 = null)
        {
            if (image1 != null)
                Image1 = image1;
            if (image2 != null)
                Image2 = image2;
            if (image3 != null)
                Image3 = image3;
        }

        public void SetShaderTexOffset(float x, float y)
        {
            TexOffX = x;
            TexOffY = y;
        }

        /// <summary>
        /// Changes how the weapon 
        /// </summary>
        public override void UpdateBehavior()
        {
            if (Owner != null)
            {
                position = Owner.Center;
                if (!Owner.active)
                    Deactivate();
            }
            position += velocity;
            if (Accelerate)
                velocity += origVelocity;
            TexOffX += TexScrollX;
            TexOffY += TexScrollY;
        }

        public void Detach(Vector2 newVel = default(Vector2))
        {
            if (Owner == null)
                return;
            position = Owner.Center;
            Owner = null;
            velocity = newVel;
        }

        public virtual float GetOpacity(float time)
        {
            float opacity = 1f;
            if (time > 0.5f)
                opacity *= 1f - (time - 0.5f) / 0.5f;
            return opacity;
        }

        public virtual Color GetColor(float time)
        {
            if (time > 0.6f)
            {
                time = 1f - (time - 0.6f) / 0.4f;
                return Color * time;
            }
            return Color;
        }
    }
}

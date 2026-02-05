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
    class LobDrawEffects
    {
        public bool active = false;
        public void Deactivate() { active = false; }
        public LobotomyCorp Mod => LobotomyCorp.Instance;

        // Set
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;

        public Color Color;
        public int Direction;
        public int Time;
        public int TimeMax;

        public LobDrawEffects()
        {
            active = false;
        }

        public void Update()
        {
            Time++;

            UpdateBehavior();

            if (Time >= TimeMax)
                Deactivate();
        }

        /// <summary>
        /// Modifies 
        /// </summary>
        public virtual void UpdateBehavior()
        {
            position += velocity;
        }

        public virtual void Draw()
        {
        }
    }
}

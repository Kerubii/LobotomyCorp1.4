using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace LobotomyCorp.Util
{
	public class Trailhelper
    {
        public Vector2[] TrailPos;
        public float[] TrailRotation;

        public Trailhelper(int length)
        {
            TrailPos = new Vector2[length];
            TrailRotation = new float[length];
        }

        public void TrailUpdate(Vector2 position, float rotation)
        {
            for (int i = TrailPos.Length - 1; i >= 0; i--)
            {
                if (i > 0)
                {
                    if (TrailPos[i - 1] != null)
                        TrailPos[i] = TrailPos[i - 1];

                    TrailRotation[i] = TrailRotation[i - 1];
                }
                else
                {
                    TrailPos[i] = position;
                    TrailRotation[i] = rotation;
                }
            }
        }

        public void ResetTrail()    
        {
            int length = TrailPos.Length;
            TrailPos = new Vector2[length];
            TrailRotation = new float[length];
        }
    }
}

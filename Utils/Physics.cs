using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;
using Steamworks;

namespace LobotomyCorp.Util
{
    class Spring
    {
		private Vector2[] PointPos;
		private Vector2[] PointVelocity;
		private float Length;

		public Spring(Vector2 startPos, int amount, float restLength)
        {
			PointPos = new Vector2[amount];
			PointVelocity = new Vector2[amount];
			for (int i = 0; i < amount; i++)
			{
				PointPos[i] = startPos;
			}

			Length = restLength;
        }

		public Vector2 GetPosition(int i)
		{
			if (i > PointPos.Length)
				i = PointPos.Length;

			return PointPos[i];
		}

		public void UpdateVelocity(Vector2 anchorPoint, float k, float resistance = 1f)
        {
			for (int i = 0; i < PointPos.Length; i++)
            {
				//float factor = 1f - i / (PointPos.Length + 1);

				PointVelocity[i] += SpringForce(anchorPoint, PointPos[i], k, Length);
				PointVelocity[i] *= resistance;

				PointPos[i] += PointVelocity[i];
				if (i - 1 >= 0)
					PointPos[i - 1] -= PointVelocity[i];
				anchorPoint = PointPos[i];

            }
        }

		public void ApplyVelocity(Vector2 velocity)
        {
			for (int i = 0; i < PointPos.Length; i++)
			{
				PointVelocity[i] += velocity;
            }
        }

		public void DustTest()
		{
			Dust dust;
			foreach (Vector2 pos in PointPos)
			{
				dust = Dust.NewDustPerfect(pos, 66);
				dust.velocity *= 0;
				dust.noGravity = true;
			}
		}
		/// <summary>
		/// Multiply by <1 to give air resistance
		/// </summary>
		/// <param name="anchorPoint"></param>
		/// <param name="endPoint"></param>
		/// <param name="k"></param>
		/// <param name="springRest"></param>
		/// <returns></returns>
        public static Vector2 SpringForce(Vector2 anchorPoint, Vector2 endPoint, float k, float springRest)
        {
			Vector2 spring = endPoint - anchorPoint;
			float length = spring.Length();

			spring.Normalize();
			length -= springRest;
			spring *= (-k * length);
            return spring;
        }
    }
}

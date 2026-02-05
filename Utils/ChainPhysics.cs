using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace LobotomyCorp.Util
{
	class ChainPhysics
    {
        Vector3[] ChainPos;
        Vector2[] ChainVelocity;
        float ChainLength;

        public Vector3 GetChainEnd()
        {
            return ChainPos[ChainPos.Length - 1];
        }

        public int ChainAmount()
        {
            return ChainPos.Length;
        }

        public ChainPhysics(int chainNumber, int chainLength)
        {
            ChainPos = new Vector3[chainNumber];
            ChainVelocity = new Vector2[chainNumber];
            ChainLength = chainLength;
        }

        public void ApplyVelocity(Vector2 AnchorPos, int Chain, Vector2 velocity, float maxVelocity)
        {
            ChainVelocity[Chain] += velocity;

            if (ChainVelocity[Chain].Length() > maxVelocity)
            {
                ChainVelocity[Chain].Normalize();
                ChainVelocity[Chain] *= maxVelocity;
            }

            ApplyPhysic(AnchorPos, Chain, ChainVelocity[Chain]);
        }

        public void ApplyVelocity(Vector2 AnchorPos, Vector2 velocity, float maxVelocity)
        {
            for (int i = 0; i < ChainPos.Length; i++)
            {
                ChainVelocity[i] += velocity;

                if (ChainVelocity[i].Length() > maxVelocity)
                {
                    ChainVelocity[i].Normalize();
                    ChainVelocity[i] *= maxVelocity;
                }

                ApplyPhysic(AnchorPos, i, ChainVelocity[i]);
            }
        }

        public void ApplyPhysic(Vector2 AnchorPos, int Chain, Vector2? force)
        {
            Vector2 anchorPoint = AnchorPos;
            if (Chain > 0)
                anchorPoint = new Vector2(ChainPos[Chain - 1].X, ChainPos[Chain - 1].Y);

            Vector2 CurrentPos = new Vector2(ChainPos[Chain].X, ChainPos[Chain].Y);

            if (force != null)
                CurrentPos += (Vector2)force;

            float angle = (float)Math.Atan2(anchorPoint.Y - CurrentPos.Y, anchorPoint.X - CurrentPos.X);

            Vector2 vectorAngle = new Vector2(1, 0).RotatedBy(angle);
            CurrentPos = anchorPoint - vectorAngle * ChainLength * 1f;

            ChainPos[Chain].X = CurrentPos.X;
            ChainPos[Chain].Y = CurrentPos.Y;
            ChainPos[Chain].Z = angle;
        }

        public void ApplyPhysics(Vector2 AnchorPos, Vector2? force)
        {
            Vector2 anchorPoint = AnchorPos;
            for (int i = 0; i < ChainPos.Length; i++)
            {
                Vector2 CurrentPos = new Vector2(ChainPos[i].X,ChainPos[i].Y);

                if (force != null)
                    CurrentPos += (Vector2)force;

                float angle = (float)Math.Atan2(anchorPoint.Y - CurrentPos.Y, anchorPoint.X - CurrentPos.X);

                Vector2 vectorAngle = new Vector2(1,0).RotatedBy(angle);
                CurrentPos = anchorPoint - vectorAngle * ChainLength * 1f;

                ChainPos[i].X = CurrentPos.X;
                ChainPos[i].Y = CurrentPos.Y;
                ChainPos[i].Z = angle;

                anchorPoint = CurrentPos;
            }
        }

        public DrawData[] DrawChains(Texture2D chainTexture, Rectangle? chainFrame, Vector2 chainOrigin, float chainRotationOffset)
        {
            DrawData[] Chains = new DrawData[ChainPos.Length];
            for (int i = 0; i < ChainPos.Length; i++)
            {
                Chains[i] = (DrawData)DrawChain(i, chainTexture, chainFrame, chainOrigin, chainRotationOffset);
            }
            return Chains;
        }

        /// <summary>
        /// Draws One Chain
        /// </summary>
        /// <param name="chainNum"></param>
        /// <param name="chainTexture"></param>
        /// <param name="chainFrame"></param>
        /// <param name="chainOrigin"></param>
        /// <param name="chainRotationOffset"></param>
        /// <returns></returns>
        public DrawData? DrawChain(int chainNum, Texture2D chainTexture, Rectangle? chainFrame,Vector2 chainOrigin, float chainRotationOffset)
        {
            if (chainNum >= ChainPos.Length)
                return null;

            Vector2 Position = new Vector2(ChainPos[chainNum].X, ChainPos[chainNum].Y);
            Color color = Lighting.GetColor((int)Position.X / 16, (int)Position.Y / 16);
            Position -= Main.screenPosition;
            Rectangle frame = (Rectangle)chainFrame;
            if (chainFrame == null)
                frame = chainTexture.Frame();

            return new DrawData(chainTexture, Position, frame, color, ChainPos[chainNum].Z, chainOrigin, 1, 0, 0);
        }
    }
}

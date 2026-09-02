using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace LobotomyCorp.Util
{
	class Bezier
    {
        private Vector2 startPoint;
        private Vector2 endPoint;
        private Vector2 point1;
        private Vector2 point2;

        public Bezier(Vector2 StartPoint, Vector2 EndPoint)
        {
            point1 = startPoint = StartPoint;
            point2 = endPoint = EndPoint;
        }

        public Vector2 Start
        {
            set { startPoint = value; }
            get { return startPoint; }
        }

        public Vector2 End
        {
            set { endPoint = value; }
            get { return endPoint; }
        }

        public Vector2 Control1
        {
            set { point1 = value; }
            get { return point1; }
        }

        public Vector2 Control2
        {
            set { point2 = value; }
            get { return point2; }
        }

        public void SetStartEnd(Vector2 start, Vector2 end)
        {
            startPoint = start;
            endPoint = end;
        }

        /// <summary>
        /// Move ControlPoint1, Near StartPoint
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="velocity"></param>
        public void CPoint1Move(Vector2 targetPosition, float velocity = -1)
        {
            PointMove(targetPosition, ref point1, velocity);
        }

        /// <summary>
        /// Move ControlPoint2, Near EndPoint
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="velocity"></param>
        public void CPoint2Move(Vector2 targetPosition, float velocity = -1)
        {
            PointMove(targetPosition, ref point2, velocity);
        }

        private void PointMove(Vector2 targetPosition, ref Vector2 point, float velocity)
        {
            if (float.IsNaN(targetPosition.X) || float.IsNaN(targetPosition.Y))
                return;

            Vector2 delta = targetPosition - point;
            if (delta.Length() <= 0)
            {
                point = targetPosition;
                return;
            }

            Vector2 movement = Vector2.Normalize(delta) * velocity;
            if (delta.Length() < velocity || velocity < 0)
                movement = delta;

            point += movement;
        }

        public Vector2 BezierPoint(float progress)
        {
            return CalculateBezierPoint(progress, startPoint, point1, point2, endPoint);
        }

        /// <summary>
        /// Calculates Bezier, p0 is startpoint, p3 is endpoint, p1 and p2 are control points to startpoint and endpoint respectively
        /// </summary>
        /// <param name="t"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 p = uuu * p0; //first term
            p += 3 * uu * t * p1; //second term
            p += 3 * u * tt * p2; //third term
            p += ttt * p3; //fourth term

            return p;
        }

        public float DerivativeRotation(float progress)
        {
            return CalculateDerivativeRotation(progress, startPoint, point1, point2, endPoint);
        }

        /// <summary>
        /// What in the fuck is this I have no Idea its just derivative I don't know how they work it scares me
        /// </summary>
        /// <param name="t"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static float CalculateDerivativeRotation(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float tt = t * t;
            float u = 1f - t;
            float uu = u * u;

            Vector2 p = uu * (p1 - p0);//first term
            p += 2 * u * t *(p2 - p1);//Second term
            p += tt * (p3 - p2);//Third term
            p *= 3;

            return (float)Math.Atan2(p.Y, p.X);
        }

        /// <summary>
        /// Get BezierLength from Divisions
        /// </summary>
        /// <param name="divisions"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public float SegmentBezierLength(float divisions)
        {
            float Length = 0;
            Vector2 p1 = startPoint;
            Vector2 p2;
            for (int i = 0; i <= divisions; i++)
            {
                float t = i / divisions;
                p2 = CalculateBezierPoint(t, startPoint, point1, point2, endPoint);
                Length += (p2 - p1).Length();
                p1 = p2;
            }
            return Length;
        }

        /// <summary>
        /// Get BezierLength from Divisions
        /// </summary>
        /// <param name="divisions"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static float SegmentBezierLength(float divisions, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float Length = 0;
            Vector2 point1 = p0;
            Vector2 point2;
            for (int i = 0; i <= divisions; i++)
            {
                float t = i / divisions;
                point2 = CalculateBezierPoint(t, p0, p1, p2, p3);
                Length += (point2 - point1).Length();
                point1 = point2;
            }
            return Length;
        }

        /// <summary>
        /// Gets next point, 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public Vector2? NextApproximatePoint(float length, ref float time, float steps)
        {
            float distance = 0;
            Vector2 p1 = BezierPoint(time);
            Vector2 p2 = p1;
            while (distance < length)
            {
                time += steps;
                p2 = BezierPoint(time);
                distance = (p2 - p1).Length();

                if (time >= 1)
                {
                    time = 1;
                    return null;
                }
            }
            return p2;
        }

        public float DistanceBetweenPoint(float progressPoint1, float progressPoint2)
        {
            Vector2 p1 = BezierPoint(progressPoint1);
            Vector2 p2 = BezierPoint(progressPoint2);

            return p1.Distance(p2);
        }

        public DrawData DrawCurveRotatedtoNext(Texture2D tex, Rectangle? frame, Color color, float scale, float rotationOffset, Vector2 frameOrigin, SpriteEffects spEffect, float progressPoint1, float progressPoint2, Vector2 posOffset = default(Vector2))
        {
            Vector2 p1 = BezierPoint(progressPoint1);
            Vector2 p2 = BezierPoint(progressPoint2);
            float rotation = (p2 - p1).ToRotation();

            return new DrawData(tex, p1 - Main.screenPosition + posOffset, frame, color, rotation + rotationOffset, frameOrigin, scale, spEffect, 0);
        }

        public DrawData DrawCurveRotatedtoNext(Texture2D tex, Rectangle? frame, Color color, Vector2 scale, float rotationOffset, Vector2 frameOrigin, SpriteEffects spEffect, float progressPoint1, float progressPoint2, Vector2 posOffset = default(Vector2))
        {
            Vector2 p1 = BezierPoint(progressPoint1);
            Vector2 p2 = BezierPoint(progressPoint2);
            float rotation = (p2 - p1).ToRotation();

            return new DrawData(tex, p1 - Main.screenPosition + posOffset, frame, color, rotation + rotationOffset, frameOrigin, scale, spEffect, 0);
        }

        public DrawData DrawCurve(Texture2D tex, Rectangle? frame, Color color, float scale, float rotation, Vector2 frameOrigin, SpriteEffects spEffect, float curveProgress)
        {
            Vector2 position = BezierPoint(curveProgress) - Main.screenPosition;
            return new DrawData(tex, position, frame, color, rotation, frameOrigin, scale, spEffect, 0);
        }

        
        public Vector2[] GetCurveVectors(int divisions)
        {
            Vector2[] array = new Vector2[divisions + 1];
            for (int i = 0; i <= divisions; i++)
            {
                float t = (float)i / divisions;
                array[i] = BezierPoint(t);
            }
            return array;
        }

        public float[] GetAngleVectors(int divisions)
        {
            float[] array = new float[divisions + 1];
            for (int i = 0; i <= divisions; i++)
            {
                float t = (float)i / divisions;
                array[i] = DerivativeRotation(t);
            }
            return array;
        }

        public bool Collision(Rectangle target, float steps, Vector2 size, float limit = 1f)
        {
            if (steps < 0)
                return false;
            float currentStep = steps;
            while (currentStep <= limit)
            {
                Vector2 point = BezierPoint(currentStep);
                Rectangle rect = new Rectangle((int)(point.X - size.X / 2f), (int)(point.Y - size.Y / 2f), (int)size.X, (int)size.Y);
                if (rect.Intersects(target)) return true;
                currentStep += steps;
            }
            return false;
        }

        public void DustTest()
        {
            Dust dust;
            dust = Dust.NewDustPerfect(startPoint, 66);
            dust.velocity *= 0;
            dust.noGravity = true;
            dust = Dust.NewDustPerfect(endPoint, 66);
            dust.velocity *= 0;
            dust.noGravity = true;
            dust = Dust.NewDustPerfect(point1, 66);
            dust.velocity *= 0;
            dust.noGravity = true;
            dust = Dust.NewDustPerfect(point2, 66);
            dust.velocity *= 0;
            dust.noGravity = true;
        }
    }
}

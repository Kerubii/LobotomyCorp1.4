using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Chat;
using LobotomyCorp;
using LobotomyCorp.Utils;
using LobotomyCorp.UI;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace LobotomyCorp.Utils
{
    public class SkeletonBase
    {
        //For General Use
        //Unsure How good this is? compared to something that exists out there, probly more fun to make up my own though
        public Dictionary<Enum, BonePart> BoneName;

        public SkeletonBase(Dictionary<Enum, BonePart> BoneList)
        {
            BoneName = BoneList;
        }

        public virtual void Record()
        {
            foreach (BonePart bp in BoneName.Values)
            {
                bp.Record();
            }
        }

        public virtual void Distance()
        {

        }

        public virtual List<BonePart> GetBoneList(bool forDraw = true)
        {
            List<BonePart> list = new List<BonePart>();
            if (forDraw)
            {
                foreach (Enum key in BoneName.Keys)
                {
                    if ((forDraw && BoneName[key].Visible) || !forDraw)
                        list.Add(BoneName[key]);
                }
                /*for (int index = 0; index < BoneName.Count; index++)
                {
                    if ((forDraw && BoneName[index].Visible) || !forDraw)
                        list.Add(BoneName[index]);
                }*/
            }
            return list;
        }

        /// <summary>
        /// dir is bend direction, 1 = CW : 2 = CCW
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="length1"></param>
        /// <param name="length2"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Vector2 ElbowIK(Vector2 startPoint, Vector2 endPoint, float length1, float length2, int dir = 1)
        {
            Vector2 elbow = startPoint;
            float Dist = Vector2.Distance(endPoint, startPoint);
            if (Dist > length1 + length2)
                Dist = length1 + length2;
            float Angle = (float)Math.Acos(Dist * Dist / ((length1 + length2) * Dist));
            float Rotation = (endPoint - elbow).ToRotation() + Angle * dir;
            elbow += new Vector2(length1, 0).RotatedBy(Rotation);
            return elbow;
        }

        /// <summary>
        /// /// dir is bend direction, 1 = CW : 2 = CCW
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="length1"></param>
        /// <param name="length2"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static float[] RotationIK(Vector2 startPoint, Vector2 endPoint, float length1, float length2, int dir = 1)
        {
            Vector2 elbow = ElbowIK(startPoint, endPoint, length1, length2, dir);
            float[] rotations = new float[2];

            rotations[0] = (elbow - startPoint).ToRotation();
            rotations[1] = (endPoint - elbow).ToRotation();

            return rotations;
        }

        public void RotationIK(Enum Bone1, Enum Bone2, Enum BoneIK,int dir = 1)
        {
            float[] rotations = RotationIK(BoneName[Bone1].GetPosition(), BoneName[BoneIK].EndPoint(), BoneName[Bone1].Length, BoneName[Bone2].Length, dir);
            BoneName[Bone1].ChangeRotation(rotations[0]);
            BoneName[Bone2].ChangeRotation(rotations[1]);
        }

        public float DistanceBone(Enum Bone1, Enum Bone2)
        {
            return BoneName[Bone1].GetPosition().Distance(BoneName[Bone2].EndPoint());
        }

        public float TotalLength(Enum[] BoneList)
        {
            float length = 0;
            for (int i = 0; i < BoneList.Length; i++)
            {
                length += BoneName[BoneList[i]].Length;
            }
            return length;
        }
    }

    public class BonePart
    {
        Vector2[] offset;
        bool InheritOffset;
        float[] Rotation;
        bool InheritRotation;
        float[] Scale;
        bool InheritScale;

        public float Length;

        BonePart Parent;

        private Texture2D Texture;
        private Rectangle Frame;
        private Vector2 Origin;
        private float RotationOffset;
        public bool Visible;

        public BonePart(Vector2 initOffset, float initRot, float initScale, float length, BonePart BoneParent = null, int oldRecord = 1)
        {
            offset = new Vector2[oldRecord];
            Rotation = new float[oldRecord];
            Scale = new float[oldRecord];

            offset[0] = initOffset;
            Rotation[0] = initRot;
            Scale[0] = initScale;
            Length = length;

            if (BoneParent != null)
            {
                Parent = BoneParent;
                InheritOffset = true;
                InheritRotation = true;
                InheritScale = true;
            }
            else
            {
                InheritOffset = false;
                InheritRotation = false;
                InheritScale = false;
            }

            Visible = false;
        }

        public void Record()
        {
            for (int i = offset.Length - 1; i > 0; i--)
            {
                if (i > 0)
                {
                    if (offset[i - 1] != null)
                        offset[i] = offset[i - 1];

                    Rotation[i] = Rotation[i - 1];
                    Scale[i] = Scale[i - 1];
                }
            }
        }

        public BonePart ChangeIOffset(bool To)
        {
            if (To != InheritOffset)
            {
                Vector2 newPos;
                if (!To)
                    newPos = GetPosition();
                else
                    newPos = Parent.EndPoint() - GetPosition();
                offset[0] = newPos;
                InheritOffset = To;
            }
            return this;
        }

        public BonePart ChangeIRotation(bool To)
        {
            InheritRotation = To;
            return this;
        }

        public BonePart ChangeIScale(bool To)
        {
            if (To != InheritScale)
            {
                float newScale;
                if (!To)
                    newScale = GetScale();
                else
                    newScale = GetScale() / Parent.GetScale();
                Scale[0] = newScale;
                InheritScale = To;
            }
            return this;
        }
        
        /// <summary>
        /// Set speed to 0 if not changed, Set speed to -1 to instant change
        /// </summary>
        /// <param name="newPos"></param>
        /// <param name="speed"></param>
        /// <param name="rot"></param>
        /// <param name="rotSpeed"></param>
        public void ChangeBone(Vector2 newPos, float speed, float rot, float rotSpeed)
        {
            if (speed > 0)
            {
                Vector2 bonePos = offset[0];
                Vector2 delta = newPos - bonePos;

                if (delta.Length() > speed)
                {
                    delta.Normalize();
                    delta *= speed;
                }
                offset[0] += delta;
            }
            else if (speed < 0)
            {
                offset[0] = newPos;
            }

            if (rotSpeed > 0)
                Rotation[0] = Terraria.Utils.AngleLerp(Rotation[0], rot, rotSpeed);
            else if (rotSpeed < 0)
                Rotation[0] = rot;
        }

        public void ChangeOffset(Vector2 newPos, float speed = -1)
        {
            ChangeBone(newPos, speed, 0, 0);
        }

        public void ChangeOffsetLerp(Vector2 newPos, float lerp)
        {
            ChangeBone(Vector2.Lerp(offset[0], newPos, lerp), -1, 0, 0);
        }

        public void ChangeRotation(float Rotation, float speed = -1)
        {
            ChangeBone(Vector2.Zero, 0, Rotation, speed);
        }

        public Vector2 GetPosition(int dir = 1, int i = 0)
        {
            Vector2 positionOffset = new Vector2(offset[i].X, offset[i].Y * dir) * GetScale(i);
            if (InheritOffset)
                return positionOffset.RotatedBy(Parent.GetRotation(dir, i)) + Parent.EndPoint(dir, i);
            return positionOffset;
        }

        public Vector2 EndPoint(int dir = 1, int i = 0)
        {
            return GetPosition(dir, i) + new Vector2(Length * GetScale(i), 0).RotatedBy(GetRotation(dir, i));
        }

        public float GetRotation(int dir = 1, int i = 0)
        {
            Vector2 vec1 = new Vector2(1, 0).RotatedBy(Rotation[i]);
            vec1.X *= dir;
            if (InheritRotation)
                return vec1.ToRotation() + Parent.GetRotation();
            return vec1.ToRotation();
        }

        public float GetScale(int i = 0)
        {
            if (InheritScale)
                return Scale[i] * Parent.GetScale(i);
            return Scale[i];
        }

        public BonePart SetDraw(Texture2D boneTexture, Rectangle texFrame, Vector2 texOrigin, float Rotation = 0)
        {
            Texture = boneTexture;
            Frame = texFrame;
            Origin = texOrigin;
            RotationOffset = Rotation;
            Visible = true;
            return this;
        }

        public DrawData DrawBone(int dir = 1, int Trail = 0)
        {
            return new DrawData(Texture,
                    GetPosition(dir, Trail) - Main.screenPosition,
                    Frame,
                    Color.White,
                    GetRotation(dir, Trail) + RotationOffset,
                    Origin,
                    Scale[Trail],
                    dir < 0 ? SpriteEffects.FlipHorizontally : 0f,
                    0);
        }
    }
}

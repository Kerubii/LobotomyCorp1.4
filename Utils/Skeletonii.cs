using LobotomyCorp;
using LobotomyCorp.NPCs.RedMist;
using LobotomyCorp.UI;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Utils
{
    public class SkeletonBase
    {
        //For General Use
        //Unsure How good this is? compared to something that exists out there, probly more fun to make up my own though
        public Dictionary<Enum, BonePart> BoneName = new Dictionary<Enum, BonePart>();
        public Mod Mod => ModContent.GetInstance<LobotomyCorp>();
        public SkeletonBase()
        {
            
        }

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
            float[] rotations = RotationIK(BoneName[Bone1].GetPosition(), BoneName[BoneIK].GetPosition(), BoneName[Bone1].Length, BoneName[Bone2].Length, dir);
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
        bool ParentOrigin;
        bool InheritOffset;
        bool InheritOffsetRotation;
        float[] Rotation;
        bool InheritRotation;
        float[] Scale;
        bool InheritScale;

        public float Length;

        BonePart Parent;

        private Texture2D Texture;
        public Rectangle Frame;
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
                ParentOrigin = false;
                InheritOffset = true;
                InheritOffsetRotation = true;
                InheritRotation = true;
                InheritScale = true;
            }
            else
            {
                ParentOrigin = false;
                InheritOffset = false;
                InheritOffsetRotation = true;
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

        /// <summary>
        /// Changes if the bone currently inherits the parent's offset, can be used to detach bone and adjust offset accordingly automatically
        /// </summary>
        /// <param name="To"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Changes if the bone's position is also rotated by the parent, Used for IK since the IK's position can be changed by the parent otherwise
        /// </summary>
        /// <param name="To"></param>
        /// <returns></returns>
        public BonePart ChangeIOffestRotation(bool To)
        {
            InheritOffsetRotation = To;
            return this;
        }

        /// <summary>
        /// Whether this bone is located in relation to it's parents startpoint(true) or endpoint(false by default)
        /// </summary>
        /// <param name="To"></param>
        /// <returns></returns>
        public BonePart ChangeParentOrigin(bool To)
        {
            ParentOrigin = To;
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

        public void ChangeScale(float scale, float speed = -1f)
        {
            if (speed < 0)
                Scale[0] = scale;
            else
            {
                if (Scale[0] < scale)
                {
                    Scale[0] += speed;
                    if (Scale[0] > scale)
                        Scale[0] = scale;
                }
                else if (Scale[0] > scale)
                {
                    Scale[0] -= speed;
                    if (Scale[0] < scale)
                        Scale[0] = scale;
                }
            }
        }

        public Vector2 GetPosition(int dir = 1, int i = 0)
        {
            Vector2 positionOffset = new Vector2(offset[i].X, offset[i].Y);// * GetScale(i);
            if (InheritOffset)
            {
                positionOffset.Y *= dir;
                if (InheritOffsetRotation)
                    positionOffset = positionOffset.RotatedBy(Parent.GetRotation(dir, i));
                if (ParentOrigin)
                    return positionOffset + Parent.GetPosition(dir, i);
                return positionOffset + Parent.EndPoint(dir, i);
            }
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

        public Vector2 DifferenceBone(BonePart bone)
        {
            return bone.GetPosition() - GetPosition();
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

        /// <summary>
        /// Standard bone drawing, Manual draw if needed
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="Trail"></param>
        /// <returns></returns>
        public DrawData DrawBone(Color color, int dir = 1, int Trail = 0)
        {
            return DrawBone(Texture, color, dir, Trail);
        }
        /// <summary>
        /// Variant usually used for Glowmasks, needs to have the same frame as original Texture
        /// </summary>
        /// <param name="color"></param>
        /// <param name="dir"></param>
        /// <param name="Trail"></param>
        /// <returns></returns>
        public DrawData DrawBone(Texture2D texture2, Color color, int dir = 1, int Trail = 0)
        {
            Vector2 origin = Origin;
            if (dir == -1)
            {
                origin.X = Frame.Width - origin.X;
            }

            return new DrawData(texture2,
                    GetPosition(dir, Trail) - Main.screenPosition,
                    Frame,
                    color,
                    GetRotation(dir, Trail) + RotationOffset,
                    Origin,
                    Scale[Trail],
                    dir < 0 ? SpriteEffects.FlipHorizontally : 0f,
                    0);
        }

        public void DrawBone(SpriteBatch sp, Color color, int dir = 1, int Trail = 0)
        {
            DrawBone(sp, Texture, color, dir, Trail);
        }

        public void DrawBone(SpriteBatch sp, Texture2D texture2, Color color, int dir = 1, int Trail = 0)
        {
            Vector2 origin = Origin;
            if (dir == -1)
            {
                origin.X = Frame.Width - origin.X;
            }

            DrawBoneManual(sp,
                texture2,
                GetPosition(dir, Trail) - Main.screenPosition,
                Frame,
                color,
                GetRotation(dir, Trail) + RotationOffset,
                origin,
                Scale[Trail],
                dir < 0 ? SpriteEffects.FlipHorizontally : 0f);
        }

        /// <summary>
        /// Used for non vertial sprites
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="Trail"></param>
        /// <param name="rot1"></param>
        /// <param name="rot2"></param>
        /// <returns></returns>
        public DrawData DrawBoneAltRot(Color color, int dir = 1, int Trail = 0, float rot1 = 0.785f, float rot2 = 2.355f)
        {
            return DrawBoneAltRot(Texture, color, dir, Trail, rot1, rot2);
        }

        /// <summary>
        /// Variant usually used for Glowmasks, needs to have the same frame as original Texture
        /// </summary>
        /// <param name="color"></param>
        /// <param name="dir"></param>
        /// <param name="Trail"></param>
        /// <returns></returns>
        public DrawData DrawBoneAltRot(Texture2D texture2, Color color, int dir = 1, int Trail = 0, float rot1 = 0.785f, float rot2 = 2.355f)
        {
            if (dir == -1)
            {
                Origin.X = Frame.Width - Origin.X;
            }

            return new DrawData(texture2,
                    GetPosition(dir, Trail) - Main.screenPosition,
                    Frame,
                    color,
                    GetRotation(dir, Trail) + RotationOffset - (dir == 1 ? 0.785f : 2.355f),
                    Origin,
                    Scale[Trail],
                    dir < 0 ? SpriteEffects.FlipHorizontally : 0f,
                    0);
        }

        /// <summary>
        /// Used for non vertial sprites
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="Trail"></param>
        /// <param name="rot1"></param>
        /// <param name="rot2"></param>
        /// <returns></returns>
        public void DrawBoneAltRot(SpriteBatch sp, Color color, int dir = 1, int Trail = 0, float rot1 = 0.785f, float rot2 = 2.355f)
        {
            DrawBoneAltRot(sp, Texture, color, dir, Trail, rot1, rot2);
        }

        /// <summary>
        /// Variant usually used for Glowmasks, needs to have the same frame as original Texture
        /// </summary>
        /// <param name="color"></param>
        /// <param name="dir"></param>
        /// <param name="Trail"></param>
        /// <returns></returns>
        public void DrawBoneAltRot(SpriteBatch sp, Texture2D texture2, Color color, int dir = 1, int Trail = 0, float rot1 = 0.785f, float rot2 = 2.355f)
        {
            Vector2 origin = Origin;
            if (dir == -1)
            {
                origin.X = Frame.Width - origin.X;
            }

            DrawBoneManual(sp,
                texture2,
                GetPosition(dir, Trail) - Main.screenPosition,
                Frame,
                color,
                GetRotation(dir, Trail) + RotationOffset - (dir == 1 ? 0.785f : 2.355f),
                origin,
                Scale[Trail],
                dir < 0 ? SpriteEffects.FlipHorizontally : 0f);
        }

        /// <summary>
        /// Manually draw Bone, ignoring parameters set by SetDraw()
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="tex"></param>
        /// <param name="Pos"></param>
        /// <param name="Frame"></param>
        /// <param name="color"></param>
        /// <param name="rot"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="speffects"></param>
        public void DrawBoneManual(SpriteBatch sp, Texture2D tex, Vector2 Pos, Rectangle Frame, Color color, float rot, Vector2 origin, float scale, SpriteEffects speffects)
        {
            sp.Draw(tex,
                    Pos,
                    Frame,
                    color,
                    rot,
                    origin,
                    scale,
                    speffects,
                    0f);
        }
    }
}

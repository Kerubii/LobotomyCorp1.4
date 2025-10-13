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

namespace LobotomyCorp.Utils
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
    
    class AIHelper
    {
        /// <summary>
        /// Time is the time it reaches END from START
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="gravity"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Vector2 ProjectileMotion(Vector2 start, Vector2 end, float time, float gravity)
        {
            Vector2 delta = start - end;
            Vector2 velocity;
            velocity.Y = (delta.Y - 0.5f * gravity * time * time) / time;
            velocity.X = delta.X / time;
            return velocity;
        }

        /// <summary>
        /// Check the distance of angles
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="targetAngle"></param>
        /// <returns></returns>
        public static float AngleDistance(float angle1, float angle2)
        {
            float pi = (float)Math.PI;
            float diff = (angle2 - angle1 + pi) % (pi * 2) - pi;
            return Math.Abs(diff < -pi ? diff + pi * 2 : diff);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="vel"></param>
        /// <param name="speed"></param>
        /// <param name="target"></param>
        public static bool ChaseTargetDirect(Vector2 pos, Vector2 target, ref Vector2 vel, float speed, float spacing = 0)
        {
            float dist = pos.Distance(target);
            if (speed > dist)
            {
                speed = dist;
            }
            Vector2 dir = pos.DirectionTo(target);
            if (!dir.HasNaNs() && dist >= spacing)
            {
                vel = dir * speed;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Adds accel to normalized Velocity
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="vel"></param>
        /// <param name="speed"></param>
        /// <param name="accel"></param>
        /// <param name="target"></param>
        public static bool ChaseTargetDirectAccel(Vector2 pos, Vector2 target, ref Vector2 vel, float speed, float accel, float spacing = 0)
        {
            float dist = pos.Distance(target);
            Vector2 dir = pos.DirectionTo(target);
            if (!dir.HasNaNs() && dist >= spacing)
            {
                vel += dir * accel;
                if (vel.Length() > speed)
                {
                    vel.Normalize();
                    vel = vel * speed;
                }
                return true;
            }
            return false;
        }

        public static bool ChaseTargetLerp(Vector2 pos, Vector2 target, ref Vector2 vel, float speed, float lerp, float spacing = 0)
        {
            float dist = pos.Distance(target);
            if (speed > dist)
            {
                speed = dist;
            }
            Vector2 dir = pos.DirectionTo(target);
            if (!dir.HasNaNs() && dist >= spacing)
            {
                vel = Vector2.Lerp(vel, dir * speed, lerp);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds accel to both Velocity Seperately
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="vel"></param>
        /// <param name="speed"></param>
        /// <param name="accel"></param>
        /// <param name="target"></param>
        /// <param name="targetSize"></param>
        public static void ChaseTargetAccel(Vector2 pos, Vector2 target, ref Vector2 vel, float speed, float accel, float spacing = 0)
        { 
            Vector2 delt = target - pos;
            float dist = delt.Length();
            delt.Normalize();
            delt *= speed;

            if (dist >= spacing)
            {
                if (pos.X < target.X)
                {
                    vel.X += accel;
                    if (vel.X < 0)
                        vel.X += accel;
                    if (vel.X > delt.X)
                        vel.X = delt.X;
                }
                else if (pos.X > target.X)
                {
                    vel.X -= accel;
                    if (vel.X > 0)
                        vel.X -= accel;
                    if (vel.X < delt.X)
                        vel.X = delt.X;
                }

                if (pos.Y < target.Y)
                {
                    vel.Y += accel;
                    if (vel.Y < 0)
                        vel.Y += accel;
                    if (vel.Y > delt.Y)
                        vel.Y = delt.Y;
                }
                else if (pos.Y > target.Y)
                {
                    vel.Y -= accel;
                    if (vel.Y > 0)
                        vel.Y -= accel;
                    if (vel.Y < delt.Y)
                        vel.Y = delt.Y;
                }
            }
        }
    }
    /*
    abstract class Fighter : NPC
	{
        private void AI_003_Fighters()
        {
            // Checks if player is same level as npc
            if (Main.player[target].position.Y + (float)Main.player[target].height == position.Y + (float)height)
            {
                directionY = -1;
            }
            bool forceJumpCheck = false;
            Vector2 val;
            Rectangle hitbox;
            bool Jump = false;
            bool JusthitorStopped = false;
            if (velocity.X == 0f)
            {
                JusthitorStopped = true;
            }
            if (justHit)
            {
                JusthitorStopped = false;
            }
            int num154 = 60;
            bool flag25 = false;
            bool flag26 = true;
            bool flag27 = false;
            bool flag2 = true;
            if (!flag27 && flag2)
            {
                if (velocity.Y == 0f && ((velocity.X > 0f && direction < 0) || (velocity.X < 0f && direction > 0)))
                {
                    flag25 = true;
                }
                if (position.X == oldPosition.X || ai[3] >= (float)num154 || flag25)
                {
                    ai[3] += 1f;
                }
                else if ((double)Math.Abs(velocity.X) > 0.9 && ai[3] > 0f)
                {
                    ai[3] -= 1f;
                }
                if (ai[3] > (float)(num154 * 10))
                {
                    ai[3] = 0f;
                }
                if (justHit)
                {
                    ai[3] = 0f;
                }
                if (ai[3] == (float)num154)
                {
                    netUpdate = true;
                }
                hitbox = Main.player[target].Hitbox;
                if (((Rectangle)(ref hitbox)).Intersects(base.Hitbox))
                {
                    ai[3] = 0f;
                }
            }
            if (ai[3] < (float)num154 && DespawnEncouragement_AIStyle3_Fighters_NotDiscouraged(type, position, this))
            {
                if (shimmerTransparency < 1f)
                {
                }
                TargetClosest();
                if (directionY > 0 && Main.player[target].Center.Y <= base.Bottom.Y)
                {
                    directionY = -1;
                }
            }
            else if (!(ai[2] > 0f) || !DespawnEncouragement_AIStyle3_Fighters_CanBeBusyWithAction(type))
            {
                if (Main.IsItDay() && (double)(position.Y / 16f) < Main.worldSurface && type != 624 && type != 631)
                {
                    EncourageDespawn(10);
                }
                if (velocity.X == 0f)
                {
                    if (velocity.Y == 0f)
                    {
                        ai[0] += 1f;
                        if (ai[0] >= 2f)
                        {
                            direction *= -1;
                            spriteDirection = direction;
                            ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    ai[0] = 0f;
                }
                if (direction == 0)
                {
                    direction = 1;
                }
            }
            if (Main.netMode != 1)
            {
                if (Main.expertMode && target >= 0 && (type == 163 || type == 238 || type == 236 || type == 237) && Collision.CanHit(base.Center, 1, 1, Main.player[target].Center, 1, 1))
                {
                    localAI[0] += 1f;
                    if (justHit)
                    {
                        localAI[0] -= Main.rand.Next(20, 60);
                        if (localAI[0] < 0f)
                        {
                            localAI[0] = 0f;
                        }
                    }
                    if (localAI[0] > (float)Main.rand.Next(180, 900))
                    {
                        localAI[0] = 0f;
                        Vector2 vector24 = Main.player[target].Center - base.Center;
                        ((Vector2)(ref vector24)).Normalize();
                        vector24 *= 8f;
                        int attackDamage_ForProjectiles2 = GetAttackDamage_ForProjectiles(18f, 18f);
                        Projectile.NewProjectile(GetSpawnSource_ForProjectile(), base.Center.X, base.Center.Y, vector24.X, vector24.Y, 472, attackDamage_ForProjectiles2, 0f, Main.myPlayer);
                    }
                }
            }
            if (velocity.Y == 0f || forceJumpCheck)
            {
                int num91 = (int)(position.Y + (float)height + 7f) / 16;
                int num92 = (int)(position.Y - 9f) / 16;
                int num93 = (int)position.X / 16;
                int num94 = (int)(position.X + (float)width) / 16;
                int num205 = (int)(position.X + 8f) / 16;
                int num95 = (int)(position.X + (float)width - 8f) / 16;
                bool flag15 = false;
                for (int num96 = num205; num96 <= num95; num96++)
                {
                    if (num96 >= num93 && num96 <= num94 && Main.tile[num96, num91] == null)
                    {
                        flag15 = true;
                        continue;
                    }
                    if (Main.tile[num96, num92] != null && Main.tile[num96, num92].nactive() && Main.tileSolid[Main.tile[num96, num92].type])
                    {
                        Jump = false;
                        break;
                    }
                    if (!flag15 && num96 >= num93 && num96 <= num94 && Main.tile[num96, num91].nactive() && Main.tileSolid[Main.tile[num96, num91].type])
                    {
                        Jump = true;
                    }
                }
                if (!Jump && velocity.Y < 0f)
                {
                    velocity.Y = 0f;
                }
                if (flag15)
                {
                    return;
                }
            }
            if (velocity.Y >= 0f && (type != 580 || directionY != 1))
            {
                int moveDirection = 0;
                if (velocity.X < 0f)
                {
                    moveDirection = -1;
                }
                if (velocity.X > 0f)
                {
                    moveDirection = 1;
                }
                Vector2 currentPosition = position;
                currentPosition.X += velocity.X;
                int num98 = (int)((currentPosition.X + (float)(width / 2) + (float)((width / 2 + 1) * moveDirection)) / 16f);
                int num100 = (int)((currentPosition.Y + (float)height - 1f) / 16f);
                if (WorldGen.InWorld(num98, num100, 4))
                {
                    if ((float)(num98 * 16) < currentPosition.X + (float)width && (float)(num98 * 16 + 16) > currentPosition.X && ((Main.tile[num98, num100].HasUnactuatedTile && !Main.tile[num98, num100].TopSlope && !Main.tile[num98, num100 - 1].topSlope() && Main.tileSolid[Main.tile[num98, num100].type] && !Main.tileSolidTop[Main.tile[num98, num100].type]) || (Main.tile[num98, num100 - 1].halfBrick() && Main.tile[num98, num100 - 1].nactive())) && (!Main.tile[num98, num100 - 1].nactive() || !Main.tileSolid[Main.tile[num98, num100 - 1].type] || Main.tileSolidTop[Main.tile[num98, num100 - 1].type] || (Main.tile[num98, num100 - 1].halfBrick() && (!Main.tile[num98, num100 - 4].nactive() || !Main.tileSolid[Main.tile[num98, num100 - 4].type] || Main.tileSolidTop[Main.tile[num98, num100 - 4].type]))) && (!Main.tile[num98, num100 - 2].nactive() || !Main.tileSolid[Main.tile[num98, num100 - 2].type] || Main.tileSolidTop[Main.tile[num98, num100 - 2].type]) && (!Main.tile[num98, num100 - 3].nactive() || !Main.tileSolid[Main.tile[num98, num100 - 3].type] || Main.tileSolidTop[Main.tile[num98, num100 - 3].type]) && (!Main.tile[num98 - moveDirection, num100 - 3].nactive() || !Main.tileSolid[Main.tile[num98 - moveDirection, num100 - 3].type]))
                    {
                        float num101 = num100 * 16;
                        if (Main.tile[num98, num100].halfBrick())
                        {
                            num101 += 8f;
                        }
                        if (Main.tile[num98, num100 - 1].halfBrick())
                        {
                            num101 -= 8f;
                        }
                        if (num101 < currentPosition.Y + (float)height)
                        {
                            float num102 = currentPosition.Y + (float)height - num101;
                            float num103 = 16.1f;
                            if (type == 163 || type == 164 || type == 236 || type == 239 || type == 530)
                            {
                                num103 += 8f;
                            }
                            if (num102 <= num103)
                            {
                                gfxOffY += position.Y + (float)height - num101;
                                position.Y = num101 - (float)height;
                                if (num102 < 9f)
                                {
                                    stepSpeed = 1f;
                                }
                                else
                                {
                                    stepSpeed = 2f;
                                }
                            }
                        }
                    }
                }
            }
            if (Jump)
            {
                // A Giant Mytery wtf is this
                int num104 = (int)((position.X + (float)(width / 2) + (float)(15 * direction)) / 16f);
                int num105 = (int)((position.Y + (float)height - 15f) / 16f);
                Main.tile[num104, num105 + 1].halfBrick();
                if (Main.tile[num104, num105 - 1].nactive() && (TileLoader.IsClosedDoor(Main.tile[num104, num105 - 1]) || Main.tile[num104, num105 - 1].type == 388) && flag26)
                {
                    ai[2] += 1f;
                    ai[3] = 0f;
                    if (ai[2] >= 60f)
                    {
                        bool flag16 = type == 3 || type == 430 || type == 590 || type == 331 || type == 332 || type == 132 || type == 161 || type == 186 || type == 187 || type == 188 || type == 189 || type == 200 || type == 223 || type == 320 || type == 321 || type == 319 || type == 21 || type == 324 || type == 323 || type == 322 || type == 44 || type == 196 || type == 167 || type == 77 || type == 197 || type == 202 || type == 203 || type == 449 || type == 450 || type == 451 || type == 452 || type == 481 || type == 201 || type == 635;
                        bool flag17 = Main.player[target].ZoneGraveyard && Main.rand.Next(60) == 0;
                        if ((!Main.bloodMoon || Main.getGoodWorld) && !flag17 && flag16)
                        {
                            ai[1] = 0f;
                        }
                        velocity.X = 0.5f * (float)(-direction);
                        int num106 = 5;
                        ai[1] += num106;
                        ai[2] = 0f;
                        bool flag18 = false;
                        if (ai[1] >= 10f)
                        {
                            flag18 = true;
                            ai[1] = 10f;
                        }
                        if (type == 460)
                        {
                            flag18 = true;
                        }
                        WorldGen.KillTile(num104, num105 - 1, fail: true);
                        if ((Main.netMode != 1 || !flag18) && flag18 && Main.netMode != 1)
                        {
                            if (type == 26)
                            {
                                WorldGen.KillTile(num104, num105 - 1);
                                if (Main.netMode == 2)
                                {
                                    NetMessage.SendData(17, -1, -1, null, 0, num104, num105 - 1);
                                }
                            }
                            else
                            {
                                if (TileLoader.IsClosedDoor(Main.tile[num104, num105 - 1]))
                                {
                                    bool flag19 = WorldGen.OpenDoor(num104, num105 - 1, direction);
                                    if (!flag19)
                                    {
                                        ai[3] = num154;
                                        netUpdate = true;
                                    }
                                    if (Main.netMode == 2 && flag19)
                                    {
                                        NetMessage.SendData(19, -1, -1, null, 0, num104, num105 - 1, direction);
                                    }
                                }
                                if (Main.tile[num104, num105 - 1].type == 388)
                                {
                                    bool flag20 = WorldGen.ShiftTallGate(num104, num105 - 1, closing: false);
                                    if (!flag20)
                                    {
                                        ai[3] = num154;
                                        netUpdate = true;
                                    }
                                    if (Main.netMode == 2 && flag20)
                                    {
                                        NetMessage.SendData(19, -1, -1, null, 4, num104, num105 - 1);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    int dir = spriteDirection;
                    if ((velocity.X < 0f && dir == -1) || (velocity.X > 0f && dir == 1))
                    {
                        if (height >= 32 && Main.tile[num104, num105 - 2].nactive() && Main.tileSolid[Main.tile[num104, num105 - 2].type])
                        {
                            if (Main.tile[num104, num105 - 3].nactive() && Main.tileSolid[Main.tile[num104, num105 - 3].type])
                            {
                                velocity.Y = -8f;
                                netUpdate = true;
                            }
                            else
                            {
                                velocity.Y = -7f;
                                netUpdate = true;
                            }
                        }
                        else if (Main.tile[num104, num105 - 1].nactive() && Main.tileSolid[Main.tile[num104, num105 - 1].type])
                        {
                            else
                            {
                                velocity.Y = -6f;
                            }
                            netUpdate = true;
                        }
                        else if (position.Y + (float)height - (float)(num105 * 16) > 20f && Main.tile[num104, num105].nactive() && !Main.tile[num104, num105].topSlope() && Main.tileSolid[Main.tile[num104, num105].type])
                        {
                            velocity.Y = -5f;
                            netUpdate = true;
                        }
                        else if (flag26)
                        {
                            ai[1] = 0f;
                            ai[2] = 0f;
                        }
                        if (velocity.Y == 0f && JusthitorStopped && ai[3] == 1f)
                        {
                            velocity.Y = -5f;
                        }
                    }
                }
            }
        }
    }*/
}

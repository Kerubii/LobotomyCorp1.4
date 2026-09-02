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
        public static bool ChaseTargetAccel(Vector2 pos, Vector2 target, ref Vector2 vel, float speed, float accel, float spacing = 0)
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
                return true;
            }
            return false;
        }
    
        public static void FighterAI(NPC npc)
        {
            Player player = Main.player[npc.target];

            if (player.position.Y + player.height == npc.position.Y + npc.height)
            {
                npc.directionY = -1;
            }
            bool forceJumpCheck = false;
            Vector2 val;
            Rectangle hitbox;
            bool Jump = false;
            bool JusthitorStopped = false;
            if (npc.velocity.X == 0f)
            {
                JusthitorStopped = true;
            }
            if (npc.justHit)
            {
                JusthitorStopped = false;
            }
            int discourageTime = 60;
            bool jumpReady = false;
            bool flag26 = true;
            bool canLoseInterest = true;
            if (canLoseInterest)
            {
                if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
                {
                    jumpReady = true;
                }
                if (npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)discourageTime || jumpReady)
                {
                    npc.ai[3] += 1f;
                }
                else if ((double)Math.Abs(npc.velocity.X) > 0.9 && npc.ai[3] > 0f)
                {
                    npc.ai[3] -= 1f;
                }
                if (npc.ai[3] > (float)(discourageTime * 10))
                {
                    npc.ai[3] = 0f;
                }
                if (npc.justHit)
                {
                    npc.ai[3] = 0f;
                }
                if (npc.ai[3] == (float)discourageTime)
                {
                    npc.netUpdate = true;
                }
                hitbox = player.Hitbox;
                if (hitbox.Intersects(npc.Hitbox))
                {
                    npc.ai[3] = 0f;
                }
            }
            if (npc.ai[3] < (float)discourageTime && NPC.DespawnEncouragement_AIStyle3_Fighters_NotDiscouraged(npc.type, npc.position, npc))
            {
                if (npc.shimmerTransparency < 1f)
                {

                }
                npc.TargetClosest();
                if (npc.directionY > 0 && player.Center.Y <= npc.Bottom.Y)
                {
                    npc.directionY = -1;
                }
            }
            else if (!(npc.ai[2] > 0f) || !NPC.DespawnEncouragement_AIStyle3_Fighters_CanBeBusyWithAction(npc.type))
            {
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.ai[0] += 1f;
                        if (npc.ai[0] >= 2f)
                        {
                            npc.direction *= -1;
                            npc.spriteDirection = npc.direction;
                            npc.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    npc.ai[0] = 0f;
                }
                if (npc.direction == 0)
                {
                    npc.direction = 1;
                }
            }

            if (npc.velocity.Y == 0f || forceJumpCheck)
            {
                int tileYT = (int)(npc.position.Y + (float)npc.height + 7f) / 16;
                int tileYB = (int)(npc.position.Y - 9f) / 16;
                int tileXL = (int)npc.position.X / 16;
                int tileXR = (int)(npc.position.X + (float)npc.width) / 16;
                int xMin = (int)(npc.position.X + 8f) / 16;
                int xMax = (int)(npc.position.X + (float)npc.width - 8f) / 16;
                bool dontJump = false;
                for (int i = xMin; i <= xMax; i++)
                {
                    if (i >= tileXL && i <= tileXR && Main.tile[i, tileYT] == null)
                    {
                        dontJump = true;
                        continue;
                    }
                    if (Main.tile[i, tileYB] != null && Main.tile[i, tileYB].HasUnactuatedTile && Main.tileSolid[Main.tile[i, tileYB].TileType])
                    {
                        Jump = false;
                        break;
                    }
                    if (!dontJump && i >= tileXL && i <= tileXR && Main.tile[i, tileYT].HasUnactuatedTile && Main.tileSolid[Main.tile[i, tileYT].TileType])
                    {
                        Jump = true;
                    }
                }
                if (!Jump && npc.velocity.Y < 0f)
                {
                    npc.velocity.Y = 0f;
                }
                if (dontJump)
                {
                    return;
                }
            }
            if (npc.velocity.Y >= 0f && npc.directionY != 1)
            {
                int moveDirection = 0;
                if (npc.velocity.X < 0f)
                {
                    moveDirection = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    moveDirection = 1;
                }
                Vector2 currentPosition = npc.position;
                currentPosition.X += npc.velocity.X;
                int tX = (int)((currentPosition.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * moveDirection)) / 16f);
                int tY = (int)((currentPosition.Y + (float)npc.height - 1f) / 16f);
                if (WorldGen.InWorld(tX, tY, 4))
                {
                    if ((float)(tX * 16) < currentPosition.X + (float)npc.width && (float)(tX * 16 + 16) > currentPosition.X && ((Main.tile[tX, tY].HasUnactuatedTile && !Main.tile[tX, tY].TopSlope && !Main.tile[tX, tY - 1].TopSlope && Main.tileSolid[Main.tile[tX, tY].TileType] && !Main.tileSolidTop[Main.tile[tX, tY].TileType]) || (Main.tile[tX, tY - 1].IsHalfBlock && Main.tile[tX, tY - 1].HasUnactuatedTile)) && (!Main.tile[tX, tY - 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[tX, tY - 1].TileType] || Main.tileSolidTop[Main.tile[tX, tY - 1].TileType] || (Main.tile[tX, tY - 1].IsHalfBlock && (!Main.tile[tX, tY - 4].HasUnactuatedTile || !Main.tileSolid[Main.tile[tX, tY - 4].TileType] || Main.tileSolidTop[Main.tile[tX, tY - 4].TileType]))) && (!Main.tile[tX, tY - 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[tX, tY - 2].TileType] || Main.tileSolidTop[Main.tile[tX, tY - 2].TileType]) && (!Main.tile[tX, tY - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[tX, tY - 3].TileType] || Main.tileSolidTop[Main.tile[tX, tY - 3].TileType]) && (!Main.tile[tX - moveDirection, tY - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[tX - moveDirection, tY - 3].TileType]))
                    {
                        float num101 = tY * 16;
                        if (Main.tile[tX, tY].IsHalfBlock)
                        {
                            num101 += 8f;
                        }
                        if (Main.tile[tX, tY - 1].IsHalfBlock)
                        {
                            num101 -= 8f;
                        }
                        if (num101 < currentPosition.Y + (float)npc.height)
                        {
                            float num102 = currentPosition.Y + (float)npc.height - num101;
                            float num103 = 16.1f;
                            if (npc.type == 163 || npc.type == 164 || npc.type == 236 || npc.type == 239 || npc.type == 530)
                            {
                                num103 += 8f;
                            }
                            if (num102 <= num103)
                            {
                                npc.gfxOffY += npc.position.Y + (float)npc.height - num101;
                                npc.position.Y = num101 - (float)npc.height;
                                if (num102 < 9f)
                                {
                                    npc.stepSpeed = 1f;
                                }
                                else
                                {
                                    npc.stepSpeed = 2f;
                                }
                            }
                        }
                    }
                }
            }
            if (Jump)
            {
                // A Giant Mytery wtf is this
                int num104 = (int)((npc.position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f);
                int num105 = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);

                if (Main.tile[num104, num105 - 1].HasUnactuatedTile && (TileLoader.IsClosedDoor(Main.tile[num104, num105 - 1]) || Main.tile[num104, num105 - 1].TileType == 388) && flag26)
                {
                    npc.ai[2] += 1f;
                    npc.ai[3] = 0f;
                    if (npc.ai[2] >= 60f)
                    {
                        bool flag16 = npc.type == 3 || npc.type == 430 || npc.type == 590 || npc.type == 331 || npc.type == 332 || npc.type == 132 || npc.type == 161 || npc.type == 186 || npc.type == 187 || npc.type == 188 || npc.type == 189 || npc.type == 200 || npc.type == 223 || npc.type == 320 || npc.type == 321 || npc.type == 319 || npc.type == 21 || npc.type == 324 || npc.type == 323 || npc.type == 322 || npc.type == 44 || npc.type == 196 || npc.type == 167 || npc.type == 77 || npc.type == 197 || npc.type == 202 || npc.type == 203 || npc.type == 449 || npc.type == 450 || npc.type == 451 || npc.type == 452 || npc.type == 481 || npc.type == 201 || npc.type == 635;
                        bool flag17 = player.ZoneGraveyard && Main.rand.Next(60) == 0;
                        if ((!Main.bloodMoon || Main.getGoodWorld) && !flag17 && flag16)
                        {
                            npc.ai[1] = 0f;
                        }
                        npc.velocity.X = 0.5f * (float)(-npc.direction);
                        int num106 = 5;
                        npc.ai[1] += num106;
                        npc.ai[2] = 0f;
                        bool flag18 = false;
                        if (npc.ai[1] >= 10f)
                        {
                            flag18 = true;
                            npc.ai[1] = 10f;
                        }
                        if (npc.type == 460)
                        {
                            flag18 = true;
                        }
                        WorldGen.KillTile(num104, num105 - 1, fail: true);
                        if ((Main.netMode != 1 || !flag18) && flag18 && Main.netMode != 1)
                        {
                            if (npc.type == 26)
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
                                    bool flag19 = WorldGen.OpenDoor(num104, num105 - 1, npc.direction);
                                    if (!flag19)
                                    {
                                        npc.ai[3] = discourageTime;
                                        npc.netUpdate = true;
                                    }
                                    if (Main.netMode == 2 && flag19)
                                    {
                                        NetMessage.SendData(19, -1, -1, null, 0, num104, num105 - 1, npc.direction);
                                    }
                                }
                                if (Main.tile[num104, num105 - 1].TileType == 388)
                                {
                                    bool flag20 = WorldGen.ShiftTallGate(num104, num105 - 1, closing: false);
                                    if (!flag20)
                                    {
                                        npc.ai[3] = discourageTime;
                                        npc.netUpdate = true;
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
                    int dir = npc.spriteDirection;
                    if ((npc.velocity.X < 0f && dir == -1) || (npc.velocity.X > 0f && dir == 1))
                    {
                        if (npc.height >= 32 && Main.tile[num104, num105 - 2].HasUnactuatedTile && Main.tileSolid[Main.tile[num104, num105 - 2].TileType])
                        {
                            if (Main.tile[num104, num105 - 3].HasUnactuatedTile && Main.tileSolid[Main.tile[num104, num105 - 3].TileType])
                            {
                                npc.velocity.Y = -8f;
                                npc.netUpdate = true;
                            }
                            else
                            {
                                npc.velocity.Y = -7f;
                                npc.netUpdate = true;
                            }
                        }
                        else if (Main.tile[num104, num105 - 1].HasUnactuatedTile && Main.tileSolid[Main.tile[num104, num105 - 1].TileType])
                        {
                            npc.velocity.Y = -6f;
                            npc.netUpdate = true;
                        }
                        else if (npc.position.Y + (float)npc.height - (float)(num105 * 16) > 20f && Main.tile[num104, num105].HasUnactuatedTile && !Main.tile[num104, num105].TopSlope && Main.tileSolid[Main.tile[num104, num105].TileType])
                        {
                            npc.velocity.Y = -5f;
                            npc.netUpdate = true;
                        }
                        else if (flag26)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }
                        if (npc.velocity.Y == 0f && JusthitorStopped && npc.ai[3] == 1f)
                        {
                            npc.velocity.Y = -5f;
                        }
                    }
                }
            }
        }
    }
}

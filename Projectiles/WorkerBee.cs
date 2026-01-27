//css_ref ../tModLoader.dll
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	class WorkerBee : ModProjectile
	{
        public static Asset<Texture2D> IntestineTex;

        public override void Load()
        {
            IntestineTex = ModContent.Request<Texture2D>(Texture + "Intestine");
        }

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 13;
			//Main.projPet[Projectile.type] = true;
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}
		
		public override void SetDefaults()
		{
			Projectile.netImportant = true;
			Projectile.width = 32;
			Projectile.height = 42;
			Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 3600;
			Projectile.tileCollide = false;

            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;

            IntestinePhysics = Vector2.Zero;
		}

        private Vector2 IntestinePhysics = Vector2.Zero;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);
            bool buffActive = player.HasBuff<Buffs.BoostSpore>();

            bool moveLeft = false;
            bool moveRight = false;
            int Pos = 0;
            for (int i = Projectile.whoAmI - 1; i >= 0; i--)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == Projectile.type && Main.projectile[i].owner == Projectile.owner) Pos++;
            }
            int targetFollowDist = 40 * (Pos + 1) * player.direction;
            if (player.Center.X < Projectile.Center.X + (float)targetFollowDist - 10f)
            {
                moveLeft = true;
            }
            else if (player.Center.X > Projectile.Center.X + (float)targetFollowDist + 10f)
            {
                moveRight = true;
            }

            int flyDistance = 1200 + 40 * Projectile.minionPos;
            if (player.rocketDelay2 > 0)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            float distance = Vector2.Distance(Projectile.Center, player.Center);
            if (distance > 2000f)
            {
                Projectile.position.X = player.position.X + (float)(player.width / 2) - (float)(Projectile.width / 2);
                Projectile.position.Y = player.position.Y + (float)(player.height / 2) - (float)(Projectile.height / 2);
            }
            else if (distance > flyDistance || Math.Abs(Projectile.Center.Y - player.Center.Y) > 480f)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            //Flying
            if (Projectile.ai[0] != 0f)
            {
                Projectile.tileCollide = false;
                float moveDistX = player.position.X + (float)(player.width / 2) - Projectile.Center.X - (float)(40 * player.direction);
                float viewRange = 600f;
                bool aggro = false;
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].CanBeChasedBy(this))
                    {
                        //distance = System.Math.Abs(player.Center.Y - Main.npc[k].Center.X) + Math.Abs(player.Center.Y - Main.npc[k].Center.Y);
                        distance = (Main.npc[k].Center - player.Center).Length();

                        if (distance < viewRange)
                        {
                            aggro = true;
                            break;
                        }
                    }
                }
                if (!aggro)
                {
                    moveDistX -= (float)(40 * Projectile.minionPos * player.direction);
                }

                float moveDistY = player.position.Y + (float)(player.height / 2) - Projectile.Center.Y;
                float moveDist = (float)System.Math.Sqrt((double)(moveDistX * moveDistX + moveDistY * moveDistY));
                float maxSpeed = 10f;
                if (moveDist < 480f && (player.velocity.Y == 0f || aggro) && Projectile.position.Y + (float)Projectile.height <= player.position.Y + (float)player.height && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    if (Projectile.velocity.Y < -6f)
                    {
                        Projectile.velocity.Y = -6f;
                    }
                    Projectile.netUpdate = true;
                }
                if (moveDist < 60f)
                {
                    moveDistX = Projectile.velocity.X;
                    moveDistY = Projectile.velocity.Y;
                }
                else
                {
                    moveDist = maxSpeed / moveDist;
                    moveDistX *= moveDist;
                    moveDistY *= moveDist;
                }
                float acceleration = 0.2f;
                if (Projectile.velocity.X < moveDistX)
                {
                    Projectile.velocity.X += acceleration;
                    if (Projectile.velocity.X < 0f)
                    {
                        Projectile.velocity.X += acceleration * 1.5f;
                    }
                }
                if (Projectile.velocity.X > moveDistX)
                {
                    Projectile.velocity.X -= acceleration;
                    if (Projectile.velocity.X > 0f)
                    {
                        Projectile.velocity.X -= acceleration * 1.5f;
                    }
                }
                if (Projectile.velocity.Y < moveDistY)
                {
                    Projectile.velocity.Y += acceleration;
                    if (Projectile.velocity.Y < 0f)
                    {
                        Projectile.velocity.Y += acceleration * 1.5f;
                    }
                }
                if (Projectile.velocity.Y > moveDistY)
                {
                    Projectile.velocity.Y -= acceleration;
                    if (Projectile.velocity.Y > 0f)
                    {
                        Projectile.velocity.Y -= acceleration * 1.5f;
                    }
                }
                if ((double)Projectile.velocity.X > 0.5)
                {
                    Projectile.spriteDirection = 1;
                }
                else if ((double)Projectile.velocity.X < -0.5)
                {
                    Projectile.spriteDirection = -1;
                }
                               

                Projectile.frame = 8;
                Projectile.frameCounter = 0;
            }
            else
            {
                //Prevent grouping up
                for (int index = 0; index < 1000; ++index)
                {
                    if (index != Projectile.whoAmI && Main.projectile[index].active && (Main.projectile[index].owner == Projectile.owner && Main.projectile[index].type == Projectile.type) && Math.Abs(Projectile.position.X - Main.projectile[index].position.X) + Math.Abs(Projectile.position.Y - Main.projectile[index].position.Y) < Projectile.width)
                    {
                        float num6 = 0.05f;
                        if (Projectile.position.X < Main.projectile[index].position.X)
                            Projectile.velocity.X -= num6;
                        else
                            Projectile.velocity.X += num6;/*
                        if ((double)Projectile.position.Y < (double)Main.projectile[index].position.Y)
                            Projectile.velocity.Y -= num6;
                        else
                            Projectile.velocity.Y += num6;*/
                    }
                }

                bool target = false;
                Vector2 targetCenter = Projectile.position;
                float seperation = (float)(40 * Pos);

                float dist = 1500f + (buffActive ? 750 : 0);
                int currentTarget = -1;
                bool priority = false;
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    if (Main.npc[k].CanBeChasedBy(this))
                    {
                        NPC n = Main.npc[k];
                        float npcDist = Vector2.Distance(n.Center, Projectile.Center);
                        if (!priority || LobotomyGlobalNPC.HornetTarget(n))
                        {
                            if (npcDist < dist)
                            {
                                if (currentTarget == -1 && npcDist <= dist)
                                {
                                    dist = npcDist;
                                }
                                if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, n.position, n.width, n.height))
                                {
                                    currentTarget = k;
                                    target = true;
                                    targetCenter = n.Center;
                                }
                            }

                            if (!priority && LobotomyGlobalNPC.HornetTarget(n))
                            {
                                priority = LobotomyGlobalNPC.HornetTarget(n);
                                dist = 1500f + (buffActive ? 750 : 0);
                                k = -1;
                                currentTarget = k;
                                target = false;
                                targetCenter = Vector2.Zero;
                            }
                        }
                    }
                }
                //Main.NewText(priority + " - " + currentTarget + " - " + dist);
                
                if (target)
                {
                    moveLeft = false;
                    moveRight = false;
                    if (targetCenter.X + 12 < Projectile.Center.X)
                    {
                        moveLeft = true;
                    }
                    else if (targetCenter.X - 12 > Projectile.Center.X)
                    {
                        moveRight = true;
                    }
                }

                Projectile.rotation = 0f;
                Projectile.tileCollide = true;
                float speed = 0.2f;
                float maxAccel = 9f;
                if (buffActive)
                {
                    speed += 0.15f;
                    maxAccel += 3f;
                }

                if (moveLeft)
                {
                    if ((double)Projectile.velocity.X > 0)
                        Projectile.velocity.X *= 0.8f;
                    Projectile.velocity.X -= speed;
                }
                else if (moveRight)
                {
                    if ((double)Projectile.velocity.X < 0)
                        Projectile.velocity.X *= 0.8f;
                    Projectile.velocity.X += speed;
                }
                else
                {
                    Projectile.velocity.X *= 0.8f;
                    if (Projectile.velocity.X >= -speed && Projectile.velocity.X <= speed)
                        Projectile.velocity.X = 0;
                }
                bool collide = false;
                if (moveLeft || moveRight)
                {
                    int x = (int)((Projectile.Center.X + Projectile.velocity.X) / 16);
                    int y = (int)(Projectile.Center.Y / 16);
                    if (moveLeft)
                        --x;
                    if (moveRight)
                        ++x;
                    //x += (int)Projectile.velocity.X/16;
                    if (WorldGen.SolidTile(x, y))
                        collide = true;
                }
                bool pBelow = player.Center.Y > Projectile.position.Y + Projectile.height;
                if (target)
                    pBelow = Main.npc[(int)currentTarget].position.Y > Projectile.position.Y + Projectile.height;
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY, 1, false);
                if (Projectile.velocity.Y == 0f)
                {
                    int dir = 0;
                    bool jump = false;
                    if (moveLeft)
                        dir = -1;
                    if (moveRight)
                        dir = 1;
                    bool targetAbove = Projectile.position.Y + Projectile.height > player.position.Y + player.height;
                    if (target)
                        targetAbove = Projectile.position.Y > targetCenter.Y + Main.npc[(int)currentTarget].height/2;
                    if (targetAbove && (moveLeft || moveRight) && dir == (int)Math.Sign(Projectile.velocity.X))
                    {
                        int y = (int)((Projectile.position.Y + Projectile.height + 8) / 16);
                        int x = (int)((Projectile.Center.X + (Projectile.width / 2 + 1) * dir) / 16);
                        if (!Main.tile[x, y].HasTile || !Main.tileSolid[Main.tile[x, y].TileType])
                        {
                            Projectile.velocity.Y -= 8f;
                            jump = true;
                        }
                    }
                    else if (target && targetAbove)
                    {
                        Vector2 delta = Main.npc[(int)currentTarget].Center - Projectile.Center;
                        if (Projectile.position.X > Main.npc[(int)currentTarget].position.X - 32 && Projectile.position.X + Projectile.width < Main.npc[(int)currentTarget].position.X + Main.npc[(int)currentTarget].width + 32)
                        {
                            float time = Math.Abs(delta.Y/3);
                            if (time == 0)
                                time = 1;
                            Projectile.velocity.Y = (delta.Y - 0.5f * 0.4f * time * time) / time;

                            float limit = -20f;
                            if (buffActive)
                                limit -= 10f;

                            if (Projectile.velocity.Y < limit)
                                Projectile.velocity.Y = limit;
                            Bite();
                            jump = true;
                        }
                    }
                    if (pBelow && (Projectile.velocity.X != 0f))
                    {
                        /*
                        int i = (int)(Projectile.Center.X) / 16;
                        int j = (int)(Projectile.Center.Y) / 16 + 1;
                        if (moveLeft)
                            --i;
                        if (moveRight)
                            ++i;
                        WorldGen.SolidTile(i, j);*/
                        FallThrough();
                    }
                    if (collide && !jump)
                    {
                        int i = (int)Projectile.Center.X / 16;
                        int j = (int)(Projectile.position.Y + Projectile.height) / 16 + 1;
                        if (WorldGen.SolidTile(i, j) || Main.tile[i, j].IsHalfBlock || ((int)Main.tile[i, j].Slope > 0))
                        {
                            try
                            {
                                j--;
                                if (moveLeft)
                                    i--;
                                if (moveRight)
                                    i--;
                                i += (int)Projectile.velocity.X;
                                if (!WorldGen.SolidTile(i, j - 1) && !WorldGen.SolidTile(i, j - 2))
                                    Projectile.velocity.Y = -7f;
                                else if (!WorldGen.SolidTile(i, j - 2))
                                    Projectile.velocity.Y = -9f;
                                else if (WorldGen.SolidTile(i, j - 5))
                                    Projectile.velocity.Y = -14f;
                                else if (WorldGen.SolidTile(i, j - 4))
                                    Projectile.velocity.Y = -13f;
                                else
                                    Projectile.velocity.Y = -11f;
                            }
                            catch
                            {
                                Projectile.velocity.Y = -9.1f;
                            }
                        }
                    }

                    if (Projectile.velocity.X > maxAccel)
                        Projectile.velocity.X = maxAccel;
                    if (Projectile.velocity.X < -maxAccel)
                        Projectile.velocity.X = -maxAccel;

                    if (Projectile.velocity.X < 0.0)
                        Projectile.direction = -1;
                    if (Projectile.velocity.X > 0.0)
                        Projectile.direction = 1;
                    if (Projectile.velocity.X > speed & moveRight)
                        Projectile.direction = 1;
                    if (Projectile.velocity.X < -speed & moveLeft)
                        Projectile.direction = -1;

                    if (moveLeft || moveRight)
                    {
                        Projectile.spriteDirection = Projectile.direction;
                    }
                    else if (target)
                    {
                        if (targetCenter.X < Projectile.Center.X)
                            Projectile.spriteDirection = -1;
                        else if (targetCenter.X > Projectile.Center.X)
                            Projectile.spriteDirection = 1;
                    }
                    else
                    {
                        Projectile.spriteDirection = player.direction;
                    }
                }

                if (Projectile.frame > 8)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 3)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame > 12)
                        {
                            if (Projectile.velocity.Y == 0)
                                Projectile.frame = 0;
                            else
                                Projectile.frame = 8;
                        }
                    }
                }
                else if (Projectile.velocity.Y == 0)
                {
                    if (Projectile.velocity.X == 0)
                    {
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 5f)
                        {
                            Projectile.frameCounter = 0;
                            Projectile.frame++;
                            if (Projectile.frame > 3)
                                Projectile.frame = 0;
                        }
                    }
                    else if (Projectile.velocity.X < -0.8f || Projectile.velocity.X > 0.8f)
                    {
                        Projectile.frameCounter = Projectile.frameCounter + (int)(Math.Abs(Projectile.velocity.X) / 2f);
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 12)
                        {
                            Projectile.frame++;
                            Projectile.frameCounter = 0;
                        }
                        if (Projectile.frame < 4 || Projectile.frame > 7)
                            Projectile.frame = 4;
                    }
                    else
                    {
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 12f)
                        {
                            Projectile.frameCounter = 0;
                            Projectile.frame++;
                            if (Projectile.frame > 3)
                                Projectile.frame = 0;
                        }
                    }
                }
                else
                {
                    if (Projectile.frame < 8)
                    {
                        Projectile.frame = 8;
                        Projectile.frameCounter = 0;
                    }
                }

                if (Projectile.wet)
                {
                    Projectile.Kill();
                }
                else
                    Projectile.velocity.Y += 0.4f;
                if (Projectile.velocity.Y > 10f)
                    Projectile.velocity.Y = 10f;
            }

            if (buffActive && Main.rand.Next(5) == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.HornetDust>());
            }

            //Intestine Physics
            if (IntestinePhysics == Vector2.Zero)
            {
                IntestinePhysics = Projectile.Center + new Vector2(0, 20);
            }

            int intestineLength = 20 - 3;
            Vector2 startPoint = IntestineAttachPoint();

            //Gravity Force
            IntestinePhysics.Y += 0.4f;

            //Reset
            Vector2 delt = (IntestinePhysics - startPoint);
            if (delt.Length() > intestineLength)
            {
                delt.Normalize();
                delt *= intestineLength;
                IntestinePhysics = startPoint + delt;
            }

            if (Projectile.ai[0] != 0 && Main.rand.Next(3) == 0)
                {
                    Dust.NewDustPerfect(IntestinePhysics, DustID.Blood).noGravity = true;
                }
        }

        private Vector2 IntestineAttachPoint()
        {
            Vector2 IntestineAttachPoint = new Vector2(Projectile.Center.X - 15 * Projectile.spriteDirection, Projectile.position.Y + Projectile.height - 13);
            if (Projectile.frame == 2 || Projectile.frame == 3)
                IntestineAttachPoint.X -= 2 * Projectile.spriteDirection;
            else if (Projectile.frame == 5)
                IntestineAttachPoint.Y -= 2;
            else if (Projectile.frame == 7)
                IntestineAttachPoint.X += 2 * Projectile.spriteDirection;
            else if (Projectile.frame >= 8)
            {
                IntestineAttachPoint.Y += 4;
                IntestineAttachPoint.X += 2 * Projectile.spriteDirection;
            }

            return IntestineAttachPoint;
        }

        private void CheckActive(Player p)
        {
            if (!p.active || p.dead)
                Projectile.Kill();
            if (p.statLife < p.statLifeMax * 0.25f)
            {
                Projectile.Kill();
                if (p.whoAmI == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<AngryWorkerBee>(), 20, Projectile.knockBack, 255, Projectile.owner);
            }
        }

        private void Bite()
        {
            if (Projectile.frame < 9)
            {
                Projectile.frame = 9;
                Projectile.frameCounter = 0;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.HornetDust>());
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.velocity.Y == 0)
            {
                Bite();
            }
            if (target.life <= 0 && (Main.rand.Next(10) == 0 && !LobotomyGlobalNPC.HornetTarget(target)))
            {
                LobotomyGlobalNPC.SpawnHornet(target, Projectile.owner, Projectile.originalDamage, Projectile.knockBack);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.player[Projectile.owner].HasBuff<Buffs.BoostSpore>())
            {
                modifiers.FinalDamage *= 0.2f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        private bool FallThroughPlatforms = false;
        private void FallThrough()
        {
            FallThroughPlatforms = true;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            Player player = Main.player[Projectile.owner];
            if (FallThroughPlatforms)
            {
                fallThrough = true;
                FallThroughPlatforms = false;
            }
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = IntestineTex.Value;
            Vector2 intPos = IntestineAttachPoint();
            Vector2 position = intPos + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Vector2 origin = new Vector2(Projectile.spriteDirection >= 0 ? 17 : 3, 3);
            float rotation = (IntestinePhysics - intPos).ToRotation() + (Projectile.spriteDirection >= 0 ? 3.14f : 0);
            Main.EntitySpriteDraw(tex, position, null, lightColor, rotation, origin, Projectile.scale, Projectile.spriteDirection >= 0 ? 0 : SpriteEffects.FlipHorizontally, 0);

            tex = TextureAssets.Projectile[Projectile.type].Value;
            position = new Vector2(Projectile.Center.X, Projectile.Center.Y + Projectile.gfxOffY) - Main.screenPosition;
            Rectangle frame = Terraria.Utils.Frame(tex, 1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            origin = frame.Size() / 2;

            Main.EntitySpriteDraw(tex, position, frame, lightColor, Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection >= 0 ? 0 : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }

    class AngryWorkerBee : ModProjectile
    {
        public override string Texture { get { return "LobotomyCorp/Projectiles/WorkerBee"; } }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 13;
            //Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 42;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        private Vector2 IntestinePhysics = Vector2.Zero;

        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[0]];
            //CheckActive(player);
            bool moveLeft = false;
            bool moveRight = false;

            if (player.Center.X < Projectile.Center.X)
            {
                moveLeft = true;
            }
            else if (player.Center.X > Projectile.Center.X)
            {
                moveRight = true;
            }

            Projectile.rotation = 0f;
            Projectile.tileCollide = true;
            float speed = 0.08f;
            float maxAccel = 9f;

            if (moveLeft)
            {
                if ((double)Projectile.velocity.X > -3.5)
                    Projectile.velocity.X -= speed;
                Projectile.velocity.X -= speed;
            }
            else if (moveRight)
            {
                if ((double)Projectile.velocity.X < 3.5)
                    Projectile.velocity.X += speed;
                Projectile.velocity.X += speed;
            }
            else
                {
                    Projectile.velocity.X *= 0.8f;
                    if (Projectile.velocity.X >= -speed && Projectile.velocity.X <= speed)
                        Projectile.velocity.X = 0;
                }
                bool collide = false;
                if (moveLeft || moveRight)
                {
                    int x = (int)((Projectile.Center.X + Projectile.velocity.X) / 16);
                    int y = (int)(Projectile.Center.Y / 16);
                    if (moveLeft)
                        --x;
                    if (moveRight)
                        ++x;
                    //x += (int)Projectile.velocity.X/16;
                    if (WorldGen.SolidTile(x, y))
                        collide = true;
                }
                bool pBelow = player.Center.Y > Projectile.position.Y + Projectile.height;
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY, 1, false);
                if (Projectile.velocity.Y == 0f)
                {
                    int dir = 0;
                    bool jump = false;
                    if (moveLeft)
                        dir = -1;
                    if (moveRight)
                        dir = 1;
                    bool targetAbove = Projectile.position.Y + Projectile.height > player.position.Y + player.height;
                    if (targetAbove && (moveLeft || moveRight) && dir == (int)Math.Sign(Projectile.velocity.X))
                    {
                        int y = (int)((Projectile.position.Y + Projectile.height + 8) / 16);
                        int x = (int)((Projectile.Center.X + (Projectile.width / 2 + 1) * dir) / 16);
                        if (!Main.tile[x, y].HasTile || !Main.tileSolid[Main.tile[x, y].TileType])
                        {
                            Projectile.velocity.Y -= 8f;
                            jump = true;
                        }
                    }
                    else if (targetAbove)
                    {
                        Vector2 delta = player.Center - Projectile.Center;
                        if (Projectile.position.X > player.position.X - 32 && Projectile.position.X + Projectile.width < player.position.X + player.width + 32)
                        {
                            float time = Math.Abs(delta.Y / 2);
                        if (time == 0)
                            time = 1;
                            Projectile.velocity.Y = (Math.Abs(delta.Y) - 0.5f * 0.4f * time * time) / time;
                            if (Projectile.velocity.Y < -12f)
                                Projectile.velocity.Y = -12f;
                            Bite();
                            jump = true;
                        }
                    }
                    if (!pBelow && (Projectile.velocity.X != 0f))
                    {
                    /*
                        int i = (int)(Projectile.Center.X) / 16;
                        int j = (int)(Projectile.Center.Y) / 16 + 1;
                        if (moveLeft)
                            --i;
                        if (moveRight)
                            ++i;
                        WorldGen.SolidTile(i, j);*/
                        FallThrough();
                    }
                    if (collide && jump)
                    {
                        int i = (int)Projectile.Center.X / 16;
                        int j = (int)(Projectile.position.Y + Projectile.height) / 16 + 1;
                        if (WorldGen.SolidTile(i, j) || Main.tile[i, j].IsHalfBlock || ((int)Main.tile[i, j].Slope > 0))
                        {
                            try
                            {
                                j--;
                                if (moveLeft)
                                    i--;
                                if (moveRight)
                                    i--;
                                i += (int)Projectile.velocity.X;
                                if (!WorldGen.SolidTile(i, j - 1) && !WorldGen.SolidTile(i, j - 2))
                                    Projectile.velocity.Y = -7f;
                                else if (!WorldGen.SolidTile(i, j - 2))
                                    Projectile.velocity.Y = -9f;
                                else if (WorldGen.SolidTile(i, j - 5))
                                    Projectile.velocity.Y = -14f;
                                else if (WorldGen.SolidTile(i, j - 4))
                                    Projectile.velocity.Y = -13f;
                                else
                                    Projectile.velocity.Y = -11f;
                            }
                            catch
                            {
                                Projectile.velocity.Y = -9.1f;
                            }
                        }
                    }

                    if (Projectile.velocity.X > maxAccel)
                        Projectile.velocity.X = maxAccel;
                    if (Projectile.velocity.X < -maxAccel)
                        Projectile.velocity.X = -maxAccel;

                    if (Projectile.velocity.X < 0.0)
                        Projectile.direction = -1;
                    if (Projectile.velocity.X > 0.0)
                        Projectile.direction = 1;
                    if (Projectile.velocity.X > speed & moveRight)
                        Projectile.direction = 1;
                    if (Projectile.velocity.X < -speed & moveLeft)
                        Projectile.direction = -1;

                    if (moveLeft || moveRight)
                    {
                        Projectile.spriteDirection = Projectile.direction;
                    }
                    else
                    {
                        Projectile.spriteDirection = player.direction;
                    }
                }
                else
                {

                }

                if (Projectile.frame > 8)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 3)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame > 12)
                        {
                            if (Projectile.velocity.Y == 0)
                                Projectile.frame = 0;
                            else
                                Projectile.frame = 8;
                        }
                    }
                }
                else if (Projectile.velocity.Y == 0)
                {
                    if (Projectile.velocity.X == 0)
                    {
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 5f)
                        {
                            Projectile.frameCounter = 0;
                            Projectile.frame++;
                            if (Projectile.frame > 3)
                                Projectile.frame = 0;
                        }
                    }
                    else if (Projectile.velocity.X < -0.8f || Projectile.velocity.X > 0.8f)
                    {
                        Projectile.frameCounter = Projectile.frameCounter + (int)(Math.Abs(Projectile.velocity.X) / 2f);
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 12)
                        {
                            Projectile.frame++;
                            Projectile.frameCounter = 0;
                        }
                        if (Projectile.frame < 4 || Projectile.frame > 7)
                            Projectile.frame = 4;
                    }
                    else
                    {
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 12f)
                        {
                            Projectile.frameCounter = 0;
                            Projectile.frame++;
                            if (Projectile.frame > 3)
                                Projectile.frame = 0;
                        }
                    }
                }
                else
                {
                    if (Projectile.frame < 8)
                    {
                        Projectile.frame = 8;
                        Projectile.frameCounter = 0;
                    }
                }

            if (Projectile.wet)
            {
                Projectile.Kill();
            }
            else
                Projectile.velocity.Y += 0.4f;
            if (Projectile.velocity.Y > 10f)
                Projectile.velocity.Y = 10f;
            
            //Intestine Physics
            if (IntestinePhysics == Vector2.Zero)
            {
                IntestinePhysics = Projectile.Center + new Vector2(0, 20);
            }

            int intestineLength = 20 - 3;
            Vector2 startPoint = IntestineAttachPoint();

            //Gravity Force
            IntestinePhysics.Y += 0.4f;

            //Reset
            Vector2 delt = (IntestinePhysics - startPoint);
            if (delt.Length() > intestineLength)
            {
                delt.Normalize();
                delt *= intestineLength;
                IntestinePhysics = startPoint + delt;
            }
        }

        /*public override void Kill(int timeLeft)
        {
            Main.NewText(Projectile.position.X);
        }*/

        private Vector2 IntestineAttachPoint()
        {
            Vector2 IntestineAttachPoint = new Vector2(Projectile.Center.X - 15 * Projectile.spriteDirection, Projectile.position.Y + Projectile.height - 13);
            if (Projectile.frame == 2 || Projectile.frame == 3)
                IntestineAttachPoint.X -= 2 * Projectile.spriteDirection;
            else if (Projectile.frame == 5)
                IntestineAttachPoint.Y -= 2;
            else if (Projectile.frame == 7)
                IntestineAttachPoint.X += 2 * Projectile.spriteDirection;
            else if (Projectile.frame >= 8)
            {
                IntestineAttachPoint.Y += 4;
                IntestineAttachPoint.X += 2 * Projectile.spriteDirection;
            }

            return IntestineAttachPoint;
        }

        private void CheckActive(Player p)
        {
            if (!p.active || p.dead)
                Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Misc.Dusts.HornetDust>());
            }
        }

        private void Bite()
        {
            if (Projectile.frame < 9)
            {
                Projectile.frame = 9;
                Projectile.frameCounter = 0;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.velocity.Y == 0)
            {
                Bite();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        private bool FallThroughPlatforms = false;
        private void FallThrough()
        {
            FallThroughPlatforms = true;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;    
            if (FallThroughPlatforms)
            {
                fallThrough = true;
                FallThroughPlatforms = false;
            }
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = WorkerBee.IntestineTex.Value;
            Vector2 intPos = IntestineAttachPoint();
            Vector2 position = intPos + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Vector2 origin = new Vector2(Projectile.spriteDirection >= 0 ? 17 : 3, 3);
            float rotation = (IntestinePhysics - intPos).ToRotation() + (Projectile.spriteDirection >= 0 ? 3.14f : 0);
            Main.EntitySpriteDraw(tex, position, null, lightColor, rotation, origin, Projectile.scale, Projectile.spriteDirection >= 0 ? 0 : SpriteEffects.FlipHorizontally, 0);

            tex = TextureAssets.Projectile[Projectile.type].Value;
            position = new Vector2(Projectile.Center.X, Projectile.Center.Y + Projectile.gfxOffY) - Main.screenPosition;
            Rectangle frame = Terraria.Utils.Frame(tex, 1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            origin = frame.Size() / 2;

            Main.EntitySpriteDraw(tex, position, frame, lightColor, Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection >= 0 ? 0 : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}
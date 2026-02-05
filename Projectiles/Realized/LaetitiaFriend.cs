using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Util;
using Terraria.Audio;
using Terraria.GameContent;
using System.IO;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace LobotomyCorp.Projectiles.Realized
{
    public class LaetitiaFriend : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Friend!");
        }

        public SkeletonBase FSkel;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 1000;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;

        }

        private Vector2[] legLocation;
        private float[] legLerp;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.Center.X < player.Center.X)
                Projectile.velocity.X += 0.05f;
            else if (Projectile.Center.X > player.Center.X)
                Projectile.velocity.X -= 0.05f;

            if (Projectile.velocity.X > 1f)
                Projectile.velocity.X = 1f;
            if (Projectile.velocity.X < -1f)
                Projectile.velocity.X = -1f;

            Projectile.direction = Math.Sign(Projectile.velocity.X);

            if (FSkel == null)
            {
                InitializeSkeleton();
                legLocation = new Vector2[4];
                legLerp = new float[4];
                legLocation[0] = FSkel.BoneName[Bone.FRONTLEFTLEGIK].GetPosition();
                legLocation[1] = FSkel.BoneName[Bone.FRONTRIGHTLEGIK].GetPosition();
                legLocation[2] = FSkel.BoneName[Bone.BACKLEFTLEGIK].GetPosition();
                legLocation[3] = FSkel.BoneName[Bone.BACKRIGHTLEGIK].GetPosition();
            }

            FSkel.BoneName[Bone.HEAD].ChangeOffset(Projectile.Center + (1 * (float)Math.Sin(6.18f * Projectile.localAI[0]++ / 60) * Vector2.UnitY));

            //IK Stuff
            //FrontLeft
            /*
            if (FSkel.BoneName[HEAD].GetPosition().Distance(legLocation[0]) > FSkel.TotalLength(new int[2] { FRONTLEFTLEG, FRONTLEFTLEG2 }))
            {
                int x = (int)(Projectile.Center.X / 16) - 2; int y = (int)(Projectile.Center.Y / 16);

                for (int i = 0; i < 5; i++)
                {
                    bool changeTo = false;
                    for (int j = 0; j < 5; j++)
                    {
                        int x2 = x - i; int y2 = y + j;
                        Tile t = Main.tile[x2, y2];
                        if ((Main.tileSolid[t.TileType] || Main.tileSolidTop[t.TileType]))
                        {
                            changeTo = true;
                            break;
                        }
                    }
                    if (changeTo)
                        break;
                }
            }*/
            SwitchLegLocation(0, Bone.FRONTLEFTLEG, Bone.FRONTLEFTLEG2);
            SwitchLegLocation(1, Bone.FRONTRIGHTLEG, Bone.FRONTRIGHTLEG2);
            SwitchLegLocation(2, Bone.BACKLEFTLEG, Bone.BACKLEFTLEG2);
            SwitchLegLocation(3, Bone.BACKRIGHTLEG, Bone.BACKRIGHTLEG2);

            for (int i = 0; i < 4; i++)
            {
                if (legLerp[i] >= 0)
                {
                    BonePart bone = FSkel.BoneName[(Bone)4 + i * 3];
                    Vector2 moveTo = Vector2.Lerp(bone.GetPosition(), legLocation[i], 0.3f);
                    moveTo.Y = moveTo.Y - (12f * (float)Math.Sin(legLerp[i] * 3.14f));
                    bone.ChangeOffset(moveTo);
                    //.NewText(legLerp[i]);
                    legLerp[i] -= 0.2f;
                    if (legLerp[i] == 0)
                    {
                        legLerp[i] = -1;
                    }
                    if (legLerp[i] < 0)
                    {
                        legLerp[i] = 0;
                    }
                }
            }

            //FSkel.BoneName[FRONTLEFTLEGIK].ChangeOffset(legLocation[0], 12);
            FSkel.RotationIK(Bone.FRONTLEFTLEG, Bone.FRONTLEFTLEG2, Bone.FRONTLEFTLEGIK);
            //Dust.NewDustPerfect(FSkel.BoneName[FRONTLEFTLEGIK].EndPoint(), 6, Vector2.Zero).noGravity = true;

            //FSkel.BoneName[FRONTRIGHTLEGIK].ChangeOffset(legLocation[1], 12);
            FSkel.RotationIK(Bone.FRONTRIGHTLEG, Bone.FRONTRIGHTLEG2, Bone.FRONTRIGHTLEGIK, -1);

            //FSkel.BoneName[BACKLEFTLEGIK].ChangeOffset(legLocation[2], 12);
            FSkel.RotationIK(Bone.BACKLEFTLEG, Bone.BACKLEFTLEG2, Bone.BACKLEFTLEGIK);

            //FSkel.BoneName[BACKRIGHTLEGIK].ChangeOffset(legLocation[3], 12);
            FSkel.RotationIK(Bone.BACKRIGHTLEG, Bone.BACKRIGHTLEG, Bone.BACKRIGHTLEG2, -1);
        }

        private void SwitchLegLocation(int legLoc, Bone bone1, Bone bone2)
        {
            if (FSkel.BoneName[bone1].GetPosition().Distance(legLocation[legLoc]) > FSkel.TotalLength(new Enum[2] { bone1, bone2 }) ||
                (legLoc % 2 == 0 && legLocation[legLoc].X > Projectile.Center.X - 32) ||
                (legLoc % 2 == 1 && legLocation[legLoc].X < Projectile.Center.X + 32))
            {
                int x = (int)(Projectile.Center.X / 16); int y = (int)(Projectile.Center.Y / 16);
                if (legLoc == 0)
                    x -= 1;
                else if (legLoc == 1)
                    x += 4;
                else if (legLoc == 2)
                    x -= 2;
                else if (legLoc == 3)
                    x += 3;

                for (int i = 0; i < 3; i++)
                {
                    bool changeTo = false;
                    for (int j = 0; j < 5; j++)
                    {
                        int i2 = i;
                        if (legLoc % 2 == 0 && Projectile.direction < 0)
                            i2 = 3 - i;
                        if (legLoc % 2 == 1 && Projectile.direction > 0)
                            i2 = 3 - i;

                        int x2 = x + (legLoc % 2 == 0 ? -i2 : i2); int y2 = y + j;
                        Tile t = Main.tile[x2, y2];
                        if (t.HasTile && (Main.tileSolid[t.TileType] || Main.tileSolidTop[t.TileType]))
                        {
                            legLerp[legLoc] = 1f;
                            legLocation[legLoc] = new Vector2(x2 * 16 - 8, y2 * 16);
                            changeTo = true;
                            break;
                        }
                    }
                    if (changeTo)
                        break;
                }
            }
        }

        private void InitializeSkeleton()
        {
            Dictionary<Enum, BonePart> list = new Dictionary<Enum, BonePart>();
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            list.Add(Bone.HEAD, new BonePart(Projectile.Center, 0, 1f, 1)
                    .SetDraw(tex, 
                    new Rectangle(0, 0, 48, 50), 
                    new Vector2(24, 26)));
            list.Add(Bone.FRONTMISCLEG, new BonePart(new Vector2(14, 15), 0f, 1, 1, list[Bone.HEAD]).
                    SetDraw(tex,
                    new Rectangle(0, 52, 20, 24),
                    new Vector2(2, 2)).
                    ChangeIRotation(false));

            list.Add(Bone.FRONTLEFTLEG, new BonePart(new Vector2(-19, 12), -2.35f, 1, 38, list[Bone.HEAD])
                   .SetDraw(tex,
                   new Rectangle(30, 52, 28, 28),
                   new Vector2(27, 27), MathHelper.ToRadians(135)).
                    ChangeIRotation(false));
            list.Add(Bone.FRONTLEFTLEG2, new BonePart(Vector2.Zero, 1.57f, 1, 30, list[Bone.FRONTLEFTLEG])
                   .SetDraw(tex,
                   new Rectangle(116, 0, 8, 50),
                   new Vector2(5, 19), -MathHelper.ToRadians(90)).
                    ChangeIRotation(false));
            list.Add(Bone.FRONTLEFTLEGIK, new BonePart(list[Bone.FRONTLEFTLEG2].EndPoint(), 0, 1, 0));

            list.Add(Bone.FRONTRIGHTLEG, new BonePart(new Vector2(19, 12), MathHelper.ToRadians(-45), 1, 49, list[Bone.HEAD])
                   .SetDraw(tex,
                   new Rectangle(30, 82, 38, 38),
                   new Vector2(3, 35), MathHelper.ToRadians(45)).
                    ChangeIRotation(false));
            list.Add(Bone.FRONTRIGHTLEG2, new BonePart(Vector2.Zero, 1.57f, 1, 30, list[Bone.FRONTRIGHTLEG])
                   .SetDraw(tex,
                   new Rectangle(76, 0, 24, 70),
                   new Vector2(14, 21), -MathHelper.ToRadians(90)).
                    ChangeIRotation(false));
            list.Add(Bone.FRONTRIGHTLEGIK, new BonePart(list[Bone.FRONTRIGHTLEG2].EndPoint(), 1.57f, 1, 0));

            list.Add(Bone.BACKLEFTLEG, new BonePart(new Vector2(-19, -9), -2.35f, 1, 52, list[Bone.HEAD])
                   .SetDraw(tex,
                   new Rectangle(22, 52, 6, 58),
                   new Vector2(3, 55), MathHelper.ToRadians(90)).
                    ChangeIRotation(false));
            list.Add(Bone.BACKLEFTLEG2, new BonePart(Vector2.Zero, 1.57f, 1, 40, list[Bone.BACKLEFTLEG])
                   .SetDraw(tex,
                   new Rectangle(102, 0, 12, 54),
                   new Vector2(6, 16), MathHelper.ToRadians(-90)).
                    ChangeIRotation(false));
            list.Add(Bone.BACKLEFTLEGIK, new BonePart(list[Bone.BACKLEFTLEG2].EndPoint(), 1.57f, 1, 0));

            list.Add(Bone.BACKRIGHTLEG, new BonePart(new Vector2(19, -9), MathHelper.ToRadians(45), 1, 27, list[Bone.HEAD])
                   .SetDraw(tex,
                   new Rectangle(22, 52, 6, 30),
                   new Vector2(3, 29), MathHelper.ToRadians(90)).
                    ChangeIRotation(false));
            list.Add(Bone.BACKRIGHTLEG2, new BonePart(Vector2.Zero, 1.57f, 1, 40, list[Bone.BACKRIGHTLEG])
                   .SetDraw(tex,
                   new Rectangle(50, 0, 24, 48),
                   new Vector2(2, 7), MathHelper.ToRadians(-90)).
                    ChangeIRotation(false));
            list.Add(Bone.BACKRIGHTLEGIK, new BonePart(list[Bone.BACKRIGHTLEG2].EndPoint(), 1.57f, 1, 0));

            FSkel = new SkeletonBase(list);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (FSkel == null)
                return false;

            List<DrawData> list =
            [
                FSkel.BoneName[Bone.BACKLEFTLEG].DrawBone(lightColor),
                FSkel.BoneName[Bone.BACKRIGHTLEG].DrawBone(lightColor),
                FSkel.BoneName[Bone.BACKLEFTLEG2].DrawBone(lightColor),
                FSkel.BoneName[Bone.BACKRIGHTLEG2].DrawBone(lightColor),
                FSkel.BoneName[Bone.FRONTLEFTLEG].DrawBone(lightColor),
                FSkel.BoneName[Bone.FRONTRIGHTLEG].DrawBone(lightColor),
                FSkel.BoneName[Bone.FRONTLEFTLEG2].DrawBone(lightColor),
                FSkel.BoneName[Bone.FRONTRIGHTLEG2].DrawBone(lightColor),
                FSkel.BoneName[Bone.FRONTMISCLEG].DrawBone(lightColor),
                FSkel.BoneName[Bone.HEAD].DrawBone(lightColor),
            ];
            foreach (DrawData data in list)
            {
                Main.EntitySpriteDraw(data);
            }

            return false;
        }

        enum Bone
        {
            HEAD,
            FRONTMISCLEG,
            FRONTLEFTLEG,
            FRONTLEFTLEG2,
            FRONTLEFTLEGIK,
            FRONTRIGHTLEG,
            FRONTRIGHTLEG2,
            FRONTRIGHTLEGIK,
            BACKLEFTLEG,
            BACKLEFTLEG2,
            BACKLEFTLEGIK,
            BACKRIGHTLEG,
            BACKRIGHTLEG2,
            BACKRIGHTLEGIK
        }

        /*Friend Dimensions
         * Body             -    48,  50
         * FrontMiscLeg     -    20,  24    origin  -    2,  2  draggin along
         * 
         * FrontLeftLeg     -    28,  28    origin  -   27, 27  length  -   38
         * FrontLeftLeg2    -     8,  50    origin  -    5, 19  length  -   30
         * 
         * FrontRightLeg    -    38,  38    origin  -    3, 35  length  -   49
         * FrontRightLeg2   -    24,  70    origin  -   14, 21  length  -   48
         * 
         * BackLegs         -     6,  58    origin  -    3, 55  
         * BackRightLeg2    -    24,  48    origin  -    2,  7  length  -   40
         * 
         * BackLeftLeg2     -    12,  54    origin  -    6, 16  length  -   38
         */
    }
}

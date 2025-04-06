using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class HomingInstinctBlock : ModProjectile
	{
		public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
        }    
            
        public override void AI() {
            if (Main.rand.Next(30) == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 57);
            }
            int x = (int)(Projectile.Center.X / 16), y = (int)(Projectile.Center.Y / 16);
            if (Projectile.ai[0] == 0)
            {
                WorldGen.PlaceTile(x, y, ModContent.TileType<Tiles.YellowBrickRoad>(), true);
                //if (Main.tile[x - 1, y + 1].HasTile() || Main.tile[x - 1, y - 1].HasTile() || Main.tile[x + 1, y + 1].HasTile() || Main.tile[x + 1, y - 1].HasTile())
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if ((x == 0 || j == 0))
                            continue;
                        if (Projectile.velocity.Y != 0 && YellowBrick(x + i, y + j) && !YellowBrickAdjacent(x + i, y + j))
                        {
                            WorldGen.PoundPlatform(x + i, y + j);
                            if (((i == 1 && j == 1) || (i == -1 && j == -1)) && YellowBrickAboveBelow(x + i, y + j))
                                WorldGen.PoundPlatform(x + i, y + j);
                        }
                    }
                }
                Projectile.ai[0]++;
            }

            if (!Main.tile[x,y].HasTile || Main.tile[x,y].TileType != ModContent.TileType<Tiles.YellowBrickRoad>())
            {
                Projectile.Kill();
            }

            if (Projectile.timeLeft == 1)
            {
                WorldGen.KillTile(x, y, false, true, true);
            }
        }

        public override void OnKill(int timeLeft)
        {
            
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        private bool YellowBrick(int x, int y)
        {
            return (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<Tiles.YellowBrickRoad>());
        }
        
        private bool YellowBrickAdjacent(int x, int y)
        {
            return (Main.tile[x - 1, y].HasTile && Main.tile[x - 1, y].TileType == ModContent.TileType<Tiles.YellowBrickRoad>() ||
                    Main.tile[x + 1, y].HasTile && Main.tile[x + 1, y].TileType == ModContent.TileType<Tiles.YellowBrickRoad>());
        }

        private bool YellowBrickAboveBelow(int x, int y)
        {
            return (Main.tile[x, y - 1].HasTile && Main.tile[x, y - 1].TileType == ModContent.TileType<Tiles.YellowBrickRoad>() ||
                    Main.tile[x, y + 1].HasTile && Main.tile[x, y + 1].TileType == ModContent.TileType<Tiles.YellowBrickRoad>());
        }
    }
}

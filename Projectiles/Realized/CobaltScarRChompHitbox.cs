using LobotomyCorp.Items;
using LobotomyCorp.Buffs;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;
using static Terraria.Player;

//Projectile.ai[0] Checks if it's actually grabbing something
//Projectile.ai[1] is just the ai target
//Projectile.ai[2] 

namespace LobotomyCorp.Projectiles.Realized
{
    public class CobaltScarRChompHitbox: ModProjectile  //The box that does damage
    {
        public override string Texture => "LobotomyCorp/Projectiles/Realized/CobaltScarRBiteHitbox"; //Honestly, this bit is useless other then to get it to not crash.


        public override void SetDefaults()
        {
            Projectile.height = 50;//Hitbox is 48 by 12*16, or 192. 
            Projectile.width = 230;//Fuck it, being inside the wolf head counts as a hit, if you are in the middle of it, roughly player height.
            Projectile.aiStyle = -1; //I want this to be anchored to the player. Flip the image every time it switches between pointing left and right.
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 7;//expires on the same tick the grab should normally.
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.timeLeft;


        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            owner.heldProj = Projectile.whoAmI;
            float rotation = Projectile.velocity.ToRotation();
            float radius = 7.1875f*16f; //Placeholder Value

            Projectile.Opacity = 0f; //Playtesting Purposes. Normally this should be 0f in normal gameplay. 






            if (Main.myPlayer == Projectile.owner)
                {
                    float targetRotation = (Main.MouseWorld - owner.Center).ToRotation(); 
                    rotation = Terraria.Utils.AngleLerp(rotation, targetRotation, .2f);
                    Projectile.velocity = new Vector2(1, 0).RotatedBy(rotation);
                }

            Projectile.Center = owner.Center + Projectile.velocity * radius;

          
            
            LobCorpLight.LobItemFrame(owner, rotation, Projectile.spriteDirection);
            Projectile.rotation = rotation;
            Projectile.spriteDirection = owner.direction; //This makes the art rotate and flip, but I

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Player player = Main.player[Projectile.owner];
            int Biteindex = player.FindBuffIndex(ModContent.BuffType<Buffs.CobaltScarRecentBite>());
            if (target.boss || NPCID.Sets.ShouldBeCountedAsBoss[target.type] == true || (target.type != NPCID.Worm && target.realLife == -1))
            {
                player.AddBuff(ModContent.BuffType<CobaltScarRecentBite>(), 240); 
                //What happens if it bites a boss,pillar or an enitity that is NOT a worm and a part of a non worm boss.
            }
            else if (Biteindex == -1) //Checks if the player has recently hit an enemy with a bite.
            {
                player.AddBuff(ModContent.BuffType<CobaltScarRecentBite>(), 180); 
                // Read CobaltScarR in items, I modified it to not add points if you didn't hit a boss.
            }



        }



       public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			float rotationFactor = Projectile.rotation; // The rotation of the Jousting Lance.
			float scaleFactor = 278f; // Width/or how far it points away from the player. I know this is a long stick but it's pve and it's meant to prevent completely bs moments.
			float widthMultiplier = 50f; 
			float collisionPoint = 0f; // collisionPoint is needed for CheckAABBvLineCollision(), but it isn't used for our collision here. Keep it at 0f.
            Player owner = Main.player[Projectile.owner];
            owner.heldProj = Projectile.whoAmI;
			
			Rectangle lanceHitboxBounds = new Rectangle(0, 0, 556, 556);

			
			lanceHitboxBounds.X = (int)Projectile.position.X - lanceHitboxBounds.Width / 2;
			lanceHitboxBounds.Y = (int)Projectile.position.Y - lanceHitboxBounds.Height / 2;

			// This is the back of the hit-line with owner.Center being the tip of the Jousting Lance. (Player.Center is the fucking start, not the pointy end)
			Vector2 hitLineEnd = owner.Center + rotationFactor.ToRotationVector2() * scaleFactor;

			// The following is for debugging the size of the hit line. This will allow you to easily see where it starts and ends.
			//Dust.NewDustPerfect(owner.Center, DustID.Pixie, Velocity: Vector2.Zero, Scale: 0.5f);
			//Dust.NewDustPerfect(hitLineEnd, DustID.Pixie, Velocity: Vector2.Zero, Scale: 0.5f);

			// First check that our large rectangle intersects with the target hitbox.
			// Then we check to see if a line from the tip of the Jousting Lance to the "end" of the lance intersects with the target hitbox.
			if (lanceHitboxBounds.Intersects(targetHitbox)
				&& Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), owner.Center, hitLineEnd, widthMultiplier * Projectile.scale, ref collisionPoint)) {
				return true;
			}
			return false;
		}



    }
}

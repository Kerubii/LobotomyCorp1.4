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
    public class CobaltScarRBiteHitbox: ModProjectile  //The box that Grabs.
    {
        public int Timer = 0; //How long the Projectile has existed in ticks.

        public override void SetDefaults()
        {
            Projectile.height = 50;//Hitbox is 48 by 12*16, or 192. 
            Projectile.width = 230;//Fuck it, being inside the wolf head counts as a hit, if you are in the middle of it, roughly player height.
            Projectile.aiStyle = -1; //I want this to be anchored to the player. Flip the image every time it switches between pointing left and right.
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;//8 frames of vaild hits, though.
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
            Timer += 1;





            if (Main.myPlayer == Projectile.owner)
                {
                    float targetRotation = (Main.MouseWorld - owner.Center).ToRotation(); 
                    rotation = Terraria.Utils.AngleLerp(rotation, targetRotation, .2f);
                    Projectile.velocity = new Vector2(1, 0).RotatedBy(rotation);
                }

            Projectile.Center = owner.Center + Projectile.velocity * radius;

            if(Projectile.ai[1] > 0)//If the NPC hit is not a boss, dummy or invincible
            {
                NPC target = Main.npc[(int)Projectile.ai[1] - 1];
                //If the target survives the initial grab damage,and the player isn't grabbing. This exists so you don't just crash the game for grabbing an enemy that doesn't exist.
                if (target.life > 0 ){
                    if(Projectile.ai[0] != 1)
                    {
                        owner.AddBuff(ModContent.BuffType<CobaltScarConsuming>(), 360); 
                        owner.GetModPlayer<LobotomyWawPlayer>().CobaltScarConsumingDamageTaken = 0; //sets it to 0.
                        Projectile.ai[0] = 1;//tell the projectile we are grabbing something, so it doesn't kill the projectile.
                    target.dontTakeDamage = true;
                    }
                  owner.GetModPlayer<LobotomyModPlayer>().GrabEnemySetWhere(target,Vector2.Zero);//Grab the mofo
            
                }

                if (owner.HasBuff<Buffs.CobaltScarFailedConsume>())
                  {
                  //
                  //the logic for failedconsume removing consuming and resetting CSConsumingDamageTaken to 0 in this outcome is in the consuming buff
                  target.dontTakeDamage = false;
                  target.life -= (int)(target.lifeMax * 0.25f); //game accurate amirite
                  Projectile.Kill(); 
				  return;
                    
                  }
                  if (!owner.HasBuff<Buffs.CobaltScarFailedConsume>() && !owner.HasBuff<Buffs.CobaltScarConsuming>())
                  {
                    //kill the target
                  owner.GetModPlayer<LobotomyWawPlayer>().CobaltScarConsumingDamageTaken = 0; //Security in ensuring that this number is 0 if you don't have the buff
                  target.dontTakeDamage = false;
                  target.life -= target.lifeMax ;
                  Projectile.Kill(); 
				  return;
                  }
            }
            else
            {}

            if (Projectile.ai[0] != 1 && Timer >= 8) //Projectile.ai[0] != 1 means it's not grabbing. I hope that's how it works.
            {
                Projectile.Kill(); 
				return;
            }
            
            LobCorpLight.LobItemFrame(owner, rotation, Projectile.spriteDirection);
            Projectile.rotation = rotation;
            Projectile.spriteDirection = owner.direction; //This makes the art rotate and flip, but I

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) //the projectile deals 1 damage just to trigger this
        {
            Player player = Main.player[Projectile.owner];
            if(target.realLife == -1 && target.life < 2000 && target.type != NPCID.Worm && target.type != NPCID.TargetDummy && !target.dontTakeDamage && !target.boss && NPCID.Sets.ProjectileNPC[target.type] == false && NPCID.Sets.ShouldBeCountedAsBoss[target.type] == false) 
            {
                Projectile.ai[1] = target.whoAmI + 1; 
            
            }

        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1) //ai[0] == 1 means it's grabbing something
                return false;
            return base.CanHitNPC(target);//while grabbing, it cannot hit anything else.
        }



//the projectile should do two things. One is a Grab(almost done), another is a flat damage dealing option.

       public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			float rotationFactor = Projectile.rotation; // The rotation of the Jousting Lance.
			float scaleFactor = 278f; // Width/or how far it points away from the player. 
			float widthMultiplier = 50f; //How thick/tall the thing is. It's a huge projectile but it only hits once.
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

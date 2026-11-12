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

namespace LobotomyCorp.Projectiles.Realized
{
    public class CobaltScarRBite1: ModProjectile  //or the windup that exists just to do nothing on it's own other then summon 2 projectiles, with one actually doing something.
    {
        public override string Texture => "LobotomyCorp/Projectiles/Realized/CobaltScarRBiteSpriteSheet"; 
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13; //13 different frames. 0-12, 10-12 is when you hit.
        }
        public override void SetDefaults()
        {
            Projectile.height = 200;//Hitbox is 48 by 12*16, or 192. The image is a lot larger then the planned hitbox.
            Projectile.width = 240;//back to the drawing board..........
            Projectile.aiStyle = -1; //I want this to be anchored to the player. Flip the image every time it switches between pointing left and right.
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 26;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;

        
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            owner.heldProj = Projectile.whoAmI;
            float rotation = Projectile.velocity.ToRotation();
            float radius = 8.65f*16f; //take horizonal size of the sheet, add about 28 pixels, then divide by 32 to get the first value.
            //This is kinda longer then i planned, but whatever. 16 tiles.


            Projectile.localAI[0]++;

            if (Projectile.localAI[0] == 8)//3+3+2 (Frame 0+1+2, or the idle to anticipation, about to go into the attack.)
            {
               Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<CobaltScarRBiteHitbox>(), 1, Projectile.knockBack, Projectile.owner, 0, 0, 0);
            }

            if (Projectile.localAI[0] == 10)
            {
                 Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<CobaltScarRChompHitbox>(), Projectile.damage * 5, Projectile.knockBack, Projectile.owner, 0, 0, 0);

            }

            if (Projectile.Opacity < 0) {
				Projectile.Opacity = 0;
			}

            if (Main.myPlayer == Projectile.owner)
                {
                    float targetRotation = (Main.MouseWorld - owner.Center).ToRotation(); 
                    rotation = Terraria.Utils.AngleLerp(rotation, targetRotation, .2f);
                    Projectile.velocity = new Vector2(1, 0).RotatedBy(rotation);
                }

            Projectile.Center = owner.Center + Projectile.velocity * radius;
           
            LobCorpLight.LobItemFrame(owner, rotation, Projectile.spriteDirection);
            Projectile.rotation = rotation;
            Projectile.spriteDirection = owner.direction; 

            Projectile.frameCounter++;

          if (Projectile.frame != 0 && Projectile.frame != 1 && Projectile.frame != 8 && Projectile.frame != 9 && Projectile.frame != 11 &&  Projectile.frame != 12 ){ //make some frames longer or shorter. 0,9 and 12 are longer while 1 is shorter.
             if (Projectile.frameCounter >= 2) { //how long a normal frame plays. The bite (of 97) is especially fast to indicate speed.
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 13; //12
             }
                
            }
            

            if (owner.HasBuff<Buffs.CobaltScarConsuming>() ||owner.HasBuff<Buffs.CobaltScarRecentBite>() && Projectile.frame == 7) //On the 8th frame, check for this shit.
            {
                Projectile.frame = Projectile.frame + 3; //move to 10-12 if true.
            }
            if ((Projectile.frame == 0 || Projectile.frame == 1) && Projectile.frameCounter >= 3) //this for 3 frame plays. The startup is a bit slower so the animation doesn't feel flat
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 13; //+6
            }
            if ((Projectile.frame == 8 || Projectile.frame == 9 || Projectile.frame == 11 || Projectile.frame == 12) && Projectile.frameCounter >= 4) //The ending sequences get 4 frames each so it's actually kinda easy to see expressions.
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 13; //+8
            }
            

        }

        public override bool? CanHitNPC(NPC target)
        {
            return false; //Purely Visual. It will NOT hit NPCs.
        }


public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = tex.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            float rotation = Projectile.rotation;
            SpriteEffects sp = 0;
            
            if (Projectile.spriteDirection == -1)
            {
                sp = SpriteEffects.FlipHorizontally;
                rotation += 3.14f;
            }
            
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, tex.Frame(1, Main.projFrames[Type], 0, Projectile.frame), lightColor, rotation, frame.Size() / 2f, Projectile.scale, sp);


            return false;
        }


        






    }
}


//A lot of this code needs like, shit for me to figure out.
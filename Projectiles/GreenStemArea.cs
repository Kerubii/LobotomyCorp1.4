using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class GreenStemArea : ModProjectile
	{
        public static Asset<Texture2D> Roots;

        public override void Load()
        {
            Roots = ModContent.Request<Texture2D>("LobotomyCorp/Projectiles/GreenStemRoot");
        }

		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Malice");
        }

		public override void SetDefaults() {
			Projectile.width = 120;
			Projectile.height = 120;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
            Projectile.timeLeft = 60;
            
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
            Projectile.hide = true;
		}

		public override void AI() {
			Player projOwner = Main.player[Projectile.owner];
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			Projectile.direction = projOwner.direction;
			Projectile.position.X = ownerMountedCenter.X - (float)(Projectile.width / 2);
            Projectile.position.Y = ownerMountedCenter.Y - (float)(Projectile.height / 2) + projOwner.height / 2;
            if (projOwner.channel && projOwner.HeldItem.type == ModContent.ItemType<Items.Ruina.History.GreenStemR>())
            {
                if (Projectile.ai[0] <= 240f * 9)
                {
                    Projectile.ai[0] += 3;
                    Projectile.rotation += 0.005f;
                    if (Projectile.ai[0] > 240 * 9)
                        Projectile.ai[0] = 240 * 9;
                }

                projOwner.itemAnimation = projOwner.itemAnimationMax;
                projOwner.itemTime = projOwner.itemAnimation;
            }
            else
            {
                Projectile.ai[0] -= 0.25f;
                if (Projectile.ai[0] <= 0)
                {
                    Projectile.Kill();
                    return;
                }
            }
            Projectile.timeLeft = 10;
            Projectile.scale = Projectile.ai[0] / 240f;
            Projectile.localAI[0]+= 2;
            if (Projectile.localAI[0] > 360)
                Projectile.localAI[0] = 0;

            if (Projectile.ai[1] <= 0 && Projectile.ai[1] % 5 == 0)
            {
                //int attack = -1;
                float extraRange = 0;
                float distance = 60f * Projectile.scale + extraRange;

                int[] targets = FindNearest(projOwner, distance);

                /*
                foreach (NPC n in Main.npc)
                {
                    if (n.active && n.chaseable && n.CanBeChasedBy(this))
                    {
                        Vector2 delta = n.Center - ownerMountedCenter;
                        float npcDist = delta.Length() - (n.width > n.height ? n.width / 2 : n.height / 2);
                        if (npcDist < distance)
                        {
                            distance = npcDist;
                            attack = n.whoAmI;
                        }
                    }
                }*/

                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] >= 0)
                    {
                        Vector2 targetCenter = Main.npc[targets[i]].Center;

                        float angle = (targetCenter - Projectile.Center).ToRotation();
                        int intAngle = (int)(angle / 0.392f);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            //for (int i = 0; i < 1; i++)
                            //{
                            float vineAngle;
                            float vineDist;
                            Vector2 vineSpawn;

                            bool valid;
                            int tries = 0;
                            do
                            {
                                vineAngle = (intAngle + Main.rand.Next(-2, 3)) * 0.392f;
                                vineDist = Main.rand.NextFloat(60f * Projectile.scale);
                                vineSpawn = Projectile.Center + new Vector2(vineDist, 0).RotatedBy(vineAngle);
                                valid = (targetCenter - vineSpawn).Length() < 128f;
                                tries++;
                            } while (!(valid || tries > 100));


                            Vector2 vel = Vector2.Normalize(targetCenter - vineSpawn) * 32;

                            int damage = Projectile.damage + (int)(Projectile.damage * 0.5f * (1f - projOwner.statLife / (float)projOwner.statLifeMax2));
                            if (Main.myPlayer == Projectile.owner)
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), vineSpawn, vel, ModContent.ProjectileType<Projectiles.GreenStemVine>(), damage, 0, Projectile.owner, vineDist, vineAngle);

                            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/SnowWhite_NormalAtk") with { Volume = 0.25f, MaxInstances = 12 }, vineSpawn);

                        }
                        if (Projectile.ai[1] == 0)
                            Projectile.ai[1]--;
                    }
                }

                /*
                if (attack >= 0)
                {
                    Vector2 targetCenter = Main.npc[attack].Center;

                    float angle = (targetCenter - Projectile.Center).ToRotation();
                    int intAngle = (int)(angle / 0.392f);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        //for (int i = 0; i < 1; i++)
                        //{
                            float vineAngle;
                            float vineDist;
                            Vector2 vineSpawn;

                            bool valid;
                            int tries = 0;
                            do
                            {
                                vineAngle = (intAngle + Main.rand.Next(-2, 3)) * 0.392f;
                                vineDist = Main.rand.NextFloat(60f * Projectile.scale);
                                vineSpawn = Projectile.Center + new Vector2(vineDist, 0).RotatedBy(vineAngle);
                                valid = (targetCenter - vineSpawn).Length() < 128f;
                                tries++;
                            } while (!(valid || tries > 100));


                            Vector2 vel = Vector2.Normalize(targetCenter - vineSpawn) * 32;

                        int damage = Projectile.damage + (int)(Projectile.damage * 0.5f * (1f - projOwner.statLife/(float)projOwner.statLifeMax2));
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), vineSpawn, vel, ModContent.ProjectileType<Projectiles.GreenStemVine>(), damage, 0, Projectile.owner, vineDist, vineAngle);

                            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/SnowWhite_NormalAtk") with { Volume = 0.25f }, vineSpawn);
                     
                    }*/
            }
            else if (Projectile.ai[1] <= -15)
            {
                Projectile.ai[1] = 30;
            }
            
            if (Projectile.ai[1] != 0)
                Projectile.ai[1]--;
            //Projectile.scale += 0.1f * (float) Math.Sin(MathHelper.ToRadians(Projectile.localAI[0]));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Circle Rendering, ehhhhh
            /*
            float radius = 60f * Projectile.scale;
            float ringLength = radius * 6.28f;
            int ringTexWidth = 20;

            float amount = (ringLength / ringTexWidth);

            Texture2D tex = Mod.Assets.Request<Texture2D>("Projectiles/GreenStemRing").Value;
            Vector2 origin = new Vector2(14, 5);
            for (int i = 0; i <= amount + 5; i++)
            {
                int n = (int)amount + 5 - i;
                float offset = (1 / amount) * 6.28f * (int)Math.Floor(n / 8f);
                float angle = 0.785f * (n % 8) + offset + Projectile.rotation;
                //float angle = 6.28f * ((float)(i / amount) + Projectile.rotation);

                Vector2 pos = Projectile.Center + new Vector2(radius, 0).RotatedBy(angle) - Main.screenPosition;
                float rotation = angle + 1.57f;
                Main.EntitySpriteDraw(tex, pos, null, lightColor, rotation, origin, 1f, 0, 0);
            }*/

            float length = 60f * Projectile.scale;
            int texLength = 20;
            int amount = (int)(length / texLength);

            Texture2D tex = Roots.Value;
            Vector2 origin = new Vector2(4, 10);
            for (int i = amount; i >= 0; i--)
            {
                Color color = lightColor;
                if (i == amount)
                {
                    float alpha = (length - texLength * i) / texLength;
                    color *= alpha;
                }

                int angles = 16;
                for (int n = 0; n < angles; n++)
                {
                    float rotation = (float)n / angles * 6.28f;
                    Vector2 pos = Projectile.Center + new Vector2(i * texLength, 0).RotatedBy(rotation) - Main.screenPosition;
                    
                    Main.EntitySpriteDraw(tex, pos, Terraria.Utils.Frame(tex, 1, 3, 0, (i + n) % 3), color, rotation, origin, 1f, 0, 0);
                }
            }

            return false;
        }

        private int[] FindNearest(Player player, float distance)
        {
            Dictionary<int, int> validTargetList = new Dictionary<int, int>();
            Vector2 compareTo = player.Center;
            foreach (NPC n in Main.npc)
            {
                if (n.active && !n.friendly && !n.dontTakeDamage && n.Center.Distance(compareTo) < distance && n.CanBeChasedBy(this))
                {
                    int localDistance = (int)n.Center.Distance(player.Center);
                    validTargetList.Add(n.whoAmI, localDistance);
                }
            }
            if (validTargetList.Count > 1)
            {
                var SortedList = from x in validTargetList orderby x.Value ascending select x;
                validTargetList = SortedList.ToDictionary(x => x.Key, x => x.Value);
            }
            int[] targets = new int[3] { -1, -1, -1 };
            for (int i = 0; i < targets.Length; i++)
            {
                if (i > validTargetList.Count - 1)
                    targets[i] = -1;
                else
                    targets[i] = validTargetList.ElementAt(i).Key;
            }
            return targets;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPvp(Player target)
        {
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}

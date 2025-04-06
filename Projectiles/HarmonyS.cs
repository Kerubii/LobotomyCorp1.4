using LobotomyCorp.Items.He;
using LobotomyCorp.Players;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class HarmonyS : ModProjectile
	{
		public override void SetDefaults() {
			Projectile.width = 46;
			Projectile.height = 46;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
            

			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;

            SawRotation = 0f;
		}

        private float SawRotation;

		public override void AI() {
            
            Player player = Main.player[Projectile.owner];
            if (player.dead || player.itemAnimation <= 0 || player.ItemTimeIsZero)
            {
                Projectile.Kill();
            }

            Vector2 mountedCenter = player.RotatedRelativePoint(player.MountedCenter, true);

            if (!player.channel) //If Player stops holding m1 slow down the saw and slash infront 
            {
                if (Projectile.ai[0] > 0)
                    Projectile.ai[0] -= 2;
                if (Projectile.ai[0] <= 0)
                    Projectile.ai[0] = 0;

                SawRotation = Projectile.velocity.ToRotation();
                float prog = Projectile.timeLeft / 30f;
                if (prog > .75f)
                {
                    prog = 1f - (prog - .75f) / .25f;
                    SawRotation -= MathHelper.ToRadians(120) * prog * player.direction;
                }
                else
                {
                    prog = 1f - prog / 0.75f;
                    SawRotation -= (MathHelper.ToRadians(120) - MathHelper.ToRadians(300) * (float)Math.Sin(1.57f * prog)) * player.direction;
                }
            }
            else //Sawblade goes faster and faster, on OnHit goes up to 60
            {
                if (Projectile.ai[0] < 40)
                    Projectile.ai[0] += 5f;
                Projectile.timeLeft = 30;
                if (Main.myPlayer == Projectile.owner)//Hold out on direction of cursor
                {
                    Projectile.velocity = Main.MouseWorld - mountedCenter;
                    SawRotation = Projectile.velocity.ToRotation();
                }
            }

            Vector2 center = new Vector2(93, 0).RotatedBy(SawRotation);
            Projectile.Center = mountedCenter + center;

            player.itemAnimation = 2;
            player.itemTime = 2;

            if (player.channel)
                player.direction = center.X >= 0 ? 1 : -1;
            Projectile.direction = player.direction;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction);

            Projectile.rotation += (MathHelper.ToRadians(20) * (Projectile.ai[0] / 60)) * player.direction;
            if (Main.rand.Next((int)(30 - 30 * (Projectile.ai[0] / 60))) == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    float rotRand = Main.rand.NextFloat((float)Math.PI * 2);
                    Vector2 dustPos = new Vector2(23, 0).RotatedBy(rotRand);
                    Vector2 dustVel = new Vector2(0, 4).RotatedBy(rotRand);
                    Dust.NewDustPerfect(Projectile.Center + dustPos, 5, dustVel);
                }
            }

            if (Projectile.localAI[0] <= -15)
            {
                SoundEngine.PlaySound(SoundID.Item22, Projectile.position);
                Projectile.localAI[0] = 45 - 15 * (Projectile.ai[0] / 60);
            }

            Projectile.localAI[0]--;
            Projectile.ai[1]--;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Vector2 origin = new Vector2(Projectile.direction > 1 ? 93 : 23, 23);
            float rot = SawRotation + (Projectile.direction > 1 ? 0 : 3.14f);

            Main.EntitySpriteDraw(tex, position, tex.Frame(), lightColor, rot, origin, Projectile.scale, (SpriteEffects)Projectile.direction, 0);

            tex = Mod.Assets.Request<Texture2D>("Projectiles/HarmonySHead").Value;
            Main.EntitySpriteDraw(tex, position, tex.Frame(), lightColor, Projectile.rotation, origin, Projectile.scale, (SpriteEffects)Projectile.direction, 0);
            tex = Mod.Assets.Request<Texture2D>("Projectiles/HarmonySString").Value;
            Main.EntitySpriteDraw(tex, position, tex.Frame(), lightColor, rot, origin, Projectile.scale, (SpriteEffects)Projectile.direction, 0);

            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach (Player p in Main.ActivePlayers)
            {
                if ((p.whoAmI == Projectile.owner || p.team == Main.player[Projectile.owner].team) && !p.dead)
                {
                    LobotomyHePlayer modPlayer = p.GetModPlayer<LobotomyHePlayer>();
                    modPlayer.HarmonyTime += 90;
                    if (modPlayer.HarmonyTime > 600)
                        modPlayer.HarmonyTime = 600;   
                    p.AddBuff(ModContent.BuffType<Buffs.MusicalAddiction>(), modPlayer.HarmonyTime, true);
                }
            }
            foreach (NPC n in Main.npc)
            {
                if (n.active && !n.friendly && !n.dontTakeDamage && n.Center.Distance(target.Center) < 1000)
                {
                    n.AddBuff(ModContent.BuffType<Buffs.CrookedNotes>(), 300);
                }
            }

            target.immune[Projectile.owner] = 15 - (int)(13 * (Projectile.ai[0] / 60f));
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(target.position, target.width, target.height, 5);
            }
            Vector2 spe = new Vector2(16f, 0).RotatedByRandom(6.28f);
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                int number = Item.NewItem(Projectile.GetSource_DropAsItem(), target.getRect(), ModContent.ItemType<Items.Ruina.Technology.HarmonyNote>(), 1, true, 0);
                Main.item[number].velocity = spe;
                if (Main.netMode == 1)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);
                }
            }

            if (Projectile.localAI[0] <= 0)
            {
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Singing_Atk") with {Volume = 0.5f, PitchVariance = 0.3f}, Projectile.position);
                Projectile.localAI[0] = 60 - 30 * (Projectile.ai[0] / 60);
            }

            if (Projectile.ai[0] > 30 && Projectile.ai[1] <= 0 && Main.player[Projectile.owner].GetModPlayer<LobotomyHePlayer>().HarmonyAddiction)
            {
                Projectile.ai[1] = 15;
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HarmonyBloodEffect>(), Projectile.damage, 0, Projectile.owner, Projectile.whoAmI, Main.rand.NextFloat(0.6f , 0.8f) * Projectile.ai[0] / 60f);
            }

            if (Projectile.ai[0] < 60)
                Projectile.ai[0] += 2f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!Main.player[Projectile.owner].channel)
                modifiers.FinalDamage *= 2f;
        }
    }

    public class HarmonyBloodEffect : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/HarmonyS";

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;

            Projectile.timeLeft = 30;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(6.28f);
                Projectile.spriteDirection = Main.rand.NextBool(2) ? 1 : -1;
                Projectile.localAI[0]++;
            }
            Projectile.rotation += Projectile.ai[1] * Projectile.spriteDirection;

            if (Main.projectile[(int)Projectile.ai[0]].active)
                Projectile.Center = Main.projectile[(int)Projectile.ai[0]].Center;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            float prog = 1f - Projectile.timeLeft / 30f;
            hitbox.Width = (int)(prog * 180);
            hitbox.Height = (int)(prog * 180);
            hitbox.X = (int)Projectile.Center.X - hitbox.Width / 2;
            hitbox.Y = (int)Projectile.Center.Y - hitbox.Height / 2;
            base.ModifyDamageHitbox(ref hitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float prog = 1f - Projectile.timeLeft / 30f;
            CustomShaderData shader = LobotomyCorp.LobcorpShaders["TextureTrail"].UseOpacity(0.5f + 0.5f * (prog));
            shader.UseImage1(Mod, "Misc/BloodTrail");
            shader.UseImage2(Mod, "Misc/BloodTrail");

            SlashTrail trail = new SlashTrail(16, 1.57f);
            trail.color = Color.DarkRed;

            float rot = Projectile.rotation;
            //trail.DrawCircle(Projectile.Center, rot, Projectile.spriteDirection, 90 * prog, 64, shader);
            trail.DrawPartCircle(Projectile.Center, rot, 4.71f, Projectile.spriteDirection, 90 * prog, 48, shader);

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using LobotomyCorp.Buffs;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.Player;
using static tModPorter.ProgressUpdate;

namespace LobotomyCorp.Projectiles.Realized
{
    class SmileRScream : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/SmileShockwave";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 15;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;

            Projectile.ArmorPenetration = 20;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(3.15f);
                Projectile.localAI[0] = 1 + Main.rand.Next(3);
            }

            if (Projectile.ai[2] == 1)
            {
                if (Projectile.ai[0] == 5 && Main.myPlayer == Projectile.owner && Projectile.ai[1] > 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.ai[1] - 1, 1);

                    float random = Main.rand.NextFloat(1.00f);
                    for (int i = 0; i < 28; i++)
                    {
                        Vector2 vel = new Vector2(16, 0).RotatedBy(random + MathHelper.ToRadians((360 / 28f) * i));
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Wraith, vel);
                        d.noGravity = true;
                    }
                }
            }
            else
            {
                if (player.itemAnimation > player.itemAnimationMax * 0.6f)
                {
                    //Projectile.velocity = new Vector2(100, 0).RotatedBy(player.itemRotation - MathHelper.ToRadians(45) + (player.direction < 0 ? -1.57f : 0));
                    //Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity;

                    if (Projectile.ai[0] == 5 && Main.myPlayer == Projectile.owner)
                    {
                        Vector2 offset = new Vector2(120, 0).RotatedBy(player.itemRotation - MathHelper.ToRadians(45) + (player.direction < 0 ? -1.57f : 0));
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center + offset, offset, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner);

                        float random = Main.rand.NextFloat(1.00f);
                        for (int i = 0; i < 28; i++)
                        {
                            Vector2 vel = new Vector2(16, 0).RotatedBy(random + MathHelper.ToRadians((360 / 28f) * i));
                            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Wraith, vel);
                            d.noGravity = true;
                        }
                    }
                }
            }

            Projectile.scale = Projectile.ai[2] - Projectile.ai[2] * (Projectile.timeLeft / 15f);// (float)Math.Sin(1.57f * (1f - (Projectile.timeLeft / 15f)));
            Projectile.alpha = 255 - (int)(255 * (1f - (Projectile.timeLeft / 15f)));
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int hitboxSize = (int)(300 * Projectile.scale);
            hitbox.X += hitbox.Width / 2 - hitboxSize / 2;
            hitbox.Y += hitbox.Height / 2 - hitboxSize / 2;
            hitbox.Width = hitboxSize;
            hitbox.Height = hitboxSize;
            base.ModifyDamageHitbox(ref hitbox);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Scream>(), 300);
            if (Main.myPlayer == Projectile.owner)
            {
                if (target.boss)
                {
                    Main.player[Projectile.owner].GetModPlayer<LobotomyAlephPlayer>().SmileCreateCorpseBoss(target);
                }
                else if(target.life <= 0)
                    Main.player[Projectile.owner].GetModPlayer<LobotomyAlephPlayer>().SmileCreateCorpse(target, true);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = SmileShockwave.SmileShockwaveTex.Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = tex.Size() / 2;

            float scaleFactor = (float)Math.Sin(1.57f * ((Projectile.scale / Projectile.ai[2]) / 0.85f));
            Vector2 scale = new Vector2(scaleFactor, scaleFactor) * 3f;
            switch (Projectile.localAI[0])
            {
                case 1:
                    scale.X *= 0.6f;
                    break;
                case 2:
                    scale.Y *= 0.6f;
                    break;
            }

            Color color = Color.Black;
            color *= 1f - scaleFactor;
            Main.EntitySpriteDraw(tex, position, tex.Frame(), color, Projectile.rotation, origin, scale * Projectile.ai[2], 0, 0);

            return false;
        }
    }
}

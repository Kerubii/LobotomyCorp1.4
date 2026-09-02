using LobotomyCorp.ModSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
    public class FourthMatchFlameSelfExplosion : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/FourthMatchFlameShot";

        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 140;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 2;
            Projectile.alpha = 255;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.rotation += 0.01f;
            Dust dust = new Dust();
            for (int i = 0; i < 15; i++)
            {
                dust = Main.dust[Dust.NewDust(Projectile.Center, 1, 1, DustID.Torch)];
                dust.noGravity = !Main.rand.NextBool(4);
                dust.velocity *= Main.rand.NextFloat(12, 25);
                dust.fadeIn = 2f;
            }
            for (int i = 0; i < 20; i++)
            {
                dust = Main.dust[Dust.NewDust(Projectile.Center, 1, 1, 31, 0, 0, 100, new Color(), 1.5f)];
                dust.velocity *= 1.4f;
            }
            for (int i = 0; i < 10; i++)
            {
                dust = Main.dust[Dust.NewDust(Projectile.Center, 1, 1, 6, 0, 0, 100, new Color(), 2.5f)];
                dust.velocity *= 5f;
                dust.noGravity = !Main.rand.NextBool(4);
                dust = Main.dust[Dust.NewDust(Projectile.Center, 1, 1, 6, 0, 0, 100, new Color(), 1.5f)];
                dust.velocity *= 3f;
            }
            Gore gore = Main.gore[Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(), Main.rand.Next(61, 64), 1f)];
            gore.velocity *= 0.6f;
            gore.velocity.X += 1f;
            gore.velocity.Y += 1f;
            gore = Main.gore[Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(), Main.rand.Next(61, 64), 1f)];
            gore.velocity *= 0.6f;
            gore.velocity.X -= 1f;
            gore.velocity.Y += 1f;
            gore = Main.gore[Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(), Main.rand.Next(61, 64), 1f)];
            gore.velocity *= 0.6f;
            gore.velocity.X += 1f;
            gore.velocity.Y -= 1f;
            gore = Main.gore[Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(), Main.rand.Next(61, 64), 1f)];
            gore.velocity *= 0.6f;
            gore.velocity.X -= 1f;
            gore.velocity.Y -= 1f;
            if (Projectile.owner == Main.myPlayer)
            {
                ModContent.GetInstance<ScreenSystem>().ScreenShake(15, 10f, 0.1f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.statLife -= Projectile.damage;
            if (player.statLife <= 0)
            {
                NetworkText text = NetworkText.FromKey("Mods.LobotomyCorp.DeathMessages.Matchstick", player.name);
                PlayerDeathReason playerDeath = PlayerDeathReason.ByCustomReason(text);
                player.KillMe(playerDeath, Projectile.damage, 1);
            }
            else
            {
                player.immune = true;
                player.immuneTime = 30;
                CombatText.NewText(player.getRect(), CombatText.DamagedFriendly, (int)Projectile.damage);
            }
            //player.statLife = 0;
            //player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " was reduced to ashes..."), 4000, 1);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += 5f; 
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Matchstick>(), 260);
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}

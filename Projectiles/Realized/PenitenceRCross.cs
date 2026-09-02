using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Ruina.Religion;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace LobotomyCorp.Projectiles.Realized
{
    public class PenitenceRCross : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Items/Ruina/Religion/PenitenceR";

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 90;

            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/OneBadManyGood/oneBad_special") with { Volume = 0.2f }, Projectile.Center);
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Entity/OneBadManyGood/oneBad_special_sry_short") with { Volume = 0.2f }, Projectile.Center);
                Projectile.soundDelay = -1;
            }

            Player projOwner = Main.player[Projectile.owner];
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = projOwner.itemTimeMax / 2;
            projOwner.itemAnimation = projOwner.itemAnimationMax / 2;

            Projectile.velocity.Y = -0.08f;
            if (Projectile.ai[0]++ == 0)
            {
                Projectile.ai[1] = projOwner.GetModPlayer<LobotomyZayinPlayer>().PenitenceHealNearbyAllies();

                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurificationPowder);
                }
            }

            if (Projectile.ai[0] % 20 == 0 && Projectile.ai[1] > 0)
            {
                List<int> targetted = new List<int>();
                for (int i = 0; i < 5; i++)
                {
                    int target = -1;
                    float dist = 10000;
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        float ndist = Projectile.Center.Distance(n.Center);
                        if (!targetted.Contains(n.whoAmI) && !n.friendly && !n.CountsAsACritter && n.CanBeChasedBy(this) && ndist < dist)
                        {
                            dist = ndist;
                            target = n.whoAmI;
                        }
                    }
                    if (target == -1)
                        break;
                    projOwner.GetModPlayer<LobotomyZayinPlayer>().PenitenceHardshipAttack(projOwner.HeldItem, 0, target, true, Projectile.ai[1]);
                    targetted.Add(target);
                }
            }
            Projectile.rotation = -MathHelper.ToRadians(45);
            
            if (Projectile.timeLeft < 20)
            {
                Projectile.alpha += 13;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = LobotomyCorp.CircleGlow.Value;
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Color color = Color.Wheat * Projectile.Opacity * 0.4f;

            Main.EntitySpriteDraw(tex, pos, null, color, 0, tex.Size() / 2, 1f + 0.1f * (float)Math.Sin(MathHelper.ToRadians(1f * Projectile.timeLeft)), 0);

            tex = TextureAssets.Projectile[Projectile.type].Value;
            color = lightColor * Projectile.Opacity;

            Main.EntitySpriteDraw(tex, pos, null, color, Projectile.rotation, tex.Size() / 2, Projectile.scale, 0);

            return false;
        }
    }
}

using LobotomyCorp.Projectiles.Realized;
using LobotomyCorp.Projectiles.RedMist;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class CobaltScar : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("The weapon resembles the claws of a vicious wolf.\n" +
                               "Once upon a time, these claws would cut open the bellies of numerous creatures and tear apart their guts.\n" +
                               "Hitting an enemy has a chance of increasing melee damage by 10%\n" +
                               "50% increased damage while under 50% health"); */
        }

        public override void LobSetDefaults()
        {
            Item.CloneDefaults(ItemID.FetidBaghnakhs);
            Item.DamageType = DamageClass.Melee;
            Item.damage = 60;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 15;
            Item.rare = ModContent.RarityType<WawB>();
            Item.scale = 1.2f;
            Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/Wolf_Scratch") with { Volume = 0.5f, PitchVariance = 0.1f };
            EGORiskLevel = RiskLevel.Waw;

            Item.shoot = ModContent.ProjectileType<CobaltScarBloodSlash>();
            Item.shootSpeed = 12f;
            Item.channel = true;
        }

        public override bool CanShoot(Player player)
        {
            if (RedMistMaskUpgrade(player))
                return true;
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (!player.wellFed)
            {
                return;
            }
            Vector2 delta = Main.MouseWorld - player.Center;
            int tileLimit = (int)Math.Min(16 * 5, delta.Length());
            delta.Normalize();
            velocity *= 0;
            position = player.Center + delta * tileLimit;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.statLife <= player.statLifeMax / 2)
            {
                damage += 0.5f;
            }
        }

        public override void HoldItem(Player player)
        {
            if (player.statLife <= player.statLifeMax / 2)
            {
                player.AddBuff(ModContent.BuffType<Buffs.WillBeBad>(), 5);
            }
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (RedMistMaskUpgrade(player))
            {
                if (!player.wellFed)
                    return 0.8f;
                return 2f;
            }

            return base.UseSpeedMultiplier(player);
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (RedMistMaskUpgrade(player))
            {
                if (player.wellFed)
                    return 0.33f;
            }

            return base.UseTimeMultiplier(player);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
                player.AddBuff(ModContent.BuffType<Buffs.WillBeBad>(), 600);

            // Assigns Cobalt Scar Tag on enemies which hits enemies 10 times over 10 seconds
            int type = ModContent.ProjectileType<CobaltScarTag>();
            int amount = 0;
            List<Projectile> tags = new List<Projectile>();
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == type && (int)p.ai[0] == target.whoAmI)
                {
                    tags.Add(p);
                    amount++;
                }
            }

            // If Cobalt scar tag is less than 3, create one more. Otherwise reset all of their timers
            if (amount < 3)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), target.Center, Vector2.Zero, type, hit.SourceDamage / 5, 0, player.whoAmI, target.whoAmI);
            }
            else
            {
                foreach (Projectile p in tags)
                {
                    p.timeLeft = 610;
                    p.netUpdate = true;
                }
            }
        }

        public override void UseItemHitboxAlt(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (RedMistMaskUpgrade(player))
            {
                noHitbox = true;
            }
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool(3))
                player.AddBuff(ModContent.BuffType<Buffs.WillBeBad>(), 600);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BladedGlove)
            .AddIngredient(ItemID.CyanHusk)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}
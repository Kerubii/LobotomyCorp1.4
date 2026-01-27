using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.Audio;
using System.Collections.Generic;
using LobotomyCorp.Players;
using ReLogic.Content;

namespace LobotomyCorp.Items.Waw
{
    public class CrimsonScar : LobCorpLight
    {
        static Asset<Texture2D> TextureMain;
        static Asset<Texture2D> Gun;
        static Asset<Texture2D> Scythe;
        static Asset<Texture2D> ScytheHalf;

        public override void Load()
        {
            TextureMain = ModContent.Request<Texture2D>(Texture);
            Gun = ModContent.Request<Texture2D>(Texture + "Gun");
            Scythe = ModContent.Request<Texture2D>(Texture + "Scythe");
            ScytheHalf = ModContent.Request<Texture2D>(Texture + "ScytheHalf");
        }

        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("With steel in one hand and gunpowder in the other, there's nothing to fear in this place.\n" +
                               "It's more important to deliver a decisive strike in blind hatred without hesitation than to hold on to insecure courage.\n" +
                               "50% increased damage under 50% health"); */
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        /*
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var x = tooltips.Find(x => x.Name == "Damage");
            x.Text = 

            base.ModifyTooltips(tooltips);
        }*/

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 32;
            Item.value = 3000;
            Item.rare = ModContent.RarityType<WawB>();
            Item.damage = 58;
            Item.knockBack = 3f;
            Item.shootSpeed = 12f;
            Item.shoot = 10;
            Item.useAmmo = AmmoID.Bullet;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            EGORiskLevel = RiskLevel.Waw;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.CombineWith(player.GetDamage(DamageClass.Ranged));
            if (player.statLife <= player.statLifeMax / 2)
            {
                damage += 0.5f;
            }
        }
        
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        /*
        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                return 0.3f;
            }
            return base.UseTimeMultiplier(player);
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                return 0.3f;
            }
            return base.UseSpeedMultiplier(player);
        }*/

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 50;
                Item.useAnimation = 50;
                Item.useStyle = ItemUseStyleID.Shoot;
                TextureAssets.Item[Item.type] = Gun;
                Item.noMelee = true;
                Item.useTurn = false;
                Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/RedHood_Gun") with { Volume = 0.2f, PitchVariance = 0.1f };
            }
            else
            {
                Item.useTime = 18;
                Item.useAnimation = 18;
                Item.useStyle = 15;
                TextureAssets.Item[Item.type] = Scythe;
                Item.noMelee = false;
                Item.useTurn = true;
                Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/RedHood_Atk1") with { Volume = 0.2f, PitchVariance = 0.1f };
            }
            return base.CanUseItem(player);
        }

        public override void HoldItem(Player player)
        {
            if (player.statLife <= player.statLifeMax / 4)
            {
                player.GetModPlayer<LobotomyWawPlayer>().CrimsonScarEmpower = 3;
            }
        }

        public override void UseStyleAlt(Player player, Rectangle heldItemFrame)
        {
            if (player.altFunctionUse != 2)
            {
                Item.useStyle = 15;
                float prog = player.itemAnimation / (float)player.itemAnimationMax;
                if (prog < 0.5f)
                {
                    TextureAssets.Item[Item.type] = ScytheHalf;
                }
            }
            base.UseStyleAlt(player, heldItemFrame);
        }

        /*public override void ModifyItemScale(Player player, ref float scale)
        {
            if (player.altFunctionUse != 2)
            {
                scale *= 1.1f;
            }
            base.ModifyItemScale(player, ref scale);
        }*/

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color ItemColor, Vector2 origin, float scale)
        {
            Texture2D tex = TextureMain.Value;
            Rectangle texFrame = tex.Frame();
            scale = 1f;
            float num = 1f;
            if ((float)texFrame.Width > 32f || (float)texFrame.Height > 32f)
            {
                num = ((texFrame.Width <= texFrame.Height) ? (32f / (float)texFrame.Height) : (32f / (float)texFrame.Width));
            }

            spriteBatch.Draw(tex, position, texFrame, drawColor, 0f, texFrame.Size() / 2, scale * num, 0, 0);
            return false;
            /*
            position = position - TextureAssets.InventoryBack.Value.Size() * Main.inventoryScale / 2f + frame.Size() * scale / 2f;
            Texture2D tex = TextureMain.Value;
            frame = tex.Frame();
            scale = 1f;
            if (frame.Width > 32 || frame.Height > 32)
                scale = frame.Width <= frame.Height ? 32f / frame.Height : 32f / frame.Width;
            scale *= Main.inventoryScale;
            position = position + TextureAssets.InventoryBack.Value.Size() * Main.inventoryScale / 2f - frame.Size() * scale / 2f;
            origin = frame.Size() * (1f / 2f - 0.5f);
            spriteBatch.Draw(tex, position, frame, drawColor, 0, origin, scale, 0, 0);
            return false;*/
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureMain.Value;
            spriteBatch.Draw(tex, Item.position - Main.screenPosition + new Vector2(Item.width / 2, Item.height - tex.Height / 2), tex.Frame(), lightColor, rotation, tex.Size() / 2, scale, 0, 0);
            return false;
        }

        public override bool CanShoot(Player player)
        {
            int empower = player.GetModPlayer<LobotomyWawPlayer>().CrimsonScarEmpower;
            return player.altFunctionUse == 2 || empower == 1 || empower == 3;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            int empower = player.GetModPlayer<LobotomyWawPlayer>().CrimsonScarEmpower;
            if (player.altFunctionUse == 2)
            {
                damage = (int)(damage * 0.7f);
                knockback *= 0.7f;
            }
            else if (empower == 1 || empower == 3)
                type = ModContent.ProjectileType<Projectiles.CrimsonScarScythe>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int empower = player.GetModPlayer<LobotomyWawPlayer>().CrimsonScarEmpower;
            EmpowerReset(player);

            if (player.altFunctionUse != 2)
            {
                if (empower == 3)
                    damage = (int)(damage * 0.5f);
                return true;
            }

            int p = Projectile.NewProjectile(source, position, velocity, type, (int)(damage * .8f), knockback, player.whoAmI);
            Main.projectile[p].GetGlobalProjectile<LobotomyGlobalProjectile>().CrimsonScarBullet = true;
            if (empower >= 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 vel = velocity.RotatedByRandom(0.349066f) * Main.rand.NextFloat(0.8f, 1f);
                    Projectile.NewProjectile(source, position, vel, type, damage / 2, knockback, player.whoAmI);
                }
            }
            return false;
        }

        private void EmpowerReset(Player player)
        {
            player.GetModPlayer<LobotomyWawPlayer>().CrimsonScarEmpower = 0;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            player.GetModPlayer<LobotomyWawPlayer>().CrimsonScarEmpower = 2;
            base.OnHitNPC(player, target, hit, damageDone);
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return player.altFunctionUse == 2;
        }

        /*public override bool ConsumeAmmo(Player player)
        {
            return 
        }*/

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.FlintlockPistol)
            .AddIngredient(ItemID.Sickle)
            .AddIngredient(ItemID.RedHusk)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class SolemnLament : LobItemBase
    {
        static Asset<Texture2D> SolemnGun1;
        static Asset<Texture2D> SolemnGun2;
        static Asset<Texture2D> SolemnGun;

        public override void Load()
        {
            SolemnGun = ModContent.Request<Texture2D>(Texture);
            SolemnGun1 = ModContent.Request<Texture2D>(Texture + "1");
            SolemnGun2 = ModContent.Request<Texture2D>(Texture + "2");
        }

        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("The somber design is a reminder that not a sliver of frivolity is allowed for the minds of those who mourn.\n" +
                               "One handgun symbolizes grief for the dead, while the other symbolizes early lament for the living.\n" +
                               "Switches between range and magic depending on the gun used"); */
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        private bool AlternateAttack;

        public override void LobSetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.value = 3000;
            Item.rare = ModContent.RarityType<WawB>();
            Item.damage = 32;
            Item.shootSpeed = 12f;
            Item.shoot = 10;
            Item.useAmmo = AmmoID.Bullet;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            //Item.UseSound = LobotomyCorp.WeaponSounds.Gun;
            Item.noMelee = true;
            AlternateAttack = false;
            EGORiskLevel = RiskLevel.Waw;
        }

        public override void LobModifyTooltips(List<TooltipLine> tooltips)
        {
            var RealizedEGOTooltip = new TooltipLine(Mod, "LobotomyCorp.SolemnLamentMana", $"{Language.GetTextValue("Mods.LobotomyCorp.Items.SolemnLament.ManaTooltip", ManaCost(Main.LocalPlayer))}");
            int index = tooltips.FindIndex(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
            tooltips.Insert(index, RealizedEGOTooltip);

            base.LobModifyTooltips(tooltips);
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1.5f;
            return base.UseTimeMultiplier(player);
        }

        public override float UseAnimationMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1.5f;
            return base.UseAnimationMultiplier(player);
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.CombineWith(player.GetDamage(DamageClass.Magic));
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        /*
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                
            }
            else
            {
                
            }
            return base.CanUseItem(player);
        }*/

        private int ManaCost(Player player)
        {
            return (int)(8 * player.manaCost);
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                TextureAssets.Item[Item.type] = SolemnGun1;
                player.manaRegenDelay = player.maxRegenDelay;
                return base.UseItem(player);
            }
            else
            {
                AlternateAttack = Main.rand.NextBool(2);
                SoundStyle sound = LobotomyCorp.WeaponSounds.Gun;
                float dingdong = ModContent.GetInstance<Configs.LobotomyConfig>().SolemnDingDongChance;
                if (AlternateAttack)
                {
                    TextureAssets.Item[Item.type] = SolemnGun2;
                    if (dingdong > 0f && Main.rand.Next(100) < (int)(dingdong * 100f))
                        sound = new SoundStyle("LobotomyCorp/Sounds/Item/ButterFlyMan_StongAtk_Black") with { Volume = 0.1f, MaxInstances = -1 };
                }
                else
                {
                    TextureAssets.Item[Item.type] = SolemnGun1;
                    if (dingdong > 0f && Main.rand.Next(100) < (int)(dingdong * 100f))
                        sound = new SoundStyle("LobotomyCorp/Sounds/Item/ButterFlyMan_StongAtk_White") with { Volume = 0.1f, MaxInstances = -1 };
                }
                SoundEngine.PlaySound(sound, player.Center);
                player.manaRegenDelay = player.maxRegenDelay;
                return true;
            }
            //return base.UseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            if (player.altFunctionUse == 2)
            {
                Vector2 vel = new Vector2(0, -6).RotatedBy(velocity.ToRotation());
                if (player.CheckMana(ManaCost(player), true))
                    Projectile.NewProjectile(source, position + vel, velocity, type, damage, knockback);
                return true;
            }
            else
            {
                if (AlternateAttack)
                {
                    if (player.CheckMana(ManaCost(player), true))
                        return true;
                    else
                        return false;
                }    
            }
            return true;
        }

        public override void HoldItem(Player player)
        {
            player.scope = false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (AlternateAttack)
                return false;
            return base.CanConsumeAmmo(ammo, player);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color ItemColor, Vector2 origin, float scale)
        {
            Texture2D tex = SolemnGun.Value;
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
            position = position - TextureAssets.InventoryBack.Size() * Main.inventoryScale / 2f + frame.Size() * scale / 2f;
            frame = tex.Frame();
            scale = 1f;
            if (frame.Width > 32 || frame.Height > 32)
                scale = frame.Width <= frame.Height ? 32f / frame.Height : 32f / frame.Width;
            scale *= Main.inventoryScale;
            position = position + TextureAssets.InventoryBack.Size() * Main.inventoryScale / 2f - frame.Size() * scale / 2f;
            origin = frame.Size() * (1f / 2f - 0.5f);
            spriteBatch.Draw(tex, position, frame, drawColor, 0, origin, scale, 0, 0);
            return false;*/
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = SolemnGun.Value;
            spriteBatch.Draw(tex, Item.position - Main.screenPosition + new Vector2(Item.width / 2, Item.height - tex.Height / 2), tex.Frame(), lightColor, rotation, tex.Size() / 2, scale, 0, 0);
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.IllegalGunParts)
            .AddIngredient(ItemID.SilverDye, 2)
            .AddIngredient(ItemID.BlackDye, 2)
            .AddRecipeGroup("LobotomyCorp:Butterflies", 5)
            .AddTile(Mod, "BlackBox2")
            .Register();
        }
    }
}
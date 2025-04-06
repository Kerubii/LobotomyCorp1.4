using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LobotomyCorp.Utils;
using ReLogic.Content;
using Terraria.Audio;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Players;

namespace LobotomyCorp.Items.Ruina.Technology
{
    public class SolemnLamentR : SEgoItem
	{
        public static Texture2D screenWhiteHit;
        public static Texture2D screenBlackHit;

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                screenWhiteHit = Mod.Assets.Request<Texture2D>("Misc/Ding", AssetRequestMode.ImmediateLoad).Value;
                screenBlackHit = Mod.Assets.Request<Texture2D>("Misc/Dong", AssetRequestMode.ImmediateLoad).Value;

                Main.QueueMainThreadAction(() =>
                {
                    LobotomyCorp.PremultiplyTexture(screenWhiteHit);
                    LobotomyCorp.PremultiplyTexture(screenBlackHit);
                });
            }
        }

        public override void SetStaticDefaults() 
		{
            // DisplayName.SetDefault("Solemn Lament"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault(GetTooltip());
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

		public override void SetDefaults() 
		{
            EgoColor = LobotomyCorp.WawRarity;

            Item.damage = 62;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;

			Item.useTime = 52;
            Item.useAnimation = 52;

            Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
			Item.value = 10000;
			Item.rare = 2;
            Item.shootSpeed = 9f;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Bullet;

            Item.noUseGraphic = true;
            Item.noMelee = true;
			Item.autoReuse = true;
            AltGun = false;
            //PerfectSwitch = false;
            LobotomyGlobalItem.LobItem(Item).CustomDraw = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return modPlayer(player).SolemnLamentDisable == 0;
            //return (!modPlayer(player).SolemnSwitch || player.itemTime == 0) && AltAmmo(player);
        }

        public bool AltGun;
        //public bool PerfectSwitch;

        public override bool SafeCanUseItem(Player player)
        {
            LobotomyGlobalItem lobItem = LobotomyGlobalItem.LobItem(Item);
            if (!modPlayer(player).SolemnSwitch)
            {
                lobItem.CustomTexture = Mod.Assets.Request<Texture2D>("Items/Ruina/Technology/SolemnLamentS2").Value;
                if (AltAmmo(player))
                {
                    if (Main.rand.NextBool(3) || player.altFunctionUse == 2)
                        Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/ButterFlyMan_StongAtk_Black") with { Volume = 0.1f, MaxInstances = -1 };
                    else
                        Item.UseSound = SoundID.Item11;
                }
                else
                    Item.UseSound = null;
                modPlayer(player).SolemnSwitch = true;
            }
            else
            {
                lobItem.CustomTexture = Mod.Assets.Request<Texture2D>("Items/Ruina/Technology/SolemnLamentS2").Value;
                if (Main.rand.NextBool(3) || player.altFunctionUse == 2)
                    Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/ButterFlyMan_StongAtk_White") with { Volume = 0.1f , MaxInstances = -1};
                else
                    Item.UseSound = SoundID.Item11;
                modPlayer(player).SolemnSwitch = false;
            }
            return base.SafeCanUseItem(player);
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2 && modPlayer(player).SolemnLamentFireRate > 0)
            {
                modPlayer(player).SolemnLamentFireRate -= 0.05f;
                if (modPlayer(player).SolemnLamentFireRate <= 0f)
                    modPlayer(player).SolemnLamentFireRate = 0;
                return true;
            }
            else if (player.altFunctionUse != 2 && modPlayer(player).SolemnLamentFireRate <= 1.5f)
            {
                modPlayer(player).SolemnLamentFireRate += 0.05f;
                if (modPlayer(player).SolemnLamentFireRate > 1.5f)
                    modPlayer(player).SolemnLamentFireRate = 1.5f;
                return true;
            }

            return base.UseItem(player);
        }

        public override float UseTimeMultiplier(Player player)
        {
            float rate = Math.Min(1f, modPlayer(player).SolemnLamentFireRate);
            return 1f - .75f * rate;
        }

        public override float UseAnimationMultiplier(Player player)
        {
            float rate = Math.Min(1f, modPlayer(player).SolemnLamentFireRate);
            return 1f - .75f * rate;
        }

        /*
        public override bool SafeCanUseItem(Player player)
        {
            LobotomyGlobalItem lobItem = LobotomyGlobalItem.LobItem(Item);

            Item.UseSound = SoundID.Item11;
            PerfectSwitch = false;
            if (player.itemTime > 8)
                PerfectSwitch = true;

            if (player.altFunctionUse == 2)
            {
                player.itemTime = 0;
                lobItem.CustomTexture = Mod.Assets.Request<Texture2D>("Items/Ruina/Technology/SolemnLamentS1").Value;
                modPlayer(player).SolemnSwitch = true;
                if (PerfectSwitch)
                    Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/ButterFlyMan_StongAtk_Black") with { Volume = 0.2f};
                return modPlayer(player).SolemnLamentDisable != 2;
            }
            else if (modPlayer(player).SolemnSwitch || player.itemTime == 0)
            {
                player.itemTime = 0;
                lobItem.CustomTexture = Mod.Assets.Request<Texture2D>("Items/Ruina/Technology/SolemnLamentS2").Value;
                modPlayer(player).SolemnSwitch = false;
                if (PerfectSwitch)
                    Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/ButterFlyMan_StongAtk_White") with { Volume = 0.2f };
                return modPlayer(player).SolemnLamentDisable != 1;
            }
            return false;
        }*/

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.altFunctionUse != 2)
                {
                    bool fire = true;
                    if (!modPlayer(player).SolemnSwitch)
                    {
                        if (AltAmmo(player))
                            AltAmmoConsume(player, ref type, ref velocity.X, ref velocity.Y, ref damage);
                        else
                            fire = false;
                    }
                    if (fire)
                    {
                        Main.projectile[Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI)].GetGlobalProjectile<LobotomyGlobalProjectile>().Lament = modPlayer(player).SolemnSwitch ? (byte)1 : (byte)2;
                        int dustType = modPlayer(player).SolemnSwitch ? 91 : 109;
                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 tempVel = velocity;
                            tempVel *= Main.rand.NextFloat(4f);
                            tempVel = tempVel.RotatedByRandom(MathHelper.ToRadians(15));

                            Dust d = Dust.NewDustPerfect(position, dustType, tempVel);
                            d.noGravity = true;
                            d.fadeIn = 1.2f;
                        }
                    }
                }
                else
                {
                    bool fire = true;
                    if (!modPlayer(player).SolemnSwitch)
                    {
                        if (AltAmmo(player))
                            AltAmmoConsume(player, ref type, ref velocity.X, ref velocity.Y, ref damage);
                        else
                            fire = false;
                    }
                    if (fire)
                    {
                        damage = (int)(damage * 0.5f);

                        int amount = Main.rand.Next(6, 9);
                        for (int i = 0; i < amount; i++)
                        {
                            Vector2 tempVel = velocity;
                            if (i > 0)
                            {
                                tempVel *= Main.rand.NextFloat(0.6f, 1f);
                                tempVel = tempVel.RotatedByRandom(MathHelper.ToRadians(15));
                            }
                            int p = Projectile.NewProjectile(source, position, tempVel, ModContent.ProjectileType<Projectiles.Kaleidoscope>(), damage, knockback, player.whoAmI, -type);
                            Main.projectile[p].localAI[0] = modPlayer(player).SolemnSwitch ? 1 : 2;
                            Main.projectile[p].GetGlobalProjectile<LobotomyGlobalProjectile>().Lament = modPlayer(player).SolemnSwitch ? (byte)1 : (byte)2;
                        }
                        int dustType = modPlayer(player).SolemnSwitch ? 91 : 109;
                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 tempVel = velocity;
                            tempVel *= Main.rand.NextFloat(4f);
                            tempVel = tempVel.RotatedByRandom(MathHelper.ToRadians(15));

                            Dust d = Dust.NewDustPerfect(position, dustType, tempVel);
                            d.noGravity = true;
                            d.fadeIn = 1.2f;
                        }
                        for (int i = 0; i < 24; i++)
                        {
                            float angle = 6.28f * i / 24f;
                            Vector2 offset = new Vector2(50f, 0).RotatedBy(velocity.ToRotation());
                            Vector2 ellipse = new Vector2(4f * (float)Math.Cos(angle), 10f * (float)Math.Sin(angle)).RotatedBy(velocity.ToRotation()) * (modPlayer(player).SolemnLamentFireRate > 1f ? 1f : 0.5f);
                            Dust d = Dust.NewDustPerfect(position + offset, dustType, ellipse);
                            d.noGravity = true;
                            d.fadeIn = 1.2f;
                        }
                    }
                }
            }

            player.itemRotation = (float)Math.Atan2(velocity.Y * player.direction, velocity.X * player.direction) - player.fullRotation;
            return false;
        }

        public override void UseItemFrame(Player player)
        {
            if (player.altFunctionUse != 2)
                return;

            int frame = player.bodyFrame.Y / player.bodyFrame.Height;
            player.bodyFrame.Y = player.bodyFrame.Height * (frame - 1);
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return modPlayer(player).SolemnSwitch;
        }

        public override void AddRecipes() 
		{
            CreateRecipe()
               .AddIngredient(ModContent.ItemType<SolemnLament>())
               .AddIngredient(ItemID.SoulofNight, 6)
               .AddIngredient(ItemID.SoulofLight, 6)
               .AddIngredient(ItemID.Ectoplasm, 8)
               .AddTile<Tiles.BlackBox3>()
               .AddCondition(RedMistCond)
               .Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }

        LobotomyWawPlayer modPlayer(Player player)
        {
            return player.GetModPlayer<LobotomyWawPlayer>();
        }

        /*public override void HoldStyle(Player player)
        {
            if (modPlayer(player).SolemnSwitch)
                player.ItemRotation
        }*/

        private bool AltAmmo(Player player)
        {
            bool canUse = false;
            if (Item.useAmmo > 0)
            {
                for (int index = 0; index < 58; ++index)
                {
                    if (player.inventory[index].ammo == Item.useAmmo && player.inventory[index].stack > 0)
                    {
                        int type = player.inventory[index].type;
                        for (int index2 = 0; index2 < 58; ++index2)
                        {
                            if (player.inventory[index2].ammo == Item.useAmmo && player.inventory[index2].stack > 0 && type != player.inventory[index2].type)
                            {
                                canUse = true;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            return canUse;
        }

        private void AltAmmoConsume(Player player, ref int shoot, ref float speedX, ref float speedY, ref int damage)
        {
            int type = -1;
            bool pew = false;
            bool ammoCheck = false;
            for (int index = 54; index < 58; ++index)
            {
                if (player.inventory[index].ammo == Item.useAmmo && player.inventory[index].stack > 0)
                {
                    type = player.inventory[index].type;
                    ammoCheck = true;
                    break;
                }
            }
            if (!ammoCheck)
            {
                for (int index = 0; index < 54; ++index)
                {
                    if (player.inventory[index].ammo == Item.useAmmo && player.inventory[index].stack > 0)
                    {
                        type = player.inventory[index].type;
                        break;
                    }
                }
            }
            ammoCheck = false;
            Item ammo = new Item();
            if (type > -1)
            {
                for (int index = 54; index < 58; ++index)
                {
                    if (player.inventory[index].ammo == Item.useAmmo && player.inventory[index].stack > 0 && type != player.inventory[index].type)
                    {
                        ammo = player.inventory[index];
                        ammoCheck = true;
                        pew = true;
                        break;
                    }
                }
                if (!ammoCheck)
                {
                    for (int index = 0; index < 54; ++index)
                    {
                        if (player.inventory[index].ammo == Item.useAmmo && player.inventory[index].stack > 0 && type != player.inventory[index].type)
                        {
                            ammo = player.inventory[index];
                            pew = true;
                            break;
                        }
                    }
                }
            }
            if (!pew)
                return;

            Vector2 dir = new Vector2(speedX, speedY);
            dir.Normalize();
            dir *= (Item.shootSpeed + ammo.shootSpeed);
            speedX = dir.X;
            speedY = dir.Y;

            if (ammo.damage > 0)
                damage = player.GetWeaponDamage(Item) + (int)player.GetTotalDamage(DamageClass.Ranged).ApplyTo(ammo.damage);
            else
                damage = Item.damage;

            shoot = ammo.shoot;
            bool consume = false;// (player.itemAnimation < Item.useAnimation - 2);
            if (player.ammoBox && Main.rand.Next(5) == 0)
                consume = true;
            if (player.ammoPotion && Main.rand.Next(5) == 0)
                consume = true;
            if (player.ammoCost80 && Main.rand.Next(5) == 0)
                consume = true;
            if (player.ammoCost75 && Main.rand.Next(4) == 0)
                consume = true;
            if (!PlayerLoader.CanConsumeAmmo(player, Item, ammo))
                consume = true;
            if (!ammo.consumable)
                consume = true;
            if (consume)
                return;
            PlayerLoader.CanConsumeAmmo(player, Item, ammo);
            ItemLoader.OnConsumeAmmo(Item, ammo, player);
            ammo.stack--;
            if (ammo.stack > 0)
                return;
            ammo.active = false;
            ammo.TurnToAir();
        }

        public override void HoldItem(Player player)
        {
            player.scope = false;
        }

        /*public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color ItemColor, Vector2 origin, float scale)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("LobotomyCorp/Items/SolemnLamentS");
            spriteBatch.Draw(texture, position, frame, drawColor, 0, origin, scale, SpriteEffects.None, 0f);
            return false;
        }*/
    }

    class SolemnLamentWhite : ScreenFilter
    {
        public override void Initialize()
        {
            Opacity = 0.3f;
            FilterTexture = SolemnLamentR.screenWhiteHit;           
        }

        public override void Update()
        {
            Time++;
            if (Time > 10)
            Opacity -= 0.1f;
        }

        public override bool DeActive()
        {
            return Opacity <= 0;
        }
    }

    class SolemnLamentBlack : ScreenFilter
    {
        public override void Initialize()
        {
            Opacity = 0.3f;
            FilterTexture = SolemnLamentR.screenBlackHit;
        }

        public override void Update()
        {
            Time++;
            if (Time > 10)
                Opacity -= 0.1f;
        }

        public override bool DeActive()
        {
            return Opacity <= 0;
        }
    }
}
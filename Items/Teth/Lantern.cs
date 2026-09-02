using LobotomyCorp.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Teth
{
    public class Lantern : LobCorpHeavy
    {
        // Used by LanternSwing
        public static Asset<Texture2D> AltTex;

        public override void Load()
        {
            AltTex = Mod.Assets.Request<Texture2D>("Items/Teth/Lantern2");
        }

        public override void LobSetDefaults()
        {
            Item.damage = 62;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 56;
            Item.useAnimation = 56;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<TethB>();
            SwingSound = LobotomyCorp.WeaponSounds.Hammer;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LanternSwing>();
            Item.shootSpeed = 1;
            EGORiskLevel = RiskLevel.Teth;
        }

        public override bool CanUseItem(Player player)
        {
            Item.noUseGraphic = false;
            Item.channel = false;
            if (RedMistMaskUpgrade(player))
            {
                Item.channel = true;
                Item.noUseGraphic = true;
            }
            return base.CanUseItem(player);
        }

        public override bool AltFunctionUse(Player player)
        {
            return RedMistMaskUpgrade(player);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (RedMistMaskUpgrade(player))
            {
                if (player.altFunctionUse == 2)
                {
                    type = ModContent.ProjectileType<LanternSwingAround>();
                }
                else
                    knockback *= 1.5f;
            }
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (RedMistMaskUpgrade(player))
                if (player.altFunctionUse == 2)
                    return 0.5f;
            return base.UseSpeedMultiplier(player);
        }

        public override bool? CanMeleeAttackCollideWithNPC(Rectangle meleeAttackHitbox, Player player, NPC target)
        {
            if (RedMistMaskUpgrade(player))
                return false;
            return base.CanMeleeAttackCollideWithNPC(meleeAttackHitbox, player, target);
        }

        public override bool CanShoot(Player player)
        {
            return RedMistMaskUpgrade(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.FlinxFur, 2)
            .AddIngredient(ItemID.Emerald, 1)
            .AddIngredient(ItemID.AntlionMandible, 4)
            .AddTile<Tiles.BlackBox>()
            .Register();
        }
    }
}
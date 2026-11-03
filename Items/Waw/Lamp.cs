using LobotomyCorp.Buffs;
using LobotomyCorp.Players.DamageType;
using LobotomyCorp.Projectiles;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Waw
{
    public class Lamp : LobCorpHeavy
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Item.type] = ModContent.ItemType<LampAlt>();
        }

        public override void LobSetDefaults()
        {
            Item.damage = 110;
            Item.DamageType = RWBPEnable(DamageClass.Melee, ModContent.GetInstance<ExtractorBlack>());
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 56;
            Item.useAnimation = 56;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<WawB>();
            Item.shoot = ModContent.ProjectileType<LampPlayerSwing>();
            Item.shootSpeed = 1;
            SwingSound = LobotomyCorp.WeaponSounds.Hammer;
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.Waw;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("LobotomyCorp:DungeonLantern")
            .AddIngredient(ItemID.Feather, 4)
            .AddIngredient(ItemID.Bone, 10)
            .AddTile(Mod, "BlackBox3")
            .AddCustomShimmerResult(ItemID.BlueDungeonLamp, 1)
            .AddCustomShimmerResult(ItemID.Feather, 4)
            .AddCustomShimmerResult(ItemID.Bone, 10)
            .Register();
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff<LampLightDebuff>())
            {
                modifiers.SourceDamage += 0.4f;
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            player.GetModPlayer<LobotomyModPlayer>().ReplaceItemCooldown(0.05f, 1f, target.whoAmI);
        }
    }
}
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp
{
    public class LobotomyGlobalItem : GlobalItem
    {
        public static LobotomyGlobalItem LobItem(Item item)
        {
            return item.GetGlobalItem<LobotomyGlobalItem>();
        }

        public override bool InstancePerEntity => true;

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            return base.Clone(item, itemClone);
        }

        public bool CustomDraw = false;
        public Texture2D CustomTexture = null;

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ItemID.QueenBeeBossBag)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Waw.Hornet>(), 2));
            }
            else if (item.type == ItemID.WallOfFleshBossBag)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Aleph.Censored>(), 4));
            }
            else if (item.type == ItemID.EyeOfCthulhuBossBag)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.He.Syrinx>(), 4));
            }
            if (item.type == ItemID.Present)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.He.Christmas>(), 100));
            }
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            LobotomyTethPlayer modPlayer = player.GetModPlayer<LobotomyTethPlayer>();
            if (modPlayer.TodaysExpressionActive)
                modifiers.FinalDamage *= modPlayer.TodaysExpressionDamage();
        }

        public override void ModifyHitPvp(Item item, Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            LobotomyTethPlayer modPlayer = player.GetModPlayer<LobotomyTethPlayer>();
            if (modPlayer.TodaysExpressionActive)
                modifiers.FinalDamage *= modPlayer.TodaysExpressionDamage();
        }
    }
}
//css_ref ../tModLoader.dll
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using LobotomyCorp.Players;

namespace LobotomyCorp.Items.Ruina.Technology
{
    public class HarmonyNote : ModItem
    {
        private int frameY = 0;
        private float rotationR = 0f;
        private int decay = 0;

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = 0;
            Item.rare = 1;
            Item.maxStack = 999;
            frameY = Main.rand.Next(4);
            rotationR = Main.rand.NextFloat(-0.785f, 0.785f);
            Item.direction = Main.rand.NextBool(2)?1:-1;
        }

        public override void GrabRange(Player player, ref int grabRange)
        {
            grabRange = 16;
        }

        public override bool ItemSpace(Player player)
        {
            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            decay++;
            if (decay > 90)
            {
                Item.TurnToAir();
                Item.active = !Item.active;
            }
            Item.velocity.Y *= 0.95f;
            gravity = 0;
            maxFallSpeed = 16f;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Vector2 position = Item.position + Item.Size/2 - Main.screenPosition;
            Vector2 origin = new Vector2(11, 10);
            Rectangle frame = new Rectangle(0, 22 * frameY, 22, 20);
            if (decay > 60)
                lightColor *= 1f - (float)(decay - 60) / 30f;
            spriteBatch.Draw(TextureAssets.Item[Item.type].Value, position, (Rectangle?)frame, lightColor, rotation + rotationR, origin, 1f, Item.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            return false;
        }

        public override bool OnPickup(Player player)
        {
            /*
            LobotomyModPlayer.ModPlayer(player).HarmonyTime += 420;
            if (LobotomyModPlayer.ModPlayer(player).HarmonyTime > 600)
                LobotomyModPlayer.ModPlayer(player).HarmonyTime = 600;
            player.AddBuff(ModContent.BuffType<Buffs.MusicalAddiction>(), LobotomyModPlayer.ModPlayer(player).HarmonyTime, true);*/
            player.AddBuff(ModContent.BuffType<Buffs.Satiated>(), 300, true);
            player.GetModPlayer<LobotomyHePlayer>().HarmonyGainRhythm(1);
            return false;
        }
    }
}

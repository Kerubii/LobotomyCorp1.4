using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ReLogic.Content;
using LobotomyCorp.Players;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Ruina.Religion;

namespace LobotomyCorp.PlayerDrawEffects
{
    public class DrawBackMiscellaneous : PlayerDrawLayer
    {
        private static Asset<Texture2D> SwordSharpenedWithTears;
        private static Asset<Texture2D> PenitenceHalo;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            LobotomyTethPlayer modTethPlayer = drawInfo.drawPlayer.GetModPlayer<LobotomyTethPlayer>();
            LobotomyHePlayer modHePlayer = drawInfo.drawPlayer.GetModPlayer<LobotomyHePlayer>();
            LobotomyWawPlayer modWawPlayer = drawInfo.drawPlayer.GetModPlayer<LobotomyWawPlayer>();
            return !drawInfo.drawPlayer.dead && ((modWawPlayer.SwordSharpenedImpaledCount > 0) || true);
        }

        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.BackAcc);

        public override void Load()
        {
            SwordSharpenedWithTears = Mod.Assets.Request<Texture2D>("Projectiles/Realized/SwordSharpenedWithTearsRSword");
            PenitenceHalo = Mod.Assets.Request<Texture2D>("Misc/PenitenceHalo");
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;
            LobotomyZayinPlayer modZayinPlayer = Player.GetModPlayer<LobotomyZayinPlayer>();
            LobotomyWawPlayer modWawPlayer = Player.GetModPlayer<LobotomyWawPlayer>();
            if (modWawPlayer.SwordSharpenedImpaledCount > 0)
            {
                for (int i = 0; i < modWawPlayer.SwordSharpenedImpaledCount; i++)
                {
                    Texture2D texture = SwordSharpenedWithTears.Value;
                    int num = 24;
                    Rectangle frame = new Rectangle(0, 0, num, num);
                    Vector3 swordData = modWawPlayer.SwordSharpenedImpalePosition[i];
                    Vector2 pos = drawInfo.Center - Main.screenPosition + new Vector2(swordData.X * Player.direction, swordData.Y - Player.gfxOffY);
                    pos = new Vector2((int)pos.X, (int)pos.Y);
                    float rotation = swordData.Z;
                    if (Player.direction == -1)
                        rotation = 3.14f - rotation;
                    rotation += 2.35619f;

                    DrawData data = new DrawData(texture, pos, frame, Color.White, rotation, texture.Size() / 2, 1f, 0, 0);
                    drawInfo.DrawDataCache.Add(data);
                }
            }

            if (Player.HasBuff<PenitenceAtonement>())
            {
                Texture2D texture = LobotomyCorp.CircleGlow.Value;
                Vector2 pos = drawInfo.Center - Main.screenPosition + new Vector2(0, -15 + Player.gfxOffY);
                pos = new Vector2((int)pos.X, (int)pos.Y);
                DrawData data = new DrawData(texture, pos, null, Color.Wheat * 0.6f, 0, texture.Size() / 2, 1, 0, 0);
                drawInfo.DrawDataCache.Add(data);

                texture = PenitenceHalo.Value;
                Rectangle frame = texture.Frame(verticalFrames: 2);
                data = new DrawData(texture, pos, frame, Color.Wheat * 0.8f, 0, frame.Size() / 2, 0.8f + 0.05f * (float)Math.Sin(MathHelper.ToRadians(1) * Main.timeForVisualEffects), 0, 0);
                drawInfo.DrawDataCache.Add(data);

                data = new DrawData(texture, pos, texture.Frame(verticalFrames: 2, frameY: 1), Color.Wheat * 0.8f, MathHelper.ToRadians(0.2f) * (float)Main.timeForVisualEffects * Player.direction, frame.Size() / 2, 0.8f + 0.05f * (float)Math.Sin(MathHelper.ToRadians(1) * Main.timeForVisualEffects), 0, 0);
                drawInfo.DrawDataCache.Add(data);

                int type = ModContent.ItemType<PenitenceR>();
                if (Player.HeldItem.type == type && Player.heldProj == -1)
                {
                    Texture2D tex = TextureAssets.Item[type].Value;
                    pos = drawInfo.Center - Main.screenPosition + Vector2.UnitY * Player.gfxOffY;
                    pos = new Vector2((int)pos.X, (int)pos.Y);
                    data = new DrawData(tex, pos, null, drawInfo.colorArmorBody, 0, tex.Size() / 2, 1, Player.direction > -1 ? 0 : SpriteEffects.FlipHorizontally);

                    drawInfo.DrawDataCache.Add(data);
                }
            }
        }
    }
}

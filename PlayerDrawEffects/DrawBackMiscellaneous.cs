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

namespace LobotomyCorp.PlayerDrawEffects
{
    public class DrawBackMiscellaneous : PlayerDrawLayer
    {
        private static Asset<Texture2D> SwordSharpenedWithTears;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            LobotomyTethPlayer modTethPlayer = drawInfo.drawPlayer.GetModPlayer<LobotomyTethPlayer>();
            LobotomyHePlayer modHePlayer = drawInfo.drawPlayer.GetModPlayer<LobotomyHePlayer>();
            LobotomyWawPlayer modWawPlayer = drawInfo.drawPlayer.GetModPlayer<LobotomyWawPlayer>();
            return !drawInfo.drawPlayer.dead && (modWawPlayer.SwordSharpenedImpaledCount > 0);
        }

        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.BackAcc);

        public override void Load()
        {
            SwordSharpenedWithTears = Mod.Assets.Request<Texture2D>("Projectiles/Realized/SwordSharpenedWithTearsRSword");
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;
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
                    float rotation = swordData.Z;
                    if (Player.direction == -1)
                        rotation = 3.14f - rotation;
                    rotation += 2.35619f;

                    DrawData data = new DrawData(texture, pos, frame, Color.White, rotation, texture.Size() / 2, 1f, 0, 0);
                    drawInfo.DrawDataCache.Add(data);
                }
            }
        }
    }
}

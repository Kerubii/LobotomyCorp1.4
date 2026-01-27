using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using LobotomyCorp;
using Terraria.UI;
using ReLogic.Graphics;
using System;
using Terraria.GameContent;
using LobotomyCorp.ModSystems;
using static ReLogic.Graphics.DynamicSpriteFont;
using Terraria.GameContent.UI.Elements;
using ReLogic.Content;

namespace LobotomyCorp.UI
{
    class UIGifElement : UIElement
    {
        string Texture;
        Asset<Texture2D> TexAsset;
        int Frame;
        int CurrentFrame;
        int MaxFrame;
        int FrameCount;

        float Opacity;
        float Scale;

        public UIGifElement(string texture, int frames, int frameCounter)
        {
            Texture = texture;
            TexAsset = ModContent.Request<Texture2D>(Texture);
            CurrentFrame = 0;
            FrameCount = 0;
            Frame = frames;
            MaxFrame = frameCounter;

            Scale = 1f;
            Opacity = 1f;
        }

        public void ChangeScale(float scale)
        {
            Scale = scale;
        }

        public void ChangeOpacity(float opacity)
        {
            Opacity = opacity;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            FrameCount++;
            if (FrameCount >= MaxFrame)
            {
                FrameCount = 0;
                CurrentFrame++;
                if (CurrentFrame >= Frame)
                {
                    CurrentFrame -= Frame;
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Texture2D tex = TexAsset.Value;
            CalculatedStyle dimensions = GetDimensions();
            Rectangle frame = tex.Frame(1, Frame, frameY: CurrentFrame);

            Vector2 position = dimensions.Position() + new Vector2(dimensions.Width / 2, dimensions.Height / 2);
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(tex, position, frame, Color.White * Opacity, 0f, origin, Scale, 0, 0);
        }
    }
}
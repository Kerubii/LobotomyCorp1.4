using LobotomyCorp;
using LobotomyCorp.Buffs;
using LobotomyCorp.ModSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace LobotomyCorp.UI
{
    internal class NihilSelection : UIState
    {
        public UIElement UIRoot;
        public UIGifElement NihilFace;
        public NihilButton[] ButtonMode = new NihilButton[4];
        int time;

        public override void OnInitialize()
        {
            UIRoot = new UIElement();
            SetRectangle(UIRoot, left: Main.screenWidth/2, top: Main.screenHeight/2, width: 0f, height: 0f);
            UIRoot.Left.Set(0, 0);
            UIRoot.Top.Set(0, 0);
            UIRoot.Width.Set(0, 1f);
            UIRoot.Height.Set(0, 1f);

            NihilFace = new UIGifElement("LobotomyCorp/UI/UITextures/NihilSmile", 5, 16);
            SetRectangle2(NihilFace, 0, 0, 96, 96);
            UIRoot.Append(NihilFace);
            NihilFace.ChangeOpacity(0f);

            ButtonMode = new NihilButton[4];
            for (int i = 0; i < ButtonMode.Length; i++)
            {
                ButtonMode[i] = new NihilButton(i);
                SetRectangle2(ButtonMode[i], 0, 0, 64f, 64f);
                UIRoot.Append(ButtonMode[i]);
            }
            time = 0;

            Append(UIRoot);
        }

        private void SetRectangle2(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left - width / 2, 0.5f);
            uiElement.Top.Set(top - height / 2, 0.5f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }

        private void SetRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            time++;

            if (time > 60)
            {
                NihilFace.ChangeOpacity(MathHelper.Clamp((time - 60f) / 120f, 0f, 1f));

                if (time > 80)
                {
                    NihilFace.ChangeScale(1f + MathHelper.Clamp((time - 80f) / 180, 0f, 1f));
                }
            }
            else
            {
                NihilFace.ChangeOpacity(0f);
                NihilFace.ChangeScale(1f);
            }

            if (time < 30)
            {
                float prog = time / 30f;
                prog *= prog * prog;
                for (int i = 0; i < ButtonMode.Length; i++)
                {
                    Vector2 pos = new Vector2(120 * prog, 0).RotatedBy(1.57f * i);
                    SetRectangle2(ButtonMode[i], (int)pos.X, (int)pos.Y, 32f, 32f);
                    ButtonMode[i].SetSelect(false);
                }
            }
            else
            {
                for (int i = 0; i < ButtonMode.Length; i++)
                {
                    ButtonMode[i].SetSelect(true);
                }
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            for (int i = 0; i < 4; i++)
            {
                if (ButtonMode[i].IsHoveredOver)
                {
                    ButtonMode[i].OnClickElement();
                }
            }
        }

        public override void OnActivate()
        {
            time = 0;
        }

        public void ResetTime()
        {
            time = 0;
        }
    }

    class NihilButton : UIElement
    {
        string FlavorText;
        int Mode;
        float SelectScale;

        bool Selectable;
        bool isHover;

        public NihilButton(int mode)
        {
            Mode = mode;
            FlavorText = "This is a test FlavorText";
            SelectScale = 1f;
        }

        public void SetSelect(bool select)
        {
            Selectable = select;
        }

        public bool IsHoveredOver => isHover;

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            if (isHover)
            {
                OnClickElement();
            }
        }

        public void OnClickElement()
        {
            ApplyBuff();
            LobUISystem.Instance.ClearUI();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 mouse = Main.MouseScreen - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

            bool hoverPrevious = isHover;
            isHover = false;
            if (mouse.Length() > 94)
            {
                if (Mode % 2 == 0)
                {
                    if (Math.Abs(mouse.X) > Math.Abs(mouse.Y))
                    {
                        if (Mode == 0)
                            isHover = mouse.X > 0;
                        else
                            isHover = mouse.X < 0;
                    }
                }
                else
                {
                    if (Math.Abs(mouse.Y) > Math.Abs(mouse.X))
                    {
                        if (Mode == 1)
                            isHover = mouse.Y > 0;
                        else
                            isHover = mouse.Y < 0;
                    }
                }
            }

            if (!Selectable)
            {
                isHover = false;
            }

            if (isHover)
            {
                SelectScale += 0.18f;
                if (SelectScale > 2f)
                    SelectScale = 2f;
            }
            else
            {
                if (SelectScale > 1f)
                    SelectScale -= 0.1f;
                if (SelectScale < 1f)
                    SelectScale = 1f;
            }
        }

        public void ApplyBuff()
        {
            string buff;
            switch (Mode)
            {
                case 0:
                    buff = "Greed";
                    break;
                case 1:
                    buff = "Hatred";
                    break;
                case 2:
                    buff = "Despair";
                    break;
                case 3:
                    buff = "Wrath";
                    break;
                default:
                    return;
            }
            Main.LocalPlayer.AddBuff(LobotomyCorp.Instance.Find<ModBuff>("Nihil" + buff).Type, 60 * 10);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            LobotomyCorp Mod = LobotomyCorp.Instance;
            CalculatedStyle dimensions = GetDimensions();
            int type;
            switch (Mode)
            {
                case 0:
                    type = ModContent.BuffType<NihilGreed>();
                    break;
                case 1:
                    type = ModContent.BuffType<NihilGreed>();
                    break;
                case 2:
                    type = ModContent.BuffType<NihilGreed>();
                    break;
                default:
                    type = ModContent.BuffType<NihilGreed>();
                    break;
            }
            Texture2D buffTex = TextureAssets.Buff[type].Value;
            Rectangle frame = buffTex.Frame();

            Vector2 position = dimensions.Position() + new Vector2(dimensions.Width / 2, dimensions.Height / 2);
            Vector2 offset = new Vector2(-frame.Width / 2 + 6, 0).RotatedBy(1.57f * Mode);
            Vector2 origin = frame.Size() / 2 + offset;

            spriteBatch.Draw(buffTex, position + offset, frame, Color.White, 0f, origin, SelectScale, 0, 0);

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText(FlavorText);
            }
        }
    }
}
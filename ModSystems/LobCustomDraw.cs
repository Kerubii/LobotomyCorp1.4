using LobotomyCorp.Configs;
using LobotomyCorp.Utils;
using LobotomyCorp.Visuals.LobEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace LobotomyCorp.ModSystems
{
    class LobCustomDraw : ModSystem
    {
        static Asset<Effect> Bloom;

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Main.QueueMainThreadAction(() =>
                {
                    layer = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
                });
            }
            Bloom = Mod.Assets.Request<Effect>("Effects/Bloom", AssetRequestMode.ImmediateLoad);
            On_Main.DrawDust += CustomDrawLayerPostDust;
        }

        public override void Unload()
        {
            Bloom = null;
            screenFilters = null;
            drawEffects = null;
            On_Main.DrawDust -= CustomDrawLayerPostDust;
        }

        private void CustomDrawLayerPostDust(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            if (!DrawEffectsIsActive()) return;

            bool enable = ModContent.GetInstance<LobotomyConfig>().BloomEnable;
            // Store all custom draws on Layer
            if (enable)
            {
                Main.instance.GraphicsDevice.SetRenderTarget(layer);
                Main.instance.GraphicsDevice.Clear(Color.Transparent);
            }

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            bool applyBloom = false;
            Rectangle gameScreen = new Rectangle((int)Main.screenPosition.X - 1000, (int)Main.screenPosition.Y - 1050, Main.screenWidth + 2000, Main.screenHeight + 2100);
            foreach (LobDrawEffects we in drawEffects)
            {
                if (!we.active)
                    continue;
                if (gameScreen.Contains((int)we.position.X, (int)we.position.Y))
                {
                    we.Draw();
                    applyBloom = true;
                }
                else
                    we.Deactivate();
            }
            Main.spriteBatch.End();

            if (enable)
            {
                Main.instance.GraphicsDevice.SetRenderTarget(null);
                if (applyBloom)
                {
                    // Save current screen + custom draws on Swap
                    Main.instance.GraphicsDevice.SetRenderTarget(Main.screenTargetSwap);
                    Main.instance.GraphicsDevice.Clear(Color.Transparent);
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                    Main.spriteBatch.Draw(layer, Vector2.Zero, Color.White);
                    Main.spriteBatch.End();

                    // Get Bloom texture through layer
                    ApplyBloom(Main.instance.GraphicsDevice, layer, Main.screenTarget);

                    // Add Bloom to Swap and render it
                    Main.instance.GraphicsDevice.SetRenderTarget(Main.screenTarget);
                    Main.instance.GraphicsDevice.Clear(Color.Transparent);
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                    Main.spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                    Main.spriteBatch.Draw(layer, Vector2.Zero, Color.White);
                    Main.spriteBatch.End();
                }
            }
        }

        RenderTarget2D layer;

        private bool DrawEffectsIsActive()
        {
            foreach (LobDrawEffects we in drawEffects)
            {
                if (we.active)
                    return true;
            }
            return false;
        }

        private void ApplyBloom(GraphicsDevice graphicsDevice, RenderTarget2D target1, RenderTarget2D target2)
        {
            // Clear screenTarget
            graphicsDevice.SetRenderTarget(target2);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            // Find bright spots
            Effect bloom = Bloom.Value;
            bloom.Parameters["uThreshold"].SetValue(0.7f);
            bloom.Parameters["uRange"].SetValue(2);
            bloom.Parameters["uIntensity"].SetValue(1.05f);
            bloom.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            bloom.CurrentTechnique.Passes[0].Apply();

            // Draw all bright spots
            Main.spriteBatch.Draw(target1, Vector2.Zero, Color.White);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            for (int i = 0; i < 5; i++)
            {
                // Clear Layer
                graphicsDevice.SetRenderTarget(target1);
                graphicsDevice.Clear(Color.Transparent);
                bloom.CurrentTechnique.Passes["GBlurH"].Apply();
                // Draw bright spots
                Main.spriteBatch.Draw(target2, Vector2.Zero, Color.White);

                // Clear screenTarget
                graphicsDevice.SetRenderTarget(target2);
                graphicsDevice.Clear(Color.Transparent);
                bloom.CurrentTechnique.Passes["GBlurV"].Apply();
                // Draw Bright spots
                Main.spriteBatch.Draw(target1, Vector2.Zero, Color.White);
            }
            Main.spriteBatch.End();
            
            // Clear Layer
            graphicsDevice.SetRenderTarget(target1);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            // Draw Final Result
            Main.spriteBatch.Draw(target2, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        private LobDrawEffects[] drawEffects = new LobDrawEffects[100];
        private ScreenFilter[] screenFilters = new ScreenFilter[5];

        public static LobCustomDraw Instance()
        {
            return ModContent.GetInstance<LobCustomDraw>();
        }

        public override void OnWorldLoad()
        {
            screenFilters = new ScreenFilter[5];
            for (int i = 0; i < 5; i++)
            {
                screenFilters[i] = new ScreenFilter();
            }
            drawEffects = new LobDrawEffects[100];
            for (int i = 0; i < drawEffects.Length; i++)
            {
                drawEffects[i] = new LobDrawEffects();
            }

            //ModContent.GetInstance<LobotomyCorp>().Logger.Info("ScreenFilterInitialized");
        }

        public override void OnWorldUnload()
        {
            screenFilters = null;

            drawEffects = null;
            //ModContent.GetInstance<LobotomyCorp>().Logger.Info("ScreenFilterDeinitialized");
        }

        public override void PostUpdateEverything()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            foreach (ScreenFilter ol in screenFilters)
            {
                if (ol.Active)
                {
                    ol.Update();
                    ol.Active = !ol.DeActive();
                }
            }

            foreach (LobDrawEffects de in drawEffects)
            {
                if (de.active)
                {
                    de.Update();
                }
            }

            if (shockwaveTime > 0)
            {
                float progress = 1f - shockwaveTime / (float)shockwaveTimeMax;
                Filters.Scene["LobotomyCorp:Shockwave"].GetShader().UseProgress(progress).UseOpacity(100f * (1 - progress / 3f));
                shockwaveTime--;
            }
            else if (Filters.Scene["LobotomyCorp:Shockwave"].IsActive())
            {
                Filters.Scene["LobotomyCorp:Shockwave"].Deactivate();
            }
        }

        int shockwaveTime = 0;
        int shockwaveTimeMax = 0;

        public void StartShockwave(Vector2 position, float rippleCount, float rippleSize, float rippleSpeed, int lifetime)
        {
            if (Main.netMode != NetmodeID.Server && !Filters.Scene["LobotomyCorp:Shockwave"].IsActive())
            {
                Filters.Scene.Activate("LobotomyCorp:Shockwave", position).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(position);
                shockwaveTime = shockwaveTimeMax = lifetime;
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 1"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "LobotomyCorp: ScreenFilter",
                    delegate
                    {
                        foreach (ScreenFilter filter in screenFilters)
                        {
                            if (filter.Active)
                            {
                                filter.Draw(Main.spriteBatch, ModContent.GetInstance<Configs.LobotomyConfig>().ScreenEffectOpacity);
                            }
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            base.ModifyInterfaceLayers(layers);
        }

        /// <summary>
        /// There are 5 Layers for screentextures, Replaces the layer as inteded limitation so as a General rule of thumb, lets say this
        /// 0-2 Lower Layers, Used as visual effects for players, will occupy a spot inactive, if all slots active replaces preferred layer
        /// 3   Used by NPCs to provide information, preferably bosses or special npcs
        /// 4   Special Cases
        /// Force to replace a specific layer, used for 0-2
        /// </summary>
        /// <param name="newLayer"></param>
        /// <param name="layer"></param>
        public void AddFilter(ScreenFilter newLayer, int layer = 0, bool force = false, bool refresh = true)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            if (!force && layer < 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (screenFilters[i].Active && screenFilters[i].GetType() == newLayer.GetType())
                    {
                        if (refresh)
                        {
                            screenFilters[i] = null;
                            screenFilters[i] = newLayer;
                        }
                        return;
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    if (!screenFilters[i].Active)
                    {
                        screenFilters[i] = newLayer;
                        return;
                    }
                }
                return;
            }

            screenFilters[layer] = newLayer;
        }

        public bool ContainsFilter(ScreenFilter filterCheck)
        {
            for (int i = 0; i < 3; i++)
            {
                if (screenFilters[i].Active && screenFilters[i].GetType() == filterCheck.GetType())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// There are 5 Layers for screentextures, Replaces the layer so as a General rule of thumb, lets say this
        /// 0-2 Lower Layers, Used as visual effects for players, will occupy a spot inactive, if all slots active replaces preferred layer
        /// 3   Used by NPCs to provide information, preferably bosses or special npcs
        /// 4   Special Cases
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool IsLayerActive(int layer)
        {
            return screenFilters[layer].Active;
        }

        public void AddVEffects(LobDrawEffects draw)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            for (int i = 0; i < drawEffects.Length; i++)
            {
                if (drawEffects[i].active)
                    continue;
                drawEffects[i] = draw;
                break;
            }
        }
    }
}

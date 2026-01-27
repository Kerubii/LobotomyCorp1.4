//css_ref ../../tModLoader.dll
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Build.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Policy;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace LobotomyCorp.Misc.LobSky
{
	public class BlueStarSky : CustomSky
	{
        public class BlueStarStars
        {
            public int Index;
            public int Time;
            public int Lifetime;
            public Vector2 Pos;
            public float Scale;
            public float MaxScale;
            public Color DrawColor;
            public Vector2 Vel;
            public float Rotation;

            public BlueStarStars(int lifetime, int index, float maxScale, Vector2 pos, Vector2 vel, Color color)
            {
                Index = index;
                Time = 0;
                Lifetime = lifetime;
                Pos = pos;
                Vel = vel;
                Scale = 0.1f;
                MaxScale = maxScale;
                DrawColor = color;
                Rotation = 0;
            }
        }

        private List<BlueStarStars> stars = new List<BlueStarStars>();

        private int projectileWhoAmI;
        private bool isActive = false;
        private float intensity = 0f;
        private float rotation = 0f;
        private float starScale = 1f;
        private Vector2 starPosition = Vector2.Zero;

        public override void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (intensity < 1f)
                {
                    Vector2? currentStarPos = blueStarCenter(out projectileWhoAmI);
                    if (intensity < 0.5f || currentStarPos.HasValue)
                    {
                        intensity += 0.01f;
                        if (currentStarPos.HasValue)
                        {
                            starPosition = (Vector2)currentStarPos;
                        }
                    }
                    else
                    {
                        intensity -= 0.1f;
                    }
                }

                if (Main.rand.NextBool(8))
                {
                    int time = Main.rand.Next(90, 180);
                    float maxScale = Main.rand.NextFloat(0.1f, 0.2f);
                    Vector2 startingPosition = Main.screenPosition + new Vector2(Main.screenWidth * Main.rand.NextFloat(-0.1f, 1.1f), Main.screenHeight * Main.rand.NextFloat(-0.1f, 1.1f));
                    Vector2 vel = new Vector2(Main.rand.NextFloat(0.1f, 1.2f), 0).RotatedBy(Main.rand.NextFloat(6.28f));
                    Color color = Color.Lerp(new Color(129, 238, 255), Color.White, Main.rand.NextFloat(1f));
                    stars.Add(new BlueStarStars(time, stars.Count, maxScale, startingPosition, vel, color));
                }

                starScale++;
                rotation -= MathHelper.ToRadians(0.1f);
            }
            else if (!isActive && intensity > 0f)
            {
                intensity -= 0.01f;
            }

            if (intensity > 0f)
            {
                foreach (var star in stars)
                {
                    if (star.Scale < star.MaxScale)
                        star.Scale += 0.05f;
                    star.Pos += star.Vel;
                    star.Vel *= 0.98f;
                    star.Time++;
                    star.Rotation += MathHelper.ToRadians(0.1f);
                }
            }

            stars.RemoveAll(c => c.Time >= c.Lifetime);
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                float bgIntensity = intensity / 0.5f;
                if (bgIntensity > 1f)
                    bgIntensity = 1f;
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * bgIntensity);

                bgIntensity = (intensity - 0.5f) / 0.5f;
                if (bgIntensity > 0f)
                {
                    Texture2D tex = TextureAssets.Background[BackgroundTextureLoader.GetBackgroundSlot(LobotomyCorp.Instance, "Misc/Backgrounds/BlueStarHoleS")].Value;
                    float scale = 1.6f + 0.05f * (float)Math.Sin(starScale / 360 * 6.28f);
                    Rectangle frame = tex.Frame();
                    spriteBatch.Draw(tex, starPosition - Main.screenPosition, frame, Color.White * bgIntensity, rotation, frame.Size() / 2, scale, 0, 0);
                }

                Texture2D starTex = LifeForADaredevilR.Sparkle.Value;
                foreach (BlueStarStars star in stars)
                {
                    Vector2 pos = star.Pos - Main.screenPosition;
                    for (int i = 0; i < 2; i++)
                    {
                        float rot = star.Rotation * (i % 2 == 0 ? 1 : -1);
                        float sca = star.Scale;
                        float opacity = 0;
                        if (star.Time < 20)
                            opacity = star.Time / 20f;
                        else if (star.Time > star.Lifetime - 60)
                            opacity = (star.Lifetime - star.Time) / 60f;
                        else
                            opacity = 1f;
                        Color starColor = star.DrawColor * opacity;
                        spriteBatch.Draw(starTex, pos, starTex.Frame(), starColor, rot, starTex.Size() / 2, sca, 0, 0);
                    }
                }
            }
        }

        private Vector2? blueStarCenter(out int index)
        {
            index = -1;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == ModContent.ProjectileType<SoundOfAStarBlueStar>())
                {
                    index = p.whoAmI;
                    return p.Center;
                }
            }
            return null;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Reset()
        {
            isActive = false;
        }
        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive || intensity > 0f;
        }
        
        public override float GetCloudAlpha()
        {
            return 0f;
        }
    }
}
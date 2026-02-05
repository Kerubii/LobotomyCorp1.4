using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using Terraria;

namespace LobotomyCorp.Util
{
	public struct TaperingTrail
	{
		public const int TotalIllusions = 1;

		public const int FramesPerImportantTrail = 60;

		private static VertexStrip _vertexStrip = new VertexStrip();

        public float RotationOffset;
        public float width;

        public Color ColorStart;
        public Color ColorEnd;

        public void Draw(Vector2[] position, float[] rotation, Vector2 offset, CustomShaderData shader)
        {
            shader.Apply();
            _vertexStrip.PrepareStrip(position, rotation, StripColors, StripWidth, -Main.screenPosition + offset, RotationOffset, position.Length, includeBacksides: true);
            _vertexStrip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        private Color StripColors(float progressOnStrip)
		{
            Color result = Color.Lerp(ColorStart, ColorEnd, GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true));// * (1f - GetLerpValue(0f, 0.98f, progressOnStrip, clamped: true));
			result.A /= 2;
			return result;
		}

		private float StripWidth(float progressOnStrip)
		{
			return width * (1f - progressOnStrip);
		}

        public static float GetLerpValue(float from, float to, float t, bool clamped = false)
        {
            if (clamped)
            {
                if (from < to)
                {
                    if (t < from)
                    {
                        return 0f;
                    }
                    if (t > to)
                    {
                        return 1f;
                    }
                }
                else
                {
                    if (t < to)
                    {
                        return 1f;
                    }
                    if (t > from)
                    {
                        return 0f;
                    }
                }
            }
            return (t - from) / (to - from);
        }
    }
}

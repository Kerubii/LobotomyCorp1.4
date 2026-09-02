using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace LobotomyCorp.Util
{
	public class CustomShaderData : ShaderData
    {
		private Vector3 _uColor = Vector3.One;

		private Vector3 _uSecondaryColor = Vector3.One;

		private float _uSaturation = 1f;

		private float _uOpacity = 1f;

		private Vector4 _uCustomShaderData = Vector4.Zero;

		private Asset<Texture2D> _uImage1;
		private Asset<Texture2D> _uImage2;
		private Asset<Texture2D> _uImage3;

		public CustomShaderData(Asset<Effect> shader, string passName)
			: base(shader, passName)
		{
		}

		public virtual void Apply(DrawData? drawData = null)
		{
			Shader.Parameters["uColor"].SetValue(_uColor);
			Shader.Parameters["uSaturation"].SetValue(_uSaturation);
			Shader.Parameters["uSecondaryColor"].SetValue(_uSecondaryColor);
			Shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
			Shader.Parameters["uOpacity"].SetValue(_uOpacity);
			Shader.Parameters["uCustomData"].SetValue(_uCustomShaderData);
			if (drawData.HasValue)
			{
				DrawData value = drawData.Value;
				Vector4 value2 = Vector4.Zero;
				if (drawData.Value.sourceRect.HasValue)
				{
					value2 = new Vector4(value.sourceRect.Value.X, value.sourceRect.Value.Y, value.sourceRect.Value.Width, value.sourceRect.Value.Height);
				}
				base.Shader.Parameters["uSourceRect"].SetValue(value2);
				base.Shader.Parameters["uWorldPosition"].SetValue(Main.screenPosition + value.position);
				base.Shader.Parameters["uImageSize0"].SetValue(new Vector2(value.texture.Width, value.texture.Height));
			}
			else
			{
				base.Shader.Parameters["uSourceRect"].SetValue(new Vector4(0f, 0f, 4f, 4f));
			}
			if (_uImage1 != null)
			{
				Main.graphics.GraphicsDevice.Textures[1] = _uImage1.Value;
				Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
				base.Shader.Parameters["uImageSize1"].SetValue(new Vector2(_uImage1.Value.Width, _uImage1.Value.Height));
			}
			if (_uImage2 != null)
			{
				Main.graphics.GraphicsDevice.Textures[2] = _uImage2.Value;
				Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.LinearWrap;
				base.Shader.Parameters["uImageSize2"].SetValue(new Vector2(_uImage2.Value.Width, _uImage2.Value.Height));
			}
			if (_uImage3 != null)
			{
				Main.graphics.GraphicsDevice.Textures[3] = _uImage3.Value;
				Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.LinearWrap;
				base.Shader.Parameters["uImageSize2"].SetValue(new Vector2(_uImage3.Value.Width, _uImage3.Value.Height));
			}
			base.Apply();
		}

		public CustomShaderData UseImage1(Mod mod, string texturePath)
        {
			_uImage1 = mod.Assets.Request<Texture2D>(texturePath);
			return this;
		}

		public CustomShaderData UseImage1(Asset<Texture2D> texture)
		{
			_uImage1 = texture;
			return this;
		}

		public CustomShaderData UseImage2(Mod mod, string texturePath)
		{
			_uImage2 = mod.Assets.Request<Texture2D>(texturePath);
			return this;
		}

        public CustomShaderData UseImage2(Asset<Texture2D> texture)
        {
            _uImage2 = texture;
            return this;
        }

        public CustomShaderData UseImage3(Mod mod, string texturePath)
		{
			_uImage3 = mod.Assets.Request<Texture2D>(texturePath);
			return this;
		}

        public CustomShaderData UseImage3(Asset<Texture2D> texture)
        {
            _uImage3 = texture;
            return this;
        }

		public CustomShaderData UseColor(float r, float g, float b)
		{
			return UseColor(new Vector3(r, g, b));
		}

		public CustomShaderData UseColor(Color color)
		{
			return UseColor(color.ToVector3());
		}

		public CustomShaderData UseColor(Vector3 color)
		{
			_uColor = color;
			return this;
		}

		public CustomShaderData UseSecondaryColor(float r, float g, float b)
		{
			return UseSecondaryColor(new Vector3(r, g, b));
		}

		public CustomShaderData UseSecondaryColor(Color color)
		{
			return UseSecondaryColor(color.ToVector3());
		}

		public CustomShaderData UseSecondaryColor(Vector3 color)
		{
			_uSecondaryColor = color;
			return this;
		}

		public CustomShaderData UseSaturation(float sat)
		{
			_uSaturation = sat;
			return this;
		}

		public CustomShaderData UseOpacity(float alpha)
		{
			_uOpacity = alpha;
			return this;
		}

		public CustomShaderData UseCustomShaderDate(float x, float y = 0, float z = 0, float w = 0)
        {
			_uCustomShaderData = new Vector4(x, y, z, w);
			return this;
        }
	}
}

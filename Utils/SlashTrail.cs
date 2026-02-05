using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace LobotomyCorp.Util
{
	//Studying VertexStrip and BladeDrawer
	public class SlashTrail
	{
		protected static LobotomyCorp mod;

		private VertexStrip _vertexStrip = new VertexStrip();

		public Color color;
		public float Width = 0;
		public float RotationOffset = 0;

		public float StartingWidth;
		public float EndingWidth;

		public float Radius1;
		public float Radius2;

		bool differentWidth = false;

		public SlashTrail(float width = 8, float rotationOffset = 0)
        {
			color = Color.White;
			Width = width;
			RotationOffset = rotationOffset;
        }

		public SlashTrail(float startingWidth = 8, float endingWidth = 8, float rotationOffset = 0)
		{
			color = Color.White;
			StartingWidth = startingWidth;
			EndingWidth = endingWidth;
			RotationOffset = rotationOffset;
			differentWidth = true;
		}
		/// <summary>
		/// Use for width variances
		/// </summary>

		public void Trail()
        {
			
        }

		public void DrawTrail(Projectile proj, CustomShaderData shader)
		{
			/*float[] rotation = (float[])proj.oldRot.Clone();
			for (int i = 0; i < rotation.Length; i++)
			{
				rotation[i] += RotationOffset;
			}*/
			shader.Apply();
			_vertexStrip.PrepareStrip(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f, RotationOffset, proj.oldPos.Length, includeBacksides: true);
			_vertexStrip.DrawTrail();
			Main.pixelShader.CurrentTechnique.Passes[0].Apply();
		}

        public void DrawTrail(Projectile proj, MiscShaderData shader)
        {
            /*float[] rotation = (float[])proj.oldRot.Clone();
			for (int i = 0; i < rotation.Length; i++)
			{
				rotation[i] += RotationOffset;
			}*/
            shader.Apply();
            _vertexStrip.PrepareStrip(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f, RotationOffset, proj.oldPos.Length, includeBacksides: true);
            _vertexStrip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        public void DrawSpecific(Vector2[] position, float[]rotation, Vector2 offset, CustomShaderData shader)
        {
			shader.Apply();
			_vertexStrip.PrepareStrip(position, rotation, StripColors, StripWidth, -Main.screenPosition + offset, RotationOffset,position.Length, includeBacksides: true);
			_vertexStrip.DrawTrail();
			Main.pixelShader.CurrentTechnique.Passes[0].Apply();
		}

		/// <summary>
		/// Draws a circle and shades it with a shader
		/// </summary>
		/// <param name="center">Automatically reduced by Main.ScreenPosition</param>
		/// <param name="startingRotation">Angle where the most left side of the texture begins</param>
		/// <param name="direction">Texture counter-clockwise(1), or clockwise(-1)</param>
		/// <param name="radius">Distance of the circle from its center</param>
		/// <param name="angles">How many slices the circle will have, Fewer the better</param>
		/// <param name="shader"></param>
		public void DrawCircle(Vector2 center, float startingRotation, int direction, float radius, int angles, CustomShaderData shader)
        {
			Vector2[] position = new Vector2[angles + 1];
			float[] rotation = new float[angles + 1];
			for (int i = 0; i < angles + 1; i++)
			{
				rotation[i] = (startingRotation - 6.28f * ((float)i / (float)angles) * direction);
				position[i] = center + new Vector2(radius, 0).RotatedBy(rotation[i]);
			}
			DrawSpecific(position, rotation, Vector2.Zero, shader);
		}

		/// <summary>
		/// Center is automatically reduced by Main.ScreenPosition
		/// StartingRotation is the angle where the most left side of the texture begins
		/// EndAngle is the length where the texture ends relative to StartingRotation (StartingRotation + EndAngle = True EndAngle)
		/// Direction is making the texture counter-clockwise(1), or clockwise(-1)
		/// Radius is the distance of the circle from its center
		/// Automatically reduces amount of angles to provide the same smoothness as desired from DrawCircle but with fewer actual angles
		/// </summary>
		/// <param name="center">Automatically reduced by Main.ScreenPosition</param>
		/// <param name="startingRotation">Angle where the most left side of the texture begins</param>
		/// <param name="endAngle">Length where the texture ends relative to StartingRotation (StartingRotation + EndAngle = True EndAngle)</param>
		/// <param name="direction">Texture counter-clockwise(1), or clockwise(-1)</param>
		/// <param name="radius">Distance of the circle from its center</param>
		/// <param name="angles">How many slices the circle will have, Fewer the better</param>
		/// <param name="shader"></param>
		public void DrawPartCircle(Vector2 center, float startingRotation, float endAngle, int direction, float radius, int angles, CustomShaderData shader)
		{
			float percentage = endAngle / 6.14f;
			angles = (int)(angles * percentage);

			Vector2[] position = new Vector2[angles + 1];
			float[] rotation = new float[angles + 1];

			for (int i = 0; i < angles + 1; i++)
			{
				rotation[i] = (startingRotation - endAngle * ((float)i / (float)angles) * direction);
				position[i] = center + new Vector2(radius, 0).RotatedBy(rotation[i]);
			}
			DrawSpecific(position, rotation, Vector2.Zero, shader);
		}

		/// <summary>
		/// Center is automatically reduced by Main.ScreenPosition
		/// OffsetRotation is the angle of the Ellipse itself
		/// StartingRotation is the angle where the most left side of the texture begins relative to OffsetRotation
		/// Direction is making the texture counter-clockwise(1), or clockwise(-1)
		/// Radius1 is the X distance of the circle from its center, StartingWidth is the subtrahend for the SecondPosition
		/// Radius2 is the Y distance of the circle from its center, EndingWidth is the subtrahend for the SecondPosition
		/// Angles dictates how many slices the circle will have, Fewer the better
		/// </summary>
		/// <param name="center">Automatically reduced by Main.ScreenPosition</param>
		/// <param name="offsetRotation">Angle of the Ellipse itself</param>
		/// <param name="startingRotation">Angle where the most left side of the texture begins</param>
		/// <param name="direction">Texture counter-clockwise(1), or clockwise(-1)</param>
		/// <param name="radius1">X distance of the circle from its center</param>
		/// <param name="radius2">Y distance of the circle from its center</param>
		/// <param name="angles">How many slices the circle will have, Fewer the better</param>
		/// <param name="shader"></param>
		public void DrawEllipse(Vector2 center, float offsetRotation, float startingRotation, int direction, float radius1, float radius2, int angles, CustomShaderData shader)
		{
			Radius1 = radius1;
			Radius2 = radius2;
			if (direction == 0)
				direction = 1;
			Vector2[] position = new Vector2[angles + 1];
			Vector2[] position2 = new Vector2[angles + 1];
			for (int i = 0; i < angles + 1; i++)
			{
				float rotation = (startingRotation - 6.28f * ((float)i / (float)angles) * direction);

				position[i].X = radius1 * (float)Math.Cos(rotation);
				position[i].Y = radius2 * (float)Math.Sin(rotation);
				position[i] = position[i].RotatedBy(offsetRotation);
				position[i] += center;

				position2[i].X = (radius1 - StartingWidth) * (float)Math.Cos(rotation);
				position2[i].Y = (radius2 - EndingWidth) * (float)Math.Sin(rotation);
				position2[i] = position2[i].RotatedBy(offsetRotation);
				position2[i] += center;
			}

			shader.Apply();
			_vertexStrip.PrepareEllipse(position, position2, StripColors, -Main.screenPosition, RotationOffset, position.Length, includeBacksides: true);
			_vertexStrip.DrawTrail();
			Main.pixelShader.CurrentTechnique.Passes[0].Apply();
		}

		/// <summary>
		/// StartingWidth is the subtrahend for the SecondPosition, EndingWidth is the subtrahend for the SecondPosition. 
		/// Automatically reduces amount of angles to provide the same smoothness as desired from DrawEllipse but with fewer actual angles
		/// </summary>
		/// <param name="center">Automatically reduced by Main.ScreenPosition</param>
		/// <param name="offsetRotation">Angle of the Ellipse itself</param>
		/// <param name="startingRotation">Angle where the most left side of the texture begins</param>
		/// <param name="endAngle">Length where the texture ends relative to StartingRotation (StartingRotation + EndAngle = True EndAngle)</param>
		/// <param name="direction">Texture counter-clockwise(1), or clockwise(-1)</param>
		/// <param name="radius1">X distance of the circle from its center</param>
		/// <param name="radius2">Y distance of the circle from its center</param>
		/// <param name="angles">How many slices the circle will have, Fewer the better</param>
		/// <param name="shader"></param>
		public void DrawPartEllipse(Vector2 center, float offsetRotation, float startingRotation, float endAngle, int direction, float radius1, float radius2, int angles, CustomShaderData shader)
		{
			float percentage = endAngle / 6.14f;
			angles = (int)(angles * percentage);

			Radius1 = radius1;
			Radius2 = radius2;

			Vector2[] position = new Vector2[angles + 1];
			Vector2[] position2 = new Vector2[angles + 1];
			for (int i = 0; i < angles + 1; i++)
			{
				float rotation = (startingRotation - endAngle * ((float)i / (float)angles) * direction);

				position[i].X = radius1 * (float)Math.Cos(rotation);
				position[i].Y = radius2 * (float)Math.Sin(rotation);
				position[i] = position[i].RotatedBy(offsetRotation);
				position[i] += center;

				position2[i].X = (radius1 - StartingWidth) * (float)Math.Cos(rotation);
				position2[i].Y = (radius2 - EndingWidth) * (float)Math.Sin(rotation);
				position2[i] = position2[i].RotatedBy(offsetRotation);
				position2[i] += center;
			}

			shader.Apply();
			_vertexStrip.PrepareEllipse(position, position2, StripColors, -Main.screenPosition, RotationOffset, position.Length, includeBacksides: true);
			_vertexStrip.DrawTrail();
			Main.pixelShader.CurrentTechnique.Passes[0].Apply();
		}

		/// <summary>
		/// THEY COME IN PAIRS, First array is top of the texture, Second array is the lower part of the texture, Its not complex though. Subtract Position with Main screenposition
		/// </summary>
		public void DrawManual(Vector2[] TopPosition, Vector2[] BottomPosition, CustomShaderData shader, float progOffset = 0f)
        {
			shader.Apply();
			_vertexStrip.PrepareManual(TopPosition, BottomPosition, StripColors, TopPosition.Length, includeBacksides: true, progOffset);
			_vertexStrip.DrawTrail();
			Main.pixelShader.CurrentTechnique.Passes[0].Apply();
		}

		public void SetWidth(int width)
        {
			Width = width;
        }

		private Color StripColors(float progressOnStrip)
		{
			return color;
		}

		private float StripWidth(float progressOnStrip)
		{
			if (Width == 0)
				Width = 8;
			return Width;
		}

		public float StripWidthEllipse(float progressOnStrip)
		{
			Vector2 ellipse1 = Ellipse(Radius1, Radius2, 6.28f * progressOnStrip);
			Vector2 ellipse2 = Ellipse(Radius1 - StartingWidth, Radius2 - EndingWidth, 6.28f * progressOnStrip);

			//Dust.NewDustPerfect(Main.LocalPlayer.Center + ellipse1, 1).noGravity = true;
			//Dust.NewDustPerfect(Main.LocalPlayer.Center + ellipse2, 1).noGravity = true;

			return (ellipse1 - ellipse2).Length();
		}

		private Vector2 Ellipse(float r1, float r2, float rot)
        {
			Vector2 ellipse1;
			ellipse1.X = r1 * (float)Math.Cos(rot);
			ellipse1.Y = r2 * (float)Math.Sin(rot);
			return ellipse1;
		}

		internal void DrawTrail(Projectile projectile, object p)
        {
            throw new NotImplementedException();
        }
    }
}

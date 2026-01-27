using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Drawing;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace LobotomyCorp.Utils
{
	public class NihilScreenShader : ScreenShaderData
    {
        private readonly Asset<Effect> _shader;
        private string _passName;
        private EffectPass _effectPass;

        public NihilScreenShader(Asset<Effect> shader, string passName, string passName2)
        : base(shader, passName)
        {
            _shader = shader;
            _passName = passName2;
        }

        public override void Apply()
        {
            base.Apply();
            SecondFilterApply();
        }

        public void SecondFilterApply()
        {
            if (Intensity <= 0)
                return;

            if (_shader != null && _passName != null)
                _effectPass = _shader.Value.CurrentTechnique.Passes[_passName];
            _effectPass.Apply();
        }
    }
}

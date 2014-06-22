using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;

using OpenTKTK.Shaders;
using OpenTKTK.Textures;
using OpenTKTK.Utils;

namespace TerrainGenTest
{
    class TerrainGenerationShader : ShaderProgram2D
    {
        private static VertexBuffer _sVB;
        
        protected override void ConstructVertexShader(ShaderBuilder vert)
        {
            base.ConstructVertexShader(vert);

            vert.AddAttribute(ShaderVarType.Vec2, "in_vertex");
            vert.AddVarying(ShaderVarType.Vec2, "var_texcoord");
            vert.Logic = @"
                void main(void)
                {
                    var_texcoord = vec2(in_vertex.x, 1 - in_vertex.y);
                    gl_Position = in_vertex * screen_resolution;
                }
            ";
        }

        protected override void ConstructFragmentShader(ShaderBuilder frag)
        {
            base.ConstructFragmentShader(frag);

            frag.AddUniform(ShaderVarType.Sampler2D, "terrain");
            frag.AddUniform(ShaderVarType.Sampler2D, "gendata");
            frag.Logic = @"
                void main(void)
                {
                    out_colour = texture2D(terrain, var_texcoord);
                }
            ";
        }

        public TerrainGenerationShader()
        {
            PrimitiveType = PrimitiveType.Quads;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            AddAttribute("in_vertex", 2);

            if (_sVB == null) {
                _sVB = new VertexBuffer(2);
                _sVB.SetData(new float[] { 0f, 0f, 1f, 0f, 1f, 1f, 0f, 1f });
            }
        }

        public void Render(Terrain terrain)
        {
            SetTexture("terrain", terrain.Texture);
            SetTexture("gendata", terrain.GenData);

            Render(terrain.Width, terrain.Height);
        }

        public void Render(int width, int height)
        {
            SetScreenSize(width, height);

            _sVB.Begin(this);
            _sVB.Render();
            _sVB.End();
        }
    }
}

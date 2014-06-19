using OpenTK.Graphics.OpenGL;

using OpenTKTK.Scene;
using OpenTKTK.Shaders;
using OpenTKTK.Utils;

namespace TerrainGenTest
{
    class ParticleShader : ShaderProgram3D<Camera>
    {
        public ParticleShader()
        {
            PrimitiveType = PrimitiveType.Points;
        }

        protected override void ConstructVertexShader(ShaderBuilder vert)
        {
            base.ConstructVertexShader(vert);

            vert.AddAttribute(ShaderVarType.Vec3, "in_position");
            vert.AddAttribute(ShaderVarType.Vec3, "in_colour");

            vert.AddVarying(ShaderVarType.Vec4, "var_colour");

            vert.Logic = @"
                void main(void)
                {
                    var_colour = vec4(in_colour, 1);

                    gl_Position = proj * view * vec4(in_position.xzy, 1);
                }
            ";
        }

        protected override void ConstructFragmentShader(ShaderBuilder frag)
        {
            base.ConstructFragmentShader(frag);

            frag.Logic = @"
                void main(void)
                {
                    out_colour = var_colour;
                }
            ";
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            AddAttribute("in_position", 3);
            AddAttribute("in_colour", 3);
        }

        public void Render(Terrain terrain)
        {
            for (int x = 0; x < terrain.Width; ++x) {
                for (int y = 0; y < terrain.Height; ++y) {
                    Render(terrain[x, y], x, y);
                }
            }
        }

        public void Render(Sector sector, int x, int y)
        {
            foreach (var part in sector) {
                Render(part, x, y);
            }
        }

        public void Render(Particle part, int x, int y)
        {
            var clr = part.Material.Colour;

            GL.VertexAttrib3(Attributes[0].Location, part.Position);
            GL.VertexAttrib3(Attributes[1].Location, clr.R, clr.G, clr.B);
        }
    }
}

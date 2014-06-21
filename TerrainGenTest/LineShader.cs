using OpenTKTK.Scene;
using OpenTKTK.Shaders;
using OpenTKTK.Utils;

namespace TerrainGenTest
{
    class LineShader : ShaderProgram3D<Camera>
    {
        public LineShader()
        {
            PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType.Lines;
        }

        protected override void ConstructVertexShader(OpenTKTK.Utils.ShaderBuilder vert)
        {
            base.ConstructVertexShader(vert);

            vert.AddAttribute(ShaderVarType.Vec3, "in_vertex");

            vert.Logic = @"
                void main(void)
                {
                    gl_Position = proj * view * vec4(in_vertex, 1);
                }
            ";
        }

        protected override void ConstructFragmentShader(ShaderBuilder frag)
        {
            base.ConstructFragmentShader(frag);

            frag.Logic = @"
                void main(void)
                {
                    out_colour = vec4(1, 1, 1, 1);
                }
            ";
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            AddAttribute("in_vertex", 3);
        }
    }
}

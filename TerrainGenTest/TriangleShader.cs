using OpenTK.Graphics.OpenGL;
using OpenTKTK.Scene;
using OpenTKTK.Shaders;
using OpenTKTK.Utils;

namespace TerrainGenTest
{
    class TriangleShader : ShaderProgram3D<Camera>
    {
        public TriangleShader()
        {
            PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType.Triangles;
        }

        protected override void ConstructVertexShader(OpenTKTK.Utils.ShaderBuilder vert)
        {
            base.ConstructVertexShader(vert);

            vert.AddAttribute(ShaderVarType.Vec3, "in_vertex");
            vert.AddVarying(ShaderVarType.Float, "var_height");

            vert.Logic = @"
                void main(void)
                {
                    var_height = in_vertex.y;

                    gl_Position = proj * view * vec4(in_vertex.x, in_vertex.y * 256, in_vertex.z, 1);
                }
            ";
        }

        protected override void ConstructFragmentShader(ShaderBuilder frag)
        {
            base.ConstructFragmentShader(frag);

            frag.Logic = @"
                void main(void)
                {
                    out_colour = vec4(var_height, var_height, var_height, 1);
                }
            ";
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            AddAttribute("in_vertex", 3);
        }

        protected override void OnBegin()
        {
            base.OnBegin();

            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            GL.Disable(EnableCap.DepthTest);
        }
    }
}

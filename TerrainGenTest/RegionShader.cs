using OpenTKTK.Shaders;
using OpenTKTK.Utils;

namespace TerrainGenTest
{
    class RegionShader : ShaderProgram2D
    {        
        protected override void ConstructVertexShader(ShaderBuilder vert)
        {
            base.ConstructVertexShader(vert);

            vert.AddAttribute(ShaderVarType.Vec3, "in_vertex");
            vert.AddVarying(ShaderVarType.Float, "var_height");
            vert.Logic = @"
                void main(void)
                {
                    var_height = in_vertex.y;
                    gl_Position = in_vertex.xz;
                }
            ";
        }

        protected override void ConstructFragmentShader(ShaderBuilder frag)
        {
            base.ConstructFragmentShader(frag);

            frag.Logic = @"
                void main(void)
                {
                    out_colour = vec4(var_height, 0, 0, 1);
                }
            ";
        }

        public RegionShader()
        {
            PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType.Triangles;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            AddAttribute("in_vertex", 3);
        }
    }
}

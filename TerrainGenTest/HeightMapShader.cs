namespace TerrainGenTest
{
    class HeightMapShader : TerrainGenerationShader
    {
        protected override void ConstructFragmentShader(OpenTKTK.Utils.ShaderBuilder frag)
        {
            base.ConstructFragmentShader(frag);

            frag.Logic = @"
                void main(void)
                {
                    vec2 invres = vec2(1 / screen_resolution.x, 1 / screen_resolution.y);

                    float c = texture2D(terrain, var_texcoord).r;
                    float l = texture2D(terrain, var_texcoord + vec2(-1, 0) * invres).r;
                    float r = texture2D(terrain, var_texcoord + vec2( 1, 0) * invres).r;
                    float t = texture2D(terrain, var_texcoord + vec2(0, -1) * invres).r;
                    float b = texture2D(terrain, var_texcoord + vec2(0,  1) * invres).r;

                    vec3 vx = normalize(vec3(2, 0, r - l));
                    vec3 vy = normalize(vec3(0, 2, b - t));

                    vec3 norm = cross(vx, vy) * 4 + vec3(0.5, 0.5, 0.5);

                    gl_FragColor = vec4(norm.x, norm.y, c, 1);
                }
            ";
        }
    }
}

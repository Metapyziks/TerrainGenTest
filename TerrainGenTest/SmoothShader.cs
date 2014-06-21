namespace TerrainGenTest
{
    class SmoothShader : TerrainGenerationShader
    {
        protected override void ConstructFragmentShader(OpenTKTK.Utils.ShaderBuilder frag)
        {
            base.ConstructFragmentShader(frag);

            frag.Logic = @"
                void main(void)
                {
                    vec2 invres = vec2(1 / screen_resolution.x, 1 / screen_resolution.y);

                    float cur = texture2D(terrain, var_texcoord).r;

                    float avg = 0, tot = 0;
                    for (int x = -3; x <= 3; ++x) {
                        for (int y = -3; y <= 3; ++y) {
                            if (x == 0 && y == 0) continue;

                            vec2 diff = vec2(x, y);
                            float mul = 1 / length(diff);                            

                            avg += texture2D(terrain, var_texcoord + diff * invres).r * mul;
                            tot += mul;
                        }
                    }

                    float val = max(0, cur + (avg / tot - cur) * 0.5);
                    out_colour = vec4(val, val, val, val);
                }
            ";
        }
    }
}

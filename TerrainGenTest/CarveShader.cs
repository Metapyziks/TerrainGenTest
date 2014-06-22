namespace TerrainGenTest
{
    class CarveShader : TerrainGenerationShader
    {
        protected override void ConstructFragmentShader(OpenTKTK.Utils.ShaderBuilder frag)
        {
            base.ConstructFragmentShader(frag);

            frag.Logic = @"
                void main(void)
                {
                    vec2 invres = vec2(1 / screen_resolution.x, 1 / screen_resolution.y);

                    vec4 cur = vec4(texture2D(gendata, var_texcoord).rgb, texture2D(terrain, var_texcoord).r);

                    vec2 grad = vec2(0, 0);
                    float dif = 0;
                    for (int x = -3; x <= 3; ++x) {
                        for (int y = -3; y <= 3; ++y) {
                            if (x == 0 && y == 0) continue;

                            vec2 diff = vec2(x, y);
                            float val = texture2D(terrain, var_texcoord + diff * invres).r;
                            float len = length(diff);
                            
                            if (val >= cur.a) continue;

                            grad += diff * (cur.a - val) / len;
                            dif += (cur.a - val) / len;
                        }
                    }

                    float slip = cur.a * dif * (cur.b * 0.6 + 0.2) * (0.1 + cur.r * 0.3);
                    float val = max(0, cur.a - slip);
                    out_colour = vec4(val, val, val, val);
                }
            ";
        }
    }
}

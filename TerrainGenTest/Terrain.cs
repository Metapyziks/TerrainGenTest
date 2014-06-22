using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using LibNoise.Primitive;

using OpenTK;

using OpenTKTK.Shaders;
using OpenTKTK.Textures;
using OpenTKTK.Utils;

namespace TerrainGenTest
{
    sealed class Terrain
    {
        const int GenDataScale = 4;

        public BitmapTexture2D GenData { get { return _genData; } }
        public AlphaTexture2D Texture { get { return (AlphaTexture2D) _front.Texture; } }

        private BitmapTexture2D _genData;
        private FrameBuffer _front;
        private FrameBuffer _back;

        private List<Region> _regions;

        private VertexBuffer _vb;

        public int Width { get { return Texture.Width; } }
        public int Height { get { return Texture.Height; } }

        public Terrain(int width, int height)
        {
            _genData = new BitmapTexture2D(width / GenDataScale, height / GenDataScale);

            _front = new FrameBuffer(new AlphaTexture2D(width, height));
            _back = new FrameBuffer(new AlphaTexture2D(width, height));
        }
        private float GetRegionHeight(SimplexPerlin perlin, float x, float y)
        {
            float a0 = perlin.GetValue(x / 32f, y / 32f) * 0.5f + 0.5f;
            float a1 = perlin.GetValue(x / 128f + 23f, y / 128f + 12f) * 0.5f + 0.5f;
            float a2 = perlin.GetValue(x / 512f - 39f, y / 512f - 81f) * 0.5f + 0.5f;
            float a3 = perlin.GetValue(x / 1024f, y / 1024f) * 0.5f + 0.5f;

            var pos = new Vector2((float) x / Width, (float) y / Height) - new Vector2(0.5f, 0.5f);

            float landMass = (1.5f * (a3 * 0.8f + 0.2f) * (a1 * 0.2f + 0.8f) - pos.LengthSquared * 4f).Clamp(0f, 1f);

            if (landMass * (a0 * 0.25f + 0.75f) < 0.25f) {
                return a2 * landMass * 2f * 15f / 256f;
            }

            landMass = Math.Max(0f, landMass * 4f - 1f) / 3f;
            //landMass *= landMass;

            if (landMass < 0.75f) {
                return Math.Min(1f, a0 * landMass * 8f * (47f / 256f) + 17f / 256f);
            }

            landMass = landMass * 4f - 3f;
            landMass *= landMass;

            return Math.Min(1f, a2 * a2 * a1 * a1 * landMass * 8f + 1f / 8f);
        }

        public void Generate(int seed)
        {
            const float maxDist = 24f;

            _regions = new List<Region>();

            var rand = new Random(seed);
            var perlin = new SimplexPerlin(seed, LibNoise.NoiseQuality.Standard);

            int tries = 0;
            while (tries++ < 256) {
                var pos = new Vector2(rand.NextSingle() * Width, rand.NextSingle() * Height);

                bool invalid = false;
                foreach (var region in _regions) {
                    if ((region.Center - pos).LengthSquared < maxDist * maxDist) {
                        invalid = true;
                        break;
                    }
                }

                if (invalid) continue;

                var newRegion = new Region(pos, Width, Height) {
                    Height = GetRegionHeight(perlin, pos.X, pos.Y)
                };

                foreach (var region in _regions) {
                    region.Cut(newRegion);
                    newRegion.Cut(region);
                }

                _regions.Add(newRegion);
                tries = 0;
            }

            var seedShader = new RegionShader();
            var carveShader = new CarveShader();
            var smoothShader = new SmoothShader();

            seedShader.SetScreenSize(Width, Height);

            _vb = new VertexBuffer(3);
            _vb.SetData(GetRegionVerts());

            _front.Begin();
            _vb.Begin(seedShader);
            _vb.Render();
            _vb.End();
            _front.End();

            var bmp = _genData.Bitmap;
            Console.WriteLine("Seeding");

            for (int x = 0; x < Width / GenDataScale; ++x) {
                for (int y = 0; y < Height / GenDataScale; ++y) {
                    int sx = x * GenDataScale;
                    int sy = y * GenDataScale;
                    
                    int a = 255;
                    int r = (int) (perlin.GetValue(sx / 8f + 12f, sy / 8f - 9f) * 128 + 127);
                    int g = (int) (perlin.GetValue(sx / 64f - 17f, sy / 64f + 3f) * 128 + 127);
                    int b = (int) (perlin.GetValue(sx / 256f - 41f, sy / 256f - 19f) * 128 + 127);

                    bmp.SetPixel(x, y, Color.FromArgb(
                        a.Clamp(0, 255),
                        r.Clamp(0, 255),
                        g.Clamp(0, 255),
                        b.Clamp(0, 255)));
                }
            }

            GenData.Invalidate();

            for (int i = 0; i < 64; ++i) {
                ShaderPass(carveShader);
            }

            for (int i = 0; i < 2; ++i) {
                ShaderPass(smoothShader);
            }

            //for (int i = 0; i < 16; ++i) {
            //    ShaderPass(carveShader);
            //}

            //for (int i = 0; i < 2; ++i) {
            //    ShaderPass(smoothShader);
            //}

            smoothShader.Render(this);
        }

        public Vector3[] GetRegionVerts()
        {
            return _regions
                .SelectMany(x => x.GetAreaVerts3D())
                .ToArray();
        }

        public void RenderRegions(ShaderProgram shader)
        {
            _vb.Begin(shader);
            _vb.Render();
            _vb.End();
        }

        public void ShaderPass<T>(T shader)
            where T : TerrainGenerationShader
        {
            _back.Begin();
            shader.Render(this);
            _back.End();

            var temp = _front;
            _front = _back;
            _back = temp;
        }

        public void Save(String fileName)
        {
            var tex = Texture;
            tex.Download();

            using (var file = File.Create(fileName)) {
                using (var writer = new BinaryWriter(file)) {
                    writer.Write((ushort) Width);
                    writer.Write((ushort) Height);

                    for (int y = 0; y < Height; ++y) {
                        for (int x = 0; x < Width; ++x) {
                            writer.Write(tex[x, y]);
                        }
                    }

                    writer.Flush();
                }
            }
        }
    }
}

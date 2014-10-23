using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTKTK.Scene;

namespace TerrainGenTest
{
    class Program : GameWindow
    {
        static void Main(string[] args)
        {
            var options = new ToolkitOptions();
            options.Backend = PlatformBackend.PreferNative;

            using (var toolkit = Toolkit.Init(options)) {
                using (var app = new Program()) {
                    app.Run();
                }
            }
        }

        private Terrain _terrain;
        private Camera _camera;
        private LineShader _lineShader;
        private TriangleShader _triShader;

        public Program()
            : base(1024, 576)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _terrain = new Terrain(2048, 2048);
            _terrain.Generate((int) DateTime.Now.Ticks);
            _terrain.Save("terrain.map");

            _camera = new Camera(Width, Height, MathHelper.PiOver3, 1f, 4096f);
            _camera.Position = new Vector3(1024f, 1024, 2048f + 512f);
            _camera.Pitch = MathHelper.PiOver4;

            _lineShader = new LineShader {
                Camera = _camera
            };

            _triShader = new TriangleShader {
                Camera = _camera
            };

            GL.ClearColor(Color4.Black);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Keyboard[Key.W]) {
                _camera.Position -= Vector3.UnitZ * 2f;
            }

            if (Keyboard[Key.S]) {
                _camera.Position += Vector3.UnitZ * 2f;
            }

            if (Keyboard[Key.A]) {
                _camera.Position -= Vector3.UnitX * 2f;
            }

            if (Keyboard[Key.D]) {
                _camera.Position += Vector3.UnitX * 2f;
            }

            if (Keyboard[Key.Q]) {
                _camera.Yaw -= MathHelper.Pi / 60f;
            }

            if (Keyboard[Key.E]) {
                _camera.Yaw += MathHelper.Pi / 60f;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _terrain.RenderRegions(_triShader);

            SwapBuffers();
        }
    }
}

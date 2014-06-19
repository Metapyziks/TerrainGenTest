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
            using (var app = new Program()) {
                app.Run();
            }
        }

        private Terrain _terrain;

        private Camera _camera;
        private ParticleShader _shader;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _terrain = new Terrain(4, 4);
            _terrain.Add(Material.Basalt, 1700f, 64);
            _terrain.Add(Material.Water, 295f, 2);

            _camera = new Camera(Width, Height);
            _camera.Position = new Vector3(2f, 8f, 5f);
            _camera.Pitch = MathHelper.PiOver4;

            _shader = new ParticleShader {
                Camera = _camera
            };

            GL.ClearColor(Color4.Black);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            _terrain.Simulate();

            if (Keyboard[Key.W]) {
                _camera.Position -= Vector3.UnitZ;
            }

            if (Keyboard[Key.S]) {
                _camera.Position += Vector3.UnitZ;
            }

            if (Keyboard[Key.A]) {
                _camera.Position -= Vector3.UnitX;
            }

            if (Keyboard[Key.D]) {
                _camera.Position += Vector3.UnitX;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Begin(true);
            _shader.Render(_terrain);
            _shader.End();

            SwapBuffers();
        }
    }
}

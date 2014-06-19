using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK;

namespace TerrainGenTest
{
    class Sector : IEnumerable<Particle>
    {
        private Terrain _terrain;

        public int X { get; private set; }
        public int Y { get; private set; }

        private List<Particle> _particles;
        private Sector[] _neighbours;

        public Sector(Terrain terrain, int x, int y)
        {
            _terrain = terrain;

            X = x;
            Y = y;
            
            _particles = new List<Particle>();
        }

        public void SetNeighbours(Sector[] neighbours)
        {
            _neighbours = neighbours;
        }

        public void Add(Material mat, float temp, int count)
        {
            for (int i = 0; i < count; ++i) {
                var pos = new Vector3(
                    Tools.Random.NextSingle() + X,
                    Tools.Random.NextSingle() + Y,
                    Tools.Random.NextSingle());

                _particles.Add(new Particle(mat, pos, temp));
            }
        }

        public void Add(Particle part)
        {
            _particles.Add(part);
        }

        public void Simulate()
        {
            foreach (var p in _particles) {
                p.Velocity = -Vector3.UnitZ / 32f;

                foreach (var sect in _neighbours) {
                    foreach (var n in sect) {
                        var diff = _terrain.Difference(p.Position, n.Position);
                        var dist2 = diff.LengthSquared;

                        if (dist2 == 0f) continue;
                        if (dist2 > 1f / 4f) continue;

                        var visc = (n.Viscosity + p.Viscosity) / 2f;
                        var dist = (float) Math.Sqrt(dist2);

                        if (dist <= 15f / 64f) {
                            p.Velocity -= diff.Normalized() / (dist * p.Density * 4f);
                        } else {
                            p.Velocity += diff.Normalized() * (1f - visc) / (dist * 512f);
                        }
                    }
                }

                p.Velocity /= 32f;
            }
        }

        public IEnumerable<Particle> Update()
        {
            for (int i = _particles.Count - 1; i >= 0; --i) {
                var part = _particles[i];
                if (part.Update(X, Y)) {
                    _particles.RemoveAt(i);
                    yield return part;
                }
            }
        }

        public IEnumerator<Particle> GetEnumerator()
        {
            return _particles.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _particles.GetEnumerator();
        }
    }
}

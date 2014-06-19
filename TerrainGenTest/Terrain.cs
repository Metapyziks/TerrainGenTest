using System.Collections.Generic;
using System.Linq;

using OpenTK;

namespace TerrainGenTest
{
    sealed class Terrain : IEnumerable<Sector>
    {
        private Sector[] _sectors;

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Sector this[int x, int y]
        {
            get
            {
                return _sectors[GetIndex(x, y)];
            }
        }

        private int GetX(int index)
        {
            return index % Width;
        }

        private int GetY(int index)
        {
            return index / Width;
        }

        private int GetIndex(int x, int y)
        {
            return x.Wrap(Width) + y.Wrap(Height) * Width;
        }

        public Terrain(int width, int height)
        {
            Width = width;
            Height = height;

            _sectors = new Sector[width * height];

            for (int i = 0; i < _sectors.Length; ++i) {
                _sectors[i] = new Sector(this, GetX(i), GetY(i));
            }

            for (int i = 0; i < _sectors.Length; ++i) {
                int x = GetX(i);
                int y = GetY(i);

                _sectors[i].SetNeighbours(new[] {
                    this[x - 1, y - 1],
                    this[x + 0, y - 1],
                    this[x + 1, y - 1],
                    this[x - 1, y + 0],
                    this[x + 0, y + 0],
                    this[x + 1, y + 0],
                    this[x - 1, y + 1],
                    this[x + 0, y + 1],
                    this[x + 1, y + 1]
                });
            }
        }

        public Vector3 Difference(Vector3 a, Vector3 b)
        {
            Vector3 diff = b - a;

            if (diff.X >= Width >> 1)
                diff.X -= Width;
            else if (diff.X < -(Width >> 1))
                diff.X += Width;

            if (diff.Y >= Height >> 1)
                diff.Y -= Height;
            else if (diff.Y < -(Height >> 1))
                diff.Y += Height;

            return diff;
        }

        public void Add(Material mat, float temp, int count)
        {
            foreach (var sector in _sectors) {
                sector.Add(mat, temp, count);
            }
        }

        public void Simulate()
        {
            foreach (var sector in _sectors) {
                sector.Simulate();
            }

            for (int i = 0; i < _sectors.Length; ++i) {
                int x = GetX(i);
                int y = GetY(i);

                var sector = _sectors[i];
                foreach (var part in sector.Update()) {
                    int dx, dy;
                    part.GetSectorPos(out dx, out dy);
                    _sectors[GetIndex(dx, dy)].Add(part);
                }
            }
        }

        public IEnumerator<Sector> GetEnumerator()
        {
            return _sectors.AsEnumerable<Sector>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _sectors.AsEnumerable<Sector>().GetEnumerator();
        }
    }
}

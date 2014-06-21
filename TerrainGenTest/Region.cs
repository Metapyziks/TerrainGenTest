using System.Collections.Generic;
using System.Linq;

using OpenTK;

namespace TerrainGenTest
{
    class Region
    {
        private List<Vector2> _verts;

        public Vector2 Center { get; private set; }

        public float Height { get; set; }

        public Region(Vector2 center, int worldWidth, int worldHeight)
        {
            Center = center;

            _verts = new List<Vector2> {
                new Vector2(0, 0),
                new Vector2(worldWidth, 0),
                new Vector2(worldWidth, worldHeight),
                new Vector2(0, worldHeight)
            };
        }

        private bool IsCut(Vector2 vert, Vector2 mid, Vector2 nml)
        {
            return Vector2.Dot(vert - mid, nml) < 0f; 
        }

        private Vector2 Intersection(Vector2 a, Vector2 b, Vector2 mid, Vector2 nml)
        {
            var diff = b - a;
            return a + diff * Vector2.Dot(nml, mid - a) / Vector2.Dot(nml, diff);
        }

        public void Cut(Region other)
        {
            var mid = (Center + other.Center) / 2f;
            var nml = (Center - other.Center);

            var found0 = false;
            var a0 = Vector2.Zero;
            var b0 = Vector2.Zero;

            var found1 = false;
            var a1 = Vector2.Zero;
            var b1 = Vector2.Zero;
            
            for (int i = 0; i < _verts.Count; ++i) {
                var a = _verts[i];
                var b = _verts[(i + 1) % _verts.Count];

                if (!found0 && !IsCut(a, mid, nml) && IsCut(b, mid, nml)) {
                    found0 = true;
                    a0 = a;
                    b0 = b;
                } else if (!found1 && IsCut(a, mid, nml) && !IsCut(b, mid, nml)) {
                    found1 = true;
                    a1 = a;
                    b1 = b;
                }
            }

            if (!found0 || !found1) return;

            int index = -1;
            for (int i = _verts.Count - 1; i >= 0; --i) {
                if (IsCut(_verts[i], mid, nml)) {
                    index = i;
                    _verts.RemoveAt(i);
                }
            }

            _verts.Insert(index + 0, Intersection(a0, b0, mid, nml));
            _verts.Insert(index + 1, Intersection(a1, b1, mid, nml));
        }

        public Vector2[] GetPerimeterVerts2D()
        {
            var verts = new Vector2[_verts.Count * 2];

            for (int i = 0; i < _verts.Count; ++i) {
                verts[i * 2 + 0] = _verts[i];
                verts[i * 2 + 1] = _verts[(i + 1) % _verts.Count];
            }

            return verts;
        }

        public Vector3[] GetPerimeterVerts3D()
        {
            var c = new Vector3(Center.X, Height, Center.Y);

            return GetPerimeterVerts2D()
                .Select(v => new Vector3(v.X, Height, v.Y))
                .Concat(new Vector3[] {
                    c - Vector3.UnitX,
                    c + Vector3.UnitX,
                    c - Vector3.UnitZ,
                    c + Vector3.UnitZ
                })
                .ToArray();
        }

        public Vector2[] GetAreaVerts2D()
        {
            var verts = new Vector2[_verts.Count * 3];

            for (int i = 0; i < _verts.Count; ++i) {
                verts[i * 3 + 0] = _verts[i];
                verts[i * 3 + 1] = Center;
                verts[i * 3 + 2] = _verts[(i + 1) % _verts.Count];
            }

            return verts;
        }

        public Vector3[] GetAreaVerts3D()
        {
            return GetAreaVerts2D()
                .Select(v => new Vector3(v.X, Height, v.Y))
                .ToArray();
        }
    }
}

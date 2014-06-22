using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;

namespace TerrainGenTest
{
    class Biome
    {
        public static readonly Biome Ocean = new Biome("Ocean") {
            Color = Color4.DarkBlue,
            Traversability = 0f
        };

        public static readonly Biome Shallows = new Biome("Shallows") {
            Color = Color4.CornflowerBlue,
            Traversability = 0.125f
        };

        public static readonly Biome Beach = new Biome("Beach") {
            Color = Color4.LightGoldenrodYellow,
            Traversability = 0.5f
        };

        public static readonly Biome Mountains = new Biome("Mountains") {
            Color = Color4.Gray,
            Traversability = 0.125f
        };

        public static readonly Biome Desert = new Biome("Desert") {
            Color = Color4.SandyBrown,
            Traversability = 0.75f
        };

        public static readonly Biome Forest = new Biome("Forest") {
            Color = Color4.DarkGreen,
            Traversability = 0.75f
        };

        public static readonly Biome Grassland = new Biome("Grassland") {
            Color = Color4.PaleGreen,
            Traversability = 1f
        };

        public static readonly Biome Hills = new Biome("Hills") {
            Color = Color4.ForestGreen,
            Traversability = 0.75f
        };

        public String Name { get; private set; }

        public Color4 Color { get; set; }

        public float Traversability { get; set; }

        public Biome(String name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

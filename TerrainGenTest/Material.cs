using OpenTK.Graphics;

namespace TerrainGenTest
{
    class Material
    {
        public static readonly Material Basalt = new Material {
            Solidus = 1300f,
            Liquidus = 1600f,
            Viscosity = 0.5f,
            DensitySolid = 3f,
            DensityLiquid = 2.7f,
            HeatCapacitySolid = 0.84f,
            HeatCapacityLiquid = 1.45f,
            Colour = Color4.Gray
        };

        public static readonly Material Water = new Material {
            Solidus = 265f,
            Liquidus = 280f,
            Viscosity = 0f,
            DensitySolid = 0.917f,
            DensityLiquid = 1f,
            HeatCapacitySolid = 2.11f,
            HeatCapacityLiquid = 4.19f,
            Colour = Color4.CornflowerBlue
        };

        public float Solidus { get; set; }

        public float Liquidus { get; set; }

        public float Viscosity { get; set; }

        public float DensitySolid { get; set; }

        public float DensityLiquid { get; set; }

        public float HeatCapacitySolid { get; set; }

        public float HeatCapacityLiquid { get; set; }

        public Color4 Colour { get; set; }
    }
}

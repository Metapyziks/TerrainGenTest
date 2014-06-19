using System;
using System.Collections.Generic;
using OpenTK;

namespace TerrainGenTest
{
    class Particle
    {
        private Vector3 _position;

        public Material Material { get; private set; }

        public Vector3 Position { get { return _position; } }

        public float Temperature { get; private set; }

        public Vector3 Velocity { get; set; }

        public float TemperatureDelta { get; set; }

        public float Density { get; private set; }
        public float HeatCapacity { get; private set; }
        public float Viscosity { get; private set; }

        public Particle(Material mat, Vector3 pos, float temp)
        {
            Material = mat;
            _position = pos;
            Temperature = temp;

            Update(0, 0);
        }

        public void GetSectorPos(out int x, out int y)
        {
            x = (int) Math.Floor(_position.X);
            y = (int) Math.Floor(_position.Y);
        }

        public bool Update(int sx, int sy)
        {
            if (Velocity.LengthSquared > 1f / 4f) {
                Velocity /= Velocity.Length * 4f;
            }

            _position += Velocity;
            Temperature += TemperatureDelta;

            Velocity = Vector3.Zero;
            TemperatureDelta = 0f;

            if (Temperature <= Material.Solidus) {
                Density = Material.DensitySolid;
                HeatCapacity = Material.HeatCapacitySolid;
                Viscosity = 1f;
            } else if (Temperature >= Material.Liquidus) {
                Density = Material.DensityLiquid;
                HeatCapacity = Material.HeatCapacityLiquid;
                Viscosity = Material.Viscosity;
            } else {
                float delta = (Temperature - Material.Solidus) / (Material.Liquidus - Material.Solidus);

                Density = Material.DensitySolid + (Material.DensityLiquid - Material.DensitySolid) * delta;
                HeatCapacity = Material.HeatCapacitySolid + (Material.HeatCapacityLiquid - Material.HeatCapacitySolid) * delta;
                Viscosity = (1f - delta) + Material.Viscosity * delta;
            }

            return Position.X < sx || Position.X >= sx + 1 || Position.Y < sy || Position.Y >= sy + 1;
        }
    }
}

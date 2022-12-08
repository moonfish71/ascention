using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ascention.Core
{
    public class massBody : Particle
    {
        public float density = 1f;
        public float mass = 1f;

        public massBody(Vector2 pos, Vector2 vel, float scal, float dense, string path) : base(pos, vel, scal, path)
        {
            density = dense;
            mass = density * (scale * tex.Width * tex.Height);
        }
    }
    public class satellite : massBody
    {
        private static readonly List<satellite> satellites = new List<satellite>();
        private static float G = ModMath.SciNotate(6.674f, -11);
        int updateTimer = 0;

        public satellite(Vector2 pos, Vector2 vel, float scal, float dense, string path) : base(pos, vel, scal, dense, path)
        {
        }

        public override void Update()
        {
            base.Update();
            updateTimer++;
            if (updateTimer >= 10)
            {
                updateNear();
                collectiveGrav();
                updateTimer = 0;
            }
        }

        public void updateNear()
        {
            float maxDelta = 600f;
            foreach (satellite body in ParticleManager.refList)
            {
                if (ModMath.Magnitude(this.position) - ModMath.Magnitude(body.position) < maxDelta)
                {
                    satellites.Add(body);
                }
            }

            foreach (satellite body in satellites)
            {
                if (ModMath.Magnitude(this.position) - ModMath.Magnitude(body.position) >= maxDelta)
                {
                    satellites.Remove(body);
                }
            }
        }

        public void collectiveGrav()
        {
            _ = Vector2.Zero;
            foreach (satellite body in satellites)
            {
                Vector2 gravTotal = UniGrav(this, body);

                this.velocity += gravTotal / this.mass;
                body.velocity += gravTotal / body.mass;
            }
        }

        public static Vector2 UniGrav(satellite body1, satellite body2) //Newton Time
        {
            Vector2 delta = body1.position - body2.position;

            double dirIndex;
            if (Math.Sign(delta.X) == 1)
            {
                dirIndex = 0;
            }
            else
            {
                dirIndex = Math.PI;
            }

            float grav = G * (body1.mass * body2.mass / ModMath.Magnitude(delta * delta));
            Vector2 dirGrav = new Vector2(grav, 0).RotatedBy(Math.Atan(delta.Y / delta.X) + dirIndex);
            return dirGrav;
        }
    }
}

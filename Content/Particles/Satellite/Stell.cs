using Microsoft.Xna.Framework;
using Terraria;
using ascention.Core;
using static ascention.Core.AssetDirector;
using Terraria.ID;

namespace ascention.Content.Particles.Satellite
{
    public class Stell: Particle
    {
        private int lifeTimer = 0;
        private readonly int MaxLifeTimer = 2000;

        public Stell(Vector2 pos, Vector2 vel) : base(pos, vel, 1f, SatelliteTx + "Stell")
        {
            scale = Main.rand.NextFloat(0.8f, 1.2f);
        }

        public override void Update()
        {
            base.Update();
            lifeTimer++;

            for(int i = 0; i < 10; i++)
            {
                int star = Dust.NewDust(position, tex.Width, tex.Height, DustID.YellowStarDust, velocity.X, velocity.Y);
                Dust dust = Main.dust[star];
                dust.noGravity = true; 
            }

            if (lifeTimer > MaxLifeTimer)
                kill = true;
        }
        public override void Draw()
        {
            drawColor = Color.White;
            
            base.Draw();
        }
    }
}

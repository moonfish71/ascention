using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace ascention.Core
{
    public class noGravPlayer : ModPlayer
    {
        public bool noGravActive;
        public float acc = 0.3f;
        public float maxVel = 10f;

        private bool NoGActive()
        {
            if (noGravActive)
                return true;
            
            return false;
        }

        public override void PostUpdate()
        {
            noGravActive = NoGActive();
            if (!noGravActive)
                return;

            Player.maxFallSpeed = 0;
            Player.gravity = 0;
            Player.velocity.Y -= 0.42f;

            if(Player.wingTime > 0 || Player.rocketBoots > 0)
            {
                Player.wingTime = 0;
                Player.rocketBoots = 0;
            }

            if (Player.controlLeft)
            {
                Player.velocity.X -= acc;
            }
            if (Player.controlRight)
            {
                Player.velocity.X += acc;
            }
            if (Player.controlUp)
            {
                Player.velocity.Y -= acc;
            }
            if (Player.controlDown)
            {
                Player.velocity.Y += acc;
            }

            Player.velocity *= .95f;
        }
        public override void UpdateDead()
        {
            noGravActive = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ModLoader;

namespace ascention.Core.Systems
{
    public class CameraSystem : ModSystem
    {
        public float shake = 0;

        public override void ModifyScreenPosition()
        {
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Main.LocalPlayer.position, Main.rand.NextFloat(3.14f).ToRotationVector2(), shake, 15f, 60, 2000, "AscentionShake"));
            
            if (shake > 0)
                shake--;
        }
    }
}

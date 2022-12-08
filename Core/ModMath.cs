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
    internal class ModMath
    {
        public static float Magnitude(Vector2 vector)
        {
            return (float)Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2));
        }

        public static float SciNotate(float baseVal, float scale)
        {
            return baseVal * (float)Math.Pow(10,scale);
        }

        public static Vector2 randomArea(float width, float height)
        {
            return new Vector2(Main.rand.NextFloat(-width, width), Main.rand.NextFloat(-height, height));
        }

        public static bool isFactor(float value, float baseValue)
        {
            if(Math.Floor(value / baseValue) == value / baseValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

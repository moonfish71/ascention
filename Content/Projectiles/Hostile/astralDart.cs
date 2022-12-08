using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using static ascention.Core.AssetDirector;
using ascention.Core;

namespace ascention.Content.Projectiles.Hostile
{
    public class astralDart : ModProjectile
    {
        public override string Texture => HostileProjectile + "astralDart";

        Texture2D tex = (Texture2D)ModContent.Request<Texture2D>(HostileProjectile + "astralDart");

        bool timeSet = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Dart");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }

        public float endX
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float endY
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public float startX
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public float startY
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        public override void AI()
        {
            if (!timeSet && ModMath.Magnitude(Projectile.velocity) > 0)
            {
                timeSet = true;
                Projectile.timeLeft = (int)(ModMath.Magnitude(new Vector2(startX - endX, startY - endY)) / ModMath.Magnitude(Projectile.velocity));
            }
            else if (!timeSet)
            {
                timeSet = true;
                Projectile.timeLeft = 60;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return true;
        }
    }
}

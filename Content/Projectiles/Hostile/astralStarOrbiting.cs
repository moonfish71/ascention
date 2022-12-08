using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using static ascention.Core.AssetDirector;
using Terraria.ID;
using ascention.Core;
using ascention.Content.NPCs.Bosses.Aster;

namespace ascention.Content.Projectiles.Hostile
{
    public class astralStarOrbiting: ascentionProjectile
    {
        public override string Texture => HostileProjectile + "astralStar";

        Texture2D speedTex = (Texture2D)ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StarWrath);

        int animTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("orbiting Astral Star");
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = Projectile.width;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
            Projectile.aiStyle = -1;
        }

        public ref float timer => ref Projectile.ai[0];

        public ref float baseNPC => ref Projectile.ai[1];

        public ref float rotationIndex => ref Projectile.localAI[0];

        public override void Init()
        {
            drawShaded = false;
            tex = (Texture2D)ModContent.Request<Texture2D>(HostileProjectile + "astralStar");
        }

        public override void AI()
        {
            NPC aster = Main.npc[(int)baseNPC];
            const float tau = 2 * (float)Math.PI;
            timer++;

            if (aster.ModNPC is Aster body)
            {
                Projectile.Center = aster.Center + new Vector2((float)(body.scale * body.starRadiusMult * 125 * Math.Cos((tau / 5 * rotationIndex) + timer / 20)), (float)(body.scale * body.starRadiusMult * 125 * Math.Sin((tau / 5 * rotationIndex) + timer / 20) * Math.Sin(body.rotationVector))).RotatedBy(body.ZrotationVector.ToRotation());
                Projectile.velocity = aster.velocity;

                parallax = body.parallax + (float)(body.scale * body.starRadiusMult * .15 * -Math.Sin((tau / 5 * rotationIndex) + timer / 20) * Math.Cos(body.rotationVector));
                Projectile.timeLeft = 100;
            }
            
            if(aster.life <= 0)
            {
                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5 * (int)scale; i++)
            {
                Dust.NewDust(drawPosition - new Vector2(Projectile.width / 2 * (int)scale, Projectile.height / 2 * (int)scale), Projectile.width * (int)scale, Projectile.height * (int)scale, DustID.GemAmethyst, Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle speedSource = new Rectangle(0, 0, (int)speedTex.Size().X, (int)speedTex.Size().Y);

            float speedScale = (float)(2 / (1 + Math.Pow(Math.E, -ModMath.Magnitude(Projectile.velocity) / 4))) - 1;

            Vector2 drawPosition2 = drawPosition + new Vector2(0, 25 * Projectile.scale * speedScale * scale).RotatedBy(Projectile.velocity.ToRotation() + (MathHelper.PiOver2 * 1));

            Main.spriteBatch.Draw(speedTex, drawPosition2 - Main.screenPosition, speedSource, Color.White * (Projectile.Opacity / 16 * ModMath.Magnitude(Projectile.velocity / 2)), Projectile.velocity.ToRotation() + (MathHelper.PiOver2 * 3), speedTex.Size() / 2, (float)(Projectile.scale * 1f * speedScale * scale + (0.1 * Math.Sin(animTimer / 7 * MathHelper.PiOver2))), SpriteEffects.None, 0f);

            return base.PreDraw(ref lightColor);
        }
    }
}

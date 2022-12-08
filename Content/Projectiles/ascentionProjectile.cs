using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ascention.Core;
using static ascention.Core.AssetDirector;
using Terraria.DataStructures;

namespace ascention.Content.Projectiles
{
    public abstract class ascentionProjectile : ModProjectile
    {
        public Texture2D tex = (Texture2D)ModContent.Request<Texture2D>(PlaceholderTx);

        public Rectangle source = new Rectangle(0, 0, 102, 106);

        public float parallax = 1f;
        public float scale = 1;
        public Vector2 drawPosition;
        public int frameIndex = 0;
        public int maxFrames = 1;

        public bool drawShaded = true;
        public override void OnSpawn(IEntitySource source)
        {
            Init();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color drawColor = Color.White;

            source = new Rectangle(0, 0, tex.Width, tex.Height);

            source.Location = new Point(source.Location.X, frameIndex * tex.Height);

            scale = parallax * parallax;

            Vector2 playerPos = Main.LocalPlayer.MountedCenter;
            Vector2 offset = (playerPos - Projectile.Center) * -((scale/2) - 0.5f);

            drawPosition = Projectile.Center + offset;

            Color lightColour = Lighting.GetColor((int)(drawPosition.X / 16f), (int)(drawPosition.Y / 16f));
            Color frontColour = (Projectile.Center.Y / 16f < Main.worldSurface) ? Main.ColorOfTheSkies : new Color(85, 85, 85);

            if (drawShaded)
            {
                drawColor = Color.Lerp(lightColour, frontColour, (parallax - 0.25f) / 1.25f);
            }

            Main.spriteBatch.Draw(tex, drawPosition - Main.screenPosition, source, drawColor * Projectile.Opacity, Projectile.rotation, new Vector2(tex.Size().X / 2, tex.Size().Y / (2 * maxFrames)), scale, SpriteEffects.None, 0f);

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (parallax > 1)
            {
                overPlayers.Add(index);
                behindNPCsAndTiles.Remove(index);
                behindNPCs.Remove(index);
                Projectile.hide = false;
            }
            else if (parallax < 1)
            {
                behindNPCsAndTiles.Add(index);
                overPlayers.Remove(index);
                behindNPCs.Remove(index);
                Projectile.hide = true;
            }
            else
            {
                behindNPCs.Add(index);
                overPlayers.Remove(index);
                behindNPCsAndTiles.Remove(index);
                Projectile.hide = false;
            }
        }

        public virtual void Init()
        {
        }
    }
}

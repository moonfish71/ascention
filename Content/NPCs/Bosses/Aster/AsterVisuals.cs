using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Audio;
using static ascention.Core.AssetDirector;
using ascention.Core;

namespace ascention.Content.NPCs.Bosses.Aster
{
    public sealed partial class Aster : ModNPC
    {
        private bool rotate = true;
        private float rotationRate = 0.2f;

        Texture2D tex = (Texture2D)ModContent.Request<Texture2D>(AsterTex + "Aster");

        Texture2D glowTex = (Texture2D)ModContent.Request<Texture2D>(AsterTex + "Aster_glowmask");

        Texture2D speedTex = (Texture2D)ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StarWrath);

        public Rectangle source = new Rectangle(0, 0, 102, 106);

        public float parallax = 1f;
        public float scale = 1;
        Vector2 drawPosition = Vector2.Zero;
        int frameIndex = 0;

        float animTimer = 0;

        bool glowActive = true;

        public override void FindFrame(int frameHeight)
        {
            if (rotate)
            {
                if (NPC.rotation == 0)
                {
                    if (NPC.frame.Y == frameHeight)
                    {
                        NPC.frame.Y = 0;
                        frameIndex = 0;
                    }
                    else
                    {
                        NPC.frame.Y = frameHeight;
                        frameIndex = 1;
                    }
                }
                source.Location = new Point(source.Location.X, frameIndex * 106);
            }
            else
            {
                NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            scale = parallax * parallax;

            float speedScale = (float)(2 / (1 + Math.Pow(Math.E, -ModMath.Magnitude(NPC.velocity) / 4))) - 1;

            animTimer++;

            if(animTimer / 7 > MathHelper.TwoPi)
            {
                animTimer = 0;
            }

            Rectangle speedSource = new Rectangle(0, 0, (int)speedTex.Size().X, (int)speedTex.Size().Y);

            Vector2 playerPos = Main.LocalPlayer.MountedCenter;
            Vector2 offset = (playerPos - NPC.Center) * -((scale / 2) - 0.5f);

            drawPosition = NPC.Center + offset;

            Vector2 drawPosition2 = drawPosition + new Vector2(0, 150 * scale * speedScale).RotatedBy(NPC.velocity.ToRotation() + (MathHelper.PiOver2));

            Color lightColour = Lighting.GetColor((int)(drawPosition.X / 16f), (int)(drawPosition.Y / 16f));
            Color frontColour = (NPC.Center.Y / 16f < Main.worldSurface) ? Main.ColorOfTheSkies : new Color(85, 85, 85);
            drawColor = Color.Lerp(lightColour, frontColour, (parallax - 0.25f) / 1.25f);

            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(speedTex, drawPosition2 - Main.screenPosition + new Vector2(20 * scale * speedScale, 0).RotatedBy((animTimer / 7) + (MathHelper.TwoPi / 3 * i)), speedSource, Color.Magenta * (NPC.Opacity / 20 * ModMath.Magnitude(NPC.velocity / 10)), NPC.velocity.ToRotation() + (MathHelper.PiOver2 * 3), speedTex.Size() / 2, scale * 4f * speedScale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(speedTex, drawPosition2 - Main.screenPosition - new Vector2(0,50 * scale * speedScale).RotatedBy(NPC.velocity.ToRotation() + (MathHelper.PiOver2)), speedSource, Color.White * (NPC.Opacity / 16 * ModMath.Magnitude(NPC.velocity / 2)), NPC.velocity.ToRotation() + (MathHelper.PiOver2 * 3), speedTex.Size() / 2, (float)(scale * 2f * speedScale + (0.1 * Math.Sin(animTimer / 7 * MathHelper.PiOver2))), SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(tex, drawPosition - Main.screenPosition, source, drawColor * NPC.Opacity, NPC.rotation, new Vector2(tex.Size().X / 2, tex.Size().Y / (2 * Main.npcFrameCount[NPC.type])), scale, SpriteEffects.None, 0f);

            if (glowActive)
            {
                Main.spriteBatch.Draw(glowTex, drawPosition - Main.screenPosition, source, Color.White * NPC.Opacity, NPC.rotation, new Vector2(glowTex.Size().X / 2, glowTex.Size().Y / (2 * Main.npcFrameCount[NPC.type])), scale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void DrawBehind(int index)
        {
            if (parallax > 1)
            {
                Main.instance.DrawCacheNPCsOverPlayers.Add(index);
                NPC.behindTiles = false;
            }
            else if (parallax < 1)
            {
                Main.instance.DrawCacheNPCsMoonMoon.Add(index);
                NPC.behindTiles = true;
            }
            else
            {
                Main.instance.DrawCacheProjsBehindNPCs.Add(index);
                NPC.behindTiles = false;
            }
        }

        public void Visuals()
        {
            if (rotate)
            {
                NPC.rotation += rotationRate;
                if (Math.Abs(NPC.rotation) > (float)Math.PI / 5)
                {
                    NPC.rotation = 0;
                }
            }
        }
    }
}

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
using static ascention.Core.AssetDirector;

namespace ascention.Core
{
    public static class ParticleManager
    {
        private static readonly List<Particle> items = new List<Particle>();
        public static readonly List<Particle> refList = new List<Particle>();

        public static void Run()
        {
            List<Particle> removals = new List<Particle>();

            foreach (var val in items)
            {
                if (Main.hasFocus && !Main.gamePaused)
                    val.Update();

                refList.Add(val);

                val.Draw();
                if (val.kill)
                    removals.Add(val);
            }

            foreach (var item in removals)
            {
                items.Remove(item);
                refList.Remove(item);
            }
        }

        public static void Unload() => items.Clear();

        public static void AddParticle(Particle item)
        {
            items.Add(item);
        }

        public static Texture2D GetTexture(string name) => ascention.Instance.Assets.Request<Texture2D>(name).Value;

        internal static TagCompound Save()
        {
            TagCompound compound = new TagCompound();
            foreach (var item in items)
            {
                if (item.SaveMe)
                {
                    var value = item.Save();
                    if (value == null)
                        continue;

                    compound.Add("particleInfo", value);
                }
            }
            return compound;
        }
    }

    public class Particle
    {
        public Vector2 position = new Vector2(0, 0);
        internal Vector2 drawPosition = new Vector2();
        public Vector2 velocity = new Vector2(0, 0);
        public float scale = 1f;
        public Rectangle source = new Rectangle();
        public Color drawColor = Color.White;
        public float rotation = 0f;

        internal bool drawLighted = true;

        public bool kill = false;

        public virtual bool SaveMe => false;

        public Vector2 Center => position + (source.Size() / 2f);

        public readonly Texture2D tex;

        public Particle(Vector2 pos, Vector2 vel, float sc, string path)
        {
            position = pos;
            velocity = vel;
            tex = ModContent.Request<Texture2D>($"{path}").Value;
            scale = sc;

            source = new Rectangle(0, 0, tex.Width, tex.Height);
        }

        public virtual void Update()
        {
            position += velocity;
        }

        public virtual void Draw()
        {
            Main.spriteBatch.Draw(tex, drawPosition - Main.screenPosition, source, drawColor, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0f);
        }

        public virtual TagCompound Save() => null;

        public virtual void Load(TagCompound info)
        {
        }

        public override string ToString() => $"{GetType().Name} at {position}\nSIZE: {scale}, SAVE: {SaveMe}, LIGHTED: {drawLighted}";
    }
}

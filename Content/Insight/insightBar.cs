using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static ascention.Core.AssetDirector;
using Terraria.DataStructures;
using Terraria.ID;
using System.IO;
using Terraria.UI;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;

namespace ascention.Content.Insight
{
    class insightBar : UIState
    {
        private UIElement area;
        public UIImage border;
        public UIImage Ibase;
        private UIText text;
        private UIText state;
        private UIImage fbLimit;
        private Color GradA;
        private Color GradB;

        InsightPlayer insightPlayer;
        private float maxPercent;

        public override void OnInitialize()
        {
            insightPlayer = new InsightPlayer();
            maxPercent = insightPlayer.maxQuotient;


            area = new UIElement();
            area.Left.Set(-area.Width.Pixels - 600, 1f);
            area.Top.Set(30, 0f);
            area.Width.Set(800, 0f);
            area.Height.Set(32, 0f);

            border = new UIImage((Texture2D)ModContent.Request<Texture2D>(BarTex + "defaultBar", AssetRequestMode.ImmediateLoad));
            border.Left.Set(240, 0f);
            border.Top.Set(50, 0f);
            border.Width.Set(32, 0f);
            border.Height.Set(368, 0f);

            Ibase = new UIImage((Texture2D)ModContent.Request<Texture2D>(BarTex + "defaultBarBase", AssetRequestMode.ImmediateLoad));
            Ibase.Left.Set(238, 0f);
            Ibase.Top.Set(410, 0f);
            Ibase.Width.Set(36, 0f);
            Ibase.Height.Set(44, 0f);

            text = new UIText("0/0", 0.8f); // text to show stat
            text.Width.Set(138, 0f);
            text.Height.Set(34, 0f);
            text.Top.Set(40, 0f);
            text.Left.Set(0, 0f);

            fbLimit = new UIImage((Texture2D)ModContent.Request<Texture2D>(BarTex + "defaultBarLimit", AssetRequestMode.ImmediateLoad));
            Ibase.Left.Set(238, 0f);
            Ibase.Top.Set(410, 0f);
            Ibase.Width.Set(36, 0f);
            Ibase.Height.Set(44, 0f);

            GradA = new Color(123, 25, 138); // A dark purple
            GradB = new Color(187, 91, 201); // A light purple

            area.Append(border);
            area.Append(Ibase);
            area.Append(text);
            area.Append(fbLimit);
            //Append(area);
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var modPlayer = Main.LocalPlayer.GetModPlayer<InsightPlayer>();
            // Calculate quotient
            float quotient = (float)modPlayer.insight / modPlayer.maxInsight; // Creating a quotient that represents the difference of your currentResource vs your maximumResource, resulting in a float of 0-1f.
            quotient = Utils.Clamp(quotient, 0f, 1f); // Clamping it to 0-1f so it doesn't go over that.

            // Here we get the screen dimensions of the barFrame element, then tweak the resulting rectangle to arrive at a rectangle within the barFrame texture that we will draw the gradient. These values were measured in a drawing program.
            Rectangle hitbox = border.GetInnerDimensions().ToRectangle();

            // Now, using this hitbox, we draw a gradient by drawing vertical lines while slowly interpolating between the 2 colors.
            int bottom = hitbox.Bottom;
            int top = hitbox.Top;
            int steps = (int)((top - bottom) * quotient);
            for (int i = 0; i < steps; i += 1)
            {
                //float percent = (float)i / steps; // Alternate Gradient Approach
                float percent = (float)i / (top - bottom);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(bottom + i, hitbox.Y, hitbox.Width, 1 ), Color.Lerp(GradA, GradB, percent));
            }
        }
        public override void Update(GameTime gameTime)
        {
            var inPlayer = Main.LocalPlayer.GetModPlayer<InsightPlayer>();

            text.SetText($"Insight: {inPlayer.insight} / {inPlayer.maxInsight}");

            base.Update(gameTime);
        }
    }
}

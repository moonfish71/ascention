using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ascention.Core;
using Terraria.DataStructures;
using Terraria.ID;
using System.IO;
using ascention.Content.Insight;
using Terraria.UI;
using System.Collections.Generic;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace ascention
{
	public class ascention : Mod
	{
        public static ascention Instance;
        public bool exMeasures = false;

        internal UserInterface insightBar;

        internal insightBar InsightBar;

        public ascention()
        {
            Instance = this;
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                InsightBar = new insightBar();
                InsightBar.Activate();

                insightBar = new UserInterface();
                insightBar.SetState(InsightBar);
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Ref<Effect> screenRef = new Ref<Effect>(ModContent.Request<Effect>("ascention/Effects/AsterBarrier", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value); // The path to the compiled shader file.
                Filters.Scene["Barrier"] = new Filter(new ScreenShaderData(screenRef, "Barrier"), EffectPriority.VeryHigh);
                Filters.Scene["Barrier"].Load();
            }
        }

        public override void Unload()
        {
            base.Unload();

            ParticleManager.Unload();
            Instance = null;
        }

        private void Main_DrawGore(On.Terraria.Main.orig_DrawGore orig, Terraria.Main self)
        {
            orig(self);
            if (Main.PlayerLoaded && !Main.gameMenu)
                ParticleManager.Run();
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ascentionMessageType msgType = (ascentionMessageType)reader.ReadByte();

            switch (msgType)
            {
                case ascentionMessageType.InsightPlayerSyncPlayer:
                    byte playernumber = reader.ReadByte();
                    InsightPlayer InsightPlayer = Main.player[playernumber].GetModPlayer<InsightPlayer>();
                    InsightPlayer.maxInsight = reader.ReadInt32();
                    InsightPlayer.insight = reader.ReadInt32();
                    InsightPlayer.insightState = reader.ReadInt32();
                    break;
                case ascentionMessageType.InsightChanged:
                    playernumber = reader.ReadByte();
                    InsightPlayer InsightPlayer2 = Main.player[playernumber].GetModPlayer<InsightPlayer>();
                    InsightPlayer2.insight = reader.ReadInt32();
                    break;
                case ascentionMessageType.InsightStatusChanged:
                    playernumber = reader.ReadByte();
                    InsightPlayer InsightPlayer3 = Main.player[playernumber].GetModPlayer<InsightPlayer>();
                    InsightPlayer3.insightState = reader.ReadInt32();
                    break;
                case ascentionMessageType.MaxInsightChanged:
                    playernumber = reader.ReadByte();
                    InsightPlayer InsightPlayer4 = Main.player[playernumber].GetModPlayer<InsightPlayer>();
                    InsightPlayer4.maxInsight = reader.ReadInt32();
                    break;
                default:
                    Logger.WarnFormat("Stuff of Legends: Unknown Message type: {0}", msgType);
                    break;
            }
        }

    }
    internal enum ascentionMessageType : byte
    {
        InsightPlayerSyncPlayer,
        InsightChanged,
        InsightStatusChanged,
        MaxInsightChanged,
        Aster
    }

    public class ascentionSystem : ModSystem
    {
        internal UserInterface insightBar;

        internal insightBar InsightBar;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                InsightBar = new insightBar();
                InsightBar.Activate();

                insightBar = new UserInterface();
                insightBar.SetState(InsightBar);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            insightBar?.Update(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Insight",
                    delegate
                    {
                        insightBar.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
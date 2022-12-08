using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ascention
{
    public class StuffOfLegendsServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public enum talkInteraction
        {
            World = 0,
            Chat,
            Both
        }

        [DefaultValue(talkInteraction.World)]
        [Label("Talk Interaction")]
        [Tooltip("Controls whether certain things talk using in-world text, the chatbox, or both.")]
        public talkInteraction TalkTextSetting;
    }

    public class StuffOfLegendsClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        [Label("Enable Foreground/Background Objects")]
        [Tooltip("Enables ambient objects appearing in the background or foreground. Does not restrict NPCs or projectiles")]
        public bool BackgroundObjects;
    }
}

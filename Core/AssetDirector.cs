namespace ascention.Core
{
    public static class AssetDirector
    {
        public const string Assets =                "ascention/Assets/";

        public const string Music =                 Assets + "Music/";
        public const string SFX =                   Assets + "SFX/";
        public const string Textures =              Assets + "Textures/";

        public const string PlaceholderTx =        Textures + "placeholder";

        public const string SatelliteTx =           Textures + "Particles/Satellites/";

        public const string AsterTex =              Textures + "NPCs/Bosses/Aster/";
        public const string GKUTex =                Textures + "NPCs/Bosses/GKU/";

        public const string BarTex =                Textures + "UI/Insight/";

        public const string TestingItem =           Textures + "Items/Testing/";

        public const string Weapon =                Textures + "Items/Weapons/";
        public const string Melee =                 Weapon + "Melee/";

        public const string HostileProjectile =     Textures + "Projectiles/Hostile/";
        public const string FriendlyProjectile =    Textures + "Projectiles/Friendly/";
        public const string MeleeProjectile =       FriendlyProjectile + "Melee/";
        public const string Pet =                   FriendlyProjectile + "Pets/";


        public static string AsterVoice =          SFX + "Aster/";

        public static string ItemSound =           SFX + "Items/";
    }
}

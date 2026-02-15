using WLO;
using WoowzTile;
using WoowzTile.Objects;
using static GOLUWorld.GW_Objects;
using static GOLUWorld.GW_Resources;

namespace GOLUWorld;

internal static class GW_Values{
    #region Константы игры

        internal const string Game_Version = "0.4";
        internal const string Game_Name    = "GOLUWorld";

    #endregion
    
    internal static Vector2F WorldPosition = new Vector2F();

    internal static int PlayerX => (int)(Game.SceneSize.X / 2F - Texture_Player_Body.Width  / 2F);
    internal static int PlayerY => (int)(Game.SceneSize.Y / 2F - Texture_Player_Body.Height / 2F);

    internal static int WorldX => (int)(WorldPosition.X + Game.SceneSize.X / 2F);
    internal static int WorldY => (int)(WorldPosition.Y + Game.SceneSize.Y / 2F);

    internal static int CoordinatesX => -(WorldX / 16) + 8;
    internal static int CoordinatesY => -(WorldY / 16) + 8;

    internal static bool     Moving => MovingDirection != Vector2I.Zero;
    internal static Vector2I MovingDirection = Vector2I.Zero;

    internal static bool OutsideLevel = false;

    internal static uint     Seed;
    internal static T_Level  CurrentLevel = T_Level.None;
    internal static Vector2U LevelSize    = new Vector2U(100, 100);
    internal static Vector2U LevelSizeTile => LevelSize * 16;

    internal const float MaxTime   = 24;
    internal static       float Time      = MaxTime / 2;
    internal static       float TimeSpeed = 1f;
    internal static       float DayPhase  => WL.Math.Clamp01(WL.Math.DSin((Time - 6) / 24 * WL.Math.PI * 2));

    internal static CollisionLayer InsideCollision = CollisionLayer.None;
    internal static byte           CollisionInfo1  = 0;
    internal static Vector2I       CollisionInfo2  = Vector2I.Zero;
    internal static int            CollisionInfo3  = 0;

    internal const uint HealthMax   = 100;
    internal const uint HealthSmall = 30;
    internal static       uint Health      = HealthMax;

    internal static bool        InMainMenu         = true;
    internal static T_Interface Interface          = T_Interface.None;
    internal static byte        MenuSelectedButton = 0;

    internal static bool Dead => Health == 0;

    internal static bool StopTime = false;

    internal const byte MaxSlots = 12;
    internal static byte SelectedItem   = 0;

    internal static readonly T_Item[] Inventory = new T_Item[MaxSlots];

    internal static float LastHealed = 0;

    internal static float Rotten = 0;

    internal static ColorB WorldBackgroundColor = ColorB.White;

    internal const uint Emotion_Max       = 100;
    internal static       uint Emotion_Happiness = Emotion_Max;

    internal static string     Thoughts        = "";
    internal static float      ThoughtsTimer   = 0;
    internal static T_Thoughts ThoughtsContext = T_Thoughts.Idle;
    
    internal static float BlinkTimer     = 0;
    internal static float AnimationTimer = 0;
    internal static bool  PlayerFlipped  = false;
    internal static float WorldDeltaTick = 0;
    
    internal static bool RenderColliders = false;
    internal static bool Immortality     = false;
    internal static bool IgnoreColliders = false;
}
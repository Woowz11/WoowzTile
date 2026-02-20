using WLO;
using WoowzTile;
using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Values;

namespace GOLUWorld;

internal static class GOLUWorld_Utility{
    /// <summary>
    /// Берёт случайный элемент с учётом весов
    /// </summary>
    /// <param name="RandomValue">от 0 до 1</param>
    internal static (T, byte) Utility_SelectWeightedObject<T>(float RandomValue, ReadOnlySpan<(T Value, byte Info, int Weight)> Variants){
        int TotalWeight = 0;

        for(int i = 0; i < Variants.Length; i++){ TotalWeight += Variants[i].Weight; }

        int Scaled = (int)(RandomValue * TotalWeight);

        if(Scaled >= TotalWeight){ Scaled = TotalWeight - 1; }

        for(int i = 0; i < Variants.Length; i++){
            if(Scaled < Variants[i].Weight){ return (Variants[i].Value, Variants[i].Info); }
            Scaled -= Variants[i].Weight;
        }

        return (Variants[^1].Value, Variants[^1].Info);
    }

    /// <summary>
    /// Случайный поворот
    /// </summary>
    internal static TextureRotation Utility_RandomRotation(uint Seed) => Utility_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref Seed), [(TextureRotation.None, 0, 1), (TextureRotation.Rotate90, 0, 1), (TextureRotation.Rotate180, 0, 1), (TextureRotation.Rotate270, 0, 1)]).Item1;
    /// <summary>
    /// Случайный поворот
    /// </summary>
    internal static TextureRotation Utility_RandomRotation() => Utility_SelectWeightedObject(WL.Math.Random.Fast_0_1(), [(TextureRotation.None, 0, 1), (TextureRotation.Rotate90, 0, 1), (TextureRotation.Rotate180, 0, 1), (TextureRotation.Rotate270, 0, 1)]).Item1;

    /// <summary>
    /// Поворот горизонтальный?
    /// </summary>
    internal static bool Utility_Horizontal(TextureRotation Rotation) => Rotation is TextureRotation.Rotate90 or TextureRotation.Rotate270;
    
    /// <summary>
    /// Поворот вертикальный?
    /// </summary>
    internal static bool Utility_Vertical(TextureRotation Rotation) => !Utility_Horizontal(Rotation);
    
    /// <summary>
    /// Возвращает уникальное число для X, Y
    /// </summary>
    internal static uint Utility_SeedXY(int X, int Y){
        uint A = (uint)X;
        uint B = (uint)Y;

        uint Hash = A;
        Hash = (Hash ^ 0xDEADBEEF) + (B << 5);
        Hash = (Hash ^ (Hash >> 13)) * 0x45d9f3b;
        Hash = Hash ^ (Hash >> 16);

        return Hash;
    }
    
    internal static Vector2I Utility_WorldToScreen(Vector2I World ) => new Vector2I(World.X  + Coordinates_World.X, World.Y  + Coordinates_World.Y);
    internal static Vector2I Utility_ScreenToWorld(Vector2I Screen) => new Vector2I(Screen.X - Coordinates_World.X, Screen.Y - Coordinates_World.Y);
        
    internal static Vector2F Utility_WorldToCamera (Vector2I World ) => new Vector2F(World.X - Game.SceneSize.X / 2f, World.Y - Game.SceneSize.Y / 2f);
    internal static Vector2F Utility_ScreenToCamera(Vector2I Screen) => Utility_WorldToCamera(Utility_ScreenToWorld(Screen));
    internal static Vector2I Utility_CameraToWorld (Vector2F Camera) => new Vector2I((int)(Camera.X + Game.SceneSize.X / 2f), (int)(Camera.Y + Game.SceneSize.Y / 2f));
    internal static Vector2I Utility_CameraToScreen(Vector2F Camera) => Utility_WorldToScreen(Utility_CameraToWorld(Camera));
        
    internal static Vector2I Utility_CameraToPlayerWorld(Vector2F Camera     ) => new Vector2I((int)(Coordinates_Player.X - Camera.X - Game.SceneSize.X / 2f), (int)(Coordinates_Player.Y - Camera.Y - Game.SceneSize.Y / 2f));
    internal static Vector2F Utility_PlayerWorldToCamera(Vector2I PlayerWorld) => new Vector2F(Coordinates_Player.X - PlayerWorld.X - Game.SceneSize.X / 2f, Coordinates_Player.Y - PlayerWorld.Y - Game.SceneSize.Y / 2f);
}
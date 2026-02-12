using System.Numerics;

namespace WoowzTile.Objects;

[Flags]
public enum CollisionLayer : uint{
    None = 0,
    All = uint.MaxValue,
    
    L1 = 1 << 0,
    L2 = 1 << 1,
    L3 = 1 << 2,
    L4 = 1 << 3,
    L5 = 1 << 4,
    L6 = 1 << 5,
    L7 = 1 << 6,
    L8 = 1 << 7,
    L9 = 1 << 8,
    L10 = 1 << 9,
    L11 = 1 << 10,
    L12 = 1 << 11,
    L13 = 1 << 12,
    L14 = 1 << 13,
    L15 = 1 << 14,
    L16 = 1 << 15,
}

public struct Collider{
    public Collider(int X, int Y, uint W, uint H, CollisionLayer Layer = CollisionLayer.L1, CollisionLayer Mask = CollisionLayer.All){
        this.X = X;
        this.Y = Y;
        this.W = W;
        this.H = H;
        this.Layer = Layer;
        this.Mask = Mask;
    }
    
    public int  X;
    public int  Y;
    public uint W;
    public uint H;

    public CollisionLayer Layer;
    public CollisionLayer Mask;

    public bool CanCollide(Collider Other) => (Mask & Other.Layer) != 0 && (Other.Mask & Layer) != 0;

    public static int GetLayerIndex(CollisionLayer Layer){
        if(Layer == CollisionLayer.None){ return -1; }
        if(Layer == CollisionLayer.All){ return -2; }
        
        return BitOperations.TrailingZeroCount((uint)Layer);
    }
    
    public bool Intersects(Collider Other) => X < Other.X + Other.W && X + W > Other.X && Y < Other.Y + Other.H && Y + H > Other.Y;
}
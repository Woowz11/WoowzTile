namespace WoowzTile.Objects;

public struct Collider{
    public Collider(int X, int Y, uint W, uint H){
        this.X = X;
        this.Y = Y;
        this.W = W;
        this.H = H;
    }
    
    public int  X;
    public int  Y;
    public uint W;
    public uint H;
    
    public bool Intersects(Collider Other) => X < Other.X + Other.W && X + W > Other.X && Y < Other.Y + Other.H && Y + H > Other.Y;
}
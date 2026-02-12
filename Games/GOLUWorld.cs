using WL;
using WLO;
using WoowzTile.Objects;

namespace WoowzTile.Games;

public class GOLUWorld : Game{
    public Palette Palette_World;
    
    public Texture Texture_Ground;
    public Texture Texture_Wall;
    public Texture Texture_Player;
    public Texture Texture_Player_Blink;
    public Texture Texture_Player_Blood;
    public Texture Texture_Player_Blood_Blink;
    public Texture Texture_GroundPlanks;
    public Texture Texture_Track;
    public Texture Texture_Blood;
    public Texture Texture_Health;
    public Texture Texture_G;
    public Texture Texture_O;
    public Texture Texture_L;
    public Texture Texture_U;
    public Texture Texture_Author;
    public Texture Texture_Title;
    public Texture Texture_Chair;
    public Texture Texture_Table;
    public Texture Texture_Spikes;
    public Texture Texture_Spider;
    public Texture Texture_Spider_Walk;

    public override string Name(){ return "GOLUWorld"; }

    public override string WindowTitle(){ return new Vector2I(PlayerX - WorldX, PlayerY - WorldY).ToShortString(); }

    public override void Start(){
        Palette_World = new Palette([
            new KeyValuePair<byte, ColorB>(1 , ColorB.Black),
            new KeyValuePair<byte, ColorB>(2 , ColorB.DarkGray),
            new KeyValuePair<byte, ColorB>(3 , ColorB.Gray),
            new KeyValuePair<byte, ColorB>(4 , ColorB.LightGray),
            new KeyValuePair<byte, ColorB>(5 , ColorB.White),
            new KeyValuePair<byte, ColorB>(6 , ColorB.Black.SetA(64)),
            new KeyValuePair<byte, ColorB>(7 , ColorB.Red.SetA(64)),
            new KeyValuePair<byte, ColorB>(8 , ColorB.Red),
            new KeyValuePair<byte, ColorB>(9 , ColorB.DarkRed),
            new KeyValuePair<byte, ColorB>(10, ColorB.DarkMagenta)
        ]);

        Dictionary<char, byte> Mapping = new Dictionary<char, byte>{
            ['.'] = 0,
            ['█'] = 1,
            ['▓'] = 2,
            ['▒'] = 3,
            ['░'] = 4,
            ['_'] = 5,
            ['('] = 6,
            [')'] = 7,
            ['R'] = 8,
            ['r'] = 9,
            ['m'] = 10
        };
        
        Texture_Ground = new Texture(
            @"__░__▒__░░____▒_
____▒_░_____▒▒__
__░▒░░__░__▒░░__
░░▒░░░_____░____
░__▒_____░____░_
____▓▒____░░____
_░▒___▒░_____▒░_
▒▒___░░_____▒_░▒
________░___▒░▒_
___▒░░_____▒____
____▒____░___░__
░░______▒░░___░_
_▒_____▒___░____
▒_____▓_░_______
_░____▒__░____░▒
░____▒░░_____░▒_",
            Mapping
        );
        
        Texture_GroundPlanks = new Texture(
            @"__░░__▒___░░░░__
░_____▒░░░_____░
▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒
_____░░░░__▓____
░░░░░______▒__░░
___________▒░___
______░░░░_▒_░__
▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒
░▒░░░░░░_______░
░▒_____░░░░░____
_▒______________
░▒░_________░░░░
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
░░░░__▓____░░___
____░░▒░░░______
______▒______░__",
            Mapping
        );

Texture_Wall = new Texture(
@"████████████████
█▓▒▒▒░░░░░▒▒▒▒██
█▒___________░▓█
█▒_█░_░░░░_░█_▓█
█▒_░________░_▒█
█▒____________▒█
█▒_░________░_▒█
█▒_░________░_▒█
█▒_░________░_▒█
█▒_░________░_▒█
█▒____________▒█
█▒_░________░_▒█
█▒_█░_░░░░_░█_▒█
█▒░__________░▓█
██▓▒▒▒▒▒▒▒▒▒▒▓▓█
████████████████",
Mapping
);

Texture_Player = new Texture(
    @"...██████████...
..█▒▒░░░__░░▒█..
.█_______▒███▒█.
█░▒███▒▒▒█_█_█▒█
█░█_█_█▓▓█_█░█░█
█░█_█░█_▒▒███▒░█
█░▒███▒_▓__░░_░█
█░░░░___▓___▒░░█
█░____░__▓_░__▒█
█▒__░__▒█▓___░██
█░▓__▒▒__▒▒_░█░█
█░_▓▓██████▓▓_░█
█▒__░░____░░__▒█
.█▒__▒▒▒▒▒▒__▒█.
..█▓▓░░__░░▓▓█..
...██████████...",
    Mapping
);

Texture_Player_Blood = new Texture(
    @"...██████████...
..█▒▒░░░__░░▒█..
.█_______▒███▒█.
█░▒███▒▒▒█_█_█▒█
█░█_█_█▓▓█_█░█░r
█░█_█░█_▒m███mR█
█░m███m_▓_mmm_░█
█░░mmm__▓___▒░░█
█░____░__▓_░__▒█
█▒__░__▒█▓___░█r
r░▓__▒▒__R▒_RrRr
rRR▓▓████rr▓▓_R█
█▒__░░____R░__▒█
.█R__▒▒▒▒▒R__▒█.
..r▓▓░░__░RR▓r..
...█████rrrrr...",
    Mapping
);

Texture_Player_Blink = new Texture(
    @"...██████████...
..█▒▒░░░__░░▒█..
.█_______▒___▒█.
█░▒___▒▒▒█▒▒▒█▒█
█░█▒▒▒█▓▓█████░█
█░█████_▒▒___▒░█
█░▒___▒_▓__░░_░█
█░░░░___▓___▒░░█
█░____░__▓_░__▒█
█▒__░__▒█▓___░██
█░▓__▒▒__▒▒_░█░█
█░_▓▓██████▓▓_░█
█▒__░░____░░__▒█
.█▒__▒▒▒▒▒▒__▒█.
..█▓▓░░__░░▓▓█..
...██████████...",
    Mapping
);

Texture_Player_Blood_Blink = new Texture(
    @"...██████████...
..█▒▒░░░__░░▒█..
.█_______▒___▒█.
█░▒___▒▒▒█▒▒▒█▒█
█░█▒▒▒█▓▓█████░r
█░█████_▒mmmmmR█
█░mmmmm_▓_mmm_░█
█░░mmm__▓___▒░░█
█░____░__▓_░__▒█
█▒__░__▒█▓___░█r
r░▓__▒▒__R▒_RrRr
rRR▓▓████rr▓▓_R█
█▒__░░____R░__▒█
.█R__▒▒▒▒▒R__▒█.
..r▓▓░░__░RR▓r..
...█████rrrrr...",
    Mapping
);

Texture_Track = new Texture(
    @"................
................
................
................
................
................
......(((.......
.....(((((......
.....(((((......
......(((.......
................
................
................
................
................
................",
    Mapping
);

Texture_Blood = new Texture(
    @"................
................
................
................
.....)..........
...........)....
....))..))......
..)...))).).....
......))))......
....)))))..)....
.......)))......
...........)....
....))..........
................
................
................",
    Mapping
);

Texture_Health = new Texture(
    @"................
......rrr.......
.....rRRRr......
.....rRRRr......
.....rRRRr......
.....rRRRr......
.rrrrRRRRRrrrr..
rRRRRRRRRRRRRRr.
rRRRRRRRRRRRRRr.
rRRRRRRRRRRRRRr.
.rrrrRRRRRrrrr..
.....rRRRr......
.....rRRRr......
.....rRRRr......
.....rRRRr......
......rrr.......",
    Mapping
);

Texture_G = new Texture(
    @"................................
........██████████████..........
......██████████████████........
.....███░░░░░░░░░░░░░░███.......
....██░░░░░░░░░░░░░░░░░░██......
....██░░░████████████░░░██......
...██░░░██████████████░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........███████.....
...██░░░██...........█████......
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██...█████████████......
...██░░░██..███████████████.....
...██░░░██..██░░░░░░░░░░░██.....
...██░░░██..██░░░░░░░░░░░██.....
...██░░░██..██████████░░░██.....
...██░░░██...█████████░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██████████████░░░██.....
....██░░░████████████░░░██......
....██░░░░░░░░░░░░░░░░░░██......
.....███░░░░░░░░░░░░░░███.......
......██████████████████........
........██████████████..........
................................
................................",
    Mapping
);
Texture_O = new Texture(
    @"................................
........██████████████..........
......██████████████████........
.....███░░░░░░░░░░░░░░███.......
....██░░░░░░░░░░░░░░░░░░██......
....██░░░████████████░░░██......
...██░░░██████████████░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██..........██░░░██.....
...██░░░██████████████░░░██.....
....██░░░████████████░░░██......
....██░░░░░░░░░░░░░░░░░░██......
.....███░░░░░░░░░░░░░░███.......
......██████████████████........
........██████████████..........
................................
................................",
    Mapping
);
Texture_L = new Texture(
    @"................................
....█████.......................
...███████......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██......................
...██░░░██████████████████......
....██░░░██████████████████.....
....██░░░░░░░░░░░░░░░░░░░██.....
.....███░░░░░░░░░░░░░░░░░██.....
......█████████████████████.....
........██████████████████......
................................
................................",
    Mapping
);
Texture_U = new Texture(
    @"................................
....█████.............█████.....
...███████...........███████....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░██...........██░░░██....
...██░░░███████████████░░░██....
....██░░░█████████████░░░██.....
....██░░░░░░░░░░░░░░░░░░░██.....
.....███░░░░░░░░░░░░░░░███......
......███████████████████.......
........███████████████.........
................................
................................",
    Mapping
);

Texture_Author = new Texture(
    @"...............█......█..██████...██████..█......█.█████████...█.....█..
...............█......█.█......█.█......█.█......█.........█..██....██..
█.....█...█....█......█.█......█.█......█.█......█........█..█.█...█.█..
█.....█...█....█......█.█......█.█......█.█......█.......█.....█.....█..
████..█...█....█......█.█......█.█......█.█......█....███......█.....█..
█...█..████....█..██..█.█......█.█......█.█..██..█...█.........█.....█..
█...█.....█....█.█..█.█.█......█.█......█.█.█..█.█..█..........█.....█..
██..█.█...█....██....██.█......█.█......█.██....██.█...........█.....█..
█.██...███.....█......█..██████...██████..█......█.█████████.█████.█████",
    Mapping
);

Texture_Title = new Texture(
    @".██........██..█████..██████..██......██████..
.██...██...██.██...██.██...██.██......██...██.
..██.████.██..██...██.██████..██......██...██.
..████..████..██...██.██...██.██......██...██.
...██....██....█████..██...██.███████.██████..",
    Mapping
);

Texture_Chair = new Texture(
    @"................
..██▓▓▓▓▓▓▓▓██..
.█▒░░▒░░░░▒░░▒█.
.█░__▒____▒__░█.
.█___▒____▒___█.
.█░__▒____▒__░█.
.█▒░░▒░░░░▒░░▒█.
.█▓▓▓▓▓▓▓▓▓▓▓▓█.
.█▒░________░▒█.
.█░__________░█.
.█▒░________░▒█.
..█▓▓▓▓▓▓▓▓▓▓█..
..█.█......█.█..
..█.((((((((.█..
..█((((((((((█..
................",
    Mapping
);

Texture_Table = new Texture(
    @".██████████████.
█░____________░█
█_░░░░░░░░░░░░_█
█_░__________░_█
█_░__________░_█
█_░__________░_█
█_░__________░_█
█_░__________░_█
█_░__________░_█
█_░__________░_█
█_░__________░_█
█_░░░░░░░░░░░░_█
█░____________░█
.██████████████.
.█((((((((((((█.
.█((........((█.",
    Mapping
);

Texture_Spikes = new Texture(
    @".............█..
.)..r...))..█▒█.
...rRr)))...▓░▓.
)).rR▓.....█▒_▒█
..rR_▒█..).█▒░▒█
..█R░▒█))..(▒▒▒(
..(▒▒▒(.))r.(((.
)).(((.))█Rr....
)........▓R▓..).
...█....█▒_▒█...
..█▒█)).█▒░▒█)..
..▓░▓.).(▒▒▒()).
.█▒_▒█.).(((..).
.█▒░▒█..........
.(▒▒▒(...)......
..(((.....).....",
    Mapping
);

Texture_Spider = new Texture(
    @"................................
................................
................................
................................
................................
................................
..............███...............
............██▓▓▓██.............
..........(█▓▒░░░▒▓█(...........
..........(█▒rr░rr▒█(...........
..........(█▒r░░░r▒█(...........
....▓.....(█▒░░r░░▒█(.....▓.....
.....█(...(█▒░░░░░▒█(...(█......
.....(██..(██▒rrr▒██(..██(......
......((██.(█▒▒▒▒▒█(.██((.......
.........(██(█▒▓▒█(██(..........
...▓██████████▓▒▓██████████▓....
....(((((((██░▒▓▒░██(((((((.....
.........████R░r░R████..........
.......██((██rR░Rr██((██........
......█((..█(█▒░▒█(█..((█.......
.....▓....█(.(█▓█(.(█....▓......
..........▓(..█(█..(▓...........
..........▓(.......(▓...........
..........▓(.......(▓...........
...........▓.......▓............
................................
................................
................................
................................
................................
................................",
    Mapping
);

Texture_Spider_Walk = new Texture(
    @"................................
................................
................................
................................
................................
................................
..............███...............
............██▓▓▓██.............
..........(█▓▒░░░▒▓█(...........
..........(█▒rr░rr▒█(...........
..........(█▒r░░░r▒█(...........
.......▓..(█▒░░r░░▒█(..▓........
.......(█.(█▒░░░░░▒█(.█(........
........█.(██▒rrr▒██(.█.........
.........█.(█▒▒▒▒▒█(.█..........
...▓████.(██(█▒▓▒█(██(.████▓....
....((((██████▓▒▓██████((((.....
........(((██░▒▓▒░██(((.........
.........████R░r░R████..........
.....▓███((██rR░Rr██((███▓......
.......((..█(█▒░▒█(█..((........
...........█((█▓█((█............
...........▓(.█(█.(▓............
...........▓(.....(▓............
...........▓(.....(▓............
...........▓.......▓............
................................
................................
................................
................................
................................
................................",
    Mapping
);

    }
    
    public override void Stop(){
        
    }
    
    private Vector2F WorldPosition = new Vector2F();

    private int PlayerX => (int)(Game.SceneSize.X / 2F - Texture_Player.Width  / 2F);
    private int PlayerY => (int)(Game.SceneSize.Y / 2F - Texture_Player.Height / 2F);
    
    private int WorldX => (int)(WorldPosition.X + Game.SceneSize.X / 2F);
    private int WorldY => (int)(WorldPosition.Y + Game.SceneSize.Y / 2F);

    private bool     Moving => MovingDirection != Vector2I.Zero;
    private Vector2I MovingDirection = Vector2I.Zero;

    private const uint HealthMax = 100;
    private       uint Health    = HealthMax;

    private bool InMainMenu = true;

    private bool Dead => Health == 0;
    
    private void Damage(uint Damage, int Range = 0){
        Health = WL.Math.SubU(Health, Damage);

        SplatBlood(PlayerX - WorldX + WL.Math.Random.Fast_Int(-Range, Range), PlayerY - WorldY + WL.Math.Random.Fast_Int(-Range, Range));
        
        //Task.Run(() => Console.Beep(WL.Math.Random.Fast_Int(0, 10000), 10000));
    }
    
    public struct Entity{
        public int             X;
        public int             Y;
        public byte            ID;
        public byte            Info;
        public Vector2I        InfoVector;
        public TextureRotation Rotation;
    }
    
    public override void Update(TickData TD){
        Game.ClearColliders();

        if(InMainMenu){
            return;
        }

        if(!Dead){
            Health = Health >= HealthMax ? HealthMax : Health + (uint)(WL.Math.Random.Fast_Bool(0.01f) ? 1 : 0);
        }
        
        foreach((int, int, byte) Block in __Blocks){
            if(Block.Item3 == 1){
                Game.AddCollider(new Collider(WorldX + Block.Item1, WorldY + Block.Item2, 16, 16));
            }
        }
        
        for(int i = 0; i < __Entity.Count; i++){
            Entity Entity = __Entity[i];
            
            if(Entity.ID is 2 or 3 or 4){
                if(Entity.ID == 4){
                    int SpiderSpeed = WL.Math.Random.Fast_Bool(0.8f) ? 1 : 0;
                    
                    byte Info = Entity.Info;
                    if(WL.Math.Random.Fast_Bool(Info == 1 ? 0.5f : 0.05f)){
                        if(WL.Math.Random.Fast_Bool(0.05f)){
                            Info = 2;
                        }else{
                            Info = (byte)(Info == 1 ? 0 : 1);
                        }
                    }

                    int PlayerX__ = PlayerX - WorldX;
                    int PlayerY__ = PlayerY - WorldY;

                    float Distance = Vector2I.Distance(new Vector2I(Entity.X, Entity.Y), new Vector2I(PlayerX__, PlayerY__));

                    Vector2I MoveDirection = Vector2I.Zero;
                    
                    Vector2I Target = Entity.InfoVector;
                    Vector2I EntityPositionOriginal = new Vector2I(Entity.X, Entity.Y);
                    
                    if(Distance < 100 && !Dead){

                        Target.X = Info is 1 or 2 ? WorldX - PlayerX : PlayerX__;
                        Target.Y = Info is 1 or 2 ? WorldY - PlayerY : PlayerY__;

                        MoveDirection.X = WL.Math.Sign(Target.X - Entity.X) * SpiderSpeed;
                        MoveDirection.Y = WL.Math.Sign(Target.Y - Entity.Y) * SpiderSpeed;
                        
                        Entity.X += MoveDirection.X;
                        Entity.Y += MoveDirection.Y;
                        Entity.Info = Info;
                        
                    }else{
                        if(WL.Math.Random.Fast_Bool(0.05f) || Target == Vector2I.Zero){
                            Target = new Vector2I(WL.Math.Random.Fast_Int(-1000, 1000), WL.Math.Random.Fast_Int(-1000, 1000));
                        }
                        
                        MoveDirection.X = WL.Math.Sign(Target.X - Entity.X) * SpiderSpeed;
                        MoveDirection.Y = WL.Math.Sign(Target.Y - Entity.Y) * SpiderSpeed;

                        Entity.X += MoveDirection.X;
                        Entity.Y += MoveDirection.Y;
                        Entity.Info = Info;
                        Entity.InfoVector = Target;
                    }

                    if(MoveDirection != Vector2I.Zero){
                        int DirectionX = 0;
                        int DirectionY = 0;

                        float DX = Target.X - EntityPositionOriginal.X;
                        float DY = Target.Y - EntityPositionOriginal.Y;

                        if(WL.Math.Abs(DX) > WL.Math.Abs(DY)){
                            DirectionX = WL.Math.Sign(DX);
                        }else{
                            DirectionY = WL.Math.Sign(DY);
                        }
                        
                        Entity.Rotation = DirectionX == 1 ? TextureRotation.Rotate270 : (DirectionX == -1 ? TextureRotation.Rotate90 : (DirectionY == -1 ? TextureRotation.Rotate180 : TextureRotation.None));
                    }
                    __Entity[i] = Entity;
                }
                
                uint Size = 16;
                if(Entity.ID == 2){
                    Size = 14;
                }

                CollisionLayer Layer = CollisionLayer.L1;
                if(Entity.ID == 3){
                    Layer = CollisionLayer.L2;
                }else if(Entity.ID == 4){
                    Layer = CollisionLayer.L3;
                }
                Game.AddCollider(new Collider(WorldX + Entity.X + (int)((16 - Size)/2), WorldY + Entity.Y + (int)((16 - Size)/2), Size, Size, Layer));
            }
        }

        bool CanMove = !Dead;

        if(Dead){
            if(WL.Math.Random.Fast_Bool(0.8f)){
                SplatBlood(PlayerX - WorldX + WL.Math.Random.Fast_Int(-128, 128), PlayerY - WorldY + WL.Math.Random.Fast_Int(-128, 128));
            }
        }
        
        uint PlayerSize = (uint)(Texture_Player.Width * 0.8f);
        int PlayerOffset = (int)((Texture_Player.Width - PlayerSize) / 2);
        
        if(CanMove){
            uint PlayerSpeed = (uint)(TD.DeltaTimeS * 100 * (Game.KeyPressed(Key.Shift) ? 1.5 : 1));
            if(Health < 30){ PlayerSpeed = (uint)(PlayerSpeed / 2); }

            bool D = Game.KeyPressed(Key.D);
            bool A = Game.KeyPressed(Key.A);
            bool W = Game.KeyPressed(Key.W);
            bool S = Game.KeyPressed(Key.S);
            MovingDirection = new Vector2I(A && D ? 0 : (A ? 1 : (D ? -1 : 0)), W && S ? 0 : (W ? 1 : (S ? -1 : 0)));

            Vector2F DesiredMove = new Vector2F();

            const CollisionLayer WallCollider = CollisionLayer.L1;
            if(MovingDirection.X != 0 && MovingDirection.Y != 0){
                for(uint i = 1; i <= PlayerSpeed; i++){
                    int TestX = (int)(PlayerX - MovingDirection.X * i + PlayerOffset);
                    int TestY = (int)(PlayerY - MovingDirection.Y * i + PlayerOffset);

                    Collider TestCollider = new Collider(TestX, TestY, PlayerSize, PlayerSize, CollisionLayer.L1, WallCollider);

                    if(!Collision(TestCollider)){
                        DesiredMove.X = MovingDirection.X * i;
                        DesiredMove.Y = MovingDirection.Y * i;
                    }
                    else{
                        TestCollider.X = TestX;
                        TestCollider.Y = PlayerY + PlayerOffset;
                        if(!Collision(TestCollider)){
                            DesiredMove.X = MovingDirection.X * i;
                            DesiredMove.Y = 0;
                        }
                        else{
                            TestCollider.X = PlayerX + PlayerOffset;
                            TestCollider.Y = TestY;
                            if(!Collision(TestCollider)){
                                DesiredMove.X = 0;
                                DesiredMove.Y = MovingDirection.Y * i;
                            }
                            else{
                                break;
                            }
                        }

                        break;
                    }
                }
            }
            else{
                for(uint i = 1; i < PlayerSpeed + 1; i++){
                    if(!Collision(new Collider((int)(PlayerX - (MovingDirection.X * i) + PlayerOffset), PlayerY + PlayerOffset, PlayerSize, PlayerSize, CollisionLayer.L1, WallCollider))){
                        DesiredMove.X = MovingDirection.X * i;
                    }
                    else{
                        break;
                    }
                }

                for(uint i = 1; i < PlayerSpeed + 1; i++){
                    if(!Collision(new Collider(PlayerX + PlayerOffset, (int)(PlayerY - (MovingDirection.Y * i) + PlayerOffset), PlayerSize, PlayerSize, CollisionLayer.L1, WallCollider))){
                        DesiredMove.Y = MovingDirection.Y * i;
                    }
                    else{
                        break;
                    }
                }
            }

            WorldPosition += DesiredMove;

            if(DesiredMove.X != 0 || DesiredMove.Y != 0){
                Track();

                if(Collision(new Collider((int)(PlayerX + PlayerOffset), PlayerY + PlayerOffset, PlayerSize, PlayerSize, CollisionLayer.L1, CollisionLayer.L2))){
                    if(WL.Math.Random.Fast_Bool(0.5f)){
                        Damage((uint)(WL.Math.Random.Fast_0_1() * 5));
                    }
                }
            }
        }
        
        if(Collision(new Collider((int)(PlayerX + PlayerOffset), PlayerY + PlayerOffset, PlayerSize, PlayerSize, CollisionLayer.L1, CollisionLayer.L3))){
            if(WL.Math.Random.Fast_Bool(0.8f)){
                Damage((uint)(WL.Math.Random.Fast_0_1() * 20), Dead ? 16 : 0);
            }
        }
    }

    private readonly List<(int, int, byte, TextureRotation)> __Tracks = [];
    private void Track(){
        if(WL.Math.Random.Fast_Bool(0.1f)){
            if(Health < 32){
                SplatBlood(PlayerX - WorldX, PlayerY - WorldY);
            }else{
                __Tracks.Add((PlayerX - WorldX, PlayerY - WorldY, 0, TextureRotation.None));
            }
        }
    }

    private void SplatBlood(int X, int Y){
        __Tracks.Add((X, Y, 1, WL.Math.Random.Fast_Bool(0.5f) ? (WL.Math.Random.Fast_Bool(0.5f) ? TextureRotation.None :  TextureRotation.Rotate90) : (WL.Math.Random.Fast_Bool(0.5f) ? TextureRotation.Rotate180 : TextureRotation.Rotate270)));
    }

    private void AddBlock(int X, int Y, byte Type){
        int FinalX = X * 16;
        int FinalY = Y * 16;
        (int, int, byte) Block = (FinalX, FinalY, Type);

        int Index = __Blocks.FindIndex(B => B.Item1 == FinalX && B.Item2 == FinalY);
        
        if(Index != -1){
            if(Type == 0){
                __Blocks.RemoveAt(Index);
            }else{
                (int, int, byte) OldBlock = __Blocks[Index];
                if(OldBlock.Item3 != Type){
                    __Blocks[Index] = Block;
                }
            }
        }else{
            if(Type != 0){
                __Blocks.Add(Block);
            }
        }
    }
    private readonly List<(int, int, byte)> __Blocks = [];
    
    private void ClearAllScene(){
        __Blocks.Clear();
    }
    
    private void AddScene(string SceneMap, int X = 0, int Y = 0){
        try{
            if(string.IsNullOrEmpty(SceneMap)){ return; }
            
            int X__ = X;
            int Y__ = Y;

            foreach(char C in SceneMap){
                switch(C){
                    case '\r': 
                        continue;
                    case '\n':
                        Y__++;
                        X__ = X;
                        continue;
                    case '#':
                        AddBlock(X__, Y__, 1);
                        break;
                    case '\'':
                        AddBlock(X__, Y__, 2);
                        break;
                }

                X__++;
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при загрузке сцены!", e);
        }
    }
    
    private void AddEntity(int X, int Y, byte Type, byte Info = 0, Vector2I InfoPosition = default){
        int FinalX = X * 16;
        int FinalY = Y * 16;
        Entity Entity = new Entity{X = FinalX, Y = FinalY, ID = Type, Info = Info, InfoVector = InfoPosition};

        if(Type != 0){
            __Entity.Add(Entity);
        }
    }
    private readonly List<Entity> __Entity = [];
    
    private void ClearAllEntityScene(){
        __Entity.Clear();
    }
    
    private void AddEntityScene(string SceneMap, int X = 0, int Y = 0){
        try{
            if(string.IsNullOrEmpty(SceneMap)){ return; }
            
            int X__ = X;
            int Y__ = Y;

            foreach(char C in SceneMap){
                switch(C){
                    case '\r': 
                        continue;
                    case '\n':
                        Y__++;
                        X__ = X;
                        continue;
                    case 'C':
                        AddEntity(X__, Y__, 1);
                        break;
                    case 'T':
                        AddEntity(X__, Y__, 2);
                        break;
                    case '^':
                        AddEntity(X__, Y__, 3);
                        break;
                    case 's':
                        AddEntity(X__, Y__, 4);
                        break;
                }

                X__++;
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при загрузке Entity сцены!", e);
        }
    }
    
    private float BlinkTimer     = 0;
    private float AnimationTimer = 0;
    private bool  PlayerFlipped  = false;
    public override void Render(TickData TD, Image.ImageContext C){
        AnimationTimer += (float)TD.DeltaTimeS;
        if(AnimationTimer > 1){ AnimationTimer = 0; }
        
        if(InMainMenu){
            Texture_Author.Render(C, Palette_World, (int)(C.Width - Texture_Author.Width) - 3, 3);
            
            Texture_G.Render(C, Palette_World, (int)(C.Width/2 - Texture_G.Width/2 - Texture_G.Width*1.5F), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2) * 10));
            Texture_O.Render(C, Palette_World, (int)(C.Width/2 - Texture_G.Width/2 - Texture_G.Width/2), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2 + 1) * 10));
            Texture_L.Render(C, Palette_World, (int)(C.Width/2 - Texture_G.Width/2 + Texture_G.Width/2), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2 + 2) * 10));
            Texture_U.Render(C, Palette_World, (int)(C.Width/2 - Texture_G.Width/2 + Texture_G.Width*1.5F), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2 + 3) * 10));

            C.Fill((int)(C.Width / 2 - Texture_G.Width / 2 - Texture_G.Width * 1.5F), 75, 127, 2, ColorB.Black);
            
            Texture_Title.Render(C, Palette_World, (int)(C.Width/2 - Texture_Title.Width/2), 80);
            
            C.Border(0, 0, C.Width, C.Height, 1, ColorB.Black);
            
            return;
        }
        
        Texture_Ground.Render(C, Palette_World, WorldX - 16 * 16, WorldY - 16 * 16, 64, 64);
        
        foreach((int, int, byte) Block in __Blocks){
            if(Block.Item3 == 2){
                Texture BlockTexture = Texture_GroundPlanks;
                BlockTexture.Render(C, Palette_World, WorldX + Block.Item1, WorldY + Block.Item2);
            }
        }

        foreach((int, int, byte, TextureRotation) Track in __Tracks){
            Texture Track__ = Track.Item3 == 1 ? Texture_Blood : Texture_Track;
            Track__.Render(C, Palette_World, WorldX + Track.Item1, WorldY + Track.Item2, false, false, Track.Item4);
        }
        
        foreach(Entity Entity in __Entity){
            if(Entity.ID is 1 or 2 or 3){
                Texture EntityTexture = Entity.ID switch{
                    1 => Texture_Chair,
                    2 => Texture_Table,
                    3 => Texture_Spikes
                };

                EntityTexture.Render(C, Palette_World, WorldX + Entity.X, WorldY + Entity.Y, false, false, Entity.Rotation);
            }
        }
        
        Texture Player = Texture_Player;
        BlinkTimer += (float)TD.DeltaTimeS;

        if(BlinkTimer > 3 || Dead){
            Player = Texture_Player_Blink;
            if(BlinkTimer > 3.25f){
                BlinkTimer = 0;
            }
        }

        if(Health < 30){
            if(Player == Texture_Player){ Player = Texture_Player_Blood; }
            else if(Player == Texture_Player_Blink){ Player = Texture_Player_Blood_Blink; }
        }
        
        if(MovingDirection.X != 0){
            PlayerFlipped = MovingDirection.X > 0;
        }
        Player.Render(C, Palette_World, PlayerX, PlayerY, PlayerFlipped);

        foreach((int, int, byte) Block in __Blocks){
            if(Block.Item3 == 1){
                Texture BlockTexture = Texture_Wall;
                BlockTexture.Render(C, Palette_World, WorldX + Block.Item1, WorldY + Block.Item2);
            }
        }
        
        foreach(Entity Entity in __Entity){
            if(Entity.ID is 4){
                Texture EntityTexture = Entity.ID switch{
                    4 => (AnimationTimer > 0.5f ? Texture_Spider_Walk : Texture_Spider)
                };

                int OffsetX = 0;
                int OffsetY = 0;

                if(Entity.ID == 4){
                    OffsetX = 8;
                    OffsetY = 8;
                }
                EntityTexture.Render(C, Palette_World, WorldX + Entity.X - OffsetX, WorldY + Entity.Y - OffsetY, false, false, Entity.Rotation);
            }
        }
        
        if(RenderColliders){ Game.RenderColliders(C); }

        #region UI

            ColorB FrameColor = new ColorB((byte)(WL.Math.DSin((float)TD.DeltaTick * 2) * 255), 0, 0);

            C.Border(0, 0, C.Width, C.Height, 1, FrameColor);
            C.Border(1, 1, C.Width - 2, C.Height - 2, 1, FrameColor.Clone().SetA(128), ImageBlend.Alpha);
            C.Border(2, 2, C.Width - 4, C.Height - 4, 1, FrameColor.Clone().SetA(64), ImageBlend.Alpha);
            
            C.Fill(20 - 1, (int)C.Height - 16 - 1, HealthMax + 2, 8 + 2, ColorB.DarkRed);
            C.Fill(20, (int)C.Height - 16, HealthMax, 8, ColorB.Black);
            C.Fill(20, (int)C.Height - 16, Health, 8, ColorB.Red);
            C.Fill(20, (int)C.Height - 16 + 3, Health, 8 - 6, ColorB.LightRed);

            Texture_Health.Render(C, Palette_World, 3, (int)C.Height - 21);
            
        #endregion
    }

    public override ColorB BackgroundColor(){
        return ColorB.White;
    }

    private void StartLevel(byte Level){
        ClearAllEntityScene();
        ClearAllScene();
        
        for(int i = 0; i < 2; i++){
            for(int j = 0; j < 2; j++){
                AddScene(@"###'################################
#'''#___''''''#''''''''''''#_______#
''#'#_''''''''#''####'''''##_###____
#'#'#___''''''#''____''''_##_#'#___#
#'#'#####'''''#''####'''''##_______#
#'#'''''''''''#''''''''''''#_______#
#'####'################'#######'####
#''''#''''''''''''#''''''''''''''''#
####'####'#'''#'#'#'_#'''''######''#
#__'''__#'#'#'#'#'#''#_''''#___##''#
#'#'''#'#'#'#'#'#'#'_#_''''#'''''''#
#''###''#'#'#'#''''''#_''''######''#
#_''''''#'''#''_#'#'_#'''''#'_''#''#
'''''''_#'#'#'__#'#''#_''''####'''''
#'''''''#'''''''''#''''''''#'_''#''#
###'################################
", 2 + i * 35, 2 + j * 15);
            }
        }

        AddEntityScene(@"___C_T_^^^^___________s__CTC");
    }
    
    private void StartGame(){
        InMainMenu = false;
        
        WorldPosition = Vector2F.Zero;
        __Tracks.Clear();

        Health = HealthMax;
        
        StartLevel(0);
    }
    
    private bool RenderColliders = false;
    public override void KeyPress(Key Key, bool Down){
        if(Down){
            if(InMainMenu){
                if(Key is Key.Enter or Key.Space){ StartGame(); }
            }else{
                if(Key == Key.C){ RenderColliders = !RenderColliders; }

                if(Key == Key.Escape){ InMainMenu = true; }
            }
        }
    }
}
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
    
    public override void Update(TickData TD){
        Game.ClearColliders();

        if(InMainMenu){
            return;
        }
        
        foreach((int, int, byte) Block in __Blocks){
            if(Block.Item3 == 1){
                Game.AddCollider(new Collider(WorldX + Block.Item1, WorldY + Block.Item2, 16, 16));
            }
        }

        uint CameraSpeed = (uint)((float)TD.DeltaTime / 10 * (Game.KeyPressed(Key.Shift) ? 2 : 1));

        bool D = Game.KeyPressed(Key.D);
        bool A = Game.KeyPressed(Key.A);
        bool W = Game.KeyPressed(Key.W);
        bool S = Game.KeyPressed(Key.S);
        MovingDirection = new Vector2I(
            A && D ? 0 : (A ? 1 : (D ? -1 : 0)),
            W && S ? 0 : (W ? 1 : (S ? -1 : 0))
        );

        Vector2F DesiredMove = new Vector2F();

        uint PlayerSize = (uint)(Texture_Player.Width * 0.8f);
        int PlayerOffset = (int)((Texture_Player.Width - PlayerSize) / 2);

        if(MovingDirection.X != 0 && MovingDirection.Y != 0){
            for(uint i = 1; i <= CameraSpeed; i++){
                int TestX = (int)(PlayerX - MovingDirection.X * i + PlayerOffset);
                int TestY = (int)(PlayerY - MovingDirection.Y * i + PlayerOffset);

                Collider TestCollider = new Collider(TestX, TestY, PlayerSize, PlayerSize);

                if(!Collision(TestCollider)){
                    DesiredMove.X = MovingDirection.X * i;
                    DesiredMove.Y = MovingDirection.Y * i;
                }else{
                    TestCollider.X = TestX;
                    TestCollider.Y = PlayerY + PlayerOffset;
                    if(!Collision(TestCollider)){
                        DesiredMove.X = MovingDirection.X * i;
                        DesiredMove.Y = 0;
                    }else{
                        TestCollider.X = PlayerX + PlayerOffset;
                        TestCollider.Y = TestY;
                        if(!Collision(TestCollider)){
                            DesiredMove.X = 0;
                            DesiredMove.Y = MovingDirection.Y * i;
                        }else{
                            break;
                        }
                    }

                    break;
                }
            }
        }else{
            for(uint i = 1; i < CameraSpeed + 1; i++){
                if(!Collision(new Collider((int)(PlayerX - (MovingDirection.X * i) + PlayerOffset), PlayerY + PlayerOffset, PlayerSize, PlayerSize))){
                    DesiredMove.X = MovingDirection.X * i;
                }else{
                    break;   
                }
            }
        
            for(uint i = 1; i < CameraSpeed + 1; i++){
                if(!Collision(new Collider(PlayerX + PlayerOffset, (int)(PlayerY - (MovingDirection.Y * i) + PlayerOffset), PlayerSize, PlayerSize))){
                    DesiredMove.Y = MovingDirection.Y * i;
                }else{
                    break;   
                }
            }
        }
        
        WorldPosition += DesiredMove;
        
        if(DesiredMove.X != 0 || DesiredMove.Y != 0){ Track(); }
    }

    private readonly List<(int, int, byte)> __Tracks = [];
    private void Track(){
        if(WL.Math.Random.Fast_Bool(0.1f)){
            __Tracks.Add((PlayerX - WorldX, PlayerY - WorldY, (byte)(Health < 32 ? 1 : 0)));
        }
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
    
    private float BlinkTimer    = 0;
    private bool  PlayerFlipped = false;
    public override void Render(TickData TD, Image.ImageContext C){
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

        foreach((int, int, byte) Track in __Tracks){
            Texture Track__ = Track.Item3 == 1 ? Texture_Blood : Texture_Track;
            Track__.Render(C, Palette_World, WorldX + Track.Item1, WorldY + Track.Item2);
        }
        
        Texture Player = Texture_Player;
        BlinkTimer += (float)TD.DeltaTimeS;

        if(BlinkTimer > 3){
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

    private void StartGame(){
        InMainMenu = false;
        
        WorldPosition = Vector2F.Zero;
        __Tracks.Clear();
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
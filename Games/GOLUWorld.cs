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
    
    public override void Start(){
        Palette_World = new Palette([
            new KeyValuePair<byte, ColorB>(1, ColorB.Black),
            new KeyValuePair<byte, ColorB>(2, ColorB.DarkGray),
            new KeyValuePair<byte, ColorB>(3, ColorB.Gray),
            new KeyValuePair<byte, ColorB>(4, ColorB.LightGray),
            new KeyValuePair<byte, ColorB>(5, ColorB.White)
        ]);

        Dictionary<char, byte> Mapping = new Dictionary<char, byte>{
            ['.'] = 0,
            ['_'] = 5,
            ['█'] = 1,
            ['▓'] = 2,
            ['▒'] = 3,
            ['░'] = 4
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

Texture_Wall = new Texture(
@"▓▒▒▒▒░░░░░▒▒▒▒██
▒█___█░█_____░▓█
▒█░█░█▒█______▓█
▒██▒██▒█░_____▒█
▒█░_░█░████___▒█
▒_____________▒█
▒_____________▒█
▒_____________▒█
▒_____________▒█
▒_____________▒█
▒_____________▒█
▒_____________▒█
▒_____________▒█
▒░___________░▓█
█▓▒▒▒▒▒▒▒▒▒▒▒▓▓█
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

for(int i = 0; i < 100; i++){
    AddBlock(i, (int)(WL.Math.Cos(i/5f) * 3));
}

    }
    
    public override void Stop(){
        
    }
    
    private Vector2F WorldPosition = new Vector2F();

    private int PlayerX => (int)(Game.SceneSize.X / 2F - Texture_Player.Width  / 2F);
    private int PlayerY => (int)(Game.SceneSize.Y / 2F - Texture_Player.Height / 2F);
    
    private int WorldX => (int)(WorldPosition.X + Game.SceneSize.X / 2F);
    private int WorldY => (int)(WorldPosition.Y + Game.SceneSize.Y / 2F);
    
    public override void Update(TickData TD){
        Game.ClearColliders();
        
        foreach((int, int) Block in __Blocks){
            Game.AddCollider(new Collider(WorldX + Block.Item1, WorldY + Block.Item2, 16, 16));
        }
        
        float CameraSpeed = (float)TD.DeltaTime / 10;

        bool D = Game.KeyPressed(Key.D);
        bool A = Game.KeyPressed(Key.A);
        bool W = Game.KeyPressed(Key.W);
        bool S = Game.KeyPressed(Key.S);
        Vector2I Axis = new Vector2I(
            A && D ? 0 : (A ? 1 : (D ? -1 : 0)),
            W && S ? 0 : (W ? 1 : (S ? -1 : 0))
        );

        Vector2F DesiredMove = new Vector2F(Axis.X * CameraSpeed, Axis.Y * CameraSpeed);

        if(Collision(new Collider((int)(PlayerX - DesiredMove.X), PlayerY, Texture_Player.Width, Texture_Player.Height))){
            DesiredMove.X = 0;
        }
        
        if(Collision(new Collider(PlayerX, (int)(PlayerY - DesiredMove.Y), Texture_Player.Width, Texture_Player.Height))){
            DesiredMove.Y = 0;
        }
        
        WorldPosition += DesiredMove;
    }

    private void AddBlock(int X, int Y){
        __Blocks.Add((X * 16, Y * 16));
    }
    private readonly List<(int, int)> __Blocks = [];
    
    private float BlinkTimer = 0;
    public override void Render(TickData TD, Image.ImageContext C){
        Texture_Ground.Render(C, Palette_World, WorldX, WorldY, 30, 30);
        
        Texture Player = Texture_Player;
        BlinkTimer += (float)TD.DeltaTimeS;

        if(BlinkTimer > 3){
            Player = Texture_Player_Blink;
            if(BlinkTimer > 1.25f){
                BlinkTimer = 0;
            }
        }

        Player.Render(C, Palette_World, PlayerX, PlayerY);

        foreach((int, int) Block in __Blocks){
            Texture_Wall.Render(C, Palette_World, WorldX + Block.Item1, WorldY + Block.Item2);   
        }
        
        //Game.RenderColliders(C);
    }

    public override ColorB BackgroundColor(){
        return ColorB.White;
    }
    
    public override void KeyPress(Key Key, bool Down){
        
    }
}
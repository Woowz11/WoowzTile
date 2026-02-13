using WL;
using WLO;
using WoowzTile;
using WoowzTile.Objects;
using Char = WoowzTile.Objects.Char;
#pragma warning disable CS8618

namespace GOLUWorld;

public enum T_Block : byte{
    Empty = 0,
    Metal = 1,
    Ground_Planks = 2,
    Ground_Asphalt = 3,
    Bricks = 4,
    Ground_Sand = 5,
    Water = 6
}

public enum T_Entity : byte{
    Empty = 0,
    Chair = 1,
    Table = 2,
    Spikes = 3,
    Mob_Spider = 4,
    Tree = 5,
    Item = 6
}

public enum T_Item : byte{
    Empty = 0,
    FirstAidKit = 1
}

public enum T_Interface : byte{
    None = 0,
    Inventory = 1
}

public enum T_Decal : byte{
    Track = 0,
    Blood = 1,
    Zero  = 2,
    One   = 3
}

public enum T_Emotion : byte{
    Happiness = 0
}

public struct Block{
    public int     X;
    public int     Y;
    public T_Block ID;
}
    
public struct Entity{
    public int             X;
    public int             Y;
    public T_Entity        ID;
    public byte            Info;
    public Vector2I        InfoVector;
    public TextureRotation Rotation;
}

public class GOLUWorld : Game{
    private Palette Palette_World;

    private Font Font;
    
    private Texture Texture_Ground;
    private Texture Texture_Metal;
    private Texture Texture_Player_Body;
    private Texture Texture_Player_Eyes;
    private Texture Texture_Player_Eyes_Blink;
    private Texture Texture_Player_Mouth;
    private Texture Texture_Player_Mouth_Happy;
    private Texture Texture_Player_Mouth_Sad;
    private Texture Texture_Player_Nose;
    private Texture Texture_Player_Blood;
    private Texture Texture_Player_Blood_Strong;
    private Texture Texture_Planks;
    private Texture Texture_Track;
    private Texture Texture_Blood;
    private Texture Texture_Health;
    private Texture Texture_G;
    private Texture Texture_O;
    private Texture Texture_L;
    private Texture Texture_U;
    private Texture Texture_Author;
    private Texture Texture_Title;
    private Texture Texture_Chair;
    private Texture Texture_Table;
    private Texture Texture_Spikes;
    private Texture Texture_Spider;
    private Texture Texture_Spider_Anim;
    private Texture Texture_Asphalt;
    private Texture Texture_Bricks;
    private Texture Texture_Sand;
    private Texture Texture_Water;
    private Texture Texture_Water_Top;
    private Texture Texture_Water_Anim;
    private Texture Texture_Water_Top_Anim;
    private Texture Texture_Tree;
    private Texture Texture_Tree_Leaves;
    private Texture Texture_FirstAidKit;
    private Texture Texture_FirstAidKit_Icon;
    private Texture Texture_Player_Healed;
    private Texture Texture_Zero;
    private Texture Texture_One;
    
    /*
     * Блоки:
     * '_' - Пустота
     * '#' - Блок металла (стена)
     * ''' - Доски (пол)
     * 'A' - Асфальт (пол)
     * 'B' - Кирпичи (блок)
     * 'S' - Песок (пол)
     * 'W' - Вода (блок)
     * 
     * Сущности:
     * '_' - Пустота
     * 'C' - Стул
     * 'T' - Стол
     * '^' - Шипы
     * 's' - Паук (моб)
     * '!' - Дерево
     *
     * Коллизии:
     * L1 - Мир и игрок
     * L2 - Наносит урон если ходить в нём
     * L3 - Наносит всегда урон
     * L4 - Предмет
     */
    
    public override string Name(){ return "GOLUWorld"; }

    public override string WindowTitle(){ return new Vector2I(PlayerX - WorldX, PlayerY - WorldY).ToShortString() + " | " + Emotion_Happiness + " | " + InsideCollision + " (" + CollisionInfo + ", " + CollisionInfoSecond + ")"; }

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
            new KeyValuePair<byte, ColorB>(10, ColorB.DarkMagenta),
            new KeyValuePair<byte, ColorB>(11, ColorB.LightRed),
            new KeyValuePair<byte, ColorB>(12, ColorB.Green)
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
            ['m'] = 10,
            ['l'] = 11,
            ['G'] = 12
        };
        
        Font = new Font(new Char(new Texture(
@"████████
█..██..█
█.█..█.█
█....█.█
█...█..█
█......█
█...█..█
████████", Mapping)),
        [
            new KeyValuePair<char, Char>(
' ' ,
new Char(new Texture(
@"...
...
...
...
...
...
...
...", Mapping))),
            
new KeyValuePair<char, Char>(
'1' ,
new Char(new Texture(
@"..█..
.██..
█.█..
..█..
..█..
..█..
..█..
█████", Mapping))),

new KeyValuePair<char, Char>(
'2' ,
new Char(new Texture(
@".███.
█...█
....█
...█.
..█..
.█...
█....
█████", Mapping))),

new KeyValuePair<char, Char>(
'3' ,
new Char(new Texture(
@".███.
█...█
....█
.███.
....█
....█
█...█
.███.", Mapping))),

new KeyValuePair<char, Char>(
'4' ,
new Char(new Texture(
@"█...█
█...█
█...█
█████
....█
....█
....█
....█", Mapping))),

new KeyValuePair<char, Char>(
'5' ,
new Char(new Texture(
@"█████
█....
█....
████.
....█
....█
....█
████.", Mapping))),

new KeyValuePair<char, Char>(
'6' ,
new Char(new Texture(
@".███.
█...█
█....
████.
█...█
█...█
█...█
.███.", Mapping))),

new KeyValuePair<char, Char>(
'7' ,
new Char(new Texture(
@"█████
....█
....█
...█.
.████
...█.
...█.
...█.", Mapping))),

new KeyValuePair<char, Char>(
'8' ,
new Char(new Texture(
@".███.
█...█
█...█
.███.
█...█
█...█
█...█
.███.", Mapping))),

new KeyValuePair<char, Char>(
'9' ,
new Char(new Texture(
@".███.
█...█
█...█
█...█
.████
....█
█...█
.███.", Mapping))),

new KeyValuePair<char, Char>(
'0' ,
new Char(new Texture(
@".███.
█...█
█...█
█.█.█
█.█.█
█...█
█...█
.███.", Mapping))),

new KeyValuePair<char, Char>(
'А' ,
new Char(new Texture(
@".███.
█...█
█...█
█████
█...█
█...█
█...█
█...█", Mapping))),

new KeyValuePair<char, Char>(
'Б' ,
new Char(new Texture(
@"█████
█....
█....
████.
█...█
█...█
█...█
████.", Mapping))),

new KeyValuePair<char, Char>(
'В' ,
new Char(new Texture(
@"████.
█...█
█...█
████.
█...█
█...█
█...█
████.", Mapping))),

new KeyValuePair<char, Char>(
'Г' ,
new Char(new Texture(
@"█████
█....
█....
█....
█....
█....
█....
█....", Mapping))),

new KeyValuePair<char, Char>(
'Д' ,
new Char(new Texture(
@".█████.
.█...█.
.█...█.
.█...█.
.█...█.
███████
█.....█
█.....█", Mapping))),

new KeyValuePair<char, Char>(
'Е' ,
new Char(new Texture(
@"█████
█....
█....
█████
█....
█....
█....
█████", Mapping))),

new KeyValuePair<char, Char>(
'Ё' ,
new Char(new Texture(
@"█...█
.....
█████
█....
█████
█....
█....
█████", Mapping))),

new KeyValuePair<char, Char>(
'Ж' ,
new Char(new Texture(
@"█..█..█
█..█..█
█..█..█
.█████.
█..█..█
█..█..█
█..█..█
█..█..█", Mapping))),

new KeyValuePair<char, Char>(
'З' ,
new Char(new Texture(
@".███.
█...█
....█
..██.
....█
....█
█...█
.███.", Mapping))),

new KeyValuePair<char, Char>(
'И' ,
new Char(new Texture(
@"█...█
█...█
█..██
█.█.█
██..█
█...█
█...█
█...█", Mapping))),

new KeyValuePair<char, Char>(
'Й' ,
new Char(new Texture(
@".███.
.....
█...█
█..██
█.█.█
██..█
█...█
█...█", Mapping))),

new KeyValuePair<char, Char>(
'К' ,
new Char(new Texture(
@"█...█
█...█
█..█.
███..
█..█.
█...█
█...█
█...█", Mapping))),

new KeyValuePair<char, Char>(
'Л' ,
new Char(new Texture(
@".█████
.█...█
.█...█
.█...█
.█...█
.█...█
█....█
█....█", Mapping))),

new KeyValuePair<char, Char>(
'М' ,
new Char(new Texture(
@"█...█
██.██
█.█.█
█...█
█...█
█...█
█...█
█...█", Mapping))),

new KeyValuePair<char, Char>(
'Н' ,
new Char(new Texture(
@"█...█
█...█
█...█
█████
█...█
█...█
█...█
█...█", Mapping))),

new KeyValuePair<char, Char>(
'О' ,
new Char(new Texture(
@".███.
█...█
█...█
█...█
█...█
█...█
█...█
.███.", Mapping))),

new KeyValuePair<char, Char>(
'П' ,
new Char(new Texture(
@"█████
█...█
█...█
█...█
█...█
█...█
█...█
█...█", Mapping))),

new KeyValuePair<char, Char>(
'Р' ,
new Char(new Texture(
@"████.
█...█
█...█
████.
█....
█....
█....
█....", Mapping))),

new KeyValuePair<char, Char>(
'С' ,
new Char(new Texture(
@".███.
█...█
█....
█....
█....
█....
█...█
.███.", Mapping))),

new KeyValuePair<char, Char>(
'Т' ,
new Char(new Texture(
@"█████
..█..
..█..
..█..
..█..
..█..
..█..
..█..", Mapping))),

new KeyValuePair<char, Char>(
'У' ,
new Char(new Texture(
@"█...█
█...█
█...█
.████
....█
....█
█...█
.███.", Mapping))),

new KeyValuePair<char, Char>(
'Ф' ,
new Char(new Texture(
@".███.
█.█.█
█.█.█
.███.
..█..
..█..
..█..
..█..", Mapping))),

new KeyValuePair<char, Char>(
'Х' ,
new Char(new Texture(
@"█...█
█...█
.█.█.
..█..
.█.█.
█...█
█...█
█...█", Mapping))),

new KeyValuePair<char, Char>(
'Ц' ,
new Char(new Texture(
@"█...█.
█...█.
█...█.
█...█.
█...█.
█...█.
██████
.....█", Mapping))),

new KeyValuePair<char, Char>(
'Ч' ,
new Char(new Texture(
@"█...█
█...█
█...█
.████
....█
....█
....█
....█", Mapping))),

new KeyValuePair<char, Char>(
'Ш' ,
new Char(new Texture(
@"█..█..█
█..█..█
█..█..█
█..█..█
█..█..█
█..█..█
█..█..█
███████", Mapping))),

new KeyValuePair<char, Char>(
'Щ' ,
new Char(new Texture(
@"█..█..█.
█..█..█.
█..█..█.
█..█..█.
█..█..█.
█..█..█.
████████
.......█", Mapping))),

new KeyValuePair<char, Char>(
'Ъ' ,
new Char(new Texture(
@"███....
..█....
..█....
..████.
..█...█
..█...█
..█...█
..████.", Mapping))),

new KeyValuePair<char, Char>(
'Ы' ,
new Char(new Texture(
@"█.....█
█.....█
█.....█
████..█
█...█.█
█...█.█
█...█.█
████..█", Mapping))),

new KeyValuePair<char, Char>(
'Ь' ,
new Char(new Texture(
@"█....
█....
█....
████.
█...█
█...█
█...█
████.", Mapping))),

new KeyValuePair<char, Char>(
'Э' ,
new Char(new Texture(
@".███.
█...█
....█
..███
....█
....█
█...█
.███.", Mapping))),

new KeyValuePair<char, Char>(
'Ю' ,
new Char(new Texture(
@"█..███.
█.█...█
█.█...█
███...█
█.█...█
█.█...█
█.█...█
█..███.", Mapping))),
        
new KeyValuePair<char, Char>(
'Я' ,
new Char(new Texture(
@".████
█...█
█...█
.████
█...█
█...█
█...█
█...█", Mapping))),

new KeyValuePair<char, Char>(
'[' ,
new Char(new Texture(
@"███
█..
█..
█..
█..
█..
█..
███", Mapping))),

new KeyValuePair<char, Char>(
']' ,
new Char(new Texture(
@"███
..█
..█
..█
..█
..█
..█
███", Mapping))),
            
new KeyValuePair<char, Char>(
'(' ,
new Char(new Texture(
@"..█
.█.
█..
█..
█..
█..
.█.
..█", Mapping))),

new KeyValuePair<char, Char>(
')' ,
new Char(new Texture(
@"█..
.█.
..█
..█
..█
..█
.█.
█..", Mapping))),
            
new KeyValuePair<char, Char>(
'.' ,
new Char(new Texture(
    @"...
...
...
...
...
...
...
.█.", Mapping))),

new KeyValuePair<char, Char>(
',' ,
new Char(new Texture(
    @"...
...
...
...
...
...
██.
.█.", Mapping))),
            
new KeyValuePair<char, Char>(
'+' ,
new Char(new Texture(
    @".....
..█..
..█..
█████
..█..
..█..
.....
.....", Mapping))),

new KeyValuePair<char, Char>(
'-' ,
new Char(new Texture(
    @".....
.....
.....
█████
.....
.....
.....
.....", Mapping))),
            
        ]);
        
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
        
        Texture_Planks = new Texture(
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
        
        Texture_Asphalt = new Texture(
            @"▓▒▓▓▓▓▒▓▓▒▓▓▓▓▒▓
▓▓▓▓▓▓▒▓▓▓▒▓▓▓▒▓
▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓
▓▒▓▓▒▓▓▒▓▓▓▓▓▓▓▒
▓▓▓▓▓▓▓▓▒▓▓▓▒▓▒▓
▒▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓
▓▒▒▓▓▓▓▒▓▓▒▓▓▒▓▓
▓▓▓▒▒▓▓▓▓▓▓▓▓▓▒▓
▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▒
▓▓▓▓▓▓▒▓▓▓▓▒▓▓▓▓
▒▓▓▓▒▓▓▓▓▓▓▓▒▓▓▒
▓▓▒▓▓▓▓▓▓▓▒▓▓▓▓▓
▓▒▓▓▓▓▓▓▒▓▓▓▓▓▒▓
▓▓▓▓▓▒▓▓▓▓▒▓▓▓▓▒
▓▓▓▓▓▒▓▓▓▓▓▒▓▓▓▓
▓▒▓▓▓▓▒▓▓▓▓▓▓▓▓▓",
            Mapping
        );
        
        Texture_Sand = new Texture(
            @"░░░░_░░░░░░░░▒░░
▒░░░░░░░▒░░░░░░░
░░░░▒░░░░░░░░░░_
░▓░░░░░░░░░▒░▓░░
░░▒░░░░░_░░░░░░░
░░░░░░░░░░░░░░▒░
░░░░░░░▒░░░_░░░░
▒░░░░░░░░░░░░░░░
░░░░▒░░░░▓░░░▒░░
░░_░░░░░░░░░░░░░
░░░░░░▒░░░▒░░░_░
░▒░░░░░░░░░░░░░░
░░░░░▓░░░░░░░░▒░
░░░▒░░░░_░▒░░░░░
░░░░░░░░░░░░░░░_
░░_░░░▒░░░░░░░░░",
            Mapping
        );

Texture_Metal = new Texture(
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

Texture_Bricks = new Texture(
    @"░░▒▓▒░░░░░▒▓▒░░░
_░░▓░░___░░▓░░__
▒▒▒▓▒▒▒▒▒▒▒▓▒▒▒▒
████████████████
▒░░░░░▒▓▒░░░░░▒▓
░░___░░▓░░___░░▓
▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▓
████████████████
░░▒▓▒░░░░░▒▓▒░░░
_░░▓░░___░░▓░░__
▒▒▒▓▒▒▒▒▒▒▒▓▒▒▒▒
████████████████
▒░░░░░▒▓▒░░░░░▒▓
░░___░░▓░░___░░▓
▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▓
████████████████",
    Mapping
);

Texture_Water = new Texture(
    @"RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRlllllllRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRlllllllR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR",
    Mapping
);

Texture_Water_Top = new Texture(
    @"▒▒▓▓▓▓▒▒▒░░▒▓▓▒▒
░░░░░▒▒▒▒▒░░░░░░
▒▒▒░░░▒▒░░░░▒▒▒▒
rrrrrrrrrrrrrrrr
RRRrrRRRRRRRrrrR
RrRRRRRRRRrRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRlllllllR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR",
    Mapping
);

Texture_Water_Anim = new Texture(
    @"RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
lllllllllllRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRllllllllll
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR",
    Mapping
);

Texture_Water_Top_Anim = new Texture(
    @"▒▒▓▓▓▓▒▒▒░░▒▓▓▒▒
░░░░░▒▒▒▒▒░░░░░░
▒▒▒░░░▒▒░░░░▒▒▒▒
rrrrrrrrrrrrrrrr
RRRrrRRRRRRRrrrR
RrRRRRRRRRrRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRllllllllll
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR
RRRRRRRRRRRRRRRR",
    Mapping
);

Texture_Player_Body = new Texture(
    @"...██████████...
..█▒▒░░░__░░▒█..
.█░__________▒█.
█░____________▒█
█░____________░█
█░____________░█
█░____________░█
█░____________░█
█░____________░█
█░____________░█
█░____________░█
█░____________░█
█▒░__________░▒█
.█▒░________░▒█.
..█▓▓░░__░░▓▓█..
...██████████...",
    Mapping
);

Texture_Player_Eyes = new Texture(
    @"................
................
.........▒███▒..
..▒███▒▒▒█_█_█..
..█_█_█▓▓█_█░█..
..█_█░█.▒▒███▒..
..▒███▒....░░...
..░░░...........
................
................
................
................
................
................
................
................",
    Mapping
);

Texture_Player_Eyes_Blink = new Texture(
    @"................
................
.........▒___▒..
..▒___▒▒▒█▒▒▒█..
..█▒▒▒█▓▓█████..
..█████.▒▒___▒..
..▒___▒....░░...
..░░░...........
................
................
................
................
................
................
................
................",
    Mapping
);

Texture_Player_Mouth_Sad = new Texture(
    @"................
................
................
................
................
................
................
................
..............▒.
....░...........
...__▒▒__▒▒_░...
....▓██████▓....
...▓░░____░░▓...
.....▒▒▒▒▒▒.....
................
................",
    Mapping
);

Texture_Player_Mouth = new Texture(
    @"................
................
................
................
................
................
................
................
..............▒.
....░...........
...__▒▒__▒▒_░...
..▓▓▓██████▓▓▓..
....░░____░░....
.....▒▒▒▒▒▒.....
................
................",
    Mapping
);

Texture_Player_Mouth_Happy = new Texture(
    @"................
................
................
................
................
................
................
................
..............▒.
.▒..░........░█.
..▓__▒▒__▒▒_░█..
...▓▓██████▓▓...
....░░____░░....
.....▒▒▒▒▒▒.....
................
................",
    Mapping
);

Texture_Player_Nose = new Texture(
    @"................
................
................
................
................
......._▒.......
......._▓..░░...
......._▓...▒░..
......░__▓.░....
.......▒█▓......
................
................
................
................
................
................",
    Mapping
);

Texture_Player_Blood = new Texture(
    @".....r..........
....R...........
......R.........
................
................
...............r
..............Rr
................
...R............
................
................
rR..............
............RR..
...........RR...
..........R.....
.........rr.r...",
    Mapping
);

Texture_Player_Blood_Strong = new Texture(
    @".....rrrrr......
....R..RRR......
......RRR.......
.......RR.......
.......rr.......
.......rR....m.r
r.m....R..mmmmRr
rRmmmmRr..mmmrRr
rRRrmmrR..rmRRRr
rRR.rRrR....RRrr
.....RR.....RrRr
rR..rrr.....rrR.
.....RRR....RRR.
....RRRRR..RR...
...RRRR.R.R.....
...rrrr.rrr.r...",
    Mapping
);

Texture_Player_Healed = new Texture(
    @"..██▓▓▓▓▓▓▓▓██..
.█░░_▓▓▓░____░█.
█▒▒░░__░▓░____░█
.█▓▓▓▓░_░▓▓█_░▒█
......▓░___░████
.......▓░░░_░▒▒█
........▓▓▓░▒▒▒█
...........▓▓▓██
................
................
................
................
................
................
................
................",
    Mapping
);

Texture_Tree = new Texture(
    @".█▓▒▒▓▓▓▓▒▓▒▒▓█.
.█▓▒▒▒▒▒▓▓▒▒▒▓█.
.█▓▒▒▒▒▒▒▒▒▒▓▓█.
.█▓▒▒▒▒▒▒▒▒▒▓▓█.
.█▓▒▒▒▒▒▒▒▒▒▓▓█.
.█▓▒▒░▒▒▒▒░▒▓▓█.
.█▓▒▒░▒░░░░▒▓▓█.
.█▓▓▒░▒_░░░▒▓▓█.
.█▓▓▒░▒_░▒░▒▒▓█.
.█▓▓▒░▒_░▒░▒▒▓█.
.█▓▓▒░░_░▒░▒▒▓█.
.█▓▓▒░░_░▒░▒▒▓█.
.█▓▒▒░░_░▒░▒▒▓█.
..█▓▒░▒░░▒░▒▓█..
...█▓▒▒▒▒▒▒▓█...
.....█▓▓▓▓█.....",
    Mapping
);

Texture_Tree_Leaves = new Texture(
    @"................................
................................
................................
................................
................................
................................
................................
............███████.............
......████.█▓▒▒▒░▒▓█.███........
.....█▓_░▒██░░▒▒▒░_▒█▒▒▓██......
....█░▒▒▒_▒█▒▓▒▒▒░__░__▒▒▓█.....
...█▓_▒▒▒_█.█▓▒▒▒___▒█▓▒░░▓█....
....█▓▒▒▒_░█▒___▒__▒█▓░░__░▓█...
.....█▒___▒___░░__▒▓░__▒__░▒█...
.....█░_▒▒█░_░▒▒▒▓▓▓▒▒█▓░░▒▓█...
....█▒____▒█▒▒▒▒▒▓▓__▒█▓▒▒▓█....
...█▒_____▓▓▒__▓▓▒▒__█▒▒▓▓█.....
...█▓_▒▒▒▓__░_▒▒_____▒░████.....
....█▓▓░░__█▒░_▒▒▒___░░█.█▓█....
.....██░__▒█▓▒▒▒▒▒▓█__▒▓█▒▒▓█...
....█▒__▒▒▓▓█▒▒▓▓██▓___▒░__▒█...
...█▒▒_░▓▓▒░_███▒▒_________▒█...
....█▓▒▓▓▓▒__▒_________▓▒░▓█....
....██▓▒____░▒▒▒▒▒▒▒_░░▒▓▓██....
...█▒▒██▒▒▓▓█▒▒░░▒▓██▒▓▓██▒▒█...
...█▒▒_░▓▓▒░_███▒▒_________▒█...
....█▓▒▓▓▓▒__▒_________▓▒░▓█....
.....█▓▒____░▒▒▒▒▒▒▒_░░▒▓▓█.....
......██▒▒▓▓█▒▒░░▒▓██▒▓▓██......
.......(████(█░░▒▓█((███(.......
.........(((((████((((..........
............((((((((............",
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

Texture_Zero = new Texture(
    @"................
................
................
................
................
................
.......GG.......
......G..G......
......G..G......
......G..G......
.......GG.......
................
................
................
................
................",
    Mapping
);

Texture_One = new Texture(
    @"................
................
................
................
................
................
........G.......
.......GG.......
........G.......
........G.......
.......GGG......
................
................
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

Texture_Spider_Anim = new Texture(
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

Texture_FirstAidKit = new Texture(
    @"................
................
...█████████....
..█▒▒▒▒░░▒▒▒█...
.█▓█▓▒____░░░█..
.█▒▒█▒░░░░░▒▒██.
.█▒░██▒▓▒▒▒▒▓▒█.
.█▒░▒▒█▓████▓░█.
.█▒░▒▒_▓___░▓░█.
.█▒░░░__░r░__░█.
..█░▒___RRr__░█.
..█▒▒▒___R__░█..
...██▓▒░__▒▒█...
.....███████....
................
................",
    Mapping
);

Texture_FirstAidKit_Icon = new Texture(
    @"..................................
..................................
..................................
..................................
..................................
..........███████████████.........
........██▓▓▒▒▒▒▒▒▒▒▒▒▒▓▓███......
.......█▒▒▒▒▒▒░░░░░▒▒▒▒▒▒▒▒▒█.....
......█▒░░░░░░░░░░░░░░░░░▒▓▓█.....
.....█▒░░___________░░░▒▒▓███.....
.....█░░░░░░_________░▒▒▓███▓█....
....██░░░░░░░░░░░░░░░░▒▓███▓▒█....
...███▒▒▒▒░░░░░░░░░░░░▒▓██▒▒▒█....
...█▓█▒▓▓▒▒▒▒▒▒▒░░░░▒▒▒▓█▒▒▒▒█....
...█▒▒█▓▓▒▒▒▒▒▒▒▒▒▓▓▒▒███▒░░▒█....
...█▒▒▒▓▓▒▒▒▒▒▒▒▒▒▓▓▒█▒█▒░░░▒█....
...█▒░░▓▓█████████▓▓█▒▒█▒░░░▒█....
...█░░░▓▓▒▒░░░░░▒▒▓▓▒▒▒█▒░░░▒█....
...█░░░░░░░░░░░░░░▓▓░▒▒█▒░░░▒█....
...█░░░░░░░__rR______░▒█▒░░░▒█....
...█▒___░░__rRR_______▒█▒░░░▒█....
....█______░rRR░______░█░░░░▒█....
....█░___rrrRRRRrr_____█░░░░▒█....
....█░░___rrrRRRrr_____█▒░░▒█.....
....█▒░_____░RRr░______█▒░░▒█.....
....█▒▒______Rr░______░█▒░▒▒█.....
....█▓▒░_____Rr______░▒█▒▒▒█......
.....██▒▒░__________░▒▒█▓▒█.......
.......██▒▒▒░_____░░▒▒▓███........
.........███████████████..........
..................................
..................................
..................................
..................................",
    Mapping
);

    }
    
    public override void Stop(){
        
    }
    
    private Vector2F WorldPosition = new Vector2F();

    private int PlayerX => (int)(Game.SceneSize.X / 2F - Texture_Player_Body.Width  / 2F);
    private int PlayerY => (int)(Game.SceneSize.Y / 2F - Texture_Player_Body.Height / 2F);
    
    private int WorldX => (int)(WorldPosition.X + Game.SceneSize.X / 2F);
    private int WorldY => (int)(WorldPosition.Y + Game.SceneSize.Y / 2F);
    
    private bool     Moving => MovingDirection != Vector2I.Zero;
    private Vector2I MovingDirection = Vector2I.Zero;

    private CollisionLayer InsideCollision = CollisionLayer.None;
    private byte           CollisionInfo       = 0;
    private int            CollisionInfoSecond = 0;
    
    private const uint HealthMax   = 100;
    private const uint HealthSmall = 30;
    private       uint Health      = HealthMax;

    private bool InMainMenu = true;
    private T_Interface Interface = T_Interface.None;

    private bool Dead => Health == 0;

    private bool StopTime = false;

    private const byte MaxSlots = 12;
    private byte SelectedItem   = 0;

    private readonly T_Item[] Inventory = new T_Item[MaxSlots];

    private float LastHealed = 0;

    private float Rotten = 0;

    private const uint Emotion_Max       = 100;
    private       uint Emotion_Happiness = Emotion_Max;

    private void EmotionChange(T_Emotion Emotion, int Value){
        if(Value == 0){ return; }
        
        uint Value__ = Emotion switch{
            T_Emotion.Happiness => Emotion_Happiness
        };

        if(Value < 0){
            Value__ = WL.Math.SubU(Value__, (uint)WL.Math.Abs(Value));
        }else{
            Value__ += (uint)Value;
            if(Value__ > Emotion_Max){ Value__ = Emotion_Max; }
        }

        switch(Emotion){
            case T_Emotion.Happiness: Emotion_Happiness = Value__; break;
        }
    }
    
    private void Damage(uint Damage, int Range = 0){
        if(Damage == 0){ return; }
        
        Health = WL.Math.SubU(Health, Damage);

        SplatBlood(PlayerX - WorldX + WL.Math.Random.Fast_Int(-Range, Range), PlayerY - WorldY + WL.Math.Random.Fast_Int(-Range, Range));

        EmotionChange(T_Emotion.Happiness, -(int)Damage * 2);
    }
    
    private void Heal(uint Heal, bool FirstAidKit = false){
        if(Heal == 0){ return; }
        
        Health += Heal;
        if(Health > HealthMax){ Health = HealthMax; }

        if(FirstAidKit){ LastHealed = 60; }
        
        EmotionChange(T_Emotion.Happiness, (int)Heal);
    }
    
    public override void Update(TickData TD){
        Game.ClearColliders();

        if(InMainMenu){
            return;
        }

        StopTime = Interface != 0;

        if(StopTime){ return; }
        
        if(!Dead){
            Heal((uint)(WL.Math.Random.Fast_Bool(0.001f) ? 1 : 0));
            
            EmotionChange(T_Emotion.Happiness, WL.Math.Random.Fast_Bool(0.01f) ? 1 : 0);
        }else{
            Interface = 0;
        }
        
        foreach(Block Block in __Blocks){
            if(Block.ID is T_Block.Metal or T_Block.Bricks or T_Block.Water){
                Game.AddCollider(new Collider(WorldX + Block.X, WorldY + Block.Y, 16, 16));
            }
        }
        
        for(int i = 0; i < __Entity.Count; i++){
            Entity Entity = __Entity[i];
            
            if(Entity.ID is T_Entity.Table or T_Entity.Spikes or T_Entity.Mob_Spider or T_Entity.Tree){
                if(Entity.ID == T_Entity.Mob_Spider){
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
                    
                    if(Distance < 100 && Rotten < 10){

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
                
                uint SizeX = 16;
                uint SizeY = 16;
                if(Entity.ID is T_Entity.Table or T_Entity.Tree){
                    SizeX = SizeY = 10;
                }

                CollisionLayer Layer = CollisionLayer.L1;
                if(Entity.ID == T_Entity.Spikes){
                    Layer = CollisionLayer.L2;
                }else if(Entity.ID == T_Entity.Mob_Spider){
                    Layer = CollisionLayer.L3;
                }
                Game.AddCollider(new Collider(WorldX + Entity.X + (int)((16 - SizeX)/2), WorldY + Entity.Y + (int)((16 - SizeY)/2), SizeX, SizeY, 0, i, Layer));
            }

            if(Entity.ID is T_Entity.Item){
                Game.AddCollider(new Collider(WorldX + Entity.X, WorldY + Entity.Y, 16, 16, Entity.Info, i, CollisionLayer.L4));
            }
        }

        bool CanMove = !Dead;

        if(Dead){
            if(WL.Math.Random.Fast_Bool(0.8f)){
                __Tracks.Add((PlayerX - WorldX + WL.Math.Random.Fast_Int(-128, 128), PlayerY - WorldY + WL.Math.Random.Fast_Int(-128, 128), WL.Math.Random.Fast_Bool() ? T_Decal.One : T_Decal.Zero, TextureRotation.None));
            }

            Rotten += (float)TD.DeltaTimeS;
        }
        
        uint PlayerSize = (uint)(Texture_Player_Body.Width * 0.8f);
        int PlayerOffset = (int)((Texture_Player_Body.Width - PlayerSize) / 2);
        
        if(CanMove){
            uint PlayerSpeed = (uint)(TD.DeltaTimeS * 100 * (Game.KeyPressed(Key.Shift) ? 1.5 : 1));
            if(Health < HealthSmall){ PlayerSpeed = (uint)(PlayerSpeed / 2); }

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

                    Collider TestCollider = new Collider(TestX, TestY, PlayerSize, PlayerSize, 0, 0, CollisionLayer.L1, WallCollider);

                    if(!Collision(TestCollider, out Collider? _)){
                        DesiredMove.X = MovingDirection.X * i;
                        DesiredMove.Y = MovingDirection.Y * i;
                    }
                    else{
                        TestCollider.X = TestX;
                        TestCollider.Y = PlayerY + PlayerOffset;
                        if(!Collision(TestCollider, out Collider? _)){
                            DesiredMove.X = MovingDirection.X * i;
                            DesiredMove.Y = 0;
                        }
                        else{
                            TestCollider.X = PlayerX + PlayerOffset;
                            TestCollider.Y = TestY;
                            if(!Collision(TestCollider, out Collider? _)){
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
                    if(!Collision(new Collider((int)(PlayerX - (MovingDirection.X * i) + PlayerOffset), PlayerY + PlayerOffset, PlayerSize, PlayerSize, 0, 0, CollisionLayer.L1, WallCollider), out Collider? _)){
                        DesiredMove.X = MovingDirection.X * i;
                    }
                    else{
                        break;
                    }
                }

                for(uint i = 1; i < PlayerSpeed + 1; i++){
                    if(!Collision(new Collider(PlayerX + PlayerOffset, (int)(PlayerY - (MovingDirection.Y * i) + PlayerOffset), PlayerSize, PlayerSize, 0, 0, CollisionLayer.L1, WallCollider), out Collider? _)){
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

                if(Collision(new Collider((int)(PlayerX + PlayerOffset), PlayerY + PlayerOffset, PlayerSize, PlayerSize, 0, 0, CollisionLayer.L1, CollisionLayer.L2), out Collider? _)){
                    if(WL.Math.Random.Fast_Bool(0.5f)){
                        Damage((uint)(WL.Math.Random.Fast_0_1() * 5));
                    }
                }
            }
        }
        
        if(Collision(new Collider((int)(PlayerX + PlayerOffset), PlayerY + PlayerOffset, PlayerSize, PlayerSize, 0, 0, CollisionLayer.L1, CollisionLayer.L3), out Collider? _)){
            if(WL.Math.Random.Fast_Bool(0.8f)){
                Damage((uint)(WL.Math.Random.Fast_0_1() * 20), Dead ? 16 : 0);
            }
        }
        
        if(Collision(new Collider((int)(PlayerX + PlayerOffset), PlayerY + PlayerOffset, PlayerSize, PlayerSize, 0, 0, CollisionLayer.L1, CollisionLayer.All), out Collider? Collider__)){
            InsideCollision     = Collider__!.Value.Layer;
            CollisionInfo       = Collider__!.Value.Info;
            CollisionInfoSecond = Collider__!.Value.InfoSecond;
        }else{
            InsideCollision     = CollisionLayer.None;
            CollisionInfo       = 0;
            CollisionInfoSecond = 0;
        }
    }

    private readonly List<(int, int, T_Decal, TextureRotation)> __Tracks = [];
    private void Track(){
        if(WL.Math.Random.Fast_Bool(0.1f)){
            if(Health < HealthSmall){
                SplatBlood(PlayerX - WorldX, PlayerY - WorldY);
            }else{
                __Tracks.Add((PlayerX - WorldX, PlayerY - WorldY, T_Decal.Track, TextureRotation.None));
            }
        }
    }

    private void SplatBlood(int X, int Y){
        __Tracks.Add((X, Y, T_Decal.Blood, WL.Math.Random.Fast_Bool(0.5f) ? (WL.Math.Random.Fast_Bool(0.5f) ? TextureRotation.None :  TextureRotation.Rotate90) : (WL.Math.Random.Fast_Bool(0.5f) ? TextureRotation.Rotate180 : TextureRotation.Rotate270)));
    }

    private void AddBlock(Block Block__){
        Block__.X *= 16;
        Block__.Y *= 16;

        int Index = __Blocks.FindIndex(B => B.X == Block__.X && B.Y == Block__.Y);
        
        if(Index != -1){
            if(Block__.ID == T_Block.Empty){
                __Blocks.RemoveAt(Index);
            }else{
                Block OldBlock = __Blocks[Index];
                if(OldBlock.ID != Block__.ID){
                    __Blocks[Index] = Block__;
                }
            }
        }else{
            if(Block__.ID != T_Block.Empty){
                __Blocks.Add(Block__);
            }
        }
    }
    private readonly List<Block> __Blocks = [];
    
    private void ClearAllScene(){
        __Blocks.Clear();
    }
    
    private void AddScene(string SceneMap, int X = 0, int Y = 0){
        try{
            if(string.IsNullOrEmpty(SceneMap)){ return; }
            
            int X__ = X;
            int Y__ = Y;
            
            foreach(char C in SceneMap){
                T_Block ID = T_Block.Empty;
                switch(C){
                    case '\r': 
                        continue;
                    case '\n':
                        Y__++;
                        X__ = X;
                        continue;
                    case '#':
                        ID = T_Block.Metal;
                        break;
                    case '\'':
                        ID = T_Block.Ground_Planks;
                        break;
                    case 'A':
                        ID = T_Block.Ground_Asphalt;
                        break;
                    case 'B':
                        ID = T_Block.Bricks;
                        break;
                    case 'S':
                        ID = T_Block.Ground_Sand;
                        break;
                    case 'W':
                        ID = T_Block.Water;
                        break;
                }

                if(ID != T_Block.Empty){
                    AddBlock(new Block{ X = X__, Y = Y__, ID = ID});
                }
                
                X__++;
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при загрузке сцены!", e);
        }
    }
    
    private void AddEntity(Entity Entity__, bool SnapToGrid = true){
        if(SnapToGrid){
            Entity__.X *= 16;
            Entity__.Y *= 16;
        }

        if(Entity__.ID != T_Entity.Empty){
            __Entity.Add(Entity__);
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
                T_Entity ID = T_Entity.Empty;
                switch(C){
                    case '\r': 
                        continue;
                    case '\n':
                        Y__++;
                        X__ = X;
                        continue;
                    case 'C':
                        ID = T_Entity.Chair;
                        break;
                    case 'T':
                        ID = T_Entity.Table;
                        break;
                    case '^':
                        ID = T_Entity.Spikes;
                        break;
                    case 's':
                        ID = T_Entity.Mob_Spider;
                        break;
                    case '!':
                        ID = T_Entity.Tree;
                        break;
                }

                if(ID != T_Entity.Empty){
                    AddEntity(new Entity{ X = X__, Y = Y__, ID = ID});
                }

                X__++;
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при загрузке Entity сцены!", e);
        }
    }

    private void SpawnItem(int X, int Y, T_Item Item){
        AddEntity(new Entity{ X = X, Y = Y, ID = T_Entity.Item, Info = (byte)Item}, false);
    }
    
    private float BlinkTimer     = 0;
    private float AnimationTimer = 0;
    private bool  PlayerFlipped  = false;
    public override void Render(TickData TD, Image.ImageContext C){
        if(!StopTime){
            AnimationTimer += (float)TD.DeltaTimeS;
            
            if(!Dead){ LastHealed -= (float)TD.DeltaTimeS; }
        }
        if(AnimationTimer > 1){ AnimationTimer = 0; }
        
        if(InMainMenu){
            Font.Render(C, Palette_World, ((float)TD.DeltaTick * 6) + "\n" + ((float)TD.DeltaTick * 5) + "\n" + ((float)TD.DeltaTick * 4) + "\n" + ((float)TD.DeltaTick * 3) + "\n" + ((float)TD.DeltaTick * 2) + "\n" + (float)TD.DeltaTick, 5, (int)C.Height - 100);
            
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
        
        foreach(Block Block in __Blocks){
            if(Block.ID is T_Block.Ground_Planks or T_Block.Ground_Asphalt or T_Block.Ground_Sand or T_Block.Water){
                Texture BlockTexture = Block.ID switch{
                    T_Block.Ground_Planks  => Texture_Planks,
                    T_Block.Ground_Asphalt => Texture_Asphalt,
                    T_Block.Ground_Sand    => Texture_Sand,
                    T_Block.Water          => (__Blocks.Any(B => B.X == Block.X && B.Y == Block.Y - 16 && B.ID == Block.ID) ? (AnimationTimer > 0.5f ? Texture_Water_Anim : Texture_Water) : (AnimationTimer > 0.5f ? Texture_Water_Top_Anim : Texture_Water_Top))
                };
                BlockTexture.Render(C, Palette_World, WorldX + Block.X, WorldY + Block.Y);
            }
        }

        foreach((int, int, T_Decal, TextureRotation) Track in __Tracks){
            Texture DecalTexture = Track.Item3 switch{
                T_Decal.Track => Texture_Track,
                T_Decal.Blood => Texture_Blood,
                T_Decal.Zero  => Texture_Zero,
                T_Decal.One   => Texture_One
            };
            DecalTexture.Render(C, Palette_World, WorldX + Track.Item1, WorldY + Track.Item2, false, false, Track.Item4);
        }
        
        foreach(Entity Entity in __Entity){
            if(Entity.ID is T_Entity.Chair or T_Entity.Table or T_Entity.Spikes or T_Entity.Tree or T_Entity.Item){
                Texture EntityTexture = Entity.ID switch{
                    T_Entity.Chair  => Texture_Chair,
                    T_Entity.Table  => Texture_Table,
                    T_Entity.Spikes => Texture_Spikes,
                    T_Entity.Tree   => Texture_Tree,
                    T_Entity.Item   => ItemTexture((T_Item)Entity.Info)
                };

                EntityTexture.Render(C, Palette_World, WorldX + Entity.X, WorldY + Entity.Y, false, false, Entity.Rotation);
            }
        }
        
        Texture PlayerBody  = Texture_Player_Body;
        Texture PlayerEyes  = Texture_Player_Eyes;
        Texture PlayerNose  = Texture_Player_Nose;
        Texture PlayerMouth = (Dead ? Texture_Player_Mouth : Emotion_Happiness < 25 ? Texture_Player_Mouth_Sad : (Emotion_Happiness > 75 ? Texture_Player_Mouth_Happy : Texture_Player_Mouth));
        BlinkTimer += (float)TD.DeltaTimeS;

        if(BlinkTimer > 3 || Dead){
            PlayerEyes = Texture_Player_Eyes_Blink;
            if(BlinkTimer > 3.25f){
                BlinkTimer = 0;
            }
        }
        
        if(MovingDirection.X != 0){
            PlayerFlipped = MovingDirection.X > 0;
        }
        
        T_Item Item = Inventory[SelectedItem];
        if(Item != T_Item.Empty){
            ItemTexture(Item).Render(C, Palette_World, PlayerX, PlayerY - 11, PlayerFlipped);
        }

        ColorB PlayerColor = ColorB.Lerp(ColorB.White, ColorB.DarkRed, WL.Math.Clamp01((Rotten - 2) / 50));
        PlayerBody .Render(C, Palette_World, PlayerX, PlayerY, PlayerFlipped, MultiplyColor: PlayerColor);
        PlayerNose .Render(C, Palette_World, PlayerX, PlayerY, PlayerFlipped, MultiplyColor: PlayerColor);
        PlayerMouth.Render(C, Palette_World, PlayerX, PlayerY, PlayerFlipped, MultiplyColor: PlayerColor);
        PlayerEyes .Render(C, Palette_World, PlayerX, PlayerY, PlayerFlipped, MultiplyColor: PlayerColor);

        if(Health < HealthSmall * 2){
            Texture PlayerBlood = Health < HealthSmall ? Texture_Player_Blood_Strong : Texture_Player_Blood;
            PlayerBlood.Render(C, Palette_World, PlayerX, PlayerY, PlayerFlipped, MultiplyColor: ColorB.Lerp(ColorB.White, ColorB.DarkGreen, WL.Math.Clamp01((Rotten - 2) / 50)));
        }
        
        if(LastHealed > 0){
            Texture_Player_Healed.Render(C, Palette_World, PlayerX, PlayerY, PlayerFlipped);
        }

        foreach(Block Block in __Blocks){
            if(Block.ID is T_Block.Metal or T_Block.Bricks){
                Texture BlockTexture = Block.ID switch{
                    T_Block.Metal  => Texture_Metal,
                    T_Block.Bricks => Texture_Bricks
                };
                BlockTexture.Render(C, Palette_World, WorldX + Block.X, WorldY + Block.Y);
            }
        }
        
        foreach(Entity Entity in __Entity){
            if(Entity.ID is T_Entity.Mob_Spider or T_Entity.Tree){
                Texture EntityTexture = Entity.ID switch{
                    T_Entity.Mob_Spider => (AnimationTimer > 0.5f ? Texture_Spider_Anim : Texture_Spider),
                    T_Entity.Tree       => Texture_Tree_Leaves
                };

                int OffsetX = 0;
                int OffsetY = 0;

                if(Entity.ID == T_Entity.Mob_Spider){
                    OffsetX = 8;
                    OffsetY = 8;
                }else if(Entity.ID == T_Entity.Tree){
                    OffsetX = 8 + (int)(WL.Math.Sin((float)TD.DeltaTick * 2 + Entity.X * 432) * 2);
                    OffsetY = 24 + (int)(WL.Math.Sin((float)TD.DeltaTick * 3 + Entity.Y * 12) * 2);;
                }
                EntityTexture.Render(C, Palette_World, WorldX + Entity.X - OffsetX, WorldY + Entity.Y - OffsetY, false, false, Entity.Rotation);
            }
        }
        
        if(RenderColliders){ Game.RenderColliders(C); }

        #region UI

            float HealthPulse = Dead ? 0 : WL.Math.DSin((float)TD.DeltaTick / WL.Math.Sqr((float)Health / HealthMax));
            ColorB FrameColor = new ColorB((byte)(HealthPulse * 255), 0, 0);

            uint Thickness = (uint)WL.Math.Min(1 + HealthPulse / WL.Math.Sqr((float)Health / HealthMax), 16);
            
            C.Border(0, 0, C.Width, C.Height, 1, FrameColor);
            C.Border(1, 1, C.Width - 2, C.Height - 2, Thickness, FrameColor.Clone().SetA(128), ImageBlend.Alpha);
            C.Border(1 + (int)Thickness, 1 + (int)Thickness, C.Width - (1 + Thickness) * 2, C.Height - (1 + Thickness) * 2, Thickness, FrameColor.Clone().SetA(64), ImageBlend.Alpha);
            
            C.Fill(20 - 1, (int)C.Height - 16 - 1, HealthMax + 2, 8 + 2, ColorB.DarkRed);
            C.Fill(20, (int)C.Height - 16, HealthMax, 8, ColorB.Black);
            C.Fill(20, (int)C.Height - 16, Health, 8, ColorB.Red);
            C.Fill(20, (int)C.Height - 16 + 3, Health, 8 - 6, ColorB.LightRed);

            Font.Render(C, Palette_World, Health.ToString(), 20, (int)C.Height - 16);
            
            Texture_Health.Render(C, Palette_World, 3, (int)C.Height - 21);

            switch(Interface){
                case T_Interface.Inventory:{
                    C.Fill(ColorB.Black.SetA(128), ImageBlend.Alpha);
                    C.Fill(10, 20, C.Width - 20, C.Height - 40);
                    C.Border(10, 20, C.Width - 20, C.Height - 40, 1, ColorB.Black);
                    
                    RenderSlot(C, 0, 0, 0);
                    RenderSlot(C, 1, 1, 0);
                    RenderSlot(C, 2, 2, 0);
                    RenderSlot(C, 3, 3, 0);
                    RenderSlot(C, 4, 4, 0);
                    RenderSlot(C, 5, 5, 0);
                    
                    RenderSlot(C, 6, 0, 1);
                    RenderSlot(C, 7, 1, 1);
                    RenderSlot(C, 8, 2, 1);
                    RenderSlot(C, 9, 3, 1);
                    RenderSlot(C, 10, 4, 1);
                    RenderSlot(C, 11, 5, 1);

                    C.Fill(20, 110, C.Width - 40, C.Height - 140, ColorB.Gray);
                    C.Border(20, 110, C.Width - 40, C.Height - 140, 1, ColorB.Black);
                    
                    if(Item != T_Item.Empty){
                        string Name = Item switch{
                            T_Item.FirstAidKit => "АПТЕЧКА"
                        };
                        
                        string Description = Item switch{
                            T_Item.FirstAidKit => "ЛЕЧИТ БЕДНЫЙ КУБИК ГУЛУ (+ 50)"
                        };
                        
                        Font.Render(C, Palette_World, "[" + (byte)Item + "] " + Name, 20 + 2, 110 + 2);
                        
                        C.Fill(20, 110 + 11, C.Width - 40, 1, ColorB.Black);
                        
                        Font.Render(C, Palette_World, Description, 20 + 2, 110 + 2 + 11);
                    }
                    break;
                }
            }
            
        #endregion
    }

    private void RenderSlot(Image.ImageContext C, byte ID, int X, int Y){
        int X__ = 20 + X * 36;
        int Y__ = 30 + Y * 36;
        C.Fill(X__, Y__, 34, 34, ColorB.Gray);
        C.Fill(X__ + 4, Y__ + 4, 34 - 4 * 2, 34 - 4 * 2, ColorB.Black.SetA(64), ImageBlend.Alpha);
        C.Fill(X__ + 8, Y__ + 8, 34 - 8 * 2, 34 - 8 * 2, ColorB.Black.SetA(64), ImageBlend.Alpha);
        C.Fill(X__ + 12, Y__ + 12, 34 - 12 * 2, 34 - 12 * 2, ColorB.Black.SetA(64), ImageBlend.Alpha);
        C.Border(X__, Y__, 34, 34, 1, SelectedItem == ID ? ColorB.Red : ColorB.Black);

        if(SelectedItem == ID){
            C.Border(X__ - 1, Y__ - 1, 34 + 2, 34 + 2, 1, ColorB.Red.SetA(128), ImageBlend.Alpha);
        }

        T_Item Item = Inventory[ID];
        
        if(Item != 0){
            Texture ItemTexture = Item switch{
                T_Item.FirstAidKit => Texture_FirstAidKit_Icon
            };
            
            ItemTexture.Render(C, Palette_World, X__, Y__);
        }
    }

    private Texture ItemTexture(T_Item Item){
        if(Item == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить текстуру!"); }

        return Item switch{
            T_Item.FirstAidKit => Texture_FirstAidKit
        };
    }

    public override ColorB BackgroundColor(){
        return ColorB.White;
    }

    private void StartLevel(byte Level){
        ClearAllEntityScene();
        ClearAllScene();

        if(Level == 1){
            
            AddScene(@"#################################################################################################################################################################################################################################################################################################################################################################################################################
__#_______#___#_____#_______#_________________#___________#_________#_________#_____#_______________#_____#_______#___________#_________#_#___________#_____#_______#_____#_______________#_______#_________#_____#_____#_____________________#_________#___#_______#_______________#_#_____#___#_________#_____#_________#___________#_#_________#___#_______#_____#_#___#_____#_________________#_____#_____#_#
#_#_#_###_#_#_#_###_#_#######_#######_#######_###_###_###_#_#######_#_#######_#####_#_#########_###_#_#_#_#####_#_#_#######_#_#_#####_#_#_#_#####_#####_###_#_#_###_#_###_#_#####_#######_###_#_###_###_###_#_###_#_#_#_#_###########_#######_#_#######_#_#_#_#####_#####_#########_#_#_###_#_#_#_#_#######_###_#_#######_#_#####_###_#_#_#####_#_#_#_#_#####_#_###_#_#_#_#_###_#_#####_#_#######_#_###_#_###_#_#
#_#_#_#_#___#_#_#_____#___#___#_____#_____#_#___#_#___#_#_#_#_____#_#_#_______#___#___#_#___#___#___#_#_#_____#_#___#_#_____#_#_____#_#_#___#_____#_____#___#_#_#_#___#_#_#___#_#___#_____#___#_#_____#_#_#___#___#_#_#_#___#___#_____#_#___#___#___#_#_#_#___#___#_______#_____#___#_#___#___#___#___#___#_#_#_#_#___#___#___#_#___#___#_#___#_#_#_#___#_____#_#___#_#_#___#_#___#_____#_#___#_____#_#_#_#___#_#
#_#_#_#_#####_#_#######_#_#_#####_#_#####_#_###_#_#_###_#_#_#_###_#_#_#_#######_#_#_###_#_#_#_###_###_#_#####_#_#####_#_#####_#####_#_#_#####_###_#_#####_###_#_#_#####_#_###_#_###_#_#####_#####_#####_#_#####_#####_#_#####_#_#_#####_#_#_#####_#_#_#_#_#####_#_#########_#_###_###_###_###########_#_#_#_#_#_#_#_#_#_#_###_#_###_###_#_#_#_#_#_#_#####_#####_#_###_#_#####_#####_#######_#_#######_#_#_#_###_#
#___#_#_____#_#_________#_________#_____#_#___#_#_#_#_____#___#_#_#___#_#_______#_#_____#_#___#___#_#_#_#_#___#_______#_#___#_#_____#_#_____#___#_#_____#_____#_#_____#___#_______#_#_______#___#___#___#_____#_#_____#_#_____#___#___#___#___#___#___#_#_______#___#_____#_#_#___#_________#_________#_#___#_#___#_#_#_#___#_#___#_#___#___#_#_#___#_______#___#_____#___#_______#_________#_#_____#_____#_____#
#_###_#_#####_#########_#######_#########_#_###_#_#_#_#####_###_#_#####_#_#_#####_###_###_#####_###_#_#_#_#_###########_###_#_#_#####_#####_###_#######_#######_#_#####_###_#######_#######_#_#_###_#_###_###_#_#_#####_#_#########_###_###_###_#_#####_#_#########_#_###_###_#_###########_#_#####_###_#####_#######_#_###_#_#_#_#_#_#######_#_###########_#_###########_#_#####_###########_#_###_###########_#
#___#_#___#___#_______#_#_____#_#_______#_#_____#_#_#_#___#_____#___#_#_#_#_____#_#___#___#_______#___#_#_#___________#___#_#_#_____#___#_______#_____#_#_____#_#_______#_#_#_____#___#_____#_#_#___#_#_#_#_#___#___#_#_#_#_________#___#___#___#___#___#___#_____#_____#___#_#_#_________#_#_#_____#___#_#_____#_____#___#_#___#_#_#_#_____#_#___#_____#_#_#___#_________#___#_#___________#_#_#_#_____#___#___#
#####_#_#_#_###_#####_###_###_###_#####_#_#_#####_#_#_#_#_#_#######_#_#_#_#####_#_#####_#_#######_#####_#_###########_###_#_#_#####_###_#_#######_#_###_#_###_#_#######_#_#_#_###_###_#######_#_#_###_#_#_#_#######_#_#_#_###_#####_#_###_###_#####_#_#####_#_#_###########_#_#_###_#####_###_#_#####_###_#_###_#_#_#####_#_#####_#_###_#_#_#_###_#_###_#_#_###_#_###########_#_###########_#_#_#_#####_#_#_#####
#_____#_#___#_____#_____#_#_#___#_#___#_#_#_#_____#_#_#_#___#_______#_#_#_#_____#_______#_#___#_________#___#_____________#_#_#_____#___#___#_____#___#_#___#_#_______#___#_#_#_____#_________#___#_#_#___#___#_____#_#_#___#_#_____#___#_____#_____#___#___#_#_____#_____#_#_______#___#_#___#_____#_#_______#___#_____#_#_#___#_#_____#_#_#_#_#_#___#_#_______#_#_________#_____#_______#_#___#_____#_#_#_____#
#_#####_#####_###_#####_#_#_###_#_###_#_#_#_###_###_#_#_#####_#######_#_###_#########_#####_#_###########_###_#############_#_#_#####_#######_#######_#_#_#_#_#######_#####_#_#####_#####_#########_#_###_#_#_#_#####_#_###_#_#########_#######_###_###_#_#######_#_#_###_#_###_#####_#_#_#_#######_#_#######_#########_#_###_#_#_#######_#_#_#_#_###_#_#######_#_#_#######_#####_###_#_###_#####_#_#_#_#_#####_#
#_#_____#___#___#_____#___#___#_#___#_#___#___#_#_#_#_#___#___#_______#___#___#___#___#___#_#_______#___#_____#___________#_#_#_____#___#_____#_____#___#_#_#_#_____#_______#___#_#_____#___________#___#___#___#___#___#___#_#_________#_____#_#___#___#_#_______#_#_#___#___#_#___#_#___#_#___#___#_______#_#_______#_#_____#___#_____#_#_#___#___#_#_______#_#_#___#___#_______#___#_#_____#___#_#_#_______#_#
#_###_###_#_###_#####_#####_#_#_###_#_#######_#_#_#_#_#####_#####_###_###_###_#_#_#####_#_#_#######_###_#_#####_#########_#_#_#####_###_#_#######_#_#######_#_#_###_###########_#_#####_#########_#####_###########_#_###_###_#_#########_###_#_#####_###_###_###_###_#_###_#_###_#_###_###_#_#_###########_#_###_#_###_###############_#_#_###_#_###_#######_#_#_###_###_#########_###_#_###_#####_###########_#
#___#_____#_#___#___#_____#_#_#_____#_______#_#___#_#_______#___#_#_____#___#_#_#_______#_#_#_____#___#___#___#_____#_#___#_#___#_#___#___#_#_____#_________#_#_#___#_____#_____#_____#___#___#___#_____#_____#_____#___#___#_#_#_____#___#___#_____#___#_____#___#___#___#_#_____#___#_#_____#_#___________#_____#_#___#___________#___#_#___#_#_____#___#___#_#___#_______#_#_____#_#_#_#_________#_________#_#
#_#_#######_###_###_#####_#_###########_###_#_#####_#####_###_#_#_#_#######_#_#_#########_#_#_###_###_#####_#_#####_#_#_###_###_#_###_#####_#_#_#############_#_#_###_#_#_#_###_#####_###_#_#_#####_#####_###_#_#_#####_###_#_#_#_#####_###_#_#####_###_###########_#####_###########_#_#######_#_###################_#_#######_###_#_###_#####_#########_#_#######_#######_#_#_###_#_#_#_#########_#_#######_#_#
#_#_#_____#___#_______#___#___________#_#___#_#___#_____#_#___#_#_#___________#_#_______#___#_#_____#_______#_________#_#_#_____#_#___#_______#_#_____________#_#_____#_#_#_#___#___#_______#_____#_#_______#_#_#_#___#_____#_#_#_______#___#_#_______#_______#_____#___#___#_________#_______#_#_#___#___#___________#_________#___#___#___#___#___#_____#_________#_____#_#___#___#___#_________#_#___#_____#_#
###_#_#_#####_###_#####_###_#######_#_#_#_###_#_#_#_###_###_###_#_#############_#_###_#######_#################_#######_#_#_#####_#_#####_#####_#_#############_#######_###_#####_#_#############_#_#_#######_#_#_#_#_#######_#_#_#######_###_#_#####_#######_#_#####_#_###_#_###############_#_#_#_#_#_###_#_###################_#####_###_#_###_#_#_###_###############_#_###_#_###############_#####_#_#####_#
#___#_#___#___#___#_____#___#_____#_#___#_#___#_#_#_#___#_____#_#___#___#_______#___#_#_________________#_#___#_____#___#_#_#_____#_#___#_#___#_#_________#___#___#___#_#___#___#_#_#___#_________#_#_#_______#_#_#_#_____#_____#_____#_#___#_#_#_____#_____#___#_#___#___#___#___________#_#_#_____#_#_____#_#___#_____________#_#_____#_#___#___#___#_#_____#_____#___#_#___#_#_________#_#___#_#_____#_#_____#
#_###_###_#_###_###_#####_###_#####_#_#####_###_#_###_#_#_#####_###_###_#_#######_###_#_###########_###_#_#_#_#####_#_###_#_#_###_#_#_#_###_#_#_#########_#_#_###_#_#_#_#_###_#_#_#_#_#_#_#########_#_#_#######_#_#_###_#_#_#########_#_###_#_#_###_###_###_#####_#_#####_#####_#########_#_#_#######_#####_#_#_#_#_#_###########_#_#_#_#_#####_#######_#####_#_#_#_#_#_#_###_###_#####_#_#_#_#_#_#_#####_###_#_#
#_#_#___#___#_#_#_#_#___#_____#___#_#_#_____#___#___#_#_#___#___#_#___#_#___#_____#___#_#_______#___#_#_#_#_#_____#_#_#_____#_#___#___#_____#_#_#_______#___#___#_#_#___#_____#_#_#___#_#_#_________#_#___#___#_#_#_#___#_#_#___#___#_____#_#_#___#_____#___#_________#_______#_#___#___#_#_#_______#_____#_#_#_#___#_#___#_______#_#_#_______#_____#___#_____#_#_#___#___#_#___#_#___#_#___#_#_#_#_#_______#_#_#
#_#_###_#####_#_#_#_###_#_#####_#_#_###_#####_#####_#_#####_#_###_###_#_###_#_#####_###_#_#####_#_###_#_#_#_#####_###_#_#######_#############_#_#######_#######_###_###_#######_#_#####_#_#_#_#######_###_#_#_###_#_#_#####_#_#_#_#####_###_#_###_#######_###########_#_#####_#_#_###_#_#_#_#######_#####_###_#######_#_#_#_#######_#_#######_#_###_#_###_#######_#######_#_###_#_#_#_#_###_#_#_#_#_#######_###_#
#_#___#_#_________#___#___#___#_#_#_#___#_____#___#_#_______#_#_#_______#___#_____#_____#___#___#_#___#_#_#_____#_____#___#_____#_______#___#_#_#_______#_____#_____#_#_#_____#___#_#___#_#_#_#_______#_#_#_#___#___#_________#_#_______#___#___#_#___#_#___________#_#_#___#___#_____#_#_______#_#_#_________#_____#_#_#___#___#___#_#___#___#_#_#_#_#___#_____#_#_____#_#___#_#___#_#_#___#_#___#_#___#___#___#
#_#_###_#############_#####_#_#_#_#_#_###_###_#_###_#######_#_#_#_#######_#######_#########_###_#_#_###_#_#####_#########_#_#_###_###_#_###_#_#_#_#####_#_###_#######_#_#_###_###_#_#_###_#_#_#_#######_#_#_###_###############_#_#######_#####_#_#_#_#_###########_#_#_#_#_###########_#######_#_#_###_#######_###_#_#_#######_#_###_#_#_#####_#_#_#_#_#####_#_#_#_#_###_###_#_#######_#_###_###_#_#_#_#_###_#_#
#_#_________________#_#_____#___#_#_#_#_____#_#___________#_#_#___#_#_____#_______________#___#___#_______#___#_____#___#_#_#_____#___#_#___#___#_#___#_#___#_____#_____#___#___#___#_#___#_#___________#___#_#___________#___#_#_#_____#_____#_#_#_#_#___________#_#_#_#_#_________#_____#_#___#_#___#_#_______#_#_#_#_#_______#___#___#_________#_#___#_____#___#_#___#_____#_#_______#___#___#_#___#_#_____#_#
#_#################_#_#_#########_#_#_#####_#_###########_###_#_###_#_#####_#################_#####_#######_#_#_###_#_#_#_###_#####_###_#_#_#####_###_#_#########_#_#######_###_###_#_#_###############_#####_###########_#_###_#_###_#_#####_#_#_#_#_###########_#_#_#_#######_###_#_###_#_#_###_###_#_#_#######_#_#_#_#_#########_###############_#_#####_#######_###_#####_#_#_#########_###_#_#####_#########
#_____#_____#___#_#_#_#_____#___#_#_#_____#_#_#_______#_______#_#___#_#_____#_#_____________#_#___#_#_#_____#_#_#___#_#_#___#___#___#_#___#_#_____#___#___________#___#___#___#___#_#_#_________#___#_#___#_________#___#_#_____#_____#_#_____#_#___#_____#_____#_#_#_#_#_______#_#_#_#___#_#___#___#_#_#_#___#_____#_#_#_________#_#___#___________#_______#_____#_#_#___#___#___#_______#_#___#_#_#___#_______#
#_###_#_###_#_#_#_#_#_#####_###_#_#_#####_###_#_#####_#_#######_###_#_###_###_#_###########_#_#_#_#_#_#_#####_###_###_#####_#_###_###_#####_#_#####_###########_#####_#_#_###_###_###_#########_#_#_#_###_#####_#_###_#_#_#_###########_#_#####_#########_#_###_#_#_###_#_#######_#_#_###_#_###_#_#_#_###_#_###_#####_#_#_#####_#_#_###_#_#_#################_#####_#_###_#_#########_#####_#_###_#_#_###_#####_#
#___#___#___#_#___#_#_________#_#_#___________#_#___#_#_#___#_______#___#_#___#___________#___#_#_____#_____#_____#_______#_#_#___#_______#_#_____#_____#_#_____#_____#_#___#_#_______#_____#___#_#_#___________#_#___#_#_#___#_____#___#_____#___#_#___#_____#_#_#___#_#_#___#_____#___#_____#_#_#_#_#___#___#_#_______#_____#_#_#_____#_#_#_________#_______#___#_#_#___#___#_______#___#_#___#___#___#_#___#_#
###_#####_###_#####_#_#######_#_#_###_#########_###_#_###_#_#_#########_#_#_#_###########_#######_#########_#############_#_###_#_#####_#_#_#_###_#_#_#_#_#_#####_#########_#_#######_#_###_#_###_#_#######_#######_###_#_###_#_###_#########_###_#_#_#_#######_#_###_#_#_###_#_#######_#######_#_#_#_#_#####_#_#######_#######_###_#####_#_#_#######_###_#_###_#_#_#_#_#####_###_###_#_#_#_###_###_###_#_#_#_#_#
#_______#_#_____#___#___#___#___#___#_#_______#_____#_____#_#_#_____#___#_#_#___________#_______#_#_______#_____________#___#___#_#___#_#_#_#___#_#_#_#___#_#___#___#_____#_#_______#_#___#_#_#___#_____#___#_____#_#_#___#_#_#_#_#___#_____#_#___#_#_#_____#___#___#___#___#_#_#_#_____#_____#_#_#_#_#_____#_#_______#_#_____#_____#_____#_#_#___#___#___#_#___#___#_#___________#_#_#_#_#_#___#___#_#_____#_#_#
#_#######_#_###_#_#######_#_###_###_#_#_#####_#####_#######_###_###_#_###_#############_#######_###_#####_#######_#####_###_#_#####_#_###_#_#####_###_#####_#_#_###_#_###_#_#######_#####_#_#_#_#######_#_###_###_#_#_#####_#_#_#_###_#_###_#_#_###_#_#_###_#_###_#######_#_#_#_#_#_#_###_###_#_#_#_#_#####_#_#######_#_#_###_#####_#_#######_#_###_###_#####_#######_#############_#_#_#_#_#_#####_#_#########_#
#_#_______#___#_#_#_______#___#_#___#_#_____#_____#___#___#_#___#_#___#_#_______#_____#_______#___#_#___#_#_____#_#___#___#_#_______#___#_#_#___#_#___#_____#_#___#___#___#_______#_____#_#_#___#___#_#_#_#___#_#___#___#_____#_#_#___#_#___#_#___#___#_#_#_#_#_#_______#_#_#___#___#___#_#_#___#_#_#_#___#_#_#_____#_#___#_#_#___#_#_______#_#_#___#___#___________#___#___#___#_#___#_#___#_____#_______#___#_#
#_#_###########_#_#_#########_#_#_#########_#_###_###_#_###_#_###_#####_#######_#_###_#######_###_#_#_#_#_#_###_#_#_#####_#_#_#########_#_#_#_#_#_#_#_#_#####_#_#_#####_#######_#######_#_#_#_###_#_#_#_###_###_#####_###_#####_#_#_###_###_#_###_###_#_#_#_#_#_#_#####_#_#_#_#########_#_#_#######_#_#_#_#_#_#_###_#_#####_#_#_#_#########_#_#_#_###_###_#########_###_#_#_#_#_#_#_###_#########_#####_###_#_#_#
#_#_#_________#_#_#_#_______#_#_#___________#_#___#___#___#___#_____________#___#_#_#___#_______#___#_#___#_#___#___#_____#_#_________#_#_#___#___#_#_#_#_____#_#_#___#_______#_______#___#_#_#___#_#_#_#___#_________#___#_____#_#_#_____#___#_#___#_#_#___#_#_#_____#___#_#_#_______#_#_#_____#___#___#___#___#___#_#_____#___#_#___#___#_#_#_#___#_____#_______#_____#_#_#_#_#___#_______#_____#_____#___#___#
###_#_###_###_#_#_#_#_#####_#_#_#############_#_###_###_#_#####_#########_#_#_###_#_###_#_#########_#_###_#_###_###_#_#####_#########_#_#_#########_###_#_#####_#_#_#########_###_###_#####_###_###_#_#_#_#########_###_###_#####_#_#_###_#####_###_###_#_###_#_#####_#####_#_###_###_#_#_#_#_#_#_###########_###_###_#_###_#####_#_#_#_#_#_#_#_###_#_#####_#####_#####_#_#_#_#_#####_#######_#_#########_#####_#
#___#_#___#_#_#_#___#___#_#___#___#_____#_____#___#___#_#_____#_#_______#_#_#_____#___#___#_______#_#___#_#___#___#_#___#_______#_#___#_#_______#___#___#_#_____#___#_______#___#_#_______#_____#___#___#_________#_____#___#_____#_#___#___#_____#_____#___#_#_____#___#___#_#___#_____#_#_#_#___#_________#___#_____#_#___#___#_#_#_#_#_#_____#___#_#_#___#_#___#_____#_#_#_#_______#_______#_#_________#___#_#
#_###_#_###_#_#_#######_#_#########_###_#_###########_###_#_#_#_#_#####_#_#_#########_###_#_#####_#####_#####_###_#_###_#######_#_#_###_#_#_###_#_#_#_#####_#########_###_#####_###_###_#########_###_###########_#_#####_###_#_###_#######_#_#_#######_#####_#####_###_#_###_#_#########_#_#_#####_###_#_#####_#######_#_#_#_###_#_#_#_#_#######_###_#_#_###_#_#_#######_#_#_#########_#######_#_#########_#_#_#
#_#___#_#_____________#_#___#_______#___#_#_________#___#_#_#_#_#_#___#_#_#___________#___#___#_#_____#_#_____#___#___#_____#_#_#___#___#_#_#_____#_#_______#___#___#_#_#_____#___#___#_#_______#_#_____#_________#_______#___#_#___#_______#_#_____#___#___#_____#___#_#_#_____#_____#___#_#_______#___#_#_____#_____#_#_#___#___#_#___#___#___#_#___#_#_#___#_#_#_______#_____#_#_____#_______#_____#___#_#___#
#_#####_###_###########_#_#_#_#######_###_#_#####_#####_#_#_#_###_#_###_#_###########_#_#####_#_#####_#_#_#####_#####_#####_#_#_#####_#####_#_###########_###_#_#_#_#_#_###_#####_###_###_###_#_#_#####_#_#######_#########_###_#_###_#######_###_###_###_#_#####_#_#_#_#_#_#####_###_#_###_#########_###_#_#####_###_#_#_###_#_###_#######_#_#_#_###_#_#_#_###_###_###########_#_#_###########_#####_#_#_#_#####
#_#___#___#_#_#_________#_#___#_____#_#___#_____#_______#_#_#_____#___#_#_#_________#_#_#_#___#_________#___#___#___#_____#_#_#_______#_____#_#_________#_#___#___#_#_#___#_____#___#_____#___#_#_____#_#_____#___#_____#___#_#_#_#___#_______#_#_____#___#_______#_#_#_#_#___#___#_#_#_#_#_____#___#_#_#_#_#___#_#_#_#_#___#_#_#___#___#___#_#_#___#___#___#___#___#_____#___#___#_#_________#_#_____#_#_#_#___#
#_#_#_###_#_#_#_#########_#####_#_#_#_#_#_#####_###########_#########_#_#_#_#######_###_#_#_#_#############_#_###_#_#_#####_#_#########_#######_#####_###_#_#######_#_#_#_#####_###_#_#####_###_#####_#_#####_#####_###_###_#_#_#_#_###_#######_#######_#######_#####_#_#_#####_###_#_#_#_#####_#_###_#_#_#_#_#_#_#_#_#####_#_#_#_###_###_###_#_###_###_###_#_###_#####_###_#_#####_#_#######_#_#_#######_#_#_#_#
#___#___#_#___#___________#_____#_#_#___#_____#_#_________#___#_______#_#___#_____#_____#_#_#_#_____#___#___#___#_#___#_____#_____#_________________#_____#_______#_#_#_#___#_#___#_#_#___#___#_#___#_#_______#_____#_#_#___#___#___#___#_____#_#_____#_____#___#___#___#___#___#___#_#_#_____#___#___#___#___#___#___#_____#_#___#___#___#___#_____#_____#_#_____#_____#___#___#___#_#_#___#_#_#_#_____#___#_#_#
#_#####_#_#################_#####_#####_#######_#_#######_#_#_#_###_###_#####_###_#_#####_#_#_#_###_#_#_#_#####_#####_#_#######_#_#_###############_#############_#_#_###_#_#_#_###_#_###_###_#_#_###_#######_#_#####_#_#_#####_#####_###_###_#_#_###_#####_#####_#_#_#####_#_###_#_#_#_#_#_#####_#_###############_###_#####_#######_#_###_#_#######_#####_#######_###_#_#####_#_#_#_#_#_#_#_###_#_###_#_#####_#
#_#_#___#___#_____#_____#_______#_____#_#___#___#___#_____#_#_#___#___#_#_____#___#_______#_#_#___#_#_#___#___#_____#_#_______#_#_#___#_______#___#___#_________#_#_#___#_#___#_____#_______#_#_#___#_#_____#_#_#_____#_#_____#_____#_#___#_____#_#_#_#___#_#_____#_#___#___#_#___#_#___#_#_____#_#_________#___#___#___#_____________#___#_#_____#___#_____#_____#_#___#_#_#___#_#_#___#_#___#___#_#___#_______#
#_#_#_#####_#_###_#_#_###_#########_###_#_#_#_###_#_###_#####_###_###_#_#_#####_#########_#_#####_#_#_#######_#####_#_#######_#_#_###_###_#_#_#_#_###_#_#_#####_#_#_###_#_###_#############_#_#_###_#_#_###_#_#_#_###_#_###_#_#_#####_#_#_#####_#_#_#_#_#_#_#_#####_#####_###_#_#######_#####_###_#######_#_#_###_###_#_#########_#######_#######_#_###_#####_#_#_#_#####_#_#_###_#_###_#_#####_###_#_#####_#####
#___#_____#___#___#_#_______#_____#_____#_#_#_#___#___#_#_____#_____#_#___#_#___#_________#_____#_#_#_____#_______#_#_______#_#_#___#___#_#_#_#_#_#_#_#_#_____#_#_#_____#___#_#_________#___#_#_____#___#_#_#_#_#___#_#___#_#_#_#___#_#_#_#___#_#___#_#_#_#___#___#_____#_______#_____#_____#_____#___#___#_#_#___#___#_____#_____#_____#_#_____#_#___#_#___#_#_#___#_____#_#___#_#___#_#_____#___#_#___#___#___#
###_#####_#_###_###_#########_###_#_#####_#_#_#####_#_#_###_#######_#_#####_#_###_###########_#_#_#_#####_#_#####_#_#_#######_#_###_###_#_#_#_#_#_#_#_#####_#_###_#########_#_###_#######_###_###_#######_#_###_###_#_###_#_#_###_#_#_#_#_#_#_#####_#_#_#_#####_#######_#######_#_###_#####_#_#####_#_#_###_#_#_###_#######_#_#####_#_###_#_###_#_#_###_#_###_#_###_#_#####_###_#_#####_#####_###_#_###_#_###_#_#
#___#_____#_#_#_#_________#_____#_#___#___#_#_#___#_#_#_____#___#___#_______#___#_#_#_______#_#_#_#_#_____#_____#_#_#_#_______#___#_#___#_#_#_#_#_#_#_____#_#___#_#_________#___#_____#___#_#___#_#_______#_#___#___#_#___#_#_#___#___#_#___#_______#_#_#_#_____#_______#___#___#___#_#___#_#_#_____#_#_#___#_#_#_#_#_____#_#_#___#_#_____#_#_#___#_#___#_#___#_#___#_#_____#___#_#___#___#_#___#___#_#_#_#___#_#
#_###_#####_#_#_#_#########_#####_###_#_###_#_#_#_###_#_#####_#_#_#######_#####_#_#_#_#####_#_###_#_#_#####_###_#_#_###_#####_#####_#_#####_#_#_#_#_#####_#####_#_#_#_#########_#####_#_###_###_###_#_#####_#_###_#####_###_#_#_###################_#_#_#_#_#_#_#_#######_#_#####_###_#_###_###_#####_#_###_#_#_#_#_#_###_#_###_#_#_#######_#_#####_#_###_#_###_#_###_#####_#_###_#_#_###_#_###_#####_#_###_###_#
#___#_____#___#___#_____#___#_#___#___#_#___#___#_#___#_#_____#_#_#_____#___#___#___#_#_____#_#___#_#_____#_#_#_#_#_____#___#_#___#___#_____#_#_#_#_____________#_#_#_#_______#_#___#___#_____#_#___#___#___#___#_#___#___#_#_#_____#_____________#_#_#_#___#_#_#_____#___#_______#___#_____#___#_____#___#___#_#___#_#___#___#_#___#_______#_______#_#_____#_#_#_#___#___#_______#_#___#___#_________#_#___#_#_#
###_#####_#######_#_###_#_###_#_###_###_#_#######_#_#####_#####_#_#####_###_#_#######_#_#####_#_###_#####_#_#_#_#_#######_###_#_#_###_#_#_#_###_#_###########_###_#_#_#_#####_#_#_#_#_###_#####_#_#####_#_#####_#_#_#_###_###_#####_#_###########_#_#_#_#####_#_#####_#_###########_#####_###_#######_###_###_#_#_###_#_###_#_#_#####_#############_#_#_#####_#_#_#_###_#_#########_#_#####_#_#########_#_###_#_#
#_#_#___#_______#_#_#___#___#_#___#_#___#_______#_#_____#___#___#_____#___#_#_#_____#_#_#_______#___#___#_#___#_#___#_________#_#___#_#_#_#_#___#___________#_#___#_#_#___#_#___#_#_#_#___#_____#_____#_#_#___#___#_#___#___#_______#_#___#___#___#_#_#_#_#___#_____#_#___#_______#_____#_#___#_____#_____#___#_#_#___#_#_#_#___#___#_____#_______#_#_#___#___#_#_#_#___#_____#_____#___#___#_________#_______#_#
#_#_###_#######_###_#_#_###_#_###_#_#_#######_#_#_###_#_###_#_#######_#_###_#_#_###_#_#_###_#####_#_###_#_#####_#####_#########_###_#_###_###_#############_#_#_#####_###_#_#####_#_#_#_###_#####_#####_#_#_#_#_###_###_###_###_#####_#_#_#_#_#_#####_#_#_#_#######_#_###_#######_#####_#_#_#_#_###_#########_#_###_###_#_#_#####_#_#####_#_#####_#_#_###_###_#_###_#_#######_#_#######_#_###########_#########_#
#_#___#_______#_____#_#_#_#_#_#___#_#___#___#_#_#___#_#_#___#_#_____#_#___#___#___#___#___#___#___#_____#_____#_______#_________#_#_#_____#___#_________#_#_#_#___#___#_#_#_____#_#_#_#___#_______#_____#_#_#___#___#_____#___#_______#_#_#_#_#_____#___#_#___#_#___#___#_______#_#___#_#___#_#___#_________#_#_____#_#_#_#_______#_#_____#___#_#_#_#_#_______#___#_#_#___#___#___#___#___#_________#_#_________#
#_###_#####_#_#######_#_#_#_#_#_#######_#_#_#_#_###_###_#_###_#####_#_#_#_#######_#######_###_#_#######_#####_#####_###_#########_#_#######_###_###_###_#_#_#####_#_###_#_#_#_#_#_#_#_###_#########_###_#_#_#####_#_#####_###_#########_#_#_#######_#_###_###_#_#_###_#_#######_#_#_#_#_#_#######_#########_#########_#_#_#########_#_#######_#_#_###_#####_#####_#_#_#_#_#_#_###_#_#_#####_###_#####_#_#######_#
#___#_______#_#___#___#_#_#_#_#_______#_#_#_#_#___#_#___#___#___#___#_#_#_________#_____#_#___#_#___#___#___#_____#___#___#_____#_#_________#_____#_#___#_#_#___#___#___#_#_#_#___#_#_____________#_#___#___#_____#_#___#_____#_________#_#_______#_#_#___#_#_#_#___#_#_______#___#_#___#_#_____#_______#_#___________#_#___#_______#_______#_#_#_#___#___#_#_____#_#___#_#_#_#___#_#_______#_#_#_____#_____#___#
#_###########_#_#_#_###_#_#_#_#####_###_#_#_#_#####_#_#####_###_#_###_#_#######_###_###_#_#####_###_#_###_#_#####_#######_###_#_#_###########_#####_#_###_#_#_#_#######_#_###_#####_#######_#######_#######_#_#######_#_###########_#####_#####_#_#_#_#_#_#_#_#_#_###_#####_#####_#_#####_#_###_#######_#_#######_###_#_###_#_#########_#_###_#_#_#_###_#_###_###_#_#####_#_#_#_###########_#_#_#_###_#####_#_###
#___#_______#_#_#___#_____#_#_____#___#_#_#_#_______#_#_______#_#___#_#___#_____#___#___#_#___#___#_#_#___#_#___#___#___#___#_#_#___________#_#_____#_____#___#_________#___#_#___#_#_#_____#_____#___#___#_#_#___#___#_____#_____#___#_________#_#_#___#___#_#___#___#_#___#___#_#_____#_#___#_____#___#_#_______#___#_#___#_#_______#_#___#_#_#___#___#_____#___#_____#_#_#_#_#_______#___#_#_#___#_#_____#___#
#_#_#_###_#_#_#_###########_#_###_###_#_#_#_#########_#_#######_#_#_#_#####_#####_###_###_#_#_###_#_#_#_#_###_#_###_#_#_###_#_#_###_#####_###_#_#########_#####_###_#######_#_#_###_#_#_#####_###_#_#_#_#_#_#_#_#_#_#######_#_#_#_###_#####_#######_#########_#####_###_#_###_#_#_#####_#####_#_#_###_###_#_###########_#_###_#_#_###_###_#_#_#_#####_#########_#######_#_#_###_#_#####_#_###_#_###_#_#_#######_#
#_#_____#_#_#_#___#_____#___#_#_____#___#_#_________#_#_#___#___#_#_______#_#_____#_#___#___#_____#___#_#_____#___#___#___#_#_#___#_____#_#___#___#_____#___#___#___#_______#_#_____#_____#___#___#_#_#_#_#_#___#_#_#_____#_#_#_#_____#___#_#_____#___#_____#_#_____#_____#___#_#_#___#_______#_#_#___#___#_____________#___#_#_#_#_#___#_#_______#___#_______________#_#_#_____#_#___#___#_#___#_#_#_#___#_____#
#_#######_###_###_#_###_#_###_#_#########_#####_###_#_#_#_#_#_#####_#####_#_#_#####_###_###########_###_#########_#####_#_#_#_###_#_###_###_#####_#_###_###_#_#####_#_#######_#_#####_#####_###_###_#_#_#_#_#####_#_#_###_#_#_#_#####_#_#_###_###_###_#_###_#_#_#_#########_###_#_###_#########_#_#_###_###################_#_###_#_###_#########_#_#_#_#############_#_#_#######_#_#_#####_#_###_#_#####_###_###
#_#_#_____#_____#_#_#_#___#___#_______#___#___#_#_#___#_#_#_#_____#_#___#___#_#_______#___#_____#___#_#_#_______#___#___#_#___#_#_#_#_#___#___#_#_#___#_____#_#___#_#_#_#_____#___#___#_____#___#_#_#_#_#_#_____#___#___#_#_#_#_____#_#_#_____#_#_____#_#___#_#_#_#___#_______#_#___#_______#___#_#_#___________#_____#___#_#___#_#___#_#_____#_#_#_#_#___#_________#_#___#___#___#_#___#___#_#___#_____#___#_#_#
#_#_#_#####_#####_#_#_#####_#######_#_#_###_###_#_#_###_#_#_#####_###_#_#####_#_#####_###_###_###_###_#_#_#####_#_#_#_#########_#_#_#_###_###_#_#_###_#######_#_#_###_#_#_###_###_#_###_#####_###_#_#_#_#_#####_#######_#_#_#_#####_###_#######_#######_#_###_#_###_#_#_#######_###_#_#_###_#_#####_#_#########_#_#_#_#_#_#_###_#_###_#_#_###_#_#_#_#_#####_#######_#####_#_#_#_###_#_#_#_#_#_###_#####_###_#_#_#
#___#_#_____#_____#___#_#___#_____#_#_______#___#_#_#___#_#_____#_____#_#_____#_____#___#___#___#___#___#_#_____#_#_#_________#_#_#_#___#_______#_#_________#_#_#_____#___#___#___#___#_#___#___#_#_#_#_#_____#___#___#_#___#_____#_____#___#_______#___#_____#_____#_______#_____#_#_#_#___#_______#___#_#_____#_#_#___#_____#___#___#_#_#___#_#_#_#___#___#_____#_____#___#___#___#_#___#_#_________#___#_#___#
#####_#_#####_#######_#_#_###_###_#_#####_###_###_#_#_###_#####_#######_#_#########_###_###_#_#_###_#_###_#####_###_#######_#_#_#_#_#_###########_###########_#_#######_###_###_###_###_#_#_###_#_#_#_#_#####_###_#_#_#_#########_#######_#_#_###_#_#_#######_###############_#_###_###_###############_#_#_#####_#_###_###########_###_#_#_###_#_#####_#_###_#########_#########_###_#_#############_###_#_###_#
#_____#_#___#_________#_____#___#_#_#___#_#___#_____#_#_#_#___#_____#_#___#_____#___#___#_#_#_#___#_#___#_____#___________#_#_#_#_#___#_________#_#_________#___#_____#_#_#___#___#_#___#_#___#_#___#_#_#_____#___#_#_#_____#___#_#_______#_____#_#___#_______#___#___________#_#___#___#___#_________#___#_______#___#_#___________#___#_#___#_#_______#___#_________#_#_#_______#___#_#___________#_#_#_#___#_#
#_###_#_#_#################_###_#_###_#_#_#_###_#####_#_#_#_#_#_###_#_#########_#_###_#_#_#_#_#_###_#########_#######_#####_#_#_#_#####_#######_#_#_#######_#####_###_#_#_###_###_#_#_###_#####_#_###_#_#_#####_#####_#####_#_###_#####_#########_#####_#######_#_#_###########_#_###_###_#_#_###_#######_#####_#####_#_#_###_#####_#_###_###_#_###_#######_#_#####_#_#_#_#_#######_###_#_###_#######_#_#_###_###
#_#___#_#_#_______#_____#_#_#_#_#___#_#_#_#_#___#___#_#___#_#___#_#_#___________#_#___#___#_#_#_____________#_______#_#_____#_#_#_#___#_#_____#_#_____#___#_______#_#_#_#___#_#___#_#_#_________#_#_#_#_#_#___#_____#_#___#_#_________#_#_______#_____#_#_____#_#___#___#_______#_#_______#_____#___#_____#___#___#_#_#_#___#_#___#_#___#_#_#_#_____#_______#_____#_#_#_#___#_____#___#_#___#_____#___#_#___#___#
#_#####_#_#_###_#_###_#_#_#_#_#_###_#_#_###_#####_#_#_###_#_#####_#_#_###_#######_#_###_###_#_#############_#######_#_#_#####_#_#_#_#_#_###_#_#_#####_###_#######_#_#_#_###_#_###_###_#_#########_#_#_#_#_###_#####_#_#_#_#_#########_###_#####_#_###_###_###_#_#####_###_#######_#########_#######_#_#####_#_###_#_#_#_###_#_#_#_#####_#_#_#_#######_###########_#_#_#_#####_###_#_#_#####_#####_#_###_###_###_#
#_______#_#___#_#_____#_#_#_#_#_#_#_#_#___#_#___#_#_#___#_#___#___#_#___#_#_____#_#_#_#_#___#_#___#_____#___________#_#_____#___#___#_#___#_#_#_____#___#_______#_#___#_#___#___#_____#_#_______#___#_#_#___#_#_____#___#_#_#_____#___#___#_#___#___#_#___#_____#_____#_________#_#_______#___#___#___#_____#_#_____#_#_#___#___#_________#_________#___________#_#_#_#_______#_#_#_#_______#___#_______#_#___#_#
#_#######_#_###_#######_#_#_#_#_#_#_#_###_#_#_#_#_#_###_#####_###_#_###_#_#_###_#_#_#_#_#_###_###_#_###_#######_#####_#####_#########_###_#_#######_###_#######_#_#####_#_#_###_#######_###_###_###_#_#_###_#_#_#########_#_#_###_#_###_###_#_#######_#_#######_#####_#_#########_#_#####_#####_#_#####_#####_#######_#_#_#############_###########_#_#########_#_#_###########_#_#_#########_#########_#_#_#_#_#
#___#_______#___#_____#_#_____#_#_#___#___#_#_#___#___#_______#___#_#_#_#_#_#___#_#_#___#___#_____#_#_#___#_____#___#___#_#_________#___#_#_______#___#_#_______#_________#_#___#_____#___#_#_#___#_#_#_#___#_____#_____#_#___#___#___#___#_#_______#_#___#___#_#___#_____#_____#_#_#___#_______#_____#_____#___#_____#_#_#_____#_____#_#_____#_#___#_#_____#___#_#_______#_#___#_#___#_______#___#___#___#_#_#_#
###_###_#####_###_#####_#_#####_#_#_#####_#_#_#######_#########_###_#_#_#_#_#_###_#_#######_#######_#_###_###_###_#_###_#_#########_###_#_#_#####_###_#_#_#####_#############_###_###_###_#_#_###_###_#_###_#####_###_#_#_#####_#####_###_#_#######_#_###_#_#_###_#_#_#####_###_#_#_#_#_#############_#_###_###_#_#####_#_#_###_#_###_###_###_#_#_#####_###_#_###_###_###_#_#_#_#_###_#_###_###_#_#_#_#####_#_#_#
#_#___#_#___#_#___#_____#_#___#_#___#___#___#_#_____#___________#___#_#_#___#_____#_______#_______#_#___#___#_#___#___#___#_______#_____#_#_#___#___#_#___#_____#___#___#___#_#_____#_#___#_#___#_#___#___#___#_#_____#_______#___#___#___#___#_____#___#___#_____#_#_#_____#_#___#_#_#___#_______#___#_#___#_#_#___#___#_#___#_#_#_#_____#___#_#_#_____#_#_#_#_#___#___#_#___#_#___#___#___#___#_#_#_______#_#_#
#_###_###_#_#_#_#_#_#######_#_#_#_###_#_#####_#_###_###_###_#####_###_#_#############_#_#_#######_#_#_#_###_#_#_#####_###_###_###_#######_###_#_###_#_###_#####_#_###_#_#_#_#_#_###_###_###_#_###_#_#_###_###_#_#######_#########_#_###_###_#_#_#######_###########_#_#_#####_#####_#_###_#_#####_#_#####_###_#_###_#####_###_###_#_#######_###_#_###_###_#_#_#_###_###_#_#####_###_#####_###_###_#_#########_#_#
#___#_____#_#_#_#_#_#_______#___#___#_#_____#___#_#_#_#___#_#___#_#_____#___#___#___#_#_#_#_____#___#_#_#_#_#_#___#___#_#_#___#_#___#___#_____#___#___#___#___#_#_#___#_#_#___#_#_#___#_____#_____#_#___#_#___#_____#___#_________#_____#___#_____________#_____#___#_#_#_______#___#_#___#_#_____#_____#_#___#___#_#_________#___#_______#___#_______#___#___#___#_#___#___#___#_#_____#___#_#___#___#_#_____#_#
#_#_#######_#_#_###_###_###_#########_#####_#####_#_#_###_###_#_#_#_#####_###_#_#_#_###_#_#####_#####_#_#_#_#####_#_###_#_#_###_###_#_#_#_#######_#_#######_#_#_#_#_###_#_#####_#_###_#_#########_#_###_#_#_#######_#_###_#################_#############_#_###_#_#####_###_###_#_###_#_###_###########_#_#_#####_#_#_#########_###_#####_###_#########_#######_#_#_#_#####_#_#_#_#####_#_###_#_#####_#_#_#####_#
#_#_____#___#_#_________#_#_#_______#___#_________#_____#___#_#_#_#_____#_#___#_#_#_#___#_____#_______#_#_#_______#_#___#_#_#___#___#_#_________#_#___#___#_#_#___#___#___#_____#___#___#_______#___#___#_#___#___#_____#_#_#_____________#___#_________#___#_#_#_#_____#___#_#___#___#_____#_________#_#_#_____#_#_#_#___#_____#___#___#___#_____#_____#_______#_#_#_#_____#_#_____#___#_#___#_#_____#_#_#___#_#
###_###_#_###_#_#########_#_#_#####_###_#_###_#########_###_#_#_#_#####_#_#_###_#_#_#_#######_#_#######_#_#########_###_#_#_#_###_###############_###_#_#_#_#_#_#####_#########_###_#####_#####_#####_###_###_#_#_###_###_#_#_#####_#####_#_###_#######_#####_#_#_#_#####_###_#####_#_#######_###_#####_#_#_###_#_#_#_#_###_#####_###_#_#_#_#####_###_#_#_###_###_#_#_#_#####_#####_#_###_#_###_#_#_###_#_#_###_#
#___#___#_#___#_____#___#___#___#_#___#_#___#_#_______#___#_#_#___#___#_#___#___#_#___#_________#_________#___#___#___#_#___#_#___________________#_#___#_#_#_#_#___#_#_______#___#_#_____#___#_#_____#_#___#___#___#_#___#___#___#___#_#_#_#___#_____#_#_______#_#_#___#___#_______#_#_____#___#___#___#_#___#_____#_#_#___#___#_#___#_#_#_____#___#_#_#___#_#___#_#_#___#_____#_#_#___#_#_#___#_#_#_#___#___#_#
#_#######_#_#####_###_#_#######_#_###_#_###_###_#_###_#_###_#_#####_#_#_#####_###_#####_###########_#######_#_#_#_###_#_#####_#_#########_#########_#####_#_#_###_#_#_#_#####_###_#_#_#####_###_#_#####_###_#######_###_###_#####_###_#_#_###_###_###_#_#######_#_#_###_#_#_#######_#_#_###_###_###_#_###_###_#####_#_#_#_###_#_#_#_###_#_#####_###_###_###_#_#####_#_###_#_###_#_#_###_###_#_#####_#_#_#####_#_#
#_#___#___#_____#_#___#_#_____#_#___#___#_#_____#_#___#_#___#___#___#_________#___#_______#___#___#___#_____#___#_____#_______#_#___#___#_#___#_____#___#_#_#_____#_#___#___#___#___#_#___#___#_#_________#_#_____#_____#_#_#_______#_#_#_#___#_#_#_#_#_______#_#_#___#___#___#_____#_#___#___#_#___#___#_#___#___#_#___#_____#_#_#_#_____#___#___#_#_____#_#_______#_#___#_#___#___#_#_____#_#_____#_#_#_____#_#
#_#_#_#_#####_#_###_###_#_###_#_###_#####_#######_#####_#_#####_###########_###_###########_#_#_#_###_#_###############_#_#####_#_#_###_#_#_#_###_#_#_#_#_#_#######_#####_#_#_#_#####_#_#_#_#_#_###_#####_#_#_#_#########_#_###_###_#_#_#_#_###_#_#_#_#######_###_###_#######_#_#####_###_#_###_#_#####_#_#####_#_#_###_#######_###_#######_###_###_#_#####_#########_#_#####_###_###_#######_###_###_#_#_#_###_#
#___#___#___#_#___#_#_#_____#_#_______#_______#_#_______#_____#_#___#___#_#_#___#_____#_____#___#___#_#_____#_____#_____#_______#_#___#___#_#___#_#___#_____#___#_#_____#_#_#_#_#___#_#_#___#___#___#___#_#_#_#___________#___#___#_#_#___#___#_#___#_____#___#___#_#_________#_#___#_#_#_#___#_#_______#_______#_#_#___#_____#_#___#_____#_#___#___#_____#_#_____#___#_______#___#_______#_#_#___#_#___#_#_#___#
#_#######_#_#####_#_#_#######_#######_#_#####_#_###########_#_#_#_#_#_#_#_#_#_###_###_#_###########_#####_#_#_#####_#############_###_#####_###_#############_#_#_#####_#_#_###_#_#_#_#_#########_###_#_###_#_###_###########_#_#_###_#_#####_#_###_#####_#_###_#_#_###########_#_###_#_#_###_#_#################_#_#_###_#####_#_###_###_#_#_###_###_###_#_#_#_###_###########_###_#####_#_#_#_###_#_###_#_#_#_#
#_____#___#_______#___#___#___#_______#_#___#_#_____#_______#_#___#_#_#_#_#_#___#___#___#_____#_#___#___#_#_#___#___#_______#_#___#_____#___#___#___#_________#_#_____#___#_#___#_#___#_#_____#_____#_#_____#___#_____#_____#_#_#___#_#_______#_____#_#___#_____#_#_________#___#_#___#_#___#_#_#_____________#___#_#_#___#_____#___#_#_#_#___#___#_#___#_____#_#___#_____#_____#___#___#___#_#_#_#_______#_#_#_#
#####_#_#_###########_###_#_###_#######_#_#_#_#_###_#_#######_#####_#_#_#_#_###_###_#####_###_#_#_###_#_###_#_#_#_#######_#_#_#_#######_#_###_###_#_#_#########_#_#########_#_#_#_#####_###_#_#######_#########_#####_#_#_#_#_#_###_#_#########_#####_#_#########_#_###_#####_#_#_#_###_###_#_#_###_#####_###_#_#####_#_###_#####_#_#_#_#_#####_###_###_#########_###_#_#_#_#####_###_#_#_###_#_#_#_#########_#_#
#_____#_#_________________#___#_#_______#_#_#___#___#_____#_______#_#_#_#_#___#___#_#___#_#_#_#_#_____#_____#_#_#_________#___#_______#___#_#_____#_#___#_____#_#_#___#___#_#_#_#_#___#___#_#_________#_______#___#___#_#_#_#_#___#_#_____#___#_#_____#___#_________#___#_____#_#_#_______#_#_#___#_#_#___#___#_________#___#_#___#_#_#_#_#_____#_____#___#_____#_#___#_#_#_#_____#_#_#_#_#___#_#_#_#_#_____#_#_#
#_#########_###############_#_#_#######_###_#####_#######_#######_#_#_#_#_#_###_#_#_#_#_#_#_#_#_###_###########_#####_###############_#####_#######_###_#_#####_#_#_#_#_#_#_#_###_#_#_###_#_###########_#_#######_#_###_#_###_#_###_#####_#_#_#_#_#_#####_#_#############_#####_#_#####_###_#_###_#_#_#_###_#####_#######_###_#_###_#_#_#_#_#########_###_#_###_#_#_###_###_#_#_###_#_#_#_#_###_#_#_#_#_###_#_#_#
#_____#___#_____#_________#_#_#___#___#_#_______#_#___#___#___#___#_#_#_#___#___#_#___#_#___#_#_____#_________#_____#_#_________#___#_#___#_______#_____#___#___#___#___#_#_#_#___#_#___#___#___________#_#_______#_#___#_____#_#___#___#_#_#_____#_#_____#_#_#_________#_#___#_#_____#_#___#___#_#___#_#_______#___#___#_#___#___#_#___#___#_______#_#___#_#___#_#___#_____#_#_#___#_#___#_#___#_#_#_#_#_____#_#
#####_#_#_###_###_#######_#_#_###_#_#_#_#_#######_#_###_###_#_###_#_#_#_#####_###_#####_###_#_###_###_#######_#_###_#_#_#####_#_#_###_#_#_#_###_#########_#_#_###_#######_#_#_#_###_#########_#####_#######_#######_#_#######_###_#_#_#_#_#_#########_###_#_#_#_#######_#_#_###_#_###_#_#_#####_#_###_#_#######_#####_#_#_#_#####_#_###_#######_###_#_#_###_#_###_#_#######_###_###_#_#####_#_###_#_#_#_#######_#
#___#___#___#_#___#_____#_#_#_______#___#_______#_#___#_____#___#_#___#_______#_#___#_#___#_#___#___#___#___#_#___#_#_#___#_#_#_#_#___#_#_#_#_#_#_____#___#___#_____#___#_____#___#___________#___#_#_______#_____#___#_____#_____#___#_#_#_#_#_____#___#_#_#_#_#_____#___#___#_#_#_#___#___#_#_#_#___#_______#_#_____#_#_#_#_____#___#_#___#___#_____#_#___#_____#_#_____#_____#___#_#_#___#___#___#_#_______#_#
#_#########_###_###_#_###_#######_#######_#####_#_###_#########_#_#############_###_#_#_###_###_###_###_###_#_#####_#_###_#_#_#_#_#_#_#_#_#_#_#_#_#_###_###########_#_#_#########_#_#######_###_#_###_#######_###_#####_###_###########_#_#_#_#_###_###_#_#_#_#_###_#_#######_#_#_#_#######_#_#_#_###########_#_#_#####_#_#_#_#######_#_#_#_#_#######_#_#_#########_#_###_#######_#_#_#_#_#####_#_###_#######_#_#
#_________#_____#___#_____#_____#_#_____#_____#_#_______#_______#_____#___#_______#_#_#___#_#_#_#_____#_____#_____#_____#_#_#_#___#_#_#_#_#___#_#_#_#___#_______#_#_#_#___#_______#_#_____#_____#_____#_____#___#_______#_____________#_#_#___#_#_______#_#___#___#_#_________#_#___#_____#___#_#_____________#___#___#___#___#___#___#_#_#___#_____#_#_#_________#_#___#_#_______#_#_#_#_#_____#_#_________#_#_#
#_#####_#_#######_#########_###_###_###_#####_#_###_#####_###########_#_#_###_#_###_#_###_#_#_#_###########_#####_#######_#_#_#####_###_#_#####_#_#_#_###_#####_#_#_#####_#_#######_#_###_###########_###_#####_#####################_###_###_#_#############_###_#_#_#######_#_#####_###_#_###_###_###############_#_#####_###_#_#_###_#_#####_###_###_#########_#####_#_#####_#_###_#_#_#_#####_#_###_###_#_#_#
#_#_____#___#___#_#_________#___#___#_#_____#_#_____#_____#_____________#_____#_#___#___#_#___#_#___#_____#___#_#_____#___#_#_____#_____#_____#_#_#_#___#___#_#_#___#_____#___#_____#_#_#_#_________#_____#_____#_____#___#_#_______#_____#___#_#_____________#_#_#_#_#___#___#___#___#_#___#_____#_#_____________#_#_#_____#___#_#___#_#___#_____#_________#___#_______#_____#_#_______#___#___#_#_#___#___#_#_#
#_#_#######_###_#_###_#######_###_###_#####_#_#######_###_#####################_#_###_#_#_###_#_#_#_#_#_#####_#_#####_#_###_#####_###########_#_#_#_###_###_#_#_#_###_#######_#######_#_#_#_#######_#_#####_###_#_###_#_#_#_#_#_###########_###_#_#######_#####_#_###_#_#_#######_#_###_#####_###_#_#_###########_#_#_#_#######_#_###_#_###_#_#############_#_#######_#######_###############_#_#_###_###_###_###
#_#_#_____#_____#_____#_____#_#___#___#_____#_#_____#_#___#_____________#_______#___#_#_#___#_#_#_#___#_____#_#_____#___#___#___#_________#_#_#___#___#_____#_#_#_____#_____#_______#_#_____#_____#_#_#___#_#___#_#_#___#_#___#_________#___#_#_#_#_____#_#_____#___#___#___#___#_#_#_______#___#_#_#_#_________#_#_#_________#_#_#___#_#___#_#_______#___#___#_____#_#_____#_#_______#_____#_#_#_____#___#_#___#
###_#_#_#######_#######_#####_#_#####_#_#####_#_###_#_#####_###########_#####_#####_###_###_#_#_#_#########_#_#_###########_#_#_#####_###_#_#_###_#_#########_#_#####_#_###_#######_#_#######_#####_###_#_#_#_###_#_#####_#_#########_###_###_#_#_#_###_#_#_#_#####_###_###_#_#_#_#_#_###_#####_#_#_#_#_#_#######_###########_#_###_#_#_#_###_#_#####_#_#####_#_###_###_###_#_#_#####_#_#_###_#_###_#####_#_###_#
#___#_#_#___#___#_____#_#___#_#_#___#_#_#_____#___#_#_____#___#_______#_____#_____#_#_____#___#___#___#___#___#___________#_#_#_______#___#_#___#_#_________#___#_____#___#___#___#_#___#_________#_____#___#_#___#___#___#_#___#___#___#_____#___#_#_#_#_#_#_#___#___#_#___#_#_#___#_#___#_____#_#_#___#_#_______#___#___#_#___#___#_#_#_____#_#_#___#_______#___#_#___#___#_#_____#___#_#___#_#___#___#___#___#
#_###_###_#_#_#####_#_#_#_#_#_#_#_#_#_#_#_#######_#######_###_###_#_#######_#####_#_#_###_#########_#_###_#########_#####_#_#_#########_###_###_#_#######_#_#_###########_###_#_#_#_#_#_#_#######_###########_#_###_#_#_#####_#_#_#_#_#_#####_#####_#_#_#_###_#_#_###_#_#_###_#_#####_#_###_#######_#####_#_#######_#_#_#_#_###_#_###_#_#######_#_#_###_#########_#_#_###_###_#_#######_#_#_###_#_###_#_###_#_###
#___#_#___#___#_____#___#_#___#___#_#_#_#_#_____#_______#___#_____#_#_____#___#___#_#_#_#_____#_____#_____________#_#_______#_#_______#_#_____#_#_____#_#_#_#___________#___#___#___#_#_#_#___#___#_#_________#___#_#_#_______#_#_#_#_#_____#_#_____#_#_#_#___#_#_#___#_#_____#___#___#_#_#_#___#_______#_#_________#___#_#___#_#___#_#_________#___#_#_#_______#_#___#_#___#___#_____#_#___#_#___#___#___#_#___#
#_#_#_#_#########_#####_#_#########_#_#_#_###_#_#####_#_###_#######_#_###_###_#_###_#_#_#####_###_#_#############_#_#######_#_#_#####_#_###_#_#_#####_#_#_#_###########_###_###########_#_#_###_#_#_#_###########_###_#########_#_#_#######_#_#_#####_#_#_#_###_#_#_#########_###_#_#_#_#_#_#_#_#_#####_#_###############_#_#_#_###_#_###########_###_#_#_#####_#_#####_###_#_###_###_#_#####_#####_#####_#####_#
#_#_#_#_________#_____#_#_#_____#___#___#_____#_____#_#___#_____#___#_#_______#___#___#_____#___#_#___#_________#_#_______#_#_#_____#_______#_#___#_#_#_#_#_#_________#_____#___________#___#___#___#_________#___#___#_______#___#_______#_#_#_#_____#_#___#___#___#___#___#_#_#___#_#___#___#___#___#_#_#_________#_____#_#___#___#___#_______#___#___#_#_______#_____#___#_#___#_#_#_#_________#_#_____#_____#
###_#_#########_#####_###_#_###_#_###_#############_#_#####_#####_###_#################_###_###_###_#_#########_#_#######_###_#####_#########_###_#_#_#_#_#_###_#####_#############_#_#####_#_###############_#_###_###_#_###_###########_#_#_#_#_#####_#####_#######_#_#_#_#_#_#####_#_#######_###_#_###_#_#####_###_#####_#####_#######_###_#####_#_###_#########_#_###_#####_###_#_#_#_#####_###_#_#####_###_#
#___#_________#_#_____#___#_#_#_#_#_________#_#___#___#_____#_____#_#___________________#_#___#___#_#_________#___________#___#_#_____#_____#_#___#_#___#_#___#_#_______#_________#_#___#___#_________#___#___#_#_______#_#___#_#___#_____#___#___#_____#_____#_______#___#_#___#_____#_#_____#___#_#___#___#_#___#___#_____#___#___#___#_#_#_____#_#___#_#___#_____#_#___#_____#_____#_#_#___#_#___#_______#___#
#_###_###_#_###_#_#####_###_#_#_#_#########_#_#_#_###_#_###_#_#####_###############_#####_###_###_###_#######_#_###########_###_#_#####_###_###_###_###_#_###_###_#####_#_#######_#####_#####_#####_#_#_#_#_###_#########_#_###_#_#_#_###########_#_#####_#####_###########_#####_#####_#_###_#####_###_#####_#_###_#########_#_#_#_#_#_#_#_#####_#_#####_#_#_###_###_#_#####_###_#####_###_#_#_#_###############
#_#_____#_#_#___#_#_____#___#___#_#___#___#_#___#_____#_#___#___#_____________#___#_#_#_________#___#_______#_#___#_________#_____#_____#_______#___#___#___#_____#___#_#_____#___#___#_____#___#___#_#_#_#___#___#_______#_#_____#_#_____#_______#_______#___#___#_____#_#_#_____#_____#___#_______#_#_______#_#_#_________#_#_#_#_#_#_#_#_____#_#_#___#___#___#___#_#___#___#___#___#_____#___#_#_____________#
#_#_#####_###_###_#_#####_#_###_#_#_#_#_###_###########_#_#####_###_#########_#_#_#_#_#_###########_#######_#_###_#_#########_#####_#############_#_#_#####_#########_#_#####_#_###_#_#_###_#####_###_###_###_###_#_#######_#_#####_#####_#_###############_###_#_#_###_#_#_#_#####_#######_#########_###_#####_#_#########_#_#_###_#_#_#_#_#_###_#_#_#_#######_#_#_#####_#_###_#####_###_#######_#_###########_#
#_#_#___#_________#_#_#___#___#_#___#_#_________________#_____#_____#_______#_#_#___#_____#_____#___#_#___#_#___#_#_#_____#___#___#_#___________#_#_______#___#_______#_______#_____#_#_#___#___#___#_____#_#_#_#___#___#___#___#___#___#___#_#_____________#___#_#___#___#_#_#___#___#___#_______#_____#_______#_____#_____#_#___#___#___#_#_#___#_#_#_________#_#_____#___#___#___#_#___#_____#_#___________#_#
#_###_#_#_#########_#_#_#####_#_#####_#######################_#######_#####_#_#_#########_#_###_#_###_#_#_#_###_###_#_#_###_###_#_#_#_#########_#############_###_#_#_###############_#_#_###_#_#_#_#####_#_#_#_#####_#_#_###_###_#_###_#####_#_#######_#_###_###_#_#_#####_#_#_#_###_###_#######_#_###_#########_#_#_#_###_#_###_#_#########_#_#_#_#_#############_#_#######_###_#_#_#_###_###_#_#_###########_#
#_#___#_#_#_________#_______#_#_____#_____#___________#_____#_____#_____#___#_#_#_______#_#_#___#_#___#_#___#_#_#___#_#_____#___#___#_#_#_____#_#___________#___#_#_#___#_______#___#_#_#_____#_#_#_#_______#___#_#___#_#_#___#___#___#_________#_#___#_#_____#___#_#_#_____#___#_#___________#___#___#___________#_#_#___#_#_#_#_#___________#_#_#_#_____#_______#_#___#___#_#___#_#_____#_#___#___#___________#
#_#_###_###_#################_#####_#####_#######_###_#_###_#####_#######_###_#_#_#####_#_#_#_#_#_#_#_#_#####_#_#_#########_#_#######_#_#_###_#_#_#########_#_#_###_###_#_###_#_###_#_#_#######_###_#_#########_#_#_#_###_#_###_#####_#####_#####_#_#_#######_#_#####_#_#######_#_###########_#_#######_###########_#####_###_#_#_#_###########_###_#####_#_#####_#_###_#_#_#_###_#_#####_#_#_#####_#_#########_#
#_#___#_____#_______________#___#___#_____#_____#___#___#_#_______________#___#_#___#___#_#_#_#_#_#_#_________#_#_________#_#_____#___#___#___#___#_______#_#_#_#___#___#_#___#_____#_#_#___#___#___#_#_________#_#_#_____#___#_#___#_____#_#_____#_#_______#_#_#_____#___#___#_#___#___#_____#_______#_#_________#_#_____#___#_#_#_#_________#_________#_#___#_#_#___#___#_______#_____#_#_#_#___#_#_#_____#___#
#_###_#######_#############_#_#_#_###_#####_###_#####_###_###############_#_###_###_#_###_#_#_###_#_###########_#########_#######_#_#####_#_###########_#_#_###_#_#######_#_#########_###_#_#_###_###_#_#########_#_###########_#_#_#####_#_#####_#_#######_###_#_#_#####_#_#_#_###_#_###_###########_###_#######_#_#_###_#_###_#_###_#######_#########_#_###_#_#_#####################_#_#_#_#_#_###_#_###_###_#
#_#___#_______#_#_____#_____#_#_#_#___________#_____#_______#_____#___#___#_#_#___#_#_#___#_#_____#_____#___#_____#_______#_______#_#___#_#_#___________#_#_#___#___#_____#_________#_____#_#_#_____#_#_#_______#_#_#_#_________#_#___#___#___#___#___#_________#_#_____#___#_#___#_#_____#_________#_#___#___#___#_#_#_#_#_#___#_____#_____#_#___#___#_#___#_#_#_#___#_______#_______#_#_#_#___#___#_#___#___#_#
#_#_###_###_###_#_###_#_#######_#_#####_#####_#####_#_#######_###_###_#_###_#_#_###_#_#####_#########_###_#_#_#_###_#####_#_#####_#_#_#_#_#_#_###_#####_###_#_#####_#_#############_#######_#_###_#_###_#_#####_#_#_#_#_###_#####_#####_#####_#_#####_#_#########_#####_#####_###_#_#_#####_###_#####_#_###_###_###_#_#_#_#_###_#######_#_###_#_#_#_#_#_#_#_#_#_#_#_#_#_#####_#_###_###_#_#_#######_#_###_###_#_#
#___#_____#_#_____#_#___#_______#_#___#_#___#___#_#_#_#_______#_#___#_#___#___#___#_#_____#___#___#___#___#___#_#___#_____#_#___#_#___#_#_#___#___#_#___#___#_#___#_________#_____#_______#_#___#_#___#___#_#___#_#_#_____#_#_____#_____#___#_#_____#_#_____#___#_#_________#___#_#_#___#___#_#_#_____#___#___#___#_#_#___#___#_____#___#___#___#_#_#_____#_#_#_#___#_#_#_____#___#_#___#_#___#___#_____#_#_#_#_#
#_###_#####_#_#####_#######_#####_#_#_###_#_###_#_#_#_#_#######_###_#_###_###_###_#_#####_###_#_#_#_###_#######_#_#####_###_#_#_#######_#####_#_###_#_###_###_#_#_###########_###_###_#_###_###_#_###_#####_#_###_#_#_#####_#####_#_#####_#_#_#_#_#_#_#_#####_#_#_#_###########_#_#_#####_###_#_#_#####_#_#_#_###_#_#_#######_#_#####_#####_#####_#####_#####_#_#####_#_#_#_#####_###_###_###_#_#_###_###_#_#_###
#_#___#_____#_#_____________#___#___#_____#_#_#_#_#_#_#___________#_#_#_____#_#_#___#_______#_#_#_#_#___#_____#_#_____#___#___#_________#___#_#_#___#_#_#_#_____#___#_______#___#___#_#_#___#___#___#_______#___#_#_#___#___#_____#_#___#_#___#_#_#_#_#_#_____#_#_#_______#_____#_#_#_____#___#_#_____#_#_#_#_______#_________#___#___#_#_______#_____#_#___#___#___#___#_#_#___#_____#_____#_#_#_____#___#_#___#
#_#_###_#####_#_###############_###########_#_#_#_#_###########_###_#_#_#####_#_#####_#####_#_###_#_#_#####_###_#####_###_###############_#_###_#_#_#_#_#_#_#######_#_#####_###_###_###_#_###_#########_###_###_#_#_#####_###_###_#_###_#_#####_#_#_#_###_#####_#######_#_#_#####_#_#_#####_#_#_#####_#_#_#########_###########_#_#_###_#_#####_#####_###_#_#_###_#_#####_#_#_#_###_#########_#########_###_###_#
#_#_#___#___#_#_#_____#_______#_#_________#_#_#___#_#_______#___#___#_____#___#_____#_#_____#___#_#_#___#___#___#_#___#___#___#_____#_#___#_____#_#_#_#_#_#_#_____#_#___#_#_______#___#___#___#_______#___#_______#_____#_#_____#_#___#_______#_#_#_#___#_#___#___#___#_#_#_#_____#_#___#___#_#_______#_#___#_____#_#___#_______#_#_____#_____#_#___#_____#___#___#_______#_#_#___#_#_______#_____#___________#_#
#_###_###_###_#_#_###_#_###_#_#_#_#######_#_#_###_#_#_###_###_###_###_#####_#####_###_#_#######_#_#_###_#_###_###_#_#####_#_#_#_###_#_#_#_#######_#_#_#_#_#_###_#_#_###_#_###########_#_###_#_#_#####_#################_#_###_#_#####_#######_###_#_###_#_###_###_#_#_###_#_#######_###_###_#_#########_###_#_###_#_#_#_#_#######_###_#####_#_#_#_#############_###########_#_###_#_###_###_###_#_#_###########_#
#_____#_______#_#_#_#_#___#_#___#_#___#___#_#_#___#___#_#_____#___#___#___#_#_____#___#___#_____#_#_#_#_#___#_____#_____#___#___#_#_#___#_#_____#_#_#___#_#_____#_#___#_#_#_________#_#_____#_#_#___#_____#_____#_______#___#_#_____#_#___#___#___#___#___#___#___#_#_____#_#_________#___#_#_____#___#___#___#___#_#_#___#_____#_____#___#_#_#_#_____#_#_____#_____#___#___#_#___#_____#_#___#_#_#_#_#_________#
#######_#######_#_#_#_###_#_#####_#_###_#_#_#_#_#######_#######_#######_#_#_#_#####_#####_#_#####_#_#_#_#_#_###_#######_#########_#_#####_#_###_#_#####_#_#######_###_#_#_#_#######_#_#######_#_###_#####_#_###_#_#########_#_###_###_#_#_#_###_#####_#####_#_#_###_#####_#_#_#####_#####_#_#_###_#_#_###_#####_###_#_#####_###_#######_#_###_#_###_#_#_#_#_#######_#_#_#_###_#_#########_###_###_#_#_#_#######_#
#___#_#_#_______#_#_#_#___#_______#___#_#_#_#_________#___#___#_#_______#___#_____#___#___#_#_________#_#_#_____#_______#___#_____#_____#_#_#_#_#_______#_____#___#___#___#_______#___#_____#_#_______#___#_#_#___#_________#___#___#___#_#___#_____#___#___#_#_#___#_____#___#___#_#_____#_#___#___#___#_#___#___#_#_#_____#___#___#___#_____#_____#_#___#_______#___#_#_#___#_____#_______#___#_#_#_#_#_____#_#
#_#_#_#_#_#######_#_#_#_###########_#_#_###_#########_#_#_###_#_#_###########_###_###_#_###_###_#######_#_#######_#######_#_###_#_#####_#_#_#_#_#_###########_#_###_#_###_#######_###_#_###_#_#######_#_###_#_#####_###########_###_#####_###_###_#_###_###_###_#_#######_#####_###_#_#####_###_#######_#_###_###_#_###_#####_###_#_#_###############_#######_###_#####_###_#######_#_###_#####_#_#_#_#_#####_#_#
#_#___#_#___#_#___#___#_#___#_______#_#___#_______#_#_#_#_____#___#_________#___#_#___#___#___#___#_____#_____#___#_______#_____#_#_#___#_____#_#_#___#_______#___#_#_#___#_____#___#_#_#_#_#___#_____#_____#_______#_______#___#_#_____#_#_____#_#___#___#_#___#___#___#_#_________#_#___#___#_#___#___#_____#___#___#___#_#_#___#___#_____#_________#_____#_#_#_____#_____#_______#___#___#___#_____#_____#_#_#
#_###_#_###_#_#_#_#####_#_#_###_#####_###_#######_#_#_#_#####_#########_###_#####_#_#########_#_###_#_#########_###_#############_#_#_#########_#_#_#_#_#########_#_###_###_###_###_###_#_#_#####_###############_###_#####_#_###_#_###_#_#####_#_###_###_#_#_###_#_#_#_###_#########_#_#_###_#_#_#_#####_#####_#####_###_#_#_#_#######_#_###_#########_###_#_#_###_#########_#########_###_#_###_#########_#_#_#
#_#_#_#_____#___#_#_____#_#___#___#___#___#_____#_#___#_#___#_________#___#___#___#_______#___#_#___#_#_______#_#_______#_______#___#___#_______#_#_#_#___#_____#_#___#_____#_#_#_#_#___#_#_______#___#___________#___#___#_#_____#_#___#_____#_#___#___#___#_#_#_#___#___#_______#___#_#_____#_#_#___#___#___#_#___#___#_#_#_#_#___#___#_#___#___________#_#_____#___#_____#___#_____#___#___#_#_#_#_______#_#_#
#_#_#_#######_###_#_#####_###_#####_###_###_#_###_#_###_#_#_#########_###_###_#_#_#######_#_#####_#_###_#####_#_#_#####_#####_###_#####_#_#_#####_#_#####_#_###_#_###_#######_#_#_#_#_###_#########_#_#_#_###########_#_###_#####_#_#_#######_#_###_###_#_###_#_#_#######_#######_#_#######_###_#_###_#_###_#_#_#_#_###_#_#_#_#_#_#_#_###_#_#############_#_#########_#_###_###_#_###_###_#####_#_#_#_#######_#_#
#_#_#___#_____#___#_____#_#_#_____#___#_#___#_____#___#___#_#___________#___#_#_#_#_______#_#___#_#_#___#_#_____#_#___#_#___#_____#___#_#_#_#___#_#_____#_#___#_____#_______#___#_#___#_____#_____#_#___#___#_________#_______#___#_#_#_#_____#_#___#___#_#___#_#_#_____#_________#_#_____#___#___#___#_#___#_#___#_#___#_#_#_#_#_#___#___#_________#_____#___#_______#_#_#___#_#___#_____#_______#_#___#___#___#
#_#_###_#######_#_#####_#_#_#####_#_#_#_#_###########_#####_#_#########_###_#_#_#_#_#######_#_#_#_#_#_###_#_#######_#_#_#_#_#_#####_#_#_#_###_#_#_#####_#_###########_#_#####_###_#######_#_###_#_#_#######_#_#######_#######_#_###_#_#_#_#####_#_###_###_#_###_#_#_###_###########_###_#_###_#####_#_#_#_###_#####_#_###_#_#_#_#_#####_###########_#_#######_#_#######_#_###_#_###_#############_#_###_#_#_#####
#_#___#___#_____#_#_____#_#_____#_#_#_#___#_________#_______#___#___#_____#_#___#_#___________#___#_#_#_____#___#___#_#___#_#_____#_#_#_#_____#_#_#___#_#___________#_#_____#_#___________#_____#_#_______#_#_#_______#_____#_#_#___#_#_#___#___#_#___#_#_#_#_#___#_#_______#___#___#___#___#_#_____#_#_#_#_________#_____#_____#_____#_______#___#_#_____#_#_#___________#___#_#___#_______#___#_#___#_#_#_____#
#_###_###_#_#_#####_#####_#_###_#_#_#_#####_#####_#_###########_#_#_#####_#_###################_#####_###_###_#_#_###_#####_#######_#_#_###_###_#_#_#_#_###########_#####_#_#_#######_#_#########_#########_#_#####_###_###_#_###_###_#_###_#_#####_###_#_#_#_#_###_#_#####_#_#_#_###_#####_###_#######_#_###########_###################_###_#_#_#_#####_#_#_#_###########_###_#_###_###_#_#_#_#_#_###_#_#####_#
#___#_#___#_#_#___#_#_____#___#_#_#_#_____#_#_____#_#___#___#___#_#___#_#_#___________#_____#___#___#___#_#___#_#_#_#_____#_________#_____#_#___#___#_#_#___________#___#_#___#_____#_#___#_____#___________#_____#_#_____#_#_____#_#___#___#_____#___#___#_#_#___#_#_____#_#_#_#_#_____#_#___#_______#_#_____#_______#_____#___________#_#___#_#___#_____#_#_#_____#_#_____#___#___#___#_#___#_#_#___#_____#___#
###_#_#_###_#_#_#_#_###_#####_#_#_#_###_#_###_#####_#_#_#_#_###_#_###_#_#_###########_#_###_#_###_#_###_###_###_#_#_#####_#_#############_###_#######_#_#_#######_###_#_#_#####_###_#_###_#_###_#################_###_#####_#######_###_#_#######_###_#_###_#_###_###_###_#_###_#_#_###_#_###_#######_#_#####_#####_###_###_#_#_#######_#_#_###_#####_#####_#_#####_#_#_#####_#####_###_#_#####_#_###_#######_#_#
#___#_______#_#_#_#_#___#___#_#___#___#_#_____#___#_#_#___#___#_#_#_#_#_#_#_______#___#_#___#_#___#_#___#___#_#_#___#___#_#_______#_____#_____#___#_____#_______#_#___#_#_#_____#___#___#___#___#_#_________#___#___#_#_____#_________#_#___#___#_____#_#___#___#___#___#_#_____#_#_#___#___#_#_____#_______#_#___#_#___#___#_#_____#___#_#___#_#_____#_____#_#_______#_____#_____#___#_#_#___#_#_________#___#_#
#_###_#########_#_#_#_###_#_#_#########_#######_#_#_#_#######_###_#_#_#_#_#_#####_#_###_#_#####_###_#_#_#_###_#_###_#_###_#########_###_#_#####_#_###########_#_#_#_###_#_###_#_###_#####_###_###_#_#####_#_#_#_###_#_#_#######_###_###_###_#_#_#######_#_###_#_###_#####_###_###_#_#_###_#_#_#_###_#######_#_#_#_###_###_###_#####_#_###_###_#_#####_#_#####_###########_#_#####_###_#_#_###_#_###########_#####
#___#_#___#___#_#___#___#_#_#_#_________#_#_____#_#___#_____#_____#_#_#_#___#_______#___#_____#_#_____#_#_____#_____#___#_____________#_#_#_____#_____#_____#_#_#_#___#_#___#_#___#_____#___#_#_______#___#___#___#___#_#_______#___#_____#_#_#___#_____#___#_#_____#_____#___#___#_#_#___#_#___#_#_#_____#_#___#___#___#_#_______#_#_#___#___#_#_____#_____#___________#_#_____#_#___#_#___#_#___#___#_____#___#
###_#_#_#_#_#_#_#######_#_#_#_###_#######_#_###_#_#####_###_#######_#_#_#####_#####_#_#######_#_#############_#########_###############_#_#_#########_#_###_###_#####_#_###_#####_#####_#####_#########_#########_#####_#_#########_#_#####_#_#_###_#######_#_#######_#####_###_###_#_#_#_#######_#_#_###_#_#######_###_#_#######_#_#_###_#####_#_#####_###_###########_#######_#_#_###_###_#_###_#_#_#_#####_#_#
#___#_#_#___#___#_______#_#_#___#___#_______#_#_#_#_______#___#___#___#_______#___#_#___#_____#_#___________#_#___________#_____#_____#_#_#___#_#_____#_#_#_____#_____#___#_______#_#___#_____#___#_____#_______#___#___#_#_______#_#_#_#___#_#_______#___#_#_#_______#_____#_#___#_#_#_#_#_______#_#___#_#_______#___#_#_______#_#_#___#_#_____#_____#_#_#_____#_____#___#_____#_#___#___#_#_______#___#___#_#_#
#_#####_#########_#########_###_###_#_#######_#_#_#_#####_#####_#_#_###_#######_#_#_###_#_#####_#_#####_###_#_###_#######_#_#####_###_#_#####_#_#_###_#_#_###_###_#######_#########_#_###_#####_#_#_#####_#####_###_#_###_#_#####_###_#_#_#############_#_#_#_###_###_#_#####_###_#_#_#_###_###_#_#_#_#_#_#######_#_###_#######_#_#_###_#_#_#####_###_#_#_#####_#_###_###_#_###_#_###_#####_###########_#_#_#_#_#
#_#_____#_____#___________#___#_____#_#_______#_#_#_#___#_______#_#___#_#_______#_#_#___#_#_____#_____#___#_#_____#_#___#___#_____#_____#_______#___#_#_#_____#_____#___#_#___#_____#_____#_____#___#_____#___#_#___#_#___#_____#_______#_#_________#___#___#___#___#_#_________#_#_#_#_#___#_#_#_#_#_#_#_#___#___#_#___#___#___#_#___#_#_#_#___#___#_#_#___#___#___#___#___#___#_#_________#_____#_____#_#___#_#
#_#_#####_###_#####_#####_###_#######_#####_#_#_###_#_#_###_#####_###_#_#_#######_###_###_#####_#####_###_#########_#_#_#_###_###_#######_#########_#_#_#######_#####_#_#_#_#_#_###_#########_###_#######_#_#_#_#_###_###_#_#############_#_#######_#_#########_#####_#######_###_###_#_#_###_#_###_###_#_#_#_###_#_#_#####_#_###_###_#_#_#_#_#_#####_#_#_###_#_###_###_#####_###_###########_###_#######_#####_#
#___#_____#___#___#_____#___#_______#_____#_#_#_____#_#___#_____#___#_#_#_____#_______#_#_____#___#___#___#_________#_#___#___#___#_____#___#_____#_#_#_______#_#_____#_#_#_#_#___#___________#___#_____#___#_#___#_____#___#_____________#_#___#___#_________#_____#_____#___#___#___#___#___#___#_#___#_#_#___#_#_________#___#_#_#_#___#_#_#_______#_#___#_#_#___#_#_____#___#___________#___#___#___#___#___#
#_#####_###_###_#_#######_#_###_#########_###_#######_###_#######_#_#_#_#####_#########_#####_###_#_###_#_#_#######_#_#####_#######_###_###_#_###_#_#_#######_#_#_#####_#_#_#####_###_#########_###_###_#####_#######_#_#_###_#############_#_#_#_###########_#####_#####_#####_###_###_###_#_###_#_#_###_#_###_###########_###_#_#_#_#####_#_#########_#_#_#_###_###_###_#####_###########_###_###_#_#_#_###_#_#
#_____#_#___#___#___#___#_#___#_#_________________#___#_#_________#_#_#_#___#_#_____#_______#_____#_#___#_#___#_#___#_____#_#_______#___#___#___#___#___#___#_#_#_#_#___#_#_____#_#_#_#___#___#_#___#_#_#___________#_#_#_#_#_#_______#_____#_#_______________#___#_______#_____#___#_#_#___#___#___#___#___#_#_____#_____#_#___#___#_#_____#_#_#_______#_#_#___#___#___#_#_____#_____#___#_____#_#___#_#_#___#_#
#####_#_#_#####_###_#_#_#####_#_#_#################_###_###########_#_#_###_#_#_###_###_###_#####_#_#_###_###_#_#_#_#####_#_#_#######_###_#####_#########_#_#_#_#_#_#_###_#_###_#_#_#_#_#_#_#_#_#_###_#_###########_###_#_#_#_###_#####_#_#######################_#######_#_#####_#_#_#_###_#_#######_#_#####_#####_#_###_###_#####_#_#_#####_#_#_#########_###_###_#_###_#_#####_###_###_#######_#####_#_#_#####
#_____#_#___#___#___#_#_______#___#_______#_______#_#_______#___#___#_#___#_#_____#___#_#_#_#_#___#_#___#___#___#_#___#___#_____#___#_____#_____#_______#_#___#_#_#_____#_#_#_#_#_#_#___#___#_#_#_#___#_#___#_____#_#___#_#_#___#___#___#_______#___#_#_______#_____#___#_#___#___#_#_#___#_#_______#_#___#_______#___#_#_____#_____#_#___#___#_#_#___________#_____#___#_#_______#___#_____________#_#_#_#_____#
#_#####_###_#_###_###_#########_###_#_#####_#####_#_#_#####_#_###_###_###_#_#########_#_#_#_#_#_###_###_###_###_#_#####_#########_#_#_#####_#####_#####_#_#####_#_#####_#_#_#_#_#_#_#########_###_#_#_#_#_#_#_###_#_#_###_#_###_###_#_#########_#_#_#_#_#_###_#_###_#_#_#_###_#####_#_###_#########_#_###_#####_#######_#######_#####_###_#_###_#_#_#########_#######_#_#_#########_###_###########_#_#_#_#####_#
#___#_____#___#___#_______#___#_#___#_#___#_#_______#_____#___#___#_#___#_____#_______#_#___#_#___#_#_#___#_#___#_______#___#___#_#_#_____#___________#_#_#_____#___#___#_#___#___#___#_____#_____#_#_#___#_#___#_#_#_#_#_#___#___#___________#_#_#_#_#_#___#___#___#_#___#_#_____#_#___#_________#_#_#_#_____#_____#_______#_______#___#_#_#___#_#_#_______#_#_______#_#___#_____#___#_______#___#_#_#_______#_#
###_#_###_#####_###_#####_#_#_#_#_#_###_#_#_###_#####_###_#####_###_###_#####_#_#######_###_#_###_#_#_###_###_###########_#_#_#_#_#_#####_#############_#_#_#######_#_###_###_#######_#_###_#_#####_#######_#####_#_#_#_#_#_#####_###########_#_#_#_#_#_###_#########_#####_#####_#_#_#_#########_#_#_#_#####_###_#_#_#_###_#_#######_#_#_#_#_#_#_#_#####_###_#########_###_#_#_#####_#_#####_###_#_#_#########_#
#___#_#___#___#_#_____#___#_#_#_#_#_#___#_#___#_#___#___#___#___#_____#___#___#_#_________#___#_#_#___#_#_____#_____#___#_#___#___#_____#___#_____#___#_#___#___#___#_#_____#_#_______#_#_#_#_#_________#___#___#_#___#_#_#_____#_#___#___#___#___#_#_____#_#___#___#___________#___#_#_____#_____#_#_#___#___#___#_#_#___#_#_#_____#_#_#_#_#_#___#_____#___#_________#___#_#_#_______#_____#___#___#_#_______#_#
#_###_#_#####_#_###_###_###_###_#_###_###_###_#_#_#_#######_#_###_#_#_###_#_###_#####_###_###_#_#_###_#_#########_#_###_#_#####_#######_###_#_###_#_#_#_#####_###_###_#_###_#_#_#_###_#_#_#_#_#######_#_#_###_#_#_#####_#_#_###_#_#_#_#_#_#_#####_#_#####_#_#_#_#_#_###########_#####_#####_#_#####_#_#_###_###_#####_###_###_#_###_###_#_#_#_#####_###_#_#_#########_#_#_#_#_#################_#_###_#_#####_#_#
#___#_#_#_____#___#_#___#___#___#___#___#_____#_#_#_#_____#___#_#_#_#_#___#_#_#_____#_#___#_____#___#_____________#_____#_#_____#_____#_#___#_#_#___#_#_______#___#_#_#_#___#_#_#_#___#___#_#_____#___#_#_____#_#_____#___#___#_#___#___#_#_#___#_#___#_#_#_#_#___#_______#_________#___#___#_#_#_____#_#___#___#___#_#_#_____#_#_____#_#_#_#___#___#___#_#_____#_____#_#_#___#_____________#___#___#_#_#_____#_#
###_#_#_#_###_###_#_#_###_###_###_#_###_#######_#_#_#_###_#_###_#_#_###_###_#_#####_#_#_###########_#################_###_#######_###_#_#####_#_#####_#_#####_#_###_#_###_###_###_#######_#_#####_#_###_#####_#_#_#####_#######_#########_#_#_#_#####_#_#_#_#_#####_#######_###########_#_###_#_#_#####_#_###_###_#_#_#_#######_#####_#_#_#_###_#####_#####_#_#_#_#####_#_#####_#####_#####_#_#####_#_#_#####_#_#
#___#_#_#_#___#___#_#___#_#___#___#___#_____#_#_#_#___#___#_#_____#_#___#_______#_#_#_#_#___#_______#___#_______#___#_#___#_________#_#_____#_#_#_____#_#___#_#_#___#_#___#_______#___#___#_#_______#_#_#___#_#_#_#_____#_______________#_#_#_#_____#_#_#_#_#_#___#_________#___#___#___#_____#_______#_#___#_____#_____#___#___#___#___#___#_#_#_____#___#_#_#_#_#_____#___#_____#___#___#___#___#___#_#___#_#_#
#_#####_#_#####_#######_#_#_#######_#_#####_#_#_#_#####_###_#######_#_#########_#_#_#_#_#_#_#_#######_#_#_###_###_#_#_#_###_#########_#####_#_#_#_#####_#_#_#_#_#_###_#_#_#######_#_#_#_###_#########_#_#_#_###_#_#_#####_###############_#_#_#####_#_#_#_#_#_#_#_###########_#_#_#_#_###############_#_###_#_#########_#_###_###_#_#########_#_#_#####_#_###_#_#_#######_#_#######_#####_#######_#_###_#_#_#_#_#
#_#___#_#_____#___#_____#_#_#_______#___#_#_#_#_#_#___#_#_____#_____#_#___#___#___#_#_#___#___#_#_____#_#___#_____#_#_#_________#_____#_#___#_#_#_#_____#_#___#_#_______#_#___#___#_#___#_#_________#___#_#_____#_____#___#_____#_____#___#_#_____#___#___#___#_#_____#___#___#___#_#_#___#_______#_#_#___#_#_____#___#_#_#___#___#_#_________#___#_____#_#___#_#___#___#_#_______#_#_____________#_#___#_#_#___#
#_#_#_#_#####_###_#_#####_#_###_#######_#_#_#_#_#_#_#_#_#####_#_#_###_#_#_###_#_###_#_#########_#_#_###_###_#######_#_#########_#_#####_#_###_#_#_#_#####_###_#_#######_###_#_#####_#####_#########_###_#_###########_#_#_#_###_#_###_#_###_#####_#####_#######_#####_#_#_#_#####_#_#_#_#_#_#_###_#_#_#_###_#_#####_#_#_#_#_###_###_#_###_###_#####_#####_#_###_###_#_#_#_#######_#_#_#_#####_#####_#_###_#_#####
#___#_#_____#___#___#_____#_____#_____#___#_#___#_#_#_#_#___#___#_#___#_#___#___#___#___#_____#___#_#___#_#_#_#_____#___#_____#_#___#___#_____#___#_#___#_#___#_______#_#___#_#_____#_#_____#_____#___#_#_______#___#___#_#___#___#_#___#___#_____#___#_#_______#_#___#_#_#_#___#_#_#___#_#_#_#_#_#_____#___#_#_____#_#_#_#_____#_#_____#_#_#_#_____#_____#_#_____#_#_#_#_#_____#_#_#_#_#___#_#_____#_____#_____#
#####_#_###_#_###########_#####_#_###_#####_#_###_#_#_#_#_#_#######_###_###_#####_#####_#_#_###_###_###_#_#_#_#_###_###_#_###_#####_#_#########_###_#_###_#_#########_#_#_###_#_#####_#_###_#_###_###_#_#######_#_#_#####_###_#####_#########_#######_#_#####_###_#_###_###_#_#_###_#####_###_#_#_#######_###_#_#####_#_#_#######_#######_#_#_#_#_#####_###_#######_#_#_#_#_#_#_#_#_#_#_#_#_###_#########_#####_#
#_____#___#_#_____________#_____#_#_#_______#_#_#_#_#_#___#_______#_____#___#___#___#_#_#_#___#___#___#___#___#___#_#___#___#_______#_________#_#_____#___#___#_____#_#_#_#_#___#_________#___#___#_#_#_#_____#___#_____#___#_______________#___#___#___#_____#___#___#___#_#_#___#_____#___#_#_#___#_____#___#_____#_#_#___#_________#___#___#_#_#___#___#_______#_#_#___#_#_#_#___#_#___#_#___#_#_____#___#___#
#_#######_#_#####_#_#######_#####_#_#########_#_#_#_#_###########_#######_###_#_###_#_#_#_###_#_#####_#_#####_###_###_#####_###_#######_#_###_#_#######_#####_#_###_#_###_#_###################_#_#_#_#_###_###########_#######_###########_###_#_#_###_#_#####_#####_###_#_#_###_#_###_###_#_#_###_#_#####_#######_#_#_#_#_###_#######_#####_###_#_#_###_#######_#_#_#####_#_#############_#_###_#_###_#####_#_#
#_____#___#_____#_#_________#_____#_________#_#_#_#_#___#___#_______#_____#___#_____#_#_#___#___#_____#_#_____#_#___#_#___#___#_#_____#_#_#_#_#_______#_#___#_#_#___#___________#_____#_________#_#_#_#___#_____#_____#_________#_________#___#___#___#___#___#___#_______#_#_#___#___#___#___#___#___#_____#_______#_#___#_____#_______#___#_____#_#___#___#___#_#___#_____#_______#_______#_____#___#_______#_#
#####_#######_#_#_###########_#####_#######_#_#_#_###_###_#_#_#######_#####_#########_#_#########_#_#####_#####_###_#_#_#_###_###_###_#_#_#_#_#######_#_###_#_#_###############_#_###_#_#########_#_#_###_###_#_#####_###########_#######_###_#_#####_#_###_#_#_#_#_#######_#_#_#########_#####_#############_#######_###_#####_#_#_#####_#_###_###_###_###_#_#_#_#######_#####_###_#_#####_#####_###_#########_#
#_#___#_____#_#_#___#_______#_#_______#___#_#___#___#_____#_#_________#_____#___#_____#_________#_#_#_____#_____#_#___#_#_#___#___#_#_#_#_#___#_____#_#___#_#_#_#_________#_____#_#_#___#_______#_#_#___#___#_#_______#_______#___#_____#_#___#___#___#_#___#_#_#___#_______#_#___________#___#_____________#_#_#___#___#_#___#_#_#_#___#_#___#_#___#_#___#_#_#_____#___#_#_____#_#_#___#_______#___#_________#_#
#_#_###_###_#_#_###_#_###_###_#_#######_#_#_###_###_#_#####_###########_#_###_###_#_###########_#_#_#_#####_###_#_#######_#_###_###_#_###_#####_#_#_#_###_#_#_#_#_#######_#_#####_#_#####_#_#####_#_###_###_#_#######_#_###_#_#_###_#####_#_#######_###_#_###_#######_#######_###########_#_#_#######_#####_#_#_#_#_###_###_#_###_###_#_#_###_###_###_###_#_#_#####_#_#_###_#####_#_###_#_#######_#####_#####_#_#
#_#_____#_#_#_#___#_#_#___#___#_#_______#_____#_#___#___#___#___#_____#_#___#_____#_#___#_______#_#_#_#_#_____#___________#___#_____#___#_#___#_#_#_#_#___#_____#___#___#_#_______#_______#_____#_#___#___#___#___#___#_#___#_#___#_____#_#_________#_#_#_#___#_________#___#_____#_____#_#_#___#_____#_____#_#___#_______#_#___#_____#_#___#_#___#_____#_#___#___#___#_____#_____#_____#_#_______#___#_#_____#_#
#_#######_#_#####_#_###_#_#_###_#_###########_#_#_#####_#_###_#_#_###_#_###_#_#####_#_#_#_#######_###_#_#_###_###_#######_###_#####_###_#_#_#_#_#_###_#_###_#######_#_###_#########_###########_#_#_#_###_#######_#_#####_###_###_#####_#_#_#########_#_#_#_###_#######_#_#_#####_#####_#_#_###_#_#####_#####_###_#######_#_###_#_#####_###_#_#_###_###_#_#####_#_#_#########_###_#_#####_#_#######_#_###_#####_#
#_____#___#_____#_#_#___#_#___#_#_______#_____#_#_#___#_#_____#___#___#___#_#_#___#___#___#_______#___#_#_#_#_#___#_____#___#_____#_#___#___#___#___#___#___#_____#_#___#___#_______#_________#___#_#_#___#_______#_______#_#___#___#___#_#_#___#_______#_#_____#___#_____#_#_____#_____#_#_#_____#___#___#_#___#_#___#_____#___#___#_____#_#_______#___#___#___#_#_#___#___#___#___#___#_#_#_____#_#_____#___#_#
###_#_#_#######_#_#_#_#######_#########_#######_#_#_#_#_###########_#####_#_#_#_#_#_#######_#######_###_#_#_#_#####_###_###_#####_#_#_#########_###_#####_###_###_#_###_###_#####_#####_#####_#######_#_###_#####_#########_#_#####_#_#_#_###_#_#########_#######_#_#########_#####_#####_#_#######_#_###_#_#_#_###_#_#######_#######_#####_#########_#####_###_#_#_#_#_#_#_###_#####_#_#_#_#_###_#_#######_###_#
#___#___#_______#_#___#_____#___#_____#_________#_#_#_______#___#___#___#_#_#_#_#_#_#_______#_____#_#_______#_#_____#___#_#___#_#___#_________#___#_____#___#_#_#___#_____#_____#_#_____#___#_#_______#_#___#_____#___________#___#_#_#_#_#___#_________________#_#_________#_#_________#_#_____#___#_#___#___#_#___#___#_____________#_______#___#_____#___#___#_#_#_#___#___#___#___#_#_#___#___#___#_#_____#_#
#_#_#####_#######_#_###_###_###_#_###_#_#########_#########_#_#_#_#_#_#_#_#_###_#_#_#_#######_###_#_#_#####_#_#_#####_###_###_#_#####_#######_###_#####_###_#_#_#####_###_#####_#_#_#####_#_#_#_#####_#_#_#_#######_###########_#_#_#_#_#_#_#################_###_#########_#_#######_#_#_#####_#####_#_###_###_#_#####_###############_#######_#_#######_###_#####_#_#######_#####_###_#_#_#########_#_#_###_#_#
#_#_#_____#_______#_#___#_#_____#___#_#_________#___#_____#___#_#_#_#_#_#_#___#_#_#___#___#___#_#___#_#___#_#_____#___#_#_______#___#_______#___#_#___#_____#___#_#_____#_____#_#___#___#_#___#___#___#_#_#_#_______#___________#___#_#_#_#_#_#___#_________#_#___#_____#___#_______#_#_#_#_____#_____#_#_____#_______#___#_______#___#_____#___#___#___#___#_____#_#___#___#_#___#_#_#_#_#_#_________#_#___#___#
#_###_#########_###_#_###_#_#######_#_###_#########_#_###_#####_#_###_#_#_###_#_#_#####_#_#_###_#######_#_#########_###_#_#######_#_###########_#_#_#_#########_#_#_###_#_#####_#####_###_#####_#_#####_#_###_#######_###############_###_#_#_#_#_#_#######_###_###_#_###_#########_#_#_#_#_#####_###_#_#############_###_#_#####_#_#_###_###_#####_#_#_#_#_#_###_#_###_#_###_#_#_#_#_#_###_#_#########_###_#####
#_#___#___#___#___#_#___#___#_______#_#_#_#_________#___#_#_____#_#___#_#_#_#___#___#___#___#_____#_____#_____#_____#_____#___#___#_____________#___#_________#_#_#_#___#_#_____#_#___________#_#_____#_#_________#___#_____#_____#___#___#_#_#_#_#_#___________#___#_#___#_______#_#_#_#_#_____#_#___#___#_________#_#___#_#_#___#_#_____#___#___#_#_#_#_#_#___#_#_#___#___#___#_#___#_____#_____#_#_____#_#___#
#_#_###_#_#_#_###_#_###_#_###_#######_#_#_#_###########_#_#######_#_###_#_#_#######_#_#######_#_#_#_#_#######_#_#####_#######_#_#########################_#####_#_#_#_###_#_#####_#_#########_#_#####_#_#########_#_###_###_#_#_#_###_#_###_#_#_#_#_#_###############_#_###_#####_#_###_#_#####_#_#######_#_#_#####_#_#_###_#_#_###_#######_###_#_#_#_#_#_#_#####_#_#_#####_#####_###_###########_#_#_###_#_#_#_#
#_#_____#___#_____#_#___#___#_#_____#_#___#_#___________#___#___#_#___#___#___#___#___#_______#_#_#_#_#_________#_____#_____#_#_#_________#_____#_______#_#___#_#___#___#_#_#___#_#_#___#___#_#_#___#_#_#___#_____#___#___#___#_#_____#_#___#___#_#_#___#___#_________#_____#___#___#___#_____#_#_______#___#_#___#_#_#___#_#_#___#___#___#_#_#_#_____#_#_#_____#___#___#___#___#_____#___#_______#_#_#_#_#_#_#_#
#_###################_#_#####_#_###_#_#####_#_#############_#_#_#_###_#####_#_###_#######_#####_###_#_###########_#######_#_#_#_###_###_###_###_#_#####_#_#_#_#_#######_###_#_#_#_#_###_#_#_#_###_#_#_#_#_#_#########_###_#####_#####_#_#_###_###_#_#_###_#_#_###_###########_#_#####_#######_#_#_###_#######_#_#_#_#####_#_#_###_#_###_#_#_#_#_#######_#######_#_#####_#_###_#_#########_#_#######_#_#_#_#_#_#_#
#_______#___________#_#_#___#___#_#_#_______#___#_________#___#___#___#_____#___#___#___#_#___#_____#_#_____#___#_#_____#_#_#_#___#___#_#___#_#___#___#___#_#_#_______#_#___#_#_#_____#_#_#_#_____#___#_#_#___#_____#_#___#___#_#___#_#___#_____#_#_#_#___#_#___#___#_____#___#_#___#_______#_#_#___#_#_______#_#_#_____#_#_#___#___#___#___#_#_#_#___#_____#___#_#_____#_____#___#_______#___#_____#___#___#_#_#
#_#####_#_#######_#_#_#_#_#_#####_#_###########_#_#######_#######_#_###_###_###_#_#_#_#_###_#_###_###_#_###_#_###_###_#_#_#_#_###_#####_#_###_#####_#######_#_#######_#_#_###_#_#####_#_#_#_#########_#_#_###_#_###_#_#_#####_#_#_#_#_###########_#_###_###_#######_#_#_###_#_#_#_#_#####_#_#_#_###_#_#_#######_#_#####_#_#_#_#_#####_#######_#_#_#_#_#####_#_###_#_#####_#####_#_#_#_#######_#_#_#_###_#####_#_#
#_#___#___#_____#_#___#_#_#_________#_________#_________#_#_______#_#_#_#_#___#___#___#___#_#___#_#___#_#___#_____#___#_#_#_____#___#___#_#_______#_____#___#___#___#_#_#_#___#_#___#___#_#___#_____#_#_#_#_#___#_#_#_#_______#___#_#_____________#_____#_#___#___#___#_____#_#___#_____#_#_#_#___#_#___#_______#_#_#___#_#_#_#___#_#_#_____#_____#_#_____#___#___#_#_____#_____#___#_#_______#_#_#___#_#_____#_#
#_###_#####_#_###_#######_#############_###_#_###########_#####_###_#_#_#_###_###########_#_###_###_###_#_###_#####_###_#_#########_#_###_#_#_###_###_#_#_#####_#_#_#_#_#_#_###_#_#_###_#_###_#_###_###_#_#_#####_#_#_#######_#####_#############_#######_###_#_#_#####_#####_#########_#_###_#_#_#_#####_#######_#_#_###_#_#_###_#_#_#_#####_#####_#####_#####_###_#_#####_#########_#_#########_#####_#####_#_#
#_____#_____#___#_______#___#___#_____#___#_#___#_________#___#_#_#_#_______#___#_____#___#_#_______#___#___#_______#_#_#___#_______#_#___#_#_#_#___#_#_#_____#___#_____#_____#_#_#___#_#___#___#_#___#_#_#_______#_#_______#_____#___#_________#_#___#_____#___#_#___#_#___#_______#_____#___#_#_#_#___#_#_#_____#___#___#_#_#___#_#_#_______#_____#___#_____#___#_#_#___#_________#_#_________#_____#_____#_#_#
#####_#########_#######_###_#_#_#_###_#####_###_#_#######_#_#_#_#_#_###########_###_#_#_###_#_#######_#####_#########_#_###_#_#######_#_#####_#_###_#_#######_#################_#_###_#####_#####_###_#_#_#_#######_#######_#####_###_#########_#_#_###_#_#######_#_#_#_#_#########_#######_#####_#_#_###_#_#_#####_#####_#_#_#_###_#_#_#######_#######_#####_###_#_###_#_#########_#_#########_#_###_#####_###_#
#___#_____#_____#___#_____#___#_#___#_______#___#_______#_#_#_#___#_____#_______#___#_#_____#_#_______#___#_#_____#___#_____#___#_____#_______#_______#_____#_____#___#_________#___#_____#_______#___#___#_#_____#_______#___#_#_#_#_________#_____#___#_______#___#_#_#_________#_____#___#___#_#_#___#___#_____#_______#___#_____#_#___#___#___#_________#___#_#_#___#___#_______#___#_____#_#_#_#_____#_#___#
###_#####_#_###_#_#_#_###_#####_###_#########_###_#####_###_#_#########_#_#######_###########_#_#######_#_#_#_###_#_#######_###_#_###_#_#####_#########_#_#_#_#####_###_###########_#####_###_#####_###_###_#_###_#_#####_###_#_#_#_#########_#######_#####_#_#######_#_#_#_###########_#_###_#_#_#_###_###_#####_#####_#####_#######_###_#_#_###_#_###_#######_#_#_#_#####_#_#####_#####_###_#_#_#_#####_#_#_#_#
#_____#___#___#_#_#___#___#___#_____#_____#___#___#___#_____#___#_____#_#_#___#___________#___#___#___#_#___#_#_#_#_________#_#_#___#___#___#_#___#_____#_#_#_#_____#___#_____#_____#___#_____#_____#_#___#___#___#_#___#___#_#_#___________#___#_____#___#_#_#_____#_#_#_#___________#___#___#_#_#_____#___#___#___#_#_#_____#_______#_#_#_#___#___#___#_____#_#_#_#___#_#___#___#_#___#___#___#_#___#_#_#___#_#
#_###_#_#####_###_#########_#_#######_#####_###_###_#_#####_###_#_###_#_#_#_#_###_#######_#_#####_#_###_#####_#_#_###_#######_#_#######_#_#_#_#_#_#_#####_###_#_#####_#_#_###_#_#####_#########_#####_###_###_#_###_#_#_#####_#_#########_#####_#_#####_#_###_#_###_#_#_###########_#####_#_###_#_#_#####_###_#_###_#_#_#_#####_#######_#_###_#######_###_###_#_###_###_#_###_#_#_###_#_###_#####_#_#_#_#_#####_#
#_#___#_#___#___#_________#_#_#_____________#_#_#_#_#_____#_#___#___#___#___#___#_#_______#_____#_#___________#_____#___#_____#_______#_#_#_#_#_#_#_#_#___#___#_____#_#_____#_____#_________#___#_______#___#_#___#_#_#_______#___________#_____#_#_____#_____#_#_#_#_#___#___#_____#___#___#_____#_#_____#___#___#_#___#___#_#___#_#___#___#_____#___#___#_#_#___#_#___#___#_#_#_____#_____#___#___#_#_#_______#
#_#####_#_#_###_#########_#_#_#_#############_#_#_#_#####_#_#_#####_#####_#####_###_#########_###_#_#######_#######_###_#####_#######_#_#_#_###_#_#_#_#_###_#######_#_###########_#######_#_#_###_###_#####_#####_#_#_#####################_###_#_#_#########_#_#_#_#_###_#_#_#_###_#_#_#############_#######_#####_#######_#_###_#_#_#_###_###_#_#_###_###_#_#_#_#_#_###_###_#_#############_#_#####_#_#########
#_____#___#_____#_______#___#_#_____#___#_________#___#___#_#___#___#_________#___#_________#_#___#_#_____#_#_____#___#___#_____#_____#_#_#_____#_#___#_#___#___#___#___#___#___#_________#_#___#_#_#_#___#_#_____#_#_____#_____________#___#___#_#___#_____#_#_#_#_#_#_____#___#___#_#_#_____#_______#_______#_____#_____#_____#_#___#___#_#___#_#_#___#_____#_#___#_#___#___#_#_____#_____#_#_____#_#_____#___#
#_###_#########_#_###########_#####_#_#_#####_#######_#_###_###_#_###_#########_#_###_###_#_#_#_#####_###_###_###_###_###_#_#####_#####_#_#######_###_#_#_###_#_#_#####_#_#_#_#_###########_###_#_#_#_#_#_#_#_###_#_#####_#_#_#########_#_###_###_###_###_#_#_#_#_#_#_#####_#########_#_#_###_#_#######_#####_#_###_#_###_#####_#_#####_#_#_#_#_###_#_###_#########_#_#_#_#_###_#####_#_#_###_#_###_#_#####_#_#_#
#_#_#_________#_#_#_________#_____#___#_____#_#_______#_#___#_#_#_#___#_______#_#___#___#_#_#_#_#_______#_____#___#___#_#___#_____#___#_#_______#_#___#___#___#_#_#___#_#_#___#___#_______#_#___#_#_#___#_____#___#_____#_#_#_#_____#_#_#___#___#_#_#___#_#_#_#_#___#___#___#_________#_#___#___#_____#_#_#___#_#___#___#_____#_#_____#_#_#___#_#___#_#___________#___#_#_____#_____#___#_____#_#___#_#___#___#_#
#_#_#######_#_#_#_#_#######_###_###########_###_#######_###_#_#_#_#####_#####_###_#_###_#_###_#_#_#############_###_###_#_###_#_###_#_#_#######_#_#_#########_#_#_#_#_#_#_#######_#_#####_#_#_###_#_###########_###_#####_###_#_###_#_#_###_#####_#_###_###_#_#_#_#####_#_###_#########_###_#####_#_#_#_#_#_###_###_###_#####_#_#####_#_#######_#_###_###########_#######_#######_#_###_#######_#_###_#_#_###_#_#
#_#_______#_#_#_#___#_____#___#_____#_____#_____#___#_#___#_#_#___#_____#___#___#_#_#_#_#_____#_______#_________#___#___#_#___#_#___#_#_#___#___#___#_________#_#_#_#___#_#___#___#_____#___#_#_____#_______#_#___#_______#___#___#___#___#_______#___#___#_#_#_#_______#_#___#_______#___#___#___#_#_#_#___#_#___#___#_#___#_#_____#_#_#_____#_#_#_#___________#_______#_#_____#_#___#___#_____#___#_#_#___#_#_#
#_###_###_#_###_#_###_#_#####_#####_#_#_#########_#_#_###_#_#_#####_#####_#_###_###_#_#_###############_#########_###_###_#_#####_###_#_###_#_#######_#####_#####_#_#####_#_#_#_#########_###_#####_#_#####_#_###_#########_###_#_#######_###_#######_###_#_#_#_#####_#####_#########_###_###_#####_#_#_#_###_###_#####_###_#_#_#####_#_#_#_###_#_#_###########_#######_#_#_###_#_###_#####_#########_#_###_###_#
#_____#_#_#_#___#___#_#_#___#_#_____#_#_____#_____#_#___#_#_______#_#_____#___#_____#_#_#___#_____#_____#___#___#___#_#___#_______#___#_____#_#_____#___#___#_____#_#___#_#_#_#___#_____#_#___#___#_#___#___#_____#_________#_#_#_______#___#___________#_#___#_#_____#___#_______#___#_____#_#_____#___#_#_____#_____#___#_#___#_____#_#_#___#_#_______#_#_____#___#___#_#_#___#_#___#___#___#___#___#_#_#___#_#
#######_#_#_#_#####_#_###_#_#_#_#####_###_###_###_#_#_###_#######_#_#_#######_#######_#_#_#_#_#####_#####_#_###_###_#_#_#########_#_#_#######_###_#_###_#_###_#####_#_###_#_#_###_#_###_#_#_###_#_#_###_#_###_#########_###_#_#_#######_###_###########_#_#####_#######_#_#######_#_#_#_#####_#_#########_#####_#####_###_#_#####_#####_#_###_#_#######_#_#_#####_###_###_#_###_###_###_#_###_#_#_#_###_#_###_#_#
#_______#_#_#_#___#___#___#___#_#_______#_______#_#___#___#___#_____#___#_#___#_____#_____#___#_____#_____#_____#___#_____#_____#_#_#_#_____#_____#___#_#___#___#_____#___#_#___#_#_#___#_#_#___#_#___#_#_#___________#_#___#___#_____#_____#___________#_____#_______#_#___#___#___#_#_____#_#_#___#___#___#_______#_#_______#___#_____#___#_#___#___#___#_____#___#_#___#___#___#___#_#_#___#_#___#___#___#___#
#_#_#_###_#_#_###_###_#_#####_#_#_###############_###_#_###_###_#######_#_#_###_###_###_#######_#####_###_#######_#######_#_###_###_###_###_#########_#_#######_#_#####_###_#_#_#_#_#_###_#_#_###_#####_#_###########_#_#_###_###_###############_###########_#######_#_###_#_#_#####_#####_#_#_#_#_###_###_#_#####_#_#_#######_#####_#####_#_###_###_###_#####_###_#_#_#####_###_###_#_#_#_###_#####_###_#####_#
#_#_#_#___#_#___#___#_#_____#_#___#___#___________#___#_#___#___#_#_____#___#___#_#___#_#_______#___#___#_________#_____#_#___#___#___#_#_#_______#___#_#_______#_#___#___#_#_#_#___#_#___#_____#___#___#_#_____#_____#_#_#_#_#_________#_______#___#_#_______#___#_#___#_#_#_#_____#_#_#___#___#_#_____#_____#___#_#___#_______#_____#___#_#___#_#_____#_#_#___#___#_#_#_____#_#___#_#_#_#_#___#_______#_#_____#
###_#_#_###_###_#_###_#####_#######_###_###########_###_#_#_#_###_#_#####_#####_#_###_###_#######_#####_###########_###_#_###_###_###_#_#_###_###_#_###_#_#########_#_###_#_#_#######_###_#########_#_###_#_###_#_#######_#_#_#######_#_###_###_###_#_#_#####_#_#_#_#####_#_#_#####_#_#_#_#######_#######_#####_#_#######_#######_#_###_#_#_#_###_#_#####_#_#_###_###_###_#####_###_#_#_#_#_#_###_#####_#_#_#####
#___#_#___#___#_#_____#___#_____#_____#_#___#_______#___#_#___#___#___#_________#_____#___#_____#_____#_______#_____#_#_#_#___#_#___#_#_#___#_#___#___#_#___________#_____#_#_#_____#___#_#_______#_____#_____#_#_#_____#_#_____#_____#___#_#_#___#_#_#___#_#___#_#___#___#_#_#_____#_#_#_________#_______#_____#___#_____#_#_____#_____#_#_#_#_____#___#___#___#___#_#___#_______#___#_#___#_#_#___#_#_#_____#_#
#_###_###_###_#_#####_###_###_###_###_#_#_#_#########_###_#######_###_###########_#####_###_#####_###_#######_#_#####_#_###_###_###_#_#_#_#_#_#######_#_#####################_#_#_#####_###_#####_#_###########_#_###_#_#_#####_#_#######_#_#_###_#_#_###_#_#####_#_#_###_#_#_#_#####_#_#######_###_#######_#######_#_#####_#_###########_#_#_#_#####_#_###_###_#_#_#_#_###_###########_#_###_#_###_#_#_#####_#_#
#___#___#_#_#_#_____#_______#_____#_#_#___#___________#___#___#_____#_#_____#___#_______#_#_____#___#_____#___#_#_#___#___#___#_____#_#___#_#_______#_____#___#_______________#_#_____#_____#___#_#_#_____#___#_#_____#_#_#___#___#_________#___#_#_____#_#_______#_#_____#___#_#___#_#_______#_#___#_____#_____#_#___#_____#_#___#_____#_#_#___#_____#___#_#___#_#_____#___#___#_______#_#___#___#_#_#___#___#_#
#_#_###_#_#_#_#####_#########_#####_#_#################_###_#_#_#_###_#_###_###_#########_#_###_#_#######_#_###_#_#_#_###_###_#_#####_#####_#######_#####_#_#_###_#####_#######_#####_#########_#_###_###_#_#_#_#######_#_#_#_###############_#_#_#_#####_#####_###_###########_#_#_#_###_###_#_#_###_#########_#_#####_###_#_#_###_###_#_#_#####_#####_###_#_#_###########_#_#_#_#####_#_#_###_###_#_###_#_###_#
#_#___#_#___#_____#_________#___#___#_______#_______#___#___#_#_#_#___#_#_#___#_______#_____#_#___#_____#_#_#___#___#___#___#_#_______#_____#___#___#_____#_#___#_#___#_#___#___#___#_______#___#___#_#_#___#_#_____#___#_#_#_______________#_#_#_#_#___#_____#_#___#_________#_#_#_#_____#___#___#___#_______#_#_#___#_#___#___#___#___#___#_____#_#___#___#_#_#___#_____#___#___#___#_#_#_#_#_____#_#___#_#___#
#_###_#####_#_#############_###_#_#_###_#_###_#_#####_###_#_#_#_#_#_###_#_###_#_#####_#_#####_#####_###_#_#_#_###_###_#####_#_###_#####_#####_#_#_###_#####_###_#_#_#_###_#_#_###_#####_###_#_#####_#_#_#####_#####_#_###_#_###############_#_###_#_#_#_#_###_#_###_#_#####_#_#_#_#_#######_#######_#_#_#####_#_#_#_#_#_#_#####_#_###_#######_#####_#_###_###_###_#_#_###_#_#######_#_#_#_#_#_#_#####_#_###_###_#
#___#_______#_______#_____#___#___#___#_#_#___#_#___#_#___#_#___#_#___#_#___#_#_#___#_#_#_________#___#___#___#___#___#_____#___#_#___#_______#_#___#_______#___#___#___#_#_______#_____#_#_#_____#___#_____#_______#___#_#_#___#_________#_#___#_#_#_#_#_#___#_____#_____#_#_#___#_______#_#_______#_#___#___#_#___#___#_______#_#_#_____#___#___#_#___#_#_#___#_#___#_#_#_#_____#_#___#_#_#___#_____#_#_#_____#
###_###############_#_###_###_#######_#_###_###_#_#_#_#_###_#####_###_#_#_#_#_#_#_#_###_#######_#_###_###########_#_#_#_#####_#_###_#_#########_###_#########_###_#####_#_#########_#####_#_#####_#####_#_#############_#_#_#_###_###_###_#_###_#_###_#_#_#_#######_#####_#_#########_###_#_#_###########_###_#_###############_#_#_#####_#_###_#_#_###_#_#_###_#_#####_#_#_#####_#_#######_#_###_#####_#_#####_#
#_#_____#___#_____#_#___#___#___#_#___#___#___#___#___#_#___#___#___#___#_#_#_#___#___#_________#___#_#_________#_#_#_#_#___#_#_____#_#___________#_________#_#___#___#___#___#_____#_____#_______#___#_#___________#___#___#_______#_#___#_#___#___#_#_#_#_#_________#___#_________#_#_#_#_#___#___#___#___#___#_____#_______#_#_______#___#___#_____#___#___#___#_____#_#_______#_________#_#___#_____#_____#_#
#_#####_###_#_###_#_###_###_###_#_#_#####_###_###########_###_#####_#######_#_#######_###########_###_#_###_###_###_#_#_###_#_#######_#_#############_#######_#_###_#_#######_#_#####_###_#######_#_#_#_#####_###_###_###_###_#######_#_###_#_#####_#_#_###_#_#######_#_#####_#####_#_#_#_#_###_#_#_#_#_###_#####_###_#_#####_#_#######_#####_###############_#####_#_###_#######_###########_#_###_#####_#####_#
#_____#_____#_#_______#_#_#_#___#___#___#_#___#_____#_____#___#_____#_____#_#___#___________#_____#___#_#_#___#_____#_#___#_#_#_#_____#_____#_#_____#_#_______#_____#_________#___#___#___#_____#_#_#_#___#_#___#_#___#___#___#_______#_#___#_#_____#_#_____#_#___#___#_#___#_#___#_#_#___#_#___#_#_#_#___#_#_____#___#___#___#_#___#___#_____#_______________#_#___#_#___#_#_____#_________#_#___#_#_____#_____#
#_###_#####_#_#########_#_#_#_###_###_#_#_#_###_###_#_#_###_###_#####_###_#_###_#_#########_#_#####_#_#_#_###_#######_###_#_#_#_#_#########_#_#_###_#_#_#########_###########_###_#####_#_#####_#_#_#####_#_###_###_###_#######_#######_#_###_#_#####_#########_#_#####_#_#_#_#_#_#_#_#_###_#_#_#_#_#_###_#_#_#####_#_###_#_#####_#_#_###_###_#_#########_###_#_#_#_###_###_#_#####_###_###_#_###_#_#_#_###_#####
#_#___#_____#_________#___#___#___#___#_#___#_____#_#_#_#___#___#___#_#_______#_#_______#___#___#___#_#_#___#_#_______#___#_____#_________#_#_#_#_____#_________#_#_________#___#___#___#_#_____#_#_______#___#_____#___________#_______#_#___#_____#___________#_#_____#_#_#_#_#___#_#___#___#_#_#___#_#___#_____#_#___#_#_______#___#_#___#___#_____#___#___#___#___#___#___#_____#_#___#_#_____#_#_#_____#___#
#_#####_#############_###_#####_###_###_#####_###_###_#_#_#_#_###_#_#_#_#####_#_#_#####_#_###_###_#####_#_#_#_###_#####_###_#######_###_###_#_#_###############_#_#_#######_#######_#_###_#_#####_#####_#####_###################_#######_#_#_#####_###_#########_#_#######_#_#_#####_###_#####_#_#####_###_#####_#_###_#_#############_###_#######_###_#####_#######_###_###_#####_#_###_#######_#_###_#######_#
#_____#_____________#___#_____#_____#_#_#_____#_#_____#_#_#___#___#___#_#___#_#_#___#_#_#___#_#___#_____#_#_#___#_#_____#_____#___#_#___#_____#___________#_____#_#_#_____#_________#_#_____#___#_____#_______#_____________#_____#_______#_#_#___#___#_________#___________#_#_#___#_#___#_#___#_____#_____#_____#_#_#_#___#_______#_______#_____#___#_#___#___________#___#_____#_____#_________#___#_________#
#####_###_###########_#_#####_#_#####_#_#_#####_#######_#_#####_#######_#_#_#_#_###_#_#_#_###_#_#_#_#######_###_###_#####_#####_#_###_###_#######_#####_#_#_#####_#_###_#############_#######_#_#####_#######_#_#_#########_#####_#_#######_#_###_###_###############_#######_#_#_###_###_#_#_#######_#######_#####_#_#_###_#_#_###_#####_###_###_###_#_#_#_#_#########_###_#####_#_#######_#########_#########_#
#_______#_#_________#_#___#___#_#___#___#_______#___#___#_#_____#_______#_#_#_#___#___#_#_#___#_#_#___#_____#___#___#___#___#___#___#_______#___#___#___#_#_#_#___#___#___#_____#_____________#_______#_______#_#_#___#_____#___#_#_#_______#_______#_______________#_#_______#_#___#___#___#_#_____#___#_____#___#_#_#_#_#_#_#___#_____#_____#_#___#_#___#_#_#_______#___#_#_____#_#_____#_____#___#_#_____#___#
#_#####_#_#_#######_###_#_#_#####_#_#_###########_#_#_###_#_###_#_#######_#_#####_###_#_###_###_#_###_###_###_#_#_###_#_#####_#####_#_#######_#_#####_#_###_#_#_#####_#_#_#_#_#_#_#######################_#####_###_#_#_#####_#_###_#####_#_#######_###############_#_#_#######_###_###_#####_###_#_###_#_#######_#_#_#_#_#_#####_#####_#######_###_#_#####_###_#####_#####_#_#######_###_#######_#_#_#_###_#####
#_#___#_#_#_#_______#___#_#_______#_#_#___________#_#_#_#_#_#___#_#_______#_______#_#_#_______#_#___#___#_#___#_#_#___#___#___#___#_#_#_____#_#_______#_#___#_____#___#_#___#_#_#_#_________#___________#_______#___#_#_______#_____#___#_#_____#___________#_____#_#_#_#_____#___#___#_________#_#___#_____#___#_#_#_____#_#_____#___#_#_____#_____#_____#_#___#___#_____#_#_________#_#_________#_#___#_#_____#
#_#_###_#_#_#######_#_###_#########_#_###_#########_#_#_#_###_#####_###############_#_#########_###_###_#_#_###_#_#_#_###_#_###_###_#_#_###_#_#########_#_#########_###_#####_###_#_###_###_#_#######_#_#########_###_#######_#######_#_###_###_###########_#_###_#_###_###_#_###_###_#########_###_#######_#_#_#_#_#_#####_#_#####_###_#_###_#_#####_###_#_#_###_#_#####_#_###########_###########_#_###_#####_#
#_#_____#_#_______#___#___#_____#___#_____#_______#___#___#___#_____#_____#_____#_________#_____#_____#___#_#___#_#_#___#_#_#_______#_#_#___#___#_______#_#_________#_#_#___#_____#_#___#_#_#_____#___#_____#_____#_#_______#_#_______#___#___#___#___#___#_#_#___#_#___#___#_#___#___#_____#_____#___#___#___#___#_#_#_____#_#___#_____#_#_#_#___#___#___#_#_#___#_#___#_#___#___________#_______#_#_#_____#___#
#_#######_#######_#####_###_###_#_#######_#_#####_#####_###_###_#####_#####_###_#_#######_#_#_#######_###_#_#####_#####_#_#_#_#######_#_#_#####_#_#######_#_#########_#_###_#########_###_#_#######_#######_#_#####_#######_#_#_#########_#_#####_#_#_###_#_###_###_#_###_###_#_###_###_###_#####_###_#_#_#########_#_#_#####_#_#_#_#####_#_#_###_#####_###_#_#_###_#_#_#_###_#######_#####_#####_#_#_#####_#_#_#
#_#_____________#_#___#_#___#___#___#_____#___#___#_____#_____#_____#_#___#_#_#_____#___#_#_#_#_____#___#_#_______#___#_#___#_#_____#_#_#_#_____#_#_____#___#_____#_#___#___#_______#___#_____#___#___#___#___#_____#_______#_#_#___#_____#_#___#_#_#___#_______#___#_#___#___#_____#___#_#___#_____#_#_#_________#_#_#_#_____#_#_#_#_____#___#_____#___#___#_#_#___#_#_____#___#___#_____#_#_____#_#_______#_#_#
#_#_###########_#_###_#_#_###_#####_#########_#_###_#####_###_#####_#_#_#_#_#_#######_#_###_###_###_#####_#########_#_#_#####_###_#_#_#_#_#_###_###_###_#######_#_#_#_###_#_###_###_###_#####_#_#_###_#_#_#########_#_#########_###_#_#####_#_#_#_#_###_#########_###_###_#_#####_###_###_###_#####_#_#_#########_#_#_#_#_#####_#_#_#####_#_#######_#_#_#_###_#_#_###_#_#######_#_#_###_#_#_#_###_#_#########_###
#_____#_____#___#___#_#___#_#_____#_#_________#_#___#_____#___#_____#___#_#_#_____#___#_____#___#_#_______#_________#_#_#_____#___#_#_#_#___#___#_____#_________#___#_#___#___#_#_____#_#___#___#___#___#___#_____#_#___________#___#_#___#_#_#___#___#_________#___#___#_#_#_____#___#___________#___#_#_______#___#_#_#___#___#_#___#___#_____#___#_#_#_#___#_#_#___#_#_____#___#___#_#___#_#_#_#_#_______#___#
#_#####_###_#_#####_#_#####_#####_#_#_#########_###_#####_#_#_#_#####_###_#_#_###_#_#######_#_###_#########_#########_#_#_#####_###_###_#_#######_#########_#_#####_#_#_#####_#_#####_#_#_#_#######_#_#####_#_###_#_#############_#_#_#_###_#_#######_#########_###_###_#_#_#_#####_#############_#_###_#_#_###_#####_#_###_#_###_###_#_#######_###_#_#_#_#_###_###_#####_###_#######_#_#####_#_#_#_#_#####_###_#
#_#_____#_#_#_#_____#_________#___#_#_#_______#___#_#___#_#_#_#___#_____#_____#_#_#_#_____#_#_____________#_#_______#___#_______#_#___#_#_#_______#_______#_#_____#_#___#___#_#___#_#_#___#___#_____#_#___#_____#_#_______#_______#_#_#_____#_#_____#_#___#_____#_#_____#_#_#___#_#_______#_____#_#_#___#_#_#_#_#___#_#_____#_#_____#_#___#___#___#___#_#_______#___#_____#_#___#___#_#_#___#_#___#___#_#___#___#
#_#_#####_#_###_#######_#_#####_###_#_###_###_###_#_#_#_###_#_###_#_###########_#_#_###_#_#_#############_#_#_#####_#####_#######_###_#_###_#######_#####_#####_#_#########_#_###_#_#_#_#####_#_#######_#_#######_#_#####_#####_###_#_#######_#_###_#_#_#_#_###_#_#######_#####_#_#######_#_###_#_###_###_#_#_#_#_#_#_#######_#_###_#_###_#_#_###_#############_#_###_#####_###_#_#_#_#_#_###_#########_#_###_###
#_#_#_#___#___#_______#_#_____#___#___#___#_____#_#___#___#_#_#___#_#_____#_#_____#___#_#_#_#___________#_#_#_____#_____#_#___#_______#_#___#_____#_#___#_#___#_#___________#___#_#___#___#___#_________#_#_______#_#_#___#___#_#___#_____#___#_#___#___#_#___#_____#_________#_____#___#___#_#_#_____#___#_#___#_#_#_#___#___#___#_#_____#_#___#_________#_#___#___#_____#___#_#_#___#_#___#_____#_____#___#___#
#_#_#_#_#_###_#_#####_###_###_###_#####_#########_#######_#_#_#_#####_###_#_#_#######_#_###_#_#####_#####_#_#####_#####_#_#_#_#######_#_#_#####_###_#_#_#_#_#_#_###_#######_###_#_#_#######_#############_#_#######_#_#_###_#_#_#_#######_#_###_#_#####_#####_#####_#_#######_#####_###_#####_#_#####_#####_#####_#_#_#_#_#_#####_#_#######_###_#########_#_#_#####_#####_#_###_#_#####_###_#####_#_#_#####_###_#
#_#_#___#_____#_____#___#___#___#_____#_____#_____#___#_____#_#_______#_#_#_______#_#_#_______#_____#_____#_____#___#___#_#_#_______#_#_#___#___#___#_#_#___#_#___#_#_____#___#___#_#_______#_#_________#___#_______#___#___#___#_______#_#___#_#_______#_____#_____#_#___________#___#_______#___#___#___#_______#_#_#_#___#___#_#_____#_#_#_#_________#_#_____#_______#_#_____#_____#_________#_#_#_____#_____#
#_#_###_#############_#_###_###_#####_#####_#_#####_#_#########_#######_#_#######_#_#_###############_#####_#######_#_#####_#######_#_#_###_#_#_#_###_#_#####_#####_#_#_#####_#####_###_#####_#_###_###_#####_#_#####_###_#_###########_#_###_#_#########_#####_#######_###_#########_#_#_###_###_#_###_#_#######_#_#_#_#####_#_#_#####_#_#_#_#_#####_###_#_#####_#######_#_#####_###_#######_###_#_#####_#####_#
#_#___#_______#_____#_#_#___#_____#_________#_______#_________#_________#___#___#_#_#_#___#___#_______#_#_________#_#___#_______#___#_#___#_#_#_#___#_#_#_____#_____#_#_____#_#___#_#___#_____#___#_#_#_#___#_#_#_________#_#_________#_#_#___#_____#_#___#_____________#___#_______#_#_#_#___#___#_#___#___#___#_#___#___#___#_#_#_#_____#_#_#_____#_#___#_#___#_#_____#_#___#_____#_#_#___#_#___#_____#_#___#_#
#_###_#######_#_###_###_#_###_#####_#######_#################_#########_###_#_#_#_#_#_#_#_#_#_#_#######_#_#######_#_###_#_#######_###_#_###_#_#_###_#_###_#####_#####_###_###_###_#_#_###_###_###_#_#_#_#_#_#_#_#############_#######_###_#_#######_#_#_#_#######_###########_###_#_#_###_#_###_#_###_#####_#_#_#_#######_#_#_###_#_#_#####_#_#####_###_#####_#_#_#_###_#_###_#######_#_#_#_#_#_#####_###_#_#_#_#
#___#_______#___#_______#_#_#_______#_____#___#_____#_______#_#_______#___#_#_#_____#___#_#_#___#_______#_#_____#_#_#_#___#_#_____#___#_______#_#___#___#_____#___#___#_#_______#_#_#_#___#_#___#_#_#_#_#_#___#___#___#_____#_#_____#_____#_#_____#___#_#_#_____#_#___________#___#_#___#_#_#___#_#___#_____#_#_#_______#___#_#___#_____#___#_#___#___________#___#_#___#___#_______#_#___#___#_______#___#_#___#
#_#####_###_#############_#_#########_###_###_#_###_#_#_#####_#_###_#####_#_#########_###_#_#####_#_###_#_#_###_#_#_#_#####_#_#####_#############_###_#_#####_###_#_###_#######_#_#_#_#_###_#_###_#_#_#_#####_###_#_#_#_###_#_#####_#######_#_###_###_#_###_###_###_###########_#######_#_###_#_###_###_#####_#_#######_###_###_###_#####_###_#_#_###############_#_#_#####_#######_#_#_#############_#_###_#####
#_#___#___#_#_______#_#___#_________#_#_#___#_#___#_#_#_#_____#___#___#___#___#_____#_#___#_#___#_#___#_#_#___#_#_#_#_______#_#___#_#_____________#___#_____#___#_#_______#___#___#___#_____#_#___#_#___#___#___#___#_#_#___#_#___#_______#___#_#_____#_______#_____#_____#___#___#_____#___#_#_#___#_#_______#_______#_#___#___#___#_#___#_____#_______#___#___#_#_#_#_____#___#___#_#_#___________#_#_#___#___#
###_#_#####_#_#####_#_#_#########_###_#_###_#_###_#_###_#_#######_###_#_#####_#_###_#_#_#_#_#_#_#####_###_###_#_#_#_#_#_#####_#_#_#_#_#_#############_###_#####_#_#########_#_#############_#_#_###_#_###_#_#########_#_#_###_#_#_#_#######_###_#############_#########_#_#_#####_#_#######_#_###_###_###############_###_###_#####_#_#_#####_#######_#_#_#_#_#_###_#_###_#####_#_###_#_#_#########_###_#_###_###
#___#_______#___#_#_#_______#_____#___#___#_#_____#_____#___#___#___#_#___#_#___#___#_#_#_#_#_#_____#___#_____#_#_#_#_#_#_____#_#_#_#_#_#___________#_#_#_______#_____#___#_#_#_______#___#_#_#___#_#_____#_#_____#___#_#___#___#_#_#_______#___#_________#___#_________#_#_____#_#___#_____#_#___#___________#___#_#_#_____#_#___#_#_#_#___#_#_______#_#_#_#_#_____#_____#_____#_#___#_#_____#_____#___#_#_____#
#_#############_#_#_#######_#_###_#_###_#_#_#####_#########_#_#_###_#_###_#_#####_#####_###_#_#####_###_#_#####_#_#_###_#_#####_#_#_#_#_#_#########_#_#_#############_#_#_#_#_#_###_#_###_#_#_#_###_#######_#_###_#_#_#_###_#####_#_#_#######_#_#_#######_#_#########_###_###_###_###_#_#####_#_###_#######_#_#_#_#_#_#_#####_#_#_#_#_#_#_#_###_###_#####_#_#_#############_###_#_###_###_#####_#####_###_#_###_#
#_#_____#_____#___#_#___#_#___#___#_#_#_#_#_#_____#___#_____#_#_#_#_#___#___#_____#___#_______#___#___#___#___#_#_#_____#_____#_#_#___#_#_________#_#_#_______________#_#_#_#___#_#_#_____#_#_#_#___#___#_#___#___#_#_#___#_______#_______#___#_#_______#_#_#_________#___#___#_____#_#_#_____#_#___#_____#_#___#_#_#___#_#___#_#_#___#___#___#___#_#_____#___#_#_______#_____#_#___#_#___#_____#___#_#___#___#_#
#_#_###_#_###_###_#_#_#_#_#####_###_#_#_#_#_#######_#_#_#####_#_#_#_#_###_###_#####_#_#####_###_#_###_#####_#_###_#####_#####_###_###_#######_#####_#_#######_#######_#_#_#_#####_#_###_###_#_#_#_###_#_#_#####_###_#_###_###############_#####_#######_#_#_###_#######_###_###_#####_#_#_###_#_#_###_###_#_#####_#_#####_#_###_#_#_#########_###_#_#_#########_#_###_###_###_#####_#_#_###_#####_#_#_#_#######_#
#_#___#_#___#___#_#___#_#_____#___#_#___#_#_____#___#_#_______#_#___#_____#___#___#_#_____#_#___#_#_#_______#_____#_____#___#___#___#_#_______#_____#_______#_____#___#_#_#_#_____#_#___#___#___#_#___#_______#_#___#___#___#_#_________#_#___________#_#_#_____#_____#___#___#_______#_#_#___#_#___#___#_#_#___#_#_______#_____#_#_#_____________#_#_____#___#___#___#___#_#_____#_#_#_#___#_____#___#_______#_#
#_###_#_###_###_#_#####_#_###_#_###_#_###_#####_#_###_#########_#_#########_###_#_#_#####_###_###_#_#############_#_#####_#_###_#_#_#_#_#######_###########_###_#_#####_#_#_###_###_#_###_#######_#_#_#######_#_#_#########_#_#_#_#####_#_#_#####_#####_#_#######_#_#_###_###_#########_#_#####_#######_#_#_#_###_#####_#_#######_#_#_###########_#######_###_#_###_#_#_###_###_###_#_#_#_#####_#############_#_#
#_#___#___#___#_#_____#_#_#_#___#___#___#_____#_#_#_#___#_______#_#___#_____#___#___#___#_____#___#_____#_________#_____#_#_#___#_#___#___#___#_#_________#___#_#_#___#_#_#_____#___#_#___#_#_____#_#_______#_#___#_________#___#_#___#_#___#_____#_____#_#_____#_#_#___#_____#_______#_#_____#_#_______#_#_#___#_______#_#_______#_#___#_______#_#_____#___#___#___#_#_____#_#_____#___#_#_____#___________#___#
#_#_#####_###_#_#####_#_#_#_#####_#######_###_#_#_#_###_#_#######_###_#_#_###_#######_#_#######_###_#_###_#############_#_#_#_###_#######_###_#_#_###_#######_###_#_#_#_#_#####_#_#####_###_#_#####_#######_#####_#_#############_###_#_###########_#####_#_#####_#_###_#####_###_###_#_#####_#_#_#######_#_#_#_#########_#_#######_###_#_#####_###_###_#_#_#_###_#########_#_#########_#_#_#_#_#_#######_#####_#
#_#_#___#_____#_____#_#_#___#_____#_______#_#_#___#___#_#_______#_____#_#_#___#_____#_#_______#_#___#_____#___#_#_____#_#_#_#_#_#___#_____#___#_#_#_#_______#___#___#___#_#___#_#___________#_#___#_#_____#_______#_#_______#_____#___#_____#_______#_#___#_______#___#_#___#___#_#_#_#___#___#_#_#_#___#___#_#_#_______#_#_#_____#_#___#_#_________#_#_#_#_#___#_________________#___#_#_#_#_#_#_#_____#___#___#
#_#_###_#########_###_#_#####_#####_#######_#_#####_###_#######_###_###_#_#_#####_#_#_#######_#_###_#######_#_#_#_###_#_#_#_#_#_#_###_#####_#_#_#_#_#######_###_#_#######_#_#_#_###########_#_#_#_#_###_#####_#####_###_###_#_#####_#_#####_#_#######_#_###_#########_#_#_#_###_#_#_#_#_###_###_#_#_#_#_#####_#_#_###_###_#_#_###_###_###_#_#########_#_#_#_#####################_#_#_###_#_#_###_#_#######_#_###
#_#___#_________#_____#_#___#_#_____#_______#___#_____#_______#___#_#___#_#_______#_#_#_____#_#___#_________#___#___#_#_#_#_#_#_#_#___#_____#_#_#_____#___#___#_#_______#___#_#___#___#___#_#_#_#_______#___#_#___#___#_#_#_#___#___#_____#___#_#_____#_#___#_#_____#_#_#_#___#___#_____#___#___#_#_#_#_____#_#_#_#___#___#_#_#___#___#___#_#___#_____#_#_#___#_______#_________#___#_____#_#_____#_______#_#___#
#_###_#########_#######_#_#_#_#####_###_#_#_###_#_###_###_#######_###_###_#########_#_#_###_#####_#############_###_#_#_#_#_#_#_#_#_###_#######_#######_#_###_#_#####_#######_###_###_#_#_###_###########_#_#_###_###_#_#_#_###_#_#######_#####_#_#_###_#_###_#_#_###_#_#_#_#######_#####_###_###_#_#_###_###_#_#_#_###_###_#_#_###_###_#####_#_#_#####_#####_#_#####_#_#######_###########_#############_#_###_#
#___#___#_________#___#___#_#_____#___#_#_#_#___#_#_#___#___#_____#___#___#_#_______#_#_#_#_#_____#_______#___#___#_#___#_#___#___#_#_____#_____#___#___#_____#_____#_#_____#_________#_#___#___#___#_____#_#_#___#_#_#_#_______#_#_____#___#_____#_____#_#_____#_#___#_#_#___#_______#___#___#___#_____#___#_#_#_#_#___#_#___#_____#___#___#_#_#_____________#_____#_#___#___#_#_#_______#_______#_______#___#_#
#_#_###_#_#######_#_#_###_#_#####_###_#_#_###_###_#_###_###_#_#####_#####_#_#_#######_#_#_#_#_###_#_#####_#_#_#_###_#####_#####_###_#_###_#_###_#_#_#_#######_#####_#_#_###_#########_#_###_#_#_#_#_#_#####_#_#_#_#_#_#_#########_#_###_#####_###########_#_#####_#_#####_###_#_#######_###_###_#########_#_#_#_#_#_#_###_#######_###_###_#_#_#_#############_#####_#_###_###_#_#_#_###_#########_#_#####_###_#_#
#_#___#_#_#___#___#_#___#_#___#_#_____#_#___#_#_______#_#___#_#_____#___#___#_#_______#_#_#___#_#_#___#_#___#_#_#_______#___#___#_#_#_#___#___#_#_#_#_#___#___#_#___#_#_#_#_____#_____#___#_#_#___#___#_____#___#___#_#_____#_____#_#_#_#___#___#_____#_#_#_#___#_#_________#_____#_____#___#_______#___#_#___#___#___#___#_____#___#_#___#___#_____________#_#___#_#___#_#___#_#_____#_________#_#_#_______#___#
#_#####_#_#_#_#####_###_###_#_#_#######_###_#_#########_#_###_#####_#_#_###_#_#_#####_#_#_#####_#_###_#_#####_###_#########_#_###_#_###_#_###_###_#_#_###_#_###_#_#####_#_#####_#_#####_#_#_#####_#####_###_#######_#_#####_#_#_###_#_#_#_#_###_#_###_#_#_###_#_#_###############_#_#_###_#########_#_#_#############_###_#_###_#####_#_###################_###_#_#_###_#_#_###_#####_###_#####_#_#_#######_#####
#_____#_#_#_#_______#___#___#_________#___#_#_____#_____#___#_____#_#_#___#_#_#___#___#_#_#_____#___#_______#_____#_#_____#_#_#___#___#_#_#_#_____#_#___#___#___#_#_____#___#___#_#___#_#_#_#___#___#___#___#___#_#_#_____#_#_#_______#___#___#___#_#_#_#___#_#___________#_____#_#_#_#___#_______#_#_#_________#___#_____#___#_#_____#_#_____#___________#_____#___#_#___#___#___#_____#_____#_#_#_____#_#___#_#
#_###_#_#_#_###########_#_###########_###_#_#####_#_###_###_#####_#_#_###_#_#_###_#_###_#_#_###_#_#########_#######_#_###_#_#_#_#_###_#_#_#_#######_#_#_#_###_#_#_#_#####_#_#_#####_#_###_#_#_#_###_#_###_###_#_#_#_#####_#_#_###############_#####_#_#_###_#_###########_#_###_###_#_#_###_#####_#_#_#########_#_#_#########_#_#_###_#_#####_#_#####_#_#############_#####_#_###_#####_#####_###_#_###_#_###_#_#
#_#_#___#_#_____#_____#___#_______#___#___#_____#___#___#_____#_#_#___#___#_#_#___#___#_#_____#___#___#___#_#_______#___#___#___#___#___#_#___#_____#_#_#_#___#_#___#_____#_#_____#_#_____#_#_#_#___#_#_#_#___#_#_______#___#_#_______#_____#_______#_#___#_#___#_________#_#_#_#___#_#_#___#___#_#_#_#_______#_#_#_______#___#___#_#_#___#___#_#___#_#_#_______________#___#___#_____#_#___#_____#___#_#_____#_#
#_#_#####_#####_#_###_#####_#######_###_#######_#####_###_###_#_#_#####_#####_#_#######_#_#########_#_#_#_#_#####_#_###_###########_#####_###_#_#####_#_#_#_###_###########_#####_#_#######_#_#_#####_#_#_#_###_#############_#_#####_#_###_###_###_#_#_###_###_#_#########_#_#_#_#####_###_#_###_#_#_#_#####_#_#_#######_#_#######_#_###_#_###_#_#_#_#_#_#########_#####_###_#_#####_###_#_###_#######_#_#####_#
#_#___#_______#_____#_#___________#_#___#_______#___#_#_#___#_#_#_____#_______#_#_____#_#___#_____#_#___#_________#___#___#_____________#___#_#_#_____#_#___#_#_____________#_______#_____#___#_______#___#_#___#_____________#_#___#___#___#___#_#_#_#___#_____#_____#_____#_#___#_____#___#_______#_#_#___#_#___#_____#_#___#_________#_#_______#_#_#_#_#_______#_#_____#___#_#_____#___#___#_________#_______#
#_#_#_#_#####_#######_#_#########_#_#_###_#####_#_###_#_###_#_#_#####_#_#######_#_###_#_#_###_###_#_#################_###_#_###############_#_#_#######_#####_#########_#####_#######_#_###############_###_#_#_#_#############_###_#####_###_###_#_#_###_###########_#_#####_###_#_#####_#####_#####_###_#_#_#####_###_#_###_#########_#_###########_###_#_###_#_#_#_#####_###_#_###_#_#####_#################_#
#___#___#___#_#___#___#_____#___#___#_#_#___#___#_#___#_#___#_______#_#___#_____#_#_____#_#___#_#_______#___#___#_____#___#_#_____#___________#_#_______#_________#_____#_____#___#___#_______#___________#_#_#_#___#___#_____#___#_____#_____#_____#___#_______#___#_#_#___#_____#_____#_____#_#___#_#___#_____#_____#_#___#_________#_#___#_______#___#_#_#_#_#_#_______#___#_#_#___#___#___#_______#_______#_#
#########_#_#_#_###_#######_#_#######_#_###_###_#_#_###_#_#########_#_###_#_###_#_###_#####_###_#########_#_###_#_#####_###_#####_#_###########_#_###########_###_#_#####_#####_#_#########_#_#_###########_#_#####_#_#_#_#_#####_#_#######_###########_#_#_#####_#_#_#_#_#_###########_###_#_###_#_#_#_#######_#_###_#_###_#########_#####_#_#####_#_#_#_#_#_#_#############_###_#_#####_#_#######_###_###_###_#
#_____#___#_#_#_#___#_#___#_#_#___#___#___#___#___#_____#_______#___#___#___#___#___#_#_____#_______#___#_#___#_____#___#___#_____#_______#_____#_#_____#___#___#_____#___#_____#_#_______#_#_#_#_________#_#_____#___#_#_#_#___#_#_#___#___#___#___#___#_#_#_____#___#___#___________#___#_#_____#___#_#_____#_#_#_#_#_#_#_#_______#_____#___#___#_#_#_#_#___#_____#_#_____#_#___#___#___#_______#_____#___#___#
###_#_#_###_#_#_#_###_#_#_#_#_#_#_#_###_#####_#########_#######_#######_#_#########_#_#_#######_#####_#_#_###_###_###_#####_#_###########_#_#####_#_###_#_#_###########_###_#####_#_#####_#_#_#_#_#######_#_#####_#####_#_###_#_#_#_#_#_#_###_#_#_#_#_###_#_#_#####################_#####_#_###########_#_###_#_#_#_#_#_#_#_#_#_###_###_#######_#_#_#_#_#_#_#####_#_#_#_###_#_#_#####_#_#########_#####_#_###_###
#___#_#___#___#_#_#___#_#___#___#_#_#___#___#_____#_____#_____#_______#_#_#_______#_#_#_#_______#_____#___#_#___#_#___#___#___#_________#_#___#_____#_#_#_#_____#_______#___#_____#___#_#_#_#___#_____#_#___#___#___#___#___#_#___#___#___#___#___#_#_#___#_#_____#_______________#_______#___#_____#___#___#___#___#_#___#_#_#_#_#___#_________#_#_#_#_#_#_#___#_#_#_#_#___#_#___#_#_#_#_____#_#_#___#_#_____#_#
#_###_###_#####_#_#_#_#_#########_#_#_###_#_#####_#_#####_#_#######_###_#_#_#####_#_###_#####_#_#_#########_###_#_#_###_#_###_#_#_#######_###_#####_#_#_#_#####_#_#######_###_#_#####_#_#_#_#########_#_#####_#####_#_#####_#_#############_#######_#_###_#_#####_#_#########_###_###########_#_###_#_###_#######_###_###_#_#_#_#_###_#######_###_#_###_#_###_#_#_#_#_#_#_###_###_#_#_#_#_###_#_#_#_#_###_#####_#
#___#___#_#_____#_#_#_#_____#_____#_#___#_#_____#_#___#___#_______#_____#_#_#_#___#___________#_#_#_______#___#_#_#_____#_#___#_#_________#_#_____#___#___#___#_#_#___#_____#_#_#_____#_#_#_#_________#_______#_____#_#___#_#_____#_______#___#___#___#___#_#_____#___#_______#_#_#_#_____#_#_#_#___#___#_#_______#___#___#___#_#_____#___#___#___#___#_#_____#_#_#_#_#_#_______#_#___#_#_#_#_#_#___#___#_______#
#_#_#####_#####_#_###_#####_#_#####_#_#_#_#####_#_###_#_#########_#########_#_#_###_#############_#_#####_#_#_#_#####_###_#####_#####_#####_#####_#####_#####_#_#_#_###_#####_###_#####_#_###_#########_###_###_#####_#_#_#_#####_#_#####_###_#_#######_###_#_#########_#######_#_#_#_#_#_#_#_#_#_###_#_###_#######_###########_#_#######_#_#_#######_#_#######_#_#_#_#_#######_#_#_###_#_#_#_#_#######_#_#######
#_#_____________#___#_____#_#___#___#_#_#_#_____#_____#_#_#___________#_____#_#___#_____#_________#_#___#___#_#_#___#___#_______#___#_________#_#_____#_______#___#_#___#_#___#___#_____#___#_#_________#___#___#___#_#_#_____#___#_#___#___#_#_____#___#___#___________#_____#___#___#_#_#_#___#_#___#_____#_____#___#_____#___#___#_____#_#_#_____#_#_____#_____#___#_____#_#_#_#___#___#_#_#_______#_#_#_____#
#_#################_#_#####_###_#_#####_#_#_###########_#_#_#########_#_#_###_###_#######_#########_###_#####_#_#_#_#############_#_#########_#_#####_###########_#_#_###_#_#_#_#####_#_###_#_#########_#_#_#_###_#_#_#_#####_#_###_#_#####_#_#_#_#_#_###################_#_#_#_###_###_#_#_#####_#####_#####_###_###_#_#_###_#####_###_#_#_###_#_###_#_#####_#######_#####_#_#_#_#########_#_###_#####_###_###_#
#_#___#___#_____#___#_______#___#_#___#___#_______#_______#_#___#_____#_#_#_____#_______#_#_#_________#_____#_#___#_______________#___#_#_____#_____#_#___#_____#___#_#___#_#_#_______#___#_#___#_____#_#_#_#_____#___#_#___#_#_____#_____#___#_#_#___#___#_______________#_#_#_#___#___#_____#_#_____#_#_____#_____#___#___#_#___#_____#_#___#_#_____#_#_____#___#___#___#_#_#_#___________#_____#___#___#___#_#
#_#_#_###_#_###_#_###_#######_###_#_#_#####_#####_#_#######_#_###_#######_#_###########_#_#_#_#######_#_#####_#######################_#_#_#####_###_#_#_#_#_###_###_#_###_#_###_###########_###_#_###_#_#_#_###########_#_#_#########_###_#####_#_#####_#_#_###############_###_#_###_#######_#_#####_#_###_#######_#######_#_#_#_#######_###_#_#######_#_#####_#_#####_#_#_#_#_###########_###_###_#_#_#_###_#_#
#___#_____#_#_____#_#_#_#_______#___#_____#___#_#___#_____#_#___#_#_______#___________#_#_#___#___#___#_____#_#_#___________#_______#_#_________#___#_#_#_____#_#___#___#_#_____#_____#_______#___#_#_#_#_#___#___#_______#_________#___#_______#_____#_#_#___#___________#___#_#___#_#___#_________#_#_____#___#_#_#___#___#___#_#_________#_#___#_________#___#_______#_#_#_#_#_________#_#___#___#_#_#___#_#_#
#########_#_#######_#_#_#_#####_#########_###_#_#####_###_#_###_#_#_#_#########_#######_#_#_###_#_#_###_###_#_#_#_#_#######_#_#_#####_###########_###_#_#######_#_#####_#_#######_###_#_#####_#####_#_###_###_#_#_#_###############_###_#############_#_#_###_###_#_#####_###_#_###_#_###_#_#########_#######_#_#_#_#_###_#######_#_#########_###_###########_###########_#_#_#_#####_#####_#_###_###_#####_#_#_#
#___#___#_#_____#_____#___#_______#_____#___#___#_____#___#___#_#_#_#_#_______#___#___#___#_#___#_#_#___#_#___#___#___#_____#_#_#___#___________#_#_#_#_#___#___#___#___#___#_____#___#_#___#_#_____#_____#___#_#_#_#___#___#___#_____#_#___________#___#___#___#_#___#_#_#_#___#___#_#___#___#_____#_________#_#_____#___#_____#_#___#___#___#_#_________#___#___#_____#___#_#___#___#___#_#___#___#_____#___#_#
#_#_#_#_#_#####_#_#########_#####_#_###_###_###_#_#####_#####_#_#_###_#_#_###_###_#_#_#####_#####_#_#_###_###########_#_#####_#_#_#_###########_#_#_#_#_#_###_#####_#_#####_#_#####_###_#_###_#_###########_###_###_#_#_#_###_#_#_#####_#_###_#####_#######_###_#####_#_#_#_#####_###_#_#####_#_###_###########_#######_###_###_#_###_#_###_#_#_#########_#_#_#_#_#####_#####_###_#_###_#_#_###_###_#####_#####_#
#_#___#___#___#_#___________#___#_#_#_____#_#___#_#_____#_____#_#_____#_#_#___#___#_#_____#___#___#_#_#___________#___#_#_____#_#_#_____#_______#_#_#___#___#_____#_#_____#_#_#_____#___#_____#___#___#___#_#_____#_#_#___#___#_#___#___#___#_#___#_#___#_____#_______#_#_#_____#_#_#_#___#___#___#___#_______#_______#_#___#_____#___#___#_#_______#___#_#_#_#_#_____#_______#_#_#___#_#___#___#_#_#___#_______#
#_#########_###_#############_###_#_#####_#_###_#_###_###_#####_#########_#_###_#_#_#####_###_#_###_#_###_#######_#_###_#_#####_#_#_#####_#######_#_#######_#####_#######_#_#_#_#####_#####_#####_#_#_#_#_#_#####_#_#_#####_###_###_#_#######_#_#_#_#_#_#_###_#########_#_###_#_#_#_#_###_#_#####_#_###_#_###_#######_#_#_#########_#####_#_#######_#_#_#_#_###_#####_#_###_#_#_#_###_#_#####_###_#_#_#_#########
#_#___________#___#___#_____#_____#___#___#___#_#___#___#_____#___#_______#_#___#_#_#___#_____#_#___#_____#_____#___#_#_#_#___#_#_#_______#_______#_________#___#_______#___#_#___#_#_____#_______#_#_#_#_#_#_____#_#_______#_#___#___#_______#_#_#___#_#_#_#_____#_____#___#_#_#___#___#_#___#___#_#___#_#___#_____#_#_#_#___#___________#_______#___#_#_#_#___#_____#___#_#___#_____#_#_____#___#___#_________#
#_#####_#####_###_#_#_#_#_###_#######_#######_#####_###_#####_###_#_#_#####_#_#####_#_#_#######_#_#########_###_#####_#_#_#_#_#_#_#########_#######_#########_#_#####_#####_#_###_#_#_###_#########_#_#_#_#_#_#####_#_#####_#_###_#######_#####_#_#####_#_#_#####_###_#####_#_#_###_###_#_#_###_###_#_###_#_#####_###_#_#_#_#_###########_#######_#####_#_#_#_###_#########_###_###_###_#_#######_#############_#
#_____#_____#___#___#_#_#_____#_____#_______#_____#_#_______#___#___#_#___#_#___#___#_#_#_______#___#_____#_#___#___#___#_#_#___#___#___#___#___________#___#_#___#___#_____#_#___#___#___#_________#___#___#_#___#_#_#___#_____#_______#_#_____#_#___#_#_#_____#_____#_____#_#_#_____#_#___#___#___#_#___#_#___#___#___#___#_#___#___#___#_____#_#_____#_#___#___#_________#_#___#_#___#_#_____#___#_____#___#_#
#####_#######_#######_#_#########_#_#####_#_#####_#_#######_#_#_#####_#_###_#_#_#_###_#_#_#########_#_###_#_#_#_#_#_###_#_#_###_###_#_#_###_###########_#_#_#_###_#_###_#####_#_#######_#_#_#################_#_#_#_###_#_#########_###_#_#_#####_#_###_#_#_#########_#_#####_#_#_#####_#_###_###_#_#_#_###_#_#_###_###_#####_#_#_###_#_#####_#_#_#######_#####_###_#########_###_#_#_#_#_#_###_#_#_#_###_#_###_#
#___#_#_____#_#_______#___________#_____#_#_____#_#___#_#___#_#___#___#_____#_#_______#_#___#_____#___#_#___#_#_#_#___#_#_#_#_____#___#___#_#_________#___#_#_#___#_____#_____#_#_______#_#_#_______#_______#___#_#_____#___________#_#_#_#___#___#_____#_#_#___#_____#_#_____#_#___#___#_#___#___#_#_#_#_____#_______#___#_#___#___#_______#_#_#___#_____#_____#___#_#___________#_#_#_#___#_#___#___#_____#___#
#_#_#_#_###_#_#_#######################_#_#####_#_###_#_#_###_###_#_###_###############_###_#_#_#######_#####_###_###_#_###_#############_#_#_#######_###_#_#_#_#########_#####_#_#_#######_#_#_###_#_###_###_###_###################_#_#_###_#_#########_#_#_#_#_#####_#_#_#######_#_###_#_###_#####_#_#########_#######_#_#######_#######_#_#_###_#_#####_#####_###_#_###########_#_#######_#_#############_#_#
#_#_#_#_#_____#_#_____#___#_________#___#_____#_#_____#_#___#___#_#_#_#_#___________#_#___#_#_#_______#___#___#___#_#_#_____#___________#_#_#_______#___#_#___#_________#_#___#___#_#_______#_#_#___#___#_#___#_#_____#___________#___#_#___#_#___________#___#___#_____#_#_#___#___#_#_#_#_#_#_#_____#_____#_____#_____#_#_______#_____#___#_#___#_#_____#_____#___#_#_#_______#___#_____#___#_#___#_________#_#
#_###_#_#####_#_#_###_#_###_#######_#_#######_#_#######_###_#####_#_#_#_#_#########_#_###_#_#_###_#####_#_#_###_###_#_#####_#_#####_#####_#_#####_#_###_#########_#_#####_#_#_#####_#_#########_#####_###_#_###_#_###_#_###_#####_#_#_#_#_###_#############_#######_#####_#_#_#_#_###_#_#_#_#_#_#_#########_#######_#_#_#_#_#####_#####_###_###_#_#_#####_#_###_#_#_#_#_#_#####_#_#######_#_#_#_#_#_#_###########
#_#___#___#___#_____#_#_#___#_____#___#_____#_#___#_______#_______#_#_#___#_____#___#___#_#_____#_______#_#_________#_#_____#_#___#_______#_#___#_#_#_#_________#_#_#___#___#_#___#_#_#_________#_____#___#_#___#_#_#_#_#___#_______#_#_#_#___#_____________#_______#_____#_#_#___#_#_#___#___#_#_#_____#_#_#_______#_#_#_#_____#___#_#___#_____#_#_____#_#___#_#_#_#___#_____#_#_______#___#_#_#_#___#_________#
#_#_#####_###########_#_#_###_#_#########_#_#_###_#####_#_#########_#_#####_###_#_###_#_#_#_#######_#####_###########_#####_#_#_#_#########_#_#_###_#_#########_#_###_#_#####_#_#_###_#_#########_#####_###_#_###_#_#_###_###_#######_#_#_#_#_###########_###_#######_#####_#_#####_#_#_###_###_#_#_###_#_#_#_#######_###_#_###_###_#_###_#########_#####_###_#_###_#_#######_#_#######_#####_#_#_#####_#######_#
#_#_#_____#___________#_#_#___#_#_________#___#___#___#_#___#___#___#_____#___#___#___#_#_#_#_____#_____#_______#___#_____#_#___#_____#___#___#_____#___#_______#_____#_____#___#___#_#_______#_______#___#___#___#_#___#___#_#___#___#___#_#_#_________#_#___#___#_#___#_#_#_#___#_#_#_#___#___#___#_#___#___#_____#_____#_#_#___#___#_#_________#_____#___#_#___#_#_______#_#_#_______#_#___#_#_______#___#___#
#_#_#_#_###_#_#########_#_#_#_###_###_#########_###_#_#_###_#_#_###_#####_###_#######_#_#_###_###_#######_###_#_#_#_#_###_###########_#_#_###########_#_#_#########_#####_#_#######_#_#_#####_#_#########_###_#_###_###_###_###_#_#####_###_###_#######_###_###_#_#_###_#_#_#_#_#_#_#_###_###_#######_###_#####_###_#######_#_###_###_#_#########_#####_###_#####_#_#######_###_#_#######_#_###_###########_#_#_#
#_#_#_#___#_#_#_____#___#_#_#_#_____#_____#___#_#___#___#_#___#_____#_____#_#_#_____#_#___#___#___#_____#_#___#_#_#_#_#___#_______#___#_#___#_________#_#___#_____#_____#_#_______#_#_#___#___#_#_______#___#_#_#_#___#_____#___#_______#___#___#_____#_____#___#_____#___#_#___#___#___#_#_#_______#___#___#___#___#___#_____#___#_______#_____#_____#_____#___#_#_______#_____#_#_______#___#_#_______#___#_#_#
#_#_#_###_#_###_###_#_###_#_#_#_#########_#_###_#_#######_###########_###_#_#_#_#_#_#####_#_###_###_###_###_#####_#_###_###_#_###_#_#######_#_#######_#_###_#_###_#_#####_#########_#_#####_###_#_#####_#_###_#_#_#_#_#####_#_###########_###_#######_#######_#_#########_#_#######_###_#_#_#####_###_#_###_#_###_#_#_#_#######_#########_###_#_#_###_#######_#_#_#_#####_#####_#_#_#_###_###_#_#_#_###_#_#_#_###
#___#_#___#_____#___#_#___#_#___#_#_______#___#_#___#_______________#_#_#___#_#_#_#_____#___#_#___#___#___#_#_____#_____#___#___#_#_______#___#_____#_#_#___#_#_#_#_#_____#___#_____#_____#_____#_#_____#_____#_#___#___#___#_#___#_____#_____#_______#___#___#_#_____#___#_________#___#_____#_#_#___#___#_#___#_#___#_______#_____#_____#___#_#_#_#_#_______#___#_____#_____#_#_#_#___#___#_#_#_#_#_____#_#___#
#_#####_#_#######_###_#_###_#####_#_#######_#_#_#_#_#_#######_#####_#_#_###_#_###_#####_#####_###_###_###_#_#_#_#######_#_#####_#_#######_###_###_###_#_#_###_#_#_###_#_###_#_#_###_#####_#######_###_###_#####_#####_#_#_###_###_#_###_#######_#_###_#_###_###_#_###_#_#############_#_#####_#_#_#_###_###_#_###_###########_#####_#_#####_#_###_#_#_#_###_#######_#####_###_###_#_###_###_#_#_###_#######_###_#
#_#_____#_#___#___#_#___#___#_____#_#_______#_#_#_#_#_______#_____#___#___#_#___#_#___#_#_____________#___#_#_#_#___#___#_#___#_#_____#_#___#_____#___#_#_#___#_#_#___#_#___#_#___#_____#_#_____#___#___#_____#_#___#_#___#_#___#___#_______#_#_#___#___#___#_#___#_#___#_____________#_#___#___#___#_#_____#_#___#_____#___________#_#_____#_#___#_#_#_#_#___#_____#___#___#___#_#___#_#___#_#___#___#___#___#_#
#_#_#_###_###_#_###_#_#####_#_#_###_#_#######_#_###_#######_#####_#######_#_###_#_#_#_#_#_#######_#######_#_#_###_#_#####_#_###_#####_#_###_#####_#_#####_#_#_#_#_#_###_#_###_#_#_#####_#_#_###_###_###_###_###_#_#_#_#####_###_#_###_#####_#_#_###_#####_###_#####_#####_#############_###_###_#####_#######_#_###_#####_###########_###_#####_###_#_#_#_###_#_#####_#_#####_#_#_###_#_#####_###_###_#_#####_#_#
#_#_#_#_____#_#_#___#_____#___#_#___#_#_____#_#___#___#___#___#_#_#_____#___#_#___#_#_#_#_#_____#_#_____#___#_#___#_#_____#___#_____#_____#_#___#_#_#_____#_#_#_____#___#_#___#_#_#_#___#___#_#_____#_#___#_#___#_#_#_#_______#_#_#___#___#_#_____#_______________#_#___#_#___#___#___#_____#_#___________#_#_#_#_#_______#___#_#_____#___#_____#___#_#_#_#___#_____#_#_____#_#___#___#___#_____#_#___#_______#_#
#_###_#####_#_#_#_#_#####_#_#####_#####_###_#_###_###_#_#_###_#_#_#_###_#_###_#######_#_###_###_###_###_#####_#_###_#_#####_#_#####_#####_#_#_#_#_#_#_#####_#_###########_#_###_#_#_#_#######_#######_###_#_#_###_#_#_#######_#_###_#####_#_#######_#############_#_#_#_#_#_#_#_#_###_#####_#_###########_#_#_#_#_#_#######_#_#_#_#####_###_#####_###_#_#_#_#######_#_#####_#_#####_#####_#_#_###_#_###_#######_#
#___#___#_#___#_#_#_______#_#_____#_____#___#___#_#___#_#___#___#___#___#___________#_#_____#___#___#_#_____#_#___#_#_#_____#_____#_____#_#_#_#_#_#___#_____#___________#_#_#___#_#___#___#_____#_______#_#_#_____#_#_______#_#_____#_____#_#_____#_#___#_____#___#_#_#_#___#_#_#_________#_#___#___#___#_#___#_#___#_#_____#_#_________#___#_#_______#_#_#_____#___#_#___#_#___#___#___#_#_#_#___#_#_#___#_____#
###_#_#_#_#_###_#_#########_#_#####_#_###_#####_#_#_###_#####_#######_#############_#_#######_###_###_#####_#_###_#_#_#######_#########_###_#_###_#########_###########_#_#_#####_###_#_###_###_#_###_###_#######_#_#######_#_#########_#_#_#_###_###_#_#_###_###_#_#_#_###_#_#_###########_#_#_#_#_#_#_#_#_###_#_###_#_#####_#_#########_###_#_#######_#_#####_#####_#_#_#_###_#####_#_#_#_###_###_#_###_#_###_#
#___#_#_#___#___#_#___#_____#_____#_#___#_____#_#_#_#_#_______#_____#_#_____#_____#_#_______#_____#_______#_#_____#_#___#___#_________#___#_#___#___#_____#___#_______#_#_#_____#___#_#___#_#_#___#___#___#_____#_#___#_______#_#_______#_#_#_#_______#_#_#_#___#___#_#___#_#_#_____#_____#_#_#_#_#___#___#_#___#_#___#___#_#_#_#___#_____#_____#_#___#_______#_____#___#_#_#_#_#_____#___#___#_#___#___#_#_#_#_#
#_#####_#####_###_#_#_###_#######_#_###_#####_#_#_#_#_#_#######_###_#_#_#####_#_#_#_###_###_#######_#######_###_###_###_#_#_###_#####_###_#_#_#_###_#_###_#_###_#####_#_#_#####_###_#_###_#_#_#####_###_###_###_#####_#_#####_#_#_###_#####_#_#########_#_#_###_#####_###_###_#####_#_###_#_###_#_###########_###_#_#####_#_#_###_#_#_#####_#####_#_#_#_#######_###_#_#####_#_#_#_#######_###_#_#_###_###_#_#_#_#
#_#_____#_____#___#_#___#_#_____#_#___#_#_#___#_#___#_#_#_____#___#___#_______#_#_#_____#_#___#___#_#_____#___#_#_#___#___#___#___#_____#___#_#_______#_#_#_#___#___#___#_#_________#___#_#_#_____#_#___#___#_________#_#___#___#___#_______#_____#___#___#___#___#___#_#___#_____#_#___#_______#_#_____#_____#___#_#_____#_#_#___#___#___#_#_______#___#_____#___#_#_#_____#_#___#_______#___#___#_______#_#___#
#_#_###_#_#####_###_###_###_###_#_#_###_#_#_###_#####_#_#_###_###_#######_#####_#########_###_#_#_#_#_###_###_#_#_#_#_#######_###_#_###_#####_#########_#_###_###_#_###_#_#####_#######_#_#_#_#####_#_###_#_#_###########_#_#######_###########_#_#_#_#####_#####_#_###_###_#_###_#_###_#########_#_#####_#_###_###_#_#####_#_#_#######_#_#_#_###########_###_#####_#_#_#####_#####_#####_#_#_#####_#######_#####
#___#___#_#_#___#___#_______#_#___#_#___#_#___#_#_______#___#___#_______#_____#_____________#___#___#_#_#_____#___#_#_#_____#_#___#_#___#___#_#_____#_________#___#___#_#_____#_#___#_____#_#_____#___#___#_#_#_____#_____#_________#_________#_#___#_#_____#___#___#_#_____#___#_#_#___#_____#___#_______#___#_#___#_____#_____#_#_____#_#_#_#_____#___#_#_#_____#___#___#_____#_#_#___#_#_#_#___#_______#_____#
#####_###_#_#_###_###########_#####_#_###_###_#_#_#########_#_#########_#############_#####_#######_#_#_###########_#_#_###_#_#_###_###_###_#_#_###_#_#########_#####_#_#####_###_#_#######_#_###_#########_###_###_#_###############_#######_#_#####_#_#####_#_#####_#_#########_#_#_###_###_#_###_#########_#_#_#######_#######_#_#####_#_#_#_###_#_###_#_#####_#######_#_###_#_#_#_#_###_#_#_#####_#########_#
#_#___#___#_______#___#_#_______#___#_#_____#_#_#___#_______#_______#_#_________#___#_#_____#_______#_#_____#_______#_#___#_#_#_#_#___#_#___#_#_#___#_#_______#_#_#___#_#___#_____#_#___#___#___#_________#_#___#_#_#_____________#___#___#___#_#_____#_______#___#___#___#_____#_#_#_#_#_#_#___#___#_____#_#_#_#_______#___#___#___#___#___#___#_#___#___#_____#_#___#___#___#___#_#_#_____#___#_____#___#_____#
#_#_###_###_#######_#_#_#_###_###_###_###_###_#_###_###_#######_###_#_#########_#_#_#_#######_#######_###_#_###_#####_###_#_#_#_#_###_#_#_#_#_#_###_###_#####_#_#_#_###_#_#########_#_#_#_#######_#######_#_#_###_#_#############_###_#_#_#_#####_#_#########_#_###_#_###_###_#_#_#_#_#_#_#_#####_#####_#_#_#_#_#######_###_#_#_#_###_#_#_#######_###_#_###_###_#_#_#_#_###_#_#####_#_#####_#####_#####_#_#_###_#
#_#_#_#_#_____#_____#_#_____#_______#___#_________#___#___#___#_#_#_#_____#___#___#_#_________#_____#___#_#_____#___#___#_#_#_#_#___#_#___#_#_#___#_____#_#___#___#_______#_______#_#_#_#_________#_#_____#_#_____#_______#_____#_____#_#_#___#___#_#_______#_#_____#___#_#___#___#_#___#___#___#_______#___#_#_____#_____#_#_#_#___#_#_#___#___#_____#_#___#___#___#_#___#_#_______#_#___#_#_____#___#_#___#___#
#_#_#_#_#######_#####_#####_###########_#############_#####_#_#_#_#_#_###_###_#####_#######_###_###_###_#_#######_#_#####_#_#_#_###_#_#####_#_###_#######_#_#####_#########_#####_#_#_#_###########_#_#####_###_#####_#_###_###_#######_#####_#_#####_#####_###########_#_#_#######_###_###_#_#_#######_#####_#####_#_#####_#_###_###_#_###_#_###_#####_#_###_#######_#_#_###_#######_#_###_#_#####_#_#_#########
#___#_#_____#___#_#___#___#_____#_____#___________#_________#___#_#_#___#_____#_#___#_____#___#_#_#___#_#_#___#___#_______#_#_#_____#_____#_#_#___#_______#_#___#___#_______#_______#_#_______#_____#___#_____#_#_____#_#___#___#_____#_______#_______#___#_____#_______#_#_#_______#___#___#_#_______#_#_____#_______#_____#_____#___#_#___#_______#___#___#_______#_#_#___#___#_____#_#___#_#_____#___#_______#
#_###_#####_#_###_#_###_#_#####_#_###_#_#########_#_#############_#_###_#####_#_#_#_#_###_###_#_#_#_###_#_#_#_#_###########_#_###########_#_#_#_#######_#_#_#_#_###_#_#######_#######_###_###_#####_###_#_#_###_#_#####_#_###_###_###_#_#######_#########_#####_#_#######_#_#_#####_#_###_###_###_#_###_#_#####_#######_###########_###_#_###########_###_#_#_#######_#_###_#####_#####_#_###_#_#########_#####_#
#_#_____#___#_____#_____#___#___#_#_#_#_#___#___#___#_______#_____#_____#___#___#_#_#_#_#___#___#___#___#___#_____#___#_____#___#___#_____#_#_#_#_____#_#___#_#_____#_#_#_________#___#_#_#_#_________#_#_#_#___#_#_____#_#_#_____#_#___#_______#_____________#_#___#_______#_#_____#_#_#_#_#___#_#_#___#_#_#___#___#___#___________#___#_#___________#___#_#_#_____#_#___#_____#___#___#___#_#_________#___#___#
#_#_###_#_#####_#######_###_#####_#_#_#_#_#_###_#####_#####_###_#_#######_#####_#_###_#_###_#########_###########_###_#_###_###_#_#_#_#####_#_#_#_###_#_#####_#######_#_#_#########_###_#_#_#########_#_#_###_###_#_#####_#_#######_#########_###_###_#####_###_#_#_#_#########_#####_#_#_#_###_#_#_#_###_#_#_###_###_###_###########_###_#_#######_#######_###_###_#_#########_###_#_#####_#_#########_###_#_###
#_#___#_#_______#_____#___#_#___#_#_#_#___#_____#___#_#___#___#_#_____#_______#_#___#_____#_#_______#_#_______#___#___#___#___#_#_#___#___#_#_#_____#_#_____#_____#_#_#___#_____#___#___#_#___________#_#_____#___#_#___#_____#_____________#_#___#___#___#_#___#_#_______#___#_#___#___#_#_____#_#_#___#_#_#_____#___#___#_______#___#_#_#___#___#_#___#___#_____#_____________#___#___#___#_#_____#_#_____#___#
#_#####_#########_###_#####_#_#_#_#_#_#########_###_#_###_###_#_#####_#_#####_#_###_###_###_#_#####_#_#_#####_#_###_#_###_###_#_#_#####_#_###_#######_#####_#####_#_#_#####_###_#_#####_#_#####_#######_###_###_###_###_#####_###_#########_#_#####_###_###_#_#####_#######_#_#_#_#_###_#_#_#####_###_###_#_#####_#_###_###_#######_###_#_###_#_###_#_#_#_#_#_#################_#_###_#_#_###_###_#_#_#########_#
#_____#_______#_#_#_________#_#___#_#_____#_____#___#_______#_#_____#_______#_____#_____#___#_____#_#___#_____#_____#_#___#___#_#_#___#_#_____#_______#_____#___#___#_________#_#_______#_____#___#___#___#___#___#___#_____#___#_#___#_____#_______#_#_____#_____#___#_____#_#___#___#_#_#_#___#___#_#___#___#___#___#_#_____#___#_#_____#___#_#___#_#_#_#_#_#_#_____#_________#_#___#_____#_____#_______#_____#
#####_###_###_#_#_#########_#_#####_#####_#_#####_#########_#_#######_###################_#_#####_#_#####_###########_#_###_###_#_#_#_#_#######_#######_#####_#####_###########_#######_#####_#####_#_###_#_#####_###_#_#_#####_#_#_#_#_#############_#_#########_#####_#####_#_#####_#_#_###_#_#_#_#_#_###_#_#_#####_#_#_###_#_#_#_#_#####_###_#_###_#_#_###_#_#_###_#_#########_#######################_#_###_#
#___#_____#_#___#___#___#_#_#_#_________#_#_____#_______#___#___#___#___#_____#_________#_#___#___#_____#_#_____#_____#_#_#_#___#___#___#_______#_____#___#___________#_______#_#_______#___#_#_____#_____#_#_____#___#_#___#___#_#_#___________#___________#___#_____#_____#_#_____#___#_____#_#_#___#_#_#_#___#___#___#_#___#_#___#_#_____#_______#_#_#_____#_#_#___#_____#_____#_______#_____#_______#_#___#_#
#_#########_###_###_#_#_#_#_#_#_#######_#_#####_###_#####_#####_#_#_#####_###_#_#######_#_###_#_#####_###_#_###_#####_#_#_#_#_###_#######_#########_#####_#_#########_#_#####_#_#_#########_#_#_###########_#_#####_#######_#_###_#_###########_###########_###_#####_#####_#_#####_###########_#######_#_#_#####_#_#######_###_#####_#####_###_#####_#_#####_#_#_###_#######_###_#_#####_#_#_###_#_#####_###_#_#
#_#_____#_____#___#___#_#___#_#___#___#_#_#___#_____#___#___#_#___#_#___#___#___#_____#_#___#_#___#_#___#_#___#_____#_#_#_#_#___#_#_______#_________#_____#___#_____#_#_#_______#___________#_#_________#_#_#_#___#_#_______#_#___#_#___#_____#_______#_________#___#_______#___#_#___#_______#_____#___#___#_____#_______#___#___#_#_____#___#_#_____#_____#___#___#___#_____#___#_#___#___#_____#_______#___#_#
#_#_#_###_###_###_#####_#_###_###_#_#_#_#_###_#_#####_#_###_#_#####_#_#_###_#####_###_#_###_#####_#_###_#_#_#_#####_###_#_#_###_###_#########_#_#####_#######_#_###_#_#_#_#######_###########_#########_#_#_#_###_#_###_#####_#_###_#_###_###_#######_#_#######_#_#_###########_#_###_#####_#_#####_#_###_###_#######_###_###_###_#_#####_#_#_###_#########_#######_###_#_###_#####_#_#####################_###_#
#___#_#___#_#_____#_#___#___#_#_#_#_#___#___#___#_____#___#___#___#_#_#___#_#___#___#_#___#_____#_#___#___#_#_____#_#___#_#_#___#___#_#_______#_#_____#_______#_#___#_#_#_#___#___#_________________#___#_#_#___#_______#___#_#___#___#___#___#_____#_#_#___#_#_#_#_______#_____#___#_______#_#_____#_#___#_____#_____#_#_____#___#_____#_#_#_#___#_____#___#_____#___#___#___#_____#___________#_______#___#_#_#
#_###_#_###_#######_#_#######_#_#_#_###_###_#_###_#######_###_#_#_#_#_###_#_#_#_###_#_###_#_#_###_###_###########_#_#_###_#_#_###_###_#_###_###_#_#######_#####_#_#####_###_#_#_###_#####_#########_#_###_#_###_#_#######_#_#_###_#####_###_###_###_#_#_#_#_#_#_###_#_#####_#######_###_#####_#_#####_#####_#####_#####_###_###_###_#_###_###_#_###_###_#_###_###_###_#####_###_#####_###_#_#####_#####_#_###_#_#
#_#___#_____#_____#_#_______#_#___#_#___#_#_#___#_#_____#_____#_#_#___#___#_#_#___#_#___#_#_#___#_____#___________#___#_#___#_#___#_#_____#_#___#___#___#_#_#___#_#_____#___#___#_#_____#_#_______#_#_#_____#_#_#_#___#___#_#___#_________#___#___#_#_#_#_#_#_____#_#_#_____#_________#_#___#_#_#___#_________#___#___#_____#___#___#_#___#___#_#___#___#_____#_#___#_#___#___#___#_#___#_#_#_____#_______#_____#
###_#######_#_###_#_#######_#_#####_#_###_#_#####_###_#_#######_#_#_###_###_#_#####_###_#_#####_#####_#####_###########_#_###_#_###_#_#####_###_###_#_#_#_#_#_###_#_###_#_#######_#####_###_#####_#_#_#_#####_#_#_#_#_#_###_###_#_#######_###_###_#_#_###_#_#####_###_#_#####_#########_#_#_###_#_###_#####_###_###_#_#_#####_###_###_#_###_#_#_#_###_#########_###_#_#_#_#_#####_#_###_#_###_#############_#####
#___#_____#_#___#___#___#___#_____#_#_____#_____#_#___#_#_______#_#___#_#___#_______#_#___#_______#_#_____#___#_________#_#___#_#_#___#___#___#___#_#_#_#___#_#_#_#_#_#_#_#_____#___#_#_____#___#_#_#_#_#_____#_#_#_#___#___#___#_#___#_____#___#_#_#_____#___#_______#_____#_#_________#_#_____#_____#___#_#___#___#___#___#_#_____#_#___#_#_#_#_#___#___________#_#___#_#_#___#_#_#___#___#_____#_____#_#_#___#
#_###_#_###_###_###_#_#_#_#######_#_#_#########_#_#_#_###_#######_#####_#_#_#_#######_#####_#####_#_#####_###_#_#####_###_#_###_#_#_###_#_###_#####_#_#_#####_#_#_#_#_#_#_#_#_#_#_#_#_#######_#_#_#_#_#_###_###_#_#_#####_###_###_#_#_#####_#_###_#_###_#####_#######_#####_#_#_#########_#########_###_#_###_#######_###_#_#_#####_#_###_#_###_#_#####_###_#######_#####_#_#_#_#_#_#_#####_#####_#_###_#_#_#_#_#
#_____#_#___#___#_____#_#_______#___#_#_______#_____#_#___#_____#_____#_#_#_#___#___________#_____#___#_#___#_#_____#_____#_____#___#___#_____#_____#_#_______#_#___#_#_#_#_#_#___#_#_#_______#_#_#_#_#___#_#___#_#___#_#___#_#___#_#_____#_#_____#_#___#___#_______#_____#_#_#_#_______#_#___#___#_#___#_____#_____#___#_#___#_____#_#_#___#___#_________#_______#_____#_#_#_#_#_#_#___#_#_____#_____#_#___#_#_#
#_#####_#_###_#_#######_###_#_#######_#_#####_###_#####_###_#_#######_#_#_#_###_#_#_#####_#####_###_#_#_###_#_#########_#########_###_#########_#####_#########_#####_#_#_###_#####_#_#_###_#####_###_###_#_#_#######_#_###_#_#####_#####_#########_#_###_#_#######_#_#####_#_#_#####_#_#_#_#_#_#_###_#######_#_###_#####_#####_#####_#_###_#_###########_#######_#####_#_###_#_#_#_###_#_#_###########_#_#####_#
#___#___#_#___#_#_____#_#___#_#___#___#_____#___#_#_____#___#_________#_#_#___#_#_#___#___#___#_____#_#___#_#_________#_#_______#_#___#_______#_#___#_#___#_______#_____#_____#_____#_____#_#___#___#___#___#_______#_____#_#___#___#___#_________#_#_#___#_____#___#_#_____#_______#_#_#_#_#___#_____#_______#___#_______#_______#_____#___#_#_________#_#___#_____#___#_____#___#___#___#_#___________#_#_____#
###_#_#_#_#_#####_###_###_#####_#_#_#######_###_###_#######_###########_#####_#_#####_#####_#_#######_#_#_#_#####_###_#_#_#####_###_###_#####_#_#_#_#_#_#_#_#_#####_###########_###########_#_#_###_###_###########_#####_#_###_#_###_#_#########_#_###_#######_#_###_#_###########_###_#_#_#####################_#########_#####_#####_#_###_###_#####_###_#_#_###_#_#_###########_#_###_#_#_###########_#_#_#_#
#___#_#_#_#_#_____#_______#___#_#___#_#_____#_#_#___#_____#_________#_____#___#_____#_____#_#_____#_#_#_#_#_____#_#___#___#___#___#_#___#___#_#___#_#___#_#_#_#___#_#_____#___#___________#_#_#_____#___#_________#___#___#___#_#_#___#___#_____#_#_#___#___#___#_#_#_#_______#_________#_#_____________________#_______#_______#_____#_#_#_#_____#___#_____#_#___#_#_#_#_________#_#_#___#_#_#_________#_#_#_#_#
#_###_#_#_#_#_#############_#_#_#####_#_#####_#_#_###_###_#########_#####_#_#######_###_#_#_#####_#_#_#_#######_#_#_#######_#####_#_#_###_#_#_#####_#####_#_###_#_#_###_#_###_###########_#_#_#####_#_###_#######_#_###_#####_#_#_#_#####_###_#_#_#_#_#####_#_###_#_#_#######_#########_#_#_#################_#_#######_#_#########_#_#_#_#_#######_#_#######_#_#_###_#_#######_#_#_###_###_#_#####_#_###_#_#_#_#
#___#_#_#___#___#___#_______#_______#_#_#___#___#_____#___#_______#___#___#_____#_____#_#___#_____#___#_#_____#_#_#_#_#_____________#_#___#_#_____#_____#_#_____#_______#_____#_______#_#___#_#___#_#___#___#___#_#_____#_____#___#_#_#___#___#_#_#_#_#___#_#_#___#_________#_#___#___#_#_#___#___#___#_____#_#_#_______#___#_____#_#_#_#_#_____#___#_______#_#_#_#___#_________#_#_#_____#_#_#___#_#___#_#_#_#_#
###_#_#####_###_#_#_#_#############_#_#_###_#_#######_#_###_#####_###_#_#######_#####_###_###_#####_###_#_###_#_###_#_#_#############_#_###_#_#########_#_#_###########_#######_#_###_#_#_###_#_#_#_###_###_###_#_#####_#_#########_#_#_###_#_###_#_#_#_#_#_#_#_#########_###_#_#_#_#_###_###_#_#_#_#_#_###_#_###_###_#####_#_###_###_#_#_#_#####_#####_#####_###_#_#############_#_#_#####_#_#_#_#####_#_###_#_#
#___#___#_____#___#___#_____#_____#___#___#_________#_#___#_____#_#_#_#_#_____#___#_____#_#___#___#_____#_#___#_#___#_____#___#_______#___#_#_______#___#_#_#_________#_#_______#___#___#_#___#_#_#_#_#_____#___#_____#_#_________#_#_______#_#___#_#_#_#___#_#_______#___#___#_#___#_____#___#_#_#_#___#_____#___#_#_#___#_#_#_#___#_#_#___#_____#___#_____#_____#___#_____#___#_#_#_#_#___#___#___#___#___#_#_#
#_#####_#_#############_###_#_###_###_###_#########_#####_###_###_#_#_#_#_###_#_#_#####_###_###_#_#####_#_#_###_#_#_#####_#_#_#_#######_#_#_#######_#_###_###_#######_###_#########_#####_#_###_#_#_#_#######_#######_#_#####_###_#_#####_#####_###_#_#_###_#_#######_#####_###_###########_###_#_#_#####_#####_###_#_#_#_#_#_#_###_#_#####_#_#####_#_#####_#_#######_#_#####_#_#_#_#_#_#_#########_#_#_###_#_###
#_#___#_#___#_____#_____#___#___#_#_#_#_#_____#_____#_______#_#___#_____#_#_#_#_#_____#___#_#_#_#_______#_#___#___#_#___#_#_#___#_____#_#_#_#___#___#_#_#_____#_____#___#___#_____#_#_____#___#_#_#_#_______#_______#_#_____#_#_#_#___#___#___#_#___#___#___#_#_#___#_#___#_#___#_____#___#_____#_#___#___#_____#_____#_#_#_#___#_#_#_______#___#___#_#___#_#_#_____#_#_______#_#_#_____#_________#_#_#___#_#___#
#_###_#_###_#_###_#_#####_#####_#_#_#_#_#####_#_#####_#######_#_#########_#_#_###_###_###_#_#_#_#########_###_#_#####_#_###_#######_#_#_#_#_###_#_#_#_#_#_#######_#_###_###_#_###_#_#_#######_###_#_#####_#_#######_#_#######_#_#_###_#_###_#_#_###_#####_###_#_#_#_#_#_#_#_#_###_###_#_#_#######_###_#_###_#######_###_#_#####_#_#_###########_#_###_###_#_#_#_###_#_#########_#_###########_###_#_#_#####_###_#
#___#_#_____#_#_#___#___#_______#_#___#_#___#_#_#_____#_______#_____#_____#_#___#___#_#_____#___#_______#___#___#___#_#_#___#___#___#_#_#_#_____#_#_#_#_________#_#___#_#___#___#___#_#_____#___#_______#_#_______#_#_______#_#___#___#_#___#_#___#_#_____#___#___#___#_#___#_____#_#___#___#_#_____#_#_#_#_______#_#___#_#_______#_______#_____#___#_____#___#_#_____#_____#___#_#___#_____#_#_#_#_#_____#_____#
###_#_#######_#_#####_###########_#_###_#_#_#_#_#####_#_###########_#_#####_###_###_#_#######_###_###_#####_#####_#_#_#_#_###_#_#_###_###_#######_#_#_#########_###_#_#_#_#####_#####_#####_###_#######_#_#######_#_#######_#_#_###_###_#_###_###_#_#_#####_###_#######_###_#######_#######_#_#_#####_#_#_#######_#_#_###_#_#########_###_#_#######_###_#######_#######_###_#_#_#_#_#_#_###_#_#_#_#_#_###_#####_#
#_#_________#_#___#_____________#_#_____#_#___#_____#_#_#_________#_#_#_______#_#___#_______#_#___#___#_____#_____#___#___#___#___#___#___#_____#_#_#___#_____#___#_#_#_#_#___#_____#_____#___#_______#___#_#___#_#___#___#_#_#_#___#_#_#_#_____#_#___#_____#_#_#_#___#___#___#_____________#___#_____#_____#_#___#_#_#_#_#___#_____#___#___#_______#_#_#_____#___#_____#_____#_#_#_#___#_#___#___#_#_#_______#_#
#_#####_#####_#_#_#_#########_#_#_#######_#########_#_#_#_#######_#_#_#####_#_#_###########_#_###_#_###_#####_#############_#######_###_###_###_#_#_###_#_###_###_#_#_#_#_###_#####_#####_#_#_#######_#####_#_#_#_#_#_#_#_#_#_#_#_###_#_#_#_#####_#####_#####_#_#_#_#_#_#_#_###_#################_#########_#_#_#####_#_#_###_#_###_###_#########_###_#_#_###_###_###_###_#######_#_#####_#####_###_#_#_#######_#
#_______#_____#_#___#_______#_#_#_______#_#_______#_#_#_#_#_______#_#_#___#_#_#___#_______#_#_____#_#___#___#_#___#_____#___#_____#___#_#___#_#___#___#_#_#___#_#___#_#___#___#___#_#_____#_#___#_____#_#_____#___#_#___#_#_#_#___#_____#_#_____#_______#_____#_#___#___#_#_#___#_________________#_______#___#_#_______#_____#___#_#_______#___#_#_#___#_#_#___#___#___#___#_____#_#_______#___#_#_#_#___#___#_#
#_#######_#####_#####_#####_###_#######_#_#_#######_#_#_#_#######_#_#_#_#_#_#_###_#_#####_#_#######_#_#####_#_#_#_#_#_#_###_#_###_###_#_#_###_#######_#_#_#_###_#####_#####_#_#_#_#_#_#######_###_#####_#_#########_#######_#_###########_#####_#########_###_#_#######_#_#_#_###_#####################_#####_#_#_###############_#_#######_#_#_#_#_#_###_#_###_#_#_###_#####_#####_#_#######_###_#_#_###_#_#_#_#
#_#_____#___#___#_#___#___#___#___#___#___#_#_____#_#_#_#___#___#___#_#_#___#_#_#___#___#_#_____#___#_#_____#_#_#___#_#___#_#___#_______#_#___#_______#___#___#_#_____#_____#_#_#___#_#_____#___#_#_____#_____#_____#_______#___________#_____#___________#___#_______#_#_#_#___#_________#_____________#_____#_#_#___#___________#___#_____#_#___#___#___#_____#_#_#___#_____#_____#_#_______#___#_#___#___#___#
#_###_#_###_#_###_#_#####_###_#_#_#_#######_#_###_#_#_#_###_#_#_#####_#_#####_#_#####_#_#_#####_#####_#_#####_#_#####_###_#_#_###########_###_#_#############_#_#_#####_#####_#_###_#_###_#_###_#_###_#_#####_#_#_###_#################_#####_###########_#_#########_###_#####_#_#######_###_#_#_#####_#_#####_#_#_###_#############_#_#####_#####_###_###_#######_#_###_#####_#####_#_#######_#_#_###_#########
#___#_#___#_#_____#___#_____#_#_#___#_________#_#___#_____#_#_#_______#___#___#_____#_#_#___#___#___#___#_____#_#___#_#___#_#_#_________#___#___#_____#_____#_#___#_____#___#_#___#_#___#_#___#_#___#_#_____#___#_#___#_#_____________#_______#_#_______#_#___#_____#___#_____#_#___#___#___#_#_#_#___#_#_#___#___#___#_#___#_____#___#_#_______#_____#___#_#_______#___#_#_____#_______#_______#_#___#_______#_#
###_#_###_#_#_#######_#_###_#_#_#####_#########_#########_#_#_###########_#_###_###_###_###_#_###_#_#_###_#######_#_#_#_###_#_#_#######_#_#_#####_###_#_#_#_#_###_#####_###_#_###_#_###_#_###_#_###_#######_#####_#_###_#_#_#########_#_#######_#_###_#_#_#####_#_#####_#####_#_###_#_#_###_###_###_#_#_#_#_#######_#_#_#_#_#_###_#_###_#######_#_#######_#_#_#_#######_#_#####_#_###############_###_#######_#_#
#_#_#___#_#_#_#_____#___#___#_#_#_____#_________#_______#_#_#_________#___#___#___#_____#_#_#_____#___#___#_______#_#_#_____#_#_#_____#_#_#___#___#___#_#_#_#___#_____#___#_#___#_#___#___#___#___#_______#_______#_#_____#___#_#___#_#_______#___#___#_#_____#_#___#___#___#_#___#___#___#_#___#___#_#_#_#_________#_#_#_#_____#_____#_________#_#_______#_#_#_#_____#_#_______#_#_____________#___#_#___#___#_#
#_#_###_#_#_###_#_#_###_#####_#_#_#####_#######_#_#_###_###_#########_#_#####_#_#_#####_#_#_#########_#_###_#######_#_#_#####_#_#_###_#_#####_#_###_#_#_#_#####_#####_###_#_###_#_#_#######_#####_#######_#########_#_#######_#_#_#_#_#######_#####_#########_#_###_#_###_#_#_###_#########_#_#_#_###_###_###########_#_#############_#####_#####_#_#######_###_#_#_#_#_#########_#######_#####_#_###_#_#_###_#_#
#_#_____#_#_#___#_#___#_#_____#_#_#_________#___#_#_#_#_____#___#_____#_#___#_#_#_#___#_#_#___#_____#_#___#_#_____#_#_#_#___#_#_#___#_#_____#_#_#_#_#_#_#_______#___#_#___#_____#_#_#_____#_#___#_______#___________#_____#___#_#_#_______#_#___#___#_______#___#_#_#_____#_#_____#_________#_#_#___#___#_#___#_____#_#_________#___#_#_____#___#_#_#_____#_____#_#_#_#_________#___#___#___#_#_#_#___#_#___#___#
#_#######_#_#_###_#_###_#_#####_#_#######_#_#_#####_#_#####_###_#_#####_#_#_#_###_#_###_#_###_#_###_#####_#_###_#_#_#_#_#_#_###_#_#_#######_#_#_#_#_###_#########_###_#_#####_###_#_#_###_#_#_#_#_#####_#################_#_###_#_#######_#_###_#_#_#_#####_#_###_#_#######_#####_#_#########_#_###_###_#_#_#_#_###_#_#########_#_#_#_#######_#_###_#_###_#########_#_###_#####_###_#_#_###_#_#_#_#_###_###_#####
#_#_____#_#___#___#_#___#___#___#_______#_#_#_#___#_#_#_____#___#_#_____#_#_#_#___#_____#___#___#_#_____#_#___#_#_#___#_#_#___#_#_#_#_____#___#_#_#___#___#_____#_______#___#_#___#_#_#_#___#_#___#___#___#_______#_______#___#_____#___#_#_____#_#_#_#_#___#_#___#___#_____#___#___#_#___#___#___#___#_____#___#___#_________#_#_#_#___#___#_#_#___#___#___#_______#_____#___#_#_#___#_____#_#_#_#_#_____#_____#
#_#_#_###_#####_###_#_#####_#_#########_###_#_#_#_#_#_#_#####_#_#_#_#_###_#_#_#_#####_###_#_#####_###_###_###_#_#####_###_###_#_###_#_###_###_#_#_###_###_###_#_#########_#_###_#####_#_#####_#####_#####_#####_#_#_#####_###_#####_#_#_#_#_#####_#_#_#_#_###_#_#####_#_#####_#_#####_#_#_#_#_#####_#############_###########_#_#_#_###_#_#_#_#_#_#####_#_###_###_#########_###_#_###########_#_#_#_#_#########_#
#___#_______#___#___#_____#_#_______#___#___#___#_#_#_#___#___#_#_#_#___#_#_#_#_____#_#___#_______#___#___#___#_#_____#___#_____#___#___#___#_#___#_#___#___#_#___________#___#_#_____#_______#___#_____#_______#_#_#___#_#_#_____#_#_#_#_#_______#_#_#_#_#_#_#___#___#_#_____#_#___#___#_#_#_______#_______#_________________#___#_#___#_#_#_#___#_____#_#___#_#_________#_#___#_____#_______#_#_#___#_#_____#_#
#_###########_###_#######_#_#_#_###_#_###_#######_#_#_###_#_#_###_#####_#_#_#_#_###_#_#_#_#######_#_#_#_###_###_#_#####_#########_#####_###_#_###_#_###_###_#_###############_#_#_#######_#####_#_#####_#########_###_#_#_#_#####_###_#_#_###########_#_#_#_#_###_#_###_#_#_###_#_#_#####_#_#########_#####_#_###############_#####_###_#_#_#_#####_#_#####_###_#########_#_#_#####_###_#_#####_#_#####_#_###_#_#
#_#_______#___#_#_#_______#_#_#_#___#_#_______#_#_#_#_____#_#___#_#_____#_#_#_#_#___#___#_#_____#_#_#_#_____#___#_____#_____#___#_________#___#_#_#___#_____#___#___#_#_______#_#_____#___#___#_#_______#_______#___#_#_#___#___#___#_#_#_____#_______#___#_#_____#_____#_#_#_#___#_____#_________#___#_#___#___#___#___#_#___#___#___#___#___#_#___#_____#_#_____#___#___#_#___#___#___#_____#_________#___#___#
#_#_#######_###_#_#_#######_###_#####_###_###_#_#_#_#_#####_###_#_#_#####_#_#_#_#_###_#####_###_#_#_#########_#_#####_#####_#_#_#_#########_###_#_#_#_#######_#_#_#_#_#_#######_#####_#_###_#_#_#######_#_###_#_###_#_#_#####_#_#_#_#_#_#####_#_#######_###_#####_#########_#_#########_#_#########_###_#_#######_#_#_#_#_#_###_#_###_#########_#_#######_#_#_###_###_#_###_###_#_#_#_#######_#_#########_#_###_#
#_#_#_____#_#_#___#_____#_#___#_#___#___#_#___#_____#_#___#_#___#___#_____#_#_#_#___#_#___#___#_#_#_____#_____#_____#___#_#___#_#_____#___#___#_#_#_#___#___#_#___#_#___#___#_______#___#___#_#_#_____#_#_#___#___#___#_____#_#_#_#_#_#_#___#___#_______#_______#___#_______#___________#_#_________#___________#_#___#_#_#___#_#___#_#___#___#___#_____#___#_#_____#_#_#_____#_#_#_#_#_____#_#_#_______#_#___#_#
#_#_#_###_#_#_#_#######_#_###_#_#_#_###_###_#########_#_#_###_#_#####_#####_#_#####_###_#_###_#_#_###_###_#_#######_###_#_#####_#######_#_###_#_#_###_#_#_#_#_#####_#_#####_#_###_#########_#_#_#####_#_#_#_#####_#########_#_#_#_#_#_#_#_#_#####_#########_#_#####_#####_#_#########_#####_#########_#########_#_#####_#_###_###_###_#_###_#_#_#######_#####_#####_#_#_###_###_#_#_#_###_###_#_#_#####_#####_#_#
#_#_____#_#_#_________#_#___#_#___#_______#_____#_#___#_#___#_#___#_______#_#_______#___#_#___#_#___#___#_#_#_______#___#_____#___#_____#___#___#_____#___#_#_____#_#_____#___#_#_#_________#___#_____#_#_#___#___#_______#_#_#_#_#_#_#_#_#___#___#_______#_#___#___#_____#_#_______#_#_____#___#___#_#_____#_#___#_#___#_________#___#_____#___#_________#_____#_____#___#_____#_#_#___#_#___#_#_#___#___#___#_#
#_#######_#_#########_#_###_#_###########_#_###_#_#_###_###_#_#####_#####_#########_#_###_#_###_#######_#_#_#_#######_#######_###_#_#######_###_###########_#####_#_#####_###_#_#_#_###_#########_#####_#_###_#_###_#####_#_#_#_###_#_#_#_###_#_###_#####_#####_#_###_#######_#####_#_###_###_#_#_#_#_#_###_#_#####_#_#############_###_#########_#######_#####_#####_###_#_#####_#####_#_#_###_#_#_#_###_#_###_#
#_________#___#___#___#___#_#_#_________#_#___#_#___#___#_#_#_____#_#___#_#___#_____#___#___#_______#___#_#_#_______#_____#___#___#_#_____#___#_#_____#_#___#_____#_____#___#___#_____#_#___#___________#_#_#_#_#_#___#___#_#_#___#_#_#_#___#_#___#_#_____#___#_____#___________#___#___#___#_#_#_#___#_#___#___________#_____#_____#_#_____#_____#_#_____#___#_____#___#_#_____#_____#___#_____#_#_#_______#___#
#########_###_#_#_#######_#_#_#_#######_#_###_#_#_###_###_#_#####_#_#_#_#_#_#_#_#######_#####_#####_#_#_#_#_#######_#####_#_#_#_###_#_#_#####_#_#_###_#_#_###_#########_###_###########_#_###_###########_#_#_#_#_###_#_#_#_#_###_#_#_#_#_#_#####_#_#_#####_#_#####_#_###########_#####_###_###_#_#####_#_###_#########_#_###_#_#####_#####_###_###_#_#####_#_#####_###_#_#####_###_#_###_#######_###############
#_____#___#___#_#_#_____#___#___#_____#_#___#_#_#_____#___#_#___#_#___#_#_#_#_#_#_____#___#___#___#___#_#_#_#_____#___#___#_#_#_____#_#_#_____#___#_____#___#_____#_____#_______________#_____#_______#_____#_#_#___#_#_#_#_#___#_#___#_#_#_____#___#___#___#_____#_#___#_#_______#___#___#___#___#_#___#_#_#___#___#_#_#___#___#___#_____#_____#_#___#_____#___#___#___#_#_________#___#_____#_#___#___________#
#_###_#_###_###_#_#_###_###_#####_###_#_###_###_#######_#_#_#_#_#_#####_#_#_#_#_#_###_###_#_###_#_###_###_#_#_###_###_#_###_#_#######_#_#_#########_#######_#######_###############_#####_#####_###_#_#######_#_#_###_#_#_#_###_#_###_#_#######_#######_#_#_#####_#_###_#_#_#####_#_#_###_#_#_#_###_#_###_#_#_###_#_#_#_###_#####_#_#_###_#######_#_#######_###_#_###_###_#############_#####_#_###_#_#########_#
#_#___#___#_#___#___#___#___#_____#___#___#___#___#_____#_#___#_#_____#_#___#_#_#_#_#___#_#_#___#_#___#___#_____#_#___#_____#_#_____#_#_#_____#___#_#_____#___#___#_______#_____#___#___#_____#_#___#_____#___#_#_#___#_#_#_____#___#_#___#_____#___#___#_#___#___#_#_____#_#_____#_#_#_#_#_#_#_____#_#_#_#___#___#_#_____#___#___#___#_____#_____#_#_______#_____#_#_#___#___________#_____#___#_#_________#___#
#_###_#####_#_#######_###_#####_###_#####_###_#_#_###_###_#####_#_#_#_#_#####_#_#_#_###_#_###_###_#_###_#########_#_#########_#_###_#_#_#####_#_#_#_#_###_###_#_#_#######_#_###_#_###_#_#####_###_#######_#_###_#_#_###_###########_#####_#_#####_#_#_#####_#_#_###_#######_#####_#_#_#_#_###_#######_#_#_#####_###_#####_###_#_#######_#####_###_#_#_#_###########_#_#_#_#_#########_###_#####_#_###########_###
#___#_______#___#_____#___#___#_#_#_#___#___#_#_#___#___#_#___#_#_#_#_#_#_#___#_#_#_____#_______#_#_#___#_________#_#_______#_#_#_#___#_____#_#_#___#___#_____#_#_______#_____#_#___#_#___#_______#___#_____#_#_#___#___#_________#___#___#_______#_#_____#_#_#___#_________#___#_#_#_#_______#_____#_#_#_#_____#_#___#___#___#_#_____#_#_____#_#_#_#_#_#_________#_#_#_#_#___#_____#_#___#___#_#_#_________#___#
###_#######_###_#_###_#_###_#_#_#_#_#_#_###_#_#####_#_#_#_#_#_#_###_###_#_#_###_#_###############_#_#_###_#########_#_#_#_###_#_#_#####_###_#_###_#####_#######_#######_#####_#_#_###_###_#########_#_#######_#_#_###_###_###_#######_#_###_#######_#####_###_###_###########_#_###_#_#########_###_#_#_#_#_#####_###_#_###_###_#_###_#_#_#####_#_#_###_#_#######_#_#_#_#####_#_#####_#####_#_#_#_#_#######_#_#_#
#___#_____#_#_#_#___#_#_#___#___#___#_#_#___#___#___#_#_#___#_#___#_#___#_#_____#_#___#_______#___#_#___#_#_______#_#_#_#_#___#_#_#_____#_#_#___#_#___#_#_______#_____#___#_#_#_#_#_____#_#_____#___#_#___#_____#___#_#___#_#_#_______#_#_________#_____#_____#_#_____________#_#___#_________#___#___#_____#_____#___#_#___#___#___#_#_#_______#_#___#_________#_#___#_____#_#_____#_______#___#_#_#_____#_#_#_#
#_###_###_#_#_#_###_###_###_#_#####_#_###_#####_#_#####_#####_###_#_#_###_#######_#_#_#_#####_#_#_#####_#_#######_#_###_###_###_#_#_#####_#_###_#_###_#_#_#_#####_#######_#_#_#_###_#####_#_###_#_###_#_#_#####_#####_#_###_#_#_#######_#######_###_###########_###############_#_###########_#_#_#############_#_#_###_#_###_#####_#_#########_#_###_###########_#######_#_#_#_###_#############_#_#_###_#_###_#
#___#___#_#_#_#___#___#_#___#_#___#___#___#_#___#_____#_____#_#_#___#_#_________#_#_#_____#___#_#_#_____#___#_____#___#___#_#_#_#_#_#_____#___#_#_____#_#_#___#_#_______#_#___#_______#___#_#_#___#_#___#_____#_______#_____#___#_________#___#_#___#_______________#_____#_____#_#_#_______#_#_#_______#_____#_#_#___#___#_#_____#_#___#_______#___#_#_________#_#_____#_#_#_#___#_____#_____#___#_#___#_#___#_#
#_#_###_#_#_#_###_###_#_#_#####_#_#####_###_#_#######_#####_#_#_###_#_#_#######_#_#_#######_###_###_#######_#_#######_###_#_#_#_#_#_#_###_#_###_#####_#_#_###_#_#_###_###_#########_###_###_#_#####_#########_#############_#############_#_#_###_###_#####_#######_#_###_#_#####_#_#_###_###_#_#######_#_###_#_#_###_#####_#_#_###_###_#_#######_###_#_#######_#_#_###_#_#_#_#####_###_#_###_#_#_#_###_#_###_#_#
#_#_#___#_#_____#_#_#___#_#___#_#_#_____#___#_________#_____#___#___#___#_#_____#_____#_____#___#___#_____#_#_#___________#_#_____#_#_#_#___#___#_____#_#___#_#_____#_________#___#_#___#___#_#_____#_____#___#___________#___#___________#_#_#___#_____#___#_____#___#___#_____#_#_____#_____#___#_____#_#___#_#___#_____#___#_______#_____#___#___#___#_#_______#_#_#_#_#_#_#_____#_#_#_#_____#_#___#_#___#___#
#_#_###_#_#_#####_#_#####_#_#_#_#_#_#######_###########_#######_#_#######_#_###########_###_#_###_#######_#_#_#_#####_#####_#######_#_#_#####_#####_###_#####_#####_#########_#_#_#_#_#_#_###_#_#_###_#_#_#_###_#####_#######_#_###########_#_#_###_#####_#_#_###_###_#########_#_#_#########_#####_#####_#_#######_#####_###########_#######_#_###_#####_#_#######_#_#_###_#_#_#####_#_#_#######_###_###_#####_#
#_#___#_#___#___#___#___#___#___#_#___#_____#_______#_________#_#_#_____#___#_________#_#___#___#_____#___#_#___#___#_#___#_________#_#_____#_____#___#_____#___#_____#_____#___#_#_#_#_#_#_____#___#_#_#_#___#_____#_____#___#_______#___#_#___#_____#___#_#___#_#___#_________#_#___#_____#_____#_#___#_#_#_____#_#_____#_______#_#_____#___#_____#___#___#_______#_#_#___#_#_#___#_#_#_____#___#_#___#_______#
#_###_#_#####_#_###_###_#_#######_###_#_#####_###_#_#_#######_#_#_###_#_#_#######_###_#_#_#####_#####_#_###_#####_#_###_#_###########_#_#########_#########_###_#####_#_###_#####_###_###_#_#######_#_#_#####_#####_#####_#_#####_###_#_#_#_#_###_#####_#####_###_#_###_#########_#####_###_#####_#_#_#_#_#_#_###_#_#_#####_#####_#_#_#####_#######_#_#_#_###_#######_#_#_###_#_#_#_#_#_#####_###_#_###_#_#######
#___#_#_#_____#_____#___#___#_____#___#_________#_#_#___#_____#_#___#_#_#_#_______#_#___#_#_______#_#_#___#_______#_____#_________#___#_#_______#_________#___#___#_#_#___#_#___#___#___#_#___#___#_#_#_____#_____#___#___#_#___#_#___#_#___#_#___#_#___#___#_#___#_#_#___#_#_____________#_#_____#_#_#___#_____#_#_#___#_#___#___#_#_#___#_______#___#_#___#_#_____#___#_______#_#___#_#___#___#_#___#_#_#_____#
#####_#_#_###########_###_###_###_#_#############_#####_#_#########_#_###_#_#_#####_#####_#_#####_#_#_###_###############_#######_#_###_#_###_###########_###_###_#_#_#_###_#_#####_#_#_#_###_#_#_#_#_#####_#####_#_###_#_#_#_#_###_###_#####_#_###_#_###_#_#_#_###_#_###_#_#_#############_###_###_#_###########_#_###_#_#_#_#_###_#_#_#_#####_#_#####_###_#_###_###_###########_###_#_#_#_###_#_###_#_#_#_###_#
#_____#_#___#_____#___#___#___#___#_#___#_______#_______#___________#___#_#_#_#___#_____#_#___#_____#_____#_______#_____#_______#_#_#_____#_#___________#___#_#___#___#_#___#_______#_#_#___#_#_#___#_#___#___#___#_#___#_#_#_#_____#___#_____#_#___#_____#___#___#_____#_#_#_#___#___#___#___#_#___#_____#_#_____#___#_#___#_#_#_#___#_#_____#_#_____#_#___#___#___#___________#___#_#_#_#_____#_____#_#___#___#
#_#####_###_#_###_#_#_#_###_#######_#_#_#_#####_#######_###############_#_###_#_#_#_#####_###_###########_###_###_###_#_#######_###_#_#####_###########_###_#_#_###_###_#_#######_#####_###_###_#####_#_#_#_###_###_#_#_###_#_#######_#_#####_#_###_#############_#####_#_#_#_#_###_#_#_#_###_#_#_#######_#_#_#####_###_#####_#_#_#_###_#####_#_#####_###_#####_###_###########_#_#_###_#_#############_#####_###
#___#___#_#_#___#___#___#_#_#_____#___#_#_____#_______#_#___#___#_______#_____#_#___#_______#___#_____#_#_____#_#___#_#_______#_#___#_#_____#_____________#_#_#_#_____#_#_#_____#_________#___#_______#_#_#___#_____#_#_#___#_#___#___#_#___#_#___#___________#___#___#_#_#___#___#_#___#_____#_#_______#___#_#_#_____#_______#_#_____#___#___#_____#_#___#___#_#_____#_______#_#_#_#___#___#_______#___#___#___#
#_#_#_###_#_#########_###_#_#_###_#####_#####_#######_#_#_#_#_###_###_#_#######_#_###_#########_#_#_#_#_#######_###_###_###_#_#_#_###_#_#_#_#_#_#######_###_#_#_#####_#_#_#_###_#########_###_#########_#_###_#######_###_###_#_#_#_###_#_#_#_###_#_#####_#####_###_###_#_###_#_#_#_###########_###_#######_#_#_#_###_#########_#########_#_#######_#_#_###_#_#_#_#####_#####_#_###_#_#####_#_#####_#_###_#_###_#
#_#_#_#___#___#_____#_#___#_____#_____#_____#___#___#_#_#_#_#_______#_#___#___#_#_#_____#_____#___#_#_#___#_______#___#_#___#_#___#_#_#_#_#_#_#_#_____#_#___#_#_#___#_#_#_____#_#___________#_____#_____#_#_____#___#_____#_____#_#_#___#_#_#_#___#_#_#___#___#_#_#_____#___#_#_#_#_____#_____#_____#_______#_#_#_#___#_________#_____#___#_______#_#___#___#___#_#___#_#___#_#___#_________#___#___#_#___#_#___#
###_#_#_#_###_#_###_#_#_#######_#####_#_#######_#_###_#_#_#_#########_###_#_#_#_###_#####_###_#####_#_###_#_###_#####_#_#_#########_#_###_###_#_#_###_#_#_###_#_#_#_#_#_#####_#_###########_#####_#_#####_#_###_#_#########_#####_#_#####_#_###_###_#_#_###_#_#_#_#_#######_#_###_#####_#_###_#######_#######_#_#_#_#_#_#########_###_#_#########_#######_#######_#_#_#_#_#_#_###_#####_#######_#_###_#_###_###_#
#___#___#___#___#_#_#___#___#_______#_#_____#_____#___#___#_____#___#___#___#___#___#_____#_#_______#_____#_#___#_____#_#_________#_______#___#_#_#_#_#_#_#___#_#_#_#_#_#___#_#_______#___#_#_____#_#___#_#_#_#_#_________#_#___#_#_______#___#___#_#_#___#_#___#_#_#_____#_#_________#_____#___#___________#_#___#_#_#_#_____#___#___#___#_____#_#_______#_#_____#_#___#_#_#_____#___#_#_____#_#_#_______#___#_#
#_###_#####_#####_#_###_#_#_#_#######_#####_#_#####_#_#########_#_#_###_###_#####_###_#####_#_#############_#_#_#_#######_###_###_#_#######_#####_#_#_#_#_#_###_#_#_#_#_#_#_#########_#_#_#_#_#####_###_#_#_#_#_#####_###_#_#_#_#_#_#########_###_#_#_###_#_#####_#_#_#_###_#_#######_#######_#_#_#######_###_#_###_#_#_#_#_#_#_###_#####_###_###_#_#######_#_###_#_#######_#######_#_#_#_###_#_#_#######_###_#_#
#_#___#___#_____#_#_#___#_#_#_#_______#_#___#_#_#___#_#_____#_____#_____#___#_____#___#___#___#___#_________#_#_#_#_____#_#___#_#_#___#___#_________#_#_#_#___#___#_#_#_#_#_________#___#_#_#_____#___#_#___#_#_____#___#_#___#_#_#___#_____#_____#___#_#_#_#___#_____#_#___#_#___#_#_#_____#_#_____#___#_#___#_#___#_#_#_#_#_#_#_________#___#___#___#_____#_#_#_#_#_______#_____#_#_#_#_#_#___#_#___#___#___#_#
#_###_#_#_#####_#_#_#####_#_###_#####_#_#_#_#_#_#_#####_###_#################_#####_###_#_#_###_#_#_#########_###_#_###_###_###_#_###_#_#_#_#########_###_###_#####_###_#_#########_###_#_#_#####_#_#_#_#####_#####_###_#_###_###_###_#_#############_#_#_#_#_#_#########_###_#_#_#_#_#_###_#_#######_#_###_###_#_#_###_#_#_#_#_###########_###_#####_###_#_#_#_#_#_#_###_#_#_###_#_#_###_#_#####_#_#_#_###_###_#
#___#_#_#_____#___#___#___#_#___#___#_#_#_#_#___#___#___#_#___________#___#___#___#_#___#_____#_#_#___#_____#___#___#_#___#___#_#___#_#_#_#___#_______#_____#_____#_____#_______#_#___#_#_#___#___#_#_#_________#___#___#___#_#___#_#_#_____________#___#_#___#_#_________#___#_#_#___#_#_#_#_#_______#___#___#_#_#_#___#_#_#_#_____#_______#___#___#___#_#_#_#_#_#_#___#_#___#_#___#_____#___#___#_#_#___#_#___#
#_#_###_#####_#######_#_###_#_###_#_#_#_#_#####_###_#_###_###########_#_#_#_#####_#_###########_#_###_###_#_###_#####_###_###_#_###_###_#_#####_#####_#_#########_#############_#_###_###_###_#_#####_#_###_#####_###_#####_###_###_#_###########_#_#####_#####_#_#############_#_#####_#_#_###_#########_###_#_#_###_#####_#_#####_#_#####_#_###_#_###_#_#_#_#_#_#_#####_#####_###########_#_#_###_#_#####_#_###
#_#_________#___________#_____#___#_____#_________#_________________#___#_________#_____________#_________#___#_______________#_________#_______#_____#_________________________#_________#___#_______#___#___________#_________#_________________#___________#_________________#_______#_______#_____________#_#___________#_______#_____#_______#_____#_#_____#_________#_________________#_______#_______#____
#################################################################################################################################################################################################################################################################################################################################################################################################################
", 5, 5);
        }
    }
    
    private void StartGame(){
        InMainMenu = false;
        
        WorldPosition = Vector2F.Zero;
        __Tracks.Clear();

        Health = HealthMax;
        Interface = 0;

        SelectedItem = 0;

        LastHealed = 0;
        Rotten = 0;

        Emotion_Happiness = Emotion_Max;
        
        Array.Clear(Inventory, 0, Inventory.Length);
        Inventory[0] = T_Item.FirstAidKit;
        Inventory[1] = T_Item.FirstAidKit;
        Inventory[2] = T_Item.FirstAidKit;
        
        StartLevel(1);
    }
    
    private bool RenderColliders = false;
    public override void KeyPress(Key Key, bool Down){
        if(Down){
            if(InMainMenu){
                if(Key is Key.Enter or Key.Space){ StartGame(); }
            }else{
                if(Key == Key.C){ RenderColliders = !RenderColliders; }

                if(Key == Key.Escape){
                    if(Interface == T_Interface.None){ StartLevel(0); InMainMenu = true; }else{ Interface = T_Interface.None; }
                }

                if(!Dead){
                    if(Key == Key.Tab){ Interface = Interface == T_Interface.None ? T_Interface.Inventory : T_Interface.None; }
                    
                    if(Key == Key.Enter){ UseItem(); }
                    
                    if(Key == Key.E){ Use(); }

                    if(Key == Key.Backspace){
                        T_Item Item = Inventory[SelectedItem];
                        if(Item != T_Item.Empty){
                            SpawnItem(PlayerX - WorldX, PlayerY - WorldY, Item);
                            Inventory[SelectedItem] = T_Item.Empty;
                        }
                    }
                }

                if(Interface == T_Interface.Inventory){
                    if(Key == Key.D){
                        if(SelectedItem > 5){
                            if(SelectedItem < 11){ SelectedItem++; }
                        }else{
                            if(SelectedItem < 5){ SelectedItem++; }
                        }
                    }

                    if(Key == Key.A){
                        if(SelectedItem > 5){
                            if(SelectedItem > 6){ SelectedItem--; }
                        }else{
                            if(SelectedItem > 0){ SelectedItem--; }
                        }
                    }

                    if(Key == Key.S){
                        if(SelectedItem + 6 < MaxSlots){ SelectedItem += 6; }
                    }
                    
                    if(Key == Key.W){
                        if(SelectedItem - 6 > -1){ SelectedItem -= 6; }
                    }
                }
            }
        }
    }

    private void UseItem(){
        T_Item Item = Inventory[SelectedItem];

        if(Item != T_Item.Empty){
            bool RemoveItem = false;
            
            switch(Item){
                case T_Item.FirstAidKit:{
                    if(Health == HealthMax){ return; }
                    
                    Heal(50, true);
                    
                    RemoveItem = true;
                    break;
                }
            }

            if(RemoveItem){
                Inventory[SelectedItem] = 0;
            }
        }
    }

    private bool AddToInventory(T_Item Item){
        for(int i = 0; i < Inventory.Length; i++){
            if(Inventory[i] == T_Item.Empty){
                Inventory[i] = Item;
                return true;
            }
        }
        
        return false;
    }
    
    private void Use(){
        if(InsideCollision == CollisionLayer.L4){
            T_Item Item = (T_Item)CollisionInfo;
            if(Item != T_Item.Empty){
                if(AddToInventory(Item)){ __Entity.RemoveAt(CollisionInfoSecond); }
            }
        }
    }
}
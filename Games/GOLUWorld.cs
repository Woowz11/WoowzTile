using WL;
using WLO;
using WoowzTile;
using WoowzTile.Objects;
using Char = WoowzTile.Objects.Char;

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
    Tree = 5
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
    Blood = 1
}

public class GOLUWorld : Game{
    public Palette Palette_World;

    public Font Font;
    
    public Texture Texture_Ground;
    public Texture Texture_Metal;
    public Texture Texture_Player;
    public Texture Texture_Player_Blink;
    public Texture Texture_Player_Blood;
    public Texture Texture_Player_Blood_Blink;
    public Texture Texture_Planks;
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
    public Texture Texture_Spider_Anim;
    public Texture Texture_Asphalt;
    public Texture Texture_Bricks;
    public Texture Texture_Sand;
    public Texture Texture_Water;
    public Texture Texture_Water_Top;
    public Texture Texture_Water_Anim;
    public Texture Texture_Water_Top_Anim;
    public Texture Texture_Tree;
    public Texture Texture_Tree_Leaves;
    public Texture Texture_FirstAidKit;
    public Texture Texture_FirstAidKit_Icon;
    public Texture Texture_Player_Healed;
    
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
     */
    
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
            new KeyValuePair<byte, ColorB>(10, ColorB.DarkMagenta),
            new KeyValuePair<byte, ColorB>(11, ColorB.LightRed)
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
            ['l'] = 11
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
@".....
.....
.....
.....
.....
.....
.....
.....", Mapping))),
            
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
....█████████...
...█▒▒▒░░▒▒▒▒█..
..█░░░____▒▓█▓█.
.██▒▒░░░░░▒█▒▒█.
.█▒▓▒▒▒▒▓▒██░▒█.
.█░▓████▓█▒▒░▒█.
.█░▓░___▓_▒▒░▒█.
.█░__░r░__░░░▒█.
.█░__rRR___▒░█..
..█░__R___▒▒▒█..
...█▒▒__░▒▓██...
....███████.....
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

    private int PlayerX => (int)(Game.SceneSize.X / 2F - Texture_Player.Width  / 2F);
    private int PlayerY => (int)(Game.SceneSize.Y / 2F - Texture_Player.Height / 2F);
    
    private int WorldX => (int)(WorldPosition.X + Game.SceneSize.X / 2F);
    private int WorldY => (int)(WorldPosition.Y + Game.SceneSize.Y / 2F);

    private bool     Moving => MovingDirection != Vector2I.Zero;
    private Vector2I MovingDirection = Vector2I.Zero;

    private const uint HealthMax   = 100;
    private const uint HealthSmall = 30;
    private       uint Health      = HealthMax;

    private bool InMainMenu = true;
    private T_Interface Interface = T_Interface.None;

    private bool Dead => Health == 0;

    private bool StopTime = false;

    private const byte MaxSlots = 12;
    private byte SelectedItem   = 0;

    private T_Item[] Inventory = new T_Item[MaxSlots];

    private float LastHealed = 0;
    
    private void Damage(uint Damage, int Range = 0){
        Health = WL.Math.SubU(Health, Damage);

        SplatBlood(PlayerX - WorldX + WL.Math.Random.Fast_Int(-Range, Range), PlayerY - WorldY + WL.Math.Random.Fast_Int(-Range, Range));
    }
    
    private void Heal(uint Heal){
        Health += Heal;
        if(Health > HealthMax){ Health = HealthMax; }

        LastHealed = 60;
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
    
    public override void Update(TickData TD){
        Game.ClearColliders();

        if(InMainMenu){
            return;
        }

        StopTime = Interface != 0;

        if(StopTime){ return; }
        
        if(!Dead){
            Health = Health >= HealthMax ? HealthMax : Health + (uint)(WL.Math.Random.Fast_Bool(0.001f) ? 1 : 0);
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
                Game.AddCollider(new Collider(WorldX + Entity.X + (int)((16 - SizeX)/2), WorldY + Entity.Y + (int)((16 - SizeY)/2), SizeX, SizeY, Layer));
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
    
    private void AddEntity(Entity Entity__){
        Entity__.X *= 16;
        Entity__.Y *= 16;

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
            Texture Track__ = Track.Item3 == T_Decal.Blood ? Texture_Blood : Texture_Track;
            Track__.Render(C, Palette_World, WorldX + Track.Item1, WorldY + Track.Item2, false, false, Track.Item4);
        }
        
        foreach(Entity Entity in __Entity){
            if(Entity.ID is T_Entity.Chair or T_Entity.Table or T_Entity.Spikes or T_Entity.Tree){
                Texture EntityTexture = Entity.ID switch{
                    T_Entity.Chair  => Texture_Chair,
                    T_Entity.Table  => Texture_Table,
                    T_Entity.Spikes => Texture_Spikes,
                    T_Entity.Tree   => Texture_Tree
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

        if(Health < HealthSmall){
            if(Player == Texture_Player){ Player = Texture_Player_Blood; }
            else if(Player == Texture_Player_Blink){ Player = Texture_Player_Blood_Blink; }
        }
        
        if(MovingDirection.X != 0){
            PlayerFlipped = MovingDirection.X > 0;
        }
        
        T_Item Item = Inventory[SelectedItem];
        if(Item != T_Item.Empty){
            Texture ItemTexture = Item switch{
                T_Item.FirstAidKit => Texture_FirstAidKit
            };
            
            ItemTexture.Render(C, Palette_World, PlayerX, PlayerY - 11, PlayerFlipped);
        }
        
        Player.Render(C, Palette_World, PlayerX, PlayerY, PlayerFlipped);

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

            ColorB FrameColor = new ColorB((byte)(WL.Math.DSin((float)TD.DeltaTick * 2) * 255), 0, 0);

            C.Border(0, 0, C.Width, C.Height, 1, FrameColor);
            C.Border(1, 1, C.Width - 2, C.Height - 2, 1, FrameColor.Clone().SetA(128), ImageBlend.Alpha);
            C.Border(2, 2, C.Width - 4, C.Height - 4, 1, FrameColor.Clone().SetA(64), ImageBlend.Alpha);
            
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
                            T_Item.FirstAidKit => "ЛЕЧИТ БЕДНЫЙ КУБИК ГУЛУ"
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

    public override ColorB BackgroundColor(){
        return ColorB.White;
    }

    private void StartLevel(byte Level){
        ClearAllEntityScene();
        ClearAllScene();

        if(Level == 1){

            AddScene(
                @"
AAAAAAAAAABBBBBBBSSSSSSS________
_________________________WWWWW__
________________________WWWWWWWW
________________________WWWWWWW_
_________________________WWWW__");
            
            AddEntityScene(
@"
________________________________
________________________________
_____________________!__________
___________________!___!________
_____________________!__!_______");
            
            AddScene(@"#'''#####
#'''#''''
#'''#''''
#'''#''''
#########
''''#'''#
''''#'''#
#####'''#
____#'#__
#'''#'###
#'''''''#
#'''''''#
##'##'''#
#'''#'''#
#'''#'''#
#'''#'''#
######'##
", 2, 2);

            AddEntityScene(@"_________
_________
_________
_________
____s____
_________
_________
_________
^____^___
_____^___
_^^^_____
_________
_________
__C__C___
__T__Cs__
__s__C___
_________", 2, 2);
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
                    
                    Heal(50);
                    
                    RemoveItem = true;
                    break;
                }
            }

            if(RemoveItem){
                Inventory[SelectedItem] = 0;
            }
        }
    }
}
using WLO;
using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Resources;

namespace GOLUWorld;

internal static class GOLUWorld_Objects{
    /*
     * Блоки:
     * '_' - Пустота
     * '#' - Блок металла (блок)
     * ''' - Доски (пол)
     * 'A' - Асфальт (пол)
     * 'B' - Кирпичи (блок)
     * 'S' - Песок (пол)
     * 'W' - Вода (блок)
     * 'b' - Чёрный блок (блок)
     * '^' - Трава (пол)
     * 'Д' - [ГЕНЕРАТОР] Случайно, трава или пустота
     * 'П' - [ГЕНЕРАТОР] Случайно, песок или пустота
     *
     * Сущности:
     * '_' - Пустота
     * 'C' - Стул
     * 'T' - Стол
     * '^' - Шипы
     * 's' - Паук (моб)
     * '!' - Дерево
     * '#' - Ящик (толкаемый блок)
     * '~' - Трава
     * '3' - Куст
     * 'Д' - [ГЕНЕРАТОР] Случайно, дерево или трава или палка или куст или пустота
     * 'д' - [ГЕНЕРАТОР] Случайно, трава или камень или куст или пустота
     *
     * Коллизии:
     * L1 - Мир и игрок
     * L2 - Наносит урон если ходить в нём
     * L3 - Наносит всегда урон
     * L4 - Предмет
     * L5 - Толкаемый блок
     */
    
    internal enum T_Block : byte{
        Empty          = 0,
        Metal          = 1,
        Ground_Planks  = 2,
        Ground_Asphalt = 3,
        Bricks         = 4,
        Ground_Sand    = 5,
        Water          = 6,
        Black          = 7,
        Ground_Grass   = 8,
        Error          = 9
    }

    internal enum T_Entity : byte{
        Empty      = 0,
        Chair      = 1,
        Table      = 2,
        Spikes     = 3,
        Mob_Spider = 4,
        Tree       = 5,
        Item       = 6,
        Crate      = 7,
        Grass      = 8,
        Bush       = 9,
        Error      = 10,
        Rock       = 11
    }

    internal enum T_Item : byte{
        Empty       = 0,
        FirstAidKit = 1,
        GPS         = 2,
        Error       = 3,
        Stick       = 4
    }

    internal enum T_Interface : byte{
        None      = 0,
        Inventory = 1,
        Menu      = 2
    }

    internal enum T_Decal : byte{
        FootStep = 0,
        Blood    = 1,
        Zero     = 2,
        One      = 3
    }

    internal enum T_Emotion : byte{
        Happiness = 0
    }

    internal enum T_Thoughts : byte{
        Idle   = 0,
        Damage = 1,
        Heal   = 2
    }

    internal enum T_World : byte{
        None = 0,
    
        /* Спокойный уровень, чёрно-белый, больше белого, земляной покров, стены из металла и кирпичей */
        Calm = 1,
        /* Тёмно-серый, красный уровень, металлические стены, трубы, завод */
        Industrial = 2,
        /* Лабиринт, из стекла и зеркал */
        Labyrinth = 3,
        /* Пустыня залитая машинным маслом */
        OilDesert = 4,
        /* Электрический уровень, микросхемы, всё бьёт током */
        Electric = 5,
        /* Всё расплавленное, горячее */
        DangerHot = 6,
        /* Мир состоящий из глитчей */
        Glitch = 7,
        /* Закулисье))) */
        BackRooms = 8,
        /* Храм GOLU */
        GOLUTemple = 9,
        /* Мир WoowzCore */
        WoowzCore = 10
    }
    
    internal enum RenderableType{
        Tile  = 0,
        Tiles = 1
    }

    internal struct Block{
        public Block(){}
    
        internal int     X    = 0;
        internal int     Y    = 0;
        internal T_Block ID   = T_Block.Empty;
        internal byte    Info = 0;
    }
    
    internal struct Entity{
        public Entity(){}

        internal int             X          = 0;
        internal int             Y          = 0;
        internal T_Entity        ID         = T_Entity.Empty;
        internal byte            Info       = 0;
        internal Vector2I        InfoVector = Vector2I.Zero;
        internal TextureRotation Rotation   = TextureRotation.None;
    }
    
    internal struct Decal{
        public Decal(){}

        internal int             X        = 0;
        internal int             Y        = 0;
        internal T_Decal        ID        = T_Decal.FootStep;
        internal TextureRotation Rotation = TextureRotation.None;
    }
    
    internal struct Renderable{
        public Renderable(){}
        
        internal int             X              = 0;
        internal int             Y              = 0;
        internal uint            W              = 0;
        internal uint            H              = 0;
        internal Palette         Palette        = Palette_World;
        internal Texture         Texture        = Texture_Black;
        internal TextureRotation Rotation       = TextureRotation.None;
        internal RenderableType  Type           = RenderableType.Tile;
        internal int             Z              = 0;
        internal bool            FlipX          = false;
        internal bool            FlipY          = false;
        internal ColorB?         MultiplyColor  = null;
        internal bool            Reflect        = false;
        internal Texture?        ReflectTexture = null;
    }
    
    internal struct Structure{
        internal Structure(string Blocks, string Entities = ""){
            this.Blocks = Blocks; this.Entities = Entities;
        }
        
        internal string Blocks{
            get => __Blocks;
            set{
                __Blocks = value;
                __CalculateSize();
            }
        }
        internal string __Blocks = "";

        internal string Entities{
            get => __Entities;
            set{
                __Entities = value;
                __CalculateSize();
            }
        }
        internal string __Entities = "";
        
        internal uint Width { get; private set; }
        internal uint Height{ get; private set; }

        internal void __CalculateSize(){
            string[] Lines1 = Blocks  .Replace("\r", "").Split('\n');
            string[] Lines2 = Entities.Replace("\r", "").Split('\n');

            Width  = (uint)WL.Math.MaxI(Lines1.Max(Line => Line.Length), Lines2.Max(Line => Line.Length));
            Height = (uint)WL.Math.MaxI(Blocks.Count(C => C == '\n') + 1, Entities.Count(C => C == '\n') + 1);
        }
    }
    
    internal struct EntityKey{
        internal EntityKey(Vector2I Position, bool HasUniqueID = false){
            this.Position = Position;
            UniqueID = HasUniqueID ? __TotalUniqueID++ : 0;
        }
        
        internal EntityKey(Vector2I Position, uint UniqueID){
            this.Position = Position;
            this.UniqueID = UniqueID;
        }
        
        internal readonly Vector2I Position;
        internal readonly uint     UniqueID;
    }
    internal static uint __TotalUniqueID = 1;
}
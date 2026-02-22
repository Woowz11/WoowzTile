using WLO;
using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Resources;

namespace GOLUWorld;

internal static class GOLUWorld_Objects{
    internal enum T_Block : byte{
        Empty              = 0,
        Metal              = 1,
        Ground_Planks      = 2,
        Ground_Asphalt     = 3,
        Bricks             = 4,
        Ground_Sand        = 5,
        Water              = 6,
        Black              = 7,
        Ground_Grass       = 8,
        Error              = 9,
        Concrete           = 10,
        Ground_Cobblestone = 11,
        Pit                = 12,
        Ground_Tiles       = 13
    }

    internal enum T_Entity : byte{
        Empty      = 0,
        Chair      = 1,
        Table      = 2,
        Spikes     = 3,
        /* Моб паук, если Health < 0 то мёртвый */
        Mob_Spider = 4,
        /* Info == 1 значит это ель, Info == 2 значит это пень */
        Tree       = 5,
        /* Info отвечает за то какой это предмет (T_Item) */
        Item       = 6,
        /* Info == 1 значит ящик открытый */
        Crate      = 7,
        Grass      = 8,
        Bush       = 9,
        Error      = 10,
        /* Info == 1 значит окно забаррикадированное, если Health < 0 то разбивается */
        Window     = 11,
        TrashBag   = 12,
        Tire       = 13,
        HighGrass  = 14,
        Cattail    = 15,
        Grave      = 16,
        Door       = 17,
        Cardboard  = 18,
        /* Info отвечает за номинал (T_Money) */
        Money      = 19,
        Trapdoor   = 20,
        /* Info == 1 значит использованное */
        Trap       = 21,
        Mob_Drone  = 22,
        Debris     = 23,
        Fence      = 24,
        Nightstand = 25
    }
    
    internal enum T_Ceiling : byte{
        Empty     = 0,
        Invisible = 1,
        Concrete  = 2,
        Error     = 3,
        RoofTiles = 4
    }

    internal enum T_Item : byte{
        Empty       = 0,
        FirstAidKit = 1,
        GPS         = 2,
        Error       = 3,
        Stick       = 4,
        Crowbar     = 5,
        Rock        = 6,
        Destroyer   = 7,
        Clock       = 8,
        Mushroom    = 9,
        Battery     = 10,
        Pipe        = 11,
        Map         = 12
    }
    
    internal enum T_Money : byte{
        M1     = 0,
        M5     = 1,
        M10    = 2,
        M25    = 3,
        M50    = 4,
        M100   = 5,
        M250   = 6,
        M500   = 7,
        M1000  = 8,
        M2500  = 9,
        M5000  = 10,
        M10000 = 11
    }

    internal enum T_Decal : byte{
        FootStep       = 0,
        Blood          = 1,
        Zero           = 2,
        One            = 3,
        Glass          = 4,
        PlasticBag     = 5,
        Paper          = 6,
        BrokenTrashBag = 7,
    }
    
    internal enum T_Interface : byte{
        None      = 0,
        Inventory = 1,
        Menu      = 2,
        Console   = 3,
        Storage12 = 4
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
        /* Пустыня залитая машинным маслом (очень темно) */
        Abyss = 4,
        /* Электрический уровень, микросхемы, всё бьёт током */
        Electric = 5,
        /* Всё расплавленное, горячее */
        DangerHot = 6,
        /* Мир состоящий из глитчей и ошибок */
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
    
    internal struct Ceiling{
        public Ceiling(){}
    
        internal int       X    = 0;
        internal int       Y    = 0;
        internal T_Ceiling ID   = T_Ceiling.Empty;
        internal byte      Info = 0;
    }
    
    internal struct Entity{
        public Entity(){}

        internal int             X          = 0;
        internal int             Y          = 0;
        internal T_Entity        ID         = T_Entity.Empty;
        internal byte            Info       = 0;
        internal Vector2I        InfoVector = Vector2I.Zero;
        internal Data            InfoData   = new Data();
        internal TextureRotation Rotation   = TextureRotation.None;
        internal uint            Health     = 100;
        internal bool            Dead       => Health == 0;
        internal uint            UniqueID   = 0;

        internal EntityKey Key => new EntityKey(new Vector2I(X, Y), UniqueID);
    }
    
    internal struct Data{
        public Data(){}

        internal long I1 = 0;
        internal long I2 = 0;
        internal long I3 = 0;
        internal long I4 = 0;

        internal byte this[int Index]{
            get{
                if((uint)Index >= 32){ throw new Exception("Вышло [" + Index + "] за пределы Data при получении!"); }
                int Block = Index >> 3;
                int Shift = (Index & 7) << 3;

                long Value = Block switch{
                    0     => I1,
                    1     => I2,
                    2     => I3,
                    var _ => I4
                };

                return (byte)((Value >> Shift) & 0xFFL);
            }
            set{
                if ((uint)Index >= 32){ throw new Exception("Вышло [" + Index + "] за пределы Data при установке!"); }

                int Block = Index >> 3;
                int Shift = (Index & 7) << 3;

                long Mask = 0xFFL << Shift;

                switch (Block){
                    case  0: I1 = (I1 & ~Mask) | ((long)value << Shift); break;
                    case  1: I2 = (I2 & ~Mask) | ((long)value << Shift); break;
                    case  2: I3 = (I3 & ~Mask) | ((long)value << Shift); break;
                    default: I4 = (I4 & ~Mask) | ((long)value << Shift); break;
                }
            }
        }
    }
    
    internal struct Decal{
        public Decal(){}

        internal int             X        = 0;
        internal int             Y        = 0;
        internal T_Decal         ID       = T_Decal.FootStep;
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
        internal bool            RenderAnyway   = false;
    }
    
    internal struct Structure{
        internal Structure(string Blocks, string Entities = "", string Ceilings = ""){
            this.Blocks = Blocks; this.Entities = Entities; this.Ceilings = Ceilings;
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
        
        internal string Ceilings{
            get => __Ceilings;
            set{
                __Ceilings = value;
                __CalculateSize();
            }
        }
        internal string __Ceilings = "";
        
        internal uint Width { get; private set; }
        internal uint Height{ get; private set; }

        internal void __CalculateSize(){
            string[] Lines1 = Blocks  .Replace("\r", "").Split('\n');
            string[] Lines2 = Entities.Replace("\r", "").Split('\n');
            string[] Lines3 = Ceilings.Replace("\r", "").Split('\n');

            Width = (uint)WL.Math.MaxI(
                Lines1.Max(Line => Line.Length),
                Lines2.Max(Line => Line.Length),
                Lines3.Max(Line => Line.Length)
            );
            
            Height = (uint)WL.Math.MaxI(
                Lines1.Length,
                Lines2.Length,
                Lines3.Length
            );
        }
    }
    
    internal struct EntityKey{
        internal EntityKey(Vector2I Position, bool HasUniqueID = false){
            this.Position = Position;
            UniqueID = HasUniqueID ? ++__TotalUniqueID : 0;
        }
        
        internal EntityKey(Vector2I Position, uint UniqueID){
            this.Position = Position;
            this.UniqueID = UniqueID;
        }
        
        internal readonly Vector2I Position;
        internal readonly uint     UniqueID;
        
        public bool Equals(EntityKey other) => Position.Equals(other.Position) && UniqueID == other.UniqueID;

        public override bool Equals(object? Obj) => Obj is EntityKey other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Position, UniqueID);

        public static bool operator ==(EntityKey L, EntityKey R) => L.Equals(R);

        public static bool operator !=(EntityKey L, EntityKey R) => !L.Equals(R);

        public override string ToString() => "EntityKey(" + Position.ToShortString() + ", " + UniqueID + ")";
    }
    internal static uint __TotalUniqueID = 1;
}
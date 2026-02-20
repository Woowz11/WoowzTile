using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_Resources;
using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Utility;
using static GOLUWorld.GOLUWorld_World;
using static GOLUWorld.GOLUWorld_Utility;

namespace GOLUWorld;

internal static class GOLUWorld_Info{
    /// <summary>
    /// Текстура предмета
    /// </summary>
    internal static Texture Info_Item_Texture(T_Item I){
        if(I == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить текстуру!"); }

        return I switch{
            T_Item.FirstAidKit => Texture_FirstAidKit,
            T_Item.GPS         => Texture_GPS,
            T_Item.Stick       => Texture_Stick,
            T_Item.Crowbar     => Texture_Crowbar,
            T_Item.Rock        => Texture_Rock,
            T_Item.Destroyer   => Texture_Destroyer,
            T_Item.Clock       => Texture_Clock,
            T_Item.Mushroom    => Texture_Mushroom,
            T_Item.Battery     => Texture_Battery,
            
            var _ => Texture_Error
        };
    }
    
    /// <summary>
    /// Иконка предмета
    /// </summary>
    internal static Texture Info_Item_Icon(T_Item I){
        if(I == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить иконку!"); }

        return I switch{
            T_Item.FirstAidKit => Texture_FirstAidKit_Icon,
            T_Item.GPS         => Texture_GPS_Icon,
            T_Item.Stick       => Texture_Stick_Icon,
            T_Item.Crowbar     => Texture_Crowbar_Icon,
            T_Item.Rock        => Texture_Rock_Icon,
            T_Item.Destroyer   => Texture_Destroyer_Icon,
            T_Item.Clock       => Texture_Clock_Icon,
            T_Item.Mushroom    => Texture_Mushroom_Icon,
            T_Item.Battery     => Texture_Battery_Icon,
                    
            var _ => Texture_Error_Icon
        };
    }

    /// <summary>
    /// Название предмета
    /// </summary>
    internal static string Info_Item_Name(T_Item I){
        if(I == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить его название!"); }
        
        return I switch{
            T_Item.FirstAidKit => "АПТЕЧКА",
            T_Item.GPS         => "GPS",
            T_Item.Error       => "ОШИБКА",
            T_Item.Stick       => "ПАЛКА",
            T_Item.Crowbar     => "МОНТИРОВКА",
            T_Item.Rock        => "КАМЕНЬ",
            T_Item.Destroyer   => "РАЗРУШИТЕЛЬ",
            T_Item.Clock       => "ЧАСЫ",
            T_Item.Mushroom    => "ГРИБ",
            T_Item.Battery     => "БАТАРЕЙКА",
            
            var _ => "ПРЕДМЕТ [" + (byte)I + "]"
        };
    }
    
    /// <summary>
    /// Описание предмета
    /// </summary>
    internal static string Info_Item_Description(T_Item I){
        if(I == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить его описание!"); }
        
        return I switch{
            T_Item.FirstAidKit => "ЛЕЧИТ БЕДНЫЙ КУБИК ГУЛУ (+с50)",
            T_Item.GPS => "ЕСЛИ ДЕРЖАТЬ В РУКАХ,\nПОКАЗЫВАЕТ КАРТУ",
            T_Item.Stick => "ИЗБЕЙ ВСЕХ ВЕТКОЙ (у10)",
            T_Item.Crowbar => "ЛОМ (у30)",
            T_Item.Rock => "МОЖНО ЗАВАЛИВАТЬ ЯМЫ",
            T_Item.Destroyer => "БЕССКОНЕЧНЫЙ УРОН (уi)",
            T_Item.Clock => "ЭЛЕКТРОННЫЕ? Я УМЕЮ ОПРЕДЕЛЯТЬ\nТОЛЬКО ПО МЕХАНИЧЕСКИМ",
            T_Item.Mushroom => "НЕЧТО (+с10, +э10)",
            T_Item.Battery => "ЗАРЯЖАЕТ (+э100)",
            
            var _ => "О БОЖЕ ЧТО ЭТО ТАКОЕ?"
        };
    }

    /// <summary>
    /// Скорость атаки оружия
    /// </summary>
    internal static float Info_Item_MeleeAttackSpeed(T_Item I) => I switch{
        T_Item.Stick => 0.15f,
        T_Item.Crowbar => 0.15f,
        T_Item.Destroyer => 0.15f,
                    
        var _ => 0
    };

    /// <summary>
    /// Урон атаки
    /// </summary>
    internal static uint Item_Info_MeleeAttackDamage(T_Item I) => I switch{
        T_Item.Stick => 10,
        T_Item.Crowbar => 30,
        T_Item.Destroyer => uint.MaxValue,

        var _ => 0
    };
    
    /// <summary>
    /// Текстура декали
    /// </summary>
    internal static Texture Info_Decal_Texture(T_Decal D) => D switch{
        T_Decal.FootStep       => Texture_FootStep,
        T_Decal.Blood          => Texture_Blood,
        T_Decal.Zero           => Texture_Zero,
        T_Decal.One            => Texture_One,
        T_Decal.Glass          => Texture_GlassShard,
        T_Decal.PlasticBag     => Texture_PlasticBag,
        T_Decal.Paper          => Texture_Paper,
        T_Decal.BrokenTrashBag => Texture_TrashBag_Broken,
        
        var _ => Texture_Error,
    };

    /// <summary>
    /// Возвращает случайную мусорную декаль
    /// </summary>
    internal static T_Decal Info_Decal_RandomTrash() => Utility_SelectWeightedObject(WL.Math.Random.Fast_0_1(), [(T_Decal.PlasticBag, 0, 1), (T_Decal.Glass, 0, 1), (T_Decal.Paper, 0, 1), (T_Decal.BrokenTrashBag, 0, 1)]).Item1;

    /// <summary>
    /// Текстура блока
    /// </summary>
    internal static Texture Info_Block_Texture(Block B) => B.ID switch{
        T_Block.Ground_Planks      => Texture_Planks,
        T_Block.Ground_Asphalt     => Texture_Asphalt,
        T_Block.Ground_Sand        => Texture_Sand,
        T_Block.Water              => World_GetBlock(B.X, B.Y - 16, SnapToGrid: false).ID == B.ID ? Texture_Water : Texture_Water_Top,
        T_Block.Ground_Grass       => Texture_Grass,
        T_Block.Metal              => Texture_Metal,
        T_Block.Bricks             => Texture_Bricks,
        T_Block.Black              => Texture_Black,
        T_Block.Error              => Texture_Error,
        T_Block.Concrete           => Texture_Concrete_Beam,
        T_Block.Ground_Cobblestone => B.Info == 1 ? Texture_Cobblestone_Water : Texture_Cobblestone,
        T_Block.Pit                => World_GetBlock(B.X, B.Y - 16, SnapToGrid: false).ID == B.ID ? (World_GetBlock(B.X, B.Y - 32, SnapToGrid: false).ID == B.ID ? Texture_Black : Texture_Pit) : Texture_Pit_Top,
        
        var _ => Texture_Error
    };
    
    /// <summary>
    /// Блок с коллайдером?
    /// </summary>
    internal static bool Info_Block_Collide(T_Block B) => B is T_Block.Black or T_Block.Bricks or T_Block.Metal or T_Block.Water or T_Block.Error or T_Block.Concrete or T_Block.Pit;

    /// <summary>
    /// Блок рендерится как пол?
    /// </summary>
    internal static bool Info_Block_Ground(T_Block B) => B is T_Block.Ground_Planks or T_Block.Ground_Asphalt or T_Block.Ground_Sand or T_Block.Water or T_Block.Ground_Grass or T_Block.Ground_Cobblestone or T_Block.Pit;

    /// <summary>
    /// Отзеркаливать блок?
    /// </summary>
    internal static bool Info_Block_Reflect(T_Block B) => Info_Block_Collide(B) && !Info_Block_Water(B);

    /// <summary>
    /// Блок вода?
    /// </summary>
    internal static bool Info_Block_Water(T_Block B) => B is T_Block.Water;

    /// <summary>
    /// Блок яма?
    /// </summary>
    internal static bool Info_Block_Pit(T_Block B) => Info_Block_Water(B) || B is T_Block.Pit;
    
    /// <summary>
    /// Поддерживает декали?
    /// </summary>
    internal static bool Info_Block_SupportDecals(T_Block B) => !Info_Block_Pit(B);

    /// <summary>
    /// На блоке может расти трава?
    /// </summary>
    internal static bool Info_Block_SupportGrass(T_Block B) => B is T_Block.Ground_Grass or T_Block.Ground_Sand or T_Block.Empty;
    
    /// <summary>
    /// Превращает символ в блок
    /// </summary>
    internal static (T_Block, byte)? Info_Block_Symbol(char C, int X, int Y, uint Seed, TextureRotation Rotation = TextureRotation.None){
        T_Block ID = T_Block.Empty;
        byte Info = 0;

        uint Unique = Utility_SeedXY(X, Y);
        
        uint Seed1 = Seed + 888542135;
        uint Seed2 = Seed1 - 12516;
        
        switch (C){
            case '#':
                ID = T_Block.Metal;
                break;
            case 'P':
                ID = T_Block.Ground_Planks;
                break;
            case 'A':{
                T_Block B = World_GetBlock(X, Y).ID;
                if(Info_Block_Water(B)){ return null; }
                ID = T_Block.Ground_Asphalt;
                break;
            }
            case 'B':
                ID = T_Block.Bricks;
                break;
            case 'S':{
                T_Block B = World_GetBlock(X, Y).ID;
                if(Info_Block_Water(B)){ return null; }
                ID = T_Block.Ground_Sand;
                break;
            }
            case 'W':
                ID = T_Block.Water;
                break;
            case 'b':
                ID = T_Block.Black;
                break;
            case '^':
                ID = T_Block.Ground_Grass;
                break;
            case 'C':
                ID = T_Block.Concrete;
                break;
            case 'p':
                ID = T_Block.Pit;
                break;
            case 'Д':
                Seed += Unique + 121;
                return Utility_SelectWeightedObject(
                    WL.Math.Random.Fast_0_1(ref Seed),
                    [(T_Block.Ground_Grass, 0, 1), (T_Block.Empty, 0, 1)]
                );
            case 'П': {
                T_Block B = World_GetBlock(X, Y).ID;
                if(Info_Block_Water(B)){ return null; }
                Seed += Unique + 774743;
                return Utility_SelectWeightedObject(
                    WL.Math.Random.Fast_0_1(ref Seed),
                    [(T_Block.Ground_Sand, 0, 1), (T_Block.Empty, 0, 1)]
                );
            }
            case 'Ũ':
                ID = WL.Math.Random.Fast_Bool(ref Seed1) ? T_Block.Ground_Planks : T_Block.Bricks;
                break;
            case 'ũ':
                ID = WL.Math.Random.Fast_Bool(ref Seed2) ? T_Block.Ground_Planks : T_Block.Bricks;
                break;
            
            case '\r':
            case '\n':
            case '.':
                return null;
            default:
                ID = T_Block.Error;
                break;
        }

        return (ID, Info);
    }

    /// <summary>
    /// Текстура сущности
    /// </summary>
    internal static Texture Info_Entity_Texture(Entity E) => E.ID switch{
        T_Entity.Chair      => Texture_Chair,
        T_Entity.Table      => Texture_Table,
        T_Entity.Spikes     => Texture_Spikes,
        T_Entity.Tree       => Texture_Tree,
        T_Entity.Item       => Info_Item_Texture((T_Item)E.Info),
        T_Entity.Crate      => Texture_Crate,
        T_Entity.Grass      => Texture_TallGrass,
        T_Entity.Bush       => Texture_Bush,
        T_Entity.Error      => Texture_Error,
        T_Entity.Mob_Spider => E.Health > 0 ? (World_AnimationTimer > 0.5f ? Texture_Spider_Walk : Texture_Spider) : Texture_Spider_Dead,
        T_Entity.Window     => E.Info == 1 ? Texture_Window_Boarded : Texture_Window,
        T_Entity.TrashBag   => Texture_TrashBag,
        T_Entity.Tire       => Texture_Tire,
        T_Entity.HighGrass  => Texture_TallGrass_High,
        T_Entity.Cattail    => Texture_Cattail,
        T_Entity.Grave      => Texture_Grave,
        T_Entity.Door       => E.Info is 1 or 3 ? Texture_Door_Open : Texture_Door,
        T_Entity.Cardboard  => Texture_Cardboard,
        T_Entity.Money      => Info_Money_Texture((T_Money)E.Info),
        T_Entity.Trapdoor   => Texture_Trapdoor,
        
        var _ => Texture_Error
    };

    /// <summary>
    /// Какие сущности рендерить?
    /// </summary>
    internal static bool Info_Entity_DoRender(T_Entity E) => true;
    
    /// <summary>
    /// Отзеркаливается сущность? Возвращает OffsetY
    /// </summary>
    internal static int? Info_Entity_Reflect(T_Entity E) => E switch{
        T_Entity.Mob_Spider => 9,
        T_Entity.Item       => 3,
        T_Entity.Window     => 0,
        T_Entity.Cattail    => 0,
        T_Entity.Door       => 0,
        
        var _ => null
    };

    /// <summary>
    /// Взаимодействующие сущности
    /// </summary>
    internal static bool Info_Entity_Interacting(T_Entity E) => E is T_Entity.Item or T_Entity.Door or T_Entity.Money or T_Entity.Trapdoor;

    /// <summary>
    /// Случайная позиция для спавна сущности?
    /// </summary>
    internal static bool Info_Entity_RandomSpawnPosition(T_Entity E, byte Info) => E == T_Entity.Item && Info is (byte)T_Item.Stick or (byte)T_Item.Rock or (byte)T_Item.Mushroom;

    /// <summary>
    /// Сущности которые может толкать вода
    /// </summary>
    internal static bool Info_Entity_CanFlow(T_Entity E) => E is T_Entity.Item or T_Entity.Mob_Spider or T_Entity.Crate or T_Entity.Money;

    /// <summary>
    /// Является растением? (случайная позиция и ветер)
    /// </summary>
    internal static bool Info_Entity_Plant(T_Entity E) => E is T_Entity.Grass or T_Entity.Bush or T_Entity.HighGrass or T_Entity.Cattail;

    /// <summary>
    /// Стартовое здоровье сущности
    /// </summary>
    internal static uint Info_Entity_Health(T_Entity E) => E switch{
        T_Entity.Window => 50,
        T_Entity.TrashBag => 30,
        T_Entity.Cardboard => 30,
        
        var _ => 100
    };
    
    /// <summary>
    /// Уникальная сущность? Не заменяется при такой же позиции
    /// </summary>
    internal static bool Info_Entity_Unique(T_Entity E) => E is T_Entity.Crate or T_Entity.Item or T_Entity.Mob_Spider or T_Entity.Money;

    /// <summary>
    /// Случайный лут из мусорного мешка
    /// </summary>
    internal static (T_Entity, byte) Info_Entity_Loot_TrashBag(uint Seed) => Utility_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref Seed), [
        (T_Entity.Empty, 0, 100),
        (T_Entity.Item, (byte)T_Item.Battery, 10),
        (T_Entity.Item, (byte)T_Item.Clock, 1),
        (T_Entity.Item, (byte)T_Item.FirstAidKit, 2),
        (T_Entity.Item, (byte)T_Item.Crowbar, 1),
        (T_Entity.Money, (byte)T_Money.M1, 50),
        (T_Entity.Money, (byte)T_Money.M5, 10),
        (T_Entity.Money, (byte)T_Money.M10, 1),
        (T_Entity.Item, (byte)T_Item.Mushroom, 2),
    ]);
    
    /// <summary>
    /// Превращает символ в сущность
    /// </summary>
    internal static (T_Entity, byte)? Info_Entity_Symbol(char C, int X, int Y, uint Seed, TextureRotation Rotation = TextureRotation.None){
        T_Entity ID = T_Entity.Empty;
        byte Info = 0;

        uint Unique = Utility_SeedXY(X, Y);
        
        uint Seed1 = Seed + 9493235;
        
        switch (C){
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
            case '#':
                ID = T_Entity.Crate;
                break;
            case '~':
                ID = T_Entity.Grass;
                break;
            case '3':
                ID = T_Entity.Bush;
                break;
            case 'G':
                ID = T_Entity.Grave;
                break;
            case 'D':
                Seed += Unique + 88329;
                bool Open = WL.Math.Random.Fast_Bool(0.1f, ref Seed);
                return (T_Entity.Door, (byte)(Utility_Vertical(Rotation) ? (Open ? 1 : 0) : (Open ? 3 : 2)));
            case 'w':
                Seed += Unique + 88555;
                return (T_Entity.Window, (byte)WL.Math.Random.Fast_Int(0, 1, ref Seed));
            case 'Д': {
                Seed += Unique + 1667;
                T_Block B = World_GetBlock(X, Y).ID;
                if(!Info_Block_SupportGrass(B)){ return null; }
                return B == T_Block.Ground_Sand
                    ? Utility_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref Seed), [(T_Entity.Grass, 0, 1), (T_Entity.Item, (byte)T_Item.Rock, 1), (T_Entity.Item, (byte)T_Item.Stick, 1), (T_Entity.Empty, 0, 99)])
                    : Utility_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref Seed),
                        [(T_Entity.Tree, (byte)(WL.Math.Random.Fast_Bool(ref Seed1) ? 1 : 0), 20), (T_Entity.Item, (byte)T_Item.Rock, 10), (T_Entity.Item, (byte)T_Item.Mushroom, 1), (T_Entity.Item, (byte)T_Item.Stick, 1), (T_Entity.Bush, 0, 5), (T_Entity.Grass, 0, 43), (T_Entity.Empty, 0, 32)]);
            }
            case 'д': {
                Seed += Unique + 1532;
                T_Block B = World_GetBlock(X, Y).ID;
                if(!Info_Block_SupportGrass(B)){ return null; }
                return B == T_Block.Ground_Sand
                    ? Utility_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref Seed), [(T_Entity.Grass, 0, 1), (T_Entity.Cattail, 0, 10), (T_Entity.Empty, 0, 99)])
                    : Utility_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref Seed), [(T_Entity.Bush, 0, 10), (T_Entity.Item, (byte)T_Item.Rock, 1), (T_Entity.Item, (byte)T_Item.Stick, 1), (T_Entity.HighGrass, 0, 10), (T_Entity.Grass, 0, 430), (T_Entity.Empty, 0, 320)]);
            }
            case 'т': {
                Seed += Unique + 8543;
                T_Block B = World_GetBlock(X, Y).ID;
                if(!Info_Block_SupportGrass(B) || B is T_Block.Ground_Sand){ return null; }
                return Utility_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref Seed), [(T_Entity.Grass, 0, 1), (T_Entity.HighGrass, 0, 5), (T_Entity.Empty, 0, 1)]);
            }
            case 'М':
                Seed += Unique + 99533221;
                return Utility_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref Seed), [(T_Entity.Empty, 0, 1), (T_Entity.Chair, 0, 1), (T_Entity.Table, 0, 1), (T_Entity.Crate, 0, 1)]);
            case 'м':
                Seed += Unique + 995321154;
                return Utility_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref Seed), [(T_Entity.Empty, 0, 2), (T_Entity.TrashBag, 0, 2), (T_Entity.Cardboard, 0, 2), (T_Entity.Tire, 0, 1)]);

            case '\r':
            case '\n':
            case '.':
                return null;
            default:
                ID = T_Entity.Error;
                break;
        }

        return (ID, Info);
    }
    
    /// <summary>
    /// Текстура потолка
    /// </summary>
    internal static Texture Info_Ceiling_Texture(Ceiling C) => C.ID switch{
        T_Ceiling.Concrete  => Texture_Concrete,
        T_Ceiling.RoofTiles => Texture_RoofTiles,
        
        var _ => Texture_Error
    };
    
    /// <summary>
    /// Превращает символ в потолок
    /// </summary>
    internal static (T_Ceiling, byte)? Info_Ceiling_Symbol(char C, int X, int Y, uint Seed, TextureRotation Rotation = TextureRotation.None){
        T_Ceiling ID = T_Ceiling.Empty;
        byte Info = 0;

        uint Unique = Utility_SeedXY(X, Y);
        
        uint Seed1 = Seed  + 88348835;
        uint Seed2 = Seed1 - 1241256;
        
        switch (C){
            case '_':
                ID = T_Ceiling.Invisible;
                break;
            case 'C':
                ID = T_Ceiling.Concrete;
                break;
            case 'R':
                return (T_Ceiling.RoofTiles, (byte)(Utility_Vertical(Rotation) ? 0 : 2));
            case 'r':
                return (T_Ceiling.RoofTiles, (byte)(Utility_Vertical(Rotation) ? 1 : 3));
            case 'Ũ':
                if(WL.Math.Random.Fast_Bool(ref Seed1)){
                    return (T_Ceiling.RoofTiles, (byte)(Utility_Vertical(Rotation) ? 0 : 2));
                }
                ID = T_Ceiling.Invisible;
                break;
            case 'ũ':
                if(WL.Math.Random.Fast_Bool(ref Seed2)){
                    return (T_Ceiling.RoofTiles, (byte)(Utility_Vertical(Rotation) ? 1 : 3));
                }
                ID = T_Ceiling.Invisible;
                break;
            
            case '\r':
            case '\n':
            case '.':
                return null;
            default:
                ID = T_Ceiling.Error;
                break;
        }

        return (ID, Info);
    }

    /// <summary>
    /// Текстура монеты
    /// </summary>
    internal static Texture Info_Money_Texture(T_Money M) => M switch{
        T_Money.M1     => Texture_Money1,
        T_Money.M5     => Texture_Money5,
        T_Money.M10    => Texture_Money10,
        T_Money.M25    => Texture_Money25,
        T_Money.M50    => Texture_Money50,
        T_Money.M100   => Texture_Money100,
        T_Money.M250   => Texture_Money250,
        T_Money.M500   => Texture_Money500,
        T_Money.M1000  => Texture_Money1000,
        T_Money.M2500  => Texture_Money2500,
        T_Money.M5000  => Texture_Money5000,
        T_Money.M10000 => Texture_Money10000
    };
    
    /// <summary>
    /// Цена монеты
    /// </summary>
    internal static uint Info_Money_Cost(T_Money M) => M switch{
        T_Money.M1     => 1,
        T_Money.M5     => 5,
        T_Money.M10    => 10,
        T_Money.M25    => 25,
        T_Money.M50    => 50,
        T_Money.M100   => 100,
        T_Money.M250   => 250,
        T_Money.M500   => 500,
        T_Money.M1000  => 1000,
        T_Money.M2500  => 2500,
        T_Money.M5000  => 5000,
        T_Money.M10000 => 10000
    };
}
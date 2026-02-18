using WLO;
using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_Resources;
using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Generator;
using static GOLUWorld.GOLUWorld_World;

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
            
            var _ => "ПРЕДМЕТ [" + (byte)I + "]"
        };
    }
    
    /// <summary>
    /// Описание предмета
    /// </summary>
    internal static string Info_Item_Description(T_Item I){
        if(I == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить его описание!"); }
        
        return I switch{
            T_Item.FirstAidKit => "ЛЕЧИТ БЕДНЫЙ КУБИК ГУЛУ (+ с50)",
            T_Item.GPS => "ЕСЛИ ДЕРЖАТЬ В РУКАХ,\nПОКАЗЫВАЕТ КАРТУ",
            T_Item.Stick => "ИЗБЕЙ ВСЕХ ВЕТКОЙ (у10)",
                        
            var _ => "О БОЖЕ ЧТО ЭТО ТАКОЕ?"
        };
    }

    /// <summary>
    /// Скорость атаки оружия
    /// </summary>
    internal static float Info_Item_MeleeAttackSpeed(T_Item I){
        return I switch{
            T_Item.Stick => 0.15f,
                        
            var _ => 0
        };
    }

    /// <summary>
    /// Урон атаки
    /// </summary>
    internal static uint Item_Info_MeleeAttackDamage(T_Item I){
        return I switch{
            T_Item.Stick => 10,

            var _ => 0
        };
    }
    
    /// <summary>
    /// Текстура декали
    /// </summary>
    internal static Texture Info_Decal_Texture(T_Decal D){
        return D switch{
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
    }

    /// <summary>
    /// Возвращает случайную мусорную декаль
    /// </summary>
    internal static T_Decal Info_Decal_RandomTrash() => Generator_SelectWeightedObject(WL.Math.Random.Fast_0_1(), [(T_Decal.PlasticBag, 0, 1), (T_Decal.Glass, 0, 1), (T_Decal.Paper, 0, 1), (T_Decal.BrokenTrashBag, 0, 1)]).Item1;

    /// <summary>
    /// Текстура блока
    /// </summary>
    internal static Texture Info_Block_Texture(Block B){
        return B.ID switch{
            T_Block.Ground_Planks  => Texture_Planks,
            T_Block.Ground_Asphalt => Texture_Asphalt,
            T_Block.Ground_Sand    => Texture_Sand,
            T_Block.Water          => (World_Blocks.TryGetValue(new Vector2I(B.X, B.Y - 16), out Block __Found) && __Found.ID == B.ID ? Texture_Water : Texture_Water_Top),
            T_Block.Ground_Grass   => Texture_Grass,
            T_Block.Metal          => Texture_Metal,
            T_Block.Bricks         => Texture_Bricks,
            T_Block.Black          => Texture_Black,
            T_Block.Error          => Texture_Error,
            T_Block.Concrete       => Texture_Concrete_Beam,
            
            var _ => Texture_Error
        };
    }
    
    /// <summary>
    /// Блок твёрдый?
    /// </summary>
    internal static bool Info_Block_Solid(T_Block B) => B is T_Block.Black or T_Block.Bricks or T_Block.Metal or T_Block.Water or T_Block.Error or T_Block.Concrete;

    /// <summary>
    /// Блок является полом?
    /// </summary>
    internal static bool Info_Block_Ground(T_Block B) => B is T_Block.Ground_Planks or T_Block.Ground_Asphalt or T_Block.Ground_Sand or T_Block.Water or T_Block.Ground_Grass;

    /// <summary>
    /// Отзеркаливать блок?
    /// </summary>
    internal static bool Info_Block_Reflect(T_Block B) => Info_Block_Solid(B) && B != T_Block.Water;

    /// <summary>
    /// Поддерживает декали?
    /// </summary>
    internal static bool Info_Block_SupportDecals(T_Block B) => B != T_Block.Water;

    /// <summary>
    /// На блоке может расти трава?
    /// </summary>
    internal static bool Info_Block_SupportGrass(T_Block B) => B is T_Block.Ground_Grass or T_Block.Ground_Sand or T_Block.Empty;
    
    /// <summary>
    /// Превращает символ в блок
    /// </summary>
    internal static (T_Block, byte)? Info_Block_Symbol(char C, int X, int Y, ref uint __Seed){
        T_Block ID = T_Block.Empty;
        byte Info = 0;

        uint __Seed1 = __Seed + 888542135;
        uint __Seed2 = __Seed1 - 12516;
        
        switch (C){
            case '#':
                ID = T_Block.Metal;
                break;
            case 'P':
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
            case 'b':
                ID = T_Block.Black;
                break;
            case '^':
                ID = T_Block.Ground_Grass;
                break;
            case 'C':
                ID = T_Block.Concrete;
                break;
            case 'Д':
                __Seed += 121;
                return Generator_SelectWeightedObject(
                    WL.Math.Random.Fast_0_1(ref __Seed),
                    [(T_Block.Ground_Grass, 0, 1), (T_Block.Empty, 0, 1)]
                );
            case 'П':
                __Seed += 774743;
                return Generator_SelectWeightedObject(
                    WL.Math.Random.Fast_0_1(ref __Seed),
                    [(T_Block.Ground_Sand, 0, 1), (T_Block.Empty, 0, 1)]
                );
            case 'Ũ':
                ID = WL.Math.Random.Fast_Bool(ref __Seed1) ? T_Block.Ground_Planks : T_Block.Bricks;
                break;
            case 'ũ':
                ID = WL.Math.Random.Fast_Bool(ref __Seed2) ? T_Block.Ground_Planks : T_Block.Bricks;
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
    internal static Texture Info_Entity_Texture(Entity E){
        return E.ID switch{
            T_Entity.Chair      => Texture_Chair,
            T_Entity.Table      => Texture_Table,
            T_Entity.Spikes     => Texture_Spikes,
            T_Entity.Tree       => Texture_Tree,
            T_Entity.Item       => Info_Item_Texture((T_Item)E.Info),
            T_Entity.Crate      => Texture_Crate,
            T_Entity.Grass      => Texture_TallGrass,
            T_Entity.Bush       => Texture_Bush,
            T_Entity.Error      => Texture_Error,
            T_Entity.Rock       => Texture_Rock,
            T_Entity.Mob_Spider => E.Health > 0 ? (World_AnimationTimer > 0.5f ? Texture_Spider_Walk : Texture_Spider) : Texture_Spider_Dead,
            T_Entity.Window     => E.Info == 1 ? Texture_Window_Boarded : Texture_Window,
            T_Entity.TrashBag   => Texture_TrashBag,
            T_Entity.Tire       => Texture_Tire,
            
            var _ => Texture_Error
        };
    }

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
        var _ => null
    };

    /// <summary>
    /// Случайная позиция для спавна сущности?
    /// </summary>
    internal static bool Info_Entity_RandomSpawnPosition(T_Entity E, byte Info) => E == T_Entity.Item && Info == (byte)T_Item.Stick;

    /// <summary>
    /// Сущности которые может толкать вода
    /// </summary>
    internal static bool Info_Entity_CanFlow(T_Entity E) => E is T_Entity.Item or T_Entity.Mob_Spider or T_Entity.Crate;

    /// <summary>
    /// Является растением? (случайная позиция и ветер)
    /// </summary>
    internal static bool Info_Entity_Plant(T_Entity E) => E is T_Entity.Grass or T_Entity.Bush;

    /// <summary>
    /// Стартовое здоровье сущности
    /// </summary>
    internal static uint Info_Entity_Health(T_Entity E) => E switch{
        T_Entity.Window => 50,
        T_Entity.TrashBag => 30,
        
        var _ => 100
    };
    
    /// <summary>
    /// Превращает символ в сущность
    /// </summary>
    internal static (T_Entity, byte)? Info_Entity_Symbol(char C, int X, int Y, ref uint __Seed){
        T_Entity ID = T_Entity.Empty;
        byte Info = 0;
        
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
            case 'w':
                __Seed += 88555;
                return (T_Entity.Window, (byte)WL.Math.Random.Fast_Int(0, 1, ref __Seed));
            case 'Д': {
                __Seed += 1667;
                T_Block B = World_GetBlock(X, Y).ID;
                if(!Info_Block_SupportGrass(B)){ return null; }
                return World_GetBlock(X, Y).ID == T_Block.Ground_Sand
                    ? Generator_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref __Seed), [(T_Entity.Grass, 0, 1), (T_Entity.Rock, 0, 1), (T_Entity.Item, (byte)T_Item.Stick, 1), (T_Entity.Empty, 0, 99)])
                    : Generator_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref __Seed),
                        [(T_Entity.Tree, 0, 20), (T_Entity.Rock, 0, 10), (T_Entity.Item, (byte)T_Item.Stick, 1), (T_Entity.Bush, 0, 5), (T_Entity.Grass, 0, 43), (T_Entity.Empty, 0, 32)]);
            }
            case 'д': {
                __Seed += 1532;
                T_Block B = World_GetBlock(X, Y).ID;
                if(!Info_Block_SupportGrass(B)){ return null; }
                return World_GetBlock(X, Y).ID == T_Block.Ground_Sand
                    ? Generator_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref __Seed), [(T_Entity.Grass, 0, 1), (T_Entity.Empty, 0, 99)])
                    : Generator_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref __Seed), [(T_Entity.Bush, 0, 5), (T_Entity.Grass, 0, 43), (T_Entity.Empty, 0, 32)]);
            }
            case 'М':
                __Seed += 99533221;
                return Generator_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref __Seed), [(T_Entity.Empty, 0, 1), (T_Entity.Chair, 0, 1), (T_Entity.Table, 0, 1), (T_Entity.Crate, 0, 1)]);
            case 'м':
                __Seed += 995321154;
                return Generator_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref __Seed), [(T_Entity.Empty, 0, 2), (T_Entity.TrashBag, 0, 2), (T_Entity.Tire, 0, 1)]);

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
    internal static Texture Info_Ceiling_Texture(Ceiling C){
        return C.ID switch{
            T_Ceiling.Concrete  => Texture_Concrete,
            T_Ceiling.RoofTiles => Texture_RoofTiles,
            
            var _ => Texture_Error
        };
    }
    
    /// <summary>
    /// Превращает символ в потолок
    /// </summary>
    internal static (T_Ceiling, byte)? Info_Ceiling_Symbol(char C, int X, int Y, ref uint __Seed){
        T_Ceiling ID = T_Ceiling.Empty;
        byte Info = 0;

        uint __Seed1 = __Seed + 88348835;
        uint __Seed2 = __Seed1 - 1241256;
        
        switch (C){
            case '_':
                ID = T_Ceiling.Invisible;
                break;
            case 'C':
                ID = T_Ceiling.Concrete;
                break;
            case 'R':
                ID = T_Ceiling.RoofTiles;
                break;
            case 'r':
                return (T_Ceiling.RoofTiles, 1);
            case 'Ũ':
                if(WL.Math.Random.Fast_Bool(ref __Seed1)){
                    return (T_Ceiling.RoofTiles, 0);
                }
                ID = T_Ceiling.Invisible;
                break;
            case 'ũ':
                if(WL.Math.Random.Fast_Bool(ref __Seed2)){
                    return (T_Ceiling.RoofTiles, 1);
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
}
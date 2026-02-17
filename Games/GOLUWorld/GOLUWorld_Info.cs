using WLO;
using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_Resources;
using static GOLUWorld.GOLUWorld_Values;

namespace GOLUWorld;

internal static class GOLUWorld_Info{
    /// <summary>
    /// Текстура предмета
    /// </summary>
    internal static Texture Info_Item_Texture(T_Item Item){
        if(Item == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить текстуру!"); }

        return Item switch{
            T_Item.FirstAidKit => Texture_FirstAidKit,
            T_Item.GPS         => Texture_GPS,
            T_Item.Stick       => Texture_Stick,
            
            var _ => Texture_Error
        };
    }
    
    /// <summary>
    /// Иконка предмета
    /// </summary>
    internal static Texture Info_Item_Icon(T_Item Item){
        if(Item == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить иконку!"); }

        return Item switch{
            T_Item.FirstAidKit => Texture_FirstAidKit_Icon,
            T_Item.GPS         => Texture_GPS_Icon,
            T_Item.Stick       => Texture_Stick_Icon,
                    
            var _ => Texture_Error_Icon
        };
    }

    /// <summary>
    /// Название предмета
    /// </summary>
    internal static string Info_Item_Name(T_Item Item){
        if(Item == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить его название!"); }
        
        return Item switch{
            T_Item.FirstAidKit => "АПТЕЧКА",
            T_Item.GPS         => "GPS",
            T_Item.Error       => "ОШИБКА",
            T_Item.Stick       => "ПАЛКА",
            
            var _ => "ПРЕДМЕТ [" + (byte)Item + "]"
        };
    }
    
    /// <summary>
    /// Описание предмета
    /// </summary>
    internal static string Info_Item_Description(T_Item Item){
        if(Item == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить его описание!"); }
        
        return Item switch{
            T_Item.FirstAidKit => "ЛЕЧИТ БЕДНЫЙ КУБИК ГУЛУ (+ с50)",
            T_Item.GPS => "ЕСЛИ ДЕРЖАТЬ В РУКАХ,\nПОКАЗЫВАЕТ КАРТУ",
            T_Item.Stick => "ИЗБЕЙ ВСЕХ ВЕТКОЙ (у10)",
                        
            var _ => "О БОЖЕ ЧТО ЭТО ТАКОЕ?"
        };
    }

    /// <summary>
    /// Скорость атаки оружия
    /// </summary>
    internal static float Info_Item_MeleeAttackSpeed(T_Item Item){
        return Item switch{
            T_Item.Stick => 0.15f,
                        
            var _ => 0
        };
    }

    /// <summary>
    /// Урон атаки
    /// </summary>
    internal static uint Item_Info_MeleeAttackDamage(T_Item Item){
        return Item switch{
            T_Item.Stick => 10,

            var _ => 0
        };
    }
    
    /// <summary>
    /// Текстура декали
    /// </summary>
    internal static Texture Info_Decal_Texture(T_Decal Decal){
        return Decal switch{
            T_Decal.FootStep => Texture_FootStep,
            T_Decal.Blood    => Texture_Blood,
            T_Decal.Zero     => Texture_Zero,
            T_Decal.One      => Texture_One,
            
            var _ => Texture_Error,
        };
    }

    /// <summary>
    /// Текстура блока
    /// </summary>
    internal static Texture Info_Block_Texture(Block Block){
        return Block.ID switch{
            T_Block.Ground_Planks  => Texture_Planks,
            T_Block.Ground_Asphalt => Texture_Asphalt,
            T_Block.Ground_Sand    => Texture_Sand,
            T_Block.Water          => (World_Blocks.TryGetValue(new Vector2I(Block.X, Block.Y - 16), out Block __Found) && __Found.ID == Block.ID ? Texture_Water : Texture_Water_Top),
            T_Block.Ground_Grass   => Texture_Grass,
            T_Block.Metal          => Texture_Metal,
            T_Block.Bricks         => Texture_Bricks,
            T_Block.Black          => Texture_Black,
            T_Block.Error          => Texture_Error,
            
            var _ => Texture_Error
        };
    }
    
    /// <summary>
    /// Блок твёрдый?
    /// </summary>
    internal static bool Info_Block_Solid(T_Block B) => B is T_Block.Black or T_Block.Bricks or T_Block.Metal or T_Block.Water or T_Block.Error;

    /// <summary>
    /// Блок является полом?
    /// </summary>
    internal static bool Info_Block_Ground(T_Block B) => B is T_Block.Ground_Planks or T_Block.Ground_Asphalt or T_Block.Ground_Sand or T_Block.Water or T_Block.Ground_Grass;

    /// <summary>
    /// Отзеркаливать блок?
    /// </summary>
    internal static bool Info_Block_Reflect(T_Block B) => Info_Block_Solid(B) && B != T_Block.Water;

    /// <summary>
    /// Текстура сущности
    /// </summary>
    internal static Texture Info_Entity_Texture(Entity Entity){
        return Entity.ID switch{
            T_Entity.Chair      => Texture_Chair,
            T_Entity.Table      => Texture_Table,
            T_Entity.Spikes     => Texture_Spikes,
            T_Entity.Tree       => Texture_Tree,
            T_Entity.Item       => Info_Item_Texture((T_Item)Entity.Info),
            T_Entity.Crate      => Texture_Crate,
            T_Entity.Grass      => Texture_TallGrass,
            T_Entity.Bush       => Texture_Bush,
            T_Entity.Error      => Texture_Error,
            T_Entity.Rock       => Texture_Rock,
            T_Entity.Mob_Spider => Entity.Health > 0 ? (World_AnimationTimer > 0.5f ? Texture_Spider_Walk : Texture_Spider) : Texture_Spider_Dead,
            
            var _ => Texture_Error
        };
    }

    /// <summary>
    /// Какие сущности рендерить?
    /// </summary>
    internal static bool Info_Entity_DoRender(T_Entity Entity) => Entity is T_Entity.Chair or T_Entity.Table or T_Entity.Spikes or T_Entity.Tree or T_Entity.Item or T_Entity.Crate or T_Entity.Grass or T_Entity.Bush or T_Entity.Error or T_Entity.Rock or T_Entity.Mob_Spider;
    
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
}
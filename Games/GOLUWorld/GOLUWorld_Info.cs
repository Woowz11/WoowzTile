using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_Resources;

namespace GOLUWorld;

internal static class GOLUWorld_Info{
    /// <summary>
    /// Блок твёрдый?
    /// </summary>
    internal static bool Info_Block_Solid(T_Block B) => B is T_Block.Black or T_Block.Bricks or T_Block.Metal or T_Block.Water or T_Block.Error;

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
                        
            var _ => "О БОЖЕ ЧТО ЭТО ТАКОЕ?"
        };
    }

    /// <summary>
    /// Скорость атаки оружия
    /// </summary>
    internal static float Info_Item_MeleeAttackSpeed(T_Item Item){
        return Item switch{
            T_Item.Stick => 0.1f,
                        
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
            T_Decal.One      => Texture_One
        };
    }
}
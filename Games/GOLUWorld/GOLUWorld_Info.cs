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
    
    internal static Texture Info_Item_Texture(T_Item Item){
        if(Item == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить текстуру!"); }

        return Item switch{
            T_Item.FirstAidKit => Texture_FirstAidKit,
            T_Item.GPS         => Texture_GPS,
            
            var _ => Texture_Debug
        };
    }

    internal static string Info_Item_Name(T_Item Item){
        if(Item == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить его название!"); }
        
        return Item switch{
            T_Item.FirstAidKit => "АПТЕЧКА",
            T_Item.GPS         => "GPS",
            
            var _ => "ПРЕДМЕТ [" + (byte)Item + "]"
        };
    }
}
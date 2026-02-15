using WoowzTile.Objects;
using static GOLUWorld.GW_Objects;
using static GOLUWorld.GW_Resources;

namespace GOLUWorld;

internal static class GW_Info{
    internal static bool BlockInfo_Solid(T_Block B) => B is T_Block.Black or T_Block.Bricks or T_Block.Metal or T_Block.Water or T_Block.Error;
    
    internal static Texture ItemTexture(T_Item Item){
        if(Item == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить текстуру!"); }

        return Item switch{
            T_Item.FirstAidKit => Texture_FirstAidKit,
            T_Item.GPS         => Texture_GPS,
            
            var _ => Texture_Debug
        };
    }

    internal static string ItemName(T_Item Item){
        if(Item == T_Item.Empty){ throw new Exception("Указан пустой предмет, невозможно получить его название!"); }
        
        return Item switch{
            T_Item.FirstAidKit => "АПТЕЧКА",
            T_Item.GPS         => "GPS",
            
            var _ => "ПРЕДМЕТ [" + (byte)Item + "]"
        };
    }
}
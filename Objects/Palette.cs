using System.Drawing;
using WLO;

namespace WoowzTile.Objects;

public class Palette{
    public readonly ColorB[] Colors = new ColorB[256];

    public Palette(IEnumerable<KeyValuePair<byte, ColorB>> InitialColors){
        for(int i = 0; i < 256; i++){
            Colors[i] = ColorB.Magenta;
        }
        
        Colors[0] = ColorB.Transparent;
        
        foreach(KeyValuePair<byte, ColorB> KV in InitialColors){
            Colors[KV.Key] = KV.Value;
        }
    }
    
    public Palette(ColorB[] InitialColors){
        for(int i = 0; i < 256; i++){
            Colors[i] = InitialColors[i];
        }
    }

    public ColorB this[byte Index]{
        get => Colors[Index];
        set => Colors[Index] = value;
    }
}
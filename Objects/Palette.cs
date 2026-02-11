using WLO;

namespace WoowzTile.Objects;

public class Palette{
    public readonly Dictionary<byte, ColorB> Colors = new Dictionary<byte, ColorB>();

    public Palette(IEnumerable<KeyValuePair<byte, ColorB>> InitialColors){
        foreach(KeyValuePair<byte, ColorB> KV in InitialColors){
            Colors[KV.Key] = KV.Value;
        }
        
        Colors[0] = ColorB.Transparent;
    }

    public ColorB this[byte Index]{
        get => Colors.GetValueOrDefault(Index, ColorB.Magenta);
        set => Colors[Index] = value;
    }
}
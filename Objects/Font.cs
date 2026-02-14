using WLO;

namespace WoowzTile.Objects;

public class Char{
    public Char(char Code, Texture Texture){
        this.Code = Code;
        this.Texture = Texture;
    }
    
    public Char(Texture Texture){
        this.Texture = Texture;
    }
    
    public Texture Texture;
    public char?   Code;
}

public class Font{
    public Font(Char UnknownChar, IEnumerable<KeyValuePair<char, Char>> InitialChars){
        this.UnknownChar = UnknownChar;
        foreach(KeyValuePair<char, Char> KV in InitialChars){
            KV.Value.Code = KV.Key;
            Chars[KV.Key] = KV.Value;
        }
    }
    
    public readonly Dictionary<char, Char> Chars = [];

    public readonly Char UnknownChar;

    private void ProcessText(string Text, int StartX, int StartY, Action<Char, int, int> OnChar){
        int X__ = StartX;
        int Y__ = StartY;

        foreach(char c in Text){
            if(c == '\r'){ continue; }
            if(c == '\n'){
                X__ = StartX;
                Y__ += (int)UnknownChar.Texture.Height + 1;
                continue;
            }

            Char C__ = Chars.GetValueOrDefault(c, UnknownChar);

            OnChar(C__, X__, Y__);

            X__ += (int)C__.Texture.Width + 1;
        }
    }
    
    public Vector2U TextSize(string Text){
        if(string.IsNullOrEmpty(Text)){ return Vector2U.Zero; }

        int MaxX = 0;
        int MaxY = 0;

        ProcessText(Text, 0, 0,
            (C__, X__, Y__) => {
                int Right = X__ + (int)C__.Texture.Width;
                int Bottom = Y__ + (int)C__.Texture.Height;

                if(Right > MaxX){ MaxX = Right; }
                if(Bottom > MaxY){ MaxY = Bottom; }
            });
        
        return new Vector2U((uint)MaxX, (uint)MaxY);
    }
    
    public void Render(Image.ImageContext C, Palette Palette, string Text, int X, int Y, ColorB? MultiplyColor = null){
        try{
            ProcessText(Text, X, Y, ((C__, X__, Y__) => C__.Texture.Render(C, Palette, X__, Y__, MultiplyColor: MultiplyColor)));
        }catch(Exception e){
            throw new Exception("Произошла ошибка при рендере текста!\nТекст: \"" + Text + "\"", e);
        }
    }
}
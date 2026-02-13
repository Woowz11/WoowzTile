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

    public void Render(Image.ImageContext C, Palette Palette, string Text, int X, int Y){
        try{
            int X__ = X;
            int Y__ = Y;

            foreach(char c in Text){
                if(c == '\r'){ continue; }
                if(c == '\n'){ X__ = X; Y__ += (int)UnknownChar.Texture.Height + 1; continue; }
                
                Char C__ = Chars.GetValueOrDefault(c, UnknownChar);
                
                C__.Texture.Render(C, Palette, X__, Y__);

                X__ += (int)C__.Texture.Width + 1;
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при рендере текста!\nТекст: \"" + Text + "\"", e);
        }
    }
}
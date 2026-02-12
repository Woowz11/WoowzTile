using WL;
using WLO;

namespace WoowzTile.Objects;

public class Texture{
    public Texture(uint Width, uint Height, byte[] Pixels){
        this.Width  = Width;
        this.Height = Height;
        this.Pixels = Pixels;
    }
    
    public Texture(string Chars, Dictionary<char, byte> Mapping){
        if(string.IsNullOrEmpty(Chars)){ throw new Exception("Chars пустой!"); }

        string[] Lines = Chars.Split(["\r\n", "\n"], StringSplitOptions.None);

        Height = (uint)Lines.Length;
        Width  = (uint)Lines[0].Length;

        foreach (string Line in Lines){
            if(Line.Length != Width){ throw new Exception("Все строки должны быть одной длины!"); }
        }

        Pixels = new byte[Width * Height];

        for(int Y = 0; Y < Height; Y++){
            string Line = Lines[Y];
            for(int X = 0; X < Width; X++)
            {
                char C = Line[X];
                if(!Mapping.TryGetValue(C, out byte Index)){ throw new Exception("Символ ['" + C + "'] не найден в Mapping!"); }

                Pixels[Y * (int)Width + X] = Index;
            }
        }
    }
    
    public uint   Width ;
    public uint   Height;
    public byte[] Pixels;

    public byte this[uint X, uint Y]{
        get => Pixels[Y * Width + X];
        set => Pixels[Y * Width + X] = value;
    }

    public void Render(Image.ImageContext C, Palette Palette, int X = 0, int Y = 0, bool FlipX = false, bool FlipY = false){
        try{
            int W = (int)Width;
            int H = (int)Height;

            int DrawX = X;
            int DrawY = Y;
            
            int SrcX = 0;
            int SrcY = 0;

            if(DrawX < 0){
                SrcX = -X;
                W -= SrcX;
                DrawX = 0;
            }

            if(DrawY < 0){
                SrcY = -Y;
                H -= SrcY;
                DrawY = 0;
            }

            int MaxW = (int)C.Width  - DrawX;
            int MaxH = (int)C.Height - DrawY;

            if(W > MaxW){ W = MaxW; }
            if(H > MaxH){ H = MaxH; }

            if(W <= 0 || H <= 0){ return; }

            for(int Y__ = 0; Y__ < H; Y__++){
                int SrcY__ = FlipY ? (H - 1 - (SrcY + Y__))  : (SrcY + Y__);
                int Row = SrcY__ * (int)Width;
                for(int X__ = 0; X__ < W; X__++){
                    int SrcX__ = FlipX ? (W - 1 - (SrcX + X__)) : (SrcX + X__);
                    
                    byte PaletteIndex = Pixels[Row + SrcX__];
                    ColorB Color = Palette[PaletteIndex];

                    if(Color.A == 0){ continue; }

                    uint DstX = (uint)(DrawX + X__);
                    uint DstY = (uint)(DrawY + Y__);

                    C.SetPixel(DstX, DstY, Color, ImageBlend.Alpha);
                }
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при рендере текстуры [" + this + "]!", e);
        }
    }

    public void Render(Image.ImageContext C, Palette Palette, int X, int Y, uint TileWidth, uint TileHeight = 1, bool FlipX = false, bool FlipY = false){
        if(TileWidth == 0 || TileHeight == 0){ return; }
        for(int Y__ = 0; Y__ < TileHeight; Y__++){
            for(int X__ = 0; X__ < TileWidth; X__++){
                Render(C, Palette, X + X__ * (int)Width, Y + Y__ * (int)Height, FlipX, FlipY);
            }
        }
    }
}
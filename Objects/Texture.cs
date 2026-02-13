using WL;
using WLO;

namespace WoowzTile.Objects;

public enum TextureRotation{
    None = 0,
    Rotate90 = 90,
    Rotate180 = 180,
    Rotate270 = 270
}

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

    public void Render(Image.ImageContext C, Palette Palette, int X = 0, int Y = 0, bool FlipX = false, bool FlipY = false, TextureRotation Rotation = TextureRotation.None, ColorB? MultiplyColor = null){
        try{
            MultiplyColor ??= ColorB.White;

            int W = (int)Width;
            int H = (int)Height;

            int DrawX = X;
            int DrawY = Y;

            int SrcXOffset = 0;
            int SrcYOffset = 0;

            if(DrawX < 0){
                SrcXOffset = -DrawX;
                W -= SrcXOffset;
                DrawX = 0;
            }

            if(DrawY < 0){
                SrcYOffset = -DrawY;
                H -= SrcYOffset;
                DrawY = 0;
            }

            int MaxW = (int)C.Width  - DrawX;
            int MaxH = (int)C.Height - DrawY;

            if(W > MaxW){ W = MaxW; }
            if(H > MaxH){ H = MaxH; }

            if(W <= 0 || H <= 0){ return; }

            float CenterX = Width  / 2f;
            float CenterY = Height / 2f;

            for(int y = 0; y < H; y++){
                int SrcY = FlipY ? (H - 1 - (SrcYOffset + y)) : (SrcYOffset + y);

                for(int x = 0; x < W; x++){
                    int SrcX = FlipX ? (W - 1 - (SrcXOffset + x)) : (SrcXOffset + x);

                    float DX = SrcX - CenterX;
                    float DY = SrcY - CenterY;

                    int RotX = 0, RotY = 0;
                    switch (Rotation){
                        case TextureRotation.None:
                            RotX = SrcX;
                            RotY = SrcY;
                            break;
                        case TextureRotation.Rotate90:
                            RotX = (int)(CenterX + DY);
                            RotY = (int)(CenterY - DX);
                            break;
                        case TextureRotation.Rotate180:
                            RotX = (int)(CenterX - DX);
                            RotY = (int)(CenterY - DY);
                            break;
                        case TextureRotation.Rotate270:
                            RotX = (int)(CenterX - DY);
                            RotY = (int)(CenterY + DX);
                            break;
                    }

                    if(RotX < 0 || RotX >= Width || RotY < 0 || RotY >= Height){ continue; }

                    byte PaletteIndex = Pixels[RotY * (int)Width + RotX];
                    ColorB Color = Palette[PaletteIndex];

                    if(Color.A == 0){ continue; }

                    uint DstX = (uint)(DrawX + x);
                    uint DstY = (uint)(DrawY + y);

                    if(DstX >= C.Width || DstY >= C.Height){ continue; }

                    C.SetPixel(DstX, DstY, Color * MultiplyColor.Value, ImageBlend.Alpha);
                }
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при рендере текстуры [" + this + "]!", e);
        }
    }

    public void Render(Image.ImageContext C, Palette Palette, int X, int Y, uint TileWidth, uint TileHeight = 1, bool FlipX = false, bool FlipY = false, TextureRotation Rotation = TextureRotation.None, ColorB? MultiplyColor = null){
        if(TileWidth == 0 || TileHeight == 0){ return; }
        for(int Y__ = 0; Y__ < TileHeight; Y__++){
            for(int X__ = 0; X__ < TileWidth; X__++){
                Render(C, Palette, X + X__ * (int)Width, Y + Y__ * (int)Height, FlipX, FlipY, Rotation, MultiplyColor);
            }
        }
    }
}
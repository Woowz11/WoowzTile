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
        try{
            if(string.IsNullOrEmpty(Chars)){ throw new Exception("Chars пустой!"); }

            string[] Lines = Chars.Split(["\r\n", "\n"], StringSplitOptions.None);

            Height = (uint)Lines.Length;
            Width = (uint)Lines[0].Length;

            foreach(string Line in Lines){
                if(Line.Length != Width){ throw new Exception("Все строки должны быть одной длины!"); }
            }

            Pixels = new byte[Width * Height];

            for(int Y = 0; Y < Height; Y++){
                string Line = Lines[Y];
                for(int X = 0; X < Width; X++){
                    char C = Line[X];
                    if(!Mapping.TryGetValue(C, out byte Index)){ throw new Exception("Символ ['" + C + "'] не найден в Mapping!"); }

                    Pixels[Y * (int)Width + X] = Index;
                }
            }
        }
        catch(Exception e){
            throw new Exception("Произошла ошибка при создании текстуры [" + this + "]!\nТекстура:\n" + Chars, e);
        }
    }
    
    public uint   Width ;
    public uint   Height;
    public byte[] Pixels;

    public byte this[uint X, uint Y]{
        get => Pixels[Y * Width + X];
        set => Pixels[Y * Width + X] = value;
    }

    public void Render(Image.ImageContext C, Palette Palette, int X = 0, int Y = 0, int SrcX = 0, int SrcY = 0, uint SrcW = 0, uint SrcH = 0, uint DstW = 0, uint DstH = 0, bool FlipX = false, bool FlipY = false, TextureRotation Rotation = TextureRotation.None, ColorB? MultiplyColor = null){
        try{
            MultiplyColor ??= ColorB.White;

            int SW = (SrcW == 0 ? (int)Width  : (int)SrcW);
            int SH = (SrcH == 0 ? (int)Height : (int)SrcH);

            int DW = (DstW == 0 ? SW : (int)DstW);
            int DH = (DstH == 0 ? SH : (int)DstH);
            
            if(DW <= 0 || DH <= 0){ return; }

            int OffsetX = 0;
            int OffsetY = 0;

            if(X < 0){
                OffsetX = -X;
                DW += X;
                X = 0;
            }

            if(Y < 0){
                OffsetY = -Y;
                DH += Y;
                Y = 0;
            }

            if (X + DW > C.Width ){ DW = (int)C.Width  - X; }
            if (Y + DH > C.Height){ DH = (int)C.Height - Y; }
            if (DW <= 0 || DH <= 0){ return; }

            float CenterX = Width  / 2f;
            float CenterY = Height / 2f;

            for(int Y__ = 0; Y__ < DH; Y__++){
                int SrcY__ = FlipY ? SrcY + SH - 1 - ((Y__ + OffsetY) % SH) : SrcY + ((Y__ + OffsetY) % SH);

                for(int X__ = 0; X__ < DW; X__++){
                    int SrcX__ = FlipX ? SrcX + SW - 1 - ((X__ + OffsetX) % SW) : SrcX + ((X__ + OffsetX) % SW);

                    float DX = SrcX__ - CenterX;
                    float DY = SrcY__ - CenterY;
                    int RotX = 0, RotY = 0;
                    
                    switch (Rotation){
                        case TextureRotation.None     : RotX = SrcX__; RotY = SrcY__; break;
                        case TextureRotation.Rotate90 : RotX = (int)(CenterX + DY); RotY = (int)(CenterY - DX); break;
                        case TextureRotation.Rotate180: RotX = (int)(CenterX - DX); RotY = (int)(CenterY - DY); break;
                        case TextureRotation.Rotate270: RotX = (int)(CenterX - DY); RotY = (int)(CenterY + DX); break;
                    }

                    if(RotX < 0 || RotX >= Width || RotY < 0 || RotY >= Height){ continue; }

                    byte PaletteIndex = Pixels[RotY * (int)Width + RotX];
                    ColorB Color = Palette[PaletteIndex];

                    if(Color.A == 0){ continue; }

                    uint DstX = (uint)(X + X__);
                    uint DstY = (uint)(Y + Y__);

                    if(DstX >= C.Width || DstY >= C.Height){ continue; }

                    C.SetPixel(DstX, DstY, Color * MultiplyColor.Value, ImageBlend.Alpha);
                }
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при рендере текстуры [" + this + "]!", e);
        }
    }

    public void RenderTiles(Image.ImageContext C, Palette Palette, int X, int Y, uint TileWidth, uint TileHeight = 1, int SrcX = 0, int SrcY = 0, uint SrcW = 0, uint SrcH = 0, bool FlipX = false, bool FlipY = false, TextureRotation Rotation = TextureRotation.None, ColorB? MultiplyColor = null){
        if(TileWidth == 0 || TileHeight == 0){ return; }
        for(int Y__ = 0; Y__ < TileHeight; Y__++){
            for(int X__ = 0; X__ < TileWidth; X__++){
                Render(C, Palette, X + X__ * (int)Width, Y + Y__ * (int)Height, SrcX, SrcY, SrcW, SrcH, 0, 0, FlipX, FlipY, Rotation, MultiplyColor);
            }
        }
    }

    public void Render9Slice(Image.ImageContext C, Palette Palette, uint Cut, int X, int Y, uint Width, uint Height, bool FlipX = false, bool FlipY = false, TextureRotation Rotation = TextureRotation.None, ColorB? MultiplyColor = null){
        try{
            int Left   = (int)Cut;
            int Right  = (int)(this.Width - Cut);
            int Top    = (int)Cut;
            int Bottom = (int)(this.Height - Cut);

            int MiddleWidth  = (int)(Width - 2 * Cut);
            int MiddleHeight = (int)(Height - 2 * Cut);
            
            Render(C, Palette, X, Y, 0, 0, Cut, Cut, Cut, Cut, FlipX, FlipY, Rotation, MultiplyColor);
            Render(C, Palette, (int)(X + Width - Cut), Y, Right, 0, Cut, Cut, Cut, Cut, FlipX, FlipY, Rotation, MultiplyColor);
            Render(C, Palette, X, (int)(Y + Height - Cut), 0, Bottom, Cut, Cut, Cut, Cut, FlipX, FlipY, Rotation, MultiplyColor);
            Render(C, Palette, (int)(X + Width - Cut), (int)(Y + Height - Cut), Right, Bottom, Cut, Cut, Cut, Cut, FlipX, FlipY, Rotation, MultiplyColor);

            if(MiddleWidth > 0){
                Render(C, Palette, (int)(X + Cut), Y, Left, 0, (uint)(Right - Left), Cut, (uint)MiddleWidth, Cut, FlipX, FlipY, Rotation, MultiplyColor);
                Render(C, Palette, (int)(X + Cut), (int)(Y + Height - Cut), Left, Bottom, (uint)(Right - Left), Cut, (uint)MiddleWidth, Cut, FlipX, FlipY, Rotation, MultiplyColor);
            }

            if(MiddleHeight > 0){
                Render(C, Palette, X, (int)(Y + Cut), 0, Top, Cut, (uint)(Bottom - Top), Cut, (uint)MiddleHeight, FlipX, FlipY, Rotation, MultiplyColor);
                Render(C, Palette, (int)(X + Width - Cut), (int)(Y + Cut), Right, Top, Cut, (uint)(Bottom - Top), Cut, (uint)MiddleHeight, FlipX, FlipY, Rotation, MultiplyColor);
            }

            if(MiddleWidth > 0 && MiddleHeight > 0){
                Render(C, Palette, (int)(X + Cut), (int)(Y + Cut), Left, Top, (uint)(Right - Left), (uint)(Bottom - Top), (uint)MiddleWidth, (uint)MiddleHeight, FlipX, FlipY, Rotation, MultiplyColor);
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при рендере текстуры 9-Slice [" + this + "]!", e);
        }
    }
}
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

            Height = (uint)Lines   .Length;
            Width  = (uint)Lines[0].Length;

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
        try
        {
            MultiplyColor ??= ColorB.White;
            
            int SW = (int)(SrcW == 0 ? Width : SrcW), SH = (int)(SrcH == 0 ? Height : SrcH);
            
            int DW = DstW == 0 ? SW : (int)DstW, DH = DstH == 0 ? SH : (int)DstH;
            
            if(DW <= 0 || DH <= 0){ return; }
            int OffsetX = X < 0 ? -X : 0, OffsetY = Y < 0 ? -Y : 0;
            
            if (X < 0){ DW += X; X = 0; } 
            if(Y < 0){ DH += Y; Y = 0; }
            
            if(X + DW > C.Width ){ DW = (int)C.Width  - X; }
            if(Y + DH > C.Height){ DH = (int)C.Height - Y; }
            
            if(DW <= 0 || DH <= 0){ return; }
            
            float CX = Width / 2f, CY = Height / 2f;
            
            for(int y__ = 0; y__ < DH; y__++){
                int sy = FlipY ? SrcY + SH - 1 - ((y__ + OffsetY) % SH) : SrcY + ((y__ + OffsetY) % SH);
                for (int x__ = 0; x__ < DW; x__++){
                    int sx = FlipX ? SrcX + SW - 1 - ((x__ + OffsetX) % SW) : SrcX + ((x__ + OffsetX) % SW);
                    float dx = sx - CX, dy = sy - CY;
                    
                    int rx = Rotation switch { TextureRotation.None => sx, TextureRotation.Rotate90 => (int)(CX + dy), TextureRotation.Rotate180 => (int)(CX - dx), TextureRotation.Rotate270 => (int)(CX - dy), var _ => sx },
                        ry = Rotation switch { TextureRotation.None => sy, TextureRotation.Rotate90 => (int)(CY - dx), TextureRotation.Rotate180 => (int)(CY - dy), TextureRotation.Rotate270 => (int)(CY + dx), var _ => sy };
                    
                    if (rx < 0 || rx >= Width || ry < 0 || ry >= Height){ continue; }
                    ColorB color = Palette[Pixels[ry * (int)Width + rx]];
                    if(color.A == 0){ continue; }
                    uint DX = (uint)(X + x__), DY = (uint)(Y + y__);
                    if(DX < C.Width && DY < C.Height) C.SetPixel(DX, DY, color * MultiplyColor.Value, ImageBlend.Alpha);
                }
            }
        }
        catch (Exception e) { throw new Exception($"Произошла ошибка при рендере текстуры [{this}]!", e); }
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
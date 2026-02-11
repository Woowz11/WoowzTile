using WLO;

namespace WoowzTile.Objects;

public class Texture(uint Width, uint Height, byte[] Pixels){
    public uint   Width  = Width;
    public uint   Height = Height;
    public byte[] Pixels = Pixels;

    public byte this[uint X, uint Y]{
        get => Pixels[Y * Width + X];
        set => Pixels[Y * Width + X] = value;
    }

    public void Render(Image.ImageContext C, Palette Palette, int X, int Y){
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

            int MaxH = (int)C.Width  - DrawX;
            int MaxW = (int)C.Height - DrawY;

            if(W > MaxW){ W = MaxW; }
            if(H > MaxH){ H = MaxH; }

            if(W <= 0 || H <= 0){ return; }

            for(int Y__ = 0; Y__ < H; Y__++){
                int Row = (SrcY + Y__) * (int)Width;
                for(int X__ = 0; X__ < W; X__++){
                    byte PaletteIndex = Pixels[Row + (SrcX + X__)];
                    ColorB Color = Palette[PaletteIndex];

                    if(Color.A == 0){ continue; }

                    uint DstX = (uint)(DrawX + X__);
                    uint DstY = (uint)(DrawY + Y__);

                    ColorB Dst = C[DstX, DstY];

                    float A  = Color.A / 255F;
                    float IA = 1 - A;
                    
                    ColorB Result = new ColorB(
                        (byte)(Color.R * A + Dst.R * IA),
                        (byte)(Color.G * A + Dst.G * IA),
                        (byte)(Color.B * A + Dst.B * IA)
                    );

                    C[DstX, DstY] = Result;
                }
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при рендере текстуры [" + this + "]!", e);
        }
    }
}
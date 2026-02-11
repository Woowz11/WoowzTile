using WLO;

namespace WoowzTile.Objects;

public class Sprite{
    public Sprite(Texture Texture, Palette Palette){
        this.Texture = Texture;
        this.Palette = Palette;
    }
    
    public Texture Texture;
    public Palette Palette;
    
    public int X = 0;
    public int Y = 0;

    public void Render(Image.ImageContext C){
        try{
            Texture.Render(C, Palette, X, Y);
        }catch(Exception e){
            throw new Exception("Произошла ошибка при рендере спрайта [" + this + "]!", e);
        }
    }
}
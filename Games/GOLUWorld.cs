using WLO;
using WoowzTile.Objects;

namespace WoowzTile.Games;

public class GOLUWorld : Game{
    public Palette TestPalette;
    public Texture TestTexture;

    public Sprite TestSprite;
    
    public override void Start(){
        TestPalette = new Palette([
            new KeyValuePair<byte, ColorB>(1, ColorB.Black)
        ]);
        
        TestTexture = new Texture(8, 8, 
            [
                0,1,0,0,1,1,1,1,
                0,1,0,0,1,0,0,0,
                0,1,0,0,1,0,0,0,
                0,1,1,1,1,1,1,1,
                0,0,0,0,1,0,0,1,
                0,0,0,0,1,0,0,1,
                0,1,1,1,1,0,0,1,
                0,0,0,0,0,0,0,0,
            ]
        );

        TestSprite = new Sprite(TestTexture, TestPalette);
    }
    
    public override void Stop(){
        
    }
    
    public override void Update(TickData TD){
        
    }
    
    public override void Render(TickData TD, Image.ImageContext C){
        TestSprite.Render(C);
    }

    public override ColorB BackgroundColor(){
        return ColorB.White;
    }
}
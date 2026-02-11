using WLO;
using WoowzTile.Objects;

namespace WoowzTile.Games;

public class GOLUWorld : Game{
    public Palette TestPalette;
    public Texture TestTexture;

    public Sprite TestSprite;
    
    public override void Start(){
        TestPalette = new Palette([
            new KeyValuePair<byte, ColorB>(1, ColorB.Black),
            new KeyValuePair<byte, ColorB>(2, ColorB.DarkGray),
            new KeyValuePair<byte, ColorB>(3, ColorB.Gray),
            new KeyValuePair<byte, ColorB>(4, ColorB.LightGray),
            new KeyValuePair<byte, ColorB>(5, ColorB.White)
        ]);
        
        TestTexture = new Texture(
@"▓▒▒▒▒░░░░░▒▒▒▒██
▒█___█░█_____░▓█
▒█░█░█▒█______▓█
▒██▒██▒█░_____▒█
▒█░_░█░████___▒█
▒_____________▒█
▒_____________▒█
▒_____________▒█
▒_____________▒█
▒_____________▒█
▒_____________▒█
▒_____________▒█
▒_____________▒█
▒░___________░▓█
█▓▒▒▒▒▒▒▒▒▒▒▒▓▓█
████████████████",
new Dictionary<char, byte>{
    ['_'] = 5,
    ['█'] = 1,
    ['▓'] = 2,
    ['▒'] = 3,
    ['░'] = 4
}
        );

        TestSprite = new Sprite(TestTexture, TestPalette);
    }
    
    public override void Stop(){
        
    }
    
    public override void Update(TickData TD){
        
    }
    
    public override void Render(TickData TD, Image.ImageContext C){
        for(int i = 0; i < 16; i++){
            for(int j = 0; j < 16; j++){
                TestSprite.X = i * 16;
                TestSprite.Y = j * 16;
                TestSprite.Render(C);
            }
        }
    }

    public override ColorB BackgroundColor(){
        return ColorB.White;
    }
}
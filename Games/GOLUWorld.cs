using WLO;

namespace WoowzTile.Games;

public class GOLUWorld : Game{
    public override void Start(){
        
    }
    
    public override void Stop(){
        
    }
    
    public override void Update(TickData TD){
        
    }
    
    public override void Render(TickData TD, Image.ImageContext C){
        C.For((X, Y, W, H) => {
            C[X, Y] = new ColorB((byte)((float)X / W * 255), (byte)((float)Y / H * 255), 0);
        });
    }
}
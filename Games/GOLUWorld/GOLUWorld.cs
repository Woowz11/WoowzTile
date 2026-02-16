using WL;
using WLO;
using WoowzTile;
using static GOLUWorld.GOLUWorld_Resources;
using static GOLUWorld.GOLUWorld_UI;
using static GOLUWorld.GOLUWorld_Render;
using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Input;
using static GOLUWorld.GOLUWorld_World;

namespace GOLUWorld;

internal class GOLUWorld : Game{
    public override string Name(){ return Game_Name + " " + Game_Version; }
    
    public override void Start(){ Game_LoadResources(); }
    public override void Stop (){}

    public override void Update(TickData TD                      ) => Game_Update(   TD);
    public override void Render(TickData TD, Image.ImageContext C) => Game_Render(C, TD);
    
    public override void KeyPress(Key Key, bool Down) => Game_KeyPress(Key, Down);

    public override string WindowTitle    () => UI_WindowTitle;
    public override ColorB BackgroundColor() => World_BackgroundColor;
}
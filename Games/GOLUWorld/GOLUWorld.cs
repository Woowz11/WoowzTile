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

    public override void Start(){
        WL.WoowzLib.OnMessage += __OnMessage;
        
        Game_LoadResources();
    }
    public override void Stop(){
        WL.WoowzLib.OnMessage -= __OnMessage;
    }
    private static void __OnMessage(Logger.MessageType MessageType, object[]? Message){
        __Messages.Add((MessageType, WL.String.Join(Message)));
        Player_ConsoleOffset = 0;
    }
    internal static readonly List<(Logger.MessageType, string Content)> __Messages = [];

    public override void Update(TickData TD                      ) => Game_Update(   TD);
    public override void Render(TickData TD, Image.ImageContext C) => Game_Render(C, TD);
    
    public override void KeyPress(Key Key, bool Down) => Game_KeyPress(Key, Down);

    public override string WindowTitle    () => UI_WindowTitle;
    public override ColorB BackgroundColor() => World_BackgroundColor;
}
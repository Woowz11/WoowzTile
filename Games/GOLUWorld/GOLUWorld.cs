using WL;
using WLO;
using WoowzTile;
using static GOLUWorld.GOLUWorld_Resources;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_Render;
using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Input;
using static GOLUWorld.GOLUWorld_World;
using static GOLUWorld.GOLUWorld_Player;

namespace GOLUWorld;

internal class GOLUWorld : Game{
    public override string Name(){ return Game_Name + " " + Game_Version; }
    
    public override void Start(){
        Game_LoadResources();
    }
    public override void Stop(){}
    
    internal static void StartGame(){
        UI_InMainMenu = false;
        
        Coordinates_Camera = Vector2F.Zero;
        __Decals.Clear();

        World_Time = World_TimeMax / 2;
        
        Player_Health = Player_HealthMax;
        UI_Interface = 0;

        Player_InventorySelectedSlot = 0;

        Player_LastTimeWereTreatedTimer = 0;
        Player_Rotting = 0;

        Player_Thought = "";
        Player_ThoughtTimer = 0;
        
        Emotion_Happiness = Emotion_Max;

        Player_ClearInventory();
        Player_Inventory[0] = T_Item.FirstAidKit;
        Player_Inventory[1] = T_Item.FirstAidKit;
        Player_Inventory[2] = T_Item.GPS;

        World_Seed = World_GenerateNewSeed();
        
        StartLevel(T_Level.Calm);
    }

    public override void Update(TickData TD){
        if(Cheat_FastTime){
            for(int i = 0; i < Cheat_FastTime_Value; i++){
                Game_Update(TD);
            }
        }else{
            Game_Update(TD);
        }
    }
    public override void Render(TickData TD, Image.ImageContext C) => Game_Render(C, TD);
    public override void KeyPress(Key Key, bool Down) => Game_KeyPress(Key, Down);

    public override string WindowTitle(){ return Emotion_Happiness + " | " + Player_InteractingCollision + " (" + Player_CollisionInfo1 + ", " + Player_CollisionInfo2 + ", " + Player_CollisionInfo3 + ") | " + World_Seed + " | " + Cheat_IgnoreColliders + " | " + World_Time + " (" + World_DayPhase + ")"; }
    public override ColorB BackgroundColor() => World_BackgroundColor;
}
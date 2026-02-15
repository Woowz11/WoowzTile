using WL;
using WLO;
using WoowzTile;
using WoowzTile.Objects;
using static GOLUWorld.GW_Resources;
using static GOLUWorld.GW_Objects;
using static GOLUWorld.GW_Render;
using static GOLUWorld.GW_Values;
using static GOLUWorld.GW_Input;
using static GOLUWorld.GW_World;
#pragma warning disable CS8618

namespace GOLUWorld;

internal class GOLUWorld : Game{
    public override string Name(){ return Game_Name + " " + Game_Version; }

    public override string WindowTitle(){ return Emotion_Happiness + " | " + InsideCollision + " (" + CollisionInfo1 + ", " + CollisionInfo2 + ", " + CollisionInfo3 + ") | " + Seed + " | " + IgnoreColliders + " | " + Time + " (" + DayPhase + ")"; }

    public override void Start(){
        Game_LoadResources();
    }
    public override void Stop(){}
    public override void Update(TickData TD) => Game_Update(TD);
    public override void Render(TickData TD, Image.ImageContext C) => Game_Render(TD, C);
    public override void KeyPress(Key Key, bool Down) => Game_KeyPress(Key, Down);

    public override ColorB BackgroundColor() => WorldBackgroundColor;
    
    internal static void StartGame(){
        InMainMenu = false;
        
        WorldPosition = Vector2F.Zero;
        __Decals.Clear();

        Time = MaxTime / 2;
        
        Health = HealthMax;
        Interface = 0;

        SelectedItem = 0;

        LastHealed = 0;
        Rotten = 0;

        Thoughts = "";
        ThoughtsTimer = 0;
        
        Emotion_Happiness = Emotion_Max;
        
        Array.Clear(Inventory, 0, Inventory.Length);
        Inventory[0] = T_Item.FirstAidKit;
        Inventory[1] = T_Item.FirstAidKit;
        Inventory[2] = T_Item.GPS;

        Seed = (uint)WL.Math.Random.Fast_Int(0, 10000000);
        
        StartLevel(T_Level.Calm);
    }
}
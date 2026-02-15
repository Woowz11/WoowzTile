using WoowzTile;
using static GOLUWorld.GW_Values;
using static GOLUWorld.GW_Objects;
using static GOLUWorld.GW_World;

namespace GOLUWorld;

internal static class GW_UI{
    internal static void GoToMainMenu(){
        StartLevel(T_Level.None);
        
        InMainMenu = true;
        Interface = T_Interface.None;
        MenuSelectedButton = 0;
    }
    
    internal static void UseMainMenuButton(){
        if(Interface == T_Interface.None){
            switch(MenuSelectedButton){
                case 0:
                case 1: GOLUWorld.StartGame(); break;
                case 2:{
                    Interface = (T_Interface)1;
                    break;
                }
                case 3:{
                    Game.Quit();
                    break;
                }
            }
        }else{
            Interface = T_Interface.None;
        }
    }
}
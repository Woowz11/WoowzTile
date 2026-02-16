using WL;
using WLO;
using WoowzTile;
using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_World;
using static GOLUWorld.GOLUWorld_Player;
using static GOLUWorld.GOLUWorld_UI;

namespace GOLUWorld;

internal static class GOLUWorld_Input{
    internal static void Game_KeyPress(Key Key, bool Down){
        bool __Enter = Key is Key.Enter or Key.Space or Key.N5;
        bool __Up    = Key is Key.W or Key.Up or Key.N8;
        bool __Down  = Key is Key.S or Key.Down or Key.N2;
        bool __Right = Key is Key.D or Key.Right or Key.N6;
        bool __Left  = Key is Key.A or Key.Left or Key.N4;
        
        if(Down){
            if(UI_InMainMenu){
                if(__Enter){ UI_UseButton_MainMenu(); }

                if(UI_Interface == T_Interface.None){
                    if(__Up){
                        if(UI_MenuSelectedButton > 0){ UI_MenuSelectedButton -= 1; }
                    }

                    if(__Down){
                        if(UI_MenuSelectedButton < 3){ UI_MenuSelectedButton += 1; }
                    }
                }
            }else{
                if(Key == Key.C){ Cheat_RenderColliders = !Cheat_RenderColliders; }
                if(Key == Key.I){ Cheat_Immortality = !Cheat_Immortality; }
                if(Key == Key.X){ Cheat_IgnoreColliders = !Cheat_IgnoreColliders; }
                if(Key == Key.F){ Cheat_FastTime = !Cheat_FastTime; }
                if(Key == Key.Home){ Coordinates_Camera = Vector2F.Zero; }

                void UpdSeed(){
                    if(Game.KeyPressed(Key.Alt)){
                        if(Game.KeyPressed(Key.Shift)){ World_Seed -= 1; }else{ World_Seed += 1; }
                    }else{
                        if(Game.KeyPressed(Key.Shift)){ World_Seed = 0; }   
                    }
                    if(Game.KeyPressed(Key.Control)){ World_Seed = World_GenerateNewSeed(); }
                }
                
                if(Key == Key.F1){ UpdSeed(); StartLevel(T_Level.Calm); }
                if(Key == Key.F2){ UpdSeed(); StartLevel(T_Level.Industrial); }

                if(Key == Key.Escape || (UI_Interface == T_Interface.Menu && UI_MenuSelectedButton == 0 && __Enter)){
                    if(UI_Interface == T_Interface.None){ UI_Interface = T_Interface.Menu; }else{ UI_Interface = T_Interface.None; UI_MenuSelectedButton = 0; }
                }

                if(!Player_Dead){
                    if(Key == Key.Tab){ UI_Interface = UI_Interface == T_Interface.None ? T_Interface.Inventory : T_Interface.None; }
                    
                    if(__Enter){ UseItem(); }
                    
                    if(Key == Key.E){ Use(); }

                    if(Key == Key.Backspace){
                        T_Item Item = Player_ItemInHands;
                        if(Item != T_Item.Empty){
                            SpawnItem(Coordinates_WorldPlayer.X, Coordinates_WorldPlayer.Y, Item);
                            Player_ItemInHands = T_Item.Empty;
                        }
                    }
                    
                    switch(Key){
                        case Key.D1:
                            Player_InventorySelectedSlot = 0;
                            break;
                        case Key.D2:
                            Player_InventorySelectedSlot = 1;
                            break;
                        case Key.D3:
                            Player_InventorySelectedSlot = 2;
                            break;
                        case Key.D4:
                            Player_InventorySelectedSlot = 3;
                            break;
                        case Key.D5:
                            Player_InventorySelectedSlot = 4;
                            break;
                        case Key.D6:
                            Player_InventorySelectedSlot = 5;
                            break;
                        case Key.D7:
                            Player_InventorySelectedSlot = 6;
                            break;
                        case Key.D8:
                            Player_InventorySelectedSlot = 7;
                            break;
                        case Key.D9:
                            Player_InventorySelectedSlot = 8;
                            break;
                        case Key.D0:
                            Player_InventorySelectedSlot = 9;
                            break;
                        case Key.Minus:
                            Player_InventorySelectedSlot = 10;
                            break;
                        case Key.Plus:
                            Player_InventorySelectedSlot = 11;
                            break;
                    }
                }

                if(UI_Interface == T_Interface.Inventory){
                    byte OldSelectedItem = Player_InventorySelectedSlot;
                    
                    if(__Right){
                        if(Player_InventorySelectedSlot > 5){
                            if(Player_InventorySelectedSlot < 11){ Player_InventorySelectedSlot++; }
                        }else{
                            if(Player_InventorySelectedSlot < 5){ Player_InventorySelectedSlot++; }
                        }
                    }

                    if(__Left){
                        if(Player_InventorySelectedSlot > 5){
                            if(Player_InventorySelectedSlot > 6){ Player_InventorySelectedSlot--; }
                        }else{
                            if(Player_InventorySelectedSlot > 0){ Player_InventorySelectedSlot--; }
                        }
                    }

                    if(__Down){
                        if(Player_InventorySelectedSlot + 6 < Player_InventorySlotsMax){ Player_InventorySelectedSlot += 6; }
                    }
                    
                    if(__Up){
                        if(Player_InventorySelectedSlot - 6 > -1){ Player_InventorySelectedSlot -= 6; }
                    }

                    if(Player_InventorySelectedSlot != OldSelectedItem && Game.KeyPressed(Key.Shift)){
                        (Player_Inventory[Player_InventorySelectedSlot], Player_Inventory[OldSelectedItem]) = (Player_Inventory[OldSelectedItem], Player_Inventory[Player_InventorySelectedSlot]);
                    }
                }else if(UI_Interface == T_Interface.Menu){
                    if(__Up){
                        if(UI_MenuSelectedButton > 0){ UI_MenuSelectedButton -= 1; }
                    }

                    if(__Down){
                        if(UI_MenuSelectedButton < 1){ UI_MenuSelectedButton += 1; }
                    }

                    if(__Enter){
                        if(UI_MenuSelectedButton == 1){
                            UI_GoToMainMenu();
                        }
                    }
                }
            }
        }
    }
}
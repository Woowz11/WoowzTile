using WL;
using WLO;
using WoowzTile;
using static GOLUWorld.GW_Values;
using static GOLUWorld.GW_Objects;
using static GOLUWorld.GW_World;
using static GOLUWorld.GW_Player;
using static GOLUWorld.GW_UI;

namespace GOLUWorld;

internal static class GW_Input{
    internal static void Game_KeyPress(Key Key, bool Down){
        bool __Enter = Key is Key.Enter or Key.Space or Key.N5;
        bool __Up    = Key is Key.W or Key.Up or Key.N8;
        bool __Down  = Key is Key.S or Key.Down or Key.N2;
        bool __Right = Key is Key.D or Key.Right or Key.N6;
        bool __Left  = Key is Key.A or Key.Left or Key.N4;
        
        if(Down){
            if(InMainMenu){
                if(__Enter){ UseMainMenuButton(); }

                if(Interface == T_Interface.None){
                    if(__Up){
                        if(MenuSelectedButton > 0){ MenuSelectedButton -= 1; }
                    }

                    if(__Down){
                        if(MenuSelectedButton < 3){ MenuSelectedButton += 1; }
                    }
                }
            }else{
                if(Key == Key.C){ RenderColliders = !RenderColliders; }
                if(Key == Key.I){ Immortality = !Immortality; }
                if(Key == Key.X){ IgnoreColliders = !IgnoreColliders; }
                if(Key == Key.Home){ WorldPosition = Vector2F.Zero; }

                void UpdSeed(){
                    if(Game.KeyPressed(Key.Alt)){
                        if(Game.KeyPressed(Key.Shift)){ Seed -= 1; }else{ Seed += 1; }
                    }else{
                        if(Game.KeyPressed(Key.Shift)){ Seed = 0; }   
                    }
                    if(Game.KeyPressed(Key.Control)){ Seed = (uint)WL.Math.Random.Fast_Int(0, 10000000); }
                }
                
                if(Key == Key.F1){ UpdSeed(); StartLevel(T_Level.Calm); }
                if(Key == Key.F2){ UpdSeed(); StartLevel(T_Level.Industrial); }

                if(Key == Key.Escape || (Interface == T_Interface.Menu && MenuSelectedButton == 0 && __Enter)){
                    if(Interface == T_Interface.None){ Interface = T_Interface.Menu; }else{ Interface = T_Interface.None; MenuSelectedButton = 0; }
                }

                if(!Dead){
                    if(Key == Key.Tab){ Interface = Interface == T_Interface.None ? T_Interface.Inventory : T_Interface.None; }
                    
                    if(__Enter){ UseItem(); }
                    
                    if(Key == Key.E){ Use(); }

                    if(Key == Key.Backspace){
                        T_Item Item = Inventory[SelectedItem];
                        if(Item != T_Item.Empty){
                            SpawnItem(PlayerX - WorldX, PlayerY - WorldY, Item);
                            Inventory[SelectedItem] = T_Item.Empty;
                        }
                    }
                    
                    switch(Key){
                        case Key.D1:
                            SelectedItem = 0;
                            break;
                        case Key.D2:
                            SelectedItem = 1;
                            break;
                        case Key.D3:
                            SelectedItem = 2;
                            break;
                        case Key.D4:
                            SelectedItem = 3;
                            break;
                        case Key.D5:
                            SelectedItem = 4;
                            break;
                        case Key.D6:
                            SelectedItem = 5;
                            break;
                        case Key.D7:
                            SelectedItem = 6;
                            break;
                        case Key.D8:
                            SelectedItem = 7;
                            break;
                        case Key.D9:
                            SelectedItem = 8;
                            break;
                        case Key.D0:
                            SelectedItem = 9;
                            break;
                        case Key.Minus:
                            SelectedItem = 10;
                            break;
                        case Key.Plus:
                            SelectedItem = 11;
                            break;
                    }
                }

                if(Interface == T_Interface.Inventory){
                    byte OldSelectedItem = SelectedItem;
                    
                    if(__Right){
                        if(SelectedItem > 5){
                            if(SelectedItem < 11){ SelectedItem++; }
                        }else{
                            if(SelectedItem < 5){ SelectedItem++; }
                        }
                    }

                    if(__Left){
                        if(SelectedItem > 5){
                            if(SelectedItem > 6){ SelectedItem--; }
                        }else{
                            if(SelectedItem > 0){ SelectedItem--; }
                        }
                    }

                    if(__Down){
                        if(SelectedItem + 6 < MaxSlots){ SelectedItem += 6; }
                    }
                    
                    if(__Up){
                        if(SelectedItem - 6 > -1){ SelectedItem -= 6; }
                    }

                    if(SelectedItem != OldSelectedItem && Game.KeyPressed(Key.Shift)){
                        (Inventory[SelectedItem], Inventory[OldSelectedItem]) = (Inventory[OldSelectedItem], Inventory[SelectedItem]);
                    }
                }else if(Interface == T_Interface.Menu){
                    if(__Up){
                        if(MenuSelectedButton > 0){ MenuSelectedButton -= 1; }
                    }

                    if(__Down){
                        if(MenuSelectedButton < 1){ MenuSelectedButton += 1; }
                    }

                    if(__Enter){
                        if(MenuSelectedButton == 1){
                            GoToMainMenu();
                        }
                    }
                }
            }
        }
    }
}
using WL;
using WLO;
using WoowzTile;
using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_World;
using static GOLUWorld.GOLUWorld_Player;
using static GOLUWorld.GOLUWorld_UI;
using static GOLUWorld.GOLUWorld_Generator;

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
                void UpdSeed(){
                    if(Game.KeyPressed(Key.Alt)){
                        if(Game.KeyPressed(Key.Shift)){ World_Seed -= 1; }else{ World_Seed += 1; }
                    }else{
                        if(Game.KeyPressed(Key.Shift)){ World_Seed = 0; }   
                    }
                    if(Game.KeyPressed(Key.Control)){ World_Seed = World_GenerateNewSeed(); }
                }
                
                if(Key == Key.F1){ UpdSeed(); World_GoToWorld(T_World.Calm); }
                if(Key == Key.F2){ UpdSeed(); World_GoToWorld(T_World.Industrial); }
                if(Key == Key.F3){
                    Generator_DebugStructure(Coordinates_PlayerWorld.X / 16, Coordinates_PlayerWorld.Y / 16);
                }
                if(Key == Key.F4){ World_Start(); }

                if(Key == Key.Tilde){
                    if(UI_Interface == T_Interface.None){ UI_Interface = T_Interface.Console; }else{
                        UI_Interface = UI_Interface == T_Interface.Console ? T_Interface.None : T_Interface.Console;
                    }
                }
                
                if(Key == Key.Escape || (UI_Interface == T_Interface.Menu && UI_MenuSelectedButton == 0 && __Enter)){
                    if(UI_Interface == T_Interface.None){ UI_Interface = T_Interface.Menu; }else{ UI_Interface = T_Interface.None; UI_MenuSelectedButton = 0; }
                }
                
                if(Key == Key.Tab && Player_Attack_Timer <= 0 && !Player_Dead){ UI_Interface = UI_Interface == T_Interface.None ? T_Interface.Inventory : T_Interface.None; }

                if(UI_Interface == T_Interface.None){
                    if(Key == Key.C){ Cheat_RenderColliders = !Cheat_RenderColliders; }
                    if(Key == Key.I){ Cheat_Immortality = !Cheat_Immortality; }
                    if(Key == Key.X){ Cheat_IgnoreColliders = !Cheat_IgnoreColliders; }
                    if(Key == Key.F){ Cheat_FastTime = !Cheat_FastTime; }
                    if(Key == Key.B){ Cheat_DisableWorldLimit = !Cheat_DisableWorldLimit; }

                    if(!Player_Dead){
                        if(Key == Key.Home){ Player_Teleport(Coordinates_Spawn.X, Coordinates_Spawn.Y); }
                        
                        if(__Enter){ Player_ItemUse(); }
                    
                        if(Key is Key.Right or Key.Left or Key.Up or Key.Down){
                            Player_ItemUse(Key switch{
                                Key.Right => Direction4.Right,
                                Key.Left  => Direction4.Left,
                                Key.Up    => Direction4.Up,
                                Key.Down  => Direction4.Down
                            });
                        }
                        
                        if(Key == Key.E){ Player_Interact(); }

                        if(Key == Key.Backspace){ Player_ItemDrop(); }
                        
                        switch(Key){
                            case Key.D1:
                                Player_ItemSwitch(0);
                                break;
                            case Key.D2:
                                Player_ItemSwitch(1);
                                break;
                            case Key.D3:
                                Player_ItemSwitch(2);
                                break;
                            case Key.D4:
                                Player_ItemSwitch(3);
                                break;
                            case Key.D5:
                                Player_ItemSwitch(4);
                                break;
                            case Key.D6:
                                Player_ItemSwitch(5);
                                break;
                            case Key.D7:
                                Player_ItemSwitch(6);
                                break;
                            case Key.D8:
                                Player_ItemSwitch(7);
                                break;
                            case Key.D9:
                                Player_ItemSwitch(8);
                                break;
                            case Key.D0:
                                Player_ItemSwitch(9);
                                break;
                            case Key.Minus:
                                Player_ItemSwitch(10);
                                break;
                            case Key.Plus:
                                Player_ItemSwitch(11);
                                break;
                        }
                    }
                }

                if(UI_Interface == T_Interface.Inventory && Player_Attack_Timer <= 0){
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
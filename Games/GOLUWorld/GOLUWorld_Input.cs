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
                        if(UI_MenuSelectedButton < 4){ UI_MenuSelectedButton += 1; }
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
                
                switch(Key){
                    case Key.F1: UpdSeed(); World_GoToWorld(T_World.Calm); break;
                    case Key.F2: UpdSeed(); World_GoToWorld(T_World.Industrial); break;
                    case Key.F3: Generator_DebugStructure(Coordinates_PlayerWorld.X / 16, Coordinates_PlayerWorld.Y / 16); break;
                    case Key.F4: World_Start(); break;
                    case Key.F5: Player_Inventory[1] = T_Item.Destroyer; Player_Inventory[2] = T_Item.GPS; break;
                    case Key.F6: World_SetBlock(new Block{ ID = T_Block.Bricks, X = Coordinates_PlayerWorld.X/16, Y = (Coordinates_PlayerWorld.Y)/16 + 1}); break;
                    case Key.F7: Logger.Debug("1\n2\n3\n4\n5"); break;
                }

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
                    if(Key == Key.M){ Cheat_FastCycleTime = !Cheat_FastCycleTime; }
                    if(Key == Key.B){ Cheat_DisableWorldLimit = !Cheat_DisableWorldLimit; }

                    if(!Player_Dead){
                        if(Key == Key.Home){ Player_Teleport(Coordinates_Spawn.X, Coordinates_Spawn.Y); }
                    
                        if(Key is Key.Right or Key.Left or Key.Up or Key.Down or Key.N2 or Key.N4 or Key.N6 or Key.N8){
                            Player_ItemUse(Key switch{
                                Key.Right => Direction4.Right,
                                Key.Left  => Direction4.Left,
                                Key.Up    => Direction4.Up,
                                Key.Down  => Direction4.Down,
                                Key.N6    => Direction4.Right,
                                Key.N4    => Direction4.Left,
                                Key.N8    => Direction4.Up,
                                Key.N2    => Direction4.Down
                            });
                        }
                        
                        if(Key == Key.E){ Player_Interact(); }
                    }
                }

                if(UI_Interface is T_Interface.None or T_Interface.Inventory && !Player_Dead){
                    if(__Enter){ Player_ItemUse(); }
                    
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
                
                if(UI_Interface == T_Interface.Inventory && Player_Attack_Timer <= 0){
                    byte OldSelectedItem = Player_InventorySelectedSlot;
                    
                    if(__Right){
                        switch(Player_InventorySelectedSlot){
                            case > 5: if(Player_InventorySelectedSlot < 11){ Player_InventorySelectedSlot++; } break;
                            case < 5: Player_InventorySelectedSlot++; break;
                        }
                    }

                    if(__Left){
                        switch(Player_InventorySelectedSlot){
                            case > 5: if(Player_InventorySelectedSlot > 6){ Player_InventorySelectedSlot--; } break;
                            case > 0: Player_InventorySelectedSlot--; break;
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
                        if(UI_MenuSelectedButton < 2){ UI_MenuSelectedButton += 1; }
                    }

                    if(__Enter){
                        if(UI_MenuSelectedButton == 2){
                            UI_GoToMainMenu();
                        }
                    }
                }else if(UI_Interface == T_Interface.Console){
                    Player_Console(Key);
                }else if(UI_Interface == T_Interface.Storage12){
                    const byte Columns = 6;
                    const byte Rows = 4;

                    byte GetRow(byte Index) => Index >= 12 ? (byte)((Index - 12) / Columns) : (byte)((Index / Columns) + 2);
                    byte GetCol(byte Index) => (byte)(Index % Columns);
                    byte FromRowCol(byte Row, byte Col) => Row < 2 ? (byte)(12 + Row * Columns + Col) : (byte)((Row - 2) * Columns + Col);

                    void MoveSelectedSlot(ref byte Slot, int DRow, int DCol){
                        byte Row = GetRow(Slot);
                        byte Col = GetCol(Slot);

                        int NewRow = Row + DRow;
                        int NewCol = Col + DCol;

                        NewCol = WL.Math.ClampI(NewCol, 0, Columns - 1);
                        NewRow = WL.Math.ClampI(NewRow, 0, Rows - 1);

                        Slot = FromRowCol((byte)NewRow, (byte)NewCol);
                    }
                    
                    void MoveInventory(ref byte Slot, byte SelectedSlot, byte PreviousSelectedSlot, int DRow, int DCol){
                        if(SelectedSlot >= 12){ return; }

                        if (PreviousSelectedSlot >= 12){
                            Slot = FromRowCol(GetRow(Slot), GetCol(SelectedSlot));
                            return;
                        }

                        byte RowSlot = GetRow(Slot);
                        byte ColSlot = GetCol(Slot);

                        int NewRow = RowSlot + DRow;
                        int NewCol = ColSlot + DCol;

                        NewRow = WL.Math.ClampI(NewRow, 2, 3);
                        NewCol = WL.Math.ClampI(NewCol, 0, Columns - 1);

                        Slot = FromRowCol((byte)NewRow, (byte)NewCol);
                    }

                    byte OldSlot = UI_SelectedSlot;
                    T_Item OldItem;
                    if(OldSlot < 12){
                        OldItem = Player_Inventory[OldSlot];
                    }else{
                        OldItem = (T_Item)UI_OpenEntity!.Value.InfoData[OldSlot - 12];
                    }
                    
                    byte PreviousUI = UI_SelectedSlot;

                    if(__Right){
                        MoveSelectedSlot(ref UI_SelectedSlot, 0, 1);
                        MoveInventory(ref Player_InventorySelectedSlot, UI_SelectedSlot, PreviousUI, 0, 1);
                    }
                    
                    if(__Left){
                        MoveSelectedSlot(ref UI_SelectedSlot, 0, -1);
                        MoveInventory(ref Player_InventorySelectedSlot, UI_SelectedSlot, PreviousUI, 0, -1);
                    }
                    
                    if(__Down){
                        MoveSelectedSlot(ref UI_SelectedSlot, 1, 0);
                        MoveInventory(ref Player_InventorySelectedSlot, UI_SelectedSlot, PreviousUI, 1, 0);
                    }
                    
                    if(__Up){
                        MoveSelectedSlot(ref UI_SelectedSlot, -1, 0);
                        MoveInventory(ref Player_InventorySelectedSlot, UI_SelectedSlot, PreviousUI, -1, 0);
                    }


                    if(Game.KeyPressed(Key.Shift)){
                        T_Item NewItem;
                        if(UI_SelectedSlot < 12){
                            NewItem = Player_Inventory[UI_SelectedSlot];
                        }else{
                            NewItem = (T_Item)UI_OpenEntity!.Value.InfoData[UI_SelectedSlot - 12];
                        }

                        Entity OpenEntity = UI_OpenEntity!.Value;
                        
                        switch(UI_SelectedSlot){
                            case < 12:
                                Player_Inventory[UI_SelectedSlot] = OldItem;
                                break;
                            
                            case >= 12:
                                OpenEntity.InfoData[UI_SelectedSlot - 12] = (byte)OldItem;
                                break;
                        }

                        switch(OldSlot){
                            case < 12:
                                Player_Inventory[OldSlot] = NewItem;
                                break;
                            
                            case >= 12:
                                OpenEntity.InfoData[OldSlot - 12] = (byte)NewItem;
                                break;
                        }

                        World_Entities[OpenEntity.Key] = OpenEntity;
                        UI_OpenEntity = OpenEntity;
                    }
                }
            }
        }
    }
}
using WL;
using WLO;
using WoowzTile;
using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_World;
using static GOLUWorld.GOLUWorld_Resources;
using static GOLUWorld.GOLUWorld_Render;
using static GOLUWorld.GOLUWorld_Info;

namespace GOLUWorld;

internal static class GOLUWorld_UI{
    /// <summary>
    /// Открывает главное меню
    /// </summary>
    internal static void UI_GoToMainMenu(){
        World_GoToWorld(T_World.None);
        
        UI_InMainMenu = true;
        UI_Interface = T_Interface.None;
        UI_MenuSelectedButton = 0;
    }
    
    /// <summary>
    /// Использование кнопок в главном меню
    /// </summary>
    internal static void UI_UseButton_MainMenu(){
        if(UI_Interface == T_Interface.None){
            switch(UI_MenuSelectedButton){
                case 0:
                case 1: World_Start(); break;
                case 2: UI_Interface = (T_Interface)1; break;
                case 3: break;
                case 4: Game.Quit(); break;
            }
        }else{
            UI_Interface = T_Interface.None;
        }
    }
    
    /// <summary>
    /// Рендерит главное меню
    /// </summary>
    internal static void UI_RenderMainMenu(Image.ImageContext C, TickData TD){
        UI_RenderMainMenu_Background(C, TD);

        switch(UI_Interface){
            case T_Interface.None:
                UI_RenderMainMenu_Menu(C, TD);
                break;
            case (T_Interface)1:
                UI_RenderMainMenu_Help(C);
                break;
        }
        
        UI_RenderFinal(C, TD);
    }
    
    /// <summary>
    /// Рендерит задний фон главного меню
    /// </summary>
    internal static void UI_RenderMainMenu_Background(Image.ImageContext C, TickData TD){
        Texture_GOLU.RenderTiles(C, Palette_Default, -(int)(WL.Math.DCos((float)TD.DeltaTick / 2) * 128 * 4), -(int)(WL.Math.DSin((float)TD.DeltaTick / 2) * 128 * 4), 6, 6, MultiplyColor: ColorB.Random.SetA(5));
    }
    
    /// <summary>
    /// Рендер простой кнопки (просто текст и обводка)
    /// </summary>
    internal static void UI_EasyButton(Image.ImageContext C, byte ButtonID, string ButtonText, int X, int Y, bool Always = false, bool Disabled = false){
        if(UI_MenuSelectedButton == ButtonID || Always){
            Render_TextColorOutline(C, ButtonText, X, Y, Disabled ? ColorB.Gray : ColorB.White, ColorB.Red);
        }else{
            Render_TextColorOutline(C, ButtonText, X, Y, ColorB.Black, Disabled ? ColorB.Gray : ColorB.White);
        }
    }

    /// <summary>
    /// Рендерит меню главного меню
    /// </summary>
    internal static void UI_RenderMainMenu_Menu(Image.ImageContext C, TickData TD){
        Texture_Author.Render(C, Palette_Default, (int)(C.Width - Texture_Author.Width) - 3, (int)(C.Height - Texture_Author.Height) - 3);
        Font_Default.Render(C, Palette_Default, Game_Version, 3, (int)(C.Height - 8 - 3));

        void RenderGOLU(float SinOffset, ColorB MultiplyColor){
            Texture_G.Render(C, Palette_Default, (int)(C.Width / 2f - Texture_G.Width / 2f - Texture_G.Width * 1.5f) + (byte)(WL.Math.DCos((float)TD.DeltaTick * 2     + SinOffset) * 10), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2     + SinOffset) * 10), MultiplyColor: MultiplyColor);
            Texture_O.Render(C, Palette_Default, (int)(C.Width / 2f - Texture_G.Width / 2f - Texture_G.Width /   2f) + (byte)(WL.Math.DCos((float)TD.DeltaTick * 2 + 1 + SinOffset) * 10), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2 + 1 + SinOffset) * 10), MultiplyColor: MultiplyColor);
            Texture_L.Render(C, Palette_Default, (int)(C.Width / 2f - Texture_G.Width / 2f + Texture_G.Width /   2f) + (byte)(WL.Math.DCos((float)TD.DeltaTick * 2 + 2 + SinOffset) * 10), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2 + 2 + SinOffset) * 10), MultiplyColor: MultiplyColor);
            Texture_U.Render(C, Palette_Default, (int)(C.Width / 2f - Texture_G.Width / 2f + Texture_G.Width * 1.5f) + (byte)(WL.Math.DCos((float)TD.DeltaTick * 2 + 3 + SinOffset) * 10), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2 + 3 + SinOffset) * 10), MultiplyColor: MultiplyColor);
        }

        for(int i = 0; i < 10; i++){
            RenderGOLU(-0.1f * i, ColorB.Black.SetA((byte)(255 * 0.1f)));
        }
        RenderGOLU(0, ColorB.White);

        C.Fill((int)(C.Width / 2f - Texture_G.Width / 2f - Texture_G.Width * 1.5F), 75, 127, 2, ColorB.Black);
        
        Texture_Title.Render(C, Palette_Default, (int)(C.Width/2 - Texture_Title.Width/2), 80);
        
        UI_EasyButton(C, 0, "ЗАГРУЗИТЬ", (int)(C.Width / 2 - Font_Default.TextSize("ЗАГРУЗИТЬ").X / 2), 150 + (0 * 13));
        UI_EasyButton(C, 1, "НОВАЯ", (int)(C.Width / 2 - Font_Default.TextSize("НОВАЯ").X / 2), 150 + (1 * 13));
        UI_EasyButton(C, 2, "ПОМОЩЬ", (int)(C.Width / 2 - Font_Default.TextSize("ПОМОЩЬ").X / 2), 150 + (2 * 13));
        UI_EasyButton(C, 3, "НАСТРОЙКИ", (int)(C.Width / 2 - Font_Default.TextSize("НАСТРОЙКИ").X / 2), 150 + (3 * 13), Disabled: true);
        UI_EasyButton(C, 4, "ВЫЙТИ", (int)(C.Width / 2 - Font_Default.TextSize("ВЫЙТИ").X / 2), 150 + (4 * 13));
    }

    /// <summary>
    /// Рендерит информацию об управлении в главном меню
    /// </summary>
    /// <param name="C"></param>
    internal static void UI_RenderMainMenu_Help(Image.ImageContext C){
        string[] HelpInfo = [
            "[W,S,A,D] - ДВИЖЕНИЕ",
            "[SHIFT] - БЕГ",
            "[CONTROL] - КРАСТЬСЯ",
            "[E] - ВЗАИМОДЕЙСТВОВАТЬ",
            "[BACKSPACE] - ВЫБРОСИТЬ",
            "[TAB] - ИНВЕНТАРЬ",
            "[1-9] ВЫБРАТЬ СЛОТ",
            "[ENTER,SPACE] - ВЫПОЛНИТЬ",
            "[ESC] - ВЫХОД",
            "[C] - ПОКАЗАТЬ КОЛЛАЙДЕРЫ",
            "[I] - БЕССМЕРТИЕ iс",
            "[X] - ИГНОРИРОВАТЬ КОЛЛАЙДЕРЫ",
            "[F] - БЫСТРОЕ ВРЕМЯ",
            "[B] - ОТКЛЮЧАЕТ ГРАНИЦЫ",
            "[M] - УСКОР. ЦИКЛ ДНЯ И НОЧИ",
            "[HOME] - ТЕЛЕПОРТ В ЦЕНТР",
            "[F1-F6] - ТЕСТОВОЕ",
            "",
            "[ДВИЖЕНИЕ + SHIFT] - ДВИГАТЬ ПРЕДМЕТЫ\nВ ИНВЕНТОРЕ"
        ];

        for(int i = 0; i < HelpInfo.Length; i++){
            string HelpInfoMessage = HelpInfo[i];
                
            Font_Default.Render(C, Palette_Default, HelpInfoMessage, 30, 5 + i * 9);
        }
            
        UI_EasyButton(C, 0, "ОБРАТНО", (int)(C.Width / 2 - Font_Default.TextSize("ОБРАТНО").X / 2), 150 + (5 * 13), true);
    }

    /// <summary>
    /// Рендерит UI
    /// </summary>
    internal static void UI_Render(Image.ImageContext C, TickData TD){
        Texture FrameTexture = World_Type is T_World.Calm ? Texture_Frame : Texture_Frame_Industrial;
        
        FrameTexture.Render(C, Palette_World, 1, 1);

        T_Item Item = Player_ItemInHands;
        
        if(UI_Interface is T_Interface.None or T_Interface.Menu){
            void RenderSlideBar(int X, int Y, ColorB Color, uint Value, uint MaxValue, string Text, Texture Icon){
                C.Fill(X + 17 - 1, (int)C.Height + Y - 1, MaxValue + 2, 8 + 2, Color - new ColorB(64, 64, 64));
                C.Fill(X + 17, (int)C.Height + Y, MaxValue, 8, ColorB.Black);
                C.Fill(X + 17, (int)C.Height + Y, Value, 8, Color);
                C.Fill(X + 17, (int)C.Height + Y + 3, Value, 8 - 6, Color + new ColorB(64, 64, 64));

                Font_Default.Render(C, Palette_Default, Text, X + 17, (int)C.Height + Y);
        
                Icon.Render(C, Palette_Default, X - 1, (int)C.Height + (Y - 4));
            }
        
            RenderSlideBar(6, -19, ColorB.Red, Player_Health, Player_Health_Max, Cheat_Immortality ? "i" : Player_Health.ToString(), Texture_Health);
            RenderSlideBar(6, -19 - 16, Palette_Default[19], Player_Energy, Player_Energy_Max, Cheat_Immortality ? "i" : Player_Energy.ToString(), Texture_Energy);
                
            string __Text = (Item == T_Item.Empty ? "" : Info_Item_Name(Item)) + " [" + (Player_InventorySelectedSlot + 1) + "]";
            Render_TextColorOutline(C, __Text, (int)C.Width - (int)Font_Default.TextSize(__Text).X - 7, (int)C.Height - 8 - 7, ColorB.Black, ColorB.White);
        }
        
        if(UI_Interface != T_Interface.None){ C.Fill(ColorB.Black.SetA(128), ImageBlend.Alpha); }
        
        switch(UI_Interface){
            case T_Interface.Inventory: UI_RenderInventory(C    ); break;
            case T_Interface.Menu     : UI_RenderMenu     (C    ); break;
            case T_Interface.Console  : UI_RenderConsole  (C, TD); break;
            case T_Interface.Storage12: UI_RenderStorage  (C    ); break;
        }
        
        UI_RenderFinal(C, TD);
    }

    /// <summary>
    /// Рендерит слот с предметом
    /// </summary>
    internal static void UI_RenderSlot(Image.ImageContext C, int X, int Y, T_Item Item, bool Selected){
        C.Fill(X, Y, 34, 34, Selected ? ColorB.Red : ColorB.Black);
        Texture_Slot.Render(C, Palette_Default, X + 1, Y + 1, MultiplyColor: Selected ? ColorB.Lerp(ColorB.Gray, ColorB.Red, 0.5f) : ColorB.Gray);

        if(Selected){
            C.Border(X - 1, Y - 1, 34 + 2, 34 + 2, 1, ColorB.Red.SetA(128), ImageBlend.Alpha);
        }

        if(Item != 0){
            Info_Item_Icon(Item).Render(C, Palette_Default, X + 1, Y + 1);
        }
    }

    /// <summary>
    /// Рендерит кол-во денег
    /// </summary>
    internal static void UI_RenderMoney(Image.ImageContext C){
        string Money = Player_Money.ToString();
        int MoneyX = (int)(C.Width - Font_Default.TextSize(Money).X) - 10;
        Render_TextColorOutline(C, Money, MoneyX, 5, ColorB.Red, ColorB.White);
        Texture_Money.Render(C, Palette_Default, MoneyX - 17, 2);
    }

    /// <summary>
    /// Рендерит слоты хранилища 12
    /// </summary>
    internal static void UI_RenderSlots_Storage12(Image.ImageContext C, int X, int Y, T_Item Item1, T_Item Item2, T_Item Item3, T_Item Item4, T_Item Item5, T_Item Item6, T_Item Item7, T_Item Item8, T_Item Item9, T_Item Item10, T_Item Item11, T_Item Item12, int SelectedSlot){
        void RenderSlot(Image.ImageContext C, byte ID, int X__, int Y__, T_Item Item, bool Selected) => UI_RenderSlot(C, X + X__ * 36, Y + Y__ * 36, Item, Selected);
        
        RenderSlot(C, 0 , 0, 0, Item1 , SelectedSlot == 0 );
        RenderSlot(C, 1 , 1, 0, Item2 , SelectedSlot == 1 );
        RenderSlot(C, 2 , 2, 0, Item3 , SelectedSlot == 2 );
        RenderSlot(C, 3 , 3, 0, Item4 , SelectedSlot == 3 );
        RenderSlot(C, 4 , 4, 0, Item5 , SelectedSlot == 4 );
        RenderSlot(C, 5 , 5, 0, Item6 , SelectedSlot == 5 );
                
        RenderSlot(C, 6 , 0, 1, Item7 , SelectedSlot == 6 );
        RenderSlot(C, 7 , 1, 1, Item8 , SelectedSlot == 7 );
        RenderSlot(C, 8 , 2, 1, Item9 , SelectedSlot == 8 );
        RenderSlot(C, 9 , 3, 1, Item10, SelectedSlot == 9 );
        RenderSlot(C, 10, 4, 1, Item11, SelectedSlot == 10);
        RenderSlot(C, 11, 5, 1, Item12, SelectedSlot == 11);
    }
    
    /// <summary>
    /// Рендерит слоты инвентаря
    /// </summary>
    internal static void UI_RenderSlots_Inventory(Image.ImageContext C, int X, int Y) => UI_RenderSlots_Storage12(C, X, Y, Player_Inventory[0], Player_Inventory[1], Player_Inventory[2], Player_Inventory[3], Player_Inventory[4], Player_Inventory[5], Player_Inventory[6], Player_Inventory[7], Player_Inventory[8], Player_Inventory[9], Player_Inventory[10], Player_Inventory[11], Player_InventorySelectedSlot);
    
    /// <summary>
    /// Рендерит инвентарь
    /// </summary>
    internal static void UI_RenderInventory(Image.ImageContext C){
        C.Fill(10, 20, C.Width - 20, C.Height - 40, ColorB.LightGray);
        C.Border(10, 20, C.Width - 20, C.Height - 40, 1, ColorB.Black);

        UI_RenderSlots_Inventory(C, 20, 30);

        C.Fill(20, 110, C.Width - 42, C.Height - 140, ColorB.Gray);
        C.Border(20, 110, C.Width - 42, C.Height - 140, 1, ColorB.Black);
        
        T_Item Item = Player_ItemInHands;
        
        if(Item != T_Item.Empty){
            string Name = Info_Item_Name(Item);
                    
            Font_Default.Render(C, Palette_Default, Name, 20 + 2, 110 + 2);
            string ItemID = "[" + (byte)Item + "]";
            Font_Default.Render(C, Palette_Default, ItemID, (int)C.Width - 24 - (int)Font_Default.TextSize(ItemID).X, 110 + 2);
                    
            C.Fill(20, 110 + 11, C.Width - 42, 1, ColorB.Black);
                    
            Font_Default.Render(C, Palette_Default, Info_Item_Description(Item), 20 + 2, 110 + 2 + 11);
        }

        UI_RenderMoney(C);
    }

    /// <summary>
    /// Рендерит хранилище
    /// </summary>
    internal static void UI_RenderStorage(Image.ImageContext C){
        C.Fill(10, 20, C.Width - 20, C.Height - 40, ColorB.LightGray);
        C.Border(10, 20, C.Width - 20, C.Height - 40, 1, ColorB.Black);

        Data StorageData = UI_OpenEntity!.Value.InfoData;
        
        UI_RenderSlots_Storage12(C, 20, 30,
            (T_Item)StorageData[0 ],
            (T_Item)StorageData[1 ],
            (T_Item)StorageData[2 ],
            (T_Item)StorageData[3 ],
            (T_Item)StorageData[4 ],
            (T_Item)StorageData[5 ],
            (T_Item)StorageData[6 ],
            (T_Item)StorageData[7 ],
            (T_Item)StorageData[8 ],
            (T_Item)StorageData[9 ],
            (T_Item)StorageData[10],
            (T_Item)StorageData[11],
            UI_SelectedSlot - 12);

        C.Fill(20, 99 + 2 + 2, C.Width - 20 - 20 - 2, 1, ColorB.Black);
        
        UI_RenderSlots_Inventory(C, 20, 99 + 2 + 4 + 2);

        C.Fill(20, 179, C.Width - 42, 53, ColorB.Gray);
        C.Border(20, 179, C.Width - 42, 53, 1, ColorB.Black);
        
        T_Item Item = UI_SelectedSlot > 12 ? (T_Item)StorageData[UI_SelectedSlot - 12] : Player_ItemInHands;
        
        if(Item != T_Item.Empty){
            string Name = Info_Item_Name(Item);
                    
            Font_Default.Render(C, Palette_Default, Name, 20 + 2, 179 + 2);
            string ItemID = "[" + (byte)Item + "]";
            Font_Default.Render(C, Palette_Default, ItemID, (int)C.Width - 24 - (int)Font_Default.TextSize(ItemID).X, 179 + 2);
                    
            C.Fill(20, 179 + 11, C.Width - 42, 1, ColorB.Black);
                    
            Font_Default.Render(C, Palette_Default, Info_Item_Description(Item), 20 + 2, 179 + 2 + 11);
        }
        
        UI_RenderMoney(C);
    }

    /// <summary>
    /// Рендерит меню
    /// </summary>
    internal static void UI_RenderMenu(Image.ImageContext C){
        UI_EasyButton(C, 0, "ПРОДОЛЖИТЬ",15, 120 + (0 * 13));
        UI_EasyButton(C, 1, "НАСТРОЙКИ",15, 120 + (1 * 13), Disabled: true);
        UI_EasyButton(C, 2, "ВЫЙТИ",15, 120 + (2 * 13));
    }

    /// <summary>
    /// Рендерит консоль
    /// </summary>
    internal static void UI_RenderConsole(Image.ImageContext C, TickData TD){
        const int MaxLines = 23;

        int Total = GOLUWorld.__Messages.Count;

        if (Total > 0 && Player_ConsoleOffset > Total - 1){ Player_ConsoleOffset = Total - 1; }

        int StartIndex = Total - 1 - Player_ConsoleOffset;
        for(int i = 0; i < MaxLines; i++){
            string Text;
            ColorB Color;
            
            if(i == 0){
                Text = Player_ConsoleCommand;
                Color = ColorB.Aqua;
            }else{
                if(Total == 0){ break; }
                
                int MessageIndex = StartIndex - (i - 1);
                if(MessageIndex < 0 || MessageIndex >= Total){ break; }
                
                (Logger.MessageType Type, string Content) Message = GOLUWorld.__Messages[MessageIndex];
                Text = Message.Content;
                Color = Message.Type switch{
                    Logger.MessageType.Info => ColorB.White,
                    Logger.MessageType.Warn => ColorB.Yellow,
                    Logger.MessageType.Error => ColorB.Red,
                    Logger.MessageType.Fatal => ColorB.Magenta,
                    Logger.MessageType.Debug => ColorB.Green
                };
            }
            
            int Y = (int)C.Height - (i + 1) * 11;
            C.Fill(0, Y, C.Width, 10, ColorB.Black.SetA(128), ImageBlend.Alpha);
            Render_TextColor(C, Text.ToUpper(), 2, Y + 1, Color);

            if(i == 0 && World_AnimationNonStopTimer >= 0.5f){
                Vector2U TextSize = Font_Default.TextSize(Player_ConsoleCommand);
                C.Fill((int)TextSize.X + 3, Y + 8, 5, 1, ColorB.Aqua);
            }
        }
    }
    
    /// <summary>
    /// Рендерит GPS UI
    /// </summary>
    internal static void UI_RenderGPS(Image.ImageContext C){
        if(!(Player_ItemInHands == T_Item.GPS && UI_Interface is T_Interface.None or T_Interface.Menu && !Player_Dead)){ return; }

        Vector2I __RenderCenterPanel(uint Size, ColorB Color){
            int __X = (int)(C.Width  - Size) / 2;
            int __Y = (int)(C.Height - Size) / 2;
            C.Fill(__X, __Y, Size, Size, Color);
            return new Vector2I(__X, __Y);
        }
        const uint GPSSize = 180;
        
        Vector2I GPSOffset = __RenderCenterPanel(GPSSize, Palette_World[5]);
        
        float Energy = (float)Player_Energy / Player_Energy_Max;
        Energy /= (Energy + 0.05f * (1 - Energy));
        
        void __GPSPixel(int X__, int Y__, ColorB Color, bool Fixed = false){
            if(Color.A <= 5){ return; }
            
            uint __X = (uint)GPSOffset.X;
            uint __Y = (uint)GPSOffset.Y;

            if(!Fixed){
                X__ += (int)Coordinates_Camera.X;
                Y__ += (int)Coordinates_Camera.Y;
                
                X__ = (int)WL.Math.Floor((float)X__ / 16);
                Y__ = (int)WL.Math.Floor((float)Y__ / 16);
            }
            
            X__ += (int)GPSSize / 2;
            Y__ += (int)GPSSize / 2;
        
            __X += (uint)X__;
            __Y += (uint)Y__;   
            
            if(__X < GPSOffset.X || __Y < GPSOffset.Y || __X > GPSOffset.X + GPSSize - 1 || __Y > GPSOffset.Y + GPSSize - 1){ return; }

            C.SetPixel(__X, __Y, Color, ImageBlend.Alpha);
        }
        
        foreach(Block Block in World_Blocks.Values){
            __GPSPixel(Block.X, Block.Y, Palette_World[MapBlocksColor.GetValueOrDefault(Block.ID, (byte)1)]);
        }

        foreach(Entity Entity in World_Entities.Values){
            byte __PaletteIndex = MapEntitiesColor.GetValueOrDefault(Entity.ID, (byte)0);
            if(__PaletteIndex == 0){ continue; }
            ColorB EntityColor = Palette_World[__PaletteIndex];
            __GPSPixel(Entity.X, Entity.Y, EntityColor);

            if(Entity.ID == T_Entity.Tree && Entity.Info != 2){
                __GPSPixel(Entity.X, Entity.Y + 16, EntityColor);
                __GPSPixel(Entity.X, Entity.Y + 16 * 2, EntityColor);
            }
        }
        
        foreach(Ceiling Ceiling in World_Ceilings.Values){
            byte __PaletteIndex = MapCeilingsColor.GetValueOrDefault(Ceiling.ID, (byte)0);
            if(__PaletteIndex == 0){ continue; }
            ColorB CeilingColor = Palette_World[__PaletteIndex];
            if(Ceiling.ID is T_Ceiling.RoofTiles && Ceiling.Info is 1 or 3){ CeilingColor = Palette_World[3]; }
            __GPSPixel(Ceiling.X, Ceiling.Y, CeilingColor);
        }
        
        for(int __Y__ = -(int)GPSSize/2; __Y__ < GPSSize/2; __Y__++){
            for(int __X__ = -(int)GPSSize/2; __X__ < GPSSize/2; __X__++){
                int PX = Coordinates_Player.X - Coordinates_World.X + __X__ * 16;
                int PY = Coordinates_Player.Y - Coordinates_World.Y + __Y__ * 16;

                if((PX <= -World_SizeWorld.X || PX >= World_SizeWorld.X || PY <= -World_SizeWorld.Y || PY >= World_SizeWorld.Y) && !Cheat_DisableWorldLimit){
                    int DistanceX = 0;
                    int DistanceY = 0;

                    if(PX < -World_SizeWorld.X){ DistanceX = -(int)World_SizeWorld.X - PX; }else if(PX > World_SizeWorld.X){ DistanceX = PX - (int)World_SizeWorld.X; }
                    if(PY < -World_SizeWorld.Y){ DistanceY = -(int)World_SizeWorld.Y - PY; }else if(PY > World_SizeWorld.Y){ DistanceY = PY - (int)World_SizeWorld.Y; }

                    int Distance = WL.Math.MaxI(DistanceX, DistanceY);

                    const int FadeDistance = 128;

                    float FadeFactor = WL.Math.Clamp01((float)Distance / FadeDistance);

                    __GPSPixel(__X__, __Y__, new ColorB((byte)(WL.Math.Random.Fast_Int(128, 255)), 0, 0, (byte)(255 * FadeFactor)), true);
                }
            }   
        }

        void __GPSPixelCross(int __X, int __Y, ColorB Color){
            void __GPSPixelPlayer(int X__, int Y__, int __X, int __Y) => __GPSPixel(X__ + __X * 16, Y__ + __Y * 16, Color);
            __GPSPixelPlayer(__X, __Y, 0,  1);
            __GPSPixelPlayer(__X, __Y, 0,  2);
            __GPSPixelPlayer(__X, __Y, 0, -1);
            __GPSPixelPlayer(__X, __Y, 0, -2);
            __GPSPixelPlayer(__X, __Y, 1,  0);
            __GPSPixelPlayer(__X, __Y, 2,  0);
            __GPSPixelPlayer(__X, __Y,-1,  0);
            __GPSPixelPlayer(__X, __Y,-2,  0);
        }
        
        __GPSPixelCross(Coordinates_Spawn.X, Coordinates_Spawn.Y, ColorB.Green);
        __GPSPixelCross(0, 0, ColorB.Yellow);
        __GPSPixelCross(Coordinates_PlayerWorld.X, Coordinates_PlayerWorld.Y, ColorB.Blue);
        
        string Coordinates = Coordinates_Beautiful.X + " : " + Coordinates_Beautiful.Y;
        Vector2U __CoordinatesSize = Font_Default.TextSize(Coordinates);
        Render_TextColorOutline(C, Coordinates, (int)(GPSOffset.X + GPSSize) - (int)__CoordinatesSize.X - 2, (int)(GPSOffset.Y + GPSSize) - 8 - 2, ColorB.Red, ColorB.Black);

        Render_TextColorOutline(C, World_Type.ToString().ToUpper(), GPSOffset.X + 2, (int)(GPSOffset.Y + GPSSize) - 8 - 2, ColorB.Red, ColorB.Black);
        
        for(int __Y__ = -(int)GPSSize/2; __Y__ < GPSSize/2; __Y__++){
            for(int __X__ = -(int)GPSSize/2; __X__ < GPSSize/2; __X__++){
                byte Color = (byte)(Player_Energy > 0 ? WL.Math.Random.Fast_Byte() : 0);
                __GPSPixel(__X__, __Y__, new ColorB(Color, Color, Color, (byte)(255 * (1 - Energy))), true);
            }
        }

        Texture_GPS_Overlay.Render(C, Palette_Default);
        if(World_AnimationTimer > 0.5f && Player_Energy > 0){
            Texture_GPS_Overlay_Button.Render(C, Palette_Default, 122, 221);
        }
    }

    /// <summary>
    /// Рендерит интерфейс часов
    /// </summary>
    internal static void UI_RenderClock(Image.ImageContext C){
        if(!(Player_ItemInHands == T_Item.Clock && UI_Interface is T_Interface.None or T_Interface.Menu && !Player_Dead)){ return; }
        
        Texture_Clock_Overlay.Render(C, Palette_Default);

        float Time = World_Time % 24;
        if(Time < 0){ Time += 24; }

        int Hours = (int)Time;
        int Minutes = (int)((Time - Hours) * 60);

        if(Minutes >= 60){
            Minutes = 0;
            Hours = (Hours + 1) % 24;
        }

        void __RenderDigital(int X, int Y, int N){
            Font_Digital.Render(C, Palette_Default, Player_Energy > 0 ? N.ToString() : "!", X, Y);
        }
        __RenderDigital(69 , 92, Hours   / 10);
        __RenderDigital(97 , 92, Hours   % 10);
        __RenderDigital(132, 92, Minutes / 10);
        __RenderDigital(160, 92, Minutes % 10);
        
        if(World_AnimationTimer > 0.5f && Player_Energy > 0){ Texture_Clock_Overlay_Colon.Render(C, Palette_Default, 123, 97); }

        if(Player_Energy > 0){
            Render_TextColor(C, "DAY: " + World_Day, 69, 141, ColorB.Red);
        }
    }
    
    /// <summary>
    /// Название окна
    /// </summary>
    internal static string UI_WindowTitle => Game_Version;

    /// <summary>
    /// Финальный UI рендер
    /// </summary>
    internal static void UI_RenderFinal(Image.ImageContext C, TickData TD){
        Render_PostPostProcessing(C, TD);
        
        C.Border(0, 0, C.Width, C.Height, 1, ColorB.Black);
    }
}
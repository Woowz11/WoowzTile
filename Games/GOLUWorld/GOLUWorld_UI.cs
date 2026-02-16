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
                case 2:{
                    UI_Interface = (T_Interface)1;
                    break;
                }
                case 3:{
                    Game.Quit();
                    break;
                }
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
    }
    
    /// <summary>
    /// Рендерит задний фон главного меню
    /// </summary>
    internal static void UI_RenderMainMenu_Background(Image.ImageContext C, TickData TD){
        Texture_GOLU.RenderTiles(C, Palette_Default, -(int)(WL.Math.DCos((float)TD.DeltaTick / 2) * 128), -(int)(WL.Math.DSin((float)TD.DeltaTick / 2) * 128), 3, 3, MultiplyColor: ColorB.White.SetA(64));
        
        C.Border(0, 0, C.Width, C.Height, 1, ColorB.Black);
    }

    /// <summary>
    /// Рендерит меню главного меню
    /// </summary>
    internal static void UI_RenderMainMenu_Menu(Image.ImageContext C, TickData TD){
        Texture_Author.Render(C, Palette_Default, (int)(C.Width - Texture_Author.Width) - 3, (int)(C.Height - Texture_Author.Height) - 3);
            Font_Default.Render(C, Palette_Default, Game_Version, 3, (int)(C.Height - 8 - 3));

            void RenderGOLU(float SinOffset, ColorB MultiplyColor){
                Texture_G.Render(C, Palette_Default, (int)(C.Width / 2f - Texture_G.Width / 2f - Texture_G.Width * 1.5f), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2     + SinOffset) * 10), MultiplyColor: MultiplyColor);
                Texture_O.Render(C, Palette_Default, (int)(C.Width / 2f - Texture_G.Width / 2f - Texture_G.Width /   2f), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2 + 1 + SinOffset) * 10), MultiplyColor: MultiplyColor);
                Texture_L.Render(C, Palette_Default, (int)(C.Width / 2f - Texture_G.Width / 2f + Texture_G.Width /   2f), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2 + 2 + SinOffset) * 10), MultiplyColor: MultiplyColor);
                Texture_U.Render(C, Palette_Default, (int)(C.Width / 2f - Texture_G.Width / 2f + Texture_G.Width * 1.5f), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2 + 3 + SinOffset) * 10), MultiplyColor: MultiplyColor);
            }
            RenderGOLU(-0.2f, ColorB.Black.SetA((byte)(255 * 0.2f)));
            RenderGOLU(-0.4f, ColorB.Black.SetA((byte)(255 * 0.2f)));
            RenderGOLU(-0.6f, ColorB.Black.SetA((byte)(255 * 0.2f)));
            RenderGOLU(-0.8f, ColorB.Black.SetA((byte)(255 * 0.2f)));
            RenderGOLU(0, ColorB.White);

            C.Fill((int)(C.Width / 2f - Texture_G.Width / 2f - Texture_G.Width * 1.5F), 75, 127, 2, ColorB.Black);
            
            Texture_Title.Render(C, Palette_Default, (int)(C.Width/2 - Texture_Title.Width/2), 80);
            
            UI_EasyButton(C, 0, "ЗАГРУЗИТЬ", (int)(C.Width / 2 - Font_Default.TextSize("ЗАГРУЗИТЬ").X / 2), 150 + (0 * 13));
            UI_EasyButton(C, 1, "НОВАЯ", (int)(C.Width / 2 - Font_Default.TextSize("НОВАЯ").X / 2), 150 + (1 * 13));
            UI_EasyButton(C, 2, "ПОМОЩЬ", (int)(C.Width / 2 - Font_Default.TextSize("ПОМОЩЬ").X / 2), 150 + (2 * 13));
            UI_EasyButton(C, 3, "ВЫЙТИ", (int)(C.Width / 2 - Font_Default.TextSize("ВЫЙТИ").X / 2), 150 + (3 * 13));
    }

    /// <summary>
    /// Рендерит информацию об управлении в гланом меню
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
            "[HOME] - ТЕЛЕПОРТ В ЦЕНТР",
            "[F1,F2,F3] - ТЕСТОВОЕ",
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
    internal static void UI_Render(Image.ImageContext C){
        float HealthPulse = Player_Dead ? 0 : WL.Math.DSin(World_DeltaTick / WL.Math.Sqr((float)Player_Health / Player_HealthMax));
        ColorB FrameColor = new ColorB((byte)(HealthPulse * 255), 0, 0);

        uint Thickness = (uint)WL.Math.Min(1 + HealthPulse / WL.Math.Sqr((float)Player_Health / Player_HealthMax), 16);
        
        C.Border(0, 0, C.Width, C.Height, 1, FrameColor);
        C.Border(1, 1, C.Width - 2, C.Height - 2, Thickness, FrameColor.Clone().SetA(128), ImageBlend.Alpha);
        C.Border(1 + (int)Thickness, 1 + (int)Thickness, C.Width - (1 + Thickness) * 2, C.Height - (1 + Thickness) * 2, Thickness, FrameColor.Clone().SetA(64), ImageBlend.Alpha);
        
        C.Fill(20 - 1, (int)C.Height - 16 - 1, Player_HealthMax + 2, 8 + 2, ColorB.DarkRed);
        C.Fill(20, (int)C.Height - 16, Player_HealthMax, 8, ColorB.Black);
        C.Fill(20, (int)C.Height - 16, Player_Health, 8, ColorB.Red);
        C.Fill(20, (int)C.Height - 16 + 3, Player_Health, 8 - 6, ColorB.LightRed);

        Font_Default.Render(C, Palette_Default, Cheat_Immortality ? "i" : Player_Health.ToString(), 20, (int)C.Height - 16);
        
        Texture_Health.Render(C, Palette_Default, 3, (int)C.Height - 21);
        
        T_Item Item = Player_ItemInHands;
        string __Text = (Item == T_Item.Empty ? "" : Info_Item_Name(Item)) + " [" + (Player_InventorySelectedSlot + 1) + "]";
        RenderOutlineColorText(C, __Text, (int)C.Width - (int)Font_Default.TextSize(__Text).X - 4, (int)C.Height - 8 - 4, ColorB.Black, ColorB.White);
        
        void RenderSlot(Image.ImageContext C, byte ID, int X, int Y){
            int X__ = 20 + X * 36;
            int Y__ = 30 + Y * 36;
            C.Fill(X__, Y__, 34, 34, Player_InventorySelectedSlot == ID ? ColorB.Lerp(ColorB.Gray, ColorB.Red, 0.5f) : ColorB.Gray);
            C.Fill(X__ + 4, Y__ + 4, 34 - 4 * 2, 34 - 4 * 2, ColorB.Black.SetA(64), ImageBlend.Alpha);
            C.Fill(X__ + 8, Y__ + 8, 34 - 8 * 2, 34 - 8 * 2, ColorB.Black.SetA(64), ImageBlend.Alpha);
            C.Fill(X__ + 12, Y__ + 12, 34 - 12 * 2, 34 - 12 * 2, ColorB.Black.SetA(64), ImageBlend.Alpha);
            C.Border(X__, Y__, 34, 34, 1, Player_InventorySelectedSlot == ID ? ColorB.Red : ColorB.Black);

            if(Player_InventorySelectedSlot == ID){
                C.Border(X__ - 1, Y__ - 1, 34 + 2, 34 + 2, 1, ColorB.Red.SetA(128), ImageBlend.Alpha);
            }

            T_Item Item = Player_Inventory[ID];
    
            if(Item != 0){
                Info_Item_Icon(Item).Render(C, Palette_Default, X__ + 1, Y__ + 1);
            }
        }
        
        if(UI_Interface != T_Interface.None){ C.Fill(ColorB.Black.SetA(128), ImageBlend.Alpha); }
        
        switch(UI_Interface){
            case T_Interface.Inventory: {
                C.Fill(10, 20, C.Width - 20, C.Height - 40);
                C.Border(10, 20, C.Width - 20, C.Height - 40, 1, ColorB.Black);
                
                RenderSlot(C, 0, 0, 0);
                RenderSlot(C, 1, 1, 0);
                RenderSlot(C, 2, 2, 0);
                RenderSlot(C, 3, 3, 0);
                RenderSlot(C, 4, 4, 0);
                RenderSlot(C, 5, 5, 0);
                
                RenderSlot(C, 6, 0, 1);
                RenderSlot(C, 7, 1, 1);
                RenderSlot(C, 8, 2, 1);
                RenderSlot(C, 9, 3, 1);
                RenderSlot(C, 10, 4, 1);
                RenderSlot(C, 11, 5, 1);

                C.Fill(20, 110, C.Width - 40, C.Height - 140, ColorB.Gray);
                C.Border(20, 110, C.Width - 40, C.Height - 140, 1, ColorB.Black);
                
                if(Item != T_Item.Empty){
                    string Name = Info_Item_Name(Item);
                    
                    Font_Default.Render(C, Palette_Default, "[" + (byte)Item + "] " + Name, 20 + 2, 110 + 2);
                    
                    C.Fill(20, 110 + 11, C.Width - 40, 1, ColorB.Black);
                    
                    Font_Default.Render(C, Palette_Default, Info_Item_Description(Item), 20 + 2, 110 + 2 + 11);
                }
                break;
            }
            case T_Interface.Menu:{
                UI_EasyButton(C, 0, "ПРОДОЛЖИТЬ",15, 120 + (0 * 13));
                UI_EasyButton(C, 1, "ВЫЙТИ",15, 120 + (1 * 13));
                
                break;
            }
        }
    }
    
    /// <summary>
    /// Рендерит GPS UI
    /// </summary>
    internal static void UI_RenderGPS(Image.ImageContext C){
        T_Item Item = Player_ItemInHands;
        if(Item == T_Item.GPS && UI_Interface is T_Interface.None or T_Interface.Menu && !Player_Dead){
            Vector2I __RenderCenterPanel(uint Size, ColorB Color){
                int __X = (int)(C.Width  - Size) / 2;
                int __Y = (int)(C.Height - Size) / 2;
                C.Fill(__X, __Y, Size, Size, Color);
                return new Vector2I(__X, __Y);
            }
            const uint GPSSize = 180;
            
            Vector2I GPSOffset = __RenderCenterPanel(GPSSize, World_BackgroundColor);

            void __GPSPixel(int X__, int Y__, ColorB Color, bool Fixed = false){
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
                ColorB BlockColor = Palette_World[MapBlocksColor.GetValueOrDefault(Block.ID, (byte)1)];
                __GPSPixel(Block.X, Block.Y, BlockColor);
            }

            foreach(Entity Entity in World_Entities.Values){
                byte __PaletteIndex = MapEntitiesColor.GetValueOrDefault(Entity.ID, (byte)0);
                if(__PaletteIndex == 0){ continue; }
                ColorB EntityColor = Palette_World[__PaletteIndex];
                __GPSPixel(Entity.X, Entity.Y, EntityColor);

                if(Entity.ID == T_Entity.Tree){
                    __GPSPixel(Entity.X, Entity.Y + 16, EntityColor);
                    __GPSPixel(Entity.X, Entity.Y + 16 * 2, EntityColor);
                }
            }
            
            for(int __Y__ = -(int)GPSSize/2; __Y__ < GPSSize/2; __Y__++){
                for(int __X__ = -(int)GPSSize/2; __X__ < GPSSize/2; __X__++){
                    int PX = Coordinates_Player.X - Coordinates_World.X + __X__ * 16;
                    int PY = Coordinates_Player.Y - Coordinates_World.Y + __Y__ * 16;

                    if((PX <= -World_BlocksSize.X || PX >= World_BlocksSize.X || PY <= -World_BlocksSize.Y || PY >= World_BlocksSize.Y) && !Cheat_DisableWorldLimit){
                        int DistanceX = 0;
                        int DistanceY = 0;

                        if(PX < -World_BlocksSize.X){ DistanceX = -(int)World_BlocksSize.X - PX; }else if(PX > World_BlocksSize.X){ DistanceX = PX - (int)World_BlocksSize.X; }
                        if(PY < -World_BlocksSize.Y){ DistanceY = -(int)World_BlocksSize.Y - PY; }else if(PY > World_BlocksSize.Y){ DistanceY = PY - (int)World_BlocksSize.Y; }

                        int Distance = WL.Math.MaxI(DistanceX, DistanceY);

                        const int FadeDistance = 128;

                        float FadeFactor = WL.Math.Clamp01((float)Distance / FadeDistance);

                        __GPSPixel(__X__, __Y__, new ColorB((byte)(WL.Math.Random.Fast_Int(128, 255)), 0, 0, (byte)(255 * FadeFactor)), true);
                    }
                }   
            }

            void __GPSPixelPlayer(int __X, int __Y) => __GPSPixel(Coordinates_Player.X - Coordinates_World.X + __X * 16, Coordinates_Player.Y - Coordinates_World.Y + __Y * 16, ColorB.Blue);
            __GPSPixelPlayer( 0,  1);
            __GPSPixelPlayer( 0,  2);
            __GPSPixelPlayer( 0, -1);
            __GPSPixelPlayer( 0, -2);
            __GPSPixelPlayer( 1,  0);
            __GPSPixelPlayer( 2,  0);
            __GPSPixelPlayer(-1,  0);
            __GPSPixelPlayer(-2,  0);
            
            string Coordinates = Coordinates_Beautiful.X + " : " + Coordinates_Beautiful.Y;
            Vector2U __CoordinatesSize = Font_Default.TextSize(Coordinates);
            Font_Default.Render(C, Palette_Default, Coordinates, (int)(GPSOffset.X + GPSSize) - (int)__CoordinatesSize.X - 2, (int)(GPSOffset.Y + GPSSize) - (int)__CoordinatesSize.Y - 2);
            
            Texture_GPS_Overlay.Render(C, Palette_Default);
        }
    }
    
    /// <summary>
    /// Название окна
    /// </summary>
    internal static string UI_WindowTitle => Emotion_Happiness + " | " + Player_InteractingCollision + " (" + Player_CollisionInfo1 + ", " + Player_CollisionInfo2 + ", " + Player_CollisionInfo3 + ") | " + World_Seed + " | " + Cheat_IgnoreColliders + " | " + World_Time + " (" + World_DayPhase + ") | " + World_Flow + " | " + Player_AttackTimer;
}
using WL;
using WLO;
using WoowzTile;
using WoowzTile.Objects;
using static GOLUWorld.GW_Values;
using static GOLUWorld.GW_Resources;
using static GOLUWorld.GW_Objects;
using static GOLUWorld.GW_Info;
using static GOLUWorld.GW_World;

namespace GOLUWorld;

internal static class GW_Render{
    internal static readonly List<Renderable> RenderQueue = [];
    
    internal static void Game_Render(TickData TD, Image.ImageContext C){
        float DTS = StopTime ? 0 : (float)TD.DeltaTimeS;
        if(!Dead){ LastHealed -= DTS; }
        WorldDeltaTick += DTS;
        AnimationTimer += DTS;
        if(AnimationTimer > 1){ AnimationTimer = 0; }
        
        void Button(byte ButtonID, string ButtonText, int X, int Y, bool Always = false){
            if(MenuSelectedButton == ButtonID || Always){
                RenderOutlineColorText(C, ButtonText, X, Y, ColorB.White, ColorB.Red);
            }else{
                RenderColorText(C, ButtonText, X, Y, ColorB.Black);   
            }
        }
        
        if(InMainMenu){
            Texture_GOLU.RenderTiles(C, Palette_Default, -(int)(WL.Math.DCos((float)TD.DeltaTick / 2) * 128), -(int)(WL.Math.DSin((float)TD.DeltaTick / 2) * 128), 3, 3, MultiplyColor: ColorB.White.SetA(64));
            
            C.Border(0, 0, C.Width, C.Height, 1, ColorB.Black);
            
            if(Interface == T_Interface.None){
                Texture_Author.Render(C, Palette_Default, (int)(C.Width - Texture_Author.Width) - 3, (int)(C.Height - Texture_Author.Height) - 3);
                Font_Default.Render(C, Palette_Default, Game_Version, 3, (int)(C.Height - 8 - 3));

                void RenderGOLU(float SinOffset, ColorB MultiplyColor){
                    Texture_G.Render(C, Palette_Default, (int)(C.Width/2 - Texture_G.Width/2 - Texture_G.Width*1.5F), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2 + SinOffset) * 10), MultiplyColor: MultiplyColor);
                    Texture_O.Render(C, Palette_Default, (int)(C.Width/2 - Texture_G.Width/2 - Texture_G.Width/2), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2 + 1 + SinOffset) * 10), MultiplyColor: MultiplyColor);
                    Texture_L.Render(C, Palette_Default, (int)(C.Width/2 - Texture_G.Width/2 + Texture_G.Width/2), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2 + 2 + SinOffset) * 10), MultiplyColor: MultiplyColor);
                    Texture_U.Render(C, Palette_Default, (int)(C.Width/2 - Texture_G.Width/2 + Texture_G.Width*1.5F), 30 + (byte)(WL.Math.DSin((float)TD.DeltaTick * 2 + 3 + SinOffset) * 10), MultiplyColor: MultiplyColor);
                }
                RenderGOLU(-0.2f, ColorB.Black.SetA((byte)(255 * 0.2f)));
                RenderGOLU(-0.4f, ColorB.Black.SetA((byte)(255 * 0.2f)));
                RenderGOLU(-0.6f, ColorB.Black.SetA((byte)(255 * 0.2f)));
                RenderGOLU(-0.8f, ColorB.Black.SetA((byte)(255 * 0.2f)));
                RenderGOLU(0, ColorB.White);

                C.Fill((int)(C.Width / 2 - Texture_G.Width / 2 - Texture_G.Width * 1.5F), 75, 127, 2, ColorB.Black);
                
                Texture_Title.Render(C, Palette_Default, (int)(C.Width/2 - Texture_Title.Width/2), 80);
                
                Button(0, "ЗАГРУЗИТЬ", (int)(C.Width / 2 - Font_Default.TextSize("ЗАГРУЗИТЬ").X / 2), 150 + (0 * 13));
                Button(1, "НОВАЯ", (int)(C.Width / 2 - Font_Default.TextSize("НОВАЯ").X / 2), 150 + (1 * 13));
                Button(2, "ПОМОЩЬ", (int)(C.Width / 2 - Font_Default.TextSize("ПОМОЩЬ").X / 2), 150 + (2 * 13));
                Button(3, "ВЫЙТИ", (int)(C.Width / 2 - Font_Default.TextSize("ВЫЙТИ").X / 2), 150 + (3 * 13));
            }else if(Interface == (T_Interface)1){
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
                    "[HOME] - ТЕЛЕПОРТ В ЦЕНТР",
                    "[F1,F2] - ТЕСТОВОЕ",
                    "",
                    "[ДВИЖЕНИЕ + SHIFT] - ДВИГАТЬ ПРЕДМЕТЫ\nВ ИНВЕНТОРЕ"
                ];

                for(int i = 0; i < HelpInfo.Length; i++){
                    string HelpInfoMessage = HelpInfo[i];
                    
                    Font_Default.Render(C, Palette_Default, HelpInfoMessage, 30, 15 + i * 12);
                }
                
                Button(0, "ОБРАТНО", (int)(C.Width / 2 - Font_Default.TextSize("ОБРАТНО").X / 2), 150 + (5 * 13), true);
            }
            
            return;
        }

        RenderQueue.Clear();
        
        const int RenderLayer_Bottom = -10000000;
        int RenderLayer_Object(int Y) => -100000 + (Y * 100);
        int RenderLayer_Top(int Y) => 1000 + (Y * 100);

        void AddToRender(Renderable R){
            int OFFSET = 5 * 16;
            int LEFT_BORDER = -OFFSET;
            int RIGHT_BORDER = (int)C.Width + OFFSET;
            int TOP_BORDER = -OFFSET;
            int BOTTOM_BORDER = (int)C.Height + OFFSET;

            int X = R.X;
            int Y = R.Y;
            
            if(!(X < LEFT_BORDER || X > RIGHT_BORDER || Y < TOP_BORDER || Y > BOTTOM_BORDER)){
                RenderQueue.Add(R);
            }
        }
        
        int __OffsetX = WorldX % 16;
        int __OffsetY = WorldY % 16;
        if(__OffsetX < 0){ __OffsetX += 16; }
        if(__OffsetY < 0){ __OffsetY += 16; }
        for(int Y__ = -1; Y__ < 16; Y__++){
            for(int X__ = -1; X__ < 16; X__++){
                int __X = X__ * 16 + __OffsetX;
                int __Y = Y__ * 16 + __OffsetY;
                AddToRender(new Renderable{ Texture = Texture_Ground, X = __X, Y = __Y, Z = RenderLayer_Bottom });
            }
        }
        
        foreach(Block Block in __Blocks.Values){
            if(Block.ID is T_Block.Ground_Planks or T_Block.Ground_Asphalt or T_Block.Ground_Sand or T_Block.Water or T_Block.Ground_Grass){
                Texture BlockTexture = Block.ID switch{
                    T_Block.Ground_Planks  => Texture_Planks,
                    T_Block.Ground_Asphalt => Texture_Asphalt,
                    T_Block.Ground_Sand    => Texture_Sand,
                    T_Block.Water          => (__Blocks.TryGetValue(new Vector2I(Block.X, Block.Y - 16), out Block __Found) && __Found.ID == Block.ID ? Texture_Water : Texture_Water_Top),
                    T_Block.Ground_Grass   => Texture_Grass,
                };
                
                AddToRender(new Renderable{Texture = BlockTexture, X = WorldX + Block.X, Y = WorldY + Block.Y, Z = RenderLayer_Bottom + 1});
            }
            
            if(Block.ID is T_Block.Metal or T_Block.Bricks or T_Block.Black or T_Block.Error){
                Texture BlockTexture = Block.ID switch{
                    T_Block.Metal  => Texture_Metal,
                    T_Block.Bricks => Texture_Bricks,
                    T_Block.Black  => Texture_Black,
                    T_Block.Error  => Texture_Debug
                };
                
                AddToRender(new Renderable{ Texture = BlockTexture, X = WorldX + Block.X, Y = WorldY + Block.Y, Z = RenderLayer_Object(Block.Y)});
            }
        }

        foreach((int, int, T_Decal, TextureRotation) Track in __Decals){
            Texture DecalTexture = Track.Item3 switch{
                T_Decal.Track => Texture_Track,
                T_Decal.Blood => Texture_Blood,
                T_Decal.Zero  => Texture_Zero,
                T_Decal.One   => Texture_One
            };
            
            AddToRender(new Renderable{ Texture = DecalTexture, Palette = Palette_Default, X = WorldX + Track.Item1, Y = WorldY + Track.Item2, Rotation = Track.Item4, Z = RenderLayer_Bottom + 2});
        }
        
        foreach(Entity Entity in __Entities.Values){
            if(Entity.ID is T_Entity.Chair or T_Entity.Table or T_Entity.Spikes or T_Entity.Tree or T_Entity.Item or T_Entity.Crate or T_Entity.Grass or T_Entity.Bush or T_Entity.Error){
                Texture EntityTexture = Entity.ID switch{
                    T_Entity.Chair  => Texture_Chair,
                    T_Entity.Table  => Texture_Table,
                    T_Entity.Spikes => Texture_Spikes,
                    T_Entity.Tree   => Texture_Tree,
                    T_Entity.Item   => ItemTexture((T_Item)Entity.Info),
                    T_Entity.Crate  => Texture_Crate,
                    T_Entity.Grass  => Texture_TallGrass,
                    T_Entity.Bush   => Texture_Bush,
                    T_Entity.Error  => Texture_Debug
                };

                bool __Top = false;
                if(Entity.ID is T_Entity.Crate){
                    __Top = true;
                }
                
                int OffsetX = 0;
                int OffsetY = 0;
                if(Entity.ID == T_Entity.Tree){
                    OffsetY = -48;
                }

                if(Entity.ID is T_Entity.Grass or T_Entity.Bush){
                    OffsetX = (int)(WL.Math.Sin(WorldDeltaTick * 2 + (Entity.X * 2 + Entity.Y)) * 2);
                }

                bool BottomRenderLayer = Entity.ID is T_Entity.Spikes;

                AddToRender(new Renderable{ Texture = EntityTexture, Palette = Palette_Default, X = WorldX + Entity.X + OffsetX, Y = WorldY + Entity.Y + OffsetY, Rotation = Entity.Rotation, Z = (BottomRenderLayer ? RenderLayer_Bottom + 3 : RenderLayer_Object(Entity.Y) + (__Top ? 1 : 0))});

                if(Entity.ID == T_Entity.Tree){
                    void __RenderLeaves(int X__, int Y__){
                        int __X__ = -16;
                        int __Y__ = -64;

                        __X__ += (X__ - 1) * (X__ == 0 ? 16 : 8);
                        __Y__ += (Y__ - 1) * (Y__ == 0 ? 8 : 16);

                        __X__ += (int)(WL.Math.Sin(WorldDeltaTick * 2 + (Entity.X + __X__) * 432) * 2);
                        __Y__ += (int)(WL.Math.Sin(WorldDeltaTick * 2 + (Entity.Y + __Y__) * 12) * 2);
                        AddToRender(new Renderable{ Texture = Texture_Tree_Leaves, Palette = Palette_Default, X = WorldX + Entity.X + __X__, Y = WorldY + Entity.Y + __Y__, Rotation = Entity.Rotation, Z = RenderLayer_Object(Entity.Y) + (X__ + Y__)});
                    }
                    __RenderLeaves(0, 0);
                    __RenderLeaves(2, 0);
                    __RenderLeaves(1, 2);
                }
            }
            
            if(Entity.ID is T_Entity.Mob_Spider){
                Texture EntityTexture = Entity.ID switch{
                    T_Entity.Mob_Spider => (AnimationTimer > 0.5f ? Texture_Spider_Anim : Texture_Spider)
                };

                int OffsetX = 0;
                int OffsetY = 0;

                if(Entity.ID == T_Entity.Mob_Spider){
                    OffsetX = 8;
                    OffsetY = 8;
                }
                
                AddToRender(new Renderable{ Texture = EntityTexture, Palette = Palette_Default, X = WorldX + Entity.X - OffsetX, Y = WorldY + Entity.Y - OffsetY, Rotation = Entity.Rotation, Z = RenderLayer_Top(Entity.Y)});
            }
        }

        T_Item Item = Inventory[SelectedItem];
        
        void RenderPlayer(){
            Texture PlayerBody  = Texture_Player_Body;
            Texture PlayerEyes  = Texture_Player_Eyes;
            Texture PlayerNose  = Texture_Player_Nose;
            Texture PlayerMouth = (Dead ? Texture_Player_Mouth : Emotion_Happiness < 25 ? Texture_Player_Mouth_Sad : (Emotion_Happiness > 75 ? Texture_Player_Mouth_Happy : Texture_Player_Mouth));
            BlinkTimer += DTS;

            if(BlinkTimer > 3 || Dead){
                PlayerEyes = Texture_Player_Eyes_Blink;
                if(BlinkTimer > 3.25f){
                    BlinkTimer = 0;
                }
            }
        
            if(MovingDirection.X != 0){
                PlayerFlipped = MovingDirection.X > 0;
            }
            
            int __PlayerZ = 0;
            void __RenderPlayerPart(Texture T, ColorB? Color, int OffsetY = 0){
                AddToRender(new Renderable{ Texture = T, Palette = Palette_Default, X = PlayerX, Y = PlayerY + OffsetY, FlipX = PlayerFlipped, MultiplyColor = Color, Z = RenderLayer_Object(PlayerY - WorldY + 1) + __PlayerZ});
                __PlayerZ++;
            }
            
            if(Item != T_Item.Empty){
                __RenderPlayerPart(ItemTexture(Item), null, -11);
            }
        
            ColorB PlayerColor = ColorB.Lerp(ColorB.White, ColorB.DarkRed, WL.Math.Clamp01((Rotten - 2) / 50));
            
            __RenderPlayerPart(PlayerBody, PlayerColor);
            __RenderPlayerPart(PlayerNose, PlayerColor);
            __RenderPlayerPart(PlayerMouth, PlayerColor);
            __RenderPlayerPart(PlayerEyes, PlayerColor);

            if(Health < HealthSmall * 2){
                Texture PlayerBlood = Health < HealthSmall ? Texture_Player_Blood_Strong : Texture_Player_Blood;
                __RenderPlayerPart(PlayerBlood, ColorB.Lerp(ColorB.White, ColorB.DarkGreen, WL.Math.Clamp01((Rotten - 2) / 50)));
            }
        
            if(LastHealed > 0){
                __RenderPlayerPart(Texture_Player_Healed, null);
            }
        }
        RenderPlayer();

        RenderQueue.Sort((A, B) => A.Z.CompareTo(B.Z));
        foreach(Renderable R in RenderQueue){
            switch(R.Type){
                case RenderableType.Tile: R.Texture.Render(C, R.Palette, R.X, R.Y, FlipX: R.FlipX, FlipY: R.FlipY, MultiplyColor: R.MultiplyColor, Rotation: R.Rotation); break;
                case RenderableType.Tiles: R.Texture.RenderTiles(C, R.Palette, R.X, R.Y, R.W, R.H); break;
            }
        }
        
        byte PerlinNoise(int X, int Y, float Scale = 1) => Texture_PerlinNoise.GetPixelRepeat(Palette_Default, X, Y, Scale).R;

        byte Light(float TX, float TY) => Texture_Light.GetPixelRepeat(Palette_Alpha, (int)(TX * Texture_Light.Width), (int)(TY * Texture_Light.Height)).A;
        
        void PostProcessing(){
            Vector2F DayCycle_Moring = new Vector2F(0, 50);
            Vector2F DayCycle_Noon  = new Vector2F((float)C.Width/2, 25);
            Vector2F DayCycle_Eving = new Vector2F(C.Width, 50);

            Vector2F SunPosition;
            float DayAlpha;
            switch(Time){
                case >= 6 and < 12:
                    DayAlpha = (Time - 6) / 6f;
                    SunPosition = Vector2F.Lerp(DayCycle_Moring, DayCycle_Noon, DayAlpha);
                    break;
                case >= 12 and < 18:
                    DayAlpha = (Time - 12) / 6f;
                    SunPosition = Vector2F.Lerp(DayCycle_Noon, DayCycle_Eving, DayAlpha);
                    DayAlpha = 1 - DayAlpha;
                    break;
                default:
                    DayAlpha = 0;
                    SunPosition = Vector2F.Zero;
                    break;
            }
            
            float NightAlpha;
            if(DayAlpha > 0){
                NightAlpha = 0;
            }else{
                if(Time >= 18f){
                    NightAlpha = 1 - (24f - Time) / 6f;
                }else{
                    NightAlpha = 1f - Time / 6f;
                }
            }
            
            const float SunRadius = 300;

            List<(int, int, Texture, Palette, ColorB)> WaterTextures = [
                ((int)SunPosition.X, (int)SunPosition.Y, Texture_Circle_16px, Palette_Alpha, new ColorB(255, 255, 255, (byte)(DayAlpha * 255))),
                (PlayerX + (int)(Texture_Player_Reflection.Width/2), PlayerY + (int)(Texture_Player_Reflection.Height*1.5f) - 2, Texture_Player_Reflection, Palette_Default, ColorB.White.SetA(64))
            ];
            
            for(uint FY = 0; FY < C.Height; FY++){
                for(uint FX = 0; FX < C.Width; FX++){
                    ColorB Color = C[FX, FY];
                    ColorB? Result = null;

                    int PX = PlayerX - WorldX - ((int)C.Width /2 - (int)FX);
                    int PY = PlayerY - WorldY - ((int)C.Height/2 - (int)FY);
                    
                    if(Color == __WaterShaderColor || Color == __WaterShaderColor_Dark){
                        byte __Noise1 = PerlinNoise((int)(PX + WorldDeltaTick * 2), (int)(PY + WorldDeltaTick * 4));
                        byte __Noise2 = PerlinNoise((int)(PX + 64 + WorldDeltaTick * -3), (int)(PY + 64 + WorldDeltaTick * 2), 2);
                        byte __Noise3 = PerlinNoise((int)((FX/1.5f + PX/4f) + 32 + WorldDeltaTick), (int)((FY/1.5f + PY/4f) + 32 - WorldDeltaTick), 4);

                        ColorB WaterSolid = new ColorB(WL.Math.ClampByteB((byte)((__Noise1 / 8 + __Noise2 / 8) + 70)), 0, 0);

                        WaterSolid += new ColorB((byte)(Texture_WaterNoise.GetPixelRepeat(Palette_Default, (int)(PX + WorldDeltaTick * -4), (int)(PY + WorldDeltaTick * -5)).R / 8));

                        WaterSolid += new ColorB(__Noise3, (byte)(__Noise3 / 2), (byte)(__Noise3 / 2));

                        if(DayAlpha > 0){
                            ColorB SunColor = new ColorB(255, 255, 200, (byte)(Light((FX - SunPosition.X + SunRadius) / (SunRadius * 2), (FY - SunPosition.Y + SunRadius) / (SunRadius * 2)) * DayAlpha));
                            Result = ColorB.BlendAlpha(WaterSolid, SunColor);
                        }else{
                            Result = WaterSolid;
                        }

                        foreach((int TX, int TY, Texture Texture, Palette Palette, ColorB MultiplyColor) in WaterTextures){
                            int LocalX = (int)FX - TX + (int)Texture.Width  / 2;
                            int LocalY = (int)FY - TY + (int)Texture.Height / 2;

                            if(LocalX >= 0 && LocalX < Texture.Width && LocalY >= 0 && LocalY < Texture.Height){
                                Result = ColorB.BlendAlpha(Result ?? WaterSolid, Texture.GetPixelRepeat(Palette, LocalX, LocalY) * MultiplyColor);
                            }
                        }

                        if(Color == __WaterShaderColor_Dark){ Result -= new ColorB(64, 64, 64); }
                    }

                    if(PX <= -LevelSizeTile.X || PX >= LevelSizeTile.X || PY <= -LevelSizeTile.Y || PY >= LevelSizeTile.Y){
                        int DistanceX = 0;
                        int DistanceY = 0;

                        if(PX < -LevelSizeTile.X){ DistanceX = -(int)LevelSizeTile.X - PX; }
                        else if(PX > LevelSizeTile.X){ DistanceX = PX - (int)LevelSizeTile.X; }
                    
                        if(PY < -LevelSizeTile.Y){ DistanceY = -(int)LevelSizeTile.Y - PY; }
                        else if(PY > LevelSizeTile.Y){ DistanceY = PY - (int)LevelSizeTile.Y; }

                        int Distance = WL.Math.MaxI(DistanceX, DistanceY);

                        const int FadeDistance = 128;

                        Result = ColorB.BlendAlpha(Result ?? Color, new ColorB((byte)WL.Math.Random.Fast_Int(128, 255), 0, 0, (byte)(WL.Math.Clamp01((float)Distance / FadeDistance) * 255)));
                    }

                    if(DayAlpha == 0){
                        Result = ColorB.BlendAlpha(Result ?? Color, new ColorB(0, 0, 0, (byte)(NightAlpha * 192)));
                    }
                    
                    if(Result.HasValue){ C.SetPixel(FX, FY, Result.Value); }
                }   
            }
        }
        PostProcessing();
        
        if(RenderColliders){ Game.RenderColliders(C); }

        if(InsideCollision == CollisionLayer.L4){
            T_Item ItemOnGround = (T_Item)CollisionInfo1;
            if(ItemOnGround != T_Item.Empty){
                Entity ItemOnGroundEntity = __Entities[new EntityKey(CollisionInfo2, (uint)CollisionInfo3)];
                string ItemName__ = ItemName(ItemOnGround);
                Vector2U ItemNameSize__ = Font_Default.TextSize(ItemName__);
                int X__ = ItemOnGroundEntity.X + WorldX - (int)(ItemNameSize__.X / 2) + (16 / 2);
                int Y__ = ItemOnGroundEntity.Y + WorldY;
                C.Fill(X__ - 1, Y__ - 1, ItemNameSize__.X + 2, ItemNameSize__.Y + 2, ColorB.White.SetA(192), ImageBlend.Alpha);
                C.Border(X__ - 2, Y__ - 2, ItemNameSize__.X + 4, ItemNameSize__.Y + 4, 1, ColorB.White);
                Font_Default.Render(C, Palette_Default, ItemName__, X__, Y__);
            }
        }
        
        void RenderThoughts(int X, int Y){
            if(string.IsNullOrWhiteSpace(Thoughts)){ return; }
            Vector2U ThoughtsSize = Font_Default.TextSize(Thoughts);
            int X__ = (int)(X - ThoughtsSize.X / 2);
            uint Height__ = ThoughtsSize.Y + 14;
            Texture_Cloud.Render9Slice(C, Palette_Default, 10, X__, Y - (int)Height__, ThoughtsSize.X + 16, Height__);
            
            RenderOutlineColorText(C, Thoughts, X__ + 9, Y + 8 - (int)Height__, ColorB.Black, ColorB.White);
        }
        RenderThoughts(PlayerX, PlayerY - 8 - (int)(WL.Math.Sin(WorldDeltaTick) * 3));

        if(Interface is T_Interface.None or T_Interface.Menu && !Dead){
            if(Item == T_Item.GPS){
                void RenderMap(){
                    Vector2I __RenderCenterPanel(uint Size, ColorB Color){
                        int __X = (int)(C.Width  - Size) / 2;
                        int __Y = (int)(C.Height - Size) / 2;
                        C.Fill(__X, __Y, Size, Size, Color);
                        return new Vector2I(__X, __Y);
                    }
                    const uint MapSize = 180;
                    
                    Vector2I MapOffset = __RenderCenterPanel(MapSize, WorldBackgroundColor);

                    void __MapPixel(int X__, int Y__, ColorB Color, bool Fixed = false){
                        uint __X = (uint)MapOffset.X;
                        uint __Y = (uint)MapOffset.Y;

                        if(!Fixed){
                            X__ += (int)WorldPosition.X;
                            Y__ += (int)WorldPosition.Y;
                            
                            X__ = (int)WL.Math.Floor((float)X__ / 16);
                            Y__ = (int)WL.Math.Floor((float)Y__ / 16);
                        }
                        
                        X__ += (int)MapSize / 2;
                        Y__ += (int)MapSize / 2;
                    
                        __X += (uint)X__;
                        __Y += (uint)Y__;   
                        
                        if(__X < MapOffset.X || __Y < MapOffset.Y || __X > MapOffset.X + MapSize - 1 || __Y > MapOffset.Y + MapSize - 1){ return; }
                        
                        C[__X, __Y] = Color;
                    }
                    
                    foreach(Block Block in __Blocks.Values){
                        ColorB BlockColor = Palette_World[MapBlocksColor.GetValueOrDefault(Block.ID, (byte)1)];
                        __MapPixel(Block.X, Block.Y, BlockColor);
                    }

                    foreach(Entity Entity in __Entities.Values){
                        byte __PaletteIndex = MapEntitiesColor.GetValueOrDefault(Entity.ID, (byte)0);
                        if(__PaletteIndex == 0){ continue; }
                        ColorB EntityColor = Palette_World[__PaletteIndex];
                        __MapPixel(Entity.X, Entity.Y, EntityColor);

                        if(Entity.ID == T_Entity.Tree){
                            __MapPixel(Entity.X, Entity.Y + 16, EntityColor);
                            __MapPixel(Entity.X, Entity.Y + 16 * 2, EntityColor);
                        }
                    }
                    
                    for(int __Y__ = -(int)MapSize/2; __Y__ < MapSize/2; __Y__++){
                        for(int __X__ = -(int)MapSize/2; __X__ < MapSize/2; __X__++){
                            int PX = PlayerX - WorldX + __X__ * 16;
                            int PY = PlayerY - WorldY + __Y__ * 16;

                            if(PX <= -LevelSizeTile.X || PX >= LevelSizeTile.X || PY <= -LevelSizeTile.Y || PY >= LevelSizeTile.Y){ __MapPixel(__X__, __Y__, new ColorB((byte)WL.Math.Random.Fast_Int(128, 255)), true); }
                        }   
                    }

                    void __MapPixelPlayer(int __X, int __Y) => __MapPixel(PlayerX - WorldX + __X * 16, PlayerY - WorldY + __Y * 16, ColorB.Blue);
                    __MapPixelPlayer(0, 1);
                    __MapPixelPlayer(0, 2);
                    __MapPixelPlayer(0, -1);
                    __MapPixelPlayer(0, -2);
                    __MapPixelPlayer(1, 0);
                    __MapPixelPlayer(2, 0);
                    __MapPixelPlayer(-1, 0);
                    __MapPixelPlayer(-2, 0);
                    
                    string Coordinates = CoordinatesX + " : " + CoordinatesY;
                    Vector2U __CoordinatesSize = Font_Default.TextSize(Coordinates);
                    Font_Default.Render(C, Palette_Default, Coordinates, (int)(MapOffset.X + MapSize) - (int)__CoordinatesSize.X - 2, (int)(MapOffset.Y + MapSize) - (int)__CoordinatesSize.Y - 2);
                    
                    Texture_GPS_Overlay.Render(C, Palette_Default);
                }
                RenderMap();
            }
        }
        
        void RenderUI(){
            float HealthPulse = Dead ? 0 : WL.Math.DSin(WorldDeltaTick / WL.Math.Sqr((float)Health / HealthMax));
            ColorB FrameColor = new ColorB((byte)(HealthPulse * 255), 0, 0);

            uint Thickness = (uint)WL.Math.Min(1 + HealthPulse / WL.Math.Sqr((float)Health / HealthMax), 16);
            
            C.Border(0, 0, C.Width, C.Height, 1, FrameColor);
            C.Border(1, 1, C.Width - 2, C.Height - 2, Thickness, FrameColor.Clone().SetA(128), ImageBlend.Alpha);
            C.Border(1 + (int)Thickness, 1 + (int)Thickness, C.Width - (1 + Thickness) * 2, C.Height - (1 + Thickness) * 2, Thickness, FrameColor.Clone().SetA(64), ImageBlend.Alpha);
            
            C.Fill(20 - 1, (int)C.Height - 16 - 1, HealthMax + 2, 8 + 2, ColorB.DarkRed);
            C.Fill(20, (int)C.Height - 16, HealthMax, 8, ColorB.Black);
            C.Fill(20, (int)C.Height - 16, Health, 8, ColorB.Red);
            C.Fill(20, (int)C.Height - 16 + 3, Health, 8 - 6, ColorB.LightRed);

            Font_Default.Render(C, Palette_Default, Immortality ? "i" : Health.ToString(), 20, (int)C.Height - 16);
            
            Texture_Health.Render(C, Palette_Default, 3, (int)C.Height - 21);
            
            string __Text = (Item == T_Item.Empty ? "" : ItemName(Item)) + " [" + (SelectedItem + 1) + "]";
            Font_Default.Render(C, Palette_Default, __Text, (int)C.Width - (int)Font_Default.TextSize(__Text).X - 3, (int)C.Height - 8 - 3);

            void RenderSlot(Image.ImageContext C, byte ID, int X, int Y){
                int X__ = 20 + X * 36;
                int Y__ = 30 + Y * 36;
                C.Fill(X__, Y__, 34, 34, SelectedItem == ID ? ColorB.Lerp(ColorB.Gray, ColorB.Red, 0.5f) : ColorB.Gray);
                C.Fill(X__ + 4, Y__ + 4, 34 - 4 * 2, 34 - 4 * 2, ColorB.Black.SetA(64), ImageBlend.Alpha);
                C.Fill(X__ + 8, Y__ + 8, 34 - 8 * 2, 34 - 8 * 2, ColorB.Black.SetA(64), ImageBlend.Alpha);
                C.Fill(X__ + 12, Y__ + 12, 34 - 12 * 2, 34 - 12 * 2, ColorB.Black.SetA(64), ImageBlend.Alpha);
                C.Border(X__, Y__, 34, 34, 1, SelectedItem == ID ? ColorB.Red : ColorB.Black);

                if(SelectedItem == ID){
                    C.Border(X__ - 1, Y__ - 1, 34 + 2, 34 + 2, 1, ColorB.Red.SetA(128), ImageBlend.Alpha);
                }

                T_Item Item = Inventory[ID];
        
                if(Item != 0){
                    Texture ItemTexture = Item switch{
                        T_Item.FirstAidKit => Texture_FirstAidKit_Icon,
                        T_Item.GPS         => Texture_GPS_Icon,
                        
                        var _ => Texture_Debug
                    };
            
                    ItemTexture.Render(C, Palette_Default, X__ + 1, Y__ + 1);
                }
            }
            
            if(Interface != T_Interface.None){ C.Fill(ColorB.Black.SetA(128), ImageBlend.Alpha); }
            
            switch(Interface){
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
                        string Name = ItemName(Item);
                        
                        string Description = Item switch{
                            T_Item.FirstAidKit => "ЛЕЧИТ БЕДНЫЙ КУБИК ГУЛУ (+ с50)",
                            T_Item.GPS => "ЕСЛИ ДЕРЖАТЬ В РУКАХ,\nПОКАЗЫВАЕТ КАРТУ",
                            
                            var _ => "О БОЖЕ ЧТО ЭТО ТАКОЕ?"
                        };
                        
                        Font_Default.Render(C, Palette_Default, "[" + (byte)Item + "] " + Name, 20 + 2, 110 + 2);
                        
                        C.Fill(20, 110 + 11, C.Width - 40, 1, ColorB.Black);
                        
                        Font_Default.Render(C, Palette_Default, Description, 20 + 2, 110 + 2 + 11);
                    }
                    break;
                }
                case T_Interface.Menu:{
                    Button(0, "ПРОДОЛЖИТЬ",15, 120 + (0 * 13));
                    Button(1, "ВЫЙТИ",15, 120 + (1 * 13));
                    
                    break;
                }
            }
        }
        RenderUI();
    }
    
    internal static void RenderColorText(Image.ImageContext C, string Text, int X, int Y, ColorB Color){
        Font_Default.Render(C, Palette_White, Text, X, Y, Color);
    }

    internal static void RenderOutlineColorText(Image.ImageContext C, string Text, int X, int Y, ColorB Color, ColorB OutlineColor){
        RenderColorText(C, Text, X - 1, Y, OutlineColor);
        RenderColorText(C, Text, X + 1, Y, OutlineColor);
        RenderColorText(C, Text, X, Y - 1, OutlineColor);
        RenderColorText(C, Text, X, Y + 1, OutlineColor);
        RenderColorText(C, Text, X - 1, Y - 1, OutlineColor);
        RenderColorText(C, Text, X + 1, Y + 1, OutlineColor);
        RenderColorText(C, Text, X - 1, Y + 1, OutlineColor);
        RenderColorText(C, Text, X + 1, Y - 1, OutlineColor);
        
        RenderColorText(C, Text, X, Y, Color);
    }
}
using WL;
using WLO;
using WoowzTile;
using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Resources;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_Info;
using static GOLUWorld.GOLUWorld_UI;
using static GOLUWorld.GOLUWorld_Player;
using static GOLUWorld.GOLUWorld_World;

namespace GOLUWorld;

internal static class GOLUWorld_Render{
    /// <summary>
    /// Рендер простой кнопки (просто текст и обводка)
    /// </summary>
    internal static void UI_EasyButton(Image.ImageContext C, byte ButtonID, string ButtonText, int X, int Y, bool Always = false){
        if(UI_MenuSelectedButton == ButtonID || Always){
            Render_TextOutlineColor(C, ButtonText, X, Y, ColorB.White, ColorB.Red);
        }else{
            Render_TextOutlineColor(C, ButtonText, X, Y, ColorB.Black, ColorB.White);   
        }
    }
    
    internal const int Render_Layer_VeryBottom = -10000000;
    internal static int Render_Layer_Object(int Y) => -100000 + Y * 100;
    internal static int Render_Layer_Top(int Y) => 1000 + Y * 100;
    internal const int Render_Layer_VeryTop = 10000000;
    
    /// <summary>
    /// Добавляет элемент в рендер цикл
    /// </summary>
    internal static void Render_RenderQueue_Add(Image.ImageContext C, Renderable R, int OffsetY = 0){
        R.Y += OffsetY;
            
        const int BorderOffset = 3 * 16;
        int Border_L = -BorderOffset;
        int Border_R = (int)C.Width + BorderOffset;
        int Border_U = -BorderOffset;
        int Border_D = (int)C.Height + BorderOffset;
            
        if(!(R.X < Border_L || R.X > Border_R || R.Y < Border_U || R.Y > Border_D)){
            __RenderQueue.Add(R);

            if(R.Reflect){
                Render_RenderQueue_Water_Add(R, OffsetY);
            }
        }
    }

    /// <summary>
    /// Добавляет элемент в рендер цикл воды
    /// </summary>
    internal static void Render_RenderQueue_Water_Add(Renderable R, int OffsetY = 0){
        R.FlipY = !R.FlipY;
        R.X += (int)(R.Texture.Width  / 2   );
        R.Y += (int)(R.Texture.Height * 1.5f) - OffsetY * 2;

        ColorB __ReflectColor = R.MultiplyColor ?? ColorB.White;
        __ReflectColor.SetA(128);
        R.MultiplyColor = __ReflectColor;

        if(R.ReflectTexture != null){ R.Texture = R.ReflectTexture; }
                
        __RenderQueue_Water.Add(R);
    }
    
    /// <summary>
    /// Рендер игры
    /// </summary>
    internal static void Game_Render(Image.ImageContext C, TickData TD){
        Player_Cheat_MakeFasterTime(ref TD);
        
        float DTS = World_StopGameTime ? 0 : (float)TD.DeltaTimeS;
        
        if(!Player_Dead){ Player_LastTimeWereTreatedTimer -= DTS; }
        
        World_DeltaTick      += DTS;
        World_AnimationTimer += DTS;
        Player_BlinkTimer    += DTS;
        
        if(World_AnimationTimer > 1){ World_AnimationTimer = 0; }
        
        if(UI_InMainMenu){ UI_RenderMainMenu(C, TD); return; }

        Vector2F DayCycle_Morning = new Vector2F(0, 50);
        Vector2F DayCycle_Noon    = new Vector2F((float)C.Width/2, 25);
        Vector2F DayCycle_Evening = new Vector2F(C.Width, 50);

        Vector2F SunPosition;
        float DayAlpha;
        switch(World_Time){
            case >= 6 and < 12:
                DayAlpha = (World_Time - 6) / 6f;
                SunPosition = Vector2F.Lerp(DayCycle_Morning, DayCycle_Noon, DayAlpha);
                break;
            case >= 12 and < 18:
                DayAlpha = (World_Time - 12) / 6f;
                SunPosition = Vector2F.Lerp(DayCycle_Noon, DayCycle_Evening, DayAlpha);
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
            if(World_Time >= 18f){
                NightAlpha = 1 - (24f - World_Time) / 6f;
            }else{
                NightAlpha = 1f - World_Time / 6f;
            }
        }
        
        __RenderQueue      .Clear();
        __RenderQueue_Water.Clear();
        
        __RenderQueue_Water.Add(new Renderable{ X = (int)SunPosition.X, Y = (int)SunPosition.Y, Texture = Texture_Light, Palette = Palette_Alpha, MultiplyColor = new ColorB(255, 255, 200, (byte)(DayAlpha * 255))});
        __RenderQueue_Water.Add(new Renderable{ X = (int)SunPosition.X, Y = (int)SunPosition.Y, Texture = Texture_Circle_16px, Palette = Palette_Alpha, MultiplyColor = new ColorB(255, 255, 255, (byte)(DayAlpha * 255))});
        
        Render_Ground  (C);
        Render_Decals  (C);
        Render_Blocks  (C);
        Render_Entities(C);
        Render_Ceilings(C);
        Render_Player  (C);
        
        Render_RenderQueue(C);

        Render_PostProcessing(C, DayAlpha, NightAlpha);
        
        if(Cheat_RenderColliders){ Game.RenderColliders(C); }

        Render_DroppedItemName(C);

        Render_Thoughts(C);

        UI_RenderGPS(C);
        
        UI_Render(C);
    }
    private static readonly List<Renderable> __RenderQueue       = [];
    private static readonly List<Renderable> __RenderQueue_Water = [];

    /// <summary>
    /// Рендерит RenderQueue
    /// </summary>
    internal static void Render_RenderQueue(Image.ImageContext C){
        try{
            __RenderQueue.Sort((A, B) => A.Z.CompareTo(B.Z));
            foreach(Renderable R in __RenderQueue){
                try{
                    switch(R.Type){
                        case RenderableType.Tile : R.Texture.Render(C, R.Palette, R.X, R.Y, FlipX: R.FlipX, FlipY: R.FlipY, MultiplyColor: R.MultiplyColor, Rotation: R.Rotation); break;
                        case RenderableType.Tiles: R.Texture.RenderTiles(C, R.Palette, R.X, R.Y, R.W, R.H); break;
                    }
                }catch(Exception e){
                    throw new Exception("Произошла ошибка при рендере [" + R + "]!", e);
                }
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка в Render_RenderQueue!");
        }
    }
    
    /// <summary>
    /// Рендерит цветной текст
    /// </summary>
    internal static void Render_TextColor(Image.ImageContext C, string Text, int X, int Y, ColorB Color){
        Font_Default.Render(C, Palette_White, Text, X, Y, Color);
    }

    /// <summary>
    /// Рендерит цветной текст с обводкой
    /// </summary>
    internal static void Render_TextOutlineColor(Image.ImageContext C, string Text, int X, int Y, ColorB Color, ColorB OutlineColor){
        Render_TextColor(C, Text, X - 1, Y, OutlineColor);
        Render_TextColor(C, Text, X + 1, Y, OutlineColor);
        Render_TextColor(C, Text, X, Y - 1, OutlineColor);
        Render_TextColor(C, Text, X, Y + 1, OutlineColor);
        Render_TextColor(C, Text, X - 1, Y - 1, OutlineColor);
        Render_TextColor(C, Text, X + 1, Y + 1, OutlineColor);
        Render_TextColor(C, Text, X - 1, Y + 1, OutlineColor);
        Render_TextColor(C, Text, X + 1, Y - 1, OutlineColor);
        
        Render_TextColor(C, Text, X, Y, Color);
    }

    /// <summary>
    /// Рендерит землю (бесконечные блоки)
    /// </summary>
    internal static void Render_Ground(Image.ImageContext C){
        int OffsetX = Coordinates_World.X % 16;
        int OffsetY = Coordinates_World.Y % 16;
        if(OffsetX < 0){ OffsetX += 16; }
        if(OffsetY < 0){ OffsetY += 16; }
        for(int Y__ = -1; Y__ < 16; Y__++){
            for(int X__ = -1; X__ < 16; X__++){
                int X = X__ * 16 + OffsetX;
                int Y = Y__ * 16 + OffsetY;
                Render_RenderQueue_Add(C, new Renderable{ Texture = Texture_Ground, X = X, Y = Y, Z = Render_Layer_VeryBottom });
            }
        }
    }

    /// <summary>
    /// Рендерит блоки
    /// </summary>
    internal static void Render_Blocks(Image.ImageContext C){
        foreach(Block Block in World_Blocks.Values){
            Render_RenderQueue_Add(C, new Renderable{Texture = Info_Block_Texture(Block), X = Coordinates_World.X + Block.X, Y = Coordinates_World.Y + Block.Y, Z = Info_Block_Ground(Block.ID) ? Render_Layer_VeryBottom + 1 : Render_Layer_Object(Block.Y), Reflect = Info_Block_Reflect(Block.ID)});
        }
    }
    
    /// <summary>
    /// Рендерит потолки
    /// </summary>
    internal static void Render_Ceilings(Image.ImageContext C){
        bool HasCeiling = Player_Ceiling.ID != T_Ceiling.Empty;
        
        if(HasCeiling){
            int OffsetX = Coordinates_World.X % 16;
            int OffsetY = Coordinates_World.Y % 16;
            if(OffsetX < 0){ OffsetX += 16; }
            if(OffsetY < 0){ OffsetY += 16; }
            for(int Y__ = -1; Y__ < 16; Y__++){
                for(int X__ = -1; X__ < 16; X__++){
                    int X = X__ * 16 + OffsetX;
                    int Y = Y__ * 16 + OffsetY;
                    
                    int TileX = ((Coordinates_PlayerWorld.X + 8) + X - 8 * 16) / 16;
                    int TileY = ((Coordinates_PlayerWorld.Y + 8) + Y - 8 * 16) / 16;
                    
                    bool HasNeighborCeiling = false;

                    for(int OffsetTileY = -1; OffsetTileY <= 1 && !HasNeighborCeiling; OffsetTileY++){
                        for(int OffsetTileX = -1; OffsetTileX <= 1; OffsetTileX++){
                            if(World_GetCeiling(TileX + OffsetTileX, TileY + OffsetTileY).ID != T_Ceiling.Empty){
                                HasNeighborCeiling = true;
                                break;
                            }
                        }
                    }
                    
                    if(HasNeighborCeiling){ continue; }
                    
                    Render_RenderQueue_Add(C, new Renderable{ Texture = Texture_Black, X = X, Y = Y, Z = Render_Layer_VeryTop + 1000, MultiplyColor = new ColorB(0, 0, 0, 192)});
                }
            }
        }

        foreach(Ceiling Ceiling in World_Ceilings.Values){
            if(Ceiling.ID is T_Ceiling.Invisible){ continue; }
            if(HasCeiling){
                Render_RenderQueue_Add(C, new Renderable{Texture = Texture_Black, X = Coordinates_World.X + Ceiling.X, Y = Coordinates_World.Y + Ceiling.Y, Z = Render_Layer_VeryTop, MultiplyColor = new ColorB(0, 0, 0, 64)});   
            }else{
                bool FlipY = Ceiling.ID is T_Ceiling.RoofTiles && Ceiling.Info == 1;
                Render_RenderQueue_Add(C, new Renderable{Texture = Info_Ceiling_Texture(Ceiling), X = Coordinates_World.X + Ceiling.X, Y = Coordinates_World.Y + Ceiling.Y, Z = Render_Layer_VeryTop, FlipY = FlipY});
            }
        }
    }

    /// <summary>
    /// Рендерит декали
    /// </summary>
    internal static void Render_Decals(Image.ImageContext C){
        foreach(Decal Decal in World_Decals){
            Render_RenderQueue_Add(C, new Renderable{ Texture = Info_Decal_Texture(Decal.ID), Palette = Palette_Default, X = Coordinates_World.X + Decal.X, Y = Coordinates_World.Y + Decal.Y, Rotation = Decal.Rotation, Z = Render_Layer_VeryBottom + 2});
        }
    }

    /// <summary>
    /// Рендерит сущностей
    /// </summary>
    internal static void Render_Entities(Image.ImageContext C){
         foreach(Entity Entity in World_Entities.Values){
            int? __ReflectOffsetY = Info_Entity_Reflect(Entity.ID);
            
            if(Info_Entity_DoRender(Entity.ID)){
                int OffsetX = 0;
                int OffsetY = 0;
                
                if(Info_Entity_Plant(Entity.ID)){
                    if(Entity.Info != 0){
                        uint __Seed = Entity.Info;
                        OffsetX += WL.Math.Random.Fast_Int(-8, 8, ref __Seed);
                        __Seed += 256;
                        OffsetY += WL.Math.Random.Fast_Int(-8, 8, ref __Seed);
                    }
                        
                    OffsetX += (int)(WL.Math.Sin(World_DeltaTick * 2 + (Entity.X * 2 + Entity.Y), 1) * 2);
                }
                
                int Z = Render_Layer_Object(Entity.Y + OffsetY);
                
                switch(Entity.ID){
                    case T_Entity.Tree:
                        OffsetY = -48;
                        break;
                    
                    case T_Entity.Mob_Spider: {
                        OffsetX = -8;
                        OffsetY = -8;

                        if(!Entity.Dead){ Z = Render_Layer_VeryTop + 100; }
                        break;
                    }
                    case T_Entity.Spikes:
                        Z = Render_Layer_VeryBottom + 3;
                        break;
                }

                if(Entity.ID is T_Entity.Crate or T_Entity.Item){ Z++; }

                Render_RenderQueue_Add(C, new Renderable{ Texture = Info_Entity_Texture(Entity), Palette = Palette_Default, X = Coordinates_World.X + Entity.X + OffsetX, Y = Coordinates_World.Y + Entity.Y + OffsetY, Rotation = Entity.Rotation, Z = Z, Reflect = __ReflectOffsetY.HasValue}, __ReflectOffsetY ?? 0);

                if(Entity.ID == T_Entity.Tree){
                    void __RenderLeaves(int X__, int Y__){
                        int __X__ = -16;
                        int __Y__ = -64;

                        __X__ += (X__ - 1) * (X__ == 0 ? 16 : 8);
                        __Y__ += (Y__ - 1) * (Y__ == 0 ? 8 : 16);

                        __X__ += (int)(WL.Math.Sin(World_DeltaTick * 2 + (Entity.X + __X__) * 432, 1) * 2);
                        __Y__ += (int)(WL.Math.Sin(World_DeltaTick * 2 + (Entity.Y + __Y__) * 12 , 1) * 2);
                        Render_RenderQueue_Add(C, new Renderable{ Texture = Texture_Tree_Leaves, Palette = Palette_Default, X = Coordinates_World.X + Entity.X + __X__, Y = Coordinates_World.Y + Entity.Y + __Y__, Rotation = Entity.Rotation, Z = Render_Layer_VeryTop + 1 + (X__ + Y__)});
                    }
                    __RenderLeaves(0, 0);
                    __RenderLeaves(2, 0);
                    __RenderLeaves(1, 2);
                }
            }
        }
    }

    /// <summary>
    /// Рендерит игрока
    /// </summary>
    internal static void Render_Player(Image.ImageContext C){
        Texture PlayerBody  = Texture_Player_Body;
        Texture PlayerEyes  = Texture_Player_Eyes;
        Texture PlayerNose  = Texture_Player_Nose;
        Texture PlayerMouth = (Player_Dead ? Texture_Player_Mouth : Emotion_Happiness < 25 ? Texture_Player_Mouth_Sad : (Emotion_Happiness > 75 ? Texture_Player_Mouth_Happy : Texture_Player_Mouth));

        if(Player_BlinkTimer > 3 || Player_Dead){
            PlayerEyes = Texture_Player_Eyes_Blink;
            if(Player_BlinkTimer > 3.25f){
                Player_BlinkTimer = 0;
            }
        }
    
        if(Player_MovingDirection.X != 0){
            Player_TextureFlipped = Player_MovingDirection.X > 0;
        }
        
        int __PlayerZ = 0;
        void __RenderPlayerPart(Texture T, ColorB? Color, int OffsetX = 0, int OffsetY = 0, Texture? UniqueReflectionTexture = null, TextureRotation Rotation = TextureRotation.None){
            Render_RenderQueue_Add(C, new Renderable{ Texture = T, Palette = Palette_Default, X = Coordinates_Player.X + OffsetX, Y = Coordinates_Player.Y, FlipX = Player_TextureFlipped, MultiplyColor = Color, Z = Render_Layer_Object(Coordinates_Player.Y - Coordinates_World.Y + 1) + __PlayerZ, Reflect = true, ReflectTexture = UniqueReflectionTexture, Rotation = Rotation}, OffsetY);
            __PlayerZ++;
        }
        
        T_Item Item = Player_ItemInHands;
        if(Item != T_Item.Empty){
            int __OffsetX = 0;
            int __OffsetY = -11;
            TextureRotation RotateItem = TextureRotation.None;

            if(Player_AttackTimer > 0){
                const int AttackDistance = 13;

                switch(Player_AttackDirection){
                    case Direction4.Right:
                        __OffsetX = AttackDistance;
                        __OffsetY = (int)WL.Math.Lerp(-AttackDistance, AttackDistance, Player_AttackTimer);
                        RotateItem = Player_TextureFlipped ? TextureRotation.Rotate270 : TextureRotation.Rotate90;
                        break;
                    case Direction4.Left:
                        __OffsetX = -AttackDistance;
                        __OffsetY = (int)WL.Math.Lerp(AttackDistance, -AttackDistance, Player_AttackTimer);
                        RotateItem = Player_TextureFlipped ? TextureRotation.Rotate90 : TextureRotation.Rotate270;
                        break;
                    case Direction4.Up:
                        __OffsetX = (int)WL.Math.Lerp(-AttackDistance, AttackDistance, Player_AttackTimer);
                        __OffsetY = -AttackDistance;
                        RotateItem = TextureRotation.None;
                        break;
                    case Direction4.Down:
                        __OffsetX = (int)WL.Math.Lerp(AttackDistance, -AttackDistance, Player_AttackTimer);
                        __OffsetY = AttackDistance;
                        RotateItem = TextureRotation.Rotate180;
                        break;
                }
            }
            
            __RenderPlayerPart(Info_Item_Texture(Item), null, __OffsetX, __OffsetY, Rotation: RotateItem);
        }
    
        ColorB PlayerColor = ColorB.Lerp(ColorB.White, ColorB.DarkRed, WL.Math.Clamp01((Player_Rotting - 2) / 50));
        
        __RenderPlayerPart(PlayerBody, PlayerColor);
        __RenderPlayerPart(PlayerNose, PlayerColor);
        __RenderPlayerPart(PlayerMouth, PlayerColor);
        __RenderPlayerPart(PlayerEyes, PlayerColor, UniqueReflectionTexture: Texture_Player_Eyes_Reflection);

        if(Player_Health < Player_HealthLow * 2){
            Texture PlayerBlood = Player_Health < Player_HealthLow ? Texture_Player_Blood_Strong : Texture_Player_Blood;
            __RenderPlayerPart(PlayerBlood, ColorB.Lerp(ColorB.White, ColorB.DarkGreen, WL.Math.Clamp01((Player_Rotting - 2) / 50)));
        }
    
        if(Player_LastTimeWereTreatedTimer > 0){
            __RenderPlayerPart(Texture_Player_Healed, null);
        }
    }
    
    /// <summary>
    /// Делает пост-процессинг
    /// </summary>
    internal static void Render_PostProcessing(Image.ImageContext C, float DayAlpha, float NightAlpha){
       byte PerlinNoise(int X, int Y, float Scale = 1) => Texture_PerlinNoise.GetPixelRepeat(Palette_Default, X, Y, Scale).R;
       
        for(uint FY = 0; FY < C.Height; FY++){
            for(uint FX = 0; FX < C.Width; FX++){
                ColorB Color = C[FX, FY];
                ColorB? Result = null;

                int PX = Coordinates_PlayerWorld.X - ((int)C.Width /2 - (int)FX);
                int PY = Coordinates_PlayerWorld.Y - ((int)C.Height/2 - (int)FY);

                ColorB __WaterShaderColor_InCeilingWarFog = new ColorB(63, 0, 0);
                if(Color == __WaterShaderColor || Color == __WaterShaderColor_Dark || Color == __WaterShaderColor_InCeilingWarFog){
                    byte __Noise1 = PerlinNoise((int)(PX + World_DeltaTick * 2), (int)(PY + World_DeltaTick * 4));
                    byte __Noise2 = PerlinNoise((int)((FX/1.5f + PX/4f) + 32 + World_DeltaTick), (int)((FY/1.5f + PY/4f) + 32 - World_DeltaTick), 4);

                    ColorB WaterSolid = new ColorB(WL.Math.ClampByteB((byte)((__Noise1 / 4) + 70)), 0, 0);

                    WaterSolid += new ColorB((byte)(Texture_WaterNoise.GetPixelRepeat(Palette_Default, (int)(PX + World_DeltaTick * -4), (int)(PY + World_DeltaTick * -5)).R / 8));

                    WaterSolid += new ColorB((byte)(__Noise2 / 2), (byte)(__Noise2 / 4), (byte)(__Noise2 / 4));
                    
                    Result = WaterSolid;
                    
                    foreach(Renderable R in __RenderQueue_Water){
                        int LocalX = (int)FX - R.X + (int)R.Texture.Width  / 2;
                        int LocalY = (int)FY - R.Y + (int)R.Texture.Height / 2;
                        
                        if(__RenderQueue_Water.Count > 10){
                            if(LocalX >= 0 && LocalX < R.Texture.Width && LocalY >= 0 && LocalY < R.Texture.Height){
                                Result = ColorB.BlendAlpha(Result ?? WaterSolid, R.Texture.GetPixelRepeat(R.Palette, LocalX, LocalY, FlipX: R.FlipX, FlipY: R.FlipY) * (R.MultiplyColor ?? ColorB.White));
                            }
                        }else{
                            float RippleX = WL.Math.Sin((Coordinates_PlayerWorld.X + FX + World_DeltaTick * 10) * 0.1f, 1) * 4f;
                            float RippleY = WL.Math.Cos((Coordinates_PlayerWorld.Y + FY + World_DeltaTick * 10) * 0.3f, 1) * 2f;
                        
                            int DistX = (int)(LocalX + RippleX);
                            int DistY = (int)(LocalY + RippleY + 2);
                        
                            if(DistX >= 0 && DistX < R.Texture.Width && DistY >= 0 && DistY < R.Texture.Height){
                                Result = ColorB.BlendAlpha(Result ?? WaterSolid, R.Texture.GetPixelRepeat(R.Palette, DistX, DistY, FlipX: R.FlipX, FlipY: R.FlipY) * (R.MultiplyColor ?? ColorB.White));
                            }
                        }
                    }

                    if(Color == __WaterShaderColor_Dark){
                        Result -= new ColorB(64, 64, 64);
                    }else if(Color == __WaterShaderColor_InCeilingWarFog){
                        Result -= new ColorB(128, 128, 128);
                    }
                }

                if((PX <= -World_SizeWorld.X || PX >= World_SizeWorld.X || PY <= -World_SizeWorld.Y || PY >= World_SizeWorld.Y) && !Cheat_DisableWorldLimit){
                    int DistanceX = 0;
                    int DistanceY = 0;

                    if(PX < -World_SizeWorld.X){ DistanceX = -(int)World_SizeWorld.X - PX; }
                    else if(PX > World_SizeWorld.X){ DistanceX = PX - (int)World_SizeWorld.X; }
                
                    if(PY < -World_SizeWorld.Y){ DistanceY = -(int)World_SizeWorld.Y - PY; }
                    else if(PY > World_SizeWorld.Y){ DistanceY = PY - (int)World_SizeWorld.Y; }

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

    /// <summary>
    /// Рендерит название лежащего предмета
    /// </summary>
    internal static void Render_DroppedItemName(Image.ImageContext C){
        if(Player_InteractingCollision == CollisionLayer.L4){
            T_Item ItemOnGround = (T_Item)Player_CollisionInfo1;
            if(ItemOnGround != T_Item.Empty){
                Entity ItemOnGroundEntity = World_Entities[new EntityKey(Player_CollisionInfo2, (uint)Player_CollisionInfo3)];
                string ItemName__ = Info_Item_Name(ItemOnGround);
                Vector2U ItemNameSize__ = Font_Default.TextSize(ItemName__);
                int X__ = ItemOnGroundEntity.X + Coordinates_World.X - (int)(ItemNameSize__.X / 2) + (16 / 2);
                int Y__ = ItemOnGroundEntity.Y + Coordinates_World.Y;
                C.Fill(X__ - 1, Y__ - 1, ItemNameSize__.X + 2, ItemNameSize__.Y + 2, ColorB.White.SetA(192), ImageBlend.Alpha);
                C.Border(X__ - 2, Y__ - 2, ItemNameSize__.X + 4, ItemNameSize__.Y + 4, 1, ColorB.White);
                Font_Default.Render(C, Palette_Default, ItemName__, X__, Y__);
            }
        }
    }

    /// <summary>
    /// Рендерит мысли игрока
    /// </summary>
    internal static void Render_Thoughts(Image.ImageContext C){
        int X = Coordinates_Player.X;
        int Y = Coordinates_Player.Y - 8 - (int)(WL.Math.Sin(World_DeltaTick, 1) * 3);
        
        if(string.IsNullOrWhiteSpace(Player_Thought)){ return; }
        Vector2U ThoughtsSize = Font_Default.TextSize(Player_Thought);
        int X__ = (int)(X - ThoughtsSize.X / 2);
        uint Height__ = ThoughtsSize.Y + 14;
        Texture_Cloud.Render9Slice(C, Palette_Default, 10, X__, Y - (int)Height__, ThoughtsSize.X + 16, Height__);
        
        Render_TextOutlineColor(C, Player_Thought, X__ + 9, Y + 8 - (int)Height__, ColorB.Black, ColorB.White);
    }
}
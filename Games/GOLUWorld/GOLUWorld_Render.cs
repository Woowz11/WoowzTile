using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
using static GOLUWorld.GOLUWorld_Utility;

namespace GOLUWorld;

internal static class GOLUWorld_Render{
    internal static int Render_CameraClip_L(Image.ImageContext C, int Offset) => -Offset;
    internal static int Render_CameraClip_R(Image.ImageContext C, int Offset) => (int)C.Width + Offset;
    internal static int Render_CameraClip_U(Image.ImageContext C, int Offset) => -Offset;
    internal static int Render_CameraClip_D(Image.ImageContext C, int Offset) => (int)C.Height + Offset;
    
    internal const int Render_Layer_VeryBottom = -10000000;
    internal static int Render_Layer_Object(int Y) => -100000 + Y * 100;
    internal static int Render_Layer_Top(int Y) => 1000 + Y * 100;
    internal const int Render_Layer_VeryTop = 10000000;
    
    /// <summary>
    /// Добавляет элемент в рендер цикл
    /// </summary>
    internal static void Render_RenderQueue_Add(Image.ImageContext C, Renderable R, int OffsetY = 0, int? ReflectOffsetY = null){
        R.Y += OffsetY;
            
        const int Offset = 5 * 16;
        if(R.RenderAnyway || !(R.X < Render_CameraClip_L(C, Offset) || R.X > Render_CameraClip_R(C, Offset) || R.Y < Render_CameraClip_U(C, Offset) || R.Y > Render_CameraClip_D(C, Offset))){
            __RenderQueue.Add(R);

            if(R.Reflect){
                Render_RenderQueue_Water_Add(R, ReflectOffsetY ?? OffsetY);
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
        
        World_AnimationNonStopTimer += (float)TD.DeltaTimeS;
        if(World_AnimationNonStopTimer > 1){ World_AnimationNonStopTimer = 0; }
        
        float DTS = World_StopGameTime ? 0 : (float)TD.DeltaTimeS;
        
        if(!Player_Dead){ Player_LastTimeWereTreated_Timer -= DTS; }
        
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

        if(World_Type == T_World.Industrial){
            Vector2I __Pos = Utility_WorldToScreen(Coordinates_Spawn);
            Render_RenderQueue_Add(C, new Renderable{ Texture = Texture_Light_Ray, X = __Pos.X - 8, Y = __Pos.Y - 256 + 32, Z = Render_Layer_Object(Coordinates_Spawn.Y + 16), Palette = Palette_Alpha, RenderAnyway = true });
        }
        
        Render_Ground  (C);
        Render_Decals  (C);
        Render_Blocks  (C);
        Render_Entities(C);
        Render_Ceilings(C);
        Render_Player  (C);
        
        Render_RenderQueue(C);

        Render_PostProcessing(C, DayAlpha, NightAlpha);
        
        if(Cheat_RenderColliders){ Game.RenderColliders(C); }

        Render_InteractInfo(C);

        Render_Thoughts(C);

        UI_RenderGPS(C);
        UI_RenderClock(C);
        
        UI_Render(C, TD);
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
    internal static void Render_TextColorOutline(Image.ImageContext C, string Text, int X, int Y, ColorB Color, ColorB OutlineColor){
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
        Texture GroundTexture = World_Type == T_World.Calm ? Texture_Ground : Texture_Ground_Industrial;
        
        int OffsetX = Coordinates_World.X % 16;
        int OffsetY = Coordinates_World.Y % 16;
        if(OffsetX < 0){ OffsetX += 16; }
        if(OffsetY < 0){ OffsetY += 16; }
        for(int Y__ = -1; Y__ < 16; Y__++){
            for(int X__ = -1; X__ < 16; X__++){
                int X = X__ * 16 + OffsetX;
                int Y = Y__ * 16 + OffsetY;
                Render_RenderQueue_Add(C, new Renderable{ Texture = GroundTexture, X = X, Y = Y, Z = Render_Layer_VeryBottom });
            }
        }
    }

    /// <summary>
    /// Рендерит блоки
    /// </summary>
    internal static void Render_Blocks(Image.ImageContext C){
        foreach((Vector2I Key, Block Block) in World_Blocks){
            int WorldX = Block.X + Coordinates_World.X;
            int WorldY = Block.Y + Coordinates_World.Y;
            
            const int Offset = 3 * 16;
            if(WorldX < Render_CameraClip_L(C, Offset) || WorldX > Render_CameraClip_R(C, Offset) || WorldY < Render_CameraClip_U(C, Offset) || WorldY > Render_CameraClip_D(C, Offset)){ continue; }
            
            Render_RenderQueue_Add(C, new Renderable{Texture = Info_Block_Texture(Block), X = WorldX, Y = WorldY, Z = Info_Block_Ground(Block.ID) ? Render_Layer_VeryBottom + 1 : Render_Layer_Object(Block.Y), Reflect = Info_Block_Reflect(Block.ID)});
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

        foreach((Vector2I Key, Ceiling Ceiling) in World_Ceilings){
            if(Ceiling.ID is T_Ceiling.Invisible){ continue; }
            if(HasCeiling){
                Render_RenderQueue_Add(C, new Renderable{Texture = Texture_Black, X = Coordinates_World.X + Ceiling.X, Y = Coordinates_World.Y + Ceiling.Y, Z = Render_Layer_VeryTop, MultiplyColor = new ColorB(0, 0, 0, 64)});   
            }else{
                bool FlipX = Ceiling.ID is T_Ceiling.RoofTiles && Ceiling.Info is 3;
                bool FlipY = Ceiling.ID is T_Ceiling.RoofTiles && Ceiling.Info is 1;
                TextureRotation Rotation = Ceiling.ID is T_Ceiling.RoofTiles && Ceiling.Info is 2 or 3 ? TextureRotation.Rotate90 : TextureRotation.None;
                Render_RenderQueue_Add(C, new Renderable{Texture = Info_Ceiling_Texture(Ceiling), X = Coordinates_World.X + Ceiling.X, Y = Coordinates_World.Y + Ceiling.Y, Z = Render_Layer_VeryTop, FlipX = FlipX, FlipY = FlipY, Rotation = Rotation});
            }
        }
    }

    /// <summary>
    /// Рендерит декали
    /// </summary>
    internal static void Render_Decals(Image.ImageContext C){
        foreach(Decal Decal in World_Decals){
            uint __Seed = (uint)((Decal.X + Decal.Y) * Decal.Y);
            Render_RenderQueue_Add(C, new Renderable{ Texture = Info_Decal_Texture(Decal.ID), Palette = Palette_World, X = Coordinates_World.X + Decal.X, Y = Coordinates_World.Y + Decal.Y, Rotation = Decal.Rotation, Z = Render_Layer_VeryBottom + WL.Math.Random.Fast_Int(2, 1000, ref __Seed)});
        }
    }

    /// <summary>
    /// Рендерит сущностей
    /// </summary>
    internal static void Render_Entities(Image.ImageContext C){
         foreach((EntityKey Key, Entity Entity) in World_Entities){
            if(!Info_Entity_DoRender(Entity.ID)){ continue; }

            int WorldX = Entity.X + Coordinates_World.X;
            int WorldY = Entity.Y + Coordinates_World.Y;
            
            const int Offset = 5 * 16;
            if(WorldX < Render_CameraClip_L(C, Offset) || WorldX > Render_CameraClip_R(C, Offset) || WorldY < Render_CameraClip_U(C, Offset) || WorldY > Render_CameraClip_D(C, Offset)){ continue; }
            
            int? __ReflectOffsetY = Info_Entity_Reflect(Entity.ID);

            int OffsetX = 0;
            int OffsetY = 0;

            if(Info_Entity_Plant(Entity.ID)){
                if(Entity.Info != 0){
                    uint __Seed = Entity.Info;
                    OffsetX += WL.Math.Random.Fast_Int(-8, 8, ref __Seed);
                    __Seed += 2567821;
                    OffsetY += WL.Math.Random.Fast_Int(-8, 8, ref __Seed);
                }
                    
                OffsetX += (int)(WL.Math.SinVeryFast(World_DeltaTick * 2 + (Entity.X * 2 + Entity.Y)) * 2);
            }

            int Z = Render_Layer_Object(Entity.Y + OffsetY);
            TextureRotation Rotation = Entity.Rotation;

            switch(Entity.ID){
                case T_Entity.Tree:
                    if(Entity.Info != 2){ OffsetY = -48; }
                    break;
                
                case T_Entity.Cattail:
                case T_Entity.HighGrass:
                    OffsetY = -16;
                    break;
                
                case T_Entity.Grave:
                    OffsetX = -8;
                    OffsetY = -16;
                    break;
                
                case T_Entity.Mob_Spider: {
                    OffsetX = -8;
                    OffsetY = -16;

                    if(!Entity.Dead){ Z = Render_Layer_VeryTop + 100; }
                    break;
                }
                
                case T_Entity.Mob_Drone: {
                    OffsetX = -8;
                    OffsetY = -8;

                    if(!Entity.Dead){ Z = Render_Layer_VeryTop + 100; }
                    break;
                }
                
                case T_Entity.Door:
                    Z = Render_Layer_Object(Entity.Y + 16 + OffsetY);
                    Rotation = Entity.Info is 2 or 3 ? TextureRotation.Rotate90 : TextureRotation.None;
                    break;
                case T_Entity.Trapdoor:
                case T_Entity.Spikes:
                case T_Entity.Money:
                case T_Entity.Tire:
                    Z = Render_Layer_Object(Entity.Y - 16 + OffsetY);
                    break;
            }

            if(Entity.ID is T_Entity.Crate or T_Entity.Item){ Z++; }

            (Texture Texture, bool? FlipX, bool? FlipY) TextureInfo = Info_Entity_Texture(Entity);
            
            Render_RenderQueue_Add(C, new Renderable{ Texture = TextureInfo.Texture, Palette = Entity.ID is T_Entity.Item or T_Entity.Money ? Palette_Default : Palette_World, X = WorldX + OffsetX, Y = WorldY + OffsetY, Rotation = Rotation, Z = Z, Reflect = __ReflectOffsetY.HasValue, MultiplyColor = Player_ClosestEntity.HasValue && Player_ClosestEntity_Distance < Player_Interact_Distance && Player_ClosestEntity.Value.Key == Entity.Key ? ColorB.Red : null, FlipX = TextureInfo.FlipX ?? false, FlipY = TextureInfo.FlipY ?? false}, __ReflectOffsetY ?? 0);

            if(Entity.ID == T_Entity.Tree){
                if(Entity.Info == 0){
                    void __RenderLeaves(int X__, int Y__){
                        int __X__ = -16;
                        int __Y__ = -64;

                        __X__ += (X__ - 1) * (X__ == 0 ? 16 : 8);
                        __Y__ += (Y__ - 1) * (Y__ == 0 ? 8 : 16);

                        __X__ += (int)(WL.Math.SinVeryFast(World_DeltaTick * 2 + (Entity.X + __X__) * 432) * 2);
                        __Y__ += (int)(WL.Math.SinVeryFast(World_DeltaTick * 2 + (Entity.Y + __Y__) * 12 ) * 2);
                        Render_RenderQueue_Add(C,
                        new Renderable{ Texture = Texture_Tree_Leaves, Palette = Palette_World, X = WorldX + __X__, Y = WorldY + __Y__, Z = Render_Layer_VeryTop + 1 + WorldY + __Y__ + (X__ + Y__) });
                    }

                    __RenderLeaves(0, 0);
                    __RenderLeaves(2, 0);
                    __RenderLeaves(1, 2);
                }else if(Entity.Info == 1){
                    int __X__ = -16;
                    int __Y__ = -60;
                    
                    __X__ += (int)(WL.Math.SinVeryFast(World_DeltaTick * 2 + (Entity.X + __X__) * 432) * 2);
                    __Y__ += (int)(WL.Math.SinVeryFast(World_DeltaTick * 2 + (Entity.Y + __Y__) * 12 ) * 2);
                    Render_RenderQueue_Add(C, new Renderable{ Texture = Texture_Tree_Spruce, Palette = Palette_World, X = WorldX + __X__, Y = WorldY + __Y__, Z = Render_Layer_VeryTop + 1 + WorldY + __Y__ });
                }
            }
        }
    }

    /// <summary>
    /// Рендерит игрока
    /// </summary>
    internal static void Render_Player(Image.ImageContext C){
        Texture Body  = Texture_Player_Body;
        Texture Eyes  = Texture_Player_Eyes;
        Texture Nose  = Texture_Player_Nose;
        Texture Mouth = (Player_Dead ? Texture_Player_Mouth : Emotion_Happiness < 25 ? Texture_Player_Mouth_Sad : (Emotion_Happiness > 75 ? Texture_Player_Mouth_Happy : Texture_Player_Mouth));
        if(Player_Character_Mute){ Mouth = Texture_Player_Mouth_Mute; }
        
        if(Player_BlinkTimer > 3 || Player_Dead){
            Eyes = Texture_Player_Eyes_Blink;
            if(Player_BlinkTimer > 3.25f){
                Player_BlinkTimer = 0;
            }
        }
    
        if(Player_MovingDirection.X != 0){
            Player_TextureFlipped = Player_MovingDirection.X > 0;
        }
        
        int __PlayerZ = 0;
        void __RenderPlayerPart(Texture T, ColorB? Color, int OffsetX = 0, int OffsetY = 0, Texture? UniqueReflectionTexture = null, TextureRotation Rotation = TextureRotation.None, int? ReflectOffsetY = null, int TextureSize = 16){
            Render_RenderQueue_Add(C, new Renderable{ Texture = T, Palette = Palette_Default, X = Coordinates_Player.X + OffsetX, Y = Coordinates_Player.Y, FlipX = Player_TextureFlipped, MultiplyColor = Color, Z = Render_Layer_Object(Coordinates_Player.Y - Coordinates_World.Y + 1) + __PlayerZ, Reflect = true, ReflectTexture = UniqueReflectionTexture, Rotation = Rotation}, OffsetY, ReflectOffsetY ?? OffsetY - (16 - TextureSize));
            __PlayerZ++;
        }
        
        T_Item Item = Player_ItemInHands;
        if(Item != T_Item.Empty){
            Texture ItemTexture = Info_Item_Texture(Item);
            
            int __OffsetX = -((int)ItemTexture.Width - 16) / 2;
            int __OffsetY = -11 - ((int)ItemTexture.Height - 16);
            TextureRotation RotateItem = TextureRotation.None;

            if(Player_Attack_Timer > 0){
                const int AttackDistance = 13;

                switch(Player_AttackDirection){
                    case Direction4.Right:
                        __OffsetX = AttackDistance;
                        __OffsetY = (int)WL.Math.Lerp(-AttackDistance, AttackDistance, Player_Attack_Timer) - ((int)ItemTexture.Width - 16) / 2;
                        RotateItem = Player_TextureFlipped ? TextureRotation.Rotate270 : TextureRotation.Rotate90;
                        break;
                    case Direction4.Left:
                        __OffsetX = -AttackDistance - ((int)ItemTexture.Height - 16);
                        __OffsetY = (int)WL.Math.Lerp(AttackDistance, -AttackDistance, Player_Attack_Timer) - ((int)ItemTexture.Width - 16) / 2;
                        RotateItem = Player_TextureFlipped ? TextureRotation.Rotate90 : TextureRotation.Rotate270;
                        break;
                    case Direction4.Up:
                        __OffsetX = (int)WL.Math.Lerp(-AttackDistance, AttackDistance, Player_Attack_Timer) - ((int)ItemTexture.Width - 16) / 2;
                        __OffsetY = -AttackDistance - ((int)ItemTexture.Height - 16);
                        RotateItem = TextureRotation.None;
                        break;
                    case Direction4.Down:
                        __OffsetX = (int)WL.Math.Lerp(AttackDistance, -AttackDistance, Player_Attack_Timer) - ((int)ItemTexture.Width - 16) / 2;
                        __OffsetY = AttackDistance;
                        RotateItem = TextureRotation.Rotate180;
                        break;
                }
            }
            
            __RenderPlayerPart(ItemTexture, null, __OffsetX, __OffsetY, Rotation: RotateItem, ReflectOffsetY: __OffsetY + ((int)ItemTexture.Height - 16));
        }
    
        ColorB BodyColor = ColorB.Lerp(ColorB.White, ColorB.DarkRed, WL.Math.Clamp01((Player_Rotting - 2) / 50));
        
        __RenderPlayerPart(Body, BodyColor);
        __RenderPlayerPart(Nose, BodyColor);
        __RenderPlayerPart(Mouth, BodyColor);

        Texture EyesReflection = Texture_Player_Eyes_Reflection;
        if(Player_UseCheats){
            (EyesReflection, Eyes) = (Eyes, EyesReflection);
        }
        __RenderPlayerPart(Eyes, BodyColor, UniqueReflectionTexture: EyesReflection);

        ColorB BloodColor = ColorB.Lerp(ColorB.White, ColorB.DarkGreen, WL.Math.Clamp01((Player_Rotting - 2) / 50));
        if(Player_Health < Player_HealthLow * 2){
            Texture PlayerBlood = Player_Health < Player_HealthLow ? Texture_Player_Blood_Strong : Texture_Player_Blood;
            __RenderPlayerPart(PlayerBlood, BloodColor);
        }
        
        if(Player_BrokenLeg){ __RenderPlayerPart(Texture_Player_BrokenLeg, BloodColor, -8, -8, TextureSize: 32); }
    
        if(Player_LastTimeWereTreated_Timer > 0){
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
                if(Color == __ShaderColor_Water || Color == __ShaderColor_WaterDark || Color == __WaterShaderColor_InCeilingWarFog){
                    byte __Noise1 = PerlinNoise((int)(PX + World_DeltaTick * 2), (int)(PY + World_DeltaTick * 4));
                    byte __Noise2 = PerlinNoise((int)((FX/1.5f + PX/4f) + 32 + World_DeltaTick), (int)((FY/1.5f + PY/4f) + 32 - World_DeltaTick), 4);

                    ColorB WaterSolid = new ColorB(WL.Math.ClampByteB((byte)((__Noise1 / 4) + 70)), 0, 0);

                    WaterSolid += new ColorB((byte)(Texture_WaterNoise.GetPixelRepeat(Palette_Default, (int)(PX + World_DeltaTick * -4), (int)(PY + World_DeltaTick * -5)).R / 8));

                    WaterSolid += new ColorB((byte)(__Noise2 / 2), (byte)(__Noise2 / 4), (byte)(__Noise2 / 4));
                    
                    Result = WaterSolid;
                    
                    float RippleX = WL.Math.SinVeryFast((Coordinates_PlayerWorld.X + FX + World_DeltaTick * 10) * 0.1f) * 4f;
                    float RippleY = WL.Math.CosVeryFast((Coordinates_PlayerWorld.Y + FY + World_DeltaTick * 10) * 0.3f) * 2f;
                    RippleY += RippleX;
                    foreach(Renderable R in __RenderQueue_Water){
                        int LocalX = (int)FX - R.X + (int)R.Texture.Width  / 2;
                        int LocalY = (int)FY - R.Y + (int)R.Texture.Height / 2;
                        
                        int DistX = (int)(LocalX + RippleX);
                        int DistY = (int)(LocalY + RippleY + 2);
                    
                        if(DistX >= 0 && DistX < R.Texture.Width && DistY >= 0 && DistY < R.Texture.Height){
                            Result = ColorB.BlendAlpha(Result ?? WaterSolid, R.Texture.GetPixelRepeat(R.Palette, DistX, DistY, FlipX: R.FlipX, FlipY: R.FlipY) * (R.MultiplyColor ?? ColorB.White));
                        }
                    }

                    if(Color == __ShaderColor_WaterDark){
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

                    float D = (float)Distance / FadeDistance;
                    ColorB ColorFade = new ColorB((byte)WL.Math.Random.Fast_Int(128, 255), 0, 0, (byte)(WL.Math.Clamp01(D) * 255));
                    if(D <= 0.001f){ ColorFade = __ShaderColor_GlowRed; }
                    Result = ColorB.BlendAlpha(Result ?? Color, ColorFade);
                }

                if(DayAlpha == 0){
                    Result = ColorB.BlendAlpha(Result ?? Color, new ColorB(0, 0, 0, (byte)(NightAlpha * 192)));
                }
                
                if(Result.HasValue){ C.SetPixel(FX, FY, Result.Value); }
            }   
        }
    }
    
     /// <summary>
    /// Делает пост-процессинг после интерфейса
    /// </summary>
    internal static void Render_PostPostProcessing(Image.ImageContext C, TickData TD){
        for(uint FY = 0; FY < C.Height; FY++){
            for(uint FX = 0; FX < C.Width; FX++){
                ColorB Color = C[FX, FY];
                ColorB? Result = null;

                if(Color == __ShaderColor_GlowRed){
                    int GlowRadius = (int)(WL.Math.DSin((float)TD.DeltaTick * 50, 1) * 4) + 2;

                    ColorB GlowColor = ColorB.Red;
                    int GlowRadiusSqr = WL.Math.SqrI(GlowRadius);
                    
                    for(int DY = -GlowRadius; DY <= GlowRadius; DY++){
                        for(int DX = -GlowRadius; DX <= GlowRadius; DX++){
                            uint FX2 = (uint)(FX + DX);
                            uint FY2 = (uint)(FY + DY);

                            if(FX2 >= C.Width || FY2 >= C.Height){ continue; }

                            int DistSqr = WL.Math.SqrI(DX) + WL.Math.SqrI(DY);
                            if(DistSqr > GlowRadiusSqr){ continue; }

                            float Factor = (1f - (float)DistSqr / GlowRadiusSqr) / 2;

                            C.SetPixel(FX2, FY2, ColorB.BlendAlpha(C[FX2, FY2], new ColorB(GlowColor.R, GlowColor.G, GlowColor.B, (byte)(GlowColor.A * Factor))));
                        }
                    }

                    Result = ColorB.BlendAlpha(Result ?? Color, GlowColor);
                }
                
                if(Result.HasValue){ C.SetPixel(FX, FY, Result.Value); }
            }   
        }
    }

    /// <summary>
    /// Рендерит информацию об взаимодействующем объекте
    /// </summary>
    internal static void Render_InteractInfo(Image.ImageContext C){
        if(Player_ClosestEntity != null && Player_ClosestEntity_Distance < Player_Interact_Distance){
            string? Text = Info_Entity_InteractText(Player_ClosestEntity.Value);
            
            if(Text != null){
                Vector2U TextSize = Font_Default.TextSize(Text);
                int X__ = Player_ClosestEntity.Value.X + Coordinates_World.X - (int)(TextSize.X / 2) + (16 / 2);
                int Y__ = Player_ClosestEntity.Value.Y + Coordinates_World.Y;
                C.Fill(X__ - 1, Y__ - 1, TextSize.X + 2, TextSize.Y + 2, ColorB.White.SetA(192), ImageBlend.Alpha);
                C.Border(X__ - 2, Y__ - 2, TextSize.X + 4, TextSize.Y + 4, 1, ColorB.White);
                Font_Default.Render(C, Palette_Default, Text, X__, Y__);
            }
        }
    }

    /// <summary>
    /// Рендерит мысли игрока
    /// </summary>
    internal static void Render_Thoughts(Image.ImageContext C){
        if(Player_Character_Mute){ return; }
        
        int X = Coordinates_Player.X;
        int Y = Coordinates_Player.Y - 8 - (int)(WL.Math.SinVeryFast(World_DeltaTick) * 3);
        
        if(string.IsNullOrWhiteSpace(Player_Thought)){ return; }
        Vector2U ThoughtsSize = Font_Default.TextSize(Player_Thought);
        int X__ = (int)(X - ThoughtsSize.X / 2);
        uint Height__ = ThoughtsSize.Y + 14;
        Texture_Cloud.Render9Slice(C, Palette_Default, 10, X__, Y - (int)Height__, ThoughtsSize.X + 16, Height__);
        
        Render_TextColorOutline(C, Player_Thought, X__ + 9, Y + 8 - (int)Height__, ColorB.Black, ColorB.White);
    }
}
using WLO;
using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_World;
using static GOLUWorld.GOLUWorld_Resources;

namespace GOLUWorld;

internal static class GOLUWorld_Generator{
    internal static int Generator_Border_L => -(int)World_Size.X - 3;
    internal static int Generator_Border_R =>  (int)World_Size.X + 3;
    internal static int Generator_Border_U => -(int)World_Size.Y - 3;
    internal static int Generator_Border_D =>  (int)World_Size.Y + 3;

    /// <summary>
    /// Берёт случайный элемент с учётом весов
    /// </summary>
    /// <param name="RandomValue">от 0 до 1</param>
    internal static (T, byte) Generator_SelectWeightedObject<T>(float RandomValue, ReadOnlySpan<(T Value, byte Info, int Weight)> Variants){
        int TotalWeight = 0;

        for(int i = 0; i < Variants.Length; i++){ TotalWeight += Variants[i].Weight; }

        int Scaled = (int)(RandomValue * TotalWeight);

        if(Scaled >= TotalWeight){ Scaled = TotalWeight - 1; }

        for(int i = 0; i < Variants.Length; i++){
            if(Scaled < Variants[i].Weight){ return (Variants[i].Value, Variants[i].Info); }
            Scaled -= Variants[i].Weight;
        }

        return (Variants[^1].Value, Variants[^1].Info);
    }
    
    /// <summary>
    /// Генерирует все блоки, сущности, предметы
    /// </summary>
    internal static void Generator_DebugStructure(int X, int Y){
        T_Block [] Blocks   = Enum.GetValues<T_Block >();
        T_Entity[] Entities = Enum.GetValues<T_Entity>();
        T_Item  [] Items    = Enum.GetValues<T_Item  >();

        int TotalCount = Blocks.Length + (Entities.Count(E => E != T_Entity.Item)) + Items.Length;
        int SquareSize = (int)WL.Math.Ceil(WL.Math.Sqrt(TotalCount));
        int Index = 0;
        for(int Y__ = 0; Y__ < SquareSize; Y__++){
            for(int X__ = 0; X__ < SquareSize; X__++){
                int WorldX = X + X__ * 5;
                int WorldY = Y + Y__ * 5;

                if(Index < Blocks.Length){
                    World_SetBlock(new Block{ ID = Blocks[Index], X = WorldX, Y = WorldY });

                    Index++;
                    continue;
                }

                int EntityIndex = Index - Blocks.Length;
                T_Entity[] NonItemEntities = Entities.Where(E => E != T_Entity.Item).ToArray();
                if(EntityIndex < NonItemEntities.Length){
                    World_SetEntity(new Entity{ ID = NonItemEntities[EntityIndex], X = WorldX, Y = WorldY });

                    Index++;
                    continue;
                }

                int ItemIndex = EntityIndex - NonItemEntities.Length;
                if(ItemIndex < Items.Length){
                    World_SetEntity(new Entity{ ID = T_Entity.Item, X = WorldX, Y = WorldY, Info = (byte)Items[ItemIndex] });

                    Index++;
                    continue;
                }
                
                return;
            }   
        }
    }
    
    /// <summary>
    /// Генерирует уровень
    /// </summary>
    internal static void Generator_World(T_World World){
        void GenerateCalm(uint Seed){
            
            //__GenerateStructure(0, 0, Lake, Seed + 123233, false);
            
            Generator_Water(Seed);
            
            Generator_SandPatch(Seed);
            
            Generator_GrassLand(Seed);
            
            Generator_GrassPatch(Seed);
            
            Generator_DebugStructure(-50, -50);
        }
        
        switch(World){
            case T_World.Calm: GenerateCalm(World_Seed); break;
        }
    }
    
    /// <summary>
    /// Генерирует структуру
    /// </summary>
    internal static void Generator_Structure(int X, int Y, Structure S, uint __Seed, bool ToCenter = true){
        int OffsetX = X;
        int OffsetY = Y;
        if(ToCenter){
            OffsetX -= (int)S.Width  / 2;
            OffsetY -= (int)S.Height / 2;
        }
        if(!string.IsNullOrWhiteSpace(S.Blocks  )){ World_AddBlocksMap  (S.Blocks  , OffsetX, OffsetY, __Seed); }
        if(!string.IsNullOrWhiteSpace(S.Entities)){ World_AddEntitiesMap(S.Entities, OffsetX, OffsetY, __Seed); }
    }
    
    /// <summary>
    /// Генерирует кусок травы
    /// </summary>
    internal static void Generator_GrassPatch(uint Seed){
        Seed += 111125;
        for(int x = 0; x < 50; x++){
            Seed -= 161616;
                    
            uint SeedOffset = (uint)x;
            uint __Seed1 = Seed + SeedOffset * 222223;
            uint __Seed2 = Seed + SeedOffset * 212224;
            uint __Seed3 = Seed + SeedOffset * 124125;

            Structure __GrassBunch = Structure_GrassPatches[WL.Math.Random.Fast_Int(0, Structure_GrassPatches.Length - 1, ref __Seed3)];
                    
            Generator_Structure(WL.Math.Random.Fast_Int(Generator_Border_L, Generator_Border_R, ref __Seed1), WL.Math.Random.Fast_Int(Generator_Border_U, Generator_Border_D, ref __Seed2), __GrassBunch, Seed + SeedOffset);
        }
    }
    
    /// <summary>
    /// Генерирует кусок песка
    /// </summary>
    internal static void Generator_SandPatch(uint Seed){
        Seed += 112125;
        for(int x = 0; x < 10; x++){
            Seed -= 1616436;
                    
            uint SeedOffset = (uint)x;
            uint __Seed1 = Seed + SeedOffset * 225223;
            uint __Seed2 = Seed + SeedOffset * 21324;
            uint __Seed3 = Seed + SeedOffset * 128125;

            Structure __SandBunch = Structure_SandPatches[WL.Math.Random.Fast_Int(0, Structure_SandPatches.Length - 1, ref __Seed3)];
                    
            Generator_Structure(WL.Math.Random.Fast_Int(Generator_Border_L, Generator_Border_R, ref __Seed1), WL.Math.Random.Fast_Int(Generator_Border_U, Generator_Border_D, ref __Seed2), __SandBunch, Seed + SeedOffset);
        }
    }
    
    /// <summary>
    /// Генерирует площадь травы
    /// </summary>
    internal static void Generator_GrassLand(uint Seed){
        Seed -= 32;
        for(int y = -(int)World_Size.Y; y < World_Size.Y; y += 10){
            Seed += 511;
            for(int x = -(int)World_Size.X; x < World_Size.X; x += 10){
                Seed *= 51;
                        
                uint SeedOffset = (uint)((x + y) * (x * y));
                uint __Seed1 = Seed + SeedOffset * 222223;
                uint __Seed2 = Seed + SeedOffset * 212224;
                    
                Generator_Structure(x + WL.Math.Random.Fast_Int(-5, 5, ref __Seed1), y + WL.Math.Random.Fast_Int(-5, 5, ref __Seed2), Structure_GrassLand, Seed + SeedOffset);
            }
        }   
    }
    
    /// <summary>
    /// Генерирует водоёмы
    /// </summary>
    internal static void Generator_Water(uint Seed){
        Seed -= 1313;

        uint __Seed2 = Seed + 14;
        Vector2I StartPoint = new Vector2I(
            WL.Math.Random.Fast_Int(-(int)World_Size.X, (int)World_Size.X, ref Seed),
            WL.Math.Random.Fast_Int(-(int)World_Size.Y, (int)World_Size.Y, ref __Seed2)
        );
        
        Generator_Structure(StartPoint.X, StartPoint.Y, Structure_Lake, Seed);
        
        Generator_RiverSystem(Seed: Seed, MainStartOverride: StartPoint);
    }
    
    /// <summary>
    /// Генерирует кусок речки
    /// </summary>
    internal static void Generator_RiverLine(Vector2I StartPosition, Vector2I EndPosition, uint Width = 3, uint SandWidth = 3, uint Seed = 0){
        SandWidth += Width;

        void GenerateRiverPoint(int X, int Y, uint Width, uint SandWidth, uint Seed = 0){
            bool IsWater(int X, int Y) => World_GetBlock(X, Y).ID == T_Block.Water;
            
            for(int WX = -(int)Width / 2; WX <= Width / 2; WX++){
                for (int WY = -(int)Width / 2; WY <= Width / 2; WY++){
                    int Px = X + WX;
                    int Py = Y + WY;
                    
                    World_SetBlock(new Block{ ID = T_Block.Water, X = Px, Y = Py });
                }
            }

            uint Seed__ = Seed + (uint)(X + Y) * (Width + SandWidth);
            SandWidth = (uint)WL.Math.Random.Fast_Int(0, (int)SandWidth, ref Seed__);
            
            for(int WX = -(int)SandWidth / 2; WX <= SandWidth / 2; WX++){
                for(int WY = -(int)SandWidth / 2; WY <= SandWidth / 2; WY++){
                    int Px = X + WX;
                    int Py = Y + WY;

                    if(!IsWater(Px, Py)){
                        World_SetBlock(new Block{ ID = T_Block.Ground_Sand, X = Px, Y = Py });
                    }
                }
            }
        }
        
        int DX = EndPosition.X - StartPosition.X;
        int DY = EndPosition.Y - StartPosition.Y;

        int Steps = WL.Math.MaxI(WL.Math.AbsI(DX), WL.Math.AbsI(DY));
        if(Steps == 0){ return; }

        float StepX = (float)DX / Steps;
        float StepY = (float)DY / Steps;

        uint Seed__ = Seed + 6612;
        
        for(int i = 0; i <= Steps; i++){
            int X = (int)WL.Math.Round(StartPosition.X + StepX * i);
            int Y = (int)WL.Math.Round(StartPosition.Y + StepY * i);

            Seed__ *= 3;
            
            GenerateRiverPoint(X, Y, Width, SandWidth, Seed__);
        }
    }
    
    /// <summary>
    /// Получает точку меандра
    /// </summary>
    internal static Vector2I Generator_Meander(Vector2I StartPosition, Vector2I EndPosition, float T, int MeanderCount, float MaxAmplitude, float Compression){
        Vector2F Direction = new Vector2F(EndPosition.X - StartPosition.X, EndPosition.Y - StartPosition.Y);
        float L = WL.Math.Sqrt(Direction.X * Direction.X + Direction.Y * Direction.Y);
        if(L <= 0.001f){ return StartPosition; }

        Vector2F Norm = new Vector2F(Direction.X / L, Direction.Y / L);
        Vector2F Perp = new Vector2F(-Norm.Y, Norm.X);

        float BaseX = StartPosition.X + Direction.X * T;
        float BaseY = StartPosition.Y + Direction.Y * T;

        float BaseWave = WL.Math.Sin(T * WL.Math.PI * MeanderCount);

        float Envelope = WL.Math.Sin(T * WL.Math.PI);
        Envelope = WL.Math.Pow(Envelope, Compression);

        float Amplitude = BaseWave * MaxAmplitude * Envelope;

        return new Vector2I(
            (int)WL.Math.Round(BaseX + Perp.X * Amplitude),
            (int)WL.Math.Round(BaseY + Perp.Y * Amplitude)
        );
    }
    
    /// <summary>
    /// Генерирует меандр речку
    /// </summary>
    internal static void Generator_RiverMeander(Vector2I StartPosition, Vector2I EndPosition, uint Width = 3, uint SandWidth = 3, int MeanderCount = 6, float MaxAmplitude = 40, int SmoothSteps = 400, float Compression = 3.5f, uint Seed = 0){
        Vector2I? Previous = null;

        uint Seed__ = Seed + 222;
            
        for(int i = 0; i <= SmoothSteps; i++){
            float T = (float)i / SmoothSteps;
                
            uint CurrentWidth = (uint)(Width * T);
            uint CurrentSandWidth = (uint)(SandWidth * T);
            if (CurrentWidth == 0) CurrentWidth = 1;
            if (CurrentSandWidth < CurrentWidth) CurrentSandWidth = (uint)CurrentWidth;

            Vector2I Current = Generator_Meander(StartPosition, EndPosition, T, MeanderCount, MaxAmplitude, Compression);

            if(Previous != null){
                Generator_RiverLine(Previous.Value, Current, CurrentWidth, CurrentSandWidth, Seed__);
                Seed__ += 77778;
            }

            Previous = Current;
        }
    }
    
    /// <summary>
    /// Генерирует систему рек
    /// </summary>
    /// <returns>Конец речки</returns>
    internal static Vector2I Generator_RiverSystem(int TributaryCount = 6, int MainRiverLength = 400, uint Seed = 0, Vector2I? MainStartOverride = null){
        uint SeedMain = Seed * 1664525 + 1013904223;

        Vector2I MainStart = MainStartOverride ?? new Vector2I(
            WL.Math.Random.Fast_Int(-(int)World_Size.X, (int)World_Size.X, ref SeedMain),
            WL.Math.Random.Fast_Int(-(int)World_Size.Y, (int)World_Size.Y, ref SeedMain)
        );

        float Angle = WL.Math.Random.Fast_0_1(ref SeedMain) * WL.Math.PI * 2f;

        float DirX = WL.Math.Cos(Angle);
        float DirY = WL.Math.Sin(Angle);

        Vector2I MainEnd = new Vector2I(
            MainStart.X + (int)(DirX * MainRiverLength),
            MainStart.Y + (int)(DirY * MainRiverLength)
        );

        for(int i = 0; i <= 600; i++){
            float T = i / 600f;

            uint Width = (uint)(4 + 16 * T);
            uint Sand  = Width + 4;

            Vector2I Current = Generator_Meander(MainStart, MainEnd, T, 5, 25, 2.5f);

            if(i > 0){
                Vector2I Previous = Generator_Meander(MainStart, MainEnd, T - (1f / 600f), 5, 25, 2.5f);

                Generator_RiverLine(Previous, Current, Width, Sand, SeedMain);
            }

            SeedMain += 99991;
        }

        for(int i = 0; i < TributaryCount; i++){
            float T = WL.Math.Random.Fast_0_1(ref SeedMain);

            Vector2I JoinPoint = Generator_Meander(MainStart, MainEnd, T, 5, 25, 2.5f);

            float PerpX = -DirY;
            float PerpY = DirX;

            int Length = WL.Math.Random.Fast_Int(80, 200, ref SeedMain);

            Vector2I TributaryStart = new Vector2I(
                JoinPoint.X + (int)(PerpX * Length),
                JoinPoint.Y + (int)(PerpY * Length)
            );

            uint Width = (uint)(3 + 6 * (1 - T));
            uint Sand  = Width + 3;

            Generator_RiverMeander(TributaryStart, JoinPoint, Width, Sand, 3, 15, 250, 2.0f, SeedMain);

            SeedMain += 712367;
        }

        return MainEnd;
    }
}
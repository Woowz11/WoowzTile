using WLO;
using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_World;
using static GOLUWorld.GOLUWorld_Resources;
using static GOLUWorld.GOLUWorld_Info;
using static GOLUWorld.GOLUWorld_Utility;

namespace GOLUWorld;

internal static class GOLUWorld_Generator{
    internal static int Generator_Border_L => -(int)World_Size.X - 3;
    internal static int Generator_Border_R =>  (int)World_Size.X + 3;
    internal static int Generator_Border_U => -(int)World_Size.Y - 3;
    internal static int Generator_Border_D =>  (int)World_Size.Y + 3;
    
    /// <summary>
    /// Генерирует все блоки, сущности, предметы, потолки
    /// </summary>
    internal static void Generator_DebugStructure(int X, int Y){
        T_Block  [] Blocks   = Enum.GetValues<T_Block  >();
        T_Entity [] Entities = Enum.GetValues<T_Entity >();
        T_Item   [] Items    = Enum.GetValues<T_Item   >();
        T_Ceiling[] Ceilings = Enum.GetValues<T_Ceiling>();
        T_Money  [] Moneys   = Enum.GetValues<T_Money  >();

        int TotalCount = Blocks.Length + (Entities.Count(E => E != T_Entity.Item && E != T_Entity.Money)) + Items.Length + Ceilings.Length + Moneys.Length;
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
                T_Entity[] NonItemEntities = Entities.Where(E => E != T_Entity.Item && E != T_Entity.Money).ToArray();
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
                
                int MoneyIndex = ItemIndex - Items.Length;
                if(MoneyIndex < Moneys.Length){
                    World_SetEntity(new Entity{ ID = T_Entity.Money, X = WorldX, Y = WorldY, Info = (byte)Moneys[MoneyIndex] });

                    Index++;
                    continue;
                }
                
                int CeilingIndex = ItemIndex - Items.Length;
                if(CeilingIndex < Ceilings.Length){
                    World_SetCeiling(new Ceiling{ ID = Ceilings[CeilingIndex], X = WorldX, Y = WorldY });

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
    internal static void Generator_World(T_World World, uint Seed){
        void GenerateCalm(uint Seed){
            Generator_Water(Seed);
            
            Generator_Village(Seed);

            Generator_Pond(Seed);
            
            Generator_SandPatch(Seed);
            
            Generator_GrassLand(Seed);
            
            Generator_GrassPatch(Seed);
            
            Generator_Trash(Seed);
        }

        void GenerateIndustrial(uint Seed){
            Generator_Water(Seed);

            Generator_Village(Seed);
        }
        
        switch(World){
            case T_World.Calm      : GenerateCalm(Seed); break;
            case T_World.Industrial: GenerateIndustrial(Seed); break;
        }
    }
    
    /// <summary>
    /// Генерирует структуру
    /// </summary>
    internal static void Generator_Structure(int X, int Y, Structure S, uint Seed = 0, bool ToCenter = true, bool Replace = false, TextureRotation? Rotation = null){
        TextureRotation Rotation__ = Rotation ??= Utility_RandomRotation(Seed + 993215123 + (uint)(X * Y * Y));
        
        int OffsetX = X;
        int OffsetY = Y;
        if(ToCenter){
            OffsetX -= (int)S.Width  / 2;
            OffsetY -= (int)S.Height / 2;
        }
        if(!string.IsNullOrWhiteSpace(S.Blocks  )){ World_AddBlocksMap  (S.Blocks  , OffsetX, OffsetY, Seed + 0 , Replace, Rotation__); }
        if(!string.IsNullOrWhiteSpace(S.Entities)){ World_AddEntitiesMap(S.Entities, OffsetX, OffsetY, Seed + 99         , Rotation__); }
        if(!string.IsNullOrWhiteSpace(S.Ceilings)){ World_AddCeilingsMap(S.Ceilings, OffsetX, OffsetY, Seed + 62, Replace, Rotation__); }
    }

    /// <summary>
    /// Генерирует деревню
    /// </summary>
    internal static void Generator_Village(uint Seed){
        Seed += 9182783;

        int Total = WL.Math.Random.Fast_Int(0, 30, ref Seed);
        Seed += 6231;
        
        for(int i = 0; i < Total; i++){
            uint Seed1 = Seed + (uint)i * 2332223;
            uint Seed2 = Seed + (uint)i * 2332224;
            uint Seed3 = Seed + (uint)i * 1334125;
            
            Structure Road = Structure_Roads[WL.Math.Random.Fast_Int(0, Structure_Roads.Length - 1, ref Seed3)];
            
            Generator_Structure(WL.Math.Random.Fast_Int(Generator_Border_L, Generator_Border_R, ref Seed1), WL.Math.Random.Fast_Int(Generator_Border_U, Generator_Border_D, ref Seed2), Road, Seed + (uint)i, Replace: true);
        }
        
        Total = WL.Math.Random.Fast_Int(0, 10, ref Seed);
        Seed += 123;
        
        for(int i = 0; i < Total; i++){
            uint Seed1 = Seed + (uint)i * 232223;
            uint Seed2 = Seed + (uint)i * 232224;
            uint Seed3 = Seed + (uint)i * 134125;
            
            Structure House = Structure_Houses[WL.Math.Random.Fast_Int(0, Structure_Houses.Length - 1, ref Seed3)];
            
            Generator_Structure(WL.Math.Random.Fast_Int(Generator_Border_L, Generator_Border_R, ref Seed1), WL.Math.Random.Fast_Int(Generator_Border_U, Generator_Border_D, ref Seed2), House, Seed + (uint)i, Replace: true);
        }
    }
    
    /// <summary>
    /// Генерирует пруды
    /// </summary>
    internal static void Generator_Pond(uint Seed){
        Seed += 18844;
        
        int Total = WL.Math.Random.Fast_Int(0, 20, ref Seed);
        Seed += 12577;
        
        for(int i = 0; i < Total; i++){
            Seed -= 152676;
                    
            uint SeedOffset = (uint)i;
            uint Seed1 = Seed + SeedOffset * 22285223;
            uint Seed2 = Seed + SeedOffset * 212346224;
            uint Seed3 = Seed + SeedOffset * 12844125;

            Structure Pond = Structure_Ponds[WL.Math.Random.Fast_Int(0, Structure_Ponds.Length - 1, ref Seed3)];
                    
            Generator_Structure(WL.Math.Random.Fast_Int(Generator_Border_L, Generator_Border_R, ref Seed1), WL.Math.Random.Fast_Int(Generator_Border_U, Generator_Border_D, ref Seed2), Pond, Seed + SeedOffset);
        }
    }
    
    /// <summary>
    /// Генерирует кусок травы
    /// </summary>
    internal static void Generator_GrassPatch(uint Seed){
        Seed += 111125;
        for(int i = 0; i < 200; i++){
            Seed -= 161616;
                    
            uint SeedOffset = (uint)i;
            uint Seed1 = Seed + SeedOffset * 222223;
            uint Seed2 = Seed + SeedOffset * 212224;
            uint Seed3 = Seed + SeedOffset * 124125;

            Structure GrassBunch = Structure_GrassPatches[WL.Math.Random.Fast_Int(0, Structure_GrassPatches.Length - 1, ref Seed3)];
                    
            Generator_Structure(WL.Math.Random.Fast_Int(Generator_Border_L, Generator_Border_R, ref Seed1), WL.Math.Random.Fast_Int(Generator_Border_U, Generator_Border_D, ref Seed2), GrassBunch, Seed + SeedOffset);
        }
    }
    
    /// <summary>
    /// Генерирует кусок песка
    /// </summary>
    internal static void Generator_SandPatch(uint Seed){
        Seed += 112125;
        for(int x = 0; x < 20; x++){
            Seed -= 1616436;
                    
            uint SeedOffset = (uint)x;
            uint Seed1 = Seed + SeedOffset * 225223;
            uint Seed2 = Seed + SeedOffset * 21324;
            uint Seed3 = Seed + SeedOffset * 128125;

            Structure SandBunch = Structure_SandPatches[WL.Math.Random.Fast_Int(0, Structure_SandPatches.Length - 1, ref Seed3)];
                    
            Generator_Structure(WL.Math.Random.Fast_Int(Generator_Border_L, Generator_Border_R, ref Seed1), WL.Math.Random.Fast_Int(Generator_Border_U, Generator_Border_D, ref Seed2), SandBunch, Seed + SeedOffset);
        }
    }
    
    /// <summary>
    /// Генерирует мусор
    /// </summary>
    internal static void Generator_Trash(uint Seed){
        Seed += 95694;
        
        int Total = WL.Math.Random.Fast_Int(100, 500, ref Seed);
        Seed += 1256;
        
        uint Seed1 = 0;
        uint Seed2 = 0;
        
        for(int i = 0; i < Total; i++){
            Seed -= 1976;
                    
            uint SeedOffset = (uint)i;
                 Seed1 = Seed + SeedOffset * 34678223;
                 Seed2 = Seed + SeedOffset * 21964724;
            uint Seed3 = Seed + SeedOffset * 1223512125;

            Generator_Structure(WL.Math.Random.Fast_Int(Generator_Border_L, Generator_Border_R, ref Seed1), WL.Math.Random.Fast_Int(Generator_Border_U, Generator_Border_D, ref Seed2), Utility_SelectWeightedObject<Structure>(WL.Math.Random.Fast_0_1(ref Seed3), Structure_Trash).Item1, Seed + SeedOffset);
        }
        
        for(int i = 0; i < 1000; i++){
            Seed1 += (uint)i;
            Seed2 += (uint)i;
            World_AddDecal(new Decal{ ID = Info_Decal_RandomTrash(), X = WL.Math.Random.Fast_Int(-(int)World_SizeWorld.X, (int)World_SizeWorld.X, ref Seed1), Y = WL.Math.Random.Fast_Int(-(int)World_SizeWorld.Y, (int)World_SizeWorld.Y, ref Seed2) }, RandomRotation: true);
        }
    }
    
    /// <summary>
    /// Генерирует площадь травы
    /// </summary>
    internal static void Generator_GrassLand(uint Seed){
        Seed -= 32;
        for(int y = -(int)World_Size.Y; y < World_Size.Y + 10; y += 10){
            Seed += 511;
            for(int x = -(int)World_Size.X; x < World_Size.X + 10; x += 10){
                Seed *= 51;
                        
                uint SeedOffset = (uint)((x + y) * (x * y));
                uint Seed1 = Seed + SeedOffset * 222223;
                uint Seed2 = Seed + SeedOffset * 212224;
                    
                Generator_Structure(x + WL.Math.Random.Fast_Int(-20, 20, ref Seed1), y + WL.Math.Random.Fast_Int(-20, 20, ref Seed2), Structure_GrassLand, Seed + SeedOffset);
            }
        }   
    }
    
    /// <summary>
    /// Генерирует водоёмы
    /// </summary>
    internal static void Generator_Water(uint Seed){
        Seed -= 1313;

        if(WL.Math.Random.Fast_Bool(ref Seed)){
            Seed += 881261274;
            Structure Rivers = Structure_Rivers[WL.Math.Random.Fast_Int(0, Structure_Rivers.Length - 1, ref Seed)];

            Generator_Structure(0, 0, Rivers);
        }
        
        uint Seed1 = Seed + 177238;
        uint Seed2 = Seed1 + 272371;
        Generator_Structure(WL.Math.Random.Fast_Int(Generator_Border_L, Generator_Border_R, ref Seed1), WL.Math.Random.Fast_Int(Generator_Border_U, Generator_Border_D, ref Seed2), Structure_Lakes[WL.Math.Random.Fast_Int(0, Structure_Lakes.Length - 1, ref Seed2)], Seed);
    }
}
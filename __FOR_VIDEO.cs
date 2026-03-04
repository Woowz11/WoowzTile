using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_World;
using static GOLUWorld.GOLUWorld_Resources;
using static GOLUWorld.GOLUWorld_Info;
using static GOLUWorld.GOLUWorld_Utility;
using static GOLUWorld.GOLUWorld_UI;

public static class __FOR_VIDEO{
    public static void FUNC1(int StartX, int StartY, int Width, int Height, uint Seed){
        if(Width % 2 == 0) Width++;
        if(Height % 2 == 0) Height++;

        bool[,] Grid = new bool[Width, Height]; // true = проход
        bool[,] Visited = new bool[Width, Height];

        int[] DX = { 0, 0, -2, 2 };
        int[] DY = { -2, 2, 0, 0 };

        void Shuffle(int[] arr, ref uint seed){
            for(int i = arr.Length - 1; i > 0; i--){
                int j = WL.Math.Random.Fast_Int(0, i, ref seed);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }

        void Carve(int x, int y){
            Visited[x, y] = true;
            Grid[x, y] = true;

            int[] dirs = {0,1,2,3};
            Shuffle(dirs, ref Seed);

            foreach(int dir in dirs){
                int nx = x + DX[dir];
                int ny = y + DY[dir];

                if(nx <= 0 || ny <= 0 || nx >= Width - 1 || ny >= Height - 1)
                    continue;
                if(Visited[nx, ny])
                    continue;

                Grid[x + DX[dir]/2, y + DY[dir]/2] = true;
                Carve(nx, ny);
            }
        }

        Carve(1,1);

        // === добавляем аномалии ===
        int breaks = WL.Math.Random.Fast_Int(5, 20, ref Seed);
        for(int i = 0; i < breaks; i++){
            int bx = WL.Math.Random.Fast_Int(1, Width-2, ref Seed);
            int by = WL.Math.Random.Fast_Int(1, Height-2, ref Seed);
            Grid[bx, by] = true;
        }

        int rooms = WL.Math.Random.Fast_Int(1, 5, ref Seed);
        for(int i = 0; i < rooms; i++){
            int cx = WL.Math.Random.Fast_Int(2, Width-3, ref Seed);
            int cy = WL.Math.Random.Fast_Int(2, Height-3, ref Seed);
            for(int ry=-1; ry<=1; ry++)
                for(int rx=-1; rx<=1; rx++)
                    Grid[cx+rx, cy+ry] = true;
        }

        int voids = WL.Math.Random.Fast_Int(5, 15, ref Seed);
        for(int i = 0; i < voids; i++){
            int vx = WL.Math.Random.Fast_Int(1, Width-2, ref Seed);
            int vy = WL.Math.Random.Fast_Int(1, Height-2, ref Seed);
            Grid[vx, vy] = false;
        }

        // === ставим блоки напрямую ===
        for(int y=0; y<Height; y++){
            for(int x=0; x<Width; x++){
                int FX = StartX + x;
                int FY = StartY + y;

                T_Block blockID = Grid[x,y] ? T_Block.Ground_Grass : T_Block.Bricks;
                World_SetBlock(new Block{ X = FX, Y = FY, ID = blockID }, Replace: true);
            }
        }
    }
    
    internal static void FUNC2(int StartX, int StartY, int Width, int Height, uint Seed){
        if(Width % 2 == 0) Width++;
        if(Height % 2 == 0) Height++;

        bool[,] Grid = new bool[Width, Height]; // true = проход
        bool[,] Visited = new bool[Width, Height];

        int[] DX = { 0, 0, -2, 2 };
        int[] DY = { -2, 2, 0, 0 };

        void Shuffle(int[] arr, ref uint seed){
            for(int i = arr.Length - 1; i > 0; i--){
                int j = WL.Math.Random.Fast_Int(0, i, ref seed);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }

        void Carve(int x, int y){
            Visited[x, y] = true;
            Grid[x, y] = true;

            int[] dirs = {0,1,2,3};
            Shuffle(dirs, ref Seed);

            foreach(int dir in dirs){
                int nx = x + DX[dir];
                int ny = y + DY[dir];

                if(nx <= 0 || ny <= 0 || nx >= Width-1 || ny >= Height-1)
                    continue;
                if(Visited[nx, ny])
                    continue;

                Grid[x + DX[dir]/2, y + DY[dir]/2] = true;
                Carve(nx, ny);
            }
        }

        Carve(1,1);

        // === добавляем хаос ===
        int breaks = WL.Math.Random.Fast_Int(10, 40, ref Seed);
        for(int i = 0; i < breaks; i++){
            int bx = WL.Math.Random.Fast_Int(1, Width-2, ref Seed);
            int by = WL.Math.Random.Fast_Int(1, Height-2, ref Seed);
            Grid[bx, by] = WL.Math.Random.Fast_Bool(ref Seed);
        }

        int rooms = WL.Math.Random.Fast_Int(2, 8, ref Seed);
        for(int i = 0; i < rooms; i++){
            int cx = WL.Math.Random.Fast_Int(2, Width-3, ref Seed);
            int cy = WL.Math.Random.Fast_Int(2, Height-3, ref Seed);

            for(int ry=-1; ry<=1; ry++)
                for(int rx=-1; rx<=1; rx++){
                    Grid[cx+rx, cy+ry] = WL.Math.Random.Fast_Bool(ref Seed);
                }
        }

        // === случайные глитчи ===
        for(int i = 0; i < 30; i++){
            int gx = WL.Math.Random.Fast_Int(1, Width-2, ref Seed);
            int gy = WL.Math.Random.Fast_Int(1, Height-2, ref Seed);
            Grid[gx, gy] = WL.Math.Random.Fast_Bool(ref Seed);
        }

        // === ставим блоки напрямую ===
        for(int y=0; y<Height; y++){
            for(int x=0; x<Width; x++){
                int FX = StartX + x;
                int FY = StartY + y;

                T_Block blockID;

                if(!Grid[x,y]){
                    // стена
                    int wallType = WL.Math.Random.Fast_Int(0, 5, ref Seed);
                    blockID = wallType switch{
                        0 => T_Block.Bricks,
                        1 => T_Block.Concrete,
                        2 => T_Block.Metal,
                        3 => T_Block.Black,
                        4 => T_Block.Ground_Tiles,
                        _ => T_Block.Ground_Planks
                    };
                } else {
                    // проход
                    int floorType = WL.Math.Random.Fast_Int(0, 6, ref Seed);
                    blockID = floorType switch{
                        0 => T_Block.Ground_Grass,
                        1 => T_Block.Ground_Sand,
                        2 => T_Block.Ground_Cobblestone,
                        3 => T_Block.Ground_Planks,
                        4 => T_Block.Ground_Tiles,
                        5 => T_Block.Water,
                        _ => T_Block.Ground_Grass
                    };
                }

                World_SetBlock(new Block{ X = FX, Y = FY, ID = blockID }, Replace: true);
            }
        }
    }
}
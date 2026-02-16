using WL;
using WLO;
using WoowzTile;
using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_Player;
using static GOLUWorld.GOLUWorld_Info;
using static GOLUWorld.GOLUWorld_Generator;
using static GOLUWorld.GOLUWorld_Resources;

namespace GOLUWorld;

internal static class GOLUWorld_World{
    internal static void StartLevel(T_Level Level){
        ClearAllEntityScene();
        ClearAllScene();

        __Decals.Clear();

        UI_Interface = T_Interface.None;
        
        Coordinates_Camera = Vector2F.Zero;
        
        World_DeltaTick = 0;

        if(Level == T_Level.Calm){
            GenerateLevel(Level);
        }
    }

    /// <summary>
    /// Генерирует случайный сид
    /// </summary>
    internal static uint World_GenerateNewSeed() => (uint)WL.Math.Random.Fast_Int(0, 10000000);
    
    internal static void Game_Update(TickData TD){
        Game.ClearColliders();

        if(UI_InMainMenu){
            return;
        }

        World_StopGameTime = UI_Interface != 0;

        if(World_StopGameTime){ return; }

        Player_OutBounds = Coordinates_Player.X - Coordinates_World.X < -World_BlocksSize.X || Coordinates_Player.X - Coordinates_World.X > World_BlocksSize.X || Coordinates_Player.Y - Coordinates_World.Y < -World_BlocksSize.Y || Coordinates_Player.Y - Coordinates_World.Y > World_BlocksSize.Y;

        World_Time += (float)TD.DeltaTimeS * World_TimeSpeed;
        if(World_Time > World_TimeMax){ World_Time = 0; }
        
        if(Cheat_Immortality){ if(Player_Health < 1){ Player_Health = 1; } }
        
        if(!Player_Dead){
            if(Player_OutBounds){
                Damage(WL.Math.Random.Fast_Bool(0.05f) ? (uint)WL.Math.Random.Fast_Int(1, 10) : 0);
            }else{
                Heal((uint)(WL.Math.Random.Fast_Bool(0.001f) ? 1 : 0));   
            }
            
            EmotionChange(T_Emotion.Happiness, WL.Math.Random.Fast_Bool(0.01f) ? 1 : 0);
            
            if(WL.Math.Random.Fast_Bool(0.001f)){ SayThoughts(T_Thoughts.Idle); }
        }else{
            UI_Interface = 0;
        }

        if(Player_ThoughtTimer < 0 || Player_Dead){ Player_Thought = ""; Player_ThoughtContext = T_Thoughts.Idle; }else{ Player_ThoughtTimer -= (float)TD.DeltaTimeS; }
        
        foreach(Block Block in __Blocks.Values){
            if(Block.ID is T_Block.Metal or T_Block.Bricks or T_Block.Water or T_Block.Black or T_Block.Error){
                Game.AddCollider(new Collider(Coordinates_World.X + Block.X, Coordinates_World.Y + Block.Y, 16, 16));
            }
        }

        foreach(KeyValuePair<EntityKey, Entity> KVP in __Entities){
            Entity Entity = KVP.Value;
            
            if(Entity.ID is T_Entity.Table or T_Entity.Spikes or T_Entity.Mob_Spider or T_Entity.Tree){
                if(Entity.ID == T_Entity.Mob_Spider){
                    int SpiderSpeed = WL.Math.Random.Fast_Bool(0.8f) ? 1 : 0;
                    
                    byte Info = Entity.Info;
                    if(WL.Math.Random.Fast_Bool(Info == 1 ? 0.5f : 0.05f)){
                        if(WL.Math.Random.Fast_Bool(0.05f)){
                            Info = 2;
                        }else{
                            Info = (byte)(Info == 1 ? 0 : 1);
                        }
                    }

                    int PlayerX__ = Coordinates_Player.X - Coordinates_World.X;
                    int PlayerY__ = Coordinates_Player.Y - Coordinates_World.Y;

                    float Distance = Vector2I.Distance(new Vector2I(Entity.X, Entity.Y), new Vector2I(PlayerX__, PlayerY__));

                    Vector2I MoveDirection = Vector2I.Zero;
                    
                    Vector2I Target = Entity.InfoVector;
                    Vector2I EntityPositionOriginal = new Vector2I(Entity.X, Entity.Y);
                    
                    if(Distance < 100 && Player_Rotting < 10){

                        Target.X = Info is 1 or 2 ? Coordinates_World.X - Coordinates_Player.X : PlayerX__;
                        Target.Y = Info is 1 or 2 ? Coordinates_World.Y - Coordinates_Player.Y : PlayerY__;

                        MoveDirection.X = WL.Math.Sign(Target.X - Entity.X) * SpiderSpeed;
                        MoveDirection.Y = WL.Math.Sign(Target.Y - Entity.Y) * SpiderSpeed;
                        
                        Entity.X += MoveDirection.X;
                        Entity.Y += MoveDirection.Y;
                        Entity.Info = Info;
                        
                    }else{
                        if(WL.Math.Random.Fast_Bool(0.05f) || Target == Vector2I.Zero){
                            Target = new Vector2I(WL.Math.Random.Fast_Int(-1000, 1000), WL.Math.Random.Fast_Int(-1000, 1000));
                        }
                        
                        MoveDirection.X = WL.Math.Sign(Target.X - Entity.X) * SpiderSpeed;
                        MoveDirection.Y = WL.Math.Sign(Target.Y - Entity.Y) * SpiderSpeed;

                        Entity.X += MoveDirection.X;
                        Entity.Y += MoveDirection.Y;
                        Entity.Info = Info;
                        Entity.InfoVector = Target;
                    }

                    if(MoveDirection != Vector2I.Zero){
                        int DirectionX = 0;
                        int DirectionY = 0;

                        float DX = Target.X - EntityPositionOriginal.X;
                        float DY = Target.Y - EntityPositionOriginal.Y;

                        int __AbsX = (int)WL.Math.Abs(DX);
                        int __AbsY = (int)WL.Math.Abs(DY);
                        if(__AbsX > __AbsY){
                            DirectionX = WL.Math.Sign(DX);
                        }else if(__AbsX < __AbsY){
                            DirectionY = WL.Math.Sign(DY);
                        }
                        
                        Entity.Rotation = DirectionX == 1 ? TextureRotation.Rotate270 : (DirectionX == -1 ? TextureRotation.Rotate90 : (DirectionY == -1 ? TextureRotation.Rotate180 : TextureRotation.None));
                    }
                    __Entities[KVP.Key] = Entity;
                }
                
                uint SizeX = 16;
                uint SizeY = 16;
                if(Entity.ID is T_Entity.Table or T_Entity.Tree){
                    SizeX = SizeY = 4;
                }

                CollisionLayer Layer = CollisionLayer.L1;
                if(Entity.ID == T_Entity.Spikes){
                    Layer = CollisionLayer.L2;
                }else if(Entity.ID == T_Entity.Mob_Spider){
                    Layer = CollisionLayer.L3;
                }
                Game.AddCollider(new Collider(Coordinates_World.X + Entity.X + (int)((16 - SizeX)/2), Coordinates_World.Y + Entity.Y + (int)((16 - SizeY)/2), SizeX, SizeY, 0, KVP.Key.Position, (int)KVP.Key.UniqueID, Layer));
            }

            if(Entity.ID is T_Entity.Item){
                Game.AddCollider(new Collider(Coordinates_World.X + Entity.X, Coordinates_World.Y + Entity.Y, 16, 16, Entity.Info, KVP.Key.Position, (int)KVP.Key.UniqueID, CollisionLayer.L4));
            }
            
            if(Entity.ID is T_Entity.Crate){
                Game.AddCollider(new Collider(Coordinates_World.X + Entity.X + 2, Coordinates_World.Y + Entity.Y + 2, 12, 12, 0, KVP.Key.Position, (int)KVP.Key.UniqueID, CollisionLayer.L5));
            }
        }

        bool CanMove = !Player_Dead;

        if(Player_Dead){
            if(WL.Math.Random.Fast_Bool(0.8f)){
                __Decals.Add((Coordinates_Player.X - Coordinates_World.X + WL.Math.Random.Fast_Int(-128, 128), Coordinates_Player.Y - Coordinates_World.Y + WL.Math.Random.Fast_Int(-128, 128), WL.Math.Random.Fast_Bool() ? T_Decal.One : T_Decal.Zero, TextureRotation.None));
            }

            Player_Rotting += (float)TD.DeltaTimeS;
        }
        
        uint PlayerSize = (uint)(Texture_Player_Body.Width * 0.8f);
        int PlayerOffset = (int)((Texture_Player_Body.Width - PlayerSize) / 2);
        
        if(CanMove){
            uint PlayerSpeed = (uint)(WL.Math.Max(1, (float)TD.DeltaTimeS * 100 * (Game.KeyPressed(Key.Shift) ? 1.5f : (Game.KeyPressed(Key.Control) ? 0.3f : 1))));
            if(Player_Health < Player_HealthLow){ PlayerSpeed = (uint)(PlayerSpeed / 2); }

            bool D = Game.KeyPressed(Key.D);
            bool A = Game.KeyPressed(Key.A);
            bool W = Game.KeyPressed(Key.W);
            bool S = Game.KeyPressed(Key.S);
            Player_MovingDirection = new Vector2I(A && D ? 0 : (A ? 1 : (D ? -1 : 0)), W && S ? 0 : (W ? 1 : (S ? -1 : 0)));

            Vector2F DesiredMove = new Vector2F();

            CollisionLayer WallMask = Cheat_IgnoreColliders ? CollisionLayer.None : CollisionLayer.L1 | CollisionLayer.L5;
            if(Player_MovingDirection.X != 0 && Player_MovingDirection.Y != 0){
                for(uint i = 1; i <= PlayerSpeed; i++){
                    int TestX = (int)(Coordinates_Player.X - Player_MovingDirection.X * i + PlayerOffset);
                    int TestY = (int)(Coordinates_Player.Y - Player_MovingDirection.Y * i + PlayerOffset);

                    Collider TestCollider = new Collider(TestX, TestY, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, WallMask);

                    if(!Game.Collision(TestCollider, out Collider? _)){
                        DesiredMove.X = Player_MovingDirection.X * i;
                        DesiredMove.Y = Player_MovingDirection.Y * i;
                    }else{
                        TestCollider.X = TestX;
                        TestCollider.Y = Coordinates_Player.Y + PlayerOffset;
                        if(!Game.Collision(TestCollider, out Collider? _)){
                            DesiredMove.X = Player_MovingDirection.X * i;
                            DesiredMove.Y = 0;
                        }else{
                            TestCollider.X = Coordinates_Player.X + PlayerOffset;
                            TestCollider.Y = TestY;
                            if(!Game.Collision(TestCollider, out Collider? _)){
                                DesiredMove.X = 0;
                                DesiredMove.Y = Player_MovingDirection.Y * i;
                            }else{
                                break;
                            }
                        }

                        break;
                    }
                }
            }
            else{
                for(uint i = 1; i < PlayerSpeed + 1; i++){
                    if(!Game.Collision(new Collider((int)(Coordinates_Player.X - (Player_MovingDirection.X * i) + PlayerOffset), Coordinates_Player.Y + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, WallMask), out Collider? _)){
                        DesiredMove.X = Player_MovingDirection.X * i;
                    }
                    else{
                        break;
                    }
                }

                for(uint i = 1; i < PlayerSpeed + 1; i++){
                    if(!Game.Collision(new Collider(Coordinates_Player.X + PlayerOffset, (int)(Coordinates_Player.Y - (Player_MovingDirection.Y * i) + PlayerOffset), PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, WallMask), out Collider? _)){
                        DesiredMove.Y = Player_MovingDirection.Y * i;
                    }
                    else{
                        break;
                    }
                }
            }

            Coordinates_Camera += DesiredMove;

            if(DesiredMove.X != 0 || DesiredMove.Y != 0){
                Track();

                if(Game.Collision(new Collider(Coordinates_Player.X + PlayerOffset, Coordinates_Player.Y + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.L2), out Collider? _)){
                    if(WL.Math.Random.Fast_Bool(0.5f)){
                        Damage((uint)(WL.Math.Random.Fast_0_1() * 5));
                    }
                }

                if(Game.Collision(new Collider((int)(Coordinates_Player.X + PlayerOffset - DesiredMove.X * 2), (int)(Coordinates_Player.Y + PlayerOffset - DesiredMove.Y * 2), PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.L5), out Collider? __PushedCollider)){
                    Vector2I PushedEntityIndex1 = __PushedCollider!.Value.Info2;
                    int PushedEntityIndex2 = __PushedCollider!.Value.Info3;
                    EntityKey Key = new EntityKey(PushedEntityIndex1, (uint)PushedEntityIndex2);
                    Entity PushedEntity = __Entities[Key];

                    int __X = (DesiredMove.X == 0 ? 0 : WL.Math.Sign(DesiredMove.X));
                    int __Y = (DesiredMove.Y == 0 ? 0 : WL.Math.Sign(DesiredMove.Y));
                    
                    int NewX = PushedEntity.X - __X;
                    int NewY = PushedEntity.Y - __Y;
                    
                    if(!Game.Collision(new Collider(Coordinates_World.X + NewX - __X * 2 + 2, Coordinates_World.Y + NewY - __Y * 2 + 2, 12, 12, 0, PushedEntityIndex1, 0, CollisionLayer.L5, WallMask), out Collider? _, true)){
                        PushedEntity.X = NewX;
                        PushedEntity.Y = NewY;
                        __Entities[Key] = PushedEntity;
                    }
                }
            }
        }
        
        if(Game.Collision(new Collider(Coordinates_Player.X + PlayerOffset, Coordinates_Player.Y + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.L3), out Collider? _)){
            if(WL.Math.Random.Fast_Bool(0.8f)){
                Damage((uint)(WL.Math.Random.Fast_0_1() * 20), Player_Dead ? 16 : 0);
            }
        }
        
        if(Game.Collision(new Collider(Coordinates_Player.X + PlayerOffset, Coordinates_Player.Y + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.All), out Collider? Collider__)){
            Player_InteractingCollision = Collider__!.Value.Layer;
            Player_CollisionInfo1  = Collider__!.Value.Info1;
            Player_CollisionInfo2  = Collider__!.Value.Info2;
            Player_CollisionInfo3  = Collider__!.Value.Info3;
        }else{
            Player_InteractingCollision = CollisionLayer.None;
            Player_CollisionInfo1  = 0;
            Player_CollisionInfo2  = Vector2I.Zero;
            Player_CollisionInfo3  = 0;
        }
    }
     
     internal static readonly List<(int, int, T_Decal, TextureRotation)> __Decals = [];
    internal static void Track(){
        if(WL.Math.Random.Fast_Bool(0.1f)){
            if(Player_Health < Player_HealthLow){
                SplatBlood(Coordinates_Player.X - Coordinates_World.X, Coordinates_Player.Y - Coordinates_World.Y);
            }else{
                __Decals.Add((Coordinates_Player.X - Coordinates_World.X + WL.Math.Random.Fast_Int(-5, 5), Coordinates_Player.Y - Coordinates_World.Y  + WL.Math.Random.Fast_Int(-5, 5), T_Decal.Track, TextureRotation.None));
            }
        }
    }

    internal static void SplatBlood(int X, int Y){
        __Decals.Add((X, Y, T_Decal.Blood, WL.Math.Random.Fast_Bool(0.5f) ? (WL.Math.Random.Fast_Bool(0.5f) ? TextureRotation.None :  TextureRotation.Rotate90) : (WL.Math.Random.Fast_Bool(0.5f) ? TextureRotation.Rotate180 : TextureRotation.Rotate270)));
    }

    internal static void SetBlock(Block Block__, bool SnapToGrid = true, bool IgnoreEntities = false, bool Replace = true){
        if(SnapToGrid){
            Block__.X *= 16;
            Block__.Y *= 16;
        }

        Vector2I Key = new Vector2I(Block__.X, Block__.Y);
        if(__Blocks.ContainsKey(Key)){
            if(Block__.ID == T_Block.Empty){
                __Blocks.Remove(Key);
            }else{
                if(Replace){
                    Block OldBlock = __Blocks[Key];
                    if(OldBlock.ID != Block__.ID){
                        __Blocks[Key] = Block__;
                    }
                }
            }
        }else{
            if(Block__.ID != T_Block.Empty){
                __Blocks[Key] = Block__;
            }
        }

        if(!IgnoreEntities && Info_Block_Solid(Block__.ID)){
            if(__Entities.ContainsKey(new EntityKey(Key))){ SetEntity(new Entity{ X = Block__.X, Y = Block__.Y }, false, true); }
        }
    }
    internal static readonly Dictionary<Vector2I, Block> __Blocks = [];

    internal static Block GetBlock(int X, int Y, bool SnapToGrid = true){
        if(SnapToGrid){
            X *= 16;
            Y *= 16;
        }
        return __Blocks.GetValueOrDefault(new Vector2I(X, Y), new Block{ ID = T_Block.Empty, X = X, Y = Y });
    }

    internal static void ClearAllScene(){
        __Blocks.Clear();
    }

    internal static void AddScene(string SceneMap, int X = 0, int Y = 0, uint __Seed = 0, bool Replace = false){
        try{
            if(string.IsNullOrEmpty(SceneMap)){ return; }
            
            int X__ = X;
            int Y__ = Y;
            
            uint Seed1__ = __Seed + 1222;
            uint Seed2__ = __Seed + 6848;
            
            foreach(char C in SceneMap){
                T_Block ID;
                switch(C){
                    case '\r': 
                        continue;
                    case '.':
                        X__++;
                        continue;
                    case '\n':
                        Y__++;
                        X__ = X;
                        continue;
                    case '#':
                        ID = T_Block.Metal;
                        break;
                    case '\'':
                        ID = T_Block.Ground_Planks;
                        break;
                    case 'A':
                        ID = T_Block.Ground_Asphalt;
                        break;
                    case 'B':
                        ID = T_Block.Bricks;
                        break;
                    case 'S':
                        ID = T_Block.Ground_Sand;
                        break;
                    case 'W':
                        ID = T_Block.Water;
                        break;
                    case 'b':
                        ID = T_Block.Black;
                        break;
                    case '^':
                        ID = T_Block.Ground_Grass;
                        break;
                    case 'Д':
                        Seed1__ += 121;
                        ID = WL.Math.Random.Fast_Bool(0.5f, ref Seed1__) ? T_Block.Ground_Grass  : T_Block.Empty;
                        break;
                    default:
                        ID = T_Block.Error;
                        break;
                }

                if(ID != T_Block.Empty){
                    SetBlock(new Block{ X = X__, Y = Y__, ID = ID}, Replace: Replace);
                }
                
                X__++;
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при загрузке сцены!", e);
        }
    }

    internal static void SetEntity(Entity Entity__, bool SnapToGrid = true, bool IgnoreBlocks = false, bool Replace = true){
        if(SnapToGrid){
            Entity__.X *= 16;
            Entity__.Y *= 16;
        }

        bool HasUniqueID = Entity__.ID is T_Entity.Crate or T_Entity.Item or T_Entity.Mob_Spider;
        
        EntityKey Key = new EntityKey(new Vector2I(Entity__.X, Entity__.Y), HasUniqueID);
        
        if(!IgnoreBlocks){
            if(__Blocks.TryGetValue(Key.Position, out Block __Found) && Info_Block_Solid(__Found.ID)){ return; }
        }

        if(Entity__.ID == T_Entity.Item && Entity__.Info == (byte)T_Item.Empty){
            Entity__.Info = (byte)T_Item.FirstAidKit;
        }
        
        if(__Entities.ContainsKey(Key)){
            if(Entity__.ID == T_Entity.Empty){
                __Entities.Remove(Key);
            }
            else{
                Entity OldEntity = __Entities[Key];
                if(OldEntity.ID != Entity__.ID){
                    __Entities[Key] = Entity__;
                }
            }
        }else{
            if(Entity__.ID != T_Entity.Empty){
                __Entities[Key] = Entity__;
            }
        }
    }
    internal static readonly Dictionary<EntityKey, Entity> __Entities = [];

    internal static void ClearAllEntityScene(){
        __Entities.Clear();
    }

    internal static void AddEntityScene(string SceneMap, int X = 0, int Y = 0, uint __Seed = 0, bool Replace = false){
        try{
            if(string.IsNullOrEmpty(SceneMap)){ return; }
            
            int X__ = X;
            int Y__ = Y;

            uint Seed1__ = __Seed;
            uint Seed2__ = __Seed + 999696;
            uint Seed3__ = __Seed + 993;
            
            foreach(char C in SceneMap){
                T_Entity ID;
                switch(C){
                    case '\r': 
                        continue;
                    case '.':
                        X__++;
                        continue;
                    case '\n':
                        Y__++;
                        X__ = X;
                        continue;
                    case 'C':
                        ID = T_Entity.Chair;
                        break;
                    case 'T':
                        ID = T_Entity.Table;
                        break;
                    case '^':
                        ID = T_Entity.Spikes;
                        break;
                    case 's':
                        ID = T_Entity.Mob_Spider;
                        break;
                    case '!':
                        ID = T_Entity.Tree;
                        break;
                    case '#':
                        ID = T_Entity.Crate;
                        break;
                    case '~':
                        ID = T_Entity.Grass;
                        break;
                    case '3':
                        ID = T_Entity.Bush;
                        break;
                    case 'Д':
                        Seed1__ += 1667;
                        Seed2__ += 551;
                        if(GetBlock(X__, Y__).ID == T_Block.Ground_Sand){
                            ID = WL.Math.Random.Fast_Bool(0.01f, ref Seed1__) ? T_Entity.Grass : T_Entity.Empty;
                        }else{
                            ID = WL.Math.Random.Fast_Bool(0.2f, ref Seed1__) ? T_Entity.Tree : (WL.Math.Random.Fast_Bool(0.4f, ref Seed2__) ? (WL.Math.Random.Fast_Bool(0.1f, ref Seed3__) ? T_Entity.Bush : T_Entity.Grass) : T_Entity.Empty);
                        }
                        break;
                    case 'д':
                        Seed1__ += 1532;
                        if(GetBlock(X__, Y__).ID == T_Block.Ground_Sand){
                            ID = WL.Math.Random.Fast_Bool(0.01f, ref Seed1__) ? T_Entity.Grass : T_Entity.Empty;
                        }else{
                            ID = WL.Math.Random.Fast_Bool(0.4f, ref Seed1__) ? (WL.Math.Random.Fast_Bool(0.1f, ref Seed1__) ? T_Entity.Bush : T_Entity.Grass) : T_Entity.Empty;
                        }
                        break;
                    default:
                        ID = T_Entity.Error;
                        break;
                }

                if(ID != T_Entity.Empty){
                    SetEntity(new Entity{ X = X__, Y = Y__, ID = ID}, Replace: Replace);
                }

                X__++;
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при загрузке Entity сцены!", e);
        }
    }

    internal static void SpawnItem(int X, int Y, T_Item Item){
        SetEntity(new Entity{ X = X, Y = Y, ID = T_Entity.Item, Info = (byte)Item}, false);
    }
}
using WL;
using WLO;
using WoowzTile;
using WoowzTile.Objects;
using static GOLUWorld.GW_Values;
using static GOLUWorld.GW_Objects;
using static GOLUWorld.GW_Player;
using static GOLUWorld.GW_Info;
using static GOLUWorld.GW_Generator;
using static GOLUWorld.GW_Resources;

namespace GOLUWorld;

internal static class GW_World{
    internal static void StartLevel(T_Level Level){
        ClearAllEntityScene();
        ClearAllScene();

        __Decals.Clear();

        Interface = T_Interface.None;
        
        WorldPosition = Vector2F.Zero;
        
        WorldDeltaTick = 0;

        if(Level == T_Level.Calm){
            GenerateLevel(Level);
        }
    }

     internal static void Game_Update(TickData TD){
        Game.ClearColliders();

        if(InMainMenu){
            return;
        }

        StopTime = Interface != 0;

        if(StopTime){ return; }

        OutsideLevel = PlayerX - WorldX < -LevelSizeTile.X || PlayerX - WorldX > LevelSizeTile.X || PlayerY - WorldY < -LevelSizeTile.Y || PlayerY - WorldY > LevelSizeTile.Y;

        Time += (float)TD.DeltaTimeS * TimeSpeed;
        if(Time > MaxTime){ Time = 0; }
        
        if(Immortality){ if(Health < 1){ Health = 1; } }
        
        if(!Dead){
            if(OutsideLevel){
                Damage(WL.Math.Random.Fast_Bool(0.05f) ? (uint)WL.Math.Random.Fast_Int(1, 10) : 0);
            }else{
                Heal((uint)(WL.Math.Random.Fast_Bool(0.001f) ? 1 : 0));   
            }
            
            EmotionChange(T_Emotion.Happiness, WL.Math.Random.Fast_Bool(0.01f) ? 1 : 0);
            
            if(WL.Math.Random.Fast_Bool(0.001f)){ SayThoughts(T_Thoughts.Idle); }
        }else{
            Interface = 0;
        }

        if(ThoughtsTimer < 0 || Dead){ Thoughts = ""; ThoughtsContext = T_Thoughts.Idle; }else{ ThoughtsTimer -= (float)TD.DeltaTimeS; }
        
        foreach(Block Block in __Blocks.Values){
            if(Block.ID is T_Block.Metal or T_Block.Bricks or T_Block.Water or T_Block.Black or T_Block.Error){
                Game.AddCollider(new Collider(WorldX + Block.X, WorldY + Block.Y, 16, 16));
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

                    int PlayerX__ = PlayerX - WorldX;
                    int PlayerY__ = PlayerY - WorldY;

                    float Distance = Vector2I.Distance(new Vector2I(Entity.X, Entity.Y), new Vector2I(PlayerX__, PlayerY__));

                    Vector2I MoveDirection = Vector2I.Zero;
                    
                    Vector2I Target = Entity.InfoVector;
                    Vector2I EntityPositionOriginal = new Vector2I(Entity.X, Entity.Y);
                    
                    if(Distance < 100 && Rotten < 10){

                        Target.X = Info is 1 or 2 ? WorldX - PlayerX : PlayerX__;
                        Target.Y = Info is 1 or 2 ? WorldY - PlayerY : PlayerY__;

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
                Game.AddCollider(new Collider(WorldX + Entity.X + (int)((16 - SizeX)/2), WorldY + Entity.Y + (int)((16 - SizeY)/2), SizeX, SizeY, 0, KVP.Key.Position, (int)KVP.Key.UniqueID, Layer));
            }

            if(Entity.ID is T_Entity.Item){
                Game.AddCollider(new Collider(WorldX + Entity.X, WorldY + Entity.Y, 16, 16, Entity.Info, KVP.Key.Position, (int)KVP.Key.UniqueID, CollisionLayer.L4));
            }
            
            if(Entity.ID is T_Entity.Crate){
                Game.AddCollider(new Collider(WorldX + Entity.X + 2, WorldY + Entity.Y + 2, 12, 12, 0, KVP.Key.Position, (int)KVP.Key.UniqueID, CollisionLayer.L5));
            }
        }

        bool CanMove = !Dead;

        if(Dead){
            if(WL.Math.Random.Fast_Bool(0.8f)){
                __Decals.Add((PlayerX - WorldX + WL.Math.Random.Fast_Int(-128, 128), PlayerY - WorldY + WL.Math.Random.Fast_Int(-128, 128), WL.Math.Random.Fast_Bool() ? T_Decal.One : T_Decal.Zero, TextureRotation.None));
            }

            Rotten += (float)TD.DeltaTimeS;
        }
        
        uint PlayerSize = (uint)(Texture_Player_Body.Width * 0.8f);
        int PlayerOffset = (int)((Texture_Player_Body.Width - PlayerSize) / 2);
        
        if(CanMove){
            uint PlayerSpeed = (uint)(WL.Math.Max(1, (float)TD.DeltaTimeS * 100 * (Game.KeyPressed(Key.Shift) ? 1.5f : (Game.KeyPressed(Key.Control) ? 0.3f : 1))));
            if(Health < HealthSmall){ PlayerSpeed = (uint)(PlayerSpeed / 2); }

            bool D = Game.KeyPressed(Key.D);
            bool A = Game.KeyPressed(Key.A);
            bool W = Game.KeyPressed(Key.W);
            bool S = Game.KeyPressed(Key.S);
            MovingDirection = new Vector2I(A && D ? 0 : (A ? 1 : (D ? -1 : 0)), W && S ? 0 : (W ? 1 : (S ? -1 : 0)));

            Vector2F DesiredMove = new Vector2F();

            CollisionLayer WallMask = IgnoreColliders ? CollisionLayer.None : CollisionLayer.L1 | CollisionLayer.L5;
            if(MovingDirection.X != 0 && MovingDirection.Y != 0){
                for(uint i = 1; i <= PlayerSpeed; i++){
                    int TestX = (int)(PlayerX - MovingDirection.X * i + PlayerOffset);
                    int TestY = (int)(PlayerY - MovingDirection.Y * i + PlayerOffset);

                    Collider TestCollider = new Collider(TestX, TestY, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, WallMask);

                    if(!Game.Collision(TestCollider, out Collider? _)){
                        DesiredMove.X = MovingDirection.X * i;
                        DesiredMove.Y = MovingDirection.Y * i;
                    }else{
                        TestCollider.X = TestX;
                        TestCollider.Y = PlayerY + PlayerOffset;
                        if(!Game.Collision(TestCollider, out Collider? _)){
                            DesiredMove.X = MovingDirection.X * i;
                            DesiredMove.Y = 0;
                        }else{
                            TestCollider.X = PlayerX + PlayerOffset;
                            TestCollider.Y = TestY;
                            if(!Game.Collision(TestCollider, out Collider? _)){
                                DesiredMove.X = 0;
                                DesiredMove.Y = MovingDirection.Y * i;
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
                    if(!Game.Collision(new Collider((int)(PlayerX - (MovingDirection.X * i) + PlayerOffset), PlayerY + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, WallMask), out Collider? _)){
                        DesiredMove.X = MovingDirection.X * i;
                    }
                    else{
                        break;
                    }
                }

                for(uint i = 1; i < PlayerSpeed + 1; i++){
                    if(!Game.Collision(new Collider(PlayerX + PlayerOffset, (int)(PlayerY - (MovingDirection.Y * i) + PlayerOffset), PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, WallMask), out Collider? _)){
                        DesiredMove.Y = MovingDirection.Y * i;
                    }
                    else{
                        break;
                    }
                }
            }

            WorldPosition += DesiredMove;

            if(DesiredMove.X != 0 || DesiredMove.Y != 0){
                Track();

                if(Game.Collision(new Collider(PlayerX + PlayerOffset, PlayerY + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.L2), out Collider? _)){
                    if(WL.Math.Random.Fast_Bool(0.5f)){
                        Damage((uint)(WL.Math.Random.Fast_0_1() * 5));
                    }
                }

                if(Game.Collision(new Collider((int)(PlayerX + PlayerOffset - DesiredMove.X * 2), (int)(PlayerY + PlayerOffset - DesiredMove.Y * 2), PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.L5), out Collider? __PushedCollider)){
                    Vector2I PushedEntityIndex1 = __PushedCollider!.Value.Info2;
                    int PushedEntityIndex2 = __PushedCollider!.Value.Info3;
                    EntityKey Key = new EntityKey(PushedEntityIndex1, (uint)PushedEntityIndex2);
                    Entity PushedEntity = __Entities[Key];

                    int __X = (DesiredMove.X == 0 ? 0 : WL.Math.Sign(DesiredMove.X));
                    int __Y = (DesiredMove.Y == 0 ? 0 : WL.Math.Sign(DesiredMove.Y));
                    
                    int NewX = PushedEntity.X - __X;
                    int NewY = PushedEntity.Y - __Y;
                    
                    if(!Game.Collision(new Collider(WorldX + NewX - __X * 2 + 2, WorldY + NewY - __Y * 2 + 2, 12, 12, 0, PushedEntityIndex1, 0, CollisionLayer.L5, WallMask), out Collider? _, true)){
                        PushedEntity.X = NewX;
                        PushedEntity.Y = NewY;
                        __Entities[Key] = PushedEntity;
                    }
                }
            }
        }
        
        if(Game.Collision(new Collider((int)(PlayerX + PlayerOffset), PlayerY + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.L3), out Collider? _)){
            if(WL.Math.Random.Fast_Bool(0.8f)){
                Damage((uint)(WL.Math.Random.Fast_0_1() * 20), Dead ? 16 : 0);
            }
        }
        
        if(Game.Collision(new Collider((int)(PlayerX + PlayerOffset), PlayerY + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.All), out Collider? Collider__)){
            InsideCollision = Collider__!.Value.Layer;
            CollisionInfo1  = Collider__!.Value.Info1;
            CollisionInfo2  = Collider__!.Value.Info2;
            CollisionInfo3  = Collider__!.Value.Info3;
        }else{
            InsideCollision = CollisionLayer.None;
            CollisionInfo1  = 0;
            CollisionInfo2  = Vector2I.Zero;
            CollisionInfo3  = 0;
        }
    }
     
     internal static readonly List<(int, int, T_Decal, TextureRotation)> __Decals = [];
    internal static void Track(){
        if(WL.Math.Random.Fast_Bool(0.1f)){
            if(Health < HealthSmall){
                SplatBlood(PlayerX - WorldX, PlayerY - WorldY);
            }else{
                __Decals.Add((PlayerX - WorldX + WL.Math.Random.Fast_Int(-5, 5), PlayerY - WorldY  + WL.Math.Random.Fast_Int(-5, 5), T_Decal.Track, TextureRotation.None));
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

        if(!IgnoreEntities && BlockInfo_Solid(Block__.ID)){
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
            if(__Blocks.TryGetValue(Key.Position, out Block __Found) && BlockInfo_Solid(__Found.ID)){ return; }
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
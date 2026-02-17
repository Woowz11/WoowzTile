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
    /// <summary>
    /// Запуск игрока в мир
    /// </summary>
    internal static void World_Start(){
        UI_InMainMenu = false;
        
        Coordinates_Camera = Vector2F.Zero;
        
        World_Decals.Clear();

        World_Time = World_TimeMax / 2;
        World_Flow = Vector2F.Zero;
        
        Player_Health = Player_HealthMax;
        UI_Interface = 0;

        Player_InventorySelectedSlot = 0;

        Player_LastTimeWereTreatedTimer = 0;
        Player_Rotting = 0;

        Player_Thought = "";
        Player_ThoughtTimer = 0;
        
        Emotion_Happiness = Emotion_Max;

        Player_ClearInventory();
        Player_Inventory[0] = T_Item.Stick;
        Player_Inventory[1] = T_Item.FirstAidKit;
        Player_Inventory[2] = T_Item.GPS;

        World_Seed = World_GenerateNewSeed();
        
        World_GoToWorld(T_World.Calm);
    }
    
    /// <summary>
    /// Запускает указанный уровень
    /// </summary>
    internal static void World_GoToWorld(T_World World){
        World_Decals  .Clear();
        World_Entities.Clear();
        World_Blocks  .Clear();

        UI_Interface = T_Interface.None;
        
        Coordinates_Camera = Vector2F.Zero;
        
        World_DeltaTick = 0;

        Player_AttackTimer = 0;

        if(World == T_World.Calm){
            Generator_World(World);
        }
    }

    /// <summary>
    /// Генерирует случайный сид
    /// </summary>
    internal static uint World_GenerateNewSeed() => (uint)WL.Math.Random.Fast_Int(0, 10000000);
    
    /// <summary>
    /// Обновление каждый кадр игры
    /// </summary>
    internal static void Game_Update(TickData TD){
        if(Cheat_FastTime){
            for(int i = 0; i < Cheat_FastTime_Value; i++){
                World_Tick(TD);
            }
        }else{
            World_Tick(TD);
        }
    }
    
    /// <summary>
    /// Обновляет 1 кадр мира
    /// </summary>
    internal static void World_Tick(TickData TD){
        Game.ClearColliders();

        if(UI_InMainMenu){ return; }

        World_StopGameTime = UI_Interface != 0;

        if(World_StopGameTime){ return; }

        Player_OutBounds = (Coordinates_Player.X - Coordinates_World.X < -World_BlocksSize.X || Coordinates_Player.X - Coordinates_World.X > World_BlocksSize.X || Coordinates_Player.Y - Coordinates_World.Y < -World_BlocksSize.Y || Coordinates_Player.Y - Coordinates_World.Y > World_BlocksSize.Y) && !Cheat_DisableWorldLimit;

        World_Time += (float)TD.DeltaTimeS * World_TimeSpeed;
        if(World_Time > World_TimeMax){ World_Time = 0; }

        World_UpdateFlow();
        
        World_UpdateBlocks();

        World_UpdateEntities();

        World_UpdatePlayer(TD);
    }
    
    /// <summary>
    /// Обновляет блоки
    /// </summary>
    internal static void World_UpdateBlocks(){
        foreach(Block Block in World_Blocks.Values){
            if(Block.ID is T_Block.Metal or T_Block.Bricks or T_Block.Water or T_Block.Black or T_Block.Error){
                Game.AddCollider(new Collider(Coordinates_World.X + Block.X, Coordinates_World.Y + Block.Y, 16, 16));
            }
        }
    }

    /// <summary>
    /// Обновляет сущностей
    /// </summary>
    internal static void World_UpdateEntities(){
        foreach(KeyValuePair<EntityKey, Entity> KVP in World_Entities){
            Entity Entity = KVP.Value;
            
            if(Entity.ID is T_Entity.Table or T_Entity.Spikes or T_Entity.Mob_Spider or T_Entity.Tree){
                if(Entity.ID == T_Entity.Mob_Spider){
                    World_Entities[KVP.Key] = World_AI_Spider(Entity);
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
                    Layer = CollisionLayer.L3 | CollisionLayer.L6;
                }
                Game.AddCollider(new Collider(Coordinates_World.X + Entity.X + (int)((16 - SizeX)/2), Coordinates_World.Y + Entity.Y + (int)((16 - SizeY)/2), SizeX, SizeY, 0, KVP.Key.Position, (int)KVP.Key.UniqueID, Layer));
            }
            
            if(Entity.ID is T_Entity.Item){
                Block Floor = World_GetBlock(Entity.X, Entity.Y, Relative: true);
                if(Floor.ID is T_Block.Water){
                    Entity.X += WL.Math.RoundProbabilistic(World_Flow.X);
                    Entity.Y += WL.Math.RoundProbabilistic(World_Flow.Y);
                    World_Entities[KVP.Key] = Entity;
                }
                
                Game.AddCollider(new Collider(Coordinates_World.X + Entity.X, Coordinates_World.Y + Entity.Y, 16, 16, Entity.Info, KVP.Key.Position, (int)KVP.Key.UniqueID, CollisionLayer.L4));
            }
            
            if(Entity.ID is T_Entity.Crate){
                Game.AddCollider(new Collider(Coordinates_World.X + Entity.X + 2, Coordinates_World.Y + Entity.Y + 2, 12, 12, 0, KVP.Key.Position, (int)KVP.Key.UniqueID, CollisionLayer.L5));
            }
        }
    }

    /// <summary>
    /// Обновляет игрока
    /// </summary>
    internal static void World_UpdatePlayer(TickData TD){
        if(Cheat_Immortality){ if(Player_Health < 1){ Player_Health = 1; } }

        Player_Floor = World_GetBlock(Coordinates_WorldPlayer.X, Coordinates_WorldPlayer.Y, Relative: true);
        
        if(Player_Dead){
            UI_Interface = 0;
            
            if(WL.Math.Random.Fast_Bool(0.8f)){
                World_Decals.Add(new Decal{ X = Coordinates_Player.X - Coordinates_World.X + WL.Math.Random.Fast_Int(-128, 128), Y = Coordinates_Player.Y - Coordinates_World.Y + WL.Math.Random.Fast_Int(-128, 128), ID = WL.Math.Random.Fast_Bool() ? T_Decal.One : T_Decal.Zero});
            }

            Player_Rotting += (float)TD.DeltaTimeS;

            Player_AttackTimer = 0;
        }else{
            if(Player_OutBounds){
                Damage(WL.Math.Random.Fast_Bool(0.05f) ? (uint)WL.Math.Random.Fast_Int(1, 10) : 0);
            }else{
                Heal((uint)(WL.Math.Random.Fast_Bool(0.001f) ? 1 : 0));   
            }
            
            EmotionChange(T_Emotion.Happiness, WL.Math.Random.Fast_Bool(0.01f) ? 1 : 0);
            
            if(WL.Math.Random.Fast_Bool(0.001f)){ SayThoughts(T_Thoughts.Idle); }

            Player_AttackTimer -= Info_Item_MeleeAttackSpeed(Player_ItemInHands);
        }

        if(Player_ThoughtTimer < 0 || Player_Dead){ Player_Thought = ""; Player_ThoughtContext = T_Thoughts.Idle; }else{ Player_ThoughtTimer -= (float)TD.DeltaTimeS; }
        
        uint PlayerSize = (uint)(Texture_Player_Body.Width * 0.8f);
        int PlayerOffset = (int)((Texture_Player_Body.Width - PlayerSize) / 2);
        
        bool CanMove = !Player_Dead;
        if(CanMove){
            uint __Player_Speed = Player_Speed(TD);
            if(Player_Health < Player_HealthLow){ __Player_Speed /= 2; }

            bool D = Game.KeyPressed(Key.D);
            bool A = Game.KeyPressed(Key.A);
            bool W = Game.KeyPressed(Key.W);
            bool S = Game.KeyPressed(Key.S);
            Player_MovingDirection = new Vector2I(A && D ? 0 : (A ? 1 : (D ? -1 : 0)), W && S ? 0 : (W ? 1 : (S ? -1 : 0)));

            if(D && !A){ 
                Player_LastDirection = Direction4.Right;
            }else if(A && !D){ 
                Player_LastDirection = Direction4.Left;
            }else if(W && !S){ 
                Player_LastDirection = Direction4.Up;
            }else if(S && !W){ 
                Player_LastDirection = Direction4.Down;
            }
            
            Vector2F DesiredMove = new Vector2F();

            CollisionLayer __Player_Collider = Player_Collider;
            if(Player_MovingDirection.X != 0 && Player_MovingDirection.Y != 0){
                for(uint i = 1; i <= __Player_Speed; i++){
                    int TestX = (int)(Coordinates_Player.X - Player_MovingDirection.X * i + PlayerOffset);
                    int TestY = (int)(Coordinates_Player.Y - Player_MovingDirection.Y * i + PlayerOffset);

                    Collider TestCollider = new Collider(TestX, TestY, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, __Player_Collider);

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
                for(uint i = 1; i < __Player_Speed + 1; i++){
                    if(!Game.Collision(new Collider((int)(Coordinates_Player.X - (Player_MovingDirection.X * i) + PlayerOffset), Coordinates_Player.Y + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, __Player_Collider), out Collider? _)){
                        DesiredMove.X = Player_MovingDirection.X * i;
                    }
                    else{
                        break;
                    }
                }

                for(uint i = 1; i < __Player_Speed + 1; i++){
                    if(!Game.Collision(new Collider(Coordinates_Player.X + PlayerOffset, (int)(Coordinates_Player.Y - (Player_MovingDirection.Y * i) + PlayerOffset), PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, __Player_Collider), out Collider? _)){
                        DesiredMove.Y = Player_MovingDirection.Y * i;
                    }
                    else{
                        break;
                    }
                }
            }

            Coordinates_Camera += DesiredMove;

            if(DesiredMove.X != 0 || DesiredMove.Y != 0){
                World_FootStep();

                if(Game.Collision(new Collider(Coordinates_Player.X + PlayerOffset, Coordinates_Player.Y + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.L2), out Collider? _)){
                    if(WL.Math.Random.Fast_Bool(0.5f)){
                        Damage((uint)(WL.Math.Random.Fast_0_1() * 5));
                    }
                }

                if(Game.Collision(new Collider((int)(Coordinates_Player.X + PlayerOffset - DesiredMove.X * 2), (int)(Coordinates_Player.Y + PlayerOffset - DesiredMove.Y * 2), PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.L5), out Collider? __PushedCollider)){
                    Vector2I PushedEntityIndex1 = __PushedCollider!.Value.Info2;
                    int PushedEntityIndex2 = __PushedCollider!.Value.Info3;
                    EntityKey Key = new EntityKey(PushedEntityIndex1, (uint)PushedEntityIndex2);
                    Entity PushedEntity = World_Entities[Key];

                    int __X = (DesiredMove.X == 0 ? 0 : WL.Math.Sign(DesiredMove.X));
                    int __Y = (DesiredMove.Y == 0 ? 0 : WL.Math.Sign(DesiredMove.Y));
                    
                    int NewX = PushedEntity.X - __X;
                    int NewY = PushedEntity.Y - __Y;
                    
                    if(!Game.Collision(new Collider(Coordinates_World.X + NewX - __X * 2 + 2, Coordinates_World.Y + NewY - __Y * 2 + 2, 12, 12, 0, PushedEntityIndex1, 0, CollisionLayer.L5, __Player_Collider), out Collider? _, true)){
                        PushedEntity.X = NewX;
                        PushedEntity.Y = NewY;
                        World_Entities[Key] = PushedEntity;
                    }
                }
            }
        }
        
        if(Game.Collision(new Collider(Coordinates_Player.X + PlayerOffset, Coordinates_Player.Y + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.L3), out Collider? Hit)){
            bool DoDamage = true;

            if(World_Entities.TryGetValue(new EntityKey(Hit.Value.Info2, (uint)Hit.Value.Info3), out Entity Entity)){
                DoDamage = Entity.Health > 0;
            }
            
            if(DoDamage && WL.Math.Random.Fast_Bool(0.8f)){
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

    /// <summary>
    /// Интеллект паука
    /// </summary>
    internal static Entity World_AI_Spider(Entity Entity){
        if(Entity.Health <= 0){ return Entity; }
        
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
        
        return Entity;
    }
    
    /// <summary>
    /// Обновляет скорость течения
    /// </summary>
    internal static void World_UpdateFlow(){
        const float __Flow_Speed_Change = 0.01f;

        __Flow_Timer_X--;
        if(__Flow_Timer_X <= 0)
        {
            __Flow_Dir_X = WL.Math.Random.Fast_Bool();
            __Flow_Timer_X = WL.Math.Random.Fast_Int(10, 60);
        }

        __Flow_Timer_Y--;
        if(__Flow_Timer_Y <= 0)
        {
            __Flow_Dir_Y = WL.Math.Random.Fast_Bool();
            __Flow_Timer_Y = WL.Math.Random.Fast_Int(10, 60);
        }

        World_Flow.X = WL.Math.Clamp(World_Flow.X + (__Flow_Dir_X ? __Flow_Speed_Change : -__Flow_Speed_Change), -World_FlowMax, World_FlowMax);
        World_Flow.Y = WL.Math.Clamp(World_Flow.Y + (__Flow_Dir_Y ? __Flow_Speed_Change : -__Flow_Speed_Change), -World_FlowMax, World_FlowMax);
    }
    private static int  __Flow_Timer_X = 0;
    private static int  __Flow_Timer_Y = 0;
    private static bool __Flow_Dir_X   = true;
    private static bool __Flow_Dir_Y   = true;
    
    /// <summary>
    /// Оставляет след
    /// </summary>
    internal static void World_FootStep(){
        if(WL.Math.Random.Fast_Bool(0.1f)){
            if(Player_Health < Player_HealthLow){
                World_SpatterBlood(Coordinates_Player.X - Coordinates_World.X, Coordinates_Player.Y - Coordinates_World.Y);
            }else{
                World_Decals.Add(new Decal{ X = Coordinates_Player.X - Coordinates_World.X + WL.Math.Random.Fast_Int(-5, 5), Y = Coordinates_Player.Y - Coordinates_World.Y  + WL.Math.Random.Fast_Int(-5, 5), ID = T_Decal.FootStep });
            }
        }
    }

    /// <summary>
    /// Оставить пятно крови
    /// </summary>
    internal static void World_SpatterBlood(int X, int Y){
        World_Decals.Add(new Decal{ X = X, Y = Y, ID = T_Decal.Blood, Rotation = WL.Math.Random.Fast_Bool(0.5f) ? (WL.Math.Random.Fast_Bool(0.5f) ? TextureRotation.None :  TextureRotation.Rotate90) : (WL.Math.Random.Fast_Bool(0.5f) ? TextureRotation.Rotate180 : TextureRotation.Rotate270)});
    }

    /// <summary>
    /// Устанавливает блок
    /// </summary>
    internal static void World_SetBlock(Block Block__, bool SnapToGrid = true, bool IgnoreEntities = false, bool Replace = true){
        if(SnapToGrid){
            Block__.X *= 16;
            Block__.Y *= 16;
        }

        Vector2I Key = new Vector2I(Block__.X, Block__.Y);
        if(World_Blocks.ContainsKey(Key)){
            if(Block__.ID == T_Block.Empty){
                World_Blocks.Remove(Key);
            }else{
                if(Replace){
                    Block OldBlock = World_Blocks[Key];
                    if(OldBlock.ID != Block__.ID){
                        World_Blocks[Key] = Block__;
                    }
                }
            }
        }else{
            if(Block__.ID != T_Block.Empty){
                World_Blocks[Key] = Block__;
            }
        }

        if(!IgnoreEntities && Info_Block_Solid(Block__.ID)){
            if(World_Entities.ContainsKey(new EntityKey(Key))){ World_SetEntity(new Entity{ X = Block__.X, Y = Block__.Y }, false, true); }
        }
    }

    /// <summary>
    /// Получает блок
    /// </summary>
    internal static Block World_GetBlock(int X, int Y, bool SnapToGrid = true, bool Relative = false){
        if(Relative){
            X -= X % 16;
            Y -= Y % 16;
        }else{
            if(SnapToGrid){
                X *= 16;
                Y *= 16;
            }
        }

        return World_Blocks.GetValueOrDefault(new Vector2I(X, Y), new Block{ ID = T_Block.Empty, X = X, Y = Y });
    }

    /// <summary>
    /// Устанавливает сущность
    /// </summary>
    internal static void World_SetEntity(Entity Entity__, bool SnapToGrid = true, bool IgnoreBlocks = false){
        if(SnapToGrid){
            Entity__.X *= 16;
            Entity__.Y *= 16;
        }

        bool HasUniqueID = Entity__.ID is T_Entity.Crate or T_Entity.Item or T_Entity.Mob_Spider;
        
        EntityKey Key = new EntityKey(new Vector2I(Entity__.X, Entity__.Y), HasUniqueID);
        
        if(!IgnoreBlocks){
            if(World_Blocks.TryGetValue(Key.Position, out Block __Found) && Info_Block_Solid(__Found.ID)){ return; }
        }

        if(Entity__.ID == T_Entity.Item && Entity__.Info == (byte)T_Item.Empty){
            Entity__.Info = (byte)T_Item.Error;
        }
        
        if(World_Entities.ContainsKey(Key)){
            if(Entity__.ID == T_Entity.Empty){
                World_Entities.Remove(Key);
            }
            else{
                Entity OldEntity = World_Entities[Key];
                if(OldEntity.ID != Entity__.ID){
                    World_Entities[Key] = Entity__;
                }
            }
        }else{
            if(Entity__.ID != T_Entity.Empty){
                World_Entities[Key] = Entity__;
            }
        }
    }
    
    /// <summary>
    /// Добавляет блоки в виде карты
    /// </summary>
    internal static void World_AddBlocksMap(string SceneMap, int X = 0, int Y = 0, uint __Seed = 0, bool Replace = false){
        try{
            if(string.IsNullOrEmpty(SceneMap)){ return; }
            
            int X__ = X;
            int Y__ = Y;
            
            __Seed += 1222;
            
            foreach(char C in SceneMap){
                T_Block ID = T_Block.Empty;
                (T_Block, byte)? ID_and_Info = null;
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
                        __Seed += 121;
                        ID_and_Info = Generator_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref __Seed), [(T_Block.Ground_Grass, 0, 1), (T_Block.Empty, 0, 1)]);
                        break;
                    case 'П':
                        __Seed += 774743;
                        ID_and_Info = Generator_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref __Seed), [(T_Block.Ground_Sand, 0, 1), (T_Block.Empty, 0, 1)]);
                        break;
                    default:
                        ID = T_Block.Error;
                        break;
                }

                byte Info = 0;
                
                if(ID_and_Info != null){
                    ID = ID_and_Info.Value.Item1;
                    Info = ID_and_Info.Value.Item2;
                }
                
                if(ID != T_Block.Empty){
                    World_SetBlock(new Block{ X = X__, Y = Y__, ID = ID, Info = Info}, Replace: Replace);
                }
                
                X__++;
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при загрузке сцены!", e);
        }
    }

    /// <summary>
    /// Добавляет сущности в виде карты
    /// </summary>
    internal static void World_AddEntitiesMap(string SceneMap, int X = 0, int Y = 0, uint __Seed = 0){
        try{
            if(string.IsNullOrEmpty(SceneMap)){ return; }
            
            int X__ = X;
            int Y__ = Y;

            __Seed -= 86;
            
            foreach(char C in SceneMap){
                T_Entity ID = T_Entity.Empty;
                (T_Entity, byte)? ID_and_Info = null;
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
                        __Seed += 1667;
                        ID_and_Info = World_GetBlock(X__, Y__).ID == T_Block.Ground_Sand ?
                            Generator_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref __Seed), [(T_Entity.Grass, 0, 1), (T_Entity.Rock, 0, 1), (T_Entity.Item, (byte)T_Item.Stick, 1), (T_Entity.Empty, 0, 99)]) :
                            Generator_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref __Seed), [(T_Entity.Tree, 0, 20), (T_Entity.Rock, 0, 10), (T_Entity.Item, (byte)T_Item.Stick, 1), (T_Entity.Bush, 0, 5), (T_Entity.Grass, 0, 43), (T_Entity.Empty, 0, 32)]);
                        break;
                    case 'д':
                        __Seed += 1532;
                        ID_and_Info = World_GetBlock(X__, Y__).ID == T_Block.Ground_Sand ?
                            Generator_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref __Seed), [(T_Entity.Grass, 0, 1), (T_Entity.Empty, 0, 99)]) :
                            Generator_SelectWeightedObject(WL.Math.Random.Fast_0_1(ref __Seed), [(T_Entity.Bush, 0, 5), (T_Entity.Grass, 0, 43), (T_Entity.Empty, 0, 32)]);
                        break;
                    default:
                        ID = T_Entity.Error;
                        break;
                }

                byte Info = 0;
                
                if(ID_and_Info != null){
                    ID = ID_and_Info.Value.Item1;
                    Info = ID_and_Info.Value.Item2;
                }
                
                if(ID != T_Entity.Empty){
                    int OffsetX = 0;
                    int OffsetY = 0;
                    
                    if(Info_Entity_RandomSpawnPosition(ID, Info)){
                        __Seed += (uint)X__;
                        OffsetX = WL.Math.Random.Fast_Int(0, 16, ref __Seed);
                        __Seed += (uint)Y__;
                        OffsetY = WL.Math.Random.Fast_Int(0, 16, ref __Seed);
                    }
                    
                    World_SetEntity(new Entity{ X = X__ * 16 + OffsetX, Y = Y__ * 16 + OffsetY, ID = ID, Info = Info}, SnapToGrid: false);
                }

                X__++;
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при загрузке Entity сцены!", e);
        }
    }

    /// <summary>
    /// Спавнит предмет
    /// </summary>
    internal static void World_SpawnItem(int X, int Y, T_Item Item){
        World_SetEntity(new Entity{ X = X, Y = Y, ID = T_Entity.Item, Info = (byte)Item}, false);
    }

    /// <summary>
    /// Нанести урон сущности
    /// </summary>
    internal static void World_DamageEntity(EntityKey Key, uint Damage){
        Entity Entity = World_Entities[Key];
        Entity.Health = WL.Math.SubU(Entity.Health, Damage);
        World_Entities[Key] = Entity;
    }
}
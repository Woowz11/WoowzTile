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
using static GOLUWorld.GOLUWorld_Utility;

namespace GOLUWorld;

internal static class GOLUWorld_World{
    /// <summary>
    /// Запуск игрока в мир
    /// </summary>
    internal static void World_Start(){
        Logger.Info("Запуск игры!");
        
        UI_InMainMenu = false;
        
        World_Seed = World_GenerateNewSeed();
        
        Coordinates_Camera = Vector2F.Zero;

        World_Time = World_TimeMax / 2;
        World_Day = 0;
        World_Flow = Vector2F.Zero;
        
        Player_Health = Player_Health_Max;
        Player_Energy = Player_Energy_Max;
        UI_Interface = 0;

        Player_InventorySelectedSlot = 0;

        Player_BrokenLeg = false;
        Player_LastTimeWereTreated_Timer = 0;
        Player_Rotting = 0;

        Player_ConsoleCommand = "HELP";
        Player_ConsoleOffset = 0;
        
        Player_Thought = "";
        Player_Thought_Timer = 0;
        
        Emotion_Happiness = Emotion_Max;

        Player_Money = 0;
        
        Player_ClearInventory();

        uint __Seed = World_Seed - 17312;
        Player_Character_Mute = WL.Math.Random.Fast_Bool(0.1f, ref __Seed);
        
        World_GoToWorld(T_World.Calm);
    }
    
    /// <summary>
    /// Запускает указанный уровень
    /// </summary>
    internal static void World_GoToWorld(T_World World){
        Logger.Info("Добро пожаловать в " + World.ToString() + "!");
        
        Game.SpecialRender((C) => {
            Texture_Loading.Render(C, Palette_Default);
        });

        World_Type = World;
        
        World_Size = new Vector2U(200, 200);
        
        World_Decals.Clear();

        World_Blocks   = new Dictionary<Vector2I , Block  >((int)(World_Size.X * World_Size.Y * 2));
        World_Ceilings = new Dictionary<Vector2I , Ceiling>((int)(World_Size.X * World_Size.Y * 2));
        World_Entities = new Dictionary<EntityKey, Entity >((int)(World_Size.X * World_Size.Y * 2) + 1000);

        UI_Interface = T_Interface.None;
        
        World_DeltaTick = 0;

        Player_Attack_Timer = 0;
        
        World_UpdatePalette(World);
        
        uint UniqueSeed = World_Seed - 1612216 + (uint)((byte)World * 23612);
        
        Generator_World(World, UniqueSeed);
        
        bool __FindSpawnLocation = false;
        while(!__FindSpawnLocation){
            uint __Seed2 = UniqueSeed + 2269909;
            Coordinates_Camera = new Vector2F(WL.Math.Random.Fast_Int(-(int)World_SizeWorld.X + 3 * 16, (int)World_SizeWorld.X - 3 * 16, ref UniqueSeed), WL.Math.Random.Fast_Int(-(int)World_SizeWorld.Y + 3 * 16, (int)World_SizeWorld.Y - 3 * 16, ref __Seed2));
            if(!Info_Block_Collide(World_GetBlock(Coordinates_PlayerWorld.X, Coordinates_PlayerWorld.Y, Relative: true).ID)){
                __FindSpawnLocation = true;
            }
            UniqueSeed++;
        }
        Coordinates_Spawn = Coordinates_PlayerWorld;
    }

    internal static void World_UpdatePalette(T_World World){
        switch(World){
            case T_World.Calm:{
                Palette_World[1 ] = ColorB.Black;
                Palette_World[2 ] = ColorB.DarkGray;
                Palette_World[3 ] = ColorB.Gray;
                Palette_World[4 ] = ColorB.LightGray;
                Palette_World[5 ] = ColorB.White;
                Palette_World[11] = ColorB.LightRed;
                Palette_World[8 ] = ColorB.Red;
                Palette_World[9 ] = ColorB.DarkRed;
                break;
            }
            
            case T_World.Industrial: {
                Palette_World[1 ] = ColorB.Black;
                Palette_World[2 ] = new ColorB(43, 36, 36);
                Palette_World[3 ] = new ColorB(87, 72, 72);
                Palette_World[4 ] = new ColorB(132, 109, 109);
                Palette_World[5 ] = new ColorB(171, 152, 152);
                Palette_World[11] = ColorB.LightRed;
                Palette_World[8 ] = ColorB.Red;
                Palette_World[9 ] = ColorB.DarkRed;
                break;
            }
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

        Player_OutBounds = (Coordinates_Player.X - Coordinates_World.X < -World_SizeWorld.X || Coordinates_Player.X - Coordinates_World.X > World_SizeWorld.X || Coordinates_Player.Y - Coordinates_World.Y < -World_SizeWorld.Y || Coordinates_Player.Y - Coordinates_World.Y > World_SizeWorld.Y) && !Cheat_DisableWorldLimit;

        World_Time += (float)TD.DeltaTimeS * World_TimeSpeed * (Cheat_FastCycleTime ? 50 : 1);
        if(World_Time > World_TimeMax){ World_Time = 0; World_Day++; }

        World_UpdateFlow();
        
        World_UpdateBlocks();

        World_UpdateEntities();

        World_UpdatePlayer(TD);
    }
    
    /// <summary>
    /// Обновляет блоки
    /// </summary>
    internal static void World_UpdateBlocks(){
        foreach((Vector2I Key, Block Block) in World_Blocks){
            if(Block.ID == T_Block.Pit){
                Game.AddCollider(new Collider(Coordinates_World.X + Block.X + 4, Coordinates_World.Y + Block.Y + 4, 16 - 4, 16 - 4, Block.Info, new Vector2I(Block.X, Block.Y), Layer: CollisionLayer.L4));
            }else{
                if(Info_Block_Collide(Block.ID)){
                    Game.AddCollider(new Collider(Coordinates_World.X + Block.X, Coordinates_World.Y + Block.Y, 16, 16, Block.Info, new Vector2I(Block.X, Block.Y)));
                }
            }
        }
    }

    /// <summary>
    /// Обновляет сущностей
    /// </summary>
    internal static void World_UpdateEntities(){
        void AddCollider(Entity Entity, int X, int Y, uint W, uint H, CollisionLayer Layer){
            Game.AddCollider(new Collider(Entity.X + Coordinates_World.X + X, Entity.Y + Coordinates_World.Y + Y, W, H, 0, new Vector2I(Entity.X, Entity.Y), (int)Entity.UniqueID, Layer));
        }
        void AddColliderBox(Entity Entity, uint SizeOffset, CollisionLayer Layer){
            AddCollider(Entity, (int)(SizeOffset/2), (int)(SizeOffset/2), 16 - SizeOffset, 16 - SizeOffset, Layer);
        }

        Dictionary<EntityKey, Entity> __UpdatedEntities = [];
        foreach((EntityKey Key, Entity Entity__) in World_Entities){
            Entity Entity = Entity__;

            bool UpdateEntity = false;
            
            if(Info_Entity_CanFlow(Entity.ID)){
                Block Floor = World_GetBlock(Entity.X, Entity.Y, Relative: true);
                if(Floor.ID is T_Block.Water){
                    Entity.X += WL.Math.RoundProbabilistic(World_Flow.X);
                    Entity.Y += WL.Math.RoundProbabilistic(World_Flow.Y);
                    UpdateEntity = true;
                }
            }

            if(Entity is{ ID: T_Entity.Mob_Spider, Health: > 0 }){
                Entity = World_AI_Spider(Entity);
                UpdateEntity = true;
            }
            
            if(Entity is{ ID: T_Entity.Mob_Drone, Health: > 0 }){
                Entity = World_AI_Drone(Entity);
                UpdateEntity = true;
            }

            switch(Entity.ID){
                case T_Entity.Debris:
                case T_Entity.Table:
                case T_Entity.Tree: AddColliderBox(Entity, 14, CollisionLayer.L1); break;
                
                case T_Entity.Spikes: AddColliderBox(Entity, 0, CollisionLayer.L2); break;
                
                case T_Entity.Mob_Drone:
                case T_Entity.Mob_Spider: if(!Entity.Dead){ AddColliderBox(Entity, 0, CollisionLayer.L3 | CollisionLayer.L6); } break;
                
                case T_Entity.Trap: AddColliderBox(Entity, 8, CollisionLayer.L3); break;
                
                case T_Entity.Crate: AddColliderBox(Entity, 4, CollisionLayer.L5); break;
                
                case T_Entity.TrashBag:
                case T_Entity.Cardboard: AddColliderBox(Entity, 4, CollisionLayer.L6); break;
                
                case T_Entity.Window: AddColliderBox(Entity, 0, CollisionLayer.L1 | CollisionLayer.L6); break;
                
                case T_Entity.Door: if(Entity.Info is 0 or 2){ AddColliderBox(Entity, 0, CollisionLayer.L1); } break;

                case T_Entity.Fence:{
                    T_Entity U = World_GetEntity(Entity.X, Entity.Y - 16, SnapToGrid: false).ID;
                    T_Entity D = World_GetEntity(Entity.X, Entity.Y + 16, SnapToGrid: false).ID;
                    T_Entity L = World_GetEntity(Entity.X - 16, Entity.Y, SnapToGrid: false).ID;
                    T_Entity R = World_GetEntity(Entity.X + 16, Entity.Y, SnapToGrid: false).ID;
                    
                    if(U == Entity.ID || D == Entity.ID){
                        if(L == Entity.ID && R == Entity.ID){
                            if(U == Entity.ID && D == Entity.ID){
                                AddCollider(Entity, 7, 0, 2, 16, CollisionLayer.L1);
                                AddCollider(Entity, 0, 7, 16, 2, CollisionLayer.L1);
                            }else{
                                if(D != Entity.ID){
                                    AddCollider(Entity, 0, 7, 16, 2, CollisionLayer.L1);
                                    AddCollider(Entity, 7, 0, 2 , 7, CollisionLayer.L1);
                                }else{
                                    AddCollider(Entity, 0, 7, 16, 2, CollisionLayer.L1);
                                    AddCollider(Entity, 7, 9, 2 , 7, CollisionLayer.L1);
                                }
                            }
                        }else{
                            if(L == Entity.ID || R == Entity.ID){
                                if(R == Entity.ID){
                                    AddCollider(Entity, 9, 7, 7, 2, CollisionLayer.L1);
                                }else{
                                    AddCollider(Entity, 0, 7, 7, 2, CollisionLayer.L1);
                                }

                                if(D == Entity.ID && U == Entity.ID){
                                    AddCollider(Entity, 7, 0, 2, 16, CollisionLayer.L1);
                                }else{
                                    if(D != Entity.ID){
                                        AddCollider(Entity, 7, 0, 2, 9, CollisionLayer.L1);
                                    }else{
                                        AddCollider(Entity, 7, 7, 2, 9, CollisionLayer.L1);
                                    }
                                }
                            }else{
                                AddCollider(Entity, 7, 0, 2, 16, CollisionLayer.L1);
                            }
                        }
                    }else{
                        AddCollider(Entity, 0, 7, 16, 2, CollisionLayer.L1);
                    }
                    
                    break;
                }
            }

            if(UpdateEntity){
                __UpdatedEntities[Key] = Entity;
            }
        }

        foreach((EntityKey Key, Entity Entity__) in __UpdatedEntities){
            World_Entities.Remove(Key);
            World_Entities[Entity__.Key] = Entity__;
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
        
        float Distance = Vector2I.Distance(new Vector2I(Entity.X, Entity.Y), new Vector2I(Coordinates_PlayerWorld.X, Coordinates_PlayerWorld.Y));

        Vector2I Target = Entity.InfoVector;
        
        if(Distance < 1000 && Player_Rotting < 10){
            Target.X = Info is 1 or 2 ? -Coordinates_PlayerWorld.X : Coordinates_PlayerWorld.X;
            Target.Y = Info is 1 or 2 ? -Coordinates_PlayerWorld.Y : Coordinates_PlayerWorld.Y;
        }else{
            if(WL.Math.Random.Fast_Bool(0.005f) || Target == Vector2I.Zero || Target == new Vector2I(Entity.X, Entity.Y)){
                Target = new Vector2I(WL.Math.Random.Fast_Int(-(int)World_SizeWorld.X, (int)World_SizeWorld.X), WL.Math.Random.Fast_Int(-(int)World_SizeWorld.Y, (int)World_SizeWorld.Y));
            }
        }
        
        float DX = Target.X - Entity.X;
        float DY = Target.Y - Entity.Y;
        float DistanceToTarget = WL.Math.Sqrt(DX * DX + DY * DY);

        if(DistanceToTarget > 0f){
            float Step = WL.Math.Min(SpiderSpeed, DistanceToTarget);

            float MoveX = DX / DistanceToTarget * Step;
            float MoveY = DY / DistanceToTarget * Step;

            Entity.X += (int)WL.Math.Round(MoveX);
            Entity.Y += (int)WL.Math.Round(MoveY);
        }

        Entity.Info = Info;
        Entity.InfoVector = Target;

        Entity.Rotation = Utility_RotationFromTwoPoints(new Vector2I(Entity.X, Entity.Y), Target);

        return Entity;
    }
     
    /// <summary>
    /// Интеллект дрона
    /// </summary>
    internal static Entity World_AI_Drone(Entity Entity){
        if(Entity.Health <= 0) return Entity;

        // Настройки
        float DroneSpeed = 10f;          // скорость дрона
        float FollowDistance = 100f;    // дистанция, которую дрон держит от игрока
        float SightDistance = 1000f;     // дистанция, на которой дрон замечает игрока
        float SmoothFactor = 0.2f;       // коэффициент плавности (0..1)

        Vector2I PlayerPos = new Vector2I(Coordinates_PlayerWorld.X, Coordinates_PlayerWorld.Y);

        // Вектор к игроку
        float DX = PlayerPos.X - Entity.X;
        float DY = PlayerPos.Y - Entity.Y;
        float Distance = WL.Math.Sqrt(DX * DX + DY * DY);

        // дрон реагирует только если игрок в зоне видимости
        if(Distance > 0f && Distance <= SightDistance){
            // вычисляем желаемый вектор движения, чтобы держать FollowDistance
            float TargetDistance = Distance - FollowDistance;
            float MoveFactor = WL.Math.Min(DroneSpeed, WL.Math.Abs(TargetDistance));

            // направление движения: приближаемся или отходим
            float Dir = TargetDistance > 0 ? 1f : -1f;

            // плавное смещение
            Entity.X += (int)WL.Math.Round(DX / Distance * MoveFactor * Dir * SmoothFactor);
            Entity.Y += (int)WL.Math.Round(DY / Distance * MoveFactor * Dir * SmoothFactor);

            // Поворот дрона
            Entity.Rotation = Utility_RotationFromTwoPoints(new Vector2I(Entity.X, Entity.Y), PlayerPos);
        }

        return Entity;
    }
    
    /// <summary>
    /// Обновляет игрока
    /// </summary>
    internal static void World_UpdatePlayer(TickData TD){
        if(Cheat_Immortality){ Player_Health = Player_Health_Max; Player_BrokenLeg = false; Player_Energy = Player_Energy_Max; }

        Player_Floor   = World_GetBlock  (Coordinates_PlayerWorld_Center.X, Coordinates_PlayerWorld_Center.Y, Relative: true);
        Player_Ceiling = World_GetCeiling(Coordinates_PlayerWorld_Center.X, Coordinates_PlayerWorld_Center.Y, Relative: true);

        Player_Running = false;
        
        if(Player_Dead){
            UI_Interface = 0;
            
            if(WL.Math.Random.Fast_Bool(0.8f)){
                World_AddDecal(new Decal{ X = Coordinates_PlayerWorld.X + WL.Math.Random.Fast_Int(-128, 128), Y = Coordinates_PlayerWorld.Y + WL.Math.Random.Fast_Int(-128, 128), ID = WL.Math.Random.Fast_Bool() ? T_Decal.One : T_Decal.Zero});
            }

            Player_Rotting += (float)TD.DeltaTimeS;

            Player_Attack_Timer = 0;
            
            Player_ClosestEntity = null;
            Player_ClosestEntity_Distance = WL.Math.MaxValue;
            
            Player_PowerDown(1);
        }else{
            if(Player_OutBounds){
                Player_Damage(WL.Math.Random.Fast_Bool(0.05f) ? (uint)WL.Math.Random.Fast_Int(1, 10) : 0);
            }else{
                if(Player_Energy > 0){
                    if(WL.Math.Random.Fast_Bool(0.001f) && Player_Health < Player_Health_Max){
                        Player_Heal(1);
                        Player_PowerDown(1);
                    }
                }else{
                    Player_Damage((uint)(WL.Math.Random.Fast_Bool(0.001f) ? 1 : 0), Comment: false);
                }
            }
            
            Player_PowerDown((uint)(WL.Math.Random.Fast_Bool(Player_Running ? 0.1f : 0.005f) ? 1 : 0));

            if(Player_Energy > 0){
                EmotionChange(T_Emotion.Happiness, WL.Math.Random.Fast_Bool(0.01f) ? 1 : 0);
            }else{
                Emotion_Happiness = 0;
            }
            
            if(WL.Math.Random.Fast_Bool(0.001f)){ SayThoughts(T_Thoughts.Idle); }

            Player_Attack_Timer -= Info_Item_MeleeAttackSpeed(Player_ItemInHands);
            
            Player_ClosestEntity_Distance = WL.Math.MaxValue;
            foreach(Entity Entity in World_Entities.Values){
                float DX = Entity.X - Coordinates_PlayerWorld_Center.X;
                float DY = Entity.Y - Coordinates_PlayerWorld_Center.Y;
                float DistanceSquare = WL.Math.Sqr(DX) + WL.Math.Sqr(DY);

                if(Info_Entity_Interacting(Entity.ID) && DistanceSquare < Player_ClosestEntity_Distance){
                    Player_ClosestEntity_Distance = DistanceSquare;
                    Player_ClosestEntity = Entity;
                }
            }
        }

        if(Player_Thought_Timer < 0 || Player_Dead){ Player_Thought = ""; Player_ThoughtContext = T_Thoughts.Idle; }else{ Player_Thought_Timer -= (float)TD.DeltaTimeS; }
        
        uint PlayerSize  = (uint)(Texture_Player_Body.Width * 0.8f);
        int PlayerOffset = (int)((Texture_Player_Body.Width - PlayerSize) / 2);
        
        bool CanMove = !Player_Dead;
        if(CanMove){
            Player_Running = Game.KeyPressed(Key.Shift);
            uint __Player_Speed = Player_Speed(TD);
            if(__Player_Speed <= 0){ __Player_Speed = 1; }

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
                    }else{
                        break;
                    }
                }

                for(uint i = 1; i < __Player_Speed + 1; i++){
                    if(!Game.Collision(new Collider(Coordinates_Player.X + PlayerOffset, (int)(Coordinates_Player.Y - (Player_MovingDirection.Y * i) + PlayerOffset), PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, __Player_Collider), out Collider? _)){
                        DesiredMove.Y = Player_MovingDirection.Y * i;
                    }else{
                        break;
                    }
                }
            }

            Coordinates_Camera += DesiredMove;

            if(DesiredMove.X != 0 || DesiredMove.Y != 0){
                World_FootStep();

                if(Game.Collision(new Collider(Coordinates_Player.X + PlayerOffset, Coordinates_Player.Y + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.L2), out Collider? _)){
                    if(WL.Math.Random.Fast_Bool(0.5f)){
                        Player_Damage((uint)(WL.Math.Random.Fast_0_1() * 5));
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
                        World_Entities.Remove(Key);
                        World_Entities[PushedEntity.Key] = PushedEntity;
                    }
                }
            }
        }
        
        if(Game.Collision(new Collider(Coordinates_Player.X + PlayerOffset, Coordinates_Player.Y + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.L3), out Collider? HitEntity)){
            EntityKey __Key = new EntityKey(HitEntity!.Value.Info2, (uint)HitEntity.Value.Info3);
            if(World_Entities.TryGetValue(__Key, out Entity Entity)){
                if(Entity.ID == T_Entity.Mob_Spider){
                    if(Entity.Health > 0 && WL.Math.Random.Fast_Bool(0.8f)){
                        Player_Damage((uint)(WL.Math.Random.Fast_0_1() * 20), Player_Dead ? 16 : 0);
                    }
                }else if(Entity is{ ID: T_Entity.Trap, Info: 0 }){
                    Player_BrokeLeg();
                    Entity.Info = 1;
                    World_Entities[__Key] = Entity;
                }
            }
        }
        
        if(Game.Collision(new Collider(Coordinates_Player.X + PlayerOffset, Coordinates_Player.Y + PlayerOffset, PlayerSize, PlayerSize, 0, Vector2I.Zero, 0, CollisionLayer.L1, CollisionLayer.L4), out Collider? HitBlock)){
            Vector2I __Key = HitBlock!.Value.Info2;
            if(World_Blocks.TryGetValue(__Key, out Block Block)){
                if(Block.ID == T_Block.Pit){
                    World_GoToWorld(T_World.Industrial);
                    Player_BrokeLeg();
                }
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
            if(Player_Health < Player_HealthLow || Player_BrokenLeg){
                World_SpatterBlood(Coordinates_PlayerWorld.X, Coordinates_PlayerWorld.Y);
            }else{
                World_AddDecal(new Decal{ X = Coordinates_PlayerWorld.X, Y = Coordinates_PlayerWorld.Y, ID = T_Decal.FootStep }, 5);
            }
        }
    }

    /// <summary>
    /// Оставить пятно крови
    /// </summary>
    internal static void World_SpatterBlood(int X, int Y){
        World_AddDecal(new Decal{ X = X, Y = Y, ID = T_Decal.Blood}, RandomRotation: true);
    }

    /// <summary>
    /// Добавляет декаль в мир
    /// </summary>
    internal static void World_AddDecal(Decal Decal, uint RandomRange = 0, bool RandomRotation = false){
        if(RandomRange != 0){
            Decal.X += WL.Math.Random.Fast_Int(-(int)RandomRange, (int)RandomRange);
            Decal.Y += WL.Math.Random.Fast_Int(-(int)RandomRange, (int)RandomRange);
        }

        if(RandomRotation){
            Decal.Rotation = Utility_RandomRotation();
        }
        
        if(Info_Block_SupportDecals(World_GetBlock(Decal.X, Decal.Y, Relative: true).ID)){
            World_Decals.Add(Decal);
        }
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

        if(!IgnoreEntities && Info_Block_Collide(Block__.ID)){
            EntityKey Key__ = new EntityKey(Key);
            if(World_Entities.ContainsKey(Key__)){ World_SetEntity(new Entity{ X = Block__.X, Y = Block__.Y }, false, true); }
        }
    }

    /// <summary>
    /// Получает блок
    /// </summary>
    internal static Block World_GetBlock(int X, int Y, bool SnapToGrid = true, bool Relative = false){
        if(Relative){
            X = World_FloorTile(X);
            Y = World_FloorTile(Y);
        }else{
            if(SnapToGrid){
                X *= 16;
                Y *= 16;
            }
        }

        return World_Blocks.TryGetValue(new Vector2I(X, Y), out Block Block) ? Block : new Block{ ID = T_Block.Empty, X = X, Y = Y };
    }

    internal static int World_FloorTile(int V, int TileSize = 16){
        return (int)WL.Math.Floor((float)V / TileSize) * TileSize;
    }
    
    /// <summary>
    /// Устанавливает сущность
    /// </summary>
    internal static void World_SetEntity(Entity Entity, bool SnapToGrid = true, bool IgnoreBlocks = false){
        if(SnapToGrid){
            Entity.X *= 16;
            Entity.Y *= 16;
        }
        
        EntityKey Key = new EntityKey(new Vector2I(Entity.X, Entity.Y), Info_Entity_Unique(Entity.ID));
        Entity.UniqueID = Key.UniqueID;
        
        uint __Seed = World_Seed + (uint)Entity.X - (uint)Entity.Y;
        __Seed *= (uint)Entity.Y;
        
        if(!IgnoreBlocks){
            Block __Block = World_GetBlock(Key.Position.X, Key.Position.Y, SnapToGrid: false);
            if(Info_Block_Collide(__Block.ID)){ return; }
        }

        if(Entity is{ ID: T_Entity.Item, Info: (byte)T_Item.Empty }){
            Entity.Info = (byte)T_Item.Error;
        }

        if(Info_Entity_Plant(Entity.ID)){
            Entity.Info = WL.Math.Random.Fast_Byte(ref __Seed);
        }

        if(Entity.ID is T_Entity.TrashBag){
            for(int i = 0; i < 6; i++){
                World_AddDecal(new Decal{ ID = Info_Decal_RandomTrash(), X = Entity.X + 8, Y = Entity.Y + 8}, 64, true);
            }  
        }

        Entity.Health = Info_Entity_Health(Entity.ID);
        
        if(World_Entities.ContainsKey(Key)){
            if(Entity.ID == T_Entity.Empty){
                World_Entities.Remove(Key);
            }else{
                Entity OldEntity = World_Entities[Key];
                if(OldEntity.ID != Entity.ID && OldEntity.ID != T_Entity.Fence){
                    World_Entities[Key] = Entity;
                }
            }
        }else{
            if(Entity.ID != T_Entity.Empty){
                World_Entities[Key] = Entity;
            }
        }
    }
    
    /// <summary>
    /// Получает сущность
    /// </summary>
    internal static Entity World_GetEntity(int X, int Y, bool SnapToGrid = true, bool Relative = false, uint UniqueID = 0){
        if(Relative){
            X = World_FloorTile(X);
            Y = World_FloorTile(Y);
        }else{
            if(SnapToGrid){
                X *= 16;
                Y *= 16;
            }
        }
        
        return World_Entities.TryGetValue(new EntityKey(new Vector2I(X, Y), UniqueID), out Entity Entity) ? Entity : new Entity{ ID = T_Entity.Empty, X = X, Y = Y };
    }

    /// <summary>
    /// Уничтожает указанную сущность
    /// </summary>
    internal static void World_RemoveEntity(Entity Entity){
        World_Entities.Remove(Entity.Key);
    }
    
    /// <summary>
    /// Добавляет блоки в виде карты
    /// </summary>
    internal static void World_AddBlocksMap(string SceneMap, int X = 0, int Y = 0, uint Seed = 0, bool Replace = false, TextureRotation Rotation = TextureRotation.None){
        try{
            if(string.IsNullOrEmpty(SceneMap)){ return; }

            string[] Lines = SceneMap.Replace("\r", "").Split('\n');
            int W = Lines.Max(L => L.Length);
            int H = Lines.Length;

            int CX = W / 2;
            int CY = H / 2;

            Seed += 1222;

            for(int Y__ = 0; Y__ < H; Y__++){
                string Line = Lines[Y__];
                for(int X__ = 0; X__ < W; X__++){
                    if(X__ >= Line.Length){ continue; }
                    
                    char C = Line[X__];
                    if(C == '.'){ continue; }

                    int FX = X__;
                    int FY = Y__;

                    if(Rotation != TextureRotation.None){
                        int RX = X__ - CX;
                        int RY = Y__ - CY;

                        switch(Rotation){
                            case TextureRotation.Rotate90:
                                FX = CX - RY;
                                FY = CY + RX;
                                break;
                            case TextureRotation.Rotate180:
                                FX = CX - RX;
                                FY = CY - RY;
                                break;
                            case TextureRotation.Rotate270:
                                FX = CX + RY;
                                FY = CY - RX;
                                break;
                        }
                    }

                    FX += X;
                    FY += Y;

                    (T_Block ID, byte Info)? ID_and_Info = Info_Block_Symbol(C, FX, FY, Seed, Rotation);
                    if(ID_and_Info != null && ID_and_Info.Value.ID != T_Block.Empty){
                        World_SetBlock(new Block{ X = FX, Y = FY, ID = ID_and_Info.Value.ID, Info = ID_and_Info.Value.Info}, Replace: Replace);
                    }
                }   
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при загрузке сцены!", e);
        }
    }

    /// <summary>
    /// Добавляет сущности в виде карты
    /// </summary>
    internal static void World_AddEntitiesMap(string SceneMap, int X = 0, int Y = 0, uint Seed = 0, TextureRotation Rotation = TextureRotation.None){
        try{
            if(string.IsNullOrEmpty(SceneMap)){ return; }

            string[] Lines = SceneMap.Replace("\r", "").Split('\n');
            int W = Lines.Max(L => L.Length);
            int H = Lines.Length;

            int CX = W / 2;
            int CY = H / 2;

            Seed -= 86;

            for(int Y__ = 0; Y__ < H; Y__++){
                string Line = Lines[Y__];
                for(int X__ = 0; X__ < W; X__++){
                    if(X__ >= Line.Length){ continue; }
                    
                    char C = Line[X__];
                    if(C == '.'){ continue; }

                    int FX = X__;
                    int FY = Y__;

                    if(Rotation != TextureRotation.None){
                        int RX = X__ - CX;
                        int RY = Y__ - CY;

                        switch(Rotation){
                            case TextureRotation.Rotate90:
                                FX = CX - RY;
                                FY = CY + RX;
                                break;
                            case TextureRotation.Rotate180:
                                FX = CX - RX;
                                FY = CY - RY;
                                break;
                            case TextureRotation.Rotate270:
                                FX = CX + RY;
                                FY = CY - RX;
                                break;
                        }
                    }

                    FX += X;
                    FY += Y;

                    (T_Entity ID, byte Info)? ID_and_Info = Info_Entity_Symbol(C, FX, FY, Seed, Rotation);
                    if(ID_and_Info != null && ID_and_Info.Value.ID != T_Entity.Empty){
                        int OffsetX = 0;
                        int OffsetY = 0;
                    
                        if(Info_Entity_RandomSpawnPosition(ID_and_Info.Value.ID, ID_and_Info.Value.Info)){
                            Seed += (uint)X__;
                            OffsetX = WL.Math.Random.Fast_Int(0, 16, ref Seed);
                            Seed += (uint)Y__;
                            OffsetY = WL.Math.Random.Fast_Int(0, 16, ref Seed);
                        }
                        
                        World_SetEntity(new Entity{ X = FX * 16 + OffsetX, Y = FY * 16 + OffsetY, ID = ID_and_Info.Value.ID, Info = ID_and_Info.Value.Info}, SnapToGrid: false);
                    }
                }
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при загрузке сцены с сущностями!", e);
        }
    }

    /// <summary>
    /// Устанавливает потолок
    /// </summary>
    internal static void World_SetCeiling(Ceiling Ceiling__, bool SnapToGrid = true, bool IgnoreEntities = false, bool Replace = true){
        if(SnapToGrid){
            Ceiling__.X *= 16;
            Ceiling__.Y *= 16;
        }

        Vector2I Key = new Vector2I(Ceiling__.X, Ceiling__.Y);
        if(World_Ceilings.ContainsKey(Key)){
            if(Ceiling__.ID == T_Ceiling.Empty){
                World_Ceilings.Remove(Key);
            }else{
                if(Replace){
                    Ceiling OldCeiling = World_Ceilings[Key];
                    if(OldCeiling.ID != Ceiling__.ID){
                        World_Ceilings[Key] = Ceiling__;
                    }
                }
            }
        }else{
            if(Ceiling__.ID != T_Ceiling.Empty){
                World_Ceilings[Key] = Ceiling__;
            }
        }
    }

    /// <summary>
    /// Получает потолок
    /// </summary>
    internal static Ceiling World_GetCeiling(int X, int Y, bool SnapToGrid = true, bool Relative = false){
        if(Relative){
            X = World_FloorTile(X);
            Y = World_FloorTile(Y);
        }else{
            if(SnapToGrid){
                X *= 16;
                Y *= 16;
            }
        }

        Vector2I Key = new Vector2I(X, Y);
        return World_Ceilings.TryGetValue(Key, out Ceiling Ceiling) ? Ceiling : new Ceiling{ ID = T_Ceiling.Empty, X = X, Y = Y };
    }
    
    /// <summary>
    /// Добавляет потолки в виде карты
    /// </summary>
    internal static void World_AddCeilingsMap(string SceneMap, int X = 0, int Y = 0, uint Seed = 0, bool Replace = false, TextureRotation Rotation = TextureRotation.None){
        try{
            if(string.IsNullOrEmpty(SceneMap)){ return; }

            string[] Lines = SceneMap.Replace("\r", "").Split('\n');
            int W = Lines.Max(L => L.Length);
            int H = Lines.Length;

            int CX = W / 2;
            int CY = H / 2;

            Seed += 12336;

            for(int Y__ = 0; Y__ < H; Y__++){
                string Line = Lines[Y__];
                for(int X__ = 0; X__ < W; X__++){
                    if(X__ >= Line.Length){ continue; }
                    
                    char C = Line[X__];
                    if(C == '.'){ continue; }

                    int FX = X__;
                    int FY = Y__;

                    if(Rotation != TextureRotation.None){
                        int RX = X__ - CX;
                        int RY = Y__ - CY;

                        switch(Rotation){
                            case TextureRotation.Rotate90:
                                FX = CX - RY;
                                FY = CY + RX;
                                break;
                            case TextureRotation.Rotate180:
                                FX = CX - RX;
                                FY = CY - RY;
                                break;
                            case TextureRotation.Rotate270:
                                FX = CX + RY;
                                FY = CY - RX;
                                break;
                        }
                    }

                    FX += X;
                    FY += Y;

                    (T_Ceiling ID, byte Info)? ID_and_Info = Info_Ceiling_Symbol(C, FX, FY, Seed, Rotation);
                    if(ID_and_Info != null && ID_and_Info.Value.ID != T_Ceiling.Empty){
                        World_SetCeiling(new Ceiling{ X = FX, Y = FY, ID = ID_and_Info.Value.ID, Info = ID_and_Info.Value.Info}, Replace: Replace);
                    }
                }   
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при загрузке сцены с потолком!", e);
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

        bool DoRemove = false;
        Entity? NewEntity = null;
        
        switch(Entity.ID){
            case T_Entity.Mob_Spider when !Entity.Dead:
                World_SpatterBlood(Entity.X, Entity.Y);
                break;
            
            case T_Entity.Window when Entity.Info == 0:{
                DoRemove = true;
                for(int i = 0; i < 6; i++){
                    World_AddDecal(new Decal{ ID = T_Decal.Glass, X = Entity.X, Y = Entity.Y}, 16, true);
                }

                break;
            }
            
            case T_Entity.Window:{
                if(Entity.Dead){ Entity.Info = 0; }
                break;
            }
            
            case T_Entity.TrashBag or T_Entity.Cardboard when Entity.Dead:{
                DoRemove = true;
                for(int i = 0; i < 6; i++){
                    World_AddDecal(new Decal{ ID = Info_Decal_RandomTrash(), X = Entity.X, Y = Entity.Y}, 16, true);
                }

                (T_Entity ID, byte Info) Loot = Info_Entity_Loot_TrashBag(World_Seed + Utility_SeedXY(Entity.X, Entity.Y));
                if(Loot.ID != T_Entity.Empty){ NewEntity = new Entity{ ID = Loot.ID, Info = Loot.Info }; }

                break;
            }
        }
        
        Entity NewEntity__ = NewEntity ?? new Entity();
        
        if(NewEntity.HasValue){
            NewEntity__.X = Entity.X;
            NewEntity__.Y = Entity.Y;
        }
        
        if(DoRemove){
            World_Entities.Remove(Key);
            if(NewEntity != null){
                World_SetEntity(NewEntity__, SnapToGrid: false);
            }
        }else{
            World_Entities[Key] = NewEntity.HasValue ? NewEntity__ : Entity;   
        }
    }

    /// <summary>
    /// Нанести урон блоку
    /// </summary>
    internal static void World_DamageBlock(Vector2I Key, uint Damage){
        Block Block = World_Blocks[Key];
    }
}
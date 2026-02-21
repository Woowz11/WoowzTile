using WL;
using WLO;
using WoowzTile;
using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_World;
using static GOLUWorld.GOLUWorld_Resources;
using static GOLUWorld.GOLUWorld_Info;
using static GOLUWorld.GOLUWorld_Utility;

namespace GOLUWorld;

internal static class GOLUWorld_Player{
    /// <summary>
    /// С какими коллизиями сталкивается игрок?
    /// </summary>
    internal static CollisionLayer Player_Collider => Cheat_IgnoreColliders ? CollisionLayer.None : CollisionLayer.L1 | CollisionLayer.L5;

    /// <summary>
    /// Скорость игрока
    /// </summary>
    internal static uint Player_Speed(TickData TD) => (uint)(WL.Math.Max(1, (float)TD.DeltaTimeS * 100 * (Player_Running ? 1.5f : (Game.KeyPressed(Key.Control) ? 0.3f : 1))) * (Player_BrokenLeg ? 0.5f : 1) * (Player_Health < Player_HealthLow || Player_Energy < 10 ? 0.5f : 1));
    
    /// <summary>
    /// Очищает инвентарь
    /// </summary>
    internal static void Player_ClearInventory() => Array.Clear(Player_Inventory, 0, Player_Inventory.Length);
    
    internal static string GetRandomThoughts(T_Thoughts Thoughts){
        byte ThoughtsKey = 0;

        if(Emotion_Happiness > 75){ ThoughtsKey = 5; }
        
        if(Player_Health < Player_HealthLow * 2){
            ThoughtsKey = (byte)(Player_Health < Player_HealthLow ? 2 : 1);
        }

        if(Thoughts is T_Thoughts.Damage){ ThoughtsKey = 3; }
        if(Thoughts is T_Thoughts.Heal  ){ ThoughtsKey = 4; }
        
        string[] Messages = ThoughtsMessages[ThoughtsKey];
        return Messages[WL.Math.Random.Fast_Int(0, Messages.Length - 1)];
    }

    internal static void SayThoughts(T_Thoughts Thoughts){
        if((Player_ThoughtContext == Thoughts && Thoughts != T_Thoughts.Idle) || Player_Dead){ return; }
        if(Thoughts == T_Thoughts.Idle && Player_Thought_Timer > 0){ return; }
        Player_ThoughtContext = Thoughts;
        
        GOLUWorld_Values.Player_Thought = GetRandomThoughts(Thoughts);

        Player_Thought_Timer = WL.Math.Random.Fast_Int(3, 6);
    }

    internal static void EmotionChange(T_Emotion Emotion, int Value){
        if(Value == 0){ return; }
        
        uint Value__ = Emotion switch{
            T_Emotion.Happiness => Emotion_Happiness
        };

        if(Value < 0){
            Value__ = WL.Math.SubU(Value__, (uint)WL.Math.Abs(Value));
        }else{
            Value__ += (uint)Value;
            if(Value__ > Emotion_Max){ Value__ = Emotion_Max; }
        }

        switch(Emotion){
            case T_Emotion.Happiness: Emotion_Happiness = Value__; break;
        }
    }
    
    /// <summary>
    /// Нанести урон игроку
    /// </summary>
    internal static void Player_Damage(uint Damage, int Range = 0, bool Comment = true){
        if(Damage == 0 || Cheat_Immortality || Player_Dead){ return; }
        
        Player_Health = WL.Math.SubU(Player_Health, Damage);

        World_SpatterBlood(Coordinates_PlayerWorld.X + WL.Math.Random.Fast_Int(-Range, Range), Coordinates_PlayerWorld.Y + WL.Math.Random.Fast_Int(-Range, Range));

        EmotionChange(T_Emotion.Happiness, -(int)Damage * 2);

        if(Comment){ SayThoughts(T_Thoughts.Damage); }
    }
    
    /// <summary>
    /// Восстановить здоровье игроку
    /// </summary>
    internal static void Player_Heal(uint Heal, bool FirstAidKit = false){
        if(Heal == 0){ return; }
        
        Player_Health += Heal;
        if(Player_Health > Player_Health_Max){ Player_Health = Player_Health_Max; }

        if(FirstAidKit){ Player_LastTimeWereTreated_Timer = 60; SayThoughts(T_Thoughts.Heal); Player_BrokenLeg = false; }
        
        EmotionChange(T_Emotion.Happiness, (int)(Heal / 2));
    }

    /// <summary>
    /// Сломать ногу игроку
    /// </summary>
    internal static void Player_BrokeLeg(){
        Player_Damage((uint)WL.Math.Random.Fast_Int(25,50), 10);
        for(int i = 0; i < 10; i++){
            World_SpatterBlood(Coordinates_PlayerWorld.X + WL.Math.Random.Fast_Int(-20, 20), Coordinates_PlayerWorld.Y + WL.Math.Random.Fast_Int(-20, 20));
        }
        Player_BrokenLeg = true;
    }
    
    /// <summary>
    /// Убавляет энергии
    /// </summary>
    internal static void Player_PowerDown(uint Value){
        if(Value == 0 || Cheat_Immortality){ return; }
        
        Player_Energy = WL.Math.SubU(Player_Energy, Value);
    }
    
    /// <summary>
    /// Прибавляет энергии
    /// </summary>
    internal static void Player_PowerUp(uint Value){
        if(Value == 0){ return; }
        
        Player_Energy += Value;
        if(Player_Energy > Player_Energy_Max){ Player_Energy = Player_Energy_Max; }
    }
    
    /// <summary>
    /// Игрок атакует в ближнем бою
    /// </summary>
    internal static void Player_AttackMelee(Direction4 Direction){
        Player_AttackDirection = Direction;
        Player_Attack_Timer = 1;

        const int PlayerColliderSize = 16;
        const int AttackRange        = 16;
        const int AttackThickness    = 24;
        
        int AttackX = 0;
        int AttackY = 0;
        int Width   = 0;
        int Height  = 0;

        switch (Player_AttackDirection)
        {
            case Direction4.Right:
                AttackX = Coordinates_Player.X + PlayerColliderSize;
                AttackY = Coordinates_Player.Y + (PlayerColliderSize - AttackThickness) / 2;
                Width   = AttackRange;
                Height  = AttackThickness;
                break;

            case Direction4.Left:
                AttackX = Coordinates_Player.X - AttackRange;
                AttackY = Coordinates_Player.Y + (PlayerColliderSize - AttackThickness) / 2;
                Width   = AttackRange;
                Height  = AttackThickness;
                break;

            case Direction4.Up:
                AttackX = Coordinates_Player.X + (PlayerColliderSize - AttackThickness) / 2;
                AttackY = Coordinates_Player.Y - AttackRange;
                Width   = AttackThickness;
                Height  = AttackRange;
                break;

            case Direction4.Down:
                AttackX = Coordinates_Player.X + (PlayerColliderSize - AttackThickness) / 2;
                AttackY = Coordinates_Player.Y + PlayerColliderSize;
                Width   = AttackThickness;
                Height  = AttackRange;
                break;
        }
        
        if(Game.Collision(new Collider(AttackX, AttackY, (uint)Width, (uint)Height, Mask: CollisionLayer.L6), out Collider? Hit)){
            World_DamageEntity(new EntityKey(Hit!.Value.Info2, (uint)Hit.Value.Info3), Item_Info_MeleeAttackDamage(Player_ItemInHands));   
        }
    }
    
    /// <summary>
    /// Использует предмет в руках
    /// </summary>
    /// <param name="Direction">Направление действия</param>
    internal static void Player_ItemUse(Direction4? Direction = null){
        if(Player_Attack_Timer > 0 || Player_Dead){ return; }

        Direction4 Direction__ = Direction ?? Player_LastDirection;
        
        T_Item Item = Player_ItemInHands;
        
        if(Item != T_Item.Empty){
            bool RemoveItem = false;
            bool Used = true;
            
            switch(Item){
                case T_Item.FirstAidKit: {
                    if(Player_Health == Player_Health_Max){ return; }
                    
                    Player_Heal(50, true);
                    
                    RemoveItem = true;
                    break;
                }

                case T_Item.Pipe:
                case T_Item.Destroyer:
                case T_Item.Crowbar:
                case T_Item.Stick: {
                    Player_AttackMelee(Direction__);
                    break;
                }

                case T_Item.Rock:{
                    int OffsetX = Direction__ switch{
                        Direction4.Left  => -1,
                        Direction4.Right => 1,
                        var _ => 0
                    };
                    int OffsetY = Direction__ switch{
                        Direction4.Up   => -1,
                        Direction4.Down => 1,
                        var _ => 0
                    };

                    Block Block = World_GetBlock(Coordinates_PlayerWorld_Center.X + OffsetX * 16, Coordinates_PlayerWorld_Center.Y + OffsetY * 16, Relative: true);
                    if(Info_Block_Pit(Block.ID)){
                        World_SetBlock(new Block{ X = Block.X, Y = Block.Y, ID = T_Block.Ground_Cobblestone, Info = (byte)(Block.ID == T_Block.Water ? 1 : 0) }, SnapToGrid: false);
                        RemoveItem = true;
                    }
                    
                    break;
                }
                
                case T_Item.Mushroom:
                    Player_Heal(10);
                    Player_PowerUp(10);
                    
                    RemoveItem = true;
                    break;
                
                case T_Item.Battery:
                    Player_PowerUp(100);
                    
                    RemoveItem = true;
                    break;
                
                default: Used = false; break;
            }
            
            if(Used){ Player_PowerDown(1); }

            if(RemoveItem){
                Player_Inventory[Player_InventorySelectedSlot] = 0;
            }
        }
    }

    /// <summary>
    /// Выкидывает предмет в руках
    /// </summary>
    internal static void Player_ItemDrop(){
        if(Player_Attack_Timer > 0 || Player_Dead){ return; }
        
        T_Item Item = Player_ItemInHands;
        if(Item != T_Item.Empty){
            World_SpawnItem(Coordinates_PlayerWorld.X, Coordinates_PlayerWorld.Y, Item);
            Player_ItemInHands = T_Item.Empty;
        }
    }

    /// <summary>
    /// Меняет выбранный слот
    /// </summary>
    internal static void Player_ItemSwitch(byte Slot){
        if(Player_Attack_Timer > 0 || Player_Dead){ return; }
        
        Player_InventorySelectedSlot = Slot;
    }

    internal static bool AddToInventory(T_Item Item){
        for(int i = 0; i < Player_Inventory.Length; i++){
            if(Player_Inventory[i] == T_Item.Empty){
                Player_Inventory[i] = Item;
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Взаимодействует с сущностью
    /// </summary>
    internal static void Player_Interact(){
        if(Player_ClosestEntity != null && Player_ClosestEntity_Distance < Player_Interact_Distance){
            Entity Entity = Player_ClosestEntity.Value;
            switch(Entity.ID){
                case T_Entity.Item:{
                    T_Item Item = (T_Item)Entity.Info;
                    if(Item != T_Item.Empty){
                        if(AddToInventory(Item)){ World_RemoveEntity(Entity); }
                    }

                    break;
                }
                
                case T_Entity.Door:
                    Entity.Info = Entity.Info switch{
                        0 => 1,
                        1 => 0,
                        2 => 3,
                        3 => 2
                    };
                    World_Entities[Entity.Key] = Entity;
                    break;
                
                case T_Entity.Money:
                    Player_Money += Info_Money_Cost((T_Money)Entity.Info);
                    World_Entities.Remove(Entity.Key);
                    break;
                
                case T_Entity.Trapdoor:
                    World_GoToWorld(T_World.Industrial);
                    break;
            }
        }
    }

    internal static void Player_Cheat_MakeFasterTime(ref TickData TD){
        if(Cheat_FastTime){
            TD.StopTime = TD.StartTime + (TD.DeltaTime * Cheat_FastTime_Value);
        }
    }
    
    /// <summary>
    /// Телепортирует игрока на координаты
    /// </summary>
    internal static void Player_Teleport(int X, int Y){
        Coordinates_Camera = Utility_PlayerWorldToCamera(new Vector2I(X, Y));
    }

    /// <summary>
    /// Работа консоли
    /// </summary>
    internal static void Player_Console(Key Key){
        if(Key is >= Key.A and <= Key.Z){
            Player_ConsoleCommand += Key.ToString();
        }else if(Key is >= Key.D0 and <= WL.Key.D9){
            Player_ConsoleCommand += (char)((int)'0' + (Key - Key.D0));
        }else if(Key == Key.Space){
            Player_ConsoleCommand += " ";
        }else if(Key == Key.Backspace && Player_ConsoleCommand.Length > 0){
            Player_ConsoleCommand = Player_ConsoleCommand[..^1];
        }else if(Key == Key.Up){
            if(GOLUWorld.__Messages.Count > 23 && Player_ConsoleOffset - 1 < GOLUWorld.__Messages.Count - 23){ Player_ConsoleOffset++; }
        }else if(Key == Key.Down){
            if(Player_ConsoleOffset > 0){ Player_ConsoleOffset--; }
        }else if(Key == Key.Enter){
            string[] Parts = Player_ConsoleCommand.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if(Parts.Length == 0){ return; }

            string Command = Parts[0];
            string[] Args = Parts.Length > 1 ? Parts[1..] : [];

            switch(Command){
                case "HELP": {
                    Logger.Info("--- HELP ---");
                    
                    Logger.Info("HELP - ПОКАЗАТЬ ЭТОТ СПИСОК");
                    Logger.Info("CLEAR - ОЧИСТИТЬ КОНСОЛЬ");
                    Logger.Info("SEED - СИД МИРА");
                    Logger.Info("SUICIDE - САМОУБИЙСТВО");
                    
                    Logger.Info("------------");
                    break;
                }

                case "CLEAR": GOLUWorld.__Messages.Clear(); break;

                case "SEED": Logger.Info("СИД: " + World_Seed); break;
                
                case "SUICIDE": Player_Damage(uint.MaxValue, Comment: false); break;
                
                default:
                    Logger.Error("Команды [\"" + Command + "\"] не существует!");
                    Logger.Error("Используйте [\"HELP\"]");
                    break;
            }

            Player_ConsoleCommand = "";
        }
    }
}
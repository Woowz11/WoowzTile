using WL;
using WLO;
using WoowzTile;
using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Values;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_World;
using static GOLUWorld.GOLUWorld_Resources;
using static GOLUWorld.GOLUWorld_Info;

namespace GOLUWorld;

internal static class GOLUWorld_Player{
    /// <summary>
    /// С какими коллизиями сталкивается игрок?
    /// </summary>
    internal static CollisionLayer Player_Collider => Cheat_IgnoreColliders ? CollisionLayer.None : CollisionLayer.L1 | CollisionLayer.L5;

    /// <summary>
    /// Скорость игрока
    /// </summary>
    internal static uint Player_Speed(TickData TD) => (uint)(WL.Math.Max(1, (float)TD.DeltaTimeS * 100 * (Game.KeyPressed(Key.Shift) ? 1.5f : (Game.KeyPressed(Key.Control) ? 0.3f : 1))));
    
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
        if(Thoughts == T_Thoughts.Idle && Player_ThoughtTimer > 0){ return; }
        Player_ThoughtContext = Thoughts;
        
        GOLUWorld_Values.Player_Thought = GetRandomThoughts(Thoughts);

        Player_ThoughtTimer = WL.Math.Random.Fast_Int(3, 6);
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
    
    internal static void Damage(uint Damage, int Range = 0){
        if(Damage == 0 || Cheat_Immortality || Player_Dead){ return; }
        
        Player_Health = WL.Math.SubU(Player_Health, Damage);

        World_SpatterBlood(Coordinates_Player.X - Coordinates_World.X + WL.Math.Random.Fast_Int(-Range, Range), Coordinates_Player.Y - Coordinates_World.Y + WL.Math.Random.Fast_Int(-Range, Range));

        EmotionChange(T_Emotion.Happiness, -(int)Damage * 2);

        SayThoughts(T_Thoughts.Damage);
    }
    
    internal static void Heal(uint Heal, bool FirstAidKit = false){
        if(Heal == 0){ return; }
        
        Player_Health += Heal;
        if(Player_Health > Player_HealthMax){ Player_Health = Player_HealthMax; }

        if(FirstAidKit){ Player_LastTimeWereTreatedTimer = 60; SayThoughts(T_Thoughts.Heal); }
        
        EmotionChange(T_Emotion.Happiness, (int)(Heal / 2));
    }
    
    /// <summary>
    /// Игрок атакует в ближнем бою
    /// </summary>
    internal static void Player_AttackMelee(Direction4? Direction = null){
        Player_AttackDirection = Direction ?? Player_LastDirection;
        Player_AttackTimer = 1;

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
                //World_DamageBlock(Hit.Value.Info2, Item_Info_MeleeAttackDamage(Player_ItemInHands));
            World_DamageEntity(new EntityKey(Hit!.Value.Info2, (uint)Hit.Value.Info3), Item_Info_MeleeAttackDamage(Player_ItemInHands));   
        }
    }
    
    /// <summary>
    /// Использует предмет в руках
    /// </summary>
    /// <param name="Direction">Направление действия</param>
    internal static void Player_ItemUse(Direction4? Direction = null){
        if(Player_AttackTimer > 0 || Player_Dead){ return; }
        
        T_Item Item = Player_ItemInHands;

        if(Item != T_Item.Empty){
            bool RemoveItem = false;
            
            switch(Item){
                case T_Item.FirstAidKit: {
                    if(Player_Health == Player_HealthMax){ return; }
                    
                    Heal(50, true);
                    
                    RemoveItem = true;
                    break;
                }

                case T_Item.Stick: {
                    Player_AttackMelee(Direction);
                    break;
                }
            }

            if(RemoveItem){
                Player_Inventory[Player_InventorySelectedSlot] = 0;
            }
        }
    }

    /// <summary>
    /// Выкидывает предмет в руках
    /// </summary>
    internal static void Player_ItemDrop(){
        if(Player_AttackTimer > 0 || Player_Dead){ return; }
        
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
        if(Player_AttackTimer > 0 || Player_Dead){ return; }
        
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
    
    internal static void Use(){
        if(Player_InteractingCollision == CollisionLayer.L4){
            T_Item Item = (T_Item)Player_CollisionInfo1;
            if(Item != T_Item.Empty){
                if(AddToInventory(Item)){ World_Entities.Remove(new EntityKey(Player_CollisionInfo2, (uint)Player_CollisionInfo3)); }
            }
        }
    }

    internal static void Player_Cheat_MakeFasterTime(ref TickData TD){
        if(Cheat_FastTime){
            TD.StopTime = TD.StartTime + (TD.DeltaTime * Cheat_FastTime_Value);
        }
    }
}
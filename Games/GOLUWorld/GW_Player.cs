using WoowzTile.Objects;
using static GOLUWorld.GW_Values;
using static GOLUWorld.GW_Objects;
using static GOLUWorld.GW_World;
using static GOLUWorld.GW_Resources;

namespace GOLUWorld;

internal static class GW_Player{
    internal static string GetRandomThoughts(T_Thoughts Thoughts){
        byte ThoughtsKey = 0;

        if(Emotion_Happiness > 75){ ThoughtsKey = 5; }
        
        if(Health < HealthSmall * 2){
            ThoughtsKey = (byte)(Health < HealthSmall ? 2 : 1);
        }

        if(Thoughts is T_Thoughts.Damage){ ThoughtsKey = 3; }
        if(Thoughts is T_Thoughts.Heal  ){ ThoughtsKey = 4; }
        
        string[] Messages = ThoughtsMessages[ThoughtsKey];
        return Messages[WL.Math.Random.Fast_Int(0, Messages.Length - 1)];
    }

    internal static void SayThoughts(T_Thoughts Thoughts){
        if((ThoughtsContext == Thoughts && Thoughts != T_Thoughts.Idle) || Dead){ return; }
        if(Thoughts == T_Thoughts.Idle && ThoughtsTimer > 0){ return; }
        ThoughtsContext = Thoughts;
        
        GW_Values.Thoughts = GetRandomThoughts(Thoughts);

        ThoughtsTimer = WL.Math.Random.Fast_Int(3, 6);
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
        if(Damage == 0 || Immortality || Dead){ return; }
        
        Health = WL.Math.SubU(Health, Damage);

        SplatBlood(PlayerX - WorldX + WL.Math.Random.Fast_Int(-Range, Range), PlayerY - WorldY + WL.Math.Random.Fast_Int(-Range, Range));

        EmotionChange(T_Emotion.Happiness, -(int)Damage * 2);

        SayThoughts(T_Thoughts.Damage);
    }
    
    internal static void Heal(uint Heal, bool FirstAidKit = false){
        if(Heal == 0){ return; }
        
        Health += Heal;
        if(Health > HealthMax){ Health = HealthMax; }

        if(FirstAidKit){ LastHealed = 60; SayThoughts(T_Thoughts.Heal); }
        
        EmotionChange(T_Emotion.Happiness, (int)(Heal / 2));
    }
    
    internal static void UseItem(){
        T_Item Item = Inventory[SelectedItem];

        if(Item != T_Item.Empty){
            bool RemoveItem = false;
            
            switch(Item){
                case T_Item.FirstAidKit:{
                    if(Health == HealthMax){ return; }
                    
                    Heal(50, true);
                    
                    RemoveItem = true;
                    break;
                }
            }

            if(RemoveItem){
                Inventory[SelectedItem] = 0;
            }
        }
    }

    internal static bool AddToInventory(T_Item Item){
        for(int i = 0; i < Inventory.Length; i++){
            if(Inventory[i] == T_Item.Empty){
                Inventory[i] = Item;
                return true;
            }
        }
        
        return false;
    }
    
    internal static void Use(){
        if(InsideCollision == CollisionLayer.L4){
            T_Item Item = (T_Item)CollisionInfo1;
            if(Item != T_Item.Empty){
                if(AddToInventory(Item)){ __Entities.Remove(new EntityKey(CollisionInfo2, (uint)CollisionInfo3)); }
            }
        }
    }
}
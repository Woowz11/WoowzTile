using WLO;
using WoowzTile;
using WoowzTile.Objects;
using static GOLUWorld.GOLUWorld_Objects;
using static GOLUWorld.GOLUWorld_Resources;

namespace GOLUWorld;

internal static class GOLUWorld_Values{
    #region Константы игры

        internal const string Game_Version = "0.7";
        internal const string Game_Name    = "GOLUWorld";

    #endregion

    #region Позиция

        /// <summary>
        /// Координаты смещения камеры
        /// </summary>
        internal static Vector2F Coordinates_Camera{
            get => __Coordinates_Camera;
            set{
                __Coordinates_Camera = value;
                
                Coordinates_Player = new Vector2I((int)(Game.SceneSize.X / 2F - Texture_Player_Body.Width  / 2F), (int)(Game.SceneSize.Y / 2F - Texture_Player_Body.Height / 2F));
        
                Coordinates_World = new Vector2I((int)(Coordinates_Camera.X + Game.SceneSize.X / 2F), (int)(Coordinates_Camera.Y + Game.SceneSize.Y / 2F));

                Coordinates_PlayerWorld = new Vector2I(Coordinates_Player.X - Coordinates_World.X, Coordinates_Player.Y - Coordinates_World.Y);

                Coordinates_PlayerWorldCenter = Coordinates_PlayerWorld + new Vector2I(8, 8);
                
                Coordinates_Beautiful = new Vector2I(-(Coordinates_World.X / 16) + 8, -(Coordinates_World.Y / 16) + 8);
            }
        }
        private static Vector2F __Coordinates_Camera;
        
        /// <summary>
        /// Координаты игрока на ЭКРАНЕ
        /// </summary>
        internal static Vector2I Coordinates_Player{ get; private set; }
        
        /// <summary>
        /// Координаты игрока в МИРЕ
        /// </summary>
        internal static Vector2I Coordinates_PlayerWorld{ get; private set; }
        
        /// <summary>
        /// Координаты игрока в МИРЕ (в центре игрока)
        /// </summary>
        internal static Vector2I Coordinates_PlayerWorldCenter{ get; private set; }
        
        /// <summary>
        /// Координаты МИРА
        /// </summary>
        internal static Vector2I Coordinates_World{ get; private set; }
        
        /// <summary>
        /// Красивые координаты, центр 0:0
        /// </summary>
        internal static Vector2I Coordinates_Beautiful{ get; private set; }

    #endregion

    #region Игрок

        /// <summary>
        /// Игрок двигается?
        /// </summary>
        internal static bool Player_Moving => Player_MovingDirection != Vector2I.Zero;
        
        /// <summary>
        /// Направление движения игрока
        /// </summary>
        internal static Vector2I Player_MovingDirection = Vector2I.Zero;

        /// <summary>
        /// В последний раз куда двигался игрок?
        /// </summary>
        internal static Direction4 Player_LastDirection = Direction4.Right;
        
        /// <summary>
        /// Игрок за пределами карты?
        /// </summary>
        internal static bool Player_OutBounds = false;
        
        /// <summary>
        /// Коллизия с которым сейчас взаимодействует игрок
        /// </summary>
        internal static CollisionLayer Player_InteractingCollision = CollisionLayer.None;
        
        /// <summary>
        /// Информация 1 об коллизии, с которой взаимодействует игрок
        /// </summary>
        internal static byte Player_CollisionInfo1 = 0;
        
        /// <summary>
        /// Информация 2 об коллизии, с которой взаимодействует игрок
        /// </summary>
        internal static Vector2I Player_CollisionInfo2 = Vector2I.Zero;
        
        /// <summary>
        /// Информация 3 об коллизии, с которой взаимодействует игрок
        /// </summary>
        internal static int Player_CollisionInfo3 = 0;

        /// <summary>
        /// Пол на котором сейчас стоит игрок
        /// </summary>
        internal static Block Player_Floor;
        
        /// <summary>
        /// Потолок под которым находится игрок
        /// </summary>
        internal static Ceiling Player_Ceiling;
        
        /// <summary>
        /// Максимальное здоровье
        /// </summary>
        internal const  uint Player_HealthMax   = 100;
        
        /// <summary>
        /// Низкое здоровье
        /// </summary>
        internal const  uint Player_HealthLow = 30;
        
        /// <summary>
        /// Текущее здоровье
        /// </summary>
        internal static uint Player_Health = Player_HealthMax;
        
        /// <summary>
        /// Игрок мёртв?
        /// </summary>
        internal static bool Player_Dead => Player_Health == 0;
        
        /// <summary>
        /// Всего слотов в инвентаре
        /// </summary>
        internal const  byte Player_InventorySlotsMax     = 12;

        /// <summary>
        /// Выбранный слот в инвентаре
        /// </summary>
        internal static byte Player_InventorySelectedSlot = 0;

        /// <summary>
        /// Инвентарь игрока
        /// </summary>
        internal static readonly T_Item[] Player_Inventory = new T_Item[Player_InventorySlotsMax];

        /// <summary>
        /// Предмет в руках
        /// </summary>
        internal static T_Item Player_ItemInHands{
            get => Player_Inventory[Player_InventorySelectedSlot];
            set => Player_Inventory[Player_InventorySelectedSlot] = value;
        }

        /// <summary>
        /// Когда последний раз лечили
        /// </summary>
        internal static float Player_LastTimeWereTreatedTimer = 0;

        /// <summary>
        /// Гниение
        /// </summary>
        internal static float Player_Rotting = 0;
        
        /// <summary>
        /// Максимальное значение эмоции
        /// </summary>
        internal const  uint Emotion_Max = 100;
        
        /// <summary>
        /// Счастье
        /// </summary>
        internal static uint Emotion_Happiness = Emotion_Max;

        /// <summary>
        /// Текущая мысль игрока
        /// </summary>
        internal static string Player_Thought = "";
        
        /// <summary>
        /// Таймер мысли
        /// </summary>
        internal static float Player_ThoughtTimer   = 0;
        
        /// <summary>
        /// Основа мысли
        /// </summary>
        internal static T_Thoughts Player_ThoughtContext = T_Thoughts.Idle;
        
        /// <summary>
        /// Таймер моргания игрока
        /// </summary>
        internal static float Player_BlinkTimer = 0;
        
        /// <summary>
        /// Визуально текстура игрока отзеркаленная?
        /// </summary>
        internal static bool Player_TextureFlipped = false;

        /// <summary>
        /// Таймер атаки (0-1)
        /// </summary>
        internal static float Player_AttackTimer = 0;

        /// <summary>
        /// Направление атаки
        /// </summary>
        internal static Direction4 Player_AttackDirection;
        
        /// <summary>
        /// Рендерить коллизии?
        /// </summary>
        internal static bool Cheat_RenderColliders = false;
        
        /// <summary>
        /// Игрок бессмертный?
        /// </summary>
        internal static bool Cheat_Immortality = false;
        
        /// <summary>
        /// Игнорировать коллизии?
        /// </summary>
        internal static bool Cheat_IgnoreColliders = false;
        
        /// <summary>
        /// Ускоренное время?
        /// </summary>
        internal static bool Cheat_FastTime = false;
        
        /// <summary>
        /// Во сколько раз ускорить время
        /// </summary>
        internal static uint Cheat_FastTime_Value = 5;

        /// <summary>
        /// Отключает лимит мира
        /// </summary>
        internal static bool Cheat_DisableWorldLimit = false;
        
        
    #endregion

    #region Мир

        /// <summary>
        /// Текущий сид мира
        /// </summary>
        internal static uint World_Seed;
        
        /// <summary>
        /// Текущий уровень
        /// </summary>
        internal static T_World  World_Type = T_World.None;
        
        /// <summary>
        /// Размер мира (в блоках 16x16)
        /// </summary>
        internal static Vector2U World_Size = new Vector2U(100, 100);
        
        /// <summary>
        /// Размер мира в пикселях
        /// </summary>
        internal static Vector2U World_SizeWorld => World_Size * 16;

        /// <summary>
        /// Максимальное время
        /// </summary>
        internal const  float World_TimeMax = 24;
        
        /// <summary>
        /// Текущее время в мире
        /// </summary>
        internal static float World_Time = World_TimeMax / 2;
        
        /// <summary>
        /// Скорость времени
        /// </summary>
        internal static float World_TimeSpeed = 0.01f;
        
        /// <summary>
        /// Фаза дня (0 - ночь, 1 - день)
        /// </summary>
        internal static float World_DayPhase  => WL.Math.Clamp01(WL.Math.DSin((World_Time - 6) / 24 * WL.Math.PI * 2));

        /// <summary>
        /// Остановить игровое время?
        /// </summary>
        internal static bool  World_StopGameTime = false;
        
        /// <summary>
        /// DeltaTick но для мира
        /// </summary>
        internal static float World_DeltaTick = 0;
        
        /// <summary>
        /// Цвет заднего фона
        /// </summary>
        internal static ColorB World_BackgroundColor = ColorB.White;
        
        /// <summary>
        /// Таймер анимаций
        /// </summary>
        internal static float World_AnimationTimer = 0;
        
        /// <summary>
        /// Течение
        /// </summary>
        internal static Vector2F World_Flow = Vector2F.Zero;

        /// <summary>
        /// Максимальная скорость течения
        /// </summary>
        internal const float World_FlowMax = 2;
        
        /// <summary>
        /// Все декали в мире
        /// </summary>
        internal static readonly List<Decal> World_Decals = [];
        
        /// <summary>
        /// Все блоки в мире
        /// </summary>
        internal static readonly Dictionary<Vector2I, Block> World_Blocks = [];
        
        /// <summary>
        /// Все потолки в мире
        /// </summary>
        internal static readonly Dictionary<Vector2I, Ceiling> World_Ceilings = [];
        
        /// <summary>
        /// Все сущности в мире
        /// </summary>
        internal static readonly Dictionary<EntityKey, Entity> World_Entities = [];
        
    #endregion

    #region Интерфейс

        /// <summary>
        /// Игрок в главном меню?
        /// </summary>
        internal static bool UI_InMainMenu = true;
        
        /// <summary>
        /// Текущий интерфейс
        /// </summary>
        internal static T_Interface UI_Interface = T_Interface.None;
        
        /// <summary>
        /// Выбранная кнопка в меню
        /// </summary>
        internal static byte UI_MenuSelectedButton = 0;

    #endregion
}
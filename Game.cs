using System.Drawing;
using WL;
using WLO;
using WoowzTile.Objects;

namespace WoowzTile;

public abstract class Game{
    /// <summary>
    /// Вызывается при загрузке игры
    /// </summary>
    public abstract void Start();

    /// <summary>
    /// Вызывается при остановке игры
    /// </summary>
    public abstract void Stop();

    /// <summary>
    /// Вызывается каждый кадр
    /// </summary>
    public abstract void Update(TickData TD);

    /// <summary>
    /// Рендер
    /// </summary>
    public abstract void Render(TickData TD, Image.ImageContext C);
    
    /// <summary>
    /// Цвет заднего фона
    /// </summary>
    public virtual ColorB BackgroundColor(){ return ColorB.Black; }

    /// <summary>
    /// Название игры
    /// </summary>
    public abstract string Name();

    /// <summary>
    /// Дополнительная информация в названии окна
    /// </summary>
    public virtual string WindowTitle(){ return Name(); }
    
    /// <summary>
    /// Нажатие клавиши
    /// </summary>
    /// <param name="Key">Клавиша</param>
    /// <param name="Down">Нажатие?</param>
    public virtual void KeyPress(Key Key, bool Down){}
    
    /// <summary>
    /// Клавиша нажата?
    /// </summary>
    /// <param name="Key">Клавиша</param>
    public static bool KeyPressed(Key Key) => Program.__Window.KeyboardKeyPressed(Key);

    /// <summary>
    /// Рендер коллайдеров
    /// </summary>
    public static void RenderColliders(Image.ImageContext C){
        foreach(Collider Collider in Colliders){
            CollisionLayer Layer = Collider.Layer;
            
            ColorB Color;

            switch(Layer){
                case CollisionLayer.None:
                    Color = ColorB.Black.SetA(128);
                    break;
                case CollisionLayer.All:
                    Color = ColorB.White;
                    break;
                default:
                    int Index = Collider.GetLayerIndex(Layer);
                    float HUE = (Index % 16) / 16f;
                    Color = ColorB.FromHSV(HUE, 0.8f, 1f);
                    break;
            }
            
            C.Border(Collider.X, Collider.Y, Collider.W, Collider.H, 1, Color);
        }
        
        foreach((Collider, bool) Collision in Collisions){
            C.Border(Collision.Item1.X, Collision.Item1.Y, Collision.Item1.W, Collision.Item1.H, 1, Collision.Item2 ? new ColorB(0, (byte)WL.Math.Random.Fast_Int(128, 255)) : new ColorB((byte)WL.Math.Random.Fast_Int(128, 255)));
        }
    }
    
    /// <summary>
    /// Очистить коллайдеры
    /// </summary>
    public static void ClearColliders(){
        Colliders .Clear();
        Collisions.Clear();
    }
    private static readonly List<Collider> Colliders = [];

    /// <summary>
    /// Добавить коллайдер
    /// </summary>
    public static void AddCollider(Collider Collider){
        Colliders.Add(Collider);
    }

    /// <summary>
    /// Проверяет, есть ли столкновения с коллайдерами
    /// </summary>
    public static bool Collision(Collider Collider, out Collider? HitCollider, bool ExceptInfoSecond = false){
        HitCollider = null;
        bool Result = false;

        foreach(Collider Collider__ in Colliders){
            if(!Collider.CanCollide(Collider__)){ continue; }
            
            if(ExceptInfoSecond && Collider__.InfoSecond == Collider.InfoSecond){ continue; }

            if(Collider__.Intersects(Collider)){
                HitCollider = Collider__;
                Result = true;
                break;
            }
        }
        
        Collisions.Add((Collider, Result));
        return Result;
    }
    private static readonly List<(Collider, bool)> Collisions = [];

    /// <summary>
    /// Размер сцены
    /// </summary>
    public static Vector2U SceneSize = Program.__Scene.Size;

    /// <summary>
    /// Выйти из запущеной игры
    /// </summary>
    public static void Quit(){
        Program.LoadGame(null);
    }
}
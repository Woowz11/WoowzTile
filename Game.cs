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
            C.Border(Collider.X, Collider.Y, Collider.W, Collider.H, 1, ColorB.Red);
        }
        
        foreach((Collider, bool) Collision in Collisions){
            C.Border(Collision.Item1.X, Collision.Item1.Y, Collision.Item1.W, Collision.Item1.H, 1, Collision.Item2 ? ColorB.Green : ColorB.Blue);
        }
    }
    
    /// <summary>
    /// Очистить коллайдеры
    /// </summary>
    public static void ClearColliders(){
        Colliders.Clear();
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
    public static bool Collision(Collider Collider){
        bool Result = false;

        foreach(Collider Collider__ in Colliders){
            if(Collider__.Intersects(Collider)){ Result = true; break; }
        }
        
        Collisions.Add((Collider, Result));
        return Result;
    }
    private static readonly List<(Collider, bool)> Collisions = [];

    /// <summary>
    /// Размер сцены
    /// </summary>
    public static Vector2U SceneSize = Program.__Scene.Size;
}
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
            int X0 = Collider.X;
            int Y0 = Collider.Y;
            int X1 = Collider.X + (int)Collider.W - 1;
            int Y1 = Collider.Y + (int)Collider.H - 1;

            for(int X = X0; X <= X1; X++){
                if(X >= 0 && X < C.Width){
                    if(Y0 >= 0 && Y0 < C.Height){ C[(uint)X, (uint)Y0] = ColorB.Red; }
                    if(Y1 >= 0 && Y1 < C.Height){ C[(uint)X, (uint)Y1] = ColorB.Red; }
                }
            }

            for(int Y = Y0 + 1; Y < Y1; Y++){
                if(Y >= 0 && Y < C.Height){
                    if(X0 >= 0 && X0 < C.Width){ C[(uint)X0, (uint)Y] = ColorB.Red; }
                    if(X1 >= 0 && X1 < C.Width){ C[(uint)X1, (uint)Y] = ColorB.Red; }
                }
            }
        }
        
        foreach((Collider, bool) Collision in Collisions){
            int X0 = Collision.Item1.X;
            int Y0 = Collision.Item1.Y;
            int X1 = Collision.Item1.X + (int)Collision.Item1.W  - 1;
            int Y1 = Collision.Item1.Y + (int)Collision.Item1.H - 1;

            ColorB Color = Collision.Item2 ? ColorB.Green : ColorB.Blue;
            
            for(int X = X0; X <= X1; X++){
                if(X >= 0 && X < C.Width){
                    if(Y0 >= 0 && Y0 < C.Height){ C[(uint)X, (uint)Y0] = Color; }
                    if(Y1 >= 0 && Y1 < C.Height){ C[(uint)X, (uint)Y1] = Color; }
                }
            }

            for(int Y = Y0 + 1; Y < Y1; Y++){
                if(Y >= 0 && Y < C.Height){
                    if(X0 >= 0 && X0 < C.Width){ C[(uint)X0, (uint)Y] = Color; }
                    if(X1 >= 0 && X1 < C.Width){ C[(uint)X1, (uint)Y] = Color; }
                }
            }
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
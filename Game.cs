using WLO;

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
    /// <returns></returns>
    public virtual ColorB BackgroundColor(){ return ColorB.Black; }
}
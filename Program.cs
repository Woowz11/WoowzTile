using System.Runtime.CompilerServices;
using WLO;

namespace WoowzTile;

public class Program{
    public static int Main(string[] Args){
        try{
            WL.WoowzLib.Start(new WoowzLibInfo(Name: "WoowzTile"));

            __Window = new Window(BackgroundColor: ColorB.Gray);

            __Window.OnResize += (_, W, H) => {
                RenderWindow();
            };
            
            Screen = new Panel(Name: "Экран");
            __Window.Add(Screen);

            Screen.Anchor_X = 0;
            Screen.Anchor_Y = 0;

            Screen.Anchor_Height = 0.99f;

            __Scene = new Image(256, ColorB.Black);
            Screen.Image = __Scene;

            __Window.OnKeyboardDown += (_, Key, Code) => {
                try{
                    LoadedGame?.KeyPress(Key, true);   
                }catch(Exception e){
                    Logger.Error("Произошла ошибка при нажатии клавиши [" + Key + "]!", e);
                }
            };
            __Window.OnKeyboardUp += (_, Key, Code) => {
                try{
                    LoadedGame?.KeyPress(Key, false);   
                }catch(Exception e){
                    Logger.Error("Произошла ошибка при отжатии клавиши [" + Key + "]!", e);
                }
            };
            
            LoadGame(new GOLUWorld.GOLUWorld());
            
            double Timer = 1000;
            float __FPS = -1;
            while(__Window.Alive){
                WL.System.Tick.LimitFPS(1, 60, TD => {
                    __TD = TD;
                    
                    Timer += TD.DeltaTimeS;
                    if(Timer > 0.25f){ __FPS = WL.Math.Round((float)TD.FPS, 2); Timer = 0; }

                    try{
                        __Window.Title = WL.WoowzLib.ProjectInfo.Name + " [" + __FPS + "] [" + (LoadedGame?.WindowTitle() ?? "Игра не загружена!") + "]";
                        
                        LoadedGame?.Update(TD);
                    }catch(Exception e){
                        Logger.Error("Произошла ошибка при обновлении игры!", e);
                    }
                    
                    RenderWindow();
                });
                
                WL.WoowzLib.Update();
            }
            
            LoadGame(null);
        }catch(Exception e){
            Logger.Fatal("Произошла ошибка в самом приложении!", e);
        }

        return 0;
    }

    public static Window __Window;
    
    private static Panel Screen;

    public static Image __Scene;

    private static Game? LoadedGame;

    private static TickData __TD;
    
    public static void RenderWindow(){
        try{
            Screen.Width = Screen.Height_Final;

            if(LoadedGame != null){
                try{
                    __Scene.Change(C => {
                        C.Fill(LoadedGame.BackgroundColor());
                        LoadedGame.Render(__TD, C); 
                    });
                }catch(Exception e){
                    Logger.Error("Произошла ошибка при рендере игры!", e);
                }
                
                __Window.Render();
            }else{
                __Window.RenderMessage("Игра не загружена!", ColorB.Blue);
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при рендере!", e);
        }
    }

    public static void LoadGame(Game? Game){
        try{
            if(LoadedGame != null){
                try{
                    LoadedGame.Stop();   
                }catch(Exception e){
                    Logger.Error("Произошла ошибка при разгрузке игры!", e);
                }
            }
            
            LoadedGame = Game;

            if(LoadedGame != null){
                try{
                    LoadedGame.Start();   
                }catch(Exception e){
                    Logger.Error("Произошла ошибка при загрузке игры!", e);
                }
            }
        }catch(Exception e){
            throw new Exception("Произошла ошибка при загрузке игры [" + Game + "]!", e);
        }
    }
}
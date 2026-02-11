using System.Runtime.CompilerServices;
using WLO;
using WoowzTile.Games;

namespace WoowzTile;

public class Program{
    public static int Main(string[] Args){
        try{
            WL.WoowzLib.Start(new WoowzLibInfo(Name: "WoowzTile"));

            Window = new Window(BackgroundColor: ColorB.Gray);

            Window.OnResize += (_, W, H) => {
                RenderWindow();
            };
            
            Screen = new Panel(Name: "Экран");
            Window.Add(Screen);

            Screen.Anchor_X = 0;
            Screen.Anchor_Y = 0;

            Screen.Anchor_Height = 0.99f;

            Scene = new Image(256, 256, ColorB.Black);
            Screen.Image = Scene;
            
            LoadGame(new GOLUWorld());
            
            double Timer = 1000;
            while(Window.Alive){
                WL.System.Tick.LimitFPS(1, 30, TD => {
                    __TD = TD;
                    
                    Timer += TD.DeltaTimeS;
                    if(Timer > 0.25f){ Window.Title = WL.WoowzLib.ProjectInfo.Name + " [" + WL.Math.Round((float)TD.FPS, 2) + "]"; Timer = 0; }

                    try{
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

    private static Window Window;
    
    private static Panel Screen;

    private static Image Scene;

    private static Game? LoadedGame;

    public static TickData __TD;
    
    public static void RenderWindow(){
        try{
            Screen.Width = Screen.Height_Final;

            if(LoadedGame != null){
                try{
                    Scene.Change(C => {
                        C.Fill(LoadedGame.BackgroundColor());
                        LoadedGame.Render(__TD, C); 
                    });
                }catch(Exception e){
                    Logger.Error("Произошла ошибка при рендере игры!", e);
                }
                
                Window.Render();
            }else{
                Window.RenderMessage("Игра не загружена!", ColorB.Blue);
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
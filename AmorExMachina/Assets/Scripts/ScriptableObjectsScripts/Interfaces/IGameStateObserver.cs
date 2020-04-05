public enum GameState
{
    WON,
    LOST
}

public interface IGameStateObserver
{
    void GameStateNotify(GameState gameState);
}
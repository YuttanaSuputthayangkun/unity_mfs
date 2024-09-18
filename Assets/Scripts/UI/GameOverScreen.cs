
#nullable enable

namespace UI
{
    public class GameOverScreen
    {
        private readonly GameOverScreenComponent _gameOverScreenComponent;

        public GameOverScreen(GameOverScreenComponent gameOverScreenComponent)
        {
            _gameOverScreenComponent = gameOverScreenComponent;
        }

        public void SetActive(bool isActive)
        {
            _gameOverScreenComponent.gameObject.SetActive(isActive); 
        }
    }
}

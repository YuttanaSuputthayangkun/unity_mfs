namespace State
{
    public class GameState : IState
    {
        public StateType GetStateType() => StateType.GameState;
    }
}

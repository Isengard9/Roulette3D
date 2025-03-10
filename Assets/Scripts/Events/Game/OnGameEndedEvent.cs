using NCGames.Interfaces;

namespace NCGames.Events.Game
{
    public class OnGameEndedEvent : IEvent
    {
        public int WinningNumber { get; set; }
    }
}
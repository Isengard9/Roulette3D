using NCGames.Data;
using NCGames.Enums;
using NCGames.Interfaces;

namespace NCGames.Events.Popup
{
    public class OnTokenPopupOpenEvent :IEvent
    {
        public BetData BetData { get; set; }
    }
}
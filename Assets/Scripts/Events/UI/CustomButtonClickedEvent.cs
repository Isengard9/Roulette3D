using NCGames.Interfaces;
using UnityEngine.UI;

namespace NCGames.Events.UI
{
    public class CustomButtonClickedEvent :IEvent
    {
        public Button ClickedButton { get; set; }
    }
}
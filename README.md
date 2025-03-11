# NCGames Roulette System

## 📋 Project Overview

NCGames Roulette is a feature-rich casino roulette simulation developed in Unity, designed with a modular, event-driven architecture. The system offers a complete betting experience with an intuitive UI, comprehensive bet tracking, and persistent statistics.

## 🏗️ Core Architecture

The project follows a service-oriented architecture with clear separation of concerns:

### Key Components

#### Services Layer
- **BetService**: Manages bet placement, retrieval, updates, and payouts
- **EventPublisherService**: Provides decoupled communication between components
- **SaveLoadSystem**: Handles persistent storage of game data and statistics

#### Controller Layer
- **BetPopupController**: Manages the main betting interface with animation states
- **TokenPopupController**: Controls chip selection and bet amount management
- **TokenUpgradeController**: Handles individual token denomination adjustments
- **BetButtonController**: Manages clickable bet placement areas on the table

#### Data Layer
- **BetData**: Stores information about bet types and associated numbers
- **UserStatistics**: Contains historical game data and player performance metrics
- **GameResult**: Records individual game outcomes with detailed bet information

## 🔧 Technical Implementation

### Event-Driven Communication

The system employs an event-driven architecture to maintain loose coupling:

```csharp
ServiceContainer.Instance.EventPublisherService.Subscribe<OnGameEndedEvent>(OnGameEnded);
ServiceContainer.Instance.EventPublisherService.Subscribe<OnTokenPopupOpenEvent>(OnTokenPopupOpen);
```

Events include:
- Game state events (`OnGameStartedEvent`, `OnGameEndedEvent`)
- UI events (`OnTokenPopupOpenEvent`, `OnBetPopupCloseEvent`)
- Bet events (`OnBetPlacedEvent`, `OnBetRemovedEvent`)

### Token Management System

The token system allows players to place bets using different chip denominations:
- Each `TokenUpgradeController` represents a specific chip value (X1, X5, X10, etc.)
- Players can increment/decrement the count of each token type
- The system automatically calculates the total bet value based on token counts

```csharp
public void UpdateAmount()
{
    _totalAmount = 0;
    foreach (var controller in _tokenUpgradeControllers)
    {
        _totalAmount += controller.TokenAmount * (int)controller.TokenType;
    }
    UpdateAmountText();
}
```

### Persistent Save System

The game includes a robust JSON-based persistence system that:
- Records every game session with detailed statistics
- Tracks all placed bets, their outcomes, and winnings
- Maintains historical balance data
- Stores files securely in `Application.persistentDataPath`
- Automatically loads previous data on application start

Data structure:
```json
{
  "totalGamesPlayed": 42,
  "totalBetAmount": 2350,
  "totalWinAmount": 2175,
  "gameHistory": [
    {
      "timestamp": "2023-08-24T19:45:32.1234",
      "winningNumber": 17,
      "balanceBefore": 5000,
      "balanceAfter": 5350,
      "totalBetAmount": 150,
      "totalWinAmount": 500,
      "bets": [
        {
          "betType": "Straight",
          "amount": 50,
          "numbers": [17],
          "isWin": true,
          "winAmount": 1750
        }
      ]
    }
  ]
}
```

## 🎲 Betting System

The system supports standard roulette bet types:

| Bet Type | Description | Example Numbers | Payout |
|----------|-------------|-----------------|--------|
| Straight | Single number | [7] | 35:1 |
| Split | Two adjacent numbers | [17, 18] | 17:1 |
| Street | Three numbers in a row | [4, 5, 6] | 11:1 |
| Corner | Four numbers in a square | [1, 2, 4, 5] | 8:1 |
| Six Line | Six numbers in two rows | [28-33] | 5:1 |
| Column | Twelve numbers (column) | [3, 6, 9, 12...] | 2:1 |
| Dozen | Twelve numbers (range) | [1-12] | 2:1 |
| Red/Black | Color bet | [all red/black] | 1:1 |
| Even/Odd | Number property | [all even/odd] | 1:1 |
| High/Low | Number range | [1-18]/[19-36] | 1:1 |

## 🛠️ Code Quality & Best Practices

The codebase demonstrates several software engineering best practices:

### Null Safety
```csharp
if (_tokenPopupAnimator != null)
    _tokenPopupAnimator.SetTrigger(Open);
```

### Proper Resource Management
```csharp
private void OnDisable()
{
    ServiceContainer.Instance?.EventPublisherService.Unsubscribe<OnTokenPopupOpenEvent>(OnTokenPopupOpen);

    if (_closeButton != null)
        _closeButton.OnButtonClicked -= OnCloseButtonClicked;
}
```

### Comprehensive Validation
```csharp
private void ValidateReferences()
{
    if (_tokenPopupAnimator == null)
        Debug.LogError($"Animator reference is missing on {gameObject.name}", this);

    if (_closeButton == null)
        Debug.LogError($"Close button reference is missing on {gameObject.name}", this);
}
```

### Structured Error Handling
```csharp
try
{
    string jsonData = File.ReadAllText(path);
    _userStatistics = JsonUtility.FromJson<UserStatistics>(jsonData);
    Debug.Log($"Statistics loaded from: {path}");
}
catch (Exception ex)
{
    Debug.LogError($"Failed to load statistics: {ex.Message}");
    _userStatistics = new UserStatistics();
}
```

### Self-Registering Components
```csharp
private void InitializeReferences()
{
    _tokenPopupController = FindObjectOfType<TokenPopupController>();
    
    if (_tokenPopupController != null)
        _tokenPopupController.AddTokenUpgradeController(this);
}
```

## 🎮 UI/UX Design

The UI system features:
- Animated transitions between game states
- Clear visual feedback for bet placement
- Token management with intuitive controls
- Dynamic text updates for bet amounts and balances
- Animator-driven state transitions for smooth UX

```csharp
// Animation parameter hashes for better performance
private static readonly int Close = Animator.StringToHash("Close");
private static readonly int Open = Animator.StringToHash("Open");
```

## 💻 Technical Requirements

- Unity 2020.3.57 LTS or newer
- .NET Standard 2.0 or higher
- TextMeshPro package for UI text rendering

## 📁 Development Details

### Namespace Organization
- `NCGames.Controllers`: UI and gameplay controllers
- `NCGames.Services`: Core service implementations
- `NCGames.Data`: Data structures and models
- `NCGames.Events`: Event definitions
- `NCGames.Enums`: Type definitions and enumerations
- `NCGames.Managers`: System-level managers

## 🧪 Testing Considerations

- Component references are validated on startup
- Extensive error logging for debugging
- Graceful handling of missing or corrupted save data
- Proper cleanup of event subscriptions

## 🚀 Future Enhancements

- Advanced statistics visualization
- Multiple game modes and table variations
- Customizable betting limits and rules
- Achievement system
- Player profiles with avatars

## 📄 License

© NCGames. All rights reserved.

---


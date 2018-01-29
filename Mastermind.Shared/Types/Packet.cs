namespace Mastermind.Shared.Types
{
  public enum MessageType
  {
    Default,
    AskInitialization,
    AskGameMode,
    AskSecretCombination,
    AskPlayerCombination,
    SetInitialization,
    SetSecretCombination,
    SetGameMode,
    RightColorsStatus,
    PlayerCombinationEntered,
    GameWin,
    GameEnd
  }
}

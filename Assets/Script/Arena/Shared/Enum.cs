public enum PlayerState
{
    Idle,
    Walk
}

public enum ConnectStatus
{
    Undefined,
    Success,
    ServerFull,
    GameInProgress,
    LoggedInAgain,
    UserRequestedDisconnect,
    GenericDisconnect
}
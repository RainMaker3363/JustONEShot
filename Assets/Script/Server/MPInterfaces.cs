public interface MPUpdateListener
{
    void UpdatePositionReceived(string participantId, float posX, float posY, float posZ, float rotY);
    void FinishedReceived(string participantId, bool GameOver);
    void LeftRoomConfirmed();
    void PlayerLeftRoom(string participantId);
}

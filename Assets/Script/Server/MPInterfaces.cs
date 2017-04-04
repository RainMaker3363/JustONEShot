public interface MPUpdateListener
{
    void UpdatePositionReceived(string participantId, int messageNum, float posX, float posY, float posZ, float rotY);
    void FinishedReceived(string participantId, bool GameOver);
    void ItemStateReceived(int Index, bool GetItem);
    void LeftRoomConfirmed();
    void PlayerLeftRoom(string participantId);
    
}

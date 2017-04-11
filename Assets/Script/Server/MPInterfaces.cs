public interface MPUpdateListener
{
    void UpdatePositionReceived(string participantId, int messageNum, float posX, float posY, float posZ, float rotY);
    void FinishedReceived(string participantId, bool GameOver);
    void ItemStateReceived(int Index, bool GetItem);
    void ShootStateReceived(float x, float y, float z);
    void DeadEyeStateReceived(bool DeadEyeOn);
    void AniStateReceived(int AniState);
    void HPStateReceived(int HPState);

    void LeftRoomConfirmed();
    void PlayerLeftRoom(string participantId);
}

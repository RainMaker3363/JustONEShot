public interface MPUpdateListener
{
    void UpdateReceived(string participantId, float posX, float posY, float velX, float velY, float rotZ);
    void FinishedReceived(string participantId, bool GameOver);
}

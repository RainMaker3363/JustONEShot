public interface MPUpdateListener
{
    void UpdatePositionReceived(string participantId, int messageNum, float posX, float posY, float posZ, float rotY);
    void FinishedReceived(string participantId, bool GameOver);

    // ===========================================================================================================================
    // 이인용 멀티 함수
    // ===========================================================================================================================
    void ItemStateReceived(string participantId, int Index, bool GetItem);
    void ShootStateReceived(string participantId, bool ShootSuccess);
    void ShootVectorReceived(string participantId, float x, float y, float z);
    void DeadEyeStateReceived(string participantId, bool DeadEyeActive);
    void DeadEyeTimerStateReceived(string participantId, float DeadEyeTimer);
    void DeadEyeRespawnIndexReceived(string participantId, int index);

    void AniStateReceived(string participantId, int AniState);
    void HPStateReceived(string participantId, int HPState);

    void LeftRoomConfirmed();
    void PlayerLeftRoom(string participantId);

    void MultiStateWaitReceived(string participantId, bool Wait);
    void MultiStateSelectReceived(string participantId, bool Select);

    //void CharacterSelectStateReceived(int CharacterNumber);
    void WeaponSelectStateReceived(string participantId, int WeaponNumber);
    void BossRaidAlarm(bool Alarm);

    // ===========================================================================================================================
    // 다인용 멀티 함수
    // ===========================================================================================================================
    //void ItemStateReceived(string participantId, int Index, bool GetItem);
    //void ShootStateReceived(string participantId, bool ShootSuccess);
    //void ShootVectorReceived(string participantId, float x, float y, float z);
    //void DeadEyeStateReceived(string participantId, bool DeadEyeActive);
    //void DeadEyeTimerStateReceived(string participantId, float DeadEyeTimer);
    //void DeadEyeRespawnIndexReceived(string participantId, int index);

    //void AniStateReceived(string participantId, int AniState);
    //void HPStateReceived(string participantId, int HPState);

    //void MultiStateWaitReceived(string participantId, bool Wait);
    //void MultiStateSelectReceived(string participantId, bool Select);

    //void CharacterSelectStateReceived(string participantId, int CharacterNumber);
    //void WeaponSelectStateReceived(string participantId, int WeaponNumber);
}

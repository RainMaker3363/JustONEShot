public interface LBUpdateListener
{
    // 상대방 캐릭터의 대한 고유 번호를 받는 리스너
    void OpponentCharacterNumberReceive(string participantId, int characterNumber);

    // 상대방의 캐릭터의 대한 스킨 고유 번호를 받는 리스너
    void OpponentCharacterSkinNumberReceive(string participantId, int skinNumber);

    // 상대방이 선택한 맵을 자신만의 고유 랜덤값을 모두에게 보내주는 리스너
    void OpponentSelectedMapSeedReceive(string participantid, int SelectedMapNumber);

    // 상대방이 가지고 있는 고유 인덱스 데드아이 총알 위치 값을 모두에게 보내주는 리스너
    void OpponentDeadEyeBulletIndexReceive(string participantid, int DeadEyeIndex);

    // 매칭을 취소했을 경우 콜백되는 함수
    void LeftRoomConfirmed();
}
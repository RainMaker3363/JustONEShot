public interface LBUpdateListener
{
    // 상대방 캐릭터의 대한 고유 번호를 받는 리스너
    void OpponentCharacterNumberReceive(string participantId, int characterNumber);

    // 상대방의 캐릭터의 대한 스킨 고유 번호를 받는 리스너
    void OpponentCharacterSkinNumberReceive(string participantId, int skinNumber);

    // 매칭을 취소했을 경우 콜백되는 함수
    void LeftRoomConfirmed();
}
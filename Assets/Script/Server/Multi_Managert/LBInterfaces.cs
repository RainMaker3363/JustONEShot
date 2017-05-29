public interface LBUpdateListener
{
    // 상대방 캐릭터의 대한 고유 번호를 받는 리스너
    void OpponentCharacterNumberReceive(string participantId, int characterNumber);
}
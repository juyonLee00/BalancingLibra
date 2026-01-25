using UnityEngine;

namespace BalancingLibra.Tests
{
    public class AudioManagerTest : MonoBehaviour
    {
        [Header("Test Settings")]
        [Tooltip("테스트할 BGM 파일명 (확장자 제외)")]
        public string bgmName = "lobby"; 

        [Tooltip("테스트할 SFX 파일명 (확장자 제외)")]
        public string sfxName = "ui_button_click";

        [Range(0f, 1f)]
        public float testVolume = 1.0f;

    
        [ContextMenu("1. Play BGM")]
        private void TestPlayBGM()
        {
            if (!IsPlaying()) return;

            Debug.Log($"[AudioTester] BGM 재생 요청: {bgmName}");
            AudioManager.Instance.PlayBGM(bgmName, testVolume);
        }

        [ContextMenu("2. Stop BGM")]
        private void TestStopBGM()
        {
            if (!IsPlaying()) return;

            Debug.Log("[AudioTester] BGM 정지 요청");
            AudioManager.Instance.StopBGM();
        }

        [ContextMenu("3. Play SFX")]
        private void TestPlaySFX()
        {
            if (!IsPlaying()) return;

            Debug.Log($"[AudioTester] SFX 재생 요청: {sfxName}");
            AudioManager.Instance.PlaySFX(sfxName, testVolume);
        }

        [ContextMenu("4. Mute ON")]
        private void TestMuteOn()
        {
            if (!IsPlaying()) return;

            Debug.Log("[AudioTester] 전체 음소거");
            AudioManager.Instance.Mute(true);
        }

        [ContextMenu("5. Mute OFF")]
        private void TestMuteOff()
        {
            if (!IsPlaying()) return;

            Debug.Log("[AudioTester] 음소거 해제");
            AudioManager.Instance.Mute(false);
        }

        [ContextMenu("6. Clear Cache")]
        private void TestClearCache()
        {
            if (!IsPlaying()) return;

            Debug.Log("[AudioTester] 오디오 캐시 메모리 정리 요청");
            AudioManager.Instance.ClearCache();
        }

        // 안전 장치: 에디터 멈춰있을 때 실행 방지
        private bool IsPlaying()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[AudioTester] 게임 실행 중에만 테스트 가능합니다.");
                return false;
            }
            return true;
        }
    }
}
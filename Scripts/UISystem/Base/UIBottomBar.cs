using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using PPM = Marvrus.Util.PlayerPrefsManager;

namespace Marvrus.UI
{
    public class UIBottomBar : MonoBehaviour
    {
        enum STATE
        {
            None,
            Show,
            Hide
        }

        private STATE state = STATE.None;

        [Header("DoTween Animation")]
        public DOTweenAnimation ani;

        [Header("Open Upload")]
        public Button uploadBtn;

        [Header("Feed Tab")]
        public Toggle feedBtn;
        public GameObject feedBtn_icon;

        [Header("World Tab")]
        public Toggle worldBtn;

        [Header("Cumus Tab")]
        public Toggle cumusBtn;

        [Header("My Tab")]
        public Toggle myBtn;

        enum SelectedToggle
        {
            Feed = 0,
            World,
            Cumus,
            My
        }

        private SelectedToggle selectedToggle = SelectedToggle.Feed;

        public bool ShowUploadBtn
        {
            get { return uploadBtn.isActiveAndEnabled; }
            set { uploadBtn.SetActive(value); }
        }

        private void Start()
        {
            uploadBtn.onClick.AddListener(() =>
            {
                if (PPM.GuestLogin is true)
                {
                    UIManager.s.OpenSelectLogin();
                    return;
                }

                if (PPM.DontLookAgainOnboardingWrite is true)
                {
                    UIManager.s.OpenPopupWithData<UIPopup_UploadFeed>(true);
                }
                else
                {
                    UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_Onboarding_FirstWritten>(true);
                    PPM.DontLookAgainOnboardingWrite = true;
                }

            });

            feedBtn.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    if (selectedToggle == SelectedToggle.Feed)
                        return;

                    UIManager.s.OpenPopup<UIPopup_Feed>();
                    selectedToggle = SelectedToggle.Feed;
                }

                feedBtn_icon.SetActive(isOn);
            });

            worldBtn.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    if (selectedToggle == SelectedToggle.World)
                        return;

                    UIManager.s.OpenPopup<UIPopup_World>();
                    selectedToggle = SelectedToggle.World;
                }
            });

            cumusBtn.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    if (selectedToggle == SelectedToggle.Cumus)
                        return;

                    UIManager.s.OpenPopup<UIPopup_Cumus>();
                    selectedToggle = SelectedToggle.Cumus;
                }
            });

            myBtn.onValueChanged.AddListener((isOn) =>
            {
                if (selectedToggle == SelectedToggle.My)
                    return;

                if (isOn)
                {
                    UIManager.s.OpenPopupWithData<UIPopup_My>();
                    selectedToggle = SelectedToggle.My;
                }
            });
        }

        public void InitBottomMenu()
        {
            feedBtn.SetIsOnWithoutNotify(true);
            feedBtn_icon.SetActive(true);
            selectedToggle = SelectedToggle.Feed;

            worldBtn.SetIsOnWithoutNotify(false);
            cumusBtn.SetIsOnWithoutNotify(false);
            myBtn.SetIsOnWithoutNotify(false);
        }

        public void SetActive(bool _active)
        {
            if (state == STATE.Show && _active == true)
                return;

            if (state == STATE.Hide && _active == false)
                return;

            if (_active is true)
            {
                ani.DORestart();
                state = STATE.Show;
            }
            else
            {
                ani.DOPlayBackwards();
                state = STATE.Hide;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PPM = Marvrus.Util.PlayerPrefsManager;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class UITopBar : MonoBehaviour
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

        [Header("Contents")]
        public Button topBtn;

        [Header("Notification")]
        public Button notiBtn;
        public Transform notiBadge;
        public TextMeshProUGUI notiCount;

        private void Start()
        {
            notiBadge.SetActive(false);

            topBtn.onClick.AddListener(() =>
            {
                OnClickTopBtn();
            });

            notiBtn.onClick.AddListener(() =>
            {
                if (PPM.GuestLogin is true)
                {
                    UIManager.s.OpenSelectLogin();
                }
                else
                {
                    UIManager.s.OpenPopupWithData<UIPopup_Notification>(true);
                }
            });
        }

        private void OnClickTopBtn()
        {
            UIManager.s.FindPopup<UIPopup_Feed>().feedScroll.ResetPosition(_animation: true);
        }

        public void InitTopBar()
        {
            UpdateNotiCount();
        }

        public void SetActive(bool _active)
        {
            if (state == STATE.Show && _active == true)
                return;

            if (state == STATE.Hide && _active == false)
                return;

            if(_active is true)
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

        public void UpdateNotiCount()
        {
            NetworkManager.s.GetNotificationCount((result) =>
            {
                int count = DC.data_notificationCount;
                notiBadge.SetActive(count > 0);
                notiCount.text = $"{count}";
            }, (error) =>
            {
                Debug.LogError($"GetNotificationCount Erorr :: {error}");
            });
        }
    }
}
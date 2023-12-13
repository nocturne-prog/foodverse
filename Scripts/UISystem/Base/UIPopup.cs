using DG.Tweening;
using Marvrus.Util;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup : MonoBehaviour
    {
        public eMenuType menuType;
        public Button button_Close;

        protected DOTweenAnimation tween;
        protected float hideDelay = 0f;
        protected SafeArea safeArea;
        protected RectTransform safeAreaRect;

        public virtual void Awake()
        {
            if (button_Close != null)
            {
                button_Close.onClick.RemoveAllListeners();
                button_Close.onClick.AddListener(() => Close());
            }

            tween = transform.GetComponent<DOTweenAnimation>();

            if (tween != null)
            {
                hideDelay = tween.duration;
            }

            safeArea = transform.parent.GetComponent<SafeArea>();
            safeAreaRect = transform.parent.GetComponent<RectTransform>();
        }

        public virtual void UpdateData(params object[] args)
        {

        }

        public virtual void Close()
        {
            this.Hide();
        }

        public virtual void Hide()
        {
            if (tween != null)
            {
                tween.DOPlayBackwards();
            }

            Invoke(nameof(HideInvoke), hideDelay);
        }

        public void HideInvoke()
        {
            gameObject.SetActive(false);
            UIManager.s.ClosePopup(this);
        }

        public virtual void Show(bool _delay = false)
        {
            if (_delay)
                Invoke(nameof(ShowInvoke), 0.6f);
            else
                ShowInvoke();
        }

        public void ShowInvoke()
        {
            gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public virtual void MenuAction()
        {
            UIManager.s.TopMenu = (menuType == eMenuType.BOTH || menuType == eMenuType.Top_Only);
            UIManager.s.BottomMenu = (menuType == eMenuType.BOTH || menuType == eMenuType.Bottom_Only);
            UIManager.s.BottomMenu_Upload = false;
        }

        public virtual void Dispose()
        {
            if (button_Close is null)
                return;

            button_Close.onClick.RemoveAllListeners();
            button_Close = null;
        }
    }
}
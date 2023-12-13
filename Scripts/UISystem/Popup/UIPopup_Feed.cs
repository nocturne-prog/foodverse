using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PPM = Marvrus.Util.PlayerPrefsManager;

namespace Marvrus.UI
{
    public class UIPopup_Feed : UIPopup
    {
        [Header("Feed Scroll")]
        public Scroll.RecycleScroll feedScroll;

        public override void UpdateData(params object[] args)
        {
            if (PPM.DontLookAgainOnboarding is false)
                UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_Onboarding>(true);

            NetworkManager.s.GetMyProfile();
            Refresh();
        }

        public void Refresh(Action _callback = null)
        {
            Data.DataContainer.data_totalFeed = new List<Feed>();

            UIManager.s.Loading = true;
            NetworkManager.s.GetFeedList(0, (result) =>
            {
                feedScroll.ListStart(result.feeds.Length);
                UIManager.s.Loading = false;

                _callback?.Invoke();
            }, (error) =>
            {
                Debug.LogError($"GetFeedList Error :: {error}");
            });
        }

        public void Repaint()
        {
            feedScroll.Repaint();
        }

        public override void MenuAction()
        {
            base.MenuAction();
            UIManager.s.BottomMenu_Upload = true;
        }
    }
}
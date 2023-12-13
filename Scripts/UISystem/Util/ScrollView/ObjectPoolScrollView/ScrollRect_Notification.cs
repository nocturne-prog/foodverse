using System;
using System.Collections;
using System.Collections.Generic;
using Marvrus.UI;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.Scroll
{
    public class ScrollRect_Notification : ObjectPoolScrollView
    {
        UIPopup_Notification popup;

        protected override bool IsNextPage()
        {
            return DC.Has_NextPage_Notification;
        }

        protected override void NetworkAction(Action<int> _callback)
        {
            NetworkManager.s.GetNotificationList(DC.LastNotificationId,(result) =>
            {
                _callback(result.notifications.Length);

                if (popup is null)
                    popup = UIManager.s.FindPopup<UIPopup_Notification>();

                popup.SetPrevTitlePos();
            }, (error) =>
            {
                Debug.LogError($"GetNotificationList Error :: {error}");
            });
        }
    }
}
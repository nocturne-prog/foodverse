using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Marvrus.Scroll;
using UnityEngine;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class UIPopup_Notification : UIPopup
    {
        public ScrollRect_Notification scroll;

        public GameObject nonText_all;
        public GameObject nonText_new;

        public Transform itemParent;
        public Transform prevItemParent;

        public override void UpdateData(params object[] args)
        {
            base.UpdateData(args);
            Refresh();
        }

        public void Refresh()
        {
            DC.data_totalNotification = new List<Notification>();

            NetworkManager.s.GetNotificationList(0, (result) =>
            {
                scroll.ListStart(DC.data_notificationList.notifications.Length);

                nonText_all.SetActive(DC.data_totalNotification.Count == 0);
                nonText_new.SetActive(DC.Has_NewNotification is false & DC.data_totalNotification.Count != 0);
                SetPrevTitlePos();
            }, (error) =>
            {
                Debug.LogError($"GetNotificationList Error :: {error}");
            });
        }

        public void SetPrevTitlePos()
        {
            int index = DC.GetReadNotificationIndex();

            if (index < 0)
            {
                prevItemParent.SetActive(false);
            }
            else
            {
                // nonText_all, nonText_new, baseItem 때문에 3개 건너뜀.
                prevItemParent.SetSiblingIndex(DC.GetReadNotificationIndex() + 3);
                prevItemParent.SetActive(true);
            }
        }

        public override void Close()
        {
            base.Close();

            long[] ids = DC.data_totalNotification.Where(x => x.is_read == false)
                        .Select(x => x.id).ToArray();

            if (ids.Length > 0)
            {
                NetworkManager.s.ConfirmNotification(false, ids, (result) =>
                {
                    UIManager.s.GetTopMenu.UpdateNotiCount();
                }, (error) =>
                {
                });
            }
            else
            {
                UIManager.s.GetTopMenu.UpdateNotiCount();
            }
        }
    }
}


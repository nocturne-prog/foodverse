using System;
using UnityEngine;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.Scroll
{
    public class RecycleScroll_Feed : RecycleScroll
    {
        protected override void OnEndScroll(Action _update)
        {
            NetworkManager.s.GetFeedList(DC.LastFeedId, (result) =>
            {
                //Debug.Log(result.feeds.Length);
                SetContentSize(result.feeds.Length);
                _update();
            }, (error) =>
            {
                Debug.LogError($"GetFeedList Error :: {error}");
            });
        }

        protected override bool CheckEndPoint(float itemPos)
        {
            bool isEndPoint = base.CheckEndPoint(itemPos);
            bool isNext = DC.Has_NextPage_Feed;

            if (isNext)
            {
                return false;
            }
            else
            {
                return isEndPoint;
            }
        }


        bool isSendRefresh = false;
        protected override void OnTopScroll()
        {
            if (isSendRefresh is true)
                return;

            isSendRefresh = true;
            UIManager.s.RefreshFeed(() => isSendRefresh = false);
        }
    }
}
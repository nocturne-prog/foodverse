using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.Scroll
{
    public class RecycleScroll_MyFeed : RecycleScroll
    {
        protected override void OnEndScroll(Action _update)
        {
            NetworkManager.s.GetMyFeedList(DC.LastMyFeedId, (result) =>
            {
                SetContentSize(result.feeds.Length / 3);
                _update();
            }, (error) =>
            {
                Debug.LogError($"GetMyFeedList Error :: {error}");
            });
        }

        protected override bool CheckEndPoint(float itemPos)
        {
            bool isEndPoint = base.CheckEndPoint(itemPos);
            bool isNext = DC.Has_NextPage_MyFeed;

            if (isNext)
            {
                return false;
            }
            else
            {
                return isEndPoint;
            }
        }
    }
}
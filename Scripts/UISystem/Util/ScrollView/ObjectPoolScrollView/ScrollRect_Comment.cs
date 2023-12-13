using System;
using UnityEngine;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.Scroll
{
    public class ScrollRect_Comment : ObjectPoolScrollView
    {
        protected override bool IsNextPage()
        {
            return DC.Has_NextPage_Comment;
        }

        protected override void NetworkAction(Action<int> _callback)
        {
            NetworkManager.s.GetCommentList(DC.SelectedFeed.id, DC.LastCommentId, (result) =>
            {
                _callback(result.comments.Length);
            }, (error) =>
            {
                Debug.LogError($"GetCommentList Error :: {error}");
            });
        }
    }
}
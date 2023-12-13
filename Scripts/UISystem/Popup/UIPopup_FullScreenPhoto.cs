using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup_FullScreenPhoto : UIPopup
    {
        [Header("Scroll Snap")]
        public Scroll.ScrollSnap_Thumbnail snap;
        public DOTweenAnimation ani;

        public override void Awake()
        {
            base.Awake();
            snap.Init();
        }

        public override void UpdateData(params object[] args)
        {
            string[] data = (string[])args;

            snap.ItemCount = data.Length;
            snap.UpdateThumbnails(data);
        }

        public override void Close()
        {
            float duration = ani.duration;

            ani.DOPlayBackwardsAllById(Const.DG_KEY_UIPOPUP_FULLSCREENPHOTO);
            Invoke(nameof(CloseInvoke), duration);
        }

        private void CloseInvoke()
        {
            snap.Clear();
            base.Close();
        }

        private void OnDisable()
        {
            snap.Clear();
        }
    }
}
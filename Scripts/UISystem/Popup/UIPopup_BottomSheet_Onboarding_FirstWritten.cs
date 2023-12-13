using DG.Tweening;
using Marvrus.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup_BottomSheet_Onboarding_FirstWritten : UIPopup
    {
        [Header("Scroll Snap")]
        public Scroll.ScrollSnap snap;

        public Button nextBtn;
        public Button goUploadBtn;

        public DOTweenAnimation[] ani;
        public IndicatorItem[] indicatorItems;

        private int prevPage = 0;

        public override void Awake()
        {
            base.Awake();

            snap.scrollRect = snap.gameObject.GetComponent<ScrollRect>();
            snap.itemCount = 3;

            nextBtn.onClick.AddListener(() =>
            {
                snap.NextSnap();
            });

            goUploadBtn.onClick.AddListener(() =>
            {
                this.Close();
            });

            snap.OnValueChanged = (_index) =>
            {
                if (prevPage == _index)
                    return;

                for(int i = 0; i < ani.Length; i++)
                {
                    if( i == _index)
                    {
                        ani[i].DORestart();
                    }
                    else
                    {
                        ani[i].DORewind();
                    }

                    indicatorItems[i].On = i == _index;
                    nextBtn.SetActive(_index != 2);
                }

                prevPage = _index;
            };
        }

        public override void UpdateData(params object[] args)
        {
            UIManager.s.Dim = true;
            base.UpdateData(args);
        }

        public override void Hide()
        {
            UIManager.s.Dim = false;
            base.Hide();
            UIManager.s.OpenPopupWithData<UIPopup_UploadFeed>(true);
        }
    }
}



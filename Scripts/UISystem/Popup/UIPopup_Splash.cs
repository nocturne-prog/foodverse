using DG.Tweening;
using System.Linq;
using UnityEngine;

namespace Marvrus.UI
{
    public class UIPopup_Splash : UIPopup
    {
        private float duration = 0;

        public override void Awake()
        {
            base.Awake();

            DOTweenAnimation[] ani = transform.GetComponents<DOTweenAnimation>();
            duration = ani.Max(x => x.duration);
            GoNext();
        }

        public override void UpdateData(params object[] args)
        {
            base.UpdateData(args);
            UIManager.s.GetBottomMenu.InitBottomMenu();
            GoNext();
        }

        public void GoNext()
        {
            Invoke(nameof(NextStep), duration);
        }


        public void NextStep()
        {
            if (Util.PlayerPrefsManager.FirstCheckPermission)
            {
                UIManager.s.OpenPopupWithData<UIPopup_Entry>();
            }
            else
            {
                UIManager.s.OpenPopupWithData<UIPopup_Permission>();
            }
        }
    }
}
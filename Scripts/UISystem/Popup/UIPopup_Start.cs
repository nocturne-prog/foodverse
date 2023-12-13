using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup_Start : UIPopup
    {
        [Header("OnClick Start")]
        public Button startBtn;

        public override void Awake()
        {
            base.Awake();
            startBtn.onClick.AddListener(OnClickStart);
        }


        private void OnClickStart()
        {
            if (Util.PlayerPrefsManager.FirstCheckPermission)
            {
                UIManager.s.OpenPopupWithData<UIPopup_Entry>();
            }
            else
            {
                UIManager.s.OpenPopupWithData<UIPopup_Permission>();
            }

            Close();
        }
    }
}
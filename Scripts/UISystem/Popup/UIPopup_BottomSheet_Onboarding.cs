using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PPM = Marvrus.Util.PlayerPrefsManager;

namespace Marvrus.UI
{
    public class UIPopup_BottomSheet_Onboarding : UIPopup
    {
        public Button dontLookAgainBtn;
        public Button goProfileEditBtn;

        public override void Awake()
        {
            base.Awake();

            dontLookAgainBtn.onClick.AddListener(OnClickDontLookAgain);
            goProfileEditBtn.onClick.AddListener(GoProfileEdit);
        }

        public override void UpdateData(params object[] args)
        {
            UIManager.s.Dim = true;
            PPM.DontLookAgainOnboarding = true;
            base.UpdateData(args);
        }

        public void OnClickDontLookAgain()
        {
            Close();
        }

        public override void Close()
        {
            UIManager.s.Dim = false;
            base.Close();
        }

        public void GoProfileEdit()
        {
            if (PPM.GuestLogin is true)
            {
                UIManager.s.OpenSelectLogin();
            }
            else
            {

                UIManager.s.OpenPopupWithData<UIPopup_MyInfo_ProfileModify>(true, true);
            }

            Close();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup_SignUp_Over14Completed : UIPopup
    {
        public Button startButton;

        public override void Awake()
        {
            base.Awake();

            startButton.onClick.AddListener(OnClickStartButton);
        }

        private void OnClickStartButton()
        {
            // 14세 이상 회원가입하는 경우, Profile 생성.
            UIManager.s.OpenFeed();
        }
    }
}

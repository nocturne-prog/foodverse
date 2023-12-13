using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PPM = Marvrus.Util.PlayerPrefsManager;

namespace Marvrus.UI
{
    public class UIPopup_SignUp_SNS : UIPopup
    {
        public TextMeshProUGUI email;
        public TextMeshProUGUI id;

        public Button startButton;

        public override void Awake()
        {
            base.Awake();

            startButton.onClick.AddListener(OnClickStartButton);
        }

        public override void UpdateData(params object[] args)
        {
            email.text = (string)args[0];
            id.text = PPM.ID;
        }

        private void OnClickStartButton()
        {
            PPM.SNS_Login = true;
            UIManager.s.OpenFeed();
            // SNS로 처음 로그인하는 경우, Profile 생성.
        }
    }
}

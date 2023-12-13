using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.UI
{
    public class UIPopup_BottomSheet_PhoneAuth_Protector : UIPopup
    {
        public Toggle agreement;
        public Button privacyButton;
        public Button nextButton;

        private WebViewObject webviewobj = null;
        private Protocol.Request.SignUp signupData = new Protocol.Request.SignUp();

        public override void Awake()
        {
            base.Awake();

            agreement.onValueChanged.AddListener((_isOn) => { nextButton.gameObject.SetActive(_isOn); });
            privacyButton.onClick.AddListener(() =>
            {
                Close();
                UIManager.s.OpenPopupWithData<UIPopup_PrivacyPolicy_Under14>(true);
            });
            nextButton.onClick.AddListener(OnClickNextButton);
        }

        public override void UpdateData(params object[] args)
        {
            UIManager.s.Dim = true;
            if (args.Length > 0) signupData = (Protocol.Request.SignUp)args[0];
        }

        public override void Close()
        {
            base.Close();
            UIManager.s.Dim = false;
        }

        private void OnClickNextButton()
        {
            string strurl = null;
#if RELEASE
            strurl = "*****";
#else
            strurl = "*****";
#endif
            if (webviewobj != null)
            {
                Destroy(webviewobj.gameObject);
                webviewobj = null;
            }
            webviewobj = new GameObject("WebViewObject").AddComponent<WebViewObject>(); //웹뷰로 불러오기
            webviewobj.Init((msg) =>
            {
                if (msg.Equals("*****"))
                {
                    if (webviewobj.CanGoBack())
                    {
                        webviewobj.GoBack();
                    }
                    else
                    {
                        DestroyImmediate(webviewobj.gameObject);
                        if (webviewobj != null) webviewobj = null;
                    }
                }

                if (msg.Contains("*****"))
                {
                    signupData.parentAgreement = "Y";

                    string[] _msg = msg.Split(new string[] { "::" }, StringSplitOptions.None);
                    PassResultData passData = JsonConvert.DeserializeObject<PassResultData>(_msg[1]);

                    signupData.parentBirthDay = DateTime.ParseExact(passData.birthDate, "yyyyMMdd", null).ToString("yyyy-MM-dd");
                    signupData.parentConnectingInformation = passData.connInfo;
                    signupData.parentDuplicationInformation = passData.dupInfo;
                    signupData.parentPhoneNo = passData.mobileNo;

                    DestroyImmediate(webviewobj.gameObject);
                    if (webviewobj != null) webviewobj = null;

                    object[] data = new object[] {
                        signupData,
                        passData
                    };

                    UIManager.s.OpenPopupWithData<UIPopup_SignUp_Under14>(true, data);
                    Close();
                    agreement.isOn = false;
                    UIManager.s.Dim = false;
                }
                Debug.Log(string.Format("CallFromJS[{0}]", msg));
            });
            webviewobj.LoadURL(strurl);
            webviewobj.SetVisibility(true);
            webviewobj.SetMargins(0, 0, 0, 0);
        }
    }
}

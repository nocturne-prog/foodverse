using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using DF = Marvrus.Util.Define;
using OB = Marvrus.Util.OpenBrowser;
using PPM = Marvrus.Util.PlayerPrefsManager;
using Newtonsoft.Json;

namespace Marvrus.UI
{
    public class UIPopup_BottomSheet_SelectLogin : UIPopup
    {
        [Header("for Editor")]
        public GameObject editor_parent;
        public Button marvrusLogin_Btn;
        public Button naverLogin_Btn;
        public Button kakaoLogin_Btn;
        public Button facebookLogin_Btn;

        [Header("for Application")]
        public GameObject app_parent;
        public Button app_login_Btn;
        public Button app_signup_Btn;

        public override void Awake()
        {
            base.Awake();

            app_parent.SetActive(DF.platform != DF.Platform.UnityEditor);
            editor_parent.SetActive(DF.platform == DF.Platform.UnityEditor);

            if (DF.platform == DF.Platform.UnityEditor)
            {
                marvrusLogin_Btn.onClick.AddListener(OnClickMarvrusLogin);
                naverLogin_Btn.onClick.AddListener(() => OnClickSNSLogin(0));
                kakaoLogin_Btn.onClick.AddListener(() => OnClickSNSLogin(1));
                facebookLogin_Btn.onClick.AddListener(() => OnClickSNSLogin(2));
            }
            else
            {
                app_login_Btn.onClick.AddListener(OnClickAppLogin);
                app_signup_Btn.onClick.AddListener(OnClickAppSignUp);
            }
        }

        public override void Close()
        {
            base.Close();
            UIManager.s.Dim = false;
        }

        public override void UpdateData(params object[] args)
        {
            UIManager.s.Dim = true;
            base.UpdateData(args);
        }

        void OnClickMarvrusLogin()
        {
            Close();
            UIManager.s.OpenPopupWithData<UIPopup_Login>(true);
            UIManager.s.Dim = false;
        }

        void OnClickSNSLogin(int _index)
        {
            Close();
            UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_SNSAuth>(true, _index);
        }

        void OnClickAppLogin()
        {
            OB.SignIn((result) =>
            {
                Login(result);
            });
        }

        void OnClickAppSignUp()
        {
            OB.SignUp((result) =>
            {
                Login(result);
            });
        }

        private void Login(string _result)
        {
            string[] parameters = _result.Split("&");
            string userId = parameters[0].Split("=")[1];
            string refreshToken = parameters[1].Split("=")[1];

            userId = WebUtility.UrlDecode(userId);
            refreshToken = WebUtility.UrlDecode(refreshToken);

            PPM.ID = userId;
            PPM.RefreshToken = refreshToken;

            Protocol.Request.AutoLogin data = new()
            {
                id = PPM.ID,
                refreshToken = PPM.RefreshToken,
                clientId = PPM.ClientID
            };

            NetworkManager.s.AutoLogin(data, (result) =>
            {
                PPM.GuestLogin = false;
                UIManager.s.Dim = false;

                if (string.IsNullOrEmpty(result.data.MarvrusUser.provider) is true)
                    return;

                bool isMarvrusUser = result.data.MarvrusUser.provider.Equals(Const.PROVIDER_MARVRUS_USER);
                PPM.SNS_Login = !isMarvrusUser;

                Close();
                UIManager.s.OpenFeed();
            }, (error) =>
            {
                Debug.LogError(error);
                PPM.AutoLogin = false;
                UIManager.s.Dim = false;
            });
        }
    }
}

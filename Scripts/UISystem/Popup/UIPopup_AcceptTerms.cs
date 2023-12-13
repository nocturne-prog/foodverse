using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PPM = Marvrus.Util.PlayerPrefsManager;

namespace Marvrus.UI
{
    public class UIPopup_AcceptTerms : UIPopup
    {
        public enum SIGNUP_TYPE
        {
            GUEST,
            SNS,
            MARVRUS_OVER,
            MARVRUS_UNDER
        }

        [Header("Toggle")]
        public Toggle totalServiceAgreement;
        public Toggle over14_Toggle;
        public Toggle servicePolicyAgreement;  // 통합회원 이용약관 동의
        public Toggle privacyInfoUseAgreement; // 개인정보처리방침 동의
        public Toggle marketingReceiveAgreement;   // 마케팅 정보 수신 동의
        public Toggle nightReceiveAgreement;   // 야간 정보 수신 동의

        [Header("Text")]
        public TextMeshProUGUI over14_ToggleText;
        public TextMeshProUGUI policyAgreementText;
        public TextMeshProUGUI privacyUseAgreementText;

        [Header("Button")]
        public Button serviceButton;
        public Button privacyButton;
        public Button marketingButton;
        public Button nextButton;

        private SIGNUP_TYPE type;
        private Protocol.Request.SocialLogin socialLoginData;
        private string socialUserEmail;

        public override void Awake()
        {
            base.Awake();

            totalServiceAgreement.enabled = false;
            Button totalServiceBtn = totalServiceAgreement.transform.parent.GetComponent<Button>();
            totalServiceBtn.onClick.AddListener(OnClickTotalServiceBtn);
            servicePolicyAgreement.onValueChanged.AddListener(delegate { CheckAgreementToggle(); });
            privacyInfoUseAgreement.onValueChanged.AddListener(delegate { CheckAgreementToggle(); });
            marketingReceiveAgreement.onValueChanged.AddListener(delegate { CheckAgreementToggle(); });
            nightReceiveAgreement.onValueChanged.AddListener(delegate { CheckAgreementToggle(); });
            button_Close.onClick.AddListener(InitTotalToggle);

            serviceButton.onClick.AddListener(() => UIManager.s.OpenPopupWithData<UIPopup_TermsofService>(true));
            privacyButton.onClick.AddListener(() => UIManager.s.OpenPopupWithData<UIPopup_PrivacyPolicy>(true));
            marketingButton.onClick.AddListener(() => UIManager.s.OpenPopupWithData<UIPopup_Marketing>(true));

            nextButton.onClick.AddListener(OnClickNextButton);
        }

        public override void UpdateData(params object[] args)
        {
            type = (SIGNUP_TYPE)args[0];
            switch(type)
            {
                case SIGNUP_TYPE.GUEST:
                    {
                        over14_Toggle.gameObject.SetActive(true);
                        over14_ToggleText.text += "(필수)";
                        break;
                    }
                case SIGNUP_TYPE.SNS:
                    {
                        over14_Toggle.gameObject.SetActive(true);
                        socialLoginData = (Protocol.Request.SocialLogin)args[1];
                        socialUserEmail = (string)args[2];
                        break;
                    }
                case SIGNUP_TYPE.MARVRUS_OVER:
                    {
                        over14_Toggle.gameObject.SetActive(true);
                        break;
                    }
                case SIGNUP_TYPE.MARVRUS_UNDER:
                    {
                        over14_Toggle.gameObject.SetActive(false);
                        break;
                    }
            }

            if(over14_Toggle.gameObject)
                over14_Toggle.onValueChanged.AddListener(delegate { CheckAgreementToggle(); });

            // 초기화
            InitTotalToggle();
        }

        private void InitTotalToggle()
        {
            totalServiceAgreement.isOn = true;
            OnClickTotalServiceBtn();
        }

        // 전체 동의 버튼 클릭 / 해제
        private void OnClickTotalServiceBtn()
        {
            if (!totalServiceAgreement.isOn)
            {
                if (over14_Toggle.gameObject)
                    over14_Toggle.isOn = true;
                servicePolicyAgreement.isOn = true;
                privacyInfoUseAgreement.isOn = true;
                marketingReceiveAgreement.isOn = true;
                nightReceiveAgreement.isOn = true;

                totalServiceAgreement.isOn = true;
            }
            else
            {
                if (over14_Toggle.gameObject)
                    over14_Toggle.isOn = false;
                servicePolicyAgreement.isOn = false;
                privacyInfoUseAgreement.isOn = false;
                marketingReceiveAgreement.isOn = false;
                nightReceiveAgreement.isOn = false;

                totalServiceAgreement.isOn = false;
            }
        }

        // toggle 체크에 따른 전체 동의, 다음 버튼 활성화
        private void CheckAgreementToggle()
        {
            // 다 선택되면 전체 동의 check
            if (servicePolicyAgreement.isOn && privacyInfoUseAgreement.isOn && marketingReceiveAgreement.isOn && nightReceiveAgreement.isOn)
            {
                // 14세 이상 항목까지 check 된 경우
                if (over14_Toggle.gameObject)
                {
                    if (over14_Toggle.isOn)
                    {
                        totalServiceAgreement.isOn = true;
                        nextButton.SetActive(true);
                    }
                    else
                    {
                        totalServiceAgreement.isOn = false;
                        if (type == SIGNUP_TYPE.GUEST) nextButton.SetActive(false);
                    }
                }
                else
                {
                    totalServiceAgreement.isOn = true;
                    nextButton.SetActive(true);
                }
            }
            else
            {
                totalServiceAgreement.isOn = false;

                if (type == SIGNUP_TYPE.GUEST)
                {
                    if (over14_Toggle.isOn && servicePolicyAgreement.isOn && privacyInfoUseAgreement.isOn)
                    {
                        nextButton.SetActive(true);
                    }
                    else
                    {
                        nextButton.SetActive(false);
                    }
                }
                else
                {
                    if (servicePolicyAgreement.isOn && privacyInfoUseAgreement.isOn)
                    {
                        nextButton.SetActive(true);
                    }
                    else
                    {
                        nextButton.SetActive(false);
                    }
                }
            }
        }

        private void OnClickNextButton()
        {
            if (type.Equals(SIGNUP_TYPE.GUEST))
            {
                UIManager.s.Loading = true;

                Protocol.Request.GuestLogin guestData = new Protocol.Request.GuestLogin(
                    PPM.ClientID,
                    _marketing: marketingReceiveAgreement.isOn ? "Y" : "N",
                    _nightMarketing: nightReceiveAgreement.isOn ? "Y" : "N",
                    _privacy: privacyInfoUseAgreement.isOn ? "Y" : "N",
                    _servicePolicy: servicePolicyAgreement.isOn ? "Y" : "N"
                );

                NetworkManager.s.GuestLogin(guestData, (result) =>
                {
                    // PlayerPrefs 저장.
                    PPM.GuestLogin = true;
                    PPM.ID = result.data.id;

                    UIManager.s.OpenFeed();
                }, (error) =>
                {
                    UIManager.s.Loading = false;
                    Debug.LogError(error);
                });
            }
            else if (type.Equals(SIGNUP_TYPE.SNS))
            {
                Protocol.Request.SocialSignUp socialSignUpData = new Protocol.Request.SocialSignUp(
                    socialLoginData,
                    _marketing: marketingReceiveAgreement.isOn ? "Y" : "N",
                    _nightMarketing: nightReceiveAgreement.isOn ? "Y" : "N",
                    _privacy: privacyInfoUseAgreement.isOn ? "Y" : "N",
                    _servicePolicy: servicePolicyAgreement.isOn ? "Y" : "N"
                );

                NetworkManager.s.SocialSignUp(socialSignUpData, (result) =>
                {
                    PPM.AutoLogin = true;
                    PPM.ID = result.data.id;

                    object[] data = new object[] { socialUserEmail };

                    UIManager.s.OpenPopupWithData<UIPopup_SignUp_SNS>(true, data);
                }, (error) =>
                {
                    Debug.LogError(error);
                });
            }
            else
            {
                Protocol.Request.SignUp signupData = new Protocol.Request.SignUp();
                signupData.marketingReceiveAgreement = marketingReceiveAgreement.isOn ? "Y" : "N";
                signupData.nightMarketingReceiveAgreement = nightReceiveAgreement.isOn ? "Y" : "N";
                signupData.privacyInfoUseAgreement = privacyInfoUseAgreement.isOn ? "Y" : "N";
                signupData.servicePolicyAgreement = servicePolicyAgreement.isOn ? "Y" : "N";

                object[] data = new object[] { signupData };

                switch (type)
                {
                    case SIGNUP_TYPE.MARVRUS_OVER:
                        {
                            UIManager.s.OpenPopupWithData<UIPopup_SignUp_Over14>(true, data);
                            break;
                        }
                    case SIGNUP_TYPE.MARVRUS_UNDER:
                        {
                            UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_PhoneAuth_Protector>(true, data);
                            break;
                        }
                }
            }
        }
    }
}

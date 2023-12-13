using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PPM = Marvrus.Util.PlayerPrefsManager;

namespace Marvrus.UI
{
    public class UIPopup_Login : UIPopup
    {
        [Header("User InputField")]
        public TMP_InputField id_InputField;
        public TMP_InputField pwd_InputField;

        [Header("Show Password")]
        public PasswordOption showPwd_Toggle;

        [Header("Auto Login")]
        public Toggle autoLogin_Toggle;
        [Header("Find ID")]
        public Button findId_Button;
        [Header("Find Password")]
        public Button findPwd_Button;

        [Header("Login")]
        public Button login_Button;

        [Header("Sign Up")]
        public Button signUp_Button;

        [Header("Login fail")]
        public GameObject loginFail;

        private string id, pwd;
        private bool checkAutoLogin = false;


        public override void Awake()
        {
            base.Awake();

            InitInputFieldSetting(_input: id_InputField, _onEndEdit: ()=> SetId(id_InputField.text));
            InitInputFieldSetting(_input: pwd_InputField, _onEndEdit: () => SetPwd(pwd_InputField.text));
            login_Button.onClick.AddListener(OnClickLogin);
            showPwd_Toggle.SetPasswordInputField(pwd_InputField);
            autoLogin_Toggle.onValueChanged.AddListener(SetAutoLogin);
            findId_Button.onClick.AddListener(OnClickFindId);
            findPwd_Button.onClick.AddListener(OnClickFindPwd);
            signUp_Button.onClick.AddListener(OnClickSignUp);
        }

        public override void UpdateData(params object[] args)
        {
            autoLogin_Toggle.SetIsOnWithoutNotify(false);

            id_InputField.text = null;
            pwd_InputField.text = null;
        }

        private void InitInputFieldSetting(TMP_InputField _input, Action _onSelect = null, Action _onDeselect = null, 
            Action _onValueChanged = null, Action _onEndEdit = null)
        {
            _input.onSelect.AddListener(delegate
            {
                if (_onSelect != null) _onSelect();
            });
            _input.onDeselect.AddListener(delegate
            {
                if (string.IsNullOrEmpty(_input.text))
                    _input.GetComponent<Animator>().Play("ValueUnchanged");
                else
                    _input.GetComponent<Animator>().Play("ValueChanged");

                if (_onDeselect != null) _onDeselect();
            });
            _input.onValueChanged.AddListener(delegate
            {
                if (_onValueChanged != null) _onValueChanged();
            });
            _input.onEndEdit.AddListener(delegate
            {
                if (_onEndEdit != null) _onEndEdit();
            });
        }

        private void OnClickFindId()
        {
            // 아이디 찾기
        }

        private void OnClickFindPwd()
        {
            // 비밃번호 찾기
        }

        private void OnClickSignUp()
        {
            UIManager.s.OpenPopupWithData<UIPopup_BottomSheet_SignUp>(true);
        }

        private void SetAutoLogin(bool isOn)
        {
            checkAutoLogin = isOn;
        }

        private void OnClickLogin()
        {
            if (checkAutoLogin is true)
            {
                SavePlayerPrefs();
            }

            PPM.GuestLogin = false;

            NetworkManager.s.Login(id, pwd, PPM.ClientID, (result) =>
            {
                Close();
                UIManager.s.OpenFeed();
            }, (error) =>
            {
                loginFail.SetActive(true);
            });
        }

        private void SavePlayerPrefs()
        {
            PPM.AutoLogin = checkAutoLogin;
            PPM.ID = id;
        }

        private void SetId(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            id = text;

            if (CheckAbleLogin())
                login_Button.SetActive(true);
        }

        private void SetPwd(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            pwd = text;

            if (CheckAbleLogin())
                login_Button.SetActive(true);
        }

        private bool CheckAbleLogin()
        {
            if (string.IsNullOrEmpty(id))
                return false;

            if (string.IsNullOrEmpty(pwd))
                return false;

            return true;
        }
    }
}
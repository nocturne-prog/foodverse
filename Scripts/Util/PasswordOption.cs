using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus
{
    public class PasswordOption : MonoBehaviour
    {
        TMP_InputField pwd_InputField;
        public Button DeActive;
        public Button Active;

        void Start()
        {
            DeActive.onClick.AddListener(
                () =>
                {
                    DeActive.gameObject.SetActive(false);
                    Active.gameObject.SetActive(true);
                    ShowPasswordInputField();
                });
            Active.onClick.AddListener(
                () =>
                {
                    DeActive.gameObject.SetActive(true);
                    Active.gameObject.SetActive(false);
                    ShowPasswordInputField();
                });
        }

        public void SetPasswordInputField(TMP_InputField _input)
        {
            pwd_InputField = _input;
        }

        // 비밀번호 보이기 / 숨기기
        private void ShowPasswordInputField()
        {
            if (pwd_InputField != null)
            {
                if (Active.gameObject.activeSelf)
                {
                    pwd_InputField.contentType = TMP_InputField.ContentType.Standard;
                }
                else
                {
                    pwd_InputField.contentType = TMP_InputField.ContentType.Password;
                }
                pwd_InputField.ForceLabelUpdate();
            }
        }
    }
}

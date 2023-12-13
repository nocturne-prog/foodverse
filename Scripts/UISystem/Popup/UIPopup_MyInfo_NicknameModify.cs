using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.UI
{
    public class UIPopup_MyInfo_NicknameModify : UIPopup
    {
        public TMP_InputField nickknameInputField;
        public Image errorMsg1Icon;
        public TextMeshProUGUI errorMsg1;
        public Image errorMsg2Icon;
        public TextMeshProUGUI errorMsg2;
        public GameObject duplicateError;
        public Button saveBtn;
        public GameObject block;

        private string newNickname = string.Empty;

        public override void Awake()
        {
            base.Awake();
            saveBtn.onClick.AddListener(OnClickSave);
            nickknameInputField.onEndEdit.AddListener(OnEndEditNickname);
        }

        public override void UpdateData(params object[] args)
        {
            ChangeColor(0, Color.gray);
            ChangeColor(1, Color.gray);
            block.SetActive(true);
            duplicateError.SetActive(false);

            nickknameInputField.text = DC.data_login.data.MarvrusUser.nickname;
            newNickname = string.Empty;
        }

        public void OnClickSave()
        {
            if (string.IsNullOrEmpty(newNickname) is true)
            {
                Debug.LogError($"New Nickname is empty...");
                return;
            }

            string id = DC.data_login.data.id;
            NetworkManager.s.EditNickName(id, newNickname, () =>
            {
                DC.data_login.data.MarvrusUser.nickname = newNickname;
                UIManager.s.OpenToast(Const.TOAST_EDIT_NICKNAME_COMPLETE);
                Hide();
            }, (error) =>
            {
                Debug.LogError(error);
            });
        }

        public void OnEndEditNickname(string text)
        {
            if (string.IsNullOrEmpty(text) is true)
            {
                ChangeColor(0, Color.red);
                ChangeColor(1, Color.red);
                duplicateError.SetActive(false);
                return;
            }

            int length = text.Length;
            bool[] check = new bool[2];

            check[0] = length >= 2 && length <= 8;
            ChangeColor(0, check[0] ? Color.blue : Color.red);

            string temp = Regex.Replace(text, @"[0-9a-zA-Z가-힣]", "");
            check[1] = string.IsNullOrEmpty(temp);
            ChangeColor(1, check[1] ? Color.blue : Color.red);

            if (check[0] is true && check[1] is true)
            {
                NetworkManager.s.CheckOverlapNickname(text, (result) =>
                {
                    if (result is true)
                    {
                        // 중복 됨
                        block.SetActive(true);
                        duplicateError.SetActive(true);
                    }
                    else
                    {
                        newNickname = text;
                        block.SetActive(false);
                        duplicateError.SetActive(false);
                    }
                }, (error) =>
                {
                    /// TODO :: Error Popup
                });
            }
            else
            {
                block.SetActive(true);
            }
        }

        public void ChangeColor(int _index, Color _color)
        {
            if (_index == 0)
            {
                errorMsg1.color = _color;
                errorMsg1Icon.color = _color;
            }
            else
            {
                errorMsg2.color = _color;
                errorMsg2Icon.color = _color;
            }
        }
    }
}
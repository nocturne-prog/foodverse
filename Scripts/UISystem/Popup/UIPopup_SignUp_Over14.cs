using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;
using PPM = Marvrus.Util.PlayerPrefsManager;

namespace Marvrus.UI
{
    public class UIPopup_SignUp_Over14 : UIPopup
    {
        [Header("InputField")]
        public TMP_InputField email;
        public TMP_InputField id;
        public TMP_InputField pw;
        public TMP_InputField pwCheck;

        [Header("Supporting")]
        public GameObject emailSupporting;
        public TextMeshProUGUI emailSupportingText;

        public GameObject idSupporting;
        public GameObject[] idSupportingObjs;

        public GameObject pwSupporting;
        public GameObject[] pwSupportingObjs;

        public GameObject pwCheckSupporting;

        [Header("Toggle")]
        public PasswordOption pwShowToggle;
        public PasswordOption pwCheckShowToggle;

        [Header("Button")]
        public Button completeButton;

        private Color redColor;
        private Color grayColor;
        private Color greenColor;

        private bool idShowGuide = false;
        private bool pwShowGuide = false;

        private Protocol.Request.SignUp signupData = new Protocol.Request.SignUp();

        public override void Awake()
        {
            base.Awake();

            ColorUtility.TryParseHtmlString("#F83535", out redColor);
            ColorUtility.TryParseHtmlString("#999999", out grayColor);
            ColorUtility.TryParseHtmlString("#07B018", out greenColor);

            InitInputFieldSetting(_input: email, _onValueChanged: CheckEmailInputField);

            InitInputFieldSetting(_input: id,
                _onSelect: () => { idSupporting.SetActive(true); },
                _onDeselect: () => { idSupporting.SetActive(idShowGuide); },
                _onValueChanged: CheckIDInputField);

            InitInputFieldSetting(_input: pw,
                _onSelect: () => { pwSupporting.SetActive(true); },
                _onDeselect: () => { pwSupporting.SetActive(pwShowGuide); },
                _onValueChanged: () => { CheckPassWordInputField(false); });

            InitInputFieldSetting(_input: pwCheck, _onValueChanged: () => { CheckPassWordInputField(true); });

            pwShowToggle.SetPasswordInputField(pw);
            pwCheckShowToggle.SetPasswordInputField(pwCheck);

            completeButton.onClick.AddListener(OnClickCompleteButton);
        }

        public override void UpdateData(params object[] args)
        {
            InitInputField();

            signupData = (Protocol.Request.SignUp)args[0];
            signupData.parentAgreement = "N";

            // 처음 InputField 활성화상태로 시작.
            email.Select();
        }

        private void InitInputField()
        {
            email.text = null;
            id.text = null;
            pw.text = null;
            pwCheck.text = null;
        }

        private void InitInputFieldSetting(TMP_InputField _input, Action _onSelect = null, Action _onDeselect = null, Action _onValueChanged = null)
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
        }

        #region InputField Check_Email
        private void CheckEmailInputField()
        {
            // 한글자라도 입력했을 경우에만 비교
            if (!string.IsNullOrEmpty(email.text))
            {
                if (email.text.Contains(" "))
                {
                    emailSupportingText.text = Const.SIGNUP_EMAIL_CHECK;
                    emailSupporting.SetActive(true);
                }
                else
                {
                    // @가 포함되어 있는가?
                    if (email.text.Contains("@"))
                    {
                        string[] _email = email.text.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                        // @앞에 문자가 있는가?
                        if (_email[0].Length > 0)
                        {
                            bool isEngMatch = Regex.IsMatch(_email[0], @"[a-zA-Z]");
                            if (isEngMatch)
                            {
                                // @뒤가 .이 포함되어 있는가?
                                if (_email.Length > 1 && _email[1].Length > 0 && _email[1].Contains("."))
                                {
                                    string[] _domain = _email[1].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                    // . 뒤에 문자가 포함되어 있는가? (. 이후에 두글자 이상이어야함)
                                    if (_domain.Length > 1 && _domain[1].Length > 1)
                                    {
                                        isEngMatch = Regex.IsMatch(_domain[1], @"[a-zA-Z]");
                                        if (isEngMatch)
                                        {
                                            // 이메일 형식이 맞은 경우, 이메일 중복 확인
                                            CheckDuplicateEmail();
                                        }
                                        else
                                        {
                                            emailSupportingText.text = Const.SIGNUP_EMAIL_CHECK;
                                            emailSupporting.SetActive(true);
                                        }
                                    }
                                    else
                                    {
                                        emailSupportingText.text = Const.SIGNUP_EMAIL_CHECK;
                                        emailSupporting.SetActive(true);
                                    }
                                }
                                else
                                {
                                    emailSupportingText.text = Const.SIGNUP_EMAIL_CHECK;
                                    emailSupporting.SetActive(true);
                                }
                            }
                            else
                            {
                                emailSupportingText.text = Const.SIGNUP_EMAIL_CHECK;
                                emailSupporting.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        emailSupportingText.text = Const.SIGNUP_EMAIL_CHECK;
                        emailSupporting.SetActive(true);
                    }
                }
            }
            else
            {
                emailSupporting.SetActive(false);
            }

            // InputField 작성 이후 Next 버튼 활성화 체크
            CheckUserInfoBtnInteractable();
        }

        private void CheckDuplicateEmail()
        {
            NetworkManager.s.CheckOverlapEmail(email.text,
                (_duplicate) =>
                {
                    if (_duplicate)
                    {
                        emailSupportingText.text = Const.SIGNUP_EMAIL_DUPLICATE;
                        emailSupporting.SetActive(true);
                    }
                    else
                    {
                        emailSupportingText.text = "";
                        emailSupporting.SetActive(false);
                    }

                    // InputField 작성 이후 Next 버튼 활성화 체크
                    CheckUserInfoBtnInteractable();
                },
                (error) =>
                {
                    Debug.LogError(error);
                });
        }
        #endregion

        #region InpuField Check_ID
        // 아이디 : 영문 / 숫자 6~12자
        private void CheckIDInputField()
        {
            // 한글자라도 입력했을 경우에만 비교
            if (!string.IsNullOrEmpty(id.text))
            {
                idShowGuide = true;

                // 특수문자 포함 여부
                string specialChar = id.text;
                specialChar = Regex.Replace(specialChar, @"[a-zA-Z0-9ㄱ-ㅎ가-힣]", "");
                bool isCharMatch = specialChar.Length > 0 ? true : false;
                if (isCharMatch)
                {
                    Debug.Log(specialChar);
                    if (!specialChar.Contains(".") && !specialChar.Contains("_"))
                        isCharMatch = true;
                    else
                        isCharMatch = false;
                }

                // 한글 포함 여부
                bool isKorMatch = Regex.IsMatch(id.text, @"[ㄱ-ㅎ가-힣]");

                // 영어 포함 여부
                bool isEngMatch = Regex.IsMatch(id.text, @"[a-zA-Z]");

                // 숫자 포함 여부
                bool isNumMatch = Regex.IsMatch(id.text, @"[0-9]");

                if (!isCharMatch && !isKorMatch && isEngMatch)
                {
                    if (id.text.Length >= 5 && id.text.Length <= 20)
                    {
                        // 글자수 조건 충분, char 조건 충분
                        // 아이디 중복 체크
                        SetIDSupportingColor(_isComplete:true);
                        CheckDuplicateID();
                    }
                    else
                    {
                        // 글자수 조건 불충분, char 조건 충분
                        SetIDSupportingColor(true, 0);
                    }
                }
                else
                {
                    if (id.text.Length >= 5 && id.text.Length <= 20)
                    {
                        // 글자수 조건 충분, char 조건 불충분
                        SetIDSupportingColor(true, 1);
                    }
                    else
                    {
                        // 글자수 조건 불충분, char 조건 불충분
                        SetIDSupportingColor(_errorIndex:2);
                    }
                }
            }
            else
            {
                idShowGuide = false;
                SetIDSupportingColor();
            }

            // InputField 작성 이후 Next 버튼 활성화 체크
            CheckUserInfoBtnInteractable();
        }

        private void SetIDSupportingColor(bool _isComplete = false, int _errorIndex = -1)
        {
            Color applyColor = _isComplete ? greenColor : grayColor;

            // index -1 => 초기화
            // index 0 => ID 길이 오류
            // index 1 => ID 형식 오류
            // index 2 => ID 길이, 형식 오류
            // index 3 => ID 중복

            for (int i = 0; i < 2; i++)
            {
                idSupportingObjs[i].SetActive(true);

                idSupportingObjs[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = applyColor;
                idSupportingObjs[i].transform.GetChild(0).GetComponent<Image>().color = applyColor;
            }
            idSupportingObjs[2].SetActive(false);

            if (_errorIndex >= 0)
            {
                if (_errorIndex < 2)
                {
                    idSupportingObjs[_errorIndex].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = redColor;
                    idSupportingObjs[_errorIndex].transform.GetChild(0).GetComponent<Image>().color = redColor;
                }
                else if (_errorIndex == 2)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        idSupportingObjs[i].SetActive(true);

                        idSupportingObjs[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = redColor;
                        idSupportingObjs[i].transform.GetChild(0).GetComponent<Image>().color = redColor;
                    }
                }
                else
                {
                    idSupportingObjs[0].SetActive(false);
                    idSupportingObjs[1].SetActive(false);
                    idSupportingObjs[2].SetActive(true);
                }
            }
        }

        private void CheckDuplicateID()
        {
            NetworkManager.s.CheckOverlapID(id.text,
                (_duplicate) =>
                {
                    if (_duplicate)
                    {
                        SetIDSupportingColor(true, 3);
                    }
                    else
                    {
                        SetIDSupportingColor(_isComplete:true);
                        idShowGuide = false;
                    }

                    // InputField 작성 이후 Next 버튼 활성화 체크
                    CheckUserInfoBtnInteractable();
                },
                (error) =>
                {
                    Debug.LogError(error);
                });
        }
        #endregion

        #region InputField Check_Password
        // 비밀번호 : 문자/숫자/기호 8자 이상
        private void CheckPassWordInputField(bool _reCheck)
        {
            // 비밀번호 입력 사항 체크
            if (!_reCheck)
            {
                // 한글자라도 입력했을 경우에만 비교
                if (!string.IsNullOrEmpty(pw.text))
                {
                    pwShowGuide = true;

                    // 특수문자 포함 여부
                    string specialChar = pw.text;
                    specialChar = Regex.Replace(specialChar, @"[a-zA-Z0-9ㄱ-ㅎ가-힣]", "");
                    bool isCharMatch = specialChar.Length > 0 ? true : false;

                    // 한글 포함 여부
                    bool isKorMatch = Regex.IsMatch(pw.text, @"[ㄱ-ㅎ가-힣]");

                    // 영어 포함 여부
                    bool isEngMatch = Regex.IsMatch(pw.text, @"[a-zA-Z]");

                    // 숫자 포함 여부
                    bool isNumMatch = Regex.IsMatch(pw.text, @"[0-9]");

                    if (isCharMatch && !isKorMatch && isEngMatch && isNumMatch)
                    {
                        // 글자수 조건 충분, char 조건 충분
                        if (pw.text.Length >= 8 && pw.text.Length <= 20)
                        {
                            SetPWSupportingColor(_isComplete: true);
                            pwShowGuide = false;

                            if (!string.IsNullOrEmpty(pwCheck.text))
                            {
                                CheckPassWordInputField(true);
                            }
                        }
                        // 글자수 조건 불충분, char 조건 충분
                        else
                        {
                            SetPWSupportingColor(true, 0);
                        }
                    }
                    else
                    {
                        // 글자수 조건 충분, char 조건 불충분
                        if (pw.text.Length >= 8 && pw.text.Length <= 20)
                        {
                            SetPWSupportingColor(true, 1);
                        }
                        // 글자수 조건 불충분, char 조건 불충분
                        else
                        {
                            SetPWSupportingColor(_errorIndex: 2);
                        }
                    }
                }
                else
                {
                    pwShowGuide = false;
                    SetPWSupportingColor();
                }
            }
            else
            {
                // 한글자라도 입력했을 경우에만 비교
                if (!string.IsNullOrEmpty(pwCheck.text))
                {
                    if (!pw.text.Equals(pwCheck.text))
                    {
                        pwCheckSupporting.SetActive(true);
                    }
                    else
                    {
                        pwCheckSupporting.SetActive(false);
                    }
                }
                else
                {
                    pwCheckSupporting.SetActive(false);
                }
            }

            // InputField 작성 이후 Next 버튼 활성화 체크
            CheckUserInfoBtnInteractable();
        }

        private void SetPWSupportingColor(bool _isComplete = false, int _errorIndex = -1)
        {
            Color applyColor = _isComplete ? greenColor : grayColor;

            // index -1 => 초기화
            // index 0 => ID 길이 오류
            // index 1 => ID 형식 오류
            // index 2 => ID 길이, 형식 오류

            for (int i = 0; i < 2; i++)
            {
                pwSupportingObjs[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = applyColor;
                pwSupportingObjs[i].transform.GetChild(0).GetComponent<Image>().color = applyColor;
            }

            if (_errorIndex >= 0)
            {
                if (_errorIndex < 2)
                {
                    pwSupportingObjs[_errorIndex].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = redColor;
                    pwSupportingObjs[_errorIndex].transform.GetChild(0).GetComponent<Image>().color = redColor;
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        pwSupportingObjs[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = redColor;
                        pwSupportingObjs[i].transform.GetChild(0).GetComponent<Image>().color = redColor;
                    }
                }
            }
        }
        #endregion

        private void CheckUserInfoBtnInteractable()
        {
            if (!string.IsNullOrEmpty(id.text) && !idSupporting.activeSelf
                && !string.IsNullOrEmpty(pw.text) && !pwSupporting.activeSelf
                && !string.IsNullOrEmpty(pwCheck.text) && !pwCheckSupporting.activeSelf
                && !string.IsNullOrEmpty(email.text) && !emailSupporting.activeSelf)
            {
                completeButton.SetActive(true);
            }
            else
            {
                completeButton.SetActive(false);
            }
        }

        // 회원 정보 입력 "다음" 버튼 클릭
        public void OnClickCompleteButton()
        {
            // 회원가입 API 하는 동안 중복으로 눌리지 않기위해 false 처리
            completeButton.enabled = false;

            signupData.emailAddress = email.text;
            signupData.id = id.text;
            signupData.password = RSAModule.RSAEncrypt(pw.text);
            signupData.passwordConfirm = RSAModule.RSAEncrypt(pwCheck.text);

            NetworkManager.s.SignUp(true, signupData, (result) =>
            {
                NetworkManager.s.Login(id.text, pw.text, PPM.ClientID, (result) =>
                {
                    UIManager.s.OpenPopupWithData<UIPopup_SignUp_Over14Completed>();
                }, (error) =>
                {
                    Debug.LogError(error);
                });
            }, (error) =>
            {
                Debug.LogError(error);
            });
        }
    }
}

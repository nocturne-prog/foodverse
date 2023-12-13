using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Marvrus.UI;
using DF = Marvrus.Util.Define;
using Marvrus.Util;

namespace Marvrus
{
    public enum eMenuType
    {
        NONE,
        Top_Only,
        Bottom_Only,
        BOTH,
        COUNT
    }

    public class UIManager : Singleton<UIManager>
    {
        [SerializeField, Header("Top")] UITopBar topMenu;
        [SerializeField, Header("Bottom")] UIBottomBar bottomMenu;
        [SerializeField, Header("Main Canvas")] Transform mainCanvasPopupParent;
        [SerializeField, Header("Front Canvas")] Transform frontCanvasPopupParent;
        [SerializeField, Header("Modal Canvas")] Transform modalCanvasPopupParent;
        [SerializeField, Header("Snack Bar")] SnackBar snackBar;
        [SerializeField, Header("Toast")] Toast toast;
        [SerializeField, Header("Dim")] GameObject dim;
        [SerializeField, Header("Loading")] GameObject loading;

        Dictionary<string, PopupStatus> popupDic = new();

        public class PopupStatus
        {
            public UIPopup popup;
            public bool isShow;
            public int layer;
            public Transform parents;
            public bool useDim;

#if UNITY_EDITOR
            public override string ToString()
            {
                return $"{popup.gameObject.name}, layer: {layer}, isShow: {isShow}, useDim: {useDim}";
            }
#endif
        }

        public bool TopMenu
        {
            set { topMenu.SetActive(value); }
        }

        public bool BottomMenu
        {
            set { bottomMenu.SetActive(value); }
        }

        public UITopBar GetTopMenu
        {
            get { return topMenu; }
        }

        public UIBottomBar GetBottomMenu
        {
            get { return bottomMenu; }
        }

        public bool BottomMenu_Upload
        {
            set { bottomMenu.ShowUploadBtn = value; }
        }

        public bool Dim
        {
            set { dim.SetActive(value); }
        }

        public bool Loading
        {
            set
            {
                Dim = value;
                loading.SetActive(value);
            }
        }

        #region UIPopup.

        protected override void Awake()
        {
            base.Awake();

            Application.targetFrameRate = 60;

            if (DF.platform == DF.Platform.Android)
            {
                ApplicationChrome.statusBarState = ApplicationChrome.States.Visible;
                ApplicationChrome.navigationBarState = ApplicationChrome.States.Visible;
            }

            Transform[] canvas = new Transform[] {
                mainCanvasPopupParent,
                frontCanvasPopupParent,
                modalCanvasPopupParent
            };

            for (int i = 0; i < canvas.Length; i++)
            {
                for (int m = 0; m < canvas[i].childCount; m++)
                {
                    Transform trf = canvas[i].transform.GetChild(m);
                    var popup = trf.GetComponent<UIPopup>();

                    if (popup != null)
                    {
                        popupDic.Add(popup.GetType().ToString(), new PopupStatus()
                        {
                            popup = popup,
                            isShow = trf.IsActive(),
                            layer = 0,
                            parents = canvas[i],
                            useDim = canvas[i] == modalCanvasPopupParent
                        });
                    }
                }
            }
        }

        public T OpenPopup<T>(bool _addtive = false) where T : UIPopup
        {
            Debug.LogWarning($"UIManager :: OpenPopup {typeof(T)}");

            var value = FindValue(typeof(T).ToString());

            if (_addtive is false)
            {
                CloseAll(value);
            }

            if (value != null)
            {
                value.popup.MenuAction();
                value.popup.Show();
                value.isShow = true;
                value.layer = MaxLayerValue + 1;
                Dim = value.useDim;

                value.popup.transform.SetAsLastSibling();

                return (T)value.popup;
            }
            else
            {
                Debug.LogError($"UIManager :: OpenPopup.{typeof(T)}");
            }

            return null;
        }

        public T OpenPopupWithData<T>(bool _addtive = false, params object[] _args) where T : UIPopup
        {
            var popup = OpenPopup<T>(_addtive);
            popup.UpdateData(_args);

            return popup;
        }

        private PopupStatus FindValue(string _key)
        {
            var value = popupDic.FirstOrDefault(x => x.Key.Equals(_key)).Value;

            if (value is null)
            {
                Debug.LogError($"UIManager :: can't not find popup {_key}");
                return null;
            }
            else
            {
                return value;
            }
        }

        public PopupStatus FindValue<T>() where T : UIPopup
        {
            Type t = typeof(T);
            string key = t.ToString();

            return FindValue(key);
        }

        private PopupStatus FindValue(UIPopup _popup)
        {
            string key = _popup.GetType().ToString();
            return FindValue(key);
        }

        public T FindPopup<T>() where T : UIPopup
        {
            var value = FindValue<T>();
            return (T)value.popup;
        }

        public PopupStatus FindFirstPopup()
        {
            PopupStatus value = popupDic.OrderByDescending(x => x.Value.layer).FirstOrDefault().Value;
            return value;
        }

        public bool IsShow<T>() where T : UIPopup
        {
            return FindValue<T>().isShow;
        }

        public int MaxLayerValue => popupDic.Max(x => x.Value.layer);

        public void ClosePopup<T>() where T : UIPopup
        {
            var popup = FindPopup<T>();
            popup.Close();
        }

        public void ClosePopup(UIPopup popup)
        {
            var value = FindValue(popup);
            value.isShow = false;
            value.layer = 0;

            var last = popupDic.OrderByDescending(x => x.Value.layer).FirstOrDefault();
            last.Value.popup.MenuAction();

            var showPopup = popupDic.FirstOrDefault(x => x.Value.isShow).Value;

            if(showPopup is null)
            {
                Debug.LogError($"all close Popup !! :: {last.Key}");
                last.Value.popup.Show();
            }
        }

        public void CloseAll(PopupStatus _except = null)
        {
            foreach (var v in popupDic.Values)
            {
                if (_except != null)
                {
                    if (v == _except)
                        continue;
                }

                if (v.isShow is true)
                {
                    Debug.LogWarning($"UIManager.CloseAll :: {v.popup}");
                    v.popup.Hide();
                }
            }
        }
        #endregion

        #region Toast / SnackBar.
        public void OpenToast(string _text, float _duration = 2f)
        {
            toast.transform.SetAsLastSibling();
            toast.Open(_text, _duration);
        }

        public void OpenSnackBar(string _text, float _duration = 2f)
        {
            snackBar.transform.SetAsLastSibling();
            snackBar.Open(_text, _duration);
        }
        #endregion

        #region Common
        public void OpenOneButton(string _title, string _desc, Action _ok, string _okText = null)
        {
            object[] data = new object[] {
                UIPopup_Modal_Default.BUTTON_TYPE.ONE,
                _title,
                _desc,
                _ok,
                _okText
            };

            OpenPopupWithData<UIPopup_Modal_Default>(true, data);
        }

        public void OpenTwoButton(string _title, string _desc, Action _left = null, Action _right = null,
            string _leftText = null, string _rightText = null)
        {
            object[] data = new object[] {
                UIPopup_Modal_Default.BUTTON_TYPE.TWO,
                _title,
                _desc,
                _left,
                _right,
                _leftText,
                _rightText
            };

            OpenPopupWithData<UIPopup_Modal_Default>(true, data);
        }
        #endregion

        #region Util
        public void OpenFeed()
        {
            Debug.LogWarning("UIManager :: OpenFeed");

            topMenu.InitTopBar();
            bottomMenu.InitBottomMenu();
            OpenPopupWithData<UIPopup_Feed>();
        }

        public void RefreshFeed(Action _callback = null)
        {
            FindPopup<UIPopup_Feed>().Refresh(_callback);
            topMenu.UpdateNotiCount();
        }

        public void OpenSelectLogin()
        {
            OpenPopupWithData<UIPopup_BottomSheet_SelectLogin>(true);
        }
        #endregion

        //private string CheckDic()
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();

        //    foreach (var v in popupDic)
        //    {
        //        sb.AppendLine($"{v.Key} :: {v.Value}");
        //    }

        //    //return Newtonsoft.Json.JsonConvert.SerializeObject(
        //    //    popupDic,
        //    //    Newtonsoft.Json.Formatting.None,
        //    //    new Newtonsoft.Json.JsonSerializerSettings()
        //    //    {
        //    //        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
        //    //    });

        //    return sb.ToString();
        //}

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                var popupStatus = FindFirstPopup();

                if (popupStatus.parents != mainCanvasPopupParent)
                {
                    popupStatus.popup.Close();
                }

            }
        }
    }
}
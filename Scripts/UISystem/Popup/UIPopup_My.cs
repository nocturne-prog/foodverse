using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Marvrus.Scroll;
using DC = Marvrus.Data.DataContainer;
using PPM = Marvrus.Util.PlayerPrefsManager;

namespace Marvrus.UI
{
    public class UIPopup_My : UIPopup
    {
        [Header("Toggle Group")]
        public Toggle feed_toggle;
        public Toggle active_toggle;

        [Header("My Feed Scroll")]
        public GameObject feed_obj;
        public RecycleScroll_MyFeed myFeed_scroll;
        public GameObject feedNonText_obj;

        [Header("My Active Scroll")]
        public GameObject active_obj;
        public RecycleScroll_My_Active myActive_scroll;
        public GameObject activeNonText_obj;
        public TextMeshProUGUI availCoin_text;
        public TextMeshProUGUI expireCoin_text;

        [Header("User Thumbnail")]
        public Util.ProfileImageItem userThumbnail_Image;

        [Header("User Name")]
        public TextMeshProUGUI userName_TMP;

        [Header("Edit Profle")]
        public Button editProfile_Btn;

        [Header("Settings")]
        public Button settings_Btn;

        [Header("Coin Refresh")]
        public Button coinRefresh_Btn;

        public override void Awake()
        {
            base.Awake();
            editProfile_Btn.onClick.AddListener(OnClickEdit);
            settings_Btn.onClick.AddListener(OnClickSettings);
            coinRefresh_Btn.onClick.AddListener(OnClickCoinRefresh);

            feed_toggle.onValueChanged.AddListener(OnValueChangedFeed);
            active_toggle.onValueChanged.AddListener(OnValueChangedActive);

            feed_toggle.SetIsOnWithoutNotify(true);
            active_toggle.SetIsOnWithoutNotify(false);

            feed_obj.SetActive(true);
            active_obj.SetActive(false);
        }

        public override void UpdateData(params object[] args)
        {

            NetworkManager.s.GetMyProfile((result) =>
            {
                Refresh(result);
                GetMyFeedListRpeat();
            });

            OnClickCoinRefresh();
        }

        public void Refresh(UserProfile _data)
        {
            if (_data.picture_url is null)
            {
                userThumbnail_Image.texture = null;
            }
            else
            {
                Util.FileCache.s.GetImage(_data.picture_url, Const.IMAGE_SIZE_MY, (texture) =>
                {
                    userThumbnail_Image.texture = texture;
                });
            }

            userName_TMP.text = _data.userName;
        }

        public void OnClickEdit()
        {
            if (PPM.GuestLogin is true)
            {
                UIManager.s.OpenSelectLogin();
                return;
            }

            UIManager.s.OpenPopupWithData<UIPopup_MyInfo_ProfileModify>(true);
        }

        public void OnClickSettings()
        {

            UIManager.s.OpenPopupWithData<UIPopup_Settings>(true);
        }

        public void GetMyFeedListRpeat()
        {
            DC.data_totalMyFeed = new List<Feed>();

            NetworkManager.s.GetMyFeedList(0, (feed_result) =>
            {
                feedNonText_obj.SetActive(feed_result.feeds.Length == 0);
                myFeed_scroll.ListStart(feed_result.feeds.Length / 3);
            }, (error) =>
            {
                // TODO :: Error Popup
            });
        }

        void OnValueChangedFeed(bool _isOn)
        {
            feed_obj.SetActive(_isOn);

            if (_isOn is false)
                return;
        }

        void OnValueChangedActive(bool _isOn)
        {
            active_obj.SetActive(_isOn);

            if (_isOn is false)
                return;

        }

        void OnClickCoinRefresh()
        {
            if (PPM.GuestLogin is true)
            {
                activeNonText_obj.SetActive(true);
                myActive_scroll.ListStart(0);
                myActive_scroll.Refresh();
                availCoin_text.text = "-";
                expireCoin_text.text = "-";
                return;
            }

            DC.CoinHistoryPage = 1;
            DC.data_coinHistory = null;
            DC.data_totalCoinHistroy = new List<CoinHistoryItem>();

            NetworkManager.s.GetCoinHistory(DC.CoinHistoryPage, (result) =>
            {
                activeNonText_obj.SetActive(result.results.Length == 0);
                myActive_scroll.ListStart(result.results.Length);
            }, (error) =>
            {
                Debug.LogError(error);
            });

            NetworkManager.s.GetCoinStatus((result) =>
            {
                availCoin_text.text = result.available.ToString("N0");
                expireCoin_text.text = result.expire_soon.ToString("N0");
            }, (error) =>
            {
                Debug.LogError(error);
            });
        }
    }
}
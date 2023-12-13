using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TM = Marvrus.Util.TextureMaker;
using DC = Marvrus.Data.DataContainer;
using PC = Marvrus.Util.PermissionCheck;
using DF = Marvrus.Util.Define;
using PPM = Marvrus.Util.PlayerPrefsManager;
using System.Linq;
using System.Text;
using System.IO;

namespace Marvrus.UI
{
    public class UIPopup_UploadFeed : UIPopup
    {
        [Header("DG Animation")]
        public DOTweenAnimation ani;

        [Header("Complete Button")]
        public Button completeBtn;

        [Header("Scroll Snap")]
        public Scroll.ScrollSnap_Thumbnail snap;

        [Header("Selected Photo")]
        public Button selectPhotoBtn;
        public Button selectCameraBtn;
        public Util.SelectedPhotoItem selectedPhotoBase;
        private List<Util.SelectedPhotoItem> selectedPhotoList = new List<Util.SelectedPhotoItem>();

        [Header("Content InputField")]
        public TMP_InputField contentInputField;

        [Header("Close Notice")]
        public Button closeNoticeBtn;
        public GameObject notice;

        [Header("Open Policy")]
        public Button policyBtn;

        private bool isEditMode = false;
        private Feed editModeFeed = null;
        private List<string> selectedFilePath = new List<string>();

        private bool passContentSize = true;

        public override void Close()
        {
            UIManager.s.OpenTwoButton(
                _title: Const.TOAST_UPLOAD_CANCEL_TITLE,
                _desc: Const.TOAST_UPLOAD_CANCEL_DESC,
                _leftText: Const.CANCEL,
                _rightText: Const.DELTE,
                _right: () =>
                {
                    ani.DOPlayBackwardsAllById(Const.DG_KEY_UIPOPUP_UPLOAD);
                    Invoke(nameof(CloseInvoke), ani.duration);
                });
        }

        private void CloseInvoke()
        {
            this.Hide();
            snap.Clear();
        }

        public override void Awake()
        {
            base.Awake();
            snap.Init();
            snap.Clear();

            selectedPhotoBase.SetActive(false, false);

            for (int i = 0; i < Const.MAX_MEDIA_COUNT; i++)
            {
                Util.SelectedPhotoItem selectedPhoto = Instantiate(selectedPhotoBase.gameObject, selectedPhotoBase.transform.parent).GetComponent<Util.SelectedPhotoItem>();
                selectedPhoto.Init();

                selectedPhotoList.Add(selectedPhoto);
            }

            completeBtn.onClick.AddListener(OnClickComplete);
            selectPhotoBtn.onClick.AddListener(OnClickSelectPhoto);
            selectCameraBtn.onClick.AddListener(OnClickCamera);
            closeNoticeBtn.AddListener(() => notice.SetActive(false));
            policyBtn.onClick.AddListener(OnClickPolicy);
            contentInputField.onValueChanged.AddListener(OnValueChangedContent);
        }

        private void FixedUpdate()
        {
            safeAreaRect.anchoredPosition =
                contentInputField.isFocused ?
                new Vector2(0, PPM.KeyboardHeight * safeArea.ratio) :
                Vector2.zero;
        }

        public override void UpdateData(params object[] args)
        {
            base.UpdateData(args);

            if (args is null || args.Length == 0)
            {
                isEditMode = false;
            }
            else
            {
                isEditMode = (bool)args[0];
            }

            snap.Clear();
            selectedFilePath = new List<string>();

            if (isEditMode is true)
            {
                Feed data = DC.SelectedFeed;

                contentInputField.text = data.content;

                SelectPhoto(data.medias.Select(x => x.thumbnail_url).ToArray());
                snap.ItemCount = selectedFilePath.Count;
                snap.UpdateThumbnails(selectedFilePath.ToArray());
            }
            else
            {
                completeBtn.SetActive(false);
                contentInputField.text = "";
                SelectPhoto();
            }

            notice.SetActive(isEditMode == false);
            selectPhotoBtn.SetActive(isEditMode == false);
            selectCameraBtn.SetActive(isEditMode == false);

            passContentSize = true;
        }

        private void SelectPhoto(params string[] _path)
        {
            for (int i = 0; i < _path.Length; i++)
            {
                if (selectedFilePath.Count >= Const.MAX_MEDIA_COUNT)
                {
                    Debug.LogWarning("The number of files has been exceeded...");
                    UIManager.s.OpenToast(string.Format(Const.TOAST_UPLOAD_MAX_MEDIA, Const.MAX_MEDIA_COUNT));
                    break;
                }

                selectedFilePath.Add(_path[i]);
            }

            RefreshPhtoItem();
        }

        private void RefreshPhtoItem()
        {
            for (int i = 0; i < selectedPhotoList.Count; i++)
            {
                string path = i >= selectedFilePath.Count ? "" : selectedFilePath[i];
                selectedPhotoList[i].SetImage(path, isEditMode);
            }
        }

        private void RefreshThumbnials()
        {
            snap.ItemCount = selectedFilePath.Count;
            snap.UpdateThumbnails(selectedFilePath.ToArray(), true);

            SetComplteBtn();
        }

        public void OnClickSelectedPhotoDelete(int _index)
        {
            selectedFilePath.RemoveAt(_index);
            RefreshPhtoItem();
            RefreshThumbnials();
        }

        public void OnClickComplete()
        {
            if (isEditMode is true)
            {
                NetworkManager.s.EditFeed(DC.SelectedFeed.id, contentInputField.text, (result) =>
                {
                    UIManager.s.RefreshFeed();
                    Hide();
                    UIManager.s.FindPopup<UIPopup_FeedDetail>().SetText(result.content);
                    UIManager.s.OpenToast(Const.TOAST_FEED_EDIT_COMPLETE);
                }, (error) =>
                {
                    Debug.LogError(error);
                });
            }
            else
            {
                List<double> sizeList = new List<double>();

                for(int i =0; i< selectedFilePath.Count; i++)
                {
                    long legth = new FileInfo(selectedFilePath[i]).Length;
                    sizeList.Add(Extension.ConvertBytesToMegabytes(legth));
                }

                if (sizeList.Sum() >= Const.MAX_MEDIA_SIZE)
                {
                    string msg = "File Size :";

                    for (int i = 0; i < sizeList.Count; i++)
                    {
                        if (i != 0)
                        {
                            msg += ",";
                        }

                        msg += $"{sizeList[i]:F2}MB";
                    }

                    UIManager.s.OpenToast($"{Const.TOAST_UPLOAD_MAX_SIZE}\n{msg}");
                    return;
                }

                UIManager.s.Loading = true;

                Protocol.Request.FeedUpload data = new Protocol.Request.FeedUpload
                {
                    content = contentInputField.text,
                    medias = selectedFilePath.ToArray()
                };

                NetworkManager.s.UploadFeed(data, (result) =>
                {
                    long coin = result.coin.amount;

                    if (coin > 0)
                    {
                        UIManager.s.OpenToast(string.Format(Const.TOAST_EARNED_COIN, result.coin.amount));
                    }

                    UIManager.s.RefreshFeed();
                    this.Hide();
                }, (error) =>
                {
                    Debug.LogError(error);
                });
            }
        }

        public void OnClickSelectPhoto()
        {
            if (DF.platform == DF.Platform.UnityEditor)
            {
                NativeGallery.GetImageFromGallery((result) =>
                {
                    if (result is null)
                    {
                        Debug.LogError($"UIPopup_UploadFeed Selected result is null");
                        return;
                    }

                    SelectPhoto(result);
                    RefreshThumbnials();
                });
            }
            else
            {
                PC.RequestGallery(() =>
                {
                    NativeGallery.GetImagesFromGallery((result) =>
                    {
                        if (result is null)
                        {
                            Debug.LogError($"UIPopup_UploadFeed Selected result is null");
                            return;
                        }

                        SelectPhoto(result);
                        RefreshThumbnials();
                    });
                });
            }
        }

        public void OnClickCamera()
        {
            if (DF.platform == DF.Platform.UnityEditor)
            {
                Debug.Log($"can't suport platform..");
            }
            else
            {
                PC.RequestCamera(() =>
                {
                    NativeCamera.TakePicture((result) =>
                    {
                        if (result is null)
                        {
                            Debug.LogError($"UIPopup_UploadFeed Selected result is null");
                            return;
                        }

                        var properties = NativeCamera.GetImageProperties(result);

                        Util.FileCache.s.LoadImage(result, new Vector2(properties.width, properties.height), (texture) =>
                        {
                            Texture2D output = TM.RotateTexture(texture, properties.orientation);
                            Texture2D output2 = TM.ResizeTexture(output, Const.IMAGE_SIZE_UPLOAD);

                            string path = TM.WriteTexture(output2, "FeedUploadCamera", DateTime.Now.Ticks.ToString());
                            SelectPhoto(path);
                            RefreshThumbnials();
                        });
                    });
                });
            }
        }

        private void SetComplteBtn()
        {
            completeBtn.SetActive(passContentSize == true && selectedFilePath.Count > 0);
        }

        public void OnClickPolicy()
        {
            UIManager.s.OpenPopupWithData<UIPopup_TermsOperatingPolicy>(true);
        }

        public void OnValueChangedContent(string _text)
        {
            int size = Encoding.Default.GetByteCount(_text);

            if (size >= Const.MAX_CONTENTS_SIZE)
            {
                UIManager.s.OpenToast(Const.TOAST_EXCESS_TEXT);
                passContentSize = false;
                SetComplteBtn();
                return;
            }

            passContentSize = true;
            SetComplteBtn();
        }
    }
}
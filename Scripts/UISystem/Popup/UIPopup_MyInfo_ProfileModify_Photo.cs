using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;
using TM = Marvrus.Util.TextureMaker;
using PC = Marvrus.Util.PermissionCheck;
using DF = Marvrus.Util.Define;

namespace Marvrus.UI
{
    public class UIPopup_MyInfo_ProfileModify_Photo : UIPopup
    {
        public RawImage picture;
        public Image mask;
        public Button cameraBtn;
        public Button selectPhotoBtn;
        public Button completeBtn;

        private Texture defaultTexture;
        private string selectedFilePath = string.Empty;


        public override void Awake()
        {
            base.Awake();
            defaultTexture = picture.texture;
            completeBtn.onClick.AddListener(OnClickComplete);
            cameraBtn.onClick.AddListener(OnClickCamera);
            selectPhotoBtn.onClick.AddListener(OnClickSelectPhoto);
        }

        public override void UpdateData(params object[] args)
        {
            selectedFilePath = string.Empty;
            completeBtn.SetActive(false);
            picture.texture = defaultTexture;
            picture.transform.localScale = Vector3.one;
        }

        public void OnClickSelectPhoto()
        {
            if (DF.platform == DF.Platform.UnityEditor)
            {
                NativeGallery.GetImageFromGallery((result) =>
                {
                    Util.FileCache.s.LoadImage(result, Const.IMAGE_SIZE_MY, (texture) =>
                    {
                        picture.texture = texture;
                        selectedFilePath = result;
                        completeBtn.SetActive(true);
                    });
                });
            }
            else
            {
                PC.RequestGallery(() =>
                {
                    NativeGallery.GetImageFromGallery((result) =>
                    {
                        var properties = NativeGallery.GetImageProperties(result);

                        Util.FileCache.s.LoadImage(result, new Vector2(properties.width, properties.height), (texture) =>
                        {
                            picture.texture = texture;
                            picture.rectTransform.sizeDelta = new Vector2(properties.width, properties.height);
                            selectedFilePath = result;
                            completeBtn.SetActive(true);
                        });
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
                            Texture2D output2 = TM.ResizeTexture(output, Const.IMAGE_SIZE_MY);

                            string path = TM.WriteTexture(output2, "ProfileUploadCamera", DateTime.Now.Ticks.ToString());

                            picture.texture = output2;
                            selectedFilePath = path;
                            completeBtn.SetActive(true);
                        });
                    });
                });
            }            
        }

        public void OnClickComplete()
        {
            if(string.IsNullOrEmpty(selectedFilePath))
            {
                return;
            }

            NetworkManager.s.EditProfile(DC.data_myProfile.userName, selectedFilePath, (result) =>
            {
                NetworkManager.s.GetMyProfile((p_result) =>
                {
                    UIManager.s.FindPopup<UIPopup_My>().Refresh(p_result);
                    var myInfoProfileModify = UIManager.s.FindPopup<UIPopup_MyInfo_ProfileModify>();
                    myInfoProfileModify.Refresh();
                    myInfoProfileModify.ChangeProfileAction();
                    Close();
                    UIManager.s.OpenToast(Const.TOAST_EDIT_PROFILE_PHOTO);
                });

            }, (error) =>
            {
                Debug.LogError(error);
            });
        }
    }
}
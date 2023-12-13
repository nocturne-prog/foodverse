using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.Util
{
    public class SelectedPhotoItem : MonoBehaviour
    {
        public RawImage image;
        private Button button;
        public Button delete;

        public void Init()
        {
            button = image.gameObject.GetComponent<Button>();

            var popup = UIManager.s.FindPopup<UI.UIPopup_UploadFeed>();

            // Base가 0번
            int index = transform.GetSiblingIndex() - 1;

            //button.onClick.AddListener(() => popup.OnClickSelectedPhoto(index));
            delete.onClick.AddListener(() => popup.OnClickSelectedPhotoDelete(index));
        }

        public void SetImage(string _path, bool _isEditMode)
        {
            if (string.IsNullOrEmpty(_path))
            {
                gameObject.SetActive(false);
                return;
            }

            if (_isEditMode is true)
            {
                FileCache.s.GetImage(_path, Const.IMAGE_SIZE_UPLOAD_ITEM, (texture) =>
                {
                    image.texture = texture;
                    SetActive(true, _isEditMode);
                });
            }
            else
            {
                FileCache.s.LoadImage(_path, Const.IMAGE_SIZE_UPLOAD_ITEM, (texture) =>
                {
                    image.texture = texture;
                    SetActive(true, _isEditMode);
                });
            }    
        }

        public void SetActive(bool _active, bool _isEditMode)
        {
            delete.SetActive(_isEditMode == false);
            gameObject.SetActive(_active);
        }
    }
}
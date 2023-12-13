using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.Scroll
{
    public class RecycleScroll_UserList_Item : RecycleScroll_Item
    {
        public Button button;
        public Util.ProfileImageItem profile;
        public TextMeshProUGUI userName;

        public override void UpdateItem(int _index)
        {
            if (_index < 0)
                return;

            var data = DC.data_userList;

            if (_index >= data.Length)
            {
                gameObject.SetActive(false);
                return;
            }
            else
            {
                gameObject.SetActive(true);
            }

            profile.texture = null;

            if (string.IsNullOrEmpty(data[_index].picture_url) is false)
            {
                Util.FileCache.s.GetImage(data[_index].picture_url, Const.IMAGE_SIZE_MY, (texture) =>
                {
                    profile.texture = texture;
                });
            }

            userName.text = data[_index].userName;
        }
    }
}
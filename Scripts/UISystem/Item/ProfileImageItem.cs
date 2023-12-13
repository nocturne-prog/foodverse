using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Marvrus.Util
{
    public class ProfileImageItem : MonoBehaviour
    {
        public RawImage img;
        public Texture2D defaultSprite;

        public Texture2D texture
        {
            set
            {
                SetImage(value);
            }
        }

        public void SetImage(Texture2D _texture)
        {
            if (_texture is null)
            {
                img.texture = defaultSprite;
            }
            else
            {
                img.texture = _texture;
            }
        }
    }
}
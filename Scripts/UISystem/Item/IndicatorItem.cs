using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Marvrus.Util
{
    public class IndicatorItem : MonoBehaviour
    {
        public GameObject obj_on;
        public GameObject obj_off;

        public void SetActive(bool _active)
        {
            gameObject.SetActive(_active);
        }

        public bool On
        {
            set
            {
                obj_on.SetActive(value);
                obj_off.SetActive(!value);
            }
        }

        public bool isOn
        {
            get
            {
                return obj_on.activeInHierarchy;
            }
        }
    }
}
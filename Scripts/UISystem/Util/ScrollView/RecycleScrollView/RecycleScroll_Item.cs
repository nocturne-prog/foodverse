using UnityEngine;

namespace Marvrus.Scroll
{
    public abstract class RecycleScroll_Item : MonoBehaviour
    {
        public abstract void UpdateItem(int _index);
        public virtual void Refresh()
        {
            UpdateItem(-100);
        }

        public virtual void Repaint()
        {
        }

        public virtual void Init()
        {
        }

        public virtual void SetData(params object[] _args)
        {
        }
    }
}
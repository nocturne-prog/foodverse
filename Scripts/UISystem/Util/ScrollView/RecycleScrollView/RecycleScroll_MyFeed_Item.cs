using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.Scroll
{

    public class RecycleScroll_MyFeed_Item : RecycleScroll_Item
    {
        public RecycleScroll_MyFeed_ItemToken[] tokens;

        public override void UpdateItem(int _index)
        {
            if (_index < 0)
            {
                foreach (var v in tokens)
                {
                    v.Hide();
                }

                return;
            }

            int localIndex = _index * tokens.Length;

            for (int i = 0; i < tokens.Length; i++)
            {
                int h = localIndex + i;

                if (h < DC.data_totalMyFeed.Count)
                {
                    tokens[i].UpdateData(DC.data_totalMyFeed[h]);
                }
                else
                {
                    tokens[i].Hide();
                }
            }
        }
    }
}
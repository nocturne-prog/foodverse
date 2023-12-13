using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DC = Marvrus.Data.DataContainer;

namespace Marvrus.Scroll
{
    public class RecycleScroll_My_Active : RecycleScroll
    {
        protected override void OnEndScroll(Action _update)
        {
            NetworkManager.s.GetCoinHistory(DC.CoinHistoryPage, (result) =>
            {
                SetContentSize(result.results.Length);
                _update();

            }, (error) =>
            {

            });
        }
    }
}

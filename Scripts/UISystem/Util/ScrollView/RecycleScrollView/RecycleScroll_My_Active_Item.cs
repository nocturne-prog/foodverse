using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DC = Marvrus.Data.DataContainer;
using Marvrus.Util;

namespace Marvrus.Scroll
{
    public class RecycleScroll_My_Active_Item : RecycleScroll_Item
    {
        public RawImage thumbnail;
        public TextMeshProUGUI text;
        public TextMeshProUGUI coin;
        public TextMeshProUGUI date;
        public Texture signupTexture;

        private CoinHistoryItem data;

        public override void UpdateItem(int _index)
        {
            if(_index < 0 || _index >= DC.data_totalCoinHistroy.Count)
            {
                gameObject.SetActive(false);
                return;
            }

            data = DC.data_totalCoinHistroy[_index];
            UpdateData();
            gameObject.SetActive(true);
        }

        private void UpdateData()
        {
            if (string.IsNullOrEmpty(data.thumbnailUrl))
            {
                thumbnail.texture = signupTexture;
            }
            else
            {
                FileCache.s.GetImage(data.thumbnailUrl, Const.IMAGE_SIZE_MY_ACTION, (texture) =>
                {
                    thumbnail.texture = texture;
                });
            }
            
            Coin = data.coinAmount;
            text.text = TransactionType(data.transactionType);
            date.text = data.createDt.ToString("yyyy.MM.dd");
        }

        private long Coin
        {
            set
            {
                coin.text = $"+ {value}C";
            }
        }

        private string TransactionType(string type)
        {
            if (string.IsNullOrEmpty(type))
                return "";

            switch (type)
            {
                case "JOIN": return Const.COIN_TRANSACTION_JOIN;
                case "FIRST_ARTICLE": return Const.COIN_TRANSACTION_FIRST_ARTICLE;
                case "ARTICLE": return Const.COIN_TRANSACTION_ARTICLE;
                case "COMMENT": return Const.COIN_TRANSACTION_COMMENT;

                default: return Const.COIN_TRANSACTION;
            }
        }
    }
}

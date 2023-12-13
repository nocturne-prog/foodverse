using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIHomeCard : MonoBehaviour
{
	public DOTweenAnimation cardThumbnail;

	public void Scroll(bool isRight)
    {
		if (isRight)
			cardThumbnail.DOPlay();
		else
			cardThumbnail.DORewind();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarSegment : MonoBehaviour
{
    [field: SerializeField] private Image image;

    public void UpdateFillPercent(float percent)
    {
        image.fillAmount = percent;
    }

    public void RemoveSelf()
    {
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarSegment : MonoBehaviour
{
    [field: SerializeField] private Image image;
    
    public void SetToGreen()
    {
        image.color = Color.green;
    }
    
    public void SetToYellow()
    {
        image.color = Color.yellow;
    }

    public void UpdateFillPercent(float percent)
    {
        image.fillAmount = percent;
    }
}

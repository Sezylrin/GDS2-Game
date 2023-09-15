using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarSegmentController : MonoBehaviour
{
    public GameObject segmentPrefab;
    private List<HealthBarSegment> segments = new List<HealthBarSegment>();
    private int MaxHealth { get; set; }
    [field: SerializeField] private int HealthOfSegment { get; set; } = 100;
    [field: SerializeField] private int LowHealthThreshholdPercent { get; set; } = 25;
    private IDamageable Parent { get; set; }

    public void SetInitialSegments(int maxHealth)
    {
        MaxHealth = maxHealth;

        if (segments.Count > (MaxHealth / HealthOfSegment)) // If there are more segments in the list than there should be
        {
            if (CheckForEvenHealth()) // If the max health divides evenly amongst all segments
            {
                while (segments.Count > MaxHealth / HealthOfSegment) // Remove segments until we reach the right amount
                {
                    RemoveSegment();
                }
            }
            else
            {
                while (segments.Count > (MaxHealth / HealthOfSegment) + 1) // Remove segments until we have the right amount of equal segments + 1 segment for the leftover health
                {
                    RemoveSegment();
                }
            }
        }
        else //If there are less segments in the list than there should be
        {
            while (segments.Count < MaxHealth / HealthOfSegment) // Add segments until we reach the right amount (including + 1 segment for leftover health if needed)
            {
                CreateNewSegment();
            }
        }

        foreach (HealthBarSegment segment in segments) // Set all the segments to full and to green
        {
            segment.SetToGreen();
            segment.UpdateFillPercent(100);
        }
        if (!CheckForEvenHealth()) // If max health does not divide evenly amongst all segments, update the last segment to the correct visual
        {
            int lastSegmentSize = MaxHealth - (HealthOfSegment * (segments.Count - 1));
            segments[segments.Count - 1].UpdateFillPercent(HealthOfSegment / lastSegmentSize);
        }
    }

    private void CreateNewSegment() // Instantiates a new segment and adds it to the list
    {
            GameObject tempObj = Instantiate(segmentPrefab, gameObject.transform);
            HealthBarSegment tempSegment = tempObj.GetComponent<HealthBarSegment>();
            segments.Add(tempSegment);
    }

    private void RemoveSegment() // Removes the last segment in the list
    {
        segments.RemoveAt(segments.Count - 1);
    }

    public void UpdateSegments(int currentHealth)
    {
        float numberOfActualSegments = (float)currentHealth / (float)HealthOfSegment;
        float numberOfFullSegments = Mathf.Floor(currentHealth / HealthOfSegment);
        float leftoverHealthPercent = numberOfActualSegments - numberOfFullSegments;

        for (int i = (int)numberOfFullSegments;  i < segments.Count; i++) 
        {
            segments[i].UpdateFillPercent(0);
        }

        if (leftoverHealthPercent != 0)
        {
            segments[(int)numberOfFullSegments].UpdateFillPercent(leftoverHealthPercent);
        }

        if (currentHealth <= MaxHealth * LowHealthThreshholdPercent / 100)
        {
            foreach (HealthBarSegment segment in segments)
            {
                segment.SetToYellow();
            }
        }
    }

    private bool CheckForEvenHealth()
    {
        return MaxHealth % HealthOfSegment == 0;
    }
}

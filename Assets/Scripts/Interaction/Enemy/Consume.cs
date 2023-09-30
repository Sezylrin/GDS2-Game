using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consume : MonoBehaviour
{
    [field: SerializeField] private Enemy enemy;
    [field: SerializeField] private GameObject UI;
    private int healthReceivedPercent;

    public void SetStats(int healthReceivedOnConsumePercent)
    {
        healthReceivedPercent = healthReceivedOnConsumePercent;
    }

    public void TriggerConsume()
    {
        if (enemy.CheckIfConsumable() && GameManager.Instance.PCM.system.CanConsume())
        {
            GameManager.Instance.RemoveConsume(this);
            GameManager.Instance.PCM.system.UseConsume(healthReceivedPercent);
            enemy.OnDeath();
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.T_Player))
        {
            GameManager.Instance.SetConsume(this);
            if (UI)
                UI.SetActive(true);
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.T_Player))
        {
            GameManager.Instance.RemoveConsume(this);
            if (UI)
                UI.SetActive(false);
        }
    }
}

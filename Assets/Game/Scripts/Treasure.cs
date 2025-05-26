using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [SerializeField]
    private int minValue, maxValue;
    private int value;

    private void Start()
    {
        value = Random.Range(minValue, maxValue);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Chest");
        GameManager.Instance.GetCoin(value);
        SpawnerManager.Instance.SpawnTreasureWithDelay(5);
        GameObject.Destroy(gameObject);
    }
}

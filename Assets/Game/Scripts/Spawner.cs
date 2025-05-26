using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject itemToSpawn;
    
    public void Spawn()
    {
        GameObject.Instantiate(itemToSpawn,transform.position, Quaternion.identity);
    }
}

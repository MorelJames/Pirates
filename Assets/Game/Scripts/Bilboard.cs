using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bilboard : MonoBehaviour
{
    private Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindAnyObjectByType<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position+cam.transform.forward);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noteScroller : MonoBehaviour
{
    public float beatTempo;
    public bool hasStarted;
    void Start()
    {
        beatTempo = beatTempo / 60f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted)
        {
            if (Input.anyKeyDown)
            {
                hasStarted = true;
            }
        }else {
            transform.position -= new Vector3(0, beatTempo * Time.deltaTime, 0);
        }
        
    }
}

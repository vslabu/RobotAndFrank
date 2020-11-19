using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSight : MonoBehaviour
{
	GameObject frank;

    // Start is called before the first frame update
    void Start()
    {
		frank = GameObject.FindGameObjectWithTag("Frank");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

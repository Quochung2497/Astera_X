using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAsteroidOnCollision : MonoBehaviour {


    
	void Update()
    {
        if(Input.GetMouseButtonDown(1))
        { Destroy(gameObject); }
    }
}

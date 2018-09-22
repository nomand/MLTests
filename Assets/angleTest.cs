using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class angleTest : MonoBehaviour {

	void Update ()
    {
        float z = (Vector3.Angle(transform.right, Vector3.up) - 90) * -1;
        float x = Vector3.Angle(transform.forward, Vector3.up) - 90;
        print(x + ", " + z);
	}
}
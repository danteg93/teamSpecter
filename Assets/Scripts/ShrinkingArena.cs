using UnityEngine;
using System.Collections;

public class ShrinkingArena : MonoBehaviour {

    // Use this to shrink the arena
    void Shrink()
    {
        //subtract 0.5 from the scale of the arena
        transform.localScale += new Vector3(-0.5F, -0.5F, 0);
    }

	// Use this for initialization
	void Start () {
        //Shrink the arena starting 15 seconds in and every 10 seconds after.
        InvokeRepeating("Shrink", 3, 3);
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.localScale.x <= 0.5 && transform.localScale.y <= 0.5)
        {
            CancelInvoke("Shrink");
        }
	}
}

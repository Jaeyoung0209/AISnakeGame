using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ray : MonoBehaviour
{
    public int val;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "wall" || collision.gameObject.tag == "tail")
            val = 1;
        else if (collision.gameObject.tag == "floor")
            val = 0;
    }
}

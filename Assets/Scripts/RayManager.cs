using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class RayManager : MonoBehaviour
{
    [SerializeField]
    private List<Ray> rays = new List<Ray>();

    public float returnval;

    public void CheckRays()
    {
        float tempval = 1;
        for(int i = rays.Count-1; i >= 0; i--)
        {
            if (rays[i].val == 1)
            {
                if (i == 4)
                    tempval = 0.8f;
                else if (i == 3)
                    tempval = 0.6f;
                else if (i == 2)
                    tempval = 0.4f;
                else if (i == 1)
                    tempval = 0.2f;
                else if (i == 0)
                    tempval = 0;
            }
        }
        //Debug.Log(tempval);
        returnval = tempval;
    }
}

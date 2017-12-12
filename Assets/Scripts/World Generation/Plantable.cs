using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plantable : MonoBehaviour {

    [Range(0,1)]
    public float fertility = 1;
    public float minTemp = 0;
    public float maxTemp = 1;

    void OnValidate() {
        if (minTemp < 0)
            minTemp = 0;
        if (minTemp > maxTemp)
            maxTemp = minTemp;
        if (maxTemp > 1)
            maxTemp = 1;
        if (maxTemp < minTemp)
            minTemp = maxTemp;
    }

}

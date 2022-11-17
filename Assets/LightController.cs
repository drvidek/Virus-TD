using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float rot = NetworkManager.GetPlayerIDNormalised() == 0 ? 180 : 0;
        transform.localEulerAngles = new Vector3(60, rot, 0);
    }
}

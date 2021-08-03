using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// 
/// Class to rotate camera when playing in mouse mode.
///
/// </summary>
////////////////////////////////////////////////////////////////////////////////

public class Rotate : MonoBehaviour
{
    //== Methods ===============================================================

    public void setAngle(float angle)
    {
        transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}

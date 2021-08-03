using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// 
/// Class to describe Jenga user info.
///
/// </summary>
////////////////////////////////////////////////////////////////////////////////

[System.Serializable]
public class JengaUser
{
    //== Properties ============================================================

    public string name;                     // Name of the user.

    public string ip;                       // IP of the user.

    public int port;                        // Port of the user.

    [System.NonSerialized]
    public float lastUpdate;                // Last time user data was updated.

    //== Constructor ===========================================================

    public JengaUser()
    {
        name = ip = "";
        port = 0;
        lastUpdate = 0;
    }
}

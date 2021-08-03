using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// 
/// Class to show Jenga user info.
///
/// </summary>
////////////////////////////////////////////////////////////////////////////////

public class JengaUserButton : MonoBehaviour
{
    //== Properties ============================================================

    public JengaUser user;

    //== UI Elements ===========================================================

    public Text labelName;

    public Text labelType;

    //== Methods ===============================================================

    public void setUser(JengaUser u)
    {
        user = u;
        user.lastUpdate = Time.time;
        labelName.text = u.name;
    }
}

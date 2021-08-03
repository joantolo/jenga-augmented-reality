using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// 
/// Class to store Jenga messages and send them to other players.
///
/// </summary>
////////////////////////////////////////////////////////////////////////////////

[System.Serializable]
public class JengaMatchMakerMessage
{
    //== Properties ============================================================

    public string cmd;                          // Message to send.

    public JengaUser[] users;                   // Users to receive messages.

    public JengaUser user;                      // User that sends message.

    //== Constructor ===========================================================

    public JengaMatchMakerMessage()
    {
        cmd = "";
        users = new JengaUser[0];
        user = null;
    }
}

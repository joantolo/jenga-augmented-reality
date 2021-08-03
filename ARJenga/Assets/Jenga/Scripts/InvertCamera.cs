using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// 
/// Class to make the rendering image of the AR camera look like a mirror.
///
/// </summary>
////////////////////////////////////////////////////////////////////////////////

[RequireComponent(typeof(Camera))]
public class InvertCamera : MonoBehaviour
{
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, new Vector2(-1, 1), new Vector2(1, 0));
    }
}

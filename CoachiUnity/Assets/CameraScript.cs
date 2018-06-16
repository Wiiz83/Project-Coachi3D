﻿//SmoothLookAt.cs
//Written by Jake Bayer
//Written and uploaded November 18, 2012
//This is a modified C# version of the SmoothLookAt JS script.  Use it the same way as the Javascript version.

using System.Collections;
using UnityEngine;

///<summary>
///Looks at a target
///</summary>
public class CameraScript : MonoBehaviour {
    public Transform target;
    public float damping = 6.0f;

    // Use this for initialization
    void Start () {
    }

    void Update () {

    }
    void LateUpdate () {
        Quaternion rotation = Quaternion.LookRotation (target.position - transform.position);
        transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * damping);
    }
}
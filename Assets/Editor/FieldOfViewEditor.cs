using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor (typeof(FieldOfView))]
public class FieldOfViewEditor : Editor {
    public void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireDisc(fov.transform.position, Vector3.up, fov.viewRadius);
    }
}

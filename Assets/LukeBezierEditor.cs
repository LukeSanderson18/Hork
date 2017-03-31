/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(LukeBezier))]
public class LukeBezierEditor : Editor
{
    LukeBezier lukeBezier;

    public override void OnInspectorGUI()
    {
       // lukeBezier = (LukeBezier)target;
    }
    void OnSceneGUI()
    {
        //lukeBezier = (LukeBezier)GameObject.Find("Bezier");
        Handles.color = Color.grey;


        //Handles.DrawLine(lukeBezier.transform.position + (Vector3)lukeBezier.startControlPoint, lukeBezier.transform.position + (Vector3)lukeBezier.startPoint);

      //  Handles.DrawLine(lukeBezier.transform.position + (Vector3)lukeBezier.endControlPoint, lukeBezier.transform.position + (Vector3)lukeBezier.endPoint);

        lukeBezier.startPoint = Handles.FreeMoveHandle(lukeBezier.transform.position + ((Vector3)lukeBezier.startPoint), Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(lukeBezier.transform.position + ((Vector3)lukeBezier.startPoint)), Vector3.zero, Handles.DotCap) - lukeBezier.transform.position;
        lukeBezier.endPoint = Handles.FreeMoveHandle(lukeBezier.transform.position + ((Vector3)lukeBezier.endPoint), Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(lukeBezier.transform.position + ((Vector3)lukeBezier.endPoint)), Vector3.zero, Handles.DotCap) - lukeBezier.transform.position;
        lukeBezier.startControlPoint = Handles.FreeMoveHandle(lukeBezier.transform.position + ((Vector3)lukeBezier.startControlPoint), Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(lukeBezier.transform.position + ((Vector3)lukeBezier.startControlPoint)), Vector3.zero, Handles.DotCap) - lukeBezier.transform.position;
        lukeBezier.endControlPoint = Handles.FreeMoveHandle(lukeBezier.transform.position + ((Vector3)lukeBezier.endControlPoint), Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(lukeBezier.transform.position + ((Vector3)lukeBezier.endControlPoint)), Vector3.zero, Handles.DotCap) - lukeBezier.transform.position;

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}
*/
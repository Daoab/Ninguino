using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticFunctions : MonoBehaviour
{

    public static Matrix4x4 GetBaseChangeMatrix(Transform goal)
    {
        Matrix4x4 CBM = Matrix4x4.zero; //Change Base Matrix
        Vector3[] GoalVectors = new Vector3[4];
        GoalVectors[0] = goal.right;
        GoalVectors[1] = goal.up;
        GoalVectors[2] = goal.forward;
        GoalVectors[3] = goal.position;
        for (int i = 0; i < 4; i++)
        {
            CBM.SetColumn(i, GoalVectors[i]);
        }
        CBM.m33 = 1.0f;
        return CBM;
    }
    public static void DoBaseChange(Transform obj, Matrix4x4 CBM, bool toObject)
    {
        if (toObject)
        {
            obj.position = CBM.inverse.MultiplyPoint(obj.position);
        }
        else
        {
            obj.position = CBM.MultiplyPoint(obj.position);
        }
    }
    public static float AngleInPlane(Vector3 firstVector, Vector3 secondVector, Vector3 normal)
    {
        firstVector = Vector3.ProjectOnPlane(firstVector, normal).normalized;
        secondVector = Vector3.ProjectOnPlane(secondVector, normal).normalized;
        return Vector3.Angle(firstVector, secondVector);
    }
    public static Vector3 CheckObstaclesAndBringCloser(Vector3 origin, Vector3 direction, float distance, int mask, float ResizeFactor, Vector3 ActualPosition,Vector3 defaultPos)
    {
        RaycastHit hit;
        Vector3 objToHitVec;
        if (Physics.Raycast(origin, direction, out hit, distance, mask))
        {
            objToHitVec = hit.point - origin;
            objToHitVec -= objToHitVec.normalized * ResizeFactor;
            ActualPosition = origin + objToHitVec;
        }
        else
            ActualPosition = defaultPos;
        return ActualPosition;
    }
}

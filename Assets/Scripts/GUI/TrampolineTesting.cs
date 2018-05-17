using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// used for the testing scene.
/// </summary>
public class TrampolineTesting : MonoBehaviour {

    public Trampoline LeftTrampoline;
    public Trampoline RightTrampoline;

    private BezierSpline LeftSplinePoints;
    private BezierSpline RightSplinePoints;
    private Vector3[] LeftSplineStartPos;
    private Vector3[] RightSplineStartPos;
    private float RightBounceHeight;
    private float LeftBounceHeight;
    private float RightBounceSkew;
    private float LeftBounceSkew;
    private float LeftStartSpeed;
    private float RightStartSpeed;
    private float Distance;
    private Vector3 LeftStartPosition;
    private Vector3 RightStartPosition;
    private float RightAngle;
    private float LeftAngle;
    private float RightStartHeight;
    private float RightEndHeight;
    private float LeftStartHeight;
    private float LeftEndHeight;

    void Start()
    {
        LeftSplineStartPos = new Vector3[4];
        RightSplineStartPos = new Vector3[4];
        LeftStartPosition = LeftTrampoline.transform.position;
        RightStartPosition = RightTrampoline.transform.position;
        LeftSplinePoints = LeftTrampoline.ObjectSpline;
        RightSplinePoints = RightTrampoline.ObjectSpline;
        LeftStartSpeed = LeftTrampoline.LaunchSplineSpeed;
        RightStartSpeed = RightTrampoline.LaunchSplineSpeed;
        for(int i = 0; i < LeftSplineStartPos.Length; i++)
        {
            LeftSplineStartPos[i] = LeftSplinePoints.points[i];
            RightSplineStartPos[i] = RightSplinePoints.points[i];
        }
    }


    public void ChangeLeftBounceSpeed(float speed)
    {
        if (speed != 0)
            LeftTrampoline.LaunchSplineSpeed = LeftStartSpeed / speed;
        else LeftTrampoline.LaunchSplineSpeed = LeftStartSpeed / 0.001f;
    }

    public void ChangeRightBounceSpeed(float speed)
    {
        if (speed != 0)
        RightTrampoline.LaunchSplineSpeed = RightStartSpeed / speed;
        else RightTrampoline.LaunchSplineSpeed = RightStartSpeed / 0.001f;
    }

    public void ChangeLeftBounceHeight(float height)
    {
        LeftBounceHeight = height;
        ReconfigureTramp();
    }

    public void ChangeRightBounceHeight(float height)
    {
        RightBounceHeight = height;
        ReconfigureTramp();
    }

    public void ChangeLeftBounceSkew(float skew)
    {
        LeftBounceSkew = skew;
        ReconfigureTramp();
    }

    public void ChangeRightBounceSkew(float skew)
    {
        RightBounceSkew = skew;
        ReconfigureTramp();
    }

    public void ChangeDistance(float dist)
    {
        Distance = dist;
        ReconfigureTramp();
    }

    public void ChangeLeftHeight(float height)
    {
        LeftStartHeight = height;
        RightEndHeight = height;
        ReconfigureTramp();
    }

    public void ChangeRightHeight(float height)
    {
        RightStartHeight = height;
        LeftEndHeight = height;
        ReconfigureTramp();
    }

    void ReconfigureTramp()
    {
        ///omg what 
        RightTrampoline.transform.position = new Vector3(RightStartPosition.x + Distance, RightStartPosition.y + RightStartHeight, RightStartPosition.z);
        LeftTrampoline.transform.position = new Vector3(LeftStartPosition.x, LeftStartPosition.y + LeftStartHeight, LeftStartPosition.z);
        LeftSplinePoints.points[0].y = LeftSplineStartPos[0].y;
        LeftSplinePoints.points[1].y = LeftSplineStartPos[1].y + LeftBounceHeight;
        LeftSplinePoints.points[2].y = LeftSplineStartPos[2].y - RightEndHeight * 1.8f + LeftEndHeight * 1.8f + LeftBounceHeight;
        LeftSplinePoints.points[3].y = LeftSplineStartPos[3].y - RightEndHeight * 1.8f + LeftEndHeight * 1.8f;
        LeftSplinePoints.points[0].x = LeftSplineStartPos[0].x;
        LeftSplinePoints.points[1].x = LeftSplineStartPos[1].x + LeftBounceSkew;
        LeftSplinePoints.points[2].x = LeftSplineStartPos[2].x + Distance * 1.8f + LeftBounceSkew;
        LeftSplinePoints.points[3].x = LeftSplineStartPos[3].x + Distance * 1.8f;
        RightSplinePoints.points[0].y = RightSplineStartPos[0].y;
        RightSplinePoints.points[1].y = RightSplineStartPos[1].y + RightBounceHeight;
        RightSplinePoints.points[2].y = RightSplineStartPos[2].y - LeftEndHeight * 1.8f + RightEndHeight * 1.8f + RightBounceHeight;
        RightSplinePoints.points[3].y = RightSplineStartPos[3].y - LeftEndHeight * 1.8f + RightEndHeight * 1.8f;
        RightSplinePoints.points[0].x = RightSplineStartPos[0].x;
        RightSplinePoints.points[1].x = RightSplineStartPos[1].x + RightBounceSkew;
        RightSplinePoints.points[2].x = RightSplineStartPos[2].x + Distance * 1.8f + RightBounceSkew;
        RightSplinePoints.points[3].x = RightSplineStartPos[3].x + Distance * 1.8f;
    }
}

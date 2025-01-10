using System;
using UnityEngine;

[Serializable]
public class BendingSegment
{
    public Transform firstTransform;
    public Transform lastTransform;

    public float thresholdAngleDifference;
    public float bendingMultiplier = 0.7f;
    public float maxAngleDifference = 10f;
    public float maxHorizontalBendingAngle = 10f;
    public float maxVerticalBendingAngle = 7f;
    public float responsiveness = 4f;

    internal float angleH;
    internal float angleV;

    internal Vector3 dirUp;
    internal Vector3 referenceLookDir;
    internal Vector3 referenceUpDir;

    internal int chainLength;

    internal Quaternion[] origRotations;
}

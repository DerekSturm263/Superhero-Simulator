using System.IO;
using UnityEngine;

public class HandInfo
{
    public static Vector3 shoulderOffsetL = new Vector3(-0.15f, 0.8f, 0f);
    public static Vector3 shoulderOffsetR = new Vector3(0.15f, 0.8f, 0f);

    public enum HandSide
    {
        Left, Right
    }

    public Vector3 RelativePosition { get; private set; }
    public Quaternion RelativeRotation { get; private set; }

    public readonly HandSide handSide;
    private readonly FingerInfo[] _fingers = new FingerInfo[5];

    public System.Collections.Generic.Dictionary<MotionInfo, float> handAngles = new System.Collections.Generic.Dictionary<MotionInfo, float>();
    public System.Collections.Generic.Dictionary<MotionInfo, float> motionSizes = new System.Collections.Generic.Dictionary<MotionInfo, float>();

    public FingerInfo this[int index]
    {
        get
        {
            return _fingers[index];
        }
        set
        {
            _fingers[index] = value;
        }
    }

    public HandInfo(HandSide handSide)
    {
        this.handSide = handSide;

        for (int i = 0; i < 5; ++i)
        {
            _fingers[i] = new FingerInfo((FingerInfo.FingerName) i);
        }
    }

    public static HandInfo FromReference(HandReference hand)
    {
        HandInfo newHand = new HandInfo(hand.handSide);

        for (int i = 0; i < 5; ++i)
        {
            newHand[i] = FingerInfo.FromReference(hand[i], newHand.handSide);
        }

        return newHand;
    }

    public static HandInfo FromFile(string filePath)
    {
        HandInfo newHand = new HandInfo(HandSide.Left);
        float[,] jointValues = new float[5, 3];

        try
        {
            using (StreamReader stream = new StreamReader(filePath, false))
            {
                for (int fingerNum = 0; fingerNum < 5; ++fingerNum)
                {
                    for (int jointNum = 0; jointNum < 3; ++jointNum)
                    {
                        jointValues[fingerNum, jointNum] = System.Convert.ToSingle(stream.ReadLine());
                    }
                }
            }
        }
        catch
        {
            for (int fingerNum = 0; fingerNum < 5; ++fingerNum)
            {
                for (int jointNum = 0; jointNum < 3; ++jointNum)
                {
                    jointValues[fingerNum, jointNum] = 0f;
                }
            }
        }

        newHand.UpdateJoints(jointValues);
        return newHand;
    }

    public void UpdateToReference(HandReference reference)
    {
        float[,] jointValues = new float[5, 3];

        for (int fingerNum = 0; fingerNum < 5; ++fingerNum)
        {
            for (int jointNum = 0; jointNum < 3; ++jointNum)
            {
                jointValues[fingerNum, jointNum] = reference[fingerNum].JointValue(jointNum, reference.handSide);
            }
        }

        RelativePosition = reference.transform.localPosition;
        RelativeRotation = reference.transform.localRotation;

        this.UpdateJoints(jointValues);
    }

    public void UpdateJoints(float[,] jointValues)
    {
        for (int fingerNum = 0; fingerNum < 5; ++fingerNum)
        {
            for (int jointNum = 0; jointNum < 3; ++jointNum)
            {
                this[fingerNum][jointNum] = jointValues[fingerNum, jointNum];
            }
        }
    }

    public bool MatchesGesture(HandInfo target)
    {
        for (int fingerNum = 0; fingerNum < 5; ++fingerNum)
        {
            if (!this[fingerNum].MatchesGesture(target[fingerNum]))
            {
                return false;
            }
        }

        return true;
    }

    public bool MatchingMotion(MotionInfo motion, float errorMargin)
    {
        if (handAngles[motion] == 0f)
        {
            return StartMatchingMotion(motion, errorMargin);
        }

        return ContinueMatchingMotion(motion, errorMargin);
    }

    public bool StartMatchingMotion(MotionInfo motion, float errorMargin)
    {
        Debug.Log("Start Match Motion");

        float distance = Mathf.Abs(RelativePosition.x - motion.offset.x);
        bool isAbove = RelativePosition.y > motion.offset.y;

        return isAbove && distance < errorMargin / 2f;
    }

    public bool ContinueMatchingMotion(MotionInfo motion, float errorMargin)
    {
        Debug.Log("Continue Match Motion");

        float distance = Vector3.Distance(RelativePosition, motion.ToVector(handAngles[motion] * -6.28f, motionSizes[motion], motion.startIndex));
        bool isClose = distance < errorMargin;

        return isClose;
    }

    public float Closeness(HandInfo target)
    {
        float matchVal = 0f;

        for (int fingerNum = 0; fingerNum < 5; ++fingerNum)
        {
            matchVal += this[fingerNum].Closeness(target[fingerNum]);
        }

        return Mathf.Lerp(0f, 1f, 1f - matchVal * 10f);
    }

    public void AddMotion(MotionInfo motion)
    {
        handAngles.Add(motion, 0f);
        motionSizes.Add(motion, 1f);
    }
}
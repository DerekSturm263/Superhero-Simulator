public class FingerInfo
{
    public enum FingerName
    {
        Index, Middle, Pinky, Ring, Thumb
    }

    public readonly FingerName fingerName;
    private readonly float[] _joints = new float[3];

    public float this[int index]
    {
        get
        {
            return _joints[index];
        }
        set
        {
            _joints[index] = value;
        }
    }

    public FingerInfo(FingerName fingerName)
    {
        this.fingerName = fingerName;
    }

    public static FingerInfo FromReference(FingerReference reference, HandInfo.HandSide handSide)
    {
        FingerInfo newFinger = new FingerInfo(reference.fingerName);

        for (int i = 0; i < 3; ++i)
        {
            newFinger[i] = reference.JointValue(i, handSide);
        }

        return newFinger;
    }

    public bool MatchesGesture(FingerInfo target)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (UnityEngine.Mathf.Abs(this[i] - target[i]) > 0.01f)
            {
                return false;
            }
        }

        return true;
    }

    public float Closeness(FingerInfo target)
    {
        float matchVal = 0f;

        for (int jointNum = 0; jointNum < 3; ++jointNum)
        {
            matchVal += UnityEngine.Mathf.Abs(this[jointNum] - target[jointNum]) * (jointNum + 1);
        }

        return matchVal;
    }
}
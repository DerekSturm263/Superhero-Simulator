using UnityEngine;

public class FingerReference : MonoBehaviour
{
    [HideInInspector] public GameObject hand;

    public FingerInfo.FingerName fingerName;
    private readonly GameObject[] _joints = new GameObject[3];

    public GameObject this[int index]
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

    private void Awake()
    {
        hand = transform.parent.parent.parent.parent.parent.parent.gameObject;
        Transform[] children = GetComponentsInChildren<Transform>();

        for (int i = 0; i < 3; ++i)
        {
            _joints[i] = children[i].gameObject;
        }
    }

    public float JointValue(int jointNum, HandInfo.HandSide handSide)
    {
        HandReference hand = handSide == HandInfo.HandSide.Left ? HandFormations.DefaultHandLeft : HandFormations.DefaultHandRight;

        return Vector3.Distance(GetLocalPositionFrom(this[jointNum].transform.position, hand.transform.position), GetLocalPositionFrom(hand[jointNum].transform.position, hand.transform.position)) / (jointNum + 1);
    }

    public Vector3 GetLocalPositionFrom(Vector3 pos1, Vector3 pos2)
    {
        return pos1 - pos2;
    }
}

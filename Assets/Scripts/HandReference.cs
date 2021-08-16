using UnityEngine;
using System.IO;

public class HandReference : MonoBehaviour
{
    [HideInInspector] public GameObject body;
    public HandInfo.HandSide handSide;
    private FingerReference[] _fingers = new FingerReference[5];

    public FingerReference this[int index]
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

    private void Awake()
    {
        _fingers = GetComponentsInChildren<FingerReference>();
    }

    public void SendToFile(string filePath)
    {
        using (StreamWriter stream = new StreamWriter(filePath, false))
        {
            for (int fingerNum = 0; fingerNum < 5; ++fingerNum)
            {
                for (int jointNum = 0; jointNum < 3; ++jointNum)
                {
                    stream.WriteLine(this[fingerNum].JointValue(jointNum, this.handSide));
                }
            }
        }
    }
}

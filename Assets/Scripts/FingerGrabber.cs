using UnityEngine;

public class FingerGrabber : MonoBehaviour
{
    [SerializeField] private FingerInfo.FingerName _fingerName;

    [HideInInspector] public Grabbable touchedObject;

    private void OnCollisionEnter(Collision col)
    {
        Grabbable grabScript = col.gameObject.GetComponent<Grabbable>();

        if (grabScript && !grabScript.HasFinger(this))
        {
            grabScript.AddFinger(this);
            touchedObject = grabScript;
        }
    }

    private void OnCollisionExit(Collision col)
    {
        Grabbable grabScript = col.gameObject.GetComponent<Grabbable>();

        if (grabScript && grabScript.HasFinger(this))
        {
            grabScript.RemoveFinger(this);
            touchedObject = null;
        }
    }
}

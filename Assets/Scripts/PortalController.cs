using UnityEngine;

public class PortalController : MonoBehaviour
{
    public static GameObject player;

    public static (GameObject portal, GameObject pair, GameObject inside, Camera camL, Camera camR, UnityEngine.VFX.VisualEffect vfx, Material mat, Light light)[] portalsInfo = new (GameObject portal, GameObject pair, GameObject inside, Camera camL, Camera camR, UnityEngine.VFX.VisualEffect vfx, Material mat, Light light)[2];

    private void Awake()
    {
        player = gameObject;

        GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");

        for (int i = 0; i < 2; ++i)
        {
            portalsInfo[i].portal = portals[i];
            portalsInfo[i].pair = portals[-i + 1];
            portalsInfo[i].inside = portalsInfo[i].portal.GetComponentInChildren<MeshFilter>().gameObject;

            Camera[] cameras = portalsInfo[i].portal.GetComponentsInChildren<Camera>();
            portalsInfo[i].camL = cameras[0];
            portalsInfo[i].camR = cameras[1];

            portalsInfo[i].vfx = portalsInfo[i].portal.GetComponent<UnityEngine.VFX.VisualEffect>();
            portalsInfo[i].mat = portalsInfo[i].inside.GetComponent<MeshRenderer>().material;
            portalsInfo[i].light = portalsInfo[i].portal.GetComponent<Light>();
        }
    }

    private void Update()
    {
        for (int i = 0; i < 2; ++i)
        {
            UpdatePortalInfo(portalsInfo[i]);
        }
    }

    public static void StartPortal(HandInfo handL, HandInfo handR, MotionInfo motion, (GameObject portal, GameObject pair, GameObject inside, Camera camL, Camera camR, UnityEngine.VFX.VisualEffect vfx, Material mat, Light light) portal)
    {
        Debug.Log("Start Portal");

        if (handR.handAngles[motion] >= 0.99f)
            return;

        handR.handAngles[motion] = 0.01f;
        handR.motionSizes[motion] = Vector2.Distance(handR.RelativePosition, motion.offset);
        portal.inside.transform.localScale = new Vector3(handR.motionSizes[motion], handR.motionSizes[motion], 1f) * 3f;

        (Vector3 pos, Vector3 forward) portalTrans = GetTransformFromHand(handL);
        portal.portal.transform.position = portalTrans.pos;
        portal.portal.transform.forward = portalTrans.forward;

        float groundDistance = -10f;
        if (Physics.Raycast(portal.portal.transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            groundDistance = -hitInfo.distance;
        }
        portal.vfx.SetFloat("_GroundDistance", groundDistance);

        SetFloats(portal.vfx, 0.025f, handR.motionSizes[motion], 0f);
    }

    public static void FormPortal(HandInfo hand, MotionInfo motion, (GameObject portal, GameObject pair, GameObject inside, Camera camL, Camera camR, UnityEngine.VFX.VisualEffect vfx, Material mat, Light light) portal)
    {
        Debug.Log("Form Portal");

        if (hand.handAngles[motion] >= 0.99f)
            return;

        hand.handAngles[motion] = GetHandAngle(hand, motion);

        SetFloats(portal.vfx, hand.handAngles[motion], hand.motionSizes[motion], Vector3.Distance(hand.RelativePosition, motion.ToVector(hand.handAngles[motion] * -6.28f, hand.motionSizes[motion], motion.startIndex)));
        portal.light.intensity = hand.handAngles[motion] * 10f;

        portal.mat.SetFloat("_Opacity", hand.handAngles[motion]);
    }

    public static void CancelPortal(HandInfo hand, MotionInfo motion, (GameObject portal, GameObject pair, GameObject inside, Camera camL, Camera camR, UnityEngine.VFX.VisualEffect vfx, Material mat, Light light) portal)
    {
        Debug.Log("Cancel Portal");

        if (hand.handAngles[motion] >= 0.99f)
            return;

        hand.handAngles[motion] = 0f;
        hand.motionSizes[motion] = 0f;
        portal.inside.transform.localScale = Vector3.zero;

        SetFloats(portal.vfx, 0f, 0f, 0f);
        portal.light.intensity = 0f;

        portal.mat.SetFloat("_Opacity", 0f);
    }

    public void UpdatePortalInfo((GameObject portal, GameObject pair, GameObject inside, Camera camL, Camera camR, UnityEngine.VFX.VisualEffect vfx, Material mat, Light light) portal)
    {
        portal.camL.transform.position = transform.position - portal.pair.transform.position;
        portal.camR.transform.position = transform.position - portal.pair.transform.position;
    }

    private static (Vector3 pos, Vector3 forward) GetTransformFromHand(HandInfo hand)
    {
        Vector3 pos;
        Vector3 forward;

        Vector3 handFromShoulder = hand.RelativePosition - HandInfo.shoulderOffsetL;
        Physics.Raycast(HandInfo.shoulderOffsetL, handFromShoulder.normalized, out RaycastHit hitInfo, handFromShoulder.magnitude * 10f);

        if (hitInfo.collider.gameObject != null)
        {
            pos = hitInfo.point;
            forward = hitInfo.normal;
        }
        else
        {
            pos = HandInfo.shoulderOffsetL + handFromShoulder.normalized * 10f;
            forward = handFromShoulder.normalized;
        }

        return (pos, forward);
    }

    private static float GetHandAngle(HandInfo hand, MotionInfo motion)
    {
        float theta = MotionInfo.Rect2PolarInverse(hand.RelativePosition - motion.offset, (int) motion.direction).theta;
        float angle = (theta + Mathf.PI) / (Mathf.PI * 2f) - 0.5f;
        float trueAngle;

        if (angle < 0)
        {
            trueAngle = 1f + angle;
        }
        else
        {
            trueAngle = angle;
        }

        if (motion.direction == MotionInfo.Direction.Counter_Clockwise)
        {
            trueAngle = 1f - trueAngle;
        }

        if (trueAngle > hand.handAngles[motion] + 0.15f)
        {
            return hand.handAngles[motion];
        }

        return trueAngle;
    }

    private static void SetFloats(UnityEngine.VFX.VisualEffect vfx, float fill, float size, float distance)
    {
        vfx.SetFloat("_Fill", fill);
        vfx.SetFloat("_Size", size * 3f);
        vfx.SetFloat("_Strength", size * 3f * 0.833f + 0.25f);
        vfx.SetFloat("_Waviness", distance * 5f);
    }
}

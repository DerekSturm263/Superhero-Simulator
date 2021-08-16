using UnityEngine;

public class HandController : MonoBehaviour
{
    public static readonly HandInfo[] hands = new HandInfo[2];

    [SerializeField] private HandReference[] _handRefs = new HandReference[2];
    private static readonly Material[] _handMats = new Material[2];

    private readonly GameObject[] _motionTargets = new GameObject[2];

    private void Awake()
    {
        HandFormations.Initialize(_handRefs[0], _handRefs[1]);

        // Initialize hands.
        for (int i = 0; i < 2; ++i)
        {
            hands[i] = HandInfo.FromReference(_handRefs[i]);
            _handMats[i] = _handRefs[i].GetComponentInChildren<SkinnedMeshRenderer>().material;

            // Make better later.
            hands[i].AddMotion(HandFormations.slingRingMotion1);
            hands[i].AddMotion(HandFormations.slingRingMotion2);
        }

        for (int i = 0; i < 1; ++i)
        {
            _motionTargets[i] = new GameObject("Target");
            _motionTargets[i].transform.SetParent(transform.GetChild(1));
            TrailRenderer tr = _motionTargets[i].AddComponent<TrailRenderer>();
            tr.time = 2.5f;
            tr.widthMultiplier = 0.1f;
        }

        //HandFormations.saveHand.onTriggered += FileSend;
        //HandFormations.shootWeb.onTriggered += ShootWeb;

        HandFormations.openPortal1.onStarted += (l, r) => PortalController.StartPortal(r, HandFormations.slingRingMotion1, PortalController.portalsInfo[0]);
        HandFormations.openPortal1.onUpdateTrigger += (l, r) => PortalController.FormPortal(r, HandFormations.slingRingMotion1, PortalController.portalsInfo[0]);
        HandFormations.openPortal1.onCancelled += (l, r) => PortalController.CancelPortal(r, HandFormations.slingRingMotion1, PortalController.portalsInfo[0]);

        //HandFormations.openPortal2.onStarted += (l, r) => PortalController.StartPortal(r, HandFormations.slingRingMotion2, PortalController.portal2);
        //HandFormations.openPortal2.onUpdateTrigger += (l, r) => PortalController.FormPortal(r, HandFormations.slingRingMotion2, PortalController.portal2);
        //HandFormations.openPortal2.onCancelled += (l, r) => PortalController.CancelPortal(r, HandFormations.slingRingMotion2, PortalController.portal2);
    }

    private void Update()
    {
        UpdateFingerValues();

        for (int i = 0; i < 2; ++i)
        {
            _handMats[i].color = Color.Lerp(Color.red, Color.green, hands[i].Closeness(HandFormations.saveHandGesture));
        }
    }

    #region Hand Methods

    private void FileSend(HandInfo hand)
    {
        Debug.Log(hand.handSide + " hand sent information about the other hand to a file.");

        switch (hand.handSide)
        {
            case HandInfo.HandSide.Left:
                _handRefs[1].SendToFile(Application.persistentDataPath + "\\" + System.DateTime.Now + ".txt");
                break;
            case HandInfo.HandSide.Right:
                _handRefs[0].SendToFile(Application.persistentDataPath + "\\" + System.DateTime.Now + ".txt");
                break;
        }
    }

    private void ShootWeb(HandInfo hand)
    {
        Debug.Log(hand.handSide + " hand shot a web!");
    }

    #endregion

    private void UpdateFingerValues()
    {
        for (int i = 0; i < 2; ++i)
        {
            hands[i].UpdateToReference(_handRefs[i]);
        }

        HandEvent.events.ForEach(x => x.TryEvent(hands[0], hands[1]));
    }
}

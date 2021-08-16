public static class HandFormations
{
    private static readonly string _filePath = UnityEngine.Application.persistentDataPath;

    public static HandReference DefaultHandLeft { get; private set; }
    public static HandReference DefaultHandRight { get; private set; }

    #region Gestures

    public static HandInfo saveHandGesture = HandInfo.FromFile(_filePath + @"\saveHandGesture.txt");
    public static HandInfo webShooterGesture = HandInfo.FromFile(_filePath + @"\webShooterGesture.txt");
    public static HandInfo slingRingGesture = HandInfo.FromFile(_filePath + @"\slingRingGesture.txt");
    public static HandInfo repulsorGesture = HandInfo.FromFile(_filePath + @"\repulsorGesture.txt");
    public static HandInfo clawsGesture = HandInfo.FromFile(_filePath + @"\clawsGesture.txt");

    #endregion

    #region Motions

    public static MotionInfo slingRingMotion1 = new MotionInfo(new System.Func<float, float>((theta) => 1f), new UnityEngine.Vector3(0.2f, 1f, 0.5f), 1.5f, MotionInfo.Direction.Clockwise);
    public static MotionInfo slingRingMotion2 = new MotionInfo(new System.Func<float, float>((theta) => 1f), new UnityEngine.Vector3(0.2f, 1f, 0.5f), 1.5f, MotionInfo.Direction.Counter_Clockwise);

    #endregion

    #region Events

    public static readonly HandEvent saveHand = new HandEvent(hand => hand.MatchesGesture(saveHandGesture));
    public static readonly HandEvent shootWeb = new HandEvent(hand => hand.MatchesGesture(webShooterGesture));
    public static readonly HandEvent openPortal1 = new HandEvent(handL => handL.MatchesGesture(saveHandGesture), handR => handR.MatchesGesture(saveHandGesture) && handR.MatchingMotion(slingRingMotion1, 0.2f));
    //public static readonly HandEvent openPortal2 = new HandEvent(handL => handL.MatchesGesture(saveHandGesture), handR => handR.MatchesGesture(saveHandGesture) && handR.MatchingMotion(slingRingMotion2, 0.2f));
    public static readonly HandEvent shootRepulsor = new HandEvent(hand => hand.MatchesGesture(repulsorGesture));
    public static readonly HandEvent extendClaws = new HandEvent(hand => hand.MatchesGesture(clawsGesture));

    #endregion

    public static void Initialize(HandReference leftHand, HandReference rightHand)
    {
        DefaultHandLeft = leftHand;
        DefaultHandRight = rightHand;
    }
}

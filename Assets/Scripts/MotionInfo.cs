using UnityEngine;

public class MotionInfo
{
    public enum Direction
    {
        Clockwise = 1, Counter_Clockwise = -1
    }

    private readonly System.Func<float, float> _polarMotionAlgorithm;
    public readonly Vector3 offset;
    public readonly float startIndex;
    public readonly Direction direction;

    public MotionInfo(System.Func<float, float> polarMotionAlgorithm, Vector3 offset, float startIndex, Direction direction)
    {
        this._polarMotionAlgorithm = polarMotionAlgorithm;
        this.offset = offset;
        this.startIndex = startIndex;
        this.direction = direction;
    }

    public Vector3 ToVector(float theta, float scalar, float startIndex)
    {
        return Polar2Rect(FromTheta(_polarMotionAlgorithm, theta) * scalar, startIndex + theta) + offset;
    }

    public static float FromTheta(System.Func<float, float> algorithm, float theta)
    {
        return algorithm.Invoke(theta);
    }

    public static Vector3 Polar2Rect(float r, float theta)
    {
        float x = r * Mathf.Cos(theta);
        float y = r * Mathf.Sin(theta);

        return new Vector3(x, y);
    }

    public static (float r, float theta) Rect2Polar(Vector3 vector)
    {
        float r = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
        float theta = Mathf.Atan2(vector.y, vector.x);

        return (r, theta);
    }

    public static (float r, float theta) Rect2PolarInverse(Vector3 vector, float scale)
    {
        float r = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
        float theta = Mathf.Atan2(vector.x * scale, vector.y);

        return (r, theta);
    }
}

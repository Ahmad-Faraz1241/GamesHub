using UnityEngine;

public class ScreenShift : MonoBehaviour
{
    public enum Orientation { Portrait, Landscape }
    public Orientation desiredOrientation;

    void Start()
    {
        if (desiredOrientation == Orientation.Portrait)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else if (desiredOrientation == Orientation.Landscape)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft; // or LandscapeRight
        }
    }
}

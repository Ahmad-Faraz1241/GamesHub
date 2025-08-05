using UnityEngine;

public class CarInputManager : MonoBehaviour
{
    public BasicCarController carController;

    public void OnGasPressed() => carController.mobileGasInput = 1f;
    public void OnGasReleased() => carController.mobileGasInput = 0f;

    public void OnBrakePressed() => carController.mobileBrakeInput = 1f;
    public void OnBrakeReleased() => carController.mobileBrakeInput = 0f;

    public void OnReversePressed() => carController.mobileReverseInput = 1f;
    public void OnReverseReleased() => carController.mobileReverseInput = 0f;

    public void OnLeftPressed() => carController.mobileSteeringInput = -1f;
    public void OnRightPressed() => carController.mobileSteeringInput = 1f;
    public void OnSteeringReleased() => carController.mobileSteeringInput = 0f;
}

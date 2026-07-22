using UnityEngine;

public abstract class CarMovementBluePrint : MonoBehaviour
{
    //Define the movement here and call it in the input script when a key is pressed
    //Override it in the car movement class

    //The role of this class is to unify the movement across all the cars (NPC, Players...)

    public abstract void Accelerate(float Throttle);

    public abstract void Brake(float value);

    public abstract void HandBrake(float input);

    public abstract void Turn(float direction);

    public abstract void ToggleLights();

    public abstract void SwitchGears(int newGear);

    public abstract void Clutch(float clutchEngagement);

    public abstract void Startup();

    public abstract void Shutdown();

    //Don't delete the methods above

    //Add new methods below
}

public enum Gears
{
    R,
    D,
    P,
    N,
}

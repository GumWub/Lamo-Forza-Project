public static class TalosTime
{
    //this Class is a plug to the Game Engine, and must always be updated
    public static float FixedDeltaTime 
    { 
        get; 
        private set; 
    } = 0;
    public static void SetFixedDeltaTime(float fixedDeltaTime)//Plug this class to the game engine fixed update
    {
        FixedDeltaTime = fixedDeltaTime;
    }
}
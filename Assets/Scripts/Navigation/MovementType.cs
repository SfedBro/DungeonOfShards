namespace Game.Navigation
{
    [System.Flags]
    public enum MovementType
    {
        None = 0,
        Ground = 1 << 0,
        Flying = 1 << 1,
        Swimming = 1 << 2,
        Breakable = 1 << 3,
    }
}

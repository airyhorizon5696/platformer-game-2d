namespace ColliderEventSystem
{
    /// <summary>
    /// What happens to this GameObject after the Actions have run.
    /// </summary>
    public enum AfterTrigger
    {
        SetInactive,
        Destroy,
        DestroyParent,
        DoNothing,
        ExecuteExitActions,
    }
}

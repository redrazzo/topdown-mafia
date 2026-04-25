namespace MafiaTopDown.Gameplay.Domain.Scenes
{
    public sealed record SceneTransitionRequest(
        string SourceScene,
        string TargetScene,
        string SpawnPointId,
        SceneFadeMode FadeMode,
        string CarryContextId = null);

    public enum SceneFadeMode
    {
        None = 0,
        FadeToBlack = 1
    }
}

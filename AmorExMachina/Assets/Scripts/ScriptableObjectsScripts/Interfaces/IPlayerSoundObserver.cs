public enum SoundType
{
    WALKING,
    DISTRACTION
}

public interface IPlayerSoundObserver
{
    void Notify(SoundType soundType, UnityEngine.Vector3 position);
}
public enum SoundType
{
    CROUCHING,
    WALKING,
    DISTRACTION
}

public interface IPlayerSoundObserver
{
    void PlayerSoundNotify(SoundType soundType, UnityEngine.Vector3 position);
}
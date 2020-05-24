public enum SoundType
{
    CROUCHING,
    WALKING,
    DISTRACTION,
    ALARM
}

public interface IPlayerSoundObserver
{
    void PlayerSoundNotify(SoundType soundType, UnityEngine.Vector3 position);
}
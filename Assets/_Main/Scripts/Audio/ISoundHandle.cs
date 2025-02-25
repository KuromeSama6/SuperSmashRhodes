namespace SuperSmashRhodes.Scripts.Audio {
public interface ISoundHandle {
    int id { get; }
    bool valid { get; }
    void Release();
    void Release(float fadeOut, float delay = 0);
}
}

using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Match {
[CreateAssetMenu(fileName = "NewRoomConfiguration", menuName = "SSR/Room/RoomConfiguration", order = 0)]
public class RoomConfiguration : ScriptableObject {
    [Title("General Config")]
    public bool isNetworked;
    public bool isTraining;
    
    [Title("Round Config")]
    public int winRounds;
    public int roundTime;
    public bool infiniteTime;
    
    [Title("Character Select")]
    public bool singleSided;
    public bool infiniteCharacterSelect;
    public int characterSelectTime;

    [Title("Debug Options")]
    public bool cacheGameState;
}
}

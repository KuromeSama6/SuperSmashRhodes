using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
[CreateAssetMenu(fileName = "Character Descriptor", menuName = "SSR/Battle/Character Descriptor")]
public class CharacterDescriptor : ScriptableObject {
    [Title("Name and Lore")]
    public string id;
    public string chineseName, englishName;
    public CharacterProfession profession;
    
    [Title("Sprites")]
    public Sprite portrait;
    public Sprite avatar;
    public Sprite professionIconFull, professionIconBlack, professionIconWhite, professionIconUI;
}

public enum CharacterProfession {
    CASTER,
    MEDIC,
    PIONEER,
    SNIPER,
    WARRIOR,
    TANK,
    SUPPORT,
    SPECIAL
}

}

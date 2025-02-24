using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
[CreateAssetMenu(fileName = "Character Descriptor", menuName = "SSR/Battle/Character Descriptor")]
public class CharacterDescriptor : ScriptableObject {
    [Title("Name and Lore")]
    public GameObject gameObject;
    public string id;
    public string chineseName, englishName;
    public CharacterProfession profession;
    public string organizationChinese, organizationEnglish;
    public string realName;
    
    [Title("Sprites")]
    public Sprite portrait;
    public Sprite superPortrait;
    public Sprite avatar;
    public Sprite professionIconFull, professionIconBlack, professionIconWhite, professionIconUI;
    public Sprite emblem;
    
    [Title("Spine")]
    public SkeletonDataAsset skeletonData;
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

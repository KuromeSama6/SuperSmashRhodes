﻿using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes.Config.Global;
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

    [Title("Ratings and Descriptions")]
    public Sprite easyOfUseRatingIcon;
    public string mainDescription;
    public string subDescription1;
    public string subDescription2;
    
    [Title("Sprites")]
    public Sprite portrait;
    public Sprite superPortrait;
    public Sprite avatar;
    public Sprite professionIconFull, professionIconBlack, professionIconWhite, professionIconUI;
    public Sprite emblem;
    
    [Title("Spine")]
    public SkeletonDataAsset skeletonData;

    [Title("Misc")]
    public float entryCinematicDuration = 7f;
    public float roundWonCinematicDuration = 10f;

    public int characterIndex => CharacterDatabase.inst.characters.IndexOf(this);
    public static CharacterDescriptor FromIndex(int index) => CharacterDatabase.inst.characters[index];
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

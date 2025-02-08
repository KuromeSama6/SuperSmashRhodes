using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Framework;

namespace SuperSmashRhodes.Config.Global {
public class CharacterDatabase : GlobalDataObject<CharacterDatabase> {
    public List<CharacterDescriptor> characters;
}
}

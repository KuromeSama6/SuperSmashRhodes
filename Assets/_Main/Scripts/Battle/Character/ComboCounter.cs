using SuperSmashRhodes.Battle.State.Implementation;

namespace SuperSmashRhodes.Battle {
public class ComboCounter : RuntimeCharacterDataRegister {
    public int count { get; private set; }
    private float overallProration = 1f;
    private float appliedProration = 1f;

    public float finalScale {
        get {
            if (count <= 1) return 1f;
            return overallProration * appliedProration;
        }
    }
    
    public ComboCounter(PlayerCharacter owner) : base(owner){

    }

    public void RegisterAttack(IAttack move, Entity victim) {
        ++count;
        
        if (count == 1) {
            overallProration = move.GetFirstHitProration(victim);
        }

        appliedProration *= move.GetComboProration(victim);
    }

    public void Reset() {
        count = 0;
        overallProration = 1f;
        appliedProration = 1f;
    }
    
}
}

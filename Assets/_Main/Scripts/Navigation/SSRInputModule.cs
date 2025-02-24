using UnityEngine;
using UnityEngine.EventSystems;

namespace SuperSmashRhodes.Navigation {
public class SSRInputModule : BaseInputModule {
    public GameObject selected { get; private set; }

    private EventSystem eventSystem => EventSystem.current;
    
    protected override void Awake() {
        base.Awake();
        selected = eventSystem.firstSelectedGameObject;
    }

    protected override void Start() {
        base.Start();
    }

    public override void Process() {
        if (!selected) selected = eventSystem.firstSelectedGameObject;
    }
}
}

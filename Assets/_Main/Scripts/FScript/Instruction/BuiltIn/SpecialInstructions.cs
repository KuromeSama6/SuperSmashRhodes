using SuperSmashRhodes.FScript.Components;

namespace SuperSmashRhodes.FScript.Instruction {
[FInstruction("section")]
public class SectionInstruction : FInstruction {
    public string sectionName { get; set; }
    public SectionType type { get; private set; }

    public SectionInstruction(FLine line, int address) : base(line, address) {
        RequireMinArgs(1);
        sectionName = args[0].value.TrimEnd(':');
        if (!sectionName.StartsWith("."))
            throw new FScriptException($"Section name must start with comma(.), got {sectionName}");
        sectionName = sectionName.TrimStart('.');

        if (args.Length >= 2) type = args[1].EnumValue<SectionType>();
        else type = SectionType.GENERIC;
    }

    public override void Execute(FScriptRuntime ctx) { //NOP
    }
}

public enum SectionType {
    GENERIC,
    MOVE,
    STATE,
    EVENT
}
}

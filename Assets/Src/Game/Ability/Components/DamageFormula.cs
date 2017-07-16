using Intelligence;
using EntitySystem;
using System.Collections.Generic;
using UnityEngine;

public class DamageFormula : AbilityComponent<Context> {
    public Context contextType;
    public List<Modifier> modifiers;
    public float inputValue;
    public float outputValue;

    public override void OnUse() {
        float sum = inputValue;
        for(int i = 0; i < modifiers.Count; i++) {
            modifiers[i].ApplyModifier(ref sum);
        }
        outputValue = sum;
    }
}

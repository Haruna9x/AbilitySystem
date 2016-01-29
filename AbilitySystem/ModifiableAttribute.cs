﻿using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class FormulaSignature : Attribute {

    public Type retnType;
    public Type[] parameters;

    public FormulaSignature(Type retnType, params Type[] parameters) {
        this.retnType = retnType;
        this.parameters = parameters;
    }
}

//todo think about how to reset base value when clearing modifiers
//its possible the formula we used to compute the base value is
//no longer accurate after removing modifiers...or just rely on
//the user to invoke Update() properly.

public abstract class AbstractModifiableAttribute {

    [SerializeField]
    protected float baseValue;
    protected float value;
    [SerializeField]
    protected List<AttributeModifier> modifiers;

    protected int assemblyGUID;
    protected string typeName;
    protected string methodName;

    public AbstractModifiableAttribute() {
        value = 0f;
        baseValue = 0f;
        modifiers = new List<AttributeModifier>();
    }

    public AbstractModifiableAttribute(float value) {
        this.value = value;
        baseValue = value;
        modifiers = new List<AttributeModifier>();
    }

    public float BaseValue {
        get { return baseValue; }
    }

    public float Value {
        get { return value; }
    }

    public virtual void AddModifier(AttributeModifier modifier) {
        if (modifier == null) return;
        modifiers.Add(modifier);
    }

    public bool HasModifier(AttributeModifier modifier) {
        return modifiers.Contains(modifier);
    }

    public virtual bool RemoveModifier(AttributeModifier modifier) {
        return modifiers.Remove(modifier);
    }

    public void ClearModifiers() {
        value = baseValue;
        modifiers.Clear();    
    }

}

public class ModifiableAttribute<T> : AbstractModifiableAttribute {

    protected Func<T, float> GetBaseValue;

    public ModifiableAttribute(float value) : base(value) {
        GetBaseValue = (T t) => { return value; };
    }

    public ModifiableAttribute(Func<T, float> fn) : base() {
        GetBaseValue = fn ?? ((T t) => { return 0f; });
    }

    public virtual float Update(T t) {
        //if(GetBaseValue == null) {
        //   // GetBaseValue = Reflector.FindMethod(typeName, methodName);
        //    if(GetBaseValue == null) {
        //        GetBaseValue = (T t1) => { return baseValue; }
        //    }
        //}
        value = baseValue = GetBaseValue(t);
        for (int i = 0; i < modifiers.Count; i++) {
            AttributeModifier modifier = modifiers[i];
            value += modifier.ModifyValue(value, baseValue);
            value = modifier.ClampValue(value, baseValue);
        }
        return value;
    }
}

[FormulaSignature(typeof(float), typeof(AbstractAbility))]
public class AbilityAttribute : ModifiableAttribute<AbstractAbility> {

    public AbilityAttribute(Func<AbstractAbility, float> fn) : base(fn) {  }
    public AbilityAttribute(float value) : base(value) {}

}

[FormulaSignature(typeof(float), typeof(Entity))]
public class EntityAttribute : ModifiableAttribute<Entity> {

    public EntityAttribute(Func<Entity, float> fn) : base(fn) { }
    public EntityAttribute(float value) : base(value) { }

}

[Serializable]
public class ModifiableAttribute {

    public string id;
    protected Func<float> GetBaseValue;
    protected float value;
    protected List<AttributeModifier> modifiers;
    MethodPointer<Entity, float, float> ptr;
    [SerializeField] protected MethodPointer formulaMetadata;
    [SerializeField] protected float baseValue;

    public ModifiableAttribute(string id, Func<float> baseValueFn = null) {
        this.id = id;
        GetBaseValue = baseValueFn ?? (() => { return 0f; });
        baseValue = value = GetBaseValue();
        //Reflector.Register(baseValueFn);
    }

    public ModifiableAttribute(string id, float value = 0) {
        this.id = id;
        this.value = value;
        GetBaseValue = () => { return value; };
    }

    public virtual float Update() {
        if (GetBaseValue == null) return -1f;
        value = baseValue = GetBaseValue();
        //value = ptr.Invoke(Entity, baseValue);
        for (int i = 0; i < modifiers.Count; i++) {
            AttributeModifier modifier = modifiers[i];
            value += modifier.ModifyValue(value, baseValue);
            value = modifier.ClampValue(value, baseValue);
        }
        return value;
    }

    public void OnAfterDeserialized() {
        ptr.OnAfterDeserialize();
        ///if !ptr.fn ptr.fn = blah
    }

    public float Value {
        get { return value; }
    }

    public ModifiableAttribute Clone() {
        return new ModifiableAttribute(id, GetBaseValue);
    }

}
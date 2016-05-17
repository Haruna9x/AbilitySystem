﻿using UnityEngine;
using UnityEditor;
using System.IO;

public static class StatusEffectMenuItem {

    [MenuItem("Assets/Status Effect")]
    public static StatusEffectCreator CreateScriptableObject() {
        StatusEffectCreator asset = ScriptableObject.CreateInstance<StatusEffectCreator>();
        string assetpath = "Assets/Status Effects/Status Effect.asset";
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(assetpath);
        var effect = new StatusEffect();
        effect.Id = Path.GetFileNameWithoutExtension(assetPathAndName);
        AssetSerializer serializer = new AssetSerializer();
        serializer.AddItem(effect);
        asset.source = serializer.WriteToString();
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return asset;
    }

}
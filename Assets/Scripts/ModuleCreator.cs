using UnityEditor;
using UnityEngine;


public class ModuleCreator {

    private const string modulePrefabPath = "Assets/Prefabs/Modules";

    private void CreateModulePrefab() {
        var moduleGameobject = new GameObject("Module");

        var spriteRenderer = moduleGameobject.AddComponent<SpriteRenderer>();
        moduleGameobject.AddComponent<Rigidbody2D>();
        moduleGameobject.AddComponent<BoxCollider2D>();
        moduleGameobject.AddComponent<Module>();

        PrefabUtility.SaveAsPrefabAsset(moduleGameobject, modulePrefabPath);
    }
}

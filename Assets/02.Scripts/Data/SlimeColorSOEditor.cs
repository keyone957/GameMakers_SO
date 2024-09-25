using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShopTokenSO))]
public class SlimeColorSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ShopTokenSO shopTokenSo = (ShopTokenSO)target;
        shopTokenSo.mode = EditorGUILayout.TextField("Mode", shopTokenSo.mode);
        
        shopTokenSo.price = EditorGUILayout.IntField("Price", shopTokenSo.price);
        
        if (shopTokenSo.mode.ToLower() == "color")
        {
            shopTokenSo.slimeColor = EditorGUILayout.ColorField("Slime Color", shopTokenSo.slimeColor);
        }
        else if (shopTokenSo.mode.ToLower() == "sword")
        {
            shopTokenSo.swordImage = (Sprite)EditorGUILayout.ObjectField("Sword Image", shopTokenSo.swordImage, typeof(Sprite), allowSceneObjects: false);
            shopTokenSo.damage = EditorGUILayout.IntField("Damage", shopTokenSo.damage);
        }
        else
        {
            EditorGUILayout.HelpBox("올바른 모드를 입력하세요 color 또는 sword", MessageType.Error);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(shopTokenSo);
        }
    }
}
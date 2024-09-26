using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SlimeColorTokenData", menuName = "SO/ShopTokenData")]
public class ShopTokenSO : ScriptableObject
{
    [Header("Mode 설정 (color 또는 sword 입력)")]
    public string mode;

    [Header("공통 정보")]
    public int price;

    [Header("Color 모드 정보")]
    public Color slimeColor;

    [Header("Sword 모드 정보")]
    public Sprite swordImage;
    public int damage;
}
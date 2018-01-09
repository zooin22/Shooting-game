using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class EnemyEditor : EditorWindow
{

    GameObject obj;
    new string name;
    new Vector2 position;
    Sprite sprite; // 적 모습
    Sprite[] bulletSprite; // 적 총알 모습
    int numOfBulletType = 0;
    int hp = 1;
    float speed;
    int size = 1;
    EnemyState.EEnemyMoveMode eEnemyMoveMode;

    [MenuItem("Custom/Enemy")]
    static public void ShowWindow()
    {
        // 윈도우 생성
        EnemyEditor window = (EnemyEditor)EditorWindow.GetWindow(typeof(EnemyEditor));
    }
    private void Awake()
    {
    }
    void OnGUI()
    {
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
        obj = (GameObject)EditorGUILayout.ObjectField("Weapon", obj, typeof(GameObject));
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
        name = EditorGUILayout.TextField("Name", name);
        position = EditorGUILayout.Vector2Field("Position", position);
        sprite = (Sprite)EditorGUILayout.ObjectField("EnemySprite", sprite, typeof(Sprite), allowSceneObjects: true);
        hp = EditorGUILayout.IntField("Hp", hp);
        size = EditorGUILayout.IntField("Size", size);
        speed = EditorGUILayout.FloatField("Speed", speed);
        eEnemyMoveMode = (EnemyState.EEnemyMoveMode)EditorGUILayout.EnumPopup("EnemyMoveMode", eEnemyMoveMode);
        EditorGUI.BeginChangeCheck();
        numOfBulletType = EditorGUILayout.IntField("NumOfBulletType", numOfBulletType);
        if (EditorGUI.EndChangeCheck())
        {
            bulletSprite = new Sprite[numOfBulletType];
        }
        for (int i = 0; i < numOfBulletType; i++)
        {
            bulletSprite[i] = (Sprite)EditorGUILayout.ObjectField("BulletSprite", bulletSprite[i], typeof(Sprite), allowSceneObjects: true);
        }


        if (GUILayout.Button("Create Object"))
        {
            if (obj == null)
                return;
            GameObject enemyObj = Instantiate(Resources.Load("Prefabs/Enemy") as GameObject, position, Quaternion.identity, null);
            enemyObj.name = name;
            enemyObj.transform.localScale = new Vector2(size, size);
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.Init(sprite, obj.GetComponent<WeaponItemWrapper>().PickItem() as Weapon, GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(), hp, speed, eEnemyMoveMode);
        }
        if (GUILayout.Button("Call Pool"))
        {
            if (obj == null)
                return;
            EnemyManager.instance.SpawnEnemy(sprite, obj.GetComponent<WeaponItemWrapper>().PickItem() as Weapon, position, size, hp, speed, eEnemyMoveMode);
        }
    }

}

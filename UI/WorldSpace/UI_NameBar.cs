using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 이름 띄우는 바 (아이템)
public class UI_NameBar : UI_Base
{
    enum Gameobjects
    {
        Background,
    }

    enum Texts
    {
        NameText,
    }

    public string nameText;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        GetText((int)Texts.NameText).text = nameText;

        return true;
    }

    void FixedUpdate()
    {
        Transform parent = transform.parent;
        float valueY = (parent.GetComponent<Collider>().bounds.size.y * 1.3f);

        transform.position = parent.position + Vector3.up * valueY;
        GetObject((int)Gameobjects.Background).transform.rotation = Camera.main.transform.rotation;
    }
}

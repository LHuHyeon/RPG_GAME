using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
[ 무기 Slot 스크립트 ]
1. EqStatPopup안에 있는 무기 Slot으로 장비를 장착/해제할 수 있다.
2. 드래그 드랍 or 우클릭으로 장비 장착이 가능하다.
*/

public class UI_WeaponItem : UI_ItemDragSlot
{
    public Define.WeaponType weaponType = Define.WeaponType.Unknown;
    public WeaponItemData weaponItem;

    public override void SetInfo()
    {
        Managers.Game._playScene._equipment.weaponSlot = this;

        // 해당 부위 장비가 장착되어 있다면
        if (Managers.Game.CurrentWeapon.id != 0)
            AddItem(Managers.Game.CurrentWeapon);
        else
            Managers.Game.CurrentWeapon = null;

        base.SetInfo();
    }

    protected override void OnClickSlot(PointerEventData eventData)
    {
        if (item.IsNull() == true || UI_DragSlot.instance.dragSlotItem.IsNull() == false)
            return;

        // 장비 벗기
        if (Input.GetMouseButtonUp(1))
        {
            if (Managers.Game._playScene._inventory.AcquireItem(weaponItem) == true)
                ClearSlot();
        }
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        // 아이템을 버린 위치가 UI가 아니라면
        if (item.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            // 아이템 인벤으로 이동
            if (Managers.Game._playScene._inventory.AcquireItem(weaponItem) == true)
                ClearSlot();
        }
        
        base.OnEndDragSlot(eventData);
    }

    protected override void OnDropSlot(PointerEventData eventData)
    {
        UI_Slot dragSlot = UI_DragSlot.instance.dragSlotItem;

        if (dragSlot.IsNull() == false)
        {
            // 자기 자신이라면
            if (dragSlot == this)
                return;

            // 장비 장착 (or 교체)
            ChangeSlot(dragSlot as UI_ItemSlot);
        }
    }

    // 무기 장착
    public void ChangeWeapon(UI_ItemSlot itemSlot) { ChangeSlot(itemSlot); }

    protected override void ChangeSlot(UI_ItemSlot itemSlot)
    {
        // 장비 확인
        if ((itemSlot.item is WeaponItemData) == false)
            return;

        // 같은 부위 확인
        WeaponItemData weapon = itemSlot.item as WeaponItemData;
        if (weaponType != weapon.weaponType)
            return;

        // 레벨 체크
        if (Managers.Game.Level < weapon.minLevel)
        {
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("레벨이 부족합니다.", new Color(1f, 0.5f, 0f));
            return;
        }

        ItemData _tempItem = item;

        // 장비 장착
        AddItem(itemSlot.item);

        // 기존 장비 인벤 이동
        UI_InvenItem inven = itemSlot as UI_InvenItem;
        if (_tempItem.IsNull() == false)
            inven.AddItem(_tempItem);
        else
            inven.ClearSlot();
    }

    public override void AddItem(ItemData _item, int count = 1)
    {
        base.AddItem(_item, count);

        weaponItem = _item as WeaponItemData;
        
        // 장착 중인 무기가 있다면 비활성화
        if (Managers.Game.CurrentWeapon.IsNull() == false)
        {
            // 장비 파츠 확인
            GetPart(Managers.Game.CurrentWeapon);

            Managers.Game.CurrentWeapon.charEquipment.SetActive(false);
        }
        
        // 장비 파츠 확인
        GetPart(weaponItem);
        Managers.Game.CurrentWeapon = weaponItem;

        UpgradeMeshEffect(weaponItem);

        weaponItem.charEquipment.SetActive(true);
    }

    // 업그레이드 일정 수치 넘으면 Mesh 적용
    public void UpgradeMeshEffect(EquipmentData equipment)
    {
        // 해당 무기 오브젝트 찾기
        GameObject weaponObj = (equipment as WeaponItemData).charEquipment;
        if (weaponObj.IsFakeNull() == true)
            weaponObj = (Managers.Data.Item[equipment.id] as WeaponItemData).charEquipment;

        // 객체 안에 자식들 삭제
        foreach(Transform child in weaponObj.transform)
                Managers.Resource.Destroy(child.gameObject);

        // 레벨 충족이 안되면 종료 
        if (equipment.upgradeCount < 6)
            return;

        // 무기 객체 자식으로 이펙트 배치
        string path = "Effect/Upgrade/UpgradeEffect_" + equipment.upgradeCount;
        GameObject effectObj = Managers.Resource.Instantiate(path, weaponObj.transform);

        PSMeshRendererUpdater meshRenderer = effectObj.GetComponent<PSMeshRendererUpdater>();
        meshRenderer.UpdateMeshEffect(weaponObj);
    }

    void GetPart(WeaponItemData weapon)
    {
        if (weapon.charEquipment.IsNull() == true)
            weapon.charEquipment = (Managers.Data.Item[weapon.id] as WeaponItemData).charEquipment;
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        Managers.Game.CurrentWeapon.charEquipment.SetActive(false);
        Managers.Game.CurrentWeapon = null;
        weaponItem = null;
    }
}

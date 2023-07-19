using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
[ 시작 Scene 스크립트 ]
1. TitleScene이 로드되면 TitleScene.cs에 의해서 생성되어 사용된다.
2. 게임 시작, 이어 하기, 종료 하기 버튼을 가지고 있다.
*/

public class UI_TitleScene : UI_Scene
{
    enum Buttons
    {
        StartButton,
        LoadButton,
        ExitButton,
    }

    enum Texts
    {
        LoadButtonText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.StartButton).onClick.AddListener(OnClickStartButton);
        GetButton((int)Buttons.LoadButton).onClick.AddListener(OnClickLoadButton);
        GetButton((int)Buttons.ExitButton).onClick.AddListener(OnClickExitButton);

        if (Managers.Game.IsSaveLoad() == false)
        {
            Color _color = GetText((int)Texts.LoadButtonText).color;
            _color.a = 0.5f;
            GetText((int)Texts.LoadButtonText).color = _color;

            string path = "Art/UI/Classic_RPG_GUI/Parts/mid_button_off";
            GetButton((int)Buttons.LoadButton).GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>(path);
        }

        return true;
    }
    
    void OnClickStartButton()
    {
        Managers.Scene.LoadScene(Define.Scene.PlayerCustom);
    }

    void OnClickLoadButton()
    {
        if (Managers.Game.LoadGame() == false)
            return;
            
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            // 인터넷 연결이 안되었을 때 행동
            Managers.UI.ShowPopupUI<UI_GuidePopup>().SetInfo("네트워크 연결이 필요합니다.", Color.red);
        }
        else if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            // 데이터로 연결이 되었을 때 행동
            Managers.UI.ShowPopupUI<UI_LoadPopup>().SetInfo(Define.Scene.Game, 6);
        }
        else
        {
            // 와이파이로 연결이 되었을 때 행동
            Managers.UI.ShowPopupUI<UI_LoadPopup>().SetInfo(Define.Scene.Game, 7);
        }
    }

    void OnClickExitButton()
    {
        Application.Quit();
    }
}

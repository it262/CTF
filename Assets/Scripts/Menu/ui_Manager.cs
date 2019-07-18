using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Collections.Specialized;

public class ui_Manager : MonoBehaviour
{
    GameManager gm;
    public GameObject Menu01,Menu02,Menu03,Name,Load;
    public List<GameObject> Players;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        LoadComp();
        Menu();
        PlayerClear();
        //Load.GetComponent<Animator>().SetBool("On", false);
        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.RoomSettingComp)
            .Subscribe(_ => AllClean());

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.RoomSettingComp)
            .Delay(System.TimeSpan.FromSeconds(3.0f))
            .Subscribe(_ => LoadComp());

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.DrawEventStart)
            .Subscribe(_ => EndGame());
    }

    private void Update()
    {
        if(!Menu01.GetComponent<Animator>().GetBool("On")  && Input.GetKeyDown(KeyCode.Escape) && gm._GameState.Value!=GameState.RoomSettingComp){
            gm._GameState.Value = GameState.Menu;
            var data = new Dictionary<string, string>();
            data["to"] = "LEAVE";
            SocketObject.Instance.EmitMessage("RoomMatching", data);
            Menu();
        }
    }

    public void CreateRoom()
    {
        PlayerClear();
        Menu01.GetComponent<Animator>().SetBool("On", false);
        Menu02.GetComponent<Animator>().SetBool("On", true);
        Menu03.GetComponent<Animator>().SetBool("On", false);
    }

    public void RoomList()
    {
        PlayerClear();
        Menu01.GetComponent<Animator>().SetBool("On", false);
        Menu02.GetComponent<Animator>().SetBool("On", false);
        Menu03.GetComponent<Animator>().SetBool("On", true);
        gm._GameState.Value = GameState.RoomSerching;
    }

    public void Menu()
    {
        PlayerClear();
        Menu01.GetComponent<Animator>().SetBool("On", true);
        Menu02.GetComponent<Animator>().SetBool("On", false);
        Menu03.GetComponent<Animator>().SetBool("On", false);
    }

    public void HideMenu3()
    {
        Menu01.GetComponent<Animator>().SetBool("On", false);
        Menu02.GetComponent<Animator>().SetBool("On", false);
        Menu03.GetComponent<Animator>().SetBool("On", false);
    }

    public void PlayerAdd(OrderedDictionary member)
    {
        HideMenu3();
        int cnt = 0;
        foreach(DictionaryEntry m in member){
            Players[cnt].GetComponent<Animator>().SetBool("On", true);
            Players[cnt].GetComponent<players_LeadyUI>().setPlayerName(m.Value.ToString());
            cnt++;
        }
    }

    public void PlayerClear()
    {
        foreach(GameObject g in Players)
        {
            if(g.GetComponent<Animator>().GetBool("On"))
                g.GetComponent<Animator>().SetBool("On", false);
        }
    }

    void LoadScene()
    {
        Load.GetComponent<Animator>().SetBool("On", true);
    }

    void AllClean()
    {
        LoadScene();
        HideMenu3();
        PlayerClear();
    }

    void LoadComp()
    {
        Load.GetComponent<Animator>().SetBool("On", false);
    }

    void EndGame()
    {
        gm._GameState.Value = GameState.Menu;
        var data = new Dictionary<string, string>();
        data["to"] = "LEAVE";
        SocketObject.Instance.EmitMessage("RoomMatching", data);
        Menu();
        gm._GameState.Value = GameState.Menu;
    }
}

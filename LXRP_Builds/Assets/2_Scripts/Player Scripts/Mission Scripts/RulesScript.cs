using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesScript : MonoBehaviour, IClickable
{
    private SO_RuleInfo ruleInfo;
    public SO_RuleInfo RuleInfo { get => ruleInfo; set => ruleInfo = value; }
    public static string bookTag = "";
    public static int but =0;
    public static List<string> array = new List<string>();

    public void OnClick()
    {
        ruleInfo.IsSelected = false;
        Debug.Log("Rule Game Object Clicked");
        UIManager.Instance.SetRuleInfo(RuleInfo);
        bookTag = gameObject.tag;
        //array.Add(gameObject.tag);
        Debug.Log("Rule tag:" + bookTag);
        Debug.Log("INDEX" +MainManager.index);
        Debug.Log("LAST TAG" + MainManager.lasttag);
        if (GameObject.FindGameObjectWithTag("playerobjectspawn").gameObject.transform.GetChild(MainManager.index).gameObject.tag == MainManager.lasttag && but > 0)
        {
            Destroy(GameObject.FindGameObjectWithTag("playerobjectspawn").gameObject.transform.GetChild(MainManager.index).gameObject);
            GameObject.FindGameObjectWithTag("Button"+RulesScript.but).GetComponent<Image>().sprite = MainManager.Instance.CurrSelectedPlayer.PlayerInfo.collectibleImage;
            Debug.Log("SUCCESS");
        }
        if (!array.Contains(bookTag+"1"))
            {
                but++;
            Debug.Log("INCREMENET");
        }
    }

    public void SendMessageToManager()
    {
        throw new System.NotImplementedException();
    }
}

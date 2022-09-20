using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = System.Object;

public class DialogSys : SingleMono<AssetSys>
{
    public static void Alert(string message, string title, Action onFinished = null)
    {
        var asset = AssetSys.GetAssetSync("ui/dialog/dialog01.prefab") as GameObject;
        var go = Instantiate(asset);
        var lua = go.GetComponent<LuaBehaviour>().Lua;
        var contentText = lua.GetInPath<Text>("ContentText_Text");
        var titleText = lua.GetInPath<Text>("TitleText_Text");

        titleText.text = title;
        contentText.text = message;

        var button = lua.GetInPath<Button>("ConfirmBtn_Button") ;
        UnityAction onclick = () =>
        {
            if (onFinished != null)
            {
                onFinished();
            }

            button.onClick.RemoveAllListeners();
            go.SetActive(false);
        };

        //防止消息框未关闭时多次被调用
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onclick);
        go.SetActive(true);
    }

    public static void Confirm(string message, string title, Action<bool> onFinished = null)
    {
        var confirmPanel = GameObject.Find("Canvas").transform.Find("ConfirmBox");
        if (confirmPanel == null)
        {
            confirmPanel = (Instantiate(Resources.Load("ConfirmBox")) as GameObject).transform;
            confirmPanel.gameObject.name = "ConfirmBox";
            confirmPanel.SetParent(GameObject.Find("Canvas").transform);
            confirmPanel.localPosition = new Vector3(-8f, -18f, 0f);
        }

        confirmPanel.Find("confirmTitle").GetComponent<Text>().text = title;
        confirmPanel.Find("conmessage").GetComponent<Text>().text = message;

        var confirmBtn = confirmPanel.Find("confirmBtn").GetComponent<Button>();
        var cancelBtn = confirmPanel.Find("cancelBtn").GetComponent<Button>();
        Action cleanup = () =>
        {
            confirmBtn.onClick.RemoveAllListeners();
            cancelBtn.onClick.RemoveAllListeners();
            confirmPanel.gameObject.SetActive(false);
        };

        UnityAction onconfirm = () =>
        {
            if (onFinished != null)
            {
                onFinished(true);
            }

            cleanup();
        };

        UnityAction oncancel = () =>
        {
            if (onFinished != null)
            {
                onFinished(false);
            }

            cleanup();
        };

        //防止消息框未关闭时多次被调用
        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(onconfirm);
        cancelBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.AddListener(oncancel);
        confirmPanel.gameObject.SetActive(true);
    }
}
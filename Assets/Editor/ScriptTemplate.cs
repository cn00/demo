namespace AppEditor
{
    using UnityEngine;
    using System.Collections;
    using System.IO;

    public class ScriptTemplate: UnityEditor.AssetModificationProcessor
    {
        private const string AuthorName="cn";
        private const string AuthorEmail = "cool_navy@qq.com";

        private const string DateFormat = "yyyy/MM/dd HH:mm:ss";
        private static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            if (path.EndsWith(".lua")
            ||  path.EndsWith(".cs"))
            {
                string allText = File.ReadAllText(path);
                allText = allText.Replace("#AuthorName#", AuthorName);
                allText = allText.Replace("#AuthorEmail#", AuthorEmail);
                allText = allText.Replace("#CreateTime#", System.DateTime.Now.ToString(DateFormat));
                File.WriteAllText(path, allText);
                UnityEditor.AssetDatabase.Refresh();
            }

        }
    }
}
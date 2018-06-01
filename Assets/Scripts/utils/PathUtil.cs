using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public static class PathUtils
{
    #region const
    public const string ImagesRegex = "(.png$|.jpg$|.tga$|.psd$|.tiff$|.gif$|.jpeg$)";
    public const string AudiosRegex = "(.mp3$|.ogg$|.wav$|.aiff$)";
    public const string VideosRegex = "(.mov$|.mpg$|.mp4$|.avi$|.asf$|.mpeg$)";
    public const string ObjectRegex = "(.asset$|.prefab$)";
    public const string UserTextRegex = "(.lua$|.sql$)";
    public const string TextRegex = "(.txt$|.lua$|.xml$|.yaml$|.sql$|.bytes$)";
    public const string ExcelRegex = "(.xls$|.xlsx$)";
    public const string PunctuationRegex = "(`|~|\\!|\\@|\\#|\\$|\\%|\\^|\\&|\\*|\\(|\\)|\\-|\\+|\\=|\\[|\\]|\\{|\\}]|;|:|'|\"|,|<|\\.|>|\\?|/|\\\\| |\\t|\\r|\\n)";
    #endregion const

    public static List<string> GetFiles(this string path, string searchPattern, SearchOption searchOption)
    {
        // string[] searchPatterns = searchPattern.Split('|');
        // List<string> files = new List<string>();
        // foreach (string sp in searchPatterns)
        //     files.AddRange(System.IO.Directory.GetFiles(path, sp, searchOption));
        // files.Sort();
        // return files;

        return searchPattern.Split('|').SelectMany(
            filter => System.IO.Directory.GetFiles(path, filter, searchOption)).ToList();
    }

    public static string upath(this string self)
    {
        return self.Trim()
            .Replace("\\", "/")
            .Replace("//", "/");
    }
    public static string wpath(this string self)
    {
        return self.Trim()
            .Replace("/", "\\")
            .Replace("\\\\", "\\");
    }

    public static string Dir(this string self)
    {
        return Path.GetDirectoryName(self);
    }

    public static string CreateDir(this string self)
    {
        if(!Directory.Exists(self))
            Directory.CreateDirectory(self);
        return self;
    }

    public static bool IsImage(this string self)
    {
        var matches = Regex.Matches(self, ImagesRegex);
        return matches.Count > 0;
    }
    public static bool IsAudio(this string self)
    {
        var matches = Regex.Matches(self, AudiosRegex);
        return matches.Count > 0;
    }
    public static bool IsVideo(this string self)
    {
        var matches = Regex.Matches(self, VideosRegex);
        return matches.Count > 0;
    }
    public static bool IsObject(this string self)
    {
        var matches = Regex.Matches(self, ObjectRegex);
        return matches.Count > 0;
    }

    public static bool IsUserText(this string self)
    {
        var matches = Regex.Matches(self, UserTextRegex);
        return matches.Count > 0;
    }
    public static bool IsText(this string self)
    {
        var matches = Regex.Matches(self, TextRegex);
        return matches.Count > 0;
    }
    public static bool IsExcel(this string self)
    {
        var matches = Regex.Matches(self, ExcelRegex);
        return matches.Count > 0;
    }
}

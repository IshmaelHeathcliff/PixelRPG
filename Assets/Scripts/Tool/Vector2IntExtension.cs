using UnityEngine;
using System;

public static class Vector2IntExtension
{
    /// <summary>
    /// 将格式为 "x,y" 或 "(x,y)" 的字符串解析为 Vector2Int
    /// </summary>
    public static Vector2Int Parse(string s)
    {
        // 去除字符串中的括号和空格
        string cleanString = s.Replace("(", "").Replace(")", "").Trim();
        string[] parts = cleanString.Split(',');

        if (parts.Length != 2)
        {
            throw new FormatException($"Invalid Vector2Int format: {s}");
        }

        if (!int.TryParse(parts[0], out int x))
        {
            throw new FormatException($"Failed to parse X value from: {s}");
        }

        if (!int.TryParse(parts[1], out int y))
        {
            throw new FormatException($"Failed to parse Y value from: {s}");
        }

        return new Vector2Int(x, y);
    }

    /// <summary>
    /// 尝试解析字符串为 Vector2Int（不会抛出异常）
    /// </summary>
    public static bool TryParse(string s, out Vector2Int result)
    {
        result = Vector2Int.zero;
        try
        {
            result = Parse(s);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

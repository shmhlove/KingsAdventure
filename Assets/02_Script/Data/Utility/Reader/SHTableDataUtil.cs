using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public static class SHTableDataUtil
{
    public static string GetTypeToName(string strName)
    {
        strName = strName.ToLower();
        if (0 == strName.IndexOf("i_"))         return "int";
        else if (0 == strName.IndexOf("f_"))    return "float";
        else if (0 == strName.IndexOf("s_"))    return "text";
        else if (0 == strName.IndexOf("n_"))    return "text";

        return null;
    }

    public static string GetTypeToDB(string strType)
    {
        strType = strType.ToLower();
        switch(strType)
        {
            case "int":     return "INTEGER";
            case "float":   return "FLOAT";
            case "text":    return "TEXT";
        }
        
        return null;
    }
}
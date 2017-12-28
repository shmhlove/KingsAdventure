using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHConvertToByte
{
    // 인터페이스 : 에디터 클래스 전용 ( Resources폴더내에 컨버팅된 Byte파일을 쏟아 냄 )
    public void RunEditorToConvert()
    {
        var pTableData = new SHTableData();
        pTableData.OnInitialize();
        ConvertByteFiles(pTableData, string.Format("{0}{1}", SHPath.GetPathToResources(), "/Table/Bytes"));
        pTableData.OnFinalize();
    }

    // 인터페이스 : 바이트파일 컨버터 ( 전달된 TableData를 참조해서 전달된 저장경로에 쏟아 냄 )
    public void ConvertByteFiles(SHTableData pTableData, string strSavePath)
    {
        if (null == pTableData)
            return;

        SHUtils.ForToDic(pTableData.Tables, (pKey, pValue) =>
        {
            ConvertByteFile(pValue, strSavePath);
        });

        Debug.Log("<color=yellow>ConvertByteFiles Finish!!!</color>");
    }

    // 인터페이스 : 바이트파일 컨버터 ( 파일 하나 )
    public void ConvertByteFile(SHBaseTable pTable, string strSavePath)
    {
        if (null == pTable)
            return;

        byte[] pBytes = pTable.GetBytesTable();
        if (null == pBytes)
            return;
        
        SHUtils.SaveByte(pBytes, string.Format("{0}/{1}{2}", strSavePath, pTable.m_strByteFileName, ".bytes"));

        Debug.Log(string.Format("{0} To Convert Byte Files : {1}",
                    (true == pTable.IsLoadTable() ? "<color=yellow>Success</color>" : "<color=red>Fail!!</color>"),
                    pTable.m_strFileName));
    }
}
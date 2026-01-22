using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class CSVReader
{
    //CSV 1줄 파싱으로 문자열 배열로 리턴
    public static string[] ParseLine(string line)
    {
        return line.Split(',');
    }

    //CSV 전체 텍스트 파싱하여 줄 단위 리스트로 리턴 (헤더 제외)
    public static List<string[]> ParseCSV(string csvText)
    {
        List<string[]> list = new List<string[]>();
        string[] lines = csvText.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i]))
                continue;
            list.Add(ParseLine(lines[i]));
        }
        return list;
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace BalancingLibra.Data
{
    // 아이템 테이블 클래스
    public class ItemTable : IDataTable
    {
        private Dictionary<int, ItemData> _items = new Dictionary<int, ItemData>();

        public Dictionary<int, ItemData> Items => _items;
        public int Count => _items.Count;

        public void Load(string path)
        {
            _items.Clear();

            // Resources 폴더에서 CSV 파일 로드
            TextAsset csvFile = Resources.Load<TextAsset>(path);
            if (csvFile == null)
            {
                Logger.LogError($"[ItemTable] CSV 파일을 찾을 수 없습니다: {path}");
                return;
            }

            // CSV 파싱
            List<string[]> rows = CSVReader.ParseCSV(csvFile.text);

            // 각 행을 ItemData로 변환
            foreach (string[] row in rows)
            {
                if (row.Length < 3)
                {
                    Logger.LogWarning($"[ItemTable] 잘못된 형식의 데이터 행을 건너뜁니다: {string.Join(",", row)}");
                    continue;
                }

                // id, name, description 파싱
                if (!int.TryParse(row[0].Trim(), out int id))
                {
                    Logger.LogWarning($"[ItemTable] ID 파싱 실패: {row[0]}");
                    continue;
                }

                string name = row[1].Trim();
                string description = row[2].Trim();

                ItemData item = new ItemData(id, name, description);
                _items[id] = item;
            }

            Logger.Log($"[ItemTable] 아이템 {_items.Count}개가 로드되었습니다.");
            Logger.Log("[ItemTable] 아이템 로드 완료되었습니다.");
        }

        // ID로 아이템 데이터 가져오기
        public ItemData GetItem(int id)
        {
            if (_items.TryGetValue(id, out ItemData item))
            {
                return item;
            }
            return null;
        }
    }
}

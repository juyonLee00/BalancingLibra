namespace BalancingLibra.Data
{
    // 아이템 데이터 클래스
    public class ItemData
    {
        public int id;
        public string name;
        public string description;

        public ItemData(int id, string name, string description)
        {
            this.id = id;
            this.name = name;
            this.description = description;
        }
    }
}

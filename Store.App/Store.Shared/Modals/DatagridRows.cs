using System.Collections.Concurrent;

namespace Store.Shared.Modals
{
    public class DatagridRows
    {
        public DatagridRows()
        {
            Counter.AddOrUpdate(GetType(), 1, (type, i) => i + 1);

            int count = 0;
            Counter.TryGetValue(GetType(), out count);
            RowId = count;
        }

        public static ConcurrentDictionary<Type, int> Counter = new ConcurrentDictionary<Type, int>();

        public int RowId { get; set; }
    }
}

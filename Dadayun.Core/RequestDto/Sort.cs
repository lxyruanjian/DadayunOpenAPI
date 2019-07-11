namespace Dadayun.Core.RequestDto
{
    /// <summary>
    /// 排序
    /// </summary>
    public interface ISort
    {
        string PropertyName { get; set; }
        bool Ascending { get; set; }
    }

    public class Sort : ISort
    {
        public Sort(string propertyName, bool ascending = true)
        {
            PropertyName = propertyName;
            Ascending = ascending;
        }

        public Sort(string propertyName, string sortDir)
        {
            PropertyName = propertyName;
            if (!string.IsNullOrEmpty(sortDir))
                Ascending = sortDir.ToLower() == "asc" ? true : false;
            else
                Ascending = true;
        }
        public string PropertyName { get; set; }
        public bool Ascending { get; set; }
    }
}

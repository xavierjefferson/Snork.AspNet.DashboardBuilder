using System.Collections.Generic;

namespace Snork.AspNet.Dashboard.SysInfo
{
    internal class GridItemList : List<GridItem>
    {
        public GridItemList(string name)
        {
            GridName = name;
        }

        public string GridName { get; }

        public void Add(string name, string value)
        {
            Add(new GridItem { Name = name, Value = value });
        }
    }
}
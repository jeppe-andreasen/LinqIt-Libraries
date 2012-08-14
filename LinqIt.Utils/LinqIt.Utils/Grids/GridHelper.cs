using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Utils.Grids
{
    public class GridHelper<T>
    {
        private readonly int _columns;
        private readonly List<Row<T>> _rows;

        public GridHelper(int columns, IEnumerable<T> items, Func<T, int> columnFunction)
        {
            _columns = columns;

            _rows = new List<Row<T>>();

            Row<T> row = null;
            var column = 0;
            foreach (var item in items)
            {
                var itemColumns = columnFunction(item);
                if (itemColumns > _columns || itemColumns < 1)
                    continue;

                if (row == null || column == 0 || column + itemColumns > _columns)
                {
                    row = new Row<T>();
                    _rows.Add(row);
                    column = 0;
                }

                row.Add(item);
                column += itemColumns;


                if (column >= _columns)
                    column = 0;
            }
        }

        public IEnumerable<Row<T>> Rows
        {
            get { return _rows; }
        }



        public class Row<TE>
        {
            private readonly List<TE> _items;

            public Row()
            {
                _items = new List<TE>();
            }

            public void Add(TE item)
            {
                _items.Add(item);
            }

            public TE[] Cells { get { return _items.ToArray(); } }

        }
    }
}

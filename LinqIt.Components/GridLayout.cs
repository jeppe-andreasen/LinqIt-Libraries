using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Utils.Extensions;

namespace LinqIt.Components
{
    public enum GridLayoutCellType
    {
        Placeholder,
        ContentBlock,
        InnerGrid
    } ;

    public class GridLayout: GridCell
    {
        private readonly List<GridRow> _rows = new List<GridRow>();
        private readonly int _columns;

        public GridLayout(int columns) : base(columns, string.Empty, string.Empty, GridLayoutCellType.InnerGrid)
        {
            _columns = columns;
        }

        public IEnumerable<GridRow> Rows { get { return _rows; } }

        public GridRow AddRow()
        {
            var row = new GridRow(this);
            _rows.Add(row);
            return row;
        }

        public static GridLayout Empty
        {
            get
            {
                return new GridLayout(0);
            }
        }

        public IEnumerable<GridCell> GetPlaceholderCells()
        {
            var result = new List<GridCell>();
            var rows = new Queue<GridRow>();
            rows.EnqueueRange(_rows);
            while (rows.Count > 0)
            {
                var row = rows.Dequeue();
                foreach (var cell in row.Cells)
                {
                    if (cell.Type == GridLayoutCellType.Placeholder)
                        result.Add(cell);
                    else if (cell.Type == GridLayoutCellType.InnerGrid)
                    {
                        var grid = (GridLayout) cell;
                        rows.EnqueueRange(grid.Rows);
                    }
                }
            }
            return result;
        }
    }

    public class GridRow
    {
        private readonly List<GridCell> _cells = new List<GridCell>();
        private readonly GridLayout _layout;

        internal GridRow(GridLayout layout)
        {
            _layout = layout;
        }

        public IEnumerable<GridCell> Cells
        {
            get { return _cells; }
        }

        public GridCell AddCell(int columnSpan, string key, string displayName, GridLayoutCellType type)
        {
            var totalSpan = _cells.Select(c => c.ColumnSpan).Sum() + columnSpan;
            if (totalSpan > _layout.ColumnSpan)
                throw new ArgumentException("Cannot add cell, as the combined row span would be larger than " + _layout.ColumnSpan);

            var cell = new GridCell(columnSpan, key, displayName, type);
            _cells.Add(cell);
            return cell;
        }

        public GridLayout AddGridCell(int columnSpan)
        {
            var totalSpan = _cells.Select(c => c.ColumnSpan).Sum() + columnSpan;
            if (totalSpan > _layout.ColumnSpan)
                throw new ArgumentException("Cannot add cell, as the combined row span would be larger than " + _layout.ColumnSpan);

            var cell = new GridLayout(columnSpan);
            _cells.Add(cell);
            return cell;
        }
    }

    public class GridCell
    {
        internal GridCell(int columnSpan, string key, string displayName, GridLayoutCellType type)
        {
            ColumnSpan = columnSpan;
            Key = key;
            DisplayName = displayName;
            Type = type;
        }

        public int ColumnSpan { get; private set; }

        public string Key { get; private set; }

        public string DisplayName { get; private set; }

        public GridLayoutCellType Type { get; private set; }
    }
}

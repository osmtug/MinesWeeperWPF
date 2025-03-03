using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperWPF.Model
{
    public class GridRow
    {
        public ObservableCollection<GridCell> cells { get; set; }

        public GridRow()
        {
            cells = new ObservableCollection<GridCell>();
        }
    }
}

using System.Collections.ObjectModel;

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

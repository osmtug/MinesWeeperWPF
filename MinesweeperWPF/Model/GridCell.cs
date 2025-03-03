using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinesweeperWPF.Model
{
    public class GridCell : INotifyPropertyChanged
    {
        private bool _visible;
        public bool visible
        {
            get => _visible;
            set { _visible = value; OnPropertyChanged(); }
        }
        private int _value;
        public int Value
        {
            get => _value;
            set { _value = value; OnPropertyChanged(); }
        }
        public static readonly int BOMBE = -1;
        private bool _hasFlag { get; set; }
        public bool hasFlag
        {
            get => _hasFlag;
            set { _hasFlag = value; OnPropertyChanged(); }
        }
        public int column { get; set; }
        public int row { get; set; }
        public Brush? color { get; set; }
        public Brush? btnColor { get; set; }
        public static Brush BTN_COLOR1 = new SolidColorBrush(Color.FromRgb(170, 215, 81));
        public static Brush BTN_COLOR2 = new SolidColorBrush(Color.FromRgb(162, 209, 73));
        public static Brush HOOVER_COLOR1 = new SolidColorBrush(Color.FromRgb(191, 225, 125));
        public static Brush HOOVER_COLOR2 = new SolidColorBrush(Color.FromRgb(185, 221, 119));
        public static Brush COLOR1 = new SolidColorBrush(Color.FromRgb(229, 194, 159));
        public static Brush COLOR2 = new SolidColorBrush(Color.FromRgb(215, 184, 153));

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public GridCell(int color, int row, int column)
        {
            visible = true;
            Value = 0;
            btnColor = BTN_COLOR2;
            this.color = COLOR2;
            if (color == 0)
            {
                this.color = COLOR1;
                btnColor = BTN_COLOR1;
            }

            this.row = row;
            this.column = column;
            this.hasFlag = false;
        }
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MinesweeperWPF.Model
{
    public class GameMode : INotifyPropertyChanged
    {
        public string name { get; set; }
        public int size {  get; set; }
        public int nbBombe { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ToString() { return name; }
    }
}

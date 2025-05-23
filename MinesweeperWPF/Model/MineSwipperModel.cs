﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;

namespace MinesweeperWPF.Model
{
    public class MineSwipperModel : INotifyPropertyChanged
    {
        public int gridSize { get; set; } = 22; // taille de la grille
        private int nbMine = 10; // nombre de bombes
        private int nbCellOpen = 0; 
        private bool GridIsInitialize = false;
        private DispatcherTimer timer;
        private int seconds = 0;
        private string _secondsWith3Digits;
        public string SecondsWith3Digits
        {
            get { return _secondsWith3Digits; }
            set
            {
                _secondsWith3Digits = value;
                OnPropertyChanged(nameof(SecondsWith3Digits));
            }
        }
        private int _nbFlag = 10;
        public int NbFlag
        {
            get { return _nbFlag; }
            set
            {
                _nbFlag = value;
                OnPropertyChanged(nameof(NbFlag));
            }
        }
        public ObservableCollection<GridRow> matrix { get; set; }
        private bool verifCells;
        private MainWindow mainWindow;
        public ObservableCollection<GameMode> gameModes { get; set; }
        public bool gameInit = false;
        private GameMode _selectedGameMode;
        public GameMode SelectedGameMode
        {
            get { return _selectedGameMode; }
            set
            {
                _selectedGameMode = value;
                OnPropertyChanged(nameof(SelectedGameMode));
                gridSize = _selectedGameMode.size;
                nbMine = _selectedGameMode.nbBombe;
                if (gameInit) NewGame();
            }
        }

        public ObservableCollection<Particle> Particles { get; } = new();

        public void AddParticle(Particle particle)
        {
            particle.OnParticleExpired += RemoveParticle;
            Particles.Add(particle);
        }

        private void RemoveParticle(Particle particle)
        {
            particle.OnParticleExpired -= RemoveParticle; 
            Particles.Remove(particle);
        }

        public ObservableCollection<Particle> FlagParticles { get; } = new();

        public void AddFlagParticle(Particle particle)
        {
            particle.OnParticleExpired += RemoveFlagParticle;
            FlagParticles.Add(particle);
        }

        private void RemoveFlagParticle(Particle particle)
        {
            particle.OnParticleExpired -= RemoveFlagParticle; 
            FlagParticles.Remove(particle);
        }

        public ObservableCollection<Confetti> Confettis { get; } = new();

        public void AddConfettiParticle(Confetti particle)
        {
            particle.OnParticleExpired += RemoveConfettiParticle;
            Confettis.Add(particle);
        }

        private void RemoveConfettiParticle(Particle particle)
        {
            particle.OnParticleExpired -= RemoveConfettiParticle;
            Confettis.Remove((Confetti)particle);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MineSwipperModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            matrix = new ObservableCollection<GridRow>();
            gameModes = new ObservableCollection<GameMode>();
            gameModes.Add(new GameMode{
                name = "Facile",
                size = 9,
                nbBombe = 10
            });
            gameModes.Add(new GameMode
            {
                name = "Moyen",
                size = 16,
                nbBombe = 40
            });
            gameModes.Add(new GameMode
            {
                name = "Difficile",
                size = 22,
                nbBombe = 99
            });
            SelectedGameMode = gameModes[0];
            gameInit = true;
        }

        public void NewGame()
        {
            Confettis.Clear();
            //foreach ( Confetti confetti in Confettis)
            //{
            //    RemoveConfettiParticle(confetti);
            //}
            mainWindow.setWindowSize();

            verifCells = false;
            
            matrix.Clear();
            for (int i = 0; i < gridSize; i++)
            {
                matrix.Add(new GridRow());
                for (int j = 0; j < gridSize; j++)
                {
                    matrix[i].cells.Add(new GridCell((i + j) % 2, j, i));
                }
            }
            StopTimer();
            seconds = 0;
            SecondsWith3Digits = "000";
            GridIsInitialize = false;
            NbFlag = nbMine;
            nbCellOpen = 0;
        }

        public void initializeGame(GridCell c)
        {
            
            StartTimer();

            setBombInMatrix(c);
            setMatrixExceptBomb();
            mainWindow.Shake();
            GridIsInitialize = true;
        }

        private void setBombInMatrix(GridCell c)
        {
            int rowBeg = Math.Max(0, c.row - 1);
            int rowEnd = Math.Min(gridSize - 1, c.row + 1);
            int colBeg = Math.Max(0, c.column - 1);
            int colEnd = Math.Min(gridSize - 1, c.column + 1);
            List<int[]> disponiblePostion = new List<int[]>();
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if(j < rowBeg || j > rowEnd || i < colBeg || i > colEnd)
                        disponiblePostion.Add(new int[] { i, j });
                }
            }
            for (int i = 0; i < nbMine; i++)
            {
                int indicePosition;
                Random random = new Random();
                indicePosition = random.Next(disponiblePostion.Count());
                matrix[disponiblePostion[indicePosition][0]].cells[disponiblePostion[indicePosition][1]].Value = GridCell.BOMBE;
                
                disponiblePostion.RemoveAt(indicePosition);
            }
        }

        private int countNeighborBomb(int l, int r)
        {
            int countBomb = 0;
            if (matrix[l].cells[r].Value == -1) { return -1; }
            int startJ = -1, endJ = 1, startI = -1, endI = 1;

            if (l == 0) { startI = 0; }
            else if (l == gridSize - 1) { endI = 0; }

            if (r == 0) { startJ = 0; }
            else if (r == gridSize - 1) { endJ = 0; }

            for (int i = startI; i <= endI; i++)
            {
                for (int j = startJ; j <= endJ; j++)
                {
                    if (matrix[i + l].cells[j + r].Value == -1) { countBomb++; }
                }
            }
            return countBomb;
        }

        private void setMatrixExceptBomb()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    matrix[i].cells[j].Value = countNeighborBomb(i, j);
                }
            }
        }

        public void verifNeighbor(GridCell c, double posX = 0, double posY = 0, double size = 0)
        {
            int col = c.column;
            int row = c.row;
            int count = 0;
            for (int i = Math.Max(0, col - 1); i <= Math.Min(gridSize - 1, col + 1); i++)
            {
                for (int j = Math.Max(0, row - 1); j <= Math.Min(gridSize - 1, row + 1); j++)
                {
                    if (matrix[i].cells[j].hasFlag)
                    {
                        count++;
                    }
                }
            }

            if (count != 0 && matrix[col].cells[row].Value == count)
            {
                verifCells = true;
                for (int i = Math.Max(0, col - 1); i <= Math.Min(gridSize - 1, col + 1); i++)
                {
                    for (int j = Math.Max(0, row - 1); j <= Math.Min(gridSize - 1, row + 1); j++)
                    {
                        if (!matrix[i].cells[j].hasFlag && verifCells)
                        {
                            verifieCellule(matrix[i].cells[j], posX + (j - c.row) * size, posY + (i - c.column) * size, size);
                        }
                    }
                }
            }
        }

        public bool verifieCellule(GridCell c, double posX = 0, double posY = 0, double size = 0)
        {
            if (!GridIsInitialize) initializeGame(c);
            if (c.visible)
            {
                c.visible = false;
                AddParticle(new Particle(posX, posY, size));
                if (c.hasFlag)
                {
                    ToogleFlag(c, posX, posY, size);
                }
                nbCellOpen++;

                if (c.Value == -1)
                {
                    Random rand = new Random();
                    c.color = getBombeColor(rand.Next(0, 7));
                    for(int i = 0; i < 8; i++)
                    {
                        AddConfettiParticle(new Confetti(posX, posY, size/2, size/4, c.color));
                    }
                    GameOver(c, posX, posY, size);
                    return true;
                }
                else
                {
                    if (nbCellOpen == gridSize * gridSize - nbMine)
                    {
                        Win();
                        return true;
                    }
                    else
                    {
                        if (c.Value == 0)
                        {
                            for (int i = Math.Max(0, c.column - 1); i <= Math.Min(gridSize - 1, c.column + 1); i++)
                            {
                                for (int j = Math.Max(0, c.row - 1); j <= Math.Min(gridSize - 1, c.row + 1); j++)
                                {
                                    bool resultat = verifieCellule(matrix[i].cells[j], posX + (j - c.row) * size, posY + (i - c.column) * size, size);
                                    if (resultat == true) { return true; }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void GameOver(GridCell c, double posX, double posY, double size)
        {
            mainWindow.Shake();
            CallFunctionWithDelay(c, posX, posY, size);
            
            if (MessageBox.Show("Perdu ;( \n Veut tu rejouer", "Perdu", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                NewGame();
            }
            else { mainWindow.Close(); }
        }

        private void Win()
        {
            if (MessageBox.Show("Gagner !!! \n Veut tu rejouer", "Gagner", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                NewGame();
            }
            else { mainWindow.Close(); }
        }

        private void discoverAllBomb(GridCell c, double posX, double posY, double size)
        {

            Task.Delay(100).Wait(); // Bloque le thread pendant 1 seconde

            var nearestBombe = FindNearestBombe(c.column, c.row);
            if (nearestBombe != null)
            {
                GridCell c2 = matrix[nearestBombe.Value.x].cells[nearestBombe.Value.y];
                double newPosX = posX + (nearestBombe.Value.y - c.row) * size;
                double newPosY = posY + (nearestBombe.Value.x - c.column) * size;
                Random rand = new Random();
                c2.color = getBombeColor(rand.Next(0, 8));
                for (int i = 0; i < 8; i++)
                {
                    AddConfettiParticle(new Confetti(newPosX, newPosY, size / 2, size / 4, c2.color));
                }
                c2.visible = false;
                CallFunctionWithDelay(c2, newPosX, newPosY, size);
            }


        }

        private void CallFunctionWithDelay(GridCell c, double posX, double posY, double size)
        {
            double min = 0;
            double max = 0.5;
            Random rand = new Random();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(min + rand.NextDouble() * (max - min));
            timer.Tick += (sender, e) =>
            {
                timer.Stop(); // Arrête le timer
                discoverAllBomb(c, posX, posY, size);
            };
            timer.Start();
        }

        public void ToogleFlag(GridCell cell, double posX = 0, double posY = 0, double size = 0)
        {
            cell.hasFlag = !cell.hasFlag;
            if (cell.hasFlag) NbFlag--;
            else
            {
                Random random = new Random();
                AddFlagParticle(new Particle(posX, posY, size, random.Next(-100, 101)));
                NbFlag++;
            }
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void StopTimer()
        {
            if (timer != null) timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            seconds++;
            if (seconds >= 100) SecondsWith3Digits = seconds.ToString();
            else if (seconds >= 10) SecondsWith3Digits = "0" + seconds.ToString();
            else SecondsWith3Digits = "00" + seconds.ToString();
        }

        public (int x, int y)? FindNearestBombe(int startX, int startY)
        {

            var directions = new (int dx, int dy)[]
            {
            (0, 1), (1, 0), (0, -1), (-1, 0), // Droite, Bas, Gauche, Haut
            (1, 1), (1, -1), (-1, -1), (-1, 1) // Diagonales
            };

            var visited = new HashSet<(int, int)>();
            var queue = new Queue<(int x, int y)>();
            queue.Enqueue((startX, startY));
            visited.Add((startX, startY));

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();

                foreach (var (dx, dy) in directions)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < gridSize && ny >= 0 && ny < gridSize && !visited.Contains((nx, ny)))
                    {
                        visited.Add((nx, ny));
                        GridCell cell = matrix[nx].cells[ny];

                        if (cell.visible && cell.Value == -1 && !cell.hasFlag)
                        {
                            return (nx, ny);
                        }

                        queue.Enqueue((nx, ny));
                    }
                }
            }

            return null; // Aucun -1 trouvé
        }

        private SolidColorBrush getBombeColor(int val)
        {
            switch(val)
            {
                case 0: return new SolidColorBrush(Color.FromRgb(72, 133, 237));
                case 1: return new SolidColorBrush(Color.FromRgb(0, 135, 68));
                case 2: return new SolidColorBrush(Color.FromRgb(72, 230, 241));
                case 3: return new SolidColorBrush(Color.FromRgb(182, 72, 242));
                case 4: return new SolidColorBrush(Color.FromRgb(244, 132, 13));
                case 5: return new SolidColorBrush(Color.FromRgb(219, 50, 54));
                case 6: return new SolidColorBrush(Color.FromRgb(237, 68, 181));
                case 7: return new SolidColorBrush(Color.FromRgb(244, 194, 13));
            }
            return null ;
        }
    }
}

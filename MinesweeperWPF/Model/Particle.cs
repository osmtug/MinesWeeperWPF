using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinesweeperWPF.Model
{
    public class Particle : INotifyPropertyChanged
    {
        private double velocityX = 0;
        private double velocityY = 0;
        private double _positionX = 0;
        private double _positionY = 0;
        private double sizeDec = 5;
        private double _size = 10;
        private double gravity = 600;
        private double jumpForce = -100;
        private Stopwatch stopwatch = new Stopwatch();
        private double lastFrameTime = 0;
        private double _rotationAngle = 0;
        private double _centerX = 0;
        public double CenterX
        {
            get => _centerX;
            set
            {
                _centerX = value;
                OnPropertyChanged(nameof(CenterX));
            }
        }
        private double _centerY = 0;
        public double CenterY
        {
            get => _centerY;
            set
            {
                _centerY = value;
                OnPropertyChanged(nameof(CenterY));
            }
        }
        public double RotationAngle
        {
            get => _rotationAngle;
            set
            {
                _rotationAngle = value;
                OnPropertyChanged(nameof(RotationAngle));
            }
        }
        private double _rotationSpeed = 50;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<Particle>? OnParticleExpired;

        public double PositionX
        {
            get => _positionX;
            set { _positionX = value; OnPropertyChanged(); }
        }

        public double PositionY
        {
            get => _positionY;
            set { _positionY = value; OnPropertyChanged(); }
        }

        public double Size
        {
            get => _size;
            set
            {
                if (_size != value)
                {
                    _size = value;
                    OnPropertyChanged();
                    _centerX = _size / 2; 
                    _centerY = _size / 2;
                }
                if (_size < 1)
                {
                    Dispose();
                    OnParticleExpired?.Invoke(this);
                }
            }
        }
        private SolidColorBrush _brushColor;
        public SolidColorBrush BrushColor
        {
            get => _brushColor;
            set
            {
                _brushColor = value;
                OnPropertyChanged(nameof(BrushColor));
            }
        }
        private int red = 162;
        private int green = 209;
        private int blue = 73;

        public Particle()
        {
            CompositionTarget.Rendering += OnRendering;
            stopwatch.Start();
            velocityY = jumpForce;
        }
        public Particle(double posx, double posy, double size)
        {
            PositionX = posx;
            PositionY = posy;
            Size = size;
            sizeDec = size*1.5;
            Random random = new Random();
            _rotationSpeed = random.Next(-300, 301);
            velocityX = random.Next(-100, 101);
            CompositionTarget.Rendering += OnRendering;
            stopwatch.Start();
            velocityY = jumpForce;
            BrushColor = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));
        }

        public Particle(double posx, double posy, double size, double velocityX)
        {
            PositionX = posx;
            PositionY = posy;
            Size = size;
            sizeDec = size;
            Random random = new Random();
            _rotationSpeed = random.Next(-300, 301);
            this.velocityX = velocityX;
            CompositionTarget.Rendering += OnRendering;
            stopwatch.Start();
            gravity = 600;
            velocityY = random.Next(-300, -100);
            
            BrushColor = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnRendering(object sender, EventArgs e)
        {
            double currentTime = stopwatch.Elapsed.TotalSeconds;
            double deltaTime = currentTime - lastFrameTime;
            lastFrameTime = currentTime;

            Update(deltaTime);
        }

        private void Update(double deltaTime)
        {
            velocityY += gravity * deltaTime;
            PositionY += velocityY * deltaTime;
            PositionX += velocityX * deltaTime;
            RotationAngle += _rotationSpeed * deltaTime;
            if (Size - deltaTime * sizeDec < 0) Size = 0;
            else Size -= deltaTime * sizeDec;
            
            red -= 1;
            green -= 1;
            blue -= 1;
            BrushColor = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));
        }

        public void Dispose()
        {
            CompositionTarget.Rendering -= OnRendering; 
        }
    }

}

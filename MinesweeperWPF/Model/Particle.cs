using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace MinesweeperWPF.Model
{
    public class Particle : INotifyPropertyChanged
    {
        protected double velocityX = 0;
        protected double velocityY = 0;
        protected double _positionX = 0;
        protected double _positionY = 0;
        protected double sizeDec = 5;
        protected double _size = 10;
        protected double gravity = 600;
        protected double jumpForce = -100;
        protected Stopwatch stopwatch = new Stopwatch();
        protected double lastFrameTime = 0;
        protected double _rotationAngle = 0;
        protected double _centerX = 0;
        public double CenterX
        {
            get => _centerX;
            set
            {
                _centerX = value;
                OnPropertyChanged(nameof(CenterX));
            }
        }
        protected double _centerY = 0;
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
        protected double _rotationSpeed = 50;

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
        protected SolidColorBrush _brushColor;
        public SolidColorBrush BrushColor
        {
            get => _brushColor;
            set
            {
                _brushColor = value;
                OnPropertyChanged(nameof(BrushColor));
            }
        }
        protected int red = 162;
        protected int green = 209;
        protected int blue = 73;

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

        protected void OnRendering(object sender, EventArgs e)
        {
            double currentTime = stopwatch.Elapsed.TotalSeconds;
            double deltaTime = currentTime - lastFrameTime;
            lastFrameTime = currentTime;

            Update(deltaTime);
        }

        protected virtual void Update(double deltaTime)
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

        protected void RaiseOnParticleExpired()
        {
            OnParticleExpired?.Invoke(this);
        }

        
    }

}

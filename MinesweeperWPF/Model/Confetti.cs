using System.Windows.Media;

namespace MinesweeperWPF.Model
{
    public class Confetti : Particle
    {
        private double _size_y;

        public double SizeY
        {
            get => _size_y;
            set
            {
                if (_size_y != value)
                {
                    _size_y = value;
                    OnPropertyChanged();
                    _centerX = _size / 2;
                    _centerY = _size_y / 2;
                }
                if (_size < 1)
                {
                    Dispose();
                    RaiseOnParticleExpired();
                }
            }
        }


        public Confetti(double posx, double posy, double size, double sizey)
        {
            PositionX = posx;
            PositionY = posy;
            Size = size;
            SizeY = sizey;
            sizeDec = size * 1.5;
            Random random = new Random();
            _rotationSpeed = 0;
            velocityX = random.Next(-100, 100);
            _oscillationAmplitude = random.Next(50, 200);
            _oscillationSpeed = randomDouble(2, 5, random);
            duration = randomDouble(4, 6, random);
            CompositionTarget.Rendering += OnRendering;
            stopwatch.Start();
            velocityY = randomDouble(jumpForce * 3, jumpForce / 2, random);
            BrushColor = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));
        }

        public Confetti(double posx, double posy, double size, double sizey, Brush color)
        {
            PositionX = posx;
            PositionY = posy;
            Size = size;
            SizeY = sizey;
            sizeDec = size * 1.5;
            Random random = new Random();
            _rotationSpeed = 0;
            velocityX = random.Next(-100, 100);
            _oscillationAmplitude = random.Next(50, 200);
            _oscillationSpeed = randomDouble(2, 5, random);
            duration = randomDouble(4, 6, random);
            CompositionTarget.Rendering += OnRendering;
            stopwatch.Start();
            velocityY = randomDouble(jumpForce * 3, jumpForce / 2, random);
            BrushColor = (SolidColorBrush)color;
        }

        private double randomDouble(double min, double max, Random rand)
        {
            return min + rand.NextDouble() * (max - min);
        }

        private double _oscillationSpeed = 5.0f;  
        private double _oscillationAmplitude = 100.0f; 
        private double _elapsedTime_oscillation = 0.0f;
        private float _rotationAmplitude = 10.0f;

        private double duration = 5;
        private double _elapsedTime = 0.0f;


        protected override void Update(double deltaTime)
        {
            _elapsedTime_oscillation += deltaTime;

            if (velocityY < 30)
            {
                velocityY += gravity * 0.5f * deltaTime;
            }
            PositionY += velocityY * deltaTime;

            if(velocityY > 0)
            {
                PositionX += MathF.Sin((float)(_elapsedTime_oscillation * _oscillationSpeed)) * _oscillationAmplitude * deltaTime;
                float rotationOffset = MathF.Sin((float)(_elapsedTime_oscillation * _oscillationSpeed)) * _rotationAmplitude;
                RotationAngle = rotationOffset;
            }
            else
            {
                PositionX += velocityX * deltaTime;
            }



            // Réduction de la taille
            if (_elapsedTime > duration)
            {
                if (Size - deltaTime * sizeDec < 0)
                    Size = 0;
                else
                    Size -= deltaTime * sizeDec;
                    SizeY -= deltaTime * sizeDec;
            }
            else
            {
                _elapsedTime += deltaTime;
            }
        }
    }
}

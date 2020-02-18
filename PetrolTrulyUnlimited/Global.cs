using System;
using System.Windows.Media.Animation;

namespace PetrolTrulyUnlimited
{
    class Global
    {
        //Constants
        public const int MIN_SPAWN_TIME = 1500;
        public const int MAX_SPAWN_TIME = 2200;

        public const int MIN_SERVICE_TIME = 1000;
        public const int MAX_SERVICE_TIME = 2000;

        public const int MAX_FUELLING_TIME = 12000; 

        public const float PUMP_VELOCITY = 1.5f;

        public const byte LOWEST_PRIORITY_PUMP = 6;

        static public QueueInformation queueInformation = new QueueInformation();

        //Animations
        static public DoubleAnimation fadeOut = new DoubleAnimation()
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(150)
        };

        static public DoubleAnimation fadeIn = new DoubleAnimation()
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(150)
        };
    }
}

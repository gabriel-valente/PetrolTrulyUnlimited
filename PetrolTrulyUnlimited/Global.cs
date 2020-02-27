using PetrolTrulyUnlimited.Entity;
using System;
using System.Windows.Media.Animation;

namespace PetrolTrulyUnlimited
{
    class Global
    {
        //Preferences
        static public int MIN_SPAWN_TIME = 1500;
        static public int MAX_SPAWN_TIME = 2200;

        static public int MIN_SERVICE_TIME = 7000;
        static public int MAX_SERVICE_TIME = 10000;

        static public int MAX_FUELLING_TIME = 18000;

        static public float PUMP_VELOCITY = 3.5f;

        static public int ANIMATION_TIME = 150;

        static public int MAX_QUEUE_SIZE = 5;

        //Constants
        public const byte LOWEST_PRIORITY_PUMP = 6;
        public const int DIESEL_INDEX = 0;
        public const int GASOLINE_INDEX = 1;
        public const int LPG_INDEX = 2;

        //Animations
        static public DoubleAnimation fadeOut = new DoubleAnimation()
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(ANIMATION_TIME)
        };

        static public DoubleAnimation fadeIn = new DoubleAnimation()
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(ANIMATION_TIME)
        };
    }
}

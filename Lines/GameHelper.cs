using System;
using System.Threading.Tasks;
using System.Windows.Media;

using static Lines.Settings;
using static Lines.GameHelper;

namespace Lines
{
    delegate void DefaultEventHandler(object sender);

    static class GameHelper
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

        public static Color GetRandomColor()
        {
            lock (SyncLock)
            {
                var colourCount = AllowedColours.Count;
                int index = RandomObject.Next(0, colourCount);
                var color = AllowedColours[index];

                return color;
            }
        }

        public static object SyncLock
        {
            get
            {
                return syncLock;
            }
        }

        public static Random RandomObject
        {
            get
            {
                return random;
            }
        }
    }
}

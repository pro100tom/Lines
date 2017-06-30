using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lines
{
    class MovementInfo
    {
        Direction direction;
        int stepNumber;

        public int StepNumber
        {
            get
            {
                return stepNumber;
            }

            set
            {
                stepNumber = value;
            }
        }

        internal Direction Direction
        {
            get
            {
                return direction;
            }

            set
            {
                direction = value;
            }
        }
    }
}

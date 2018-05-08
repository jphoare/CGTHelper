using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CGT2
{
	[Serializable, ClassInterface(ClassInterfaceType.None), ComVisible(true)]
    public class Positions : List<Position>, IPositionsBase
    {
        
        public double PNL
        {
            get
            {
                double total = 0;
                this.ForEach((x) => total += x.PNL);
                return total;
            }
        }
        public double BookCost
        {
            get
            {
                double total = 0;
                this.ForEach((x) => total += x.BookCost);
                return total;
            }
        }
        public double Value
        {
            get
            {
                double total = 0;
                this.ForEach((x) => total += x.Value);
                return total;
            }
        }
    }
}

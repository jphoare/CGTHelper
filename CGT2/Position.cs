using System;
using System.Runtime.InteropServices;

namespace CGT2
{
	[Serializable, ClassInterface(ClassInterfaceType.None), ComVisible(true)]
	public class Position : IPosition
    {
        string m_name;
        double m_bookcost;
        double m_value;
        double m_sell;

		public Position()
		{
			
		}
        public Position(string name, double value, double bookcost)
        {
            Name = name;
            Value = value;
            BookCost = bookcost;
        }
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        public double Value
        {
            get
            {
                return m_value;
            }
            set
            {
                if (value < 0) throw new System.ArgumentException("Cannot have a negative value");
                m_value = value;
            }
        }
        public double BookCost
        {
            get
            {
                return m_bookcost;
            }
            set
            {
                if (value < 0) throw new System.ArgumentException("Cannot have a negative book cost");
                m_bookcost = value;
            }
        }
        public double PNL
        {
            get
            {
                return m_value - m_bookcost;
            }

        }
        public double PercentageConstrained
        {
            get
            {
                return PNL/Value;
            }

        }
        public double Sell
        {
            get
            {
                return m_sell;    
            }
            set
            {
                if (value > m_value) throw new System.ArgumentException("Sell amount is greater than value");
                m_sell = value;
            }
        }
        public double Execute()
        {
            double pnl = PNL * m_sell / m_value;
            if (m_sell > 0) 
            {
                Console.WriteLine("Selling " + m_name + " Value " + m_sell + " PNL " + pnl);
                m_bookcost *= (m_value - m_sell) / m_value;
                m_value -= m_sell;
                m_sell = 0;
            }

            return pnl;
        }
    }
}

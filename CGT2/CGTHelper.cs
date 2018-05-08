using System;
using System.Runtime.InteropServices;

namespace CGT2
{
    
	[Serializable, ClassInterface(ClassInterfaceType.None), ComVisible(true)]
	public class CGTHelper : ICGTHelper
    {
        static double EPSILON = 1e-10;

        double m_ISAAllowance;
        double m_CGTAllowance;
        double m_ValueSold;
        double m_TaxableGain;

      
        public double ISAAllowance
        {
            get
            {
                return m_ISAAllowance;
            }
            set
            {
                if (value < 0) throw new System.ArgumentException("Cannot have a negative allowance!");
                m_ISAAllowance = value;
            }
        }

        public double CGTAllowance
        {
            get
            {
                return m_CGTAllowance;
            }
            set
            {
                if (value < 0) throw new System.ArgumentException("Cannot have a negative allowance!");
                m_CGTAllowance = value;
            }
        }

        public double ValueSold
        {
            get
            {
                return m_ValueSold;
            }

        }

        public double TaxableGain
        {
            get
            {
                return m_TaxableGain;
            }
           
        }

		public void BedAndISA(IPositions positions)
		{
			 BedAndISA((Positions) positions);
		}

        public void BedAndISA(Positions positions)
        {
            // This method finds the optimal positions to sell to meet a specific ISA subsubscription.
            // Where there is available CGTAllowance then the most heavilty CGT constrained positions are sold first.
            // Once the CGT allowance has been used then it looks to match off the most heavily constrained positions with any offsetting uncrystalised loss making positions.
            // Once there is no more losses or allowances to use then the algorithm will take the least constained positions to minimise any Taxable Gain.

            // Sort the positions by the most CGT constrained first.
            positions.Sort((a, b) => b.PercentageConstrained.CompareTo(a.PercentageConstrained));

			double start_value = positions.Value;

			double total_pnl = 0;
            int i = 0;
            int j = positions.Count - 1;
            while (j > i)
            {
                Position top = positions[i]; // The position with the highest PnL (by %).
                Position bottom = positions[j]; // The position with the lowest PnL (by %).

                // First find out max i can sell to net off PnL completely.
				double net_pnl = top.PNL + bottom.PNL;

                if (CGTAllowance > 0)
                {
                    // If there is any CGT Allowance left then use that first by selling the most CGT constrained positions first.
					double r = Math.Min(1.0, CGTAllowance / top.PNL);
                    top.Sell = r * top.Value;
                    bottom.Sell = 0;
                }
                else if (net_pnl <= 0)
                {
                    // If the net PnL is negative then we can safely sell all of the top holding and then enough of the bottom holding to be PnL flat.
                    top.Sell = top.Value;
                    // If both top and bottom positions have negative pnl then only the top position will be sold.
                    if (Math.Abs(bottom.PNL) > EPSILON) 
                        bottom.Sell = bottom.Value * Math.Max(0, top.PNL / -bottom.PNL);
                }
                else
                {
                    // If the net PnL is positive then we can safely sell all of the bottom holding and then enough of the top holding to be PnL flat.
                    bottom.Sell = bottom.Value;
                    // If both the top and bottom positions have positive PnL then only the bottom position will be sold.
                    if (Math.Abs(top.PNL) > EPSILON)
                        top.Sell = top.Value * Math.Max(0, -bottom.PNL / top.PNL);
                }

                // Next check to see if we need to sell all of it.
				double adjustment = Math.Min(1, ISAAllowance / (bottom.Sell + top.Sell));
                bottom.Sell *= adjustment;
                top.Sell *= adjustment;

                // Adjust the ISA allowance
                ISAAllowance -= (bottom.Sell + top.Sell);

                // Execute this trade and return total PnL it generate
				double pnl = top.Execute() + bottom.Execute();

                // Calculate how much of this falls within the remaining CGT Allowance
				double offset = Math.Min(pnl, CGTAllowance);

                // Adjust Allowance
                CGTAllowance -= offset;

                // Calculate total PnL subject to CGT.
                total_pnl += pnl - offset;

                // Terminate if ISA Allowance is forfilled
                if (Math.Abs(ISAAllowance) < EPSILON) break;

                // Adjust pointers if we sold all the position.
                if (Math.Abs(top.Value) < EPSILON) i++; // Move top down
                if (Math.Abs(bottom.Value) < EPSILON) j--; // Move bottom up

            }

            if (i == j)
            {
                Position top = positions[i];

                top.Sell = Math.Min(top.Value, ISAAllowance);
                
				double pnl = top.Execute();
                
				double offset = Math.Min(pnl, CGTAllowance);

                // Adjust Allowance
                CGTAllowance -= offset;

                // Calculate total PnL subject to CGT.
                total_pnl += pnl - offset;
            }

            m_ValueSold = start_value - positions.Value;
            m_TaxableGain = total_pnl;

        }

    }
}

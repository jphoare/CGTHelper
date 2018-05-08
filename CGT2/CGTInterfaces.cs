using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CGT2
{
	public interface IPosition
    {
		string Name
        {
            get;
            set;
        }
		double Value
        {
            get;
            set;
        }
		double BookCost
        {
            get;
            set;
        }
		double PNL
        {
            get;
        }
		double PercentageConstrained
        {
            get;
        }
		double Sell
        {
            get;
            set;
        }
		double Execute();
    }

    public interface IPositionsBase
	{
		double PNL
        {
            get;
        }
		double BookCost
        {
            get;
        }
		double Value
        {
            get;
        }	
	}

	public interface IPositions : IList<IPosition>, IPositionsBase
    {

    }

    public interface ICGTHelper
	{
		double ISAAllowance
        {
            get;
            set;
        }
		double CGTAllowance
        {
            get;
            set;
        }
		double ValueSold
        {
            get;
        }
		double TaxableGain
        {
            get;
        }   
		void BedAndISA(IPositions positions);
	}
}

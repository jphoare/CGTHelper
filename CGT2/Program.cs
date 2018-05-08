using System;


namespace CGT2
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello World!");

            Positions positions = new Positions();

            positions.Add(new Position("Fund A", 1000, 500));
            positions.Add(new Position("Fund B", 30000, 10000)); 
            positions.Add(new Position("Fund C", 5000, 10000)); 
            positions.Add(new Position("Fund D", 11000, 10000));

            var helper = new CGTHelper();

            helper.CGTAllowance = 10000.0;
            helper.ISAAllowance = 20000.0;

            helper.BedAndISA(positions);

            Console.WriteLine("Finished");

        }
    }
}

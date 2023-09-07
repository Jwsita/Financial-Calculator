using System;

namespace RateCalculator
{
    public class Mortgage
    {
        private double Pv { get; set; }
        private double Pmt { get; set; }
        private double Nper { get; set; }
        internal double Rate { get; set; }

        public Mortgage(double pv, double pmt, double nper)
        {
            Pv = pv;
            Pmt = pmt;
            Nper = nper;
            CalculateRate();
        }

        private void CalculateRate()
        {
            const double guess = 0.1;
            const double tolerance = 1e-6;
            const int maxIterations = 100;

            int whenValue = 0;  // 'end' equivalent
            double rn = guess;
            int iterator = 0;
            bool close = false;

            while (iterator < maxIterations && !close)
            {
                double rnp1 = rn - GDivGp(rn, whenValue);
                close = Math.Abs(rnp1 - rn) < tolerance;
                iterator++;
                rn = rnp1;
            }

            Rate = close ? rn : double.NaN;
        }

        private double GDivGp(double rn, int when)
        {
            double g = Pmt * (1 + rn * when) / rn * (Math.Pow(1 + rn, Nper) - 1) + Pv * Math.Pow(1 + rn, Nper);
            double gp = Pv * Nper * Math.Pow(1 + rn, Nper - 1) + Pmt * when * (Math.Pow(1 + rn, Nper) - 1) / rn + Pmt * Nper * (1 + rn * when) / rn * Math.Pow(1 + rn, Nper - 1) - Pmt * (Math.Pow(1 + rn, Nper) - 1) / Math.Pow(rn, 2);
            return g / gp;
        }
    }
}

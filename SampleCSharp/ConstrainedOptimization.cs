using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleCSharp
{
    /// <summary>
    /// Constrained optimization for the Heuristic algorithm
    /// </summary>
    class ConstrainedOptimization : LibOptimization.Optimization.absObjectiveFunction
    {
        /// <summary>
        /// polynomial function
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public override double F(List<double> a)
        {
            var x = a[0];
            var eval = 10 - 10 * x + 0.5 * x * x + x * x * x + 0.1 * x * x * x * x;
            if (x <= -8 || x > -6)
            {
                return 30.0 + Math.Abs(eval) + Math.Abs(x);
            }
            else
            {
                return eval;
            }
        }

        public override List<double> Gradient(List<double> aa)
        {
            return null;
        }

        public override List<List<double>> Hessian(List<double> aa)
        {
            return null;
        }

        public override int NumberOfVariable
        {
            get
            {
                return 1;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleCSharp
{
    /// <summary>
    /// Unconstrained optimization
    /// </summary>
    class UnconstrainedOptimization : LibOptimization.Optimization.absObjectiveFunction
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
            return eval;
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

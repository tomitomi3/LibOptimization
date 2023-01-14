using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleCSharp
{
    /// <summary>
    /// for Rosen block function(Bench mark function)
    /// </summary>
    class RosenBrock : LibOptimization.Optimization.absObjectiveFunction
    {
        private int _dim = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dim"></param>
        public RosenBrock(int dim)
        {
            if(dim <=1)
            {
                throw new NotImplementedException();
            }
            this._dim = dim;
        }

        public override double F(List<double> x)
        {
            double ret = 0.0;
            for(int i=0;i<_dim-1;i++)
            {
                var temp1 = x[i + 1] - x[i] * x[i];
                var temp2 = x[i] - 1;
                ret += 100 * temp1* temp1 + temp2*temp2;
            }
            return ret;
        }

        public override List<double> Gradient(List<double> aa, double h = 1E-08)
        {
            return null;
        }

        public override List<List<double>> Hessian(List<double> aa, double h = 1E-08)
        {
            return null;
        }

        public override int NumberOfVariable()
        {
            return this._dim;
        }
    }
}

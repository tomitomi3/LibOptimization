using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleCSharp
{
    //RosenBlock Functio
    class RosenBlock : LibOptimization.absObjectiveFunction 
    {
        public override double F(List<double> ai_var)
        {
            if (ai_var == null)
            {
                return 0.0;
            }
            double x1 = ai_var[0];
            double x2 = ai_var[1];
            return 100 * (x2 - x1 * x1) * (x2 - x1 * x1) + (1 - x1) * (1 - x1);
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
                return 2;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Target Function
            RosenBlock func = new RosenBlock();

            //Set Function
            LibOptimization.absOptimization opt = new LibOptimization.clsOptNelderMead(func);
            opt.Init();

            //Optimization
            opt.DoIteration();

            //Check Error
            if (opt.IsRecentError() == true)
            {
                return;
            }
            else
            {
                //Get Result
                LibOptimization.clsUtil.DebugValue(opt);
            }
        }
    }
}

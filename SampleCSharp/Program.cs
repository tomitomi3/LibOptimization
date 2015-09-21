using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibOptimization.Optimization;
using LibOptimization.Util;

namespace SampleCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Typical use
            {
                //Target Function
                var func = new RosenBlock();

                //Set Function
                var opt = new clsOptNelderMead(func);
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
                    clsUtil.DebugValue(opt);
                }
            }

            //Unconstrained or Constrained
            {
                //Target Function
                var func = new UnconstrainedOptimization();

                //Set Function
                var opt = new clsOptRealGASPX(func);
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
                    clsUtil.DebugValue(opt);
                }
            }

            {
                //Target Function
                var func = new ConstrainedOptimization();

                //Set Function
                var opt = new clsOptRealGASPX(func);
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
                    clsUtil.DebugValue(opt);
                }
            }

            //LowerBounds and UpperBounds
            {
                //Target Function
                var func = new LibOptimization.BenchmarkFunction.clsBenchSphere(2);

                //Set Function
                var opt = new clsOptRealGASPX(func);

                //Set opt optparameter
                opt.LowerBounds = new double[] { 0.0, 0.0 };
                opt.UpperBounds = new double[] { 1.0, 1.0 };
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
                    clsUtil.DebugValue(opt);
                }
            }
        }
    }
}

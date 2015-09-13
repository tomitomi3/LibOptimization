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
        }
    }
}

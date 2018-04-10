Imports System.Text
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Optimization
Imports LibOptimization.Util
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class TestLibOptimizationDotNet35
    ''' <summary>
    ''' 最適化アルゴリズムの確認 球面関数
    ''' </summary>
    <TestMethod()> Public Sub TestOptimizationSphere2DDotNet35()
        Dim optimizers = clsUtil.GetOptimizersForUnitTest(New clsBenchSphere(2))
        For Each opt In optimizers
            Console.WriteLine("Test optimize algo : {0}", opt.GetType().Name)

            'check init
            opt.Init()
            Dim errorFlg = opt.IsRecentError()
            Assert.IsFalse(errorFlg)

            'check iterate
            opt.DoIteration()
            errorFlg = opt.IsRecentError()
            Assert.IsFalse(errorFlg)

            'Eval
            If Math.Abs(opt.Result.Eval) > 0.1 Then
                Assert.Fail(String.Format("fail:{0} Eval:{1}", opt.GetType().Name, opt.Result.Eval))
            End If
            Console.WriteLine(String.Format("Success Eval {0}", opt.Result.Eval))

            'Result
            If Math.Abs(opt.Result(0)) > 0.1 OrElse Math.Abs(opt.Result(1)) > 0.1 Then
                Assert.Fail(String.Format("fail:{0} Result:{1} {2}", opt.GetType().Name, opt.Result(0), opt.Result(1)))
            End If
            Console.WriteLine(String.Format("Success Result {0} {1}", opt.Result(0), opt.Result(1)))
        Next
    End Sub
End Class
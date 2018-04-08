Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

'LibOptimization
Imports LibOptimization
Imports LibOptimization.MathUtil
Imports LibOptimization.Optimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Util

<TestClass()> Public Class UnitTestLibOptimization2
#Region "Optimization"
    Private Function GetOptimizers() As List(Of absOptimization)
        Dim optimizers As New List(Of absOptimization)
        optimizers.Add(New clsOptCS(New clsBenchSphere(2)))
        optimizers.Add(New clsOptDE(New clsBenchSphere(2)))
        optimizers.Add(New clsOptDEJADE(New clsBenchSphere(2)))
        optimizers.Add(New clsOptES(New clsBenchSphere(2)))
        optimizers.Add(New clsOptFA(New clsBenchSphere(2)))
        optimizers.Add(New clsOptHillClimbing(New clsBenchSphere(2)))
        optimizers.Add(New clsOptNelderMead(New clsBenchSphere(2)))
        optimizers.Add(New clsOptNelderMeadWiki(New clsBenchSphere(2)))
        optimizers.Add(New clsOptNewtonMethod(New clsBenchSphere(2)))
        optimizers.Add(New clsOptPatternSearch(New clsBenchSphere(2)))
        optimizers.Add(New clsOptPSO(New clsBenchSphere(2)))
        optimizers.Add(New clsOptPSOAIW(New clsBenchSphere(2)))
        optimizers.Add(New clsOptPSOChaoticIW(New clsBenchSphere(2)))
        optimizers.Add(New clsOptPSOLDIW(New clsBenchSphere(2)))
        optimizers.Add(New clsOptRealGABLX(New clsBenchSphere(2)))
        optimizers.Add(New clsOptRealGAPCX(New clsBenchSphere(2)))
        optimizers.Add(New clsOptRealGAREX(New clsBenchSphere(2)))
        optimizers.Add(New clsOptRealGASPX(New clsBenchSphere(2)))
        Dim undx = New clsOptRealGAUNDX(New clsBenchSphere(2))
        undx.ALPHA = 0.6
        optimizers.Add(undx)
        Dim sa = New clsOptSimulatedAnnealing(New clsBenchSphere(2))
        'sa.NeighborRange = 0.1
        optimizers.Add(sa)
        optimizers.Add(New clsOptSteepestDescent(New clsBenchSphere(2)))
        Return optimizers
    End Function

    <TestMethod()> Public Sub TestRetryCheck()
        Dim optimizers = GetOptimizers()
        For Each opt In optimizers
            opt.Init()
            opt.DoIteration()
            Dim recentCount = opt.IterationCount
            opt.Init()
            If opt.IterationCount = recentCount Then
                Dim str = String.Format("fail {0} : init iteration count", opt.GetType().Name)
                Console.WriteLine(str)
                Assert.Fail(str)
            End If
        Next
    End Sub

    <TestMethod()> Public Sub TestInitialPositon()
        Dim optimizers = GetOptimizers()
        For Each opt In optimizers
            Console.WriteLine("Test optimize algo : {0}", opt.GetType().Name)

            Dim range = 100.0

            'init
            opt.InitialPosition = {range, range}
            opt.InitialValueRangeLower /= 10
            opt.InitialValueRangeUpper /= 10
            opt.Init()

            'initial position check
            Dim result = opt.Result.ToArray()
            Dim minRange = range + opt.InitialValueRangeLower - 0.1
            Dim maxRange = range + opt.InitialValueRangeUpper + 0.1
            If minRange > result(0) OrElse maxRange < result(0) OrElse minRange > result(1) OrElse maxRange < result(1) Then
                Assert.Fail(String.Format("fail {0} : initial posiition {1} {2}", opt.GetType().Name, opt.Result(0), opt.Result(1)))
            End If

            'do opt
            Console.WriteLine(" Before Result {0} {1}", opt.Result(0), opt.Result(1))
            opt.DoIteration()
            Console.WriteLine(" After  Result {0} {1}  Iteration count : {2}", opt.Result(0), opt.Result(1), opt.IterationCount)

            'result check
            If Math.Abs(opt.Result(0)) > 0.05 OrElse Math.Abs(opt.Result(1)) > 0.05 Then
                'retry
                Console.WriteLine(" ---Retry using recent best result---")
                opt.InitialPosition = opt.Result.ToArray()
                opt.Init()
                Console.WriteLine(" Before Result {0} {1}", opt.Result(0), opt.Result(1))
                opt.DoIteration()
                Console.WriteLine(" After  Result {0} {1}  Iteration count : {2}", opt.Result(0), opt.Result(1), opt.IterationCount)
                If Math.Abs(opt.Result(0)) > 0.05 OrElse Math.Abs(opt.Result(1)) > 0.05 Then
                    Assert.Fail(String.Format("fail {0} : result {1} {2}", opt.GetType().Name, opt.Result(0), opt.Result(1)))
                End If
            End If
        Next
    End Sub
#End Region
End Class
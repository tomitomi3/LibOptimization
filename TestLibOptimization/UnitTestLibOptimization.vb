Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

'LibOptimization
Imports LibOptimization
Imports LibOptimization.MathUtil
Imports LibOptimization.Optimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Util

''' <summary>
''' 単体テスト 最適化
''' </summary>
<TestClass()> Public Class UnitTestLibOptimization
#Region "Optimization"
    ''' <summary>
    ''' 最適化アルゴリズムの確認 球面関数
    ''' </summary>
    <TestMethod()> Public Sub TestOptimizationSphere2D()
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

    ''' <summary>
    ''' 最適化アルゴリズムの確認 球面関数
    ''' </summary>
    <TestMethod()> Public Sub TestOptimizationSphere3D()
        Dim optimizers = clsUtil.GetOptimizersForUnitTest(New clsBenchSphere(3))
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
            If Math.Abs(opt.Result.Eval) > 0.5 Then
                Assert.Fail(String.Format("fail:{0} Eval:{1}", opt.GetType().Name, opt.Result.Eval))
            End If
            Console.WriteLine(String.Format("Success Eval {0}", opt.Result.Eval))

            'Result
            If Math.Abs(opt.Result(0)) > 0.2 OrElse Math.Abs(opt.Result(1)) > 0.2 OrElse Math.Abs(opt.Result(2)) > 0.2 Then
                Assert.Fail(String.Format("fail:{0} Result:{1} {2} {3}", opt.GetType().Name, opt.Result(0), opt.Result(1), opt.Result(2)))
            End If
            Console.WriteLine(String.Format("Success Result {0} {1} {2}", opt.Result(0), opt.Result(1), opt.Result(2)))
        Next
    End Sub

    '''' <summary>
    '''' 最適化アルゴリズムの確認 ローゼンブロック
    '''' </summary>
    '<TestMethod()> Public Sub TestOptimizationRosenblock()
    '    Dim optimizers = clsUtil.GetOptimizersForUnitTest(New clsBenchRosenblock(2))
    '    For Each opt In optimizers
    '        Console.WriteLine("Test optimize algo : {0}", opt.GetType().Name)

    '        'check init
    '        opt.InitialPosition = {1, 1}
    '        opt.Init()
    '        Dim errorFlg = opt.IsRecentError()
    '        Assert.IsFalse(errorFlg)

    '        'check iterate
    '        opt.DoIteration()
    '        errorFlg = opt.IsRecentError()
    '        Assert.IsFalse(errorFlg)

    '        'Eval
    '        If Math.Abs(opt.Result.Eval) > 5 Then
    '            Assert.Fail(String.Format("fail:{0} Eval:{1}", opt.GetType().Name, opt.Result.Eval))
    '        End If
    '        Console.WriteLine(String.Format("Success Eval {0}", opt.Result.Eval))

    '        'Result
    '        If Math.Abs(opt.Result(0) - 1.0) > 0.5 OrElse Math.Abs(opt.Result(1) - 1.0) > 0.5 Then
    '            Assert.Fail(String.Format("fail:{0} Result:{1} {2}", opt.GetType().Name, opt.Result(0), opt.Result(1)))
    '        End If
    '        Console.WriteLine(String.Format("Success Result {0} {1}", opt.Result(0), opt.Result(1)))
    '    Next
    'End Sub

    ''' <summary>
    ''' リトライ確認
    ''' </summary>
    <TestMethod()> Public Sub TestRetryCheck()
        Dim optimizers = clsUtil.GetOptimizersForUnitTest(New clsBenchSphere(2))
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

    ''' <summary>
    ''' 初期位置生成確認
    ''' </summary>
    <TestMethod()> Public Sub TestInitialPositon()
        Dim optimizers = clsUtil.GetOptimizersForUnitTest(New clsBenchSphere(2))
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

    ''' <summary>
    ''' 境界条件
    ''' </summary>
    <TestMethod()> Public Sub TestOptimizationDEWithBound()
        Dim opt As New clsOptDE(New clsBenchTest2())
        'x1-> 0 to 5, x2-> 0 to 5
        opt.LowerBounds = New Double() {0, 0}
        opt.UpperBounds = New Double() {5, 5}
        opt.Init()
        Dim errorFlg = opt.IsRecentError()
        Assert.IsFalse(errorFlg)

        'check iterate
        opt.DoIteration()
        errorFlg = opt.IsRecentError()
        Assert.IsFalse(errorFlg)

        'Eval
        If -78.99 < opt.Result.Eval AndAlso opt.Result.Eval < -78.98 Then
            'OK
        Else
            Assert.Fail(String.Format("fail Eval {0}", opt.Result.Eval))
        End If
        Console.WriteLine(String.Format("Success Eval {0}", opt.Result.Eval))

        'Result
        If 2.8 < opt.Result(0) AndAlso opt.Result(0) < 2.9 Then
            'OK
        Else
            Assert.Fail(String.Format("fail Result {0} {1}", opt.Result(0), opt.Result(1)))
        End If
        Console.WriteLine(String.Format("Success Result {0} {1}", opt.Result(0), opt.Result(1)))
    End Sub

    Public Class nothingFunc : Inherits LibOptimization.Optimization.absObjectiveFunction
        Public Overrides Function F(x As List(Of Double)) As Double
            Return clsRandomXorshiftSingleton.GetInstance().NextDouble * 10
        End Function

        Public Overrides Function Gradient(x As List(Of Double)) As List(Of Double)
            Dim aa = New List(Of Double)
            aa.Add(clsRandomXorshiftSingleton.GetInstance().NextDouble * 10)
            aa.Add(clsRandomXorshiftSingleton.GetInstance().NextDouble * 10)
            Return aa
        End Function

        Public Overrides Function Hessian(x As List(Of Double)) As List(Of List(Of Double))
            Throw New NotImplementedException()
        End Function

        Public Overrides Function NumberOfVariable() As Integer
            Return 2
        End Function
    End Class

    ''' <summary>
    ''' 回数のテスト
    ''' </summary>
    <TestMethod()> Public Sub TestOptimizationIterationCheckAll()
        Dim optimizers = clsUtil.GetOptimizersForUnitTest(New nothingFunc())
        For Each opt In optimizers
            opt.IsUseCriterion = False
            opt.InitialPosition = New Double() {100, 100}
            opt.Iteration = 2
            opt.Init()
            opt.DoIteration()
            If opt.IterationCount <> opt.Iteration Then
                Assert.Fail(String.Format("{0} : IterationCount : {1}", opt.GetType().Name, opt.IterationCount))
            Else
                Console.WriteLine("Success : {0}", opt.GetType().Name)
            End If
        Next
    End Sub

    ''' <summary>
    ''' 回数のテスト 1回ずつ
    ''' </summary>
    <TestMethod()> Public Sub TestOptimizationIterationCheck1Iteration()
        Dim optimizers = clsUtil.GetOptimizersForUnitTest(New nothingFunc())
        For Each opt In optimizers
            opt.IsUseCriterion = False
            opt.InitialPosition = New Double() {100, 100}
            opt.Iteration = 3
            opt.Init()
            While (opt.DoIteration(1) = False)
                'done
            End While
            If opt.IterationCount <> opt.Iteration Then
                Assert.Fail(String.Format("Fail! {0} : IterationCount : {1}", opt.GetType().Name, opt.IterationCount))
            Else
                Console.WriteLine("Success : {0}", opt.GetType().Name)
            End If
        Next
    End Sub

    ''' <summary>
    ''' 回数のテスト 1回ずつ
    ''' </summary>
    <TestMethod()> Public Sub TestOptimizationIterationCheck2Iteration()
        Dim optimizers = clsUtil.GetOptimizersForUnitTest(New nothingFunc())
        For Each opt In optimizers
            opt.IsUseCriterion = False
            opt.InitialPosition = New Double() {100, 100}
            opt.Iteration = 3
            opt.Init()
            While (opt.DoIteration(2) = False)
                'done
            End While
            If opt.IterationCount <> opt.Iteration Then
                Assert.Fail(String.Format("{0} : IterationCount : {1}", opt.GetType().Name, opt.IterationCount))
            Else
                Console.WriteLine("Success : {0}", opt.GetType().Name)
            End If
        Next
    End Sub

#End Region
End Class
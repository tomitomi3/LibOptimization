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

    Public Sub New()
        'fix rng
        Util.clsRandomXorshiftSingleton.GetInstance.SetDefaultSeed()
    End Sub

#Region "Optimization"
    Dim lock As Object

    Sub TestOpt(ByVal opt As absOptimization)
        SyncLock Me
            Dim EVAL As Double = 0.0001
            Try
                Console.Write("{0,-40}", opt.GetType().Name)

                'fix rng
                'opt.Random = New clsRandomXorshift()

                Dim sw As New Stopwatch()
                sw.Start()

                'check init
                opt.InitialPosition = New Double() {10, 10}
                opt.Init()
                Dim errorFlg = opt.IsRecentError()
                Assert.IsFalse(errorFlg)

                'check iterate
                opt.DoIteration()
                errorFlg = opt.IsRecentError()
                Assert.IsFalse(errorFlg)

                sw.Stop()
                Console.Write("ElapsedTime:{0}[ms] ", sw.ElapsedMilliseconds)

                'Eval
                Console.Write("Eval:{0} ", opt.Result.Eval)
                Dim isConversion = False
                For retry As Integer = 0 To 20
                    If Math.Abs(opt.Result.Eval) > EVAL Then
                        Console.Write("{0}Retry! Eval:{1}", Environment.NewLine, opt.Result.Eval)
                        opt.InitialPosition = opt.Result.ToArray()
                        opt.InitialValueRangeLower = opt.InitialValueRangeLower / 2
                        opt.InitialValueRangeUpper = opt.InitialValueRangeUpper / 2
                        opt.Init()
                        opt.DoIteration()
                    Else
                        isConversion = True
                        Exit For
                    End If
                Next
                If isConversion = False Then
                    Throw New Exception(String.Format("fail:{0} Eval:{1}", opt.GetType().Name, opt.Result.Eval))
                End If

                'Result
                If Math.Abs(opt.Result(0)) > 0.1 OrElse Math.Abs(opt.Result(1)) > 0.1 Then
                    Throw New Exception(String.Format("fail:{0} Result:{1} {2}", opt.GetType().Name, opt.Result(0), opt.Result(1)))
                End If
                Console.WriteLine(String.Format("Result:{0} {1}", opt.Result(0), opt.Result(1)))
            Catch ex As Exception
                Assert.Fail("Throw Exception! {0} {1}", opt.GetType().Name, ex.Message)
            End Try
        End SyncLock
    End Sub

    ''' <summary>
    ''' test Optimization
    ''' </summary>
    <TestMethod()> Public Sub Opt_OptimizeSphere_2D()
        For i As Integer = 2 To 3 - 1
            Console.WriteLine("===sphere {0}D===", i)
            Dim optimizers = clsUtil.GetOptimizersForUnitTest(New clsBenchSphere(i))
            Threading.Tasks.Parallel.ForEach(optimizers,
                                             Sub(opt)
                                                 TestOpt(opt)
                                             End Sub)
        Next
    End Sub

    ''' <summary>
    ''' test Retry
    ''' </summary>
    <TestMethod()> Public Sub Opt_RetryCheck()
        Dim optimizers = clsUtil.GetOptimizersForUnitTest(New clsBenchSphere(2))
        For Each opt In optimizers
            Try
                Console.WriteLine("{0,-40}", opt.GetType().Name)
                '1st
                opt.Init()
                opt.Iteration = 1
                opt.DoIteration()

                '2nd
                Dim recentCount = opt.IterationCount
                Dim recentEval = opt.Result.Eval
                opt.InitialPosition = opt.Result.ToArray()
                opt.Iteration = 100
                opt.Init()

                'check iteration count
                If opt.IterationCount <> 0 Then
                    Assert.Fail("fail {0} : 2nd try init iteration count", opt.GetType().Name)
                End If

                'do optimize
                opt.DoIteration()
                If opt.IterationCount = 0 Then
                    Assert.Fail("fail {0} : 2nd try DoIteration", opt.GetType().Name)
                End If
            Catch ex As Exception
                Assert.Fail("Throw Exception! {0} {1}", opt.GetType().Name, ex.Message)
            End Try
        Next
    End Sub

    ''' <summary>
    ''' test initial position range
    ''' </summary>
    <TestMethod()> Public Sub Opt_InitialPositon()
        Dim optimizers = clsUtil.GetOptimizersForUnitTest(New clsBenchSphere(2))
        For Each opt In optimizers
            Try
                Console.WriteLine("Test optimize algo : {0}", opt.GetType().Name)

                'init
                Dim range = 100.0
                opt.InitialPosition = {range, range}
                opt.InitialValueRangeLower = -range / 10
                opt.InitialValueRangeUpper = range / 10
                opt.Init()

                'initial position check
                Dim results = opt.Results.ToArray()
                Dim resultMat As clsEasyMatrix = clsUtil.ToConvertMat(opt.Results)
                Dim g As New clsEasyVector()
                For i As Integer = 0 To results(0).Count - 1
                    Dim ar = resultMat.Column(i)
                    Dim tempVal = clsUtil.Average(ar)
                    g.Add(clsUtil.Average(resultMat.Column(i)))
                Next
                g.PrintValue(name:="average initial position")

                If Math.Abs(range - g(0)) > 10 OrElse Math.Abs(range - g(1)) > 10 Then
                    Assert.Fail(String.Format("fail {0}", opt.GetType().Name))
                End If
            Catch ex As Exception
                Assert.Fail("Throw {0} object", opt.GetType().Name)
            End Try
        Next
    End Sub

    ''' <summary>
    ''' bound check
    ''' </summary>
    <TestMethod()> Public Sub Opt_OptimizationDEWithBound()
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
    ''' test Iterarion count
    ''' </summary>
    <TestMethod()> Public Sub Opt_IterationCount()
        Dim optimizers = clsUtil.GetOptimizersForUnitTest(New nothingFunc())

        'all iteration
        With Nothing
            Console.WriteLine("=all=")
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
        End With

        'per 1
        With Nothing
            Console.WriteLine("=per 1=")
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
        End With

        'per 2
        With Nothing
            Console.WriteLine("=per 2=")
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
        End With
    End Sub

    ''' <summary>
    ''' test serialize and deserialize
    ''' </summary>
    <TestMethod()> Public Sub Opt_SerializeAndDesrialize()
        Dim opt = New LibOptimization.Optimization.clsOptCS(New clsBenchSphere(5))
        opt.Random = New clsRandomXorshift()
        opt.Init()

        'serialize
        opt.DoIteration(10)
        Util.clsRandomXorshiftSingleton.GetInstance.SetDefaultSeed()
        Dim PATH_SERIALIZE = "serialize_file.txt"
        clsUtil.SerializeOpt(CType(opt, absOptimization), PATH_SERIALIZE)
        opt.DoIteration(10)
        Dim result1 = opt.Result()
        Dim itr1 = opt.IterationCount

        'deserialize
        Dim temp = clsUtil.DeSerializeOpt(PATH_SERIALIZE)
        opt = CType(temp, clsOptCS)
        Util.clsRandomXorshiftSingleton.GetInstance.SetDefaultSeed()
        opt.DoIteration(10)
        Dim result2 = opt.Result()
        Dim itr2 = opt.IterationCount

        'compare
        Dim flg = True

        flg = flg And (itr1 = itr2)
        If flg = False Then
            Assert.Fail(String.Format("not same iteration count"))
        End If

        flg = flg And clsMathUtil.IsNearyEqualVector(result1, result2)
        If flg = False Then
            Assert.Fail(String.Format("not same result"))
        End If
    End Sub
#End Region
End Class
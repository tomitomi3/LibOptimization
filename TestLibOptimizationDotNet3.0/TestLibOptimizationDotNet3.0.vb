Imports System.Text
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Optimization
Imports LibOptimization.Util
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class TestLibOptimizationDotNet30
    ''' <summary>
    ''' test Optimization
    ''' </summary>
    <TestMethod()> Public Sub Opt_OptimizeSphereDotNet30()
        Dim EVAL As Double = 0.0001
        For i As Integer = 2 To 3
            Console.WriteLine("===sphere {0}D===", i)
            Dim optimizers = clsUtil.GetOptimizersForUnitTest(New clsBenchSphere(i))
            For Each opt In optimizers
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
                    For retry As Integer = 0 To 5
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
            Next
        Next
    End Sub
End Class
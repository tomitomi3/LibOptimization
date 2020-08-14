Imports System.Text
Imports LibOptimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.MathUtil
Imports LibOptimization.Optimization
Imports LibOptimization.Util
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class TestLibOptimizationDotNet35
    ''' <summary>
    ''' test Optimization net35
    ''' </summary>
    <TestMethod()> Public Sub Opt_OptimizeSphere_2D_NET35()
        Dim EVAL As Double = 0.0001
        For i As Integer = 2 To 3 - 1
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
            Next
        Next
    End Sub

    ''' <summary>
    ''' Matrix x Vector
    ''' </summary>
    <TestMethod()> Public Sub Mat_Product_MatrixVectorNet35()
        '----------------
        'bad
        '----------------
        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 1, 0},
                                                  New Double() {0, 0, 1}})
            Dim v As New clsEasyVector(New Double() {2, 2}, clsEasyVector.VectorDirection.ROW)
            Try
                Dim temp = mat * v
            Catch myex As clsException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        'bad
        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0},
                                                  New Double() {0, 1},
                                                  New Double() {0, 0}})
            Dim v As New clsEasyVector(New Double() {2, 2, 2}, clsEasyVector.VectorDirection.ROW)
            Try
                Dim temp = mat * v
            Catch myex As clsException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        'bad
        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 0, 1}})
            Dim v As New clsEasyVector(New Double() {2, 2, 2}, clsEasyVector.VectorDirection.ROW)
            Try
                Dim temp = mat * v
            Catch myex As clsException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        '----------------
        'OK
        '----------------
        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 1, 0},
                                                  New Double() {0, 0, 1}})
            Dim v As New clsEasyVector(New Double() {2, 2, 2}, clsEasyVector.VectorDirection.COL)
            Try
                mat.PrintValue()
                Dim temp = mat * v

                'size check
                If temp.Count <> mat.RowCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction = clsEasyVector.VectorDirection.ROW Then
                    Assert.Fail()
                End If

                'value check
                For Each val As Double In temp
                    If val <> 2.0 Then
                        Assert.Fail()
                    End If
                Next
            Catch ex As Exception
                Assert.Fail()
            End Try
        End With

        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 2, 3}})
            Dim v As New clsEasyVector(New Double() {2, 2, 2}, clsEasyVector.VectorDirection.COL)
            Try
                Dim temp = mat * v

                'size check
                If temp.Count <> mat.RowCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction = clsEasyVector.VectorDirection.ROW Then
                    Assert.Fail()
                End If

                'value check
                For Each val As Double In temp
                    If val <> 12 Then
                        Assert.Fail()
                    End If
                Next
            Catch ex As Exception
                Assert.Fail()
            End Try
        End With

        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0},
                                                  New Double() {1, 0},
                                                  New Double() {1, 0}})
            Dim v As New clsEasyVector(New Double() {2, 2}, clsEasyVector.VectorDirection.COL)
            Try
                Dim temp = mat * v

                'size check
                If temp.Count <> mat.RowCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction = clsEasyVector.VectorDirection.ROW Then
                    Assert.Fail()
                End If

                'value check
                For Each val As Double In temp
                    If val <> 2.0 Then
                        Assert.Fail()
                    End If
                Next
            Catch ex As Exception
                Assert.Fail()
            End Try
        End With
    End Sub

    ''' <summary>
    ''' test Matrix x Matrix
    ''' </summary>
    <TestMethod()> Public Sub Mat_Product_MatrixMatrixNet35()
        For dimNum = 2 To 10 - 1
            With Nothing
                Dim matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum)
                Dim matB = New clsEasyMatrix(dimNum, True)
                Try
                    Dim temp = matA * matB
                    temp.PrintValue(name:="A*B")

                    If clsMathUtil.IsNearyEqualMatrix(temp, matA) = False Then
                        Assert.Fail()
                    End If
                Catch myex As clsException
                    Assert.Fail()
                End Try
            End With

            With Nothing
                Dim matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum)
                Dim matB = New clsEasyMatrix(dimNum, False)
                Try
                    Dim temp = matA * matB
                    temp.PrintValue(name:="B(zero)*A")

                    If clsMathUtil.IsNearyEqualMatrix(temp, matB) = False Then
                        Assert.Fail()
                    End If
                Catch myex As clsException
                    Assert.Fail()
                End Try
            End With
        Next
    End Sub
End Class
Imports LibOptimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.MathUtil
Imports LibOptimization.Optimization
Imports LibOptimization.Util

Imports Dbl = System.Double

Module Module1
    Sub Main()

        With Nothing
            'Dim mat As New clsEasyMatrix(New Double()() {
            '                             New Double() {3, 0, 0},
            '                             New Double() {-2, -2, 4},
            '                             New Double() {0, -1, 3}})
            'Dim mat As New clsEasyMatrix(New Double()() {
            '                             New Double() {2, 1, 0},
            '                             New Double() {1, 2, 1},
            '                             New Double() {1, 5, 3}})
            Dim mat As New clsEasyMatrix(New Double()() {
                                         New Double() {16, -1, 1, 2},
                                         New Double() {2, 12, 1, -1},
                                         New Double() {1, 3, -24, 2},
                                         New Double() {4, -2, 1, 20}})
            Dim eigen = mat.Eigen2()

            'eigen
            Dim retV = eigen.EigenValue
            Dim retM = eigen.EigenVector
            Dim retD = retV.ToDiagonalMatrix()

            retV.PrintValue(ai_preci:=4)
            retM.PrintValue(ai_preci:=4)

            'Ax=ramudax
            'check
            '固有ベクトルの転置と固有ベクトルの逆行列は同じ＝直交
            Dim matI = retM * retM.T()
        End With

        With Nothing
            Dim mat As New clsEasyMatrix(New Double()() {
                                         New Double() {2, 1},
                                         New Double() {1, 2}})
            Dim eigen = mat.Eigen2()

            'eigen
            Dim retV = eigen.EigenValue
            Dim retM = eigen.EigenVector
            Dim retD = retV.ToDiagonalMatrix()

            retV.PrintValue(ai_preci:=4)
            retM.PrintValue(ai_preci:=4)

            'Ax=ramudax

            'check
            '固有ベクトルの転置と固有ベクトルの逆行列は同じ＝直交
            Dim matI = retM * retM.T()
            Return
        End With


        With Nothing
            Dim s0 As New clsEasyMatrix(New Double()() {New Double() {1, 2, 1, 2, 1, 2}})
            Dim s1 As New clsEasyMatrix(New Double()() {New Double() {1, 2, 1, 2, 1, 2},
                                                   New Double() {2, 4, 2, 4, 2, 4}})
            Dim s2 As New clsEasyMatrix(New Double()() {New Double() {1, 2, 1, 2, 1, 2},
                                                   New Double() {2, 4, 2, 4, 2, 4},
                                                   New Double() {4, 8, 4, 8, 4, 8}})
            Dim s3 As New clsEasyMatrix(New Double()() {New Double() {50, 50, 80, 70, 90},
                                                   New Double() {50, 70, 60, 90, 100}})

            Dim rng = New LibOptimization.Util.clsRandomXorshift()
            For i = 0 To 2000 - 1
                Dim dimNum = 3
                Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng)

                Dim resultLU = source.LUP()
                Dim P = resultLU.P
                Dim L = resultLU.L
                Dim U = resultLU.U

                'check
                If clsMathUtil.IsNearyEqualMatrix(P * source, L * U) = True Then
                    'OK
                Else
                    Console.WriteLine("No={0} det={1}", i, resultLU.Det)
                    source.PrintValue(name:="souce")
                    P.PrintValue(name:="P")
                    L.PrintValue(name:="L")
                    U.PrintValue(name:="U")
                    CType(P * L * U, clsEasyMatrix).PrintValue(name:="LUP")
                End If
            Next
        End With

        With Nothing
            Dim rng = New LibOptimization.Util.clsRandomXorshift()
            For i = 0 To 2000 - 1
                Dim dimNum = 4
                Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng)
                Try
                    Dim matLUP = source.LUP()
                    Dim colVec = source.Inverse().Column(0)
                    Dim result = matLUP.Solve((New clsEasyMatrix(dimNum, True))(0))

                    'check
                    If clsMathUtil.IsNearyEqualVector(colVec, result) = True Then
                        'OK
                    Else
                        colVec.PrintValue()
                        result.PrintValue()
                        source.PrintValue(name:="Source vector")
                    End If
                Catch ex As Exception
                    source.PrintValue(name:="Source matrix")
                End Try
            Next
        End With
        Return

        'Dim v = New Vector(Of Double)
        'Dim c = Vector(Of Double).Count
        'Dim retV = System.Numerics.Vector.Abs(v)

        'Console.WriteLine("{0} {1}", Vector.IsHardwareAccelerated, Vector(Of Double).Count)

        'With Nothing
        '    Dim answerTemp = New Vector(Of Double)(New Double() {1, 1, 1, 1})
        '    Dim multiTemp = New Vector(Of Double)(New Double() {1.0, 2.0, 3.0, 4.0})
        '    answerTemp = answerTemp + multiTemp
        '    Console.WriteLine(answerTemp.ToString())
        '    Dim answer = answerTemp(0) * answerTemp(1) * answerTemp(2) * answerTemp(3)
        '    Console.WriteLine(answer)
        'End With

        'For i = 0 To 1000000 - 1
        '    Dim answerTemp = New Vector(Of Double)(New Double() {1, 1, 1, 1})
        '    Dim multiTemp = New Vector(Of Double)(New Double() {1.0, 2.0, 3.0, 4.0})
        '    Dim loopEnd = 1000000 / Vector(Of Double).Count
        '    For j = 0 To loopEnd - 1
        '        multiTemp += Vector(Of Double).One
        '        answerTemp *= multiTemp

        '    Next
        '    Dim answer = answerTemp(0) * answerTemp(1) * answerTemp(2) * answerTemp(3)
        'Next

        Return
        With Nothing
            Dim dimNum = 5
            Dim source = clsMathUtil.CreateRandomASymmetricMatrix(dimNum)
            Try
                Dim hoge = source.HouseholderTransformation()
                hoge.PrintValue()
                hoge = source.HouseholderTransformationForQR()
                hoge.PrintValue()

                Dim ee = source.Eigen()
                ee.EigenValue.PrintValue()
                ee.EigenVector.PrintValue()

                Dim e = hoge.Eigen()
                e.EigenValue.PrintValue()
                e.EigenVector.PrintValue()

                Return
            Catch ex As Exception
                source.PrintValue(name:="Source matrix")
            End Try
        End With

    End Sub

    ''' <summary>
    ''' Optimization Benchmark
    ''' </summary>
    Public Class clsBenchmark
        Private _result As New Dictionary(Of KeyValuePair(Of String, Integer), List(Of clsPoint))

        Public Sub Add(name As String, iteration As Integer, result As clsPoint)
            'gen key
            Dim key = New KeyValuePair(Of String, Integer)(name, iteration)
            If _result.TryGetValue(key, Nothing) = False Then
                _result(key) = New List(Of clsPoint)
            End If
            _result(key).Add(result.Copy)
        End Sub

        Public Sub DisplayStatic()
            If _result.Count = 0 Then
                Return
            End If

            Dim calcResult As New Dictionary(Of Integer, Dictionary(Of String, List(Of Double)))

            For Each kv As KeyValuePair(Of KeyValuePair(Of String, Integer), List(Of clsPoint)) In _result
                Dim min As Double = 0.0
                Dim max As Double = 0.0
                Dim ave As Double = 0.0
                Dim var As Double = 0.0
                Dim evals As New List(Of Double)
                For Each p In kv.Value
                    evals.Add(p.Eval)
                    var += p.Eval ^ 2
                Next
                min = evals.Min()
                max = evals.Max()
                ave = evals.Average
                var = var / kv.Value.Count

                '結果の格納ー
                Dim iteration = kv.Key.Value
                If calcResult.TryGetValue(iteration, Nothing) = False Then
                    calcResult.Add(iteration, New Dictionary(Of String, List(Of Double)))
                End If
                Dim optName = kv.Key.Key
                If calcResult(iteration).TryGetValue(optName, Nothing) = False Then
                    calcResult(iteration).Add(optName, New List(Of Double))
                End If

                '後で編集用
                calcResult(iteration)(optName).Add(ave)
                calcResult(iteration)(optName).Add(min)
                calcResult(iteration)(optName).Add(max)
                calcResult(iteration)(optName).Add(var)
            Next

            'まとめ出力
            Dim count As Integer = 0
            Dim once As Boolean = False
            Dim outputTotal() As String = Nothing
            For Each tempResult In calcResult
                '名前だけ
                If once = False Then
                    '数える
                    For Each temp In tempResult.Value
                        count = temp.Value.Count
                        Exit For
                    Next
                    '確保
                    ReDim outputTotal(count - 1)
                    '関数名取得
                    Dim outputFuncName As String = String.Empty
                    For Each temp In tempResult.Value
                        outputFuncName = outputFuncName + String.Format(",{0}", temp.Key)
                    Next
                    outputFuncName = outputFuncName + Environment.NewLine

                    For i = 0 To outputTotal.Length - 1
                        outputTotal(i) = outputFuncName
                    Next
                    once = True
                End If

                '結果
                For i = 0 To count - 1
                    Dim outputResult As String = String.Empty
                    Dim once2 As Boolean = False
                    For Each temp In tempResult.Value
                        If once2 = False Then
                            outputResult = outputResult + String.Format("{0},", tempResult.Key)
                            once2 = True
                        End If
                        outputResult = outputResult + String.Format("{0},", temp.Value(i))
                    Next
                    outputResult += Environment.NewLine
                    outputTotal(i) += outputResult
                Next
            Next

            'display
            For Each temp In outputTotal
                Console.WriteLine(temp)
            Next
        End Sub

        Public Sub DoMeasument(ByVal opt As absOptimization, Optional ByVal loopCount As Integer = 10, Optional ByVal msg As String = "")
            For i As Integer = 0 To loopCount - 1
                '初期化
                opt.Init()

                '指定イテレーションまで回す
                For Each iteration In {1, 10, 50, 100, 500, 1000, 5000, 10000}
                    opt.Iteration = iteration
                    opt.DoIteration()
                    Me.Add(opt.GetType.Name & "_" & opt.Memo, iteration, opt.Result)
                Next
            Next
        End Sub

        Public Sub Clear()
            _result.Clear()
        End Sub
    End Class

    ''' <summary>
    ''' LU decompostion benchmark
    ''' </summary>
    Public Sub BenchmarkLU()
        With Nothing
            Dim rng1 As clsRandomXorshift = Nothing
            Dim rng2 As clsRandomXorshift = Nothing
            'bench mark
            System.Threading.Thread.Sleep(1000)

            Dim loopNum = 5
            Dim trynum = 2000
            For kk = 0 To 10 - 1
                With Nothing
                    rng2 = New LibOptimization.Util.clsRandomXorshift()
                    Dim plu2 = 0.0
                    For j = 0 To loopNum - 1
                        Dim sw = New Stopwatch()
                        sw.Start()
                        For i = 0 To trynum - 1
                            Dim dimNum = 4
                            Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng2)
                            Dim resultLU As LU = Nothing
                            Try
                                resultLU = source.LUP()
                                Dim P = resultLU.P
                                Dim L = resultLU.L
                                Dim U = resultLU.U
                                Dim flg = clsMathUtil.IsNearyEqualMatrix(P * L * U, source)
                                If flg = False Then
                                    Return
                                End If
                            Catch ex As Exception
                                source.PrintValue()
                                Return
                            End Try
                        Next
                        sw.Stop()
                        plu2 += sw.ElapsedMilliseconds
                    Next
                    Console.WriteLine("{0}s CALGO", plu2 / 1000.0)
                End With
                With Nothing
                    Dim plu1 = 0.0
                    rng1 = New LibOptimization.Util.clsRandomXorshift()
                    For j = 0 To loopNum - 1
                        Dim sw = New Stopwatch()
                        sw.Start()
                        For i = 0 To trynum - 1
                            Dim dimNum = 4
                            Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng1)
                            Dim resultLU As LU = Nothing
                            Try
                                resultLU = source.LUP()
                                Dim P = resultLU.P
                                Dim L = resultLU.L
                                Dim U = resultLU.U
                                Dim flg = clsMathUtil.IsNearyEqualMatrix(P * L * U, source)
                                If flg = False Then
                                    Return
                                End If
                            Catch ex As Exception
                                source.PrintValue()
                                Return
                            End Try
                        Next
                        sw.Stop()
                        plu1 += sw.ElapsedMilliseconds
                    Next
                    Console.WriteLine("{0}s NR", plu1 / 1000.0)
                End With
            Next
            Console.ReadKey()
            Return
        End With
    End Sub

End Module

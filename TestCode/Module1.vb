Imports LibOptimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.MathUtil
Imports LibOptimization.Optimization
Imports LibOptimization.Util

Imports Dbl = System.Double

Module Module1
    Sub Main()
        With Nothing
            Dim opt = New LibOptimization.Optimization.clsOptNelderMeadANMS(New LibOptimization.BenchmarkFunction.clsBenchRosenblock(20))
            opt.Iteration = 20000
            opt.Init()
            clsUtil.DebugValue(opt)
            opt.DoIteration()
            clsUtil.DebugValue(opt)
            Return
        End With
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
            'Dim mat As New clsEasyMatrix(New Double()() {
            '                             New Double() {2, 1},
            '                             New Double() {1, 2}})
            mat = clsMathUtil.CreateRandomSymmetricMatrix(2)
            mat.PrintValue(4, "Source")

            'eigen
            Dim eigen = mat.Eigen3()
            Dim retV = eigen.EigenValue
            Dim retM = eigen.EigenVector
            retV.PrintValue(4, "EigenValue")
            retM.PrintValue(4, "EigenVector")
            Dim matI = retM * retM.T()
            matI.PrintValue(ai_preci:=4)
            Dim prodSource = retM * retV.ToDiagonalMatrix() * retM.T
            prodSource.PrintValue(ai_preci:=4)

            'jacobi
            eigen = mat.Eigen()
            retV = eigen.EigenValue
            retM = eigen.EigenVector
            retV.PrintValue(4, "EigenValue")
            retM.PrintValue(4, "EigenVector")
            matI = retM * retM.T()
            matI.PrintValue(ai_preci:=4)
            prodSource = retM * retV.ToDiagonalMatrix() * retM.T
            prodSource.PrintValue(ai_preci:=4)
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
    End Sub
End Module

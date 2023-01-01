Imports LibOptimization.MathTool
Imports LibOptimization.MathTool.RNG
Imports LibOptimization.Optimization
Imports LibOptimization.Util

Module Module1
    Sub Main()
        'benchmark
        With Nothing
            Dim rng = New Random(System.DateTime.Now.Millisecond)
            Dim sw As New Stopwatch()
            Dim eigen_tred2 As New List(Of Double)
            Dim eigen_jacobi As New List(Of Double)
            Dim MAX_DIM = 100
            For l = 0 To 10 - 1
                Console.WriteLine("{0} times", l)
                sw.Start()
                MathUtil.CreateRandomSymmetricMatrix(MAX_DIM, rng:=rng).Eigen()
                'For i As Integer = 2 To MAX_DIM - 1
                '    Dim srcMat = MathUtil.CreateRandomSymmetricMatrix(i, rng:=rng)
                '    Dim eigen = srcMat.Eigen()
                'Next
                sw.Stop()
                Console.WriteLine("using 三重対角化->QL. elapsed time {0} ms", sw.ElapsedMilliseconds)
                eigen_tred2.Add(sw.ElapsedMilliseconds)
                sw.Reset()

                sw.Start()
                MathUtil.CreateRandomSymmetricMatrix(MAX_DIM, rng:=rng).EigenJacobi()
                'For i As Integer = 2 To MAX_DIM - 1
                '    Dim srcMat = MathUtil.CreateRandomSymmetricMatrix(i, rng:=rng)
                '    Dim eigen = srcMat.EigenJacobi()
                'Next
                sw.Stop()
                Console.WriteLine("using Jacobi. elapsed time {0} ms", sw.ElapsedMilliseconds)
                eigen_jacobi.Add(sw.ElapsedMilliseconds)
                sw.Reset()
                Console.WriteLine("")
            Next

            Console.WriteLine("Compute eigenvalue and vector of {0}dim real symmetric matrixnd vector.", MAX_DIM)
            Console.WriteLine("10times average")
            Console.WriteLine("三重対角化->QL    elapsed time {0} ms", eigen_tred2.Average())
            Console.WriteLine("Jacobi            elapsed time {0} ms", eigen_jacobi.Average())
            Return
        End With

        With Nothing
            Dim opt = New LibOptimization.Optimization.clsOptNelderMeadANMS(New LibOptimization.BenchmarkFunction.clsBenchRosenblock(20))
            opt.Iteration = 20000
            opt.Init()
            clsUtil.DebugValue(opt)
            opt.DoIteration()
            clsUtil.DebugValue(opt)
            Return
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
        Dim rng1 As RandomXorshift = Nothing
        Dim rng2 As RandomXorshift = Nothing
        'bench mark
        System.Threading.Thread.Sleep(1000)

        Dim loopNum = 5
        Dim trynum = 2000
        For kk = 0 To 10 - 1
            With Nothing
                rng2 = New RandomXorshift()
                Dim plu2 = 0.0
                For j = 0 To loopNum - 1
                    Dim sw = New Stopwatch()
                    sw.Start()
                    For i = 0 To trynum - 1
                        Dim dimNum = 4
                        Dim source = MathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng2)
                        Dim resultLU As LU = Nothing
                        Try
                            resultLU = source.LUP()
                            Dim P = resultLU.P
                            Dim L = resultLU.L
                            Dim U = resultLU.U
                            Dim flg = MathUtil.IsNearyEqualMatrix(P * L * U, source)
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
                rng1 = New RandomXorshift()
                For j = 0 To loopNum - 1
                    Dim sw = New Stopwatch()
                    sw.Start()
                    For i = 0 To trynum - 1
                        Dim dimNum = 4
                        Dim source = MathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng1)
                        Dim resultLU As LU = Nothing
                        Try
                            resultLU = source.LUP()
                            Dim P = resultLU.P
                            Dim L = resultLU.L
                            Dim U = resultLU.U
                            Dim flg = MathUtil.IsNearyEqualMatrix(P * L * U, source)
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

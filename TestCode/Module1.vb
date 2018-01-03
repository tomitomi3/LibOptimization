Imports LibOptimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Optimization
Imports LibOptimization.Util

Module Module1

    ''' <summary>
    ''' Benchmark
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

    Sub Main()
        With Nothing
            Dim func As absObjectiveFunction = New clsBenchSphere(10)

            Dim aaa = 10
            Dim optimizers As New List(Of absOptimization)
            optimizers.Add(New clsOptCS(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptDE(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptDEJADE(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptES(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptFA(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptNelderMead(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptNelderMeadWiki(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptPatternSearch(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptPSO(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptPSOAIW(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptPSOChaoticIW(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptPSOLDIW(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptRealGABLX(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptRealGAPCX(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptRealGAREX(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptRealGASPX(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptRealGAUNDX(New clsBenchSphere(aaa)))
            optimizers.Add(New clsOptSimulatedAnnealing(New clsBenchSphere(aaa)))
            'optimizers.Add(New clsOptSteepestDescent(New clsBenchSphere(aaa)))
            'optimizers.Add(New clsOptNewtonMethod(New clsBenchSphere(aaa)))

            Dim sw As New Stopwatch()
            sw.Start()

            Dim statistics As New clsBenchmark()
            For Each opt In optimizers
                Console.WriteLine("Optimization:{0}", opt.GetType().Name())
                opt.InitialPosition = {10, 10}
                statistics.DoMeasument(opt)
            Next
            Console.WriteLine()

            '結果の表示ze
            statistics.DisplayStatic()

            sw.Stop()
            Console.WriteLine()
            Console.WriteLine("Calc time:{0}[s]", sw.ElapsedMilliseconds / 1000)
            'Console.ReadLine()
            Return
        End With

    End Sub

End Module

Imports LibOptimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.MathUtil
Imports LibOptimization.Optimization
Imports LibOptimization.Util

Module Module1
    Sub Main()
        Dim rng1 As clsRandomXorshift = Nothing
        Dim rng2 As clsRandomXorshift = Nothing

        With Nothing
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
                                resultLU = source.PLU()
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
                                resultLU = source.PLU()
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

            'test
            For i = 0 To 10000 - 1
                Dim dimNum = 4
                Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=New clsRandomXorshift())

                '下記だと特異行列になる
                'source(0)(0) = 0.0
                'source(2)(2) = 0.0

                Dim resultLU As LU = Nothing
                Try
                    resultLU = source.PLU_CALGO()
                Catch ex As Exception
                    source.PrintValue()
                    Return
                End Try

                Dim d = source.Det
                Dim P = resultLU.P
                Dim L = resultLU.L
                Dim U = resultLU.U
                'source.PrintValue(name:="souce")
                'P.PrintValue(name:="P")
                'L.PrintValue(name:="L")
                'U.PrintValue(name:="U")
                'Dim plt = P * L * U
                'plt.PrintValue()

                'check
                Dim flg = clsMathUtil.IsNearyEqualMatrix(P * L * U, source)
                If flg = False Then
                    Console.WriteLine("No={0} det={1}", i, resultLU.Det)
                    P.PrintValue(name:="P")
                    P.T().PrintValue(name:="P^T")
                    resultLU = source.PLU()
                End If
                If resultLU.Det = 0.0 Then
                    Console.WriteLine("No={0} det={1}", i, resultLU.Det)
                End If
            Next
            Return
        End With

    End Sub

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

    Public Class clsOptCMAES
        Inherits absOptimization

        Public Overrides Property Iteration As Integer
            Get
                Throw New NotImplementedException()
            End Get
            Set(value As Integer)
                Throw New NotImplementedException()
            End Set
        End Property

        Public Overrides ReadOnly Property Result As clsPoint
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides ReadOnly Property Results As List(Of clsPoint)
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        ''' <summary>平均ベクトル</summary>
        Private _mean As clsEasyVector = Nothing

        ''' <summary>sigma</summary>
        Private _sigma As Double = 0.0

        ''' <summary>共分散行列</summary>
        Private _covarianceMat As clsEasyMatrix = Nothing

        ''' <summary>進化パスpc</summary>
        Private _pc As clsEasyVector = Nothing

        ''' <summary>進化パスpsigma</summary>
        Private _psigma As clsEasyVector = Nothing

        Public Overrides Sub Init()
            'init
            Dim n = m_func.NumberOfVariable
            _mean = New clsEasyVector(n)
            _sigma = 0.0
            _covarianceMat = New clsEasyMatrix(n, True)
            _pc = New clsEasyVector(n)
            _psigma = New clsEasyVector(n)

            'N(0,I)
            Dim z As New clsEasyVector(n)
            z.Direction = clsEasyVector.VectorDirection.COL '列ベクトル
            For i As Integer = 0 To n - 1
                z(i) = clsUtil.NormRand()
            Next

            Dim temp = _mean + _covarianceMat.Cholesky() * z

        End Sub

        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
            Throw New NotImplementedException()
        End Function

        Public Overrides Function IsRecentError() As Boolean
            Throw New NotImplementedException()
        End Function
    End Class

End Module

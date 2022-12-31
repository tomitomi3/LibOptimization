Namespace MathTool
    ''' <summary>
    ''' Utility class for Math
    ''' </summary>
    Public Class MathUtil
        ''' <summary>
        ''' for sort
        ''' </summary>
        Private Class ValueDescSort
            Implements IComparable

            Public v As Double = 0.0

            Public idx As Integer = 0

            Public Sub New(ByVal v As Double, ByVal idx As Integer)
                Me.v = v
                Me.idx = idx
            End Sub

            Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
                'Nothing check
                If obj Is Nothing Then
                    Return 1
                End If

                'Type check
                If Not Me.GetType() Is obj.GetType() Then
                    Throw New ArgumentException("Different type", "obj")
                End If

                'Compare descent sort
                Dim mineValue As Double = Me.v
                Dim compareValue As Double = DirectCast(obj, ValueDescSort).v
                If mineValue < compareValue Then
                    Return 1
                ElseIf mineValue > compareValue Then
                    Return -1
                Else
                    Return 0
                End If
            End Function
        End Class

        ''' <summary>
        ''' Calculate the variance(sample variance)
        ''' </summary>
        ''' <param name="v"></param>
        ''' <returns></returns>
        Public Shared Function Var(ByVal v As DenseVector) As Double
            Return (v.SquareSum() / v.Count) - Math.Pow(v.Average(), 2)
        End Function

        ''' <summary>
        ''' Calculate the covariance(sample variance) from two vectors.
        ''' </summary>
        ''' <param name="v1">vector a</param>
        ''' <param name="v2">vector b</param>
        ''' <returns></returns>
        Public Shared Function CoVar(ByVal v1 As DenseVector, ByVal v2 As DenseVector) As Double
            Dim ave_xy = v1.InnerProduct(v2) / v1.Count
            Return ave_xy - v1.Average() * v2.Average()
        End Function

        ''' <summary>
        ''' Calculate the standard deviation
        ''' </summary>
        ''' <param name="v">vector</param>
        ''' <returns></returns>
        Public Shared Function Stddev(ByVal v As DenseVector) As Double
            Return Math.Sqrt(Var(v))
        End Function

        ''' <summary>
        ''' Calculate the correlation
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        Public Shared Function Cor(ByVal v1 As DenseVector, ByVal v2 As DenseVector) As Double
            Return CoVar(v1, v2) / (Stddev(v1) * Stddev(v2))
        End Function

        ''' <summary>
        ''' Create random symmetric matrix(for Debug) 対象行列を生成
        ''' </summary>
        ''' <param name="size"></param>
        ''' <param name="rng"></param>
        ''' <returns></returns>
        Public Shared Function CreateRandomSymmetricMatrix(ByVal size As Integer,
                                                               Optional ByVal rng As System.Random = Nothing,
                                                               Optional ByVal isIncludeZero As Boolean = False,
                                                               Optional ByVal isFloating As Boolean = False,
                                                               Optional ByVal lower As Double = -10,
                                                               Optional ByVal upper As Double = 10) As DenseMatrix
            If rng Is Nothing Then
                rng = New System.Random()
            End If
            Dim matTemp = New DenseMatrix(size)
            For i As Integer = 0 To matTemp.Count - 1
                For j As Integer = 1 + i To matTemp.Count - 1
                    Dim r As Double = 0.0
                    If isFloating = False Then
                        r = rng.Next(CInt(lower), CInt(upper))
                    Else
                        r = System.Math.Abs(upper - lower) * rng.NextDouble() + lower
                    End If

                    If isIncludeZero = True Then
                        matTemp(i)(j) = r
                        matTemp(j)(i) = r
                    Else
                        If r = 0 Then
                            If (rng.Next Mod 2) = 0 Then
                                r = 2
                            Else
                                r = 3
                            End If
                        End If
                        matTemp(i)(j) = r
                        matTemp(j)(i) = r
                    End If
                Next

                If isFloating = False Then
                    matTemp(i)(i) = rng.Next(CInt(lower), CInt(upper))
                Else
                    matTemp(i)(i) = System.Math.Abs(upper - lower) * rng.NextDouble() + lower
                End If
            Next

            Return matTemp
        End Function

        ''' <summary>
        ''' Create random Asymmetric matrix(for Debug) 非対象行列を生成
        ''' </summary>
        ''' <param name="size"></param>
        ''' <returns></returns>
        Public Shared Function CreateRandomASymmetricMatrix(ByVal size As Integer,
                                                               Optional ByVal rng As System.Random = Nothing,
                                                               Optional ByVal isIncludeZero As Boolean = True,
                                                               Optional ByVal isFloating As Boolean = False,
                                                               Optional ByVal lower As Double = -10,
                                                               Optional ByVal upper As Double = 10) As DenseMatrix
            If rng Is Nothing Then
                'rng = New Util.clsRandomXorshift()
                rng = New System.Random()
            End If
            Dim matTemp = New DenseMatrix(size)
            For i As Integer = 0 To matTemp.Count - 1
                For j As Integer = 0 To matTemp.Count - 1
                    Dim r As Double = 0.0
                    If isFloating = False Then
                        r = rng.Next(CInt(lower), CInt(upper))
                    Else
                        r = System.Math.Abs(upper - lower) * rng.NextDouble() + lower
                    End If

                    If isIncludeZero = True Then
                        matTemp(i)(j) = r
                    Else
                        If r = 0 Then
                            If (rng.Next Mod 2) = 0 Then
                                r = 2
                            Else
                                r = 3
                            End If
                        End If
                        matTemp(i)(j) = r
                    End If
                Next
            Next

            Return matTemp
        End Function

        ''' <summary>
        ''' check eaual matrix(for debug)
        ''' </summary>
        ''' <param name="matA"></param>
        ''' <param name="matB"></param>
        ''' <param name="eps">default:1E-8</param>
        ''' <returns></returns>
        Public Shared Function IsNearyEqualMatrix(ByVal matA As DenseMatrix, ByVal matB As DenseMatrix,
                                                      Optional ByVal eps As Double = 0.00000001) As Boolean
            Try
                For i As Integer = 0 To matA.RowCount - 1
                    For j As Integer = 0 To matA.ColCount - 1
                        Dim tempValA = matA(i)(j)
                        Dim tempValB = matB(i)(j)
                        If MathUtil.IsCloseToValues(tempValA, tempValB, eps) = False Then
                            Return False
                        End If
                    Next
                Next
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' <summary>
        ''' check eaual vector(for debug)
        ''' </summary>
        ''' <param name="vecA"></param>
        ''' <param name="vecB"></param>
        ''' <param name="eps">default:1E-8</param>
        ''' <returns></returns>
        Public Shared Function IsNearyEqualVector(ByVal vecA As DenseVector, ByVal vecB As DenseVector,
                                                      Optional ByVal eps As Double = 0.00000001) As Boolean
            Try
                For i As Integer = 0 To vecA.Count - 1
                    Dim tempValA = vecA(i)
                    Dim tempValB = vecB(i)
                    If MathUtil.IsCloseToValues(tempValA, tempValB, eps) = False Then
                        Return False
                    End If
                Next
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Swap row
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="fromIdx"></param>
        ''' <param name="destIdx"></param>
        Public Shared Sub SwapRow(ByRef source As DenseMatrix, ByVal fromIdx As Integer, ByVal destIdx As Integer)
            Dim temp = source(fromIdx)
            source(fromIdx) = source(destIdx)
            source(destIdx) = temp
        End Sub

        ''' <summary>
        ''' Swap row
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="fromIdx"></param>
        ''' <param name="destIdx"></param>
        Public Shared Sub SwapCol(ByRef source As DenseMatrix, ByVal fromIdx As Integer, ByVal destIdx As Integer)
            Dim rowCount = source.RowCount
            For i As Integer = 0 To rowCount - 1
                Dim temp As Double = source(i)(fromIdx)
                source(i)(fromIdx) = source(i)(destIdx)
                source(i)(destIdx) = temp
            Next
        End Sub

        ''' <summary>
        ''' check close to zero
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="eps">2.20E-16</param>
        ''' <returns></returns>
        Public Shared Function IsCloseToZero(ByVal value As Double, Optional ByVal eps As Double = ConstantValues.MachineEpsiron) As Boolean
            If System.Math.Abs(value + eps) <= eps Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' compare 2 value
        ''' </summary>
        ''' <param name="value1"></param>
        ''' <param name="value2"></param>
        ''' <param name="eps"></param>
        ''' <returns></returns>
        Public Shared Function IsCloseToValues(ByVal value1 As Double, ByVal value2 As Double, Optional ByVal eps As Double = ConstantValues.MachineEpsiron) As Boolean
            If System.Math.Abs(value1 - value2) < eps Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' sort by eigen value
        ''' </summary>
        ''' <param name="eigenValue"></param>
        ''' <param name="eigenVector"></param>
        ''' <param name="isColOrder"></param>
        Public Shared Sub EigenSort(ByRef eigenValue As DenseVector, ByRef eigenVector As DenseMatrix, ByVal isColOrder As Boolean)
            Dim n = eigenValue.Count
            Dim colSwapInfo = New List(Of ValueDescSort)
            For i As Integer = 0 To n - 1
                colSwapInfo.Add(New ValueDescSort(eigenValue(i), i))
            Next
            colSwapInfo.Sort()

            If isColOrder = True Then
                Dim newEigenVector = New DenseMatrix(n)
                For j As Integer = 0 To n - 1
                    'eigen value
                    eigenValue(j) = colSwapInfo(j).v

                    'eigen vector
                    Dim k = colSwapInfo(j).idx
                    For i As Integer = 0 To n - 1
                        newEigenVector(i)(j) = eigenVector(i)(k)
                    Next
                Next
                eigenVector = newEigenVector
            Else
                Dim newEigenVector = New DenseMatrix(n)
                For i = 0 To n - 1
                    'eigen value
                    eigenValue(i) = colSwapInfo(i).v

                    'eigen vector
                    Dim k = colSwapInfo(i).idx
                    newEigenVector(i) = eigenVector(k)
                Next
                eigenVector = newEigenVector
            End If
        End Sub

        ''' <summary>
        ''' Create Covariance Matrix
        ''' </summary>
        ''' <param name="mat"></param>
        ''' <param name="isRowOrder">True:The data sequence is "row".</param>
        ''' <param name="isUnbalanceVariance">use UnbalanceVariance. default is true</param>
        ''' <returns></returns>
        Public Shared Function CreateCovarianceMatrix(ByRef mat As DenseMatrix,
                                                      ByVal isRowOrder As Boolean,
                                                      Optional ByVal isUnbalanceVariance As Boolean = True) As DenseMatrix
            Dim var_covar As DenseMatrix = Nothing

            If isRowOrder = True Then
                'de-mean
                Dim avev = mat.AverageVector(DenseVector.VectorDirection.ROW)
                Dim mat_demean As DenseMatrix = mat - avev

                'E[X.T * X] -> 1/N[X.T * X]
                Dim gramMatrix = mat_demean.T * mat_demean
                If isUnbalanceVariance Then
                    var_covar = gramMatrix / (mat.RowCount - 1)
                Else
                    var_covar = gramMatrix / mat.RowCount
                End If
            Else
                'de-mean
                Dim avev = mat.AverageVector(DenseVector.VectorDirection.COL)
                Dim mat_demean As DenseMatrix = mat - avev

                'E[X.T * X] -> 1/N[X.T * X]
                Dim gramMatrix = mat_demean * mat_demean.T
                If isUnbalanceVariance Then
                    var_covar = gramMatrix / (mat.RowCount - 1)
                Else
                    var_covar = gramMatrix / mat.RowCount
                End If
            End If

            Return var_covar
        End Function

        ''' <summary>
        ''' Pythagorean Addition (sqrt(x1^2 + x2^2)) using Moler-Morrison Algorithm
        ''' </summary>
        ''' <param name="valA"></param>
        ''' <param name="valB"></param>
        ''' <returns></returns>
        Public Shared Function PythagoreanAddition(ByVal valA As Double, ByVal valB As Double) As Double
            Dim aMax = System.Math.Abs(valA)
            Dim bMin = System.Math.Abs(valB)

            If bMin = 0.0 Then
                Return 0.0
            ElseIf aMax < bMin Then
                Dim temp = aMax
                aMax = bMin
                bMin = temp
            End If

            'For i = 0 To 4 - 1
            '    Dim b = System.Math.Pow(a / f, 2)
            '    Dim c = b / (4.0 + b)
            '    f = f + 2.0 * c * f
            '    a = c * a
            'Next

            ' 平方根を使わない計算
            ' Loop1
            Dim b = System.Math.Pow(bMin / aMax, 2)
            Dim c = b / (4.0 + b)
            aMax = aMax + 2.0 * c * aMax
            bMin = c * bMin

            ' Loop2
            b = System.Math.Pow(bMin / aMax, 2)
            c = b / (4.0 + b)
            aMax = aMax + 2.0 * c * aMax
            bMin = c * bMin

            ' Loop3
            b = System.Math.Pow(bMin / aMax, 2)
            c = b / (4.0 + b)
            aMax = aMax + 2.0 * c * aMax
            bMin = c * bMin

            Return aMax
        End Function

        ''' <summary>
        ''' 相対誤差 | target - true | / target
        ''' </summary>
        ''' <param name="trueValue"></param>
        ''' <param name="targetValue"></param>
        ''' <returns></returns>
        Public Shared Function RelativeError(ByVal trueValue As Double, ByVal targetValue As Double) As Double
            If MathUtil.IsCloseToValues(trueValue, targetValue) Then
                Return 0
            Else
                '分母が0の場合はNaN
                Return Math.Abs(targetValue - trueValue) / targetValue
            End If
        End Function

        ''' <summary>
        ''' Log Gamma function
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Refference:
        ''' C言語による最新アルゴリズム事典
        ''' </remarks>
        Public Shared Function LogGamma(ByVal x As Double) As Double
            Dim LOG_2PI = 1.8378770664093456
            Dim N = 8

            Dim B0 = 1              '   /* 以下はBernoulli数 */
            Dim B1 = (-1.0 / 2.0)
            Dim B2 = (1.0 / 6.0)
            Dim B4 = (-1.0 / 30.0)
            Dim B6 = (1.0 / 42.0)
            Dim B8 = (-1.0 / 30.0)
            Dim B10 = (5.0 / 66.0)
            Dim B12 = (-691.0 / 2730.0)
            Dim B14 = (7.0 / 6.0)
            Dim B16 = (-3617.0 / 510.0)

            Dim v As Double = 1
            Dim w As Double

            While x < N
                v *= x
                x += 1
            End While
            w = 1 / (x * x)
            Return ((((((((B16 / (16 * 15)) * w + (B14 / (14 * 13))) * w _
                        + (B12 / (12 * 11))) * w + (B10 / (10 * 9))) * w _
                        + (B8 / (8 * 7))) * w + (B6 / (6 * 5))) * w _
                        + (B4 / (4 * 3))) * w + (B2 / (2 * 1))) / x _
                        + 0.5 * LOG_2PI - Math.Log(v) - x + (x - 0.5) * Math.Log(x)
        End Function

        ''' <summary>
        ''' Gamma function
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Refference:
        ''' C言語による最新アルゴリズム事典
        ''' </remarks>
        Public Shared Function Gamma(ByVal x As Double) As Double
            If (x < 0) Then
                Return Math.PI / (Math.Sin(Math.PI * x) * Math.Exp(LogGamma(1 - x)))
            End If
            Return Math.Exp(LogGamma(x))
        End Function
    End Class

End Namespace

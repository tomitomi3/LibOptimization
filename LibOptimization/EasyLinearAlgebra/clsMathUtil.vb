Namespace MathUtil
    ''' <summary>
    ''' Utility class for Math
    ''' </summary>
    Public Class clsMathUtil
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
        ''' Create random symmetric matrix(for Debug)
        ''' </summary>
        ''' <param name="size"></param>
        ''' <param name="rng"></param>
        ''' <returns></returns>
        Public Shared Function CreateRandomSymmetricMatrix(ByVal size As Integer,
                                                           Optional ByVal rng As Random = Nothing,
                                                           Optional ByVal isIncludeZero As Boolean = False,
                                                           Optional ByVal isFloating As Boolean = False,
                                                           Optional ByVal lower As Double = -10,
                                                           Optional ByVal upper As Double = 10) As clsEasyMatrix
            If rng Is Nothing Then
                rng = New Util.clsRandomXorshift()
            End If
            Dim matTemp = New MathUtil.clsEasyMatrix(size)
            For i As Integer = 0 To matTemp.Count - 1
                For j As Integer = 1 + i To matTemp.Count - 1
                    Dim r As Double = 0.0
                    If isFloating = False Then
                        r = rng.Next(CInt(lower), CInt(upper))
                    Else
                        r = Math.Abs(upper - lower) * rng.NextDouble() + lower
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
                    matTemp(i)(i) = Math.Abs(upper - lower) * rng.NextDouble() + lower
                End If
            Next

            Return matTemp
        End Function

        ''' <summary>
        ''' Create random Asymmetric matrix(for Debug)
        ''' </summary>
        ''' <param name="size"></param>
        ''' <returns></returns>
        Public Shared Function CreateRandomASymmetricMatrix(ByVal size As Integer,
                                                           Optional ByVal rng As Random = Nothing,
                                                           Optional ByVal isIncludeZero As Boolean = True,
                                                           Optional ByVal isFloating As Boolean = False,
                                                           Optional ByVal lower As Double = -10,
                                                           Optional ByVal upper As Double = 10) As clsEasyMatrix
            If rng Is Nothing Then
                rng = New Util.clsRandomXorshift()
            End If
            Dim matTemp = New MathUtil.clsEasyMatrix(size)
            For i As Integer = 0 To matTemp.Count - 1
                For j As Integer = 0 To matTemp.Count - 1
                    Dim r As Double = 0.0
                    If isFloating = False Then
                        r = rng.Next(CInt(lower), CInt(upper))
                    Else
                        r = Math.Abs(upper - lower) * rng.NextDouble() + lower
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
        Public Shared Function IsNearyEqualMatrix(ByVal matA As clsEasyMatrix, ByVal matB As clsEasyMatrix,
                                                  Optional ByVal eps As Double = 0.00000001) As Boolean
            Try
                For i As Integer = 0 To matA.RowCount - 1
                    For j As Integer = 0 To matA.ColCount - 1
                        Dim tempValA = matA(i)(j)
                        Dim tempValB = matB(i)(j)
                        If clsMathUtil.IsCloseToValues(tempValA, tempValB, eps) = False Then
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
        Public Shared Function IsNearyEqualVector(ByVal vecA As clsEasyVector, ByVal vecB As clsEasyVector,
                                                  Optional ByVal eps As Double = 0.00000001) As Boolean
            Try
                For i As Integer = 0 To vecA.Count - 1
                    Dim tempValA = vecA(i)
                    Dim tempValB = vecB(i)
                    If clsMathUtil.IsCloseToValues(tempValA, tempValB, eps) = False Then
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
        Public Shared Sub SwapRow(ByRef source As clsEasyMatrix, ByVal fromIdx As Integer, ByVal destIdx As Integer)
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
        Public Shared Sub SwapCol(ByRef source As clsEasyMatrix, ByVal fromIdx As Integer, ByVal destIdx As Integer)
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
        Public Shared Function IsCloseToZero(ByVal value As Double, Optional ByVal eps As Double = clsEasyMatrix.MachineEpsiron) As Boolean
            If Math.Abs(value + eps) <= eps Then
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
        Public Shared Function IsCloseToValues(ByVal value1 As Double, ByVal value2 As Double, Optional ByVal eps As Double = clsEasyMatrix.MachineEpsiron) As Boolean
            If Math.Abs(value1 - value2) < eps Then
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
        Public Shared Sub EigenSort(ByRef eigenValue As clsEasyVector, ByRef eigenVector As clsEasyMatrix, ByVal isColOrder As Boolean)
            Dim n = eigenValue.Count
            Dim colSwapInfo = New List(Of ValueDescSort)
            For i As Integer = 0 To n - 1
                colSwapInfo.Add(New ValueDescSort(eigenValue(i), i))
            Next
            colSwapInfo.Sort()

            If isColOrder = True Then
                Dim newEigenVector = New clsEasyMatrix(n)
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
                Dim newEigenVector = New clsEasyMatrix(n)
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
        ''' Create Variane Co-Varianve Matrix
        ''' </summary>
        ''' <param name="mat"></param>
        ''' <param name="isRowOrder">True:The data sequence is "row".</param>
        ''' <param name="isUnbalanceVariance">use UnbalanceVariance</param>
        ''' <returns></returns>
        Public Shared Function CreateVarCoVarMat(ByRef mat As clsEasyMatrix,
                                                 ByVal isRowOrder As Boolean,
                                                 Optional ByVal isUnbalanceVariance As Boolean = True) As clsEasyMatrix
            If isRowOrder = True Then
                'de-mean
                Dim avev = mat.AverageVector(True)
                Dim mat_demean As clsEasyMatrix = mat - avev

                'E[X.T * X] -> 1/N[X.T * X]
                Dim gramMatrix = mat_demean.T * mat_demean
                Dim var_covar As clsEasyMatrix = Nothing
                If isUnbalanceVariance Then
                    var_covar = gramMatrix / (mat.RowCount - 1)
                Else
                    var_covar = gramMatrix / mat.RowCount
                End If

                Return var_covar
            Else
                'de-mean
                Dim avev = mat.AverageVector(False)
                Dim mat_demean As clsEasyMatrix = mat - avev

                'E[X.T * X] -> 1/N[X.T * X]
                Dim gramMatrix = mat_demean * mat_demean.T
                Dim var_covar As clsEasyMatrix = Nothing
                If isUnbalanceVariance Then
                    var_covar = gramMatrix / (mat.RowCount - 1)
                Else
                    var_covar = gramMatrix / mat.RowCount
                End If

                Return var_covar
            End If
        End Function

        ''' <summary>
        ''' Pythagorean Addition (sqrt(x1^2 + x2^2))
        ''' </summary>
        ''' <param name="valA"></param>
        ''' <param name="valB"></param>
        ''' <returns></returns>
        Public Shared Function PythagoreanAddition(ByVal valA As Double, ByVal valB As Double) As Double
            Dim a = Math.Abs(valA)
            Dim b = Math.Abs(valB)

            If a > b Then
                Return a * Math.Sqrt(1.0 + (b / a) * (b / a))
            ElseIf b = 0.0 Then
                Return 0.0
            Else
                Return b * Math.Sqrt(1.0 + (a / b) * (a / b))
            End If
        End Function

        ''' <summary>
        ''' Pythagorean Addition (sqrt(x1^2 + x2^2)) using Moler-Morrison Algorithm
        ''' </summary>
        ''' <param name="valA"></param>
        ''' <param name="valB"></param>
        ''' <returns></returns>
        Public Shared Function PythagoreanAddition_MolerMorrison(ByVal valA As Double, ByVal valB As Double) As Double
            Dim a = Math.Abs(valA)
            Dim b = Math.Abs(valB)

            If b = 0.0 Then
                Return 0.0
            ElseIf a < b Then
                Dim temp = a
                a = b
                b = temp
            End If

            For i = 0 To 4 - 1
                Dim s = Math.Pow(b / a, 2)
                s /= 4.0 + s
                a += 2.0 * a * s
                b *= s
            Next

            Return a
        End Function

        ''' <summary>
        ''' set identify matrix
        ''' </summary>
        ''' <param name="mat"></param>
        Public Shared Sub SetIdentifyMatrix(ByRef mat As clsEasyMatrix)
            Dim n_dim = mat.RowCount
            For i = 0 To n_dim - 1
                For j = 0 To n_dim - 1
                    If i = j Then
                        mat(i)(j) = 1.0
                    Else
                        mat(i)(j) = 0
                    End If
                Next
            Next
        End Sub
    End Class
End Namespace

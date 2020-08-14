Namespace MathUtil
    ''' <summary>
    ''' Utility class for Math
    ''' </summary>
    Public Class clsMathUtil
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
    End Class
End Namespace

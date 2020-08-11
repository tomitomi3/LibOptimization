Namespace MathUtil
    ''' <summary>
    ''' Utility class for Math
    ''' </summary>
    Public Class clsMathUtil
        ''' <summary>
        ''' Create random symmetric matrix(for Debug)
        ''' </summary>
        ''' <param name="size"></param>
        ''' <param name="rngSeed"></param>
        ''' <param name="rng"></param>
        ''' <returns></returns>
        Public Shared Function CreateRandomSymmetricMatrix(ByVal size As Integer,
                                                           Optional ByVal rngSeed As Integer = 123456,
                                                           Optional ByVal rng As Random = Nothing) As clsEasyMatrix
            If rng Is Nothing Then
                rng = New System.Random(rngSeed)
            End If
            Dim matTemp = New MathUtil.clsEasyMatrix(size)
            For i As Integer = 0 To matTemp.Count - 1
                For j As Integer = 1 + i To matTemp.Count - 1
                    Dim r = rng.Next(-10, 10)
                    matTemp(i)(j) = r
                    matTemp(j)(i) = r
                Next
                matTemp(i)(i) = 1
            Next

            Return matTemp
        End Function

        ''' <summary>
        ''' Create random Asymmetric matrix(for Debug)
        ''' </summary>
        ''' <param name="size"></param>
        ''' <param name="rngSeed"></param>
        ''' <returns></returns>
        Public Shared Function CreateRandomASymmetricMatrix(ByVal size As Integer,
                                                            Optional ByVal rngSeed As Integer = 123456,
                                                            Optional ByVal rng As Random = Nothing) As clsEasyMatrix
            If rng Is Nothing Then
                rng = New System.Random(rngSeed)
            End If
            Dim matTemp = New MathUtil.clsEasyMatrix(size)
            For i As Integer = 0 To matTemp.Count - 1
                For j As Integer = 0 To matTemp.Count - 1
                    Dim r = rng.Next(-10, 10)
                    matTemp(i)(j) = r
                Next
            Next

            Return matTemp
        End Function

        ''' <summary>
        ''' check eaual matrix
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
                        Dim diff = Math.Sqrt(Math.Pow(tempValA - tempValB, 2))
                        If diff > eps Then
                            Return False
                        End If
                    Next
                Next
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

    End Class
End Namespace

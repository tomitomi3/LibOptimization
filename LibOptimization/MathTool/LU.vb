Namespace MathTool
    ''' <summary>
    ''' store LU decomposition with solver
    ''' </summary>
    <Serializable>
    Public Class LU
        ''' <summary>Pivot matrix</summary>
        Public Property P As DenseMatrix = Nothing

        ''' <summary>Lower matrix</summary>
        Public Property L As DenseMatrix = Nothing

        ''' <summary>Upper matrix</summary>
        Public Property U As DenseMatrix = Nothing

        ''' <summary>Determinant</summary>
        Public Property Det As Double = 0.0

        ''' <summary>pivto row info</summary>
        Public Property PivotRow As Integer() = Nothing

        ''' <summary>
        ''' default constructtor
        ''' </summary>
        Private Sub New()
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="matP"></param>
        ''' <param name="matL"></param>
        ''' <param name="matU"></param>
        ''' <param name="det"></param>
        Public Sub New(ByRef matP As DenseMatrix, ByRef matL As DenseMatrix, ByRef matU As DenseMatrix, ByVal det As Double)
            Me.P = matP
            Me.L = matL
            Me.U = matU
            Me.Det = det
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="matP"></param>
        ''' <param name="matL"></param>
        ''' <param name="matU"></param>
        ''' <param name="det"></param>
        Public Sub New(ByRef matP As DenseMatrix, ByRef matL As DenseMatrix, ByRef matU As DenseMatrix, ByVal det As Double, ByRef p() As Integer)
            Me.P = matP
            Me.L = matL
            Me.U = matU
            Me.Det = det
            Me.PivotRow = p
        End Sub

        ''' <summary>
        ''' solve(Ax=b)
        ''' </summary>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Function Solve(ByRef b As DenseVector) As DenseVector
            Return Me.Solve(Me.P, Me.L, Me.U, Me.PivotRow, b)
        End Function

        ''' <summary>
        ''' solve(Ax=b)
        ''' </summary>
        ''' <param name="matP">pivot matrix(LU decomposition of A matrix)</param>
        ''' <param name="matL">lower triangle matrix(LU decomposition of A matrix)</param>
        ''' <param name="matU">upper triangle matrix(LU decomposition of A matrix)</param>
        ''' <param name="pivotRow"></param>
        ''' <param name="vecB"></param>
        ''' <returns>x</returns>
        Private Function Solve(ByRef matP As DenseMatrix,
                               ByRef matL As DenseMatrix,
                               ByRef matU As DenseMatrix,
                               ByRef pivotRow() As Integer,
                               ByRef vecB As DenseVector) As DenseVector
            Dim n = matP.ColCount
            Dim x = New DenseVector(n)
            Dim y = New DenseVector(n)

            'transopose
            'Dim b = vecB * matP

            For i = 0 To n - 1
                Dim s = 0.0
                Dim j As Integer = 0
                For j = 0 To i - 1
                    s += matL(i)(j) * y(j)
                Next

                'y(j) = b(i) - s
                y(j) = vecB(pivotRow(i)) - s
            Next

            For i = n - 1 To 0 Step -1
                Dim s = 0.0
                For k = i + 1 To n - 1
                    s += matU(i)(k) * x(k)
                Next
                x(i) = (y(i) - s) / matU(i)(i)
            Next

            Return x
        End Function
    End Class
End Namespace

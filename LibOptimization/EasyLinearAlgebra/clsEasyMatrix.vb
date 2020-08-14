Namespace MathUtil
    ''' <summary>
    ''' store LU decomposition
    ''' </summary>
    Public Class LU
        ''' <summary>Pivot matrix</summary>
        Public Property P As clsEasyMatrix = Nothing

        ''' <summary>Lower matrix</summary>
        Public Property L As clsEasyMatrix = Nothing

        ''' <summary>Upper matrix</summary>
        Public Property U As clsEasyMatrix = Nothing

        ''' <summary>Determinant</summary>
        Public Property Det As Double = 0.0

        ''' <summary>pivto row info</summary>
        Public Property PivotRow As Double() = Nothing

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
        Public Sub New(ByRef matP As clsEasyMatrix, ByRef matL As clsEasyMatrix, ByRef matU As clsEasyMatrix, ByVal det As Double)
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
        Public Sub New(ByRef matP As clsEasyMatrix, ByRef matL As clsEasyMatrix, ByRef matU As clsEasyMatrix, ByVal det As Double, ByRef p() As Double)
            Me.P = matP
            Me.L = matL
            Me.U = matU
            Me.Det = det
            Me.PivotRow = p
        End Sub
    End Class

    ''' <summary>
    ''' store SVD decomposition
    ''' </summary>
    Public Class SVD
        ''' <summary></summary>
        Public Property S As clsEasyMatrix = Nothing

        ''' <summary></summary>
        Public Property V As clsEasyVector = Nothing

        ''' <summary></summary>
        Public Property D As clsEasyMatrix = Nothing

        Private Sub New()
        End Sub

        Public Sub New(ByRef matS As clsEasyMatrix, ByRef matV As clsEasyVector, ByRef matD As clsEasyMatrix)
            Me.S = matS
            Me.V = matV
            Me.D = matD
        End Sub
    End Class

    ''' <summary>
    ''' store Eigen values, vector
    ''' </summary>
    Public Class Eigen
        ''' <summary></summary>
        Public Property EigenValue As clsEasyVector = Nothing

        ''' <summary></summary>
        Public Property EigenVector As clsEasyMatrix = Nothing

        ''' <summary></summary>
        Public Property IsConversion As Boolean = Nothing

        Private Sub New()
        End Sub

        Public Sub New(ByRef eigeValue As clsEasyVector, ByRef eigenVec As clsEasyMatrix)
            Me.EigenValue = eigeValue
            Me.EigenVector = eigenVec
        End Sub

        Public Sub New(ByRef eigeValue As clsEasyVector, ByRef eigenVec As clsEasyMatrix, ByVal isconversion As Boolean)
            Me.EigenValue = eigeValue
            Me.EigenVector = eigenVec
            Me.IsConversion = isconversion
        End Sub

    End Class

    ''' <summary>
    ''' Matrix class
    ''' </summary>
    ''' <remarks>
    ''' Inherits List(Of List(Of Double))
    ''' </remarks>
    Public Class clsEasyMatrix : Inherits List(Of List(Of Double))
        Public Const SAME_ZERO As Double = 2.0E-50 '2.0*10^-50
        Public Const MachineEpsiron As Double = 0.000000000000000222 ' 2.20*E-16 = 2.20*10^-16

#Region "Constructor"
        ''' <summary>
        ''' Default construcotr
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            'nop
        End Sub

        ''' <summary>
        ''' Copy constructor
        ''' </summary>
        ''' <param name="ai_base"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_base As clsEasyMatrix)
            For i As Integer = 0 To ai_base.Count - 1
                Dim temp As New clsEasyVector(ai_base(i))
                Me.Add(temp)
            Next
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_dim"></param>
        ''' <param name="ai_isIdentity">Make Identify matrix</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_dim As Integer, Optional ai_isIdentity As Boolean = False)
            For i As Integer = 0 To ai_dim - 1
                Dim temp As New clsEasyVector(ai_dim)
                If ai_isIdentity Then
                    temp(i) = 1.0
                End If
                Me.Add(temp)
            Next
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_rowSize"></param>
        ''' <param name="ai_colSize"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_rowSize As Integer, ByVal ai_colSize As Integer)
            For i As Integer = 0 To ai_rowSize - 1
                Me.Add(New clsEasyVector(ai_colSize))
            Next
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_val"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_val As List(Of List(Of Double)))
            For i As Integer = 0 To ai_val.Count - 1
                Me.Add(New clsEasyVector(ai_val(i)))
            Next
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_val"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_val()() As Double)
            For i As Integer = 0 To ai_val.Length - 1
                Me.Add(New clsEasyVector(ai_val(i)))
            Next
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_val"></param>
        ''' <param name="ai_direction">Row or Col</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_val As List(Of Double), ByVal ai_direction As clsEasyVector.VectorDirection)
            If ai_direction = clsEasyVector.VectorDirection.ROW Then
                Dim temp As New clsEasyVector(ai_val)
                Me.Add(temp)
            Else
                For i As Integer = 0 To ai_val.Count - 1
                    Me.Add(New clsEasyVector({ai_val(i)}))
                Next
            End If
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="vector"></param>
        ''' <param name="order"></param>
        Public Sub New(ByVal vector As List(Of Double), ByVal order As Integer)
            For i As Integer = 0 To order - 1
                Dim temp(order - 1) As Double
                Me.Add(New clsEasyVector(temp))
            Next
            Dim index As Integer = 0
            For i As Integer = 0 To order - 1
                For j As Integer = 0 To order - 1
                    Me(j)(i) = vector(index)
                    index += 1
                Next
            Next
        End Sub
#End Region

#Region "Public Operator"
        ''' <summary>
        ''' Add(Matrix + Matrix)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator +(ByVal ai_source As clsEasyMatrix, ByVal ai_dest As clsEasyMatrix) As clsEasyMatrix
            If IsSameDimension(ai_source, ai_dest) = False Then
                Throw New clsException(clsException.Series.DifferRowNumberAndCollumnNumber)
            End If
            Dim ret As New clsEasyMatrix(ai_source)
            For i As Integer = 0 To ret.RowCount() - 1
                ret.Row(i) = ai_source.Row(i) + ai_dest.Row(i)
            Next
            Return ret
        End Operator

        ''' <summary>s
        ''' Add(Matrix + Vector)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator +(ByVal ai_source As clsEasyMatrix, ByVal ai_dest As clsEasyVector) As clsEasyVector
            If IsComputableMatrixVector(ai_source, ai_dest) = False Then
                Throw New clsException(clsException.Series.NotComputable)
            End If
            Dim ret As New clsEasyVector(ai_dest)
            For i As Integer = 0 To ai_dest.Count - 1
                ret(i) = ai_source(i)(0) + ai_dest(i)
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Add(Vector + Matrix)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator +(ByVal ai_source As clsEasyVector, ByVal ai_dest As clsEasyMatrix) As clsEasyVector
            If IsComputableMatrixVector(ai_dest, ai_source) = False Then
                Throw New clsException(clsException.Series.NotComputable)
            End If
            Dim ret As New clsEasyVector(ai_source)
            For i As Integer = 0 To ai_source.Count - 1
                ret(i) = ai_source(i) + ai_dest(i)(0)
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Diff
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator -(ByVal ai_source As clsEasyMatrix, ByVal ai_dest As clsEasyMatrix) As clsEasyMatrix
            If IsSameDimension(ai_source, ai_dest) = False Then
                Throw New clsException(clsException.Series.DifferRowNumberAndCollumnNumber)
            End If
            Dim ret As New clsEasyMatrix(ai_source)
            For i As Integer = 0 To ret.RowCount() - 1
                ret.Row(i) = ai_source.Row(i) - ai_dest.Row(i)
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Diff(Matrix + Vector)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator -(ByVal ai_source As clsEasyMatrix, ByVal ai_dest As clsEasyVector) As clsEasyVector
            If IsComputableMatrixVector(ai_source, ai_dest) = False Then
                Throw New clsException(clsException.Series.NotComputable)
            End If
            Dim ret As New clsEasyVector(ai_dest)
            For i As Integer = 0 To ai_dest.Count - 1
                ret(i) = ai_source(i)(0) - ai_dest(i)
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Diff(Vector + Matrix)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator -(ByVal ai_source As clsEasyVector, ByVal ai_dest As clsEasyMatrix) As clsEasyVector
            If IsComputableMatrixVector(ai_dest, ai_source) = False Then
                Throw New clsException(clsException.Series.NotComputable)
            End If
            Dim ret As New clsEasyVector(ai_source)
            For i As Integer = 0 To ai_source.Count - 1
                ret(i) = ai_source(i) - ai_dest(i)(0)
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Product( Matrix * Matrix )
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Public Shared Operator *(ByVal ai_source As clsEasyMatrix, ByVal ai_dest As clsEasyMatrix) As clsEasyMatrix
            '.NET Frameworkのバージョンで区分け
#If (NET30_CUSTOM OrElse NET35_CUSTOM) = True Then
            '------------------------------------------------------------------
            '.net 3.0, 3.5
            '------------------------------------------------------------------
            If IsSameDimension(ai_source, ai_dest) = True Then
                '[M*M] X [M*M]
                Dim size = ai_source.RowCount
                Dim ret As New clsEasyMatrix(size)

                For i As Integer = 0 To size - 1
                    For j As Integer = 0 To size - 1
                        For k As Integer = 0 To size - 1
                            Dim tempA = ai_source(i)(k)
                            Dim tempB = ai_dest(k)(j)
                            ret(i)(j) += tempA * tempB
                        Next
                    Next
                Next
                Return ret
            ElseIf ai_source.ColCount = ai_dest.RowCount Then
                '[M*N] X [N*O]
                Dim ret As New clsEasyMatrix(ai_source.RowCount, ai_dest.ColCount)

                For i As Integer = 0 To ret.RowCount - 1
                    For j As Integer = 0 To ret.ColCount - 1
                        Dim temp As Double = 0.0
                        For k As Integer = 0 To ai_source.ColCount - 1
                            temp += ai_source(i)(k) * ai_dest(k)(j)
                        Next
                        ret(i)(j) = temp
                    Next
                Next
                Return ret
            End If

            Throw New clsException(clsException.Series.NotComputable, "Matrix * Matrix")
#Else
            '------------------------------------------------------------------
            '.net 4.0
            '------------------------------------------------------------------
            'using Paralle .NET4
            'Dim pOption = New Threading.Tasks.ParallelOptions()
            '#If DEBUG Then
            '            pOption.MaxDegreeOfParallelism = 1
            '#End If

            If IsSameDimension(ai_source, ai_dest) = True Then
                '[M*M] X [M*M]
                Dim size = ai_source.RowCount
                Dim ret As New clsEasyMatrix(size)

                Threading.Tasks.Parallel.For(0, size,
                                             Sub(i)
                                                 For j As Integer = 0 To size - 1
                                                     For k As Integer = 0 To size - 1
                                                         Dim tempA = ai_source(i)(k)
                                                         Dim tempB = ai_dest(k)(j)
                                                         ret(i)(j) += tempA * tempB
                                                     Next
                                                 Next
                                             End Sub)
                Return ret
            ElseIf ai_source.ColCount = ai_dest.RowCount Then
                '[M*N] X [N*O]
                Dim rowSize = ai_source.RowCount
                Dim ret As New clsEasyMatrix(ai_source.RowCount, ai_dest.ColCount)

                Threading.Tasks.Parallel.For(0, rowSize,
                                             Sub(i)
                                                 For j As Integer = 0 To ret.ColCount - 1
                                                     Dim temp As Double = 0.0
                                                     For k As Integer = 0 To ai_source.ColCount - 1
                                                         temp += ai_source(i)(k) * ai_dest(k)(j)
                                                     Next
                                                     ret(i)(j) = temp
                                                 Next
                                             End Sub)
                Return ret
            End If

            Throw New clsException(clsException.Series.NotComputable, "Matrix * Matrix")
#End If
        End Operator

        ''' <summary>
        ''' Product( Matrix * Vector )
        ''' </summary>
        ''' <param name="mat">Matrix</param>
        ''' <param name="vec">Vector</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Public Shared Operator *(ByVal mat As clsEasyMatrix, ByVal vec As clsEasyVector) As clsEasyVector
            'ベクトルと行列のサイズ確認
            ' M*M * M
            ' |a11 a12| * |v1| = cv1
            ' |a21 a22|   |v2|   cv2
            '
            ' M*N * N
            ' |a11 a12| * |v1| = cv1
            '             |v2|
            '
            ' |a11 a12| * |v1| = cv1
            ' |a21 a22|   |v2|   cv2
            ' |a31 a32|          cv3
            '
            If mat.ColCount <> vec.Count Then
                Throw New clsException(clsException.Series.NotComputable, "Matrix * Vector - size error")
            End If
            'If vec.Direction <> clsEasyVector.VectorDirection.COL Then
            '    Throw New clsException(clsException.Series.NotComputable, "Matrix * Vector - vector direction is row")
            'End If

            Dim vSize As Integer = mat.RowCount
            Dim ret As New clsEasyVector(vSize, clsEasyVector.VectorDirection.COL)
#If (NET30_CUSTOM OrElse NET35_CUSTOM) = True Then
            For i As Integer = 0 To vSize - 1
                Dim sum As Double = 0.0
                For j As Integer = 0 To mat.ColCount - 1
                    sum += mat(i)(j) * vec(j)
                Next
                ret(i) = sum
            Next
            Return ret
#Else
            Threading.Tasks.Parallel.For(0, vSize,
                                             Sub(i)
                                                 Dim sum As Double = 0.0
                                                 For j As Integer = 0 To mat.ColCount - 1
                                                     sum += mat(i)(j) * vec(j)
                                                 Next
                                                 ret(i) = sum
                                             End Sub)
            Return ret
#End If
        End Operator

        ''' <summary>
        ''' Product( Vector * Matrix)
        ''' </summary>
        ''' <param name="vec">Vector</param>
        ''' <param name="mat">Matrix</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Public Shared Operator *(ByVal vec As clsEasyVector, ByVal mat As clsEasyMatrix) As clsEasyVector
            'ベクトルと行列のサイズ確認
            'OK
            ' |v1 v2| * |a11| = |c1|
            '           |a12|   
            '
            ' |v1 v2| * |a11 a12| = |c1 c2|
            '           |a11 a22|   
            '
            ' |v1 v2| * |a11 a12 a13| = |c1 c2 c3|
            '           |a21 a22 a23|   
            '
            If vec.Count <> mat.RowCount Then
                Throw New clsException(clsException.Series.NotComputable, "Vector * Matrix - size error")
            End If
            'If vec.Direction <> clsEasyVector.VectorDirection.COL Then
            '    Throw New clsException(clsException.Series.NotComputable, "Vector * Matrix - vector direction is col")
            'End If

            Dim vSize As Integer = mat.ColCount '行列の行サイズ
            Dim ret As New clsEasyVector(vSize, clsEasyVector.VectorDirection.ROW)
#If (NET30_CUSTOM OrElse NET35_CUSTOM) = True Then
           For j As Integer = 0 To vSize - 1
                Dim sum As Double = 0.0
                For i As Integer = 0 To mat.RowCount - 1
                    sum += vec(i) * mat(i)(j)
                Next
                ret(j) = sum
            Next
            Return ret
#Else
            Threading.Tasks.Parallel.For(0, vSize,
                                             Sub(j)
                                                 Dim sum As Double = 0.0
                                                 For i As Integer = 0 To mat.RowCount - 1
                                                     sum += vec(i) * mat(i)(j)
                                                 Next
                                                 ret(j) = sum
                                             End Sub)
            Return ret
#End If
        End Operator

        ''' <summary>
        ''' Product(value * Matrix)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator *(ByVal ai_source As Double, ByVal ai_dest As clsEasyMatrix) As clsEasyMatrix
            Dim ret As New clsEasyMatrix(ai_dest)
            For i As Integer = 0 To ret.RowCount() - 1
                ret.Row(i) = ai_source * ai_dest.Row(i)
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Product(Matrix * value)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator *(ByVal ai_source As clsEasyMatrix, ByVal ai_dest As Double) As clsEasyMatrix
            Dim ret As New clsEasyMatrix(ai_source)
            For i As Integer = 0 To ret.RowCount() - 1
                ret.Row(i) = ai_source.Row(i) * ai_dest
            Next
            Return ret
        End Operator
#End Region

#Region "Public func"
        ''' <summary>
        ''' Transpose
        ''' </summary>
        ''' <remarks></remarks>
        Public Function T() As clsEasyMatrix
            Dim ret As New clsEasyMatrix(Me.ColCount, Me.RowCount)

#If (NET30_CUSTOM OrElse NET35_CUSTOM) = True Then
            '------------------------------------------------------------------
            '.net 3.0, 3.5
            '------------------------------------------------------------------
            For rowIndex As Integer = 0 To ret.RowCount - 1
                ret.Row(rowIndex) = Me.Column(rowIndex)
            Next
#Else
            '------------------------------------------------------------------
            '.net 4.0
            '------------------------------------------------------------------
            'using Paralle .NET4
            'Dim pOption = New Threading.Tasks.ParallelOptions()
            Threading.Tasks.Parallel.For(0, ret.RowCount,
                                             Sub(rowIndex)
                                                 ret.Row(rowIndex) = Me.Column(rowIndex)
                                             End Sub)
#End If

            Return ret
        End Function

        ''' <summary>
        ''' Determinant
        ''' </summary>
        ''' <remarks></remarks>
        Public Function Det() As Double
            If Me.RowCount <> Me.ColCount Then
                Return 0
            End If
            Dim n = Me.RowCount

            If n = 1 Then
                Return Me(0)(0)
            ElseIf n = 2 Then
                '2 dim
                ' | a b |
                ' | c d | = ad-bc
                Return Me(0)(0) * Me(1)(1) - Me(0)(1) * Me(1)(0)
            ElseIf n = 3 Then
                '3 dim using Sarrus rule
                Dim d As Double = 0.0
                d += Me(0)(0) * Me(1)(1) * Me(2)(2)
                d += Me(1)(0) * Me(2)(1) * Me(0)(2)
                d += Me(2)(0) * Me(0)(1) * Me(1)(2)
                d -= Me(2)(0) * Me(1)(1) * Me(0)(2)
                d -= Me(1)(0) * Me(0)(1) * Me(2)(2)
                d -= Me(0)(0) * Me(2)(1) * Me(1)(2)
                Return d
            Else
                'over 4 dim using PLU decomposition
                'Dim detVal As Double = 0.0
                'Dim b As New clsEasyMatrix(ai_dim - 1)
                'Dim sign As Integer = 0
                'If ((ai_dim + 1) Mod (2)) = 0 Then
                '    sign = 1
                'Else
                '    sign = -1
                'End If

                'For k As Integer = 0 To ai_dim - 1
                '    For i As Integer = 0 To ai_dim - 2
                '        For j As Integer = 0 To ai_dim - 1
                '            If j = k Then
                '                Continue For
                '            End If
                '            If j > k Then
                '                b(i)(j - 1) = ai_clsMatrix(i)(j)
                '            Else
                '                b(i)(j) = ai_clsMatrix(i)(j)
                '            End If
                '        Next
                '    Next
                '    If ai_isDebug = True Then
                '        Console.WriteLine(sign.ToString() & " " & ai_clsMatrix(ai_dim - 1)(k).ToString())
                '        b.PrintValue()
                '    End If
                '    detVal += sign * ai_clsMatrix(ai_dim - 1)(k) * CalcDeterminant(b, ai_dim - 1)
                '    sign *= -1
                'Next
                Dim detVal = Me.PLU().Det
                Return detVal
            End If
        End Function

        ''' <summary>
        ''' Convert to a matrix with only diagonal values
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToDiagonalMatrix() As clsEasyMatrix
            If Me.RowCount <> Me.ColCount Then
                Throw New clsException(clsException.Series.NotComputable, "ToDiagonalMatrix()")
            End If
            Dim ret As New clsEasyMatrix(Me.RowCount)
            For i As Integer = 0 To Me.Count - 1
                ret(i)(i) = Me(i)(i)
            Next
            Return ret
        End Function

        ''' <summary>
        ''' Convert to a vector with only diagonal values
        ''' </summary>
        ''' <param name="direction">direction of vector(default:row)</param>
        ''' <returns></returns>
        Public Function ToDiagonalVector(Optional ByVal direction As clsEasyVector.VectorDirection = clsEasyVector.VectorDirection.ROW) As clsEasyVector
            If Me.RowCount <> Me.ColCount Then
                Throw New clsException(clsException.Series.NotComputable, "ToVectorFromDiagonal")
            End If
            Dim ret As New clsEasyVector(Me.RowCount, direction)
            For i As Integer = 0 To Me.Count - 1
                ret(i) = Me(i)(i)
            Next
            Return ret
        End Function

        ''' <summary>
        ''' Inverse
        ''' </summary>
        ''' <param name="same_zero_value">optional:2.0E-50</param>
        ''' <returns></returns>
        Public Function Inverse(Optional ByVal same_zero_value As Double = SAME_ZERO) As clsEasyMatrix
            If Me.RowCount <> Me.ColCount Then
                Return New clsEasyMatrix(0)
            End If

            Dim n As Integer = Me.RowCount
            Dim source As New clsEasyMatrix(Me)
            Dim retInverse As New clsEasyMatrix(n, False)
            If n = 0 Then
                'nop
            ElseIf n = 1 Then
                retInverse(0)(0) = 1.0 / source(0)(0)
            ElseIf n = 2 Then
                Dim temp = Me(0)(0) * Me(1)(1) - Me(0)(1) * Me(1)(0)
                If Math.Abs(temp) < same_zero_value Then
                    Throw New clsException(clsException.Series.NotComputable, "Inverse 2x2")
                End If
                temp = 1.0 / temp
                retInverse(0)(0) = temp * Me(1)(1)
                retInverse(0)(1) = temp * -Me(0)(1)
                retInverse(1)(0) = temp * -Me(1)(0)
                retInverse(1)(1) = temp * Me(0)(0)
            ElseIf n = 3 Then
                Dim tempDet = Me.Det()
                If Math.Abs(tempDet) < same_zero_value Then
                    Throw New clsException(clsException.Series.NotComputable, "Inverse 3x3")
                End If
                retInverse(0)(0) = ((Me(1)(1) * Me(2)(2) - Me(1)(2) * Me(2)(1))) / tempDet
                retInverse(0)(1) = -((Me(0)(1) * Me(2)(2) - Me(0)(2) * Me(2)(1))) / tempDet
                retInverse(0)(2) = ((Me(0)(1) * Me(1)(2) - Me(0)(2) * Me(1)(1))) / tempDet
                retInverse(1)(0) = -((Me(1)(0) * Me(2)(2) - Me(1)(2) * Me(2)(0))) / tempDet
                retInverse(1)(1) = ((Me(0)(0) * Me(2)(2) - Me(0)(2) * Me(2)(0))) / tempDet
                retInverse(1)(2) = -((Me(0)(0) * Me(1)(2) - Me(0)(2) * Me(1)(0))) / tempDet
                retInverse(2)(0) = ((Me(1)(0) * Me(2)(1) - Me(1)(1) * Me(2)(0))) / tempDet
                retInverse(2)(1) = -((Me(0)(0) * Me(2)(1) - Me(0)(1) * Me(2)(0))) / tempDet
                retInverse(2)(2) = ((Me(0)(0) * Me(1)(1) - Me(0)(1) * Me(1)(0))) / tempDet
            Else
                Dim plu = Me.PLU()
                For j = 0 To n - 1
                    Dim y = New clsEasyVector(n)
                    Dim b = plu.P(j)
                    For i = 0 To n - 1
                        Dim s = 0.0
                        Dim k As Integer = 0
                        For k = 0 To i - 1
                            s += plu.L(i)(k) * y(k)
                        Next
                        y(k) = b(i) - s
                    Next
                    For i = n - 1 To 0 Step -1
                        Dim s = 0.0
                        For k = i + 1 To n - 1
                            s += plu.U(i)(k) * retInverse(k)(j)
                        Next
                        retInverse(i)(j) = (y(i) - s) / plu.U(i)(i)
                    Next
                Next
            End If

            Return retInverse
        End Function

        ''' <summary>
        ''' For Debug
        ''' </summary>
        ''' <param name="ai_preci"></param>
        ''' <remarks></remarks>
        Public Sub PrintValue(Optional ByVal ai_preci As Integer = 1, Optional ByVal name As String = "")
            Dim str As New System.Text.StringBuilder()
            If String.IsNullOrEmpty(name) = False Then
                str.Append(String.Format("{0} =", name) & Environment.NewLine)
            Else
                str.Append("Mat =" & Environment.NewLine)
            End If
            For Each vec As clsEasyVector In Me
                For i As Integer = 0 To vec.Count - 1
                    str.Append(vec(i).ToString("F" & ai_preci.ToString()) & ControlChars.Tab)
                Next
                str.Append(Environment.NewLine)
            Next
            str.Append(Environment.NewLine)
            Console.Write(str.ToString())
        End Sub

        ''' <summary>
        ''' To Vector
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToVector() As clsEasyVector
            If Me.RowCount = 1 Then
                Return Me.Row(0)
            ElseIf Me.ColCount = 1 Then
                Return Me.Column(0)
            End If

            Throw New clsException(clsException.Series.NotComputable, "Matrix")
        End Function

        ''' <summary>
        ''' 正方行列判定
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsSquare() As Boolean
            If Me.Count = 0 Then
                Return False
            End If

            If Me.Count = Me(0).Count Then
                Return True
            End If

            Return False
        End Function

        ''' <summary>
        ''' Cholesky decomposition A=LL^T
        ''' </summary>
        ''' <returns></returns>
        Public Function Cholesky() As clsEasyMatrix
            If Me.IsSquare() = False Then
                Throw New clsException(clsException.Series.NotComputable, "Cholesky() not Square")
            End If

            Dim ret As New MathUtil.clsEasyMatrix(Me.RowCount)
            Dim n = CInt(Math.Sqrt(ret.RowCount * ret.ColCount))
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To i
                    If j = i Then
                        Dim sum = 0.0
                        For k As Integer = 0 To j
                            sum += ret(j)(k) * ret(j)(k)
                        Next
                        ret(j)(j) = Math.Sqrt(Me(j)(j) - sum)
                    Else
                        Dim sum = 0.0
                        For k As Integer = 0 To j
                            sum += ret(i)(k) * ret(j)(k)
                        Next
                        ret(i)(j) = 1.0 / ret(j)(j) * (Me(i)(j) - sum)
                    End If
                Next
            Next

            Return ret
        End Function

        ''' <summary>
        ''' To Tridiagonal matrix using Householder transform 三重対角行列
        ''' </summary>
        ''' <param name="sourceMat"></param>
        ''' <returns></returns>
        Public Function ToTridiagonalMatrix(ByVal sourceMat As clsEasyMatrix, Optional ByVal eps As Double = MachineEpsiron) As clsEasyMatrix
            'ref
            'ハウスホルダー変換
            'http://www.slis.tsukuba.ac.jp/~fujisawa.makoto.fu/cgi-bin/wiki/index.php?%B8%C7%CD%AD%C3%CD/%B8%C7%CD%AD%A5%D9%A5%AF%A5%C8%A5%EB

            Dim tempMat = New clsEasyMatrix(sourceMat)
            Dim n As Integer = tempMat.Count
            Dim u = New clsEasyVector(n)
            Dim v = New clsEasyVector(n)
            Dim q = New clsEasyVector(n)

            For k As Integer = 0 To n - 2
                's
                Dim s As Double = 0.0
                For i = k + 1 To n - 1
                    s += tempMat(i)(k) * tempMat(i)(k)
                Next

                ' | -
                ' |a10 -
                ' |   a21
                Dim temp As Double = tempMat(k + 1)(k)
                If temp >= 0 Then
                    s = Math.Sqrt(s)
                Else
                    s = -Math.Sqrt(s)
                End If

                ' |x-y|
                Dim alpha = Math.Sqrt(2.0 * s * (s - temp))
                If clsMathUtil.IsCloseToZero(alpha) = True Then
                    Continue For
                End If

                'u
                u(k + 1) = (temp - s) / alpha
                For i = k + 2 To n - 1
                    u(i) = tempMat(i)(k) / alpha
                Next

                'Au
                q(k) = alpha / 2.0
                For i = k + 1 To n - 1
                    q(i) = 0.0
                    For j = k + 1 To n - 1
                        q(i) += tempMat(i)(j) * u(j)
                    Next
                Next

                'v=2(Au-uu^T(Au))
                alpha = 0.0
                'uu^T
                For i = k + 1 To n - 1
                    alpha += u(i) * q(i)
                Next
                v(k) = 2.0 * q(k)
                For i = k + 1 To n - 1
                    v(i) = 2.0 * (q(i) - alpha * u(i))
                Next

                'A = PAP
                ' | -    a02
                ' |    -
                ' |a20    -
                tempMat(k)(k + 1) = s
                tempMat(k + 1)(k) = s
                For i = k + 2 To n - 1
                    tempMat(k)(i) = 0.0
                    tempMat(i)(k) = 0.0
                Next
                For i = k + 1 To n - 1
                    tempMat(i)(i) = tempMat(i)(i) - 2.0 * u(i) * v(i)
                    For j = i + 1 To n - 1
                        Dim tempVal = tempMat(i)(j) - u(i) * v(j) - v(i) * u(j)
                        tempMat(i)(j) = tempVal
                        tempMat(j)(i) = tempVal
                    Next
                Next
            Next

            Return tempMat
        End Function

        ''' <summary>
        ''' Eigen decomposition using Jacobi Method. 対象行列のみ
        ''' Memo: A = V*D*V−1, D is diag(eigen value1 ... eigen valueN), V is eigen vectors. V is orthogonal matrix.
        ''' </summary>
        ''' <param name="Iteration">default iteration:1000</param>
        ''' <param name="Conversion">default conversion:1.0e-16</param>
        ''' <returns></returns>
        Public Function Eigen(Optional ByVal Iteration As Integer = 1000,
                              Optional ByVal Conversion As Double = 0.0000000000000001) As Eigen
            Dim size = Me.ColCount()
            Dim retEigenMat = New clsEasyMatrix(Me)
            Dim rotate = New clsEasyMatrix(size, True)
            Dim isConversion = False
            Dim rowIdx() = New Integer(size * 4 - 1) {}
            Dim colIdx() = New Integer(size * 4 - 1) {}
            Dim value() = New Double(size * 4 - 1) {}

            'iteration
            For itr As Integer = 0 To Iteration - 1
                'find abs max value without diag
                Dim max = Math.Abs(retEigenMat(0)(1))
                Dim p As Integer = 0
                Dim q As Integer = 1
                For i As Integer = 0 To size - 1
                    For j As Integer = i + 1 To size - 1
                        If max < Math.Abs(retEigenMat(i)(j)) Then
                            max = Math.Abs(retEigenMat(i)(j))
                            p = i
                            q = j
                        End If
                    Next
                Next

                'check conversion
                If max < Conversion Then
                    isConversion = True
                    Exit For
                End If

                'debug
                'Console.WriteLine("Itr = {0} Max = {1} p={2} q={3}", itr, max, p, q)
                'B.PrintValue(name:="B")
                'R.PrintValue(name:="R")

                Dim temp_pp = retEigenMat(p)(p)
                Dim temp_qq = retEigenMat(q)(q)
                Dim temp_pq = retEigenMat(p)(q)
                Dim theta = 0.0
                Dim diff = temp_pp - temp_qq
                If MathUtil.clsMathUtil.IsCloseToZero(diff) = True Then
                    theta = Math.PI / 4.0
                Else
                    theta = Math.Atan(-2.0 * temp_pq / diff) * 0.5
                End If

                Dim D = New clsEasyMatrix(retEigenMat)
                Dim cosTheta = Math.Cos(theta)
                Dim sinTheta = Math.Sin(theta)
                For i As Integer = 0 To size - 1
                    Dim temp = 0.0
                    temp = retEigenMat(p)(i) * cosTheta - retEigenMat(q)(i) * sinTheta
                    D(p)(i) = temp
                    D(i)(p) = temp
                    temp = retEigenMat(p)(i) * sinTheta + retEigenMat(q)(i) * cosTheta
                    D(i)(q) = temp
                    D(q)(i) = temp
                Next
                Dim cosThetasinTheta = cosTheta * cosTheta - sinTheta * sinTheta
                Dim tempA = (temp_pp + temp_qq) / 2.0
                Dim tempB = (temp_pp - temp_qq) / 2.0
                Dim tempC = temp_pq * (sinTheta * cosTheta) * 2.0
                D(p)(p) = tempA + tempB * cosThetasinTheta - tempC
                D(q)(q) = tempA - tempB * cosThetasinTheta + tempC
                D(p)(q) = 0.0
                D(q)(p) = 0.0
                retEigenMat = D

                'expand
                'Dim cosTheta = Math.Cos(theta)
                'Dim sinTheta = Math.Sin(theta)
                'For k As Integer = 0 To size - 1
                '    Dim idx As Integer = k * 4
                '    'store value
                '    Dim temp = 0.0
                '    temp = retEigenMat(p)(k) * cosTheta - retEigenMat(q)(k) * sinTheta
                '    value(idx) = temp
                '    value(idx + 1) = temp
                '    rowIdx(idx) = p
                '    colIdx(idx) = k
                '    rowIdx(idx + 1) = k
                '    colIdx(idx + 1) = p
                '    temp = retEigenMat(p)(k) * sinTheta + retEigenMat(q)(k) * cosTheta
                '    value(idx + 2) = temp
                '    value(idx + 3) = temp
                '    rowIdx(idx + 2) = k
                '    colIdx(idx + 2) = q
                '    rowIdx(idx + 3) = q
                '    colIdx(idx + 3) = k
                'Next
                'Dim cosThetasinTheta = cosTheta * cosTheta - sinTheta * sinTheta
                'Dim tempA = (retEigenMat(p)(p) + retEigenMat(q)(q)) / 2.0
                'Dim tempB = (retEigenMat(p)(p) - retEigenMat(q)(q)) / 2.0
                'Dim tempC = retEigenMat(p)(q) * (sinTheta * cosTheta) * 2.0
                'For k As Integer = 0 To value.Length - 1
                '    retEigenMat(rowIdx(k))(colIdx(k)) = value(k)
                'Next
                'retEigenMat(p)(p) = tempA + tempB * cosThetasinTheta - tempC
                'retEigenMat(q)(q) = tempA - tempB * cosThetasinTheta + tempC
                'retEigenMat(p)(q) = 0.0
                'retEigenMat(q)(p) = 0.0

                'rotate
                Dim rotateNew = New clsEasyMatrix(size, True)
                rotateNew(p)(p) = cosTheta
                rotateNew(p)(q) = sinTheta
                rotateNew(q)(p) = -sinTheta
                rotateNew(q)(q) = cosTheta
                rotate = rotate * rotateNew
            Next

            Return New Eigen(retEigenMat.ToDiagonalVector(), rotate, isConversion)
        End Function

        ''' <summary>
        ''' LU decomposition (using PLU())
        ''' </summary>
        ''' <param name="eps">2.20*10^-16</param>
        ''' <returns></returns>
        Public Function LU(Optional ByVal eps As Double = MachineEpsiron) As LU
            Return Me.PLU(eps)
        End Function

        ''' <summary>
        ''' PLU decomposition
        ''' </summary>
        ''' <param name="eps">2.20*10^-16</param>
        ''' <returns></returns>
        Public Function PLU( Optional ByVal eps As Double = MachineEpsiron) As LU
            'Refference
            '[1]C言語による標準アルゴリズム事典
            '[2]NUMERICAL RECIPES in C 日本語版 C言語による数値計算のレシピ

            Dim n = Me.ColCount
            Dim source = New clsEasyMatrix(Me)
            Dim matL = New clsEasyMatrix(n, True)
            Dim matU = New clsEasyMatrix(n, True)
            Dim matP = New clsEasyMatrix(n, True)

            Dim det = 1.0
            Dim weight = New Double(n - 1) {}
            Dim indx = New Double(n - 1) {}

            'Find absolute max value of each row
            For i = 0 To n - 1
                indx(i) = i
                Dim absValue = 0.0
                For j = 0 To n - 1
                    Dim temp = Math.Abs(Me(i)(j))
                    If temp >= absValue Then
                        absValue = temp
                    End If
                Next
                '列要素の絶対最大値が0に近い場合
                If clsMathUtil.IsCloseToZero(absValue, eps) Then
                    Throw New clsException(clsException.Series.NotComputable, "PLU() singular matrix")
                End If
                weight(i) = 1.0 / absValue
            Next

            'calc PLU
            For j = 0 To n - 1
                For i = 0 To j - 1
                    Dim sum = source(i)(j)
                    For k = 0 To i - 1
                        sum -= source(i)(k) * source(k)(j)
                    Next
                    source(i)(j) = sum
                Next

                Dim imax = 0
                Dim big = -1.0
                For i = j To n - 1
                    Dim sum = source(i)(j)
                    For k = 0 To j - 1
                        sum -= source(i)(k) * source(k)(j)
                    Next
                    source(i)(j) = sum
                    Dim dum = weight(i) * Math.Abs(sum)
                    If dum > big Then
                        big = dum
                        imax = i
                    End If
                Next

                'interchange row
                If j <> imax Then
                    clsMathUtil.SwapRow(source, imax, j)
                    clsMathUtil.SwapRow(matP, imax, j)
                    weight(imax) = weight(j)

                    'change sign
                    det = -det

                    'swap row info
                    indx(imax) = indx(j)
                    indx(j) = imax
                End If
                'diagonal value is close to 0.
                If clsMathUtil.IsCloseToZero(source(j)(j), eps) Then
                    source(j)(j) = SAME_ZERO
                End If

                'calc det
                det *= source(j)(j)

                If j <> n Then
                    Dim dum = 1.0 / (source(j)(j))
                    For i = j + 1 To n - 1
                        source(i)(j) *= dum
                    Next
                End If
            Next

            'transopose
            matP = matP.T()

            'replace
            For i = 1 To n - 1
                For j = 0 To i - 1
                    matL(i)(j) = source(i)(j)
                Next
            Next
            For i = 0 To n - 1
                For j = i To n - 1
                    matU(i)(j) = source(i)(j)
                Next
            Next

            Return New LU(matP, matL, matU, det, indx)
        End Function

        ''' <summary>
        ''' PLU decomposition
        ''' </summary>
        ''' <param name="eps">2.20*10^-16</param>
        ''' <returns></returns>
        Public Function PLU_CALGO(Optional ByVal eps As Double = MachineEpsiron) As LU
            'Refference
            '[1]C言語による標準アルゴリズム事典
            '[2]NUMERICAL RECIPES in C 日本語版 C言語による数値計算のレシピ

            Dim n = Me.ColCount
            Dim source = New clsEasyMatrix(Me)
            Dim matL = New clsEasyMatrix(n, True)
            Dim matU = New clsEasyMatrix(n, True)
            Dim matP = New clsEasyMatrix(n, True)

            Dim det = 1.0
            Dim imax As Integer = 0
            Dim weight = New Double(n - 1) {}
            Dim ip = New Integer(n - 1) {}
            Dim j = 0
            Dim ik = 0
            Dim ii = 0

            'Find absolute max value of each row
            For k = 0 To n - 1
                ip(k) = k
                Dim absValue = 0.0
                For j = 0 To n - 1
                    Dim temp = Math.Abs(source(k)(j))
                    If temp > absValue Then
                        absValue = temp
                    End If
                Next
                If absValue = 0.0 Then
                    'If absValue < SAME_ZERO Then
                    Throw New clsException(clsException.Series.NotComputable, "singular matrix")
                End If
                weight(k) = 1.0 / absValue
            Next

            'calc PLU
            For k = 0 To n - 1
                Dim u = -1.0
                For i = k To n - 1
                    ii = ip(i)
                    Dim t = Math.Abs(source(ii)(k) * weight(ii))
                    If t > u Then
                        u = t
                        j = i
                    End If
                Next
                ik = ip(j)

                'interchange row
                If j <> k Then
                    ip(j) = ip(k)
                    ip(k) = ik
                    clsMathUtil.SwapRow(matP, j, k)
                    det = -det
                End If
                u = source(ik)(k)
                If clsMathUtil.IsCloseToZero(u) Then
                    u = SAME_ZERO
                End If
                det *= u

                'gauss eimination
                For i = k + 1 To n - 1
                    ii = ip(i)
                    source(ii)(k) /= u
                    Dim t = source(ii)(k)
                    For j = k + 1 To n - 1
                        source(ii)(j) -= t * source(ik)(j)
                    Next
                Next
            Next

            source = matP * source
            matP = matP.T()

            'replace
            For i = 1 To n - 1
                For j = 0 To i - 1
                    matL(i)(j) = source(i)(j)
                Next
            Next
            For i = 0 To n - 1
                For j = i To n - 1
                    matU(i)(j) = source(i)(j)
                Next
            Next

            Return New LU(matP, matL, matU, det)
        End Function

        ''' <summary>
        ''' Resize matrix dimension
        ''' </summary>
        ''' <param name="row">number of row</param>
        ''' <param name="col">number of col</param>
        Public Sub Resize(ByVal row As Integer, ByVal col As Integer)
            Me.Clear()
            If row = 0 OrElse col = 0 Then
                Return
            End If
            For i As Integer = 0 To row - 1
                Me.Add(New List(Of Double)(New Double(col - 1) {}))
            Next
        End Sub

        ''' <summary>
        ''' find Max value
        ''' </summary>
        ''' <returns></returns>
        Public Function MaxValue() As Double
            Dim retMax As Double = 0.0
            For Each vec In Me
                For Each value In vec
                    If retMax > value Then
                        retMax = value
                    End If
                Next
            Next
            Return retMax
        End Function

        ''' <summary>
        ''' find Absolute Max value
        ''' </summary>
        ''' <returns></returns>
        Public Function MaxAbs() As Double
            Dim retMaxabs As Double = 0.0
            For Each vec In Me
                For Each value In vec
                    Dim absValue = Math.Abs(value)
                    If retMaxabs > absValue Then
                        retMaxabs = absValue
                    End If
                Next
            Next
            Return retMaxabs
        End Function
#End Region

#Region "Property"
        ''' <summary>
        ''' Get Row count
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property RowCount() As Integer
            Get
                Return Me.Count
            End Get
        End Property

        ''' <summary>
        ''' Get Collumn count
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ColCount() As Integer
            Get
                If Me.Count = 0 Then
                    Return 0
                Else
                    Return Me(0).Count
                End If
            End Get
        End Property

        ''' <summary>
        ''' Row accessor
        ''' </summary>
        ''' <param name="ai_rowIndex"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Row(ByVal ai_rowIndex As Integer) As clsEasyVector
            Get
                Return New clsEasyVector(Me(ai_rowIndex))
            End Get
            Set(ByVal value As clsEasyVector)
                Me(ai_rowIndex) = New clsEasyVector(value)
            End Set
        End Property

        ''' <summary>
        ''' Column accessor
        ''' </summary>
        ''' <param name="ai_colIndex"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Column(ByVal ai_colIndex As Integer) As clsEasyVector
            Get
                Dim temp(Me.RowCount - 1) As Double
                For i As Integer = 0 To temp.Length - 1
                    temp(i) = Me.Row(i)(ai_colIndex)
                Next
                Dim tempVector As New clsEasyVector(temp)
                tempVector.Direction = clsEasyVector.VectorDirection.COL
                Return tempVector
            End Get
            Set(ByVal value As clsEasyVector)
                For i As Integer = 0 To value.Count - 1
                    Me(i)(ai_colIndex) = value(i)
                Next
            End Set
        End Property

        ''' <summary>
        ''' To List(Of List(Of Double))
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RawMatrix() As List(Of List(Of Double))
            Get
                Return Me
            End Get
            Set(ByVal value As List(Of List(Of Double)))
                Me.Clear()
                Me.AddRange(value)
            End Set
        End Property

        ''' <summary>
        ''' Multiply the diagonal values(a_00 * a_11 * ...)
        ''' </summary>
        ''' <returns></returns>
        Public Function ProductDiagonal() As Double
            If IsSquare() = False Then
                Return 0.0
            End If

            Dim ret = 1.0
            Dim n = Me.RowCount()
            For i As Integer = 0 To n - 1
                ret *= Me(i)(i)
            Next
            Return ret
        End Function
#End Region

#Region "Public shared"
        ''' <summary>
        ''' solve(Ax=b)
        ''' </summary>
        ''' <param name="luMat">LU decomposition of A matrix</param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Shared Function Solve(ByRef luMat As LU, ByRef b As clsEasyVector) As clsEasyVector
            Return clsEasyMatrix.Solve(luMat.P, luMat.L, luMat.U, b)
        End Function

        ''' <summary>
        ''' solve(Ax=b)
        ''' </summary>
        ''' <param name="matP">pivot matrix(LU decomposition of A matrix)</param>
        ''' <param name="matL">lower triangle matrix(LU decomposition of A matrix)</param>
        ''' <param name="matU">upper triangle matrix(LU decomposition of A matrix)</param>
        ''' <param name="vecB"></param>
        ''' <returns>x</returns>
        Public Shared Function Solve(ByRef matP As clsEasyMatrix,
                                     ByRef matL As clsEasyMatrix,
                                     ByRef matU As clsEasyMatrix,
                                     ByRef vecB As clsEasyVector) As clsEasyVector
            Dim n = matP.ColCount
            Dim x = New clsEasyVector(n)
            Dim y = New clsEasyVector(n)
            Dim b = vecB * matP

            For i = 0 To n - 1
                Dim s = 0.0
                Dim j As Integer = 0
                For j = 0 To i - 1
                    s += matL(i)(j) * y(j)
                Next
                y(j) = b(i) - s
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
#End Region

#Region "Private"
        ''' <summary>
        ''' Check Dimension
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function IsSameDimension(ByRef ai_source As clsEasyMatrix, ByRef ai_dest As clsEasyMatrix) As Boolean
            If ai_source.RowCount <> ai_dest.RowCount Then
                Return False
            End If
            If ai_source.ColCount() <> ai_dest.ColCount Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>
        ''' Check Matrix Vector
        ''' </summary>
        ''' <param name="ai_matrix"></param>
        ''' <param name="ai_vector"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function IsComputableMatrixVector(ByRef ai_matrix As clsEasyMatrix, ByRef ai_vector As clsEasyVector) As Boolean
            If (ai_matrix.ColCount = 1) AndAlso (ai_matrix.RowCount = ai_vector.Count) Then
                Return True
            ElseIf (ai_matrix.RowCount = 1) AndAlso (ai_matrix.ColCount = ai_vector.Count) Then
                Return True
            End If

            Return False
        End Function

#End Region
    End Class

End Namespace

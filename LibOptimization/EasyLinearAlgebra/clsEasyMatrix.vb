Namespace MathUtil
    ''' <summary>
    ''' Matrix class
    ''' </summary>
    ''' <remarks>
    ''' Inherits List(Of List(Of Double))
    ''' 
    ''' TODO:
    ''' LU, Solve ,SVD
    ''' </remarks>
    Public Class clsEasyMatrix : Inherits List(Of List(Of Double))
        Public Const SAME_ZERO As Double = 2.0E-50 '2^-50
        Public Const Epsiron As Double = 0.0000000000000001 '1.0^-16

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
            If IsSameDimension(ai_source, ai_dest) = True Then
                '[M*M] X [M*M]
                Dim size = ai_source.RowCount
                Dim ret As New clsEasyMatrix(size)

                '並列化 .NET4
                'Threading.Tasks.Parallel.For(0, size - 1,
                '                             Sub(i)
                '                                 For j As Integer = 0 To size - 1
                '                                     For k As Integer = 0 To size - 1
                '                                         Dim tempA = ai_source(i)(k)
                '                                         Dim tempB = ai_dest(k)(j)
                '                                         ret(i)(j) += tempA * tempB
                '                                     Next
                '                                 Next
                '                             End Sub)

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
            'OK
            ' |a11 a12| * |v1| = cv1
            ' |a21 a22|   |v2|   cv2
            '
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
            If vec.Direction <> clsEasyVector.VectorDirection.COL Then
                Throw New clsException(clsException.Series.NotComputable, "Matrix * Vector - size error")
            End If

            '計算
            Dim vSize As Integer = mat.RowCount '行列の行サイズ
            Dim ret As New clsEasyVector(vSize, clsEasyVector.VectorDirection.COL)
            For i As Integer = 0 To vSize - 1
                Dim sum As Double = 0.0
                For j As Integer = 0 To mat.ColCount - 1
                    sum += mat(i)(j) * vec(j)
                Next
                ret(i) = sum
            Next
            Return ret
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

            '計算
            Dim vSize As Integer = mat.ColCount '行列の行サイズ
            Dim ret As New clsEasyVector(vSize, clsEasyVector.VectorDirection.ROW)
            For j As Integer = 0 To vSize - 1
                Dim sum As Double = 0.0
                For i As Integer = 0 To mat.RowCount - 1
                    sum += vec(i) * mat(i)(j)
                Next
                ret(j) = sum
            Next
            Return ret
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
            For rowIndex As Integer = 0 To ret.RowCount - 1
                ret.Row(rowIndex) = Me.Column(rowIndex)
            Next
            Return ret
        End Function

        ''' <summary>
        ''' Determinant
        ''' </summary>
        ''' <remarks></remarks>
        Public Function Det(Optional ByVal ai_isDebug As Boolean = False) As Double
            If Me.RowCount <> Me.ColCount Then
                Return 0
            End If
            Return CalcDeterminant(Me, Me.RowCount(), ai_isDebug)
        End Function

        ''' <summary>
        ''' conversion Diagonal matrix
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
        ''' 対角成分をベクトルに変換
        ''' </summary>
        ''' <param name="direction">direction of vector</param>
        ''' <returns></returns>
        Public Function ToVectorFromDiagonal(Optional ByVal direction As clsEasyVector.VectorDirection = clsEasyVector.VectorDirection.ROW) As clsEasyVector
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
        ''' <remarks></remarks>
        Public Function Inverse() As clsEasyMatrix
            If Me.RowCount <> Me.ColCount Then
                Return New clsEasyMatrix(0)
            End If

            Dim n As Integer = Me.RowCount
            Dim source As New clsEasyMatrix(Me)
            Dim retInverse As New clsEasyMatrix(n, True)
            If n = 0 Then
                'nop
            ElseIf n = 1 Then
                source(0)(0) = 1.0 / source(0)(0)
            ElseIf n = 2 Then
                'Dim det As Double = source.Det()
                'If SAME_ZERO > det Then
                '    Return New clsShoddyMatrix(n, True)
                'End If
                retInverse(0)(0) = (1.0 / Det()) * Me(1)(1)
                retInverse(0)(1) = (1.0 / Det()) * -Me(0)(1)
                retInverse(1)(0) = (1.0 / Det()) * -Me(1)(0)
                retInverse(1)(1) = (1.0 / Det()) * Me(0)(0)
            ElseIf n = 3 Then
                'Dim det As Double = source.Det()
                'If SAME_ZERO > det Then
                '    Return New clsShoddyMatrix(n, True)
                'End If
                retInverse(0)(0) = (1.0 / Det()) * (Me(1)(1) * Me(2)(2) - Me(1)(2) * Me(2)(1))
                retInverse(0)(1) = (1.0 / Det()) * (Me(0)(2) * Me(2)(1) - Me(0)(1) * Me(2)(2))
                retInverse(0)(2) = (1.0 / Det()) * (Me(0)(1) * Me(1)(2) - Me(0)(2) * Me(1)(1))
                retInverse(1)(0) = (1.0 / Det()) * (Me(1)(2) * Me(2)(0) - Me(1)(0) * Me(2)(2))
                retInverse(1)(1) = (1.0 / Det()) * (Me(0)(0) * Me(2)(2) - Me(0)(2) * Me(2)(0))
                retInverse(1)(2) = (1.0 / Det()) * (Me(0)(2) * Me(1)(0) - Me(0)(0) * Me(1)(2))
                retInverse(2)(0) = (1.0 / Det()) * (Me(1)(0) * Me(2)(1) - Me(1)(1) * Me(2)(0))
                retInverse(2)(1) = (1.0 / Det()) * (Me(0)(1) * Me(2)(0) - Me(0)(0) * Me(2)(1))
                retInverse(2)(2) = (1.0 / Det()) * (Me(0)(0) * Me(1)(1) - Me(0)(1) * Me(1)(0))
            Else
                'Dim det As Double = source.Det() 'omoi...
                'If SAME_ZERO > det Then
                '    Return New clsShoddyMatrix(n, True)
                'End If
                'Gauss elimination with pivot select
                For i As Integer = 0 To n - 1
                    'diagonal element
                    Dim ip As Integer = i 'maxIndex
                    Dim amax As Double = source(i)(i) 'maxValue
                    For index As Integer = 0 To n - 1
                        Dim temp As Double = Math.Abs(source(index)(i))
                        If temp > amax Then
                            amax = temp
                            ip = index
                        End If
                    Next

                    'check 正則性の判定
                    If amax < SAME_ZERO Then
                        Return New clsEasyMatrix(Me.ColCount)
                    End If

                    'change row
                    If i <> ip Then
                        source.SwapRow(i, ip)
                        retInverse.SwapRow(i, ip)
                    End If

                    'discharge calculation
                    Dim tempValue As Double = 1.0 / source(i)(i)
                    For j As Integer = 0 To n - 1
                        source(i)(j) *= tempValue
                        retInverse(i)(j) *= tempValue
                    Next
                    For j As Integer = 0 To n - 1
                        If i <> j Then
                            tempValue = source(j)(i)
                            For k As Integer = 0 To n - 1
                                source(j)(k) -= source(i)(k) * tempValue
                                retInverse(j)(k) -= retInverse(i)(k) * tempValue
                            Next
                        End If
                    Next
                Next
            End If

            Return retInverse
        End Function

        ''' <summary>
        ''' Swap Row
        ''' </summary>
        ''' <param name="ai_sourceRowIndex"></param>
        ''' <param name="ai_destRowIndex"></param>
        ''' <remarks></remarks>
        Public Sub SwapRow(ByVal ai_sourceRowIndex As Integer, ByVal ai_destRowIndex As Integer)
            Dim temp As clsEasyVector = Me.Row(ai_sourceRowIndex)
            Me.Row(ai_sourceRowIndex) = Me.Row(ai_destRowIndex)
            Me.Row(ai_destRowIndex) = temp
        End Sub

        ''' <summary>
        ''' Swap Col
        ''' </summary>
        ''' <param name="ai_sourceColIndex"></param>
        ''' <param name="ai_destColIndex"></param>
        ''' <remarks></remarks>
        Public Sub SwapCol(ByVal ai_sourceColIndex As Integer, ByVal ai_destColIndex As Integer)
            Dim temp As clsEasyVector = Me.Column(ai_sourceColIndex)
            Me.Column(ai_sourceColIndex) = Me.Row(ai_destColIndex)
            Me.Column(ai_destColIndex) = temp
        End Sub

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
        ''' 対角成分で行列を作る
        ''' </summary>
        ''' <param name="vec"></param>
        ''' <returns></returns>
        Public Shared Function ToDiagonalMatrix(ByVal vec As clsEasyVector) As clsEasyMatrix
            Dim ret = New clsEasyMatrix(vec.Count)
            For i As Integer = 0 To ret.Count - 1
                ret(i)(i) = vec(i)
            Next
            Return ret
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
        ''' To Tridiagonal matrix using Householder transform
        ''' 三重対角行列
        ''' </summary>
        ''' <param name="sourceMat"></param>
        ''' <returns></returns>
        Public Shared Function ToTridiagonalMatrix(ByVal sourceMat As clsEasyMatrix) As clsEasyMatrix
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
                If Math.Abs(alpha) < 0.0000000000000001 Then
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
        ''' Eigen decomposition using Jacobi Method.
        ''' Memo: A = V*D*V−1, D is diag(eigen value1 ... eigen valueN), V is eigen vectors. V is orthogonal matrix.
        ''' </summary>
        ''' <param name="inMat">source matrix</param>
        ''' <param name="eigenvalues">eigen vaues(Vector)</param>
        ''' <param name="eigenVectors">eigen vectors(Matrix)</param>
        ''' <param name="Iteration">default iteration:1000</param>
        ''' <param name="Conversion">default conversion:1.0e-16</param>
        ''' <returns>True:success conversion. False:not conversion</returns>
        Public Shared Function Eigen(ByRef inMat As clsEasyMatrix,
                                     ByRef eigenvalues As clsEasyVector, ByRef eigenVectors As clsEasyMatrix,
                                     Optional ByVal Iteration As Integer = 1000,
                                     Optional ByVal Conversion As Double = 0.0000000000000001,
                                     Optional ByRef SuspendMat As clsEasyMatrix = Nothing) As Boolean
            Dim size = inMat.ColCount()
            Dim retEigenMat = New clsEasyMatrix(inMat)
            Dim rotate = New clsEasyMatrix(size, True)

            Dim rowIdx() = New Integer(size * 4 - 1) {}
            Dim colIdx() = New Integer(size * 4 - 1) {}
            Dim value() = New Double(size * 4 - 1) {}

            'iteration
            Dim isConversion As Boolean = False
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

                Dim theta = 0.0
                Dim diff = retEigenMat(p)(p) - retEigenMat(q)(q)
                If Math.Abs(diff) = 0.0 Then
                    theta = Math.PI / 4.0
                Else
                    theta = Math.Atan(-2.0 * retEigenMat(p)(q) / diff) * 0.5
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
                Dim tempA = (retEigenMat(p)(p) + retEigenMat(q)(q)) / 2.0
                Dim tempB = (retEigenMat(p)(p) - retEigenMat(q)(q)) / 2.0
                Dim tempC = retEigenMat(p)(q) * (sinTheta * cosTheta) * 2.0
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
                rotateNew(p)(p) = Math.Cos(theta)
                rotateNew(p)(q) = Math.Sin(theta)
                rotateNew(q)(p) = -Math.Sin(theta)
                rotateNew(q)(q) = Math.Cos(theta)
                rotate = rotate * rotateNew
            Next

            '途中結果
            SuspendMat = retEigenMat

            '値を代入
            eigenvalues = retEigenMat.ToVectorFromDiagonal()
            eigenVectors = rotate

            Return isConversion
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
#End Region

#Region "Private"
        ''' <summary>
        ''' Check Dimension
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function IsSameDimension(ByVal ai_source As clsEasyMatrix, ByVal ai_dest As clsEasyMatrix) As Boolean
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
        Private Shared Function IsComputableMatrixVector(ByVal ai_matrix As clsEasyMatrix, ByVal ai_vector As clsEasyVector) As Boolean
            If (ai_matrix.ColCount = 1) AndAlso (ai_matrix.RowCount = ai_vector.Count) Then
                Return True
            ElseIf (ai_matrix.RowCount = 1) AndAlso (ai_matrix.ColCount = ai_vector.Count) Then
                Return True
            End If

            Return False
        End Function

        ''' <summary>
        ''' Determinant(Recursive)
        ''' </summary>
        ''' <param name="ai_clsMatrix"></param>
        ''' <param name="ai_dim"></param>
        ''' <param name="ai_isDebug"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CalcDeterminant(ByVal ai_clsMatrix As clsEasyMatrix,
                                         ByVal ai_dim As Integer, Optional ai_isDebug As Boolean = False) As Double
            If ai_dim = 1 Then
                Return ai_clsMatrix(0)(0)
            ElseIf ai_dim = 2 Then
                '2 order
                ' | a b |
                ' | c d | = ad-bc
                Return ai_clsMatrix(0)(0) * ai_clsMatrix(1)(1) - ai_clsMatrix(0)(1) * ai_clsMatrix(1)(0)
            ElseIf ai_dim = 3 Then
                '3 order using Sarrus rule
                Dim d As Double = 0.0
                d += ai_clsMatrix(0)(0) * ai_clsMatrix(1)(1) * ai_clsMatrix(2)(2)
                d += ai_clsMatrix(1)(0) * ai_clsMatrix(2)(1) * ai_clsMatrix(0)(2)
                d += ai_clsMatrix(2)(0) * ai_clsMatrix(0)(1) * ai_clsMatrix(1)(2)
                d -= ai_clsMatrix(2)(0) * ai_clsMatrix(1)(1) * ai_clsMatrix(0)(2)
                d -= ai_clsMatrix(1)(0) * ai_clsMatrix(0)(1) * ai_clsMatrix(2)(2)
                d -= ai_clsMatrix(0)(0) * ai_clsMatrix(2)(1) * ai_clsMatrix(1)(2)
                Return d
            Else
                'over 4 order
                Dim detVal As Double = 0.0
                Dim b As New clsEasyMatrix(ai_dim - 1)
                Dim sign As Integer = 0
                If ((ai_dim + 1) Mod (2)) = 0 Then
                    sign = 1
                Else
                    sign = -1
                End If

                For k As Integer = 0 To ai_dim - 1
                    For i As Integer = 0 To ai_dim - 2
                        For j As Integer = 0 To ai_dim - 1
                            If j = k Then
                                Continue For
                            End If
                            If j > k Then
                                b(i)(j - 1) = ai_clsMatrix(i)(j)
                            Else
                                b(i)(j) = ai_clsMatrix(i)(j)
                            End If
                        Next
                    Next
                    If ai_isDebug = True Then
                        Console.WriteLine(sign.ToString() & " " & ai_clsMatrix(ai_dim - 1)(k).ToString())
                        b.PrintValue()
                    End If
                    detVal += sign * ai_clsMatrix(ai_dim - 1)(k) * CalcDeterminant(b, ai_dim - 1)
                    sign *= -1
                Next
                Return detVal
            End If

            'other
            'number of row exchanges
            'det A = sign * det(L) * det(U)
        End Function
#End Region
    End Class

End Namespace

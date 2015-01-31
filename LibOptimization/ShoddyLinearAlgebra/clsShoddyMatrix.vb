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
    Public Class clsShoddyMatrix : Inherits List(Of List(Of Double))
        Public Const SAME_ZERO As Double = 2.0E-50 '2^-50

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
        Public Sub New(ByVal ai_base As clsShoddyMatrix)
            For i As Integer = 0 To ai_base.Count - 1
                Dim temp As New clsShoddyVector(ai_base(i))
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
                Dim temp As New clsShoddyVector(ai_dim)
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
                Me.Add(New clsShoddyVector(ai_colSize))
            Next
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_val"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_val As List(Of List(Of Double)))
            For i As Integer = 0 To ai_val.Count - 1
                Me.Add(New clsShoddyVector(ai_val(i)))
            Next
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_val"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_val()() As Double)
            For i As Integer = 0 To ai_val.Length - 1
                Me.Add(New clsShoddyVector(ai_val(i)))
            Next
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_val"></param>
        ''' <param name="ai_direction">Row or Col</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_val As List(Of Double), ByVal ai_direction As clsShoddyVector.VectorDirection)
            If ai_direction = clsShoddyVector.VectorDirection.ROW Then
                Dim temp As New clsShoddyVector(ai_val)
                Me.Add(temp)
            Else
                For i As Integer = 0 To ai_val.Count - 1
                    Me.Add(New clsShoddyVector({ai_val(i)}))
                Next
            End If
        End Sub

        Public Sub New(ByVal data As List(Of Double), ByVal order As Integer)
            Me.Clear()
            For i As Integer = 0 To order - 1
                Dim temp(order - 1) As Double
                Me.Add(New clsShoddyVector(temp))
            Next
            Dim index As Integer = 0
            For i As Integer = 0 To order - 1
                For j As Integer = 0 To order - 1
                    Me(j)(i) = data(index)
                    index += 1
                Next
            Next
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Add(Matrix + Matrix)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator +(ByVal ai_source As clsShoddyMatrix, ByVal ai_dest As clsShoddyMatrix) As clsShoddyMatrix
            If IsSameDimension(ai_source, ai_dest) = False Then
                Throw New clsException(clsException.Series.DifferRowNumberAndCollumnNumber)
            End If
            Dim ret As New clsShoddyMatrix(ai_source)
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
        Public Shared Operator +(ByVal ai_source As clsShoddyMatrix, ByVal ai_dest As clsShoddyVector) As clsShoddyVector
            If IsComputableMatrixVector(ai_source, ai_dest) = False Then
                Throw New clsException(clsException.Series.NotComputable)
            End If
            Dim ret As New clsShoddyVector(ai_dest)
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
        Public Shared Operator +(ByVal ai_source As clsShoddyVector, ByVal ai_dest As clsShoddyMatrix) As clsShoddyVector
            If IsComputableMatrixVector(ai_dest, ai_source) = False Then
                Throw New clsException(clsException.Series.NotComputable)
            End If
            Dim ret As New clsShoddyVector(ai_source)
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
        Public Shared Operator -(ByVal ai_source As clsShoddyMatrix, ByVal ai_dest As clsShoddyMatrix) As clsShoddyMatrix
            If IsSameDimension(ai_source, ai_dest) = False Then
                Throw New clsException(clsException.Series.DifferRowNumberAndCollumnNumber)
            End If
            Dim ret As New clsShoddyMatrix(ai_source)
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
        Public Shared Operator -(ByVal ai_source As clsShoddyMatrix, ByVal ai_dest As clsShoddyVector) As clsShoddyVector
            If IsComputableMatrixVector(ai_source, ai_dest) = False Then
                Throw New clsException(clsException.Series.NotComputable)
            End If
            Dim ret As New clsShoddyVector(ai_dest)
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
        Public Shared Operator -(ByVal ai_source As clsShoddyVector, ByVal ai_dest As clsShoddyMatrix) As clsShoddyVector
            If IsComputableMatrixVector(ai_dest, ai_source) = False Then
                Throw New clsException(clsException.Series.NotComputable)
            End If
            Dim ret As New clsShoddyVector(ai_source)
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
        Public Shared Operator *(ByVal ai_source As clsShoddyMatrix, ByVal ai_dest As clsShoddyMatrix) As clsShoddyMatrix
            If IsSameDimension(ai_source, ai_dest) = True Then
                '[M*M] X [M*M]
                Dim ret As New clsShoddyMatrix(ai_source.RowCount)
                For i As Integer = 0 To ret.RowCount - 1
                    For j As Integer = 0 To ret.ColCount - 1
                        For k As Integer = 0 To ret.ColCount - 1
                            ret(i)(j) += ai_source(k)(i) * ai_dest(j)(k)
                        Next
                    Next
                Next
                Return ret
            ElseIf ai_source.ColCount = ai_dest.RowCount Then
                '[M*N] X [N*O]
                Dim ret As New clsShoddyMatrix(ai_source.RowCount, ai_dest.ColCount)
                For mIndex As Integer = 0 To ret.RowCount - 1
                    For nIndex As Integer = 0 To ret.ColCount - 1
                        Dim temp As Double = 0.0
                        For i As Integer = 0 To ai_source.ColCount - 1
                            temp += ai_source(mIndex)(i) * ai_dest(i)(nIndex)
                        Next
                        ret(mIndex)(nIndex) = temp
                    Next
                Next
                Return ret
            End If

            Throw New clsException(clsException.Series.NotComputable, "Matrix * Matrix")
        End Operator

        ''' <summary>
        ''' Product( Matrix * Vector )
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Public Shared Operator *(ByVal ai_source As clsShoddyMatrix, ByVal ai_dest As clsShoddyVector) As clsShoddyVector
            Dim temp As clsShoddyMatrix = ai_source * ai_dest.ToMatrix()
            Return temp.ToVector()
        End Operator

        ''' <summary>
        ''' Product(Vector * Matrix)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Public Shared Operator *(ByVal ai_source As clsShoddyVector, ByVal ai_dest As clsShoddyMatrix) As clsShoddyVector
            Dim tempV As clsShoddyMatrix = ai_source.ToMatrix() * ai_dest
            Return tempV.ToVector()
        End Operator

        ''' <summary>
        ''' Product(value * Matrix)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator *(ByVal ai_source As Double, ByVal ai_dest As clsShoddyMatrix) As clsShoddyMatrix
            Dim ret As New clsShoddyMatrix(ai_dest)
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
        Public Shared Operator *(ByVal ai_source As clsShoddyMatrix, ByVal ai_dest As Double) As clsShoddyMatrix
            Dim ret As New clsShoddyMatrix(ai_source)
            For i As Integer = 0 To ret.RowCount() - 1
                ret.Row(i) = ai_source.Row(i) * ai_dest
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Transpose
        ''' </summary>
        ''' <remarks></remarks>
        Public Function T() As clsShoddyMatrix
            Dim ret As New clsShoddyMatrix(Me.ColCount, Me.RowCount)
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
        ''' Diagonal matrix
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Diag() As clsShoddyMatrix
            Dim ret As New clsShoddyMatrix(Me.Count)
            For i As Integer = 0 To Me.Count - 1
                ret(i)(i) = Me(i)(i)
            Next
            Return ret
        End Function

        ''' <summary>
        ''' Inverse
        ''' </summary>
        ''' <remarks></remarks>
        Public Function Inverse() As clsShoddyMatrix
            If Me.RowCount <> Me.ColCount Then
                Return New clsShoddyMatrix(0)
            End If

            Dim n As Integer = Me.RowCount
            Dim source As New clsShoddyMatrix(Me)
            Dim retInverse As New clsShoddyMatrix(n, True)
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
                        Return New clsShoddyMatrix(Me.ColCount)
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
            Dim temp As clsShoddyVector = Me.Row(ai_sourceRowIndex)
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
            Dim temp As clsShoddyVector = Me.Column(ai_sourceColIndex)
            Me.Column(ai_sourceColIndex) = Me.Row(ai_destColIndex)
            Me.Column(ai_destColIndex) = temp
        End Sub

        ''' <summary>
        ''' For Debug
        ''' </summary>
        ''' <param name="ai_preci"></param>
        ''' <remarks></remarks>
        Public Sub PrintValue(Optional ByVal ai_preci As Integer = 3)
            Dim str As New System.Text.StringBuilder()
            For Each vec As clsShoddyVector In Me
                str.Clear()
                For i As Integer = 0 To vec.Count - 1
                    str.Append(vec(i).ToString("F" & ai_preci.ToString()) & ControlChars.Tab)
                Next
                str.AppendLine("")
                Console.Write(str.ToString())
            Next
            Console.WriteLine()
        End Sub

        ''' <summary>
        ''' To Vector
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToVector() As clsShoddyVector
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
                Return Me(0).Count
            End Get
        End Property

        ''' <summary>
        ''' Row accessor
        ''' </summary>
        ''' <param name="ai_rowIndex"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Row(ByVal ai_rowIndex As Integer) As clsShoddyVector
            Get
                Return New clsShoddyVector(Me(ai_rowIndex))
            End Get
            Set(ByVal value As clsShoddyVector)
                Me(ai_rowIndex) = New clsShoddyVector(value)
            End Set
        End Property

        ''' <summary>
        ''' Column accessor
        ''' </summary>
        ''' <param name="ai_colIndex"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Column(ByVal ai_colIndex As Integer) As clsShoddyVector
            Get
                Dim temp(Me.RowCount - 1) As Double
                For i As Integer = 0 To temp.Length - 1
                    temp(i) = Me.Row(i)(ai_colIndex)
                Next
                Dim tempVector As New clsShoddyVector(temp)
                tempVector.Direction = clsShoddyVector.VectorDirection.COL
                Return tempVector
            End Get
            Set(ByVal value As clsShoddyVector)
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
        Private Shared Function IsSameDimension(ByVal ai_source As clsShoddyMatrix, ByVal ai_dest As clsShoddyMatrix) As Boolean
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
        Private Shared Function IsComputableMatrixVector(ByVal ai_matrix As clsShoddyMatrix, ByVal ai_vector As clsShoddyVector) As Boolean
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
        Private Function CalcDeterminant(ByVal ai_clsMatrix As clsShoddyMatrix, _
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
                Dim b As New clsShoddyMatrix(ai_dim - 1)
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
        End Function
#End Region
    End Class

End Namespace

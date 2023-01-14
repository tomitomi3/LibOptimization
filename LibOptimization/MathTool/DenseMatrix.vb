Namespace MathTool
    ''' <summary>
    ''' Matrix class
    ''' </summary>
    ''' <remarks>
    ''' Inherits List(Of List(Of Double))
    ''' </remarks>
    ''' delete DebuggerDisplay
    <Serializable>
    Public Class DenseMatrix : Inherits List(Of List(Of Double))
#Region "Constructor"
        ''' <summary>
        ''' Default construcotr
        ''' </summary>
        ''' <remarks></remarks>
        'Public Sub New()
        '    'nop
        'End Sub

        ''' <summary>
        ''' Copy constructor
        ''' </summary>
        ''' <param name="ai_base"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_base As DenseMatrix)
            For i As Integer = 0 To ai_base.Count - 1
                Dim temp As New DenseVector(ai_base(i))
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
                Dim temp As New DenseVector(ai_dim)
                If ai_isIdentity Then
                    temp(i) = 1.0
                End If
                Me.Add(temp)
            Next
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_dim"></param>
        ''' <param name="diagonalValue"></param>
        Public Sub New(ByVal ai_dim As Integer, diagonalValue As Double)
            For i As Integer = 0 To ai_dim - 1
                Dim temp As New DenseVector(ai_dim)
                temp(i) = diagonalValue
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
                Me.Add(New DenseVector(ai_colSize))
            Next
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_val"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_val As List(Of List(Of Double)))
            For i As Integer = 0 To ai_val.Count - 1
                Me.Add(New DenseVector(ai_val(i)))
            Next
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_val"></param>
        ''' <remarks></remarks>
        Public Sub New(ParamArray ai_val As Double()())
            For i As Integer = 0 To ai_val.Length - 1
                Me.Add(New DenseVector(ai_val(i)))
            Next
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_val"></param>
        ''' <param name="ai_direction">Row or Col</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_val As List(Of Double), ByVal ai_direction As DenseVector.VectorDirection)
            If ai_direction = DenseVector.VectorDirection.ROW Then
                Dim temp As New DenseVector(ai_val)
                Me.Add(temp)
            Else
                For i As Integer = 0 To ai_val.Count - 1
                    Me.Add(New DenseVector({ai_val(i)}))
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
                Me.Add(New DenseVector(temp))
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
        ''' DenseMatrix -> double[][]
        ''' </summary>
        ''' <returns></returns>
        Public Function ToArrayMulti() As Double()()
            Dim ret = New Double(Me.Count - 1)() {}

            For i As Integer = 0 To Me.Count - 1
                ret(i) = Me(i).ToArray()
            Next

            Return ret
        End Function

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
        Public Property Row(ByVal ai_rowIndex As Integer) As DenseVector
            Get
                Return New DenseVector(Me(ai_rowIndex))
            End Get
            Set(ByVal value As DenseVector)
                Me(ai_rowIndex) = New DenseVector(value)
            End Set
        End Property

        ''' <summary>
        ''' Column accessor
        ''' </summary>
        ''' <param name="ai_colIndex"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Column(ByVal ai_colIndex As Integer) As DenseVector
            Get
                '            Dim temp(Me.RowCount - 1) As Double
                Dim tempVector As New DenseVector(Me.RowCount)
                For i As Integer = 0 To tempVector.Count - 1
                    tempVector(i) = Me.Row(i)(ai_colIndex)
                Next
                tempVector.Direction = DenseVector.VectorDirection.COL
                Return tempVector
            End Get
            Set(ByVal value As DenseVector)
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
        ''' Matrix dimension
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Dimension As Integer
            Get
                Dim row = Me.RowCount
                Dim col = Me.ColCount
                If row = col Then
                    Return row
                Else
                    Return -1
                End If
            End Get
        End Property
#End Region

#Region "Public Operator"
        ''' <summary>
        ''' Add(Matrix + Matrix)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator +(ByVal ai_source As DenseMatrix, ByVal ai_dest As DenseMatrix) As DenseMatrix
            If IsSameDimension(ai_source, ai_dest) = False Then
                Throw New MathException(MathException.ErrorSeries.DifferRowNumberAndCollumnNumber)
            End If
            Dim ret As New DenseMatrix(ai_source)
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
        Public Shared Operator +(ByVal ai_source As DenseMatrix, ByVal ai_dest As DenseVector) As DenseVector
            If IsComputableMatrixVector(ai_source, ai_dest) = False Then
                Throw New MathException(MathException.ErrorSeries.NotComputable)
            End If
            Dim ret As New DenseVector(ai_dest)
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
        Public Shared Operator +(ByVal ai_source As DenseVector, ByVal ai_dest As DenseMatrix) As DenseVector
            If IsComputableMatrixVector(ai_dest, ai_source) = False Then
                Throw New MathException(MathException.ErrorSeries.NotComputable)
            End If
            Dim ret As New DenseVector(ai_source)
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
        Public Shared Operator -(ByVal ai_source As DenseMatrix, ByVal ai_dest As DenseMatrix) As DenseMatrix
            If IsSameDimension(ai_source, ai_dest) = False Then
                Throw New MathException(MathException.ErrorSeries.DifferRowNumberAndCollumnNumber)
            End If
            Dim ret As New DenseMatrix(ai_source)
            Dim row = ret.RowCount()
            Dim col = ret.ColCount()
            For i As Integer = 0 To row - 1
                For j = 0 To col - 1
                    ret(i)(j) = ai_source(i)(j) - ai_dest(i)(j)
                Next
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Diff(Matrix - Vector)
        ''' </summary>
        ''' <param name="mat"></param>
        ''' <param name="vec"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator -(ByVal mat As DenseMatrix, ByVal vec As DenseVector) As DenseMatrix
            If vec.Direction = DenseVector.VectorDirection.COL AndAlso mat.ColCount = vec.Count Then
                Dim ret As New DenseMatrix(mat)
                Dim row = mat.RowCount
                Dim col = mat.ColCount
                For j = 0 To col - 1
                    For i = 0 To row - 1
                        ret(i)(j) -= vec(j)
                    Next
                Next
                Return ret
            ElseIf vec.Direction = DenseVector.VectorDirection.ROW AndAlso mat.RowCount = vec.Count Then
                Dim ret As New DenseMatrix(mat)
                Dim row = mat.RowCount
                Dim col = mat.ColCount
                For i = 0 To row - 1
                    For j = 0 To col - 1
                        ret(i)(j) -= vec(i)
                    Next
                Next
                Return ret
            Else
                Throw New MathException(MathException.ErrorSeries.NotComputable, "The number of dimensions of matrices and vectors is different.")
            End If
        End Operator

        ''' <summary>
        ''' Diff(Vector - Matrix)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator -(ByVal ai_source As DenseVector, ByVal ai_dest As DenseMatrix) As DenseVector
            If IsComputableMatrixVector(ai_dest, ai_source) = False Then
                Throw New MathException(MathException.ErrorSeries.NotComputable)
            End If
            Dim ret As New DenseVector(ai_source)
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
        Public Shared Operator *(ByVal ai_source As DenseMatrix, ByVal ai_dest As DenseMatrix) As DenseMatrix
            '.NET Frameworkのバージョンで区分け
#If (NET30_CUSTOM OrElse NET35_CUSTOM OrElse NET35) = True Then
            '------------------------------------------------------------------
            '.net 3.0, 3.5
            '------------------------------------------------------------------
            If IsSameDimension(ai_source, ai_dest) = True Then
                '[M*M] X [M*M]
                Dim size = ai_source.RowCount
                Dim ret As New DenseMatrix(size)

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
                Dim ret As New DenseMatrix(ai_source.RowCount, ai_dest.ColCount)

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

            Throw New MathException(MathException.ErrorSeries.NotComputable, "Matrix * Matrix")
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
            Dim ret As New DenseMatrix(size)

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
            Dim ret As New DenseMatrix(ai_source.RowCount, ai_dest.ColCount)

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

        Throw New MathException(MathException.ErrorSeries.NotComputable, "Matrix * Matrix")
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
        Public Shared Operator *(ByVal mat As DenseMatrix, ByVal vec As DenseVector) As DenseVector
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
                Throw New MathException(MathException.ErrorSeries.NotComputable, "Matrix * Vector - size error")
            End If
            'If vec.Direction <> clsEasyVector.VectorDirection.COL Then
            '    Throw New clsException(clsException.Series.NotComputable, "Matrix * Vector - vector direction is row")
            'End If

            Dim vSize As Integer = mat.RowCount
            Dim ret As New DenseVector(vSize, DenseVector.VectorDirection.COL)
#If (NET30_CUSTOM OrElse NET35_CUSTOM OrElse NET35) = True Then
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
        Public Shared Operator *(ByVal vec As DenseVector, ByVal mat As DenseMatrix) As DenseVector
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
                Throw New MathException(MathException.ErrorSeries.NotComputable, "Vector * Matrix - size error")
            End If
            'If vec.Direction <> clsEasyVector.VectorDirection.COL Then
            '    Throw New clsException(clsException.Series.NotComputable, "Vector * Matrix - vector direction is col")
            'End If

            Dim vSize As Integer = mat.ColCount '行列の行サイズ
            Dim ret As New DenseVector(vSize, DenseVector.VectorDirection.ROW)
#If (NET30_CUSTOM OrElse NET35_CUSTOM OrElse NET35) = True Then
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
        Public Shared Operator *(ByVal ai_source As Double, ByVal ai_dest As DenseMatrix) As DenseMatrix
            Dim ret As New DenseMatrix(ai_dest)
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
        Public Shared Operator *(ByVal ai_source As DenseMatrix, ByVal ai_dest As Double) As DenseMatrix
            Dim ret As New DenseMatrix(ai_source)
            For i As Integer = 0 To ret.RowCount() - 1
                ret.Row(i) = ai_source.Row(i) * ai_dest
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Divide(Matrix / value)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator /(ByVal ai_source As DenseMatrix, ByVal ai_dest As Double) As DenseMatrix
            Dim ret As New DenseMatrix(ai_source)
            For i As Integer = 0 To ret.RowCount() - 1
                ret.Row(i) = ai_source.Row(i) / ai_dest
            Next
            Return ret
        End Operator
#End Region

#Region "Public Utility"
        ''' <summary>
        ''' Transpose
        ''' </summary>
        ''' <remarks></remarks>
        Public Function T() As DenseMatrix
            Dim ret As New DenseMatrix(Me.ColCount, Me.RowCount)

#If (NET30_CUSTOM OrElse NET35_CUSTOM OrElse NET35) = True Then
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
        Parallel.For(0, ret.RowCount,
            Sub(rowIndex)
                ret.Row(rowIndex) = Me.Column(rowIndex)
            End Sub)
#End If

            Return ret
        End Function

        ''' <summary>
        ''' Condition number
        ''' </summary>
        ''' <returns></returns>
        Public Function Cond() As Double
            If Me.IsSquare() = False Then
                Throw New MathException(MathException.ErrorSeries.NotComputable)
            End If
            If Me.IsSymmetricMatrix() = False Then
                Throw New MathException(MathException.ErrorSeries.NotComputable)
            End If

            Dim eig = Me.Eigen(isCheckSymetricMatrix:=False)
            Return eig.EigenVector.MaxElementValue() / eig.EigenVector.MinElementValue()
        End Function

        ''' <summary>
        ''' Convert to a matrix with only diagonal values
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToDiagonalMatrix() As DenseMatrix
            If Me.RowCount <> Me.ColCount Then
                Throw New MathException(MathException.ErrorSeries.NotComputable, "ToDiagonalMatrix()")
            End If
            Dim ret As New DenseMatrix(Me.RowCount)
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
        Public Function ToDiagonalVector(Optional ByVal direction As DenseVector.VectorDirection = DenseVector.VectorDirection.ROW) As DenseVector
            If Me.RowCount <> Me.ColCount Then
                Throw New MathException(MathException.ErrorSeries.NotComputable, "ToVectorFromDiagonal")
            End If
            Dim ret As New DenseVector(Me.RowCount, direction)
            For i As Integer = 0 To Me.Count - 1
                ret(i) = Me(i)(i)
            Next
            Return ret
        End Function

        ''' <summary>
        ''' For Debug
        ''' </summary>
        ''' <param name="ai_preci"></param>
        ''' <param name="name"></param>
        ''' <param name="isScientificNotation"></param>
        Public Sub PrintValue(Optional ByVal ai_preci As Integer = 4,
                              Optional ByVal name As String = "",
                              Optional isScientificNotation As Boolean = False)
            Dim str As New System.Text.StringBuilder()
            If String.IsNullOrEmpty(name) = False Then
                str.Append(String.Format("{0} {1}x{2} =", name, Me.RowCount, Me.ColCount) & Environment.NewLine)
            Else
                str.Append(String.Format("Mat {0}x{1} =", Me.RowCount, Me.ColCount) & Environment.NewLine)
            End If
            For Each vec As DenseVector In Me
                For i As Integer = 0 To vec.Count - 1
                    If isScientificNotation = False Then
                        str.Append(vec(i).ToString("F" & ai_preci.ToString()) & vbTab)
                    Else
                        str.Append(vec(i).ToString("G" & ai_preci.ToString()) & vbTab)
                    End If
                Next
                str.Append(Environment.NewLine)
            Next
            str.Append(Environment.NewLine)
            Console.Write(str.ToString())
        End Sub

        ''' <summary>
        ''' check Square matrix
        ''' </summary>
        ''' <param name="tgtMat"></param>
        ''' <returns></returns>
        Public Function IsSquare() As Boolean
            Dim dimension = Me.Dimension
            If dimension = -1 Then
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' check Symmetric matrix
        ''' </summary>
        ''' <param name="eps">acceptable error. Set eps is 0 for precise comparisons. default:0</param>
        ''' <returns></returns>
        Public Function IsSymmetricMatrix(Optional ByVal eps As Double = 0.0) As Boolean
            Dim tgtMat = Me
            Dim n = tgtMat.Dimension
            For i = 0 To n - 1
                For j = 0 To i - 1
                    Dim isSameValue = MathUtil.IsSameValues(tgtMat(i)(j), tgtMat(j)(i), eps)
                    If isSameValue = False Then
                        Return False
                    End If
                Next
            Next
            Return True
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
        ''' Max value
        ''' </summary>
        ''' <returns></returns>
        Public Function MaxElementValue() As Double
            Dim retMax As Double = Double.NegativeInfinity
            For Each vec In Me
                For Each value In vec
                    If value > retMax Then
                        retMax = value
                    End If
                Next
            Next
            Return retMax
        End Function

        ''' <summary>
        ''' Min value
        ''' </summary>
        ''' <returns></returns>
        Public Function MinElementValue() As Double
            Dim retMin As Double = Double.PositiveInfinity
            For Each vec In Me
                For Each value In vec
                    If value < retMin Then
                        retMin = value
                    End If
                Next
            Next
            Return retMin
        End Function

        ''' <summary>
        ''' Absolute Max value
        ''' </summary>
        ''' <returns></returns>
        'Public Function MaxAbsElementValue() As Double
        '    Dim retMaxabs As Double = 0.0
        '    For Each vec In Me
        '        For Each value In vec
        '            Dim absValue = System.Math.Abs(value)
        '            If retMaxabs > absValue Then
        '                retMaxabs = absValue
        '            End If
        '        Next
        '    Next
        '    Return retMaxabs
        'End Function

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

        ''' <summary>
        ''' Swap value
        ''' </summary>
        ''' <param name="rowA"></param>
        ''' <param name="colA"></param>
        ''' <param name="rowB"></param>
        ''' <param name="colB"></param>
        Public Sub SwapValue(rowA As Integer, colA As Integer, rowB As Integer, colB As Integer)
            Dim temp = Me(rowA)(colA)
            Me(rowA)(colA) = Me(rowB)(colB)
            Me(rowB)(colB) = temp
        End Sub

        ''' <summary>
        ''' Norm L2 (Euclid norm)
        ''' </summary>
        Public Function NormL2FromAllElement() As Double
            Dim sum = 0.0
            For Each vec In Me
                For Each v In vec
                    sum += v * v
                Next
            Next
            Return Math.Sqrt(sum)
        End Function

        ''' <summary>
        ''' sum( x11 + x12 + ... xnn )
        ''' </summary>
        ''' <returns></returns>
        Public Function SumAllElement() As Double
            Dim sum = 0.0
            For Each vec In Me
                For Each v In vec
                    sum += v
                Next
            Next
            Return sum
        End Function

        ''' <summary>
        ''' Add row vector
        ''' </summary>
        ''' <param name="vector"></param>
        Public Sub AddRow(ByVal vector As DenseVector)
            'check
            For i = 0 To Me.Count - 1
                If Me(i).Count <> vector.Count Then
                    Throw New MathException(MathException.ErrorSeries.DifferElementNumber)
                End If
            Next

            Me.Add(vector)
        End Sub

        ''' <summary>
        ''' Add col vector
        ''' </summary>
        ''' <param name="vector"></param>
        Public Sub AddCol(ByVal vector As DenseVector)
            If Me.Count = 0 Then
                For Each val As Double In vector
                    Me.Add(New DenseVector(New Double() {val}))
                Next
            Else
                'check 行数とベクター要素数が同じかどうか
                If Me.Count <> vector.Count Then
                    Throw New MathException(MathException.ErrorSeries.DifferElementNumber)
                End If
                For i = 0 To Me.RowCount - 1
                    Me(i).Add(vector(i))
                Next
            End If
        End Sub
#End Region

#Region "Public Func"
        ''' <summary>
        ''' Sqrt(Mat)
        ''' </summary>
        ''' <returns></returns>
        Public Function Sqrt() As DenseMatrix
            Dim ret = New DenseMatrix(Me)
            Dim row = Me.RowCount
            Dim col = Me.ColCount
            For i = 0 To row - 1
                For j = 0 To col - 1
                    ret(i)(j) = System.Math.Sqrt(Me(i)(j))
                Next
            Next
            Return ret
        End Function

        ''' <summary>
        ''' Hadamard product ( a1 * b1, a2 * b2, ... )
        ''' </summary>
        ''' <param name="b">( a1 * a1, a2 * a2, ... )</param>
        Public Function HadamardProduct(Optional ByRef b As DenseMatrix = Nothing) As DenseMatrix
            If b Is Nothing Then
                Dim ret = New DenseMatrix(Me)
                Dim row = Me.RowCount
                Dim col = Me.ColCount
                For i = 0 To row - 1
                    For j = 0 To col - 1
                        ret(i)(j) = Me(i)(j) * Me(i)(j)
                    Next
                Next
                Return ret
            Else
                If IsSameDimension(b, Me) = False Then
                    Throw New MathException(MathException.ErrorSeries.DifferElementNumber)
                End If
                Dim ret = New DenseMatrix(Me)
                Dim row = Me.RowCount
                Dim col = Me.ColCount
                For i = 0 To row - 1
                    For j = 0 To col - 1
                        ret(i)(j) = Me(i)(j) * b(i)(j)
                    Next
                Next
                Return ret
            End If
        End Function

        ''' <summary>
        ''' Hadamard divide ( a1 / b1, a2 / b2, ... )
        ''' </summary>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Function HadamardDivide(ByRef b As DenseMatrix) As DenseMatrix
            If IsSameDimension(b, Me) = False Then
                Throw New MathException(MathException.ErrorSeries.DifferElementNumber)
            End If
            Dim ret = New DenseMatrix(Me)
            Dim row = Me.RowCount
            Dim col = Me.ColCount
            For i = 0 To row - 1
                For j = 0 To col - 1
                    ret(i)(j) = Me(i)(j) / b(i)(j)
                Next
            Next
            Return ret
        End Function

        ''' <summary>
        ''' Average vector
        ''' </summary>
        ''' <param name="vectorDirection"></param>
        ''' <returns></returns>
        Public Function AverageVector(ByVal vectorDirection As DenseVector.VectorDirection) As DenseVector
            Dim row = Me.RowCount
            Dim col = Me.ColCount
            Dim ret As DenseVector = Nothing
            If vectorDirection = DenseVector.VectorDirection.ROW Then
                ret = New DenseVector(col, DenseVector.VectorDirection.COL)
                For j = 0 To col - 1
                    For i = 0 To row - 1
                        ret(j) += Me(i)(j)
                    Next
                    ret(j) /= row
                Next
                Return ret
            Else
                ret = New DenseVector(row, DenseVector.VectorDirection.ROW)
                For i = 0 To row - 1
                    For j = 0 To col - 1
                        ret(i) += Me(i)(j)
                    Next
                    ret(i) /= col
                Next
                Return ret
            End If
            Return ret
        End Function

        ''' <summary>
        ''' Determinant
        ''' </summary>
        ''' <remarks></remarks>
        Public Function Det(Optional ByVal eps As Double = ConstantValues.MachineEpsiron, Optional ByVal samezero As Double = ConstantValues.SAME_ZERO) As Double
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
                'over 4 dim using LUP decomposition
                Dim detVal = Me.LUP(eps, samezero).Det()
                Return detVal
            End If
        End Function

        ''' <summary>
        ''' Inverse
        ''' </summary>
        ''' <param name="eps"></param>
        ''' <param name="epsNearlyZero"></param>
        ''' <param name="isUsingLUDecomposition"></param>
        ''' <param name="isForceInverse"></param>
        ''' <returns></returns>
        Public Function Inverse(Optional ByVal eps As Double = ConstantValues.MachineEpsiron,
                                Optional ByVal epsNearlyZero As Double = ConstantValues.SAME_ZERO,
                                Optional ByVal isUsingLUDecomposition As Boolean = False) As DenseMatrix
            If Me.RowCount <> Me.ColCount Then
                Return New DenseMatrix(0)
            End If

            Dim n As Integer = Me.RowCount
            Dim source As New DenseMatrix(Me)
            Dim retInverse As New DenseMatrix(n, False)
            If n = 0 Then
                'nop
            ElseIf n = 1 Then
                If MathUtil.IsSameZero(source(0)(0), eps) = True Then
                    Throw New MathException(MathException.ErrorSeries.NotComputable, "Inverse 1x1")
                End If
                retInverse(0)(0) = 1.0 / source(0)(0)
            ElseIf n = 2 AndAlso isUsingLUDecomposition = False Then
                Dim det = Me.Det(eps, epsNearlyZero)
                If MathUtil.IsSameZero(det, eps) = True Then
                    Throw New MathException(MathException.ErrorSeries.NotComputable, "Inverse 2x2")
                End If
                det = 1.0 / det
                retInverse(0)(0) = det * Me(1)(1)
                retInverse(0)(1) = det * -Me(0)(1)
                retInverse(1)(0) = det * -Me(1)(0)
                retInverse(1)(1) = det * Me(0)(0)
            ElseIf n = 3 AndAlso isUsingLUDecomposition = False Then
                Dim det = Me.Det(eps, epsNearlyZero)
                If MathUtil.IsSameZero(det, eps) = True Then
                    Throw New MathException(MathException.ErrorSeries.NotComputable, "Inverse 3x3")
                End If
                retInverse(0)(0) = ((Me(1)(1) * Me(2)(2) - Me(1)(2) * Me(2)(1))) / det
                retInverse(0)(1) = -((Me(0)(1) * Me(2)(2) - Me(0)(2) * Me(2)(1))) / det
                retInverse(0)(2) = ((Me(0)(1) * Me(1)(2) - Me(0)(2) * Me(1)(1))) / det
                retInverse(1)(0) = -((Me(1)(0) * Me(2)(2) - Me(1)(2) * Me(2)(0))) / det
                retInverse(1)(1) = ((Me(0)(0) * Me(2)(2) - Me(0)(2) * Me(2)(0))) / det
                retInverse(1)(2) = -((Me(0)(0) * Me(1)(2) - Me(0)(2) * Me(1)(0))) / det
                retInverse(2)(0) = ((Me(1)(0) * Me(2)(1) - Me(1)(1) * Me(2)(0))) / det
                retInverse(2)(1) = -((Me(0)(0) * Me(2)(1) - Me(0)(1) * Me(2)(0))) / det
                retInverse(2)(2) = ((Me(0)(0) * Me(1)(1) - Me(0)(1) * Me(1)(0))) / det
            Else
                Dim lup = Me.LUP(eps, epsNearlyZero)
                Dim det = lup.Det
                If MathUtil.IsSameZero(det, epsNearlyZero) = True Then
                    Throw New MathException(MathException.ErrorSeries.NotComputable, String.Format("Inverse {0}x{0} det=0", n))
                End If

                'lup.P = lup.P.T() 'Transopose = Inverse
                For j = 0 To n - 1
                    Dim y = New DenseVector(n)
                    Dim b = lup.P(j)
                    For i = 0 To n - 1
                        Dim s = 0.0
                        Dim k As Integer = 0
                        For k = 0 To i - 1
                            s += lup.L(i)(k) * y(k)
                        Next
                        y(k) = b(i) - s
                    Next
                    For i = n - 1 To 0 Step -1
                        Dim s = 0.0
                        For k = i + 1 To n - 1
                            s += lup.U(i)(k) * retInverse(k)(j)
                        Next
                        retInverse(i)(j) = (y(i) - s) / lup.U(i)(i)
                    Next
                Next
            End If

            Return retInverse
        End Function

        ''' <summary>
        ''' LUP decomposition( X=PLU -> P^-1X=LU )
        ''' </summary>
        ''' <param name="eps">2.20*10^-16</param>
        ''' <remarks>
        ''' Refference
        ''' [1]奧村晴彥. C 言語による最新アルゴリズム事典. 技術評論社, 1991.
        ''' [2]Press, W. H., et al. "円慶寺勝市, 奥村晴彦, 佐藤俊郎, 他訳: C 言語による数値計算のレシピ." (1993).
        ''' </remarks>
        ''' <returns><see cref="MathTool.LU"/></returns>
        Public Function LUP(Optional ByVal eps As Double = ConstantValues.MachineEpsiron,
                            Optional ByVal epsNearlyZero As Double = ConstantValues.SAME_ZERO) As LU
            Dim n = Me.ColCount
            Dim source = New DenseMatrix(Me)
            Dim matP = New DenseMatrix(n, True)

            Dim det = 1.0
            Dim weight = New Double(n - 1) {}
            Dim pivotrow = New Integer(n - 1) {}

            'Find absolute max value of each row
            For i = 0 To n - 1
                pivotrow(i) = i
                Dim absValue = 0.0
                For j = 0 To n - 1
                    Dim temp = System.Math.Abs(Me(i)(j))
                    If temp >= absValue Then
                        absValue = temp
                    End If
                Next
                '列要素の絶対最大値が0に近い場合
                If MathUtil.IsSameZero(absValue, epsNearlyZero) Then
                    Throw New MathException(MathException.ErrorSeries.NotComputable, "LUP() singular matrix")
                End If
                weight(i) = 1.0 / absValue
            Next

            'calc LUP
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
                    Dim dum = weight(i) * System.Math.Abs(sum)
                    If dum > big Then
                        big = dum
                        imax = i
                    End If
                Next

                'interchange row
                If j <> imax Then
                    MathUtil.SwapRow(source, imax, j)
                    MathUtil.SwapRow(matP, imax, j)
                    weight(imax) = weight(j)

                    'change sign
                    det = -det

                    'swap row info
                    Dim temp = pivotrow(imax)
                    pivotrow(imax) = pivotrow(j)
                    pivotrow(j) = temp
                End If
                'diagonal value is close to 0.
                If MathUtil.IsSameZero(source(j)(j), eps) Then
                    source(j)(j) = epsNearlyZero
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
            matP = matP.T

            'replace
            Dim matL = New DenseMatrix(n, True)
            Dim matU = New DenseMatrix(n, True)
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

            Return New LU(matP, matL, matU, det, pivotrow)
        End Function

        ''' <summary>
        ''' Cholesky decomposition A=LL^T
        ''' </summary>
        ''' <returns></returns>
        Public Function Cholesky() As DenseMatrix
            If Me.IsSquare() = False Then
                Throw New MathException(MathException.ErrorSeries.NotComputable, "Cholesky() not Square")
            End If

            Dim ret As New DenseMatrix(Me.RowCount)
            Dim n = CInt(System.Math.Sqrt(ret.RowCount * ret.ColCount))
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To i
                    If j = i Then
                        Dim sum = 0.0
                        For k As Integer = 0 To j
                            sum += ret(j)(k) * ret(j)(k)
                        Next
                        ret(j)(j) = System.Math.Sqrt(Me(j)(j) - sum)
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
#End Region

#Region "Public Eigen"
        ''' <summary>
        ''' Eigen decomposition using Jacobi Method. for Symmetric Matrix.
        ''' Memo: A = V*D*V−1, D is diag(eigen value1 ... eigen valueN), V is eigen vectors. V is orthogonal matrix.
        ''' </summary>
        ''' <param name="iteration">default:1000</param>
        ''' <param name="eps"><see cref="ConstantValues.MachineEpsiron"/></param>
        ''' <param name="isSort">descent sort by EigenValue default:true</param>
        ''' <param name="epsNearlyZero"></param>
        ''' <remarks>
        ''' 固有値、固有ベクトルを求める方針
        '''  反復計算によって求める。計算しやすい行列に変換
        ''' 対称行列 or 非対称行列
        '''  対称行列   :べき乗法、ヤコビ法、三重対角行列->QR法
        '''  非対称行列 :ヘッセンベルグ行列->QR法
        ''' QR法の高速
        '''  原点移動（ウィルキンソンシフト）、減次（デフレーション）
        '''  QR法だと固有値のみ固有ベクトルは別途計算
        ''' </remarks>
        ''' <returns></returns>
        <Obsolete>
        Public Function EigenJacobi(Optional ByVal iteration As Integer = 1000,
                                    Optional ByVal eps As Double = ConstantValues.MachineEpsiron,
                                    Optional ByVal isSort As Boolean = True,
                                    Optional ByVal epsNearlyZero As Double = Double.Epsilon) As Eigen
            '正方行列、対象行列チェック
            If Me.IsSquare() = False Then
                Throw New MathException(MathException.ErrorSeries.DifferRowNumberAndCollumnNumber)
            End If

            Dim size = Me.ColCount()
            Dim retEigenMat = New DenseMatrix(Me)
            Dim rotate = New DenseMatrix(size, True)
            Dim isConversion = False
            Dim rowIdx() = New Integer(size * 4 - 1) {}
            Dim colIdx() = New Integer(size * 4 - 1) {}
            Dim value() = New Double(size * 4 - 1) {}

            'iteration
            For itr As Integer = 0 To iteration - 1
                'find abs max value without diag
                Dim max = System.Math.Abs(retEigenMat(0)(1))
                Dim p As Integer = 0
                Dim q As Integer = 1
                For i As Integer = 0 To size - 1
                    For j As Integer = i + 1 To size - 1
                        Dim tempMax = System.Math.Abs(retEigenMat(i)(j))
                        If max < tempMax Then
                            max = tempMax
                            p = i
                            q = j
                        End If
                    Next
                Next

                'check conversion
                If max < eps Then
                    isConversion = True
                    Exit For
                End If

                Dim temp_pp = retEigenMat(p)(p)
                Dim temp_qq = retEigenMat(q)(q)
                Dim temp_pq = retEigenMat(p)(q)
                Dim theta = 0.0
                Dim diff = temp_pp - temp_qq
                If MathUtil.IsSameZero(diff, epsNearlyZero) = True Then
                    theta = System.Math.PI / 4.0
                Else
                    theta = System.Math.Atan(-2.0 * temp_pq / diff) * 0.5
                End If

                Dim D = New DenseMatrix(retEigenMat)
                Dim cosTheta = System.Math.Cos(theta)
                Dim sinTheta = System.Math.Sin(theta)
                For i As Integer = 0 To size - 1
                    Dim temp = retEigenMat(p)(i) * cosTheta - retEigenMat(q)(i) * sinTheta
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

                'rotate
                Dim rotateNew = New DenseMatrix(size, True)
                rotateNew(p)(p) = cosTheta
                rotateNew(p)(q) = sinTheta
                rotateNew(q)(p) = -sinTheta
                rotateNew(q)(q) = cosTheta
                rotate = rotate * rotateNew
            Next

            'sort by Eigen value
            Dim eigenValue = retEigenMat.ToDiagonalVector()
            If isSort = True Then
                Eigen.EigenSort(eigenValue, rotate, True)
            End If

            Return New Eigen(eigenValue, rotate, isConversion)
        End Function

        ''' <summary>
        ''' Eigen (実対称行列)
        ''' </summary>
        ''' <param name="iteration">iteration default 100</param>
        ''' <param name="eps">eps default:<see cref="ConstantValues.MachineEpsiron"/></param>
        ''' <param name="isCheckSymetricMatrix">symmetric matrix check</param>
        ''' <returns><see cref="MathTool.Eigen"/></returns>
        ''' <remarks>
        ''' Refference
        ''' Press, W. H., et al. "円慶寺勝市, 奥村晴彦, 佐藤俊郎, 他訳: C 言語による数値計算のレシピ." (1993).
        ''' </remarks>
        Public Function Eigen(Optional iteration As Integer = 100,
                              Optional eps As Double = ConstantValues.MachineEpsiron,
                              Optional isCheckSymetricMatrix As Boolean = True) As Eigen
            '対称行列の確認
            If isCheckSymetricMatrix Then
                Me.IsSymmetricMatrix()
                If Me.IsSymmetricMatrix(ConstantValues.MachineEpsiron) = False Then
                    Throw New MathException(MathException.ErrorSeries.NotComputable, "Not symmetric matrix. Eigenvalue decomposition of asymmetric matrices is not yet implemented.")
                End If
            End If

            Dim n = Me.RowCount()
            Dim eigenVec = New DenseMatrix(Me)
            Dim subDiag = New DenseVector(n)    '3重対角行列の副対角要素
            Dim eigenValue = New DenseVector(n) '3重対角行列の対角要素

            'calc Eigen value and vector
            Me.tred2(eigenVec, subDiag, eigenValue)
            Dim isConversion = Me.tqli(eigenVec, subDiag, eigenValue, iteration, eps)
            Eigen.EigenSort(eigenValue, eigenVec, True)

            Return New Eigen(eigenValue, eigenVec, isConversion)
        End Function

        ''' <summary>
        ''' 実対称行列のHouseholder変換 -> 3重対角行列
        ''' </summary>
        ''' <param name="eigenVec"></param>
        ''' <param name="subDiag">subdiagonal elements</param>
        ''' <param name="eigenValue"></param>
        ''' <remarks>
        ''' Refference
        ''' Press, W. H., et al. "円慶寺勝市, 奥村晴彦, 佐藤俊郎, 他訳: C 言語による数値計算のレシピ." (1993).
        ''' </remarks>
        Private Sub tred2(ByRef eigenVec As DenseMatrix, ByRef subDiag As DenseVector, ByRef eigenValue As DenseVector)
            Dim n = subDiag.Count()

            Dim l, k, j, i As Integer
            Dim scale, hh, h, g, f As Double

            For i = n - 1 To 1 Step -1
                l = i - 1
                h = 0.0
                scale = 0.0

                If l > 0.0 Then

                    For k = 0 To i - 1
                        scale += Math.Abs(eigenVec(i)(k))
                    Next

                    If scale = 0.0 Then
                        subDiag(i) = eigenVec(i)(l)
                    Else
                        For k = 0 To i - 1
                            eigenVec(i)(k) = eigenVec(i)(k) / scale
                            h += eigenVec(i)(k) * eigenVec(i)(k)
                        Next

                        f = eigenVec(i)(l)

                        If f > 0.0 Then
                            g = -Math.Sqrt(h)
                        Else
                            g = Math.Sqrt(h)
                        End If

                        subDiag(i) = scale * g
                        h -= f * g
                        eigenVec(i)(l) = f - g
                        f = 0.0

                        For j = 0 To i - 1

                            eigenVec(j)(i) = eigenVec(i)(j) / h

                            g = 0.0

                            For k = 0 To j + 1 - 1
                                g += eigenVec(j)(k) * eigenVec(i)(k)
                            Next

                            For k = j + 1 To i - 1
                                g += eigenVec(k)(j) * eigenVec(i)(k)
                            Next

                            subDiag(j) = g / h
                            f += subDiag(j) * eigenVec(i)(j)
                        Next

                        hh = f / (h + h)

                        For j = 0 To i - 1
                            f = eigenVec(i)(j)
                            g = subDiag(j) - hh * f
                            subDiag(j) = g

                            For k = 0 To j + 1 - 1
                                eigenVec(j)(k) -= (f * subDiag(k) + g * eigenVec(i)(k))
                            Next
                        Next
                    End If
                Else
                    subDiag(i) = eigenVec(i)(l)
                End If

                eigenValue(i) = h
            Next

            eigenValue(0) = 0.0
            subDiag(0) = 0.0
            For i = 0 To n - 1
                If eigenValue(i) <> 0.0 Then

                    For j = 0 To i - 1
                        g = 0.0

                        For k = 0 To i - 1
                            g += eigenVec(i)(k) * eigenVec(k)(j)
                        Next

                        For k = 0 To i - 1
                            eigenVec(k)(j) -= g * eigenVec(k)(i)
                        Next
                    Next
                End If

                eigenValue(i) = eigenVec(i)(i)
                eigenVec(i)(i) = 1.0

                For j = 0 To i - 1
                    eigenVec(j)(i) = 0.0
                    eigenVec(i)(j) = 0.0
                Next
            Next
        End Sub

        ''' <summary>
        ''' 3重対角陰的QL法 実対称3重対角行列の固有値と固有ベクトルを求める
        ''' </summary>
        ''' <param name="eigenVec"></param>
        ''' <param name="subDiag">subdiagonal elements</param>
        ''' <param name="iteration">iteration</param>
        ''' <param name="eps">eps</param>
        ''' <remarks>
        ''' Refference
        ''' Press, W. H., et al. "円慶寺勝市, 奥村晴彦, 佐藤俊郎, 他訳: C 言語による数値計算のレシピ." (1993).
        ''' </remarks>
        Private Function tqli(ByRef eigenVec As DenseMatrix, ByRef subDiag As DenseVector, ByRef eigenValue As DenseVector,
                              ByVal iteration As Integer, ByVal eps As Double) As Boolean
            Dim n = Me.RowCount

            Dim m, l, iter, i, k As Integer
            Dim s, r, p, g, f, dd, c, b As Double
            For i = 1 To n - 1
                subDiag(i - 1) = subDiag(i)
            Next
            subDiag(n - 1) = 0.0

            For l = 0 To n - 1
                iter = 0
                Do
                    For m = l To n - 1 - 1
                        dd = Math.Abs(eigenValue(m)) + Math.Abs(eigenValue(m + 1))
                        If Math.Abs(subDiag(m)) <= eps * dd Then
                            Exit For
                        End If
                    Next

                    If m <> l Then
                        iter += 1
                        If iter = iteration Then
                            'not conversion
                            Return False
                        End If

                        g = (eigenValue(l + 1) - eigenValue(l)) / (2.0 * subDiag(l))
                        r = MathUtil.PythagoreanAddition(g, 1.0)
                        g = eigenValue(m) - eigenValue(l) + subDiag(l) / (g + SIGN(r, g))
                        s = 1.0
                        c = 1.0
                        p = 0.0

                        For i = m - 1 To l Step -1
                            f = s * subDiag(i)
                            b = c * subDiag(i)
                            r = MathUtil.PythagoreanAddition(f, g)
                            subDiag(i + 1) = r

                            If r = 0.0 Then
                                eigenValue(i + 1) -= p
                                subDiag(m) = 0.0
                                Exit For
                            End If

                            s = f / r
                            c = g / r
                            g = eigenValue(i + 1) - p
                            r = (eigenValue(i) - g) * s + 2.0 * c * b
                            p = s * r
                            eigenValue(i + 1) = g + p
                            g = c * r - b

                            For k = 0 To n - 1
                                f = eigenVec(k)(i + 1)
                                eigenVec(k)(i + 1) = s * eigenVec(k)(i) + c * f
                                eigenVec(k)(i) = c * eigenVec(k)(i) - s * f
                            Next
                        Next

                        If r = 0.0 AndAlso i >= l Then Continue Do
                        eigenValue(l) -= p
                        subDiag(l) = g
                        subDiag(m) = 0.0
                    End If
                Loop While m <> l
            Next

            Return True
        End Function

        ''' <summary>
        ''' 符号 Eigenで使用
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Private Function SIGN(ByVal a As Double, ByVal b As Double) As Double
            Return If(b >= 0, (If(a >= 0, a, -a)), (If(a >= 0, -a, a)))
        End Function
#End Region

#Region "Experiment"

        '''' <summary>
        '''' Householder Transformation
        '''' </summary>
        '''' <remarks>
        '''' 対称行列をハウスホルダー変換 → 三重対角行列
        '''' 非対称行列をハウスホルダー変換 → ヘッセンベルグ行列
        '''' </remarks>
        '''' <returns></returns>
        'Public Function Householder() As DenseMatrix
        '    Dim ret = New DenseMatrix(Me)
        '    Dim n = ret.Count
        '    Dim f = New DenseVector(n)
        '    Dim g = New DenseVector(n)
        '    Dim u = New DenseVector(n)

        '    For k = 0 To n - 3
        '        For i = 0 To k
        '            u(i) = 0
        '        Next
        '        For i = k + 1 To n - 1
        '            u(i) = ret(i)(k) 'col
        '        Next

        '        Dim ss = 0.0
        '        For i = k + 2 To n - 1
        '            ss += u(i) * u(i)
        '        Next
        '        If MathUtil.IsCloseToZero(ss) = True Then
        '            Continue For
        '        End If
        '        Dim s = Math.Sqrt(ss + u(k + 1) * u(k + 1))
        '        If u(k + 1) > 0.0 Then
        '            s = -s
        '        End If

        '        u(k + 1) -= s
        '        Dim uu = Math.Sqrt(ss + u(k + 1) * u(k + 1))
        '        For i = k + 1 To n - 1
        '            u(i) /= uu
        '        Next

        '        For i = 0 To n - 1
        '            f(i) = 0.0
        '            g(i) = 0.0
        '            For j = k + 1 To n - 1
        '                f(i) += ret(i)(j) * u(j)
        '                g(i) += ret(j)(i) * u(j)
        '            Next
        '        Next

        '        Dim gamma = 0.0
        '        For j = 0 To n - 1
        '            gamma += u(j) * g(j)
        '        Next

        '        For i = 0 To n - 1
        '            f(i) -= gamma * u(i)
        '            g(i) -= gamma * u(i)
        '        Next

        '        For i = 0 To n - 1
        '            For j = 0 To n - 1
        '                ret(i)(j) = ret(i)(j) - 2.0 * u(i) * g(j) - 2.0 * f(i) * u(j)
        '            Next
        '        Next
        '    Next
        '    Return ret
        'End Function

#End Region

#Region "Public Shared"
        ''' <summary>
        ''' ベクトルを対角成分とした対角行列を作る
        ''' </summary>
        ''' <param name="v"></param>
        ''' <returns></returns>
        Public Shared Function CreateDiagonalMatrix(ByRef v() As Double) As DenseMatrix
            Return (New DenseVector(v)).ToDiagonalMatrix()
        End Function

        ''' <summary>
        ''' ベクトルを対角成分とした対角行列を作る
        ''' </summary>
        ''' <param name="v"></param>
        ''' <returns></returns>
        Public Shared Function CreateDiagonalMatrix(ByRef v As DenseVector) As DenseMatrix
            Return v.ToDiagonalMatrix()
        End Function

        ''' <summary>
        ''' Create Identifiy matrix
        ''' </summary>
        ''' <param name="count"></param>
        ''' <returns></returns>
        Public Shared Function Eye(count As Integer) As DenseMatrix
            Return New DenseMatrix(count, True)
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
        Private Shared Function IsSameDimension(ByRef ai_source As DenseMatrix, ByRef ai_dest As DenseMatrix) As Boolean
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
        Private Shared Function IsComputableMatrixVector(ByRef ai_matrix As DenseMatrix, ByRef ai_vector As DenseVector) As Boolean
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

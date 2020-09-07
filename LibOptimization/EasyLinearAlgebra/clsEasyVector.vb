Namespace MathUtil
    ''' <summary>
    ''' Vector class
    ''' </summary>
    ''' <remarks>
    ''' Inherits List(of double)
    ''' </remarks>
    <Serializable>
    <DebuggerDisplay("Count={Me.Count}")>
    Public Class clsEasyVector : Inherits List(Of Double)
        ''' <summary>
        ''' Direction( RowVector or ColVector )
        ''' </summary>
        Private m_direcition As VectorDirection = VectorDirection.ROW

        ''' <summary>
        ''' Vector direction.
        ''' Row vector or Column vector.
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum VectorDirection
            ROW
            COL
        End Enum

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
        Public Sub New(ByVal ai_base As clsEasyVector)
            Me.AddRange(ai_base)
            Me.m_direcition = ai_base.Direction
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_dim"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_dim As Integer, Optional ByVal ai_direction As VectorDirection = VectorDirection.ROW)
            Me.AddRange(New Double(ai_dim - 1) {})
            Me.m_direcition = ai_direction
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_val"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_val As List(Of Double), Optional ByVal ai_direction As VectorDirection = VectorDirection.ROW)
            Me.AddRange(ai_val)
            Me.m_direcition = ai_direction
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_val"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_val() As Double, Optional ByVal ai_direction As VectorDirection = VectorDirection.ROW)
            Me.AddRange(ai_val)
            Me.m_direcition = ai_direction
        End Sub
#End Region

#Region "Property"
        ''' <summary>
        ''' Accessor 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RawVector() As List(Of Double)
            Get
                Return Me
            End Get
            Set(ByVal value As List(Of Double))
                Me.Clear()
                Me.AddRange(value)
            End Set
        End Property

        ''' <summary>
        ''' Vector direction
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Direction() As VectorDirection
            Get
                Return Me.m_direcition
            End Get
            Set(ByVal value As VectorDirection)
                Me.m_direcition = value
            End Set
        End Property
#End Region

#Region "Public Operator"
        ''' <summary>
        ''' Type convert
        ''' </summary>
        ''' <param name="ai_ar"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' double() -> clsShoddyVector
        ''' </remarks>
        Public Shared Widening Operator CType(ByVal ai_ar() As Double) As clsEasyVector
            Return New clsEasyVector(ai_ar)
        End Operator

        ''' <summary>
        ''' Add
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator +(ByVal ai_source As clsEasyVector, ByVal ai_dest As clsEasyVector) As clsEasyVector
            If IsSameDimension(ai_source, ai_dest) = False Then
                Throw New clsException(clsException.Series.DifferElementNumber)
            End If

            Dim ret As New clsEasyVector(ai_source)
            For i As Integer = 0 To ret.Count - 1
                ret(i) = ai_source(i) + ai_dest(i)
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
        Public Shared Operator -(ByVal ai_source As clsEasyVector, ByVal ai_dest As clsEasyVector) As clsEasyVector
            If IsSameDimension(ai_source, ai_dest) = False Then
                Throw New clsException(clsException.Series.DifferElementNumber)
            End If

            Dim ret As New clsEasyVector(ai_source)
            For i As Integer = 0 To ret.Count - 1
                ret(i) = ai_source(i) - ai_dest(i)
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Product(scalr * vector)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator *(ByVal ai_source As Double, ByVal ai_dest As clsEasyVector) As clsEasyVector
            Dim ret As New clsEasyVector(ai_dest)
            For i As Integer = 0 To ret.Count - 1
                ret(i) = ai_source * ai_dest(i)
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Product(vector * scalar)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator *(ByVal ai_source As clsEasyVector, ByVal ai_dest As Double) As clsEasyVector
            Dim ret As New clsEasyVector(ai_source)
            For i As Integer = 0 To ret.Count - 1
                ret(i) = ai_source(i) * ai_dest
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Product(vector * vector)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator *(ByVal ai_source As clsEasyVector, ByVal ai_dest As clsEasyVector) As clsEasyMatrix
            Dim n = ai_source.Count
            If n <> ai_dest.Count Then
                Throw New clsException(clsException.Series.NotComputable, "Vector * Vector - size error")
            End If
            If ai_source.Direction = VectorDirection.ROW AndAlso ai_dest.Direction = VectorDirection.COL Then
                '|v1 v2| * |v3|
                '          |v4|
                Dim temp As Double = 0.0
                For i As Integer = 0 To n - 1
                    temp += ai_source(i) * ai_dest(i)
                Next
                Dim ret = New clsEasyMatrix(1)
                ret(0)(0) = temp
                Return ret
            ElseIf ai_source.Direction = VectorDirection.COL AndAlso ai_dest.Direction = VectorDirection.ROW Then
                '|v1| * |v3 v4|
                '|v2|
                Dim ret = New clsEasyMatrix(n)

#If (NET30_CUSTOM OrElse NET35_CUSTOM OrElse NET35) = True Then
                '------------------------------------------------------------------
                '.net 3.0, 3.5
                '------------------------------------------------------------------
                For i = 0 To n - 1
                    For j = 0 To n - 1
                                                     ret(j)(i) = ai_source(i) * ai_dest(j)
                                                 Next
                Next
#Else
                '------------------------------------------------------------------
                '.net 4.0
                '------------------------------------------------------------------
                'using Paralle .NET4
                'Dim pOption = New Threading.Tasks.ParallelOptions()
                Threading.Tasks.Parallel.For(0, n,
                                             Sub(i)
                                                 For j = 0 To n - 1
                                                     ret(j)(i) = ai_source(i) * ai_dest(j)
                                                 Next
                                             End Sub)
#End If
                Return ret
            End If

            'error
            Throw New clsException(clsException.Series.NotComputable, "Vector * Vector - direction error")
        End Operator

        ''' <summary>
        ''' Divide
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator /(ByVal ai_source As Double, ByVal ai_dest As clsEasyVector) As clsEasyVector
            Dim ret As New clsEasyVector(ai_dest)
            For i As Integer = 0 To ret.Count - 1
                ret(i) = ai_source / ai_dest(i)
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Divide
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator /(ByVal ai_source As clsEasyVector, ByVal ai_dest As Double) As clsEasyVector
            Dim ret As New clsEasyVector(ai_source)
            For i As Integer = 0 To ret.Count - 1
                ret(i) = ai_source(i) / ai_dest
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Power(exponentiation)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator ^(ByVal ai_source As clsEasyVector, ByVal ai_dest As Double) As clsEasyVector
            Dim ret As New clsEasyVector(ai_source)
            For i As Integer = 0 To ret.Count - 1
                ret(i) = Math.Pow(ai_source(i), ai_dest)
            Next
            Return ret
        End Operator
#End Region

#Region "Public Utility"
        ''' <summary>
        ''' Set List(Of double)
        ''' </summary>
        ''' <param name="ai_list"></param>
        ''' <remarks></remarks>
        Public Sub SetList(ByRef ai_list As List(Of Double))
            Me.Clear()
            For i As Integer = 0 To ai_list.Count - 1
                Me.Add(ai_list(i))
            Next
        End Sub

        ''' <summary>
        ''' Set Double()
        ''' </summary>
        ''' <param name="ai_list"></param>
        ''' <remarks></remarks>
        Public Sub SetList(ByVal ai_list() As Double)
            Me.Clear()
            For i As Integer = 0 To ai_list.Length - 1
                Me.Add(ai_list(i))
            Next
        End Sub

        ''' <summary>
        ''' To Matrix
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToMatrix() As clsEasyMatrix
            If Me.m_direcition = VectorDirection.ROW Then
                Return New clsEasyMatrix(Me, VectorDirection.ROW)
            Else
                Return New clsEasyMatrix(Me, VectorDirection.COL)
            End If
        End Function

        ''' <summary>
        ''' Transpose
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function T() As clsEasyVector
            Dim temp As New clsEasyVector(Me)
            If temp.Direction = VectorDirection.ROW Then
                temp.Direction = VectorDirection.COL
            Else
                temp.Direction = VectorDirection.ROW
            End If
            Return temp
        End Function

        ''' <summary>
        ''' create diagonal matrix from vector
        ''' </summary>
        ''' <returns></returns>
        Public Function ToDiagonalMatrix() As clsEasyMatrix
            Dim ret = New clsEasyMatrix(Me.Count)
            For i As Integer = 0 To ret.Count - 1
                ret(i)(i) = Me(i)
            Next
            Return ret
        End Function

        ''' <summary>
        ''' Resize vector dimension
        ''' </summary>
        ''' <param name="vecDim">vector dimension</param>
        Public Sub Resize(ByVal vecDim As Integer)
            Me.Clear()
            If vecDim <> 0 Then
                Me.AddRange(New Double(vecDim - 1) {})
            End If
        End Sub

        ''' <summary>
        ''' For Debug
        ''' </summary>
        ''' <param name="ai_preci"></param>
        ''' <remarks></remarks>
        Public Sub PrintValue(Optional ByVal ai_preci As Integer = 4, Optional ByVal name As String = "")
            Dim str As New System.Text.StringBuilder()
            If String.IsNullOrEmpty(name) = False Then
                str.Append(String.Format("{0} =", name) & Environment.NewLine)
            Else
                str.Append("Vec =" & Environment.NewLine)
            End If
            If Me.m_direcition = VectorDirection.ROW Then
                For i As Integer = 0 To Me.Count - 1
                    str.Append(Me(i).ToString("F" & ai_preci.ToString()) & vbTab)
                Next
                str.AppendLine("")
            Else
                For i As Integer = 0 To Me.Count - 1
                    str.Append(Me(i).ToString("F" & ai_preci.ToString()))
                    str.AppendLine("")
                Next
            End If
            str.Append(Environment.NewLine)
            Console.Write(str.ToString())
        End Sub
#End Region

#Region "Public Func"
        ''' <summary>
        ''' Norm L1 ( |x| = |x1| + |x2| ... )
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Refference:
        ''' 皆本晃弥, "C言語による「数値計算入門」～ 解法・アルゴリズム・プログラム ～", サイエンス社 2008年 初版第4刷, pp28-32
        ''' </remarks>
        Public Function NormL1() As Double
            Dim ret As Double = 0.0
            For Each value As Double In Me
                ret += Math.Abs(value)
            Next
            Return ret
        End Function

        ''' <summary>
        ''' Norm L2 ( ||x||2 = Sqrt( x1^2 + x2^2 ... ) )
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Refference:
        ''' 皆本晃弥, "C言語による「数値計算入門」～ 解法・アルゴリズム・プログラム ～", サイエンス社 2008年 初版第4刷, pp28-32
        ''' </remarks>
        Public Function NormL2() As Double
            Dim ret As Double = 0.0
            For Each val As Double In Me
                ret += val * val
            Next
            Return Math.Sqrt(ret)
        End Function

        ''' <summary>
        ''' NormMax
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Refference:
        ''' 皆本晃弥, "C言語による「数値計算入門」～ 解法・アルゴリズム・プログラム ～", サイエンス社 2008年 初版第4刷, pp28-32
        ''' </remarks>
        Public Function NormMax() As Double
#If NET30_CUSTOM = True Then
            Dim ret As New List(Of Double)
            For Each Val As Double In Me
                ret.Add(Math.Abs(Val))
            Next
            ret.Sort()
            Return ret(ret.Count - 1)
#Else
            Return Me.Max()
#End If
        End Function

        ''' <summary>
        ''' x1 + x2 + ... + xn
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Sum() As Double
            Dim ret As Double = 0.0
            For Each value As Double In Me
                ret += value
            Next
            Return ret
        End Function

        ''' <summary>
        ''' Square Sum ( x1^2 + x2^2... )
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SquareSum() As Double
            Dim ret As Double = 0.0
            For Each value As Double In Me
                ret += value * value
            Next
            Return ret
        End Function

        ''' <summary>
        ''' Prodcut ( x1 * x2 * ... * xn )
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Product() As Double
            Dim ret As Double = 1.0
            For Each value As Double In Me
                ret *= value
            Next
            Return ret
        End Function

        ''' <summary>
        ''' Average
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Average() As Double
            Return Me.Sum() / Me.Count
        End Function

        ''' <summary>
        ''' Inner product, dot product (a1*b1 + a2*b2 ... )
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' a dot b = |a||b|cos(theta)
        ''' </remarks>
        Public Function InnerProduct(ByVal ai_source As clsEasyVector) As Double
            If IsSameDimension(ai_source, Me) = False Then
                Throw New clsException(clsException.Series.DifferElementNumber)
            End If
            Dim ret As Double = 0
            For i As Integer = 0 To ai_source.Count - 1
                ret += ai_source(i) * Me(i)
            Next
            Return ret
        End Function

        ''' <summary>
        ''' Product(Outer product, cross product)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function OuterProduct(ByVal ai_source As clsEasyVector) As clsEasyVector
            If IsSameDimension(ai_source, Me) = False Then
                Throw New clsException(clsException.Series.DifferElementNumber)
            End If

            Dim ret = New clsEasyVector(ai_source.Count)
            If ai_source.Count = 0 Then
                Throw New clsException(clsException.Series.NotComputable, "OuterProduct")
            ElseIf ai_source.Count = 1 Then
                Throw New clsException(clsException.Series.NotComputable, "OuterProduct")
            ElseIf ai_source.Count = 2 Then
                Throw New clsException(clsException.Series.NotComputable, "2 dim outerproduct is scalar. this value is area.")
            ElseIf ai_source.Count = 3 Then
                ret(0) = Me(1) * ai_source(2) - Me(2) * ai_source(1)
                ret(1) = Me(2) * ai_source(0) - Me(2) * ai_source(2)
                ret(2) = Me(0) * ai_source(1) - Me(2) * ai_source(0)
            Else
                Throw New clsException(clsException.Series.NotComputable, "sorry, not implementation")
            End If
            Return ret
        End Function

        ''' <summary>
        ''' Hadamard product ( a1 * b1, a2 * b2, ... )
        ''' </summary>
        ''' <param name="b">( a1 * a1, a2 * a2, ... )</param>
        ''' <returns></returns>
        Public Function HadamardProduct(Optional ByRef b As clsEasyVector = Nothing) As clsEasyVector
            If b Is Nothing Then
                Dim ret = New clsEasyVector(Me.Count)
                For i As Integer = 0 To Me.Count - 1
                    ret(i) = Me(i) * Me(i)
                Next
                Return ret
            Else
                If IsSameDimension(b, Me) = False Then
                    Throw New clsException(clsException.Series.DifferElementNumber)
                End If
                Dim ret = New clsEasyVector(Me.Count)
                For i As Integer = 0 To Me.Count - 1
                    ret(i) = Me(i) * b(i)
                Next
                Return ret
            End If
        End Function
#End Region

#Region "Public Shared"

#End Region

#Region "Private"
        ''' <summary>
        ''' CheckDimension
        ''' </summary>
        ''' <param name="ai_vec1"></param>
        ''' <param name="ai_vec2"></param>
        ''' <param name="isCheckDirection"></param>
        ''' <returns></returns>
        Private Shared Function IsSameDimension(ByVal ai_vec1 As clsEasyVector,
                                                ByVal ai_vec2 As clsEasyVector,
                                                Optional ByVal isCheckDirection As Boolean = False) As Boolean
            If ai_vec1 Is Nothing Then
                Return False
            End If
            If ai_vec2 Is Nothing Then
                Return False
            End If
            If ai_vec1.Count <> ai_vec2.Count Then
                Return False
            End If
            If isCheckDirection = True Then
                If ai_vec1.Direction <> ai_vec2.Direction Then
                    Return False
                End If
            End If

            Return True
        End Function
#End Region
    End Class
End Namespace

Namespace MathUtil
    ''' <summary>
    ''' Vector class
    ''' </summary>
    ''' <remarks>
    ''' Inherits List(of double)
    ''' </remarks>
    Public Class clsShoddyVector : Inherits List(Of Double)
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
        Public Sub New(ByVal ai_base As clsShoddyVector)
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

#Region "Public Operator"
        ''' <summary>
        ''' Type convert
        ''' </summary>
        ''' <param name="ai_ar"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' double() -> clsShoddyVector
        ''' </remarks>
        Public Shared Widening Operator CType(ByVal ai_ar() As Double) As clsShoddyVector
            Return New clsShoddyVector(ai_ar)
        End Operator

        ''' <summary>
        ''' Add
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator +(ByVal ai_source As clsShoddyVector, ByVal ai_dest As clsShoddyVector) As clsShoddyVector
            If IsSameDimension(ai_source, ai_dest) = False Then
                Throw New clsException(clsException.Series.DifferElementNumber)
            End If

            Dim ret As New clsShoddyVector(ai_source)
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
        Public Shared Operator -(ByVal ai_source As clsShoddyVector, ByVal ai_dest As clsShoddyVector) As clsShoddyVector
            If IsSameDimension(ai_source, ai_dest) = False Then
                Throw New clsException(clsException.Series.DifferElementNumber)
            End If

            Dim ret As New clsShoddyVector(ai_source)
            For i As Integer = 0 To ret.Count - 1
                ret(i) = ai_source(i) - ai_dest(i)
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Product(Inner product, dot product)
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' a dot b = |a||b|cos(theta)
        ''' </remarks>
        Public Function InnerProduct(ByVal ai_source As clsShoddyVector) As Double
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
        ''' Product
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator *(ByVal ai_source As Double, ByVal ai_dest As clsShoddyVector) As clsShoddyVector
            Dim ret As New clsShoddyVector(ai_dest)
            For i As Integer = 0 To ret.Count - 1
                ret(i) = ai_source * ai_dest(i)
            Next
            Return ret
        End Operator

        ''' <summary>
        ''' Product
        ''' </summary>
        ''' <param name="ai_source"></param>
        ''' <param name="ai_dest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator *(ByVal ai_source As clsShoddyVector, ByVal ai_dest As Double) As clsShoddyVector
            Dim ret As New clsShoddyVector(ai_source)
            For i As Integer = 0 To ret.Count - 1
                ret(i) = ai_source(i) * ai_dest
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
        Public Shared Operator /(ByVal ai_source As Double, ByVal ai_dest As clsShoddyVector) As clsShoddyVector
            Dim ret As New clsShoddyVector(ai_dest)
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
        Public Shared Operator /(ByVal ai_source As clsShoddyVector, ByVal ai_dest As Double) As clsShoddyVector
            Dim ret As New clsShoddyVector(ai_source)
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
        Public Shared Operator ^(ByVal ai_source As clsShoddyVector, ByVal ai_dest As Double) As clsShoddyVector
            Dim ret As New clsShoddyVector(ai_source)
            For i As Integer = 0 To ret.Count - 1
                ret(i) = Math.Pow(ai_source(i), ai_dest)
            Next
            Return ret
        End Operator
#End Region

#Region "Public"
        ''' <summary>
        ''' Set List(Of double)
        ''' </summary>
        ''' <param name="ai_list"></param>
        ''' <remarks></remarks>
        Public Sub SetList(ByVal ai_list As List(Of Double))
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
        Public Function ToMatrix() As clsShoddyMatrix
            If Me.m_direcition = VectorDirection.ROW Then
                Return New clsShoddyMatrix(Me, VectorDirection.ROW)
            Else
                Return New clsShoddyMatrix(Me, VectorDirection.COL)
            End If
        End Function

        ''' <summary>
        ''' Norm L1 ( |x1| + |x2| ... )
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' |x|1
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
        ''' Norm L2 ( Sqrt( x1^2 + x2^2 ... ) )
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' ||x||
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
        ''' |x|max
        ''' Refference:
        ''' 皆本晃弥, "C言語による「数値計算入門」～ 解法・アルゴリズム・プログラム ～", サイエンス社 2008年 初版第4刷, pp28-32
        ''' </remarks>
        Public Function NormMax() As Double
            Dim ret As New List(Of Double)
            For Each Val As Double In Me
                ret.Add(Math.Abs(Val))
            Next
            ret.Sort()
            Return ret(ret.Count - 1)
        End Function

        ''' <summary>
        ''' Transpose
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function T() As clsShoddyVector
            Dim temp As New clsShoddyVector(Me)
            If temp.Direction = VectorDirection.ROW Then
                temp.Direction = VectorDirection.COL
            Else
                temp.Direction = VectorDirection.ROW
            End If
            Return temp
        End Function

        ''' <summary>
        ''' Sum
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
        ''' Average
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Average() As Double
            Return Me.Sum / Me.Count
        End Function

        ''' <summary>
        ''' For Debug
        ''' </summary>
        ''' <param name="ai_preci"></param>
        ''' <remarks></remarks>
        Public Sub PrintValue(Optional ByVal ai_preci As Integer = 3)
            Dim str As New System.Text.StringBuilder()
            If Me.m_direcition = VectorDirection.ROW Then
                For i As Integer = 0 To Me.Count - 1
                    str.Append(Me(i).ToString("F" & ai_preci.ToString()) & ControlChars.Tab)
                Next
                str.AppendLine("")
            Else
                For i As Integer = 0 To Me.Count - 1
                    str.Append(Me(i).ToString("F" & ai_preci.ToString()))
                    str.AppendLine("")
                Next
            End If
            Console.Write(str.ToString())
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

#Region "Private"
        ''' <summary>
        ''' CheckDimension
        ''' </summary>
        ''' <param name="ai_vec1"></param>
        ''' <param name="ai_vec2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function IsSameDimension(ByVal ai_vec1 As clsShoddyVector, ByVal ai_vec2 As clsShoddyVector) As Boolean
            If ai_vec1 Is Nothing Then
                Return False
            End If
            If ai_vec2 Is Nothing Then
                Return False
            End If
            If ai_vec1.Count <> ai_vec2.Count Then
                Return False
            End If
            If ai_vec1.Direction <> ai_vec2.Direction Then
                Return False
            End If

            Return True
        End Function
#End Region
    End Class

End Namespace

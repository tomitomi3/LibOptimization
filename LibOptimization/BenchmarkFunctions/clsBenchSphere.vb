Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' Sphere Function
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  F(0,...,0) = 0
    ''' Range:
    '''  -5.12 to 5.12
    ''' Referrence:
    ''' http://mikilab.doshisha.ac.jp/dia/research/pdga/archive/doc/ga2k_performance.pdf
    ''' </remarks>
    <Serializable>
    Public Class clsBenchSphere : Inherits absObjectiveFunction
        Private dimension As Integer = 0

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <param name="ai_dim">Set dimension</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_dim As Integer)
            Me.dimension = ai_dim
        End Sub

        ''' <summary>
        ''' Target Function
        ''' </summary>
        ''' <param name="ai_var"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function F(ByVal ai_var As List(Of Double)) As Double
            If ai_var Is Nothing Then
                Return 0
            End If

            Dim ret As Double = 0
            For i As Integer = 0 To Me.dimension - 1
                ret += ai_var(i) ^ 2
            Next
            Return ret
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
            Dim ret As New List(Of Double)
            For i As Integer = 0 To Me.dimension - 1
                ret.Add(2.0 * ai_var(i))
            Next
            Return ret
        End Function

        Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
            Dim ret As New List(Of List(Of Double))
            For i As Integer = 0 To Me.dimension - 1
                ret.Add(New List(Of Double))
                For j As Integer = 0 To Me.dimension - 1
                    If i = j Then
                        ret(i).Add(2.0)
                    Else
                        ret(i).Add(0)
                    End If
                Next
            Next
            Return ret
        End Function

        Public Overrides Function NumberOfVariable() As Integer
            Return dimension
        End Function
    End Class

End Namespace

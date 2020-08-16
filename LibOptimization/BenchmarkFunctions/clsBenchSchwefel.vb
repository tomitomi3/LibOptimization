Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' Schwefel function
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  F(420.96875,...,420.96875) = 0
    ''' Range:
    '''  -512 to 512
    ''' Referrence:
    ''' http://mikilab.doshisha.ac.jp/dia/research/pdga/archive/doc/ga2k_performance.pdf
    ''' </remarks>
    <Serializable>
    Public Class clsBenchSchwefel : Inherits absObjectiveFunction
        Private dimension As Integer = 0

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_dim As Integer)
            Me.dimension = ai_dim
        End Sub

        ''' <summary>
        ''' Target Function
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function F(ByVal x As List(Of Double)) As Double
            If x Is Nothing Then
                Return 0
            End If

            If Me.dimension <> x.Count Then
                Return 0
            End If

            Dim ret As Double = 0.0
            For i As Integer = 0 To Me.dimension - 1
                If (x(i) >= -512) AndAlso (x(i) <= 512) Then
                    ret += -x(i) * Math.Sin(Math.Sqrt(Math.Abs(x(i))))
                Else
                    'out of range
                    ret += Math.Abs(x(i))
                End If
            Next
            ret = ret + 418.982887272434 * Me.dimension

            Return ret
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
            Throw New NotImplementedException
        End Function

        Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
            Throw New NotImplementedException
        End Function

        Public Overrides Function NumberOfVariable() As Integer
            Return Me.dimension
        End Function
    End Class
End Namespace

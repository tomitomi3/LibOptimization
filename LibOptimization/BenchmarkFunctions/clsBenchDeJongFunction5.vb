Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' De Jong’s function 5
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  x = {-32,-32}
    '''  f(x) ~ 1
    ''' Range
    '''  -65.536 ~ 65.536
    ''' Refference:
    '''  De Jong, K. A., "Analysis of the Behavior of a Class of Genetic Adaptive Systems", PhD dissertation, The University of Michigan, Computer and Communication Sciences Department (1975)
    ''' </remarks>
    Public Class clsBenchDeJongFunction5 : Inherits absObjectiveFunction
        Private ReadOnly a()() As Double = {New Double() {-32, -16, 0, 16, 32, -32, -16, 0, 16, 32, -32, -16, 0, 16, 32, -32, -16, 0, 16, 32, -32, -16, 0, 16, 32}, _
                                            New Double() {-32, -32, -32, -32, -32, -16, -16, -16, -16, -16, 0, 0, 0, 0, 0, 16, 16, 16, 16, 16, 32, 32, 32, 32, 32}}

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
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

            'Correc value
            ' Taken from : http://jp.mathworks.com/help/gads/example-minimizing-de-jongs-fifth-function.html
            ' x = {-16.1292, -15.8214}
            ' f(x) = 6.9034

            'range check
            If (x(0) >= -65.536) AndAlso (x(0) <= 65.536) Then
                If (x(1) >= -65.536) AndAlso (x(1) <= 65.536) Then
                    Dim ret As Double = 1 / 500
                    For j As Integer = 0 To 24
                        ret += 1 / (j + 1 + (x(0) - a(0)(j)) ^ 6 + (x(1) - a(1)(j)) ^ 6)
                    Next

                    Return 1 / ret
                End If
            End If

            'out of range
            Return Math.Abs(x(0)) + Math.Abs(x(1)) + 1000
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
            Throw New NotImplementedException
        End Function

        Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
            Throw New NotImplementedException
        End Function

        Public Overrides ReadOnly Property NumberOfVariable As Integer
            Get
                Return 2
            End Get
        End Property
    End Class

End Namespace

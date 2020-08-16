Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' De Jong’s function 3 (Step Function)
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  x = {-5.12~-5, -5.12~-5, -5.12~-5, -5.12~-5, -5.12~-5}
    ''' Range
    '''  -5.12 ~ 5.12 
    ''' Refference:
    '''  De Jong, K. A., "Analysis of the Behavior of a Class of Genetic Adaptive Systems", PhD dissertation, The University of Michigan, Computer and Communication Sciences Department (1975)
    ''' </remarks>
    <Serializable>
    Public Class clsBenchDeJongFunction3 : Inherits absObjectiveFunction
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

            Dim ret As Double = 0
            For i As Integer = 0 To 4
                If (x(i) >= -5.12) AndAlso (x(i) <= 5.12) Then
                    ret += Math.Floor(x(i))
                Else
                    'out of range
                    ret += Math.Abs(x(i)) 'penarty
                End If
            Next
            Return ret
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
            Throw New NotImplementedException
        End Function

        Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
            Throw New NotImplementedException
        End Function

        Public Overrides Function NumberOfVariable() As Integer
            Return 5
        End Function
    End Class

End Namespace

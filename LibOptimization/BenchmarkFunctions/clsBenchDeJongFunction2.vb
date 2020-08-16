Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' De Jong’s function 2 (2D Rosenblock Function)
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  F(1, 1) = 0
    ''' Range
    '''  -2.048 ~ 2.048
    ''' Refference:
    '''  De Jong, K. A., "Analysis of the Behavior of a Class of Genetic Adaptive Systems", PhD dissertation, The University of Michigan, Computer and Communication Sciences Department (1975)
    ''' </remarks>
    <Serializable>
    Public Class clsBenchDeJongFunction2 : Inherits absObjectiveFunction
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

            If (x(0) >= -2.048) AndAlso (x(0) <= 2.048) Then
                If (x(1) >= -2.048) AndAlso (x(1) <= 2.048) Then
                    Return 100 * ((x(0) ^ 2 - x(1)) ^ 2) + (1 - x(0)) ^ 2
                End If
            End If

            'out of range
            Return Math.Abs(x(0)) * Math.Abs(x(1)) * 100 'penarty
        End Function

        Public Overrides Function Gradient(ByVal x As List(Of Double)) As List(Of Double)
            Throw New NotImplementedException
        End Function

        Public Overrides Function Hessian(ByVal x As List(Of Double)) As List(Of List(Of Double))
            Throw New NotImplementedException
        End Function

        Public Overrides Function NumberOfVariable() As Integer
            Return 2
        End Function
    End Class

End Namespace

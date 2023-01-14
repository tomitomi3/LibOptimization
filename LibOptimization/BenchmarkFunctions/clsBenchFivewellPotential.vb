Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' Fivewell-Potential
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  F(4.92, -9.89) = -1.4616
    ''' Range:
    '''  -20 to 20
    ''' Referrence:
    ''' Ilya Pavlyukevich, "Levy flights, non-local search and simulated annealing", Journal of Computational Physics 226 (2007) 1830-1844
    ''' </remarks>
    <Serializable>
    Public Class clsBenchFivewellPotential : Inherits absObjectiveFunction
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

            If Me.NumberOfVariable <> x.Count Then
                Return 0
            End If

            Dim ret As Double = 0.0
            ret = 1 - 1 / (1 + 0.05 * (x(0) ^ 2 + (x(1) - 10) ^ 2)) - 1 / (1 + 0.05 * ((x(0) - 10)) ^ 2 + x(1) ^ 2) - 1.5 / (1 + 0.03 * ((x(0) + 10)) ^ 2 + x(1) ^ 2) - 2 / (1 + 0.05 * ((x(0) - 5)) ^ 2 + (x(1) + 10) ^ 2) - 1 / (1 + 0.1 * ((x(0) + 5)) ^ 2 + (x(1) + 10) ^ 2)
            ret = ret * (1 + 0.0001 * (x(0) ^ 2 + x(1) ^ 2) ^ 1.2)
            Return ret
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double), Optional h As Double = 0.00000001) As List(Of Double)
            Throw New NotImplementedException
        End Function

        Public Overrides Function Hessian(ByVal ai_var As List(Of Double), Optional h As Double = 0.00000001) As List(Of List(Of Double))
            Throw New NotImplementedException
        End Function

        Public Overrides Function NumberOfVariable() As Integer
            Return 2
        End Function
    End Class

End Namespace

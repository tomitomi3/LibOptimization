Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' ShubertsFunction
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  Fmin = −186.7309
    ''' Range:
    '''  −10≦x1 , x2≦10
    ''' Referrence:
    ''' [1]Test fXin-She Yang, "Test Problems in Optimization", arXiv(http://arxiv.org/abs/1008.0549)
    ''' </remarks>
    <Serializable>
    Public Class clsBenchShubertsFunction : Inherits absObjectiveFunction
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
            Dim x1 = x(0)
            Dim x2 = x(1)
            Dim temp1 As Double = (Math.Cos(1 + (1 + 1) * x1) + 2 * Math.Cos(2 + (2 + 1) * x1) + 3 * Math.Cos(3 + (3 + 1) * x1) + 4 * Math.Cos(4 + (4 + 1) * x1) + 5 * Math.Cos(5 + (5 + 1) * x1))
            Dim temp2 As Double = (Math.Cos(1 + (1 + 1) * x2) + 2 * Math.Cos(2 + (2 + 1) * x2) + 3 * Math.Cos(3 + (3 + 1) * x2) + 4 * Math.Cos(4 + (4 + 1) * x2) + 5 * Math.Cos(5 + (5 + 1) * x2))

            Return temp1 * temp2
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
            Throw New NotImplementedException
        End Function

        Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
            Throw New NotImplementedException
        End Function

        Public Overrides Function NumberOfVariable() As Integer
            Return 2
        End Function
    End Class

End Namespace

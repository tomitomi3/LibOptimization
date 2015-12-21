Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' Booth Function
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  F(1,3) = 0
    ''' Range:
    '''  -10 to 10
    ''' Referrence:
    ''' http://www-optima.amp.i.kyoto-u.ac.jp/member/student/hedar/Hedar_files/TestGO_files/Page816.htm
    ''' </remarks>
    Public Class clsBenchBoothFunction : Inherits absObjectiveFunction
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

            Return (x(0) + 2 * x(1) - 7) ^ 2 + (2 * x(0) + x(1) - 5) ^ 2
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

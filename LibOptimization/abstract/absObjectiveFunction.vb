Namespace Optimization
    ''' <summary>
    ''' Abstarct objective function class
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class absObjectiveFunction
        ''' <summary>
        ''' Get number of variables
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function NumberOfVariable() As Integer

        ''' <summary>
        ''' Evaluate
        ''' </summary>
        ''' <param name="x"></param>
        ''' <remarks></remarks>
        Public MustOverride Function F(ByVal x As List(Of Double)) As Double

        ''' <summary>
        ''' Gradient vector (for Steepest descent method, newton method)
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' ex)
        ''' f(x1,..,xn) = x1^2 + ... + xn^2
        ''' del f =  [df/dx1 , ... , df/dxn]
        ''' </remarks>
        Public MustOverride Function Gradient(ByVal x As List(Of Double)) As List(Of Double)

        ''' <summary>
        ''' Hessian matrix (for newton method)
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' ex)
        ''' f(x1,x2) = x1^2 + x2^2
        ''' del f   =  [df/dx1 df/dx2]
        ''' del^2 f = [d^2f/d^2x1     d^2f/dx1dx2]
        '''           [d^2f/d^2dx2dx1 d^2f/d^2x2]
        ''' </remarks>
        Public MustOverride Function Hessian(ByVal x As List(Of Double)) As List(Of List(Of Double))

        ''' <summary>
        ''' Numerical derivertive
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="h"></param>
        ''' <returns></returns>
        Public Function NumericDerivertive(ByVal x As List(Of Double), Optional ByVal h As Double = 0.0000000001) As List(Of Double)
            Dim df As New List(Of Double)
            For i As Integer = 0 To NumberOfVariable() - 1
                '中心差分
                Dim tempX1 = x.ToList()
                tempX1(i) = tempX1(i) + h
                Dim tempX2 = x.ToList()
                tempX2(i) = tempX2(i) - h
                Dim tempDf = (F(tempX1) - F(tempX2))
                tempDf = tempDf / (2.0 * h)
                df.Add(tempDf)
            Next
            Return df
        End Function
    End Class

End Namespace
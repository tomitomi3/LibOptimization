Namespace Optimization
    ''' <summary>
    ''' Abstarct objective function class
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class absObjectiveFunction
        ''' <summary>
        ''' Get number of variables
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride ReadOnly Property NumberOfVariable() As Integer

        ''' <summary>
        ''' Evaluate
        ''' </summary>
        ''' <param name="x"></param>
        ''' <remarks></remarks>
        Public MustOverride Function F(ByVal x As List(Of Double)) As Double

        ''' <summary>
        ''' Gradient vector
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
        ''' Hessian matrix
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
    End Class

End Namespace
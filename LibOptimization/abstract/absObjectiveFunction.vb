Namespace Optimization
    ''' <summary>
    ''' Abstarct objective function class
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable>
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
        ''' <param name="h">step default 1e-6</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' ex)
        ''' f(x1,..,xn) = x1^2 + ... + xn^2
        ''' del f =  [df/dx1 , ... , df/dxn]
        ''' </remarks>
        Public Overridable Function Gradient(ByVal x As List(Of Double),
                                             Optional ByVal h As Double = 0.000001) As List(Of Double)
            Return MathTool.MathUtil.CalcNumericGradient(Me, x, h)
        End Function

        ''' <summary>
        ''' Hessian matrix (for newton method)
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="h">step default 1e-6</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' ex)
        ''' f(x1,x2) = x1^2 + x2^2
        ''' del f   =  [df/dx1 df/dx2]
        ''' del^2 f = [d^2f/d^2x1     d^2f/dx1dx2]
        '''           [d^2f/d^2dx2dx1 d^2f/d^2x2]
        ''' </remarks>
        Public Overridable Function Hessian(ByVal x As List(Of Double),
                                            Optional ByVal h As Double = 0.000001) As List(Of List(Of Double))
            Return MathTool.MathUtil.CalcNumericHessian(Me, x, h)
        End Function
    End Class
End Namespace
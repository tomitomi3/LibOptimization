﻿Namespace Optimization
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
                                             Optional ByVal h As Double = 0.00000001) As List(Of Double)
            Return Me.CalcNumericGradient(x)
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
                                            Optional ByVal h As Double = 0.00000001) As List(Of List(Of Double))
            Return Me.CalcNumericDiagonalHessianApproximation(x)
        End Function

        ''' <summary>
        ''' Numerical derivertive
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="h">step default 1e-6</param>
        ''' <returns></returns>
        Public Function CalcNumericGradient(ByVal x As List(Of Double), Optional ByVal h As Double = 0.00000001) As List(Of Double)
            Dim gradient As New List(Of Double)
            Dim n = NumberOfVariable()
            For i As Integer = 0 To n - 1
                'central differences
                Dim tempX1 As New List(Of Double)(x)
                Dim tempX2 As New List(Of Double)(x)
                tempX1(i) = tempX1(i) + h
                tempX2(i) = tempX2(i) - h

                'diff
                Dim tempDf = (F(tempX1) - F(tempX2)) / (2.0 * h)
                gradient.Add(tempDf)
            Next
            Return gradient
        End Function

        ''' <summary>
        ''' Diagonal Hessian Approximation ヘッセ行列の対角近似
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="h">step default 1e-6</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' ex)
        ''' f(x1,x2) = x1^2 + x2^2
        ''' del^2 f = [d^2f/d^2x1   0         ]
        '''           [0            d^2f/d^2x2]
        ''' </remarks>
        Public Function CalcNumericDiagonalHessianApproximation(ByVal x As List(Of Double),
                                                                Optional ByVal h As Double = 0.000001) As List(Of List(Of Double))
            '2階微分 d^2x/df^2
            Dim secDerivertive As New List(Of Double)
            For i As Integer = 0 To NumberOfVariable() - 1
                Dim tempX1 As New List(Of Double)(x)
                Dim tempX2 As New List(Of Double)(x)
                tempX1(i) = tempX1(i) + h
                tempX2(i) = tempX2(i) - h

                'x_+1 - 2*x + x_-1 / h^2
                Dim tempDf = (F(tempX1) - 2.0 * F(x) + F(tempX2)) / (h * h)
                secDerivertive.Add(tempDf)
            Next

            '対角成分のみ
            Dim retDiagH As New List(Of List(Of Double))
            For i As Integer = 0 To Me.NumberOfVariable - 1
                retDiagH.Add(New List(Of Double))
                For j As Integer = 0 To Me.NumberOfVariable - 1
                    If i = j Then
                        retDiagH(i).Add(secDerivertive(i))
                    Else
                        retDiagH(i).Add(0.0)
                    End If
                Next
            Next
            Return retDiagH
        End Function

        ''' <summary>
        ''' Hessian
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="h">step default 1e-6</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' ex)
        ''' f(x1,x2) = x1^2 + x2^2
        ''' del   f = [df/dx1 df/dx2]
        ''' del^2 f = [d^2f/d^2x1     d^2f/dx1dx2]
        '''           [d^2f/d^2dx2dx1 d^2f/d^2x2]
        ''' </remarks>
        Public Function CalcNumericHessian(ByVal x As List(Of Double),
                                           Optional ByVal h As Double = 0.000001) As List(Of List(Of Double))
            'df/dx =~ f(x+h)-f(x-h)/2h
            'd^2f/d^2x = d(df/dx)/dx =~ (f(x+h)-f(x-h)/2h) - (f(x+h)-f(x-h)/2h) / 2h
            Dim n = Me.NumberOfVariable()
            Dim retH As New List(Of List(Of Double))
            For i As Integer = 0 To n - 1
                retH.Add(New List(Of Double))
                For j As Integer = 0 To n - 1
                    Dim ei = New MathTool.DenseVector(n)
                    ei(i) = 1
                    Dim ej = New MathTool.DenseVector(n)
                    ej(j) = 1
                    ei = ei * h
                    ej = ej * h
                    Dim initial = New MathTool.DenseVector(x)
                    Dim f1 = initial + ei + ej
                    Dim f2 = initial + ei - ej
                    Dim f3 = initial - ei + ej
                    Dim f4 = initial - ei - ej
                    Dim tempDf = (F(f1.ToList()) - F(f2.ToList()) - F(f3.ToList()) + F(f4.ToList())) / (4.0 * h * h)
                    retH(i).Add(tempDf)

                    'Dim tempX1 As New List(Of Double)(x)
                    'Dim tempX2 As New List(Of Double)(x)
                    'Dim tempX3 As New List(Of Double)(x)
                    'Dim tempX4 As New List(Of Double)(x)
                    'tempX1(i) = tempX1(i) + h
                    'tempX2(i) = tempX2(i) + h
                    'tempX3(i) = tempX3(i) - h
                    'tempX4(i) = tempX4(i) - h
                    'Dim tempDf = (F(tempX1) - F(tempX2) - F(tempX3) + F(tempX4)) / (4.0 * h * h)
                Next
            Next
            Return retH
        End Function
    End Class
End Namespace
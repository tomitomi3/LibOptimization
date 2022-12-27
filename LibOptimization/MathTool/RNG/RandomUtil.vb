Namespace MathTool.RNG
    ''' <summary>
    ''' Random number generator utility class
    ''' </summary>
    Public Class RandomUtil

        ''' <summary>
        ''' Normal Distribution
        ''' </summary>
        ''' <param name="ai_ave">Average</param>
        ''' <param name="ai_sigma2">Varianse s^2</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' using Box-Muller method
        ''' </remarks>
        Public Shared Function NormRand(Optional ByVal ai_ave As Double = 0,
                                        Optional ByVal ai_sigma2 As Double = 1) As Double
            Dim x As Double = RandomXorshiftSingleton.GetInstance().NextDouble()
            Dim y As Double = RandomXorshiftSingleton.GetInstance().NextDouble()

            Dim c As Double = Math.Sqrt(-2.0 * Math.Log(x))
            If (0.5 - RandomXorshiftSingleton.GetInstance().NextDouble() > 0.0) Then
                Return c * Math.Sin(2.0 * Math.PI * y) * ai_sigma2 + ai_ave
            Else
                Return c * Math.Cos(2.0 * Math.PI * y) * ai_sigma2 + ai_ave
            End If
        End Function

        ''' <summary>
        ''' Normal Distribution using Box-Muller method
        ''' </summary>
        ''' <param name="oRand"></param>
        ''' <param name="ai_ave">ai_ave</param>
        ''' <param name="ai_sigma2">Varianse s^2</param>
        ''' <returns></returns>
        Public Shared Function NormRand(ByVal oRand As System.Random,
                                        Optional ByVal ai_ave As Double = 0,
                                        Optional ByVal ai_sigma2 As Double = 1) As Double
            Dim x As Double = oRand.NextDouble()
            Dim y As Double = oRand.NextDouble()

            Dim c As Double = Math.Sqrt(-2.0 * Math.Log(x))
            If (0.5 - RandomXorshiftSingleton.GetInstance().NextDouble() > 0.0) Then
                Return c * Math.Sin(2.0 * Math.PI * y) * ai_sigma2 + ai_ave
            Else
                Return c * Math.Cos(2.0 * Math.PI * y) * ai_sigma2 + ai_ave
            End If
        End Function

        ''' <summary>
        ''' Cauchy Distribution
        ''' </summary>
        ''' <param name="ai_mu">default:0</param>
        ''' <param name="ai_gamma">default:1</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' http://www.sat.t.u-tokyo.ac.jp/~omi/random_variables_generation.html#Cauchy
        ''' </remarks>
        Public Shared Function CauchyRand(Optional ByVal ai_mu As Double = 0, Optional ByVal ai_gamma As Double = 1) As Double
            Return ai_mu + ai_gamma * Math.Tan(Math.PI * (RandomXorshiftSingleton.GetInstance().NextDouble() - 0.5))
        End Function
    End Class
End Namespace

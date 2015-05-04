Namespace Util
    ''' <summary>
    ''' Levy distributed random number generator
    ''' </summary>
    ''' <remarks>
    ''' Refference:
    ''' 四辻 哲章, "計算機シミュレーションのための確率分布乱数生成法", プレアデス出版, 2010年第1版第1刷
    ''' </remarks>
    Class clsRandomLevy
        Private m_rand As clsRandomXorshift = Nothing

        'Parameters for Half-ND
        Private y1 As Double = 0
        Private y2 As Double = 0
        Private flg As Boolean = True

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            m_rand = New clsRandomXorshift()
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_seed">random seed</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_seed As UInteger)
            m_rand = New clsRandomXorshift(ai_seed)
        End Sub

        ''' <summary>
        ''' Levy distributed random number
        ''' </summary>
        ''' <param name="ai_u"></param>
        ''' <param name="ai_theta"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Random(Optional ByVal ai_u As Double = 0, Optional ByVal ai_theta As Double = 1) As Double
            Dim z As Double = 0
            Do
                z = Me.HalfNormalDistributaionRNG()
            Loop While z < 0

            Return ai_u + ai_theta / (z * z)
        End Function

        ''' <summary>
        ''' Half Normal distributed random number generator
        ''' </summary>
        ''' <param name="ai_sigma2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function HalfNormalDistributaionRNG(Optional ByVal ai_sigma2 As Double = 1) As Double
            Dim u1 As Double = Me.m_rand.NextDouble()
            Dim u2 As Double = Me.m_rand.NextDouble()

            Dim r As Double = Math.Sqrt(-2.0 * Math.Log(u1))
            Dim theta = Math.PI * u2 * 0.5

            Me.y1 = r * Math.Cos(theta)
            Me.y2 = r * Math.Sin(theta)

            If Me.flg = True Then
                Me.flg = False
                Return y1 * ai_sigma2
            Else
                Me.flg = True
                Return y2 * ai_sigma2
            End If
        End Function
    End Class
End Namespace

Namespace MathTool.RNG
    ''' <summary>
    ''' Xorshift random algorithm
    ''' Inherits System.Random
    ''' </summary>
    ''' <remarks>
    ''' Refference:
    ''' George Marsaglia, "Xorshift RNGs", Journal of Statistical Software Vol. 8, Issue 14, Jul 2003
    ''' </remarks>
    <Serializable>
    Public Class clsRandomXorshift

        Inherits System.Random
        'DefaultParameter
        Private x As UInt32 = 123456789
        Private y As UInt32 = 362436069
        Private z As UInt32 = 521288629
        Private w As UInt32 = 88675123
        Private t As UInt32

#Region "Public"
        ''' <summary>
        ''' Constructor with refference seed
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            'nop using member seed
        End Sub

        ''' <summary>
        ''' Constructor with seed
        ''' </summary>
        ''' <param name="ai_seed">seed for random algorithm</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_seed As UInt32)
            SetSeed(ai_seed)
        End Sub

        ''' <summary>
        ''' Set default seed with itinitialize
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub SetDefaultSeed()
            x = 123456789
            y = 362436069
            z = 521288629
            w = 88675123
            t = 0
        End Sub

        ''' <summary>
        ''' Set random seed with itinitialize
        ''' </summary>
        ''' <param name="x">random parameter x</param>
        ''' <param name="y">random parameter y</param>
        ''' <param name="z">random parameter z</param>
        ''' <param name="w">random parameter w</param>
        Public Sub SetSeed(ByVal x As UInteger, ByVal y As UInteger, ByVal z As UInteger, ByVal w As UInteger)
            '"The seed set for xor128 is four 32-bit integers x,y,z,w not all 0" by refference
            If x = 0 AndAlso y = 0 AndAlso z = 0 AndAlso w = 0 Then
                'default parameter
                SetDefaultSeed()
            Else
                Me.x = x
                Me.y = y
                Me.z = z
                Me.w = w
            End If
        End Sub

        ''' <summary>
        ''' Set simple random seed with itinitialize
        ''' </summary>
        ''' <param name="ai_seed">seed for random algorithm</param>
        ''' <remarks></remarks>
        Public Sub SetSeed(ByVal ai_seed As UInt32)
            'Init parameter and rorate seed.
            '全パラメータにseedの影響を与えないと初期の乱数が同じ傾向になる。8bitずつ回転左シフト
            SetDefaultSeed()
            x = x Xor RotateLeftShiftForUInteger(ai_seed, 8)
            y = y Xor RotateLeftShiftForUInteger(ai_seed, 16)
            z = z Xor RotateLeftShiftForUInteger(ai_seed, 24)
            w = w Xor ai_seed 'Set seed
            t = 0

            '"The seed set for xor128 is four 32-bit integers x,y,z,w not all 0" by refference
            If x = 0 AndAlso y = 0 AndAlso z = 0 AndAlso w = 0 Then
                'default parameter
                x = 123456789
                y = 362436069
                z = 521288629
                w = 88675123
                t = 0
            End If
        End Sub

        ''' <summary>
        ''' Override Next
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function [Next]() As Integer
            Return CInt(Xor128() And &H7FFFFFFF)
        End Function

        ''' <summary>
        ''' Override Next
        ''' </summary>
        ''' <param name="maxValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function [Next](maxValue As Integer) As Integer
            Return [Next](0, maxValue)
        End Function

        ''' <summary>
        ''' Override Next
        ''' </summary>
        ''' <param name="minValue"></param>
        ''' <param name="maxValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function [Next](minValue As Integer, maxValue As Integer) As Integer
            Return CInt(minValue + Xor128() Mod (maxValue - minValue))
        End Function

        ''' <summary>
        ''' Override NextDouble
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function NextDouble() As Double
            Return Xor128() / UInteger.MaxValue
        End Function

        ''' <summary>
        ''' Random double with range
        ''' </summary>
        ''' <param name="ai_min"></param>
        ''' <param name="ai_max"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Public Overloads Function NextDouble(ByVal ai_min As Double, ByVal ai_max As Double) As Double
            Dim ret As Double = Math.Abs(ai_max - ai_min) * MyBase.NextDouble() + ai_min
            Return ret
        End Function

        ''' <summary>
        ''' for random seed
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTimeSeed() As UInteger
            Return CUInt(Date.Now.Millisecond * Date.Now.Minute * Date.Now.Second)
        End Function
#End Region

#Region "Private"
        ''' <summary>
        ''' Xor128
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' C Source by refference
        '''  t=(xˆ(x leftshift 11));
        '''  x=y;
        '''  y=z;
        '''  z=w;
        '''  return( w=(wˆ(w rightshift 19))ˆ(tˆ(t rightshift 8)) )
        ''' </remarks>
        Private Function Xor128() As UInteger
            t = (x Xor (x << 11))
            x = y
            y = z
            z = w
            w = (w Xor (w >> 19)) Xor (t Xor (t >> 8))

            Return w
        End Function

        ''' <summary>
        ''' Rotate Shift
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RotateLeftShiftForUInteger(ByVal ai_val As UInteger, ByVal ai_leftshit As Integer) As UInteger
            If 32 - ai_leftshit <= 0 Then
                Return ai_val
            End If

            'keeping upper bits
            Dim maskBit As UInteger = 0
            For i As Integer = 32 - ai_leftshit To 32 - 1
                maskBit = CUInt(maskBit + (2 ^ i))
            Next
            Dim upperBit As UInteger = ai_val And maskBit
            upperBit = upperBit >> (32 - ai_leftshit)

            'left shift
            Dim temp As UInteger = ai_val << ai_leftshit

            'rotate upperbits
            temp = temp Or upperBit
            Return temp
        End Function
#End Region
    End Class

    ''' <summary>
    ''' Xorshift random algorithm singleton
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    <Serializable>
    Public Class clsRandomXorshiftSingleton
        Private Shared m_rand As New clsRandomXorshift()

#Region "Constructor"
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub New()
            'nop
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Instance
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetInstance() As clsRandomXorshift
            Return m_rand
        End Function
#End Region
    End Class
End Namespace

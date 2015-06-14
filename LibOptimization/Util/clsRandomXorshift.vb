Namespace Util
    ''' <summary>
    ''' Xorshift random algorithm
    ''' Inherits System.Random
    ''' </summary>
    ''' <remarks>
    ''' Refference:
    ''' George Marsaglia, "Xorshift RNGs", Journal of Statistical Software Vol. 8, Issue 14, Jul 2003
    ''' </remarks>
    Public Class clsRandomXorshift : Inherits System.Random
        'DefaultParameter
        Private x As UInteger = 123456789
        Private y As UInteger = 362436069
        Private z As UInteger = 521288629
        Private w As UInteger = 88675123
        Private t As UInteger

#Region "Public"
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            'nop
        End Sub

        ''' <summary>
        ''' Constructor with seed
        ''' </summary>
        ''' <param name="ai_seed"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_seed As UInteger)
            Me.SetSeed(ai_seed)
        End Sub

        ''' <summary>
        ''' Set random seed
        ''' </summary>
        ''' <param name="ai_seed"></param>
        ''' <remarks></remarks>
        Public Sub SetSeed(Optional ByVal ai_seed As UInteger = 88675123)
            '"The seed set for xor128 is four 32-bit integers x,y,z,w not all 0" by refference

            'default parameter
            x = 123456789
            y = 362436069
            z = 521288629
            w = 88675123
            t = 0

            If ai_seed = 88675123 Then
                'nop (using default parameter)
            Else
                'Init parameter
                '全パラメータにseedの影響を与えないと初期の乱数が同じ傾向になる。8bitずつ回転左シフト
                x = x Xor Me.RotateLeftShiftForUInteger(ai_seed, 8)
                y = y Xor Me.RotateLeftShiftForUInteger(ai_seed, 16)
                z = z Xor Me.RotateLeftShiftForUInteger(ai_seed, 24)
                w = w Xor ai_seed 'Set seed
                t = 0
            End If
        End Sub

        ''' <summary>
        ''' Override Next
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function [Next]() As Integer
            Return CInt(Me.Xor128 And &H7FFFFFFF)
        End Function

        ''' <summary>
        ''' Override Next
        ''' </summary>
        ''' <param name="maxValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function [Next](maxValue As Integer) As Integer
            Return Me.Next(0, maxValue)
        End Function

        ''' <summary>
        ''' Override Next
        ''' </summary>
        ''' <param name="minValue"></param>
        ''' <param name="maxValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function [Next](minValue As Integer, maxValue As Integer) As Integer
            Return CInt(minValue + Me.Xor128() Mod (maxValue - minValue))
        End Function

        ''' <summary>
        ''' Override NextDouble
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function NextDouble() As Double
            Return Me.Xor128() / UInteger.MaxValue
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

        ''' <summary>
        ''' Set Seed
        ''' </summary>
        ''' <param name="ai_seed"></param>
        ''' <remarks></remarks>
        Public Sub SetSeed(Optional ByVal ai_seed As UInteger = 88675123)
            clsRandomXorshiftSingleton.GetInstance().SetSeed(ai_seed)
        End Sub
#End Region
    End Class
End Namespace

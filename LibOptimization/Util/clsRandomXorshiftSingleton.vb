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
    Public Sub SetSeed(Optional ByVal ai_seed As UInteger = 123456)
        clsRandomXorshiftSingleton.GetInstance().SetSeed(ai_seed)
    End Sub
#End Region
End Class

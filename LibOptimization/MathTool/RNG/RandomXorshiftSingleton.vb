Namespace MathTool.RNG
    ''' <summary>
    ''' Xorshift random algorithm singleton
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    <Serializable>
    Public Class RandomXorshiftSingleton
        Private Shared m_rand As New RandomXorshift()

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
        Public Shared Function GetInstance() As RandomXorshift
            Return m_rand
        End Function
#End Region
    End Class
End Namespace

Namespace Util
    ''' <summary>
    ''' ErrorManage class
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Class clsError
        Private m_errList As New List(Of clsErrorInfomation)

        Enum ErrorType
            NO_ERR
            ERR_INIT
            ERR_OPT_MAXITERATION
            ERR_UNKNOWN
        End Enum

#Region "Error Infomation class"
        ''' <summary>
        ''' Error infomation class
        ''' </summary>
        ''' <remarks></remarks>
        Public Class clsErrorInfomation
            ''' <summary>
            ''' Default constructor(do not use)
            ''' </summary>
            ''' <remarks></remarks>
            Private Sub New()
                'nop
            End Sub

            ''' <summary>
            ''' Constructor
            ''' </summary>
            ''' <param name="ai_setError"></param>
            ''' <param name="ai_errorType"></param>
            ''' <param name="ai_errorMsg"></param>
            ''' <remarks></remarks>
            Public Sub New(ByVal ai_setError As Boolean, ByVal ai_errorType As ErrorType, ByVal ai_errorMsg As String)
                Me.ErrorFlg = ai_setError
                Me.ErrorType = ai_errorType
                Me.ErrorMsg = ai_errorMsg
            End Sub

            Public Property ErrorFlg As Boolean = False
            Public Property ErrorType As ErrorType = ErrorType.NO_ERR
            Public Property ErrorMsg As String = String.Empty
        End Class
#End Region

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Set Error
        ''' </summary>
        ''' <param name="ai_setError"></param>
        ''' <param name="ai_errorType"></param>
        ''' <param name="ai_errorMsg"></param>
        ''' <remarks></remarks>
        Public Sub SetError(ByVal ai_setError As Boolean, ByVal ai_errorType As ErrorType, Optional ByVal ai_errorMsg As String = "")
            Me.m_errList.Add(New clsErrorInfomation(ai_setError, ai_errorType, ai_errorMsg))
        End Sub

        ''' <summary>
        ''' Is error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsError() As Boolean
            If Me.m_errList.Count = 0 Then
                Return False
            End If
            Dim index As Integer = Me.m_errList.Count
            Return Me.m_errList(index - 1).ErrorFlg
        End Function

        ''' <summary>
        ''' Get Last error infomation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLastErrorInfomation() As clsErrorInfomation
            If Me.m_errList.Count = 0 Then
                Return New clsErrorInfomation(False, ErrorType.NO_ERR, "")
            End If
            Dim index As Integer = Me.m_errList.Count
            Return Me.m_errList(index - 1)
        End Function

        ''' <summary>
        ''' Clear error
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Clear()
            Me.m_errList.Clear()
        End Sub

        ''' <summary>
        ''' Error Output
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub Print(ByVal ai_errInfomation As clsErrorInfomation)
            If ai_errInfomation.ErrorFlg = True Then
                Console.WriteLine("IsError     :True")
            Else
                Console.WriteLine("IsError     :False")
            End If
            Console.WriteLine("ErrorType   :" & ai_errInfomation.ErrorType)
            Console.WriteLine("ErrorMessage:" & ai_errInfomation.ErrorMsg)
        End Sub
    End Class
End Namespace

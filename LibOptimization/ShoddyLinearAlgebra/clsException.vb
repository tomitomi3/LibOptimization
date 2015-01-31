Namespace MathUtil
    Public Class clsException : Inherits Exception
        Public Enum Series
            UnknownError
            NotImpementaion
            NotSquare
            DifferRowNumberAndCollumnNumber
            NotComputable
            DifferElementNumber
        End Enum

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub New()
            'nop
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_series"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_series As Series)
            MyBase.New(String.Format("ErrorCode:{0}", ai_series))
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_series"></param>
        ''' <param name="ai_msg"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_series As Series, ByVal ai_msg As String)
            MyBase.New(String.Format("ErrorCode:{0}\nErrorMsg:", ai_series, ai_msg))
        End Sub
    End Class

End Namespace

Namespace MathUtil
    ''' <summary>
    ''' Exception class for SimpleLinearAlgebra lib
    ''' </summary>
    Public Class clsException : Inherits Exception
        ''' <summary>
        ''' Error series
        ''' </summary>
        Public Enum Series
            UnknownError
            NotImplementaion
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
        ''' <param name="ai_series">error series</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_series As Series)
            MyBase.New(String.Format("ErrorCode:{0}", ai_series))
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_series">error series</param>
        ''' <param name="ai_msg">message</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_series As Series, ByVal ai_msg As String)
            MyBase.New(String.Format("ErrorCode:{0}\nErrorMsg:{1}", ai_series, ai_msg))
        End Sub
    End Class
End Namespace

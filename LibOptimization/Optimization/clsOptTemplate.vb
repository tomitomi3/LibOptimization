Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' optimize template
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsOptTemplate : Inherits absOptimization
#Region "Member"
        'Parameters
        Private EPS As Double = 0.000000001
        Private MAX_ITERATION As Integer = 5000

        'This Parameter to use when generate a variable
        Private INIT_PARAM_RANGE As Double = 5.12

        Private m_points As clsPoint = Nothing

        'ErrorManage
        Private m_error As New Util.clsError()
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
            Me.m_points = New clsPoint(ai_func)
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                '
                ' Init
                '
            Catch ex As Exception
                Me.m_error.SetError(True, Util.clsError.ErrorType.ERR_INIT)
            Finally
                System.GC.Collect()
            End Try
        End Sub

        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'Do Iterate
            ai_iteration = If(ai_iteration = 0, Me.MAX_ITERATION - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Iteration count
                If MAX_ITERATION <= m_iteration Then
                    Me.m_error.SetError(True, Util.clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                m_iteration += 1

                '
                ' implement optimization algorighm
                '
            Next

            Return False
        End Function

        ''' <summary>
        ''' Recent Error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsRecentError() As Boolean
            Return Me.m_error.IsError()
        End Function

        ''' <summary>
        ''' Result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As Optimization.clsPoint
            Get
                Return Me.m_points
            End Get
        End Property

        ''' <summary>
        ''' for Debug
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property ResultForDebug As List(Of Optimization.clsPoint)
            Get
                Return Nothing
            End Get
        End Property
#End Region

#Region "Private"

#End Region
    End Class
End Namespace

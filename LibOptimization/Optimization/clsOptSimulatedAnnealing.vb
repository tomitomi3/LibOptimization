Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Simulated Annealing
    ''' </summary>
    ''' <remarks>
    ''' 
    ''' Refference:
    ''' A. CORANA, M. MARCHESI, C. MARTINI, and S. RIDELLA, "Minimizing multimodal functions of continuous variables with the “simulated annealing” algorithm: Corrigenda for this article is available", ACM Transactions on Mathematical Software (TOMS) TOMS Homepage archive, Volume 13 Issue 3, Sept. 1987, pp262-280
    ''' </remarks>
    Public Class clsOptSimulatedAnnealing : Inherits absOptimization
#Region "Member"
        'Parameters
        Private EPS As Double = 0.000000001
        Private MAX_ITERATION As Integer = 100000

        'ErrorManage
        Private m_error As New Util.clsError()

        'This Parameter to use when generate a variable
        Private INIT_PARAM_RANGE As Double = 5.12

        Private m_point As clsPoint = Nothing

        'Simulated Annealing Parameters
        Private COOLING_RATIO As Double = 0.995 '0.5%
        Private NEIGHBOR_RANGE As Double = 0.01
        Private TEMPERTURE As Double = 5000.0
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
            Me.m_point = New clsPoint(ai_func)
        End Sub
#End Region

#Region "Property(Parameter setting)"
        ''' <summary>
        ''' epsilon
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_EPS As Double
            Set(value As Double)
                Me.EPS = value
            End Set
        End Property

        ''' <summary>
        ''' Max iteration count
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_MAX_ITERATION As Integer
            Set(value As Integer)
                Me.MAX_ITERATION = value
            End Set
        End Property

        ''' <summary>
        ''' Range of initial value
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_InitRange As Double
            Set(value As Double)
                Me.INIT_PARAM_RANGE = value
            End Set
        End Property
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                Me.m_point.Clear()
                For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                    Me.m_point.Add(Math.Abs(2.0 * INIT_PARAM_RANGE) * m_rand.NextDouble() - INIT_PARAM_RANGE)
                Next
                Me.m_point.ReEvaluate()

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
                'iteration count
                If MAX_ITERATION <= Me.m_iteration Then
                    Me.m_error.SetError(True, Util.clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                Me.m_iteration += 1

                'neighbor
                Dim temp As New clsPoint(Me.m_point)
                For i As Integer = 0 To temp.Count - 1
                    temp(i) += Math.Abs(2.0 * NEIGHBOR_RANGE) * m_rand.NextDouble() - NEIGHBOR_RANGE
                Next
                temp.ReEvaluate()

                'transition
                Dim evalNow As Double = Me.m_point.Eval
                Dim evalNew As Double = temp.Eval
                Dim r1 As Double = 0.0
                Dim r2 = Me.m_rand.NextDouble()
                If evalNew < evalNow Then
                    r1 = 1.0
                Else
                    r1 = Math.Exp((evalNow - evalNew) / TEMPERTURE)
                End If
                If r1 >= r2 Then
                    Me.m_point = temp
                End If

                'cooling
                Me.TEMPERTURE *= Me.COOLING_RATIO
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
                Return Me.m_point
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
                Throw New NotImplementedException()
            End Get
        End Property
#End Region

#Region "Private"
#End Region
    End Class
End Namespace

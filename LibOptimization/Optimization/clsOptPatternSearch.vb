Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Hooke-Jeeves Pattern Search Method
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    ''' 
    ''' Reffrence:
    ''' Hooke, R. and Jeeves, T.A., ""Direct search" solution of numerical and statistical problems", Journal of the Association for Computing Machinery (ACM) 8 (2), pp212–229.
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptPatternSearch : Inherits absOptimization
#Region "Member"
        Private ReadOnly MAX_ITERATION As Integer = 20000
        Private ReadOnly EPS As Double = 0.00000001
        Private ReadOnly DEFAULT_STEPLENGTH As Double = 0.6
        Private ReadOnly COEFF_Shrink As Double = 2.0

        'This Parameter to use when generate a variable
        Private ReadOnly INIT_PARAM_RANGE As Double = 5

        Private m_stepLength As Double = 0.6
        Private m_base As clsPoint = Nothing

        'ErrorManage
        Private m_error As New clsError
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Optimize Function</param>
        ''' <param name="ai_randomRange">Optional:random range(Default 5 => -5 to 5)</param>
        ''' <param name="ai_maxIteration">Optional:Iteration(default 20000)</param>
        ''' <param name="ai_eps">Optional:Eps(default:1e-8)</param>
        ''' <param name="ai_steplength">Optinal:step length(default 0.6)</param>
        ''' <param name="ai_coeffShrink">Optional:Shrink coeffcient(default:2.0)</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction, _
                       Optional ByVal ai_randomRange As Double = 5, _
                       Optional ByVal ai_maxIteration As Integer = 20000, _
                       Optional ByVal ai_eps As Double = 0.00000001, _
                       Optional ByVal ai_steplength As Double = 0.6, _
                       Optional ByVal ai_coeffShrink As Double = 2.0
                       )
            Me.m_func = ai_func

            Me.INIT_PARAM_RANGE = ai_randomRange
            Me.MAX_ITERATION = ai_maxIteration
            Me.EPS = ai_eps
            Me.DEFAULT_STEPLENGTH = ai_steplength
            Me.m_stepLength = ai_steplength
            Me.COEFF_Shrink = ai_coeffShrink
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                'Init meber varibles
                Me.InitInner()

                'Initialize
                Me.m_base = New clsPoint(MyBase.m_func)
                For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                    Me.m_base(i) = Math.Abs(2.0 * INIT_PARAM_RANGE) * m_rand.NextDouble() - INIT_PARAM_RANGE
                Next
                Me.m_base.ReEvaluate()

            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
            Finally
                System.GC.Collect()
            End Try
        End Sub

        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks></remarks>
        Public Overloads Sub Init(ByVal ai_initPoint() As Double)
            Try
                'Init meber varibles
                Me.InitInner()

                If ai_initPoint.Length <> Me.m_func.NumberOfVariable Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT, "")
                    Return
                End If

                'Initialize
                Me.m_base = New clsPoint(MyBase.m_func, ai_initPoint)
            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT, "")
            Finally
                System.GC.Collect()
            End Try
        End Sub

        ''' <summary>
        ''' Init parameter
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub InitInner()
            Me.m_error.Clear()
            Me.m_iteration = 0

            Me.m_stepLength = Me.DEFAULT_STEPLENGTH
            Me.m_base = Nothing
        End Sub

        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
        ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'Do Iterate
            ai_iteration = If(ai_iteration = 0, Me.MAX_ITERATION - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Counting Iteration
                If MAX_ITERATION <= m_iteration Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_OPT_MAXITERATION, "")
                    Return True
                End If
                m_iteration += 1

                'MakeExploratoryMoves
                Dim exp As clsPoint = Me.MakeExploratoryMoves(Me.m_base, Me.m_stepLength)

                If exp.Eval < Me.m_base.Eval Then
                    'Replace basepoint
                    Dim previousBasePoint As clsPoint = Me.m_base
                    Me.m_base = exp

                    'MakePatternMove and MakeExploratoryMoves
                    Dim temp As clsPoint = Me.MakePatternMove(previousBasePoint, Me.m_base)
                    Dim expUsingPatternMove = Me.MakeExploratoryMoves(temp, Me.m_stepLength)
                    If expUsingPatternMove.Eval < Me.m_base.Eval Then
                        Me.m_base = expUsingPatternMove
                    End If
                Else
                    'Check conversion
                    If Me.m_stepLength < EPS Then
                        Return True
                    End If

                    'Shrink Step
                    Me.m_stepLength /= Me.COEFF_Shrink
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Exploratory Move
        ''' </summary>
        ''' <param name="ai_base">Base point</param>
        ''' <param name="ai_stepLength">Step</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MakeExploratoryMoves(ByVal ai_base As clsPoint, ByVal ai_stepLength As Double) As clsPoint
            Dim explorePoint As New List(Of clsPoint)
            For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                Dim tempPlus As New clsPoint(ai_base)
                tempPlus(i) += ai_stepLength
                tempPlus.ReEvaluate()
                explorePoint.Add(tempPlus)

                Dim tempMinus As New clsPoint(ai_base)
                tempMinus(i) -= ai_stepLength
                tempMinus.ReEvaluate()
                explorePoint.Add(tempMinus)
            Next
            explorePoint.Sort()

            If explorePoint(0).Eval < ai_base.Eval Then
                Return explorePoint(0)
            Else
                Return New clsPoint(ai_base)
            End If
        End Function

        ''' <summary>
        ''' Pattern Move
        ''' </summary>
        ''' <param name="ai_previousBasePoint"></param>
        ''' <param name="ai_base"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function MakePatternMove(ByVal ai_previousBasePoint As clsPoint, ByVal ai_base As clsPoint) As clsPoint
            Dim ret As New clsPoint(ai_base)
            For i As Integer = 0 To ai_base.Count - 1
                ret(i) = 2.0 * ai_base(i) - ai_previousBasePoint(i)
            Next
            ret.ReEvaluate()

            Return ret
        End Function

        ''' <summary>
        ''' Result
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result() As clsPoint
            Get
                Return Me.m_base
            End Get
        End Property

        ''' <summary>
        ''' Base with length
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' for Debug
        ''' </remarks>
        Public Overrides ReadOnly Property ResultForDebug As List(Of clsPoint)
            Get
                Dim ret = New List(Of clsPoint)
                ret.Add(New clsPoint(Me.m_base))
                For i As Integer = 0 To Me.m_base.Count - 1
                    Dim tempPlus As New clsPoint(Me.m_base)
                    tempPlus(i) += Me.m_stepLength
                    ret.Add(tempPlus)
                    Dim tempMinus As New clsPoint(Me.m_base)
                    tempMinus(i) -= Me.m_stepLength
                    ret.Add(tempMinus)
                Next
                Return ret
            End Get
        End Property

        ''' <summary>
        ''' Get recent error infomation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLastErrorInfomation() As clsError.clsErrorInfomation
            Return Me.m_error.GetLastErrorInfomation()
        End Function

        ''' <summary>
        ''' Get recent error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsRecentError() As Boolean
            Return Me.m_error.IsError()
        End Function
#End Region
    End Class

End Namespace

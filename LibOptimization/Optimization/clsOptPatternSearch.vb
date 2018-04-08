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
        ''' <summary>Max iteration count(Default:20,000)</summary>
        Public Overrides Property Iteration As Integer = 20000

        ''' <summary>Epsilon(Default:0.000001) for Criterion</summary>
        Public Property EPS As Double = 0.000001

        '-------------------------------------------------------------------
        'Coefficient of pattern search
        '-------------------------------------------------------------------
        ''' <summary>step length(Default:0.6)</summary>
        Private ReadOnly StepLength As Double = 0.6

        ''' <summary>shrink parameter(Default:2.0)</summary>
        Private ReadOnly Shrink As Double = 2.0

        ''' <summary>current step length</summary>
        Private m_stepLength As Double = 0.6

        ''' <summary>current base</summary>
        Private m_base As clsPoint = Nothing
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Optimize Function</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
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
                Me.m_error.Clear()
                Me.m_iteration = 0
                Me.m_stepLength = Me.StepLength
                Me.m_base = Nothing

                'init position
                If InitialPosition IsNot Nothing AndAlso InitialPosition.Length = m_func.NumberOfVariable Then
                    Me.m_base = New clsPoint(Me.m_func, InitialPosition)
                Else
                    Dim array = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    Me.m_base = New clsPoint(Me.m_func, array)
                End If
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
                Me.m_error.Clear()
                Me.m_iteration = 0
                Me.m_stepLength = Me.StepLength
                Me.m_base = Nothing

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
            ai_iteration = If((Iteration - m_iteration) > ai_iteration, Iteration - m_iteration - 1, If((Iteration - m_iteration) > ai_iteration, ai_iteration - 1, Iteration - m_iteration - 1))
            For iterate As Integer = 0 To ai_iteration
                'Counting Iteration
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
                    Me.m_stepLength /= Me.Shrink
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
        Public Overrides ReadOnly Property Result As clsPoint
            Get
                Return Me.m_base.Copy()
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
        Public Overrides ReadOnly Property Results As List(Of clsPoint)
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

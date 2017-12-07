Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Evolution Strategy (1+1)-ES without Criterion
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    ''' 
    ''' Refference:
    ''' [1]進化戦略 https://ja.wikipedia.org/wiki/%E9%80%B2%E5%8C%96%E6%88%A6%E7%95%A5
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptES : Inherits absOptimization
#Region "Member"
        '----------------------------------------------------------------
        'Common parameters
        '----------------------------------------------------------------
        ''' <summary>epsilon(Default:1e-8) for Criterion</summary>
        Public Property EPS As Double = 0.000000001

        ''' <summary>Use criterion</summary>
        Public Property IsUseCriterion As Boolean = True

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        ''' <summary>
        ''' Max iteration count
        ''' </summary>
        Public Property Iteration As Integer = 50000

        ''' <summary>Upper bound(limit solution space)</summary>
        Public Property UpperBounds As Double() = Nothing

        ''' <summary>Lower bound(limit solution space)</summary>
        Public Property LowerBounds As Double() = Nothing

        '----------------------------------------------------------------
        'parameters
        '----------------------------------------------------------------
        ''' <summary>population</summary>
        Private _populations As New List(Of clsPoint)

        ''' <summary>Population Size</summary>
        Public Property PopulationSize As Integer = 1

        ''' <summary>update ratio C(Schwefel 0.85)</summary>
        Public Property C As Double = 0.85

        ''' <summary>recent result for Criterion</summary>
        Private _recentResult As Double = 0.0

        ''' <summary>variance</summary>
        Private _variance As Double = 0.0

        ''' <summary>recent Mutate success history for 1/5 rule</summary>
        Private _successMutate As New List(Of Integer)
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Objective Function</param>
        ''' <remarks>
        ''' </remarks>
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
                'init meber varibles
                Me.m_iteration = 0
                Me._populations.Clear()
                Me.m_error.Clear()

                'bound check
                If UpperBounds IsNot Nothing AndAlso LowerBounds IsNot Nothing Then
                    If UpperBounds.Length <> Me.m_func.NumberOfVariable Then
                        Throw New Exception("UpperBounds.Length is different")
                    End If
                    If LowerBounds.Length <> Me.m_func.NumberOfVariable Then
                        Throw New Exception("LowerBounds.Length is different")
                    End If
                End If

                'generate population
                For i As Integer = 0 To Me.PopulationSize - 1
                    'initialize
                    Dim temp As New List(Of Double)
                    For j As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        Dim value As Double = clsUtil.GenRandomRange(Me.m_rand, -Me.InitialValueRange, Me.InitialValueRange)
                        temp.Add(value)
                    Next

                    'bound check
                    Dim tempPoint = New clsPoint(MyBase.m_func, temp)
                    If UpperBounds IsNot Nothing AndAlso LowerBounds IsNot Nothing Then
                        clsUtil.LimitSolutionSpace(tempPoint, Me.LowerBounds, Me.UpperBounds)
                    End If

                    'save point
                    Me._populations.Add(tempPoint)
                Next

                'add initial point
                clsUtil.SetInitialPoint(Me._populations, InitialPosition)

                'Sort Evaluate
                Me._populations.Sort()

                'Detect HigherNPercentIndex
                Me.HigherNPercentIndex = CInt(Me._populations.Count * Me.HigherNPercent)
                If Me.HigherNPercentIndex = Me._populations.Count OrElse Me.HigherNPercentIndex >= Me._populations.Count Then
                    Me.HigherNPercentIndex = Me._populations.Count - 1
                End If

                'specify ES
                _successMutate.Clear()
                For i As Integer = 0 To (Me.m_func.NumberOfVariable * 10) - 1
                    _successMutate.Add(0)
                Next
                'init variance
                _variance = clsUtil.GenRandomRange(m_rand, 0.1, 5)

            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
            End Try
        End Sub

        ''' <summary>
        ''' Do Iteration
        ''' </summary>
        ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
        ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'for 1/5 rule
            Dim n = Me.m_func.NumberOfVariable * 10.0

            'Do Iterate
            ai_iteration = If(ai_iteration = 0, Me.Iteration - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Sort Evaluate
                Me._populations.Sort()

                'check criterion
                'If Me.IsUseCriterion = True AndAlso m_iteration <> 0 Then
                '    'higher N percentage particles are finished at the time of same evaluate value.
                '    If clsUtil.IsCriterion(Me.EPS, Me._populations(0).Eval, _recentResult) Then
                '        Return True
                '    End If
                'End If

                'Counting generation
                If Me.Iteration <= Me.m_iteration Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                m_iteration += 1

                '-------------------------------------------------
                'ES
                '-------------------------------------------------
                'mutate
                Dim child = _populations(0).Copy()
                For i As Integer = 0 To _populations(0).Count - 1
                    child(i) = _populations(0)(i) + clsUtil.NormRand(m_rand, 0, _variance)
                Next
                child.ReEvaluate()

                'check best
                Dim index = m_iteration Mod Me.m_func.NumberOfVariable * 10
                If child.Eval < _populations(0).Eval Then
                    _recentResult = _populations(0).Eval
                    _populations(0) = child
                    _successMutate(index) = 1
                Else
                    _recentResult = child.Eval
                    _successMutate(index) = 0
                End If

                '1/5 rule
                Dim successCount = _successMutate.Sum()
                Dim updateSuccessRatio = successCount / n
                If updateSuccessRatio < 0.2 Then
                    _variance = _variance * C
                Else
                    _variance = _variance / C
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result() As clsPoint
            Get
                Return clsUtil.GetBestPoint(_populations, True)
            End Get
        End Property

        ''' <summary>
        ''' Get recent error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsRecentError() As Boolean
            Return Me.m_error.IsError()
        End Function

        ''' <summary>
        ''' All Result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' for Debug
        ''' </remarks>
        Public Overrides ReadOnly Property Results As List(Of clsPoint)
            Get
                Return Me._populations
            End Get
        End Property
#End Region

#Region "Private"

#End Region
    End Class
End Namespace

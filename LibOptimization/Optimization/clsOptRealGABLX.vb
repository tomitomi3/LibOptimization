Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Real-coded Genetic Algorithm
    ''' BLX-Alpha + JGG
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Alternation of generation algorithm is JGG.
    ''' 
    ''' Refference:
    ''' 北野 宏明 (編集), 遺伝的アルゴリズム 4, 産業図書出版株式会社, 2000年 初版, p261
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptRealGABLX : Inherits absOptimization
#Region "Member"
        'Common settings
        Private EPS As Double = 0.000000001
        Private IsUseCriterion As Boolean = True
        Private HigherNPercent As Double = 0.7 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())
        Private MAX_ITERATION As Integer = 10000 'generation
        Private INIT_PARAM_RANGE As Double = 5 'parameter range

        'GA Parameters
        Private POPULATION_SIZE As Integer = 100
        Private CHILDS_SIZE As Integer = 100
        Private Alpha As Double = 0.5
        Private m_parents As New List(Of clsPoint)

        'ErrorManage
        Private m_error As New clsError
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Optimize Function</param>
        ''' <remarks>
        ''' "n" is function dimension.
        ''' </remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func

            Me.POPULATION_SIZE = Me.m_func.NumberOfVariable * 50
            Me.CHILDS_SIZE = Me.m_func.NumberOfVariable * 20
        End Sub
#End Region

#Region "Property(Parameter setting)"
        ''' <summary>
        ''' epsilon(Default:1e-8)
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_EPS As Double
            Set(value As Double)
                Me.EPS = value
            End Set
        End Property

        ''' <summary>
        ''' Use criterion
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_IsUseCriterion As Boolean
            Set(value As Boolean)
                Me.IsUseCriterion = value
            End Set
        End Property

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when PARAM_IsUseCriterion is true.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_CriterionPersent As Double
            Set(value As Double)
                Me.HigherNPercent = value
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

        ''' <summary>
        ''' Population Size
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property PARAM_PopulationSize As Integer
            Set(value As Integer)
                Me.POPULATION_SIZE = value
            End Set
        End Property

        ''' <summary>
        ''' Child size
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property PARAM_ChildSize As Integer
            Set(value As Integer)
                Me.CHILDS_SIZE = value
            End Set
        End Property

        ''' <summary>
        ''' Alpha is expantion ratio(Default:0.5)
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property PARAM_Alpha As Integer
            Set(value As Integer)
                Me.Alpha = value
            End Set
        End Property
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
                Me.m_parents.Clear()

                'Set initialize value
                For i As Integer = 0 To Me.POPULATION_SIZE - 1
                    Dim temp As New List(Of Double)
                    For j As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        temp.Add(Math.Abs(2.0 * INIT_PARAM_RANGE) * m_rand.NextDouble() - INIT_PARAM_RANGE)
                    Next
                    Me.m_parents.Add(New clsPoint(MyBase.m_func, temp))
                Next

                'Sort Evaluate
                Me.m_parents.Sort()

                'Detect HigherNPercentIndex
                Me.HigherNPercentIndex = CInt(Me.m_parents.Count * Me.HigherNPercent)
                If Me.HigherNPercentIndex = Me.m_parents.Count OrElse Me.HigherNPercentIndex >= Me.m_parents.Count Then
                    Me.HigherNPercentIndex = Me.m_parents.Count - 1
                End If

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

            'Do Iterate
            ai_iteration = If(ai_iteration = 0, Me.MAX_ITERATION - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Sort Evaluate
                Me.m_parents.Sort()

                'Check criterion
                If Me.IsUseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If clsUtil.IsCriterion(Me.EPS, Me.m_parents(0).Eval, Me.m_parents(Me.HigherNPercentIndex).Eval) Then
                        Return True
                    End If
                End If

                'Counting generation
                If Me.MAX_ITERATION <= Me.m_iteration Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                m_iteration += 1

                'BLX-alpha cross-over
                'Pick parent
                Dim p1Index As Integer = CInt(Me.m_parents.Count * Me.m_rand.NextDouble())
                Dim p2Index As Integer = CInt(Me.m_parents.Count * Me.m_rand.NextDouble())
                If p1Index >= Me.m_parents.Count Then
                    p1Index -= 1
                End If
                If p2Index >= Me.m_parents.Count Then
                    p2Index -= 1
                End If
                Dim p1 = Me.m_parents(p1Index)
                Dim p2 = Me.m_parents(p2Index)

                'cross over
                Dim child As New List(Of clsPoint)
                For numChild As Integer = 0 To Me.CHILDS_SIZE - 1
                    child.Add(New clsPoint(Me.m_func))
                    For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        Dim range As Double = Math.Abs(p1(i) - p2(i))
                        Dim min As Double = 0
                        Dim max As Double = 0
                        If p1(i) > p2(i) Then
                            min = p2(i)
                            max = p1(i)
                        Else
                            min = p1(i)
                            max = p2(i)
                        End If
                        child(numChild)(i) = clsUtil.GenRandomRange(Me.m_rand, min - Me.Alpha * range, max + Me.Alpha * range)
                    Next
                    child(numChild).ReEvaluate()
                Next

                'replace(JGG)
                child.Sort()
                Me.m_parents(p1Index) = child(0)
                Me.m_parents(p2Index) = child(1)
            Next

            Return False
        End Function

        ''' <summary>
        ''' using Elite Strategy
        ''' </summary>
        ''' <param name="ai_density">density</param>
        ''' <remarks>
        ''' Elite strategy
        ''' </remarks>
        Public Sub UseEliteStrategy(ByVal ai_density As Double)
            If ai_density > 1 Then
                Return
            End If
            If ai_density < 0 Then
                Return
            End If
            Dim index As Integer = CInt(Me.m_parents.Count * ai_density)
            If index = 0 Then
                Return
            End If

            'replace new point
            For i As Integer = index To Me.POPULATION_SIZE - 1
                Dim temp As New List(Of Double)
                For j As Integer = 0 To Me.m_func.NumberOfVariable - 1
                    temp.Add(Math.Abs(2.0 * INIT_PARAM_RANGE) * m_rand.NextDouble() - INIT_PARAM_RANGE)
                Next
                Me.m_parents(i) = New clsPoint(MyBase.m_func, temp)
            Next

            'iteration count reset
            Me.m_iteration = 0
        End Sub

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result() As clsPoint
            Get
                Return Me.m_parents(0)
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
        Public Overrides ReadOnly Property ResultForDebug As List(Of clsPoint)
            Get
                Return Me.m_parents
            End Get
        End Property
#End Region
    End Class
End Namespace
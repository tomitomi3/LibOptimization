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
    ''' 北野宏明 (編集), "遺伝的アルゴリズム 4", 産業図書出版株式会社, 2000年初版
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptRealGABLX : Inherits absOptimization
#Region "Member"
        ''' <summary>Max iteration count(Default:20,000)</summary>
        Public Overrides Property Iteration As Integer = 20000

        ''' <summary>Epsilon(Default:1e-8) for Criterion</summary>
        Public Property EPS As Double = 0.00000001

        ''' <summary>Use criterion</summary>
        Public Property IsUseCriterion As Boolean = True

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        '-------------------------------------------------------------------
        'Coefficient of GA
        '-------------------------------------------------------------------
        ''' <summary>Population Size(Default:dimension*50)</summary>
        Public Property PopulationSize As Integer = 100

        ''' <summary>Children Size(Default:dimension*20)</summary>
        Public Property ChildrenSize As Integer = 100

        ''' <summary>Alpha is expantion ratio(Default:0.5)</summary>
        Public Property Alpha As Double = 0.5

        ''' <summary>population</summary>
        Private m_parents As New List(Of clsPoint) 'Parent
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

            Me.PopulationSize = Me.m_func.NumberOfVariable * 50
            Me.ChildrenSize = Me.m_func.NumberOfVariable * 20
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
                Me.m_parents.Clear()

                'initial position
                For i As Integer = 0 To Me.PopulationSize - 1
                    Dim array = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    Me.m_parents.Add(New clsPoint(Me.m_func, array))
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
            ai_iteration = If(ai_iteration = 0, Me.Iteration - 1, ai_iteration - 1)
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
                If Me.Iteration <= Me.m_iteration Then
                    Return True
                End If
                m_iteration += 1

                'BLX-alpha cross-over
                'Pick parent
                Dim randIndex As List(Of Integer) = clsUtil.RandomPermutaion(Me.m_parents.Count)
                Dim p1Index As Integer = randIndex(0)
                Dim p2Index As Integer = randIndex(1)
                Dim p1 = Me.m_parents(p1Index)
                Dim p2 = Me.m_parents(p2Index)

                'cross over
                Dim children As New List(Of clsPoint)
                For numChild As Integer = 0 To Me.ChildrenSize - 1
                    children.Add(New clsPoint(Me.m_func))
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
                        children(numChild)(i) = clsUtil.GenRandomRange(min - Me.Alpha * range, max + Me.Alpha * range)
                    Next
                    children(numChild).ReEvaluate()
                Next

                'replace(JGG)
                children.Sort()
                Me.m_parents(p1Index) = children(0)
                Me.m_parents(p2Index) = children(1)
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
        Public Overrides ReadOnly Property Results As List(Of clsPoint)
            Get
                Return Me.m_parents
            End Get
        End Property
#End Region
    End Class
End Namespace
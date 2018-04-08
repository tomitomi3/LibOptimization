Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Real-coded Genetic Algorithm
    ''' REX(Real-coded Ensemble Crossover) + JGG
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Cross over algorithm is REX(Real-coded Ensemble Cross over).
    '''  -Alternation of generation algorithm is JGG.
    ''' 
    ''' Refference:
    ''' 小林重信, "実数値GAのフロンティア"，人工知能学会誌 Vol. 24, No. 1, pp.147-162 (2009)
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptRealGAREX : Inherits absOptimization
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
        ''' <summary>population</summary>
        Public Property m_parents As New List(Of clsPoint) 'Parent

        ''' <summary>Population Size(Default:50*Log(n)+10)</summary>
        Public Property PopulationSize As Integer = 1000

        ''' <summary>Parent size for cross over(Default:n+1)</summary>
        Public Property ParentSize As Integer = 100 'REX(phi, n+k) -> n+1<n+k<npop

        ''' <summary>Children Size(Default30*Log(n)+10)</summary>
        Public Property ChildrenSize As Integer = 100

        ''' <summary>REX randomo mode(Default:UNIFORM)</summary>
        Public Property RandomMode As RexRandomMode = RexRandomMode.UNIFORM

        Public Enum RexRandomMode
            UNIFORM
            NORMAL_DIST
        End Enum
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Target Function</param>
        ''' <remarks>
        ''' </remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
            Me.ParentSize = Me.m_func.NumberOfVariable + 1 'n+k

            'Me.PopulationSize = Me.m_func.NumberOfVariable * 8
            'Me.ChildrenSize = Me.m_func.NumberOfVariable * 6 '6-8 * n
            Me.PopulationSize = CInt(50 * Math.Log(Me.m_func.NumberOfVariable) + 10)
            Me.ChildrenSize = CInt(30 * Math.Log(Me.m_func.NumberOfVariable) + 10)
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
            ai_iteration = If((Iteration - m_iteration) > ai_iteration, Iteration - m_iteration - 1, If((Iteration - m_iteration) > ai_iteration, ai_iteration - 1, Iteration - m_iteration - 1))
            For iterate As Integer = 0 To ai_iteration
                'Counting Iteration
                m_iteration += 1

                'Sort Evaluate
                Me.m_parents.Sort()

                'check criterion
                If Me.IsUseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If clsUtil.IsCriterion(Me.EPS, Me.m_parents(0).Eval, Me.m_parents(Me.HigherNPercentIndex).Eval) Then
                        Return True
                    End If
                End If

                'REX with JGG
                Dim parents As List(Of KeyValuePair(Of Integer, clsPoint)) = Me.SelectParent(Me.m_parents, Me.ParentSize)

                'Crossover
                Dim children As List(Of clsPoint) = Me.CrossOverREX(Me.RandomMode, Me.ChildrenSize, parents)

                'Replace
                Dim index As Integer = 0
                For Each p As KeyValuePair(Of Integer, clsPoint) In parents
                    Me.m_parents(p.Key) = children(index)
                    index += 1
                Next
            Next

            Return False
        End Function

        ''' <summary>
        ''' Select Parent
        ''' </summary>
        ''' <param name="ai_population"></param>
        ''' <param name="ai_parentSize"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function SelectParent(ByVal ai_population As List(Of clsPoint), ByVal ai_parentSize As Integer) As List(Of KeyValuePair(Of Integer, clsPoint))
            Dim ret As New List(Of KeyValuePair(Of Integer, clsPoint))

            'Index
            Dim randIndex As List(Of Integer) = clsUtil.RandomPermutaion(ai_population.Count)

            'PickParents
            For i As Integer = 0 To ai_parentSize - 1
                ret.Add(New KeyValuePair(Of Integer, clsPoint)(randIndex(i), ai_population(randIndex(i))))
            Next

            Return ret
        End Function

        ''' <summary>
        ''' REX(Real-coded Ensemble Crossover)
        ''' </summary>
        ''' <param name="ai_randomMode"></param>
        ''' <param name="ai_childNum">ChildNum</param>
        ''' <param name="ai_parents"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' REX(U, n+k) -> U is UniformRandom
        ''' REX(N, n+k) -> N is NormalDistribution
        ''' "n+k" is parents size.
        ''' </remarks>
        Private Function CrossOverREX(ByVal ai_randomMode As RexRandomMode, _
                             ByVal ai_childNum As Integer, _
                             ByVal ai_parents As List(Of KeyValuePair(Of Integer, clsPoint))) As List(Of clsPoint)
            'Calc Centroid
            Dim xg As New clsEasyVector(MyBase.m_func.NumberOfVariable)
            For Each p As KeyValuePair(Of Integer, clsPoint) In ai_parents
                xg += p.Value
            Next
            xg /= ai_parents.Count 'sum(xi)/(n+k)

            'Range
            Dim range As Double = (Me.InitialValueRangeUpper - Me.InitialValueRangeLower) / 2.0

            'cross over
            Dim retChilds As New List(Of clsPoint)
            Dim uniformRandParam As Double = Math.Sqrt(3 / ai_parents.Count)
            Dim normalDistParam As Double = 1 / ai_parents.Count '???
            For i As Integer = 0 To ai_childNum
                'cross over
                Dim childV As New clsEasyVector(MyBase.m_func.NumberOfVariable)
                'sum( rand * (xi-xg) )
                For Each xi As KeyValuePair(Of Integer, clsPoint) In ai_parents
                    'rand parameter
                    Dim randVal As Double = 0.0
                    If ai_randomMode = RexRandomMode.NORMAL_DIST Then
                        randVal = clsUtil.NormRand(0, normalDistParam)
                    Else
                        randVal = Math.Abs(2.0 * uniformRandParam) * m_rand.NextDouble() - range
                    End If
                    'rand * (xi-xg)
                    childV += randVal * (xi.Value - xg)
                Next
                'xg + sum( rand * (xi-xg) )
                childV += xg

                'convert clsPoint
                Dim child As New clsPoint(MyBase.m_func, childV)
                child.ReEvaluate()
                retChilds.Add(child)
            Next
            retChilds.Sort()

            Return retChilds
        End Function

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As clsPoint
            Get
                Return clsUtil.GetBestPoint(m_parents, True)
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
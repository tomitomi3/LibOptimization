Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Real-coded Genetic Algorithm
    ''' SPX + JGG
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
        Private ReadOnly EPS As Double = 0.000000001
        Private ReadOnly IsUseEps As Boolean = True

        'GA Parameters
        Private ReadOnly MAX_ITERATION As Integer = 10000 'generation
        Private ReadOnly POPULATION_SIZE As Integer = 1000
        Private ReadOnly PARENT_SIZE As Integer = 100 'REX(phi, n+k) -> n+1<n+k<npop
        Private ReadOnly CHILDS_SIZE As Integer = 100
        Private ReadOnly REX_RAND As REX_RANDMODE = REX_RANDMODE.UNIFORM

        'This Parameter to use when generate a variable
        Private ReadOnly INIT_PARAM_RANGE As Double = 5

        Public Enum REX_RANDMODE
            UNIFORM
            NORMAL_DIST
        End Enum

        'Parent
        Private m_parents As New List(Of clsPoint)

        'ErrorManage
        Private m_error As New clsError
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Optimize Function</param>
        ''' <param name="ai_randomRange">Optional:random range(Default: 10 => -10 to 10)</param>
        ''' <param name="ai_generation">Optional:Generation(Default: 10000)</param>
        ''' <param name="ai_eps">Optional:Eps(Default:1e-8)</param>
        ''' <param name="ai_isUseEps">Optional:Use criterion(Default: true)</param>
        ''' <param name="ai_populationSize">Optional:Population size(0 is n*8)</param>
        ''' <param name="ai_parentsSize">Optional:Parents size(0 is n+1)</param>
        ''' <param name="ai_REXRandomMode">Optional:REX(phi) Uniform or ND(default: Uniform)</param>
        ''' <param name="ai_childsSize">Optional:Childs size(0 is n*6)</param>
        ''' <remarks>
        ''' "n" is function dimension.
        ''' </remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction, _
                       Optional ByVal ai_randomRange As Double = 10, _
                       Optional ByVal ai_generation As Integer = 10000, _
                       Optional ByVal ai_eps As Double = 0.000000001, _
                       Optional ByVal ai_isUseEps As Boolean = True, _
                       Optional ByVal ai_populationSize As Integer = 0, _
                       Optional ByVal ai_parentsSize As Integer = 0, _
                       Optional ByVal ai_REXRandomMode As REX_RANDMODE = REX_RANDMODE.UNIFORM, _
                       Optional ByVal ai_childsSize As Integer = 0)
            Me.m_func = ai_func

            Me.INIT_PARAM_RANGE = ai_randomRange

            Me.MAX_ITERATION = ai_generation

            Me.EPS = ai_eps
            Me.IsUseEps = ai_isUseEps

            If ai_populationSize = 0 Then
                Me.POPULATION_SIZE = Me.m_func.NumberOfVariable * 8
            Else
                Me.POPULATION_SIZE = ai_populationSize
            End If

            If ai_parentsSize = 0 Then
                Me.PARENT_SIZE = Me.m_func.NumberOfVariable + 1 'n+k
            Else
                Me.PARENT_SIZE = ai_parentsSize
            End If

            Me.REX_RAND = ai_REXRandomMode

            If ai_childsSize = 0 Then
                Me.CHILDS_SIZE = Me.m_func.NumberOfVariable * 6 '6-8 * n
            Else
                Me.CHILDS_SIZE = ai_childsSize
            End If
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

                'Check stop criterion
                If Me.IsUseEps = True Then
                    Dim lastIndex As Integer = CInt(m_parents.Count * 0.7)
                    If lastIndex = m_parents.Count Then
                        lastIndex = m_parents.Count - 1
                    End If
                    If Me.IsCriterion(m_parents(0), m_parents(lastIndex)) < Me.EPS Then
                        Return True
                    End If
                End If

                'Counting generation
                If Me.MAX_ITERATION <= Me.m_iteration Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                m_iteration += 1

                'REX with JGG
                Dim parents As List(Of KeyValuePair(Of Integer, clsPoint)) = Me.SelectParent(Me.m_parents, Me.PARENT_SIZE)

                'Crossover
                Dim childs As List(Of clsPoint) = Me.CrossOverREX(Me.REX_RAND, Me.CHILDS_SIZE, parents)

                'Replace
                Dim index As Integer = 0
                For Each p As KeyValuePair(Of Integer, clsPoint) In parents
                    Me.m_parents(p.Key) = childs(index)
                    index += 1
                Next
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
        Private Function CrossOverREX(ByVal ai_randomMode As REX_RANDMODE, _
                             ByVal ai_childNum As Integer, _
                             ByVal ai_parents As List(Of KeyValuePair(Of Integer, clsPoint))) As List(Of clsPoint)
            'Calc Centroid
            Dim xg As New clsShoddyVector(MyBase.m_func.NumberOfVariable)
            For Each p As KeyValuePair(Of Integer, clsPoint) In ai_parents
                xg += p.Value
            Next
            xg /= ai_parents.Count 'sum(xi)/(n+k)

            'cross over
            Dim retChilds As New List(Of clsPoint)
            Dim uniformRandParam As Double = Math.Sqrt(3 / ai_parents.Count)
            Dim normalDistParam As Double = 1 / ai_parents.Count '???
            For i As Integer = 0 To ai_childNum
                'cross over
                Dim childV As New clsShoddyVector(MyBase.m_func.NumberOfVariable)
                'sum( rand * (xi-xg) )
                For Each xi As KeyValuePair(Of Integer, clsPoint) In ai_parents
                    'rand parameter
                    Dim randVal As Double = 0.0
                    If ai_randomMode = REX_RANDMODE.NORMAL_DIST Then
                        randVal = clsUtil.NormRand(0, normalDistParam)
                    Else
                        randVal = Math.Abs(2.0 * uniformRandParam) * m_rand.NextDouble() - INIT_PARAM_RANGE
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
        ''' for Debug, Experiment
        ''' </remarks>
        Public ReadOnly Property AllResult() As List(Of clsPoint)
            Get
                Return Me.m_parents
            End Get
        End Property
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Check Criterion
        ''' </summary>
        ''' <param name="ai_best"></param>
        ''' <param name="ai_worst"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsCriterion(ByVal ai_best As clsPoint, ByVal ai_worst As clsPoint) As Double
            Dim bestEval As Double = ai_best.Eval
            Dim worstEval As Double = ai_worst.Eval
            Dim temp As Double = 2.0 * Math.Abs(worstEval - bestEval) / (Math.Abs(worstEval) + Math.Abs(bestEval) + 0.0000000001)
            Return temp
        End Function
#End Region
    End Class

End Namespace
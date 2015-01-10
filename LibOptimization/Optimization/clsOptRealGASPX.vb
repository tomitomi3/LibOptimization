''' <summary>
''' Real-coded Genetic Algorithm
''' REX + JGG
''' </summary>
''' <remarks>
''' Features:
'''  -Derivative free optimization algorithm.
'''  -Cross over algorithm is SPX(Simplex Cross over).
'''  -Alternation of generation algorithm is JGG.
''' 
''' Refference:
''' 樋口 隆英, 筒井 茂義, 山村 雅幸, "実数値GAにおけるシンプレクス交叉", 人工知能学会論文誌Vol. 16 (2001) No. 1 pp.147-155
''' 
''' Implment:
''' N.Tomi(tomi.nori+github at gmail.com)
''' </remarks>
Public Class clsOptRealGASPX : Inherits absOptimization
#Region "Member"
    Private ReadOnly EPS As Double = 0.000000001
    Private ReadOnly IsUseEps As Boolean = True

    'GA Parameters
    Private ReadOnly MAX_ITERATION As Integer = 10000 'generation
    Private ReadOnly POPULATION_SIZE As Integer = 1000
    Private ReadOnly CHILDS_SIZE As Integer = 100
    Private ReadOnly REX_RAND As REX_RANDMODE = REX_RANDMODE.UNIFORM

    'This Parameter to use when generate a variable
    Private ReadOnly INIT_PARAM_RANGE As Double = 5
    Private rand As System.Random = New clsRandomXorshift(clsRandomXorshift.GetTimeSeed())

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
                   Optional ByVal ai_REXRandomMode As REX_RANDMODE = REX_RANDMODE.UNIFORM, _
                   Optional ByVal ai_childsSize As Integer = 0)
        Me.m_func = ai_func

        Me.INIT_PARAM_RANGE = ai_randomRange

        Me.MAX_ITERATION = ai_generation

        Me.EPS = ai_eps
        Me.IsUseEps = ai_isUseEps

        If ai_populationSize = 0 Then
            Me.POPULATION_SIZE = Me.m_func.NumberOfVariable * 33
        Else
            Me.POPULATION_SIZE = ai_populationSize
        End If

        Me.REX_RAND = ai_REXRandomMode

        If ai_childsSize = 0 Then
            Me.CHILDS_SIZE = Me.m_func.NumberOfVariable * 10
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
                    temp.Add(Math.Abs(2.0 * INIT_PARAM_RANGE) * rand.NextDouble() - INIT_PARAM_RANGE)
                Next
                Me.m_parents.Add(New clsPoint(MyBase.m_func, temp))
            Next

            'Sort Evaluate
            m_parents.Sort()

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
            If MAX_ITERATION <= m_iteration Then
                Me.m_error.SetError(True, clsError.ErrorType.ERR_OPT_MAXITERATION)
                Return True
            End If
            m_iteration += 1

            'SPX with JGG
            'Parent is n+1
            Dim parents As List(Of KeyValuePair(Of Integer, clsPoint)) = Me.SelectParent(Me.m_parents, Me.m_func.NumberOfVariable + 1)

            'Crossover
            Dim childs As List(Of clsPoint) = Me.CrossOverSPX(Me.CHILDS_SIZE, parents)

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
    ''' Simplex Crossover
    ''' </summary>
    ''' <param name="ai_childSize"></param>
    ''' <param name="ai_parents"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CrossOverSPX(ByVal ai_childSize As Integer, _
                         ByVal ai_parents As List(Of KeyValuePair(Of Integer, clsPoint))) As List(Of clsPoint)
        'Calc Centroid
        Dim xg As New clsShoddyVector(MyBase.m_func.NumberOfVariable)
        For Each p As KeyValuePair(Of Integer, clsPoint) In ai_parents
            xg += p.Value
        Next
        xg /= ai_parents.Count 'sum(xi)/(n+k)

        'SPX
        Dim retChilds As New List(Of clsPoint)
        Dim alpha As Double = Math.Sqrt(MyBase.m_func.NumberOfVariable + 2) 'expantion rate
        For i As Integer = 0 To ai_childSize - 1
            Dim cVector As New List(Of clsShoddyVector)
            Dim pVector As New List(Of clsShoddyVector)
            Dim k As Integer = 0
            For Each xi As KeyValuePair(Of Integer, clsPoint) In ai_parents
                pVector.Add(xg + alpha * (xi.Value - xg))

                If k = 0 Then
                    cVector.Add(New clsShoddyVector(MyBase.m_func.NumberOfVariable)) 'all zero
                Else
                    Dim rk As Double = rand.NextDouble() ^ (1 / k)
                    cVector.Add(rk * (pVector(k - 1) - pVector(k) + cVector(k - 1)))
                End If
                k += 1
            Next
            Dim temp As clsShoddyVector = pVector(pVector.Count - 1) + cVector(cVector.Count - 1)

            retChilds.Add(New clsPoint(MyBase.m_func, temp))
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
            Return Me.BestPoint
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

#Region "Property"
    Private Property BestPoint() As clsPoint
        Get
            Return m_parents(0)
        End Get
        Set(value As clsPoint)
            m_parents(0) = value
        End Set
    End Property

    ''' <summary>
    ''' Random object
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Property Random As System.Random
        Get
            Return Me.rand
        End Get
        Set(ByVal value As System.Random)
            Me.rand = value
        End Set
    End Property
#End Region
End Class

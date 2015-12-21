Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Real-coded Genetic Algorithm
    ''' SPX(Simplex Crossover) + JGG
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Cross over algorithm is SPX(Simplex Crossover).
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
        'Common settings
        Private EPS As Double = 0.000000001
        Private IsUseCriterion As Boolean = True
        Private HigherNPercent As Double = 0.7 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())
        Private MAX_ITERATION As Integer = 10000 'generation

        'GA Parameters
        Private m_parents As New List(Of clsPoint) 'Parent
        Private POPULATION_SIZE As Integer = 1000
        Private CHILDREN_SIZE As Integer = 100
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
            Me.POPULATION_SIZE = Me.m_func.NumberOfVariable * 33
            Me.CHILDREN_SIZE = Me.m_func.NumberOfVariable * 10
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
        ''' Population Size(Default:n*8)
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property PARAM_PopulationSize As Integer
            Set(value As Integer)
                Me.POPULATION_SIZE = value
            End Set
        End Property

        ''' <summary>
        ''' Children size(Default:n*6)
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property PARAM_ChildrenSize As Integer
            Set(value As Integer)
                Me.CHILDREN_SIZE = value
            End Set
        End Property

        ''' <summary>Upper bound(limit solution space)</summary>
        Public Property UpperBounds As Double() = Nothing

        ''' <summary>Lower bound(limit solution space)</summary>
        Public Property LowerBounds As Double() = Nothing
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
                        Dim value As Double = clsUtil.GenRandomRange(Me.m_rand, -Me.InitialValueRange, Me.InitialValueRange)
                        If MyBase.InitialPosition IsNot Nothing AndAlso MyBase.InitialPosition.Length = Me.m_func.NumberOfVariable Then
                            value += Me.InitialPosition(j)
                        End If
                        temp.Add(value)
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

                'check criterion
                If Me.IsUseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If clsUtil.IsCriterion(Me.EPS, Me.m_parents(0).Eval, Me.m_parents(Me.HigherNPercentIndex).Eval) Then
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
                Dim children As List(Of clsPoint) = Me.CrossOverSPX(Me.CHILDREN_SIZE, parents)

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
                    Dim value As Double = clsUtil.GenRandomRange(Me.m_rand, -Me.InitialValueRange, Me.InitialValueRange)
                    If MyBase.InitialPosition IsNot Nothing AndAlso MyBase.InitialPosition.Length = Me.m_func.NumberOfVariable Then
                        value += Me.InitialPosition(j)
                    End If
                    temp.Add(value)
                Next
                Me.m_parents(i) = New clsPoint(MyBase.m_func, temp)
            Next

            'iteration count reset
            Me.m_iteration = 0

            'reset error
            Me.m_error.Clear()
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
        ''' Simplex Crossover
        ''' </summary>
        ''' <param name="ai_childSize"></param>
        ''' <param name="ai_parents"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CrossOverSPX(ByVal ai_childSize As Integer, _
                             ByVal ai_parents As List(Of KeyValuePair(Of Integer, clsPoint))) As List(Of clsPoint)
            'Calc Centroid
            Dim xg As New clsEasyVector(MyBase.m_func.NumberOfVariable)
            For Each p As KeyValuePair(Of Integer, clsPoint) In ai_parents
                xg += p.Value
            Next
            xg /= ai_parents.Count 'sum(xi)/(n+k)

            'SPX
            Dim retChilds As New List(Of clsPoint)
            Dim alpha As Double = Math.Sqrt(MyBase.m_func.NumberOfVariable + 2) 'expantion rate
            For i As Integer = 0 To ai_childSize - 1
                Dim cVector As New List(Of clsEasyVector)
                Dim pVector As New List(Of clsEasyVector)
                Dim k As Integer = 0
                For Each xi As KeyValuePair(Of Integer, clsPoint) In ai_parents
                    pVector.Add(xg + alpha * (xi.Value - xg))

                    If k = 0 Then
                        cVector.Add(New clsEasyVector(MyBase.m_func.NumberOfVariable)) 'all zero
                    Else
                        Dim rk As Double = m_rand.NextDouble() ^ (1 / k)
                        Dim pos = rk * (pVector(k - 1) - pVector(k) + cVector(k - 1))
                        cVector.Add(pos)
                    End If
                    k += 1
                Next
                Dim tempChild = New clsPoint(MyBase.m_func, pVector(pVector.Count - 1) + cVector(cVector.Count - 1))

                'limit solution space
                clsUtil.LimitSolutionSpace(tempChild, Me.LowerBounds, Me.UpperBounds)

                retChilds.Add(tempChild)
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
        ''' for Debug
        ''' </remarks>
        Public Overrides ReadOnly Property Results As List(Of clsPoint)
            Get
                Return Me.m_parents
            End Get
        End Property
#End Region

#Region "Private Methods"
#End Region
    End Class

End Namespace

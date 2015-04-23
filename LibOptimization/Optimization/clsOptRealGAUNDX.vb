Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Real-coded Genetic Algorithm
    ''' UNDX(Unimodal Normal Distribution Crossover)
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
    Public Class clsOptRealGAUNDX : Inherits absOptimization
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
        Private CHILDREN_SIZE As Integer = 100
        Private SD As Double = 0.70710678
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
        ''' Population Size(Default:200)
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
                Me.CHILDREN_SIZE = value
            End Set
        End Property

        ''' <summary>
        ''' SD(Default:0.70710678)
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property PARAM_SD As Integer
            Set(value As Integer)
                Me.SD = value
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

                'check criterion
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

                'select parent
                Dim randIndex As List(Of Integer) = clsUtil.RandomPermutaion(Me.m_parents.Count)
                Dim p1Index As Integer = randIndex(0)
                Dim p2Index As Integer = randIndex(1)
                Dim p3Index As Integer = randIndex(2)
                Dim p1 = Me.m_parents(p1Index)
                Dim p2 = Me.m_parents(p2Index)
                Dim p3 = Me.m_parents(p3Index) 'for d

                'calc d
                'p1(0) = 1
                'p1(1) = 1
                'p2(0) = 4
                'p2(1) = 5
                'p3(0) = 1
                'p3(1) = 4
                'd = 1.8
                Dim diff1 = p2 - p1
                Dim diff2 = p3 - p1
                Dim diff3 = p3 - p2
                Const TINY = 0.000000000001
                Dim a = diff1.NormL2() + TINY
                Dim b = diff2.NormL2() + TINY
                Dim c = diff3.NormL2() + TINY
                Dim s = (a + b + c) / 2.0
                Dim areaTemp = s * (s - a) * (s - b) * (s - c)
                Dim area = Math.Sqrt(areaTemp)
                Dim d2 = 2.0 * area / a 'S=1/2 * h * a -> h = 2.0 * S / a

                'UNDX
                Dim Alpha = 0.5
                Dim Beta = 0.35
                Dim sd1 = Alpha * (p1 - p2).NormL2()
                Dim sd2 = Beta * d2 / Math.Sqrt(Me.m_func.NumberOfVariable)
                Dim e = (p1 - p2) / (p1 - p2).NormL2()
                Dim chidren As New List(Of clsPoint)
                For genChild As Integer = 0 To CInt(Me.CHILDREN_SIZE / 2 - 1)
                    Dim t = New clsShoddyVector(Me.m_func.NumberOfVariable)
                    For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        t(i) = clsUtil.NormRand(0, sd2 ^ 2)
                    Next
                    t = t - (t.InnerProduct(e)) * e

                    For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        t(i) = t(i) + clsUtil.NormRand(0, sd1 ^ 2) * e(i)
                    Next
                    Dim child1 = (p1 + p2) / 2.0 + t
                    Dim child2 = (p1 + p2) / 2.0 - t

                    chidren.Add(New clsPoint(Me.m_func, child1))
                    chidren.Add(New clsPoint(Me.m_func, child2))
                Next

                'replace(JGG)
                chidren.Sort()
                For Each child In chidren
                    Console.WriteLine("{0}", child.Eval)
                Next
                Me.m_parents(p1Index) = chidren(0)
                Me.m_parents(p2Index) = chidren(1)
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

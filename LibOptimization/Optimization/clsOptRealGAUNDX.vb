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
    '''  -Alternation of generation algorithm is MGG.
    '''   JGGだと発散する。
    ''' 
    ''' Refference:
    ''' [1]小野功，佐藤浩，小林重信, "単峰性正規分布交叉UNDXを用いた実数値GAによる関数最適化"，人工知能学会誌，Vol. 14，No. 6，pp. 1146-1155 (1999)
    ''' [2]北野 宏明 (編集), 遺伝的アルゴリズム 4, 産業図書出版株式会社, 2000年 初版, p261
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
        Private MAX_ITERATION As Integer = 50000 'generation
        Private INIT_PARAM_RANGE As Double = 5 'parameter range

        'GA Parameters
        Private POPULATION_SIZE As Integer = 100
        Private CHILDREN_SIZE As Integer = 100
        Private ALPHA As Double = 0.5
        Private BETA As Double = 0.35
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

            Me.POPULATION_SIZE = 200
            Me.CHILDREN_SIZE = Me.m_func.NumberOfVariable * 15
        End Sub
#End Region

#Region "Property(Parameter setting)"
        ''' <summary>
        ''' epsilon(Default:1e-8)
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public Property PARAM_EPS As Double
            Get
                Return Me.EPS
            End Get
            Set(value As Double)
                Me.EPS = value
            End Set
        End Property

        ''' <summary>
        ''' Use criterion
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public Property PARAM_IsUseCriterion As Boolean
            Get
                Return Me.IsUseCriterion
            End Get
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
        Public Property PARAM_CriterionPersent As Double
            Get
                Return Me.HigherNPercent
            End Get
            Set(value As Double)
                Me.HigherNPercent = value
            End Set
        End Property

        ''' <summary>
        ''' Max iteration count
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public Property PARAM_MAX_ITERATION As Integer
            Get
                Return Me.MAX_ITERATION
            End Get
            Set(value As Integer)
                Me.MAX_ITERATION = value
            End Set
        End Property

        ''' <summary>
        ''' Range of initial value
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public Property PARAM_InitRange As Double
            Get
                Return Me.INIT_PARAM_RANGE
            End Get
            Set(value As Double)
                Me.INIT_PARAM_RANGE = value
            End Set
        End Property

        ''' <summary>
        ''' Population Size(Default:200)
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property PARAM_PopulationSize As Integer
            Get
                Return Me.POPULATION_SIZE
            End Get
            Set(value As Integer)
                Me.POPULATION_SIZE = value
            End Set
        End Property

        ''' <summary>
        ''' Children size( = Number of CrossOver/2 )
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property PARAM_ChildrenSize As Integer
            Get
                Return Me.CHILDREN_SIZE
            End Get
            Set(value As Integer)
                Me.CHILDREN_SIZE = value
            End Set
        End Property

        ''' <summary>
        ''' Alpha(Default:0.5)
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property PARAM_Alpha As Double
            Get
                Return Me.ALPHA
            End Get
            Set(value As Double)
                Me.ALPHA = value
            End Set
        End Property

        ''' <summary>
        ''' Beta(Default:0.35)
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property PARAM_Beta As Double
            Get
                Return Me.BETA
            End Get
            Set(value As Double)
                Me.BETA = value
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
                Dim p1 = Me.m_parents(p1Index)
                Dim p2 = Me.m_parents(p2Index)
                Dim p3 = Me.m_parents(randIndex(2)) 'for d2

                'UNDX
                Dim children = Me.UNDX(p1, p2, p3)

                'MGG Strategy
                children.Add(p1)
                children.Add(p2)
                Dim replace As List(Of clsPoint) = Me.SelectReplacePopulation(children)
                Me.m_parents(p1Index) = replace(0)
                Me.m_parents(p2Index) = replace(1)
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

#Region "Private"
        ''' <summary>
        ''' Calc Triangle Area using Heron formula
        ''' </summary>
        ''' <param name="lengthA"></param>
        ''' <param name="lengthB"></param>
        ''' <param name="lengthC"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CalcTriangleArea(ByVal lengthA As Double, ByVal lengthB As Double, ByVal lengthC As Double) As Double
            '    p3
            '   /| 
            '  / |
            ' ----
            ' p1  p2
            Dim s = (lengthA + lengthB + lengthC) / 2.0
            Dim area = Math.Sqrt(s * (s - lengthA) * (s - lengthB) * (s - lengthC))
            Return area
        End Function

        ''' <summary>
        ''' UNDX CrossOver
        ''' </summary>
        ''' <param name="p1"></param>
        ''' <param name="p2"></param>
        ''' <param name="p3"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function UNDX(ByVal p1 As clsPoint, ByVal p2 As clsPoint, ByVal p3 As clsPoint) As List(Of clsPoint)
            'calc d
            Dim diffVectorP2P1 = p2 - p1
            Dim length = diffVectorP2P1.NormL2()
            If length = 0 Then
                length += 0.0000000001 'avoid Zero Divide
            End If
            Dim areaTriangle As Double = Me.CalcTriangleArea(length, (p3 - p2).NormL2(), (p3 - p1).NormL2())
            Dim d2 = 2.0 * areaTriangle / length 'S=1/2 * h * a -> h = 2.0 * S / a

            'UNDX
            Dim children As New List(Of clsPoint)(Me.CHILDREN_SIZE + 1)
            Dim gVector = (p1 + p2) / 2.0
            Dim sd1 = (Me.ALPHA * length) ^ 2
            Dim sd2 = (Me.BETA * d2 / Math.Sqrt(Me.m_func.NumberOfVariable)) ^ 2
            Dim e = diffVectorP2P1 / length
            For genChild As Integer = 0 To CInt(Me.CHILDREN_SIZE / 2 - 1)
                Dim t = New clsShoddyVector(Me.m_func.NumberOfVariable)
                For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                    t(i) = clsUtil.NormRand(0, sd2)
                Next
                t = t - (t.InnerProduct(e)) * e

                'child
                Dim child1 As New clsShoddyVector(Me.m_func.NumberOfVariable)
                Dim child2 As New clsShoddyVector(Me.m_func.NumberOfVariable)
                For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                    t(i) = t(i) + clsUtil.NormRand(0, sd1) * e(i)
                Next
                child1 = gVector + t
                child2 = gVector - t

                children.Add(New clsPoint(Me.m_func, child1))
                children.Add(New clsPoint(Me.m_func, child2))
            Next

            Return children
        End Function

        ''' <summary>
        ''' Select child for MGG
        ''' </summary>
        ''' <param name="ai_children"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function SelectReplacePopulation(ByVal ai_children As List(Of clsPoint)) As List(Of clsPoint)
            ai_children.Sort()

            'best
            Dim ret As New List(Of clsPoint)
            ret.Add(ai_children(0))

            'roulette
            ai_children.RemoveAt(0)
            Dim newP2Index = Me.SelectRoulette(ai_children)
            ret.Add(ai_children(newP2Index)) 'roulete select

            Return ret
        End Function

        ''' <summary>
        ''' RouletteWheel selection for Minimize
        ''' </summary>
        ''' <param name="ai_chidren">sorted children by evaluate value</param>
        ''' <returns>index</returns>
        ''' <remarks></remarks>
        Private Function SelectRoulette(ByVal ai_chidren As List(Of clsPoint)) As Integer
            'sum
            Dim absSum As Double = 0
            For Each child In ai_chidren
                absSum += Math.Abs(child.Eval)
            Next
            Dim newSum As Double = 0
            Dim evalList As New List(Of Double)(ai_chidren.Count)
            For Each child In ai_chidren
                Dim d = absSum - child.Eval
                evalList.Add(d)
                newSum += d
            Next

            'select
            Dim r = Me.m_rand.NextDouble()
            Dim cumulativeRatio As Double = 0.0
            For i As Integer = 0 To evalList.Count - 1
                cumulativeRatio += evalList(i) / newSum
                If cumulativeRatio > r Then
                    Return i
                End If
            Next

            Return ai_chidren.Count - 1
        End Function
#End Region

    End Class
End Namespace

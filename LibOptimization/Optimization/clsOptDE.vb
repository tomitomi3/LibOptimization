﻿Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Differential Evolution Algorithm (DE/rand/1/bin, DE/rand/2/bin, DE/best/1/bin, DE/best/2/bin)
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -similar to GA algorithm.
    ''' 
    ''' Memo:
    '''  Notation of DE
    '''   DE/x/y/z
    '''    x: pick parent strategy(rand or best)
    '''    y: number of difference vector
    '''    z: crossover scheme
    '''       ex.Binomial -> bin
    ''' 
    ''' Refference:
    ''' [1]Storn, R., Price, K., "Differential Evolution – A Simple and Efficient Heuristic for Global Optimization over Continuous Spaces", Journal of Global Optimization 11: 341–359.
    ''' [2]Price, K. and Storn, R., "Minimizing the Real Functions of the ICEC’96 contest by Differential Evolution", IEEE International Conference on Evolutionary Computation (ICEC’96), may 1996, pp. 842–844.
    ''' [3]Sk. Minhazul Islam, Swagatam Das, "An Adaptive Differential Evolution Algorithm With Novel Mutation and Crossover Strategies for Global Numerical Optimization", IEEE TRANSACTIONS ON SYSTEMS, MAN, AND CYBERNETICS—PART B: CYBERNETICS, VOL. 42, NO. 2, APRIL 2012, pp482-500.
    ''' [4]田邊遼司, and 福永Alex. "自動チューナーを用いた異なる最大評価回数における Differential Evolution アルゴリズムのパラメータ設定の調査." 進化計算学会論文誌 6.2 (2015): 67-81.
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    <Serializable>
    Public Class clsOptDE : Inherits absOptimization
#Region "Member"
        '----------------------------------------------------------------
        'Common parameters
        '----------------------------------------------------------------
        ''' <summary>Max iteration count</summary>
        Public Overrides Property Iteration As Integer = 20000

        ''' <summary>epsilon(Default:1e-8) for Criterion</summary>
        Public Property EPS As Double = 0.000000001

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        ''' <summary>Upper bound(limit solution space)</summary>
        Public Property UpperBounds As Double() = Nothing

        ''' <summary>Lower bound(limit solution space)</summary>
        Public Property LowerBounds As Double() = Nothing

        '----------------------------------------------------------------
        'DE parameters
        '----------------------------------------------------------------
        ''' <summary>
        ''' Population Size(Default:100)
        ''' </summary>
        Public Property PopulationSize As Integer = 100

        ''' <summary>
        ''' Differential weight(Scaling factor)(Default:0.5)
        ''' </summary>
        Public Property F As Double = 0.5

        ''' <summary>
        ''' Cross over ratio(Default:0.9)
        ''' </summary>
        Public Property CrossOverRatio As Double = 0.9

        ''' <summary>
        ''' Differential Evolution Strategy
        ''' </summary>
        Public Property DEStrategy As EnumDEStrategyType = EnumDEStrategyType.DE_rand_1_bin

        ''' <summary>Enum Differential Evolution Strategy[3][4]</summary>
        Public Enum EnumDEStrategyType
            ''' <summary>DE/rand/1/bin - global searchability(大域検索)</summary>
            DE_rand_1_bin
            ''' <summary>DE/rand/2/bin - Strong global searchability(強い大域検索)</summary>
            DE_rand_2_bin
            ''' <summary>DE/best/1/bin - local searchability(局所検索)</summary>
            DE_best_1_bin
            ''' <summary>DE/best/2/bin - Strong local searchability(強い局所検索)</summary>
            DE_best_2_bin
            ''' <summary>DE/currentToRand/1/bin</summary>
            DE_current_to_rand_1_bin
            ''' <summary>DE/currentToBest/1/bin - local searchability(局所検索)</summary>
            DE_current_to_Best_1_bin
            ''' <summary>DE/currentToBest/2/bin - local searchability(局所検索)</summary>
            DE_current_to_Best_2_bin
        End Enum

        ''' <summary>population</summary>
        Private m_parents As New List(Of clsPoint)
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

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Objective Function</param>
        ''' <param name="ai_destrategy">DE Strategt(e.g. DE/rand/1/bin)</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction, ByVal ai_destrategy As EnumDEStrategyType)
            Me.m_func = ai_func
            Me.DEStrategy = ai_destrategy
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
                    Dim array = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)

                    'bound check
                    Dim tempPoint = New clsPoint(MyBase.m_func, array)
                    If UpperBounds IsNot Nothing AndAlso LowerBounds IsNot Nothing Then
                        clsUtil.LimitSolutionSpace(tempPoint, Me.LowerBounds, Me.UpperBounds)
                    End If

                    'save point
                    Me.m_parents.Add(tempPoint)
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
            If Me.Iteration <= m_iteration Then
                Return True
            Else
                ai_iteration = If(ai_iteration = 0, Iteration - m_iteration - 1, Math.Min(ai_iteration, Iteration - m_iteration) - 1)
            End If
            Dim blankChild = New clsPoint(Me.m_func)
            For iterate As Integer = 0 To ai_iteration
                'Counting generation
                m_iteration += 1

                'Sort Evaluate
                'Me.m_parents.Sort()
                Dim evalIndexs = clsUtil.GetIndexSortedEvalFromPoints(m_parents)

                'check criterion - higher N percentage particles are finished at the time of same evaluate value.
                If Me.IsUseCriterion = True Then
                    If clsUtil.IsCriterion(Me.EPS, m_parents(evalIndexs(0).Index).Eval, m_parents(evalIndexs(HigherNPercentIndex).Index).Eval) Then
                        Return True
                    End If
                End If

                'reserve best value
                Dim best = m_parents(evalIndexs(0).Index).Copy()

                'DE
                For i As Integer = 0 To Me.PopulationSize - 1
                    'pick different parent without i
                    Dim randIndex As List(Of Integer) = clsUtil.RandomPermutaion(Me.m_parents.Count, i)
                    Dim xi = Me.m_parents(i)
                    Dim p1 As clsPoint = Me.m_parents(randIndex(0))
                    Dim p2 As clsPoint = Me.m_parents(randIndex(1))
                    Dim p3 As clsPoint = Me.m_parents(randIndex(2))
                    Dim p4 As clsPoint = Me.m_parents(randIndex(3))
                    Dim p5 As clsPoint = Me.m_parents(randIndex(4))

                    'Mutation and Crossover
                    Dim child = blankChild.Copy()
                    Dim j = Me.m_rand.Next() Mod Me.m_func.NumberOfVariable
                    Dim D = Me.m_func.NumberOfVariable - 1

                    Select Case Me.DEStrategy
                        Case EnumDEStrategyType.DE_best_1_bin
                            'DE/best/1/bin
                            For k = 0 To Me.m_func.NumberOfVariable - 1
                                If Me.m_rand.NextDouble() < Me.CrossOverRatio OrElse k = D Then
                                    child(j) = best(j) + Me.F * (p1(j) - p2(j))
                                Else
                                    child(j) = xi(k)
                                End If
                                j = (j + 1) Mod Me.m_func.NumberOfVariable 'next
                            Next
                        Case EnumDEStrategyType.DE_best_2_bin
                            'DE/best/2/bin
                            For k = 0 To Me.m_func.NumberOfVariable - 1
                                If Me.m_rand.NextDouble() < Me.CrossOverRatio OrElse k = D Then
                                    child(j) = best(j) + Me.F * (p1(j) + p2(j) - p3(j) - p4(j))
                                Else
                                    child(j) = xi(k)
                                End If
                                j = (j + 1) Mod Me.m_func.NumberOfVariable 'next
                            Next
                        Case EnumDEStrategyType.DE_current_to_Best_1_bin
                            'DE/current-to-best/1/bin
                            For k = 0 To Me.m_func.NumberOfVariable - 1
                                If Me.m_rand.NextDouble() < Me.CrossOverRatio OrElse k = D Then
                                    child(j) = xi(j) + Me.F * (best(j) - p1(j)) + Me.F * (p2(j) - p3(j))
                                Else
                                    child(j) = xi(k)
                                End If
                                j = (j + 1) Mod Me.m_func.NumberOfVariable 'next
                            Next
                        Case EnumDEStrategyType.DE_current_to_Best_2_bin
                            'DE/current-to-best/2/bin
                            For k = 0 To Me.m_func.NumberOfVariable - 1
                                If Me.m_rand.NextDouble() < Me.CrossOverRatio OrElse k = D Then
                                    child(j) = xi(j) + Me.F * (best(j) - p1(j)) + Me.F * (p2(j) - p3(j)) + Me.F * (p4(j) - p5(j))
                                Else
                                    child(j) = xi(k)
                                End If
                                j = (j + 1) Mod Me.m_func.NumberOfVariable 'next
                            Next
                        Case EnumDEStrategyType.DE_current_to_rand_1_bin
                            'DE/current-to-rand/1/bin
                            For k = 0 To Me.m_func.NumberOfVariable - 1
                                If Me.m_rand.NextDouble() < Me.CrossOverRatio OrElse k = D Then
                                    child(j) = xi(j) + Me.F * (p2(j) - p3(j))
                                Else
                                    child(j) = xi(k)
                                End If
                                j = (j + 1) Mod Me.m_func.NumberOfVariable 'next
                            Next
                        Case EnumDEStrategyType.DE_rand_1_bin
                            'DE/rand/1/bin
                            For k = 0 To Me.m_func.NumberOfVariable - 1
                                If Me.m_rand.NextDouble() < Me.CrossOverRatio OrElse k = D Then
                                    child(j) = p1(j) + Me.F * (p2(j) - p3(j))
                                Else
                                    child(j) = xi(k)
                                End If
                                j = (j + 1) Mod Me.m_func.NumberOfVariable 'next
                            Next
                        Case EnumDEStrategyType.DE_rand_2_bin
                            'DE/rand/2/bin
                            For k = 0 To Me.m_func.NumberOfVariable - 1
                                If Me.m_rand.NextDouble() < Me.CrossOverRatio OrElse k = D Then
                                    child(j) = p1(j) + Me.F * (p2(j) + p3(j) - p4(j) - p5(j))
                                Else
                                    child(j) = xi(k)
                                End If
                                j = (j + 1) Mod Me.m_func.NumberOfVariable 'next
                            Next
                    End Select
                    child.ReEvaluate() 'Evaluate child

                    'Limit solution space
                    clsUtil.LimitSolutionSpace(child, Me.LowerBounds, Me.UpperBounds)

                    'Survive
                    If child.Eval < Me.m_parents(i).Eval Then
                        Me.m_parents(i) = child
                    End If

                    'Current best
                    If child.Eval < best.Eval Then
                        best = child
                    End If
                Next
            Next

            Return False
        End Function

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As clsPoint
            Get
                Return clsUtil.FindCurrentBestPointFromPoints(Me.m_parents, True)
            End Get
        End Property

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

#Region "Private"

#End Region
    End Class
End Namespace

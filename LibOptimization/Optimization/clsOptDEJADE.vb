Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Adaptive Differential Evolution Algorithm
    ''' JADE
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -similar to GA algorithm.
    ''' 
    ''' Refference:
    '''  [1]Z.-H. Zhan, J. Zhang, Y. Li, and H. Chung, “JADE: Adaptive Differential Evolution With Optional External Archive” IEEE Trans. Systems, Man, and Cybernetics-Part B, vol. 39, no. 6, pp. 1362-1381, Dec. 2009. 
    '''  [2]阪井 節子,高濱 徹行, "パラメータの相関を考慮した適応型差分進化アルゴリズムJADEの改良", 不確実性の下での数理モデルとその周辺 Mathematical Model under Uncertainty and Related Topics RIMS 研究集会報告集
    '''     (JADEの原著論文が見れなかったので[2]文献を参照)
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptDEJADE : Inherits absOptimization
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
        'DE parameters
        '----------------------------------------------------------------
        ''' <summary>
        ''' Population Size(Default:100)
        ''' </summary>
        Public Property PopulationSize As Integer = 100

        ''' <summary>
        ''' Scale factor(Default:0.5)
        ''' </summary>
        Public Property F As Double = 0.5

        ''' <summary>
        ''' Cross over ratio(Default:0.5)
        ''' </summary>
        Public Property CrossOverRatio As Double = 0.5


        ''' <summary>
        ''' Differential Evolution Strategy
        ''' </summary>
        Public Property DEStrategy As EnumDEStrategyType = EnumDEStrategyType.DE_current_to_pBest_1

        ''' <summary>
        ''' Enum Differential Evolution Strategy
        ''' </summary>
        Public Enum EnumDEStrategyType
            ''' <summary>DE/current-to-pbest/1</summary>
            DE_current_to_pBest_1
        End Enum

        ''' <summary>population</summary>
        Private m_parents As New List(Of clsPoint)

        ''' <summary>individual cross over ratio for population</summary>
        Private CRs As New List(Of Double)

        ''' <summary>individual F for population</summary>
        Private Fs As New List(Of Double)
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
                Me.m_parents.Clear()
                Me.m_error.Clear()
                Me.CRs.Clear()
                Me.Fs.Clear()

                'generate population
                For i As Integer = 0 To Me.PopulationSize - 1
                    Me.CRs.Add(0.5)
                    Me.Fs.Add(0.5)

                    'initialize
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

                'update F and CR
                Me.UpdateFAndCR()

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

                'check criterion
                If Me.IsUseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If clsUtil.IsCriterion(Me.EPS, Me.m_parents(0).Eval, Me.m_parents(Me.HigherNPercentIndex).Eval) Then
                        Return True
                    End If
                End If

                'Counting generation
                If Me.Iteration <= Me.m_iteration Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                m_iteration += 1

                '--------------------------------------------------------------------------------------------
                'DE process
                '--------------------------------------------------------------------------------------------
                'update F and CR
                Dim sumF As Double = 0.0
                Dim sumFSquare As Double = 0.0
                Dim sumCR As Double = 0.0
                Dim countSuccess As Integer = 0
                Me.UpdateFAndCR()

                For i As Integer = 0 To Me.PopulationSize - 1
                    'pick different parent without i
                    Dim randIndex As List(Of Integer) = clsUtil.RandomPermutaion(Me.m_parents.Count, i)
                    Dim xi = Me.m_parents(i)
                    Dim p1 As clsPoint = Me.m_parents(randIndex(0))
                    Dim p2 As clsPoint = Me.m_parents(randIndex(1))

                    'Mutation and Crossover
                    Dim child = New clsPoint(Me.m_func)
                    Dim jj = Me.m_rand.Next() Mod Me.m_func.NumberOfVariable
                    Dim D = Me.m_func.NumberOfVariable - 1
                    If Me.DEStrategy = EnumDEStrategyType.DE_current_to_pBest_1 Then
                        'DE/current-to-pbest/1 for JADE Strategy
                        Dim p = CInt(Me.PopulationSize - 1 * Me.m_rand.NextDouble()) '100p%
                        Dim index = Me.m_rand.Next(0, p)
                        Dim pbest = Me.m_parents(index)

                        'crossover
                        For k = 0 To Me.m_func.NumberOfVariable - 1
                            If Me.m_rand.NextDouble() < Me.CRs(i) OrElse k = D Then
                                child(jj) = xi(jj) + Me.Fs(i) * (pbest(jj) - xi(jj)) + Me.Fs(i) * (p1(jj) - p2(jj))
                            Else
                                child(jj) = xi(k)
                            End If
                            jj = (jj + 1) Mod Me.m_func.NumberOfVariable 'next
                        Next
                    End If
                    child.ReEvaluate() 'Evaluate child

                    'Limit solution space
                    clsUtil.LimitSolutionSpace(child, Me.LowerBounds, Me.UpperBounds)

                    'Survive
                    If child.Eval < Me.m_parents(i).Eval Then
                        Me.m_parents(i) = child

                        'for adaptive parameter
                        sumF += Me.Fs(i)
                        sumFSquare += Me.Fs(i) ^ 2
                        sumCR += Me.CRs(i)
                        countSuccess += 1
                    End If
                Next 'population iteration

                'update muF, muCR
                If countSuccess > 0 Then
                    Me.F = (1 - 0.1) * Me.F + 0.1 * (sumFSquare / sumF)
                    Me.CrossOverRatio = (1 - 0.1) * Me.CrossOverRatio + 0.1 * (sumCR / countSuccess)
                Else
                    Me.F = (1 - 0.1) * Me.F
                    Me.CrossOverRatio = (1 - 0.1) * Me.CrossOverRatio
                    'Console.WriteLine("not success")
                    'Console.WriteLine(" F={0} CR={1}", Me.F, Me.CrossOverRatio)
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

#Region "Private"

        ''' <summary>
        ''' Update scalafactor and crossover ratio
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub UpdateFAndCR()
            For i As Integer = 0 To Me.PopulationSize - 1
                'generate F
                While (True)
                    Dim tempF As Double = clsUtil.CauchyRand(Me.F, 0.1)
                    If tempF < 0 Then
                        Continue While
                    ElseIf tempF > 1 Then
                        Me.Fs(i) = 1
                    Else
                        Me.Fs(i) = tempF
                    End If
                    Exit While
                End While

                'generate CR 0 to 1
                Dim tempCR As Double = clsUtil.NormRand(Me.CrossOverRatio, 0.1)
                If tempCR < 0 Then
                    Me.CRs(i) = 0
                ElseIf tempCR > 1 Then
                    Me.CRs(i) = 1
                Else
                    Me.CRs(i) = tempCR
                End If
            Next
        End Sub
#End Region
    End Class
End Namespace
